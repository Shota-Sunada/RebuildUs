namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Seer : RoleBase<Seer>
{
    public static Color NameColor = new Color32(97, 178, 108, byte.MaxValue);
    public override Color RoleColor => NameColor;
    public static List<Vector3> DeadBodyPositions = [];

    // write configs here
    public static int Mode { get { return CustomOptionHolder.SeerMode.GetSelection(); } }
    public static bool LimitSoulDuration { get { return CustomOptionHolder.SeerLimitSoulDuration.GetBool(); } }
    public static float SoulDuration { get { return CustomOptionHolder.SeerSoulDuration.GetFloat(); } }

    public Seer()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Seer;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd()
    {
        if (DeadBodyPositions != null && CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Seer) && (Mode is 0 or 2))
        {
            foreach (var pos in DeadBodyPositions)
            {
                var soul = new GameObject();
                // soul.transform.position = pos;
                soul.transform.position = new Vector3(pos.x, pos.y, pos.y / 1000 - 1f);
                soul.layer = 5;
                var rend = soul.AddComponent<SpriteRenderer>();
                soul.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
                rend.sprite = AssetLoader.Soul;

                if (LimitSoulDuration)
                {
                    FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(SoulDuration, new Action<float>((p) =>
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
            DeadBodyPositions = [];
        }
    }
    public override void OnIntroEnd() { }
    public override void FixedUpdate() { }
    public override void OnKill(PlayerControl target) { }
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
        DeadBodyPositions = [];
    }
}