using Object = UnityEngine.Object;

namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
internal class BountyHunter : RoleBase<BountyHunter>
{
    internal static Color NameColor = Palette.ImpostorRed;

    internal static Arrow Arrow;
    internal static PlayerControl Bounty;
    internal static TextMeshPro CooldownText;

    private static int _lastSecond = -1;
    internal float ArrowUpdateTimer;
    internal float BountyUpdateTimer;

    public BountyHunter()
    {
        // write value init here
        ArrowUpdateTimer = 0f;
        BountyUpdateTimer = 0f;
        StaticRoleType = CurrentRoleType = RoleType.BountyHunter;
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    internal static int BountyDuration { get => (int)CustomOptionHolder.BountyHunterBountyDuration.GetFloat(); }
    internal static int ReducedCooldown { get => (int)CustomOptionHolder.BountyHunterReducedCooldown.GetFloat(); }
    internal static int PunishmentTime { get => (int)CustomOptionHolder.BountyHunterPunishmentTime.GetFloat(); }
    internal static bool ShowArrow { get => CustomOptionHolder.BountyHunterShowArrow.GetBool(); }
    internal static int ArrowUpdateInterval { get => (int)CustomOptionHolder.BountyHunterArrowUpdateInterval.GetFloat(); }

    internal override void OnMeetingStart() { }

    internal override void OnMeetingEnd()
    {
        if (PlayerControl.LocalPlayer.IsRole(RoleType.BountyHunter)) BountyUpdateTimer = 0f;
    }

    internal override void OnIntroEnd()
    {
        if (PlayerControl.LocalPlayer.IsRole(RoleType.BountyHunter))
        {
            BountyUpdateTimer = 0f;
            if (FastDestroyableSingleton<HudManager>.Instance != null)
            {
                CooldownText = Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText, FastDestroyableSingleton<HudManager>.Instance.transform);
                CooldownText.alignment = TextAlignmentOptions.Center;
                Vector3 bottomLeft = AspectPosition.ComputePosition(AspectPosition.EdgeAlignments.LeftBottom, new(0.9f, 0.7f, -10f));
                CooldownText.transform.localPosition = bottomLeft + new Vector3(0f, -0.35f, -0.1f);
                CooldownText.transform.localScale = Vector3.one * 0.4f;
                CooldownText.gameObject.SetActive(true);
                CooldownText.gameObject.layer = 5;
            }
        }
    }

    internal override void FixedUpdate()
    {
        if (!PlayerControl.LocalPlayer.IsRole(RoleType.BountyHunter)) return;

        if (Local != null)
        {
            if (Player.IsDead())
            {
                if (Arrow != null && Arrow.ArrowObject != null) Object.Destroy(Arrow.ArrowObject);
                Arrow = null;
                if (CooldownText != null && CooldownText.gameObject != null) Object.Destroy(CooldownText.gameObject);
                CooldownText = null;
                Bounty = null;
                _lastSecond = -1;
                foreach (PoolablePlayer p in MapSettings.PlayerIcons.Values)
                    if (p != null && p.gameObject != null)
                        p.gameObject.SetActive(false);

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
                List<PlayerControl> possibleTargets = new();
                PlayerControl partner = Player.GetPartner();

                foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                {
                    if (p == null || p.Data == null) continue;

                    if (!p.Data.IsDead && !p.Data.Disconnected && !p.Data.Role.IsImpostor && !p.IsRole(RoleType.Spy) && (!p.IsRole(RoleType.Sidekick) || !Sidekick.GetRole(p).WasTeamRed) && (!p.IsRole(RoleType.Jackal) || !Jackal.GetRole(p).WasTeamRed) && !(p.HasModifier(ModifierType.Mini) && !Mini.IsGrownUp(p)) && !p.IsGm() && partner != p) possibleTargets.Add(p);
                }

                if (possibleTargets.Count > 0) Bounty = possibleTargets[RebuildUs.Rnd.Next(0, possibleTargets.Count)];

                if (Bounty == null) return;

                // Show poolable player
                if (FastDestroyableSingleton<HudManager>.Instance != null)
                {
                    foreach (PoolablePlayer pp in MapSettings.PlayerIcons.Values)
                        if (pp != null && pp.gameObject != null)
                            pp.gameObject.SetActive(false);

                    if (MapSettings.PlayerIcons.TryGetValue(Bounty.PlayerId, out PoolablePlayer icon) && icon != null && icon.gameObject != null)
                    {
                        Vector3 bottomLeft = AspectPosition.ComputePosition(AspectPosition.EdgeAlignments.LeftBottom, new(0.9f, 0.7f, -10f));
                        icon.transform.localPosition = bottomLeft;
                        icon.transform.localScale = Vector3.one * 0.3f;
                        icon.gameObject.SetActive(true);
                    }
                }
            }

            // Hide in meeting
            if (MeetingHud.Instance && MapSettings.PlayerIcons.TryGetValue(Bounty.PlayerId, out PoolablePlayer mIcon) && mIcon != null && mIcon.gameObject != null) mIcon.gameObject.SetActive(false);

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
                Arrow ??= new(RoleColor);
                if (ArrowUpdateTimer <= 0f)
                {
                    Arrow.Update(Bounty.transform.position);
                    ArrowUpdateTimer = ArrowUpdateInterval;
                }

                Arrow.Update();
            }
        }
    }

    internal override void OnKill(PlayerControl target)
    {
        if (PlayerControl.LocalPlayer.IsRole(RoleType.BountyHunter))
        {
            if (target == Bounty)
            {
                Player.SetKillTimer(ReducedCooldown);
                BountyUpdateTimer = 0f; // Force bounty update
            }
            else
                Player.SetKillTimer(Helpers.GetOption(FloatOptionNames.KillCooldown) + PunishmentTime);
        }
    }

    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    // write functions here

    internal static void Clear()
    {
        // reset configs here
        Players.Clear();

        Bounty = null;
        if (Arrow != null && Arrow.ArrowObject != null) Object.Destroy(Arrow.ArrowObject);
        Arrow = null;
        if (CooldownText != null && CooldownText.gameObject != null) Object.Destroy(CooldownText.gameObject);
        CooldownText = null;
        foreach (PoolablePlayer p in MapSettings.PlayerIcons.Values)
            if (p != null && p.gameObject != null)
                p.gameObject.SetActive(false);
    }
}