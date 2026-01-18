namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Medium : RoleBase<Medium>
{
    public static Color NameColor = new Color32(98, 120, 115, byte.MaxValue);
    public override Color RoleColor => NameColor;
    public static CustomButton mediumButton;
    public static List<(DeadPlayer deadPlayer, Vector3 pos)> deadBodies = [];
    public static List<(DeadPlayer deadPlayer, Vector3 pos)> featureDeadBodies = [];
    public static List<SpriteRenderer> souls = [];

    // write configs here
    public static float cooldown { get { return CustomOptionHolder.mediumCooldown.GetFloat(); } }
    public static float duration { get { return CustomOptionHolder.mediumDuration.GetFloat(); } }
    public static bool oneTimeUse { get { return CustomOptionHolder.mediumOneTimeUse.GetBool(); } }

    public DeadPlayer target;
    public DeadPlayer soulTarget;
    public static DateTime meetingStartTime = DateTime.UtcNow;

    public Medium()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Medium;
    }

    public override void OnMeetingStart()
    {
        meetingStartTime = DateTime.UtcNow;
    }
    public override void OnMeetingEnd()
    {
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Medium))
        {
            if (souls != null)
            {
                foreach (var sr in souls)
                {
                    UnityEngine.Object.Destroy(sr.gameObject);
                }
                souls = [];
            }

            if (featureDeadBodies != null)
            {
                foreach ((DeadPlayer db, Vector3 ps) in featureDeadBodies)
                {
                    var s = new GameObject();
                    // s.transform.position = ps;
                    s.transform.position = new Vector3(ps.x, ps.y, ps.y / 1000 - 1f);
                    s.layer = 5;
                    var rend = s.AddComponent<SpriteRenderer>();
                    s.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
                    rend.sprite = AssetLoader.Soul;
                    souls.Add(rend);
                }
                deadBodies = featureDeadBodies;
                featureDeadBodies = [];
            }
        }
    }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Medium) || CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead || deadBodies != null || MapUtilities.CachedShipStatus?.AllVents != null)
        {
            DeadPlayer target = null;
            Vector2 truePosition = CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition();
            float closestDistance = float.MaxValue;
            float usableDistance = MapUtilities.CachedShipStatus.AllVents.FirstOrDefault().UsableDistance;
            foreach ((DeadPlayer dp, Vector3 ps) in Medium.deadBodies)
            {
                float distance = Vector2.Distance(ps, truePosition);
                if (distance <= usableDistance && distance < closestDistance)
                {
                    closestDistance = distance;
                    target = dp;
                }
            }
            Local.target = target;
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void MakeButtons(HudManager hm)
    {
        mediumButton = new CustomButton(
            () =>
            {
                if (Local.target != null)
                {
                    Local.soulTarget = Local.target;
                    mediumButton.HasEffect = true;
                }
            },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Medium) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
            () =>
            {
                if (mediumButton.IsEffectActive && Local.target != Local.soulTarget)
                {
                    Local.soulTarget = null;
                    mediumButton.Timer = 0f;
                    mediumButton.IsEffectActive = false;
                }
                return Local.target != null && CachedPlayer.LocalPlayer.PlayerControl.CanMove;
            },
            () =>
            {
                mediumButton.Timer = mediumButton.MaxTimer;
                mediumButton.IsEffectActive = false;
                Local.soulTarget = null;
            },
            AssetLoader.MediumButton,
            new Vector3(-1.8f, -0.06f, 0),
            hm,
            hm.UseButton,
            KeyCode.F,
            true,
            duration,
            () =>
            {
                mediumButton.Timer = mediumButton.MaxTimer;
                if (Local.target == null || Local.target.Player == null) return;
                string msg = "";

                int randomNumber = RebuildUs.Instance.Rnd.Next(4);
                if (Local.target.KillerIfExisting != null)
                {
                    if (Helpers.PlayerById(Local.target.KillerIfExisting.PlayerId).HasModifier(ModifierType.Mini))
                    {
                        randomNumber = RebuildUs.Instance.Rnd.Next(3);
                    }
                }
                string typeOfColor = Helpers.isLighterColor(Local.target.KillerIfExisting.Data.DefaultOutfit.ColorId) ? Tr.Get("detectiveColorLight") : Tr.Get("detectiveColorDark");
                float timeSinceDeath = (float)(meetingStartTime - Local.target.TimeOfDeath).TotalMilliseconds;
                string name = " (" + Local.target.Player.Data.PlayerName + ")";

                msg = randomNumber == 0
                    ? string.Format(Tr.Get("mediumQuestion1"), RoleInfo.GetRolesString(Local.target.Player, false, includeHidden: true)) + name
                    : randomNumber == 1
                        ? string.Format(Tr.Get("mediumQuestion2"), typeOfColor) + name
                        : randomNumber == 2
                        ? string.Format(Tr.Get("mediumQuestion3"), Math.Round(timeSinceDeath / 1000)) + name
                        : string.Format(Tr.Get("mediumQuestion4"), RoleInfo.GetRolesString(Local.target.KillerIfExisting, false, includeHidden: true)) + name;

                // Excludes mini

                bool CensorChat = AmongUs.Data.DataManager.Settings.Multiplayer.CensorChat;
                if (CensorChat) AmongUs.Data.DataManager.Settings.Multiplayer.CensorChat = false;
                FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(CachedPlayer.LocalPlayer.PlayerControl, $"{msg}");
                AmongUs.Data.DataManager.Settings.Multiplayer.CensorChat = CensorChat;

                // Remove soul
                if (oneTimeUse)
                {
                    float closestDistance = float.MaxValue;
                    SpriteRenderer target = null;

                    foreach ((DeadPlayer db, Vector3 ps) in deadBodies)
                    {
                        if (db == Local.target)
                        {
                            deadBodies.Remove((db, ps));
                            break;
                        }
                    }
                    foreach (var rend in souls)
                    {
                        float distance = Vector2.Distance(rend.transform.position, CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition());
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

                    souls.Remove(target);
                }
            }
        )
        {
            ButtonText = Tr.Get("MediumText")
        };
    }
    public override void SetButtonCooldowns()
    {
        mediumButton.MaxTimer = cooldown;
        mediumButton.EffectDuration = duration;
    }

    // write functions here

    public override void Clear()
    {
        // reset configs here
        deadBodies = [];
        featureDeadBodies = [];
        souls = [];
        Players.Clear();
    }
}