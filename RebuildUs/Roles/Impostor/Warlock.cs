namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
public class Warlock : RoleBase<Warlock>
{
    public static Color NameColor = Palette.ImpostorRed;
    public override Color RoleColor => NameColor;
    public static CustomButton warlockCurseButton;

    // write configs here
    public static float cooldown { get { return CustomOptionHolder.warlockCooldown.GetFloat(); } }
    public static float rootTime { get { return CustomOptionHolder.warlockRootTime.GetFloat(); } }

    public static PlayerControl currentTarget;
    public static PlayerControl curseVictim;
    public static PlayerControl curseVictimTarget;

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
        if (!CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Warlock)) return;
        if (curseVictim != null && (curseVictim.Data.Disconnected || curseVictim.Data.IsDead))
        {
            // If the cursed victim is disconnected or dead reset the curse so a new curse can be applied
            resetCurse();
        }
        if (curseVictim == null)
        {
            currentTarget = Helpers.SetTarget();
            Helpers.SetPlayerOutline(currentTarget, RoleColor);
        }
        else
        {
            curseVictimTarget = Helpers.SetTarget(targetingPlayer: curseVictim);
            Helpers.SetPlayerOutline(curseVictimTarget, RoleColor);
        }
    }
    public override void OnKill(PlayerControl target)
    {
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Warlock) && warlockCurseButton != null)
        {
            if (Player.killTimer > warlockCurseButton.Timer)
            {
                warlockCurseButton.Timer = Player.killTimer;
            }
        }
    }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void MakeButtons(HudManager hm)
    {
        warlockCurseButton = new CustomButton(
                () =>
                {
                    if (curseVictim == null)
                    {
                        // Apply Curse
                        curseVictim = currentTarget;
                        warlockCurseButton.Sprite = AssetLoader.CurseKillButton;
                        warlockCurseButton.Timer = 1f;
                        warlockCurseButton.ButtonText = Tr.Get("CurseKillText");
                    }
                    else if (curseVictim != null && curseVictimTarget != null)
                    {
                        MurderAttemptResult murder = Helpers.CheckMurderAttemptAndKill(Player, curseVictimTarget, showAnimation: false);
                        if (murder == MurderAttemptResult.SuppressKill) return;

                        // If blanked or killed
                        warlockCurseButton.ButtonText = Tr.Get("CurseText");
                        if (rootTime > 0)
                        {
                            CachedPlayer.LocalPlayer.PlayerControl.moveable = false;
                            CachedPlayer.LocalPlayer.PlayerControl.NetTransform.Halt(); // Stop current movement so the warlock is not just running straight into the next object
                            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(rootTime, new Action<float>((p) =>
                            { // Delayed action
                                if (p == 1f)
                                {
                                    CachedPlayer.LocalPlayer.PlayerControl.moveable = true;
                                }
                            })));
                        }

                        curseVictim = null;
                        curseVictimTarget = null;
                        warlockCurseButton.Sprite = AssetLoader.CurseButton;
                        Player.killTimer = warlockCurseButton.Timer = warlockCurseButton.MaxTimer;
                    }
                },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Warlock) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
                () => { return ((curseVictim == null && currentTarget != null) || (curseVictim != null && curseVictimTarget != null)) && CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
                () =>
                {
                    warlockCurseButton.Timer = warlockCurseButton.MaxTimer;
                    warlockCurseButton.Sprite = AssetLoader.CurseButton;
                    warlockCurseButton.ButtonText = Tr.Get("CurseText");
                    curseVictim = null;
                    curseVictimTarget = null;
                },
                AssetLoader.CurseButton,
                new Vector3(-1.8f, -0.06f, 0),
                hm,
                hm.KillButton,
                KeyCode.F
            )
        {
            ButtonText = Tr.Get("CurseText")
        };
    }
    public override void SetButtonCooldowns()
    {
        warlockCurseButton.MaxTimer = cooldown;
    }

    // write functions here
    public static void resetCurse()
    {
        warlockCurseButton.Timer = warlockCurseButton.MaxTimer;
        warlockCurseButton.Sprite = AssetLoader.CurseButton;
        warlockCurseButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
        currentTarget = null;
        curseVictim = null;
        curseVictimTarget = null;
    }

    public override void Clear()
    {
        // reset configs here
        Players.Clear();
        currentTarget = null;
        curseVictim = null;
        curseVictimTarget = null;
    }
}
