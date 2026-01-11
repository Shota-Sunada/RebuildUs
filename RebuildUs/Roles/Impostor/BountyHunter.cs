using AmongUs.GameOptions;
using RebuildUs.Objects;
using RebuildUs.Roles.Neutral;

namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
public class BountyHunter : RoleBase<BountyHunter>
{
    public static Color RoleColor = Palette.ImpostorRed;
    public Arrow arrow;
    public PlayerControl bounty;
    public TextMeshPro cooldownText;
    public float arrowUpdateTimer = 0f;
    public float bountyUpdateTimer = 0f;

    // write configs here
    public static int bountyDuration { get { return (int)CustomOptionHolder.bountyHunterBountyDuration.GetFloat(); } }
    public static int reducedCooldown { get { return (int)CustomOptionHolder.bountyHunterReducedCooldown.GetFloat(); } }
    public static int punishmentTime { get { return (int)CustomOptionHolder.bountyHunterPunishmentTime.GetFloat(); } }
    public static bool showArrow { get { return CustomOptionHolder.bountyHunterShowArrow.GetBool(); } }
    public static int arrowUpdateInterval { get { return (int)CustomOptionHolder.bountyHunterArrowUpdateInterval.GetFloat(); } }

    public BountyHunter()
    {
        // write value init here
        arrow = new Arrow(RoleColor);
        arrowUpdateTimer = 0f;
        bountyUpdateTimer = 0f;
        StaticRoleType = CurrentRoleType = ERoleType.BountyHunter;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd()
    {
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.BountyHunter))
        {
            bountyUpdateTimer = 0f;
        }
    }
    public override void OnIntroEnd()
    {
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.BountyHunter))
        {
            bountyUpdateTimer = 0f;
            if (FastDestroyableSingleton<HudManager>.Instance != null)
            {
                var bottomLeft = new Vector3(-FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.x, FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.y, FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.z) + new Vector3(-0.25f, 1f, 0);
                cooldownText = UnityEngine.Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText, FastDestroyableSingleton<HudManager>.Instance.transform);
                cooldownText.alignment = TextAlignmentOptions.Center;
                cooldownText.transform.localPosition = bottomLeft + new Vector3(0f, -1f, -1f);
                cooldownText.gameObject.SetActive(true);
            }
        }
    }
    public override void FixedUpdate()
    {
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.BountyHunter))
        {
            if (Player.IsDead())
            {
                if (arrow != null || arrow.arrow != null) UnityEngine.Object.Destroy(arrow.arrow);
                arrow = null;
                if (cooldownText != null && cooldownText.gameObject != null) UnityEngine.Object.Destroy(cooldownText.gameObject);
                cooldownText = null;
                bounty = null;
                foreach (PoolablePlayer p in MapOptions.PlayerIcons.Values)
                {
                    if (p != null && p.gameObject != null) p.gameObject.SetActive(false);
                }
                return;
            }

            arrowUpdateTimer -= Time.fixedDeltaTime;
            bountyUpdateTimer -= Time.fixedDeltaTime;

            if (bounty == null || bountyUpdateTimer <= 0f)
            {
                // Set new bounty
                bounty = null;
                arrowUpdateTimer = 0f; // Force arrow to update
                bountyUpdateTimer = bountyDuration;
                var possibleTargets = new List<PlayerControl>();
                foreach (var p in CachedPlayer.AllPlayers)
                {
                    if (!p.Data.IsDead && !p.Data.Disconnected && !p.Data.Role.IsImpostor && !p.PlayerControl.IsRole(ERoleType.Spy)
                    && (!p.PlayerControl.IsRole(ERoleType.Sidekick) || !Sidekick.GetRole(p).wasTeamRed)
                    && (!p.PlayerControl.IsRole(ERoleType.Jackal) || !Jackal.GetRole(p).wasTeamRed)
                    // && !(p.hasModifier(ModifierType.Mini) && !Mini.isGrownUp(p))
                    && !p.PlayerControl.IsGM()
                    // && Player.GetPartner() != p
                    )
                    {
                        possibleTargets.Add(p);
                    }
                }
                bounty = possibleTargets[RebuildUs.Instance.rnd.Next(0, possibleTargets.Count)];
                if (bounty == null) return;

                // Show poolable player
                if (FastDestroyableSingleton<HudManager>.Instance != null && FastDestroyableSingleton<HudManager>.Instance.UseButton != null)
                {
                    foreach (var pp in MapOptions.PlayerIcons.Values)
                    {
                        pp.gameObject.SetActive(false);
                    }
                    if (MapOptions.PlayerIcons.ContainsKey(bounty.PlayerId) && MapOptions.PlayerIcons[bounty.PlayerId].gameObject != null)
                    {
                        MapOptions.PlayerIcons[bounty.PlayerId].gameObject.SetActive(true);
                    }
                }
            }

            // Hide in meeting
            if (MeetingHud.Instance && MapOptions.PlayerIcons.ContainsKey(bounty.PlayerId) && MapOptions.PlayerIcons[bounty.PlayerId].gameObject != null)
            {
                MapOptions.PlayerIcons[bounty.PlayerId].gameObject.SetActive(false);
            }

            // Update Cooldown Text
            cooldownText?.text = Mathf.CeilToInt(Mathf.Clamp(bountyUpdateTimer, 0, bountyDuration)).ToString();

            // Update Arrow
            if (showArrow && bounty != null)
            {
                arrow ??= new Arrow(RoleColor);
                if (arrowUpdateTimer <= 0f)
                {
                    arrow.Update(bounty.transform.position);
                    arrowUpdateTimer = arrowUpdateInterval;
                }
                arrow.Update();
            }
        }
    }

    public override void OnKill(PlayerControl target)
    {
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.BountyHunter))
        {
            if (target == bounty)
            {
                Player.SetKillTimer(reducedCooldown);
                bountyUpdateTimer = 0f; // Force bounty update
            }
            else
            {
                Player.SetKillTimer(GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.KillCooldown) + punishmentTime);
            }
        }
    }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    public static void MakeButtons(HudManager hm) { }
    public static void SetButtonCooldowns() { }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}