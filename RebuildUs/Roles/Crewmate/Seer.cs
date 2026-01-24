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
        if (DeadBodyPositions != null && PlayerControl.LocalPlayer.IsRole(RoleType.Seer) && (Mode is 0 or 2))
        {
            for (int i = 0; i < DeadBodyPositions.Count; i++)
            {
                var pos = DeadBodyPositions[i];
                var soul = new GameObject("Soul");
                soul.transform.position = new Vector3(pos.x, pos.y, pos.y / 1000f - 1f);
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
            DeadBodyPositions.Clear();
        }
    }
    public override void OnIntroEnd() { }
    public override void FixedUpdate() { }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
        DeadBodyPositions = [];
    }
}