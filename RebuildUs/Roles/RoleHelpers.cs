using System.Reflection;

namespace RebuildUs.Roles;

internal static class RoleHelpers
{
    private static readonly Dictionary<RoleType, (MethodInfo exists, MethodInfo isRole, MethodInfo setRole, MethodInfo eraseRole, MethodInfo swapRole)> MethodCache = [];

    private static (MethodInfo exists, MethodInfo isRole, MethodInfo setRole, MethodInfo eraseRole, MethodInfo swapRole) GetMethods(RoleType roleType)
    {
        if (MethodCache.TryGetValue(roleType, out (MethodInfo exists, MethodInfo isRole, MethodInfo setRole, MethodInfo eraseRole, MethodInfo swapRole) cached)) return cached;

        foreach (RoleData.RoleRegistration reg in RoleData.Roles)
        {
            if (reg.RoleType == roleType)
            {
                Type type = reg.ClassType;
                if (type == null) break;

                (MethodInfo GetMethod, MethodInfo, MethodInfo, MethodInfo, MethodInfo) methods = (type.GetProperty("Exists", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)?.GetMethod,
                                                                                                  type.GetMethod("IsRole", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy),
                                                                                                  type.GetMethod("SetRole", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy),
                                                                                                  type.GetMethod("EraseRole", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy),
                                                                                                  type.GetMethod("SwapRole", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy));
                MethodCache[roleType] = methods;
                return methods;
            }
        }

        if (roleType != RoleType.NoRole && roleType != RoleType.Crewmate && roleType != RoleType.Impostor && roleType != RoleType.Lovers)
            Logger.LogWarn($"There is no role type registration for: {roleType}", "GetMethods");
        (MethodInfo, MethodInfo, MethodInfo, MethodInfo, MethodInfo) nullMethods = (null, null, null, null, null);
        MethodCache[roleType] = nullMethods;
        return nullMethods;
    }

    internal static bool IsRole(this PlayerControl player, RoleType roleType)
    {
        if (player == null || roleType == RoleType.NoRole) return false;

        PlayerRole role = PlayerRole.GetRole(player);
        if (role != null && role.CurrentRoleType == roleType) return true;

        return roleType switch
        {
            RoleType.Crewmate => player.IsTeamCrewmate(),
            RoleType.Impostor => player.IsTeamImpostor(),
            RoleType.Lovers => player.IsLovers(),
            _ => false
        };
    }

    internal static bool SetRole(this PlayerControl player, RoleType roleType)
    {
        if (player == null || roleType == RoleType.NoRole) return false;

        Logger.LogInfo($"{player.Data?.PlayerName}({player.PlayerId}): {Enum.GetName(typeof(RoleType), roleType)}");

        if (roleType != RoleType.Lovers)
            player.EraseAllRoles();

        if (roleType is RoleType.Crewmate or RoleType.Impostor) return true;

        if (roleType == RoleType.Lovers) return true;

        (MethodInfo exists, MethodInfo isRole, MethodInfo setRole, MethodInfo eraseRole, MethodInfo swapRole) methods = GetMethods(roleType);
        if (methods.setRole != null)
        {
            methods.setRole.Invoke(null, [player]);
            return true;
        }

        Logger.LogWarn($"There is no role type: {roleType}", "SetRole");
        return false;
    }

    internal static void EraseRole(this PlayerControl player, RoleType roleType)
    {
        if (player == null || roleType == RoleType.NoRole) return;

        if (roleType == RoleType.Lovers)
        {
            Lovers.EraseCouple(player);
            return;
        }

        if (player.IsRole(roleType))
        {
            (MethodInfo exists, MethodInfo isRole, MethodInfo setRole, MethodInfo eraseRole, MethodInfo swapRole) methods = GetMethods(roleType);
            if (methods.eraseRole != null)
            {
                methods.eraseRole.Invoke(null, [player]);
                return;
            }

            Logger.LogWarn($"There is no role type: {roleType}", "EraseRole");
        }
    }

    internal static void EraseAllRoles(this PlayerControl player)
    {
        if (player == null) return;

        foreach (RoleData.RoleRegistration reg in RoleData.Roles)
        {
            if (reg.ClassType == null) continue;
            (MethodInfo exists, MethodInfo isRole, MethodInfo setRole, MethodInfo eraseRole, MethodInfo swapRole) methods = GetMethods(reg.RoleType);
            methods.eraseRole?.Invoke(null, [player]);
        }
    }

    internal static void SwapRoles(this PlayerControl player, PlayerControl target)
    {
        if (player == null || target == null) return;

        foreach (RoleData.RoleRegistration reg in RoleData.Roles)
        {
            if (reg.ClassType != null && (player.IsRole(reg.RoleType) || target.IsRole(reg.RoleType)))
            {
                (MethodInfo exists, MethodInfo isRole, MethodInfo setRole, MethodInfo eraseRole, MethodInfo swapRole) methods = GetMethods(reg.RoleType);
                methods.swapRole?.Invoke(null, [player, target]);
            }
        }
    }

    internal static string ModifyNameText(this PlayerControl player, string nameText)
    {
        if (player == null || player.Data.Disconnected) return nameText;

        foreach (PlayerRole role in PlayerRole.AllRoles)
        {
            if (role.Player == player)
                nameText = role.ModifyNameText(nameText);
        }

        foreach (PlayerModifier mod in PlayerModifier.AllModifiers)
        {
            if (mod.Player == player)
                nameText = mod.ModifyNameText(nameText);
        }

        // nameText += Lovers.getIcon(player);

        return nameText;
    }

    internal static string ModifyRoleText(this PlayerControl player, string roleText, List<RoleInfo> roleInfo, bool useColors = true, bool includeHidden = false)
    {
        foreach (PlayerModifier mod in PlayerModifier.AllModifiers)
        {
            if (mod.Player == player)
                roleText = mod.ModifyRoleText(roleText, roleInfo, useColors, includeHidden);
        }

        return roleText;
    }

    internal static void OnKill(this PlayerControl player, PlayerControl target)
    {
        PlayerRole.AllRoles.DoIf(x => x.Player == player, x => x.OnKill(target));
        PlayerModifier.AllModifiers.DoIf(x => x.Player == player, x => x.OnKill(target));
    }

    internal static void OnDeath(this PlayerControl player, PlayerControl killer)
    {
        PlayerRole.AllRoles.DoIf(x => x.Player == player, x => x.OnDeath(killer));
        PlayerModifier.AllModifiers.DoIf(x => x.Player == player, x => x.OnDeath(killer));

        // Lover suicide trigger on exile/death
        // if (player.isLovers())
        //     Lovers.killLovers(player, killer);

        if (MeetingHud.Instance?.state != MeetingHud.VoteStates.Animating) RPCProcedure.UpdateMeeting(player.PlayerId);
    }

    internal static void OnFinishShipStatusBegin(this PlayerControl player)
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