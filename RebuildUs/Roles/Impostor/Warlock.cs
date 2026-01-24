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

    public static PlayerControl CurrentTarget;
    public static PlayerControl CurseVictim;
    public static PlayerControl CurseVictimTarget;

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
            ResetCurse();
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
                if (CurseVictim == null)
                {
                    // Apply Curse
                    CurseVictim = CurrentTarget;
                    WarlockCurseButton.Sprite = AssetLoader.CurseKillButton;
                    WarlockCurseButton.Timer = 1f;
                    WarlockCurseButton.ButtonText = Tr.Get("Hud.CurseKillText");
                }
                else if (CurseVictim != null && CurseVictimTarget != null)
                {
                    MurderAttemptResult murder = Helpers.CheckMurderAttemptAndKill(Local.Player, CurseVictimTarget, showAnimation: false);
                    if (murder == MurderAttemptResult.SuppressKill) return;

                    // If blanked or killed
                    WarlockCurseButton.ButtonText = Tr.Get("Hud.CurseText");
                    if (RootTime > 0)
                    {
                        PlayerControl.LocalPlayer.moveable = false;
                        PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement so the warlock is not just running straight into the next object
                        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(RootTime, new Action<float>((p) =>
                        { // Delayed action
                            if (p == 1f)
                            {
                                PlayerControl.LocalPlayer.moveable = true;
                            }
                        })));
                    }

                    CurseVictim = null;
                    CurseVictimTarget = null;
                    WarlockCurseButton.Sprite = AssetLoader.CurseButton;
                    Local.Player.killTimer = WarlockCurseButton.Timer = WarlockCurseButton.MaxTimer;
                }
            },
            () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Warlock) && PlayerControl.LocalPlayer.IsAlive(); },
            () => { return ((CurseVictim == null && CurrentTarget != null) || (CurseVictim != null && CurseVictimTarget != null)) && PlayerControl.LocalPlayer.CanMove; },
            () =>
            {
                WarlockCurseButton.Timer = WarlockCurseButton.MaxTimer;
                WarlockCurseButton.Sprite = AssetLoader.CurseButton;
                WarlockCurseButton.ButtonText = Tr.Get("Hud.CurseText");
                CurseVictim = null;
                CurseVictimTarget = null;
            },
            AssetLoader.CurseButton,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            KeyCode.F
        )
        {
            ButtonText = Tr.Get("Hud.CurseText")
        };
    }
    public static void SetButtonCooldowns()
    {
        WarlockCurseButton.MaxTimer = Cooldown;
    }

    // write functions here
    public static void ResetCurse()
    {
        WarlockCurseButton.Timer = WarlockCurseButton.MaxTimer;
        WarlockCurseButton.Sprite = AssetLoader.CurseButton;
        WarlockCurseButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
        CurrentTarget = null;
        CurseVictim = null;
        CurseVictimTarget = null;
    }

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
        CurrentTarget = null;
        CurseVictim = null;
        CurseVictimTarget = null;
    }
}