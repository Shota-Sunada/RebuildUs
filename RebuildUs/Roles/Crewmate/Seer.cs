using Object = UnityEngine.Object;

namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Seer : RoleBase<Seer>
{
    public static Color NameColor = new Color32(97, 178, 108, byte.MaxValue);
    public static List<Vector3> DeadBodyPositions = [];

    public Seer()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Seer;
    }

    public override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    public static int Mode
    {
        get => CustomOptionHolder.SeerMode.GetSelection();
    }

    public static bool LimitSoulDuration
    {
        get => CustomOptionHolder.SeerLimitSoulDuration.GetBool();
    }

    public static float SoulDuration
    {
        get => CustomOptionHolder.SeerSoulDuration.GetFloat();
    }

    public override void OnMeetingStart() { }

    public override void OnMeetingEnd()
    {
        if (DeadBodyPositions != null && PlayerControl.LocalPlayer.IsRole(RoleType.Seer) && Mode is 0 or 2)
        {
            for (var i = 0; i < DeadBodyPositions.Count; i++)
            {
                var pos = DeadBodyPositions[i];
                var soul = new GameObject("Soul");
                soul.transform.position = new(pos.x, pos.y, (pos.y / 1000f) - 1f);
                soul.layer = 5;
                var rend = soul.AddComponent<SpriteRenderer>();
                soul.AddSubmergedComponent(SubmergedCompatibility.ELEVATOR_MOVER);
                rend.sprite = AssetLoader.Soul;

                if (LimitSoulDuration)
                {
                    FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(SoulDuration, new Action<float>(p =>
                    {
                        if (rend != null)
                        {
                            var tmp = rend.color;
                            tmp.a = Mathf.Clamp01(1 - p);
                            rend.color = tmp;
                        }

                        if (p == 1f && rend != null && rend.gameObject != null) Object.Destroy(rend.gameObject);
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
