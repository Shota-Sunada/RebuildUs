namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
public class Camouflager : RoleBase<Camouflager>
{
    public static Color NameColor = Palette.ImpostorRed;
    public override Color RoleColor => NameColor;
    private static CustomButton CamouflagerButton;

    // write configs here
    public static float Cooldown { get { return CustomOptionHolder.CamouflagerCooldown.GetFloat(); } }
    public static float Duration { get { return CustomOptionHolder.CamouflagerDuration.GetFloat(); } }
    public static bool RandomColors { get { return CustomOptionHolder.CamouflagerRandomColors.GetBool(); } }
    public static float CamouflageTimer = 0f;
    public static NetworkedPlayerInfo.PlayerOutfit Data;

    public Camouflager()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Camouflager;
        CamouflageTimer = 0f;

        Data = new()
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
        ResetCamouflage();
    }
    public override void FixedUpdate() { }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void MakeButtons(HudManager hm)
    {
        CamouflagerButton = new CustomButton(
                () =>
                {
                    {
                        using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.CamouflagerCamouflage);
                    }
                    RPCProcedure.CamouflagerCamouflage();
                },
                () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Camouflager) && PlayerControl.LocalPlayer.IsAlive(); },
                () => { return PlayerControl.LocalPlayer.CanMove; },
                () =>
                {
                    CamouflagerButton.Timer = CamouflagerButton.MaxTimer;
                    CamouflagerButton.IsEffectActive = false;
                    CamouflagerButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                },
                AssetLoader.CamouflageButton,
                new Vector3(-1.8f, -0.06f, 0),
                hm,
                hm.KillButton,
                KeyCode.F,
                true,
                Camouflager.Duration,
                () => { CamouflagerButton.Timer = CamouflagerButton.MaxTimer; }
            )
        {
            ButtonText = Tr.Get("Hud.CamoText")
        };
    }
    public override void SetButtonCooldowns()
    {
        CamouflagerButton.MaxTimer = Camouflager.Cooldown;
        CamouflagerButton.EffectDuration = Camouflager.Duration;
    }

    // write functions here
    public static void StartCamouflage()
    {
        CamouflageTimer = Duration;

        Data.ColorId = RandomColors ? (byte)RebuildUs.Instance.Rnd.Next(0, Palette.PlayerColors.Length) : 6;

        foreach (PlayerControl p in CachedPlayer.AllPlayers)
        {
            if (p == null) continue;
            p.SetOutfit(Data, visible: false);
        }
    }

    public static void ResetCamouflage()
    {
        CamouflageTimer = 0f;
        foreach (var p in CachedPlayer.AllPlayers)
        {
            if (p.PlayerControl == null) continue;

            // special case for morphing
            if (p.PlayerControl.IsRole(RoleType.Morphing))
            {
                Morphing.GetRole(p).HandleMorphing();
            }
            else
            {
                p.PlayerControl.ResetMorph();
            }
        }
    }

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}