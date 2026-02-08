namespace RebuildUs.Modules.GameMode;

public static partial class PoliceAndThief
{
    // Police and Thief
    private static CustomButton _policeplayer01JailButton;
    private static CustomButton _policeplayer01KillButton;
    private static CustomButton _policeplayer01LightButton;
    private static CustomButton _policeplayer02KillButton;
    private static CustomButton _policeplayer02TaseButton;
    private static CustomButton _policeplayer02LightButton;
    private static CustomButton _policeplayer03JailButton;
    private static CustomButton _policeplayer03KillButton;
    private static CustomButton _policeplayer03LightButton;
    private static CustomButton _policeplayer04TaseButton;
    private static CustomButton _policeplayer04KillButton;
    private static CustomButton _policeplayer04LightButton;
    private static CustomButton _policeplayer05JailButton;
    private static CustomButton _policeplayer05KillButton;
    private static CustomButton _policeplayer05LightButton;
    private static CustomButton _policeplayer06JailButton;
    private static CustomButton _policeplayer06KillButton;
    private static CustomButton _policeplayer06LightButton;

    private static CustomButton _thiefplayer01KillButton;
    private static CustomButton _thiefplayer01FreeThiefButton;
    private static CustomButton _thiefplayer01TakeDeliverJewelButton;
    private static CustomButton _thiefplayer02KillButton;
    private static CustomButton _thiefplayer02FreeThiefButton;
    private static CustomButton _thiefplayer02TakeDeliverJewelButton;
    private static CustomButton _thiefplayer03KillButton;
    private static CustomButton _thiefplayer03FreeThiefButton;
    private static CustomButton _thiefplayer03TakeDeliverJewelButton;
    private static CustomButton _thiefplayer04KillButton;
    private static CustomButton _thiefplayer04FreeThiefButton;
    private static CustomButton _thiefplayer04TakeDeliverJewelButton;
    private static CustomButton _thiefplayer05KillButton;
    private static CustomButton _thiefplayer05FreeThiefButton;
    private static CustomButton _thiefplayer05TakeDeliverJewelButton;
    private static CustomButton _thiefplayer06KillButton;
    private static CustomButton _thiefplayer06FreeThiefButton;
    private static CustomButton _thiefplayer06TakeDeliverJewelButton;
    private static CustomButton _thiefplayer07KillButton;
    private static CustomButton _thiefplayer07FreeThiefButton;
    private static CustomButton _thiefplayer07TakeDeliverJewelButton;
    private static CustomButton _thiefplayer08KillButton;
    private static CustomButton _thiefplayer08FreeThiefButton;
    private static CustomButton _thiefplayer08TakeDeliverJewelButton;
    private static CustomButton _thiefplayer09KillButton;
    private static CustomButton _thiefplayer09FreeThiefButton;
    private static CustomButton _thiefplayer09TakeDeliverJewelButton;

    public static void SetButtonCooldowns()
    {
        // Police And Thief buttons
        _policeplayer01KillButton.MaxTimer = MapSettings.GamemodeKillCooldown;
        _policeplayer01JailButton.MaxTimer = PoliceCatchCooldown;
        _policeplayer01JailButton.EffectDuration = CaptureThiefTime;
        _policeplayer01LightButton.MaxTimer = 15;
        _policeplayer01LightButton.EffectDuration = 10;
        _policeplayer02KillButton.MaxTimer = MapSettings.GamemodeKillCooldown;
        _policeplayer02TaseButton.MaxTimer = PoliceTaseCooldown;
        _policeplayer02LightButton.MaxTimer = 15;
        _policeplayer02LightButton.EffectDuration = 10;
        _policeplayer03KillButton.MaxTimer = MapSettings.GamemodeKillCooldown;
        _policeplayer03JailButton.MaxTimer = PoliceCatchCooldown;
        _policeplayer03JailButton.EffectDuration = CaptureThiefTime;
        _policeplayer03LightButton.MaxTimer = 15;
        _policeplayer03LightButton.EffectDuration = 10;
        _policeplayer04KillButton.MaxTimer = MapSettings.GamemodeKillCooldown;
        _policeplayer04TaseButton.MaxTimer = PoliceTaseCooldown;
        _policeplayer04LightButton.MaxTimer = 15;
        _policeplayer04LightButton.EffectDuration = 10;
        _policeplayer05KillButton.MaxTimer = MapSettings.GamemodeKillCooldown;
        _policeplayer05JailButton.MaxTimer = PoliceCatchCooldown;
        _policeplayer05JailButton.EffectDuration = CaptureThiefTime;
        _policeplayer05LightButton.MaxTimer = 15;
        _policeplayer05LightButton.EffectDuration = 10;
        _policeplayer06KillButton.MaxTimer = MapSettings.GamemodeKillCooldown;
        _policeplayer06JailButton.MaxTimer = PoliceCatchCooldown;
        _policeplayer06JailButton.EffectDuration = CaptureThiefTime;
        _policeplayer06LightButton.MaxTimer = 15;
        _policeplayer06LightButton.EffectDuration = 10;

        _thiefplayer01KillButton.MaxTimer = MapSettings.GamemodeKillCooldown * 1.25f;
        _thiefplayer01FreeThiefButton.MaxTimer = 20f;
        _thiefplayer01TakeDeliverJewelButton.MaxTimer = 5f;
        _thiefplayer02KillButton.MaxTimer = MapSettings.GamemodeKillCooldown * 1.25f;
        _thiefplayer02FreeThiefButton.MaxTimer = 20f;
        _thiefplayer02TakeDeliverJewelButton.MaxTimer = 5f;
        _thiefplayer03KillButton.MaxTimer = MapSettings.GamemodeKillCooldown * 1.25f;
        _thiefplayer03FreeThiefButton.MaxTimer = 20f;
        _thiefplayer03TakeDeliverJewelButton.MaxTimer = 5f;
        _thiefplayer04KillButton.MaxTimer = MapSettings.GamemodeKillCooldown * 1.25f;
        _thiefplayer04FreeThiefButton.MaxTimer = 20f;
        _thiefplayer04TakeDeliverJewelButton.MaxTimer = 5f;
        _thiefplayer05KillButton.MaxTimer = MapSettings.GamemodeKillCooldown * 1.25f;
        _thiefplayer05FreeThiefButton.MaxTimer = 20f;
        _thiefplayer05TakeDeliverJewelButton.MaxTimer = 5f;
        _thiefplayer06KillButton.MaxTimer = MapSettings.GamemodeKillCooldown * 1.25f;
        _thiefplayer06FreeThiefButton.MaxTimer = 20f;
        _thiefplayer06TakeDeliverJewelButton.MaxTimer = 5f;
        _thiefplayer07KillButton.MaxTimer = MapSettings.GamemodeKillCooldown * 1.25f;
        _thiefplayer07FreeThiefButton.MaxTimer = 20f;
        _thiefplayer07TakeDeliverJewelButton.MaxTimer = 5f;
        _thiefplayer08KillButton.MaxTimer = MapSettings.GamemodeKillCooldown * 1.25f;
        _thiefplayer08FreeThiefButton.MaxTimer = 20f;
        _thiefplayer08TakeDeliverJewelButton.MaxTimer = 5f;
        _thiefplayer09KillButton.MaxTimer = MapSettings.GamemodeKillCooldown * 1.25f;
        _thiefplayer09FreeThiefButton.MaxTimer = 20f;
        _thiefplayer09TakeDeliverJewelButton.MaxTimer = 5f;
    }

    public static void MakeButtons(HudManager __instance)
    {
        // Police and Thief Mode
        // Policeplayer01 Kill
        _policeplayer01KillButton = new(() =>
        {
            var targetId = Policeplayer01CurrentTarget.PlayerId;
            var sourceId = Policeplayer01.PlayerId;
            var killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            killWriter.Write(targetId);
            killWriter.Write(sourceId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.GamemodeKills(targetId, sourceId);
            _policeplayer01KillButton.Timer = _policeplayer01KillButton.MaxTimer;
            Policeplayer01CurrentTarget = null;
        }, () => { return Policeplayer01 != null && Policeplayer01 == PlayerControl.LocalPlayer; }, () =>
        {
            var canUse = true;
            if (((Cellbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Cellbuttontwo.transform.position) <= 3f) || (Cellbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Cellbutton.transform.position) <= 3f)) && !PlayerControl.LocalPlayer.Data.IsDead) canUse = false;
            return canUse && Policeplayer01CurrentTarget && PlayerControl.LocalPlayer.CanMove && !Policeplayer01IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _policeplayer01KillButton.Timer = _policeplayer01KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // Policeplayer01 Jail
        _policeplayer01JailButton = new(() =>
        {
            if (Policeplayer01CurrentTarget != null)
            {
                Policeplayer01TargetedPlayer = Policeplayer01CurrentTarget;
                _policeplayer01JailButton.HasEffect = true;
            }
        }, () => { return Policeplayer01 != null && Policeplayer01 == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; }, () =>
        {
            if (_policeplayer01JailButton.IsEffectActive && Policeplayer01TargetedPlayer != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Policeplayer01TargetedPlayer.transform.position) > LegacyGameOptions.KillDistances[Mathf.Clamp(GameOptionsManager.Instance.currentGameOptions.GetInt(Int32OptionNames.KillDistance), 0, 2)])
            {
                Policeplayer01TargetedPlayer = null;
                _policeplayer01JailButton.Timer = 0f;
                _policeplayer01JailButton.IsEffectActive = false;
            }

            var canUse = true;
            if (((Jewelbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Jewelbuttontwo.transform.position) <= 3f) || (Jewelbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Jewelbutton.transform.position) <= 3f)) && !PlayerControl.LocalPlayer.Data.IsDead) canUse = false;
            return canUse && PlayerControl.LocalPlayer.CanMove && !Policeplayer01IsReviving && Policeplayer01CurrentTarget != null;
        }, () =>
        {
            Policeplayer01TargetedPlayer = null;
            _policeplayer01JailButton.IsEffectActive = false;
            _policeplayer01JailButton.Timer = _policeplayer01JailButton.MaxTimer;
            _policeplayer01JailButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
        }, AssetLoader.PoliceAndThiefCaptureButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CommonAbilityPrimary, true, CaptureThiefTime, () =>
        {
            if (Policeplayer01TargetedPlayer != null && !Policeplayer01TargetedPlayer.Data.IsDead)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefJail, SendOption.Reliable);
                writer.Write(Policeplayer01TargetedPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.PoliceandThiefJail(Policeplayer01TargetedPlayer.PlayerId);
                Policeplayer01TargetedPlayer = null;
                _policeplayer01JailButton.Timer = _policeplayer01JailButton.MaxTimer;
            }
        });

        // Policeplayer01 Light
        _policeplayer01LightButton = new(() => { Policeplayer01LightTimer = 10; }, () => { return Policeplayer01 != null && Policeplayer01 == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; }, () => { return PlayerControl.LocalPlayer.CanMove && !Policeplayer01IsReviving; }, () =>
        {
            _policeplayer01LightButton.Timer = _policeplayer01LightButton.MaxTimer;
            _policeplayer01LightButton.IsEffectActive = false;
            _policeplayer01LightButton.ActionButton.graphic.color = Palette.EnabledColor;
        }, AssetLoader.PoliceAndThiefsLightButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.ImpostorAbilitySecondary, true, 10, () => { _policeplayer01LightButton.Timer = _policeplayer01LightButton.MaxTimer; });

        // Policeplayer02 Kill
        _policeplayer02KillButton = new(() =>
        {
            var targetId = Policeplayer02CurrentTarget.PlayerId;
            var sourceId = Policeplayer02.PlayerId;
            var killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            killWriter.Write(targetId);
            killWriter.Write(sourceId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.GamemodeKills(targetId, sourceId);
            _policeplayer02KillButton.Timer = _policeplayer02KillButton.MaxTimer;
            Policeplayer02CurrentTarget = null;
        }, () => { return Policeplayer02 != null && Policeplayer02 == PlayerControl.LocalPlayer; }, () =>
        {
            var canUse = true;
            if (((Cellbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Cellbuttontwo.transform.position) <= 3f) || (Cellbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Cellbutton.transform.position) <= 3f)) && !PlayerControl.LocalPlayer.Data.IsDead) canUse = false;
            return canUse && Policeplayer02CurrentTarget && PlayerControl.LocalPlayer.CanMove && !Policeplayer02IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _policeplayer02KillButton.Timer = _policeplayer02KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // Policeplayer02 Tase
        _policeplayer02TaseButton = new(() =>
        {
            var target = GetTasedPlayer(2 * 0.2f, 6, true);

            if (target == null) target = PlayerControl.LocalPlayer;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefsTased, SendOption.Reliable);
            writer.Write(target.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.PoliceandThiefsTased(target.PlayerId);

            _policeplayer02TaseButton.Timer = _policeplayer02TaseButton.MaxTimer;
            Policeplayer02CurrentTarget = null;

            target = null;
        }, () => { return Policeplayer02 != null && Policeplayer02 == PlayerControl.LocalPlayer; }, () =>
        {
            if (Policeplayer02Taser == null)
            {
                Policeplayer02Taser = new("Weapon");
                var renderer = Policeplayer02Taser.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.PoliceAndThiefsTaser;
                renderer.transform.parent = Policeplayer02.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                Policeplayer02MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = Policeplayer02.transform.position + new Vector3(0.8f * (float)Math.Cos(Policeplayer02MouseAngle), 0.8f * (float)Math.Sin(Policeplayer02MouseAngle));
                Policeplayer02Taser.transform.position += (targetPosition - Policeplayer02Taser.transform.position) * 0.4f;
                Policeplayer02Taser.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((Policeplayer02MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(Policeplayer02MouseAngle) < 0.0)
                {
                    if (Policeplayer02Taser.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        Policeplayer02Taser.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (Policeplayer02Taser.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        Policeplayer02Taser.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            var canUse = true;
            if (((Jewelbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Jewelbuttontwo.transform.position) <= 3f) || (Jewelbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Jewelbutton.transform.position) <= 3f)) && !PlayerControl.LocalPlayer.Data.IsDead) canUse = false;
            return canUse && PlayerControl.LocalPlayer.CanMove && !Policeplayer02IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _policeplayer02TaseButton.Timer = _policeplayer02TaseButton.MaxTimer; }, AssetLoader.PoliceAndThiefsTaserButton, ButtonPosition.Layout, __instance, __instance.UseButton, KeyCode.Mouse1, false, Tr.Get(TrKey.ParalyzeButton));

        // Policeplayer02 Light
        _policeplayer02LightButton = new(() => { Policeplayer02LightTimer = 10; }, () => { return Policeplayer02 != null && Policeplayer02 == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; }, () => { return PlayerControl.LocalPlayer.CanMove && !Policeplayer02IsReviving; }, () =>
        {
            _policeplayer02LightButton.Timer = _policeplayer02LightButton.MaxTimer;
            _policeplayer02LightButton.IsEffectActive = false;
            _policeplayer02LightButton.ActionButton.graphic.color = Palette.EnabledColor;
        }, AssetLoader.PoliceAndThiefsLightButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.ImpostorAbilitySecondary, true, 10, () => { _policeplayer02LightButton.Timer = _policeplayer02LightButton.MaxTimer; });

        // Policeplayer03 Kill
        _policeplayer03KillButton = new(() =>
        {
            var targetId = Policeplayer03CurrentTarget.PlayerId;
            var sourceId = Policeplayer03.PlayerId;
            var killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            killWriter.Write(targetId);
            killWriter.Write(sourceId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.GamemodeKills(targetId, sourceId);
            _policeplayer03KillButton.Timer = _policeplayer03KillButton.MaxTimer;
            Policeplayer03CurrentTarget = null;
        }, () => { return Policeplayer03 != null && Policeplayer03 == PlayerControl.LocalPlayer; }, () =>
        {
            var canUse = true;
            if (((Cellbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Cellbuttontwo.transform.position) <= 3f) || (Cellbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Cellbutton.transform.position) <= 3f)) && !PlayerControl.LocalPlayer.Data.IsDead) canUse = false;
            return canUse && Policeplayer03CurrentTarget && PlayerControl.LocalPlayer.CanMove && !Policeplayer03IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _policeplayer03KillButton.Timer = _policeplayer03KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // Policeplayer03 Jail
        _policeplayer03JailButton = new(() =>
        {
            if (Policeplayer03CurrentTarget != null)
            {
                Policeplayer03TargetedPlayer = Policeplayer03CurrentTarget;
                _policeplayer03JailButton.HasEffect = true;
            }
        }, () => { return Policeplayer03 != null && Policeplayer03 == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; }, () =>
        {
            if (_policeplayer03JailButton.IsEffectActive && Policeplayer03TargetedPlayer != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Policeplayer03TargetedPlayer.transform.position) > LegacyGameOptions.KillDistances[Mathf.Clamp(GameOptionsManager.Instance.currentGameOptions.GetInt(Int32OptionNames.KillDistance), 0, 2)])
            {
                Policeplayer03TargetedPlayer = null;
                _policeplayer03JailButton.Timer = 0f;
                _policeplayer03JailButton.IsEffectActive = false;
            }

            var canUse = true;
            if (((Jewelbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Jewelbuttontwo.transform.position) <= 3f) || (Jewelbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Jewelbutton.transform.position) <= 3f)) && !PlayerControl.LocalPlayer.Data.IsDead) canUse = false;
            return canUse && PlayerControl.LocalPlayer.CanMove && !Policeplayer03IsReviving && Policeplayer03CurrentTarget != null;
        }, () =>
        {
            Policeplayer03TargetedPlayer = null;
            _policeplayer03JailButton.IsEffectActive = false;
            _policeplayer03JailButton.Timer = _policeplayer03JailButton.MaxTimer;
            _policeplayer03JailButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
        }, AssetLoader.PoliceAndThiefCaptureButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CommonAbilityPrimary, true, CaptureThiefTime, () =>
        {
            if (Policeplayer03TargetedPlayer != null && !Policeplayer03TargetedPlayer.Data.IsDead)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefJail, SendOption.Reliable);
                writer.Write(Policeplayer03TargetedPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.PoliceandThiefJail(Policeplayer03TargetedPlayer.PlayerId);
                Policeplayer03TargetedPlayer = null;
                _policeplayer03JailButton.Timer = _policeplayer03JailButton.MaxTimer;
            }
        });

        // Policeplayer03 Light
        _policeplayer03LightButton = new(() => { Policeplayer03LightTimer = 10; }, () => { return Policeplayer03 != null && Policeplayer03 == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; }, () => { return PlayerControl.LocalPlayer.CanMove && !Policeplayer03IsReviving; }, () =>
        {
            _policeplayer03LightButton.Timer = _policeplayer03LightButton.MaxTimer;
            _policeplayer03LightButton.IsEffectActive = false;
            _policeplayer03LightButton.ActionButton.graphic.color = Palette.EnabledColor;
        }, AssetLoader.PoliceAndThiefsLightButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.ImpostorAbilitySecondary, true, 10, () => { _policeplayer03LightButton.Timer = _policeplayer03LightButton.MaxTimer; });

        // Policeplayer04 Kill
        _policeplayer04KillButton = new(() =>
        {
            var targetId = Policeplayer04CurrentTarget.PlayerId;
            var sourceId = Policeplayer04.PlayerId;
            var killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            killWriter.Write(targetId);
            killWriter.Write(sourceId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.GamemodeKills(targetId, sourceId);
            _policeplayer04KillButton.Timer = _policeplayer04KillButton.MaxTimer;
            Policeplayer04CurrentTarget = null;
        }, () => { return Policeplayer04 != null && Policeplayer04 == PlayerControl.LocalPlayer; }, () =>
        {
            var canUse = true;
            if (((Cellbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Cellbuttontwo.transform.position) <= 3f) || (Cellbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Cellbutton.transform.position) <= 3f)) && !PlayerControl.LocalPlayer.Data.IsDead) canUse = false;
            return canUse && Policeplayer04CurrentTarget && PlayerControl.LocalPlayer.CanMove && !Policeplayer04IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _policeplayer04KillButton.Timer = _policeplayer04KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // Policeplayer04 Tase
        _policeplayer04TaseButton = new(() =>
        {
            var target = GetTasedPlayer(2 * 0.2f, 6, false);

            if (target == null) target = PlayerControl.LocalPlayer;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefsTased, SendOption.Reliable);
            writer.Write(target.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.PoliceandThiefsTased(target.PlayerId);

            _policeplayer04TaseButton.Timer = _policeplayer04TaseButton.MaxTimer;
            Policeplayer04CurrentTarget = null;

            target = null;
        }, () => { return Policeplayer04 != null && Policeplayer04 == PlayerControl.LocalPlayer; }, () =>
        {
            if (Policeplayer04Taser == null)
            {
                Policeplayer04Taser = new("Weapon");
                var renderer = Policeplayer04Taser.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.PoliceAndThiefsTaser;
                renderer.transform.parent = Policeplayer04.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                Policeplayer04MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = Policeplayer04.transform.position + new Vector3(0.8f * (float)Math.Cos(Policeplayer04MouseAngle), 0.8f * (float)Math.Sin(Policeplayer04MouseAngle));
                Policeplayer04Taser.transform.position += (targetPosition - Policeplayer04Taser.transform.position) * 0.4f;
                Policeplayer04Taser.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((Policeplayer04MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(Policeplayer04MouseAngle) < 0.0)
                {
                    if (Policeplayer04Taser.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        Policeplayer04Taser.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (Policeplayer04Taser.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        Policeplayer04Taser.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            var canUse = true;
            if (((Jewelbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Jewelbuttontwo.transform.position) <= 3f) || (Jewelbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Jewelbutton.transform.position) <= 3f)) && !PlayerControl.LocalPlayer.Data.IsDead) canUse = false;
            return canUse && PlayerControl.LocalPlayer.CanMove && !Policeplayer04IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _policeplayer04TaseButton.Timer = _policeplayer04TaseButton.MaxTimer; }, AssetLoader.PoliceAndThiefsTaserButton, ButtonPosition.Layout, __instance, __instance.UseButton, KeyCode.Mouse1, false, Tr.Get(TrKey.ParalyzeButton));

        // Policeplayer04 Light
        _policeplayer04LightButton = new(() => { Policeplayer04LightTimer = 10; }, () => { return Policeplayer04 != null && Policeplayer04 == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; }, () => { return PlayerControl.LocalPlayer.CanMove && !Policeplayer04IsReviving; }, () =>
        {
            _policeplayer04LightButton.Timer = _policeplayer04LightButton.MaxTimer;
            _policeplayer04LightButton.IsEffectActive = false;
            _policeplayer04LightButton.ActionButton.graphic.color = Palette.EnabledColor;
        }, AssetLoader.PoliceAndThiefsLightButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.ImpostorAbilitySecondary, true, 10, () => { _policeplayer04LightButton.Timer = _policeplayer04LightButton.MaxTimer; });

        // Policeplayer05 Kill
        _policeplayer05KillButton = new(() =>
        {
            var targetId = Policeplayer05CurrentTarget.PlayerId;
            var sourceId = Policeplayer05.PlayerId;
            var killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            killWriter.Write(targetId);
            killWriter.Write(sourceId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.GamemodeKills(targetId, sourceId);
            _policeplayer05KillButton.Timer = _policeplayer05KillButton.MaxTimer;
            Policeplayer05CurrentTarget = null;
        }, () => { return Policeplayer05 != null && Policeplayer05 == PlayerControl.LocalPlayer; }, () =>
        {
            var canUse = true;
            if (((Cellbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Cellbuttontwo.transform.position) <= 3f) || (Cellbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Cellbutton.transform.position) <= 3f)) && !PlayerControl.LocalPlayer.Data.IsDead) canUse = false;
            return canUse && Policeplayer05CurrentTarget && PlayerControl.LocalPlayer.CanMove && !Policeplayer05IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _policeplayer05KillButton.Timer = _policeplayer05KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // Policeplayer05 Jail
        _policeplayer05JailButton = new(() =>
        {
            if (Policeplayer05CurrentTarget != null)
            {
                Policeplayer05TargetedPlayer = Policeplayer05CurrentTarget;
                _policeplayer05JailButton.HasEffect = true;
            }
        }, () => { return Policeplayer05 != null && Policeplayer05 == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; }, () =>
        {
            if (_policeplayer05JailButton.IsEffectActive && Policeplayer05TargetedPlayer != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Policeplayer05TargetedPlayer.transform.position) > LegacyGameOptions.KillDistances[Mathf.Clamp(GameOptionsManager.Instance.currentGameOptions.GetInt(Int32OptionNames.KillDistance), 0, 2)])
            {
                Policeplayer05TargetedPlayer = null;
                _policeplayer05JailButton.Timer = 0f;
                _policeplayer05JailButton.IsEffectActive = false;
            }

            var canUse = true;
            if (((Jewelbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Jewelbuttontwo.transform.position) <= 3f) || (Jewelbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Jewelbutton.transform.position) <= 3f)) && !PlayerControl.LocalPlayer.Data.IsDead) canUse = false;
            return canUse && PlayerControl.LocalPlayer.CanMove && !Policeplayer05IsReviving && Policeplayer05CurrentTarget != null;
        }, () =>
        {
            Policeplayer05TargetedPlayer = null;
            _policeplayer05JailButton.IsEffectActive = false;
            _policeplayer05JailButton.Timer = _policeplayer05JailButton.MaxTimer;
            _policeplayer05JailButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
        }, AssetLoader.PoliceAndThiefCaptureButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CommonAbilityPrimary, true, CaptureThiefTime, () =>
        {
            if (Policeplayer05TargetedPlayer != null && !Policeplayer05TargetedPlayer.Data.IsDead)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefJail, SendOption.Reliable);
                writer.Write(Policeplayer05TargetedPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.PoliceandThiefJail(Policeplayer05TargetedPlayer.PlayerId);
                Policeplayer05TargetedPlayer = null;
                _policeplayer05JailButton.Timer = _policeplayer05JailButton.MaxTimer;
            }
        });

        // Policeplayer05 Light
        _policeplayer05LightButton = new(() => { Policeplayer05LightTimer = 10; }, () => { return Policeplayer05 != null && Policeplayer05 == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; }, () => { return PlayerControl.LocalPlayer.CanMove && !Policeplayer05IsReviving; }, () =>
        {
            _policeplayer05LightButton.Timer = _policeplayer05LightButton.MaxTimer;
            _policeplayer05LightButton.IsEffectActive = false;
            _policeplayer05LightButton.ActionButton.graphic.color = Palette.EnabledColor;
        }, AssetLoader.PoliceAndThiefsLightButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.ImpostorAbilitySecondary, true, 10, () => { _policeplayer05LightButton.Timer = _policeplayer05LightButton.MaxTimer; });

        // Policeplayer06 Kill
        _policeplayer06KillButton = new(() =>
        {
            var targetId = Policeplayer06CurrentTarget.PlayerId;
            var sourceId = Policeplayer06.PlayerId;
            var killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            killWriter.Write(targetId);
            killWriter.Write(sourceId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.GamemodeKills(targetId, sourceId);
            _policeplayer06KillButton.Timer = _policeplayer06KillButton.MaxTimer;
            Policeplayer06CurrentTarget = null;
        }, () => { return Policeplayer06 != null && Policeplayer06 == PlayerControl.LocalPlayer; }, () =>
        {
            var canUse = true;
            if (((Cellbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Cellbuttontwo.transform.position) <= 3f) || (Cellbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Cellbutton.transform.position) <= 3f)) && !PlayerControl.LocalPlayer.Data.IsDead) canUse = false;
            return canUse && Policeplayer06CurrentTarget && PlayerControl.LocalPlayer.CanMove && !Policeplayer06IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _policeplayer06KillButton.Timer = _policeplayer06KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, AbilitySlot.ImpostorAbilityPrimary, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // Policeplayer06 Jail
        _policeplayer06JailButton = new(() =>
        {
            if (Policeplayer06CurrentTarget != null)
            {
                Policeplayer06TargetedPlayer = Policeplayer06CurrentTarget;
                _policeplayer06JailButton.HasEffect = true;
            }
        }, () => { return Policeplayer06 != null && Policeplayer06 == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; }, () =>
        {
            if (_policeplayer06JailButton.IsEffectActive && Policeplayer06TargetedPlayer != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Policeplayer06TargetedPlayer.transform.position) > LegacyGameOptions.KillDistances[Mathf.Clamp(GameOptionsManager.Instance.currentGameOptions.GetInt(Int32OptionNames.KillDistance), 0, 2)])
            {
                Policeplayer06TargetedPlayer = null;
                _policeplayer06JailButton.Timer = 0f;
                _policeplayer06JailButton.IsEffectActive = false;
            }

            var canUse = true;
            if (((Jewelbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Jewelbuttontwo.transform.position) <= 3f) || (Jewelbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Jewelbutton.transform.position) <= 3f)) && !PlayerControl.LocalPlayer.Data.IsDead) canUse = false;
            return canUse && PlayerControl.LocalPlayer.CanMove && !Policeplayer06IsReviving && Policeplayer06CurrentTarget != null;
        }, () =>
        {
            Policeplayer06TargetedPlayer = null;
            _policeplayer06JailButton.IsEffectActive = false;
            _policeplayer06JailButton.Timer = _policeplayer06JailButton.MaxTimer;
            _policeplayer06JailButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
        }, AssetLoader.PoliceAndThiefCaptureButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CommonAbilityPrimary, true, CaptureThiefTime, () =>
        {
            if (Policeplayer06TargetedPlayer != null && !Policeplayer06TargetedPlayer.Data.IsDead)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefJail, SendOption.Reliable);
                writer.Write(Policeplayer06TargetedPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.PoliceandThiefJail(Policeplayer06TargetedPlayer.PlayerId);
                Policeplayer06TargetedPlayer = null;
                _policeplayer06JailButton.Timer = _policeplayer06JailButton.MaxTimer;
            }
        });

        // Policeplayer06 Light
        _policeplayer06LightButton = new(() => { Policeplayer06LightTimer = 10; }, () => { return Policeplayer06 != null && Policeplayer06 == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; }, () => { return PlayerControl.LocalPlayer.CanMove && !Policeplayer06IsReviving; }, () =>
        {
            _policeplayer06LightButton.Timer = _policeplayer06LightButton.MaxTimer;
            _policeplayer06LightButton.IsEffectActive = false;
            _policeplayer06LightButton.ActionButton.graphic.color = Palette.EnabledColor;
        }, AssetLoader.PoliceAndThiefsLightButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.ImpostorAbilitySecondary, true, 10, () => { _policeplayer06LightButton.Timer = _policeplayer06LightButton.MaxTimer; });

        // Thiefplayer01 Kill
        _thiefplayer01KillButton = new(() =>
        {
            var targetId = Thiefplayer01CurrentTarget.PlayerId;
            var sourceId = Thiefplayer01.PlayerId;
            var killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            killWriter.Write(targetId);
            killWriter.Write(sourceId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.GamemodeKills(targetId, sourceId);
            _thiefplayer01KillButton.Timer = _thiefplayer01KillButton.MaxTimer;
            Thiefplayer01CurrentTarget = null;
        }, () => { return Thiefplayer01 != null && Thiefplayer01 == PlayerControl.LocalPlayer && WhoCanThiefsKill != 2; }, () =>
        {
            var canUse = false;
            switch (WhoCanThiefsKill)
            {
                case 0:
                    if ((Policeplayer02 != null && Thiefplayer01CurrentTarget == Policeplayer02) || (Policeplayer04 != null && Thiefplayer01CurrentTarget == Policeplayer04)) canUse = true;
                    break;
                case 1:
                    canUse = true;
                    break;
            }

            return canUse && Thiefplayer01CurrentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead && !Thiefplayer01IsReviving && !Thiefplayer01IsStealing;
        }, () => { _thiefplayer01KillButton.Timer = _thiefplayer01KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, AbilitySlot.CrewmateAbilityPrimary, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // Thiefplayer01 FreeThief Button
        _thiefplayer01FreeThiefButton = new(() =>
        {
            var thiefFree = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefFreeThief, SendOption.Reliable);
            AmongUsClient.Instance.FinishRpcImmediately(thiefFree);
            RPCProcedure.PoliceandThiefFreeThief();
            _thiefplayer01FreeThiefButton.Timer = _thiefplayer01FreeThiefButton.MaxTimer;
        }, () => { return Thiefplayer01 != null && Thiefplayer01 == PlayerControl.LocalPlayer; }, () =>
        {
            if (LocalThiefReleaseArrow.Count != 0)
            {
                LocalThiefReleaseArrow[0].Update(Cellbutton.transform.position);
                if (GameOptionsManager.Instance.currentGameOptions.MapId == 6) LocalThiefReleaseArrow[1].Update(Cellbuttontwo.transform.position);
            }

            if (LocalThiefDeliverArrow.Count != 0)
            {
                LocalThiefDeliverArrow[0].Update(Jewelbutton.transform.position);
                if (GameOptionsManager.Instance.currentGameOptions.MapId == 6) LocalThiefDeliverArrow[1].Update(Jewelbuttontwo.transform.position);
            }

            var canUse = false;
            if (CurrentThiefsCaptured > 0)
            {
                if (((Cellbuttontwo != null && Vector2.Distance(Thiefplayer01.transform.position, Cellbuttontwo.transform.position) < 0.4f) || Vector2.Distance(Thiefplayer01.transform.position, Cellbutton.transform.position) < 0.4f) && !Thiefplayer01.Data.IsDead)
                    canUse = true;
            }

            return canUse && PlayerControl.LocalPlayer.CanMove && !Thiefplayer01IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _thiefplayer01FreeThiefButton.Timer = _thiefplayer01FreeThiefButton.MaxTimer; }, AssetLoader.PoliceAndThiefFreeButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CrewmateAbilitySecondary, false, Tr.Get(TrKey.ReleaseButton));

        // Thiefplayer01 Take/Deliver Jewel Button
        _thiefplayer01TakeDeliverJewelButton = new(() =>
        {
            if (Thiefplayer01IsStealing)
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var jewelId = Thiefplayer01JewelId;
                var thiefScore = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefDeliverJewel, SendOption.Reliable);
                thiefScore.Write(targetId);
                thiefScore.Write(jewelId);
                AmongUsClient.Instance.FinishRpcImmediately(thiefScore);
                RPCProcedure.PoliceandThiefDeliverJewel(targetId, jewelId);
            }
            else
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var jewelId = Thiefplayer01JewelId;
                var thiefWhoTookATreasure = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefTakeJewel, SendOption.Reliable);
                thiefWhoTookATreasure.Write(targetId);
                thiefWhoTookATreasure.Write(jewelId);
                AmongUsClient.Instance.FinishRpcImmediately(thiefWhoTookATreasure);
                RPCProcedure.PoliceandThiefTakeJewel(targetId, jewelId);
            }

            _thiefplayer01TakeDeliverJewelButton.Timer = _thiefplayer01TakeDeliverJewelButton.MaxTimer;
        }, () => { return Thiefplayer01 != null && Thiefplayer01 == PlayerControl.LocalPlayer; }, () =>
        {
            if (Thiefplayer01IsStealing)
                _thiefplayer01TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefDeliverJewelButton;
            else
                _thiefplayer01TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefTakeJewelButton;
            var canUse = false;
            if (ThiefTreasures.Count != 0)
            {
                foreach (var jewel in ThiefTreasures)
                {
                    if (jewel != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, jewel.transform.position) < 0.5f && !Thiefplayer01IsStealing)
                    {
                        switch (jewel.name)
                        {
                            case "jewel01":
                                Thiefplayer01JewelId = 1;
                                canUse = !Jewel01BeingStealed;
                                break;
                            case "jewel02":
                                Thiefplayer01JewelId = 2;
                                canUse = !Jewel02BeingStealed;
                                break;
                            case "jewel03":
                                Thiefplayer01JewelId = 3;
                                canUse = !Jewel03BeingStealed;
                                break;
                            case "jewel04":
                                Thiefplayer01JewelId = 4;
                                canUse = !Jewel04BeingStealed;
                                break;
                            case "jewel05":
                                Thiefplayer01JewelId = 5;
                                canUse = !Jewel05BeingStealed;
                                break;
                            case "jewel06":
                                Thiefplayer01JewelId = 6;
                                canUse = !Jewel06BeingStealed;
                                break;
                            case "jewel07":
                                Thiefplayer01JewelId = 7;
                                canUse = !Jewel07BeingStealed;
                                break;
                            case "jewel08":
                                Thiefplayer01JewelId = 8;
                                canUse = !Jewel08BeingStealed;
                                break;
                            case "jewel09":
                                Thiefplayer01JewelId = 9;
                                canUse = !Jewel09BeingStealed;
                                break;
                            case "jewel10":
                                Thiefplayer01JewelId = 10;
                                canUse = !Jewel10BeingStealed;
                                break;
                            case "jewel11":
                                Thiefplayer01JewelId = 11;
                                canUse = !Jewel11BeingStealed;
                                break;
                            case "jewel12":
                                Thiefplayer01JewelId = 12;
                                canUse = !Jewel12BeingStealed;
                                break;
                            case "jewel13":
                                Thiefplayer01JewelId = 13;
                                canUse = !Jewel13BeingStealed;
                                break;
                            case "jewel14":
                                Thiefplayer01JewelId = 14;
                                canUse = !Jewel14BeingStealed;
                                break;
                            case "jewel15":
                                Thiefplayer01JewelId = 15;
                                canUse = !Jewel15BeingStealed;
                                break;
                        }
                    }
                    else if (((Jewelbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Jewelbuttontwo.transform.position) < 0.5f) || (Jewelbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Jewelbutton.transform.position) < 0.5f)) && Thiefplayer01IsStealing) canUse = true;
                }
            }

            return canUse && PlayerControl.LocalPlayer.CanMove && !Thiefplayer01IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _thiefplayer01TakeDeliverJewelButton.Timer = _thiefplayer01TakeDeliverJewelButton.MaxTimer; }, AssetLoader.PoliceAndThiefTakeJewelButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CommonAbilityPrimary, false, Tr.Get(TrKey.DeliverButton));

        // Thiefplayer02 Kill
        _thiefplayer02KillButton = new(() =>
        {
            var targetId = Thiefplayer02CurrentTarget.PlayerId;
            var sourceId = Thiefplayer02.PlayerId;
            var killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            killWriter.Write(targetId);
            killWriter.Write(sourceId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.GamemodeKills(targetId, sourceId);
            _thiefplayer02KillButton.Timer = _thiefplayer02KillButton.MaxTimer;
            Thiefplayer02CurrentTarget = null;
        }, () => { return Thiefplayer02 != null && Thiefplayer02 == PlayerControl.LocalPlayer && WhoCanThiefsKill != 2; }, () =>
        {
            var canUse = false;
            switch (WhoCanThiefsKill)
            {
                case 0:
                    if ((Policeplayer02 != null && Thiefplayer02CurrentTarget == Policeplayer02) || (Policeplayer04 != null && Thiefplayer02CurrentTarget == Policeplayer04)) canUse = true;
                    break;
                case 1:
                    canUse = true;
                    break;
            }

            return canUse && Thiefplayer02CurrentTarget && PlayerControl.LocalPlayer.CanMove && !Thiefplayer02IsReviving && !PlayerControl.LocalPlayer.Data.IsDead && !Thiefplayer02IsStealing;
        }, () => { _thiefplayer02KillButton.Timer = _thiefplayer02KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, AbilitySlot.CrewmateAbilityPrimary, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // Thiefplayer02 FreeThief Button
        _thiefplayer02FreeThiefButton = new(() =>
        {
            var thiefFree = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefFreeThief, SendOption.Reliable);
            AmongUsClient.Instance.FinishRpcImmediately(thiefFree);
            RPCProcedure.PoliceandThiefFreeThief();
            _thiefplayer02FreeThiefButton.Timer = _thiefplayer02FreeThiefButton.MaxTimer;
        }, () => { return Thiefplayer02 != null && Thiefplayer02 == PlayerControl.LocalPlayer; }, () =>
        {
            if (LocalThiefReleaseArrow.Count != 0)
            {
                LocalThiefReleaseArrow[0].Update(Cellbutton.transform.position);
                if (GameOptionsManager.Instance.currentGameOptions.MapId == 6) LocalThiefReleaseArrow[1].Update(Cellbuttontwo.transform.position);
            }

            if (LocalThiefDeliverArrow.Count != 0)
            {
                LocalThiefDeliverArrow[0].Update(Jewelbutton.transform.position);
                if (GameOptionsManager.Instance.currentGameOptions.MapId == 6) LocalThiefDeliverArrow[1].Update(Jewelbuttontwo.transform.position);
            }

            var canUse = false;
            if (CurrentThiefsCaptured > 0)
            {
                if (((Cellbuttontwo != null && Vector2.Distance(Thiefplayer02.transform.position, Cellbuttontwo.transform.position) < 0.4f) || Vector2.Distance(Thiefplayer02.transform.position, Cellbutton.transform.position) < 0.4f) && !Thiefplayer02.Data.IsDead)
                    canUse = true;
            }

            return canUse && PlayerControl.LocalPlayer.CanMove && !Thiefplayer02IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _thiefplayer02FreeThiefButton.Timer = _thiefplayer02FreeThiefButton.MaxTimer; }, AssetLoader.PoliceAndThiefFreeButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CrewmateAbilitySecondary, false, Tr.Get(TrKey.ReleaseButton));

        // Thiefplayer02 Take/Deliver Jewel Button
        _thiefplayer02TakeDeliverJewelButton = new(() =>
        {
            if (Thiefplayer02IsStealing)
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var jewelId = Thiefplayer02JewelId;
                var thiefScore = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefDeliverJewel, SendOption.Reliable);
                thiefScore.Write(targetId);
                thiefScore.Write(jewelId);
                AmongUsClient.Instance.FinishRpcImmediately(thiefScore);
                RPCProcedure.PoliceandThiefDeliverJewel(targetId, jewelId);
            }
            else
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var jewelId = Thiefplayer02JewelId;
                var thiefWhoTookATreasure = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefTakeJewel, SendOption.Reliable);
                thiefWhoTookATreasure.Write(targetId);
                thiefWhoTookATreasure.Write(jewelId);
                AmongUsClient.Instance.FinishRpcImmediately(thiefWhoTookATreasure);
                RPCProcedure.PoliceandThiefTakeJewel(targetId, jewelId);
            }

            _thiefplayer02TakeDeliverJewelButton.Timer = _thiefplayer02TakeDeliverJewelButton.MaxTimer;
        }, () => { return Thiefplayer02 != null && Thiefplayer02 == PlayerControl.LocalPlayer; }, () =>
        {
            if (Thiefplayer02IsStealing)
                _thiefplayer02TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefDeliverJewelButton;
            else
                _thiefplayer02TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefTakeJewelButton;
            var canUse = false;
            if (ThiefTreasures.Count != 0)
            {
                foreach (var jewel in ThiefTreasures)
                {
                    if (jewel != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, jewel.transform.position) < 0.5f && !Thiefplayer02IsStealing)
                    {
                        switch (jewel.name)
                        {
                            case "jewel01":
                                Thiefplayer02JewelId = 1;
                                canUse = !Jewel01BeingStealed;
                                break;
                            case "jewel02":
                                Thiefplayer02JewelId = 2;
                                canUse = !Jewel02BeingStealed;
                                break;
                            case "jewel03":
                                Thiefplayer02JewelId = 3;
                                canUse = !Jewel03BeingStealed;
                                break;
                            case "jewel04":
                                Thiefplayer02JewelId = 4;
                                canUse = !Jewel04BeingStealed;
                                break;
                            case "jewel05":
                                Thiefplayer02JewelId = 5;
                                canUse = !Jewel05BeingStealed;
                                break;
                            case "jewel06":
                                Thiefplayer02JewelId = 6;
                                canUse = !Jewel06BeingStealed;
                                break;
                            case "jewel07":
                                Thiefplayer02JewelId = 7;
                                canUse = !Jewel07BeingStealed;
                                break;
                            case "jewel08":
                                Thiefplayer02JewelId = 8;
                                canUse = !Jewel08BeingStealed;
                                break;
                            case "jewel09":
                                Thiefplayer02JewelId = 9;
                                canUse = !Jewel09BeingStealed;
                                break;
                            case "jewel10":
                                Thiefplayer02JewelId = 10;
                                canUse = !Jewel10BeingStealed;
                                break;
                            case "jewel11":
                                Thiefplayer02JewelId = 11;
                                canUse = !Jewel11BeingStealed;
                                break;
                            case "jewel12":
                                Thiefplayer02JewelId = 12;
                                canUse = !Jewel12BeingStealed;
                                break;
                            case "jewel13":
                                Thiefplayer02JewelId = 13;
                                canUse = !Jewel13BeingStealed;
                                break;
                            case "jewel14":
                                Thiefplayer02JewelId = 14;
                                canUse = !Jewel14BeingStealed;
                                break;
                            case "jewel15":
                                Thiefplayer02JewelId = 15;
                                canUse = !Jewel15BeingStealed;
                                break;
                        }
                    }
                    else if (((Jewelbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Jewelbuttontwo.transform.position) < 0.5f) || (Jewelbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Jewelbutton.transform.position) < 0.5f)) && Thiefplayer02IsStealing) canUse = true;
                }
            }

            return canUse && PlayerControl.LocalPlayer.CanMove && !Thiefplayer02IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _thiefplayer02TakeDeliverJewelButton.Timer = _thiefplayer02TakeDeliverJewelButton.MaxTimer; }, AssetLoader.PoliceAndThiefTakeJewelButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CommonAbilityPrimary, false, Tr.Get(TrKey.DeliverButton));

        // Thiefplayer03 Kill
        _thiefplayer03KillButton = new(() =>
        {
            var targetId = Thiefplayer03CurrentTarget.PlayerId;
            var sourceId = Thiefplayer03.PlayerId;
            var killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            killWriter.Write(targetId);
            killWriter.Write(sourceId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.GamemodeKills(targetId, sourceId);
            _thiefplayer03KillButton.Timer = _thiefplayer03KillButton.MaxTimer;
            Thiefplayer03CurrentTarget = null;
        }, () => { return Thiefplayer03 != null && Thiefplayer03 == PlayerControl.LocalPlayer && WhoCanThiefsKill != 2; }, () =>
        {
            var canUse = false;
            switch (WhoCanThiefsKill)
            {
                case 0:
                    if ((Policeplayer02 != null && Thiefplayer03CurrentTarget == Policeplayer02) || (Policeplayer04 != null && Thiefplayer03CurrentTarget == Policeplayer04)) canUse = true;
                    break;
                case 1:
                    canUse = true;
                    break;
            }

            return canUse && Thiefplayer03CurrentTarget && PlayerControl.LocalPlayer.CanMove && !Thiefplayer03IsReviving && !PlayerControl.LocalPlayer.Data.IsDead && !Thiefplayer03IsStealing;
        }, () => { _thiefplayer03KillButton.Timer = _thiefplayer03KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, AbilitySlot.CrewmateAbilityPrimary, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // Thiefplayer03 FreeThief Button
        _thiefplayer03FreeThiefButton = new(() =>
        {
            var thiefFree = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefFreeThief, SendOption.Reliable);
            AmongUsClient.Instance.FinishRpcImmediately(thiefFree);
            RPCProcedure.PoliceandThiefFreeThief();
            _thiefplayer03FreeThiefButton.Timer = _thiefplayer03FreeThiefButton.MaxTimer;
        }, () => { return Thiefplayer03 != null && Thiefplayer03 == PlayerControl.LocalPlayer; }, () =>
        {
            if (LocalThiefReleaseArrow.Count != 0)
            {
                LocalThiefReleaseArrow[0].Update(Cellbutton.transform.position);
                if (GameOptionsManager.Instance.currentGameOptions.MapId == 6) LocalThiefReleaseArrow[1].Update(Cellbuttontwo.transform.position);
            }

            if (LocalThiefDeliverArrow.Count != 0)
            {
                LocalThiefDeliverArrow[0].Update(Jewelbutton.transform.position);
                if (GameOptionsManager.Instance.currentGameOptions.MapId == 6) LocalThiefDeliverArrow[1].Update(Jewelbuttontwo.transform.position);
            }

            var canUse = false;
            if (CurrentThiefsCaptured > 0)
            {
                if (((Cellbuttontwo != null && Vector2.Distance(Thiefplayer03.transform.position, Cellbuttontwo.transform.position) < 0.4f) || Vector2.Distance(Thiefplayer03.transform.position, Cellbutton.transform.position) < 0.4f) && !Thiefplayer03.Data.IsDead)
                    canUse = true;
            }

            return canUse && PlayerControl.LocalPlayer.CanMove && !Thiefplayer03IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _thiefplayer03FreeThiefButton.Timer = _thiefplayer03FreeThiefButton.MaxTimer; }, AssetLoader.PoliceAndThiefFreeButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CrewmateAbilitySecondary, false, Tr.Get(TrKey.ReleaseButton));

        // Thiefplayer03 Take/Deliver Jewel Button
        _thiefplayer03TakeDeliverJewelButton = new(() =>
        {
            if (Thiefplayer03IsStealing)
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var jewelId = Thiefplayer03JewelId;
                var thiefScore = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefDeliverJewel, SendOption.Reliable);
                thiefScore.Write(targetId);
                thiefScore.Write(jewelId);
                AmongUsClient.Instance.FinishRpcImmediately(thiefScore);
                RPCProcedure.PoliceandThiefDeliverJewel(targetId, jewelId);
            }
            else
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var jewelId = Thiefplayer03JewelId;
                var thiefWhoTookATreasure = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefTakeJewel, SendOption.Reliable);
                thiefWhoTookATreasure.Write(targetId);
                thiefWhoTookATreasure.Write(jewelId);
                AmongUsClient.Instance.FinishRpcImmediately(thiefWhoTookATreasure);
                RPCProcedure.PoliceandThiefTakeJewel(targetId, jewelId);
            }

            _thiefplayer03TakeDeliverJewelButton.Timer = _thiefplayer03TakeDeliverJewelButton.MaxTimer;
        }, () => { return Thiefplayer03 != null && Thiefplayer03 == PlayerControl.LocalPlayer; }, () =>
        {
            if (Thiefplayer03IsStealing)
                _thiefplayer03TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefDeliverJewelButton;
            else
                _thiefplayer03TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefTakeJewelButton;
            var canUse = false;
            if (ThiefTreasures.Count != 0)
            {
                foreach (var jewel in ThiefTreasures)
                {
                    if (jewel != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, jewel.transform.position) < 0.5f && !Thiefplayer03IsStealing)
                    {
                        switch (jewel.name)
                        {
                            case "jewel01":
                                Thiefplayer03JewelId = 1;
                                canUse = !Jewel01BeingStealed;
                                break;
                            case "jewel02":
                                Thiefplayer03JewelId = 2;
                                canUse = !Jewel02BeingStealed;
                                break;
                            case "jewel03":
                                Thiefplayer03JewelId = 3;
                                canUse = !Jewel03BeingStealed;
                                break;
                            case "jewel04":
                                Thiefplayer03JewelId = 4;
                                canUse = !Jewel04BeingStealed;
                                break;
                            case "jewel05":
                                Thiefplayer03JewelId = 5;
                                canUse = !Jewel05BeingStealed;
                                break;
                            case "jewel06":
                                Thiefplayer03JewelId = 6;
                                canUse = !Jewel06BeingStealed;
                                break;
                            case "jewel07":
                                Thiefplayer03JewelId = 7;
                                canUse = !Jewel07BeingStealed;
                                break;
                            case "jewel08":
                                Thiefplayer03JewelId = 8;
                                canUse = !Jewel08BeingStealed;
                                break;
                            case "jewel09":
                                Thiefplayer03JewelId = 9;
                                canUse = !Jewel09BeingStealed;
                                break;
                            case "jewel10":
                                Thiefplayer03JewelId = 10;
                                canUse = !Jewel10BeingStealed;
                                break;
                            case "jewel11":
                                Thiefplayer03JewelId = 11;
                                canUse = !Jewel11BeingStealed;
                                break;
                            case "jewel12":
                                Thiefplayer03JewelId = 12;
                                canUse = !Jewel12BeingStealed;
                                break;
                            case "jewel13":
                                Thiefplayer03JewelId = 13;
                                canUse = !Jewel13BeingStealed;
                                break;
                            case "jewel14":
                                Thiefplayer03JewelId = 14;
                                canUse = !Jewel14BeingStealed;
                                break;
                            case "jewel15":
                                Thiefplayer03JewelId = 15;
                                canUse = !Jewel15BeingStealed;
                                break;
                        }
                    }
                    else if (((Jewelbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Jewelbuttontwo.transform.position) < 0.5f) || (Jewelbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Jewelbutton.transform.position) < 0.5f)) && Thiefplayer03IsStealing) canUse = true;
                }
            }

            return canUse && PlayerControl.LocalPlayer.CanMove && !Thiefplayer03IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _thiefplayer03TakeDeliverJewelButton.Timer = _thiefplayer03TakeDeliverJewelButton.MaxTimer; }, AssetLoader.PoliceAndThiefTakeJewelButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CommonAbilityPrimary, false, Tr.Get(TrKey.DeliverButton));

        // Thiefplayer04 Kill
        _thiefplayer04KillButton = new(() =>
        {
            var targetId = Thiefplayer04CurrentTarget.PlayerId;
            var sourceId = Thiefplayer04.PlayerId;
            var killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            killWriter.Write(targetId);
            killWriter.Write(sourceId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.GamemodeKills(targetId, sourceId);
            _thiefplayer04KillButton.Timer = _thiefplayer04KillButton.MaxTimer;
            Thiefplayer04CurrentTarget = null;
        }, () => { return Thiefplayer04 != null && Thiefplayer04 == PlayerControl.LocalPlayer && WhoCanThiefsKill != 2; }, () =>
        {
            var canUse = false;
            switch (WhoCanThiefsKill)
            {
                case 0:
                    if ((Policeplayer02 != null && Thiefplayer04CurrentTarget == Policeplayer02) || (Policeplayer04 != null && Thiefplayer04CurrentTarget == Policeplayer04)) canUse = true;
                    break;
                case 1:
                    canUse = true;
                    break;
            }

            return canUse && Thiefplayer04CurrentTarget && PlayerControl.LocalPlayer.CanMove && !Thiefplayer04IsReviving && !PlayerControl.LocalPlayer.Data.IsDead && !Thiefplayer04IsStealing;
        }, () => { _thiefplayer04KillButton.Timer = _thiefplayer04KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, AbilitySlot.CrewmateAbilityPrimary, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // Thiefplayer04 FreeThief Button
        _thiefplayer04FreeThiefButton = new(() =>
        {
            var thiefFree = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefFreeThief, SendOption.Reliable);
            AmongUsClient.Instance.FinishRpcImmediately(thiefFree);
            RPCProcedure.PoliceandThiefFreeThief();
            _thiefplayer04FreeThiefButton.Timer = _thiefplayer04FreeThiefButton.MaxTimer;
        }, () => { return Thiefplayer04 != null && Thiefplayer04 == PlayerControl.LocalPlayer; }, () =>
        {
            if (LocalThiefReleaseArrow.Count != 0)
            {
                LocalThiefReleaseArrow[0].Update(Cellbutton.transform.position);
                if (GameOptionsManager.Instance.currentGameOptions.MapId == 6) LocalThiefReleaseArrow[1].Update(Cellbuttontwo.transform.position);
            }

            if (LocalThiefDeliverArrow.Count != 0)
            {
                LocalThiefDeliverArrow[0].Update(Jewelbutton.transform.position);
                if (GameOptionsManager.Instance.currentGameOptions.MapId == 6) LocalThiefDeliverArrow[1].Update(Jewelbuttontwo.transform.position);
            }

            var canUse = false;
            if (CurrentThiefsCaptured > 0)
            {
                if (((Cellbuttontwo != null && Vector2.Distance(Thiefplayer04.transform.position, Cellbuttontwo.transform.position) < 0.4f) || Vector2.Distance(Thiefplayer04.transform.position, Cellbutton.transform.position) < 0.4f) && !Thiefplayer04.Data.IsDead)
                    canUse = true;
            }

            return canUse && PlayerControl.LocalPlayer.CanMove && !Thiefplayer04IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _thiefplayer04FreeThiefButton.Timer = _thiefplayer04FreeThiefButton.MaxTimer; }, AssetLoader.PoliceAndThiefFreeButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CrewmateAbilitySecondary, false, Tr.Get(TrKey.ReleaseButton));

        // Thiefplayer04 Take/Deliver Jewel Button
        _thiefplayer04TakeDeliverJewelButton = new(() =>
        {
            if (Thiefplayer04IsStealing)
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var jewelId = Thiefplayer04JewelId;
                var thiefScore = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefDeliverJewel, SendOption.Reliable);
                thiefScore.Write(targetId);
                thiefScore.Write(jewelId);
                AmongUsClient.Instance.FinishRpcImmediately(thiefScore);
                RPCProcedure.PoliceandThiefDeliverJewel(targetId, jewelId);
            }
            else
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var jewelId = Thiefplayer04JewelId;
                var thiefWhoTookATreasure = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefTakeJewel, SendOption.Reliable);
                thiefWhoTookATreasure.Write(targetId);
                thiefWhoTookATreasure.Write(jewelId);
                AmongUsClient.Instance.FinishRpcImmediately(thiefWhoTookATreasure);
                RPCProcedure.PoliceandThiefTakeJewel(targetId, jewelId);
            }

            _thiefplayer04TakeDeliverJewelButton.Timer = _thiefplayer04TakeDeliverJewelButton.MaxTimer;
        }, () => { return Thiefplayer04 != null && Thiefplayer04 == PlayerControl.LocalPlayer; }, () =>
        {
            if (Thiefplayer04IsStealing)
                _thiefplayer04TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefDeliverJewelButton;
            else
                _thiefplayer04TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefTakeJewelButton;
            var canUse = false;
            if (ThiefTreasures.Count != 0)
            {
                foreach (var jewel in ThiefTreasures)
                {
                    if (jewel != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, jewel.transform.position) < 0.5f && !Thiefplayer04IsStealing)
                    {
                        switch (jewel.name)
                        {
                            case "jewel01":
                                Thiefplayer04JewelId = 1;
                                canUse = !Jewel01BeingStealed;
                                break;
                            case "jewel02":
                                Thiefplayer04JewelId = 2;
                                canUse = !Jewel02BeingStealed;
                                break;
                            case "jewel03":
                                Thiefplayer04JewelId = 3;
                                canUse = !Jewel03BeingStealed;
                                break;
                            case "jewel04":
                                Thiefplayer04JewelId = 4;
                                canUse = !Jewel04BeingStealed;
                                break;
                            case "jewel05":
                                Thiefplayer04JewelId = 5;
                                canUse = !Jewel05BeingStealed;
                                break;
                            case "jewel06":
                                Thiefplayer04JewelId = 6;
                                canUse = !Jewel06BeingStealed;
                                break;
                            case "jewel07":
                                Thiefplayer04JewelId = 7;
                                canUse = !Jewel07BeingStealed;
                                break;
                            case "jewel08":
                                Thiefplayer04JewelId = 8;
                                canUse = !Jewel08BeingStealed;
                                break;
                            case "jewel09":
                                Thiefplayer04JewelId = 9;
                                canUse = !Jewel09BeingStealed;
                                break;
                            case "jewel10":
                                Thiefplayer04JewelId = 10;
                                canUse = !Jewel10BeingStealed;
                                break;
                            case "jewel11":
                                Thiefplayer04JewelId = 11;
                                canUse = !Jewel11BeingStealed;
                                break;
                            case "jewel12":
                                Thiefplayer04JewelId = 12;
                                canUse = !Jewel12BeingStealed;
                                break;
                            case "jewel13":
                                Thiefplayer04JewelId = 13;
                                canUse = !Jewel13BeingStealed;
                                break;
                            case "jewel14":
                                Thiefplayer04JewelId = 14;
                                canUse = !Jewel14BeingStealed;
                                break;
                            case "jewel15":
                                Thiefplayer04JewelId = 15;
                                canUse = !Jewel15BeingStealed;
                                break;
                        }
                    }
                    else if (((Jewelbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Jewelbuttontwo.transform.position) < 0.5f) || (Jewelbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Jewelbutton.transform.position) < 0.5f)) && Thiefplayer04IsStealing) canUse = true;
                }
            }

            return canUse && PlayerControl.LocalPlayer.CanMove && !Thiefplayer04IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _thiefplayer04TakeDeliverJewelButton.Timer = _thiefplayer04TakeDeliverJewelButton.MaxTimer; }, AssetLoader.PoliceAndThiefTakeJewelButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CommonAbilityPrimary, false, Tr.Get(TrKey.DeliverButton));

        // Thiefplayer05 Kill
        _thiefplayer05KillButton = new(() =>
        {
            var targetId = Thiefplayer05CurrentTarget.PlayerId;
            var sourceId = Thiefplayer05.PlayerId;
            var killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            killWriter.Write(targetId);
            killWriter.Write(sourceId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.GamemodeKills(targetId, sourceId);
            _thiefplayer05KillButton.Timer = _thiefplayer05KillButton.MaxTimer;
            Thiefplayer05CurrentTarget = null;
        }, () => { return Thiefplayer05 != null && Thiefplayer05 == PlayerControl.LocalPlayer && WhoCanThiefsKill != 2; }, () =>
        {
            var canUse = false;
            switch (WhoCanThiefsKill)
            {
                case 0:
                    if ((Policeplayer02 != null && Thiefplayer05CurrentTarget == Policeplayer02) || (Policeplayer04 != null && Thiefplayer05CurrentTarget == Policeplayer04)) canUse = true;
                    break;
                case 1:
                    canUse = true;
                    break;
            }

            return canUse && Thiefplayer05CurrentTarget && PlayerControl.LocalPlayer.CanMove && !Thiefplayer05IsReviving && !PlayerControl.LocalPlayer.Data.IsDead && !Thiefplayer05IsStealing;
        }, () => { _thiefplayer05KillButton.Timer = _thiefplayer05KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, AbilitySlot.CrewmateAbilityPrimary, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // Thiefplayer05 FreeThief Button
        _thiefplayer05FreeThiefButton = new(() =>
        {
            var thiefFree = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefFreeThief, SendOption.Reliable);
            AmongUsClient.Instance.FinishRpcImmediately(thiefFree);
            RPCProcedure.PoliceandThiefFreeThief();
            _thiefplayer05FreeThiefButton.Timer = _thiefplayer05FreeThiefButton.MaxTimer;
        }, () => { return Thiefplayer05 != null && Thiefplayer05 == PlayerControl.LocalPlayer; }, () =>
        {
            if (LocalThiefReleaseArrow.Count != 0)
            {
                LocalThiefReleaseArrow[0].Update(Cellbutton.transform.position);
                if (GameOptionsManager.Instance.currentGameOptions.MapId == 6) LocalThiefReleaseArrow[1].Update(Cellbuttontwo.transform.position);
            }

            if (LocalThiefDeliverArrow.Count != 0)
            {
                LocalThiefDeliverArrow[0].Update(Jewelbutton.transform.position);
                if (GameOptionsManager.Instance.currentGameOptions.MapId == 6) LocalThiefDeliverArrow[1].Update(Jewelbuttontwo.transform.position);
            }

            var canUse = false;
            if (CurrentThiefsCaptured > 0)
            {
                if (((Cellbuttontwo != null && Vector2.Distance(Thiefplayer05.transform.position, Cellbuttontwo.transform.position) < 0.4f) || Vector2.Distance(Thiefplayer05.transform.position, Cellbutton.transform.position) < 0.4f) && !Thiefplayer05.Data.IsDead)
                    canUse = true;
            }

            return canUse && PlayerControl.LocalPlayer.CanMove && !Thiefplayer05IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _thiefplayer05FreeThiefButton.Timer = _thiefplayer05FreeThiefButton.MaxTimer; }, AssetLoader.PoliceAndThiefFreeButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CrewmateAbilitySecondary, false, Tr.Get(TrKey.ReleaseButton));

        // Thiefplayer05 Take/Deliver Jewel Button
        _thiefplayer05TakeDeliverJewelButton = new(() =>
        {
            if (Thiefplayer05IsStealing)
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var jewelId = Thiefplayer05JewelId;
                var thiefScore = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefDeliverJewel, SendOption.Reliable);
                thiefScore.Write(targetId);
                thiefScore.Write(jewelId);
                AmongUsClient.Instance.FinishRpcImmediately(thiefScore);
                RPCProcedure.PoliceandThiefDeliverJewel(targetId, jewelId);
            }
            else
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var jewelId = Thiefplayer05JewelId;
                var thiefWhoTookATreasure = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefTakeJewel, SendOption.Reliable);
                thiefWhoTookATreasure.Write(targetId);
                thiefWhoTookATreasure.Write(jewelId);
                AmongUsClient.Instance.FinishRpcImmediately(thiefWhoTookATreasure);
                RPCProcedure.PoliceandThiefTakeJewel(targetId, jewelId);
            }

            _thiefplayer05TakeDeliverJewelButton.Timer = _thiefplayer05TakeDeliverJewelButton.MaxTimer;
        }, () => { return Thiefplayer05 != null && Thiefplayer05 == PlayerControl.LocalPlayer; }, () =>
        {
            if (Thiefplayer05IsStealing)
                _thiefplayer05TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefDeliverJewelButton;
            else
                _thiefplayer05TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefTakeJewelButton;
            var canUse = false;
            if (ThiefTreasures.Count != 0)
            {
                foreach (var jewel in ThiefTreasures)
                {
                    if (jewel != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, jewel.transform.position) < 0.5f && !Thiefplayer05IsStealing)
                    {
                        switch (jewel.name)
                        {
                            case "jewel01":
                                Thiefplayer05JewelId = 1;
                                canUse = !Jewel01BeingStealed;
                                break;
                            case "jewel02":
                                Thiefplayer05JewelId = 2;
                                canUse = !Jewel02BeingStealed;
                                break;
                            case "jewel03":
                                Thiefplayer05JewelId = 3;
                                canUse = !Jewel03BeingStealed;
                                break;
                            case "jewel04":
                                Thiefplayer05JewelId = 4;
                                canUse = !Jewel04BeingStealed;
                                break;
                            case "jewel05":
                                Thiefplayer05JewelId = 5;
                                canUse = !Jewel05BeingStealed;
                                break;
                            case "jewel06":
                                Thiefplayer05JewelId = 6;
                                canUse = !Jewel06BeingStealed;
                                break;
                            case "jewel07":
                                Thiefplayer05JewelId = 7;
                                canUse = !Jewel07BeingStealed;
                                break;
                            case "jewel08":
                                Thiefplayer05JewelId = 8;
                                canUse = !Jewel08BeingStealed;
                                break;
                            case "jewel09":
                                Thiefplayer05JewelId = 9;
                                canUse = !Jewel09BeingStealed;
                                break;
                            case "jewel10":
                                Thiefplayer05JewelId = 10;
                                canUse = !Jewel10BeingStealed;
                                break;
                            case "jewel11":
                                Thiefplayer05JewelId = 11;
                                canUse = !Jewel11BeingStealed;
                                break;
                            case "jewel12":
                                Thiefplayer05JewelId = 12;
                                canUse = !Jewel12BeingStealed;
                                break;
                            case "jewel13":
                                Thiefplayer05JewelId = 13;
                                canUse = !Jewel13BeingStealed;
                                break;
                            case "jewel14":
                                Thiefplayer05JewelId = 14;
                                canUse = !Jewel14BeingStealed;
                                break;
                            case "jewel15":
                                Thiefplayer05JewelId = 15;
                                canUse = !Jewel15BeingStealed;
                                break;
                        }
                    }
                    else if (((Jewelbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Jewelbuttontwo.transform.position) < 0.5f) || (Jewelbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Jewelbutton.transform.position) < 0.5f)) && Thiefplayer05IsStealing) canUse = true;
                }
            }

            return canUse && PlayerControl.LocalPlayer.CanMove && !Thiefplayer05IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _thiefplayer05TakeDeliverJewelButton.Timer = _thiefplayer05TakeDeliverJewelButton.MaxTimer; }, AssetLoader.PoliceAndThiefTakeJewelButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CommonAbilityPrimary, false, Tr.Get(TrKey.DeliverButton));

        // Thiefplayer06 Kill
        _thiefplayer06KillButton = new(() =>
        {
            var targetId = Thiefplayer06CurrentTarget.PlayerId;
            var sourceId = Thiefplayer06.PlayerId;
            var killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            killWriter.Write(targetId);
            killWriter.Write(sourceId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.GamemodeKills(targetId, sourceId);
            _thiefplayer06KillButton.Timer = _thiefplayer06KillButton.MaxTimer;
            Thiefplayer06CurrentTarget = null;
        }, () => { return Thiefplayer06 != null && Thiefplayer06 == PlayerControl.LocalPlayer && WhoCanThiefsKill != 2; }, () =>
        {
            var canUse = false;
            switch (WhoCanThiefsKill)
            {
                case 0:
                    if ((Policeplayer02 != null && Thiefplayer06CurrentTarget == Policeplayer02) || (Policeplayer04 != null && Thiefplayer06CurrentTarget == Policeplayer04)) canUse = true;
                    break;
                case 1:
                    canUse = true;
                    break;
            }

            return canUse && Thiefplayer06CurrentTarget && PlayerControl.LocalPlayer.CanMove && !Thiefplayer06IsReviving && !PlayerControl.LocalPlayer.Data.IsDead && !Thiefplayer06IsStealing;
        }, () => { _thiefplayer06KillButton.Timer = _thiefplayer06KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, AbilitySlot.CrewmateAbilityPrimary, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // Thiefplayer06 FreeThief Button
        _thiefplayer06FreeThiefButton = new(() =>
        {
            var thiefFree = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefFreeThief, SendOption.Reliable);
            AmongUsClient.Instance.FinishRpcImmediately(thiefFree);
            RPCProcedure.PoliceandThiefFreeThief();
            _thiefplayer06FreeThiefButton.Timer = _thiefplayer06FreeThiefButton.MaxTimer;
        }, () => { return Thiefplayer06 != null && Thiefplayer06 == PlayerControl.LocalPlayer; }, () =>
        {
            if (LocalThiefReleaseArrow.Count != 0)
            {
                LocalThiefReleaseArrow[0].Update(Cellbutton.transform.position);
                if (GameOptionsManager.Instance.currentGameOptions.MapId == 6) LocalThiefReleaseArrow[1].Update(Cellbuttontwo.transform.position);
            }

            if (LocalThiefDeliverArrow.Count != 0)
            {
                LocalThiefDeliverArrow[0].Update(Jewelbutton.transform.position);
                if (GameOptionsManager.Instance.currentGameOptions.MapId == 6) LocalThiefDeliverArrow[1].Update(Jewelbuttontwo.transform.position);
            }

            var canUse = false;
            if (CurrentThiefsCaptured > 0)
            {
                if (((Cellbuttontwo != null && Vector2.Distance(Thiefplayer06.transform.position, Cellbuttontwo.transform.position) < 0.4f) || Vector2.Distance(Thiefplayer06.transform.position, Cellbutton.transform.position) < 0.4f) && !Thiefplayer06.Data.IsDead)
                    canUse = true;
            }

            return canUse && PlayerControl.LocalPlayer.CanMove && !Thiefplayer06IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _thiefplayer06FreeThiefButton.Timer = _thiefplayer06FreeThiefButton.MaxTimer; }, AssetLoader.PoliceAndThiefFreeButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CrewmateAbilitySecondary, false, Tr.Get(TrKey.ReleaseButton));

        // Thiefplayer06 Take/Deliver Jewel Button
        _thiefplayer06TakeDeliverJewelButton = new(() =>
        {
            if (Thiefplayer06IsStealing)
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var jewelId = Thiefplayer06JewelId;
                var thiefScore = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefDeliverJewel, SendOption.Reliable);
                thiefScore.Write(targetId);
                thiefScore.Write(jewelId);
                AmongUsClient.Instance.FinishRpcImmediately(thiefScore);
                RPCProcedure.PoliceandThiefDeliverJewel(targetId, jewelId);
            }
            else
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var jewelId = Thiefplayer06JewelId;
                var thiefWhoTookATreasure = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefTakeJewel, SendOption.Reliable);
                thiefWhoTookATreasure.Write(targetId);
                thiefWhoTookATreasure.Write(jewelId);
                AmongUsClient.Instance.FinishRpcImmediately(thiefWhoTookATreasure);
                RPCProcedure.PoliceandThiefTakeJewel(targetId, jewelId);
            }

            _thiefplayer06TakeDeliverJewelButton.Timer = _thiefplayer06TakeDeliverJewelButton.MaxTimer;
        }, () => { return Thiefplayer06 != null && Thiefplayer06 == PlayerControl.LocalPlayer; }, () =>
        {
            if (Thiefplayer06IsStealing)
                _thiefplayer06TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefDeliverJewelButton;
            else
                _thiefplayer06TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefTakeJewelButton;
            var canUse = false;
            if (ThiefTreasures.Count != 0)
            {
                foreach (var jewel in ThiefTreasures)
                {
                    if (jewel != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, jewel.transform.position) < 0.5f && !Thiefplayer06IsStealing)
                    {
                        switch (jewel.name)
                        {
                            case "jewel01":
                                Thiefplayer06JewelId = 1;
                                canUse = !Jewel01BeingStealed;
                                break;
                            case "jewel02":
                                Thiefplayer06JewelId = 2;
                                canUse = !Jewel02BeingStealed;
                                break;
                            case "jewel03":
                                Thiefplayer06JewelId = 3;
                                canUse = !Jewel03BeingStealed;
                                break;
                            case "jewel04":
                                Thiefplayer06JewelId = 4;
                                canUse = !Jewel04BeingStealed;
                                break;
                            case "jewel05":
                                Thiefplayer06JewelId = 5;
                                canUse = !Jewel05BeingStealed;
                                break;
                            case "jewel06":
                                Thiefplayer06JewelId = 6;
                                canUse = !Jewel06BeingStealed;
                                break;
                            case "jewel07":
                                Thiefplayer06JewelId = 7;
                                canUse = !Jewel07BeingStealed;
                                break;
                            case "jewel08":
                                Thiefplayer06JewelId = 8;
                                canUse = !Jewel08BeingStealed;
                                break;
                            case "jewel09":
                                Thiefplayer06JewelId = 9;
                                canUse = !Jewel09BeingStealed;
                                break;
                            case "jewel10":
                                Thiefplayer06JewelId = 10;
                                canUse = !Jewel10BeingStealed;
                                break;
                            case "jewel11":
                                Thiefplayer06JewelId = 11;
                                canUse = !Jewel11BeingStealed;
                                break;
                            case "jewel12":
                                Thiefplayer06JewelId = 12;
                                canUse = !Jewel12BeingStealed;
                                break;
                            case "jewel13":
                                Thiefplayer06JewelId = 13;
                                canUse = !Jewel13BeingStealed;
                                break;
                            case "jewel14":
                                Thiefplayer06JewelId = 14;
                                canUse = !Jewel14BeingStealed;
                                break;
                            case "jewel15":
                                Thiefplayer06JewelId = 15;
                                canUse = !Jewel15BeingStealed;
                                break;
                        }
                    }
                    else if (((Jewelbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Jewelbuttontwo.transform.position) < 0.5f) || (Jewelbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Jewelbutton.transform.position) < 0.5f)) && Thiefplayer06IsStealing) canUse = true;
                }
            }

            return canUse && PlayerControl.LocalPlayer.CanMove && !Thiefplayer06IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _thiefplayer06TakeDeliverJewelButton.Timer = _thiefplayer06TakeDeliverJewelButton.MaxTimer; }, AssetLoader.PoliceAndThiefTakeJewelButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CommonAbilityPrimary, false, Tr.Get(TrKey.DeliverButton));

        // Thiefplayer07 Kill
        _thiefplayer07KillButton = new(() =>
        {
            var targetId = Thiefplayer07CurrentTarget.PlayerId;
            var sourceId = Thiefplayer07.PlayerId;
            var killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            killWriter.Write(targetId);
            killWriter.Write(sourceId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.GamemodeKills(targetId, sourceId);
            _thiefplayer07KillButton.Timer = _thiefplayer07KillButton.MaxTimer;
            Thiefplayer07CurrentTarget = null;
        }, () => { return Thiefplayer07 != null && Thiefplayer07 == PlayerControl.LocalPlayer && WhoCanThiefsKill != 2; }, () =>
        {
            var canUse = false;
            switch (WhoCanThiefsKill)
            {
                case 0:
                    if ((Policeplayer02 != null && Thiefplayer07CurrentTarget == Policeplayer02) || (Policeplayer04 != null && Thiefplayer07CurrentTarget == Policeplayer04)) canUse = true;
                    break;
                case 1:
                    canUse = true;
                    break;
            }

            return canUse && Thiefplayer07CurrentTarget && PlayerControl.LocalPlayer.CanMove && !Thiefplayer07IsReviving && !PlayerControl.LocalPlayer.Data.IsDead && !Thiefplayer07IsStealing;
        }, () => { _thiefplayer07KillButton.Timer = _thiefplayer07KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, AbilitySlot.CrewmateAbilityPrimary, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // Thiefplayer07 FreeThief Button
        _thiefplayer07FreeThiefButton = new(() =>
        {
            var thiefFree = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefFreeThief, SendOption.Reliable);
            AmongUsClient.Instance.FinishRpcImmediately(thiefFree);
            RPCProcedure.PoliceandThiefFreeThief();
            _thiefplayer07FreeThiefButton.Timer = _thiefplayer07FreeThiefButton.MaxTimer;
        }, () => { return Thiefplayer07 != null && Thiefplayer07 == PlayerControl.LocalPlayer; }, () =>
        {
            if (LocalThiefReleaseArrow.Count != 0)
            {
                LocalThiefReleaseArrow[0].Update(Cellbutton.transform.position);
                if (GameOptionsManager.Instance.currentGameOptions.MapId == 6) LocalThiefReleaseArrow[1].Update(Cellbuttontwo.transform.position);
            }

            if (LocalThiefDeliverArrow.Count != 0)
            {
                LocalThiefDeliverArrow[0].Update(Jewelbutton.transform.position);
                if (GameOptionsManager.Instance.currentGameOptions.MapId == 6) LocalThiefDeliverArrow[1].Update(Jewelbuttontwo.transform.position);
            }

            var canUse = false;
            if (CurrentThiefsCaptured > 0)
            {
                if (((Cellbuttontwo != null && Vector2.Distance(Thiefplayer07.transform.position, Cellbuttontwo.transform.position) < 0.4f) || Vector2.Distance(Thiefplayer07.transform.position, Cellbutton.transform.position) < 0.4f) && !Thiefplayer07.Data.IsDead)
                    canUse = true;
            }

            return canUse && PlayerControl.LocalPlayer.CanMove && !Thiefplayer07IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _thiefplayer07FreeThiefButton.Timer = _thiefplayer07FreeThiefButton.MaxTimer; }, AssetLoader.PoliceAndThiefFreeButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CrewmateAbilitySecondary, false, Tr.Get(TrKey.ReleaseButton));

        // Thiefplayer07 Take/Deliver Jewel Button
        _thiefplayer07TakeDeliverJewelButton = new(() =>
        {
            if (Thiefplayer07IsStealing)
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var jewelId = Thiefplayer07JewelId;
                var thiefScore = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefDeliverJewel, SendOption.Reliable);
                thiefScore.Write(targetId);
                thiefScore.Write(jewelId);
                AmongUsClient.Instance.FinishRpcImmediately(thiefScore);
                RPCProcedure.PoliceandThiefDeliverJewel(targetId, jewelId);
            }
            else
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var jewelId = Thiefplayer07JewelId;
                var thiefWhoTookATreasure = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefTakeJewel, SendOption.Reliable);
                thiefWhoTookATreasure.Write(targetId);
                thiefWhoTookATreasure.Write(jewelId);
                AmongUsClient.Instance.FinishRpcImmediately(thiefWhoTookATreasure);
                RPCProcedure.PoliceandThiefTakeJewel(targetId, jewelId);
            }

            _thiefplayer07TakeDeliverJewelButton.Timer = _thiefplayer07TakeDeliverJewelButton.MaxTimer;
        }, () => { return Thiefplayer07 != null && Thiefplayer07 == PlayerControl.LocalPlayer; }, () =>
        {
            if (Thiefplayer07IsStealing)
                _thiefplayer07TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefDeliverJewelButton;
            else
                _thiefplayer07TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefTakeJewelButton;
            var canUse = false;
            if (ThiefTreasures.Count != 0)
            {
                foreach (var jewel in ThiefTreasures)
                {
                    if (jewel != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, jewel.transform.position) < 0.5f && !Thiefplayer07IsStealing)
                    {
                        switch (jewel.name)
                        {
                            case "jewel01":
                                Thiefplayer07JewelId = 1;
                                canUse = !Jewel01BeingStealed;
                                break;
                            case "jewel02":
                                Thiefplayer07JewelId = 2;
                                canUse = !Jewel02BeingStealed;
                                break;
                            case "jewel03":
                                Thiefplayer07JewelId = 3;
                                canUse = !Jewel03BeingStealed;
                                break;
                            case "jewel04":
                                Thiefplayer07JewelId = 4;
                                canUse = !Jewel04BeingStealed;
                                break;
                            case "jewel05":
                                Thiefplayer07JewelId = 5;
                                canUse = !Jewel05BeingStealed;
                                break;
                            case "jewel06":
                                Thiefplayer07JewelId = 6;
                                canUse = !Jewel06BeingStealed;
                                break;
                            case "jewel07":
                                Thiefplayer07JewelId = 7;
                                canUse = !Jewel07BeingStealed;
                                break;
                            case "jewel08":
                                Thiefplayer07JewelId = 8;
                                canUse = !Jewel08BeingStealed;
                                break;
                            case "jewel09":
                                Thiefplayer07JewelId = 9;
                                canUse = !Jewel09BeingStealed;
                                break;
                            case "jewel10":
                                Thiefplayer07JewelId = 10;
                                canUse = !Jewel10BeingStealed;
                                break;
                            case "jewel11":
                                Thiefplayer07JewelId = 11;
                                canUse = !Jewel11BeingStealed;
                                break;
                            case "jewel12":
                                Thiefplayer07JewelId = 12;
                                canUse = !Jewel12BeingStealed;
                                break;
                            case "jewel13":
                                Thiefplayer07JewelId = 13;
                                canUse = !Jewel13BeingStealed;
                                break;
                            case "jewel14":
                                Thiefplayer07JewelId = 14;
                                canUse = !Jewel14BeingStealed;
                                break;
                            case "jewel15":
                                Thiefplayer07JewelId = 15;
                                canUse = !Jewel15BeingStealed;
                                break;
                        }
                    }
                    else if (((Jewelbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Jewelbuttontwo.transform.position) < 0.5f) || (Jewelbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Jewelbutton.transform.position) < 0.5f)) && Thiefplayer07IsStealing) canUse = true;
                }
            }

            return canUse && PlayerControl.LocalPlayer.CanMove && !Thiefplayer07IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _thiefplayer07TakeDeliverJewelButton.Timer = _thiefplayer07TakeDeliverJewelButton.MaxTimer; }, AssetLoader.PoliceAndThiefTakeJewelButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CommonAbilityPrimary, false, Tr.Get(TrKey.DeliverButton));

        // Thiefplayer08 Kill
        _thiefplayer08KillButton = new(() =>
        {
            var targetId = Thiefplayer08CurrentTarget.PlayerId;
            var sourceId = Thiefplayer08.PlayerId;
            var killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            killWriter.Write(targetId);
            killWriter.Write(sourceId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.GamemodeKills(targetId, sourceId);
            _thiefplayer08KillButton.Timer = _thiefplayer08KillButton.MaxTimer;
            Thiefplayer08CurrentTarget = null;
        }, () => { return Thiefplayer08 != null && Thiefplayer08 == PlayerControl.LocalPlayer && WhoCanThiefsKill != 2; }, () =>
        {
            var canUse = false;
            switch (WhoCanThiefsKill)
            {
                case 0:
                    if ((Policeplayer02 != null && Thiefplayer08CurrentTarget == Policeplayer02) || (Policeplayer04 != null && Thiefplayer08CurrentTarget == Policeplayer04)) canUse = true;
                    break;
                case 1:
                    canUse = true;
                    break;
            }

            return canUse && Thiefplayer08CurrentTarget && PlayerControl.LocalPlayer.CanMove && !Thiefplayer08IsReviving && !PlayerControl.LocalPlayer.Data.IsDead && !Thiefplayer08IsStealing;
        }, () => { _thiefplayer08KillButton.Timer = _thiefplayer08KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, AbilitySlot.CrewmateAbilityPrimary, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // Thiefplayer08 FreeThief Button
        _thiefplayer08FreeThiefButton = new(() =>
        {
            var thiefFree = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefFreeThief, SendOption.Reliable);
            AmongUsClient.Instance.FinishRpcImmediately(thiefFree);
            RPCProcedure.PoliceandThiefFreeThief();
            _thiefplayer08FreeThiefButton.Timer = _thiefplayer08FreeThiefButton.MaxTimer;
        }, () => { return Thiefplayer08 != null && Thiefplayer08 == PlayerControl.LocalPlayer; }, () =>
        {
            if (LocalThiefReleaseArrow.Count != 0)
            {
                LocalThiefReleaseArrow[0].Update(Cellbutton.transform.position);
                if (GameOptionsManager.Instance.currentGameOptions.MapId == 6) LocalThiefReleaseArrow[1].Update(Cellbuttontwo.transform.position);
            }

            if (LocalThiefDeliverArrow.Count != 0)
            {
                LocalThiefDeliverArrow[0].Update(Jewelbutton.transform.position);
                if (GameOptionsManager.Instance.currentGameOptions.MapId == 6) LocalThiefDeliverArrow[1].Update(Jewelbuttontwo.transform.position);
            }

            var canUse = false;
            if (CurrentThiefsCaptured > 0)
            {
                if (((Cellbuttontwo != null && Vector2.Distance(Thiefplayer08.transform.position, Cellbuttontwo.transform.position) < 0.4f) || Vector2.Distance(Thiefplayer08.transform.position, Cellbutton.transform.position) < 0.4f) && !Thiefplayer08.Data.IsDead)
                    canUse = true;
            }

            return canUse && PlayerControl.LocalPlayer.CanMove && !Thiefplayer08IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _thiefplayer08FreeThiefButton.Timer = _thiefplayer08FreeThiefButton.MaxTimer; }, AssetLoader.PoliceAndThiefFreeButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CrewmateAbilitySecondary, false, Tr.Get(TrKey.ReleaseButton));

        // Thiefplayer08 Take/Deliver Jewel Button
        _thiefplayer08TakeDeliverJewelButton = new(() =>
        {
            if (Thiefplayer08IsStealing)
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var jewelId = Thiefplayer08JewelId;
                var thiefScore = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefDeliverJewel, SendOption.Reliable);
                thiefScore.Write(targetId);
                thiefScore.Write(jewelId);
                AmongUsClient.Instance.FinishRpcImmediately(thiefScore);
                RPCProcedure.PoliceandThiefDeliverJewel(targetId, jewelId);
            }
            else
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var jewelId = Thiefplayer08JewelId;
                var thiefWhoTookATreasure = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefTakeJewel, SendOption.Reliable);
                thiefWhoTookATreasure.Write(targetId);
                thiefWhoTookATreasure.Write(jewelId);
                AmongUsClient.Instance.FinishRpcImmediately(thiefWhoTookATreasure);
                RPCProcedure.PoliceandThiefTakeJewel(targetId, jewelId);
            }

            _thiefplayer08TakeDeliverJewelButton.Timer = _thiefplayer08TakeDeliverJewelButton.MaxTimer;
        }, () => { return Thiefplayer08 != null && Thiefplayer08 == PlayerControl.LocalPlayer; }, () =>
        {
            if (Thiefplayer08IsStealing)
                _thiefplayer08TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefDeliverJewelButton;
            else
                _thiefplayer08TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefTakeJewelButton;
            var canUse = false;
            if (ThiefTreasures.Count != 0)
            {
                foreach (var jewel in ThiefTreasures)
                {
                    if (jewel != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, jewel.transform.position) < 0.5f && !Thiefplayer08IsStealing)
                    {
                        switch (jewel.name)
                        {
                            case "jewel01":
                                Thiefplayer08JewelId = 1;
                                canUse = !Jewel01BeingStealed;
                                break;
                            case "jewel02":
                                Thiefplayer08JewelId = 2;
                                canUse = !Jewel02BeingStealed;
                                break;
                            case "jewel03":
                                Thiefplayer08JewelId = 3;
                                canUse = !Jewel03BeingStealed;
                                break;
                            case "jewel04":
                                Thiefplayer08JewelId = 4;
                                canUse = !Jewel04BeingStealed;
                                break;
                            case "jewel05":
                                Thiefplayer08JewelId = 5;
                                canUse = !Jewel05BeingStealed;
                                break;
                            case "jewel06":
                                Thiefplayer08JewelId = 6;
                                canUse = !Jewel06BeingStealed;
                                break;
                            case "jewel07":
                                Thiefplayer08JewelId = 7;
                                canUse = !Jewel07BeingStealed;
                                break;
                            case "jewel08":
                                Thiefplayer08JewelId = 8;
                                canUse = !Jewel08BeingStealed;
                                break;
                            case "jewel09":
                                Thiefplayer08JewelId = 9;
                                canUse = !Jewel09BeingStealed;
                                break;
                            case "jewel10":
                                Thiefplayer08JewelId = 10;
                                canUse = !Jewel10BeingStealed;
                                break;
                            case "jewel11":
                                Thiefplayer08JewelId = 11;
                                canUse = !Jewel11BeingStealed;
                                break;
                            case "jewel12":
                                Thiefplayer08JewelId = 12;
                                canUse = !Jewel12BeingStealed;
                                break;
                            case "jewel13":
                                Thiefplayer08JewelId = 13;
                                canUse = !Jewel13BeingStealed;
                                break;
                            case "jewel14":
                                Thiefplayer08JewelId = 14;
                                canUse = !Jewel14BeingStealed;
                                break;
                            case "jewel15":
                                Thiefplayer08JewelId = 15;
                                canUse = !Jewel15BeingStealed;
                                break;
                        }
                    }
                    else if (((Jewelbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Jewelbuttontwo.transform.position) < 0.5f) || (Jewelbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Jewelbutton.transform.position) < 0.5f)) && Thiefplayer08IsStealing) canUse = true;
                }
            }

            return canUse && PlayerControl.LocalPlayer.CanMove && !Thiefplayer08IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _thiefplayer08TakeDeliverJewelButton.Timer = _thiefplayer08TakeDeliverJewelButton.MaxTimer; }, AssetLoader.PoliceAndThiefTakeJewelButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CommonAbilityPrimary, false, Tr.Get(TrKey.DeliverButton));

        // Thiefplayer09 Kill
        _thiefplayer09KillButton = new(() =>
        {
            var targetId = Thiefplayer09CurrentTarget.PlayerId;
            var sourceId = Thiefplayer09.PlayerId;
            var killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            killWriter.Write(targetId);
            killWriter.Write(sourceId);
            AmongUsClient.Instance.FinishRpcImmediately(killWriter);
            RPCProcedure.GamemodeKills(targetId, sourceId);
            _thiefplayer09KillButton.Timer = _thiefplayer09KillButton.MaxTimer;
            Thiefplayer09CurrentTarget = null;
        }, () => { return Thiefplayer09 != null && Thiefplayer09 == PlayerControl.LocalPlayer && WhoCanThiefsKill != 2; }, () =>
        {
            var canUse = false;
            switch (WhoCanThiefsKill)
            {
                case 0:
                    if ((Policeplayer02 != null && Thiefplayer09CurrentTarget == Policeplayer02) || (Policeplayer04 != null && Thiefplayer09CurrentTarget == Policeplayer04)) canUse = true;
                    break;
                case 1:
                    canUse = true;
                    break;
            }

            return canUse && Thiefplayer09CurrentTarget && PlayerControl.LocalPlayer.CanMove && !Thiefplayer09IsReviving && !PlayerControl.LocalPlayer.Data.IsDead && !Thiefplayer09IsStealing;
        }, () => { _thiefplayer09KillButton.Timer = _thiefplayer09KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, AbilitySlot.CrewmateAbilityPrimary, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // Thiefplayer09 FreeThief Button
        _thiefplayer09FreeThiefButton = new(() =>
        {
            var thiefFree = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefFreeThief, SendOption.Reliable);
            AmongUsClient.Instance.FinishRpcImmediately(thiefFree);
            RPCProcedure.PoliceandThiefFreeThief();
            _thiefplayer09FreeThiefButton.Timer = _thiefplayer09FreeThiefButton.MaxTimer;
        }, () => { return Thiefplayer09 != null && Thiefplayer09 == PlayerControl.LocalPlayer; }, () =>
        {
            if (LocalThiefReleaseArrow.Count != 0)
            {
                LocalThiefReleaseArrow[0].Update(Cellbutton.transform.position);
                if (GameOptionsManager.Instance.currentGameOptions.MapId == 6) LocalThiefReleaseArrow[1].Update(Cellbuttontwo.transform.position);
            }

            if (LocalThiefDeliverArrow.Count != 0)
            {
                LocalThiefDeliverArrow[0].Update(Jewelbutton.transform.position);
                if (GameOptionsManager.Instance.currentGameOptions.MapId == 6) LocalThiefDeliverArrow[1].Update(Jewelbuttontwo.transform.position);
            }

            var canUse = false;
            if (CurrentThiefsCaptured > 0)
            {
                if (((Cellbuttontwo != null && Vector2.Distance(Thiefplayer09.transform.position, Cellbuttontwo.transform.position) < 0.4f) || Vector2.Distance(Thiefplayer09.transform.position, Cellbutton.transform.position) < 0.4f) && !Thiefplayer09.Data.IsDead)
                    canUse = true;
            }

            return canUse && PlayerControl.LocalPlayer.CanMove && !Thiefplayer09IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _thiefplayer09FreeThiefButton.Timer = _thiefplayer09FreeThiefButton.MaxTimer; }, AssetLoader.PoliceAndThiefFreeButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CrewmateAbilitySecondary, false, Tr.Get(TrKey.ReleaseButton));

        // Thiefplayer09 Take/Deliver Jewel Button
        _thiefplayer09TakeDeliverJewelButton = new(() =>
        {
            if (Thiefplayer09IsStealing)
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var jewelId = Thiefplayer09JewelId;
                var thiefScore = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefDeliverJewel, SendOption.Reliable);
                thiefScore.Write(targetId);
                thiefScore.Write(jewelId);
                AmongUsClient.Instance.FinishRpcImmediately(thiefScore);
                RPCProcedure.PoliceandThiefDeliverJewel(targetId, jewelId);
            }
            else
            {
                var targetId = PlayerControl.LocalPlayer.PlayerId;
                var jewelId = Thiefplayer09JewelId;
                var thiefWhoTookATreasure = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PoliceandThiefTakeJewel, SendOption.Reliable);
                thiefWhoTookATreasure.Write(targetId);
                thiefWhoTookATreasure.Write(jewelId);
                AmongUsClient.Instance.FinishRpcImmediately(thiefWhoTookATreasure);
                RPCProcedure.PoliceandThiefTakeJewel(targetId, jewelId);
            }

            _thiefplayer09TakeDeliverJewelButton.Timer = _thiefplayer09TakeDeliverJewelButton.MaxTimer;
        }, () => { return Thiefplayer09 != null && Thiefplayer09 == PlayerControl.LocalPlayer; }, () =>
        {
            if (Thiefplayer09IsStealing)
                _thiefplayer09TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefDeliverJewelButton;
            else
                _thiefplayer09TakeDeliverJewelButton.ActionButton.graphic.sprite = AssetLoader.PoliceAndThiefTakeJewelButton;
            var canUse = false;
            if (ThiefTreasures.Count != 0)
            {
                foreach (var jewel in ThiefTreasures)
                {
                    if (jewel != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, jewel.transform.position) < 0.5f && !Thiefplayer09IsStealing)
                    {
                        switch (jewel.name)
                        {
                            case "jewel01":
                                Thiefplayer09JewelId = 1;
                                canUse = !Jewel01BeingStealed;
                                break;
                            case "jewel02":
                                Thiefplayer09JewelId = 2;
                                canUse = !Jewel02BeingStealed;
                                break;
                            case "jewel03":
                                Thiefplayer09JewelId = 3;
                                canUse = !Jewel03BeingStealed;
                                break;
                            case "jewel04":
                                Thiefplayer09JewelId = 4;
                                canUse = !Jewel04BeingStealed;
                                break;
                            case "jewel05":
                                Thiefplayer09JewelId = 5;
                                canUse = !Jewel05BeingStealed;
                                break;
                            case "jewel06":
                                Thiefplayer09JewelId = 6;
                                canUse = !Jewel06BeingStealed;
                                break;
                            case "jewel07":
                                Thiefplayer09JewelId = 7;
                                canUse = !Jewel07BeingStealed;
                                break;
                            case "jewel08":
                                Thiefplayer09JewelId = 8;
                                canUse = !Jewel08BeingStealed;
                                break;
                            case "jewel09":
                                Thiefplayer09JewelId = 9;
                                canUse = !Jewel09BeingStealed;
                                break;
                            case "jewel10":
                                Thiefplayer09JewelId = 10;
                                canUse = !Jewel10BeingStealed;
                                break;
                            case "jewel11":
                                Thiefplayer09JewelId = 11;
                                canUse = !Jewel11BeingStealed;
                                break;
                            case "jewel12":
                                Thiefplayer09JewelId = 12;
                                canUse = !Jewel12BeingStealed;
                                break;
                            case "jewel13":
                                Thiefplayer09JewelId = 13;
                                canUse = !Jewel13BeingStealed;
                                break;
                            case "jewel14":
                                Thiefplayer09JewelId = 14;
                                canUse = !Jewel14BeingStealed;
                                break;
                            case "jewel15":
                                Thiefplayer09JewelId = 15;
                                canUse = !Jewel15BeingStealed;
                                break;
                        }
                    }
                    else if (((Jewelbuttontwo != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Jewelbuttontwo.transform.position) < 0.5f) || (Jewelbutton != null && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, Jewelbutton.transform.position) < 0.5f)) && Thiefplayer09IsStealing) canUse = true;
                }
            }

            return canUse && PlayerControl.LocalPlayer.CanMove && !Thiefplayer09IsReviving && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _thiefplayer09TakeDeliverJewelButton.Timer = _thiefplayer09TakeDeliverJewelButton.MaxTimer; }, AssetLoader.PoliceAndThiefTakeJewelButton, ButtonPosition.Layout, __instance, __instance.UseButton, AbilitySlot.CommonAbilityPrimary, false, Tr.Get(TrKey.DeliverButton));
    }
}
