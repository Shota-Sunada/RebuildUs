namespace RebuildUs.Roles;

internal static class RoleHelpers
{
    private static readonly
        Dictionary<RoleType, (MethodInfo exists, MethodInfo isRole, MethodInfo setRole, MethodInfo eraseRole, MethodInfo swapRole)> MethodCache = [];

    private static (MethodInfo exists, MethodInfo isRole, MethodInfo setRole, MethodInfo eraseRole, MethodInfo swapRole) GetMethods(RoleType roleType)
    {
        if (MethodCache.TryGetValue(roleType,
                out var cached))
        {
            return cached;
        }

        foreach (var reg in RoleData.Roles)
        {
            if (reg.RoleType == roleType)
            {
                var type = reg.ClassType;
                if (type == null)
                {
                    break;
                }

                var methods = (
                    type.GetProperty("Exists", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)?.GetMethod,
                    type.GetMethod("IsRole", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy),
                    type.GetMethod("SetRole", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy),
                    type.GetMethod("EraseRole", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy),
                    type.GetMethod("SwapRole", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy));
                MethodCache[roleType] = methods;
                return methods;
            }
        }

        if (roleType != RoleType.NoRole && roleType != RoleType.Crewmate && roleType != RoleType.Impostor && roleType != RoleType.Lovers)
        {
            Logger.LogWarn("[GetMethods] There is no role type registration for: {0}", roleType);
        }
        (MethodInfo, MethodInfo, MethodInfo, MethodInfo, MethodInfo) nullMethods = (null, null, null, null, null);
        MethodCache[roleType] = nullMethods;
        return nullMethods;
    }

    extension(PlayerControl player)
    {
        internal bool IsRole(RoleType roleType)
        {
            if (player == null || roleType == RoleType.NoRole)
            {
                return false;
            }

            var role = ModRoleManager.GetRole(player);
            if (role != null && role.CurrentRoleType == roleType)
            {
                return true;
            }

            return roleType switch
            {
                RoleType.Crewmate => player.IsTeamCrewmate(),
                RoleType.Impostor => player.IsTeamImpostor(),
                RoleType.Lovers => player.IsLovers(),
                _ => false,
            };
        }

        internal bool SetRole(RoleType roleType)
        {
            if (player == null || roleType == RoleType.NoRole)
            {
                return false;
            }

            Logger.LogInfo("[SetRole] {0}({1}): {2}", player.Data?.PlayerName, player.PlayerId, Enum.GetName(roleType));

            if (roleType != RoleType.Lovers)
            {
                player.EraseAllRoles();
            }

            if (roleType is RoleType.Crewmate or RoleType.Impostor)
            {
                return true;
            }

            if (roleType == RoleType.Lovers)
            {
                return true;
            }

            var (_, _, setRole, _, _) = GetMethods(roleType);
            if (setRole != null)
            {
                setRole.Invoke(null, [player]);
                return true;
            }

            Logger.LogWarn("[SetRole] There is no role type: {0}", Enum.GetName(roleType));
            return false;
        }

        internal void EraseRole(RoleType roleType)
        {
            if (player == null || roleType == RoleType.NoRole)
            {
                return;
            }

            if (roleType == RoleType.Lovers)
            {
                Lovers.EraseCouple(player);
                return;
            }

            if (player.IsRole(roleType))
            {
                var (_, _, _, eraseRole, _) = GetMethods(roleType);
                if (eraseRole != null)
                {
                    eraseRole.Invoke(null, [player]);
                    return;
                }

                Logger.LogWarn("[EraseRole] There is no role type: {0}", Enum.GetName(roleType));
            }
        }

        internal void EraseAllRoles()
        {
            if (player == null)
            {
                return;
            }

            foreach (var reg in RoleData.Roles)
            {
                if (reg.ClassType == null)
                {
                    continue;
                }

                var (_, _, _, eraseRole, _) = GetMethods(reg.RoleType);
                eraseRole?.Invoke(null, [player]);
            }
        }

        internal void SwapRoles(PlayerControl target)
        {
            if (player == null || target == null)
            {
                return;
            }

            foreach (var reg in RoleData.Roles)
            {
                if (reg.ClassType == null || !player.IsRole(reg.RoleType) && !target.IsRole(reg.RoleType))
                {
                    continue;
                }

                var (_, _, _, _, swapRole) = GetMethods(reg.RoleType);
                swapRole?.Invoke(null, [player, target]);
            }
        }

        internal string ModifyNameText(string nameText)
        {
            if (player == null || player.Data.Disconnected)
            {
                return nameText;
            }

            foreach (var role in ModRoleManager.AllRoles)
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

        internal string ModifyRoleText(string roleText, List<RoleInfo> roleInfo, bool useColors = true, bool includeHidden = false)
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

        internal void OnKill(PlayerControl target)
        {
            ModRoleManager.AllRoles.DoIf(x => x.Player == player, x => ModEventDispatcher.DispatchOnKill(x, target));
            PlayerModifier.AllModifiers.DoIf(x => x.Player == player, x => ModEventDispatcher.DispatchOnKill(x, target));
        }

        internal void OnDeath(PlayerControl killer)
        {
            ModRoleManager.AllRoles.DoIf(x => x.Player == player, x => ModEventDispatcher.DispatchOnDeath(x, killer));
            PlayerModifier.AllModifiers.DoIf(x => x.Player == player, x => ModEventDispatcher.DispatchOnDeath(x, killer));

            // Lover suicide trigger on exile/death
            // if (player.isLovers())
            //     Lovers.killLovers(player, killer);

            if (MeetingHud.Instance?.state != MeetingHud.VoteStates.Animating)
            {
                RPCProcedure.UpdateMeeting(player.PlayerId);
            }
        }

        internal void OnFinishShipStatusBegin()
        {
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(1f,
                new Action<float>(p =>
                {
                    if (!Mathf.Approximately(p, 1f))
                    {
                        return;
                    }
                    ModRoleManager.AllRoles.DoIf(x => x.Player == player, ModEventDispatcher.DispatchOnFinishShipStatusBegin);
                    PlayerModifier.AllModifiers.DoIf(x => x.Player == player, ModEventDispatcher.DispatchOnFinishShipStatusBegin);
                })));
        }
    }
}