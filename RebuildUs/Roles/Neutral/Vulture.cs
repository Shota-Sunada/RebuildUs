using RebuildUs.Objects;

namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
public class Vulture : RoleBase<Vulture>
{
    public static Color RoleColor = new Color32(139, 69, 19, byte.MaxValue);
    public static bool triggerVultureWin = false;
    public List<Arrow> localArrows = [];
    public int eatenBodies = 0;

    public static CustomButton vultureEatButton;
    public static TMP_Text vultureNumCorpsesText;

    // write configs here
    public static float cooldown { get { return CustomOptionHolder.vultureCooldown.GetFloat(); } }
    public static int numberToWin { get { return (int)CustomOptionHolder.vultureNumberToWin.GetFloat(); } }
    public static bool canUseVents { get { return CustomOptionHolder.vultureCanUseVents.GetBool(); } }
    public static bool showArrows { get { return CustomOptionHolder.vultureShowArrows.GetBool(); } }

    public Vulture()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = ERoleType.Vulture;
        eatenBodies = 0;
        localArrows = [];
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {
        if (localArrows == null || !showArrows) return;

        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.Vulture))
        {
            if (Player.IsDead())
            {
                foreach (var arrow in localArrows)
                {
                    UnityEngine.Object.Destroy(arrow.arrow);
                }
                localArrows = [];
                return;
            }

            DeadBody[] deadBodies = UnityEngine.Object.FindObjectsOfType<DeadBody>();
            var arrowUpdate = localArrows.Count != deadBodies.Length;
            int index = 0;

            if (arrowUpdate)
            {
                foreach (var arrow in localArrows) {
                    UnityEngine.Object.Destroy(arrow.arrow);}
                localArrows = [];
            }

            foreach (var db in deadBodies)
            {
                if (arrowUpdate)
                {
                    localArrows.Add(new(Color.blue));
                    localArrows[index].arrow.SetActive(true);
                }
                localArrows[index]?.Update(db.transform.position);
                index++;
            }
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    public static void MakeButtons(HudManager hm)
    {
        vultureEatButton = new CustomButton(
                () =>
                {
                    foreach (var collider2D in Physics2D.OverlapCircleAll(CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition(), CachedPlayer.LocalPlayer.PlayerControl.MaxReportDistance, Constants.PlayersOnlyMask))
                    {
                        if (collider2D.tag == "DeadBody")
                        {
                            var component = collider2D.GetComponent<DeadBody>();
                            if (component && !component.Reported)
                            {
                                var truePosition = CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition();
                                var truePosition2 = component.TruePosition;
                                if (Vector2.Distance(truePosition2, truePosition) <= CachedPlayer.LocalPlayer.PlayerControl.MaxReportDistance && CachedPlayer.LocalPlayer.PlayerControl.CanMove && !PhysicsHelpers.AnythingBetween(truePosition, truePosition2, Constants.ShipAndObjectsMask, false))
                                {
                                    var playerInfo = GameData.Instance.GetPlayerById(component.ParentId);

                                    using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.VultureEat);
                                    sender.Write(playerInfo.PlayerId);
                                    sender.Write(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                                    RPCProcedure.vultureEat(playerInfo.PlayerId, CachedPlayer.LocalPlayer.PlayerControl.PlayerId);

                                    vultureEatButton.Timer = vultureEatButton.MaxTimer;
                                    break;
                                }
                            }
                        }
                    }
                    if (Local.eatenBodies >= numberToWin)
                    {
                        using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.VultureWin);
                        RPCProcedure.vultureWin();
                        return;
                    }
                },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.Vulture) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
                () =>
                {
                    vultureNumCorpsesText?.text = string.Format(Tr.Get("vultureCorpses"), numberToWin - Local.eatenBodies);
                    return hm.ReportButton.graphic.color == Palette.EnabledColor && CachedPlayer.LocalPlayer.PlayerControl.CanMove;
                },
                () => { vultureEatButton.Timer = vultureEatButton.MaxTimer; },
                getButtonSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                hm,
                hm.KillButton,
                KeyCode.F
            )
        {
            buttonText = Tr.Get("VultureText")
        };

        vultureNumCorpsesText = UnityEngine.Object.Instantiate(vultureEatButton.actionButton.cooldownTimerText, vultureEatButton.actionButton.cooldownTimerText.transform.parent);
        vultureNumCorpsesText.text = "";
        vultureNumCorpsesText.enableWordWrapping = false;
        vultureNumCorpsesText.transform.localScale = Vector3.one * 0.5f;
        vultureNumCorpsesText.transform.localPosition += new Vector3(0.0f, 0.7f, 0);
    }
    public static void SetButtonCooldowns()
    {
        vultureEatButton.MaxTimer = cooldown;
    }

    // write functions here
    private static Sprite buttonSprite;
    public static Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = Helpers.LoadSpriteFromResources("RebuildUs.Resources.VultureButton.png", 115f);
        return buttonSprite;
    }

    public static void Clear()
    {
        // reset configs here
        triggerVultureWin = false;
        Players.Clear();
    }
}