using RebuildUs.Objects;

namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
public class Vulture : RoleBase<Vulture>
{
    public static Color RoleColor = new Color32(139, 69, 19, byte.MaxValue);
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

        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Vulture))
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

    public static void MakeButtons(HudManager hm)
    {
        VultureEatButton = new CustomButton(
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
                                    RPCProcedure.VultureEat(playerInfo.PlayerId, CachedPlayer.LocalPlayer.PlayerControl.PlayerId);

                                    VultureEatButton.Timer = VultureEatButton.MaxTimer;
                                    break;
                                }
                            }
                        }
                    }
                    if (Local.EatenBodies >= NumberToWin)
                    {
                        using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.VultureWin);
                        RPCProcedure.VultureWin();
                        return;
                    }
                },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Vulture) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
                () =>
                {
                    VultureNumCorpsesText?.text = string.Format(Tr.Get("vultureCorpses"), NumberToWin - Local.EatenBodies);
                    return hm.ReportButton.graphic.color == Palette.EnabledColor && CachedPlayer.LocalPlayer.PlayerControl.CanMove;
                },
                () => { VultureEatButton.Timer = VultureEatButton.MaxTimer; },
                AssetLoader.VultureButton,
                new Vector3(-1.8f, -0.06f, 0),
                hm,
                hm.KillButton,
                KeyCode.F
            )
        {
            ButtonText = Tr.Get("VultureText")
        };

        VultureNumCorpsesText = UnityEngine.Object.Instantiate(VultureEatButton.ActionButton.cooldownTimerText, VultureEatButton.ActionButton.cooldownTimerText.transform.parent);
        VultureNumCorpsesText.text = "";
        VultureNumCorpsesText.enableWordWrapping = false;
        VultureNumCorpsesText.transform.localScale = Vector3.one * 0.5f;
        VultureNumCorpsesText.transform.localPosition += new Vector3(0.0f, 0.7f, 0);
    }
    public static void SetButtonCooldowns()
    {
        VultureEatButton.MaxTimer = Cooldown;
    }

    public static void Clear()
    {
        // reset configs here
        TriggerVultureWin = false;
        Players.Clear();
    }
}