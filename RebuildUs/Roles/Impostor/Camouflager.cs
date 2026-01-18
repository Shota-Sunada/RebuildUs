namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
public class Camouflager : RoleBase<Camouflager>
{
    public static Color RoleColor = Palette.ImpostorRed;
    private static CustomButton camouflagerButton;

    // write configs here
    public static float cooldown { get { return CustomOptionHolder.camouflagerCooldown.GetFloat(); } }
    public static float duration { get { return CustomOptionHolder.camouflagerDuration.GetFloat(); } }
    public static bool randomColors { get { return CustomOptionHolder.camouflagerRandomColors.GetBool(); } }
    public static float camouflageTimer = 0f;
    public static NetworkedPlayerInfo.PlayerOutfit data;

    public Camouflager()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Camouflager;
        camouflageTimer = 0f;

        data = new()
        {
            PlayerName = "",
            HatId = "",
            ColorId = 6,
            SkinId = "",
            PetId = "",
            VisorId = "",
            NamePlateId = ""
        };
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd()
    {
        resetCamouflage();
    }
    public override void FixedUpdate() { }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void MakeButtons(HudManager hm)
    {
        camouflagerButton = new CustomButton(
                () =>
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.CamouflagerCamouflage, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.camouflagerCamouflage();
                },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Camouflager) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove; },
                () =>
                {
                    camouflagerButton.Timer = camouflagerButton.MaxTimer;
                    camouflagerButton.IsEffectActive = false;
                    camouflagerButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                },
                AssetLoader.CamouflageButton,
                new Vector3(-1.8f, -0.06f, 0),
                hm,
                hm.KillButton,
                KeyCode.F,
                true,
                Camouflager.duration,
                () => { camouflagerButton.Timer = camouflagerButton.MaxTimer; }
            )
        {
            ButtonText = Tr.Get("CamoText")
        };
    }
    public override void SetButtonCooldowns()
    {
        camouflagerButton.MaxTimer = Camouflager.cooldown;
        camouflagerButton.EffectDuration = Camouflager.duration;
    }

    // write functions here
    public static void startCamouflage()
    {
        camouflageTimer = duration;

        data.ColorId = randomColors ? (byte)RebuildUs.Instance.Rnd.Next(0, Palette.PlayerColors.Length) : 6;

        foreach (PlayerControl p in CachedPlayer.AllPlayers)
        {
            if (p == null) continue;
            p.setOutfit(data, visible: false);
        }
    }

    public static void resetCamouflage()
    {
        camouflageTimer = 0f;
        foreach (var p in CachedPlayer.AllPlayers)
        {
            if (p.PlayerControl == null) continue;

            // special case for morphing
            if (p.PlayerControl.IsRole(RoleType.Morphing))
            {
                Morphing.GetRole(p).handleMorphing();
            }
            else
            {
                p.PlayerControl.resetMorph();
            }
        }
    }

    public override void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}