using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RebuildUs.Roles;

public partial class RoleInfo(string nameKey, Color color, CustomOption baseOption, RoleType roleType)
{
    public Color Color = color;
    public virtual string Name { get { return Tr.Get($"Role.{NameKey}"); } }
    public virtual string NameColored { get { return Helpers.Cs(Color, Name); } }
    public virtual string IntroDescription { get { return Tr.Get($"IntroDesc.{NameKey}"); } }
    public virtual string ShortDescription { get { return Tr.Get($"ShortDesc.{NameKey}"); } }
    public virtual string FullDescription { get { return Tr.Get($"FullDesc.{NameKey}"); } }
    public virtual string RoleOptions { get { return CustomOption.OptionsToString(BaseOption); } }
    public bool Enabled { get { return Helpers.RolesEnabled && (BaseOption == null || BaseOption.Enabled); } }

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

        var allRoleInfos = AllRoleInfos;
        for (int i = 0; i < allRoleInfos.Count; i++)
        {
            var info = allRoleInfos[i];
            if (info.RoleType == RoleType.Jackal) continue;
            if (player.IsRole(info.RoleType)) infos.Add(info);
        }

        if (player.IsRole(RoleType.Jackal) || (Neutral.Jackal.FormerJackals != null && Neutral.Jackal.FormerJackals.Any(x => x.PlayerId == player.PlayerId)))
        {
            infos.Add(Jackal);
        }

        if (infos.Count == 0 && player.Data != null && player.Data.Role != null)
        {
            if (player.IsTeamImpostor()) infos.Add(Impostor);
            else infos.Add(Crewmate);
        }

        if (excludeRoles != null && excludeRoles.Length > 0)
        {
            for (int i = infos.Count - 1; i >= 0; i--)
            {
                var roleType = infos[i].RoleType;
                bool shouldRemove = false;
                for (int j = 0; j < excludeRoles.Length; j++)
                {
                    if (roleType == excludeRoles[j])
                    {
                        shouldRemove = true;
                        break;
                    }
                }
                if (shouldRemove) infos.RemoveAt(i);
            }
        }

        return infos;
    }

    public static string GetRolesString(PlayerControl p, bool useColors, bool showModifier = true, RoleType[] excludeRoles = null, bool includeHidden = false, string joinSeparator = " ")
    {
        if (p == null || p.Data == null || p.Data.Disconnected) return string.Empty;

        var roleInfo = GetRoleInfoForPlayer(p, showModifier, excludeRoles);
        if (roleInfo.Count == 0) return string.Empty;

        var sb = new StringBuilder();

        void AppendNames(bool useMadmateColor = false)
        {
            bool first = true;
            foreach (var info in roleInfo)
            {
                if (!first) sb.Append(joinSeparator);
                Color c = useMadmateColor ? Madmate.NameColor : info.Color;
                sb.Append(useColors ? Helpers.Cs(c, info.Name) : info.Name);
                first = false;
            }
        }

        if (p.HasModifier(ModifierType.Madmate) || p.HasModifier(ModifierType.CreatedMadmate))
        {
            bool hasCrewmate = false;
            for (int i = 0; i < roleInfo.Count; i++) if (roleInfo[i].RoleType == RoleType.Crewmate) { hasCrewmate = true; break; }

            if (hasCrewmate)
            {
                return useColors ? Helpers.Cs(Madmate.NameColor, Madmate.FullName) : Madmate.FullName;
            }
            else
            {
                string prefix = useColors ? Helpers.Cs(Madmate.NameColor, Madmate.Prefix) : Madmate.Prefix;
                sb.Append(prefix);
                AppendNames(true);
                return sb.ToString();
            }
        }

        if (p.HasModifier(ModifierType.LastImpostor))
        {
            bool hasImpostor = false;
            for (int i = 0; i < roleInfo.Count; i++) if (roleInfo[i].RoleType == RoleType.Impostor) { hasImpostor = true; break; }

            if (hasImpostor)
            {
                return useColors ? Helpers.Cs(LastImpostor.NameColor, LastImpostor.FullName) : LastImpostor.FullName;
            }
            else
            {
                string postfix = useColors ? Helpers.Cs(LastImpostor.NameColor, LastImpostor.Postfix) : LastImpostor.Postfix;
                AppendNames();
                sb.Append(postfix);
                return sb.ToString();
            }
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