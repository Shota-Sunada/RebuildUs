namespace RebuildUs.Roles;

internal class RoleInfo(TrKey nameKey, Color color, CustomOption baseOption, RoleType roleType)
{
    internal static List<RoleInfo> AllRoleInfos = [];

    private static readonly Dictionary<RoleType, RoleInfo> RoleDict = [];
    private readonly CustomOption _baseOption = baseOption;
    internal Color Color = color;

    internal TrKey NameKey = nameKey;
    internal RoleType RoleType = roleType;

    internal virtual string Name { get => Tr.Get(NameKey); }
    internal virtual string NameColored { get => Helpers.Cs(Color, Name); }
    internal virtual string IntroDescription { get => Tr.GetDynamic($"{NameKey}IntroDesc"); }
    internal virtual string ShortDescription { get => Tr.GetDynamic($"{NameKey}ShortDesc"); }
    internal virtual string FullDescription { get => Tr.GetDynamic($"{NameKey}FullDesc"); }
    internal virtual string RoleOptions { get => CustomOption.OptionsToString(_baseOption); }

    internal bool Enabled { get => Helpers.RolesEnabled && (_baseOption == null || _baseOption.Enabled); }

    internal static RoleInfo Jackal { get => Get(RoleType.Jackal); }
    internal static RoleInfo Crewmate { get => Get(RoleType.Crewmate); }
    internal static RoleInfo Impostor { get => Get(RoleType.Impostor); }

    internal static List<RoleInfo> GetRoleInfoForPlayer(PlayerControl player, bool showModifier = true, RoleType[] excludeRoles = null)
    {
        List<RoleInfo> infos = [];
        if (player == null)
        {
            return infos;
        }

        if (showModifier)
        {
            // TODO: Write modifiers here
        }

        var allRoleInfos = AllRoleInfos;
        for (var i = 0; i < allRoleInfos.Count; i++)
        {
            var info = allRoleInfos[i];
            if (info.RoleType == RoleType.Jackal)
            {
                continue;
            }
            if (player.IsRole(info.RoleType))
            {
                infos.Add(info);
            }
        }

        if (player.IsRole(RoleType.Jackal) || Neutral.Jackal.FormerJackals != null)
        {
            var isJackalOrFormer = player.IsRole(RoleType.Jackal);
            if (!isJackalOrFormer)
            {
                var formerJackals = Neutral.Jackal.FormerJackals;
                for (var i = 0; i < formerJackals.Count; i++)
                {
                    if (formerJackals[i].PlayerId == player.PlayerId)
                    {
                        isJackalOrFormer = true;
                        break;
                    }
                }
            }

            if (isJackalOrFormer)
            {
                infos.Add(Jackal);
            }
        }

        if (infos.Count == 0 && player.Data != null && player.Data.Role != null)
        {
            if (player.IsTeamImpostor())
            {
                infos.Add(Impostor);
            }
            else
            {
                infos.Add(Crewmate);
            }
        }

        if (excludeRoles != null && excludeRoles.Length > 0)
        {
            for (var i = infos.Count - 1; i >= 0; i--)
            {
                var roleType = infos[i].RoleType;
                var shouldRemove = false;
                for (var j = 0; j < excludeRoles.Length; j++)
                {
                    if (roleType == excludeRoles[j])
                    {
                        shouldRemove = true;
                        break;
                    }
                }

                if (shouldRemove)
                {
                    infos.RemoveAt(i);
                }
            }
        }

        return infos;
    }

    internal static string GetRolesString(PlayerControl p, bool useColors, bool showModifier = true, RoleType[] excludeRoles = null, bool includeHidden = false, string joinSeparator = " ")
    {
        if (p == null || p.Data == null || p.Data.Disconnected)
        {
            return string.Empty;
        }

        var roleInfo = GetRoleInfoForPlayer(p, showModifier, excludeRoles);
        if (roleInfo.Count == 0)
        {
            return string.Empty;
        }

        StringBuilder sb = new();

        void AppendNames(bool useMadmateColor = false)
        {
            for (var i = 0; i < roleInfo.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append(joinSeparator);
                }
                var info = roleInfo[i];
                var c = useMadmateColor ? Madmate.NameColor : info.Color;
                sb.Append(useColors ? Helpers.Cs(c, info.Name) : info.Name);
            }
        }

        if (p.HasModifier(ModifierType.Madmate) || p.HasModifier(ModifierType.CreatedMadmate))
        {
            var hasCrewmate = false;
            for (var i = 0; i < roleInfo.Count; i++)
            {
                if (roleInfo[i].RoleType == RoleType.Crewmate)
                {
                    hasCrewmate = true;
                    break;
                }
            }

            if (hasCrewmate)
            {
                return useColors ? Helpers.Cs(Madmate.NameColor, Madmate.FullName) : Madmate.FullName;
            }

            var prefix = useColors ? Helpers.Cs(Madmate.NameColor, Madmate.Prefix) : Madmate.Prefix;
            sb.Append(prefix);
            AppendNames(true);
            return sb.ToString();
        }

        if (p.HasModifier(ModifierType.LastImpostor))
        {
            var hasImpostor = false;
            for (var i = 0; i < roleInfo.Count; i++)
            {
                if (roleInfo[i].RoleType == RoleType.Impostor)
                {
                    hasImpostor = true;
                    break;
                }
            }

            if (hasImpostor)
            {
                return useColors ? Helpers.Cs(LastImpostor.NameColor, LastImpostor.FullName) : LastImpostor.FullName;
            }

            var postfix = useColors ? Helpers.Cs(LastImpostor.NameColor, LastImpostor.Postfix) : LastImpostor.Postfix;
            AppendNames();
            sb.Append(postfix);
            return sb.ToString();
        }

        if (p.HasModifier(ModifierType.AntiTeleport))
        {
            var postfix = useColors ? Helpers.Cs(AntiTeleport.NameColor, AntiTeleport.Postfix) : AntiTeleport.Postfix;
            AppendNames();
            sb.Append(postfix);
            return sb.ToString();
        }

        AppendNames();
        return sb.ToString();
    }

    internal static RoleInfo Get(RoleType type)
    {
        return RoleDict.GetValueOrDefault(type);
    }

    internal static void Load()
    {
        RoleDict.Clear();
        AllRoleInfos.Clear();

        var registration = RoleData.Roles;
        if (registration == null)
        {
            Logger.LogError("RoleData.Roles is null!", "RoleInfo");
            return;
        }

        foreach (var reg in registration)
        {
            try
            {
                var color = reg.GetColor != null ? reg.GetColor() : Color.white;
                var option = reg.GetOption != null ? reg.GetOption() : null;
                var name = Enum.GetName(reg.RoleType);
                RoleInfo info = new(Enum.TryParse(name, out TrKey key) ? key : TrKey.None, color, option, reg.RoleType);
                RoleDict[reg.RoleType] = info;
                AllRoleInfos.Add(info);
            }
            catch (Exception e)
            {
                Logger.LogError($"Error loading role info for {reg.RoleType}: {e}", "RoleInfo");
            }
        }

        RoleDict[RoleType.Crewmate] = new(TrKey.Crewmate, Palette.CrewmateBlue, null, RoleType.Crewmate);
        RoleDict[RoleType.Impostor] = new(TrKey.Impostor, Palette.ImpostorRed, null, RoleType.Impostor);
    }
}