namespace RebuildUs.Modules.GameMode;

public static partial class HotPotato
{
    // Hot Potato button
    public static CustomButton hotPotatoButton;

    public static void SetButtonCooldowns()
    {
        // Hot Potato buttons
        hotPotatoButton.MaxTimer = HotPotato.transferCooldown;
    }

    public static void MakeButtons(HudManager __instance)
    {
        // Hot Potato buttons code
        // Hot Potato transfer
        hotPotatoButton = new CustomButton(
            () =>
            {

                hotPotatoButton.Timer = hotPotatoButton.MaxTimer;
                byte targetId = HotPotato.hotPotatoPlayerCurrentTarget.PlayerId;
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.HotPotatoTransfer, Hazel.SendOption.Reliable, -1);
                writer.Write(targetId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                RPCProcedure.hotPotatoTransfer(targetId);
            },
            () => { return HotPotato.hotPotatoPlayer != null && HotPotato.hotPotatoPlayer == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
            () =>
            {
                return HotPotato.hotPotatoPlayerCurrentTarget && PlayerControl.LocalPlayer.CanMove && HotPotato.timeforTransfer >= 1;
            },
            () => { hotPotatoButton.Timer = hotPotatoButton.MaxTimer; },
            AssetLoader.HotPotatoHotPotatusButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            Tr.Get(TrKey.HotPotatoButton)
        );
    }
}