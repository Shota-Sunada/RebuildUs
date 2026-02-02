namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
public class Warlock : RoleBase<Warlock>
{
    public static Color NameColor = Palette.ImpostorRed;
    public override Color RoleColor => NameColor;
    public static CustomButton WarlockCurseButton;

    // write configs here
    public static float Cooldown { get { return CustomOptionHolder.WarlockCooldown.GetFloat(); } }
    public static float RootTime { get { return CustomOptionHolder.WarlockRootTime.GetFloat(); } }

    public PlayerControl CurrentTarget;
    public PlayerControl CurseVictim;
    public PlayerControl CurseVictimTarget;

    public Warlock()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Warlock;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {
        var local = Local;
        if (local == null) return;
        if (CurseVictim != null && (CurseVictim.Data.Disconnected || CurseVictim.Data.IsDead))
        {
            // If the cursed victim is disconnected or dead reset the curse so a new curse can be applied
            WarlockCurseButton.Timer = WarlockCurseButton.MaxTimer;
            WarlockCurseButton.Sprite = AssetLoader.CurseButton;
            WarlockCurseButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
            Local.CurrentTarget = null;
            Local.CurseVictim = null;
            Local.CurseVictimTarget = null;
        }
        if (CurseVictim == null)
        {
            CurrentTarget = Helpers.SetTarget();
            Helpers.SetPlayerOutline(CurrentTarget, RoleColor);
        }
        else
        {
            CurseVictimTarget = Helpers.SetTarget(targetingPlayer: CurseVictim);
            Helpers.SetPlayerOutline(CurseVictimTarget, RoleColor);
        }
    }
    public override void OnKill(PlayerControl target)
    {
        if (PlayerControl.LocalPlayer.IsRole(RoleType.Warlock) && WarlockCurseButton != null)
        {
            if (Player.killTimer > WarlockCurseButton.Timer)
            {
                WarlockCurseButton.Timer = Player.killTimer;
            }
        }
    }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public static void MakeButtons(HudManager hm)
    {
        WarlockCurseButton = new CustomButton(
            () =>
            {
                if (Local.CurseVictim == null)
                {
                    // Apply Curse
                    Local.CurseVictim = Local.CurrentTarget;
                    WarlockCurseButton.Sprite = AssetLoader.CurseKillButton;
                    WarlockCurseButton.Timer = 1f;
                    WarlockCurseButton.ButtonText = Tr.Get(TranslateKey.CurseKillText);
                }
                else if (Local.CurseVictim != null && Local.CurseVictimTarget != null)
                {
                    MurderAttemptResult murder = Helpers.CheckMurderAttemptAndKill(Local.CurseVictim, Local.CurseVictimTarget, showAnimation: false);
                    if (murder == MurderAttemptResult.SuppressKill) return;

                    // If blanked or killed
                    WarlockCurseButton.ButtonText = Tr.Get(TranslateKey.CurseText);
                    if (RootTime > 0)
                    {
                        PlayerControl.LocalPlayer.moveable = false;
                        PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement so the warlock is not just running straight into the next object
                        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(RootTime, new Action<float>((p) =>
                        {
                            // Delayed action
                            if (p == 1f)
                            {
                                PlayerControl.LocalPlayer.moveable = true;
                            }
                        })));
                    }

                    Local.CurseVictim = null;
                    Local.CurseVictimTarget = null;
                    WarlockCurseButton.Sprite = AssetLoader.CurseButton;
                    Local.Player.killTimer = WarlockCurseButton.Timer = WarlockCurseButton.MaxTimer;
                }
            },
            () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Warlock) && PlayerControl.LocalPlayer.IsAlive(); },
            () => { return ((Local.CurseVictim == null && Local.CurrentTarget != null) || (Local.CurseVictim != null && Local.CurseVictimTarget != null)) && PlayerControl.LocalPlayer.CanMove; },
            () =>
            {
                WarlockCurseButton.Timer = WarlockCurseButton.MaxTimer;
                WarlockCurseButton.Sprite = AssetLoader.CurseButton;
                WarlockCurseButton.ButtonText = Tr.Get(TranslateKey.CurseText);
                Local.CurseVictim = null;
                Local.CurseVictimTarget = null;
            },
            AssetLoader.CurseButton,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            Tr.Get(TranslateKey.CurseText)
        );
    }
    public static void SetButtonCooldowns()
    {
        WarlockCurseButton.MaxTimer = Cooldown;
    }

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}