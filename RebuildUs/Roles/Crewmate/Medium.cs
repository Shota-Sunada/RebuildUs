using AmongUs.Data;
using Object = UnityEngine.Object;

namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
internal class Medium : RoleBase<Medium>
{
    internal static Color NameColor = new Color32(98, 120, 115, byte.MaxValue);

    private static CustomButton _mediumButton;
    internal static List<(DeadPlayer deadPlayer, Vector3 pos)> DeadBodies = [];
    internal static List<(DeadPlayer deadPlayer, Vector3 pos)> FeatureDeadBodies = [];
    private static List<SpriteRenderer> _souls = [];
    internal static DateTime MeetingStartTime = DateTime.UtcNow;
    private DeadPlayer _soulTarget;

    private DeadPlayer _target;

    public Medium()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Medium;
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    private static float Cooldown { get => CustomOptionHolder.MediumCooldown.GetFloat(); }
    private static float Duration { get => CustomOptionHolder.MediumDuration.GetFloat(); }
    private static bool OneTimeUse { get => CustomOptionHolder.MediumOneTimeUse.GetBool(); }

    internal override void OnMeetingStart()
    {
        MeetingStartTime = DateTime.UtcNow;
    }

    internal override void OnMeetingEnd()
    {
        if (!PlayerControl.LocalPlayer.IsRole(RoleType.Medium)) return;
        if (_souls != null)
        {
            foreach (var t in _souls)
            {
                if (t != null && t.gameObject != null)
                {
                    Object.Destroy(t.gameObject);
                }
            }

            _souls.Clear();
        }

        if (FeatureDeadBodies == null) return;
        for (int i = 0; i < FeatureDeadBodies.Count; i++)
        {
            (DeadPlayer _, Vector3 ps) = FeatureDeadBodies[i];
            GameObject s = new("Soul")
            {
                transform =
                {
                    position = new(ps.x, ps.y, (ps.y / 1000f) - 1f),
                },
                layer = 5,
            };
            SpriteRenderer rend = s.AddComponent<SpriteRenderer>();
            s.AddSubmergedComponent(SubmergedCompatibility.Classes.ELEVATOR_MOVER);
            rend.sprite = AssetLoader.Soul;
            _souls?.Add(rend);
        }

        DeadBodies = FeatureDeadBodies;
        FeatureDeadBodies = [];
    }

    internal override void OnIntroEnd() { }

    internal override void FixedUpdate()
    {
        if (!PlayerControl.LocalPlayer.IsRole(RoleType.Medium)) return;

        if (DeadBodies == null || MapUtilities.CachedShipStatus?.AllVents == null || MapUtilities.CachedShipStatus.AllVents.Length <= 0) return;
        DeadPlayer target = null;
        Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
        float closestDistance = float.MaxValue;
        float usableDistance = MapUtilities.CachedShipStatus.AllVents[0].UsableDistance;
        foreach ((DeadPlayer dp, Vector3 ps) in DeadBodies)
        {
            float distance = Vector2.Distance(ps, truePosition);
            if (!(distance <= usableDistance) || !(distance < closestDistance)) continue;
            closestDistance = distance;
            target = dp;
        }

        Local._target = target;
    }

    internal override void OnKill(PlayerControl target) { }
    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    internal static void MakeButtons(HudManager hm)
    {
        _mediumButton = new(() =>
        {
            if (Local._target == null) return;
            Local._soulTarget = Local._target;
            _mediumButton.HasEffect = true;
        }, () => PlayerControl.LocalPlayer.IsRole(RoleType.Medium) && PlayerControl.LocalPlayer.IsAlive(), () =>
        {
            if (!_mediumButton.IsEffectActive || Local._target == Local._soulTarget) return Local._target != null && PlayerControl.LocalPlayer.CanMove;
            Local._soulTarget = null;
            _mediumButton.Timer = 0f;
            _mediumButton.IsEffectActive = false;

            return Local._target != null && PlayerControl.LocalPlayer.CanMove;
        }, () =>
        {
            _mediumButton.Timer = _mediumButton.MaxTimer;
            _mediumButton.IsEffectActive = false;
            Local._soulTarget = null;
        }, AssetLoader.MediumButton, ButtonPosition.Layout, hm, hm.UseButton, AbilitySlot.CrewmateAbilityPrimary, true, Duration, () =>
        {
            _mediumButton.Timer = _mediumButton.MaxTimer;
            if (Local._target == null || Local._target.Player == null) return;
            string msg = "";

            int randomNumber = RebuildUs.Rnd.Next(4);
            if (Local._target.KillerIfExisting != null)
            {
                if (Helpers.PlayerById(Local._target.KillerIfExisting.PlayerId).HasModifier(ModifierType.Mini))
                    randomNumber = RebuildUs.Rnd.Next(3);
            }

            string typeOfColor = Helpers.IsLighterColor(Local._target.KillerIfExisting.Data.DefaultOutfit.ColorId) ? Tr.Get(TrKey.DetectiveColorLight) : Tr.Get(TrKey.DetectiveColorDark);
            float timeSinceDeath = (float)(MeetingStartTime - Local._target.TimeOfDeath).TotalMilliseconds;
            string name = " (" + Local._target.Player.Data.PlayerName + ")";

            msg = randomNumber == 0 ? string.Format(Tr.Get(TrKey.MediumQuestion1), RoleInfo.GetRolesString(Local._target.Player, false, includeHidden: true)) + name : randomNumber == 1 ? string.Format(Tr.Get(TrKey.MediumQuestion2), typeOfColor) + name : randomNumber == 2 ? string.Format(Tr.Get(TrKey.MediumQuestion3), Math.Round(timeSinceDeath / 1000)) + name : string.Format(Tr.Get(TrKey.MediumQuestion4), RoleInfo.GetRolesString(Local._target.KillerIfExisting, false, includeHidden: true)) + name;

            // Excludes mini

            bool censorChat = DataManager.Settings.Multiplayer.CensorChat;
            if (censorChat) DataManager.Settings.Multiplayer.CensorChat = false;
            FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"{msg}");
            DataManager.Settings.Multiplayer.CensorChat = censorChat;

            // Remove soul
            if (!OneTimeUse) return;
            float closestDistance = float.MaxValue;
            SpriteRenderer target = null;

            foreach ((DeadPlayer db, Vector3 ps) in DeadBodies)
            {
                if (db != Local._target) continue;
                DeadBodies.Remove((db, ps));
                break;
            }

            foreach (SpriteRenderer rend in _souls)
            {
                float distance = Vector2.Distance(rend.transform.position, PlayerControl.LocalPlayer.GetTruePosition());
                if (!(distance < closestDistance)) continue;
                closestDistance = distance;
                target = rend;
            }

            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(5f, new Action<float>(p =>
            {
                if (target != null)
                {
                    Color tmp = target.color;
                    tmp.a = Mathf.Clamp01(1 - p);
                    target.color = tmp;
                }

                if (Mathf.Approximately(p, 1f) && target != null && target.gameObject != null) Object.Destroy(target.gameObject);
            })));

            _souls.Remove(target);
        }, false, Tr.Get(TrKey.MediumText));
    }

    internal static void SetButtonCooldowns()
    {
        _mediumButton.MaxTimer = Cooldown;
        _mediumButton.EffectDuration = Duration;
    }

    // write functions here

    internal static void Clear()
    {
        // reset configs here
        DeadBodies = [];
        FeatureDeadBodies = [];
        _souls = [];
        Players.Clear();
    }
}