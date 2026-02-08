namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
public class Camouflager : RoleBase<Camouflager>
{
    public static Color NameColor = Palette.ImpostorRed;

    private static CustomButton _camouflagerButton;
    public static float CamouflageTimer;
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
            NamePlateId = "",
        };
    }

    public override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    public static float Cooldown
    {
        get => CustomOptionHolder.CamouflagerCooldown.GetFloat();
    }

    public static float Duration
    {
        get => CustomOptionHolder.CamouflagerDuration.GetFloat();
    }

    public static bool RandomColors
    {
        get => CustomOptionHolder.CamouflagerRandomColors.GetBool();
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

    public static void MakeButtons(HudManager hm)
    {
        _camouflagerButton = new(() =>
        {
            {
                using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.CamouflagerCamouflage);
            }
            RPCProcedure.CamouflagerCamouflage();
        }, () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Camouflager) && PlayerControl.LocalPlayer.IsAlive(); }, () => { return PlayerControl.LocalPlayer.CanMove; }, () =>
        {
            _camouflagerButton.Timer = _camouflagerButton.MaxTimer;
            _camouflagerButton.IsEffectActive = false;
            _camouflagerButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
        }, AssetLoader.CamouflageButton, ButtonPosition.Layout, hm, hm.KillButton, AbilitySlot.ImpostorAbilityPrimary, true, Duration, () => { _camouflagerButton.Timer = _camouflagerButton.MaxTimer; }, false, Tr.Get(TrKey.CamoText));
    }

    public static void SetButtonCooldowns()
    {
        _camouflagerButton.MaxTimer = Cooldown;
        _camouflagerButton.EffectDuration = Duration;
    }

    // write functions here
    public static void StartCamouflage()
    {
        CamouflageTimer = Duration;

        Data.ColorId = RandomColors ? (byte)RebuildUs.Instance.Rnd.Next(0, Palette.PlayerColors.Length) : 6;

        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p == null) continue;
            p.SetOutfit(Data, false);
        }
    }

    public static void ResetCamouflage()
    {
        CamouflageTimer = 0f;
        var players = PlayerControl.AllPlayerControls.GetFastEnumerator();
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p == null) continue;

            // special case for morphing
            if (p.IsRole(RoleType.Morphing))
            {
                var morphRole = Morphing.GetRole(p);
                morphRole?.HandleMorphing();
            }
            else
                p.ResetMorph();
        }
    }

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}
