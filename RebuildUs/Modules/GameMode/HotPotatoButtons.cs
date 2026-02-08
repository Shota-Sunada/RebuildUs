namespace RebuildUs.Modules.GameMode;

internal static partial class HotPotato
{
    // Hot Potato button
    public static CustomButton HotPotatoButton;

    public static void SetButtonCooldowns()
    {
        // Hot Potato buttons
        HotPotatoButton.MaxTimer = TransferCooldown;
    }

    public static void MakeButtons(HudManager __instance)
    {
        // Hot Potato buttons code
        // Hot Potato transfer
        HotPotatoButton = new(() =>
                              {
                                  HotPotatoButton.Timer = HotPotatoButton.MaxTimer;
                                  var targetId = _hotPotatoPlayerCurrentTarget.PlayerId;
                                  var writer =
                                      AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                                          (byte)CustomRPC.HotPotatoTransfer,
                                          SendOption.Reliable);
                                  writer.Write(targetId);
                                  AmongUsClient.Instance.FinishRpcImmediately(writer);

                                  RPCProcedure.HotPotatoTransfer(targetId);
                              },
                              () =>
                              {
                                  return HotPotatoPlayer != null
                                         && HotPotatoPlayer == PlayerControl.LocalPlayer
                                         && !PlayerControl.LocalPlayer.Data.IsDead;
                              },
                              () =>
                              {
                                  return _hotPotatoPlayerCurrentTarget
                                         && PlayerControl.LocalPlayer.CanMove
                                         && TimeforTransfer >= 1;
                              },
                              () => { HotPotatoButton.Timer = HotPotatoButton.MaxTimer; },
                              AssetLoader.HotPotatoHotPotatusButton,
                              ButtonPosition.Layout, __instance, __instance.UseButton,
                              AbilitySlot.CrewmateAbilityPrimary, false,
                              Tr.Get(TrKey.HotPotatoButton));
    }
}
