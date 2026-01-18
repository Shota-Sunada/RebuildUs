using AmongUs.GameOptions;
using RebuildUs.Objects;
using RebuildUs.Roles.Neutral;

namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
public class BountyHunter : RoleBase<BountyHunter>
{
    public static Color NameColor = Palette.ImpostorRed;
    public override Color RoleColor => NameColor;
    public Arrow Arrow;
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
        Arrow = new Arrow(RoleColor);
        ArrowUpdateTimer = 0f;
        BountyUpdateTimer = 0f;
        StaticRoleType = CurrentRoleType = RoleType.BountyHunter;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd()
    {
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.BountyHunter))
        {
            BountyUpdateTimer = 0f;
        }
    }
    public override void OnIntroEnd()
    {
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.BountyHunter))
        {
            BountyUpdateTimer = 0f;
            if (FastDestroyableSingleton<HudManager>.Instance != null)
            {
                var bottomLeft = new Vector3(-FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.x, FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.y, FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.z) + new Vector3(-0.25f, 1f, 0);
                CooldownText = UnityEngine.Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText, FastDestroyableSingleton<HudManager>.Instance.transform);
                CooldownText.alignment = TextAlignmentOptions.Center;
                CooldownText.transform.localPosition = bottomLeft + new Vector3(0f, -1f, -1f);
                CooldownText.gameObject.SetActive(true);
            }
        }
    }
    public override void FixedUpdate()
    {
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.BountyHunter))
        {
            if (Player.IsDead())
            {
                if (Arrow != null || Arrow.ArrowObject != null) UnityEngine.Object.Destroy(Arrow.ArrowObject);
                Arrow = null;
                if (CooldownText != null && CooldownText.gameObject != null) UnityEngine.Object.Destroy(CooldownText.gameObject);
                CooldownText = null;
                Bounty = null;
                foreach (PoolablePlayer p in ModMapOptions.PlayerIcons.Values)
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
                foreach (var p in CachedPlayer.AllPlayers)
                {
                    if (!p.Data.IsDead && !p.Data.Disconnected && !p.Data.Role.IsImpostor && !p.PlayerControl.IsRole(RoleType.Spy)
                    && (!p.PlayerControl.IsRole(RoleType.Sidekick) || !Sidekick.GetRole(p).WasTeamRed)
                    && (!p.PlayerControl.IsRole(RoleType.Jackal) || !Jackal.GetRole(p).WasTeamRed)
                    // && !(p.hasModifier(ModifierType.Mini) && !Mini.isGrownUp(p))
                    && !p.PlayerControl.IsGM()
                    // && Player.GetPartner() != p
                    )
                    {
                        possibleTargets.Add(p);
                    }
                }
                Bounty = possibleTargets[RebuildUs.Instance.Rnd.Next(0, possibleTargets.Count)];
                if (Bounty == null) return;

                // Show poolable player
                if (FastDestroyableSingleton<HudManager>.Instance != null && FastDestroyableSingleton<HudManager>.Instance.UseButton != null)
                {
                    foreach (var pp in ModMapOptions.PlayerIcons.Values)
                    {
                        pp.gameObject.SetActive(false);
                    }
                    if (ModMapOptions.PlayerIcons.ContainsKey(Bounty.PlayerId) && ModMapOptions.PlayerIcons[Bounty.PlayerId].gameObject != null)
                    {
                        ModMapOptions.PlayerIcons[Bounty.PlayerId].gameObject.SetActive(true);
                    }
                }
            }

            // Hide in meeting
            if (MeetingHud.Instance && ModMapOptions.PlayerIcons.ContainsKey(Bounty.PlayerId) && ModMapOptions.PlayerIcons[Bounty.PlayerId].gameObject != null)
            {
                ModMapOptions.PlayerIcons[Bounty.PlayerId].gameObject.SetActive(false);
            }

            // Update Cooldown Text
            CooldownText?.text = Mathf.CeilToInt(Mathf.Clamp(BountyUpdateTimer, 0, BountyDuration)).ToString();

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
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.BountyHunter))
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

    public override void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}
