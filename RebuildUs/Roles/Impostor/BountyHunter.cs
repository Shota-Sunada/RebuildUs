namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
public class BountyHunter : RoleBase<BountyHunter>
{
    public static Color NameColor = Palette.ImpostorRed;
    public override Color RoleColor => NameColor;
    public static Arrow Arrow;
    public static PlayerControl Bounty;
    public static TextMeshPro CooldownText;
    public float ArrowUpdateTimer = 0f;
    public float BountyUpdateTimer = 0f;

    // write configs here
    public static int BountyDuration { get { return (int)CustomOptionHolder.BountyHunterBountyDuration.GetFloat(); } }
    public static int ReducedCooldown { get { return (int)CustomOptionHolder.BountyHunterReducedCooldown.GetFloat(); } }
    public static int PunishmentTime { get { return (int)CustomOptionHolder.BountyHunterPunishmentTime.GetFloat(); } }
    public static bool ShowArrow { get { return CustomOptionHolder.BountyHunterShowArrow.GetBool(); } }
    public static int ArrowUpdateInterval { get { return (int)CustomOptionHolder.BountyHunterArrowUpdateInterval.GetFloat(); } }

    public BountyHunter()
    {
        // write value init here
        ArrowUpdateTimer = 0f;
        BountyUpdateTimer = 0f;
        StaticRoleType = CurrentRoleType = RoleType.BountyHunter;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd()
    {
        if (PlayerControl.LocalPlayer.IsRole(RoleType.BountyHunter))
        {
            BountyUpdateTimer = 0f;
        }
    }
    public override void OnIntroEnd()
    {
        if (PlayerControl.LocalPlayer.IsRole(RoleType.BountyHunter))
        {
            BountyUpdateTimer = 0f;
            if (FastDestroyableSingleton<HudManager>.Instance != null)
            {
                CooldownText = UnityEngine.Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText, FastDestroyableSingleton<HudManager>.Instance.transform);
                CooldownText.alignment = TextAlignmentOptions.Center;
                CooldownText.transform.localPosition = Intro.BottomLeft + new Vector3(0f, -0.35f, -62f);
                CooldownText.transform.localScale = Vector3.one * 0.4f;
                CooldownText.gameObject.SetActive(true);
            }
        }
    }
    private static int _lastSecond = -1;

    public override void FixedUpdate()
    {
        if (!PlayerControl.LocalPlayer.IsRole(RoleType.BountyHunter)) return;

        if (Local != null)
        {
            if (Player.IsDead())
            {
                if (Arrow != null && Arrow.ArrowObject != null) UnityEngine.Object.Destroy(Arrow.ArrowObject);
                Arrow = null;
                if (CooldownText != null && CooldownText.gameObject != null) UnityEngine.Object.Destroy(CooldownText.gameObject);
                CooldownText = null;
                Bounty = null;
                _lastSecond = -1;
                foreach (var p in ModMapOptions.PlayerIcons.Values)
                {
                    if (p != null && p.gameObject != null) p.gameObject.SetActive(false);
                }
                return;
            }

            ArrowUpdateTimer -= Time.fixedDeltaTime;
            BountyUpdateTimer -= Time.fixedDeltaTime;

            if (Bounty == null || BountyUpdateTimer <= 0f)
            {
                // Set new bounty
                Bounty = null;
                ArrowUpdateTimer = 0f; // Force arrow to update
                BountyUpdateTimer = BountyDuration;
                var possibleTargets = new List<PlayerControl>();
                var players = PlayerControl.AllPlayerControls;
                var partner = Player.GetPartner();

                for (int i = 0; i < players.Count; i++)
                {
                    var p = players[i];
                    if (p == null || p.Data == null) continue;

                    if (!p.Data.IsDead && !p.Data.Disconnected && !p.Data.Role.IsImpostor && !p.IsRole(RoleType.Spy)
                    && (!p.IsRole(RoleType.Sidekick) || !Sidekick.GetRole(p).WasTeamRed)
                    && (!p.IsRole(RoleType.Jackal) || !Jackal.GetRole(p).WasTeamRed)
                    && !(p.HasModifier(ModifierType.Mini) && !Mini.IsGrownUp(p))
                    && !p.IsGM()
                    && partner != p
                    )
                    {
                        possibleTargets.Add(p);
                    }
                }

                if (possibleTargets.Count > 0)
                {
                    Bounty = possibleTargets[RebuildUs.Instance.Rnd.Next(0, possibleTargets.Count)];
                }

                if (Bounty == null) return;

                // Show poolable player
                if (FastDestroyableSingleton<HudManager>.Instance != null && FastDestroyableSingleton<HudManager>.Instance.UseButton != null)
                {
                    foreach (var pp in ModMapOptions.PlayerIcons.Values)
                    {
                        if (pp != null && pp.gameObject != null) pp.gameObject.SetActive(false);
                    }
                    if (ModMapOptions.PlayerIcons.TryGetValue(Bounty.PlayerId, out var icon) && icon != null && icon.gameObject != null)
                    {
                        icon.gameObject.SetActive(true);
                    }
                }
            }

            // Hide in meeting
            if (MeetingHud.Instance && ModMapOptions.PlayerIcons.TryGetValue(Bounty.PlayerId, out var mIcon) && mIcon != null && mIcon.gameObject != null)
            {
                mIcon.gameObject.SetActive(false);
            }

            // Update Cooldown Text
            if (CooldownText != null)
            {
                int currentSecond = Mathf.CeilToInt(Mathf.Clamp(BountyUpdateTimer, 0, BountyDuration));
                if (currentSecond != _lastSecond)
                {
                    _lastSecond = currentSecond;
                    CooldownText.text = currentSecond.ToString();
                }
            }

            // Update Arrow
            if (ShowArrow && Bounty != null)
            {
                Arrow ??= new Arrow(RoleColor);
                if (ArrowUpdateTimer <= 0f)
                {
                    Arrow.Update(Bounty.transform.position);
                    ArrowUpdateTimer = ArrowUpdateInterval;
                }
                Arrow.Update();
            }
        }
    }

    public override void OnKill(PlayerControl target)
    {
        if (PlayerControl.LocalPlayer.IsRole(RoleType.BountyHunter))
        {
            if (target == Bounty)
            {
                Player.SetKillTimer(ReducedCooldown);
                BountyUpdateTimer = 0f; // Force bounty update
            }
            else
            {
                Player.SetKillTimer(Helpers.GetOption(FloatOptionNames.KillCooldown) + PunishmentTime);
            }
        }
    }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void MakeButtons(HudManager hm) { }
    public override void SetButtonCooldowns() { }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        Players.Clear();

        Bounty = null;
        if (Arrow != null && Arrow.ArrowObject != null) UnityEngine.Object.Destroy(Arrow.ArrowObject);
        Arrow = null;
        if (CooldownText != null && CooldownText.gameObject != null) UnityEngine.Object.Destroy(CooldownText.gameObject);
        CooldownText = null;
        foreach (var p in ModMapOptions.PlayerIcons.Values)
        {
            if (p != null && p.gameObject != null) p.gameObject.SetActive(false);
        }
    }
}