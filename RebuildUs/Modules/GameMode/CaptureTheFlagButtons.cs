namespace RebuildUs.Modules.GameMode;

public static partial class CaptureTheFlag
{
    // Capture the flag buttons
    private static CustomButton _redplayer01KillButton;
    private static CustomButton _redplayer01TakeFlagButton;
    private static CustomButton _redplayer02KillButton;
    private static CustomButton _redplayer02TakeFlagButton;
    private static CustomButton _redplayer03KillButton;
    private static CustomButton _redplayer03TakeFlagButton;
    private static CustomButton _redplayer04KillButton;
    private static CustomButton _redplayer04TakeFlagButton;
    private static CustomButton _redplayer05KillButton;
    private static CustomButton _redplayer05TakeFlagButton;
    private static CustomButton _redplayer06KillButton;
    private static CustomButton _redplayer06TakeFlagButton;
    private static CustomButton _redplayer07KillButton;
    private static CustomButton _redplayer07TakeFlagButton;
    private static CustomButton _blueplayer01KillButton;
    private static CustomButton _blueplayer01TakeFlagButton;
    private static CustomButton _blueplayer02KillButton;
    private static CustomButton _blueplayer02TakeFlagButton;
    private static CustomButton _blueplayer03KillButton;
    private static CustomButton _blueplayer03TakeFlagButton;
    private static CustomButton _blueplayer04KillButton;
    private static CustomButton _blueplayer04TakeFlagButton;
    private static CustomButton _blueplayer05KillButton;
    private static CustomButton _blueplayer05TakeFlagButton;
    private static CustomButton _blueplayer06KillButton;
    private static CustomButton _blueplayer06TakeFlagButton;
    private static CustomButton _blueplayer07KillButton;
    private static CustomButton _blueplayer07TakeFlagButton;
    private static CustomButton _stealerPlayerKillButton;

    public static void SetButtonCooldowns()
    {
        // Capture the flag buttons
        _redplayer01KillButton.MaxTimer = MapSettings.GamemodeKillCooldown;
        _redplayer01TakeFlagButton.MaxTimer = 0;
        _redplayer02KillButton.MaxTimer = MapSettings.GamemodeKillCooldown;
        _redplayer02TakeFlagButton.MaxTimer = 0;
        _redplayer03KillButton.MaxTimer = MapSettings.GamemodeKillCooldown;
        _redplayer03TakeFlagButton.MaxTimer = 0;
        _redplayer04KillButton.MaxTimer = MapSettings.GamemodeKillCooldown;
        _redplayer04TakeFlagButton.MaxTimer = 0;
        _redplayer05KillButton.MaxTimer = MapSettings.GamemodeKillCooldown;
        _redplayer05TakeFlagButton.MaxTimer = 0;
        _redplayer06KillButton.MaxTimer = MapSettings.GamemodeKillCooldown;
        _redplayer06TakeFlagButton.MaxTimer = 0;
        _redplayer07KillButton.MaxTimer = MapSettings.GamemodeKillCooldown;
        _redplayer07TakeFlagButton.MaxTimer = 0;
        _blueplayer01KillButton.MaxTimer = MapSettings.GamemodeKillCooldown;
        _blueplayer01TakeFlagButton.MaxTimer = 0;
        _blueplayer02KillButton.MaxTimer = MapSettings.GamemodeKillCooldown;
        _blueplayer02TakeFlagButton.MaxTimer = 0;
        _blueplayer03KillButton.MaxTimer = MapSettings.GamemodeKillCooldown;
        _blueplayer03TakeFlagButton.MaxTimer = 0;
        _blueplayer04KillButton.MaxTimer = MapSettings.GamemodeKillCooldown;
        _blueplayer04TakeFlagButton.MaxTimer = 0;
        _blueplayer05KillButton.MaxTimer = MapSettings.GamemodeKillCooldown;
        _blueplayer05TakeFlagButton.MaxTimer = 0;
        _blueplayer06KillButton.MaxTimer = MapSettings.GamemodeKillCooldown;
        _blueplayer06TakeFlagButton.MaxTimer = 0;
        _blueplayer07KillButton.MaxTimer = MapSettings.GamemodeKillCooldown;
        _blueplayer07TakeFlagButton.MaxTimer = 0;
        _stealerPlayerKillButton.MaxTimer = MapSettings.GamemodeKillCooldown;
    }

    public static void MakeButtons(HudManager __instance)
    {
        // Capture the flag buttons
        // Redplayer01 Kill
        _redplayer01KillButton = new(() =>
        {
            var targetId = Redplayer01CurrentTarget.PlayerId;
            var sourceId = Redplayer01.PlayerId;
            var killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            killWriter.Write(targetId);
            killWriter.Write(sourceId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.GamemodeKills(targetId, sourceId);
            _redplayer01KillButton.Timer = _redplayer01KillButton.MaxTimer;
            Redplayer01CurrentTarget = null;
        }, () => { return Redplayer01 != null && Redplayer01 == PlayerControl.LocalPlayer; }, () => { return Redplayer01CurrentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !Redplayer01IsReviving && PlayerControl.LocalPlayer != RedPlayerWhoHasBlueFlag; }, () => { _redplayer01KillButton.Timer = _redplayer01KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // Redplayer01 TakeFlag Button
        _redplayer01TakeFlagButton = new(() =>
        {
            if (PlayerControl.LocalPlayer == RedPlayerWhoHasBlueFlag)
            {
                var whichTeamScored = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhichTeamScored, SendOption.Reliable);
                whichTeamScored.Write(1);
                AmongUsClient.Instance.FinishRpcImmediately(whichTeamScored);
                RPCProcedure.CaptureTheFlagWhichTeamScored(1);
            }
            else
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var redPlayerStoleBlueFlag = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhoTookTheFlag, SendOption.Reliable);
                redPlayerStoleBlueFlag.Write(targetId);
                redPlayerStoleBlueFlag.Write(1);
                AmongUsClient.Instance.FinishRpcImmediately(redPlayerStoleBlueFlag);
                RPCProcedure.CaptureTheFlagWhoTookTheFlag(targetId, 1);
            }

            _redplayer01TakeFlagButton.Timer = _redplayer01TakeFlagButton.MaxTimer;
        }, () => { return Redplayer01 != null && Redplayer01 == PlayerControl.LocalPlayer; }, () =>
        {
            if (LocalRedFlagArrow.Count != 0) LocalRedFlagArrow[0].Update(Redflag.transform.position);
            if (LocalBlueFlagArrow.Count != 0) LocalBlueFlagArrow[0].Update(Blueflag.transform.position);
            if (Redplayer01 == RedPlayerWhoHasBlueFlag)
                _redplayer01TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagDeliverBlueFlagButton;
            else
                _redplayer01TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagStealBlueFlagButton;
            var canUse = false;
            if (Blueflag != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Blueflag.transform.position) < 0.5f && !PlayerControl.LocalPlayer.Data.IsDead && RedPlayerWhoHasBlueFlag == null)
                canUse = true;
            else if (Redflagbase != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Redflagbase.transform.position) < 0.5f && PlayerControl.LocalPlayer == RedPlayerWhoHasBlueFlag) canUse = true;
            return canUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _redplayer01TakeFlagButton.Timer = _redplayer01TakeFlagButton.MaxTimer; }, AssetLoader.CaptureTheFlagStealBlueFlagButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CrewmateAbilityPrimary, false, Tr.Get(TrKey.StealButton));

        // Redplayer02 Kill
        _redplayer02KillButton = new(() =>
        {
            var targetId = Redplayer02CurrentTarget.PlayerId;
            var sourceId = Redplayer02.PlayerId;
            var killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            killWriter.Write(targetId);
            killWriter.Write(sourceId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.GamemodeKills(targetId, sourceId);
            _redplayer02KillButton.Timer = _redplayer02KillButton.MaxTimer;
            Redplayer02CurrentTarget = null;
        }, () => { return Redplayer02 != null && Redplayer02 == PlayerControl.LocalPlayer; }, () => { return Redplayer02CurrentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !Redplayer02IsReviving && PlayerControl.LocalPlayer != RedPlayerWhoHasBlueFlag; }, () => { _redplayer02KillButton.Timer = _redplayer02KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // Redplayer02 TakeFlag Button
        _redplayer02TakeFlagButton = new(() =>
        {
            if (PlayerControl.LocalPlayer == RedPlayerWhoHasBlueFlag)
            {
                var whichTeamScored = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhichTeamScored, SendOption.Reliable);
                whichTeamScored.Write(1);
                AmongUsClient.Instance.FinishRpcImmediately(whichTeamScored);
                RPCProcedure.CaptureTheFlagWhichTeamScored(1);
            }
            else
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var redPlayerStoleBlueFlag = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhoTookTheFlag, SendOption.Reliable);
                redPlayerStoleBlueFlag.Write(targetId);
                redPlayerStoleBlueFlag.Write(1);
                AmongUsClient.Instance.FinishRpcImmediately(redPlayerStoleBlueFlag);
                RPCProcedure.CaptureTheFlagWhoTookTheFlag(targetId, 1);
            }

            _redplayer02TakeFlagButton.Timer = _redplayer02TakeFlagButton.MaxTimer;
        }, () => { return Redplayer02 != null && Redplayer02 == PlayerControl.LocalPlayer; }, () =>
        {
            if (LocalRedFlagArrow.Count != 0) LocalRedFlagArrow[0].Update(Redflag.transform.position);
            if (LocalBlueFlagArrow.Count != 0) LocalBlueFlagArrow[0].Update(Blueflag.transform.position);
            if (Redplayer02 == RedPlayerWhoHasBlueFlag)
                _redplayer02TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagDeliverBlueFlagButton;
            else
                _redplayer02TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagStealBlueFlagButton;
            var canUse = false;
            if (Blueflag != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Blueflag.transform.position) < 0.5f && !PlayerControl.LocalPlayer.Data.IsDead && RedPlayerWhoHasBlueFlag == null)
                canUse = true;
            else if (Redflagbase != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Redflagbase.transform.position) < 0.5f && PlayerControl.LocalPlayer == RedPlayerWhoHasBlueFlag) canUse = true;
            return canUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _redplayer02TakeFlagButton.Timer = _redplayer02TakeFlagButton.MaxTimer; }, AssetLoader.CaptureTheFlagStealBlueFlagButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CrewmateAbilityPrimary, false, Tr.Get(TrKey.StealButton));

        // Redplayer03 Kill
        _redplayer03KillButton = new(() =>
        {
            var targetId = Redplayer03CurrentTarget.PlayerId;
            var sourceId = Redplayer03.PlayerId;
            var killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            killWriter.Write(targetId);
            killWriter.Write(sourceId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.GamemodeKills(targetId, sourceId);
            _redplayer03KillButton.Timer = _redplayer03KillButton.MaxTimer;
            Redplayer03CurrentTarget = null;
        }, () => { return Redplayer03 != null && Redplayer03 == PlayerControl.LocalPlayer; }, () => { return Redplayer03CurrentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !Redplayer03IsReviving && PlayerControl.LocalPlayer != RedPlayerWhoHasBlueFlag; }, () => { _redplayer03KillButton.Timer = _redplayer03KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // Redplayer03 TakeFlag Button
        _redplayer03TakeFlagButton = new(() =>
        {
            if (PlayerControl.LocalPlayer == RedPlayerWhoHasBlueFlag)
            {
                var whichTeamScored = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhichTeamScored, SendOption.Reliable);
                whichTeamScored.Write(1);
                AmongUsClient.Instance.FinishRpcImmediately(whichTeamScored);
                RPCProcedure.CaptureTheFlagWhichTeamScored(1);
            }
            else
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var redPlayerStoleBlueFlag = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhoTookTheFlag, SendOption.Reliable);
                redPlayerStoleBlueFlag.Write(targetId);
                redPlayerStoleBlueFlag.Write(1);
                AmongUsClient.Instance.FinishRpcImmediately(redPlayerStoleBlueFlag);
                RPCProcedure.CaptureTheFlagWhoTookTheFlag(targetId, 1);
            }

            _redplayer03TakeFlagButton.Timer = _redplayer03TakeFlagButton.MaxTimer;
        }, () => { return Redplayer03 != null && Redplayer03 == PlayerControl.LocalPlayer; }, () =>
        {
            if (LocalRedFlagArrow.Count != 0) LocalRedFlagArrow[0].Update(Redflag.transform.position);
            if (LocalBlueFlagArrow.Count != 0) LocalBlueFlagArrow[0].Update(Blueflag.transform.position);
            if (Redplayer03 == RedPlayerWhoHasBlueFlag)
                _redplayer03TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagDeliverBlueFlagButton;
            else
                _redplayer03TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagStealBlueFlagButton;
            var canUse = false;
            if (Blueflag != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Blueflag.transform.position) < 0.5f && !PlayerControl.LocalPlayer.Data.IsDead && RedPlayerWhoHasBlueFlag == null)
                canUse = true;
            else if (Redflagbase != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Redflagbase.transform.position) < 0.5f && PlayerControl.LocalPlayer == RedPlayerWhoHasBlueFlag) canUse = true;
            return canUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _redplayer03TakeFlagButton.Timer = _redplayer03TakeFlagButton.MaxTimer; }, AssetLoader.CaptureTheFlagStealBlueFlagButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CrewmateAbilityPrimary, false, Tr.Get(TrKey.StealButton));

        // Redplayer04 Kill
        _redplayer04KillButton = new(() =>
        {
            var targetId = Redplayer04CurrentTarget.PlayerId;
            var sourceId = Redplayer04.PlayerId;
            var killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            killWriter.Write(targetId);
            killWriter.Write(sourceId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.GamemodeKills(targetId, sourceId);
            _redplayer04KillButton.Timer = _redplayer04KillButton.MaxTimer;
            Redplayer04CurrentTarget = null;
        }, () => { return Redplayer04 != null && Redplayer04 == PlayerControl.LocalPlayer; }, () => { return Redplayer04CurrentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !Redplayer04IsReviving && PlayerControl.LocalPlayer != RedPlayerWhoHasBlueFlag; }, () => { _redplayer04KillButton.Timer = _redplayer04KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // Redplayer04 TakeFlag Button
        _redplayer04TakeFlagButton = new(() =>
        {
            if (PlayerControl.LocalPlayer == RedPlayerWhoHasBlueFlag)
            {
                var whichTeamScored = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhichTeamScored, SendOption.Reliable);
                whichTeamScored.Write(1);
                AmongUsClient.Instance.FinishRpcImmediately(whichTeamScored);
                RPCProcedure.CaptureTheFlagWhichTeamScored(1);
            }
            else
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var redPlayerStoleBlueFlag = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhoTookTheFlag, SendOption.Reliable);
                redPlayerStoleBlueFlag.Write(targetId);
                redPlayerStoleBlueFlag.Write(1);
                AmongUsClient.Instance.FinishRpcImmediately(redPlayerStoleBlueFlag);
                RPCProcedure.CaptureTheFlagWhoTookTheFlag(targetId, 1);
            }

            _redplayer04TakeFlagButton.Timer = _redplayer04TakeFlagButton.MaxTimer;
        }, () => { return Redplayer04 != null && Redplayer04 == PlayerControl.LocalPlayer; }, () =>
        {
            if (LocalRedFlagArrow.Count != 0) LocalRedFlagArrow[0].Update(Redflag.transform.position);
            if (LocalBlueFlagArrow.Count != 0) LocalBlueFlagArrow[0].Update(Blueflag.transform.position);
            if (Redplayer04 == RedPlayerWhoHasBlueFlag)
                _redplayer04TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagDeliverBlueFlagButton;
            else
                _redplayer04TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagStealBlueFlagButton;
            var canUse = false;
            if (Blueflag != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Blueflag.transform.position) < 0.5f && !PlayerControl.LocalPlayer.Data.IsDead && RedPlayerWhoHasBlueFlag == null)
                canUse = true;
            else if (Redflagbase != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Redflagbase.transform.position) < 0.5f && PlayerControl.LocalPlayer == RedPlayerWhoHasBlueFlag) canUse = true;
            return canUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _redplayer04TakeFlagButton.Timer = _redplayer04TakeFlagButton.MaxTimer; }, AssetLoader.CaptureTheFlagStealBlueFlagButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CrewmateAbilityPrimary, false, Tr.Get(TrKey.StealButton));

        // Redplayer05 Kill
        _redplayer05KillButton = new(() =>
        {
            var targetId = Redplayer05CurrentTarget.PlayerId;
            var sourceId = Redplayer05.PlayerId;
            var killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            killWriter.Write(targetId);
            killWriter.Write(sourceId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.GamemodeKills(targetId, sourceId);
            _redplayer05KillButton.Timer = _redplayer05KillButton.MaxTimer;
            Redplayer05CurrentTarget = null;
        }, () => { return Redplayer05 != null && Redplayer05 == PlayerControl.LocalPlayer; }, () => { return Redplayer05CurrentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !Redplayer05IsReviving && PlayerControl.LocalPlayer != RedPlayerWhoHasBlueFlag; }, () => { _redplayer05KillButton.Timer = _redplayer05KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // Redplayer05 TakeFlag Button
        _redplayer05TakeFlagButton = new(() =>
        {
            if (PlayerControl.LocalPlayer == RedPlayerWhoHasBlueFlag)
            {
                var whichTeamScored = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhichTeamScored, SendOption.Reliable);
                whichTeamScored.Write(1);
                AmongUsClient.Instance.FinishRpcImmediately(whichTeamScored);
                RPCProcedure.CaptureTheFlagWhichTeamScored(1);
            }
            else
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var redPlayerStoleBlueFlag = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhoTookTheFlag, SendOption.Reliable);
                redPlayerStoleBlueFlag.Write(targetId);
                redPlayerStoleBlueFlag.Write(1);
                AmongUsClient.Instance.FinishRpcImmediately(redPlayerStoleBlueFlag);
                RPCProcedure.CaptureTheFlagWhoTookTheFlag(targetId, 1);
            }

            _redplayer05TakeFlagButton.Timer = _redplayer05TakeFlagButton.MaxTimer;
        }, () => { return Redplayer05 != null && Redplayer05 == PlayerControl.LocalPlayer; }, () =>
        {
            if (LocalRedFlagArrow.Count != 0) LocalRedFlagArrow[0].Update(Redflag.transform.position);
            if (LocalBlueFlagArrow.Count != 0) LocalBlueFlagArrow[0].Update(Blueflag.transform.position);
            if (Redplayer05 == RedPlayerWhoHasBlueFlag)
                _redplayer05TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagDeliverBlueFlagButton;
            else
                _redplayer05TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagStealBlueFlagButton;
            var canUse = false;
            if (Blueflag != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Blueflag.transform.position) < 0.5f && !PlayerControl.LocalPlayer.Data.IsDead && RedPlayerWhoHasBlueFlag == null)
                canUse = true;
            else if (Redflagbase != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Redflagbase.transform.position) < 0.5f && PlayerControl.LocalPlayer == RedPlayerWhoHasBlueFlag) canUse = true;
            return canUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _redplayer05TakeFlagButton.Timer = _redplayer05TakeFlagButton.MaxTimer; }, AssetLoader.CaptureTheFlagStealBlueFlagButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CrewmateAbilityPrimary, false, Tr.Get(TrKey.StealButton));

        // Redplayer06 Kill
        _redplayer06KillButton = new(() =>
        {
            var targetId = Redplayer06CurrentTarget.PlayerId;
            var sourceId = Redplayer06.PlayerId;
            var killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            killWriter.Write(targetId);
            killWriter.Write(sourceId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.GamemodeKills(targetId, sourceId);
            _redplayer06KillButton.Timer = _redplayer06KillButton.MaxTimer;
            Redplayer06CurrentTarget = null;
        }, () => { return Redplayer06 != null && Redplayer06 == PlayerControl.LocalPlayer; }, () => { return Redplayer06CurrentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !Redplayer06IsReviving && PlayerControl.LocalPlayer != RedPlayerWhoHasBlueFlag; }, () => { _redplayer06KillButton.Timer = _redplayer06KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // Redplayer06 TakeFlag Button
        _redplayer06TakeFlagButton = new(() =>
        {
            if (PlayerControl.LocalPlayer == RedPlayerWhoHasBlueFlag)
            {
                var whichTeamScored = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhichTeamScored, SendOption.Reliable);
                whichTeamScored.Write(1);
                AmongUsClient.Instance.FinishRpcImmediately(whichTeamScored);
                RPCProcedure.CaptureTheFlagWhichTeamScored(1);
            }
            else
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var redPlayerStoleBlueFlag = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhoTookTheFlag, SendOption.Reliable);
                redPlayerStoleBlueFlag.Write(targetId);
                redPlayerStoleBlueFlag.Write(1);
                AmongUsClient.Instance.FinishRpcImmediately(redPlayerStoleBlueFlag);
                RPCProcedure.CaptureTheFlagWhoTookTheFlag(targetId, 1);
            }

            _redplayer06TakeFlagButton.Timer = _redplayer06TakeFlagButton.MaxTimer;
        }, () => { return Redplayer06 != null && Redplayer06 == PlayerControl.LocalPlayer; }, () =>
        {
            if (LocalRedFlagArrow.Count != 0) LocalRedFlagArrow[0].Update(Redflag.transform.position);
            if (LocalBlueFlagArrow.Count != 0) LocalBlueFlagArrow[0].Update(Blueflag.transform.position);
            if (Redplayer06 == RedPlayerWhoHasBlueFlag)
                _redplayer06TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagDeliverBlueFlagButton;
            else
                _redplayer06TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagStealBlueFlagButton;
            var canUse = false;
            if (Blueflag != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Blueflag.transform.position) < 0.5f && !PlayerControl.LocalPlayer.Data.IsDead && RedPlayerWhoHasBlueFlag == null)
                canUse = true;
            else if (Redflagbase != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Redflagbase.transform.position) < 0.5f && PlayerControl.LocalPlayer == RedPlayerWhoHasBlueFlag) canUse = true;
            return canUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _redplayer06TakeFlagButton.Timer = _redplayer06TakeFlagButton.MaxTimer; }, AssetLoader.CaptureTheFlagStealBlueFlagButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CrewmateAbilityPrimary, false, Tr.Get(TrKey.StealButton));

        // Redplayer07  Kill
        _redplayer07KillButton = new(() =>
        {
            var targetId = Redplayer07CurrentTarget.PlayerId;
            var sourceId = Redplayer07.PlayerId;
            var killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            killWriter.Write(targetId);
            killWriter.Write(sourceId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.GamemodeKills(targetId, sourceId);
            _redplayer07KillButton.Timer = _redplayer07KillButton.MaxTimer;
            Redplayer07CurrentTarget = null;
        }, () => { return Redplayer07 != null && Redplayer07 == PlayerControl.LocalPlayer; }, () => { return Redplayer07CurrentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !Redplayer07IsReviving && PlayerControl.LocalPlayer != RedPlayerWhoHasBlueFlag; }, () => { _redplayer07KillButton.Timer = _redplayer07KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // Redplayer07 TakeFlag Button
        _redplayer07TakeFlagButton = new(() =>
        {
            if (PlayerControl.LocalPlayer == RedPlayerWhoHasBlueFlag)
            {
                var whichTeamScored = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhichTeamScored, SendOption.Reliable);
                whichTeamScored.Write(1);
                AmongUsClient.Instance.FinishRpcImmediately(whichTeamScored);
                RPCProcedure.CaptureTheFlagWhichTeamScored(1);
            }
            else
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var redPlayerStoleBlueFlag = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhoTookTheFlag, SendOption.Reliable);
                redPlayerStoleBlueFlag.Write(targetId);
                redPlayerStoleBlueFlag.Write(1);
                AmongUsClient.Instance.FinishRpcImmediately(redPlayerStoleBlueFlag);
                RPCProcedure.CaptureTheFlagWhoTookTheFlag(targetId, 1);
            }

            _redplayer07TakeFlagButton.Timer = _redplayer07TakeFlagButton.MaxTimer;
        }, () => { return Redplayer07 != null && Redplayer07 == PlayerControl.LocalPlayer; }, () =>
        {
            if (LocalRedFlagArrow.Count != 0) LocalRedFlagArrow[0].Update(Redflag.transform.position);
            if (LocalBlueFlagArrow.Count != 0) LocalBlueFlagArrow[0].Update(Blueflag.transform.position);
            if (Redplayer07 == RedPlayerWhoHasBlueFlag)
                _redplayer07TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagDeliverBlueFlagButton;
            else
                _redplayer07TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagStealBlueFlagButton;
            var canUse = false;
            if (Blueflag != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Blueflag.transform.position) < 0.5f && !PlayerControl.LocalPlayer.Data.IsDead && RedPlayerWhoHasBlueFlag == null)
                canUse = true;
            else if (Redflagbase != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Redflagbase.transform.position) < 0.5f && PlayerControl.LocalPlayer == RedPlayerWhoHasBlueFlag) canUse = true;
            return canUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _redplayer07TakeFlagButton.Timer = _redplayer07TakeFlagButton.MaxTimer; }, AssetLoader.CaptureTheFlagStealBlueFlagButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CrewmateAbilityPrimary, false, Tr.Get(TrKey.StealButton));

        // Blueplayer01 Kill
        _blueplayer01KillButton = new(() =>
        {
            var targetId = Blueplayer01CurrentTarget.PlayerId;
            var sourceId = Blueplayer01.PlayerId;
            var killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            killWriter.Write(targetId);
            killWriter.Write(sourceId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.GamemodeKills(targetId, sourceId);
            _blueplayer01KillButton.Timer = _blueplayer01KillButton.MaxTimer;
            Blueplayer01CurrentTarget = null;
        }, () => { return Blueplayer01 != null && Blueplayer01 == PlayerControl.LocalPlayer; }, () => { return Blueplayer01CurrentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !Blueplayer01IsReviving && PlayerControl.LocalPlayer != BluePlayerWhoHasRedFlag; }, () => { _blueplayer01KillButton.Timer = _blueplayer01KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // Blueplayer01 TakeFlag Button
        _blueplayer01TakeFlagButton = new(() =>
        {
            if (PlayerControl.LocalPlayer == BluePlayerWhoHasRedFlag)
            {
                var whichTeamScored = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhichTeamScored, SendOption.Reliable);
                whichTeamScored.Write(2);
                AmongUsClient.Instance.FinishRpcImmediately(whichTeamScored);
                RPCProcedure.CaptureTheFlagWhichTeamScored(2);
            }
            else
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var bluePlayerStoleRedFlag = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhoTookTheFlag, SendOption.Reliable);
                bluePlayerStoleRedFlag.Write(targetId);
                bluePlayerStoleRedFlag.Write(2);
                AmongUsClient.Instance.FinishRpcImmediately(bluePlayerStoleRedFlag);
                RPCProcedure.CaptureTheFlagWhoTookTheFlag(targetId, 2);
            }

            _blueplayer01TakeFlagButton.Timer = _blueplayer01TakeFlagButton.MaxTimer;
        }, () => { return Blueplayer01 != null && Blueplayer01 == PlayerControl.LocalPlayer; }, () =>
        {
            if (LocalRedFlagArrow.Count != 0) LocalRedFlagArrow[0].Update(Redflag.transform.position);
            if (LocalBlueFlagArrow.Count != 0) LocalBlueFlagArrow[0].Update(Blueflag.transform.position);
            if (Blueplayer01 == BluePlayerWhoHasRedFlag)
                _blueplayer01TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagDeliverRedFlagButton;
            else
                _blueplayer01TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagStealRedFlagButton;
            var canUse = false;
            if (Redflag != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Redflag.transform.position) < 0.5f && !PlayerControl.LocalPlayer.Data.IsDead && BluePlayerWhoHasRedFlag == null)
                canUse = true;
            else if (Blueflagbase != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Blueflagbase.transform.position) < 0.5f && PlayerControl.LocalPlayer == BluePlayerWhoHasRedFlag) canUse = true;
            return canUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _blueplayer01TakeFlagButton.Timer = _blueplayer01TakeFlagButton.MaxTimer; }, AssetLoader.CaptureTheFlagStealRedFlagButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CrewmateAbilityPrimary, false, Tr.Get(TrKey.StealButton));

        // Blueplayer02 Kill
        _blueplayer02KillButton = new(() =>
        {
            var targetId = Blueplayer02CurrentTarget.PlayerId;
            var sourceId = Blueplayer02.PlayerId;
            var killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            killWriter.Write(targetId);
            killWriter.Write(sourceId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.GamemodeKills(targetId, sourceId);
            _blueplayer02KillButton.Timer = _blueplayer02KillButton.MaxTimer;
            Blueplayer02CurrentTarget = null;
        }, () => { return Blueplayer02 != null && Blueplayer02 == PlayerControl.LocalPlayer; }, () => { return Blueplayer02CurrentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !Blueplayer02IsReviving && PlayerControl.LocalPlayer != BluePlayerWhoHasRedFlag; }, () => { _blueplayer02KillButton.Timer = _blueplayer02KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // Blueplayer02 TakeFlag Button
        _blueplayer02TakeFlagButton = new(() =>
        {
            if (PlayerControl.LocalPlayer == BluePlayerWhoHasRedFlag)
            {
                var whichTeamScored = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhichTeamScored, SendOption.Reliable);
                whichTeamScored.Write(2);
                AmongUsClient.Instance.FinishRpcImmediately(whichTeamScored);
                RPCProcedure.CaptureTheFlagWhichTeamScored(2);
            }
            else
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var bluePlayerStoleRedFlag = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhoTookTheFlag, SendOption.Reliable);
                bluePlayerStoleRedFlag.Write(targetId);
                bluePlayerStoleRedFlag.Write(2);
                AmongUsClient.Instance.FinishRpcImmediately(bluePlayerStoleRedFlag);
                RPCProcedure.CaptureTheFlagWhoTookTheFlag(targetId, 2);
            }

            _blueplayer02TakeFlagButton.Timer = _blueplayer02TakeFlagButton.MaxTimer;
        }, () => { return Blueplayer02 != null && Blueplayer02 == PlayerControl.LocalPlayer; }, () =>
        {
            if (LocalRedFlagArrow.Count != 0) LocalRedFlagArrow[0].Update(Redflag.transform.position);
            if (LocalBlueFlagArrow.Count != 0) LocalBlueFlagArrow[0].Update(Blueflag.transform.position);
            if (Blueplayer02 == BluePlayerWhoHasRedFlag)
                _blueplayer02TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagDeliverRedFlagButton;
            else
                _blueplayer02TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagStealRedFlagButton;
            var canUse = false;
            if (Redflag != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Redflag.transform.position) < 0.5f && !PlayerControl.LocalPlayer.Data.IsDead && BluePlayerWhoHasRedFlag == null)
                canUse = true;
            else if (Blueflagbase != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Blueflagbase.transform.position) < 0.5f && PlayerControl.LocalPlayer == BluePlayerWhoHasRedFlag) canUse = true;
            return canUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _blueplayer02TakeFlagButton.Timer = _blueplayer02TakeFlagButton.MaxTimer; }, AssetLoader.CaptureTheFlagStealRedFlagButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CrewmateAbilityPrimary, false, Tr.Get(TrKey.StealButton));

        // Blueplayer03 Kill
        _blueplayer03KillButton = new(() =>
        {
            var targetId = Blueplayer03CurrentTarget.PlayerId;
            var sourceId = Blueplayer03.PlayerId;
            var killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            killWriter.Write(targetId);
            killWriter.Write(sourceId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.GamemodeKills(targetId, sourceId);
            _blueplayer03KillButton.Timer = _blueplayer03KillButton.MaxTimer;
            Blueplayer03CurrentTarget = null;
        }, () => { return Blueplayer03 != null && Blueplayer03 == PlayerControl.LocalPlayer; }, () => { return Blueplayer03CurrentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !Blueplayer03IsReviving && PlayerControl.LocalPlayer != BluePlayerWhoHasRedFlag; }, () => { _blueplayer03KillButton.Timer = _blueplayer03KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // Blueplayer03 TakeFlag Button
        _blueplayer03TakeFlagButton = new(() =>
        {
            if (PlayerControl.LocalPlayer == BluePlayerWhoHasRedFlag)
            {
                var whichTeamScored = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhichTeamScored, SendOption.Reliable);
                whichTeamScored.Write(2);
                AmongUsClient.Instance.FinishRpcImmediately(whichTeamScored);
                RPCProcedure.CaptureTheFlagWhichTeamScored(2);
            }
            else
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var bluePlayerStoleRedFlag = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhoTookTheFlag, SendOption.Reliable);
                bluePlayerStoleRedFlag.Write(targetId);
                bluePlayerStoleRedFlag.Write(2);
                AmongUsClient.Instance.FinishRpcImmediately(bluePlayerStoleRedFlag);
                RPCProcedure.CaptureTheFlagWhoTookTheFlag(targetId, 2);
            }

            _blueplayer03TakeFlagButton.Timer = _blueplayer03TakeFlagButton.MaxTimer;
        }, () => { return Blueplayer03 != null && Blueplayer03 == PlayerControl.LocalPlayer; }, () =>
        {
            if (LocalRedFlagArrow.Count != 0) LocalRedFlagArrow[0].Update(Redflag.transform.position);
            if (LocalBlueFlagArrow.Count != 0) LocalBlueFlagArrow[0].Update(Blueflag.transform.position);
            if (Blueplayer03 == BluePlayerWhoHasRedFlag)
                _blueplayer03TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagDeliverRedFlagButton;
            else
                _blueplayer03TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagStealRedFlagButton;
            var canUse = false;
            if (Redflag != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Redflag.transform.position) < 0.5f && !PlayerControl.LocalPlayer.Data.IsDead && BluePlayerWhoHasRedFlag == null)
                canUse = true;
            else if (Blueflagbase != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Blueflagbase.transform.position) < 0.5f && PlayerControl.LocalPlayer == BluePlayerWhoHasRedFlag) canUse = true;
            return canUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _blueplayer03TakeFlagButton.Timer = _blueplayer03TakeFlagButton.MaxTimer; }, AssetLoader.CaptureTheFlagStealRedFlagButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CrewmateAbilityPrimary, false, Tr.Get(TrKey.StealButton));

        // Blueplayer04 Kill
        _blueplayer04KillButton = new(() =>
        {
            var targetId = Blueplayer04CurrentTarget.PlayerId;
            var sourceId = Blueplayer04.PlayerId;
            var killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            killWriter.Write(targetId);
            killWriter.Write(sourceId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.GamemodeKills(targetId, sourceId);
            _blueplayer04KillButton.Timer = _blueplayer04KillButton.MaxTimer;
            Blueplayer04CurrentTarget = null;
        }, () => { return Blueplayer04 != null && Blueplayer04 == PlayerControl.LocalPlayer; }, () => { return Blueplayer04CurrentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !Blueplayer04IsReviving && PlayerControl.LocalPlayer != BluePlayerWhoHasRedFlag; }, () => { _blueplayer04KillButton.Timer = _blueplayer04KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // Blueplayer04 TakeFlag Button
        _blueplayer04TakeFlagButton = new(() =>
        {
            if (PlayerControl.LocalPlayer == BluePlayerWhoHasRedFlag)
            {
                var whichTeamScored = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhichTeamScored, SendOption.Reliable);
                whichTeamScored.Write(2);
                AmongUsClient.Instance.FinishRpcImmediately(whichTeamScored);
                RPCProcedure.CaptureTheFlagWhichTeamScored(2);
            }
            else
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var bluePlayerStoleRedFlag = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhoTookTheFlag, SendOption.Reliable);
                bluePlayerStoleRedFlag.Write(targetId);
                bluePlayerStoleRedFlag.Write(2);
                AmongUsClient.Instance.FinishRpcImmediately(bluePlayerStoleRedFlag);
                RPCProcedure.CaptureTheFlagWhoTookTheFlag(targetId, 2);
            }

            _blueplayer04TakeFlagButton.Timer = _blueplayer04TakeFlagButton.MaxTimer;
        }, () => { return Blueplayer04 != null && Blueplayer04 == PlayerControl.LocalPlayer; }, () =>
        {
            if (LocalRedFlagArrow.Count != 0) LocalRedFlagArrow[0].Update(Redflag.transform.position);
            if (LocalBlueFlagArrow.Count != 0) LocalBlueFlagArrow[0].Update(Blueflag.transform.position);
            if (Blueplayer04 == BluePlayerWhoHasRedFlag)
                _blueplayer04TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagDeliverRedFlagButton;
            else
                _blueplayer04TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagStealRedFlagButton;
            var canUse = false;
            if (Redflag != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Redflag.transform.position) < 0.5f && !PlayerControl.LocalPlayer.Data.IsDead && BluePlayerWhoHasRedFlag == null)
                canUse = true;
            else if (Blueflagbase != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Blueflagbase.transform.position) < 0.5f && PlayerControl.LocalPlayer == BluePlayerWhoHasRedFlag) canUse = true;
            return canUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _blueplayer04TakeFlagButton.Timer = _blueplayer04TakeFlagButton.MaxTimer; }, AssetLoader.CaptureTheFlagStealRedFlagButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CrewmateAbilityPrimary, false, Tr.Get(TrKey.StealButton));

        // Blueplayer05 Kill
        _blueplayer05KillButton = new(() =>
        {
            var targetId = Blueplayer05CurrentTarget.PlayerId;
            var sourceId = Blueplayer05.PlayerId;
            var killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            killWriter.Write(targetId);
            killWriter.Write(sourceId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.GamemodeKills(targetId, sourceId);
            _blueplayer05KillButton.Timer = _blueplayer05KillButton.MaxTimer;
            Blueplayer05CurrentTarget = null;
        }, () => { return Blueplayer05 != null && Blueplayer05 == PlayerControl.LocalPlayer; }, () => { return Blueplayer05CurrentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !Blueplayer05IsReviving && PlayerControl.LocalPlayer != BluePlayerWhoHasRedFlag; }, () => { _blueplayer05KillButton.Timer = _blueplayer05KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // Blueplayer05 TakeFlag Button
        _blueplayer05TakeFlagButton = new(() =>
        {
            if (PlayerControl.LocalPlayer == BluePlayerWhoHasRedFlag)
            {
                var whichTeamScored = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhichTeamScored, SendOption.Reliable);
                whichTeamScored.Write(2);
                AmongUsClient.Instance.FinishRpcImmediately(whichTeamScored);
                RPCProcedure.CaptureTheFlagWhichTeamScored(2);
            }
            else
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var bluePlayerStoleRedFlag = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhoTookTheFlag, SendOption.Reliable);
                bluePlayerStoleRedFlag.Write(targetId);
                bluePlayerStoleRedFlag.Write(2);
                AmongUsClient.Instance.FinishRpcImmediately(bluePlayerStoleRedFlag);
                RPCProcedure.CaptureTheFlagWhoTookTheFlag(targetId, 2);
            }

            _blueplayer05TakeFlagButton.Timer = _blueplayer05TakeFlagButton.MaxTimer;
        }, () => { return Blueplayer05 != null && Blueplayer05 == PlayerControl.LocalPlayer; }, () =>
        {
            if (LocalRedFlagArrow.Count != 0) LocalRedFlagArrow[0].Update(Redflag.transform.position);
            if (LocalBlueFlagArrow.Count != 0) LocalBlueFlagArrow[0].Update(Blueflag.transform.position);
            if (Blueplayer05 == BluePlayerWhoHasRedFlag)
                _blueplayer05TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagDeliverRedFlagButton;
            else
                _blueplayer05TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagStealRedFlagButton;
            var canUse = false;
            if (Redflag != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Redflag.transform.position) < 0.5f && !PlayerControl.LocalPlayer.Data.IsDead && BluePlayerWhoHasRedFlag == null)
                canUse = true;
            else if (Blueflagbase != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Blueflagbase.transform.position) < 0.5f && PlayerControl.LocalPlayer == BluePlayerWhoHasRedFlag) canUse = true;
            return canUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _blueplayer05TakeFlagButton.Timer = _blueplayer05TakeFlagButton.MaxTimer; }, AssetLoader.CaptureTheFlagStealRedFlagButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CrewmateAbilityPrimary, false, Tr.Get(TrKey.StealButton));

        // Blueplayer06 Kill
        _blueplayer06KillButton = new(() =>
        {
            var targetId = Blueplayer06CurrentTarget.PlayerId;
            var sourceId = Blueplayer06.PlayerId;
            var killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            killWriter.Write(targetId);
            killWriter.Write(sourceId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.GamemodeKills(targetId, sourceId);
            _blueplayer06KillButton.Timer = _blueplayer06KillButton.MaxTimer;
            Blueplayer06CurrentTarget = null;
        }, () => { return Blueplayer06 != null && Blueplayer06 == PlayerControl.LocalPlayer; }, () => { return Blueplayer06CurrentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !Blueplayer06IsReviving && PlayerControl.LocalPlayer != BluePlayerWhoHasRedFlag; }, () => { _blueplayer06KillButton.Timer = _blueplayer06KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // Blueplayer06 TakeFlag Button
        _blueplayer06TakeFlagButton = new(() =>
        {
            if (PlayerControl.LocalPlayer == BluePlayerWhoHasRedFlag)
            {
                var whichTeamScored = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhichTeamScored, SendOption.Reliable);
                whichTeamScored.Write(2);
                AmongUsClient.Instance.FinishRpcImmediately(whichTeamScored);
                RPCProcedure.CaptureTheFlagWhichTeamScored(2);
            }
            else
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var bluePlayerStoleRedFlag = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhoTookTheFlag, SendOption.Reliable);
                bluePlayerStoleRedFlag.Write(targetId);
                bluePlayerStoleRedFlag.Write(2);
                AmongUsClient.Instance.FinishRpcImmediately(bluePlayerStoleRedFlag);
                RPCProcedure.CaptureTheFlagWhoTookTheFlag(targetId, 2);
            }

            _blueplayer06TakeFlagButton.Timer = _blueplayer06TakeFlagButton.MaxTimer;
        }, () => { return Blueplayer06 != null && Blueplayer06 == PlayerControl.LocalPlayer; }, () =>
        {
            if (LocalRedFlagArrow.Count != 0) LocalRedFlagArrow[0].Update(Redflag.transform.position);
            if (LocalBlueFlagArrow.Count != 0) LocalBlueFlagArrow[0].Update(Blueflag.transform.position);
            if (Blueplayer06 == BluePlayerWhoHasRedFlag)
                _blueplayer06TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagDeliverRedFlagButton;
            else
                _blueplayer06TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagStealRedFlagButton;
            var canUse = false;
            if (Redflag != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Redflag.transform.position) < 0.5f && !PlayerControl.LocalPlayer.Data.IsDead && BluePlayerWhoHasRedFlag == null)
                canUse = true;
            else if (Blueflagbase != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Blueflagbase.transform.position) < 0.5f && PlayerControl.LocalPlayer == BluePlayerWhoHasRedFlag) canUse = true;
            return canUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _blueplayer06TakeFlagButton.Timer = _blueplayer06TakeFlagButton.MaxTimer; }, AssetLoader.CaptureTheFlagStealRedFlagButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CrewmateAbilityPrimary, false, Tr.Get(TrKey.StealButton));

        // Blueplayer07 Kill
        _blueplayer07KillButton = new(() =>
        {
            var targetId = Blueplayer07CurrentTarget.PlayerId;
            var sourceId = Blueplayer07.PlayerId;
            var killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            killWriter.Write(targetId);
            killWriter.Write(sourceId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.GamemodeKills(targetId, sourceId);
            _blueplayer07KillButton.Timer = _blueplayer07KillButton.MaxTimer;
            Blueplayer07CurrentTarget = null;
        }, () => { return Blueplayer07 != null && Blueplayer07 == PlayerControl.LocalPlayer; }, () => { return Blueplayer07CurrentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !Blueplayer07IsReviving && PlayerControl.LocalPlayer != BluePlayerWhoHasRedFlag; }, () => { _blueplayer07KillButton.Timer = _blueplayer07KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // Blueplayer07 TakeFlag Button
        _blueplayer07TakeFlagButton = new(() =>
        {
            if (PlayerControl.LocalPlayer == BluePlayerWhoHasRedFlag)
            {
                var whichTeamScored = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhichTeamScored, SendOption.Reliable);
                whichTeamScored.Write(2);
                AmongUsClient.Instance.FinishRpcImmediately(whichTeamScored);
                RPCProcedure.CaptureTheFlagWhichTeamScored(2);
            }
            else
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var bluePlayerStoleRedFlag = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhoTookTheFlag, SendOption.Reliable);
                bluePlayerStoleRedFlag.Write(targetId);
                bluePlayerStoleRedFlag.Write(2);
                AmongUsClient.Instance.FinishRpcImmediately(bluePlayerStoleRedFlag);
                RPCProcedure.CaptureTheFlagWhoTookTheFlag(targetId, 2);
            }

            _blueplayer07TakeFlagButton.Timer = _blueplayer07TakeFlagButton.MaxTimer;
        }, () => { return Blueplayer07 != null && Blueplayer07 == PlayerControl.LocalPlayer; }, () =>
        {
            if (LocalRedFlagArrow.Count != 0) LocalRedFlagArrow[0].Update(Redflag.transform.position);
            if (LocalBlueFlagArrow.Count != 0) LocalBlueFlagArrow[0].Update(Blueflag.transform.position);
            if (Blueplayer07 == BluePlayerWhoHasRedFlag)
                _blueplayer07TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagDeliverRedFlagButton;
            else
                _blueplayer07TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagStealRedFlagButton;
            var canUse = false;
            if (Redflag != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Redflag.transform.position) < 0.5f && !PlayerControl.LocalPlayer.Data.IsDead && BluePlayerWhoHasRedFlag == null)
                canUse = true;
            else if (Blueflagbase != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Blueflagbase.transform.position) < 0.5f && PlayerControl.LocalPlayer == BluePlayerWhoHasRedFlag) canUse = true;
            return canUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _blueplayer07TakeFlagButton.Timer = _blueplayer07TakeFlagButton.MaxTimer; }, AssetLoader.CaptureTheFlagStealRedFlagButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CrewmateAbilityPrimary, false, Tr.Get(TrKey.StealButton));

        // stealer Kill
        _stealerPlayerKillButton = new(() =>
        {
            var targetId = StealerPlayercurrentTarget.PlayerId;
            var sourceId = StealerPlayer.PlayerId;
            var killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            killWriter.Write(targetId);
            killWriter.Write(sourceId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.GamemodeKills(targetId, sourceId);
            _stealerPlayerKillButton.Timer = _stealerPlayerKillButton.MaxTimer;
            StealerPlayercurrentTarget = null;
        }, () => { return StealerPlayer != null && StealerPlayer == PlayerControl.LocalPlayer; }, () =>
        {
            if (LocalRedFlagArrow.Count != 0) LocalRedFlagArrow[0].Update(Redflag.transform.position);
            if (LocalBlueFlagArrow.Count != 0) LocalBlueFlagArrow[0].Update(Blueflag.transform.position);
            var canUse = true;
            foreach (var spawns in StealerSpawns)
            {
                if (spawns != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, spawns.transform.position) < 2.5f)
                    canUse = false;
            }

            return canUse && StealerPlayercurrentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !StealerPlayerIsReviving;
        }, () => { _stealerPlayerKillButton.Timer = _stealerPlayerKillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, TranslationController.Instance.GetString(StringNames.KillLabel));
    }
}
