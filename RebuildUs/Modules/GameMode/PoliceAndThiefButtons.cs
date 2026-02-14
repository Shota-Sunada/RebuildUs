namespace RebuildUs.Modules.GameMode;

public static partial class PoliceAndThief
{
    // Police and Thief
    private static CustomButton policeplayer01JailButton;
    private static CustomButton policeplayer01KillButton;
    private static CustomButton policeplayer01LightButton;
    private static CustomButton policeplayer02KillButton;
    private static CustomButton policeplayer02TaseButton;
    private static CustomButton policeplayer02LightButton;
    private static CustomButton policeplayer03JailButton;
    private static CustomButton policeplayer03KillButton;
    private static CustomButton policeplayer03LightButton;
    private static CustomButton policeplayer04TaseButton;
    private static CustomButton policeplayer04KillButton;
    private static CustomButton policeplayer04LightButton;
    private static CustomButton policeplayer05JailButton;
    private static CustomButton policeplayer05KillButton;
    private static CustomButton policeplayer05LightButton;
    private static CustomButton policeplayer06JailButton;
    private static CustomButton policeplayer06KillButton;
    private static CustomButton policeplayer06LightButton;

    private static CustomButton thiefplayer01KillButton;
    private static CustomButton thiefplayer01FreeThiefButton;
    private static CustomButton thiefplayer01TakeDeliverJewelButton;
    private static CustomButton thiefplayer02KillButton;
    private static CustomButton thiefplayer02FreeThiefButton;
    private static CustomButton thiefplayer02TakeDeliverJewelButton;
    private static CustomButton thiefplayer03KillButton;
    private static CustomButton thiefplayer03FreeThiefButton;
    private static CustomButton thiefplayer03TakeDeliverJewelButton;
    private static CustomButton thiefplayer04KillButton;
    private static CustomButton thiefplayer04FreeThiefButton;
    private static CustomButton thiefplayer04TakeDeliverJewelButton;
    private static CustomButton thiefplayer05KillButton;
    private static CustomButton thiefplayer05FreeThiefButton;
    private static CustomButton thiefplayer05TakeDeliverJewelButton;
    private static CustomButton thiefplayer06KillButton;
    private static CustomButton thiefplayer06FreeThiefButton;
    private static CustomButton thiefplayer06TakeDeliverJewelButton;
    private static CustomButton thiefplayer07KillButton;
    private static CustomButton thiefplayer07FreeThiefButton;
    private static CustomButton thiefplayer07TakeDeliverJewelButton;
    private static CustomButton thiefplayer08KillButton;
    private static CustomButton thiefplayer08FreeThiefButton;
    private static CustomButton thiefplayer08TakeDeliverJewelButton;
    private static CustomButton thiefplayer09KillButton;
    private static CustomButton thiefplayer09FreeThiefButton;
    private static CustomButton thiefplayer09TakeDeliverJewelButton;

    public static void SetButtonCooldowns()
    {
        // Police And Thief buttons
        policeplayer01KillButton.MaxTimer = MapSettings.gamemodeKillCooldown;
        policeplayer01JailButton.MaxTimer = PoliceAndThief.policeCatchCooldown;
        policeplayer01JailButton.EffectDuration = PoliceAndThief.captureThiefTime;
        policeplayer01LightButton.MaxTimer = 15;
        policeplayer01LightButton.EffectDuration = 10;
        policeplayer02KillButton.MaxTimer = MapSettings.gamemodeKillCooldown;
        policeplayer02TaseButton.MaxTimer = PoliceAndThief.policeTaseCooldown;
        policeplayer02LightButton.MaxTimer = 15;
        policeplayer02LightButton.EffectDuration = 10;
        policeplayer03KillButton.MaxTimer = MapSettings.gamemodeKillCooldown;
        policeplayer03JailButton.MaxTimer = PoliceAndThief.policeCatchCooldown;
        policeplayer03JailButton.EffectDuration = PoliceAndThief.captureThiefTime;
        policeplayer03LightButton.MaxTimer = 15;
        policeplayer03LightButton.EffectDuration = 10;
        policeplayer04KillButton.MaxTimer = MapSettings.gamemodeKillCooldown;
        policeplayer04TaseButton.MaxTimer = PoliceAndThief.policeTaseCooldown;
        policeplayer04LightButton.MaxTimer = 15;
        policeplayer04LightButton.EffectDuration = 10;
        policeplayer05KillButton.MaxTimer = MapSettings.gamemodeKillCooldown;
        policeplayer05JailButton.MaxTimer = PoliceAndThief.policeCatchCooldown;
        policeplayer05JailButton.EffectDuration = PoliceAndThief.captureThiefTime;
        policeplayer05LightButton.MaxTimer = 15;
        policeplayer05LightButton.EffectDuration = 10;
        policeplayer06KillButton.MaxTimer = MapSettings.gamemodeKillCooldown;
        policeplayer06JailButton.MaxTimer = PoliceAndThief.policeCatchCooldown;
        policeplayer06JailButton.EffectDuration = PoliceAndThief.captureThiefTime;
        policeplayer06LightButton.MaxTimer = 15;
        policeplayer06LightButton.EffectDuration = 10;

        thiefplayer01KillButton.MaxTimer = MapSettings.gamemodeKillCooldown * 1.25f;
        thiefplayer01FreeThiefButton.MaxTimer = 20f;
        thiefplayer01TakeDeliverJewelButton.MaxTimer = 5f;
        thiefplayer02KillButton.MaxTimer = MapSettings.gamemodeKillCooldown * 1.25f;
        thiefplayer02FreeThiefButton.MaxTimer = 20f;
        thiefplayer02TakeDeliverJewelButton.MaxTimer = 5f;
        thiefplayer03KillButton.MaxTimer = MapSettings.gamemodeKillCooldown * 1.25f;
        thiefplayer03FreeThiefButton.MaxTimer = 20f;
        thiefplayer03TakeDeliverJewelButton.MaxTimer = 5f;
        thiefplayer04KillButton.MaxTimer = MapSettings.gamemodeKillCooldown * 1.25f;
        thiefplayer04FreeThiefButton.MaxTimer = 20f;
        thiefplayer04TakeDeliverJewelButton.MaxTimer = 5f;
        thiefplayer05KillButton.MaxTimer = MapSettings.gamemodeKillCooldown * 1.25f;
        thiefplayer05FreeThiefButton.MaxTimer = 20f;
        thiefplayer05TakeDeliverJewelButton.MaxTimer = 5f;
        thiefplayer06KillButton.MaxTimer = MapSettings.gamemodeKillCooldown * 1.25f;
        thiefplayer06FreeThiefButton.MaxTimer = 20f;
        thiefplayer06TakeDeliverJewelButton.MaxTimer = 5f;
        thiefplayer07KillButton.MaxTimer = MapSettings.gamemodeKillCooldown * 1.25f;
        thiefplayer07FreeThiefButton.MaxTimer = 20f;
        thiefplayer07TakeDeliverJewelButton.MaxTimer = 5f;
        thiefplayer08KillButton.MaxTimer = MapSettings.gamemodeKillCooldown * 1.25f;
        thiefplayer08FreeThiefButton.MaxTimer = 20f;
        thiefplayer08TakeDeliverJewelButton.MaxTimer = 5f;
        thiefplayer09KillButton.MaxTimer = MapSettings.gamemodeKillCooldown * 1.25f;
        thiefplayer09FreeThiefButton.MaxTimer = 20f;
        thiefplayer09TakeDeliverJewelButton.MaxTimer = 5f;
    }

    public static void MakeButtons(HudManager __instance)
    {
        // Police and Thief Mode
        // Policeplayer01 Kill
        policeplayer01KillButton = new CustomButton(
            () =>
            {
                byte targetId = PoliceAndThief.policeplayer01currentTarget.PlayerId;
                byte sourceId = PoliceAndThief.policeplayer01.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                killWriter.Write(targetId);
                killWriter.Write(sourceId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.gamemodeKills(targetId, sourceId);
                policeplayer01KillButton.Timer = policeplayer01KillButton.MaxTimer;
                PoliceAndThief.policeplayer01currentTarget = null;
            },
            () => { return PoliceAndThief.policeplayer01 != null && PoliceAndThief.policeplayer01 == PlayerControl.LocalPlayer; },
            () =>
            {
                bool CanUse = true;
                if ((PoliceAndThief.cellbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.cellbuttontwo.transform.position) <= 3f || PoliceAndThief.cellbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.cellbutton.transform.position) <= 3f) && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    CanUse = false;
                }
                return CanUse && PoliceAndThief.policeplayer01currentTarget && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.policeplayer01IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { policeplayer01KillButton.Timer = policeplayer01KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // Policeplayer01 Jail
        policeplayer01JailButton = new CustomButton(
            () =>
            {
                if (PoliceAndThief.policeplayer01currentTarget != null)
                {
                    PoliceAndThief.policeplayer01targetedPlayer = PoliceAndThief.policeplayer01currentTarget;
                    policeplayer01JailButton.HasEffect = true;
                }
            },
            () => { return PoliceAndThief.policeplayer01 != null && PoliceAndThief.policeplayer01 == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
            () =>
            {
                if (policeplayer01JailButton.IsEffectActive && PoliceAndThief.policeplayer01targetedPlayer != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.policeplayer01targetedPlayer.transform.position) > LegacyGameOptions.KillDistances[Mathf.Clamp(GameOptionsManager.Instance.currentGameOptions.GetInt(Int32OptionNames.KillDistance), 0, 2)])
                {
                    PoliceAndThief.policeplayer01targetedPlayer = null;
                    policeplayer01JailButton.Timer = 0f;
                    policeplayer01JailButton.IsEffectActive = false;
                }
                bool CanUse = true;
                if ((PoliceAndThief.jewelbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.jewelbuttontwo.transform.position) <= 3f || PoliceAndThief.jewelbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.jewelbutton.transform.position) <= 3f) && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    CanUse = false;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.policeplayer01IsReviving && PoliceAndThief.policeplayer01currentTarget != null;
            },
            () =>
            {
                PoliceAndThief.policeplayer01targetedPlayer = null;
                policeplayer01JailButton.IsEffectActive = false;
                policeplayer01JailButton.Timer = policeplayer01JailButton.MaxTimer;
                policeplayer01JailButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            AssetLoader.PoliceAndThiefCaptureButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CommonAbilityPrimary,
            true,
            PoliceAndThief.captureThiefTime,
            () =>
            {
                if (PoliceAndThief.policeplayer01targetedPlayer != null && !PoliceAndThief.policeplayer01targetedPlayer.Data.IsDead)
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefJail, Hazel.SendOption.Reliable, -1);
                    writer.Write(PoliceAndThief.policeplayer01targetedPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.policeandThiefJail(PoliceAndThief.policeplayer01targetedPlayer.PlayerId);
                    PoliceAndThief.policeplayer01targetedPlayer = null;
                    policeplayer01JailButton.Timer = policeplayer01JailButton.MaxTimer;
                }
            }
        );

        // Policeplayer01 Light
        policeplayer01LightButton = new CustomButton(
            () =>
            {
                PoliceAndThief.policeplayer01lightTimer = 10;
            },
            () => { return PoliceAndThief.policeplayer01 != null && PoliceAndThief.policeplayer01 == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.policeplayer01IsReviving; },
            () =>
            {
                policeplayer01LightButton.Timer = policeplayer01LightButton.MaxTimer;
                policeplayer01LightButton.IsEffectActive = false;
                policeplayer01LightButton.ActionButton.graphic.color = Palette.EnabledColor;
            },
            AssetLoader.PoliceAndThiefsLightButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.ImpostorAbilitySecondary,
            true,
            10,
            () => { policeplayer01LightButton.Timer = policeplayer01LightButton.MaxTimer; }
        );

        // Policeplayer02 Kill
        policeplayer02KillButton = new CustomButton(
            () =>
            {
                byte targetId = PoliceAndThief.policeplayer02currentTarget.PlayerId;
                byte sourceId = PoliceAndThief.policeplayer02.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                killWriter.Write(targetId);
                killWriter.Write(sourceId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.gamemodeKills(targetId, sourceId);
                policeplayer02KillButton.Timer = policeplayer02KillButton.MaxTimer;
                PoliceAndThief.policeplayer02currentTarget = null;
            },
            () => { return PoliceAndThief.policeplayer02 != null && PoliceAndThief.policeplayer02 == PlayerControl.LocalPlayer; },
            () =>
            {
                bool CanUse = true;
                if ((PoliceAndThief.cellbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.cellbuttontwo.transform.position) <= 3f || PoliceAndThief.cellbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.cellbutton.transform.position) <= 3f) && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    CanUse = false;
                }
                return CanUse && PoliceAndThief.policeplayer02currentTarget && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.policeplayer02IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { policeplayer02KillButton.Timer = policeplayer02KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // Policeplayer02 Tase
        policeplayer02TaseButton = new CustomButton(
            () =>
            {
                PlayerControl target = PoliceAndThief.GetTasedPlayer(2 * 0.2f, 6, true);

                if (target == null)
                {
                    target = PlayerControl.LocalPlayer;
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefsTased, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.policeandThiefsTased(target.PlayerId);

                policeplayer02TaseButton.Timer = policeplayer02TaseButton.MaxTimer;
                PoliceAndThief.policeplayer02currentTarget = null;

                target = null;
            },
            () => { return PoliceAndThief.policeplayer02 != null && PoliceAndThief.policeplayer02 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (PoliceAndThief.policeplayer02Taser == null)
                {
                    PoliceAndThief.policeplayer02Taser = new GameObject("Weapon");
                    var renderer = PoliceAndThief.policeplayer02Taser.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.PoliceAndThiefsTaser;
                    renderer.transform.parent = PoliceAndThief.policeplayer02.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {
                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    PoliceAndThief.policeplayer02mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = PoliceAndThief.policeplayer02.transform.position + new Vector3(0.8f * (float)Math.Cos(PoliceAndThief.policeplayer02mouseAngle), 0.8f * (float)Math.Sin(PoliceAndThief.policeplayer02mouseAngle));
                    PoliceAndThief.policeplayer02Taser.transform.position += (targetPosition - PoliceAndThief.policeplayer02Taser.transform.position) * 0.4f;
                    PoliceAndThief.policeplayer02Taser.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(PoliceAndThief.policeplayer02mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(PoliceAndThief.policeplayer02mouseAngle) < 0.0)
                    {
                        if (PoliceAndThief.policeplayer02Taser.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            PoliceAndThief.policeplayer02Taser.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (PoliceAndThief.policeplayer02Taser.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            PoliceAndThief.policeplayer02Taser.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                bool CanUse = true;
                if ((PoliceAndThief.jewelbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.jewelbuttontwo.transform.position) <= 3f || PoliceAndThief.jewelbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.jewelbutton.transform.position) <= 3f) && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    CanUse = false;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.policeplayer02IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { policeplayer02TaseButton.Timer = policeplayer02TaseButton.MaxTimer; },
            AssetLoader.PoliceAndThiefsTaserButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            KeyCode.Mouse1,
            false,
            Tr.Get(TrKey.ParalyzeButton)
        );

        // Policeplayer02 Light
        policeplayer02LightButton = new CustomButton(
            () =>
            {
                PoliceAndThief.policeplayer02lightTimer = 10;
            },
            () => { return PoliceAndThief.policeplayer02 != null && PoliceAndThief.policeplayer02 == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.policeplayer02IsReviving; },
            () =>
            {
                policeplayer02LightButton.Timer = policeplayer02LightButton.MaxTimer;
                policeplayer02LightButton.IsEffectActive = false;
                policeplayer02LightButton.ActionButton.graphic.color = Palette.EnabledColor;
            },
            AssetLoader.PoliceAndThiefsLightButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.ImpostorAbilitySecondary,
            true,
            10,
            () => { policeplayer02LightButton.Timer = policeplayer02LightButton.MaxTimer; }
        );

        // Policeplayer03 Kill
        policeplayer03KillButton = new CustomButton(
            () =>
            {
                byte targetId = PoliceAndThief.policeplayer03currentTarget.PlayerId;
                byte sourceId = PoliceAndThief.policeplayer03.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                killWriter.Write(targetId);
                killWriter.Write(sourceId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.gamemodeKills(targetId, sourceId);
                policeplayer03KillButton.Timer = policeplayer03KillButton.MaxTimer;
                PoliceAndThief.policeplayer03currentTarget = null;
            },
            () => { return PoliceAndThief.policeplayer03 != null && PoliceAndThief.policeplayer03 == PlayerControl.LocalPlayer; },
            () =>
            {
                bool CanUse = true;
                if ((PoliceAndThief.cellbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.cellbuttontwo.transform.position) <= 3f || PoliceAndThief.cellbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.cellbutton.transform.position) <= 3f) && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    CanUse = false;
                }
                return CanUse && PoliceAndThief.policeplayer03currentTarget && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.policeplayer03IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { policeplayer03KillButton.Timer = policeplayer03KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // Policeplayer03 Jail
        policeplayer03JailButton = new CustomButton(
            () =>
            {
                if (PoliceAndThief.policeplayer03currentTarget != null)
                {
                    PoliceAndThief.policeplayer03targetedPlayer = PoliceAndThief.policeplayer03currentTarget;
                    policeplayer03JailButton.HasEffect = true;
                }
            },
            () => { return PoliceAndThief.policeplayer03 != null && PoliceAndThief.policeplayer03 == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
            () =>
            {
                if (policeplayer03JailButton.IsEffectActive && PoliceAndThief.policeplayer03targetedPlayer != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.policeplayer03targetedPlayer.transform.position) > LegacyGameOptions.KillDistances[Mathf.Clamp(GameOptionsManager.Instance.currentGameOptions.GetInt(Int32OptionNames.KillDistance), 0, 2)])
                {
                    PoliceAndThief.policeplayer03targetedPlayer = null;
                    policeplayer03JailButton.Timer = 0f;
                    policeplayer03JailButton.IsEffectActive = false;
                }

                bool CanUse = true;
                if ((PoliceAndThief.jewelbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.jewelbuttontwo.transform.position) <= 3f || PoliceAndThief.jewelbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.jewelbutton.transform.position) <= 3f) && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    CanUse = false;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.policeplayer03IsReviving && PoliceAndThief.policeplayer03currentTarget != null;
            },
            () =>
            {
                PoliceAndThief.policeplayer03targetedPlayer = null;
                policeplayer03JailButton.IsEffectActive = false;
                policeplayer03JailButton.Timer = policeplayer03JailButton.MaxTimer;
                policeplayer03JailButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            AssetLoader.PoliceAndThiefCaptureButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CommonAbilityPrimary,
            true,
            PoliceAndThief.captureThiefTime,
            () =>
            {
                if (PoliceAndThief.policeplayer03targetedPlayer != null && !PoliceAndThief.policeplayer03targetedPlayer.Data.IsDead)
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefJail, Hazel.SendOption.Reliable, -1);
                    writer.Write(PoliceAndThief.policeplayer03targetedPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.policeandThiefJail(PoliceAndThief.policeplayer03targetedPlayer.PlayerId);
                    PoliceAndThief.policeplayer03targetedPlayer = null;
                    policeplayer03JailButton.Timer = policeplayer03JailButton.MaxTimer;
                }
            }
        );

        // Policeplayer03 Light
        policeplayer03LightButton = new CustomButton(
            () =>
            {
                PoliceAndThief.policeplayer03lightTimer = 10;
            },
            () => { return PoliceAndThief.policeplayer03 != null && PoliceAndThief.policeplayer03 == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.policeplayer03IsReviving; },
            () =>
            {
                policeplayer03LightButton.Timer = policeplayer03LightButton.MaxTimer;
                policeplayer03LightButton.IsEffectActive = false;
                policeplayer03LightButton.ActionButton.graphic.color = Palette.EnabledColor;
            },
            AssetLoader.PoliceAndThiefsLightButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.ImpostorAbilitySecondary,
            true,
            10,
            () => { policeplayer03LightButton.Timer = policeplayer03LightButton.MaxTimer; }
        );

        // Policeplayer04 Kill
        policeplayer04KillButton = new CustomButton(
            () =>
            {
                byte targetId = PoliceAndThief.policeplayer04currentTarget.PlayerId;
                byte sourceId = PoliceAndThief.policeplayer04.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                killWriter.Write(targetId);
                killWriter.Write(sourceId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.gamemodeKills(targetId, sourceId);
                policeplayer04KillButton.Timer = policeplayer04KillButton.MaxTimer;
                PoliceAndThief.policeplayer04currentTarget = null;
            },
            () => { return PoliceAndThief.policeplayer04 != null && PoliceAndThief.policeplayer04 == PlayerControl.LocalPlayer; },
            () =>
            {
                bool CanUse = true;
                if ((PoliceAndThief.cellbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.cellbuttontwo.transform.position) <= 3f || PoliceAndThief.cellbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.cellbutton.transform.position) <= 3f) && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    CanUse = false;
                }
                return CanUse && PoliceAndThief.policeplayer04currentTarget && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.policeplayer04IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { policeplayer04KillButton.Timer = policeplayer04KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // Policeplayer04 Tase
        policeplayer04TaseButton = new CustomButton(
            () =>
            {
                PlayerControl target = PoliceAndThief.GetTasedPlayer(2 * 0.2f, 6, false);

                if (target == null)
                {
                    target = PlayerControl.LocalPlayer;
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefsTased, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.policeandThiefsTased(target.PlayerId);

                policeplayer04TaseButton.Timer = policeplayer04TaseButton.MaxTimer;
                PoliceAndThief.policeplayer04currentTarget = null;

                target = null;
            },
            () => { return PoliceAndThief.policeplayer04 != null && PoliceAndThief.policeplayer04 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (PoliceAndThief.policeplayer04Taser == null)
                {
                    PoliceAndThief.policeplayer04Taser = new GameObject("Weapon");
                    var renderer = PoliceAndThief.policeplayer04Taser.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.PoliceAndThiefsTaser;
                    renderer.transform.parent = PoliceAndThief.policeplayer04.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {
                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    PoliceAndThief.policeplayer04mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = PoliceAndThief.policeplayer04.transform.position + new Vector3(0.8f * (float)Math.Cos(PoliceAndThief.policeplayer04mouseAngle), 0.8f * (float)Math.Sin(PoliceAndThief.policeplayer04mouseAngle));
                    PoliceAndThief.policeplayer04Taser.transform.position += (targetPosition - PoliceAndThief.policeplayer04Taser.transform.position) * 0.4f;
                    PoliceAndThief.policeplayer04Taser.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(PoliceAndThief.policeplayer04mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(PoliceAndThief.policeplayer04mouseAngle) < 0.0)
                    {
                        if (PoliceAndThief.policeplayer04Taser.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            PoliceAndThief.policeplayer04Taser.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (PoliceAndThief.policeplayer04Taser.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            PoliceAndThief.policeplayer04Taser.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                bool CanUse = true;
                if ((PoliceAndThief.jewelbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.jewelbuttontwo.transform.position) <= 3f || PoliceAndThief.jewelbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.jewelbutton.transform.position) <= 3f) && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    CanUse = false;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.policeplayer04IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { policeplayer04TaseButton.Timer = policeplayer04TaseButton.MaxTimer; },
            AssetLoader.PoliceAndThiefsTaserButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            KeyCode.Mouse1,
            false,
            Tr.Get(TrKey.ParalyzeButton)
        );

        // Policeplayer04 Light
        policeplayer04LightButton = new CustomButton(
            () =>
            {
                PoliceAndThief.policeplayer04lightTimer = 10;
            },
            () => { return PoliceAndThief.policeplayer04 != null && PoliceAndThief.policeplayer04 == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.policeplayer04IsReviving; },
            () =>
            {
                policeplayer04LightButton.Timer = policeplayer04LightButton.MaxTimer;
                policeplayer04LightButton.IsEffectActive = false;
                policeplayer04LightButton.ActionButton.graphic.color = Palette.EnabledColor;
            },
            AssetLoader.PoliceAndThiefsLightButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.ImpostorAbilitySecondary,
            true,
            10,
            () => { policeplayer04LightButton.Timer = policeplayer04LightButton.MaxTimer; }
        );

        // Policeplayer05 Kill
        policeplayer05KillButton = new CustomButton(
            () =>
            {
                byte targetId = PoliceAndThief.policeplayer05currentTarget.PlayerId;
                byte sourceId = PoliceAndThief.policeplayer05.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                killWriter.Write(targetId);
                killWriter.Write(sourceId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.gamemodeKills(targetId, sourceId);
                policeplayer05KillButton.Timer = policeplayer05KillButton.MaxTimer;
                PoliceAndThief.policeplayer05currentTarget = null;
            },
            () => { return PoliceAndThief.policeplayer05 != null && PoliceAndThief.policeplayer05 == PlayerControl.LocalPlayer; },
            () =>
            {
                bool CanUse = true;
                if ((PoliceAndThief.cellbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.cellbuttontwo.transform.position) <= 3f || PoliceAndThief.cellbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.cellbutton.transform.position) <= 3f) && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    CanUse = false;
                }
                return CanUse && PoliceAndThief.policeplayer05currentTarget && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.policeplayer05IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { policeplayer05KillButton.Timer = policeplayer05KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // Policeplayer05 Jail
        policeplayer05JailButton = new CustomButton(
            () =>
            {
                if (PoliceAndThief.policeplayer05currentTarget != null)
                {
                    PoliceAndThief.policeplayer05targetedPlayer = PoliceAndThief.policeplayer05currentTarget;
                    policeplayer05JailButton.HasEffect = true;
                }
            },
            () => { return PoliceAndThief.policeplayer05 != null && PoliceAndThief.policeplayer05 == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
            () =>
            {
                if (policeplayer05JailButton.IsEffectActive && PoliceAndThief.policeplayer05targetedPlayer != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.policeplayer05targetedPlayer.transform.position) > LegacyGameOptions.KillDistances[Mathf.Clamp(GameOptionsManager.Instance.currentGameOptions.GetInt(Int32OptionNames.KillDistance), 0, 2)])
                {
                    PoliceAndThief.policeplayer05targetedPlayer = null;
                    policeplayer05JailButton.Timer = 0f;
                    policeplayer05JailButton.IsEffectActive = false;
                }

                bool CanUse = true;
                if ((PoliceAndThief.jewelbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.jewelbuttontwo.transform.position) <= 3f || PoliceAndThief.jewelbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.jewelbutton.transform.position) <= 3f) && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    CanUse = false;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.policeplayer05IsReviving && PoliceAndThief.policeplayer05currentTarget != null;
            },
            () =>
            {
                PoliceAndThief.policeplayer05targetedPlayer = null;
                policeplayer05JailButton.IsEffectActive = false;
                policeplayer05JailButton.Timer = policeplayer05JailButton.MaxTimer;
                policeplayer05JailButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            AssetLoader.PoliceAndThiefCaptureButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CommonAbilityPrimary,
            true,
            PoliceAndThief.captureThiefTime,
            () =>
            {
                if (PoliceAndThief.policeplayer05targetedPlayer != null && !PoliceAndThief.policeplayer05targetedPlayer.Data.IsDead)
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefJail, Hazel.SendOption.Reliable, -1);
                    writer.Write(PoliceAndThief.policeplayer05targetedPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.policeandThiefJail(PoliceAndThief.policeplayer05targetedPlayer.PlayerId);
                    PoliceAndThief.policeplayer05targetedPlayer = null;
                    policeplayer05JailButton.Timer = policeplayer05JailButton.MaxTimer;
                }
            }
        );

        // Policeplayer05 Light
        policeplayer05LightButton = new CustomButton(
            () =>
            {
                PoliceAndThief.policeplayer05lightTimer = 10;
            },
            () => { return PoliceAndThief.policeplayer05 != null && PoliceAndThief.policeplayer05 == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.policeplayer05IsReviving; },
            () =>
            {
                policeplayer05LightButton.Timer = policeplayer05LightButton.MaxTimer;
                policeplayer05LightButton.IsEffectActive = false;
                policeplayer05LightButton.ActionButton.graphic.color = Palette.EnabledColor;
            },
            AssetLoader.PoliceAndThiefsLightButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.ImpostorAbilitySecondary,
            true,
            10,
            () => { policeplayer05LightButton.Timer = policeplayer05LightButton.MaxTimer; }
        );

        // Policeplayer06 Kill
        policeplayer06KillButton = new CustomButton(
            () =>
            {
                byte targetId = PoliceAndThief.policeplayer06currentTarget.PlayerId;
                byte sourceId = PoliceAndThief.policeplayer06.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                killWriter.Write(targetId);
                killWriter.Write(sourceId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.gamemodeKills(targetId, sourceId);
                policeplayer06KillButton.Timer = policeplayer06KillButton.MaxTimer;
                PoliceAndThief.policeplayer06currentTarget = null;
            },
            () => { return PoliceAndThief.policeplayer06 != null && PoliceAndThief.policeplayer06 == PlayerControl.LocalPlayer; },
            () =>
            {
                bool CanUse = true;
                if ((PoliceAndThief.cellbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.cellbuttontwo.transform.position) <= 3f || PoliceAndThief.cellbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.cellbutton.transform.position) <= 3f) && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    CanUse = false;
                }
                return CanUse && PoliceAndThief.policeplayer06currentTarget && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.policeplayer06IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { policeplayer06KillButton.Timer = policeplayer06KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            AbilitySlot.ImpostorAbilityPrimary,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // Policeplayer06 Jail
        policeplayer06JailButton = new CustomButton(
            () =>
            {
                if (PoliceAndThief.policeplayer06currentTarget != null)
                {
                    PoliceAndThief.policeplayer06targetedPlayer = PoliceAndThief.policeplayer06currentTarget;
                    policeplayer06JailButton.HasEffect = true;
                }
            },
            () => { return PoliceAndThief.policeplayer06 != null && PoliceAndThief.policeplayer06 == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
            () =>
            {
                if (policeplayer06JailButton.IsEffectActive && PoliceAndThief.policeplayer06targetedPlayer != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.policeplayer06targetedPlayer.transform.position) > LegacyGameOptions.KillDistances[Mathf.Clamp(GameOptionsManager.Instance.currentGameOptions.GetInt(Int32OptionNames.KillDistance), 0, 2)])
                {
                    PoliceAndThief.policeplayer06targetedPlayer = null;
                    policeplayer06JailButton.Timer = 0f;
                    policeplayer06JailButton.IsEffectActive = false;
                }

                bool CanUse = true;
                if ((PoliceAndThief.jewelbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.jewelbuttontwo.transform.position) <= 3f || PoliceAndThief.jewelbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.jewelbutton.transform.position) <= 3f) && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    CanUse = false;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.policeplayer06IsReviving && PoliceAndThief.policeplayer06currentTarget != null;
            },
            () =>
            {
                PoliceAndThief.policeplayer06targetedPlayer = null;
                policeplayer06JailButton.IsEffectActive = false;
                policeplayer06JailButton.Timer = policeplayer06JailButton.MaxTimer;
                policeplayer06JailButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            AssetLoader.PoliceAndThiefCaptureButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CommonAbilityPrimary,
            true,
            PoliceAndThief.captureThiefTime,
            () =>
            {
                if (PoliceAndThief.policeplayer06targetedPlayer != null && !PoliceAndThief.policeplayer06targetedPlayer.Data.IsDead)
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefJail, Hazel.SendOption.Reliable, -1);
                    writer.Write(PoliceAndThief.policeplayer06targetedPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.policeandThiefJail(PoliceAndThief.policeplayer06targetedPlayer.PlayerId);
                    PoliceAndThief.policeplayer06targetedPlayer = null;
                    policeplayer06JailButton.Timer = policeplayer06JailButton.MaxTimer;
                }
            }
        );

        // Policeplayer06 Light
        policeplayer06LightButton = new CustomButton(
            () =>
            {
                PoliceAndThief.policeplayer06lightTimer = 10;
            },
            () => { return PoliceAndThief.policeplayer06 != null && PoliceAndThief.policeplayer06 == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.policeplayer06IsReviving; },
            () =>
            {
                policeplayer06LightButton.Timer = policeplayer06LightButton.MaxTimer;
                policeplayer06LightButton.IsEffectActive = false;
                policeplayer06LightButton.ActionButton.graphic.color = Palette.EnabledColor;
            },
            AssetLoader.PoliceAndThiefsLightButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.ImpostorAbilitySecondary,
            true,
            10,
            () => { policeplayer06LightButton.Timer = policeplayer06LightButton.MaxTimer; }
        );

        // Thiefplayer01 Kill
        thiefplayer01KillButton = new CustomButton(
            () =>
            {
                byte targetId = PoliceAndThief.thiefplayer01currentTarget.PlayerId;
                byte sourceId = PoliceAndThief.thiefplayer01.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                killWriter.Write(targetId);
                killWriter.Write(sourceId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.gamemodeKills(targetId, sourceId);
                thiefplayer01KillButton.Timer = thiefplayer01KillButton.MaxTimer;
                PoliceAndThief.thiefplayer01currentTarget = null;
            },
            () => { return PoliceAndThief.thiefplayer01 != null && PoliceAndThief.thiefplayer01 == PlayerControl.LocalPlayer && PoliceAndThief.whoCanThiefsKill != 2; },
            () =>
            {
                bool canUse = false;
                switch (PoliceAndThief.whoCanThiefsKill)
                {
                    case 0:
                        if (PoliceAndThief.policeplayer02 != null && PoliceAndThief.thiefplayer01currentTarget == PoliceAndThief.policeplayer02 || PoliceAndThief.policeplayer04 != null && PoliceAndThief.thiefplayer01currentTarget == PoliceAndThief.policeplayer04)
                        {
                            canUse = true;
                        }
                        break;
                    case 1:
                        canUse = true;
                        break;
                }
                return canUse && PoliceAndThief.thiefplayer01currentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !PoliceAndThief.thiefplayer01IsReviving && !PoliceAndThief.thiefplayer01IsStealing;
            },
            () => { thiefplayer01KillButton.Timer = thiefplayer01KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // Thiefplayer01 FreeThief Button
        thiefplayer01FreeThiefButton = new CustomButton(
            () =>
            {
                MessageWriter thiefFree = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefFreeThief, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(thiefFree);
                RPCProcedure.policeandThiefFreeThief();
                thiefplayer01FreeThiefButton.Timer = thiefplayer01FreeThiefButton.MaxTimer;
            },
            () => { return PoliceAndThief.thiefplayer01 != null && PoliceAndThief.thiefplayer01 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (PoliceAndThief.localThiefReleaseArrow.Count != 0)
                {
                    PoliceAndThief.localThiefReleaseArrow[0].Update(PoliceAndThief.cellbutton.transform.position);
                    if (GameOptionsManager.Instance.currentGameOptions.MapId == 6)
                    {
                        PoliceAndThief.localThiefReleaseArrow[1].Update(PoliceAndThief.cellbuttontwo.transform.position);
                    }
                }
                if (PoliceAndThief.localThiefDeliverArrow.Count != 0)
                {
                    PoliceAndThief.localThiefDeliverArrow[0].Update(PoliceAndThief.jewelbutton.transform.position);
                    if (GameOptionsManager.Instance.currentGameOptions.MapId == 6)
                    {
                        PoliceAndThief.localThiefDeliverArrow[1].Update(PoliceAndThief.jewelbuttontwo.transform.position);
                    }
                }

                bool CanUse = false;
                if (PoliceAndThief.currentThiefsCaptured > 0)
                {
                    if ((PoliceAndThief.cellbuttontwo != null && Vector2.Distance(PoliceAndThief.thiefplayer01.transform.position, PoliceAndThief.cellbuttontwo.transform.position) < 0.4f || Vector2.Distance(PoliceAndThief.thiefplayer01.transform.position, PoliceAndThief.cellbutton.transform.position) < 0.4f) && !PoliceAndThief.thiefplayer01.Data.IsDead)
                    {
                        CanUse = true;
                    }
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.thiefplayer01IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { thiefplayer01FreeThiefButton.Timer = thiefplayer01FreeThiefButton.MaxTimer; },
            AssetLoader.PoliceAndThiefFreeButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CrewmateAbilitySecondary,
            false,
            Tr.Get(TrKey.ReleaseButton)
        );

        // Thiefplayer01 Take/Deliver Jewel Button
        thiefplayer01TakeDeliverJewelButton = new CustomButton(
            () =>
            {
                if (PoliceAndThief.thiefplayer01IsStealing)
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    byte jewelId = PoliceAndThief.thiefplayer01JewelId;
                    MessageWriter thiefScore = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefDeliverJewel, Hazel.SendOption.Reliable, -1);
                    thiefScore.Write(targetId);
                    thiefScore.Write(jewelId);
                    AmongUsClient.Instance.FinishRpcImmediately(thiefScore);
                    RPCProcedure.policeandThiefDeliverJewel(targetId, jewelId);
                }
                else
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    byte jewelId = PoliceAndThief.thiefplayer01JewelId;
                    MessageWriter thiefWhoTookATreasure = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefTakeJewel, Hazel.SendOption.Reliable, -1);
                    thiefWhoTookATreasure.Write(targetId);
                    thiefWhoTookATreasure.Write(jewelId);
                    AmongUsClient.Instance.FinishRpcImmediately(thiefWhoTookATreasure);
                    RPCProcedure.policeandThiefTakeJewel(targetId, jewelId);
                }
                thiefplayer01TakeDeliverJewelButton.Timer = thiefplayer01TakeDeliverJewelButton.MaxTimer;
            },
            () => { return PoliceAndThief.thiefplayer01 != null && PoliceAndThief.thiefplayer01 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (PoliceAndThief.thiefplayer01IsStealing)
                    thiefplayer01TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefDeliverJewelButton;
                else
                    thiefplayer01TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefTakeJewelButton;
                bool CanUse = false;
                if (PoliceAndThief.thiefTreasures.Count != 0)
                {
                    foreach (GameObject jewel in PoliceAndThief.thiefTreasures)
                    {
                        if (jewel != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, jewel.transform.position) < 0.5f && !PoliceAndThief.thiefplayer01IsStealing)
                        {
                            switch (jewel.name)
                            {
                                case "jewel01":
                                    PoliceAndThief.thiefplayer01JewelId = 1;
                                    CanUse = !PoliceAndThief.jewel01BeingStealed;
                                    break;
                                case "jewel02":
                                    PoliceAndThief.thiefplayer01JewelId = 2;
                                    CanUse = !PoliceAndThief.jewel02BeingStealed;
                                    break;
                                case "jewel03":
                                    PoliceAndThief.thiefplayer01JewelId = 3;
                                    CanUse = !PoliceAndThief.jewel03BeingStealed;
                                    break;
                                case "jewel04":
                                    PoliceAndThief.thiefplayer01JewelId = 4;
                                    CanUse = !PoliceAndThief.jewel04BeingStealed;
                                    break;
                                case "jewel05":
                                    PoliceAndThief.thiefplayer01JewelId = 5;
                                    CanUse = !PoliceAndThief.jewel05BeingStealed;
                                    break;
                                case "jewel06":
                                    PoliceAndThief.thiefplayer01JewelId = 6;
                                    CanUse = !PoliceAndThief.jewel06BeingStealed;
                                    break;
                                case "jewel07":
                                    PoliceAndThief.thiefplayer01JewelId = 7;
                                    CanUse = !PoliceAndThief.jewel07BeingStealed;
                                    break;
                                case "jewel08":
                                    PoliceAndThief.thiefplayer01JewelId = 8;
                                    CanUse = !PoliceAndThief.jewel08BeingStealed;
                                    break;
                                case "jewel09":
                                    PoliceAndThief.thiefplayer01JewelId = 9;
                                    CanUse = !PoliceAndThief.jewel09BeingStealed;
                                    break;
                                case "jewel10":
                                    PoliceAndThief.thiefplayer01JewelId = 10;
                                    CanUse = !PoliceAndThief.jewel10BeingStealed;
                                    break;
                                case "jewel11":
                                    PoliceAndThief.thiefplayer01JewelId = 11;
                                    CanUse = !PoliceAndThief.jewel11BeingStealed;
                                    break;
                                case "jewel12":
                                    PoliceAndThief.thiefplayer01JewelId = 12;
                                    CanUse = !PoliceAndThief.jewel12BeingStealed;
                                    break;
                                case "jewel13":
                                    PoliceAndThief.thiefplayer01JewelId = 13;
                                    CanUse = !PoliceAndThief.jewel13BeingStealed;
                                    break;
                                case "jewel14":
                                    PoliceAndThief.thiefplayer01JewelId = 14;
                                    CanUse = !PoliceAndThief.jewel14BeingStealed;
                                    break;
                                case "jewel15":
                                    PoliceAndThief.thiefplayer01JewelId = 15;
                                    CanUse = !PoliceAndThief.jewel15BeingStealed;
                                    break;
                            }
                        }
                        else if ((PoliceAndThief.jewelbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.jewelbuttontwo.transform.position) < 0.5f || PoliceAndThief.jewelbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.jewelbutton.transform.position) < 0.5f) && PoliceAndThief.thiefplayer01IsStealing)
                        {
                            CanUse = true;
                        }
                    }

                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.thiefplayer01IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { thiefplayer01TakeDeliverJewelButton.Timer = thiefplayer01TakeDeliverJewelButton.MaxTimer; },
            AssetLoader.PoliceAndThiefTakeJewelButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CommonAbilityPrimary,
            false,
            Tr.Get(TrKey.DeliverButton)
        );

        // Thiefplayer02 Kill
        thiefplayer02KillButton = new CustomButton(
            () =>
            {
                byte targetId = PoliceAndThief.thiefplayer02currentTarget.PlayerId;
                byte sourceId = PoliceAndThief.thiefplayer02.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                killWriter.Write(targetId);
                killWriter.Write(sourceId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.gamemodeKills(targetId, sourceId);
                thiefplayer02KillButton.Timer = thiefplayer02KillButton.MaxTimer;
                PoliceAndThief.thiefplayer02currentTarget = null;
            },
            () => { return PoliceAndThief.thiefplayer02 != null && PoliceAndThief.thiefplayer02 == PlayerControl.LocalPlayer && PoliceAndThief.whoCanThiefsKill != 2; },
            () =>
            {
                bool canUse = false;
                switch (PoliceAndThief.whoCanThiefsKill)
                {
                    case 0:
                        if (PoliceAndThief.policeplayer02 != null && PoliceAndThief.thiefplayer02currentTarget == PoliceAndThief.policeplayer02 || PoliceAndThief.policeplayer04 != null && PoliceAndThief.thiefplayer02currentTarget == PoliceAndThief.policeplayer04)
                        {
                            canUse = true;
                        }
                        break;
                    case 1:
                        canUse = true;
                        break;
                }
                return canUse && PoliceAndThief.thiefplayer02currentTarget && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.thiefplayer02IsReviving && !PlayerControl.LocalPlayer.Data.IsDead && !PoliceAndThief.thiefplayer02IsStealing;
            },
            () => { thiefplayer02KillButton.Timer = thiefplayer02KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // Thiefplayer02 FreeThief Button
        thiefplayer02FreeThiefButton = new CustomButton(
            () =>
            {
                MessageWriter thiefFree = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefFreeThief, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(thiefFree);
                RPCProcedure.policeandThiefFreeThief();
                thiefplayer02FreeThiefButton.Timer = thiefplayer02FreeThiefButton.MaxTimer;
            },
            () => { return PoliceAndThief.thiefplayer02 != null && PoliceAndThief.thiefplayer02 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (PoliceAndThief.localThiefReleaseArrow.Count != 0)
                {
                    PoliceAndThief.localThiefReleaseArrow[0].Update(PoliceAndThief.cellbutton.transform.position);
                    if (GameOptionsManager.Instance.currentGameOptions.MapId == 6)
                    {
                        PoliceAndThief.localThiefReleaseArrow[1].Update(PoliceAndThief.cellbuttontwo.transform.position);
                    }
                }
                if (PoliceAndThief.localThiefDeliverArrow.Count != 0)
                {
                    PoliceAndThief.localThiefDeliverArrow[0].Update(PoliceAndThief.jewelbutton.transform.position);
                    if (GameOptionsManager.Instance.currentGameOptions.MapId == 6)
                    {
                        PoliceAndThief.localThiefDeliverArrow[1].Update(PoliceAndThief.jewelbuttontwo.transform.position);
                    }
                }

                bool CanUse = false;
                if (PoliceAndThief.currentThiefsCaptured > 0)
                {
                    if ((PoliceAndThief.cellbuttontwo != null && Vector2.Distance(PoliceAndThief.thiefplayer02.transform.position, PoliceAndThief.cellbuttontwo.transform.position) < 0.4f || Vector2.Distance(PoliceAndThief.thiefplayer02.transform.position, PoliceAndThief.cellbutton.transform.position) < 0.4f) && !PoliceAndThief.thiefplayer02.Data.IsDead)
                    {
                        CanUse = true;
                    }
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.thiefplayer02IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { thiefplayer02FreeThiefButton.Timer = thiefplayer02FreeThiefButton.MaxTimer; },
            AssetLoader.PoliceAndThiefFreeButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CrewmateAbilitySecondary,
            false,
            Tr.Get(TrKey.ReleaseButton)
        );

        // Thiefplayer02 Take/Deliver Jewel Button
        thiefplayer02TakeDeliverJewelButton = new CustomButton(
            () =>
            {
                if (PoliceAndThief.thiefplayer02IsStealing)
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    byte jewelId = PoliceAndThief.thiefplayer02JewelId;
                    MessageWriter thiefScore = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefDeliverJewel, Hazel.SendOption.Reliable, -1);
                    thiefScore.Write(targetId);
                    thiefScore.Write(jewelId);
                    AmongUsClient.Instance.FinishRpcImmediately(thiefScore);
                    RPCProcedure.policeandThiefDeliverJewel(targetId, jewelId);
                }
                else
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    byte jewelId = PoliceAndThief.thiefplayer02JewelId;
                    MessageWriter thiefWhoTookATreasure = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefTakeJewel, Hazel.SendOption.Reliable, -1);
                    thiefWhoTookATreasure.Write(targetId);
                    thiefWhoTookATreasure.Write(jewelId);
                    AmongUsClient.Instance.FinishRpcImmediately(thiefWhoTookATreasure);
                    RPCProcedure.policeandThiefTakeJewel(targetId, jewelId);
                }
                thiefplayer02TakeDeliverJewelButton.Timer = thiefplayer02TakeDeliverJewelButton.MaxTimer;
            },
            () => { return PoliceAndThief.thiefplayer02 != null && PoliceAndThief.thiefplayer02 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (PoliceAndThief.thiefplayer02IsStealing)
                    thiefplayer02TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefDeliverJewelButton;
                else
                    thiefplayer02TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefTakeJewelButton;
                bool CanUse = false;
                if (PoliceAndThief.thiefTreasures.Count != 0)
                {
                    foreach (GameObject jewel in PoliceAndThief.thiefTreasures)
                    {
                        if (jewel != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, jewel.transform.position) < 0.5f && !PoliceAndThief.thiefplayer02IsStealing)
                        {
                            switch (jewel.name)
                            {
                                case "jewel01":
                                    PoliceAndThief.thiefplayer02JewelId = 1;
                                    CanUse = !PoliceAndThief.jewel01BeingStealed;
                                    break;
                                case "jewel02":
                                    PoliceAndThief.thiefplayer02JewelId = 2;
                                    CanUse = !PoliceAndThief.jewel02BeingStealed;
                                    break;
                                case "jewel03":
                                    PoliceAndThief.thiefplayer02JewelId = 3;
                                    CanUse = !PoliceAndThief.jewel03BeingStealed;
                                    break;
                                case "jewel04":
                                    PoliceAndThief.thiefplayer02JewelId = 4;
                                    CanUse = !PoliceAndThief.jewel04BeingStealed;
                                    break;
                                case "jewel05":
                                    PoliceAndThief.thiefplayer02JewelId = 5;
                                    CanUse = !PoliceAndThief.jewel05BeingStealed;
                                    break;
                                case "jewel06":
                                    PoliceAndThief.thiefplayer02JewelId = 6;
                                    CanUse = !PoliceAndThief.jewel06BeingStealed;
                                    break;
                                case "jewel07":
                                    PoliceAndThief.thiefplayer02JewelId = 7;
                                    CanUse = !PoliceAndThief.jewel07BeingStealed;
                                    break;
                                case "jewel08":
                                    PoliceAndThief.thiefplayer02JewelId = 8;
                                    CanUse = !PoliceAndThief.jewel08BeingStealed;
                                    break;
                                case "jewel09":
                                    PoliceAndThief.thiefplayer02JewelId = 9;
                                    CanUse = !PoliceAndThief.jewel09BeingStealed;
                                    break;
                                case "jewel10":
                                    PoliceAndThief.thiefplayer02JewelId = 10;
                                    CanUse = !PoliceAndThief.jewel10BeingStealed;
                                    break;
                                case "jewel11":
                                    PoliceAndThief.thiefplayer02JewelId = 11;
                                    CanUse = !PoliceAndThief.jewel11BeingStealed;
                                    break;
                                case "jewel12":
                                    PoliceAndThief.thiefplayer02JewelId = 12;
                                    CanUse = !PoliceAndThief.jewel12BeingStealed;
                                    break;
                                case "jewel13":
                                    PoliceAndThief.thiefplayer02JewelId = 13;
                                    CanUse = !PoliceAndThief.jewel13BeingStealed;
                                    break;
                                case "jewel14":
                                    PoliceAndThief.thiefplayer02JewelId = 14;
                                    CanUse = !PoliceAndThief.jewel14BeingStealed;
                                    break;
                                case "jewel15":
                                    PoliceAndThief.thiefplayer02JewelId = 15;
                                    CanUse = !PoliceAndThief.jewel15BeingStealed;
                                    break;
                            }
                        }
                        else if ((PoliceAndThief.jewelbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.jewelbuttontwo.transform.position) < 0.5f || PoliceAndThief.jewelbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.jewelbutton.transform.position) < 0.5f) && PoliceAndThief.thiefplayer02IsStealing)
                        {
                            CanUse = true;
                        }
                    }

                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.thiefplayer02IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { thiefplayer02TakeDeliverJewelButton.Timer = thiefplayer02TakeDeliverJewelButton.MaxTimer; },
            AssetLoader.PoliceAndThiefTakeJewelButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CommonAbilityPrimary,
            false,
            Tr.Get(TrKey.DeliverButton)
        );

        // Thiefplayer03 Kill
        thiefplayer03KillButton = new CustomButton(
            () =>
            {
                byte targetId = PoliceAndThief.thiefplayer03currentTarget.PlayerId;
                byte sourceId = PoliceAndThief.thiefplayer03.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                killWriter.Write(targetId);
                killWriter.Write(sourceId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.gamemodeKills(targetId, sourceId);
                thiefplayer03KillButton.Timer = thiefplayer03KillButton.MaxTimer;
                PoliceAndThief.thiefplayer03currentTarget = null;
            },
            () => { return PoliceAndThief.thiefplayer03 != null && PoliceAndThief.thiefplayer03 == PlayerControl.LocalPlayer && PoliceAndThief.whoCanThiefsKill != 2; },
            () =>
            {
                bool canUse = false;
                switch (PoliceAndThief.whoCanThiefsKill)
                {
                    case 0:
                        if (PoliceAndThief.policeplayer02 != null && PoliceAndThief.thiefplayer03currentTarget == PoliceAndThief.policeplayer02 || PoliceAndThief.policeplayer04 != null && PoliceAndThief.thiefplayer03currentTarget == PoliceAndThief.policeplayer04)
                        {
                            canUse = true;
                        }
                        break;
                    case 1:
                        canUse = true;
                        break;
                }
                return canUse && PoliceAndThief.thiefplayer03currentTarget && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.thiefplayer03IsReviving && !PlayerControl.LocalPlayer.Data.IsDead && !PoliceAndThief.thiefplayer03IsStealing;
            },
            () => { thiefplayer03KillButton.Timer = thiefplayer03KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // Thiefplayer03 FreeThief Button
        thiefplayer03FreeThiefButton = new CustomButton(
            () =>
            {
                MessageWriter thiefFree = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefFreeThief, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(thiefFree);
                RPCProcedure.policeandThiefFreeThief();
                thiefplayer03FreeThiefButton.Timer = thiefplayer03FreeThiefButton.MaxTimer;
            },
            () => { return PoliceAndThief.thiefplayer03 != null && PoliceAndThief.thiefplayer03 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (PoliceAndThief.localThiefReleaseArrow.Count != 0)
                {
                    PoliceAndThief.localThiefReleaseArrow[0].Update(PoliceAndThief.cellbutton.transform.position);
                    if (GameOptionsManager.Instance.currentGameOptions.MapId == 6)
                    {
                        PoliceAndThief.localThiefReleaseArrow[1].Update(PoliceAndThief.cellbuttontwo.transform.position);
                    }
                }
                if (PoliceAndThief.localThiefDeliverArrow.Count != 0)
                {
                    PoliceAndThief.localThiefDeliverArrow[0].Update(PoliceAndThief.jewelbutton.transform.position);
                    if (GameOptionsManager.Instance.currentGameOptions.MapId == 6)
                    {
                        PoliceAndThief.localThiefDeliverArrow[1].Update(PoliceAndThief.jewelbuttontwo.transform.position);
                    }
                }

                bool CanUse = false;
                if (PoliceAndThief.currentThiefsCaptured > 0)
                {
                    if ((PoliceAndThief.cellbuttontwo != null && Vector2.Distance(PoliceAndThief.thiefplayer03.transform.position, PoliceAndThief.cellbuttontwo.transform.position) < 0.4f || Vector2.Distance(PoliceAndThief.thiefplayer03.transform.position, PoliceAndThief.cellbutton.transform.position) < 0.4f) && !PoliceAndThief.thiefplayer03.Data.IsDead)
                    {
                        CanUse = true;
                    }
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.thiefplayer03IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { thiefplayer03FreeThiefButton.Timer = thiefplayer03FreeThiefButton.MaxTimer; },
            AssetLoader.PoliceAndThiefFreeButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CrewmateAbilitySecondary,
            false,
            Tr.Get(TrKey.ReleaseButton)
        );

        // Thiefplayer03 Take/Deliver Jewel Button
        thiefplayer03TakeDeliverJewelButton = new CustomButton(
            () =>
            {
                if (PoliceAndThief.thiefplayer03IsStealing)
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    byte jewelId = PoliceAndThief.thiefplayer03JewelId;
                    MessageWriter thiefScore = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefDeliverJewel, Hazel.SendOption.Reliable, -1);
                    thiefScore.Write(targetId);
                    thiefScore.Write(jewelId);
                    AmongUsClient.Instance.FinishRpcImmediately(thiefScore);
                    RPCProcedure.policeandThiefDeliverJewel(targetId, jewelId);
                }
                else
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    byte jewelId = PoliceAndThief.thiefplayer03JewelId;
                    MessageWriter thiefWhoTookATreasure = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefTakeJewel, Hazel.SendOption.Reliable, -1);
                    thiefWhoTookATreasure.Write(targetId);
                    thiefWhoTookATreasure.Write(jewelId);
                    AmongUsClient.Instance.FinishRpcImmediately(thiefWhoTookATreasure);
                    RPCProcedure.policeandThiefTakeJewel(targetId, jewelId);
                }
                thiefplayer03TakeDeliverJewelButton.Timer = thiefplayer03TakeDeliverJewelButton.MaxTimer;
            },
            () => { return PoliceAndThief.thiefplayer03 != null && PoliceAndThief.thiefplayer03 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (PoliceAndThief.thiefplayer03IsStealing)
                    thiefplayer03TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefDeliverJewelButton;
                else
                    thiefplayer03TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefTakeJewelButton;
                bool CanUse = false;
                if (PoliceAndThief.thiefTreasures.Count != 0)
                {
                    foreach (GameObject jewel in PoliceAndThief.thiefTreasures)
                    {
                        if (jewel != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, jewel.transform.position) < 0.5f && !PoliceAndThief.thiefplayer03IsStealing)
                        {
                            switch (jewel.name)
                            {
                                case "jewel01":
                                    PoliceAndThief.thiefplayer03JewelId = 1;
                                    CanUse = !PoliceAndThief.jewel01BeingStealed;
                                    break;
                                case "jewel02":
                                    PoliceAndThief.thiefplayer03JewelId = 2;
                                    CanUse = !PoliceAndThief.jewel02BeingStealed;
                                    break;
                                case "jewel03":
                                    PoliceAndThief.thiefplayer03JewelId = 3;
                                    CanUse = !PoliceAndThief.jewel03BeingStealed;
                                    break;
                                case "jewel04":
                                    PoliceAndThief.thiefplayer03JewelId = 4;
                                    CanUse = !PoliceAndThief.jewel04BeingStealed;
                                    break;
                                case "jewel05":
                                    PoliceAndThief.thiefplayer03JewelId = 5;
                                    CanUse = !PoliceAndThief.jewel05BeingStealed;
                                    break;
                                case "jewel06":
                                    PoliceAndThief.thiefplayer03JewelId = 6;
                                    CanUse = !PoliceAndThief.jewel06BeingStealed;
                                    break;
                                case "jewel07":
                                    PoliceAndThief.thiefplayer03JewelId = 7;
                                    CanUse = !PoliceAndThief.jewel07BeingStealed;
                                    break;
                                case "jewel08":
                                    PoliceAndThief.thiefplayer03JewelId = 8;
                                    CanUse = !PoliceAndThief.jewel08BeingStealed;
                                    break;
                                case "jewel09":
                                    PoliceAndThief.thiefplayer03JewelId = 9;
                                    CanUse = !PoliceAndThief.jewel09BeingStealed;
                                    break;
                                case "jewel10":
                                    PoliceAndThief.thiefplayer03JewelId = 10;
                                    CanUse = !PoliceAndThief.jewel10BeingStealed;
                                    break;
                                case "jewel11":
                                    PoliceAndThief.thiefplayer03JewelId = 11;
                                    CanUse = !PoliceAndThief.jewel11BeingStealed;
                                    break;
                                case "jewel12":
                                    PoliceAndThief.thiefplayer03JewelId = 12;
                                    CanUse = !PoliceAndThief.jewel12BeingStealed;
                                    break;
                                case "jewel13":
                                    PoliceAndThief.thiefplayer03JewelId = 13;
                                    CanUse = !PoliceAndThief.jewel13BeingStealed;
                                    break;
                                case "jewel14":
                                    PoliceAndThief.thiefplayer03JewelId = 14;
                                    CanUse = !PoliceAndThief.jewel14BeingStealed;
                                    break;
                                case "jewel15":
                                    PoliceAndThief.thiefplayer03JewelId = 15;
                                    CanUse = !PoliceAndThief.jewel15BeingStealed;
                                    break;
                            }
                        }
                        else if ((PoliceAndThief.jewelbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.jewelbuttontwo.transform.position) < 0.5f || PoliceAndThief.jewelbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.jewelbutton.transform.position) < 0.5f) && PoliceAndThief.thiefplayer03IsStealing)
                        {
                            CanUse = true;
                        }
                    }

                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.thiefplayer03IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { thiefplayer03TakeDeliverJewelButton.Timer = thiefplayer03TakeDeliverJewelButton.MaxTimer; },
            AssetLoader.PoliceAndThiefTakeJewelButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CommonAbilityPrimary,
            false,
            Tr.Get(TrKey.DeliverButton)
        );

        // Thiefplayer04 Kill
        thiefplayer04KillButton = new CustomButton(
            () =>
            {
                byte targetId = PoliceAndThief.thiefplayer04currentTarget.PlayerId;
                byte sourceId = PoliceAndThief.thiefplayer04.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                killWriter.Write(targetId);
                killWriter.Write(sourceId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.gamemodeKills(targetId, sourceId);
                thiefplayer04KillButton.Timer = thiefplayer04KillButton.MaxTimer;
                PoliceAndThief.thiefplayer04currentTarget = null;
            },
            () => { return PoliceAndThief.thiefplayer04 != null && PoliceAndThief.thiefplayer04 == PlayerControl.LocalPlayer && PoliceAndThief.whoCanThiefsKill != 2; },
            () =>
            {
                bool canUse = false;
                switch (PoliceAndThief.whoCanThiefsKill)
                {
                    case 0:
                        if (PoliceAndThief.policeplayer02 != null && PoliceAndThief.thiefplayer04currentTarget == PoliceAndThief.policeplayer02 || PoliceAndThief.policeplayer04 != null && PoliceAndThief.thiefplayer04currentTarget == PoliceAndThief.policeplayer04)
                        {
                            canUse = true;
                        }
                        break;
                    case 1:
                        canUse = true;
                        break;
                }
                return canUse && PoliceAndThief.thiefplayer04currentTarget && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.thiefplayer04IsReviving && !PlayerControl.LocalPlayer.Data.IsDead && !PoliceAndThief.thiefplayer04IsStealing;
            },
            () => { thiefplayer04KillButton.Timer = thiefplayer04KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // Thiefplayer04 FreeThief Button
        thiefplayer04FreeThiefButton = new CustomButton(
            () =>
            {
                MessageWriter thiefFree = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefFreeThief, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(thiefFree);
                RPCProcedure.policeandThiefFreeThief();
                thiefplayer04FreeThiefButton.Timer = thiefplayer04FreeThiefButton.MaxTimer;
            },
            () => { return PoliceAndThief.thiefplayer04 != null && PoliceAndThief.thiefplayer04 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (PoliceAndThief.localThiefReleaseArrow.Count != 0)
                {
                    PoliceAndThief.localThiefReleaseArrow[0].Update(PoliceAndThief.cellbutton.transform.position);
                    if (GameOptionsManager.Instance.currentGameOptions.MapId == 6)
                    {
                        PoliceAndThief.localThiefReleaseArrow[1].Update(PoliceAndThief.cellbuttontwo.transform.position);
                    }
                }
                if (PoliceAndThief.localThiefDeliverArrow.Count != 0)
                {
                    PoliceAndThief.localThiefDeliverArrow[0].Update(PoliceAndThief.jewelbutton.transform.position);
                    if (GameOptionsManager.Instance.currentGameOptions.MapId == 6)
                    {
                        PoliceAndThief.localThiefDeliverArrow[1].Update(PoliceAndThief.jewelbuttontwo.transform.position);
                    }
                }

                bool CanUse = false;
                if (PoliceAndThief.currentThiefsCaptured > 0)
                {
                    if ((PoliceAndThief.cellbuttontwo != null && Vector2.Distance(PoliceAndThief.thiefplayer04.transform.position, PoliceAndThief.cellbuttontwo.transform.position) < 0.4f || Vector2.Distance(PoliceAndThief.thiefplayer04.transform.position, PoliceAndThief.cellbutton.transform.position) < 0.4f) && !PoliceAndThief.thiefplayer04.Data.IsDead)
                    {
                        CanUse = true;
                    }
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.thiefplayer04IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { thiefplayer04FreeThiefButton.Timer = thiefplayer04FreeThiefButton.MaxTimer; },
            AssetLoader.PoliceAndThiefFreeButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CrewmateAbilitySecondary,
            false,
            Tr.Get(TrKey.ReleaseButton)
        );

        // Thiefplayer04 Take/Deliver Jewel Button
        thiefplayer04TakeDeliverJewelButton = new CustomButton(
            () =>
            {
                if (PoliceAndThief.thiefplayer04IsStealing)
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    byte jewelId = PoliceAndThief.thiefplayer04JewelId;
                    MessageWriter thiefScore = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefDeliverJewel, Hazel.SendOption.Reliable, -1);
                    thiefScore.Write(targetId);
                    thiefScore.Write(jewelId);
                    AmongUsClient.Instance.FinishRpcImmediately(thiefScore);
                    RPCProcedure.policeandThiefDeliverJewel(targetId, jewelId);
                }
                else
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    byte jewelId = PoliceAndThief.thiefplayer04JewelId;
                    MessageWriter thiefWhoTookATreasure = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefTakeJewel, Hazel.SendOption.Reliable, -1);
                    thiefWhoTookATreasure.Write(targetId);
                    thiefWhoTookATreasure.Write(jewelId);
                    AmongUsClient.Instance.FinishRpcImmediately(thiefWhoTookATreasure);
                    RPCProcedure.policeandThiefTakeJewel(targetId, jewelId);
                }
                thiefplayer04TakeDeliverJewelButton.Timer = thiefplayer04TakeDeliverJewelButton.MaxTimer;
            },
            () => { return PoliceAndThief.thiefplayer04 != null && PoliceAndThief.thiefplayer04 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (PoliceAndThief.thiefplayer04IsStealing)
                    thiefplayer04TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefDeliverJewelButton;
                else
                    thiefplayer04TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefTakeJewelButton;
                bool CanUse = false;
                if (PoliceAndThief.thiefTreasures.Count != 0)
                {
                    foreach (GameObject jewel in PoliceAndThief.thiefTreasures)
                    {
                        if (jewel != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, jewel.transform.position) < 0.5f && !PoliceAndThief.thiefplayer04IsStealing)
                        {
                            switch (jewel.name)
                            {
                                case "jewel01":
                                    PoliceAndThief.thiefplayer04JewelId = 1;
                                    CanUse = !PoliceAndThief.jewel01BeingStealed;
                                    break;
                                case "jewel02":
                                    PoliceAndThief.thiefplayer04JewelId = 2;
                                    CanUse = !PoliceAndThief.jewel02BeingStealed;
                                    break;
                                case "jewel03":
                                    PoliceAndThief.thiefplayer04JewelId = 3;
                                    CanUse = !PoliceAndThief.jewel03BeingStealed;
                                    break;
                                case "jewel04":
                                    PoliceAndThief.thiefplayer04JewelId = 4;
                                    CanUse = !PoliceAndThief.jewel04BeingStealed;
                                    break;
                                case "jewel05":
                                    PoliceAndThief.thiefplayer04JewelId = 5;
                                    CanUse = !PoliceAndThief.jewel05BeingStealed;
                                    break;
                                case "jewel06":
                                    PoliceAndThief.thiefplayer04JewelId = 6;
                                    CanUse = !PoliceAndThief.jewel06BeingStealed;
                                    break;
                                case "jewel07":
                                    PoliceAndThief.thiefplayer04JewelId = 7;
                                    CanUse = !PoliceAndThief.jewel07BeingStealed;
                                    break;
                                case "jewel08":
                                    PoliceAndThief.thiefplayer04JewelId = 8;
                                    CanUse = !PoliceAndThief.jewel08BeingStealed;
                                    break;
                                case "jewel09":
                                    PoliceAndThief.thiefplayer04JewelId = 9;
                                    CanUse = !PoliceAndThief.jewel09BeingStealed;
                                    break;
                                case "jewel10":
                                    PoliceAndThief.thiefplayer04JewelId = 10;
                                    CanUse = !PoliceAndThief.jewel10BeingStealed;
                                    break;
                                case "jewel11":
                                    PoliceAndThief.thiefplayer04JewelId = 11;
                                    CanUse = !PoliceAndThief.jewel11BeingStealed;
                                    break;
                                case "jewel12":
                                    PoliceAndThief.thiefplayer04JewelId = 12;
                                    CanUse = !PoliceAndThief.jewel12BeingStealed;
                                    break;
                                case "jewel13":
                                    PoliceAndThief.thiefplayer04JewelId = 13;
                                    CanUse = !PoliceAndThief.jewel13BeingStealed;
                                    break;
                                case "jewel14":
                                    PoliceAndThief.thiefplayer04JewelId = 14;
                                    CanUse = !PoliceAndThief.jewel14BeingStealed;
                                    break;
                                case "jewel15":
                                    PoliceAndThief.thiefplayer04JewelId = 15;
                                    CanUse = !PoliceAndThief.jewel15BeingStealed;
                                    break;
                            }
                        }
                        else if ((PoliceAndThief.jewelbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.jewelbuttontwo.transform.position) < 0.5f || PoliceAndThief.jewelbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.jewelbutton.transform.position) < 0.5f) && PoliceAndThief.thiefplayer04IsStealing)
                        {
                            CanUse = true;
                        }
                    }

                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.thiefplayer04IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { thiefplayer04TakeDeliverJewelButton.Timer = thiefplayer04TakeDeliverJewelButton.MaxTimer; },
            AssetLoader.PoliceAndThiefTakeJewelButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CommonAbilityPrimary,
            false,
            Tr.Get(TrKey.DeliverButton)
        );

        // Thiefplayer05 Kill
        thiefplayer05KillButton = new CustomButton(
            () =>
            {
                byte targetId = PoliceAndThief.thiefplayer05currentTarget.PlayerId;
                byte sourceId = PoliceAndThief.thiefplayer05.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                killWriter.Write(targetId);
                killWriter.Write(sourceId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.gamemodeKills(targetId, sourceId);
                thiefplayer05KillButton.Timer = thiefplayer05KillButton.MaxTimer;
                PoliceAndThief.thiefplayer05currentTarget = null;
            },
            () => { return PoliceAndThief.thiefplayer05 != null && PoliceAndThief.thiefplayer05 == PlayerControl.LocalPlayer && PoliceAndThief.whoCanThiefsKill != 2; },
            () =>
            {
                bool canUse = false;
                switch (PoliceAndThief.whoCanThiefsKill)
                {
                    case 0:
                        if (PoliceAndThief.policeplayer02 != null && PoliceAndThief.thiefplayer05currentTarget == PoliceAndThief.policeplayer02 || PoliceAndThief.policeplayer04 != null && PoliceAndThief.thiefplayer05currentTarget == PoliceAndThief.policeplayer04)
                        {
                            canUse = true;
                        }
                        break;
                    case 1:
                        canUse = true;
                        break;
                }
                return canUse && PoliceAndThief.thiefplayer05currentTarget && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.thiefplayer05IsReviving && !PlayerControl.LocalPlayer.Data.IsDead && !PoliceAndThief.thiefplayer05IsStealing;
            },
            () => { thiefplayer05KillButton.Timer = thiefplayer05KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // Thiefplayer05 FreeThief Button
        thiefplayer05FreeThiefButton = new CustomButton(
            () =>
            {
                MessageWriter thiefFree = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefFreeThief, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(thiefFree);
                RPCProcedure.policeandThiefFreeThief();
                thiefplayer05FreeThiefButton.Timer = thiefplayer05FreeThiefButton.MaxTimer;
            },
            () => { return PoliceAndThief.thiefplayer05 != null && PoliceAndThief.thiefplayer05 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (PoliceAndThief.localThiefReleaseArrow.Count != 0)
                {
                    PoliceAndThief.localThiefReleaseArrow[0].Update(PoliceAndThief.cellbutton.transform.position);
                    if (GameOptionsManager.Instance.currentGameOptions.MapId == 6)
                    {
                        PoliceAndThief.localThiefReleaseArrow[1].Update(PoliceAndThief.cellbuttontwo.transform.position);
                    }
                }
                if (PoliceAndThief.localThiefDeliverArrow.Count != 0)
                {
                    PoliceAndThief.localThiefDeliverArrow[0].Update(PoliceAndThief.jewelbutton.transform.position);
                    if (GameOptionsManager.Instance.currentGameOptions.MapId == 6)
                    {
                        PoliceAndThief.localThiefDeliverArrow[1].Update(PoliceAndThief.jewelbuttontwo.transform.position);
                    }
                }

                bool CanUse = false;
                if (PoliceAndThief.currentThiefsCaptured > 0)
                {
                    if ((PoliceAndThief.cellbuttontwo != null && Vector2.Distance(PoliceAndThief.thiefplayer05.transform.position, PoliceAndThief.cellbuttontwo.transform.position) < 0.4f || Vector2.Distance(PoliceAndThief.thiefplayer05.transform.position, PoliceAndThief.cellbutton.transform.position) < 0.4f) && !PoliceAndThief.thiefplayer05.Data.IsDead)
                    {
                        CanUse = true;
                    }
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.thiefplayer05IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { thiefplayer05FreeThiefButton.Timer = thiefplayer05FreeThiefButton.MaxTimer; },
            AssetLoader.PoliceAndThiefFreeButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CrewmateAbilitySecondary,
            false,
            Tr.Get(TrKey.ReleaseButton)
        );

        // Thiefplayer05 Take/Deliver Jewel Button
        thiefplayer05TakeDeliverJewelButton = new CustomButton(
            () =>
            {
                if (PoliceAndThief.thiefplayer05IsStealing)
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    byte jewelId = PoliceAndThief.thiefplayer05JewelId;
                    MessageWriter thiefScore = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefDeliverJewel, Hazel.SendOption.Reliable, -1);
                    thiefScore.Write(targetId);
                    thiefScore.Write(jewelId);
                    AmongUsClient.Instance.FinishRpcImmediately(thiefScore);
                    RPCProcedure.policeandThiefDeliverJewel(targetId, jewelId);
                }
                else
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    byte jewelId = PoliceAndThief.thiefplayer05JewelId;
                    MessageWriter thiefWhoTookATreasure = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefTakeJewel, Hazel.SendOption.Reliable, -1);
                    thiefWhoTookATreasure.Write(targetId);
                    thiefWhoTookATreasure.Write(jewelId);
                    AmongUsClient.Instance.FinishRpcImmediately(thiefWhoTookATreasure);
                    RPCProcedure.policeandThiefTakeJewel(targetId, jewelId);
                }
                thiefplayer05TakeDeliverJewelButton.Timer = thiefplayer05TakeDeliverJewelButton.MaxTimer;
            },
            () => { return PoliceAndThief.thiefplayer05 != null && PoliceAndThief.thiefplayer05 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (PoliceAndThief.thiefplayer05IsStealing)
                    thiefplayer05TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefDeliverJewelButton;
                else
                    thiefplayer05TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefTakeJewelButton;
                bool CanUse = false;
                if (PoliceAndThief.thiefTreasures.Count != 0)
                {
                    foreach (GameObject jewel in PoliceAndThief.thiefTreasures)
                    {
                        if (jewel != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, jewel.transform.position) < 0.5f && !PoliceAndThief.thiefplayer05IsStealing)
                        {
                            switch (jewel.name)
                            {
                                case "jewel01":
                                    PoliceAndThief.thiefplayer05JewelId = 1;
                                    CanUse = !PoliceAndThief.jewel01BeingStealed;
                                    break;
                                case "jewel02":
                                    PoliceAndThief.thiefplayer05JewelId = 2;
                                    CanUse = !PoliceAndThief.jewel02BeingStealed;
                                    break;
                                case "jewel03":
                                    PoliceAndThief.thiefplayer05JewelId = 3;
                                    CanUse = !PoliceAndThief.jewel03BeingStealed;
                                    break;
                                case "jewel04":
                                    PoliceAndThief.thiefplayer05JewelId = 4;
                                    CanUse = !PoliceAndThief.jewel04BeingStealed;
                                    break;
                                case "jewel05":
                                    PoliceAndThief.thiefplayer05JewelId = 5;
                                    CanUse = !PoliceAndThief.jewel05BeingStealed;
                                    break;
                                case "jewel06":
                                    PoliceAndThief.thiefplayer05JewelId = 6;
                                    CanUse = !PoliceAndThief.jewel06BeingStealed;
                                    break;
                                case "jewel07":
                                    PoliceAndThief.thiefplayer05JewelId = 7;
                                    CanUse = !PoliceAndThief.jewel07BeingStealed;
                                    break;
                                case "jewel08":
                                    PoliceAndThief.thiefplayer05JewelId = 8;
                                    CanUse = !PoliceAndThief.jewel08BeingStealed;
                                    break;
                                case "jewel09":
                                    PoliceAndThief.thiefplayer05JewelId = 9;
                                    CanUse = !PoliceAndThief.jewel09BeingStealed;
                                    break;
                                case "jewel10":
                                    PoliceAndThief.thiefplayer05JewelId = 10;
                                    CanUse = !PoliceAndThief.jewel10BeingStealed;
                                    break;
                                case "jewel11":
                                    PoliceAndThief.thiefplayer05JewelId = 11;
                                    CanUse = !PoliceAndThief.jewel11BeingStealed;
                                    break;
                                case "jewel12":
                                    PoliceAndThief.thiefplayer05JewelId = 12;
                                    CanUse = !PoliceAndThief.jewel12BeingStealed;
                                    break;
                                case "jewel13":
                                    PoliceAndThief.thiefplayer05JewelId = 13;
                                    CanUse = !PoliceAndThief.jewel13BeingStealed;
                                    break;
                                case "jewel14":
                                    PoliceAndThief.thiefplayer05JewelId = 14;
                                    CanUse = !PoliceAndThief.jewel14BeingStealed;
                                    break;
                                case "jewel15":
                                    PoliceAndThief.thiefplayer05JewelId = 15;
                                    CanUse = !PoliceAndThief.jewel15BeingStealed;
                                    break;
                            }
                        }
                        else if ((PoliceAndThief.jewelbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.jewelbuttontwo.transform.position) < 0.5f || PoliceAndThief.jewelbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.jewelbutton.transform.position) < 0.5f) && PoliceAndThief.thiefplayer05IsStealing)
                        {
                            CanUse = true;
                        }
                    }

                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.thiefplayer05IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { thiefplayer05TakeDeliverJewelButton.Timer = thiefplayer05TakeDeliverJewelButton.MaxTimer; },
            AssetLoader.PoliceAndThiefTakeJewelButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CommonAbilityPrimary,
            false,
            Tr.Get(TrKey.DeliverButton)
        );

        // Thiefplayer06 Kill
        thiefplayer06KillButton = new CustomButton(
            () =>
            {
                byte targetId = PoliceAndThief.thiefplayer06currentTarget.PlayerId;
                byte sourceId = PoliceAndThief.thiefplayer06.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                killWriter.Write(targetId);
                killWriter.Write(sourceId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.gamemodeKills(targetId, sourceId);
                thiefplayer06KillButton.Timer = thiefplayer06KillButton.MaxTimer;
                PoliceAndThief.thiefplayer06currentTarget = null;
            },
            () => { return PoliceAndThief.thiefplayer06 != null && PoliceAndThief.thiefplayer06 == PlayerControl.LocalPlayer && PoliceAndThief.whoCanThiefsKill != 2; },
            () =>
            {
                bool canUse = false;
                switch (PoliceAndThief.whoCanThiefsKill)
                {
                    case 0:
                        if (PoliceAndThief.policeplayer02 != null && PoliceAndThief.thiefplayer06currentTarget == PoliceAndThief.policeplayer02 || PoliceAndThief.policeplayer04 != null && PoliceAndThief.thiefplayer06currentTarget == PoliceAndThief.policeplayer04)
                        {
                            canUse = true;
                        }
                        break;
                    case 1:
                        canUse = true;
                        break;
                }
                return canUse && PoliceAndThief.thiefplayer06currentTarget && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.thiefplayer06IsReviving && !PlayerControl.LocalPlayer.Data.IsDead && !PoliceAndThief.thiefplayer06IsStealing;
            },
            () => { thiefplayer06KillButton.Timer = thiefplayer06KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // Thiefplayer06 FreeThief Button
        thiefplayer06FreeThiefButton = new CustomButton(
            () =>
            {
                MessageWriter thiefFree = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefFreeThief, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(thiefFree);
                RPCProcedure.policeandThiefFreeThief();
                thiefplayer06FreeThiefButton.Timer = thiefplayer06FreeThiefButton.MaxTimer;
            },
            () => { return PoliceAndThief.thiefplayer06 != null && PoliceAndThief.thiefplayer06 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (PoliceAndThief.localThiefReleaseArrow.Count != 0)
                {
                    PoliceAndThief.localThiefReleaseArrow[0].Update(PoliceAndThief.cellbutton.transform.position);
                    if (GameOptionsManager.Instance.currentGameOptions.MapId == 6)
                    {
                        PoliceAndThief.localThiefReleaseArrow[1].Update(PoliceAndThief.cellbuttontwo.transform.position);
                    }
                }
                if (PoliceAndThief.localThiefDeliverArrow.Count != 0)
                {
                    PoliceAndThief.localThiefDeliverArrow[0].Update(PoliceAndThief.jewelbutton.transform.position);
                    if (GameOptionsManager.Instance.currentGameOptions.MapId == 6)
                    {
                        PoliceAndThief.localThiefDeliverArrow[1].Update(PoliceAndThief.jewelbuttontwo.transform.position);
                    }
                }

                bool CanUse = false;
                if (PoliceAndThief.currentThiefsCaptured > 0)
                {
                    if ((PoliceAndThief.cellbuttontwo != null && Vector2.Distance(PoliceAndThief.thiefplayer06.transform.position, PoliceAndThief.cellbuttontwo.transform.position) < 0.4f || Vector2.Distance(PoliceAndThief.thiefplayer06.transform.position, PoliceAndThief.cellbutton.transform.position) < 0.4f) && !PoliceAndThief.thiefplayer06.Data.IsDead)
                    {
                        CanUse = true;
                    }
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.thiefplayer06IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { thiefplayer06FreeThiefButton.Timer = thiefplayer06FreeThiefButton.MaxTimer; },
            AssetLoader.PoliceAndThiefFreeButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CrewmateAbilitySecondary,
            false,
            Tr.Get(TrKey.ReleaseButton)
        );

        // Thiefplayer06 Take/Deliver Jewel Button
        thiefplayer06TakeDeliverJewelButton = new CustomButton(
            () =>
            {
                if (PoliceAndThief.thiefplayer06IsStealing)
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    byte jewelId = PoliceAndThief.thiefplayer06JewelId;
                    MessageWriter thiefScore = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefDeliverJewel, Hazel.SendOption.Reliable, -1);
                    thiefScore.Write(targetId);
                    thiefScore.Write(jewelId);
                    AmongUsClient.Instance.FinishRpcImmediately(thiefScore);
                    RPCProcedure.policeandThiefDeliverJewel(targetId, jewelId);
                }
                else
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    byte jewelId = PoliceAndThief.thiefplayer06JewelId;
                    MessageWriter thiefWhoTookATreasure = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefTakeJewel, Hazel.SendOption.Reliable, -1);
                    thiefWhoTookATreasure.Write(targetId);
                    thiefWhoTookATreasure.Write(jewelId);
                    AmongUsClient.Instance.FinishRpcImmediately(thiefWhoTookATreasure);
                    RPCProcedure.policeandThiefTakeJewel(targetId, jewelId);
                }
                thiefplayer06TakeDeliverJewelButton.Timer = thiefplayer06TakeDeliverJewelButton.MaxTimer;
            },
            () => { return PoliceAndThief.thiefplayer06 != null && PoliceAndThief.thiefplayer06 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (PoliceAndThief.thiefplayer06IsStealing)
                    thiefplayer06TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefDeliverJewelButton;
                else
                    thiefplayer06TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefTakeJewelButton;
                bool CanUse = false;
                if (PoliceAndThief.thiefTreasures.Count != 0)
                {
                    foreach (GameObject jewel in PoliceAndThief.thiefTreasures)
                    {
                        if (jewel != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, jewel.transform.position) < 0.5f && !PoliceAndThief.thiefplayer06IsStealing)
                        {
                            switch (jewel.name)
                            {
                                case "jewel01":
                                    PoliceAndThief.thiefplayer06JewelId = 1;
                                    CanUse = !PoliceAndThief.jewel01BeingStealed;
                                    break;
                                case "jewel02":
                                    PoliceAndThief.thiefplayer06JewelId = 2;
                                    CanUse = !PoliceAndThief.jewel02BeingStealed;
                                    break;
                                case "jewel03":
                                    PoliceAndThief.thiefplayer06JewelId = 3;
                                    CanUse = !PoliceAndThief.jewel03BeingStealed;
                                    break;
                                case "jewel04":
                                    PoliceAndThief.thiefplayer06JewelId = 4;
                                    CanUse = !PoliceAndThief.jewel04BeingStealed;
                                    break;
                                case "jewel05":
                                    PoliceAndThief.thiefplayer06JewelId = 5;
                                    CanUse = !PoliceAndThief.jewel05BeingStealed;
                                    break;
                                case "jewel06":
                                    PoliceAndThief.thiefplayer06JewelId = 6;
                                    CanUse = !PoliceAndThief.jewel06BeingStealed;
                                    break;
                                case "jewel07":
                                    PoliceAndThief.thiefplayer06JewelId = 7;
                                    CanUse = !PoliceAndThief.jewel07BeingStealed;
                                    break;
                                case "jewel08":
                                    PoliceAndThief.thiefplayer06JewelId = 8;
                                    CanUse = !PoliceAndThief.jewel08BeingStealed;
                                    break;
                                case "jewel09":
                                    PoliceAndThief.thiefplayer06JewelId = 9;
                                    CanUse = !PoliceAndThief.jewel09BeingStealed;
                                    break;
                                case "jewel10":
                                    PoliceAndThief.thiefplayer06JewelId = 10;
                                    CanUse = !PoliceAndThief.jewel10BeingStealed;
                                    break;
                                case "jewel11":
                                    PoliceAndThief.thiefplayer06JewelId = 11;
                                    CanUse = !PoliceAndThief.jewel11BeingStealed;
                                    break;
                                case "jewel12":
                                    PoliceAndThief.thiefplayer06JewelId = 12;
                                    CanUse = !PoliceAndThief.jewel12BeingStealed;
                                    break;
                                case "jewel13":
                                    PoliceAndThief.thiefplayer06JewelId = 13;
                                    CanUse = !PoliceAndThief.jewel13BeingStealed;
                                    break;
                                case "jewel14":
                                    PoliceAndThief.thiefplayer06JewelId = 14;
                                    CanUse = !PoliceAndThief.jewel14BeingStealed;
                                    break;
                                case "jewel15":
                                    PoliceAndThief.thiefplayer06JewelId = 15;
                                    CanUse = !PoliceAndThief.jewel15BeingStealed;
                                    break;
                            }
                        }
                        else if ((PoliceAndThief.jewelbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.jewelbuttontwo.transform.position) < 0.5f || PoliceAndThief.jewelbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.jewelbutton.transform.position) < 0.5f) && PoliceAndThief.thiefplayer06IsStealing)
                        {
                            CanUse = true;
                        }
                    }

                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.thiefplayer06IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { thiefplayer06TakeDeliverJewelButton.Timer = thiefplayer06TakeDeliverJewelButton.MaxTimer; },
            AssetLoader.PoliceAndThiefTakeJewelButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CommonAbilityPrimary,
            false,
            Tr.Get(TrKey.DeliverButton)
        );

        // Thiefplayer07 Kill
        thiefplayer07KillButton = new CustomButton(
            () =>
            {
                byte targetId = PoliceAndThief.thiefplayer07currentTarget.PlayerId;
                byte sourceId = PoliceAndThief.thiefplayer07.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                killWriter.Write(targetId);
                killWriter.Write(sourceId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.gamemodeKills(targetId, sourceId);
                thiefplayer07KillButton.Timer = thiefplayer07KillButton.MaxTimer;
                PoliceAndThief.thiefplayer07currentTarget = null;
            },
            () => { return PoliceAndThief.thiefplayer07 != null && PoliceAndThief.thiefplayer07 == PlayerControl.LocalPlayer && PoliceAndThief.whoCanThiefsKill != 2; },
            () =>
            {
                bool canUse = false;
                switch (PoliceAndThief.whoCanThiefsKill)
                {
                    case 0:
                        if (PoliceAndThief.policeplayer02 != null && PoliceAndThief.thiefplayer07currentTarget == PoliceAndThief.policeplayer02 || PoliceAndThief.policeplayer04 != null && PoliceAndThief.thiefplayer07currentTarget == PoliceAndThief.policeplayer04)
                        {
                            canUse = true;
                        }
                        break;
                    case 1:
                        canUse = true;
                        break;
                }
                return canUse && PoliceAndThief.thiefplayer07currentTarget && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.thiefplayer07IsReviving && !PlayerControl.LocalPlayer.Data.IsDead && !PoliceAndThief.thiefplayer07IsStealing;
            },
            () => { thiefplayer07KillButton.Timer = thiefplayer07KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // Thiefplayer07 FreeThief Button
        thiefplayer07FreeThiefButton = new CustomButton(
            () =>
            {
                MessageWriter thiefFree = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefFreeThief, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(thiefFree);
                RPCProcedure.policeandThiefFreeThief();
                thiefplayer07FreeThiefButton.Timer = thiefplayer07FreeThiefButton.MaxTimer;
            },
            () => { return PoliceAndThief.thiefplayer07 != null && PoliceAndThief.thiefplayer07 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (PoliceAndThief.localThiefReleaseArrow.Count != 0)
                {
                    PoliceAndThief.localThiefReleaseArrow[0].Update(PoliceAndThief.cellbutton.transform.position);
                    if (GameOptionsManager.Instance.currentGameOptions.MapId == 6)
                    {
                        PoliceAndThief.localThiefReleaseArrow[1].Update(PoliceAndThief.cellbuttontwo.transform.position);
                    }
                }
                if (PoliceAndThief.localThiefDeliverArrow.Count != 0)
                {
                    PoliceAndThief.localThiefDeliverArrow[0].Update(PoliceAndThief.jewelbutton.transform.position);
                    if (GameOptionsManager.Instance.currentGameOptions.MapId == 6)
                    {
                        PoliceAndThief.localThiefDeliverArrow[1].Update(PoliceAndThief.jewelbuttontwo.transform.position);
                    }
                }

                bool CanUse = false;
                if (PoliceAndThief.currentThiefsCaptured > 0)
                {
                    if ((PoliceAndThief.cellbuttontwo != null && Vector2.Distance(PoliceAndThief.thiefplayer07.transform.position, PoliceAndThief.cellbuttontwo.transform.position) < 0.4f || Vector2.Distance(PoliceAndThief.thiefplayer07.transform.position, PoliceAndThief.cellbutton.transform.position) < 0.4f) && !PoliceAndThief.thiefplayer07.Data.IsDead)
                    {
                        CanUse = true;
                    }
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.thiefplayer07IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { thiefplayer07FreeThiefButton.Timer = thiefplayer07FreeThiefButton.MaxTimer; },
            AssetLoader.PoliceAndThiefFreeButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CrewmateAbilitySecondary,
            false,
            Tr.Get(TrKey.ReleaseButton)
        );

        // Thiefplayer07 Take/Deliver Jewel Button
        thiefplayer07TakeDeliverJewelButton = new CustomButton(
            () =>
            {
                if (PoliceAndThief.thiefplayer07IsStealing)
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    byte jewelId = PoliceAndThief.thiefplayer07JewelId;
                    MessageWriter thiefScore = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefDeliverJewel, Hazel.SendOption.Reliable, -1);
                    thiefScore.Write(targetId);
                    thiefScore.Write(jewelId);
                    AmongUsClient.Instance.FinishRpcImmediately(thiefScore);
                    RPCProcedure.policeandThiefDeliverJewel(targetId, jewelId);
                }
                else
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    byte jewelId = PoliceAndThief.thiefplayer07JewelId;
                    MessageWriter thiefWhoTookATreasure = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefTakeJewel, Hazel.SendOption.Reliable, -1);
                    thiefWhoTookATreasure.Write(targetId);
                    thiefWhoTookATreasure.Write(jewelId);
                    AmongUsClient.Instance.FinishRpcImmediately(thiefWhoTookATreasure);
                    RPCProcedure.policeandThiefTakeJewel(targetId, jewelId);
                }
                thiefplayer07TakeDeliverJewelButton.Timer = thiefplayer07TakeDeliverJewelButton.MaxTimer;
            },
            () => { return PoliceAndThief.thiefplayer07 != null && PoliceAndThief.thiefplayer07 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (PoliceAndThief.thiefplayer07IsStealing)
                    thiefplayer07TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefDeliverJewelButton;
                else
                    thiefplayer07TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefTakeJewelButton;
                bool CanUse = false;
                if (PoliceAndThief.thiefTreasures.Count != 0)
                {
                    foreach (GameObject jewel in PoliceAndThief.thiefTreasures)
                    {
                        if (jewel != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, jewel.transform.position) < 0.5f && !PoliceAndThief.thiefplayer07IsStealing)
                        {
                            switch (jewel.name)
                            {
                                case "jewel01":
                                    PoliceAndThief.thiefplayer07JewelId = 1;
                                    CanUse = !PoliceAndThief.jewel01BeingStealed;
                                    break;
                                case "jewel02":
                                    PoliceAndThief.thiefplayer07JewelId = 2;
                                    CanUse = !PoliceAndThief.jewel02BeingStealed;
                                    break;
                                case "jewel03":
                                    PoliceAndThief.thiefplayer07JewelId = 3;
                                    CanUse = !PoliceAndThief.jewel03BeingStealed;
                                    break;
                                case "jewel04":
                                    PoliceAndThief.thiefplayer07JewelId = 4;
                                    CanUse = !PoliceAndThief.jewel04BeingStealed;
                                    break;
                                case "jewel05":
                                    PoliceAndThief.thiefplayer07JewelId = 5;
                                    CanUse = !PoliceAndThief.jewel05BeingStealed;
                                    break;
                                case "jewel06":
                                    PoliceAndThief.thiefplayer07JewelId = 6;
                                    CanUse = !PoliceAndThief.jewel06BeingStealed;
                                    break;
                                case "jewel07":
                                    PoliceAndThief.thiefplayer07JewelId = 7;
                                    CanUse = !PoliceAndThief.jewel07BeingStealed;
                                    break;
                                case "jewel08":
                                    PoliceAndThief.thiefplayer07JewelId = 8;
                                    CanUse = !PoliceAndThief.jewel08BeingStealed;
                                    break;
                                case "jewel09":
                                    PoliceAndThief.thiefplayer07JewelId = 9;
                                    CanUse = !PoliceAndThief.jewel09BeingStealed;
                                    break;
                                case "jewel10":
                                    PoliceAndThief.thiefplayer07JewelId = 10;
                                    CanUse = !PoliceAndThief.jewel10BeingStealed;
                                    break;
                                case "jewel11":
                                    PoliceAndThief.thiefplayer07JewelId = 11;
                                    CanUse = !PoliceAndThief.jewel11BeingStealed;
                                    break;
                                case "jewel12":
                                    PoliceAndThief.thiefplayer07JewelId = 12;
                                    CanUse = !PoliceAndThief.jewel12BeingStealed;
                                    break;
                                case "jewel13":
                                    PoliceAndThief.thiefplayer07JewelId = 13;
                                    CanUse = !PoliceAndThief.jewel13BeingStealed;
                                    break;
                                case "jewel14":
                                    PoliceAndThief.thiefplayer07JewelId = 14;
                                    CanUse = !PoliceAndThief.jewel14BeingStealed;
                                    break;
                                case "jewel15":
                                    PoliceAndThief.thiefplayer07JewelId = 15;
                                    CanUse = !PoliceAndThief.jewel15BeingStealed;
                                    break;
                            }
                        }
                        else if ((PoliceAndThief.jewelbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.jewelbuttontwo.transform.position) < 0.5f || PoliceAndThief.jewelbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.jewelbutton.transform.position) < 0.5f) && PoliceAndThief.thiefplayer07IsStealing)
                        {
                            CanUse = true;
                        }
                    }

                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.thiefplayer07IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { thiefplayer07TakeDeliverJewelButton.Timer = thiefplayer07TakeDeliverJewelButton.MaxTimer; },
            AssetLoader.PoliceAndThiefTakeJewelButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CommonAbilityPrimary,
            false,
            Tr.Get(TrKey.DeliverButton)
        );

        // Thiefplayer08 Kill
        thiefplayer08KillButton = new CustomButton(
            () =>
            {
                byte targetId = PoliceAndThief.thiefplayer08currentTarget.PlayerId;
                byte sourceId = PoliceAndThief.thiefplayer08.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                killWriter.Write(targetId);
                killWriter.Write(sourceId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.gamemodeKills(targetId, sourceId);
                thiefplayer08KillButton.Timer = thiefplayer08KillButton.MaxTimer;
                PoliceAndThief.thiefplayer08currentTarget = null;
            },
            () => { return PoliceAndThief.thiefplayer08 != null && PoliceAndThief.thiefplayer08 == PlayerControl.LocalPlayer && PoliceAndThief.whoCanThiefsKill != 2; },
            () =>
            {
                bool canUse = false;
                switch (PoliceAndThief.whoCanThiefsKill)
                {
                    case 0:
                        if (PoliceAndThief.policeplayer02 != null && PoliceAndThief.thiefplayer08currentTarget == PoliceAndThief.policeplayer02 || PoliceAndThief.policeplayer04 != null && PoliceAndThief.thiefplayer08currentTarget == PoliceAndThief.policeplayer04)
                        {
                            canUse = true;
                        }
                        break;
                    case 1:
                        canUse = true;
                        break;
                }
                return canUse && PoliceAndThief.thiefplayer08currentTarget && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.thiefplayer08IsReviving && !PlayerControl.LocalPlayer.Data.IsDead && !PoliceAndThief.thiefplayer08IsStealing;
            },
            () => { thiefplayer08KillButton.Timer = thiefplayer08KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // Thiefplayer08 FreeThief Button
        thiefplayer08FreeThiefButton = new CustomButton(
            () =>
            {
                MessageWriter thiefFree = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefFreeThief, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(thiefFree);
                RPCProcedure.policeandThiefFreeThief();
                thiefplayer08FreeThiefButton.Timer = thiefplayer08FreeThiefButton.MaxTimer;
            },
            () => { return PoliceAndThief.thiefplayer08 != null && PoliceAndThief.thiefplayer08 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (PoliceAndThief.localThiefReleaseArrow.Count != 0)
                {
                    PoliceAndThief.localThiefReleaseArrow[0].Update(PoliceAndThief.cellbutton.transform.position);
                    if (GameOptionsManager.Instance.currentGameOptions.MapId == 6)
                    {
                        PoliceAndThief.localThiefReleaseArrow[1].Update(PoliceAndThief.cellbuttontwo.transform.position);
                    }
                }
                if (PoliceAndThief.localThiefDeliverArrow.Count != 0)
                {
                    PoliceAndThief.localThiefDeliverArrow[0].Update(PoliceAndThief.jewelbutton.transform.position);
                    if (GameOptionsManager.Instance.currentGameOptions.MapId == 6)
                    {
                        PoliceAndThief.localThiefDeliverArrow[1].Update(PoliceAndThief.jewelbuttontwo.transform.position);
                    }
                }

                bool CanUse = false;
                if (PoliceAndThief.currentThiefsCaptured > 0)
                {
                    if ((PoliceAndThief.cellbuttontwo != null && Vector2.Distance(PoliceAndThief.thiefplayer08.transform.position, PoliceAndThief.cellbuttontwo.transform.position) < 0.4f || Vector2.Distance(PoliceAndThief.thiefplayer08.transform.position, PoliceAndThief.cellbutton.transform.position) < 0.4f) && !PoliceAndThief.thiefplayer08.Data.IsDead)
                    {
                        CanUse = true;
                    }
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.thiefplayer08IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { thiefplayer08FreeThiefButton.Timer = thiefplayer08FreeThiefButton.MaxTimer; },
            AssetLoader.PoliceAndThiefFreeButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CrewmateAbilitySecondary,
            false,
            Tr.Get(TrKey.ReleaseButton)
        );

        // Thiefplayer08 Take/Deliver Jewel Button
        thiefplayer08TakeDeliverJewelButton = new CustomButton(
            () =>
            {
                if (PoliceAndThief.thiefplayer08IsStealing)
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    byte jewelId = PoliceAndThief.thiefplayer08JewelId;
                    MessageWriter thiefScore = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefDeliverJewel, Hazel.SendOption.Reliable, -1);
                    thiefScore.Write(targetId);
                    thiefScore.Write(jewelId);
                    AmongUsClient.Instance.FinishRpcImmediately(thiefScore);
                    RPCProcedure.policeandThiefDeliverJewel(targetId, jewelId);
                }
                else
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    byte jewelId = PoliceAndThief.thiefplayer08JewelId;
                    MessageWriter thiefWhoTookATreasure = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefTakeJewel, Hazel.SendOption.Reliable, -1);
                    thiefWhoTookATreasure.Write(targetId);
                    thiefWhoTookATreasure.Write(jewelId);
                    AmongUsClient.Instance.FinishRpcImmediately(thiefWhoTookATreasure);
                    RPCProcedure.policeandThiefTakeJewel(targetId, jewelId);
                }
                thiefplayer08TakeDeliverJewelButton.Timer = thiefplayer08TakeDeliverJewelButton.MaxTimer;
            },
            () => { return PoliceAndThief.thiefplayer08 != null && PoliceAndThief.thiefplayer08 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (PoliceAndThief.thiefplayer08IsStealing)
                    thiefplayer08TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefDeliverJewelButton;
                else
                    thiefplayer08TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefTakeJewelButton;
                bool CanUse = false;
                if (PoliceAndThief.thiefTreasures.Count != 0)
                {
                    foreach (GameObject jewel in PoliceAndThief.thiefTreasures)
                    {
                        if (jewel != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, jewel.transform.position) < 0.5f && !PoliceAndThief.thiefplayer08IsStealing)
                        {
                            switch (jewel.name)
                            {
                                case "jewel01":
                                    PoliceAndThief.thiefplayer08JewelId = 1;
                                    CanUse = !PoliceAndThief.jewel01BeingStealed;
                                    break;
                                case "jewel02":
                                    PoliceAndThief.thiefplayer08JewelId = 2;
                                    CanUse = !PoliceAndThief.jewel02BeingStealed;
                                    break;
                                case "jewel03":
                                    PoliceAndThief.thiefplayer08JewelId = 3;
                                    CanUse = !PoliceAndThief.jewel03BeingStealed;
                                    break;
                                case "jewel04":
                                    PoliceAndThief.thiefplayer08JewelId = 4;
                                    CanUse = !PoliceAndThief.jewel04BeingStealed;
                                    break;
                                case "jewel05":
                                    PoliceAndThief.thiefplayer08JewelId = 5;
                                    CanUse = !PoliceAndThief.jewel05BeingStealed;
                                    break;
                                case "jewel06":
                                    PoliceAndThief.thiefplayer08JewelId = 6;
                                    CanUse = !PoliceAndThief.jewel06BeingStealed;
                                    break;
                                case "jewel07":
                                    PoliceAndThief.thiefplayer08JewelId = 7;
                                    CanUse = !PoliceAndThief.jewel07BeingStealed;
                                    break;
                                case "jewel08":
                                    PoliceAndThief.thiefplayer08JewelId = 8;
                                    CanUse = !PoliceAndThief.jewel08BeingStealed;
                                    break;
                                case "jewel09":
                                    PoliceAndThief.thiefplayer08JewelId = 9;
                                    CanUse = !PoliceAndThief.jewel09BeingStealed;
                                    break;
                                case "jewel10":
                                    PoliceAndThief.thiefplayer08JewelId = 10;
                                    CanUse = !PoliceAndThief.jewel10BeingStealed;
                                    break;
                                case "jewel11":
                                    PoliceAndThief.thiefplayer08JewelId = 11;
                                    CanUse = !PoliceAndThief.jewel11BeingStealed;
                                    break;
                                case "jewel12":
                                    PoliceAndThief.thiefplayer08JewelId = 12;
                                    CanUse = !PoliceAndThief.jewel12BeingStealed;
                                    break;
                                case "jewel13":
                                    PoliceAndThief.thiefplayer08JewelId = 13;
                                    CanUse = !PoliceAndThief.jewel13BeingStealed;
                                    break;
                                case "jewel14":
                                    PoliceAndThief.thiefplayer08JewelId = 14;
                                    CanUse = !PoliceAndThief.jewel14BeingStealed;
                                    break;
                                case "jewel15":
                                    PoliceAndThief.thiefplayer08JewelId = 15;
                                    CanUse = !PoliceAndThief.jewel15BeingStealed;
                                    break;
                            }
                        }
                        else if ((PoliceAndThief.jewelbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.jewelbuttontwo.transform.position) < 0.5f || PoliceAndThief.jewelbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.jewelbutton.transform.position) < 0.5f) && PoliceAndThief.thiefplayer08IsStealing)
                        {
                            CanUse = true;
                        }
                    }

                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.thiefplayer08IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { thiefplayer08TakeDeliverJewelButton.Timer = thiefplayer08TakeDeliverJewelButton.MaxTimer; },
            AssetLoader.PoliceAndThiefTakeJewelButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CommonAbilityPrimary,
            false,
            Tr.Get(TrKey.DeliverButton)
        );

        // Thiefplayer09 Kill
        thiefplayer09KillButton = new CustomButton(
            () =>
            {
                byte targetId = PoliceAndThief.thiefplayer09currentTarget.PlayerId;
                byte sourceId = PoliceAndThief.thiefplayer09.PlayerId;
                MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                killWriter.Write(targetId);
                killWriter.Write(sourceId);
                AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                RPCProcedure.gamemodeKills(targetId, sourceId);
                thiefplayer09KillButton.Timer = thiefplayer09KillButton.MaxTimer;
                PoliceAndThief.thiefplayer09currentTarget = null;
            },
            () => { return PoliceAndThief.thiefplayer09 != null && PoliceAndThief.thiefplayer09 == PlayerControl.LocalPlayer && PoliceAndThief.whoCanThiefsKill != 2; },
            () =>
            {
                bool canUse = false;
                switch (PoliceAndThief.whoCanThiefsKill)
                {
                    case 0:
                        if (PoliceAndThief.policeplayer02 != null && PoliceAndThief.thiefplayer09currentTarget == PoliceAndThief.policeplayer02 || PoliceAndThief.policeplayer04 != null && PoliceAndThief.thiefplayer09currentTarget == PoliceAndThief.policeplayer04)
                        {
                            canUse = true;
                        }
                        break;
                    case 1:
                        canUse = true;
                        break;
                }
                return canUse && PoliceAndThief.thiefplayer09currentTarget && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.thiefplayer09IsReviving && !PlayerControl.LocalPlayer.Data.IsDead && !PoliceAndThief.thiefplayer09IsStealing;
            },
            () => { thiefplayer09KillButton.Timer = thiefplayer09KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // Thiefplayer09 FreeThief Button
        thiefplayer09FreeThiefButton = new CustomButton(
            () =>
            {
                MessageWriter thiefFree = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefFreeThief, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(thiefFree);
                RPCProcedure.policeandThiefFreeThief();
                thiefplayer09FreeThiefButton.Timer = thiefplayer09FreeThiefButton.MaxTimer;
            },
            () => { return PoliceAndThief.thiefplayer09 != null && PoliceAndThief.thiefplayer09 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (PoliceAndThief.localThiefReleaseArrow.Count != 0)
                {
                    PoliceAndThief.localThiefReleaseArrow[0].Update(PoliceAndThief.cellbutton.transform.position);
                    if (GameOptionsManager.Instance.currentGameOptions.MapId == 6)
                    {
                        PoliceAndThief.localThiefReleaseArrow[1].Update(PoliceAndThief.cellbuttontwo.transform.position);
                    }
                }
                if (PoliceAndThief.localThiefDeliverArrow.Count != 0)
                {
                    PoliceAndThief.localThiefDeliverArrow[0].Update(PoliceAndThief.jewelbutton.transform.position);
                    if (GameOptionsManager.Instance.currentGameOptions.MapId == 6)
                    {
                        PoliceAndThief.localThiefDeliverArrow[1].Update(PoliceAndThief.jewelbuttontwo.transform.position);
                    }
                }

                bool CanUse = false;
                if (PoliceAndThief.currentThiefsCaptured > 0)
                {
                    if ((PoliceAndThief.cellbuttontwo != null && Vector2.Distance(PoliceAndThief.thiefplayer09.transform.position, PoliceAndThief.cellbuttontwo.transform.position) < 0.4f || Vector2.Distance(PoliceAndThief.thiefplayer09.transform.position, PoliceAndThief.cellbutton.transform.position) < 0.4f) && !PoliceAndThief.thiefplayer09.Data.IsDead)
                    {
                        CanUse = true;
                    }
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.thiefplayer09IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { thiefplayer09FreeThiefButton.Timer = thiefplayer09FreeThiefButton.MaxTimer; },
            AssetLoader.PoliceAndThiefFreeButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CrewmateAbilitySecondary,
            false,
            Tr.Get(TrKey.ReleaseButton)
        );

        // Thiefplayer09 Take/Deliver Jewel Button
        thiefplayer09TakeDeliverJewelButton = new CustomButton(
            () =>
            {
                if (PoliceAndThief.thiefplayer09IsStealing)
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    byte jewelId = PoliceAndThief.thiefplayer09JewelId;
                    MessageWriter thiefScore = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefDeliverJewel, Hazel.SendOption.Reliable, -1);
                    thiefScore.Write(targetId);
                    thiefScore.Write(jewelId);
                    AmongUsClient.Instance.FinishRpcImmediately(thiefScore);
                    RPCProcedure.policeandThiefDeliverJewel(targetId, jewelId);
                }
                else
                {
                    byte targetId = PlayerControl.LocalPlayer.PlayerId;
                    byte jewelId = PoliceAndThief.thiefplayer09JewelId;
                    MessageWriter thiefWhoTookATreasure = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefTakeJewel, Hazel.SendOption.Reliable, -1);
                    thiefWhoTookATreasure.Write(targetId);
                    thiefWhoTookATreasure.Write(jewelId);
                    AmongUsClient.Instance.FinishRpcImmediately(thiefWhoTookATreasure);
                    RPCProcedure.policeandThiefTakeJewel(targetId, jewelId);
                }
                thiefplayer09TakeDeliverJewelButton.Timer = thiefplayer09TakeDeliverJewelButton.MaxTimer;
            },
            () => { return PoliceAndThief.thiefplayer09 != null && PoliceAndThief.thiefplayer09 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (PoliceAndThief.thiefplayer09IsStealing)
                    thiefplayer09TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefDeliverJewelButton;
                else
                    thiefplayer09TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefTakeJewelButton;
                bool CanUse = false;
                if (PoliceAndThief.thiefTreasures.Count != 0)
                {
                    foreach (GameObject jewel in PoliceAndThief.thiefTreasures)
                    {
                        if (jewel != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, jewel.transform.position) < 0.5f && !PoliceAndThief.thiefplayer09IsStealing)
                        {
                            switch (jewel.name)
                            {
                                case "jewel01":
                                    PoliceAndThief.thiefplayer09JewelId = 1;
                                    CanUse = !PoliceAndThief.jewel01BeingStealed;
                                    break;
                                case "jewel02":
                                    PoliceAndThief.thiefplayer09JewelId = 2;
                                    CanUse = !PoliceAndThief.jewel02BeingStealed;
                                    break;
                                case "jewel03":
                                    PoliceAndThief.thiefplayer09JewelId = 3;
                                    CanUse = !PoliceAndThief.jewel03BeingStealed;
                                    break;
                                case "jewel04":
                                    PoliceAndThief.thiefplayer09JewelId = 4;
                                    CanUse = !PoliceAndThief.jewel04BeingStealed;
                                    break;
                                case "jewel05":
                                    PoliceAndThief.thiefplayer09JewelId = 5;
                                    CanUse = !PoliceAndThief.jewel05BeingStealed;
                                    break;
                                case "jewel06":
                                    PoliceAndThief.thiefplayer09JewelId = 6;
                                    CanUse = !PoliceAndThief.jewel06BeingStealed;
                                    break;
                                case "jewel07":
                                    PoliceAndThief.thiefplayer09JewelId = 7;
                                    CanUse = !PoliceAndThief.jewel07BeingStealed;
                                    break;
                                case "jewel08":
                                    PoliceAndThief.thiefplayer09JewelId = 8;
                                    CanUse = !PoliceAndThief.jewel08BeingStealed;
                                    break;
                                case "jewel09":
                                    PoliceAndThief.thiefplayer09JewelId = 9;
                                    CanUse = !PoliceAndThief.jewel09BeingStealed;
                                    break;
                                case "jewel10":
                                    PoliceAndThief.thiefplayer09JewelId = 10;
                                    CanUse = !PoliceAndThief.jewel10BeingStealed;
                                    break;
                                case "jewel11":
                                    PoliceAndThief.thiefplayer09JewelId = 11;
                                    CanUse = !PoliceAndThief.jewel11BeingStealed;
                                    break;
                                case "jewel12":
                                    PoliceAndThief.thiefplayer09JewelId = 12;
                                    CanUse = !PoliceAndThief.jewel12BeingStealed;
                                    break;
                                case "jewel13":
                                    PoliceAndThief.thiefplayer09JewelId = 13;
                                    CanUse = !PoliceAndThief.jewel13BeingStealed;
                                    break;
                                case "jewel14":
                                    PoliceAndThief.thiefplayer09JewelId = 14;
                                    CanUse = !PoliceAndThief.jewel14BeingStealed;
                                    break;
                                case "jewel15":
                                    PoliceAndThief.thiefplayer09JewelId = 15;
                                    CanUse = !PoliceAndThief.jewel15BeingStealed;
                                    break;
                            }
                        }
                        else if ((PoliceAndThief.jewelbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.jewelbuttontwo.transform.position) < 0.5f || PoliceAndThief.jewelbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, PoliceAndThief.jewelbutton.transform.position) < 0.5f) && PoliceAndThief.thiefplayer09IsStealing)
                        {
                            CanUse = true;
                        }
                    }

                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PoliceAndThief.thiefplayer09IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { thiefplayer09TakeDeliverJewelButton.Timer = thiefplayer09TakeDeliverJewelButton.MaxTimer; },
            AssetLoader.PoliceAndThiefTakeJewelButton,
            ButtonPosition.Layout,
            __instance,
            __instance.UseButton,
            AbilitySlot.CommonAbilityPrimary,
            false,
            Tr.Get(TrKey.DeliverButton)
        );
    }
}