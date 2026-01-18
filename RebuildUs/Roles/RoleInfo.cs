using RebuildUs.Roles.Modifier;

namespace RebuildUs.Roles;

public partial class RoleInfo(string nameKey, Color color, CustomOption baseOption, RoleType roleType)
{
    public Color Color = color;
    public virtual string Name { get { return NameKey; } }
    // public virtual string Name { get { return Tr.Get(NameKey); } }
    public virtual string NameColored { get { return Helpers.Cs(Color, Name); } }
    public virtual string IntroDescription { get { return "IntroDesc" + NameKey; } }
    // public virtual string IntroDescription { get { return Tr.Get(("IntroDesc", NameKey)); } }
    public virtual string ShortDescription { get { return "ShortDesc" + NameKey; } }
    // public virtual string ShortDescription { get { return Tr.Get(("ShortDesc", NameKey)); } }
    public virtual string FullDescription { get { return "FullDesc" + NameKey; } }
    // public virtual string FullDescription { get { return Tr.Get(("FullDesc", NameKey)); } }
    public virtual string RoleOptions { get { return CustomOption.optionsToString(BaseOption); } }
    public bool Enabled { get { return Helpers.RolesEnabled && (BaseOption == null /*|| BaseOption.Enabled*/); } }

    public string NameKey = nameKey;
    public RoleType RoleType = roleType;
    private readonly CustomOption BaseOption = baseOption;
    public static List<RoleInfo> AllRoleInfos = [];

    public static List<RoleInfo> GetRoleInfoForPlayer(PlayerControl player, bool showModifier = true, RoleType[] excludeRoles = null)
    {
        var infos = new List<RoleInfo>();
        if (player == null) return infos;

        if (showModifier)
        {
            // TODO: Write modifiers here
        }

        foreach (var info in AllRoleInfos)
        {
            if (info.RoleType == RoleType.Jackal) continue; // Jackalは後で特殊判定を行う
            if (player.IsRole(info.RoleType)) infos.Add(info);
        }

        if (player.IsRole(RoleType.Jackal) || (Neutral.Jackal.FormerJackals != null && Neutral.Jackal.FormerJackals.Any(x => x.PlayerId == player.PlayerId))) infos.Add(Jackal);

        if (infos.Count == 0 && player.Data.Role != null)
        {
            if (player.IsTeamImpostor()) infos.Add(Impostor);
            else infos.Add(Crewmate);
        }

        if (excludeRoles != null)
        {
            infos.RemoveAll(x => excludeRoles.Contains(x.RoleType));
        }

        return infos;
    }

    public static string GetRolesString(PlayerControl p, bool useColors, bool showModifier = true, RoleType[] excludeRoles = null, bool includeHidden = false, string joinSeparator = " ")
    {
        if (p?.Data?.Disconnected != false) return "";

        var roleInfo = GetRoleInfoForPlayer(p, showModifier, excludeRoles);
        var roleName = string.Join(joinSeparator, [.. roleInfo.Select(x => useColors ? Helpers.Cs(x.Color, x.Name) : x.Name)]);

        if (p.HasModifier(ModifierType.Madmate) || p.HasModifier(ModifierType.CreatedMadmate))
        {
            // Madmate only
            if (roleInfo.Contains(Crewmate))
            {
                roleName = useColors ? Helpers.Cs(Madmate.NameColor, Madmate.fullName) : Madmate.fullName;
            }
            else
            {
                string prefix = useColors ? Helpers.Cs(Madmate.NameColor, Madmate.prefix) : Madmate.prefix;
                roleName = string.Join(joinSeparator, [.. roleInfo.Select(x => useColors ? Helpers.Cs(Madmate.NameColor, x.Name) : x.Name)]);
                roleName = prefix + roleName;
            }
        }

        if (p.HasModifier(ModifierType.LastImpostor))
        {
            if (roleInfo.Contains(Impostor))
            {
                roleName = useColors ? Helpers.Cs(LastImpostor.NameColor, LastImpostor.fullName) : LastImpostor.fullName;
            }
            else
            {
                string postfix = useColors ? Helpers.Cs(LastImpostor.NameColor, LastImpostor.postfix) : LastImpostor.postfix;
                roleName = string.Join(joinSeparator, [.. roleInfo.Select(x => useColors ? Helpers.Cs(x.Color, x.Name) : x.Name)]);
                roleName += postfix;
            }
        }

        if (p.HasModifier(ModifierType.Munou))
        {
            /* TODO: Munou is not implemented
            if (CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead || Munou.endGameFlag)
            {
                string postfix = useColors ? Helpers.Cs(Munou.color, Munou.postfix) : Munou.postfix;
                roleName += postfix;
            }
            */
        }

        if (p.HasModifier(ModifierType.AntiTeleport))
        {
            string postfix = useColors ? Helpers.Cs(AntiTeleport.NameColor, AntiTeleport.postfix) : AntiTeleport.postfix;
            roleName += postfix;
        }

        return roleName;
    }

    private static readonly Dictionary<RoleType, RoleInfo> RoleDict = [];

    public static RoleInfo Get(RoleType type) => RoleDict.GetValueOrDefault(type);

    public static RoleInfo Jackal => Get(RoleType.Jackal);
    public static RoleInfo Crewmate => Get(RoleType.Crewmate);
    public static RoleInfo Impostor => Get(RoleType.Impostor);

    public static void Load()
    {
        RoleDict.Clear();
        AllRoleInfos.Clear();

        foreach (var reg in RoleData.Roles)
        {
            var info = new RoleInfo(Enum.GetName(reg.roleType), reg.getColor(), reg.getOption(), reg.roleType);
            RoleDict[reg.roleType] = info;
            AllRoleInfos.Add(info);
        }

        RoleDict[RoleType.Crewmate] = new(Enum.GetName(RoleType.Crewmate), Palette.CrewmateBlue, null, RoleType.Crewmate);
        RoleDict[RoleType.Impostor] = new(Enum.GetName(RoleType.Impostor), Palette.ImpostorRed, null, RoleType.Impostor);
    }
}
