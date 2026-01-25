namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Medium : RoleBase<Medium>
{
    public static Color NameColor = new Color32(98, 120, 115, byte.MaxValue);
    public override Color RoleColor => NameColor;
    public static CustomButton MediumButton;
    public static List<(DeadPlayer deadPlayer, Vector3 pos)> DeadBodies = [];
    public static List<(DeadPlayer deadPlayer, Vector3 pos)> FeatureDeadBodies = [];
    public static List<SpriteRenderer> Souls = [];

    // write configs here
    public static float Cooldown { get { return CustomOptionHolder.MediumCooldown.GetFloat(); } }
    public static float Duration { get { return CustomOptionHolder.MediumDuration.GetFloat(); } }
    public static bool OneTimeUse { get { return CustomOptionHolder.MediumOneTimeUse.GetBool(); } }

    public DeadPlayer Target;
    public DeadPlayer SoulTarget;
    public static DateTime MeetingStartTime = DateTime.UtcNow;

    public Medium()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Medium;
    }

    public override void OnMeetingStart()
    {
        MeetingStartTime = DateTime.UtcNow;
    }
    public override void OnMeetingEnd()
    {
        if (PlayerControl.LocalPlayer.IsRole(RoleType.Medium))
        {
            if (Souls != null)
            {
                for (int i = 0; i < Souls.Count; i++)
                {
                    if (Souls[i] != null && Souls[i].gameObject != null)
                    {
                        UnityEngine.Object.Destroy(Souls[i].gameObject);
                    }
                }
                Souls.Clear();
            }

            if (FeatureDeadBodies != null)
            {
                for (int i = 0; i < FeatureDeadBodies.Count; i++)
                {
                    var (db, ps) = FeatureDeadBodies[i];
                    var s = new GameObject("Soul");
                    s.transform.position = new Vector3(ps.x, ps.y, ps.y / 1000f - 1f);
                    s.layer = 5;
                    var rend = s.AddComponent<SpriteRenderer>();
                    s.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
                    rend.sprite = AssetLoader.Soul;
                    Souls.Add(rend);
                }
                DeadBodies = FeatureDeadBodies;
                FeatureDeadBodies = [];
            }
        }
    }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {
        if (!PlayerControl.LocalPlayer.IsRole(RoleType.Medium)) return;

        if (DeadBodies != null && MapUtilities.CachedShipStatus?.AllVents != null && MapUtilities.CachedShipStatus.AllVents.Length > 0)
        {
            DeadPlayer target = null;
            Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            float closestDistance = float.MaxValue;
            float usableDistance = MapUtilities.CachedShipStatus.AllVents[0].UsableDistance;
            for (int i = 0; i < DeadBodies.Count; i++)
            {
                var (dp, ps) = DeadBodies[i];
                float distance = Vector2.Distance(ps, truePosition);
                if (distance <= usableDistance && distance < closestDistance)
                {
                    closestDistance = distance;
                    target = dp;
                }
            }
            Local.Target = target;
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public static void MakeButtons(HudManager hm)
    {
        MediumButton = new CustomButton(
            () =>
            {
                if (Local.Target != null)
                {
                    Local.SoulTarget = Local.Target;
                    MediumButton.HasEffect = true;
                }
            },
            () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Medium) && PlayerControl.LocalPlayer.IsAlive(); },
            () =>
            {
                if (MediumButton.IsEffectActive && Local.Target != Local.SoulTarget)
                {
                    Local.SoulTarget = null;
                    MediumButton.Timer = 0f;
                    MediumButton.IsEffectActive = false;
                }
                return Local.Target != null && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                MediumButton.Timer = MediumButton.MaxTimer;
                MediumButton.IsEffectActive = false;
                Local.SoulTarget = null;
            },
            AssetLoader.MediumButton,
            ButtonPosition.Layout,
            hm,
            hm.UseButton,
            KeyCode.F,
            true,
            Duration,
            () =>
            {
                MediumButton.Timer = MediumButton.MaxTimer;
                if (Local.Target == null || Local.Target.Player == null) return;
                string msg = "";

                int randomNumber = RebuildUs.Instance.Rnd.Next(4);
                if (Local.Target.KillerIfExisting != null)
                {
                    if (Helpers.PlayerById(Local.Target.KillerIfExisting.PlayerId).HasModifier(ModifierType.Mini))
                    {
                        randomNumber = RebuildUs.Instance.Rnd.Next(3);
                    }
                }
                string typeOfColor = Helpers.IsLighterColor(Local.Target.KillerIfExisting.Data.DefaultOutfit.ColorId) ? Tr.Get("Hud.DetectiveColorLight") : Tr.Get("Hud.DetectiveColorDark");
                float timeSinceDeath = (float)(MeetingStartTime - Local.Target.TimeOfDeath).TotalMilliseconds;
                string name = " (" + Local.Target.Player.Data.PlayerName + ")";

                msg = randomNumber == 0
                    ? string.Format(Tr.Get("Hud.MediumQuestion1"), RoleInfo.GetRolesString(Local.Target.Player, false, includeHidden: true)) + name
                    : randomNumber == 1
                        ? string.Format(Tr.Get("Hud.MediumQuestion2"), typeOfColor) + name
                        : randomNumber == 2
                        ? string.Format(Tr.Get("Hud.MediumQuestion3"), Math.Round(timeSinceDeath / 1000)) + name
                        : string.Format(Tr.Get("Hud.MediumQuestion4"), RoleInfo.GetRolesString(Local.Target.KillerIfExisting, false, includeHidden: true)) + name;

                // Excludes mini

                bool censorChat = AmongUs.Data.DataManager.Settings.Multiplayer.CensorChat;
                if (censorChat) AmongUs.Data.DataManager.Settings.Multiplayer.CensorChat = false;
                FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"{msg}");
                AmongUs.Data.DataManager.Settings.Multiplayer.CensorChat = censorChat;

                // Remove soul
                if (OneTimeUse)
                {
                    float closestDistance = float.MaxValue;
                    SpriteRenderer target = null;

                    foreach ((DeadPlayer db, Vector3 ps) in DeadBodies)
                    {
                        if (db == Local.Target)
                        {
                            DeadBodies.Remove((db, ps));
                            break;
                        }
                    }
                    foreach (var rend in Souls)
                    {
                        float distance = Vector2.Distance(rend.transform.position, PlayerControl.LocalPlayer.GetTruePosition());
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            target = rend;
                        }
                    }

                    FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(5f, new Action<float>((p) =>
                    {
                        if (target != null)
                        {
                            var tmp = target.color;
                            tmp.a = Mathf.Clamp01(1 - p);
                            target.color = tmp;
                        }
                        if (p == 1f && target != null && target.gameObject != null) UnityEngine.Object.Destroy(target.gameObject);
                    })));

                    Souls.Remove(target);
                }
            },
            false,
            Tr.Get("Hud.MediumText")
        );
    }
    public static void SetButtonCooldowns()
    {
        MediumButton.MaxTimer = Cooldown;
        MediumButton.EffectDuration = Duration;
    }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        DeadBodies = [];
        FeatureDeadBodies = [];
        Souls = [];
        Players.Clear();
    }
}