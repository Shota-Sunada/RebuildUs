namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
[RegisterRole(RoleType.Camouflager, RoleTeam.Impostor, typeof(SingleRoleBase<Camouflager>), nameof(CustomOptionHolder.CamouflagerSpawnRate))]
internal class Camouflager : SingleRoleBase<Camouflager>
{
    public static Color Color = Palette.ImpostorRed;

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

    [CustomEvent(CustomEventType.OnIntroEnd)]
    internal void OnIntroEnd()
    {
        ResetCamouflage();
    }



    [RegisterCustomButton]
    internal static void MakeButtons(HudManager hm)
    {
        _camouflagerButton = new(() =>
            {
                CamouflagerCamouflage(PlayerControl.LocalPlayer);
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

    [RegisterCustomButton]
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

        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
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
        var players = PlayerControl.AllPlayerControls.GetFastEnumerator();
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p == null)
            {
                continue;
            }

            // special case for morphing
            if (p.IsRole(RoleType.Morphing))
            {
                var morphRole = Morphing.GetRole(p);
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

    [MethodRpc((uint)CustomRPC.CamouflagerCamouflage)]
    internal static void CamouflagerCamouflage(PlayerControl sender)
    {
        if (!Exists)
        {
            return;
        }
        StartCamouflage();
    }
}