using Object = UnityEngine.Object;

namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
internal class Seer : MultiRoleBase<Seer>
{
    internal static Color NameColor = new Color32(97, 178, 108, byte.MaxValue);

    internal static List<Vector3> DeadBodyPositions = [];

    public Seer()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Seer;
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    internal static int Mode { get => CustomOptionHolder.SeerMode.GetSelection(); }
    internal static bool LimitSoulDuration { get => CustomOptionHolder.SeerLimitSoulDuration.GetBool(); }
    internal static float SoulDuration { get => CustomOptionHolder.SeerSoulDuration.GetFloat(); }

    internal override void OnMeetingStart() { }

    internal override void OnMeetingEnd()
    {
        if (DeadBodyPositions != null && PlayerControl.LocalPlayer.IsRole(RoleType.Seer) && Mode is 0 or 2)
        {
            for (int i = 0; i < DeadBodyPositions.Count; i++)
            {
                Vector3 pos = DeadBodyPositions[i];
                GameObject soul = new("Soul");
                soul.transform.position = new(pos.x, pos.y, (pos.y / 1000f) - 1f);
                soul.layer = 5;
                SpriteRenderer rend = soul.AddComponent<SpriteRenderer>();
                soul.AddSubmergedComponent(SubmergedCompatibility.Classes.ELEVATOR_MOVER);
                rend.sprite = AssetLoader.Soul;

                if (LimitSoulDuration)
                {
                    FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(SoulDuration, new Action<float>(p =>
                    {
                        if (rend != null)
                        {
                            Color tmp = rend.color;
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

    internal override void OnIntroEnd() { }
    internal override void FixedUpdate() { }
    internal override void OnKill(PlayerControl target) { }
    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    // write functions here

    internal static void Clear()
    {
        // reset configs here
        Players.Clear();
        DeadBodyPositions = [];
    }
}