using AmongUs.Data;
using Object = UnityEngine.Object;

namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Medium : RoleBase<Medium>
{
    public static Color NameColor = new Color32(98, 120, 115, byte.MaxValue);
    public static CustomButton MediumButton;
    public static List<(DeadPlayer deadPlayer, Vector3 pos)> DeadBodies = [];
    public static List<(DeadPlayer deadPlayer, Vector3 pos)> FeatureDeadBodies = [];
    public static List<SpriteRenderer> Souls = [];
    public static DateTime MeetingStartTime = DateTime.UtcNow;
    public DeadPlayer SoulTarget;

    public DeadPlayer Target;

    public Medium()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Medium;
    }

    public override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    public static float Cooldown
    {
        get => CustomOptionHolder.MediumCooldown.GetFloat();
    }

    public static float Duration
    {
        get => CustomOptionHolder.MediumDuration.GetFloat();
    }

    public static bool OneTimeUse
    {
        get => CustomOptionHolder.MediumOneTimeUse.GetBool();
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
                for (var i = 0; i < Souls.Count; i++)
                {
                    if (Souls[i] != null && Souls[i].gameObject != null)
                        Object.Destroy(Souls[i].gameObject);
                }

                Souls.Clear();
            }

            if (FeatureDeadBodies != null)
            {
                for (var i = 0; i < FeatureDeadBodies.Count; i++)
                {
                    var (db, ps) = FeatureDeadBodies[i];
                    var s = new GameObject("Soul");
                    s.transform.position = new(ps.x, ps.y, (ps.y / 1000f) - 1f);
                    s.layer = 5;
                    var rend = s.AddComponent<SpriteRenderer>();
                    s.AddSubmergedComponent(SubmergedCompatibility.ELEVATOR_MOVER);
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
            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            var closestDistance = float.MaxValue;
            var usableDistance = MapUtilities.CachedShipStatus.AllVents[0].UsableDistance;
            for (var i = 0; i < DeadBodies.Count; i++)
            {
                var (dp, ps) = DeadBodies[i];
                var distance = Vector2.Distance(ps, truePosition);
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
        MediumButton = new(() =>
        {
            if (Local.Target != null)
            {
                Local.SoulTarget = Local.Target;
                MediumButton.HasEffect = true;
            }
        }, () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Medium) && PlayerControl.LocalPlayer.IsAlive(); }, () =>
        {
            if (MediumButton.IsEffectActive && Local.Target != Local.SoulTarget)
            {
                Local.SoulTarget = null;
                MediumButton.Timer = 0f;
                MediumButton.IsEffectActive = false;
            }

            return Local.Target != null && PlayerControl.LocalPlayer.CanMove;
        }, () =>
        {
            MediumButton.Timer = MediumButton.MaxTimer;
            MediumButton.IsEffectActive = false;
            Local.SoulTarget = null;
        }, AssetLoader.MediumButton, ButtonPosition.Layout, hm, hm.UseButton, AbilitySlot.CrewmateAbilityPrimary, true, Duration, () =>
        {
            MediumButton.Timer = MediumButton.MaxTimer;
            if (Local.Target == null || Local.Target.Player == null) return;
            var msg = "";

            var randomNumber = RebuildUs.Instance.Rnd.Next(4);
            if (Local.Target.KillerIfExisting != null)
            {
                if (Helpers.PlayerById(Local.Target.KillerIfExisting.PlayerId).HasModifier(ModifierType.Mini))
                    randomNumber = RebuildUs.Instance.Rnd.Next(3);
            }

            var typeOfColor = Helpers.IsLighterColor(Local.Target.KillerIfExisting.Data.DefaultOutfit.ColorId) ? Tr.Get(TrKey.DetectiveColorLight) : Tr.Get(TrKey.DetectiveColorDark);
            var timeSinceDeath = (float)(MeetingStartTime - Local.Target.TimeOfDeath).TotalMilliseconds;
            var name = " (" + Local.Target.Player.Data.PlayerName + ")";

            msg = randomNumber == 0 ? string.Format(Tr.Get(TrKey.MediumQuestion1), RoleInfo.GetRolesString(Local.Target.Player, false, includeHidden: true)) + name : randomNumber == 1 ? string.Format(Tr.Get(TrKey.MediumQuestion2), typeOfColor) + name : randomNumber == 2 ? string.Format(Tr.Get(TrKey.MediumQuestion3), Math.Round(timeSinceDeath / 1000)) + name : string.Format(Tr.Get(TrKey.MediumQuestion4), RoleInfo.GetRolesString(Local.Target.KillerIfExisting, false, includeHidden: true)) + name;

            // Excludes mini

            var censorChat = DataManager.Settings.Multiplayer.CensorChat;
            if (censorChat) DataManager.Settings.Multiplayer.CensorChat = false;
            FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"{msg}");
            DataManager.Settings.Multiplayer.CensorChat = censorChat;

            // Remove soul
            if (OneTimeUse)
            {
                var closestDistance = float.MaxValue;
                SpriteRenderer target = null;

                foreach (var (db, ps) in DeadBodies)
                {
                    if (db == Local.Target)
                    {
                        DeadBodies.Remove((db, ps));
                        break;
                    }
                }

                foreach (var rend in Souls)
                {
                    var distance = Vector2.Distance(rend.transform.position, PlayerControl.LocalPlayer.GetTruePosition());
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        target = rend;
                    }
                }

                FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(5f, new Action<float>(p =>
                {
                    if (target != null)
                    {
                        var tmp = target.color;
                        tmp.a = Mathf.Clamp01(1 - p);
                        target.color = tmp;
                    }

                    if (p == 1f && target != null && target.gameObject != null) Object.Destroy(target.gameObject);
                })));

                Souls.Remove(target);
            }
        }, false, Tr.Get(TrKey.MediumText));
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
