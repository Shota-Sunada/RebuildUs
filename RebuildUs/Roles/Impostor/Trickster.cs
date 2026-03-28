namespace RebuildUs.Roles.Impostor;

[HarmonyPatch]
[RegisterRole(RoleType.Trickster, RoleTeam.Impostor, typeof(SingleRoleBase<Trickster>), nameof(CustomOptionHolder.TricksterSpawnRate))]
internal class Trickster : SingleRoleBase<Trickster>
{
    public static Color Color = Palette.ImpostorRed;

    private static CustomButton PlaceJackInTheBoxButton;
    private static CustomButton LightsOutButton;
    internal static float LightsOutTimer;

    public Trickster()
    {
        StaticRoleType = CurrentRoleType = RoleType.Trickster;
    }

    private static float PlaceBoxCooldown { get => CustomOptionHolder.TricksterPlaceBoxCooldown.GetFloat(); }
    private static float LightsOutCooldown { get => CustomOptionHolder.TricksterLightsOutCooldown.GetFloat(); }
    internal static float LightsOutDuration { get => CustomOptionHolder.TricksterLightsOutDuration.GetFloat(); }

    [RegisterCustomButton]
    internal static void MakeButtons(HudManager hm)
    {
        PlaceJackInTheBoxButton = new(
            nameof(PlaceJackInTheBoxButton),
            () =>
            {
                PlaceJackInTheBoxButton.Timer = PlaceJackInTheBoxButton.MaxTimer;

                var pos = PlayerControl.LocalPlayer.transform.position;
                var buff = new byte[sizeof(float) * 2];
                Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.PlaceJackInTheBox);
                sender.WriteBytesAndSize(buff);
                RPCProcedure.PlaceJackInTheBox(buff);
            },
            () =>
            {
                return PlayerControl.LocalPlayer.IsRole(RoleType.Trickster) && PlayerControl.LocalPlayer.IsAlive() && !JackInTheBox.HasJackInTheBoxLimitReached();
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove && !JackInTheBox.HasJackInTheBoxLimitReached();
            },
            () =>
            {
                PlaceJackInTheBoxButton.Timer = PlaceJackInTheBoxButton.MaxTimer;
            },
            AssetLoader.PlaceJackInTheBoxButton,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            Tr.Get(TrKey.PlaceJackInTheBoxText));

        LightsOutButton = new(
            nameof(LightsOutButton),
            () =>
            {
                using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.LightsOut);
                RPCProcedure.LightsOut();
            },
            () =>
            {
                return PlayerControl.LocalPlayer.IsRole(RoleType.Trickster) && PlayerControl.LocalPlayer.IsAlive() && JackInTheBox.HasJackInTheBoxLimitReached() && JackInTheBox.BoxesConvertedToVents;
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove && JackInTheBox.HasJackInTheBoxLimitReached() && JackInTheBox.BoxesConvertedToVents;
            },
            () =>
            {
                LightsOutButton.Timer = LightsOutButton.MaxTimer;
                LightsOutButton.IsEffectActive = false;
                LightsOutButton.ActionButton.graphic.color = Palette.EnabledColor;
            },
            AssetLoader.LightsOutButton,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            true,
            LightsOutDuration,
            () =>
            {
                LightsOutButton.Timer = LightsOutButton.MaxTimer;
            },
            false,
            Tr.Get(TrKey.LightsOutText));
    }

    [SetCustomButtonTimer]
    internal static void SetButtonCooldowns()
    {
        PlaceJackInTheBoxButton.MaxTimer = PlaceBoxCooldown;
        LightsOutButton.MaxTimer = LightsOutCooldown;
        LightsOutButton.EffectDuration = LightsOutDuration;
    }

    internal static void Clear()
    {
        LightsOutTimer = 0f;

        ModRoleManager.RemoveRole(Instance);
        Instance = null;
    }
}