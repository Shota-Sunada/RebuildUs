namespace RebuildUs.Roles;

internal class RoleInfo(TrKey nameKey, Color color, CustomOption baseOption, RoleType roleType)
{
    internal static List<RoleInfo> AllRoleInfos = [];

    private static readonly Dictionary<RoleType, RoleInfo> RoleDict = [];
    private readonly CustomOption _baseOption = baseOption;
    internal Color Color = color;

    internal TrKey NameKey = nameKey;
    internal RoleType RoleType = roleType;

    internal virtual string Name
    {
        get => Tr.Get(NameKey);
    }

    internal virtual string NameColored
    {
        get => Helpers.Cs(Color, Name);
    }

    internal virtual string IntroDescription
    {
        get => Tr.GetDynamic($"{NameKey}IntroDesc");
    }

    internal virtual string ShortDescription
    {
        get => Tr.GetDynamic($"{NameKey}ShortDesc");
    }

    internal virtual string FullDescription
    {
        get => Tr.GetDynamic($"{NameKey}FullDesc");
    }

    internal virtual string RoleOptions
    {
        get => CustomOption.OptionsToString(_baseOption);
    }

    internal bool Enabled
    {
        get => Helpers.RolesEnabled && (_baseOption == null || _baseOption.Enabled);
    }

    internal static RoleInfo Jackal
    {
        get => Get(RoleType.Jackal);
    }

    internal static RoleInfo Crewmate
    {
        get => Get(RoleType.Crewmate);
    }

    internal static RoleInfo Impostor
    {
        get => Get(RoleType.Impostor);
    }

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

        List<RoleInfo> allRoleInfos = AllRoleInfos;
        for (int i = 0; i < allRoleInfos.Count; i++)
        {
            RoleInfo info = allRoleInfos[i];
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
            bool isJackalOrFormer = player.IsRole(RoleType.Jackal);
            if (!isJackalOrFormer)
            {
                List<PlayerControl> formerJackals = Neutral.Jackal.FormerJackals;
                for (int i = 0; i < formerJackals.Count; i++)
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
            for (int i = infos.Count - 1; i >= 0; i--)
            {
                RoleType roleType = infos[i].RoleType;
                bool shouldRemove = false;
                for (int j = 0; j < excludeRoles.Length; j++)
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

    internal static string GetRolesString(PlayerControl p,
                                          bool useColors,
                                          bool showModifier = true,
                                          RoleType[] excludeRoles = null,
                                          bool includeHidden = false,
                                          string joinSeparator = " ")
    {
        if (p == null || p.Data == null || p.Data.Disconnected)
        {
            return string.Empty;
        }

        List<RoleInfo> roleInfo = GetRoleInfoForPlayer(p, showModifier, excludeRoles);
        if (roleInfo.Count == 0)
        {
            return string.Empty;
        }

        StringBuilder sb = new();

        void AppendNames(bool useMadmateColor = false)
        {
            for (int i = 0; i < roleInfo.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append(joinSeparator);
                }
                RoleInfo info = roleInfo[i];
                Color c = useMadmateColor ? Madmate.NameColor : info.Color;
                sb.Append(useColors ? Helpers.Cs(c, info.Name) : info.Name);
            }
        }

        if (p.HasModifier(ModifierType.Madmate) || p.HasModifier(ModifierType.CreatedMadmate))
        {
            bool hasCrewmate = false;
            for (int i = 0; i < roleInfo.Count; i++)
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

            string prefix = useColors ? Helpers.Cs(Madmate.NameColor, Madmate.Prefix) : Madmate.Prefix;
            sb.Append(prefix);
            AppendNames(true);
            return sb.ToString();
        }

        if (p.HasModifier(ModifierType.LastImpostor))
        {
            bool hasImpostor = false;
            for (int i = 0; i < roleInfo.Count; i++)
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

            string postfix = useColors ? Helpers.Cs(LastImpostor.NameColor, LastImpostor.Postfix) : LastImpostor.Postfix;
            AppendNames();
            sb.Append(postfix);
            return sb.ToString();
        }

        if (p.HasModifier(ModifierType.AntiTeleport))
        {
            string postfix = useColors ? Helpers.Cs(AntiTeleport.NameColor, AntiTeleport.Postfix) : AntiTeleport.Postfix;
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

        foreach (RoleData.RoleRegistration reg in RoleData.Roles)
        {
            RoleInfo info = new(Enum.TryParse(Enum.GetName(reg.RoleType), out TrKey key) ? key : TrKey.None,
                reg.GetColor(),
                reg.GetOption(),
                reg.RoleType);
            RoleDict[reg.RoleType] = info;
            AllRoleInfos.Add(info);
        }

        RoleDict[RoleType.Crewmate] = new(TrKey.Crewmate, Palette.CrewmateBlue, null, RoleType.Crewmate);
        RoleDict[RoleType.Impostor] = new(TrKey.Impostor, Palette.ImpostorRed, null, RoleType.Impostor);
    }
}