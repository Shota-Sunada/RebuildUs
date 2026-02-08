namespace RebuildUs.Modules.GameMode;

public static partial class BattleRoyale
{
    // Battle Royale
    private static CustomButton _soloPlayer01KillButton;
    private static CustomButton _soloPlayer02KillButton;
    private static CustomButton _soloPlayer03KillButton;
    private static CustomButton _soloPlayer04KillButton;
    private static CustomButton _soloPlayer05KillButton;
    private static CustomButton _soloPlayer06KillButton;
    private static CustomButton _soloPlayer07KillButton;
    private static CustomButton _soloPlayer08KillButton;
    private static CustomButton _soloPlayer09KillButton;
    private static CustomButton _soloPlayer10KillButton;
    private static CustomButton _soloPlayer11KillButton;
    private static CustomButton _soloPlayer12KillButton;
    private static CustomButton _soloPlayer13KillButton;
    private static CustomButton _soloPlayer14KillButton;
    private static CustomButton _soloPlayer15KillButton;
    private static CustomButton _limePlayer01KillButton;
    private static CustomButton _limePlayer02KillButton;
    private static CustomButton _limePlayer03KillButton;
    private static CustomButton _limePlayer04KillButton;
    private static CustomButton _limePlayer05KillButton;
    private static CustomButton _limePlayer06KillButton;
    private static CustomButton _limePlayer07KillButton;
    private static CustomButton _pinkPlayer01KillButton;
    private static CustomButton _pinkPlayer02KillButton;
    private static CustomButton _pinkPlayer03KillButton;
    private static CustomButton _pinkPlayer04KillButton;
    private static CustomButton _pinkPlayer05KillButton;
    private static CustomButton _pinkPlayer06KillButton;
    private static CustomButton _pinkPlayer07KillButton;
    private static CustomButton _serialKillerKillButton;

    public static void SetButtonCooldowns()
    {
        // Battle Royale
        _soloPlayer01KillButton.MaxTimer = KillCooldown;
        _soloPlayer02KillButton.MaxTimer = KillCooldown;
        _soloPlayer03KillButton.MaxTimer = KillCooldown;
        _soloPlayer04KillButton.MaxTimer = KillCooldown;
        _soloPlayer05KillButton.MaxTimer = KillCooldown;
        _soloPlayer06KillButton.MaxTimer = KillCooldown;
        _soloPlayer07KillButton.MaxTimer = KillCooldown;
        _soloPlayer08KillButton.MaxTimer = KillCooldown;
        _soloPlayer09KillButton.MaxTimer = KillCooldown;
        _soloPlayer10KillButton.MaxTimer = KillCooldown;
        _soloPlayer11KillButton.MaxTimer = KillCooldown;
        _soloPlayer12KillButton.MaxTimer = KillCooldown;
        _soloPlayer13KillButton.MaxTimer = KillCooldown;
        _soloPlayer14KillButton.MaxTimer = KillCooldown;
        _soloPlayer15KillButton.MaxTimer = KillCooldown;
        _limePlayer01KillButton.MaxTimer = KillCooldown;
        _limePlayer02KillButton.MaxTimer = KillCooldown;
        _limePlayer03KillButton.MaxTimer = KillCooldown;
        _limePlayer04KillButton.MaxTimer = KillCooldown;
        _limePlayer05KillButton.MaxTimer = KillCooldown;
        _limePlayer06KillButton.MaxTimer = KillCooldown;
        _limePlayer07KillButton.MaxTimer = KillCooldown;
        _pinkPlayer01KillButton.MaxTimer = KillCooldown;
        _pinkPlayer02KillButton.MaxTimer = KillCooldown;
        _pinkPlayer03KillButton.MaxTimer = KillCooldown;
        _pinkPlayer04KillButton.MaxTimer = KillCooldown;
        _pinkPlayer05KillButton.MaxTimer = KillCooldown;
        _pinkPlayer06KillButton.MaxTimer = KillCooldown;
        _pinkPlayer07KillButton.MaxTimer = KillCooldown;
        _serialKillerKillButton.MaxTimer = SerialKillerCooldown;
    }

    public static void MakeButtons(HudManager __instance)
    {
        // Battle Royale
        // SoloPlayer01 Kill
        _soloPlayer01KillButton = new(() =>
        {
            var target = GetShotPlayer(2 * 0.2f, 6, 1);

            var writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, SendOption.Reliable);
            writerShot.Write(SoloPlayer01.PlayerId);
            writerShot.Write(0);
            writerShot.Write(SoloPlayer01MouseAngle);
            AmongUsClient.Instance.FinishRpcImmediately(writerShot);
            RPCProcedure.BattleRoyaleShowShoots(SoloPlayer01.PlayerId, 0, SoloPlayer01MouseAngle);

            if (target == null)
            {
                _soloPlayer01KillButton.Timer = _soloPlayer01KillButton.MaxTimer;
                return;
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            writer.Write(target.PlayerId);
            writer.Write(SoloPlayer01.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.GamemodeKills(target.PlayerId, SoloPlayer01.PlayerId);

            _soloPlayer01KillButton.Timer = _soloPlayer01KillButton.MaxTimer;
            SoundManager.Instance.PlaySound(AssetLoader.RoyaleHitPlayer, false, 100f);

            target = null;
        }, () => { return SoloPlayer01 != null && SoloPlayer01 == PlayerControl.LocalPlayer; }, () =>
        {
            if (SoloPlayer01Wep == null)
            {
                SoloPlayer01Wep = new("Weapon");
                var renderer = SoloPlayer01Wep.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.Bow;
                renderer.transform.parent = SoloPlayer01.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.IsDead) SoloPlayer01Wep.SetActive(false);

                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                SoloPlayer01MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = SoloPlayer01.transform.position + new Vector3(0.8f * (float)Math.Cos(SoloPlayer01MouseAngle), 0.8f * (float)Math.Sin(SoloPlayer01MouseAngle));
                SoloPlayer01Wep.transform.position += (targetPosition - SoloPlayer01Wep.transform.position) * 0.4f;
                SoloPlayer01Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((SoloPlayer01MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(SoloPlayer01MouseAngle) < 0.0)
                {
                    if (SoloPlayer01Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        SoloPlayer01Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (SoloPlayer01Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        SoloPlayer01Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            return PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _soloPlayer01KillButton.Timer = _soloPlayer01KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, KeyCode.Mouse1, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // SoloPlayer02 Kill
        _soloPlayer02KillButton = new(() =>
        {
            var target = GetShotPlayer(2 * 0.2f, 6, 2);

            var writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, SendOption.Reliable);
            writerShot.Write(SoloPlayer02.PlayerId);
            writerShot.Write(0);
            writerShot.Write(SoloPlayer02MouseAngle);
            AmongUsClient.Instance.FinishRpcImmediately(writerShot);
            RPCProcedure.BattleRoyaleShowShoots(SoloPlayer02.PlayerId, 0, SoloPlayer02MouseAngle);

            if (target == null)
            {
                _soloPlayer02KillButton.Timer = _soloPlayer02KillButton.MaxTimer;
                return;
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            writer.Write(target.PlayerId);
            writer.Write(SoloPlayer02.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.GamemodeKills(target.PlayerId, SoloPlayer02.PlayerId);

            _soloPlayer02KillButton.Timer = _soloPlayer02KillButton.MaxTimer;
            SoundManager.Instance.PlaySound(AssetLoader.RoyaleHitPlayer, false, 100f);

            target = null;
        }, () => { return SoloPlayer02 != null && SoloPlayer02 == PlayerControl.LocalPlayer; }, () =>
        {
            if (SoloPlayer02Wep == null)
            {
                SoloPlayer02Wep = new("Weapon");
                var renderer = SoloPlayer02Wep.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.Bow;
                renderer.transform.parent = SoloPlayer02.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.IsDead) SoloPlayer02Wep.SetActive(false);

                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                SoloPlayer02MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = SoloPlayer02.transform.position + new Vector3(0.8f * (float)Math.Cos(SoloPlayer02MouseAngle), 0.8f * (float)Math.Sin(SoloPlayer02MouseAngle));
                SoloPlayer02Wep.transform.position += (targetPosition - SoloPlayer02Wep.transform.position) * 0.4f;
                SoloPlayer02Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((SoloPlayer02MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(SoloPlayer02MouseAngle) < 0.0)
                {
                    if (SoloPlayer02Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        SoloPlayer02Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (SoloPlayer02Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        SoloPlayer02Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            return PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _soloPlayer02KillButton.Timer = _soloPlayer02KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, KeyCode.Mouse1, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // SoloPlayer03 Kill
        _soloPlayer03KillButton = new(() =>
        {
            var target = GetShotPlayer(2 * 0.2f, 6, 3);

            var writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, SendOption.Reliable);
            writerShot.Write(SoloPlayer03.PlayerId);
            writerShot.Write(0);
            writerShot.Write(SoloPlayer03MouseAngle);
            AmongUsClient.Instance.FinishRpcImmediately(writerShot);
            RPCProcedure.BattleRoyaleShowShoots(SoloPlayer03.PlayerId, 0, SoloPlayer03MouseAngle);

            if (target == null)
            {
                _soloPlayer03KillButton.Timer = _soloPlayer03KillButton.MaxTimer;
                return;
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            writer.Write(target.PlayerId);
            writer.Write(SoloPlayer03.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.GamemodeKills(target.PlayerId, SoloPlayer03.PlayerId);

            _soloPlayer03KillButton.Timer = _soloPlayer03KillButton.MaxTimer;
            SoundManager.Instance.PlaySound(AssetLoader.RoyaleHitPlayer, false, 100f);

            target = null;
        }, () => { return SoloPlayer03 != null && SoloPlayer03 == PlayerControl.LocalPlayer; }, () =>
        {
            if (SoloPlayer03Wep == null)
            {
                SoloPlayer03Wep = new("Weapon");
                var renderer = SoloPlayer03Wep.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.Bow;
                renderer.transform.parent = SoloPlayer03.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.IsDead) SoloPlayer03Wep.SetActive(false);

                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                SoloPlayer03MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = SoloPlayer03.transform.position + new Vector3(0.8f * (float)Math.Cos(SoloPlayer03MouseAngle), 0.8f * (float)Math.Sin(SoloPlayer03MouseAngle));
                SoloPlayer03Wep.transform.position += (targetPosition - SoloPlayer03Wep.transform.position) * 0.4f;
                SoloPlayer03Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((SoloPlayer03MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(SoloPlayer03MouseAngle) < 0.0)
                {
                    if (SoloPlayer03Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        SoloPlayer03Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (SoloPlayer03Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        SoloPlayer03Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            return PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _soloPlayer03KillButton.Timer = _soloPlayer03KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, KeyCode.Mouse1, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // SoloPlayer04 Kill
        _soloPlayer04KillButton = new(() =>
        {
            var target = GetShotPlayer(2 * 0.2f, 6, 4);

            var writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, SendOption.Reliable);
            writerShot.Write(SoloPlayer04.PlayerId);
            writerShot.Write(0);
            writerShot.Write(SoloPlayer04MouseAngle);
            AmongUsClient.Instance.FinishRpcImmediately(writerShot);
            RPCProcedure.BattleRoyaleShowShoots(SoloPlayer04.PlayerId, 0, SoloPlayer04MouseAngle);

            if (target == null)
            {
                _soloPlayer04KillButton.Timer = _soloPlayer04KillButton.MaxTimer;
                return;
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            writer.Write(target.PlayerId);
            writer.Write(SoloPlayer04.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.GamemodeKills(target.PlayerId, SoloPlayer04.PlayerId);

            _soloPlayer04KillButton.Timer = _soloPlayer04KillButton.MaxTimer;
            SoundManager.Instance.PlaySound(AssetLoader.RoyaleHitPlayer, false, 100f);

            target = null;
        }, () => { return SoloPlayer04 != null && SoloPlayer04 == PlayerControl.LocalPlayer; }, () =>
        {
            if (SoloPlayer04Wep == null)
            {
                SoloPlayer04Wep = new("Weapon");
                var renderer = SoloPlayer04Wep.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.Bow;
                renderer.transform.parent = SoloPlayer04.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.IsDead) SoloPlayer04Wep.SetActive(false);

                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                SoloPlayer04MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = SoloPlayer04.transform.position + new Vector3(0.8f * (float)Math.Cos(SoloPlayer04MouseAngle), 0.8f * (float)Math.Sin(SoloPlayer04MouseAngle));
                SoloPlayer04Wep.transform.position += (targetPosition - SoloPlayer04Wep.transform.position) * 0.4f;
                SoloPlayer04Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((SoloPlayer04MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(SoloPlayer04MouseAngle) < 0.0)
                {
                    if (SoloPlayer04Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        SoloPlayer04Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (SoloPlayer04Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        SoloPlayer04Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            return PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _soloPlayer04KillButton.Timer = _soloPlayer04KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, KeyCode.Mouse1, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // SoloPlayer05 Kill
        _soloPlayer05KillButton = new(() =>
        {
            var target = GetShotPlayer(2 * 0.2f, 6, 5);

            var writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, SendOption.Reliable);
            writerShot.Write(SoloPlayer05.PlayerId);
            writerShot.Write(0);
            writerShot.Write(SoloPlayer05MouseAngle);
            AmongUsClient.Instance.FinishRpcImmediately(writerShot);
            RPCProcedure.BattleRoyaleShowShoots(SoloPlayer05.PlayerId, 0, SoloPlayer05MouseAngle);

            if (target == null)
            {
                _soloPlayer05KillButton.Timer = _soloPlayer05KillButton.MaxTimer;
                return;
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            writer.Write(target.PlayerId);
            writer.Write(SoloPlayer05.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.GamemodeKills(target.PlayerId, SoloPlayer05.PlayerId);

            _soloPlayer05KillButton.Timer = _soloPlayer05KillButton.MaxTimer;
            SoundManager.Instance.PlaySound(AssetLoader.RoyaleHitPlayer, false, 100f);

            target = null;
        }, () => { return SoloPlayer05 != null && SoloPlayer05 == PlayerControl.LocalPlayer; }, () =>
        {
            if (SoloPlayer05Wep == null)
            {
                SoloPlayer05Wep = new("Weapon");
                var renderer = SoloPlayer05Wep.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.Bow;
                renderer.transform.parent = SoloPlayer05.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.IsDead) SoloPlayer05Wep.SetActive(false);

                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                SoloPlayer05MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = SoloPlayer05.transform.position + new Vector3(0.8f * (float)Math.Cos(SoloPlayer05MouseAngle), 0.8f * (float)Math.Sin(SoloPlayer05MouseAngle));
                SoloPlayer05Wep.transform.position += (targetPosition - SoloPlayer05Wep.transform.position) * 0.4f;
                SoloPlayer05Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((SoloPlayer05MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(SoloPlayer05MouseAngle) < 0.0)
                {
                    if (SoloPlayer05Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        SoloPlayer05Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (SoloPlayer05Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        SoloPlayer05Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            return PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _soloPlayer05KillButton.Timer = _soloPlayer05KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, KeyCode.Mouse1, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // SoloPlayer06 Kill
        _soloPlayer06KillButton = new(() =>
        {
            var target = GetShotPlayer(2 * 0.2f, 6, 6);

            var writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, SendOption.Reliable);
            writerShot.Write(SoloPlayer06.PlayerId);
            writerShot.Write(0);
            writerShot.Write(SoloPlayer06MouseAngle);
            AmongUsClient.Instance.FinishRpcImmediately(writerShot);
            RPCProcedure.BattleRoyaleShowShoots(SoloPlayer06.PlayerId, 0, SoloPlayer06MouseAngle);

            if (target == null)
            {
                _soloPlayer06KillButton.Timer = _soloPlayer06KillButton.MaxTimer;
                return;
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            writer.Write(target.PlayerId);
            writer.Write(SoloPlayer06.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.GamemodeKills(target.PlayerId, SoloPlayer06.PlayerId);

            _soloPlayer06KillButton.Timer = _soloPlayer06KillButton.MaxTimer;
            SoundManager.Instance.PlaySound(AssetLoader.RoyaleHitPlayer, false, 100f);

            target = null;
        }, () => { return SoloPlayer06 != null && SoloPlayer06 == PlayerControl.LocalPlayer; }, () =>
        {
            if (SoloPlayer06Wep == null)
            {
                SoloPlayer06Wep = new("Weapon");
                var renderer = SoloPlayer06Wep.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.Bow;
                renderer.transform.parent = SoloPlayer06.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.IsDead) SoloPlayer06Wep.SetActive(false);

                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                SoloPlayer06MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = SoloPlayer06.transform.position + new Vector3(0.8f * (float)Math.Cos(SoloPlayer06MouseAngle), 0.8f * (float)Math.Sin(SoloPlayer06MouseAngle));
                SoloPlayer06Wep.transform.position += (targetPosition - SoloPlayer06Wep.transform.position) * 0.4f;
                SoloPlayer06Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((SoloPlayer06MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(SoloPlayer06MouseAngle) < 0.0)
                {
                    if (SoloPlayer06Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        SoloPlayer06Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (SoloPlayer06Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        SoloPlayer06Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            return PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _soloPlayer06KillButton.Timer = _soloPlayer06KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, KeyCode.Mouse1, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // SoloPlayer07 Kill
        _soloPlayer07KillButton = new(() =>
        {
            var target = GetShotPlayer(2 * 0.2f, 6, 7);

            var writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, SendOption.Reliable);
            writerShot.Write(SoloPlayer07.PlayerId);
            writerShot.Write(0);
            writerShot.Write(SoloPlayer07MouseAngle);
            AmongUsClient.Instance.FinishRpcImmediately(writerShot);
            RPCProcedure.BattleRoyaleShowShoots(SoloPlayer07.PlayerId, 0, SoloPlayer07MouseAngle);

            if (target == null)
            {
                _soloPlayer07KillButton.Timer = _soloPlayer07KillButton.MaxTimer;
                return;
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            writer.Write(target.PlayerId);
            writer.Write(SoloPlayer07.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.GamemodeKills(target.PlayerId, SoloPlayer07.PlayerId);

            _soloPlayer07KillButton.Timer = _soloPlayer07KillButton.MaxTimer;
            SoundManager.Instance.PlaySound(AssetLoader.RoyaleHitPlayer, false, 100f);

            target = null;
        }, () => { return SoloPlayer07 != null && SoloPlayer07 == PlayerControl.LocalPlayer; }, () =>
        {
            if (SoloPlayer07Wep == null)
            {
                SoloPlayer07Wep = new("Weapon");
                var renderer = SoloPlayer07Wep.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.Bow;
                renderer.transform.parent = SoloPlayer07.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.IsDead) SoloPlayer07Wep.SetActive(false);

                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                SoloPlayer07MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = SoloPlayer07.transform.position + new Vector3(0.8f * (float)Math.Cos(SoloPlayer07MouseAngle), 0.8f * (float)Math.Sin(SoloPlayer07MouseAngle));
                SoloPlayer07Wep.transform.position += (targetPosition - SoloPlayer07Wep.transform.position) * 0.4f;
                SoloPlayer07Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((SoloPlayer07MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(SoloPlayer07MouseAngle) < 0.0)
                {
                    if (SoloPlayer07Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        SoloPlayer07Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (SoloPlayer07Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        SoloPlayer07Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            return PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _soloPlayer07KillButton.Timer = _soloPlayer07KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, KeyCode.Mouse1, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // SoloPlayer08 Kill
        _soloPlayer08KillButton = new(() =>
        {
            var target = GetShotPlayer(2 * 0.2f, 6, 8);

            var writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, SendOption.Reliable);
            writerShot.Write(SoloPlayer08.PlayerId);
            writerShot.Write(0);
            writerShot.Write(SoloPlayer08MouseAngle);
            AmongUsClient.Instance.FinishRpcImmediately(writerShot);
            RPCProcedure.BattleRoyaleShowShoots(SoloPlayer08.PlayerId, 0, SoloPlayer08MouseAngle);

            if (target == null)
            {
                _soloPlayer08KillButton.Timer = _soloPlayer08KillButton.MaxTimer;
                return;
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            writer.Write(target.PlayerId);
            writer.Write(SoloPlayer08.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.GamemodeKills(target.PlayerId, SoloPlayer08.PlayerId);

            _soloPlayer08KillButton.Timer = _soloPlayer08KillButton.MaxTimer;
            SoundManager.Instance.PlaySound(AssetLoader.RoyaleHitPlayer, false, 100f);

            target = null;
        }, () => { return SoloPlayer08 != null && SoloPlayer08 == PlayerControl.LocalPlayer; }, () =>
        {
            if (SoloPlayer08Wep == null)
            {
                SoloPlayer08Wep = new("Weapon");
                var renderer = SoloPlayer08Wep.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.Bow;
                renderer.transform.parent = SoloPlayer08.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.IsDead) SoloPlayer08Wep.SetActive(false);

                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                SoloPlayer08MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = SoloPlayer08.transform.position + new Vector3(0.8f * (float)Math.Cos(SoloPlayer08MouseAngle), 0.8f * (float)Math.Sin(SoloPlayer08MouseAngle));
                SoloPlayer08Wep.transform.position += (targetPosition - SoloPlayer08Wep.transform.position) * 0.4f;
                SoloPlayer08Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((SoloPlayer08MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(SoloPlayer08MouseAngle) < 0.0)
                {
                    if (SoloPlayer08Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        SoloPlayer08Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (SoloPlayer08Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        SoloPlayer08Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            return PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _soloPlayer08KillButton.Timer = _soloPlayer08KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, KeyCode.Mouse1, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // SoloPlayer09 Kill
        _soloPlayer09KillButton = new(() =>
        {
            var target = GetShotPlayer(2 * 0.2f, 6, 9);

            var writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, SendOption.Reliable);
            writerShot.Write(SoloPlayer09.PlayerId);
            writerShot.Write(0);
            writerShot.Write(SoloPlayer09MouseAngle);
            AmongUsClient.Instance.FinishRpcImmediately(writerShot);
            RPCProcedure.BattleRoyaleShowShoots(SoloPlayer09.PlayerId, 0, SoloPlayer09MouseAngle);

            if (target == null)
            {
                _soloPlayer09KillButton.Timer = _soloPlayer09KillButton.MaxTimer;
                return;
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            writer.Write(target.PlayerId);
            writer.Write(SoloPlayer09.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.GamemodeKills(target.PlayerId, SoloPlayer09.PlayerId);

            _soloPlayer09KillButton.Timer = _soloPlayer09KillButton.MaxTimer;
            SoundManager.Instance.PlaySound(AssetLoader.RoyaleHitPlayer, false, 100f);

            target = null;
        }, () => { return SoloPlayer09 != null && SoloPlayer09 == PlayerControl.LocalPlayer; }, () =>
        {
            if (SoloPlayer09Wep == null)
            {
                SoloPlayer09Wep = new("Weapon");
                var renderer = SoloPlayer09Wep.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.Bow;
                renderer.transform.parent = SoloPlayer09.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.IsDead) SoloPlayer09Wep.SetActive(false);

                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                SoloPlayer09MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = SoloPlayer09.transform.position + new Vector3(0.8f * (float)Math.Cos(SoloPlayer09MouseAngle), 0.8f * (float)Math.Sin(SoloPlayer09MouseAngle));
                SoloPlayer09Wep.transform.position += (targetPosition - SoloPlayer09Wep.transform.position) * 0.4f;
                SoloPlayer09Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((SoloPlayer09MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(SoloPlayer09MouseAngle) < 0.0)
                {
                    if (SoloPlayer09Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        SoloPlayer09Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (SoloPlayer09Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        SoloPlayer09Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            return PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _soloPlayer09KillButton.Timer = _soloPlayer09KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, KeyCode.Mouse1, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // SoloPlayer10 Kill
        _soloPlayer10KillButton = new(() =>
        {
            var target = GetShotPlayer(2 * 0.2f, 6, 10);

            var writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, SendOption.Reliable);
            writerShot.Write(SoloPlayer10.PlayerId);
            writerShot.Write(0);
            writerShot.Write(SoloPlayer10MouseAngle);
            AmongUsClient.Instance.FinishRpcImmediately(writerShot);
            RPCProcedure.BattleRoyaleShowShoots(SoloPlayer10.PlayerId, 0, SoloPlayer10MouseAngle);

            if (target == null)
            {
                _soloPlayer10KillButton.Timer = _soloPlayer10KillButton.MaxTimer;
                return;
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            writer.Write(target.PlayerId);
            writer.Write(SoloPlayer10.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.GamemodeKills(target.PlayerId, SoloPlayer10.PlayerId);

            _soloPlayer10KillButton.Timer = _soloPlayer10KillButton.MaxTimer;
            SoundManager.Instance.PlaySound(AssetLoader.RoyaleHitPlayer, false, 100f);

            target = null;
        }, () => { return SoloPlayer10 != null && SoloPlayer10 == PlayerControl.LocalPlayer; }, () =>
        {
            if (SoloPlayer10Wep == null)
            {
                SoloPlayer10Wep = new("Weapon");
                var renderer = SoloPlayer10Wep.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.Bow;
                renderer.transform.parent = SoloPlayer10.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.IsDead) SoloPlayer10Wep.SetActive(false);

                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                SoloPlayer10MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = SoloPlayer10.transform.position + new Vector3(0.8f * (float)Math.Cos(SoloPlayer10MouseAngle), 0.8f * (float)Math.Sin(SoloPlayer10MouseAngle));
                SoloPlayer10Wep.transform.position += (targetPosition - SoloPlayer10Wep.transform.position) * 0.4f;
                SoloPlayer10Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((SoloPlayer10MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(SoloPlayer10MouseAngle) < 0.0)
                {
                    if (SoloPlayer10Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        SoloPlayer10Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (SoloPlayer10Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        SoloPlayer10Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            return PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _soloPlayer10KillButton.Timer = _soloPlayer10KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, KeyCode.Mouse1, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // SoloPlayer11 Kill
        _soloPlayer11KillButton = new(() =>
        {
            var target = GetShotPlayer(2 * 0.2f, 6, 11);

            var writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, SendOption.Reliable);
            writerShot.Write(SoloPlayer11.PlayerId);
            writerShot.Write(0);
            writerShot.Write(SoloPlayer11MouseAngle);
            AmongUsClient.Instance.FinishRpcImmediately(writerShot);
            RPCProcedure.BattleRoyaleShowShoots(SoloPlayer11.PlayerId, 0, SoloPlayer11MouseAngle);

            if (target == null)
            {
                _soloPlayer11KillButton.Timer = _soloPlayer11KillButton.MaxTimer;
                return;
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            writer.Write(target.PlayerId);
            writer.Write(SoloPlayer11.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.GamemodeKills(target.PlayerId, SoloPlayer11.PlayerId);

            _soloPlayer11KillButton.Timer = _soloPlayer11KillButton.MaxTimer;
            SoundManager.Instance.PlaySound(AssetLoader.RoyaleHitPlayer, false, 100f);

            target = null;
        }, () => { return SoloPlayer11 != null && SoloPlayer11 == PlayerControl.LocalPlayer; }, () =>
        {
            if (SoloPlayer11Wep == null)
            {
                SoloPlayer11Wep = new("Weapon");
                var renderer = SoloPlayer11Wep.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.Bow;
                renderer.transform.parent = SoloPlayer11.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.IsDead) SoloPlayer11Wep.SetActive(false);

                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                SoloPlayer11MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = SoloPlayer11.transform.position + new Vector3(0.8f * (float)Math.Cos(SoloPlayer11MouseAngle), 0.8f * (float)Math.Sin(SoloPlayer11MouseAngle));
                SoloPlayer11Wep.transform.position += (targetPosition - SoloPlayer11Wep.transform.position) * 0.4f;
                SoloPlayer11Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((SoloPlayer11MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(SoloPlayer11MouseAngle) < 0.0)
                {
                    if (SoloPlayer11Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        SoloPlayer11Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (SoloPlayer11Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        SoloPlayer11Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            return PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _soloPlayer11KillButton.Timer = _soloPlayer11KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, KeyCode.Mouse1, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // SoloPlayer12 Kill
        _soloPlayer12KillButton = new(() =>
        {
            var target = GetShotPlayer(2 * 0.2f, 6, 12);

            var writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, SendOption.Reliable);
            writerShot.Write(SoloPlayer12.PlayerId);
            writerShot.Write(0);
            writerShot.Write(SoloPlayer12MouseAngle);
            AmongUsClient.Instance.FinishRpcImmediately(writerShot);
            RPCProcedure.BattleRoyaleShowShoots(SoloPlayer12.PlayerId, 0, SoloPlayer12MouseAngle);

            if (target == null)
            {
                _soloPlayer12KillButton.Timer = _soloPlayer12KillButton.MaxTimer;
                return;
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            writer.Write(target.PlayerId);
            writer.Write(SoloPlayer12.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.GamemodeKills(target.PlayerId, SoloPlayer12.PlayerId);

            _soloPlayer12KillButton.Timer = _soloPlayer12KillButton.MaxTimer;
            SoundManager.Instance.PlaySound(AssetLoader.RoyaleHitPlayer, false, 100f);

            target = null;
        }, () => { return SoloPlayer12 != null && SoloPlayer12 == PlayerControl.LocalPlayer; }, () =>
        {
            if (SoloPlayer12Wep == null)
            {
                SoloPlayer12Wep = new("Weapon");
                var renderer = SoloPlayer12Wep.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.Bow;
                renderer.transform.parent = SoloPlayer12.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.IsDead) SoloPlayer12Wep.SetActive(false);

                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                SoloPlayer12MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = SoloPlayer12.transform.position + new Vector3(0.8f * (float)Math.Cos(SoloPlayer12MouseAngle), 0.8f * (float)Math.Sin(SoloPlayer12MouseAngle));
                SoloPlayer12Wep.transform.position += (targetPosition - SoloPlayer12Wep.transform.position) * 0.4f;
                SoloPlayer12Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((SoloPlayer12MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(SoloPlayer12MouseAngle) < 0.0)
                {
                    if (SoloPlayer12Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        SoloPlayer12Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (SoloPlayer12Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        SoloPlayer12Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            return PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _soloPlayer12KillButton.Timer = _soloPlayer12KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, KeyCode.Mouse1, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // SoloPlayer13 Kill
        _soloPlayer13KillButton = new(() =>
        {
            var target = GetShotPlayer(2 * 0.2f, 6, 13);

            var writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, SendOption.Reliable);
            writerShot.Write(SoloPlayer13.PlayerId);
            writerShot.Write(0);
            writerShot.Write(SoloPlayer13MouseAngle);
            AmongUsClient.Instance.FinishRpcImmediately(writerShot);
            RPCProcedure.BattleRoyaleShowShoots(SoloPlayer13.PlayerId, 0, SoloPlayer13MouseAngle);

            if (target == null)
            {
                _soloPlayer13KillButton.Timer = _soloPlayer13KillButton.MaxTimer;
                return;
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            writer.Write(target.PlayerId);
            writer.Write(SoloPlayer13.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.GamemodeKills(target.PlayerId, SoloPlayer13.PlayerId);

            _soloPlayer13KillButton.Timer = _soloPlayer13KillButton.MaxTimer;
            SoundManager.Instance.PlaySound(AssetLoader.RoyaleHitPlayer, false, 100f);

            target = null;
        }, () => { return SoloPlayer13 != null && SoloPlayer13 == PlayerControl.LocalPlayer; }, () =>
        {
            if (SoloPlayer13Wep == null)
            {
                SoloPlayer13Wep = new("Weapon");
                var renderer = SoloPlayer13Wep.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.Bow;
                renderer.transform.parent = SoloPlayer13.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.IsDead) SoloPlayer13Wep.SetActive(false);

                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                SoloPlayer13MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = SoloPlayer13.transform.position + new Vector3(0.8f * (float)Math.Cos(SoloPlayer13MouseAngle), 0.8f * (float)Math.Sin(SoloPlayer13MouseAngle));
                SoloPlayer13Wep.transform.position += (targetPosition - SoloPlayer13Wep.transform.position) * 0.4f;
                SoloPlayer13Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((SoloPlayer13MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(SoloPlayer13MouseAngle) < 0.0)
                {
                    if (SoloPlayer13Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        SoloPlayer13Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (SoloPlayer13Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        SoloPlayer13Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            return PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _soloPlayer13KillButton.Timer = _soloPlayer13KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, KeyCode.Mouse1, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // SoloPlayer14 Kill
        _soloPlayer14KillButton = new(() =>
        {
            var target = GetShotPlayer(2 * 0.2f, 6, 14);

            var writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, SendOption.Reliable);
            writerShot.Write(SoloPlayer14.PlayerId);
            writerShot.Write(0);
            writerShot.Write(SoloPlayer14MouseAngle);
            AmongUsClient.Instance.FinishRpcImmediately(writerShot);
            RPCProcedure.BattleRoyaleShowShoots(SoloPlayer14.PlayerId, 0, SoloPlayer14MouseAngle);

            if (target == null)
            {
                _soloPlayer14KillButton.Timer = _soloPlayer14KillButton.MaxTimer;
                return;
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            writer.Write(target.PlayerId);
            writer.Write(SoloPlayer14.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.GamemodeKills(target.PlayerId, SoloPlayer14.PlayerId);

            _soloPlayer14KillButton.Timer = _soloPlayer14KillButton.MaxTimer;
            SoundManager.Instance.PlaySound(AssetLoader.RoyaleHitPlayer, false, 100f);

            target = null;
        }, () => { return SoloPlayer14 != null && SoloPlayer14 == PlayerControl.LocalPlayer; }, () =>
        {
            if (SoloPlayer14Wep == null)
            {
                SoloPlayer14Wep = new("Weapon");
                var renderer = SoloPlayer14Wep.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.Bow;
                renderer.transform.parent = SoloPlayer14.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.IsDead) SoloPlayer14Wep.SetActive(false);

                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                SoloPlayer14MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = SoloPlayer14.transform.position + new Vector3(0.8f * (float)Math.Cos(SoloPlayer14MouseAngle), 0.8f * (float)Math.Sin(SoloPlayer14MouseAngle));
                SoloPlayer14Wep.transform.position += (targetPosition - SoloPlayer14Wep.transform.position) * 0.4f;
                SoloPlayer14Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((SoloPlayer14MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(SoloPlayer14MouseAngle) < 0.0)
                {
                    if (SoloPlayer14Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        SoloPlayer14Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (SoloPlayer14Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        SoloPlayer14Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            return PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _soloPlayer14KillButton.Timer = _soloPlayer14KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, KeyCode.Mouse1, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // SoloPlayer15 Kill
        _soloPlayer15KillButton = new(() =>
        {
            var target = GetShotPlayer(2 * 0.2f, 6, 15);

            var writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, SendOption.Reliable);
            writerShot.Write(SoloPlayer15.PlayerId);
            writerShot.Write(0);
            writerShot.Write(SoloPlayer15MouseAngle);
            AmongUsClient.Instance.FinishRpcImmediately(writerShot);
            RPCProcedure.BattleRoyaleShowShoots(SoloPlayer15.PlayerId, 0, SoloPlayer15MouseAngle);

            if (target == null)
            {
                _soloPlayer15KillButton.Timer = _soloPlayer15KillButton.MaxTimer;
                return;
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            writer.Write(target.PlayerId);
            writer.Write(SoloPlayer15.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.GamemodeKills(target.PlayerId, SoloPlayer15.PlayerId);

            _soloPlayer15KillButton.Timer = _soloPlayer15KillButton.MaxTimer;
            SoundManager.Instance.PlaySound(AssetLoader.RoyaleHitPlayer, false, 100f);

            target = null;
        }, () => { return SoloPlayer15 != null && SoloPlayer15 == PlayerControl.LocalPlayer; }, () =>
        {
            if (SoloPlayer15Wep == null)
            {
                SoloPlayer15Wep = new("Weapon");
                var renderer = SoloPlayer15Wep.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.Bow;
                renderer.transform.parent = SoloPlayer15.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.IsDead) SoloPlayer15Wep.SetActive(false);

                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                SoloPlayer15MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = SoloPlayer15.transform.position + new Vector3(0.8f * (float)Math.Cos(SoloPlayer15MouseAngle), 0.8f * (float)Math.Sin(SoloPlayer15MouseAngle));
                SoloPlayer15Wep.transform.position += (targetPosition - SoloPlayer15Wep.transform.position) * 0.4f;
                SoloPlayer15Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((SoloPlayer15MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(SoloPlayer15MouseAngle) < 0.0)
                {
                    if (SoloPlayer15Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        SoloPlayer15Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (SoloPlayer15Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        SoloPlayer15Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            return PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _soloPlayer15KillButton.Timer = _soloPlayer15KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, KeyCode.Mouse1, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // LimePlayer01 Kill
        _limePlayer01KillButton = new(() =>
        {
            var target = GetLimeShotPlayer(2 * 0.2f, 6, 1);

            var writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, SendOption.Reliable);
            writerShot.Write(LimePlayer01.PlayerId);
            writerShot.Write(1);
            writerShot.Write(LimePlayer01MouseAngle);
            AmongUsClient.Instance.FinishRpcImmediately(writerShot);
            RPCProcedure.BattleRoyaleShowShoots(LimePlayer01.PlayerId, 1, LimePlayer01MouseAngle);

            if (target == null)
            {
                _limePlayer01KillButton.Timer = _limePlayer01KillButton.MaxTimer;
                return;
            }

            if (MatchType == 2)
            {
                if ((PinkPlayer01 != null && target == PinkPlayer01 && PinkPlayer01IsReviving) || (PinkPlayer02 != null && target == PinkPlayer02 && PinkPlayer02IsReviving) || (PinkPlayer03 != null && target == PinkPlayer03 && PinkPlayer03IsReviving) || (PinkPlayer04 != null && target == PinkPlayer04 && PinkPlayer04IsReviving) || (PinkPlayer05 != null && target == PinkPlayer05 && PinkPlayer05IsReviving) || (PinkPlayer06 != null && target == PinkPlayer06 && PinkPlayer06IsReviving) || (PinkPlayer07 != null && target == PinkPlayer07 && PinkPlayer07IsReviving) || (SerialKiller != null && target == SerialKiller && SerialKillerIsReviving))
                {
                    _limePlayer01KillButton.Timer = _limePlayer01KillButton.MaxTimer;
                    return;
                }
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            writer.Write(target.PlayerId);
            writer.Write(LimePlayer01.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.GamemodeKills(target.PlayerId, LimePlayer01.PlayerId);

            _limePlayer01KillButton.Timer = _limePlayer01KillButton.MaxTimer;
            SoundManager.Instance.PlaySound(AssetLoader.RoyaleHitPlayer, false, 100f);

            target = null;
        }, () => { return LimePlayer01 != null && LimePlayer01 == PlayerControl.LocalPlayer; }, () =>
        {
            if (LimePlayer01Wep == null)
            {
                LimePlayer01Wep = new("Weapon");
                var renderer = LimePlayer01Wep.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.Bow;
                renderer.transform.parent = LimePlayer01.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.IsDead && MatchType != 2) LimePlayer01Wep.SetActive(false);

                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                LimePlayer01MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = LimePlayer01.transform.position + new Vector3(0.8f * (float)Math.Cos(LimePlayer01MouseAngle), 0.8f * (float)Math.Sin(LimePlayer01MouseAngle));
                LimePlayer01Wep.transform.position += (targetPosition - LimePlayer01Wep.transform.position) * 0.4f;
                LimePlayer01Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((LimePlayer01MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(LimePlayer01MouseAngle) < 0.0)
                {
                    if (LimePlayer01Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        LimePlayer01Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (LimePlayer01Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        LimePlayer01Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            var canUse = true;
            if (LimePlayer01IsReviving) canUse = false;
            return canUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _limePlayer01KillButton.Timer = _limePlayer01KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, KeyCode.Mouse1, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // LimePlayer02 Kill
        _limePlayer02KillButton = new(() =>
        {
            var target = GetLimeShotPlayer(2 * 0.2f, 6, 2);

            var writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, SendOption.Reliable);
            writerShot.Write(LimePlayer02.PlayerId);
            writerShot.Write(1);
            writerShot.Write(LimePlayer02MouseAngle);
            AmongUsClient.Instance.FinishRpcImmediately(writerShot);
            RPCProcedure.BattleRoyaleShowShoots(LimePlayer02.PlayerId, 1, LimePlayer02MouseAngle);

            if (target == null)
            {
                _limePlayer02KillButton.Timer = _limePlayer02KillButton.MaxTimer;
                return;
            }

            if (MatchType == 2)
            {
                if ((PinkPlayer01 != null && target == PinkPlayer01 && PinkPlayer01IsReviving) || (PinkPlayer02 != null && target == PinkPlayer02 && PinkPlayer02IsReviving) || (PinkPlayer03 != null && target == PinkPlayer03 && PinkPlayer03IsReviving) || (PinkPlayer04 != null && target == PinkPlayer04 && PinkPlayer04IsReviving) || (PinkPlayer05 != null && target == PinkPlayer05 && PinkPlayer05IsReviving) || (PinkPlayer06 != null && target == PinkPlayer06 && PinkPlayer06IsReviving) || (PinkPlayer07 != null && target == PinkPlayer07 && PinkPlayer07IsReviving) || (SerialKiller != null && target == SerialKiller && SerialKillerIsReviving))
                {
                    _limePlayer02KillButton.Timer = _limePlayer02KillButton.MaxTimer;
                    return;
                }
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            writer.Write(target.PlayerId);
            writer.Write(LimePlayer02.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.GamemodeKills(target.PlayerId, LimePlayer02.PlayerId);

            _limePlayer02KillButton.Timer = _limePlayer02KillButton.MaxTimer;
            SoundManager.Instance.PlaySound(AssetLoader.RoyaleHitPlayer, false, 100f);

            target = null;
        }, () => { return LimePlayer02 != null && LimePlayer02 == PlayerControl.LocalPlayer; }, () =>
        {
            if (LimePlayer02Wep == null)
            {
                LimePlayer02Wep = new("Weapon");
                var renderer = LimePlayer02Wep.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.Bow;
                renderer.transform.parent = LimePlayer02.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.IsDead && MatchType != 2) LimePlayer02Wep.SetActive(false);

                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                LimePlayer02MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = LimePlayer02.transform.position + new Vector3(0.8f * (float)Math.Cos(LimePlayer02MouseAngle), 0.8f * (float)Math.Sin(LimePlayer02MouseAngle));
                LimePlayer02Wep.transform.position += (targetPosition - LimePlayer02Wep.transform.position) * 0.4f;
                LimePlayer02Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((LimePlayer02MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(LimePlayer02MouseAngle) < 0.0)
                {
                    if (LimePlayer02Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        LimePlayer02Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (LimePlayer02Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        LimePlayer02Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            var canUse = true;
            if (LimePlayer02IsReviving) canUse = false;
            return canUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _limePlayer02KillButton.Timer = _limePlayer02KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, KeyCode.Mouse1, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // LimePlayer03 Kill
        _limePlayer03KillButton = new(() =>
        {
            var target = GetLimeShotPlayer(2 * 0.2f, 6, 3);

            var writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, SendOption.Reliable);
            writerShot.Write(LimePlayer03.PlayerId);
            writerShot.Write(1);
            writerShot.Write(LimePlayer03MouseAngle);
            AmongUsClient.Instance.FinishRpcImmediately(writerShot);
            RPCProcedure.BattleRoyaleShowShoots(LimePlayer03.PlayerId, 1, LimePlayer03MouseAngle);

            if (target == null)
            {
                _limePlayer03KillButton.Timer = _limePlayer03KillButton.MaxTimer;
                return;
            }

            if (MatchType == 2)
            {
                if ((PinkPlayer01 != null && target == PinkPlayer01 && PinkPlayer01IsReviving) || (PinkPlayer02 != null && target == PinkPlayer02 && PinkPlayer02IsReviving) || (PinkPlayer03 != null && target == PinkPlayer03 && PinkPlayer03IsReviving) || (PinkPlayer04 != null && target == PinkPlayer04 && PinkPlayer04IsReviving) || (PinkPlayer05 != null && target == PinkPlayer05 && PinkPlayer05IsReviving) || (PinkPlayer06 != null && target == PinkPlayer06 && PinkPlayer06IsReviving) || (PinkPlayer07 != null && target == PinkPlayer07 && PinkPlayer07IsReviving) || (SerialKiller != null && target == SerialKiller && SerialKillerIsReviving))
                {
                    _limePlayer03KillButton.Timer = _limePlayer03KillButton.MaxTimer;
                    return;
                }
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            writer.Write(target.PlayerId);
            writer.Write(LimePlayer03.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.GamemodeKills(target.PlayerId, LimePlayer03.PlayerId);

            _limePlayer03KillButton.Timer = _limePlayer03KillButton.MaxTimer;
            SoundManager.Instance.PlaySound(AssetLoader.RoyaleHitPlayer, false, 100f);

            target = null;
        }, () => { return LimePlayer03 != null && LimePlayer03 == PlayerControl.LocalPlayer; }, () =>
        {
            if (LimePlayer03Wep == null)
            {
                LimePlayer03Wep = new("Weapon");
                var renderer = LimePlayer03Wep.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.Bow;
                renderer.transform.parent = LimePlayer03.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.IsDead && MatchType != 2) LimePlayer03Wep.SetActive(false);

                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                LimePlayer03MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = LimePlayer03.transform.position + new Vector3(0.8f * (float)Math.Cos(LimePlayer03MouseAngle), 0.8f * (float)Math.Sin(LimePlayer03MouseAngle));
                LimePlayer03Wep.transform.position += (targetPosition - LimePlayer03Wep.transform.position) * 0.4f;
                LimePlayer03Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((LimePlayer03MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(LimePlayer03MouseAngle) < 0.0)
                {
                    if (LimePlayer03Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        LimePlayer03Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (LimePlayer03Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        LimePlayer03Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            var canUse = true;
            if (LimePlayer03IsReviving) canUse = false;
            return canUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _limePlayer03KillButton.Timer = _limePlayer03KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, KeyCode.Mouse1, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // LimePlayer04 Kill
        _limePlayer04KillButton = new(() =>
        {
            var target = GetLimeShotPlayer(2 * 0.2f, 6, 4);

            var writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, SendOption.Reliable);
            writerShot.Write(LimePlayer04.PlayerId);
            writerShot.Write(1);
            writerShot.Write(LimePlayer04MouseAngle);
            AmongUsClient.Instance.FinishRpcImmediately(writerShot);
            RPCProcedure.BattleRoyaleShowShoots(LimePlayer04.PlayerId, 1, LimePlayer04MouseAngle);

            if (target == null)
            {
                _limePlayer04KillButton.Timer = _limePlayer04KillButton.MaxTimer;
                return;
            }

            if (MatchType == 2)
            {
                if ((PinkPlayer01 != null && target == PinkPlayer01 && PinkPlayer01IsReviving) || (PinkPlayer02 != null && target == PinkPlayer02 && PinkPlayer02IsReviving) || (PinkPlayer03 != null && target == PinkPlayer03 && PinkPlayer03IsReviving) || (PinkPlayer04 != null && target == PinkPlayer04 && PinkPlayer04IsReviving) || (PinkPlayer05 != null && target == PinkPlayer05 && PinkPlayer05IsReviving) || (PinkPlayer06 != null && target == PinkPlayer06 && PinkPlayer06IsReviving) || (PinkPlayer07 != null && target == PinkPlayer07 && PinkPlayer07IsReviving) || (SerialKiller != null && target == SerialKiller && SerialKillerIsReviving))
                {
                    _limePlayer04KillButton.Timer = _limePlayer04KillButton.MaxTimer;
                    return;
                }
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            writer.Write(target.PlayerId);
            writer.Write(LimePlayer04.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.GamemodeKills(target.PlayerId, LimePlayer04.PlayerId);

            _limePlayer04KillButton.Timer = _limePlayer04KillButton.MaxTimer;
            SoundManager.Instance.PlaySound(AssetLoader.RoyaleHitPlayer, false, 100f);

            target = null;
        }, () => { return LimePlayer04 != null && LimePlayer04 == PlayerControl.LocalPlayer; }, () =>
        {
            if (LimePlayer04Wep == null)
            {
                LimePlayer04Wep = new("Weapon");
                var renderer = LimePlayer04Wep.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.Bow;
                renderer.transform.parent = LimePlayer04.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.IsDead && MatchType != 2) LimePlayer04Wep.SetActive(false);

                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                LimePlayer04MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = LimePlayer04.transform.position + new Vector3(0.8f * (float)Math.Cos(LimePlayer04MouseAngle), 0.8f * (float)Math.Sin(LimePlayer04MouseAngle));
                LimePlayer04Wep.transform.position += (targetPosition - LimePlayer04Wep.transform.position) * 0.4f;
                LimePlayer04Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((LimePlayer04MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(LimePlayer04MouseAngle) < 0.0)
                {
                    if (LimePlayer04Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        LimePlayer04Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (LimePlayer04Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        LimePlayer04Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            var canUse = true;
            if (LimePlayer04IsReviving) canUse = false;
            return canUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _limePlayer04KillButton.Timer = _limePlayer04KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, KeyCode.Mouse1, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // LimePlayer05 Kill
        _limePlayer05KillButton = new(() =>
        {
            var target = GetLimeShotPlayer(2 * 0.2f, 6, 5);

            var writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, SendOption.Reliable);
            writerShot.Write(LimePlayer05.PlayerId);
            writerShot.Write(1);
            writerShot.Write(LimePlayer05MouseAngle);
            AmongUsClient.Instance.FinishRpcImmediately(writerShot);
            RPCProcedure.BattleRoyaleShowShoots(LimePlayer05.PlayerId, 1, LimePlayer05MouseAngle);

            if (target == null)
            {
                _limePlayer05KillButton.Timer = _limePlayer05KillButton.MaxTimer;
                return;
            }

            if (MatchType == 2)
            {
                if ((PinkPlayer01 != null && target == PinkPlayer01 && PinkPlayer01IsReviving) || (PinkPlayer02 != null && target == PinkPlayer02 && PinkPlayer02IsReviving) || (PinkPlayer03 != null && target == PinkPlayer03 && PinkPlayer03IsReviving) || (PinkPlayer04 != null && target == PinkPlayer04 && PinkPlayer04IsReviving) || (PinkPlayer05 != null && target == PinkPlayer05 && PinkPlayer05IsReviving) || (PinkPlayer06 != null && target == PinkPlayer06 && PinkPlayer06IsReviving) || (PinkPlayer07 != null && target == PinkPlayer07 && PinkPlayer07IsReviving) || (SerialKiller != null && target == SerialKiller && SerialKillerIsReviving))
                {
                    _limePlayer05KillButton.Timer = _limePlayer05KillButton.MaxTimer;
                    return;
                }
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            writer.Write(target.PlayerId);
            writer.Write(LimePlayer05.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.GamemodeKills(target.PlayerId, LimePlayer05.PlayerId);

            _limePlayer05KillButton.Timer = _limePlayer05KillButton.MaxTimer;
            SoundManager.Instance.PlaySound(AssetLoader.RoyaleHitPlayer, false, 100f);

            target = null;
        }, () => { return LimePlayer05 != null && LimePlayer05 == PlayerControl.LocalPlayer; }, () =>
        {
            if (LimePlayer05Wep == null)
            {
                LimePlayer05Wep = new("Weapon");
                var renderer = LimePlayer05Wep.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.Bow;
                renderer.transform.parent = LimePlayer05.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.IsDead && MatchType != 2) LimePlayer05Wep.SetActive(false);

                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                LimePlayer05MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = LimePlayer05.transform.position + new Vector3(0.8f * (float)Math.Cos(LimePlayer05MouseAngle), 0.8f * (float)Math.Sin(LimePlayer05MouseAngle));
                LimePlayer05Wep.transform.position += (targetPosition - LimePlayer05Wep.transform.position) * 0.4f;
                LimePlayer05Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((LimePlayer05MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(LimePlayer05MouseAngle) < 0.0)
                {
                    if (LimePlayer05Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        LimePlayer05Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (LimePlayer05Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        LimePlayer05Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            var canUse = true;
            if (LimePlayer05IsReviving) canUse = false;
            return canUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _limePlayer05KillButton.Timer = _limePlayer05KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, KeyCode.Mouse1, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // LimePlayer06 Kill
        _limePlayer06KillButton = new(() =>
        {
            var target = GetLimeShotPlayer(2 * 0.2f, 6, 6);

            var writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, SendOption.Reliable);
            writerShot.Write(LimePlayer06.PlayerId);
            writerShot.Write(1);
            writerShot.Write(LimePlayer06MouseAngle);
            AmongUsClient.Instance.FinishRpcImmediately(writerShot);
            RPCProcedure.BattleRoyaleShowShoots(LimePlayer06.PlayerId, 1, LimePlayer06MouseAngle);

            if (target == null)
            {
                _limePlayer06KillButton.Timer = _limePlayer06KillButton.MaxTimer;
                return;
            }

            if (MatchType == 2)
            {
                if ((PinkPlayer01 != null && target == PinkPlayer01 && PinkPlayer01IsReviving) || (PinkPlayer02 != null && target == PinkPlayer02 && PinkPlayer02IsReviving) || (PinkPlayer03 != null && target == PinkPlayer03 && PinkPlayer03IsReviving) || (PinkPlayer04 != null && target == PinkPlayer04 && PinkPlayer04IsReviving) || (PinkPlayer05 != null && target == PinkPlayer05 && PinkPlayer05IsReviving) || (PinkPlayer06 != null && target == PinkPlayer06 && PinkPlayer06IsReviving) || (PinkPlayer07 != null && target == PinkPlayer07 && PinkPlayer07IsReviving) || (SerialKiller != null && target == SerialKiller && SerialKillerIsReviving))
                {
                    _limePlayer06KillButton.Timer = _limePlayer06KillButton.MaxTimer;
                    return;
                }
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            writer.Write(target.PlayerId);
            writer.Write(LimePlayer06.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.GamemodeKills(target.PlayerId, LimePlayer06.PlayerId);

            _limePlayer06KillButton.Timer = _limePlayer06KillButton.MaxTimer;
            SoundManager.Instance.PlaySound(AssetLoader.RoyaleHitPlayer, false, 100f);

            target = null;
        }, () => { return LimePlayer06 != null && LimePlayer06 == PlayerControl.LocalPlayer; }, () =>
        {
            if (LimePlayer06Wep == null)
            {
                LimePlayer06Wep = new("Weapon");
                var renderer = LimePlayer06Wep.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.Bow;
                renderer.transform.parent = LimePlayer06.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.IsDead && MatchType != 2) LimePlayer06Wep.SetActive(false);

                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                LimePlayer06MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = LimePlayer06.transform.position + new Vector3(0.8f * (float)Math.Cos(LimePlayer06MouseAngle), 0.8f * (float)Math.Sin(LimePlayer06MouseAngle));
                LimePlayer06Wep.transform.position += (targetPosition - LimePlayer06Wep.transform.position) * 0.4f;
                LimePlayer06Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((LimePlayer06MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(LimePlayer06MouseAngle) < 0.0)
                {
                    if (LimePlayer06Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        LimePlayer06Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (LimePlayer06Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        LimePlayer06Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            var canUse = true;
            if (LimePlayer06IsReviving) canUse = false;
            return canUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _limePlayer06KillButton.Timer = _limePlayer06KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, KeyCode.Mouse1, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // LimePlayer07 Kill
        _limePlayer07KillButton = new(() =>
        {
            var target = GetLimeShotPlayer(2 * 0.2f, 6, 7);

            var writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, SendOption.Reliable);
            writerShot.Write(LimePlayer07.PlayerId);
            writerShot.Write(1);
            writerShot.Write(LimePlayer07MouseAngle);
            AmongUsClient.Instance.FinishRpcImmediately(writerShot);
            RPCProcedure.BattleRoyaleShowShoots(LimePlayer07.PlayerId, 1, LimePlayer07MouseAngle);

            if (target == null)
            {
                _limePlayer07KillButton.Timer = _limePlayer07KillButton.MaxTimer;
                return;
            }

            if (MatchType == 2)
            {
                if ((PinkPlayer01 != null && target == PinkPlayer01 && PinkPlayer01IsReviving) || (PinkPlayer02 != null && target == PinkPlayer02 && PinkPlayer02IsReviving) || (PinkPlayer03 != null && target == PinkPlayer03 && PinkPlayer03IsReviving) || (PinkPlayer04 != null && target == PinkPlayer04 && PinkPlayer04IsReviving) || (PinkPlayer05 != null && target == PinkPlayer05 && PinkPlayer05IsReviving) || (PinkPlayer06 != null && target == PinkPlayer06 && PinkPlayer06IsReviving) || (PinkPlayer07 != null && target == PinkPlayer07 && PinkPlayer07IsReviving) || (SerialKiller != null && target == SerialKiller && SerialKillerIsReviving))
                {
                    _limePlayer07KillButton.Timer = _limePlayer07KillButton.MaxTimer;
                    return;
                }
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            writer.Write(target.PlayerId);
            writer.Write(LimePlayer07.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.GamemodeKills(target.PlayerId, LimePlayer07.PlayerId);

            _limePlayer07KillButton.Timer = _limePlayer07KillButton.MaxTimer;
            SoundManager.Instance.PlaySound(AssetLoader.RoyaleHitPlayer, false, 100f);

            target = null;
        }, () => { return LimePlayer07 != null && LimePlayer07 == PlayerControl.LocalPlayer; }, () =>
        {
            if (LimePlayer07Wep == null)
            {
                LimePlayer07Wep = new("Weapon");
                var renderer = LimePlayer07Wep.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.Bow;
                renderer.transform.parent = LimePlayer07.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.IsDead && MatchType != 2) LimePlayer07Wep.SetActive(false);

                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                LimePlayer07MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = LimePlayer07.transform.position + new Vector3(0.8f * (float)Math.Cos(LimePlayer07MouseAngle), 0.8f * (float)Math.Sin(LimePlayer07MouseAngle));
                LimePlayer07Wep.transform.position += (targetPosition - LimePlayer07Wep.transform.position) * 0.4f;
                LimePlayer07Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((LimePlayer07MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(LimePlayer07MouseAngle) < 0.0)
                {
                    if (LimePlayer07Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        LimePlayer07Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (LimePlayer07Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        LimePlayer07Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            var canUse = true;
            if (LimePlayer07IsReviving) canUse = false;
            return canUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _limePlayer07KillButton.Timer = _limePlayer07KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, KeyCode.Mouse1, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // PinkPlayer01 Kill
        _pinkPlayer01KillButton = new(() =>
        {
            var target = GetPinkShotPlayer(2 * 0.2f, 6, 1);

            var writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, SendOption.Reliable);
            writerShot.Write(PinkPlayer01.PlayerId);
            writerShot.Write(2);
            writerShot.Write(PinkPlayer01MouseAngle);
            AmongUsClient.Instance.FinishRpcImmediately(writerShot);
            RPCProcedure.BattleRoyaleShowShoots(PinkPlayer01.PlayerId, 2, PinkPlayer01MouseAngle);

            if (target == null)
            {
                _pinkPlayer01KillButton.Timer = _pinkPlayer01KillButton.MaxTimer;
                return;
            }

            if (MatchType == 2)
            {
                if ((LimePlayer01 != null && target == LimePlayer01 && LimePlayer01IsReviving) || (LimePlayer02 != null && target == LimePlayer02 && LimePlayer02IsReviving) || (LimePlayer03 != null && target == LimePlayer03 && LimePlayer03IsReviving) || (LimePlayer04 != null && target == LimePlayer04 && LimePlayer04IsReviving) || (LimePlayer05 != null && target == LimePlayer05 && LimePlayer05IsReviving) || (LimePlayer06 != null && target == LimePlayer06 && LimePlayer06IsReviving) || (LimePlayer07 != null && target == LimePlayer07 && LimePlayer07IsReviving) || (SerialKiller != null && target == SerialKiller && SerialKillerIsReviving))
                {
                    _pinkPlayer01KillButton.Timer = _pinkPlayer01KillButton.MaxTimer;
                    return;
                }
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            writer.Write(target.PlayerId);
            writer.Write(PinkPlayer01.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.GamemodeKills(target.PlayerId, PinkPlayer01.PlayerId);

            _pinkPlayer01KillButton.Timer = _pinkPlayer01KillButton.MaxTimer;
            SoundManager.Instance.PlaySound(AssetLoader.RoyaleHitPlayer, false, 100f);

            target = null;
        }, () => { return PinkPlayer01 != null && PinkPlayer01 == PlayerControl.LocalPlayer; }, () =>
        {
            if (PinkPlayer01Wep == null)
            {
                PinkPlayer01Wep = new("Weapon");
                var renderer = PinkPlayer01Wep.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.Bow;
                renderer.transform.parent = PinkPlayer01.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.IsDead && MatchType != 2) PinkPlayer01Wep.SetActive(false);

                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                PinkPlayer01MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = PinkPlayer01.transform.position + new Vector3(0.8f * (float)Math.Cos(PinkPlayer01MouseAngle), 0.8f * (float)Math.Sin(PinkPlayer01MouseAngle));
                PinkPlayer01Wep.transform.position += (targetPosition - PinkPlayer01Wep.transform.position) * 0.4f;
                PinkPlayer01Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((PinkPlayer01MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(PinkPlayer01MouseAngle) < 0.0)
                {
                    if (PinkPlayer01Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        PinkPlayer01Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (PinkPlayer01Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        PinkPlayer01Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            var canUse = true;
            if (PinkPlayer01IsReviving) canUse = false;
            return canUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _pinkPlayer01KillButton.Timer = _pinkPlayer01KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, KeyCode.Mouse1, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // PinkPlayer02 Kill
        _pinkPlayer02KillButton = new(() =>
        {
            var target = GetPinkShotPlayer(2 * 0.2f, 6, 2);

            var writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, SendOption.Reliable);
            writerShot.Write(PinkPlayer02.PlayerId);
            writerShot.Write(2);
            writerShot.Write(PinkPlayer02MouseAngle);
            AmongUsClient.Instance.FinishRpcImmediately(writerShot);
            RPCProcedure.BattleRoyaleShowShoots(PinkPlayer02.PlayerId, 2, PinkPlayer02MouseAngle);

            if (target == null)
            {
                _pinkPlayer02KillButton.Timer = _pinkPlayer02KillButton.MaxTimer;
                return;
            }

            if (MatchType == 2)
            {
                if ((LimePlayer01 != null && target == LimePlayer01 && LimePlayer01IsReviving) || (LimePlayer02 != null && target == LimePlayer02 && LimePlayer02IsReviving) || (LimePlayer03 != null && target == LimePlayer03 && LimePlayer03IsReviving) || (LimePlayer04 != null && target == LimePlayer04 && LimePlayer04IsReviving) || (LimePlayer05 != null && target == LimePlayer05 && LimePlayer05IsReviving) || (LimePlayer06 != null && target == LimePlayer06 && LimePlayer06IsReviving) || (LimePlayer07 != null && target == LimePlayer07 && LimePlayer07IsReviving) || (SerialKiller != null && target == SerialKiller && SerialKillerIsReviving))
                {
                    _pinkPlayer02KillButton.Timer = _pinkPlayer02KillButton.MaxTimer;
                    return;
                }
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            writer.Write(target.PlayerId);
            writer.Write(PinkPlayer02.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.GamemodeKills(target.PlayerId, PinkPlayer02.PlayerId);

            _pinkPlayer02KillButton.Timer = _pinkPlayer02KillButton.MaxTimer;
            SoundManager.Instance.PlaySound(AssetLoader.RoyaleHitPlayer, false, 100f);

            target = null;
        }, () => { return PinkPlayer02 != null && PinkPlayer02 == PlayerControl.LocalPlayer; }, () =>
        {
            if (PinkPlayer02Wep == null)
            {
                PinkPlayer02Wep = new("Weapon");
                var renderer = PinkPlayer02Wep.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.Bow;
                renderer.transform.parent = PinkPlayer02.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.IsDead && MatchType != 2) PinkPlayer02Wep.SetActive(false);

                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                PinkPlayer02MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = PinkPlayer02.transform.position + new Vector3(0.8f * (float)Math.Cos(PinkPlayer02MouseAngle), 0.8f * (float)Math.Sin(PinkPlayer02MouseAngle));
                PinkPlayer02Wep.transform.position += (targetPosition - PinkPlayer02Wep.transform.position) * 0.4f;
                PinkPlayer02Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((PinkPlayer02MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(PinkPlayer02MouseAngle) < 0.0)
                {
                    if (PinkPlayer02Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        PinkPlayer02Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (PinkPlayer02Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        PinkPlayer02Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            var canUse = true;
            if (PinkPlayer02IsReviving) canUse = false;
            return canUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _pinkPlayer02KillButton.Timer = _pinkPlayer02KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, KeyCode.Mouse1, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // PinkPlayer03 Kill
        _pinkPlayer03KillButton = new(() =>
        {
            var target = GetPinkShotPlayer(2 * 0.2f, 6, 3);

            var writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, SendOption.Reliable);
            writerShot.Write(PinkPlayer03.PlayerId);
            writerShot.Write(2);
            writerShot.Write(PinkPlayer03MouseAngle);
            AmongUsClient.Instance.FinishRpcImmediately(writerShot);
            RPCProcedure.BattleRoyaleShowShoots(PinkPlayer03.PlayerId, 2, PinkPlayer03MouseAngle);

            if (target == null)
            {
                _pinkPlayer03KillButton.Timer = _pinkPlayer03KillButton.MaxTimer;
                return;
            }

            if (MatchType == 2)
            {
                if ((LimePlayer01 != null && target == LimePlayer01 && LimePlayer01IsReviving) || (LimePlayer02 != null && target == LimePlayer02 && LimePlayer02IsReviving) || (LimePlayer03 != null && target == LimePlayer03 && LimePlayer03IsReviving) || (LimePlayer04 != null && target == LimePlayer04 && LimePlayer04IsReviving) || (LimePlayer05 != null && target == LimePlayer05 && LimePlayer05IsReviving) || (LimePlayer06 != null && target == LimePlayer06 && LimePlayer06IsReviving) || (LimePlayer07 != null && target == LimePlayer07 && LimePlayer07IsReviving) || (SerialKiller != null && target == SerialKiller && SerialKillerIsReviving))
                {
                    _pinkPlayer03KillButton.Timer = _pinkPlayer03KillButton.MaxTimer;
                    return;
                }
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            writer.Write(target.PlayerId);
            writer.Write(PinkPlayer03.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.GamemodeKills(target.PlayerId, PinkPlayer03.PlayerId);

            _pinkPlayer03KillButton.Timer = _pinkPlayer03KillButton.MaxTimer;
            SoundManager.Instance.PlaySound(AssetLoader.RoyaleHitPlayer, false, 100f);

            target = null;
        }, () => { return PinkPlayer03 != null && PinkPlayer03 == PlayerControl.LocalPlayer; }, () =>
        {
            if (PinkPlayer03Wep == null)
            {
                PinkPlayer03Wep = new("Weapon");
                var renderer = PinkPlayer03Wep.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.Bow;
                renderer.transform.parent = PinkPlayer03.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.IsDead && MatchType != 2) PinkPlayer03Wep.SetActive(false);

                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                PinkPlayer03MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = PinkPlayer03.transform.position + new Vector3(0.8f * (float)Math.Cos(PinkPlayer03MouseAngle), 0.8f * (float)Math.Sin(PinkPlayer03MouseAngle));
                PinkPlayer03Wep.transform.position += (targetPosition - PinkPlayer03Wep.transform.position) * 0.4f;
                PinkPlayer03Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((PinkPlayer03MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(PinkPlayer03MouseAngle) < 0.0)
                {
                    if (PinkPlayer03Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        PinkPlayer03Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (PinkPlayer03Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        PinkPlayer03Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            var canUse = true;
            if (PinkPlayer03IsReviving) canUse = false;
            return canUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _pinkPlayer03KillButton.Timer = _pinkPlayer03KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, KeyCode.Mouse1, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // PinkPlayer04 Kill
        _pinkPlayer04KillButton = new(() =>
        {
            var target = GetPinkShotPlayer(2 * 0.2f, 6, 4);

            var writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, SendOption.Reliable);
            writerShot.Write(PinkPlayer04.PlayerId);
            writerShot.Write(2);
            writerShot.Write(PinkPlayer04MouseAngle);
            AmongUsClient.Instance.FinishRpcImmediately(writerShot);
            RPCProcedure.BattleRoyaleShowShoots(PinkPlayer04.PlayerId, 2, PinkPlayer04MouseAngle);

            if (target == null)
            {
                _pinkPlayer04KillButton.Timer = _pinkPlayer04KillButton.MaxTimer;
                return;
            }

            if (MatchType == 2)
            {
                if ((LimePlayer01 != null && target == LimePlayer01 && LimePlayer01IsReviving) || (LimePlayer02 != null && target == LimePlayer02 && LimePlayer02IsReviving) || (LimePlayer03 != null && target == LimePlayer03 && LimePlayer03IsReviving) || (LimePlayer04 != null && target == LimePlayer04 && LimePlayer04IsReviving) || (LimePlayer05 != null && target == LimePlayer05 && LimePlayer05IsReviving) || (LimePlayer06 != null && target == LimePlayer06 && LimePlayer06IsReviving) || (LimePlayer07 != null && target == LimePlayer07 && LimePlayer07IsReviving) || (SerialKiller != null && target == SerialKiller && SerialKillerIsReviving))
                {
                    _pinkPlayer04KillButton.Timer = _pinkPlayer04KillButton.MaxTimer;
                    return;
                }
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            writer.Write(target.PlayerId);
            writer.Write(PinkPlayer04.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.GamemodeKills(target.PlayerId, PinkPlayer04.PlayerId);

            _pinkPlayer04KillButton.Timer = _pinkPlayer04KillButton.MaxTimer;
            SoundManager.Instance.PlaySound(AssetLoader.RoyaleHitPlayer, false, 100f);

            target = null;
        }, () => { return PinkPlayer04 != null && PinkPlayer04 == PlayerControl.LocalPlayer; }, () =>
        {
            if (PinkPlayer04Wep == null)
            {
                PinkPlayer04Wep = new("Weapon");
                var renderer = PinkPlayer04Wep.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.Bow;
                renderer.transform.parent = PinkPlayer04.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.IsDead && MatchType != 2) PinkPlayer04Wep.SetActive(false);

                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                PinkPlayer04MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = PinkPlayer04.transform.position + new Vector3(0.8f * (float)Math.Cos(PinkPlayer04MouseAngle), 0.8f * (float)Math.Sin(PinkPlayer04MouseAngle));
                PinkPlayer04Wep.transform.position += (targetPosition - PinkPlayer04Wep.transform.position) * 0.4f;
                PinkPlayer04Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((PinkPlayer04MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(PinkPlayer04MouseAngle) < 0.0)
                {
                    if (PinkPlayer04Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        PinkPlayer04Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (PinkPlayer04Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        PinkPlayer04Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            var canUse = true;
            if (PinkPlayer04IsReviving) canUse = false;
            return canUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _pinkPlayer04KillButton.Timer = _pinkPlayer04KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, KeyCode.Mouse1, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // PinkPlayer05 Kill
        _pinkPlayer05KillButton = new(() =>
        {
            var target = GetPinkShotPlayer(2 * 0.2f, 6, 5);

            var writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, SendOption.Reliable);
            writerShot.Write(PinkPlayer05.PlayerId);
            writerShot.Write(2);
            writerShot.Write(PinkPlayer05MouseAngle);
            AmongUsClient.Instance.FinishRpcImmediately(writerShot);
            RPCProcedure.BattleRoyaleShowShoots(PinkPlayer05.PlayerId, 2, PinkPlayer05MouseAngle);

            if (target == null)
            {
                _pinkPlayer05KillButton.Timer = _pinkPlayer05KillButton.MaxTimer;
                return;
            }

            if (MatchType == 2)
            {
                if ((LimePlayer01 != null && target == LimePlayer01 && LimePlayer01IsReviving) || (LimePlayer02 != null && target == LimePlayer02 && LimePlayer02IsReviving) || (LimePlayer03 != null && target == LimePlayer03 && LimePlayer03IsReviving) || (LimePlayer04 != null && target == LimePlayer04 && LimePlayer04IsReviving) || (LimePlayer05 != null && target == LimePlayer05 && LimePlayer05IsReviving) || (LimePlayer06 != null && target == LimePlayer06 && LimePlayer06IsReviving) || (LimePlayer07 != null && target == LimePlayer07 && LimePlayer07IsReviving) || (SerialKiller != null && target == SerialKiller && SerialKillerIsReviving))
                {
                    _pinkPlayer05KillButton.Timer = _pinkPlayer05KillButton.MaxTimer;
                    return;
                }
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            writer.Write(target.PlayerId);
            writer.Write(PinkPlayer05.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.GamemodeKills(target.PlayerId, PinkPlayer05.PlayerId);

            _pinkPlayer05KillButton.Timer = _pinkPlayer05KillButton.MaxTimer;
            SoundManager.Instance.PlaySound(AssetLoader.RoyaleHitPlayer, false, 100f);

            target = null;
        }, () => { return PinkPlayer05 != null && PinkPlayer05 == PlayerControl.LocalPlayer; }, () =>
        {
            if (PinkPlayer05Wep == null)
            {
                PinkPlayer05Wep = new("Weapon");
                var renderer = PinkPlayer05Wep.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.Bow;
                renderer.transform.parent = PinkPlayer05.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.IsDead && MatchType != 2) PinkPlayer05Wep.SetActive(false);

                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                PinkPlayer05MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = PinkPlayer05.transform.position + new Vector3(0.8f * (float)Math.Cos(PinkPlayer05MouseAngle), 0.8f * (float)Math.Sin(PinkPlayer05MouseAngle));
                PinkPlayer05Wep.transform.position += (targetPosition - PinkPlayer05Wep.transform.position) * 0.4f;
                PinkPlayer05Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((PinkPlayer05MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(PinkPlayer05MouseAngle) < 0.0)
                {
                    if (PinkPlayer05Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        PinkPlayer05Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (PinkPlayer05Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        PinkPlayer05Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            var canUse = true;
            if (PinkPlayer05IsReviving) canUse = false;
            return canUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _pinkPlayer05KillButton.Timer = _pinkPlayer05KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, KeyCode.Mouse1, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // PinkPlayer06 Kill
        _pinkPlayer06KillButton = new(() =>
        {
            var target = GetPinkShotPlayer(2 * 0.2f, 6, 6);

            var writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, SendOption.Reliable);
            writerShot.Write(PinkPlayer06.PlayerId);
            writerShot.Write(2);
            writerShot.Write(PinkPlayer06MouseAngle);
            AmongUsClient.Instance.FinishRpcImmediately(writerShot);
            RPCProcedure.BattleRoyaleShowShoots(PinkPlayer06.PlayerId, 2, PinkPlayer06MouseAngle);

            if (target == null)
            {
                _pinkPlayer06KillButton.Timer = _pinkPlayer06KillButton.MaxTimer;
                return;
            }

            if (MatchType == 2)
            {
                if ((LimePlayer01 != null && target == LimePlayer01 && LimePlayer01IsReviving) || (LimePlayer02 != null && target == LimePlayer02 && LimePlayer02IsReviving) || (LimePlayer03 != null && target == LimePlayer03 && LimePlayer03IsReviving) || (LimePlayer04 != null && target == LimePlayer04 && LimePlayer04IsReviving) || (LimePlayer05 != null && target == LimePlayer05 && LimePlayer05IsReviving) || (LimePlayer06 != null && target == LimePlayer06 && LimePlayer06IsReviving) || (LimePlayer07 != null && target == LimePlayer07 && LimePlayer07IsReviving) || (SerialKiller != null && target == SerialKiller && SerialKillerIsReviving))
                {
                    _pinkPlayer06KillButton.Timer = _pinkPlayer06KillButton.MaxTimer;
                    return;
                }
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            writer.Write(target.PlayerId);
            writer.Write(PinkPlayer06.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.GamemodeKills(target.PlayerId, PinkPlayer06.PlayerId);

            _pinkPlayer06KillButton.Timer = _pinkPlayer06KillButton.MaxTimer;
            SoundManager.Instance.PlaySound(AssetLoader.RoyaleHitPlayer, false, 100f);

            target = null;
        }, () => { return PinkPlayer06 != null && PinkPlayer06 == PlayerControl.LocalPlayer; }, () =>
        {
            if (PinkPlayer06Wep == null)
            {
                PinkPlayer06Wep = new("Weapon");
                var renderer = PinkPlayer06Wep.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.Bow;
                renderer.transform.parent = PinkPlayer06.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.IsDead && MatchType != 2) PinkPlayer06Wep.SetActive(false);

                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                PinkPlayer06MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = PinkPlayer06.transform.position + new Vector3(0.8f * (float)Math.Cos(PinkPlayer06MouseAngle), 0.8f * (float)Math.Sin(PinkPlayer06MouseAngle));
                PinkPlayer06Wep.transform.position += (targetPosition - PinkPlayer06Wep.transform.position) * 0.4f;
                PinkPlayer06Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((PinkPlayer06MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(PinkPlayer06MouseAngle) < 0.0)
                {
                    if (PinkPlayer06Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        PinkPlayer06Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (PinkPlayer06Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        PinkPlayer06Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            var canUse = true;
            if (PinkPlayer06IsReviving) canUse = false;
            return canUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _pinkPlayer06KillButton.Timer = _pinkPlayer06KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, KeyCode.Mouse1, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // PinkPlayer07 Kill
        _pinkPlayer07KillButton = new(() =>
        {
            var target = GetPinkShotPlayer(2 * 0.2f, 6, 7);

            var writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, SendOption.Reliable);
            writerShot.Write(PinkPlayer07.PlayerId);
            writerShot.Write(2);
            writerShot.Write(PinkPlayer07MouseAngle);
            AmongUsClient.Instance.FinishRpcImmediately(writerShot);
            RPCProcedure.BattleRoyaleShowShoots(PinkPlayer07.PlayerId, 2, PinkPlayer07MouseAngle);

            if (target == null)
            {
                _pinkPlayer07KillButton.Timer = _pinkPlayer07KillButton.MaxTimer;
                return;
            }

            if (MatchType == 2)
            {
                if ((LimePlayer01 != null && target == LimePlayer01 && LimePlayer01IsReviving) || (LimePlayer02 != null && target == LimePlayer02 && LimePlayer02IsReviving) || (LimePlayer03 != null && target == LimePlayer03 && LimePlayer03IsReviving) || (LimePlayer04 != null && target == LimePlayer04 && LimePlayer04IsReviving) || (LimePlayer05 != null && target == LimePlayer05 && LimePlayer05IsReviving) || (LimePlayer06 != null && target == LimePlayer06 && LimePlayer06IsReviving) || (LimePlayer07 != null && target == LimePlayer07 && LimePlayer07IsReviving) || (SerialKiller != null && target == SerialKiller && SerialKillerIsReviving))
                {
                    _pinkPlayer07KillButton.Timer = _pinkPlayer07KillButton.MaxTimer;
                    return;
                }
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            writer.Write(target.PlayerId);
            writer.Write(PinkPlayer07.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.GamemodeKills(target.PlayerId, PinkPlayer07.PlayerId);

            _pinkPlayer07KillButton.Timer = _pinkPlayer07KillButton.MaxTimer;
            SoundManager.Instance.PlaySound(AssetLoader.RoyaleHitPlayer, false, 100f);

            target = null;
        }, () => { return PinkPlayer07 != null && PinkPlayer07 == PlayerControl.LocalPlayer; }, () =>
        {
            if (PinkPlayer07Wep == null)
            {
                PinkPlayer07Wep = new("Weapon");
                var renderer = PinkPlayer07Wep.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.Bow;
                renderer.transform.parent = PinkPlayer07.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.IsDead && MatchType != 2) PinkPlayer07Wep.SetActive(false);

                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                PinkPlayer07MouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = PinkPlayer07.transform.position + new Vector3(0.8f * (float)Math.Cos(PinkPlayer07MouseAngle), 0.8f * (float)Math.Sin(PinkPlayer07MouseAngle));
                PinkPlayer07Wep.transform.position += (targetPosition - PinkPlayer07Wep.transform.position) * 0.4f;
                PinkPlayer07Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((PinkPlayer07MouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(PinkPlayer07MouseAngle) < 0.0)
                {
                    if (PinkPlayer07Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        PinkPlayer07Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (PinkPlayer07Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        PinkPlayer07Wep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            var canUse = true;
            if (PinkPlayer07IsReviving) canUse = false;
            return canUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _pinkPlayer07KillButton.Timer = _pinkPlayer07KillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, KeyCode.Mouse1, false, TranslationController.Instance.GetString(StringNames.KillLabel));

        // Serial Killer Kill
        _serialKillerKillButton = new(() =>
        {
            var target = GetSerialShootPlayer(2 * 0.2f, 6);

            var writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, SendOption.Reliable);
            writerShot.Write(SerialKiller.PlayerId);
            writerShot.Write(3);
            writerShot.Write(SerialKillermouseAngle);
            AmongUsClient.Instance.FinishRpcImmediately(writerShot);
            RPCProcedure.BattleRoyaleShowShoots(SerialKiller.PlayerId, 3, SerialKillermouseAngle);

            if (target == null)
            {
                _serialKillerKillButton.Timer = _serialKillerKillButton.MaxTimer;
                return;
            }

            if (MatchType == 2)
            {
                if ((LimePlayer01 != null && target == LimePlayer01 && LimePlayer01IsReviving) || (LimePlayer02 != null && target == LimePlayer02 && LimePlayer02IsReviving) || (LimePlayer03 != null && target == LimePlayer03 && LimePlayer03IsReviving) || (LimePlayer04 != null && target == LimePlayer04 && LimePlayer04IsReviving) || (LimePlayer05 != null && target == LimePlayer05 && LimePlayer05IsReviving) || (LimePlayer06 != null && target == LimePlayer06 && LimePlayer06IsReviving) || (LimePlayer07 != null && target == LimePlayer07 && LimePlayer07IsReviving) || (PinkPlayer01 != null && target == PinkPlayer01 && PinkPlayer01IsReviving) || (PinkPlayer02 != null && target == PinkPlayer02 && PinkPlayer02IsReviving) || (PinkPlayer03 != null && target == PinkPlayer03 && PinkPlayer03IsReviving) || (PinkPlayer04 != null && target == PinkPlayer04 && PinkPlayer04IsReviving) || (PinkPlayer05 != null && target == PinkPlayer05 && PinkPlayer05IsReviving) || (PinkPlayer06 != null && target == PinkPlayer06 && PinkPlayer06IsReviving) || (PinkPlayer07 != null && target == PinkPlayer07 && PinkPlayer07IsReviving))
                {
                    _serialKillerKillButton.Timer = _serialKillerKillButton.MaxTimer;
                    return;
                }
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, SendOption.Reliable);
            writer.Write(target.PlayerId);
            writer.Write(SerialKiller.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.GamemodeKills(target.PlayerId, SerialKiller.PlayerId);

            _serialKillerKillButton.Timer = _serialKillerKillButton.MaxTimer;
            SoundManager.Instance.PlaySound(AssetLoader.RoyaleHitPlayer, false, 100f);

            target = null;
        }, () => { return SerialKiller != null && SerialKiller == PlayerControl.LocalPlayer; }, () =>
        {
            if (SerialKillerWep == null)
            {
                SerialKillerWep = new("Weapon");
                var renderer = SerialKillerWep.AddComponent<SpriteRenderer>();

                renderer.sprite = AssetLoader.Bow;
                renderer.transform.parent = SerialKiller.transform;
                renderer.color = new(1, 1, 1, 1);
                renderer.transform.position = new(0, 0, -30f);
            }
            else
            {
                if (PlayerControl.LocalPlayer.Data.IsDead && MatchType != 2) SerialKillerWep.SetActive(false);

                var mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                SerialKillermouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                var targetPosition = SerialKiller.transform.position + new Vector3(0.8f * (float)Math.Cos(SerialKillermouseAngle), 0.8f * (float)Math.Sin(SerialKillermouseAngle));
                SerialKillerWep.transform.position += (targetPosition - SerialKillerWep.transform.position) * 0.4f;
                SerialKillerWep.GetComponent<SpriteRenderer>().transform.eulerAngles = new(0f, 0f, (float)((SerialKillermouseAngle * 360f) / Math.PI / 2f));
                if (Math.Cos(SerialKillermouseAngle) < 0.0)
                {
                    if (SerialKillerWep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                        SerialKillerWep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, -1f);
                }
                else
                {
                    if (SerialKillerWep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                        SerialKillerWep.GetComponent<SpriteRenderer>().transform.localScale = new(1f, 1f);
                }
            }

            var canUse = true;
            if (SerialKillerIsReviving) canUse = false;
            var canSpawnKill = true;
            foreach (var spawns in SerialKillerSpawns)
            {
                if (spawns != null && MatchType == 2 && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, spawns.transform.position) < 3f)
                    canSpawnKill = false;
            }

            return canUse && canSpawnKill && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
        }, () => { _serialKillerKillButton.Timer = _serialKillerKillButton.MaxTimer; }, __instance.KillButton.graphic.sprite, ButtonPosition.Layout, __instance, __instance.KillButton, KeyCode.Mouse1, false, TranslationController.Instance.GetString(StringNames.KillLabel));
    }
}
