namespace RebuildUs.Roles;

public partial class RoleInfo(string nameKey, Color color, CustomOption baseOption, ERoleType roleType)
{
    public Color Color = color;
    public virtual string Name { get { return Tr.Get(NameKey); } }
    public virtual string NameColored { get { return Helpers.Cs(Color, Name); } }
    public virtual string IntroDescription { get { return Tr.Get(("IntroDesc", NameKey)); } }
    public virtual string ShortDescription { get { return Tr.Get(("ShortDesc", NameKey)); } }
    public virtual string FullDescription { get { return Tr.Get(("FullDesc", NameKey)); } }
    // public virtual string RoleOptions { get { return CustomOption.}}
    public bool Enabled { get { return Helpers.RolesEnabled && (BaseOption == null /*|| BaseOption.Enabled*/); } }

    public string NameKey = nameKey;
    public ERoleType RoleType = roleType;
    private readonly CustomOption BaseOption = baseOption;
    public static List<RoleInfo> AllRoleInfos = [];

    public static List<RoleInfo> GetRoleInfoForPlayer(PlayerControl player, bool showModifier = true, ERoleType[] excludeRoles = null)
    {
        var infos = new List<RoleInfo>();
        if (player == null) return infos;

        if (showModifier)
        {
            // TODO: Write modifiers here
        }

        foreach (var info in AllRoleInfos)
        {
            if (info.RoleType == ERoleType.Jackal) continue; // Jackalは後で特殊判定を行う
            if (player.IsRole(info.RoleType)) infos.Add(info);
        }

        if (player.IsRole(ERoleType.Jackal) || (Neutral.Jackal.FormerJackals != null && Neutral.Jackal.FormerJackals.Any(x => x.PlayerId == player.PlayerId))) infos.Add(Jackal);

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

    public static string GetRolesString(PlayerControl p, bool useColors, bool showModifier = true, ERoleType[] excludeRoles = null, bool includeHidden = false, string joinSeparator = " ")
    {
        if (p?.Data?.Disconnected != false) return "";

        var roleInfo = GetRoleInfoForPlayer(p, showModifier, excludeRoles);
        var roleName = string.Join(joinSeparator, [.. roleInfo.Select(x => useColors ? Helpers.Cs(x.Color, x.Name) : x.Name)]);
        // if (Lawyer.target != null && p?.PlayerId == Lawyer.target.PlayerId && CachedPlayer.LocalPlayer.PlayerControl != Lawyer.target) roleName += useColors ? Helpers.cs(Pursuer.color, " §") : " §";

        // if (p.HasModifier(EModifierType.Madmate) || p.HasModifier(EModifierType.CreatedMadmate))
        // {
        //     // Madmate only
        //     if (roleInfo.Contains(crewmate))
        //     {
        //         roleName = useColors ? Helpers.cs(Madmate.color, Madmate.fullName) : Madmate.fullName;
        //     }
        //     else
        //     {
        //         string prefix = useColors ? Helpers.cs(Madmate.color, Madmate.prefix) : Madmate.prefix;
        //         roleName = String.Join(joinSeparator, roleInfo.Select(x => useColors ? Helpers.cs(Madmate.color, x.name) : x.name).ToArray());
        //         roleName = prefix + roleName;
        //     }
        // }

        // if (p.hasModifier(ModifierType.LastImpostor))
        // {
        //     if (roleInfo.Contains(impostor))
        //     {
        //         roleName = useColors ? Helpers.cs(LastImpostor.color, LastImpostor.fullName) : LastImpostor.fullName;
        //     }
        //     else
        //     {
        //         string postfix = useColors ? Helpers.cs(LastImpostor.color, LastImpostor.postfix) : LastImpostor.postfix;
        //         roleName = String.Join(joinSeparator, roleInfo.Select(x => useColors ? Helpers.cs(x.color, x.name) : x.name).ToArray());
        //         roleName += postfix;
        //     }
        // }

        // if (p.hasModifier(ModifierType.Munou))
        // {
        //     if (CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead || Munou.endGameFlag)
        //     {
        //         string postfix = useColors ? Helpers.cs(Munou.color, Munou.postfix) : Munou.postfix;
        //         // roleName = String.Join(joinSeparator, roleInfo.Select(x => useColors? Helpers.cs(x.color, x.name)  : x.name).ToArray());
        //         roleName += postfix;
        //     }
        // }

        // if (p.hasModifier(ModifierType.AntiTeleport))
        // {
        //     string postfix = useColors ? Helpers.cs(AntiTeleport.color, AntiTeleport.postfix) : AntiTeleport.postfix;
        //     // roleName = String.Join(joinSeparator, roleInfo.Select(x => useColors? Helpers.cs(x.color, x.name)  : x.name).ToArray());
        //     roleName += postfix;
        // }

        return roleName;
    }
}