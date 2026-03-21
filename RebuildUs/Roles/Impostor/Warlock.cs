namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
[RegisterRole(RoleType.Warlock, RoleTeam.Impostor, typeof(MultiRoleBase<Warlock>), nameof(CustomOptionHolder.WarlockSpawnRate))]
internal class Warlock : MultiRoleBase<Warlock>
{
    public static Color Color = Palette.ImpostorRed;

    internal static CustomButton WarlockCurseButton;

    private PlayerControl _currentTarget;
    private PlayerControl _curseVictim;
    private PlayerControl _curseVictimTarget;

    public Warlock()
    {
        StaticRoleType = CurrentRoleType = RoleType.Warlock;
    }

    private static float Cooldown { get => CustomOptionHolder.WarlockCooldown.GetFloat(); }
    private static float RootTime { get => CustomOptionHolder.WarlockRootTime.GetFloat(); }

    [CustomEvent(CustomEventType.FixedUpdate)]
    internal void FixedUpdate()
    {
        var local = Local;
        if (local == null)
        {
            return;
        }
        if (_curseVictim != null && (_curseVictim.Data.Disconnected || _curseVictim.Data.IsDead))
        {
            // If the cursed victim is disconnected or dead reset the curse so a new curse can be applied
            WarlockCurseButton.Timer = WarlockCurseButton.MaxTimer;
            WarlockCurseButton.Sprite = AssetLoader.CurseButton;
            WarlockCurseButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
            Local._currentTarget = null;
            Local._curseVictim = null;
            Local._curseVictimTarget = null;
        }

        if (_curseVictim == null)
        {
            _currentTarget = Helpers.SetTarget();
            Helpers.SetPlayerOutline(_currentTarget, RoleColor);
        }
        else
        {
            _curseVictimTarget = Helpers.SetTarget(targetingPlayer: _curseVictim);
            Helpers.SetPlayerOutline(_curseVictimTarget, RoleColor);
        }
    }

    [CustomEvent(CustomEventType.OnKill)]
    internal void OnKill(PlayerControl target)
    {
        if (!PlayerControl.LocalPlayer.IsRole(RoleType.Warlock) || WarlockCurseButton == null)
        {
            return;
        }
        if (Player.killTimer > WarlockCurseButton.Timer)
        {
            WarlockCurseButton.Timer = Player.killTimer;
        }
    }

    [RegisterCustomButton]
    internal static void MakeButtons(HudManager hm)
    {
        WarlockCurseButton = new(
            () =>
            {
                if (Local._curseVictim == null)
                {
                    // Apply Curse
                    Local._curseVictim = Local._currentTarget;
                    WarlockCurseButton.Sprite = AssetLoader.CurseKillButton;
                    WarlockCurseButton.Timer = 1f;
                    WarlockCurseButton.ButtonText = Tr.Get(TrKey.CurseKillText);
                }
                else if (Local._curseVictim != null && Local._curseVictimTarget != null)
                {
                    var murder = Helpers.CheckMurderAttemptAndKill(Local._curseVictim, Local._curseVictimTarget, showAnimation: false);
                    if (murder == MurderAttemptResult.SuppressKill)
                    {
                        return;
                    }

                    // If blanked or killed
                    WarlockCurseButton.ButtonText = Tr.Get(TrKey.CurseText);
                    if (RootTime > 0)
                    {
                        PlayerControl.LocalPlayer.moveable = false;
                        PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement so the warlock is not just running straight into the next object
                        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(RootTime,
                            new Action<float>(p =>
                            {
                                // Delayed action
                                if (Mathf.Approximately(p, 1f))
                                {
                                    PlayerControl.LocalPlayer.moveable = true;
                                }
                            })));
                    }

                    Local._curseVictim = null;
                    Local._curseVictimTarget = null;
                    WarlockCurseButton.Sprite = AssetLoader.CurseButton;
                    Local.Player.killTimer = WarlockCurseButton.Timer = WarlockCurseButton.MaxTimer;
                }
            },
            () =>
            {
                return PlayerControl.LocalPlayer.IsRole(RoleType.Warlock) && PlayerControl.LocalPlayer.IsAlive();
            },
            () =>
            {
                return (Local._curseVictim == null && Local._currentTarget != null || Local._curseVictim != null && Local._curseVictimTarget != null) && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                WarlockCurseButton.Timer = WarlockCurseButton.MaxTimer;
                WarlockCurseButton.Sprite = AssetLoader.CurseButton;
                WarlockCurseButton.ButtonText = Tr.Get(TrKey.CurseText);
                Local._curseVictim = null;
                Local._curseVictimTarget = null;
            },
            AssetLoader.CurseButton,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            Tr.Get(TrKey.CurseText));
    }

    [SetCustomButtonTimer]
    internal static void SetButtonCooldowns()
    {
        WarlockCurseButton.MaxTimer = Cooldown;
    }

    internal static void Clear()
    {
        Players.Clear();
    }
}