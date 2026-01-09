using System.Reflection;
using RebuildUs.Modules.RPC;

namespace RebuildUs.Roles;

public static class RoleHelpers
{
    public static bool IsRole(this PlayerControl player, ERoleType roleType)
    {
        foreach (var type in RoleData.AllRoleTypes)
        {
            if (roleType == type.RoleType)
            {
                return (bool)type.Type.GetMethod("IsRole", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, [player]);
            }
        }

        Logger.LogWarning($"There is no role type: {roleType}");

        return false;
    }

    public static bool SetRole(this PlayerControl player, ERoleType roleType)
    {
        Logger.LogInfo($"{player?.Data?.PlayerName}({player?.PlayerId}): {Enum.GetName(typeof(ERoleType), roleType)}");
        foreach (var type in RoleData.AllRoleTypes)
        {
            if (roleType == type.RoleType)
            {
                return (bool)type.Type.GetMethod("SetRole", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, [player]);
            }
        }

        Logger.LogWarning($"There is no role type: {roleType}");

        return false;
    }

    public static void EraseRole(this PlayerControl player, ERoleType roleType)
    {
        if (IsRole(player, roleType))
        {
            foreach (var type in RoleData.AllRoleTypes)
            {
                if (roleType == type.RoleType)
                {
                    type.Type.GetMethod("EraseRole", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, [player]);
                    return;
                }
            }

            Logger.LogWarning($"There is no role type: {roleType}");
        }
    }

    public static void EraseAllRoles(this PlayerControl player)
    {
        foreach (var type in RoleData.AllRoleTypes)
        {
            type.Type.GetMethod("EraseRole", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, [player]);
        }

        // if (player.isRole(RoleType.Mayor)) Mayor.clearAndReload();
    }

    public static void SwapRoles(this PlayerControl player, PlayerControl target)
    {
        foreach (var type in RoleData.AllRoleTypes)
        {
            if (player.IsRole(type.RoleType))
            {
                type.Type.GetMethod("SwapRole", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, [player, target]);
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

        // foreach (var mod in Modifier.allModifiers)
        // {
        //     if (mod.player == player)
        //         nameText = mod.modifyNameText(nameText);
        // }

        // nameText += Lovers.getIcon(player);

        return nameText;
    }

    // public static string ModifyRoleText(this PlayerControl player, string roleText, List<RoleInfo> roleInfo, bool useColors = true, bool includeHidden = false)
    // {
    //     foreach (var mod in Modifier.allModifiers)
    //     {
    //         if (mod.player == player)
    //             roleText = mod.modifyRoleText(roleText, roleInfo, useColors, includeHidden);
    //     }
    //     return roleText;
    // }

    public static void OnKill(this PlayerControl player, PlayerControl target)
    {
        PlayerRole.AllRoles.DoIf(x => x.Player == player, x => x.OnKill(target));
        // Modifier.allModifiers.DoIf(x => x.player == player, x => x.OnKill(target));
    }

    public static void OnDeath(this PlayerControl player, PlayerControl killer)
    {
        PlayerRole.AllRoles.DoIf(x => x.Player == player, x => x.OnDeath(killer));
        // Modifier.allModifiers.DoIf(x => x.player == player, x => x.OnDeath(killer));

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
                // Modifier.allModifiers.DoIf(x => x.player == player, x => x.OnFinishShipStatusBegin());
            }
        })));
    }
}