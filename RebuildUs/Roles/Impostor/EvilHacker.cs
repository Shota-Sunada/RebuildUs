namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
[RegisterRole(RoleType.EvilHacker, RoleTeam.Impostor, typeof(MultiRoleBase<EvilHacker>), nameof(CustomOptionHolder.EvilHackerSpawnRate))]
internal class EvilHacker : MultiRoleBase<EvilHacker>
{
    internal static Color Color = Palette.ImpostorRed;
    private static CustomButton _evilHackerButton;
    private static CustomButton _evilHackerCreatesMadmateButton;

    private PlayerControl _currentTarget;
    internal bool CanCreateMadmate;
    internal PlayerControl FakeMadmate;

    public EvilHacker()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.EvilHacker;
        CanCreateMadmate = CustomOptionHolder.EvilHackerCanCreateMadmate.GetBool();
    }

    // write configs here

    internal static bool CanHasBetterAdmin
    {
        get => CustomOptionHolder.EvilHackerCanHasBetterAdmin.GetBool();
    }

    internal static bool CanMoveEvenIfUsesAdmin
    {
        get => CustomOptionHolder.EvilHackerCanMoveEvenIfUsesAdmin.GetBool();
    }

    internal static bool CanInheritAbility
    {
        get => CustomOptionHolder.EvilHackerCanInheritAbility.GetBool();
    }

    internal static bool CanSeeDoorStatus
    {
        get => CustomOptionHolder.EvilHackerCanSeeDoorStatus.GetBool();
    }

    internal static bool CreatedMadmateCanDieToSheriff
    {
        get => CustomOptionHolder.CreatedMadmateCanDieToSheriff.GetBool();
    }

    internal static bool CreatedMadmateCanEnterVents
    {
        get => CustomOptionHolder.CreatedMadmateCanEnterVents.GetBool();
    }

    internal static bool CanCreateMadmateFromJackal
    {
        get => CustomOptionHolder.EvilHackerCanCreateMadmateFromJackal.GetBool();
    }

    internal static bool CreatedMadmateHasImpostorVision
    {
        get => CustomOptionHolder.CreatedMadmateHasImpostorVision.GetBool();
    }

    internal static bool CreatedMadmateCanSabotage
    {
        get => CustomOptionHolder.CreatedMadmateCanSabotage.GetBool();
    }

    internal static bool CreatedMadmateCanFixComm
    {
        get => CustomOptionHolder.CreatedMadmateCanFixComm.GetBool();
    }

    internal static int CreatedMadmateAbility
    {
        get => CustomOptionHolder.CreatedMadmateAbility.GetSelection();
    }

    internal static float CreatedMadmateNumTasks
    {
        get => CustomOptionHolder.CreatedMadmateNumTasks.GetFloat();
    }

    internal static bool CreatedMadmateExileCrewmate
    {
        get => CustomOptionHolder.CreatedMadmateExileCrewmate.GetBool();
    }

    [CustomEvent(CustomEventType.FixedUpdate)]
    internal void FixedUpdate()
    {
        var local = Local;
        if (local != null)
        {
            _currentTarget = Helpers.SetTarget(true);
            Helpers.SetPlayerOutline(_currentTarget, RoleColor);
        }
    }



    [RegisterCustomButton]
    internal static void MakeButtons(HudManager hm)
    {
        _evilHackerButton = new(() =>
            {
                PlayerControl.LocalPlayer.NetTransform.Halt();
                Admin.IsEvilHackerAdmin = true;
                FastDestroyableSingleton<HudManager>.Instance.ToggleMapVisible(new()
                {
                    Mode = MapOptions.Modes.CountOverlay,
                    AllowMovementWhileMapOpen = CanMoveEvenIfUsesAdmin,
                    ShowLivePlayerPosition = true,
                    IncludeDeadBodies = true,
                });
            },
            () =>
            {
                return (PlayerControl.LocalPlayer.IsRole(RoleType.EvilHacker) && PlayerControl.LocalPlayer.IsAlive()
                        || IsInherited() && PlayerControl.LocalPlayer.IsTeamImpostor())
                       && !RebuildUs.BetterSabotageMap.Value;
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove;
            },
            () => { },
            Hacker.GetAdminSprite(),
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            0f,
            () => { },
            ByteOptionNames.MapId.Get() == 3,
            FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Admin));

        _evilHackerCreatesMadmateButton = new(() =>
            {
                EvilHackerCreatesMadmate(PlayerControl.LocalPlayer, Local._currentTarget.PlayerId, Local.Player.PlayerId);
            },
            () =>
            {
                return PlayerControl.LocalPlayer.IsRole(RoleType.EvilHacker) && Local.CanCreateMadmate && PlayerControl.LocalPlayer.IsAlive();
            },
            () =>
            {
                return Local._currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () => { },
            AssetLoader.SidekickButton,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            AbilitySlot.ImpostorAbilitySecondary,
            false,
            Tr.Get(TrKey.Madmate));
    }

    [RegisterCustomButton]
    internal static void SetButtonCooldowns()
    {
        _evilHackerButton.MaxTimer = 0f;
        _evilHackerCreatesMadmateButton.MaxTimer = 0f;
    }

    // write functions here
    internal static bool IsInherited()
    {
        return CanInheritAbility && Exists && LivingPlayers.Count == 0 && PlayerControl.LocalPlayer.IsTeamImpostor();
    }

    internal static void Clear()
    {
        // reset configs here
        Players.Clear();
    }

    [MethodRpc((uint)CustomRPC.EvilHackerCreatesMadmate)]
    internal static void EvilHackerCreatesMadmate(PlayerControl sender, byte targetId, byte evilHackerId)
    {
        var targetPlayer = Helpers.PlayerById(targetId);
        var evilHackerPlayer = Helpers.PlayerById(evilHackerId);
        if (targetPlayer == null || evilHackerPlayer == null)
        {
            return;
        }
        var evilHacker = GetRole(evilHackerPlayer);
        if (evilHacker == null)
        {
            return;
        }
        if (!CanCreateMadmateFromJackal && targetPlayer.IsRole(RoleType.Jackal))
        {
            evilHacker.FakeMadmate = targetPlayer;
        }
        else
        {
            List<PlayerControl> tmpFormerJackals = [.. Jackal.FormerJackals];

            if (targetPlayer.HasFakeTasks())
            {
                if (CreatedMadmate.HasTasks)
                {
                    targetPlayer.ClearAllTasks();
                    targetPlayer.GenerateAndAssignTasks(0, CreatedMadmate.NumTasks, 0);
                }
            }

            FastDestroyableSingleton<RoleManager>.Instance.SetRole(targetPlayer, RoleTypes.Crewmate);
            Eraser.ErasePlayerRolesLocal(targetPlayer.PlayerId, true, false);

            Jackal.FormerJackals = tmpFormerJackals;

            targetPlayer.AddModifier(ModifierType.CreatedMadmate);
        }

        evilHacker.CanCreateMadmate = false;
    }
}