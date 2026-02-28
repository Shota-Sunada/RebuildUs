namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
[RegisterRole(RoleType.Camouflager, RoleTeam.Impostor, typeof(SingleRoleBase<Camouflager>), nameof(Camouflager.NameColor), nameof(CustomOptionHolder.CamouflagerSpawnRate))]
internal class Camouflager : SingleRoleBase<Camouflager>
{
    internal static Color NameColor = Palette.ImpostorRed;

    private static CustomButton _camouflagerButton;
    internal static float CamouflageTimer;
    private static NetworkedPlayerInfo.PlayerOutfit _data;

    public Camouflager()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Camouflager;
        CamouflageTimer = 0f;

        _data = new()
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

    internal override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    private static float Cooldown
    {
        get => CustomOptionHolder.CamouflagerCooldown.GetFloat();
    }

    private static float Duration
    {
        get => CustomOptionHolder.CamouflagerDuration.GetFloat();
    }

    private static bool RandomColors
    {
        get => CustomOptionHolder.CamouflagerRandomColors.GetBool();
    }

    internal override void OnMeetingStart() { }
    internal override void OnMeetingEnd() { }

    internal override void OnIntroEnd()
    {
        ResetCamouflage();
    }

    internal override void FixedUpdate() { }
    internal override void OnKill(PlayerControl target) { }
    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }

    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    internal static void MakeButtons(HudManager hm)
    {
        _camouflagerButton = new(() =>
            {
                {
                    using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.CamouflagerCamouflage);
                }
                RPCProcedure.CamouflagerCamouflage();
            },
            () => PlayerControl.LocalPlayer.IsRole(RoleType.Camouflager) && PlayerControl.LocalPlayer.IsAlive(),
            () => PlayerControl.LocalPlayer.CanMove,
            () =>
            {
                _camouflagerButton.Timer = _camouflagerButton.MaxTimer;
                _camouflagerButton.IsEffectActive = false;
                _camouflagerButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            AssetLoader.CamouflageButton,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            true,
            Duration,
            () =>
            {
                _camouflagerButton.Timer = _camouflagerButton.MaxTimer;
            },
            false,
            Tr.Get(TrKey.CamoText));
    }

    internal static void SetButtonCooldowns()
    {
        _camouflagerButton.MaxTimer = Cooldown;
        _camouflagerButton.EffectDuration = Duration;
    }

    // write functions here
    internal static void StartCamouflage()
    {
        CamouflageTimer = Duration;

        _data.ColorId = RandomColors ? (byte)RebuildUs.Rnd.Next(0, Palette.PlayerColors.Length) : 6;

        foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p == null)
            {
                continue;
            }
            p.SetOutfit(_data, false);
        }
    }

    internal static void ResetCamouflage()
    {
        CamouflageTimer = 0f;
        IEnumerable<PlayerControl> players = PlayerControl.AllPlayerControls.GetFastEnumerator();
        foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p == null)
            {
                continue;
            }

            // special case for morphing
            if (p.IsRole(RoleType.Morphing))
            {
                Morphing morphRole = Morphing.GetRole(p);
                morphRole?.HandleMorphing();
            }
            else
            {
                p.ResetMorph();
            }
        }
    }

    internal static void Clear()
    {
        ModRoleManager.RemoveRole(Instance);
        Instance = null;
    }
}