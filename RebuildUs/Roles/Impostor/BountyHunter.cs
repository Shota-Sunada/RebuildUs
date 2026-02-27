namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
internal class BountyHunter : SingleRoleBase<BountyHunter>
{
    internal static Color NameColor = Palette.ImpostorRed;

    private static Arrow _arrow;
    internal static PlayerControl Bounty;
    internal static TextMeshPro CooldownText;

    private static int _lastSecond = -1;
    private float _arrowUpdateTimer;
    private float _bountyUpdateTimer;

    public BountyHunter()
    {
        // write value init here
        _arrowUpdateTimer = 0f;
        _bountyUpdateTimer = 0f;
        StaticRoleType = CurrentRoleType = RoleType.BountyHunter;
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    private static int BountyDuration
    {
        get => (int)CustomOptionHolder.BountyHunterBountyDuration.GetFloat();
    }

    private static int ReducedCooldown
    {
        get => (int)CustomOptionHolder.BountyHunterReducedCooldown.GetFloat();
    }

    internal static int PunishmentTime
    {
        get => (int)CustomOptionHolder.BountyHunterPunishmentTime.GetFloat();
    }

    private static bool ShowArrow
    {
        get => CustomOptionHolder.BountyHunterShowArrow.GetBool();
    }

    private static int ArrowUpdateInterval
    {
        get => (int)CustomOptionHolder.BountyHunterArrowUpdateInterval.GetFloat();
    }

    internal override void OnMeetingStart() { }

    internal override void OnMeetingEnd()
    {
        if (PlayerControl.LocalPlayer.IsRole(RoleType.BountyHunter))
        {
            _bountyUpdateTimer = 0f;
        }
    }

    internal override void OnIntroEnd()
    {
        if (!PlayerControl.LocalPlayer.IsRole(RoleType.BountyHunter))
        {
            return;
        }
        _bountyUpdateTimer = 0f;
        if (FastDestroyableSingleton<HudManager>.Instance == null)
        {
            return;
        }
        CooldownText = UnityObject.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText,
            FastDestroyableSingleton<HudManager>.Instance.transform);
        CooldownText.alignment = TextAlignmentOptions.Center;
        Vector3 bottomLeft = AspectPosition.ComputePosition(AspectPosition.EdgeAlignments.LeftBottom, new(0.9f, 0.7f, -10f));
        CooldownText.transform.localPosition = bottomLeft + new Vector3(0f, -0.35f, -0.1f);
        CooldownText.transform.localScale = Vector3.one * 0.4f;
        CooldownText.gameObject.SetActive(true);
        CooldownText.gameObject.layer = 5;
    }

    internal override void FixedUpdate()
    {
        if (!PlayerControl.LocalPlayer.IsRole(RoleType.BountyHunter))
        {
            return;
        }

        if (Local == null)
        {
            return;
        }
        if (Player.IsDead())
        {
            if (_arrow != null && _arrow.ArrowObject != null)
            {
                UnityObject.Destroy(_arrow.ArrowObject);
            }
            _arrow = null;
            if (CooldownText != null && CooldownText.gameObject != null)
            {
                UnityObject.Destroy(CooldownText.gameObject);
            }
            CooldownText = null;
            Bounty = null;
            _lastSecond = -1;
            foreach (PoolablePlayer p in MapSettings.PlayerIcons.Values)
            {
                if (p != null && p.gameObject != null)
                {
                    p.gameObject.SetActive(false);
                }
            }

            return;
        }

        _arrowUpdateTimer -= Time.fixedDeltaTime;
        _bountyUpdateTimer -= Time.fixedDeltaTime;

        if (Bounty == null || _bountyUpdateTimer <= 0f)
        {
            // Set new bounty
            Bounty = null;
            _arrowUpdateTimer = 0f; // Force arrow to update
            _bountyUpdateTimer = BountyDuration;
            List<PlayerControl> possibleTargets = [];
            PlayerControl partner = Player.GetPartner();

            foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (p == null || p.Data == null)
                {
                    continue;
                }

                if (!p.Data.IsDead
                    && !p.Data.Disconnected
                    && !p.Data.Role.IsImpostor
                    && !p.IsRole(RoleType.Spy)
                    && (!p.IsRole(RoleType.Sidekick) || !Sidekick.Instance.WasTeamRed)
                    && (!p.IsRole(RoleType.Jackal) || !Jackal.Instance.WasTeamRed)
                    && !(p.HasModifier(ModifierType.Mini) && !Mini.IsGrownUp(p))
                    && !p.IsGm()
                    && partner != p)
                {
                    possibleTargets.Add(p);
                }
            }

            if (possibleTargets.Count > 0)
            {
                Bounty = possibleTargets[RebuildUs.Rnd.Next(0, possibleTargets.Count)];
            }

            if (Bounty == null)
            {
                return;
            }

            // Show poolable player
            if (FastDestroyableSingleton<HudManager>.Instance != null)
            {
                foreach (PoolablePlayer pp in MapSettings.PlayerIcons.Values)
                {
                    if (pp != null && pp.gameObject != null)
                    {
                        pp.gameObject.SetActive(false);
                    }
                }

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
        if (MeetingHud.Instance
            && MapSettings.PlayerIcons.TryGetValue(Bounty.PlayerId, out PoolablePlayer mIcon)
            && mIcon != null
            && mIcon.gameObject != null)
        {
            mIcon.gameObject.SetActive(false);
        }

        // Update Cooldown Text
        if (CooldownText != null)
        {
            int currentSecond = Mathf.CeilToInt(Mathf.Clamp(_bountyUpdateTimer, 0, BountyDuration));
            if (currentSecond != _lastSecond)
            {
                _lastSecond = currentSecond;
                CooldownText.text = currentSecond.ToString();
            }
        }

        // Update Arrow
        if (!ShowArrow || Bounty == null)
        {
            return;
        }
        _arrow ??= new(RoleColor);
        if (_arrowUpdateTimer <= 0f)
        {
            _arrow.Update(Bounty.transform.position);
            _arrowUpdateTimer = ArrowUpdateInterval;
        }

        _arrow.Update();
    }

    internal override void OnKill(PlayerControl target)
    {
        if (!PlayerControl.LocalPlayer.IsRole(RoleType.BountyHunter))
        {
            return;
        }
        if (target == Bounty)
        {
            Player.SetKillTimer(ReducedCooldown);
            _bountyUpdateTimer = 0f; // Force bounty update
        }
        else
        {
            Player.SetKillTimer(Helpers.GetOption(FloatOptionNames.KillCooldown) + PunishmentTime);
        }
    }

    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    internal static void Clear()
    {
        ModRoleManager.RemoveRole(Instance);
        Instance = null;

        Bounty = null;
        _arrow = null;
        if (CooldownText != null && CooldownText.gameObject != null)
        {
            UnityObject.Destroy(CooldownText.gameObject);
        }
        CooldownText = null;
        foreach (PoolablePlayer p in MapSettings.PlayerIcons.Values)
        {
            if (p != null && p.gameObject != null)
            {
                p.gameObject.SetActive(false);
            }
        }
    }
}