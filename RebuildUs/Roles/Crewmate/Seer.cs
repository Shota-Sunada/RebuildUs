namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
[RegisterRole(RoleType.Seer, RoleTeam.Crewmate, typeof(MultiRoleBase<Seer>), nameof(CustomOptionHolder.SeerSpawnRate))]
internal class Seer : MultiRoleBase<Seer>
{
    public static Color Color = new Color32(97, 178, 108, byte.MaxValue);

    internal static List<Vector3> DeadBodyPositions = [];

    public Seer()
    {
        StaticRoleType = CurrentRoleType = RoleType.Seer;
    }

    internal static int Mode { get => CustomOptionHolder.SeerMode.GetSelection(); }
    internal static bool LimitSoulDuration { get => CustomOptionHolder.SeerLimitSoulDuration.GetBool(); }
    internal static float SoulDuration { get => CustomOptionHolder.SeerSoulDuration.GetFloat(); }

    [CustomEvent(CustomEventType.OnMeetingEnd)]
    internal void OnMeetingEnd()
    {
        if (DeadBodyPositions != null && PlayerControl.LocalPlayer.IsRole(RoleType.Seer) && Mode is 0 or 2)
        {
            for (var i = 0; i < DeadBodyPositions.Count; i++)
            {
                var pos = DeadBodyPositions[i];
                GameObject soul = new("Soul");
                soul.transform.position = new(pos.x, pos.y, pos.y / 1000f - 1f);
                soul.layer = 5;
                var rend = soul.AddComponent<SpriteRenderer>();
                soul.AddSubmergedComponent(SubmergedCompatibility.Classes.ELEVATOR_MOVER);
                rend.sprite = AssetLoader.Soul;

                if (LimitSoulDuration)
                {
                    FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(SoulDuration,
                        new Action<float>(p =>
                        {
                            if (rend != null)
                            {
                                var tmp = rend.color;
                                tmp.a = Mathf.Clamp01(1 - p);
                                rend.color = tmp;
                            }

                            if (p == 1f && rend != null && rend.gameObject != null)
                            {
                                UnityObject.Destroy(rend.gameObject);
                            }
                        })));
                }
            }

            DeadBodyPositions.Clear();
        }
    }

    internal static void Clear()
    {
        Players.Clear();
        DeadBodyPositions = [];
    }
}