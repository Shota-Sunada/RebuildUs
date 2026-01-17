namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Seer : RoleBase<Seer>
{
    public static Color RoleColor = new Color32(97, 178, 108, byte.MaxValue);
    public static List<Vector3> deadBodyPositions = [];

    // write configs here
    public static int mode { get { return CustomOptionHolder.seerMode.GetSelection(); } }
    public static bool limitSoulDuration { get { return CustomOptionHolder.seerLimitSoulDuration.GetBool(); } }
    public static float soulDuration { get { return CustomOptionHolder.seerSoulDuration.GetFloat(); } }

    public Seer()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Seer;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd()
    {
        if (deadBodyPositions != null && CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Seer) && (mode is 0 or 2))
        {
            foreach (var pos in deadBodyPositions)
            {
                var soul = new GameObject();
                // soul.transform.position = pos;
                soul.transform.position = new Vector3(pos.x, pos.y, pos.y / 1000 - 1f);
                soul.layer = 5;
                var rend = soul.AddComponent<SpriteRenderer>();
                soul.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
                rend.sprite = Seer.getSoulSprite();

                if (limitSoulDuration)
                {
                    FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(soulDuration, new Action<float>((p) =>
                    {
                        if (rend != null)
                        {
                            var tmp = rend.color;
                            tmp.a = Mathf.Clamp01(1 - p);
                            rend.color = tmp;
                        }
                        if (p == 1f && rend != null && rend.gameObject != null) UnityEngine.Object.Destroy(rend.gameObject);
                    })));
                }
            }
            deadBodyPositions = [];
        }
    }
    public override void OnIntroEnd() { }
    public override void FixedUpdate() { }
    public override void OnKill(PlayerControl target) { }
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
        deadBodyPositions = [];
    }
}