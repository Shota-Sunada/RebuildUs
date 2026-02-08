using System.Reflection;

namespace RebuildUs.Roles;

public static class RoleHelpers
{
    private static readonly Dictionary<RoleType, (MethodInfo exists, MethodInfo isRole, MethodInfo setRole, MethodInfo eraseRole, MethodInfo swapRole)> METHOD_CACHE = [];

    private static (MethodInfo exists, MethodInfo isRole, MethodInfo setRole, MethodInfo eraseRole, MethodInfo swapRole) GetMethods(RoleType roleType)
    {
        if (METHOD_CACHE.TryGetValue(roleType, out var cached)) return cached;

        foreach (var reg in RoleData.ROLES)
        {
            if (reg.RoleType == roleType)
            {
                var type = reg.ClassType;
                if (type == null) break;

                var methods = (type.GetProperty("Exists", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)?.GetMethod, type.GetMethod("IsRole", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy), type.GetMethod("SetRole", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy), type.GetMethod("EraseRole", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy), type.GetMethod("SwapRole", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy));
                METHOD_CACHE[roleType] = methods;
                return methods;
            }
        }

        if (roleType != RoleType.NoRole && roleType != RoleType.Crewmate && roleType != RoleType.Impostor && roleType != RoleType.Lovers && roleType != RoleType.Gm)
            Logger.LogWarn($"There is no role type registration for: {roleType}", "GetMethods");

        var nullMethods = ((MethodInfo)null, (MethodInfo)null, (MethodInfo)null, (MethodInfo)null, (MethodInfo)null);
        METHOD_CACHE[roleType] = nullMethods;
        return nullMethods;
    }

    public static bool IsRole(this PlayerControl player, RoleType roleType)
    {
        if (player == null || roleType == RoleType.NoRole) return false;

        var role = PlayerRole.GetRole(player);
        if (role != null && role.CurrentRoleType == roleType) return true;

        if (roleType == RoleType.Crewmate) return player.IsTeamCrewmate();
        if (roleType == RoleType.Impostor) return player.IsTeamImpostor();
        if (roleType == RoleType.Lovers) return player.IsLovers();
        if (roleType == RoleType.Gm) return player.IsGm();

        return false;
    }

    public static bool SetRole(this PlayerControl player, RoleType roleType)
    {
        if (player == null || roleType == RoleType.NoRole) return false;

        Logger.LogInfo($"{player.Data?.PlayerName}({player.PlayerId}): {Enum.GetName(typeof(RoleType), roleType)}");

        if (roleType != RoleType.Lovers)
            player.EraseAllRoles();

        if (roleType == RoleType.Crewmate || roleType == RoleType.Impostor) return true;

        if (roleType == RoleType.Lovers) return true;

        var methods = GetMethods(roleType);
        if (methods.setRole != null)
        {
            methods.setRole.Invoke(null, [player]);
            return true;
        }

        Logger.LogWarn($"There is no role type: {roleType}", "SetRole");
        return false;
    }

    public static void EraseRole(this PlayerControl player, RoleType roleType)
    {
        if (player == null || roleType == RoleType.NoRole) return;

        if (roleType == RoleType.Lovers)
        {
            Lovers.EraseCouple(player);
            return;
        }

        if (player.IsRole(roleType))
        {
            var methods = GetMethods(roleType);
            if (methods.eraseRole != null)
            {
                methods.eraseRole.Invoke(null, [player]);
                return;
            }

            Logger.LogWarn($"There is no role type: {roleType}", "EraseRole");
        }
    }

    public static void EraseAllRoles(this PlayerControl player)
    {
        if (player == null) return;

        foreach (var reg in RoleData.ROLES)
        {
            if (reg.ClassType == null) continue;
            var methods = GetMethods(reg.RoleType);
            methods.eraseRole?.Invoke(null, [player]);
        }
    }

    public static void SwapRoles(this PlayerControl player, PlayerControl target)
    {
        if (player == null || target == null) return;

        foreach (var reg in RoleData.ROLES)
        {
            if (reg.ClassType != null && (player.IsRole(reg.RoleType) || target.IsRole(reg.RoleType)))
            {
                var methods = GetMethods(reg.RoleType);
                methods.swapRole?.Invoke(null, [player, target]);
            }
        }
    }

    public static string ModifyNameText(this PlayerControl player, string nameText)
    {
        if (player == null || player.Data.Disconnected) return nameText;

        foreach (var role in PlayerRole.AllRoles)
        {
            if (role.Player == player)
                nameText = role.ModifyNameText(nameText);
        }

        foreach (var mod in PlayerModifier.AllModifiers)
        {
            if (mod.Player == player)
                nameText = mod.ModifyNameText(nameText);
        }

        // nameText += Lovers.getIcon(player);

        return nameText;
    }

    public static string ModifyRoleText(this PlayerControl player, string roleText, List<RoleInfo> roleInfo, bool useColors = true, bool includeHidden = false)
    {
        foreach (var mod in PlayerModifier.AllModifiers)
        {
            if (mod.Player == player)
                roleText = mod.ModifyRoleText(roleText, roleInfo, useColors, includeHidden);
        }

        return roleText;
    }

    public static void OnKill(this PlayerControl player, PlayerControl target)
    {
        PlayerRole.AllRoles.DoIf(x => x.Player == player, x => x.OnKill(target));
        PlayerModifier.AllModifiers.DoIf(x => x.Player == player, x => x.OnKill(target));
    }

    public static void OnDeath(this PlayerControl player, PlayerControl killer)
    {
        PlayerRole.AllRoles.DoIf(x => x.Player == player, x => x.OnDeath(killer));
        PlayerModifier.AllModifiers.DoIf(x => x.Player == player, x => x.OnDeath(killer));

        // Lover suicide trigger on exile/death
        // if (player.isLovers())
        //     Lovers.killLovers(player, killer);

        if (MeetingHud.Instance?.state != MeetingHud.VoteStates.Animating) RPCProcedure.UpdateMeeting(player.PlayerId);
    }

    public static void OnFinishShipStatusBegin(this PlayerControl player)
    {
        HudManager.Instance.StartCoroutine(Effects.Lerp(1f, new Action<float>(p =>
        {
            if (p == 1f)
            {
                PlayerRole.AllRoles.DoIf(x => x.Player == player, x => x.OnFinishShipStatusBegin());
                PlayerModifier.AllModifiers.DoIf(x => x.Player == player, x => x.OnFinishShipStatusBegin());
            }
        })));
    }
}
