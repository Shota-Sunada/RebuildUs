namespace RebuildUs.Modules.GameMode;

public static partial class CaptureTheFlag
{
    // Capture the flag buttons
    private static CustomButton redplayer01KillButton;
    private static CustomButton redplayer01TakeFlagButton;
    private static CustomButton redplayer02KillButton;
    private static CustomButton redplayer02TakeFlagButton;
    private static CustomButton redplayer03KillButton;
    private static CustomButton redplayer03TakeFlagButton;
    private static CustomButton redplayer04KillButton;
    private static CustomButton redplayer04TakeFlagButton;
    private static CustomButton redplayer05KillButton;
    private static CustomButton redplayer05TakeFlagButton;
    private static CustomButton redplayer06KillButton;
    private static CustomButton redplayer06TakeFlagButton;
    private static CustomButton redplayer07KillButton;
    private static CustomButton redplayer07TakeFlagButton;
    private static CustomButton blueplayer01KillButton;
    private static CustomButton blueplayer01TakeFlagButton;
    private static CustomButton blueplayer02KillButton;
    private static CustomButton blueplayer02TakeFlagButton;
    private static CustomButton blueplayer03KillButton;
    private static CustomButton blueplayer03TakeFlagButton;
    private static CustomButton blueplayer04KillButton;
    private static CustomButton blueplayer04TakeFlagButton;
    private static CustomButton blueplayer05KillButton;
    private static CustomButton blueplayer05TakeFlagButton;
    private static CustomButton blueplayer06KillButton;
    private static CustomButton blueplayer06TakeFlagButton;
    private static CustomButton blueplayer07KillButton;
    private static CustomButton blueplayer07TakeFlagButton;
    private static CustomButton stealerPlayerKillButton;

    public static void SetButtonCooldowns()
    {
        // Capture the flag buttons
        redplayer01KillButton.MaxTimer = MapSettings.gamemodeKillCooldown;
        redplayer01TakeFlagButton.MaxTimer = 0;
        redplayer02KillButton.MaxTimer = MapSettings.gamemodeKillCooldown;
        redplayer02TakeFlagButton.MaxTimer = 0;
        redplayer03KillButton.MaxTimer = MapSettings.gamemodeKillCooldown;
        redplayer03TakeFlagButton.MaxTimer = 0;
        redplayer04KillButton.MaxTimer = MapSettings.gamemodeKillCooldown;
        redplayer04TakeFlagButton.MaxTimer = 0;
        redplayer05KillButton.MaxTimer = MapSettings.gamemodeKillCooldown;
        redplayer05TakeFlagButton.MaxTimer = 0;
        redplayer06KillButton.MaxTimer = MapSettings.gamemodeKillCooldown;
        redplayer06TakeFlagButton.MaxTimer = 0;
        redplayer07KillButton.MaxTimer = MapSettings.gamemodeKillCooldown;
        redplayer07TakeFlagButton.MaxTimer = 0;
        blueplayer01KillButton.MaxTimer = MapSettings.gamemodeKillCooldown;
        blueplayer01TakeFlagButton.MaxTimer = 0;
        blueplayer02KillButton.MaxTimer = MapSettings.gamemodeKillCooldown;
        blueplayer02TakeFlagButton.MaxTimer = 0;
        blueplayer03KillButton.MaxTimer = MapSettings.gamemodeKillCooldown;
        blueplayer03TakeFlagButton.MaxTimer = 0;
        blueplayer04KillButton.MaxTimer = MapSettings.gamemodeKillCooldown;
        blueplayer04TakeFlagButton.MaxTimer = 0;
        blueplayer05KillButton.MaxTimer = MapSettings.gamemodeKillCooldown;
        blueplayer05TakeFlagButton.MaxTimer = 0;
        blueplayer06KillButton.MaxTimer = MapSettings.gamemodeKillCooldown;
        blueplayer06TakeFlagButton.MaxTimer = 0;
        blueplayer07KillButton.MaxTimer = MapSettings.gamemodeKillCooldown;
        blueplayer07TakeFlagButton.MaxTimer = 0;
        stealerPlayerKillButton.MaxTimer = MapSettings.gamemodeKillCooldown;
    }

    public static void MakeButtons(HudManager __instance)
    {
        // Capture the flag buttons
        // Redplayer01 Kill
        redplayer01KillButton = new CustomButton(
            () =>
            {
                byte targetId = CaptureTheFlag.redplayer01currentTarget.PlayerId;
                byte sourceId = CaptureTheFlag.redplayer01.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                killWriter.Write(targetId);
                killWriter.Write(sourceId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.gamemodeKills(targetId, sourceId);
                redplayer01KillButton.Timer = redplayer01KillButton.MaxTimer;
                CaptureTheFlag.redplayer01currentTarget = null;
            },
            () => { return CaptureTheFlag.redplayer01 != null && CaptureTheFlag.redplayer01 == PlayerControl.LocalPlayer; },
            () => { return CaptureTheFlag.redplayer01currentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !CaptureTheFlag.redplayer01IsReviving && PlayerControl.LocalPlayer != CaptureTheFlag.redPlayerWhoHasBlueFlag; },
            () => { redplayer01KillButton.Timer = redplayer01KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // Redplayer01 TakeFlag Button
        redplayer01TakeFlagButton = new CustomButton(
            () =>
            {
                if (PlayerControl.LocalPlayer == CaptureTheFlag.redPlayerWhoHasBlueFlag)
                {
                    MessageWriter whichTeamScored = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhichTeamScored, Hazel.SendOption.Reliable, -1);
                    whichTeamScored.Write(1);
                    AmongUsClient.Instance.FinishRpcImmediately(whichTeamScored);
                    RPCProcedure.captureTheFlagWhichTeamScored(1);
                }
                else
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    MessageWriter redPlayerStoleBlueFlag = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhoTookTheFlag, Hazel.SendOption.Reliable, -1);
                    redPlayerStoleBlueFlag.Write(targetId);
                    redPlayerStoleBlueFlag.Write(1);
                    AmongUsClient.Instance.FinishRpcImmediately(redPlayerStoleBlueFlag);
                    RPCProcedure.captureTheFlagWhoTookTheFlag(targetId, 1);
                }
                redplayer01TakeFlagButton.Timer = redplayer01TakeFlagButton.MaxTimer;
            },
            () => { return CaptureTheFlag.redplayer01 != null && CaptureTheFlag.redplayer01 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (CaptureTheFlag.localRedFlagArrow.Count != 0)
                {
                    CaptureTheFlag.localRedFlagArrow[0].Update(CaptureTheFlag.redflag.transform.position);
                }
                if (CaptureTheFlag.localBlueFlagArrow.Count != 0)
                {
                    CaptureTheFlag.localBlueFlagArrow[0].Update(CaptureTheFlag.blueflag.transform.position);
                }
                if (CaptureTheFlag.redplayer01 == CaptureTheFlag.redPlayerWhoHasBlueFlag)
                    redplayer01TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagDeliverBlueFlagButton;
                else
                    redplayer01TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagStealBlueFlagButton;
                bool CanUse = false;
                if (CaptureTheFlag.blueflag != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, CaptureTheFlag.blueflag.transform.position) < 0.5f && !PlayerControl.LocalPlayer.Data.IsDead && CaptureTheFlag.redPlayerWhoHasBlueFlag == null)
                {
                    CanUse = true;
                }
                else if (CaptureTheFlag.redflagbase != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, CaptureTheFlag.redflagbase.transform.position) < 0.5f && PlayerControl.LocalPlayer == CaptureTheFlag.redPlayerWhoHasBlueFlag)
                {
                    CanUse = true;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { redplayer01TakeFlagButton.Timer = redplayer01TakeFlagButton.MaxTimer; },
            AssetLoader.CaptureTheFlagStealBlueFlagButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            Tr.Get(TrKey.StealButton)
        );

        // Redplayer02 Kill
        redplayer02KillButton = new CustomButton(
            () =>
            {
                byte targetId = CaptureTheFlag.redplayer02currentTarget.PlayerId;
                byte sourceId = CaptureTheFlag.redplayer02.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                killWriter.Write(targetId);
                killWriter.Write(sourceId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.gamemodeKills(targetId, sourceId);
                redplayer02KillButton.Timer = redplayer02KillButton.MaxTimer;
                CaptureTheFlag.redplayer02currentTarget = null;
            },
            () => { return CaptureTheFlag.redplayer02 != null && CaptureTheFlag.redplayer02 == PlayerControl.LocalPlayer; },
            () => { return CaptureTheFlag.redplayer02currentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !CaptureTheFlag.redplayer02IsReviving && PlayerControl.LocalPlayer != CaptureTheFlag.redPlayerWhoHasBlueFlag; },
            () => { redplayer02KillButton.Timer = redplayer02KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // Redplayer02 TakeFlag Button
        redplayer02TakeFlagButton = new CustomButton(
            () =>
            {
                if (PlayerControl.LocalPlayer == CaptureTheFlag.redPlayerWhoHasBlueFlag)
                {
                    MessageWriter whichTeamScored = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhichTeamScored, Hazel.SendOption.Reliable, -1);
                    whichTeamScored.Write(1);
                    AmongUsClient.Instance.FinishRpcImmediately(whichTeamScored);
                    RPCProcedure.captureTheFlagWhichTeamScored(1);
                }
                else
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    MessageWriter redPlayerStoleBlueFlag = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhoTookTheFlag, Hazel.SendOption.Reliable, -1);
                    redPlayerStoleBlueFlag.Write(targetId);
                    redPlayerStoleBlueFlag.Write(1);
                    AmongUsClient.Instance.FinishRpcImmediately(redPlayerStoleBlueFlag);
                    RPCProcedure.captureTheFlagWhoTookTheFlag(targetId, 1);
                }
                redplayer02TakeFlagButton.Timer = redplayer02TakeFlagButton.MaxTimer;
            },
            () => { return CaptureTheFlag.redplayer02 != null && CaptureTheFlag.redplayer02 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (CaptureTheFlag.localRedFlagArrow.Count != 0)
                {
                    CaptureTheFlag.localRedFlagArrow[0].Update(CaptureTheFlag.redflag.transform.position);
                }
                if (CaptureTheFlag.localBlueFlagArrow.Count != 0)
                {
                    CaptureTheFlag.localBlueFlagArrow[0].Update(CaptureTheFlag.blueflag.transform.position);
                }
                if (CaptureTheFlag.redplayer02 == CaptureTheFlag.redPlayerWhoHasBlueFlag)
                    redplayer02TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagDeliverBlueFlagButton;
                else
                    redplayer02TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagStealBlueFlagButton;
                bool CanUse = false;
                if (CaptureTheFlag.blueflag != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, CaptureTheFlag.blueflag.transform.position) < 0.5f && !PlayerControl.LocalPlayer.Data.IsDead && CaptureTheFlag.redPlayerWhoHasBlueFlag == null)
                {
                    CanUse = true;
                }
                else if (CaptureTheFlag.redflagbase != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, CaptureTheFlag.redflagbase.transform.position) < 0.5f && PlayerControl.LocalPlayer == CaptureTheFlag.redPlayerWhoHasBlueFlag)
                {
                    CanUse = true;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { redplayer02TakeFlagButton.Timer = redplayer02TakeFlagButton.MaxTimer; },
            AssetLoader.CaptureTheFlagStealBlueFlagButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            Tr.Get(TrKey.StealButton)
        );

        // Redplayer03 Kill
        redplayer03KillButton = new CustomButton(
            () =>
            {
                byte targetId = CaptureTheFlag.redplayer03currentTarget.PlayerId;
                byte sourceId = CaptureTheFlag.redplayer03.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                killWriter.Write(targetId);
                killWriter.Write(sourceId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.gamemodeKills(targetId, sourceId);
                redplayer03KillButton.Timer = redplayer03KillButton.MaxTimer;
                CaptureTheFlag.redplayer03currentTarget = null;
            },
            () => { return CaptureTheFlag.redplayer03 != null && CaptureTheFlag.redplayer03 == PlayerControl.LocalPlayer; },
            () => { return CaptureTheFlag.redplayer03currentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !CaptureTheFlag.redplayer03IsReviving && PlayerControl.LocalPlayer != CaptureTheFlag.redPlayerWhoHasBlueFlag; },
            () => { redplayer03KillButton.Timer = redplayer03KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // Redplayer03 TakeFlag Button
        redplayer03TakeFlagButton = new CustomButton(
            () =>
            {
                if (PlayerControl.LocalPlayer == CaptureTheFlag.redPlayerWhoHasBlueFlag)
                {
                    MessageWriter whichTeamScored = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhichTeamScored, Hazel.SendOption.Reliable, -1);
                    whichTeamScored.Write(1);
                    AmongUsClient.Instance.FinishRpcImmediately(whichTeamScored);
                    RPCProcedure.captureTheFlagWhichTeamScored(1);
                }
                else
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    MessageWriter redPlayerStoleBlueFlag = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhoTookTheFlag, Hazel.SendOption.Reliable, -1);
                    redPlayerStoleBlueFlag.Write(targetId);
                    redPlayerStoleBlueFlag.Write(1);
                    AmongUsClient.Instance.FinishRpcImmediately(redPlayerStoleBlueFlag);
                    RPCProcedure.captureTheFlagWhoTookTheFlag(targetId, 1);
                }
                redplayer03TakeFlagButton.Timer = redplayer03TakeFlagButton.MaxTimer;
            },
            () => { return CaptureTheFlag.redplayer03 != null && CaptureTheFlag.redplayer03 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (CaptureTheFlag.localRedFlagArrow.Count != 0)
                {
                    CaptureTheFlag.localRedFlagArrow[0].Update(CaptureTheFlag.redflag.transform.position);
                }
                if (CaptureTheFlag.localBlueFlagArrow.Count != 0)
                {
                    CaptureTheFlag.localBlueFlagArrow[0].Update(CaptureTheFlag.blueflag.transform.position);
                }
                if (CaptureTheFlag.redplayer03 == CaptureTheFlag.redPlayerWhoHasBlueFlag)
                    redplayer03TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagDeliverBlueFlagButton;
                else
                    redplayer03TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagStealBlueFlagButton;
                bool CanUse = false;
                if (CaptureTheFlag.blueflag != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, CaptureTheFlag.blueflag.transform.position) < 0.5f && !PlayerControl.LocalPlayer.Data.IsDead && CaptureTheFlag.redPlayerWhoHasBlueFlag == null)
                {
                    CanUse = true;
                }
                else if (CaptureTheFlag.redflagbase != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, CaptureTheFlag.redflagbase.transform.position) < 0.5f && PlayerControl.LocalPlayer == CaptureTheFlag.redPlayerWhoHasBlueFlag)
                {
                    CanUse = true;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { redplayer03TakeFlagButton.Timer = redplayer03TakeFlagButton.MaxTimer; },
            AssetLoader.CaptureTheFlagStealBlueFlagButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            Tr.Get(TrKey.StealButton)
        );

        // Redplayer04 Kill
        redplayer04KillButton = new CustomButton(
            () =>
            {
                byte targetId = CaptureTheFlag.redplayer04currentTarget.PlayerId;
                byte sourceId = CaptureTheFlag.redplayer04.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                killWriter.Write(targetId);
                killWriter.Write(sourceId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.gamemodeKills(targetId, sourceId);
                redplayer04KillButton.Timer = redplayer04KillButton.MaxTimer;
                CaptureTheFlag.redplayer04currentTarget = null;
            },
            () => { return CaptureTheFlag.redplayer04 != null && CaptureTheFlag.redplayer04 == PlayerControl.LocalPlayer; },
            () => { return CaptureTheFlag.redplayer04currentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !CaptureTheFlag.redplayer04IsReviving && PlayerControl.LocalPlayer != CaptureTheFlag.redPlayerWhoHasBlueFlag; },
            () => { redplayer04KillButton.Timer = redplayer04KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // Redplayer04 TakeFlag Button
        redplayer04TakeFlagButton = new CustomButton(
            () =>
            {
                if (PlayerControl.LocalPlayer == CaptureTheFlag.redPlayerWhoHasBlueFlag)
                {
                    MessageWriter whichTeamScored = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhichTeamScored, Hazel.SendOption.Reliable, -1);
                    whichTeamScored.Write(1);
                    AmongUsClient.Instance.FinishRpcImmediately(whichTeamScored);
                    RPCProcedure.captureTheFlagWhichTeamScored(1);
                }
                else
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    MessageWriter redPlayerStoleBlueFlag = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhoTookTheFlag, Hazel.SendOption.Reliable, -1);
                    redPlayerStoleBlueFlag.Write(targetId);
                    redPlayerStoleBlueFlag.Write(1);
                    AmongUsClient.Instance.FinishRpcImmediately(redPlayerStoleBlueFlag);
                    RPCProcedure.captureTheFlagWhoTookTheFlag(targetId, 1);
                }
                redplayer04TakeFlagButton.Timer = redplayer04TakeFlagButton.MaxTimer;
            },
            () => { return CaptureTheFlag.redplayer04 != null && CaptureTheFlag.redplayer04 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (CaptureTheFlag.localRedFlagArrow.Count != 0)
                {
                    CaptureTheFlag.localRedFlagArrow[0].Update(CaptureTheFlag.redflag.transform.position);
                }
                if (CaptureTheFlag.localBlueFlagArrow.Count != 0)
                {
                    CaptureTheFlag.localBlueFlagArrow[0].Update(CaptureTheFlag.blueflag.transform.position);
                }
                if (CaptureTheFlag.redplayer04 == CaptureTheFlag.redPlayerWhoHasBlueFlag)
                    redplayer04TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagDeliverBlueFlagButton;
                else
                    redplayer04TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagStealBlueFlagButton;
                bool CanUse = false;
                if (CaptureTheFlag.blueflag != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, CaptureTheFlag.blueflag.transform.position) < 0.5f && !PlayerControl.LocalPlayer.Data.IsDead && CaptureTheFlag.redPlayerWhoHasBlueFlag == null)
                {
                    CanUse = true;
                }
                else if (CaptureTheFlag.redflagbase != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, CaptureTheFlag.redflagbase.transform.position) < 0.5f && PlayerControl.LocalPlayer == CaptureTheFlag.redPlayerWhoHasBlueFlag)
                {
                    CanUse = true;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { redplayer04TakeFlagButton.Timer = redplayer04TakeFlagButton.MaxTimer; },
            AssetLoader.CaptureTheFlagStealBlueFlagButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            Tr.Get(TrKey.StealButton)
        );

        // Redplayer05 Kill
        redplayer05KillButton = new CustomButton(
            () =>
            {
                byte targetId = CaptureTheFlag.redplayer05currentTarget.PlayerId;
                byte sourceId = CaptureTheFlag.redplayer05.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                killWriter.Write(targetId);
                killWriter.Write(sourceId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.gamemodeKills(targetId, sourceId);
                redplayer05KillButton.Timer = redplayer05KillButton.MaxTimer;
                CaptureTheFlag.redplayer05currentTarget = null;
            },
            () => { return CaptureTheFlag.redplayer05 != null && CaptureTheFlag.redplayer05 == PlayerControl.LocalPlayer; },
            () => { return CaptureTheFlag.redplayer05currentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !CaptureTheFlag.redplayer05IsReviving && PlayerControl.LocalPlayer != CaptureTheFlag.redPlayerWhoHasBlueFlag; },
            () => { redplayer05KillButton.Timer = redplayer05KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // Redplayer05 TakeFlag Button
        redplayer05TakeFlagButton = new CustomButton(
            () =>
            {
                if (PlayerControl.LocalPlayer == CaptureTheFlag.redPlayerWhoHasBlueFlag)
                {
                    MessageWriter whichTeamScored = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhichTeamScored, Hazel.SendOption.Reliable, -1);
                    whichTeamScored.Write(1);
                    AmongUsClient.Instance.FinishRpcImmediately(whichTeamScored);
                    RPCProcedure.captureTheFlagWhichTeamScored(1);
                }
                else
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    MessageWriter redPlayerStoleBlueFlag = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhoTookTheFlag, Hazel.SendOption.Reliable, -1);
                    redPlayerStoleBlueFlag.Write(targetId);
                    redPlayerStoleBlueFlag.Write(1);
                    AmongUsClient.Instance.FinishRpcImmediately(redPlayerStoleBlueFlag);
                    RPCProcedure.captureTheFlagWhoTookTheFlag(targetId, 1);
                }
                redplayer05TakeFlagButton.Timer = redplayer05TakeFlagButton.MaxTimer;
            },
            () => { return CaptureTheFlag.redplayer05 != null && CaptureTheFlag.redplayer05 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (CaptureTheFlag.localRedFlagArrow.Count != 0)
                {
                    CaptureTheFlag.localRedFlagArrow[0].Update(CaptureTheFlag.redflag.transform.position);
                }
                if (CaptureTheFlag.localBlueFlagArrow.Count != 0)
                {
                    CaptureTheFlag.localBlueFlagArrow[0].Update(CaptureTheFlag.blueflag.transform.position);
                }
                if (CaptureTheFlag.redplayer05 == CaptureTheFlag.redPlayerWhoHasBlueFlag)
                    redplayer05TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagDeliverBlueFlagButton;
                else
                    redplayer05TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagStealBlueFlagButton;
                bool CanUse = false;
                if (CaptureTheFlag.blueflag != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, CaptureTheFlag.blueflag.transform.position) < 0.5f && !PlayerControl.LocalPlayer.Data.IsDead && CaptureTheFlag.redPlayerWhoHasBlueFlag == null)
                {
                    CanUse = true;
                }
                else if (CaptureTheFlag.redflagbase != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, CaptureTheFlag.redflagbase.transform.position) < 0.5f && PlayerControl.LocalPlayer == CaptureTheFlag.redPlayerWhoHasBlueFlag)
                {
                    CanUse = true;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { redplayer05TakeFlagButton.Timer = redplayer05TakeFlagButton.MaxTimer; },
            AssetLoader.CaptureTheFlagStealBlueFlagButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            Tr.Get(TrKey.StealButton)
        );

        // Redplayer06 Kill
        redplayer06KillButton = new CustomButton(
            () =>
            {
                byte targetId = CaptureTheFlag.redplayer06currentTarget.PlayerId;
                byte sourceId = CaptureTheFlag.redplayer06.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                killWriter.Write(targetId);
                killWriter.Write(sourceId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.gamemodeKills(targetId, sourceId);
                redplayer06KillButton.Timer = redplayer06KillButton.MaxTimer;
                CaptureTheFlag.redplayer06currentTarget = null;
            },
            () => { return CaptureTheFlag.redplayer06 != null && CaptureTheFlag.redplayer06 == PlayerControl.LocalPlayer; },
            () => { return CaptureTheFlag.redplayer06currentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !CaptureTheFlag.redplayer06IsReviving && PlayerControl.LocalPlayer != CaptureTheFlag.redPlayerWhoHasBlueFlag; },
            () => { redplayer06KillButton.Timer = redplayer06KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // Redplayer06 TakeFlag Button
        redplayer06TakeFlagButton = new CustomButton(
            () =>
            {
                if (PlayerControl.LocalPlayer == CaptureTheFlag.redPlayerWhoHasBlueFlag)
                {
                    MessageWriter whichTeamScored = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhichTeamScored, Hazel.SendOption.Reliable, -1);
                    whichTeamScored.Write(1);
                    AmongUsClient.Instance.FinishRpcImmediately(whichTeamScored);
                    RPCProcedure.captureTheFlagWhichTeamScored(1);
                }
                else
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    MessageWriter redPlayerStoleBlueFlag = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhoTookTheFlag, Hazel.SendOption.Reliable, -1);
                    redPlayerStoleBlueFlag.Write(targetId);
                    redPlayerStoleBlueFlag.Write(1);
                    AmongUsClient.Instance.FinishRpcImmediately(redPlayerStoleBlueFlag);
                    RPCProcedure.captureTheFlagWhoTookTheFlag(targetId, 1);
                }
                redplayer06TakeFlagButton.Timer = redplayer06TakeFlagButton.MaxTimer;
            },
            () => { return CaptureTheFlag.redplayer06 != null && CaptureTheFlag.redplayer06 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (CaptureTheFlag.localRedFlagArrow.Count != 0)
                {
                    CaptureTheFlag.localRedFlagArrow[0].Update(CaptureTheFlag.redflag.transform.position);
                }
                if (CaptureTheFlag.localBlueFlagArrow.Count != 0)
                {
                    CaptureTheFlag.localBlueFlagArrow[0].Update(CaptureTheFlag.blueflag.transform.position);
                }
                if (CaptureTheFlag.redplayer06 == CaptureTheFlag.redPlayerWhoHasBlueFlag)
                    redplayer06TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagDeliverBlueFlagButton;
                else
                    redplayer06TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagStealBlueFlagButton;
                bool CanUse = false;
                if (CaptureTheFlag.blueflag != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, CaptureTheFlag.blueflag.transform.position) < 0.5f && !PlayerControl.LocalPlayer.Data.IsDead && CaptureTheFlag.redPlayerWhoHasBlueFlag == null)
                {
                    CanUse = true;
                }
                else if (CaptureTheFlag.redflagbase != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, CaptureTheFlag.redflagbase.transform.position) < 0.5f && PlayerControl.LocalPlayer == CaptureTheFlag.redPlayerWhoHasBlueFlag)
                {
                    CanUse = true;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { redplayer06TakeFlagButton.Timer = redplayer06TakeFlagButton.MaxTimer; },
            AssetLoader.CaptureTheFlagStealBlueFlagButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            Tr.Get(TrKey.StealButton)
        );

        // Redplayer07  Kill
        redplayer07KillButton = new CustomButton(
            () =>
            {
                byte targetId = CaptureTheFlag.redplayer07currentTarget.PlayerId;
                byte sourceId = CaptureTheFlag.redplayer07.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                killWriter.Write(targetId);
                killWriter.Write(sourceId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.gamemodeKills(targetId, sourceId);
                redplayer07KillButton.Timer = redplayer07KillButton.MaxTimer;
                CaptureTheFlag.redplayer07currentTarget = null;
            },
            () => { return CaptureTheFlag.redplayer07 != null && CaptureTheFlag.redplayer07 == PlayerControl.LocalPlayer; },
            () => { return CaptureTheFlag.redplayer07currentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !CaptureTheFlag.redplayer07IsReviving && PlayerControl.LocalPlayer != CaptureTheFlag.redPlayerWhoHasBlueFlag; },
            () => { redplayer07KillButton.Timer = redplayer07KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // Redplayer07 TakeFlag Button
        redplayer07TakeFlagButton = new CustomButton(
            () =>
            {
                if (PlayerControl.LocalPlayer == CaptureTheFlag.redPlayerWhoHasBlueFlag)
                {
                    MessageWriter whichTeamScored = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhichTeamScored, Hazel.SendOption.Reliable, -1);
                    whichTeamScored.Write(1);
                    AmongUsClient.Instance.FinishRpcImmediately(whichTeamScored);
                    RPCProcedure.captureTheFlagWhichTeamScored(1);
                }
                else
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    MessageWriter redPlayerStoleBlueFlag = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhoTookTheFlag, Hazel.SendOption.Reliable, -1);
                    redPlayerStoleBlueFlag.Write(targetId);
                    redPlayerStoleBlueFlag.Write(1);
                    AmongUsClient.Instance.FinishRpcImmediately(redPlayerStoleBlueFlag);
                    RPCProcedure.captureTheFlagWhoTookTheFlag(targetId, 1);
                }
                redplayer07TakeFlagButton.Timer = redplayer07TakeFlagButton.MaxTimer;
            },
            () => { return CaptureTheFlag.redplayer07 != null && CaptureTheFlag.redplayer07 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (CaptureTheFlag.localRedFlagArrow.Count != 0)
                {
                    CaptureTheFlag.localRedFlagArrow[0].Update(CaptureTheFlag.redflag.transform.position);
                }
                if (CaptureTheFlag.localBlueFlagArrow.Count != 0)
                {
                    CaptureTheFlag.localBlueFlagArrow[0].Update(CaptureTheFlag.blueflag.transform.position);
                }
                if (CaptureTheFlag.redplayer07 == CaptureTheFlag.redPlayerWhoHasBlueFlag)
                    redplayer07TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagDeliverBlueFlagButton;
                else
                    redplayer07TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagStealBlueFlagButton;
                bool CanUse = false;
                if (CaptureTheFlag.blueflag != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, CaptureTheFlag.blueflag.transform.position) < 0.5f && !PlayerControl.LocalPlayer.Data.IsDead && CaptureTheFlag.redPlayerWhoHasBlueFlag == null)
                {
                    CanUse = true;
                }
                else if (CaptureTheFlag.redflagbase != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, CaptureTheFlag.redflagbase.transform.position) < 0.5f && PlayerControl.LocalPlayer == CaptureTheFlag.redPlayerWhoHasBlueFlag)
                {
                    CanUse = true;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { redplayer07TakeFlagButton.Timer = redplayer07TakeFlagButton.MaxTimer; },
            AssetLoader.CaptureTheFlagStealBlueFlagButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            Tr.Get(TrKey.StealButton)
        );

        // Blueplayer01 Kill
        blueplayer01KillButton = new CustomButton(
            () =>
            {
                byte targetId = CaptureTheFlag.blueplayer01currentTarget.PlayerId;
                byte sourceId = CaptureTheFlag.blueplayer01.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                killWriter.Write(targetId);
                killWriter.Write(sourceId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.gamemodeKills(targetId, sourceId);
                blueplayer01KillButton.Timer = blueplayer01KillButton.MaxTimer;
                CaptureTheFlag.blueplayer01currentTarget = null;
            },
            () => { return CaptureTheFlag.blueplayer01 != null && CaptureTheFlag.blueplayer01 == PlayerControl.LocalPlayer; },
            () => { return CaptureTheFlag.blueplayer01currentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !CaptureTheFlag.blueplayer01IsReviving && PlayerControl.LocalPlayer != CaptureTheFlag.bluePlayerWhoHasRedFlag; },
            () => { blueplayer01KillButton.Timer = blueplayer01KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // Blueplayer01 TakeFlag Button
        blueplayer01TakeFlagButton = new CustomButton(
            () =>
            {
                if (PlayerControl.LocalPlayer == CaptureTheFlag.bluePlayerWhoHasRedFlag)
                {
                    MessageWriter whichTeamScored = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhichTeamScored, Hazel.SendOption.Reliable, -1);
                    whichTeamScored.Write(2);
                    AmongUsClient.Instance.FinishRpcImmediately(whichTeamScored);
                    RPCProcedure.captureTheFlagWhichTeamScored(2);
                }
                else
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    MessageWriter bluePlayerStoleRedFlag = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhoTookTheFlag, Hazel.SendOption.Reliable, -1);
                    bluePlayerStoleRedFlag.Write(targetId);
                    bluePlayerStoleRedFlag.Write(2);
                    AmongUsClient.Instance.FinishRpcImmediately(bluePlayerStoleRedFlag);
                    RPCProcedure.captureTheFlagWhoTookTheFlag(targetId, 2);
                }
                blueplayer01TakeFlagButton.Timer = blueplayer01TakeFlagButton.MaxTimer;
            },
            () => { return CaptureTheFlag.blueplayer01 != null && CaptureTheFlag.blueplayer01 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (CaptureTheFlag.localRedFlagArrow.Count != 0)
                {
                    CaptureTheFlag.localRedFlagArrow[0].Update(CaptureTheFlag.redflag.transform.position);
                }
                if (CaptureTheFlag.localBlueFlagArrow.Count != 0)
                {
                    CaptureTheFlag.localBlueFlagArrow[0].Update(CaptureTheFlag.blueflag.transform.position);
                }
                if (CaptureTheFlag.blueplayer01 == CaptureTheFlag.bluePlayerWhoHasRedFlag)
                    blueplayer01TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagDeliverRedFlagButton;
                else
                    blueplayer01TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagStealRedFlagButton;
                bool CanUse = false;
                if (CaptureTheFlag.redflag != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, CaptureTheFlag.redflag.transform.position) < 0.5f && !PlayerControl.LocalPlayer.Data.IsDead && CaptureTheFlag.bluePlayerWhoHasRedFlag == null)
                {
                    CanUse = true;
                }
                else if (CaptureTheFlag.blueflagbase != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, CaptureTheFlag.blueflagbase.transform.position) < 0.5f && PlayerControl.LocalPlayer == CaptureTheFlag.bluePlayerWhoHasRedFlag)
                {
                    CanUse = true;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { blueplayer01TakeFlagButton.Timer = blueplayer01TakeFlagButton.MaxTimer; },
            AssetLoader.CaptureTheFlagStealRedFlagButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            Tr.Get(TrKey.StealButton)
        );

        // Blueplayer02 Kill
        blueplayer02KillButton = new CustomButton(
            () =>
            {
                byte targetId = CaptureTheFlag.blueplayer02currentTarget.PlayerId;
                byte sourceId = CaptureTheFlag.blueplayer02.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                killWriter.Write(targetId);
                killWriter.Write(sourceId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.gamemodeKills(targetId, sourceId);
                blueplayer02KillButton.Timer = blueplayer02KillButton.MaxTimer;
                CaptureTheFlag.blueplayer02currentTarget = null;
            },
            () => { return CaptureTheFlag.blueplayer02 != null && CaptureTheFlag.blueplayer02 == PlayerControl.LocalPlayer; },
            () => { return CaptureTheFlag.blueplayer02currentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !CaptureTheFlag.blueplayer02IsReviving && PlayerControl.LocalPlayer != CaptureTheFlag.bluePlayerWhoHasRedFlag; },
            () => { blueplayer02KillButton.Timer = blueplayer02KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // Blueplayer02 TakeFlag Button
        blueplayer02TakeFlagButton = new CustomButton(
            () =>
            {
                if (PlayerControl.LocalPlayer == CaptureTheFlag.bluePlayerWhoHasRedFlag)
                {
                    MessageWriter whichTeamScored = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhichTeamScored, Hazel.SendOption.Reliable, -1);
                    whichTeamScored.Write(2);
                    AmongUsClient.Instance.FinishRpcImmediately(whichTeamScored);
                    RPCProcedure.captureTheFlagWhichTeamScored(2);
                }
                else
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    MessageWriter bluePlayerStoleRedFlag = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhoTookTheFlag, Hazel.SendOption.Reliable, -1);
                    bluePlayerStoleRedFlag.Write(targetId);
                    bluePlayerStoleRedFlag.Write(2);
                    AmongUsClient.Instance.FinishRpcImmediately(bluePlayerStoleRedFlag);
                    RPCProcedure.captureTheFlagWhoTookTheFlag(targetId, 2);
                }
                blueplayer02TakeFlagButton.Timer = blueplayer02TakeFlagButton.MaxTimer;
            },
            () => { return CaptureTheFlag.blueplayer02 != null && CaptureTheFlag.blueplayer02 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (CaptureTheFlag.localRedFlagArrow.Count != 0)
                {
                    CaptureTheFlag.localRedFlagArrow[0].Update(CaptureTheFlag.redflag.transform.position);
                }
                if (CaptureTheFlag.localBlueFlagArrow.Count != 0)
                {
                    CaptureTheFlag.localBlueFlagArrow[0].Update(CaptureTheFlag.blueflag.transform.position);
                }
                if (CaptureTheFlag.blueplayer02 == CaptureTheFlag.bluePlayerWhoHasRedFlag)
                    blueplayer02TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagDeliverRedFlagButton;
                else
                    blueplayer02TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagStealRedFlagButton;
                bool CanUse = false;
                if (CaptureTheFlag.redflag != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, CaptureTheFlag.redflag.transform.position) < 0.5f && !PlayerControl.LocalPlayer.Data.IsDead && CaptureTheFlag.bluePlayerWhoHasRedFlag == null)
                {
                    CanUse = true;
                }
                else if (CaptureTheFlag.blueflagbase != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, CaptureTheFlag.blueflagbase.transform.position) < 0.5f && PlayerControl.LocalPlayer == CaptureTheFlag.bluePlayerWhoHasRedFlag)
                {
                    CanUse = true;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { blueplayer02TakeFlagButton.Timer = blueplayer02TakeFlagButton.MaxTimer; },
            AssetLoader.CaptureTheFlagStealRedFlagButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            Tr.Get(TrKey.StealButton)
        );

        // Blueplayer03 Kill
        blueplayer03KillButton = new CustomButton(
            () =>
            {
                byte targetId = CaptureTheFlag.blueplayer03currentTarget.PlayerId;
                byte sourceId = CaptureTheFlag.blueplayer03.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                killWriter.Write(targetId);
                killWriter.Write(sourceId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.gamemodeKills(targetId, sourceId);
                blueplayer03KillButton.Timer = blueplayer03KillButton.MaxTimer;
                CaptureTheFlag.blueplayer03currentTarget = null;
            },
            () => { return CaptureTheFlag.blueplayer03 != null && CaptureTheFlag.blueplayer03 == PlayerControl.LocalPlayer; },
            () => { return CaptureTheFlag.blueplayer03currentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !CaptureTheFlag.blueplayer03IsReviving && PlayerControl.LocalPlayer != CaptureTheFlag.bluePlayerWhoHasRedFlag; },
            () => { blueplayer03KillButton.Timer = blueplayer03KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // Blueplayer03 TakeFlag Button
        blueplayer03TakeFlagButton = new CustomButton(
            () =>
            {
                if (PlayerControl.LocalPlayer == CaptureTheFlag.bluePlayerWhoHasRedFlag)
                {
                    MessageWriter whichTeamScored = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhichTeamScored, Hazel.SendOption.Reliable, -1);
                    whichTeamScored.Write(2);
                    AmongUsClient.Instance.FinishRpcImmediately(whichTeamScored);
                    RPCProcedure.captureTheFlagWhichTeamScored(2);
                }
                else
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    MessageWriter bluePlayerStoleRedFlag = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhoTookTheFlag, Hazel.SendOption.Reliable, -1);
                    bluePlayerStoleRedFlag.Write(targetId);
                    bluePlayerStoleRedFlag.Write(2);
                    AmongUsClient.Instance.FinishRpcImmediately(bluePlayerStoleRedFlag);
                    RPCProcedure.captureTheFlagWhoTookTheFlag(targetId, 2);
                }
                blueplayer03TakeFlagButton.Timer = blueplayer03TakeFlagButton.MaxTimer;
            },
            () => { return CaptureTheFlag.blueplayer03 != null && CaptureTheFlag.blueplayer03 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (CaptureTheFlag.localRedFlagArrow.Count != 0)
                {
                    CaptureTheFlag.localRedFlagArrow[0].Update(CaptureTheFlag.redflag.transform.position);
                }
                if (CaptureTheFlag.localBlueFlagArrow.Count != 0)
                {
                    CaptureTheFlag.localBlueFlagArrow[0].Update(CaptureTheFlag.blueflag.transform.position);
                }
                if (CaptureTheFlag.blueplayer03 == CaptureTheFlag.bluePlayerWhoHasRedFlag)
                    blueplayer03TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagDeliverRedFlagButton;
                else
                    blueplayer03TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagStealRedFlagButton;
                bool CanUse = false;
                if (CaptureTheFlag.redflag != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, CaptureTheFlag.redflag.transform.position) < 0.5f && !PlayerControl.LocalPlayer.Data.IsDead && CaptureTheFlag.bluePlayerWhoHasRedFlag == null)
                {
                    CanUse = true;
                }
                else if (CaptureTheFlag.blueflagbase != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, CaptureTheFlag.blueflagbase.transform.position) < 0.5f && PlayerControl.LocalPlayer == CaptureTheFlag.bluePlayerWhoHasRedFlag)
                {
                    CanUse = true;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { blueplayer03TakeFlagButton.Timer = blueplayer03TakeFlagButton.MaxTimer; },
            AssetLoader.CaptureTheFlagStealRedFlagButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            Tr.Get(TrKey.StealButton)
        );

        // Blueplayer04 Kill
        blueplayer04KillButton = new CustomButton(
            () =>
            {
                byte targetId = CaptureTheFlag.blueplayer04currentTarget.PlayerId;
                byte sourceId = CaptureTheFlag.blueplayer04.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                killWriter.Write(targetId);
                killWriter.Write(sourceId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.gamemodeKills(targetId, sourceId);
                blueplayer04KillButton.Timer = blueplayer04KillButton.MaxTimer;
                CaptureTheFlag.blueplayer04currentTarget = null;
            },
            () => { return CaptureTheFlag.blueplayer04 != null && CaptureTheFlag.blueplayer04 == PlayerControl.LocalPlayer; },
            () => { return CaptureTheFlag.blueplayer04currentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !CaptureTheFlag.blueplayer04IsReviving && PlayerControl.LocalPlayer != CaptureTheFlag.bluePlayerWhoHasRedFlag; },
            () => { blueplayer04KillButton.Timer = blueplayer04KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // Blueplayer04 TakeFlag Button
        blueplayer04TakeFlagButton = new CustomButton(
            () =>
            {
                if (PlayerControl.LocalPlayer == CaptureTheFlag.bluePlayerWhoHasRedFlag)
                {
                    MessageWriter whichTeamScored = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhichTeamScored, Hazel.SendOption.Reliable, -1);
                    whichTeamScored.Write(2);
                    AmongUsClient.Instance.FinishRpcImmediately(whichTeamScored);
                    RPCProcedure.captureTheFlagWhichTeamScored(2);
                }
                else
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    MessageWriter bluePlayerStoleRedFlag = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhoTookTheFlag, Hazel.SendOption.Reliable, -1);
                    bluePlayerStoleRedFlag.Write(targetId);
                    bluePlayerStoleRedFlag.Write(2);
                    AmongUsClient.Instance.FinishRpcImmediately(bluePlayerStoleRedFlag);
                    RPCProcedure.captureTheFlagWhoTookTheFlag(targetId, 2);
                }
                blueplayer04TakeFlagButton.Timer = blueplayer04TakeFlagButton.MaxTimer;
            },
            () => { return CaptureTheFlag.blueplayer04 != null && CaptureTheFlag.blueplayer04 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (CaptureTheFlag.localRedFlagArrow.Count != 0)
                {
                    CaptureTheFlag.localRedFlagArrow[0].Update(CaptureTheFlag.redflag.transform.position);
                }
                if (CaptureTheFlag.localBlueFlagArrow.Count != 0)
                {
                    CaptureTheFlag.localBlueFlagArrow[0].Update(CaptureTheFlag.blueflag.transform.position);
                }
                if (CaptureTheFlag.blueplayer04 == CaptureTheFlag.bluePlayerWhoHasRedFlag)
                    blueplayer04TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagDeliverRedFlagButton;
                else
                    blueplayer04TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagStealRedFlagButton;
                bool CanUse = false;
                if (CaptureTheFlag.redflag != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, CaptureTheFlag.redflag.transform.position) < 0.5f && !PlayerControl.LocalPlayer.Data.IsDead && CaptureTheFlag.bluePlayerWhoHasRedFlag == null)
                {
                    CanUse = true;
                }
                else if (CaptureTheFlag.blueflagbase != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, CaptureTheFlag.blueflagbase.transform.position) < 0.5f && PlayerControl.LocalPlayer == CaptureTheFlag.bluePlayerWhoHasRedFlag)
                {
                    CanUse = true;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { blueplayer04TakeFlagButton.Timer = blueplayer04TakeFlagButton.MaxTimer; },
            AssetLoader.CaptureTheFlagStealRedFlagButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            Tr.Get(TrKey.StealButton)
        );

        // Blueplayer05 Kill
        blueplayer05KillButton = new CustomButton(
            () =>
            {
                byte targetId = CaptureTheFlag.blueplayer05currentTarget.PlayerId;
                byte sourceId = CaptureTheFlag.blueplayer05.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                killWriter.Write(targetId);
                killWriter.Write(sourceId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.gamemodeKills(targetId, sourceId);
                blueplayer05KillButton.Timer = blueplayer05KillButton.MaxTimer;
                CaptureTheFlag.blueplayer05currentTarget = null;
            },
            () => { return CaptureTheFlag.blueplayer05 != null && CaptureTheFlag.blueplayer05 == PlayerControl.LocalPlayer; },
            () => { return CaptureTheFlag.blueplayer05currentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !CaptureTheFlag.blueplayer05IsReviving && PlayerControl.LocalPlayer != CaptureTheFlag.bluePlayerWhoHasRedFlag; },
            () => { blueplayer05KillButton.Timer = blueplayer05KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // Blueplayer05 TakeFlag Button
        blueplayer05TakeFlagButton = new CustomButton(
            () =>
            {
                if (PlayerControl.LocalPlayer == CaptureTheFlag.bluePlayerWhoHasRedFlag)
                {
                    MessageWriter whichTeamScored = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhichTeamScored, Hazel.SendOption.Reliable, -1);
                    whichTeamScored.Write(2);
                    AmongUsClient.Instance.FinishRpcImmediately(whichTeamScored);
                    RPCProcedure.captureTheFlagWhichTeamScored(2);
                }
                else
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    MessageWriter bluePlayerStoleRedFlag = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhoTookTheFlag, Hazel.SendOption.Reliable, -1);
                    bluePlayerStoleRedFlag.Write(targetId);
                    bluePlayerStoleRedFlag.Write(2);
                    AmongUsClient.Instance.FinishRpcImmediately(bluePlayerStoleRedFlag);
                    RPCProcedure.captureTheFlagWhoTookTheFlag(targetId, 2);
                }
                blueplayer05TakeFlagButton.Timer = blueplayer05TakeFlagButton.MaxTimer;
            },
            () => { return CaptureTheFlag.blueplayer05 != null && CaptureTheFlag.blueplayer05 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (CaptureTheFlag.localRedFlagArrow.Count != 0)
                {
                    CaptureTheFlag.localRedFlagArrow[0].Update(CaptureTheFlag.redflag.transform.position);
                }
                if (CaptureTheFlag.localBlueFlagArrow.Count != 0)
                {
                    CaptureTheFlag.localBlueFlagArrow[0].Update(CaptureTheFlag.blueflag.transform.position);
                }
                if (CaptureTheFlag.blueplayer05 == CaptureTheFlag.bluePlayerWhoHasRedFlag)
                    blueplayer05TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagDeliverRedFlagButton;
                else
                    blueplayer05TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagStealRedFlagButton;
                bool CanUse = false;
                if (CaptureTheFlag.redflag != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, CaptureTheFlag.redflag.transform.position) < 0.5f && !PlayerControl.LocalPlayer.Data.IsDead && CaptureTheFlag.bluePlayerWhoHasRedFlag == null)
                {
                    CanUse = true;
                }
                else if (CaptureTheFlag.blueflagbase != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, CaptureTheFlag.blueflagbase.transform.position) < 0.5f && PlayerControl.LocalPlayer == CaptureTheFlag.bluePlayerWhoHasRedFlag)
                {
                    CanUse = true;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { blueplayer05TakeFlagButton.Timer = blueplayer05TakeFlagButton.MaxTimer; },
            AssetLoader.CaptureTheFlagStealRedFlagButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            Tr.Get(TrKey.StealButton)
        );

        // Blueplayer06 Kill
        blueplayer06KillButton = new CustomButton(
            () =>
            {
                byte targetId = CaptureTheFlag.blueplayer06currentTarget.PlayerId;
                byte sourceId = CaptureTheFlag.blueplayer06.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                killWriter.Write(targetId);
                killWriter.Write(sourceId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.gamemodeKills(targetId, sourceId);
                blueplayer06KillButton.Timer = blueplayer06KillButton.MaxTimer;
                CaptureTheFlag.blueplayer06currentTarget = null;
            },
            () => { return CaptureTheFlag.blueplayer06 != null && CaptureTheFlag.blueplayer06 == PlayerControl.LocalPlayer; },
            () => { return CaptureTheFlag.blueplayer06currentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !CaptureTheFlag.blueplayer06IsReviving && PlayerControl.LocalPlayer != CaptureTheFlag.bluePlayerWhoHasRedFlag; },
            () => { blueplayer06KillButton.Timer = blueplayer06KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // Blueplayer06 TakeFlag Button
        blueplayer06TakeFlagButton = new CustomButton(
            () =>
            {
                if (PlayerControl.LocalPlayer == CaptureTheFlag.bluePlayerWhoHasRedFlag)
                {
                    MessageWriter whichTeamScored = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhichTeamScored, Hazel.SendOption.Reliable, -1);
                    whichTeamScored.Write(2);
                    AmongUsClient.Instance.FinishRpcImmediately(whichTeamScored);
                    RPCProcedure.captureTheFlagWhichTeamScored(2);
                }
                else
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    MessageWriter bluePlayerStoleRedFlag = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhoTookTheFlag, Hazel.SendOption.Reliable, -1);
                    bluePlayerStoleRedFlag.Write(targetId);
                    bluePlayerStoleRedFlag.Write(2);
                    AmongUsClient.Instance.FinishRpcImmediately(bluePlayerStoleRedFlag);
                    RPCProcedure.captureTheFlagWhoTookTheFlag(targetId, 2);
                }
                blueplayer06TakeFlagButton.Timer = blueplayer06TakeFlagButton.MaxTimer;
            },
            () => { return CaptureTheFlag.blueplayer06 != null && CaptureTheFlag.blueplayer06 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (CaptureTheFlag.localRedFlagArrow.Count != 0)
                {
                    CaptureTheFlag.localRedFlagArrow[0].Update(CaptureTheFlag.redflag.transform.position);
                }
                if (CaptureTheFlag.localBlueFlagArrow.Count != 0)
                {
                    CaptureTheFlag.localBlueFlagArrow[0].Update(CaptureTheFlag.blueflag.transform.position);
                }
                if (CaptureTheFlag.blueplayer06 == CaptureTheFlag.bluePlayerWhoHasRedFlag)
                    blueplayer06TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagDeliverRedFlagButton;
                else
                    blueplayer06TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagStealRedFlagButton;
                bool CanUse = false;
                if (CaptureTheFlag.redflag != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, CaptureTheFlag.redflag.transform.position) < 0.5f && !PlayerControl.LocalPlayer.Data.IsDead && CaptureTheFlag.bluePlayerWhoHasRedFlag == null)
                {
                    CanUse = true;
                }
                else if (CaptureTheFlag.blueflagbase != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, CaptureTheFlag.blueflagbase.transform.position) < 0.5f && PlayerControl.LocalPlayer == CaptureTheFlag.bluePlayerWhoHasRedFlag)
                {
                    CanUse = true;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { blueplayer06TakeFlagButton.Timer = blueplayer06TakeFlagButton.MaxTimer; },
            AssetLoader.CaptureTheFlagStealRedFlagButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            Tr.Get(TrKey.StealButton)
        );

        // Blueplayer07 Kill
        blueplayer07KillButton = new CustomButton(
            () =>
            {
                byte targetId = CaptureTheFlag.blueplayer07currentTarget.PlayerId;
                byte sourceId = CaptureTheFlag.blueplayer07.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                killWriter.Write(targetId);
                killWriter.Write(sourceId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.gamemodeKills(targetId, sourceId);
                blueplayer07KillButton.Timer = blueplayer07KillButton.MaxTimer;
                CaptureTheFlag.blueplayer07currentTarget = null;
            },
            () => { return CaptureTheFlag.blueplayer07 != null && CaptureTheFlag.blueplayer07 == PlayerControl.LocalPlayer; },
            () => { return CaptureTheFlag.blueplayer07currentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !CaptureTheFlag.blueplayer07IsReviving && PlayerControl.LocalPlayer != CaptureTheFlag.bluePlayerWhoHasRedFlag; },
            () => { blueplayer07KillButton.Timer = blueplayer07KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // Blueplayer07 TakeFlag Button
        blueplayer07TakeFlagButton = new CustomButton(
            () =>
            {
                if (PlayerControl.LocalPlayer == CaptureTheFlag.bluePlayerWhoHasRedFlag)
                {
                    MessageWriter whichTeamScored = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhichTeamScored, Hazel.SendOption.Reliable, -1);
                    whichTeamScored.Write(2);
                    AmongUsClient.Instance.FinishRpcImmediately(whichTeamScored);
                    RPCProcedure.captureTheFlagWhichTeamScored(2);
                }
                else
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    MessageWriter bluePlayerStoleRedFlag = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CaptureTheFlagWhoTookTheFlag, Hazel.SendOption.Reliable, -1);
                    bluePlayerStoleRedFlag.Write(targetId);
                    bluePlayerStoleRedFlag.Write(2);
                    AmongUsClient.Instance.FinishRpcImmediately(bluePlayerStoleRedFlag);
                    RPCProcedure.captureTheFlagWhoTookTheFlag(targetId, 2);
                }
                blueplayer07TakeFlagButton.Timer = blueplayer07TakeFlagButton.MaxTimer;
            },
            () => { return CaptureTheFlag.blueplayer07 != null && CaptureTheFlag.blueplayer07 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (CaptureTheFlag.localRedFlagArrow.Count != 0)
                {
                    CaptureTheFlag.localRedFlagArrow[0].Update(CaptureTheFlag.redflag.transform.position);
                }
                if (CaptureTheFlag.localBlueFlagArrow.Count != 0)
                {
                    CaptureTheFlag.localBlueFlagArrow[0].Update(CaptureTheFlag.blueflag.transform.position);
                }
                if (CaptureTheFlag.blueplayer07 == CaptureTheFlag.bluePlayerWhoHasRedFlag)
                    blueplayer07TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagDeliverRedFlagButton;
                else
                    blueplayer07TakeFlagButton.ActionButton.graphic.sprite = AssetLoader.CaptureTheFlagStealRedFlagButton;
                bool CanUse = false;
                if (CaptureTheFlag.redflag != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, CaptureTheFlag.redflag.transform.position) < 0.5f && !PlayerControl.LocalPlayer.Data.IsDead && CaptureTheFlag.bluePlayerWhoHasRedFlag == null)
                {
                    CanUse = true;
                }
                else if (CaptureTheFlag.blueflagbase != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, CaptureTheFlag.blueflagbase.transform.position) < 0.5f && PlayerControl.LocalPlayer == CaptureTheFlag.bluePlayerWhoHasRedFlag)
                {
                    CanUse = true;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { blueplayer07TakeFlagButton.Timer = blueplayer07TakeFlagButton.MaxTimer; },
            AssetLoader.CaptureTheFlagStealRedFlagButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            Tr.Get(TrKey.StealButton)
        );

        // stealer Kill
        stealerPlayerKillButton = new CustomButton(
            () =>
            {
                byte targetId = CaptureTheFlag.stealerPlayercurrentTarget.PlayerId;
                byte sourceId = CaptureTheFlag.stealerPlayer.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                killWriter.Write(targetId);
                killWriter.Write(sourceId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.gamemodeKills(targetId, sourceId);
                stealerPlayerKillButton.Timer = stealerPlayerKillButton.MaxTimer;
                CaptureTheFlag.stealerPlayercurrentTarget = null;
            },
            () => { return CaptureTheFlag.stealerPlayer != null && CaptureTheFlag.stealerPlayer == PlayerControl.LocalPlayer; },
            () =>
            {
                if (CaptureTheFlag.localRedFlagArrow.Count != 0)
                {
                    CaptureTheFlag.localRedFlagArrow[0].Update(CaptureTheFlag.redflag.transform.position);
                }
                if (CaptureTheFlag.localBlueFlagArrow.Count != 0)
                {
                    CaptureTheFlag.localBlueFlagArrow[0].Update(CaptureTheFlag.blueflag.transform.position);
                }
                bool canUse = true;
                foreach (GameObject spawns in CaptureTheFlag.stealerSpawns)
                {
                    if (spawns != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, spawns.transform.position) < 2.5f)
                    {
                        canUse = false;
                    }
                }
                return canUse && CaptureTheFlag.stealerPlayercurrentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !CaptureTheFlag.stealerPlayerIsReviving;
            },
            () => { stealerPlayerKillButton.Timer = stealerPlayerKillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );
    }
}