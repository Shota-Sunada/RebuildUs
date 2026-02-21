using Object = UnityEngine.Object;

namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
internal class Vulture : RoleBase<Vulture>
{
    internal static Color NameColor = new Color32(139, 69, 19, byte.MaxValue);

    internal static bool TriggerVultureWin;

    internal static CustomButton VultureEatButton;
    internal static TMP_Text VultureNumCorpsesText;
    private float _timeUntilUpdate;
    internal int EatenBodies;
    internal List<Arrow> LocalArrows = [];

    public Vulture()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Vulture;
        EatenBodies = 0;
        LocalArrows = [];
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    internal static float Cooldown { get => CustomOptionHolder.VultureCooldown.GetFloat(); }
    internal static int NumberToWin { get => (int)CustomOptionHolder.VultureNumberToWin.GetFloat(); }
    internal static bool CanUseVents { get => CustomOptionHolder.VultureCanUseVents.GetBool(); }
    internal static bool ShowArrows { get => CustomOptionHolder.VultureShowArrows.GetBool(); }

    internal override void OnMeetingStart() { }
    internal override void OnMeetingEnd() { }
    internal override void OnIntroEnd() { }

    internal override void FixedUpdate()
    {
        if (LocalArrows == null || !ShowArrows) return;

        Vulture local = Local;
        if (local != null)
        {
            if (Player.IsDead())
            {
                for (int i = 0; i < LocalArrows.Count; i++) Object.Destroy(LocalArrows[i].ArrowObject);

                LocalArrows.Clear();
                return;
            }

            _timeUntilUpdate -= Time.fixedDeltaTime;
            if (_timeUntilUpdate > 0f) return;
            _timeUntilUpdate = 0.25f;

            DeadBody[] deadBodies = Object.FindObjectsOfType<DeadBody>();
            bool arrowUpdate = LocalArrows.Count != deadBodies.Length;

            if (arrowUpdate)
            {
                for (int i = 0; i < LocalArrows.Count; i++) Object.Destroy(LocalArrows[i].ArrowObject);

                LocalArrows.Clear();
            }

            for (int i = 0; i < deadBodies.Length; i++)
            {
                DeadBody db = deadBodies[i];
                if (arrowUpdate)
                {
                    LocalArrows.Add(new(Color.blue));
                    LocalArrows[i].ArrowObject.SetActive(true);
                }

                LocalArrows[i]?.Update(db.transform.position);
            }
        }
    }

    internal override void OnKill(PlayerControl target) { }
    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    internal static void MakeButtons(HudManager hm)
    {
        VultureEatButton = new(() =>
        {
            Il2CppArrayBase<DeadBody> bodies = Object.FindObjectsOfType<DeadBody>();
            PlayerControl local = PlayerControl.LocalPlayer;
            Vector2 truePosition = local.GetTruePosition();
            float maxDist = local.MaxReportDistance;

            for (int i = 0; i < bodies.Length; i++)
            {
                DeadBody body = bodies[i];
                if (body == null || body.Reported) continue;

                Vector2 bodyPos = body.TruePosition;
                if (Vector2.Distance(bodyPos, truePosition) <= maxDist && local.CanMove && !PhysicsHelpers.AnythingBetween(truePosition, bodyPos, Constants.ShipAndObjectsMask, false))
                {
                    NetworkedPlayerInfo playerInfo = GameData.Instance.GetPlayerById(body.ParentId);
                    if (playerInfo != null)
                    {
                        using RPCSender sender = new(local.NetId, CustomRPC.VultureEat);
                        sender.Write(playerInfo.PlayerId);
                        sender.Write(local.PlayerId);
                        RPCProcedure.VultureEat(playerInfo.PlayerId, local.PlayerId);

                        VultureEatButton.Timer = VultureEatButton.MaxTimer;
                        break;
                    }
                }
            }

            if (Local.EatenBodies >= NumberToWin)
            {
                using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.VultureWin);
                RPCProcedure.VultureWin();
            }
        }, () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Vulture) && PlayerControl.LocalPlayer.IsAlive(); }, () =>
        {
            VultureNumCorpsesText?.text = string.Format(Tr.Get(TrKey.VultureCorpses), NumberToWin - Local.EatenBodies);
            return hm.ReportButton.graphic.color == Palette.EnabledColor && PlayerControl.LocalPlayer.CanMove;
        }, () => { VultureEatButton.Timer = VultureEatButton.MaxTimer; }, AssetLoader.VultureButton, ButtonPosition.Layout, hm, hm.KillButton, AbilitySlot.NeutralAbilityPrimary, false, Tr.Get(TrKey.VultureText));

        VultureNumCorpsesText = Object.Instantiate(VultureEatButton.ActionButton.cooldownTimerText, VultureEatButton.ActionButton.cooldownTimerText.transform.parent);
        VultureNumCorpsesText.text = "";
        VultureNumCorpsesText.enableWordWrapping = false;
        VultureNumCorpsesText.transform.localScale = Vector3.one * 0.5f;
        VultureNumCorpsesText.transform.localPosition += new Vector3(0.0f, 0.7f, 0);
    }

    internal static void SetButtonCooldowns()
    {
        VultureEatButton.MaxTimer = Cooldown;
    }

    internal static void Clear()
    {
        TriggerVultureWin = false;
        for (int i = 0; i < Players.Count; i++)
        {
            Vulture p = Players[i];
            if (p.LocalArrows != null)
            {
                for (int j = 0; j < p.LocalArrows.Count; j++)
                {
                    Arrow arrow = p.LocalArrows[j];
                    if (arrow?.ArrowObject != null) Object.Destroy(arrow.ArrowObject);
                }

                p.LocalArrows.Clear();
            }
        }

        Players.Clear();
    }
}