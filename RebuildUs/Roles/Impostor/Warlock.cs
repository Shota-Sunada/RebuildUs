namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
internal class Warlock : RoleBase<Warlock>
{
    internal static Color NameColor = Palette.ImpostorRed;

    internal static CustomButton WarlockCurseButton;

    internal PlayerControl CurrentTarget;
    internal PlayerControl CurseVictim;
    internal PlayerControl CurseVictimTarget;

    public Warlock()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Warlock;
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    internal static float Cooldown { get => CustomOptionHolder.WarlockCooldown.GetFloat(); }
    internal static float RootTime { get => CustomOptionHolder.WarlockRootTime.GetFloat(); }

    internal override void OnMeetingStart() { }
    internal override void OnMeetingEnd() { }
    internal override void OnIntroEnd() { }

    internal override void FixedUpdate()
    {
        Warlock local = Local;
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

    internal override void OnKill(PlayerControl target)
    {
        if (PlayerControl.LocalPlayer.IsRole(RoleType.Warlock) && WarlockCurseButton != null)
            if (Player.killTimer > WarlockCurseButton.Timer)
                WarlockCurseButton.Timer = Player.killTimer;
    }

    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    internal static void MakeButtons(HudManager hm)
    {
        WarlockCurseButton = new(() =>
        {
            if (Local.CurseVictim == null)
            {
                // Apply Curse
                Local.CurseVictim = Local.CurrentTarget;
                WarlockCurseButton.Sprite = AssetLoader.CurseKillButton;
                WarlockCurseButton.Timer = 1f;
                WarlockCurseButton.ButtonText = Tr.Get(TrKey.CurseKillText);
            }
            else if (Local.CurseVictim != null && Local.CurseVictimTarget != null)
            {
                MurderAttemptResult murder = Helpers.CheckMurderAttemptAndKill(Local.CurseVictim, Local.CurseVictimTarget, showAnimation: false);
                if (murder == MurderAttemptResult.SuppressKill) return;

                // If blanked or killed
                WarlockCurseButton.ButtonText = Tr.Get(TrKey.CurseText);
                if (RootTime > 0)
                {
                    PlayerControl.LocalPlayer.moveable = false;
                    PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement so the warlock is not just running straight into the next object
                    FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(RootTime, new Action<float>(p =>
                    {
                        // Delayed action
                        if (p == 1f) PlayerControl.LocalPlayer.moveable = true;
                    })));
                }

                Local.CurseVictim = null;
                Local.CurseVictimTarget = null;
                WarlockCurseButton.Sprite = AssetLoader.CurseButton;
                Local.Player.killTimer = WarlockCurseButton.Timer = WarlockCurseButton.MaxTimer;
            }
        }, () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Warlock) && PlayerControl.LocalPlayer.IsAlive(); }, () => { return ((Local.CurseVictim == null && Local.CurrentTarget != null) || (Local.CurseVictim != null && Local.CurseVictimTarget != null)) && PlayerControl.LocalPlayer.CanMove; }, () =>
        {
            WarlockCurseButton.Timer = WarlockCurseButton.MaxTimer;
            WarlockCurseButton.Sprite = AssetLoader.CurseButton;
            WarlockCurseButton.ButtonText = Tr.Get(TrKey.CurseText);
            Local.CurseVictim = null;
            Local.CurseVictimTarget = null;
        }, AssetLoader.CurseButton, ButtonPosition.Layout, hm, hm.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, Tr.Get(TrKey.CurseText));
    }

    internal static void SetButtonCooldowns()
    {
        WarlockCurseButton.MaxTimer = Cooldown;
    }

    internal static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}