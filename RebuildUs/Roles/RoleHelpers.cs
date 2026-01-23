using System.Reflection;

namespace RebuildUs.Roles;

public static class RoleHelpers
{
    public static bool IsRole(this PlayerControl player, RoleType roleType)
    {
        if (player == null) return false;

        if (roleType == RoleType.Crewmate) return player.IsTeamCrewmate();
        if (roleType == RoleType.Impostor) return player.IsTeamImpostor();
        if (roleType == RoleType.Lovers) return player.IsLovers();
        if (roleType == RoleType.GM) return player.IsGM();

        foreach (var type in RoleData.Roles)
        {
            if (roleType == type.roleType)
            {
                return type.classType != null
                    && (bool)(type.classType.GetProperty("Exists", BindingFlags.Public | BindingFlags.Static)?.GetValue(null) ?? false)
                    && (bool)(type.classType.GetMethod("IsRole", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, [player]) ?? false);
            }
        }

        Logger.LogWarn($"There is no role type: {roleType}", "IsRole");

        return false;
    }

    public static bool SetRole(this PlayerControl player, RoleType roleType)
    {
        if (player == null) return false;

        Logger.LogInfo($"{player?.Data?.PlayerName}({player?.PlayerId}): {Enum.GetName(typeof(RoleType), roleType)}");

        // SetRole usually implies setting a primary role, so we should clear existing primary roles.
        // But for Lovers, it might be additive. However, Lovers uses its own SetRole usually.
        if (roleType != RoleType.Lovers)
            player.EraseAllRoles();

        if (roleType == RoleType.Crewmate || roleType == RoleType.Impostor)
        {
            // vanilla roles are already set by the game, or handled via other means.
            // EraseAllRoles above already removed any custom roles.
            return true;
        }

        if (roleType == RoleType.Lovers)
        {
            // Lovers doesn't follow the RoleBase pattern. It's managed via Couples.
            // If we just want to mark someone as Lovers, we need a partner.
            // In RebuildUs, Lovers assignment is usually handled via separate RPC setLovers.
            return true;
        }

        foreach (var type in RoleData.Roles)
        {
            if (roleType == type.roleType)
            {
                if (type.classType == null) return false;
                var method = type.classType.GetMethod("SetRole", BindingFlags.Public | BindingFlags.Static);
                if (method != null)
                {
                    method.Invoke(null, [player]);
                    return true;
                }
            }
        }

        Logger.LogWarn($"There is no role type: {roleType}", "SetRole");

        return false;
    }

    public static void EraseRole(this PlayerControl player, RoleType roleType)
    {
        if (player == null) return;

        if (roleType == RoleType.Lovers)
        {
            Lovers.EraseCouple(player);
            return;
        }

        if (IsRole(player, roleType))
        {
            foreach (var type in RoleData.Roles)
            {
                if (roleType == type.roleType)
                {
                    if (type.classType == null) return;
                    type.classType.GetMethod("EraseRole", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, [player]);
                    return;
                }
            }

            Logger.LogWarn($"There is no role type: {roleType}", "EraseRole");
        }
    }

    public static void EraseAllRoles(this PlayerControl player)
    {
        foreach (var type in RoleData.Roles)
        {
            if (type.classType == null) continue;
            type.classType.GetMethod("EraseRole", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, [player]);
        }
    }

    public static void SwapRoles(this PlayerControl player, PlayerControl target)
    {
        foreach (var type in RoleData.Roles)
        {
            if (type.classType != null && player.IsRole(type.roleType))
            {
                type.classType.GetMethod("SwapRole", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, [player, target]);
            }
        }
    }

    public static string ModifyNameText(this PlayerControl player, string nameText)
    {
        if (player == null || player.Data.Disconnected) return nameText;

        foreach (var role in PlayerRole.AllRoles)
        {
            if (role.Player == player)
            {
                nameText = role.ModifyNameText(nameText);
            }
        }

        foreach (var mod in PlayerModifier.AllModifiers)
        {
            if (mod.Player == player)
            {
                nameText = mod.ModifyNameText(nameText);
            }
        }

        // nameText += Lovers.getIcon(player);

        return nameText;
    }

    public static string ModifyRoleText(this PlayerControl player, string roleText, List<RoleInfo> roleInfo, bool useColors = true, bool includeHidden = false)
    {
        foreach (var mod in PlayerModifier.AllModifiers)
        {
            if (mod.Player == player)
            {
                roleText = mod.ModifyRoleText(roleText, roleInfo, useColors, includeHidden);
            }
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

        if (MeetingHud.Instance?.state != MeetingHud.VoteStates.Animating)
        {
            RPCProcedure.UpdateMeeting(player.PlayerId, true);
        }
    }

    public static void OnFinishShipStatusBegin(this PlayerControl player)
    {
        HudManager.Instance.StartCoroutine(Effects.Lerp(1f, new Action<float>((p) =>
        {
            if (p == 1f)
            {
                PlayerRole.AllRoles.DoIf(x => x.Player == player, x => x.OnFinishShipStatusBegin());
                PlayerModifier.AllModifiers.DoIf(x => x.Player == player, x => x.OnFinishShipStatusBegin());
            }
        })));
    }
}