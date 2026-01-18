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

        if (PlayerControl.LocalPlayer.IsRole(RoleType.Vulture))
        {
            if (Player.IsDead())
            {
                foreach (var arrow in LocalArrows)
                {
                    UnityEngine.Object.Destroy(arrow.ArrowObject);
                }
                LocalArrows = [];
                return;
            }

            DeadBody[] deadBodies = UnityEngine.Object.FindObjectsOfType<DeadBody>();
            var arrowUpdate = LocalArrows.Count != deadBodies.Length;
            int index = 0;

            if (arrowUpdate)
            {
                foreach (var arrow in LocalArrows)
                {
                    UnityEngine.Object.Destroy(arrow.ArrowObject);
                }
                LocalArrows = [];
            }

            foreach (var db in deadBodies)
            {
                if (arrowUpdate)
                {
                    LocalArrows.Add(new(Color.blue));
                    LocalArrows[index].ArrowObject.SetActive(true);
                }
                LocalArrows[index]?.Update(db.transform.position);
                index++;
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
                    foreach (var collider2D in Physics2D.OverlapCircleAll(PlayerControl.LocalPlayer.GetTruePosition(), PlayerControl.LocalPlayer.MaxReportDistance, Constants.PlayersOnlyMask))
                    {
                        if (collider2D.tag == "DeadBody")
                        {
                            var component = collider2D.GetComponent<DeadBody>();
                            if (component && !component.Reported)
                            {
                                var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                                var truePosition2 = component.TruePosition;
                                if (Vector2.Distance(truePosition2, truePosition) <= PlayerControl.LocalPlayer.MaxReportDistance && PlayerControl.LocalPlayer.CanMove && !PhysicsHelpers.AnythingBetween(truePosition, truePosition2, Constants.ShipAndObjectsMask, false))
                                {
                                    var playerInfo = GameData.Instance.GetPlayerById(component.ParentId);

                                    using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.VultureEat);
                                    sender.Write(playerInfo.PlayerId);
                                    sender.Write(PlayerControl.LocalPlayer.PlayerId);
                                    RPCProcedure.VultureEat(playerInfo.PlayerId, PlayerControl.LocalPlayer.PlayerId);

                                    VultureEatButton.Timer = VultureEatButton.MaxTimer;
                                    break;
                                }
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
        // reset configs here
        TriggerVultureWin = false;
        foreach (var p in Players)
        {
            if (p.LocalArrows != null)
            {
                foreach (var arrow in p.LocalArrows)
                {
                    if (arrow?.ArrowObject != null)
                    {
                        UnityEngine.Object.Destroy(arrow.ArrowObject);
                    }
                }
            }
        }
        Players.Clear();
    }
}