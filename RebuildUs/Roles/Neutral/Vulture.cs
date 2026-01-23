namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
public class Vulture : RoleBase<Vulture>
{
    public static Color NameColor = new Color32(139, 69, 19, byte.MaxValue);
    public override Color RoleColor => NameColor;
    public static bool TriggerVultureWin = false;
    public List<Arrow> LocalArrows = [];
    public int EatenBodies = 0;

    public static CustomButton VultureEatButton;
    public static TMP_Text VultureNumCorpsesText;

    // write configs here
    public static float Cooldown { get { return CustomOptionHolder.VultureCooldown.GetFloat(); } }
    public static int NumberToWin { get { return (int)CustomOptionHolder.VultureNumberToWin.GetFloat(); } }
    public static bool CanUseVents { get { return CustomOptionHolder.VultureCanUseVents.GetBool(); } }
    public static bool ShowArrows { get { return CustomOptionHolder.VultureShowArrows.GetBool(); } }

    public Vulture()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Vulture;
        EatenBodies = 0;
        LocalArrows = [];
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {
        if (LocalArrows == null || !ShowArrows) return;

        var local = Local;
        if (local != null)
        {
            if (Player.IsDead())
            {
                for (var i = 0; i < LocalArrows.Count; i++)
                {
                    UnityEngine.Object.Destroy(LocalArrows[i].ArrowObject);
                }
                LocalArrows.Clear();
                return;
            }

            DeadBody[] deadBodies = UnityEngine.Object.FindObjectsOfType<DeadBody>();
            var arrowUpdate = LocalArrows.Count != deadBodies.Length;

            if (arrowUpdate)
            {
                for (var i = 0; i < LocalArrows.Count; i++)
                {
                    UnityEngine.Object.Destroy(LocalArrows[i].ArrowObject);
                }
                LocalArrows.Clear();
            }

            for (var i = 0; i < deadBodies.Length; i++)
            {
                var db = deadBodies[i];
                if (arrowUpdate)
                {
                    LocalArrows.Add(new(Color.blue));
                    LocalArrows[i].ArrowObject.SetActive(true);
                }
                LocalArrows[i]?.Update(db.transform.position);
            }
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void MakeButtons(HudManager hm)
    {
        VultureEatButton = new CustomButton(
                () =>
                {
                    var bodies = UnityEngine.Object.FindObjectsOfType<DeadBody>();
                    var local = PlayerControl.LocalPlayer;
                    var truePosition = local.GetTruePosition();
                    var maxDist = local.MaxReportDistance;

                    for (var i = 0; i < bodies.Length; i++)
                    {
                        var body = bodies[i];
                        if (body == null || body.Reported) continue;

                        var bodyPos = body.TruePosition;
                        if (Vector2.Distance(bodyPos, truePosition) <= maxDist && local.CanMove && !PhysicsHelpers.AnythingBetween(truePosition, bodyPos, Constants.ShipAndObjectsMask, false))
                        {
                            var playerInfo = GameData.Instance.GetPlayerById(body.ParentId);
                            if (playerInfo != null)
                            {
                                using var sender = new RPCSender(local.NetId, CustomRPC.VultureEat);
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
                        using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.VultureWin);
                        RPCProcedure.VultureWin();
                        return;
                    }
                },
                () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Vulture) && PlayerControl.LocalPlayer.IsAlive(); },
                () =>
                {
                    VultureNumCorpsesText?.text = string.Format(Tr.Get("Hud.VultureCorpses"), NumberToWin - Local.EatenBodies);
                    return hm.ReportButton.graphic.color == Palette.EnabledColor && PlayerControl.LocalPlayer.CanMove;
                },
                () => { VultureEatButton.Timer = VultureEatButton.MaxTimer; },
                AssetLoader.VultureButton,
                new Vector3(-1.8f, -0.06f, 0),
                hm,
                hm.KillButton,
                KeyCode.F
            )
        {
            ButtonText = Tr.Get("Hud.VultureText")
        };

        VultureNumCorpsesText = UnityEngine.Object.Instantiate(VultureEatButton.ActionButton.cooldownTimerText, VultureEatButton.ActionButton.cooldownTimerText.transform.parent);
        VultureNumCorpsesText.text = "";
        VultureNumCorpsesText.enableWordWrapping = false;
        VultureNumCorpsesText.transform.localScale = Vector3.one * 0.5f;
        VultureNumCorpsesText.transform.localPosition += new Vector3(0.0f, 0.7f, 0);
    }
    public override void SetButtonCooldowns()
    {
        VultureEatButton.MaxTimer = Cooldown;
    }

    public static void Clear()
    {
        TriggerVultureWin = false;
        for (var i = 0; i < Players.Count; i++)
        {
            var p = Players[i];
            if (p.LocalArrows != null)
            {
                for (var j = 0; j < p.LocalArrows.Count; j++)
                {
                    var arrow = p.LocalArrows[j];
                    if (arrow?.ArrowObject != null)
                    {
                        UnityEngine.Object.Destroy(arrow.ArrowObject);
                    }
                }
                p.LocalArrows.Clear();
            }
        }
        Players.Clear();
    }
}