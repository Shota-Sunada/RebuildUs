namespace RebuildUs.Modules.GameMode;

public static partial class BattleRoyale
{
    // Battle Royale
    private static CustomButton soloPlayer01KillButton;
    private static CustomButton soloPlayer02KillButton;
    private static CustomButton soloPlayer03KillButton;
    private static CustomButton soloPlayer04KillButton;
    private static CustomButton soloPlayer05KillButton;
    private static CustomButton soloPlayer06KillButton;
    private static CustomButton soloPlayer07KillButton;
    private static CustomButton soloPlayer08KillButton;
    private static CustomButton soloPlayer09KillButton;
    private static CustomButton soloPlayer10KillButton;
    private static CustomButton soloPlayer11KillButton;
    private static CustomButton soloPlayer12KillButton;
    private static CustomButton soloPlayer13KillButton;
    private static CustomButton soloPlayer14KillButton;
    private static CustomButton soloPlayer15KillButton;
    private static CustomButton limePlayer01KillButton;
    private static CustomButton limePlayer02KillButton;
    private static CustomButton limePlayer03KillButton;
    private static CustomButton limePlayer04KillButton;
    private static CustomButton limePlayer05KillButton;
    private static CustomButton limePlayer06KillButton;
    private static CustomButton limePlayer07KillButton;
    private static CustomButton pinkPlayer01KillButton;
    private static CustomButton pinkPlayer02KillButton;
    private static CustomButton pinkPlayer03KillButton;
    private static CustomButton pinkPlayer04KillButton;
    private static CustomButton pinkPlayer05KillButton;
    private static CustomButton pinkPlayer06KillButton;
    private static CustomButton pinkPlayer07KillButton;
    private static CustomButton serialKillerKillButton;

    public static void SetButtonCooldowns()
    {
        // Battle Royale
        soloPlayer01KillButton.MaxTimer = BattleRoyale.killCooldown;
        soloPlayer02KillButton.MaxTimer = BattleRoyale.killCooldown;
        soloPlayer03KillButton.MaxTimer = BattleRoyale.killCooldown;
        soloPlayer04KillButton.MaxTimer = BattleRoyale.killCooldown;
        soloPlayer05KillButton.MaxTimer = BattleRoyale.killCooldown;
        soloPlayer06KillButton.MaxTimer = BattleRoyale.killCooldown;
        soloPlayer07KillButton.MaxTimer = BattleRoyale.killCooldown;
        soloPlayer08KillButton.MaxTimer = BattleRoyale.killCooldown;
        soloPlayer09KillButton.MaxTimer = BattleRoyale.killCooldown;
        soloPlayer10KillButton.MaxTimer = BattleRoyale.killCooldown;
        soloPlayer11KillButton.MaxTimer = BattleRoyale.killCooldown;
        soloPlayer12KillButton.MaxTimer = BattleRoyale.killCooldown;
        soloPlayer13KillButton.MaxTimer = BattleRoyale.killCooldown;
        soloPlayer14KillButton.MaxTimer = BattleRoyale.killCooldown;
        soloPlayer15KillButton.MaxTimer = BattleRoyale.killCooldown;
        limePlayer01KillButton.MaxTimer = BattleRoyale.killCooldown;
        limePlayer02KillButton.MaxTimer = BattleRoyale.killCooldown;
        limePlayer03KillButton.MaxTimer = BattleRoyale.killCooldown;
        limePlayer04KillButton.MaxTimer = BattleRoyale.killCooldown;
        limePlayer05KillButton.MaxTimer = BattleRoyale.killCooldown;
        limePlayer06KillButton.MaxTimer = BattleRoyale.killCooldown;
        limePlayer07KillButton.MaxTimer = BattleRoyale.killCooldown;
        pinkPlayer01KillButton.MaxTimer = BattleRoyale.killCooldown;
        pinkPlayer02KillButton.MaxTimer = BattleRoyale.killCooldown;
        pinkPlayer03KillButton.MaxTimer = BattleRoyale.killCooldown;
        pinkPlayer04KillButton.MaxTimer = BattleRoyale.killCooldown;
        pinkPlayer05KillButton.MaxTimer = BattleRoyale.killCooldown;
        pinkPlayer06KillButton.MaxTimer = BattleRoyale.killCooldown;
        pinkPlayer07KillButton.MaxTimer = BattleRoyale.killCooldown;
        serialKillerKillButton.MaxTimer = BattleRoyale.serialKillerCooldown;
    }

    public static void MakeButtons(HudManager __instance)
    {
        // Battle Royale
        // SoloPlayer01 Kill
        soloPlayer01KillButton = new CustomButton(
            () =>
            {
                PlayerControl target = BattleRoyale.GetShotPlayer(2 * 0.2f, 6, 1);

                MessageWriter writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, Hazel.SendOption.Reliable, -1);
                writerShot.Write(BattleRoyale.soloPlayer01.PlayerId);
                writerShot.Write(0);
                writerShot.Write(BattleRoyale.soloPlayer01mouseAngle);
                AmongUsClient.Instance.FinishRpcImmediately(writerShot);
                RPCProcedure.battleRoyaleShowShoots(BattleRoyale.soloPlayer01.PlayerId, 0, BattleRoyale.soloPlayer01mouseAngle);

                if (target == null)
                {
                    soloPlayer01KillButton.Timer = soloPlayer01KillButton.MaxTimer;
                    return;
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(BattleRoyale.soloPlayer01.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.gamemodeKills(target.PlayerId, BattleRoyale.soloPlayer01.PlayerId);

                soloPlayer01KillButton.Timer = soloPlayer01KillButton.MaxTimer;
                SoundManager.Instance.PlaySound(AssetLoader.royaleHitPlayer, false, 100f);

                target = null;
            },
            () => { return BattleRoyale.soloPlayer01 != null && BattleRoyale.soloPlayer01 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (BattleRoyale.soloPlayer01Wep == null)
                {
                    BattleRoyale.soloPlayer01Wep = new GameObject("Weapon");
                    var renderer = BattleRoyale.soloPlayer01Wep.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.Bow;
                    renderer.transform.parent = BattleRoyale.soloPlayer01.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {

                    if (PlayerControl.LocalPlayer.Data.IsDead)
                    {
                        BattleRoyale.soloPlayer01Wep.SetActive(false);
                    }

                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    BattleRoyale.soloPlayer01mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = BattleRoyale.soloPlayer01.transform.position + new Vector3(0.8f * (float)Math.Cos(BattleRoyale.soloPlayer01mouseAngle), 0.8f * (float)Math.Sin(BattleRoyale.soloPlayer01mouseAngle));
                    BattleRoyale.soloPlayer01Wep.transform.position += (targetPosition - BattleRoyale.soloPlayer01Wep.transform.position) * 0.4f;
                    BattleRoyale.soloPlayer01Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(BattleRoyale.soloPlayer01mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(BattleRoyale.soloPlayer01mouseAngle) < 0.0)
                    {
                        if (BattleRoyale.soloPlayer01Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            BattleRoyale.soloPlayer01Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (BattleRoyale.soloPlayer01Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            BattleRoyale.soloPlayer01Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                return PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { soloPlayer01KillButton.Timer = soloPlayer01KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            KeyCode.Mouse1,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // SoloPlayer02 Kill
        soloPlayer02KillButton = new CustomButton(
            () =>
            {
                PlayerControl target = BattleRoyale.GetShotPlayer(2 * 0.2f, 6, 2);

                MessageWriter writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, Hazel.SendOption.Reliable, -1);
                writerShot.Write(BattleRoyale.soloPlayer02.PlayerId);
                writerShot.Write(0);
                writerShot.Write(BattleRoyale.soloPlayer02mouseAngle);
                AmongUsClient.Instance.FinishRpcImmediately(writerShot);
                RPCProcedure.battleRoyaleShowShoots(BattleRoyale.soloPlayer02.PlayerId, 0, BattleRoyale.soloPlayer02mouseAngle);

                if (target == null)
                {
                    soloPlayer02KillButton.Timer = soloPlayer02KillButton.MaxTimer;
                    return;
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(BattleRoyale.soloPlayer02.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.gamemodeKills(target.PlayerId, BattleRoyale.soloPlayer02.PlayerId);

                soloPlayer02KillButton.Timer = soloPlayer02KillButton.MaxTimer;
                SoundManager.Instance.PlaySound(AssetLoader.royaleHitPlayer, false, 100f);

                target = null;
            },
            () => { return BattleRoyale.soloPlayer02 != null && BattleRoyale.soloPlayer02 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (BattleRoyale.soloPlayer02Wep == null)
                {
                    BattleRoyale.soloPlayer02Wep = new GameObject("Weapon");
                    var renderer = BattleRoyale.soloPlayer02Wep.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.Bow;
                    renderer.transform.parent = BattleRoyale.soloPlayer02.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {

                    if (PlayerControl.LocalPlayer.Data.IsDead)
                    {
                        BattleRoyale.soloPlayer02Wep.SetActive(false);
                    }

                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    BattleRoyale.soloPlayer02mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = BattleRoyale.soloPlayer02.transform.position + new Vector3(0.8f * (float)Math.Cos(BattleRoyale.soloPlayer02mouseAngle), 0.8f * (float)Math.Sin(BattleRoyale.soloPlayer02mouseAngle));
                    BattleRoyale.soloPlayer02Wep.transform.position += (targetPosition - BattleRoyale.soloPlayer02Wep.transform.position) * 0.4f;
                    BattleRoyale.soloPlayer02Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(BattleRoyale.soloPlayer02mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(BattleRoyale.soloPlayer02mouseAngle) < 0.0)
                    {
                        if (BattleRoyale.soloPlayer02Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            BattleRoyale.soloPlayer02Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (BattleRoyale.soloPlayer02Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            BattleRoyale.soloPlayer02Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                return PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { soloPlayer02KillButton.Timer = soloPlayer02KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            KeyCode.Mouse1,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // SoloPlayer03 Kill
        soloPlayer03KillButton = new CustomButton(
            () =>
            {
                PlayerControl target = BattleRoyale.GetShotPlayer(2 * 0.2f, 6, 3);

                MessageWriter writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, Hazel.SendOption.Reliable, -1);
                writerShot.Write(BattleRoyale.soloPlayer03.PlayerId);
                writerShot.Write(0);
                writerShot.Write(BattleRoyale.soloPlayer03mouseAngle);
                AmongUsClient.Instance.FinishRpcImmediately(writerShot);
                RPCProcedure.battleRoyaleShowShoots(BattleRoyale.soloPlayer03.PlayerId, 0, BattleRoyale.soloPlayer03mouseAngle);

                if (target == null)
                {
                    soloPlayer03KillButton.Timer = soloPlayer03KillButton.MaxTimer;
                    return;
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(BattleRoyale.soloPlayer03.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.gamemodeKills(target.PlayerId, BattleRoyale.soloPlayer03.PlayerId);

                soloPlayer03KillButton.Timer = soloPlayer03KillButton.MaxTimer;
                SoundManager.Instance.PlaySound(AssetLoader.royaleHitPlayer, false, 100f);

                target = null;
            },
            () => { return BattleRoyale.soloPlayer03 != null && BattleRoyale.soloPlayer03 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (BattleRoyale.soloPlayer03Wep == null)
                {
                    BattleRoyale.soloPlayer03Wep = new GameObject("Weapon");
                    var renderer = BattleRoyale.soloPlayer03Wep.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.Bow;
                    renderer.transform.parent = BattleRoyale.soloPlayer03.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {

                    if (PlayerControl.LocalPlayer.Data.IsDead)
                    {
                        BattleRoyale.soloPlayer03Wep.SetActive(false);
                    }

                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    BattleRoyale.soloPlayer03mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = BattleRoyale.soloPlayer03.transform.position + new Vector3(0.8f * (float)Math.Cos(BattleRoyale.soloPlayer03mouseAngle), 0.8f * (float)Math.Sin(BattleRoyale.soloPlayer03mouseAngle));
                    BattleRoyale.soloPlayer03Wep.transform.position += (targetPosition - BattleRoyale.soloPlayer03Wep.transform.position) * 0.4f;
                    BattleRoyale.soloPlayer03Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(BattleRoyale.soloPlayer03mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(BattleRoyale.soloPlayer03mouseAngle) < 0.0)
                    {
                        if (BattleRoyale.soloPlayer03Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            BattleRoyale.soloPlayer03Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (BattleRoyale.soloPlayer03Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            BattleRoyale.soloPlayer03Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                return PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { soloPlayer03KillButton.Timer = soloPlayer03KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            KeyCode.Mouse1,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // SoloPlayer04 Kill
        soloPlayer04KillButton = new CustomButton(
            () =>
            {
                PlayerControl target = BattleRoyale.GetShotPlayer(2 * 0.2f, 6, 4);

                MessageWriter writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, Hazel.SendOption.Reliable, -1);
                writerShot.Write(BattleRoyale.soloPlayer04.PlayerId);
                writerShot.Write(0);
                writerShot.Write(BattleRoyale.soloPlayer04mouseAngle);
                AmongUsClient.Instance.FinishRpcImmediately(writerShot);
                RPCProcedure.battleRoyaleShowShoots(BattleRoyale.soloPlayer04.PlayerId, 0, BattleRoyale.soloPlayer04mouseAngle);

                if (target == null)
                {
                    soloPlayer04KillButton.Timer = soloPlayer04KillButton.MaxTimer;
                    return;
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(BattleRoyale.soloPlayer04.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.gamemodeKills(target.PlayerId, BattleRoyale.soloPlayer04.PlayerId);

                soloPlayer04KillButton.Timer = soloPlayer04KillButton.MaxTimer;
                SoundManager.Instance.PlaySound(AssetLoader.royaleHitPlayer, false, 100f);

                target = null;
            },
            () => { return BattleRoyale.soloPlayer04 != null && BattleRoyale.soloPlayer04 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (BattleRoyale.soloPlayer04Wep == null)
                {
                    BattleRoyale.soloPlayer04Wep = new GameObject("Weapon");
                    var renderer = BattleRoyale.soloPlayer04Wep.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.Bow;
                    renderer.transform.parent = BattleRoyale.soloPlayer04.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {

                    if (PlayerControl.LocalPlayer.Data.IsDead)
                    {
                        BattleRoyale.soloPlayer04Wep.SetActive(false);
                    }

                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    BattleRoyale.soloPlayer04mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = BattleRoyale.soloPlayer04.transform.position + new Vector3(0.8f * (float)Math.Cos(BattleRoyale.soloPlayer04mouseAngle), 0.8f * (float)Math.Sin(BattleRoyale.soloPlayer04mouseAngle));
                    BattleRoyale.soloPlayer04Wep.transform.position += (targetPosition - BattleRoyale.soloPlayer04Wep.transform.position) * 0.4f;
                    BattleRoyale.soloPlayer04Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(BattleRoyale.soloPlayer04mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(BattleRoyale.soloPlayer04mouseAngle) < 0.0)
                    {
                        if (BattleRoyale.soloPlayer04Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            BattleRoyale.soloPlayer04Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (BattleRoyale.soloPlayer04Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            BattleRoyale.soloPlayer04Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                return PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { soloPlayer04KillButton.Timer = soloPlayer04KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            KeyCode.Mouse1,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // SoloPlayer05 Kill
        soloPlayer05KillButton = new CustomButton(
            () =>
            {
                PlayerControl target = BattleRoyale.GetShotPlayer(2 * 0.2f, 6, 5);

                MessageWriter writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, Hazel.SendOption.Reliable, -1);
                writerShot.Write(BattleRoyale.soloPlayer05.PlayerId);
                writerShot.Write(0);
                writerShot.Write(BattleRoyale.soloPlayer05mouseAngle);
                AmongUsClient.Instance.FinishRpcImmediately(writerShot);
                RPCProcedure.battleRoyaleShowShoots(BattleRoyale.soloPlayer05.PlayerId, 0, BattleRoyale.soloPlayer05mouseAngle);

                if (target == null)
                {
                    soloPlayer05KillButton.Timer = soloPlayer05KillButton.MaxTimer;
                    return;
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(BattleRoyale.soloPlayer05.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.gamemodeKills(target.PlayerId, BattleRoyale.soloPlayer05.PlayerId);

                soloPlayer05KillButton.Timer = soloPlayer05KillButton.MaxTimer;
                SoundManager.Instance.PlaySound(AssetLoader.royaleHitPlayer, false, 100f);

                target = null;
            },
            () => { return BattleRoyale.soloPlayer05 != null && BattleRoyale.soloPlayer05 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (BattleRoyale.soloPlayer05Wep == null)
                {
                    BattleRoyale.soloPlayer05Wep = new GameObject("Weapon");
                    var renderer = BattleRoyale.soloPlayer05Wep.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.Bow;
                    renderer.transform.parent = BattleRoyale.soloPlayer05.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {

                    if (PlayerControl.LocalPlayer.Data.IsDead)
                    {
                        BattleRoyale.soloPlayer05Wep.SetActive(false);
                    }

                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    BattleRoyale.soloPlayer05mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = BattleRoyale.soloPlayer05.transform.position + new Vector3(0.8f * (float)Math.Cos(BattleRoyale.soloPlayer05mouseAngle), 0.8f * (float)Math.Sin(BattleRoyale.soloPlayer05mouseAngle));
                    BattleRoyale.soloPlayer05Wep.transform.position += (targetPosition - BattleRoyale.soloPlayer05Wep.transform.position) * 0.4f;
                    BattleRoyale.soloPlayer05Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(BattleRoyale.soloPlayer05mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(BattleRoyale.soloPlayer05mouseAngle) < 0.0)
                    {
                        if (BattleRoyale.soloPlayer05Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            BattleRoyale.soloPlayer05Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (BattleRoyale.soloPlayer05Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            BattleRoyale.soloPlayer05Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                return PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { soloPlayer05KillButton.Timer = soloPlayer05KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            KeyCode.Mouse1,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // SoloPlayer06 Kill
        soloPlayer06KillButton = new CustomButton(
            () =>
            {
                PlayerControl target = BattleRoyale.GetShotPlayer(2 * 0.2f, 6, 6);

                MessageWriter writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, Hazel.SendOption.Reliable, -1);
                writerShot.Write(BattleRoyale.soloPlayer06.PlayerId);
                writerShot.Write(0);
                writerShot.Write(BattleRoyale.soloPlayer06mouseAngle);
                AmongUsClient.Instance.FinishRpcImmediately(writerShot);
                RPCProcedure.battleRoyaleShowShoots(BattleRoyale.soloPlayer06.PlayerId, 0, BattleRoyale.soloPlayer06mouseAngle);

                if (target == null)
                {
                    soloPlayer06KillButton.Timer = soloPlayer06KillButton.MaxTimer;
                    return;
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(BattleRoyale.soloPlayer06.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.gamemodeKills(target.PlayerId, BattleRoyale.soloPlayer06.PlayerId);

                soloPlayer06KillButton.Timer = soloPlayer06KillButton.MaxTimer;
                SoundManager.Instance.PlaySound(AssetLoader.royaleHitPlayer, false, 100f);

                target = null;
            },
            () => { return BattleRoyale.soloPlayer06 != null && BattleRoyale.soloPlayer06 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (BattleRoyale.soloPlayer06Wep == null)
                {
                    BattleRoyale.soloPlayer06Wep = new GameObject("Weapon");
                    var renderer = BattleRoyale.soloPlayer06Wep.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.Bow;
                    renderer.transform.parent = BattleRoyale.soloPlayer06.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {

                    if (PlayerControl.LocalPlayer.Data.IsDead)
                    {
                        BattleRoyale.soloPlayer06Wep.SetActive(false);
                    }

                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    BattleRoyale.soloPlayer06mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = BattleRoyale.soloPlayer06.transform.position + new Vector3(0.8f * (float)Math.Cos(BattleRoyale.soloPlayer06mouseAngle), 0.8f * (float)Math.Sin(BattleRoyale.soloPlayer06mouseAngle));
                    BattleRoyale.soloPlayer06Wep.transform.position += (targetPosition - BattleRoyale.soloPlayer06Wep.transform.position) * 0.4f;
                    BattleRoyale.soloPlayer06Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(BattleRoyale.soloPlayer06mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(BattleRoyale.soloPlayer06mouseAngle) < 0.0)
                    {
                        if (BattleRoyale.soloPlayer06Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            BattleRoyale.soloPlayer06Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (BattleRoyale.soloPlayer06Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            BattleRoyale.soloPlayer06Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                return PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { soloPlayer06KillButton.Timer = soloPlayer06KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            KeyCode.Mouse1,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // SoloPlayer07 Kill
        soloPlayer07KillButton = new CustomButton(
            () =>
            {
                PlayerControl target = BattleRoyale.GetShotPlayer(2 * 0.2f, 6, 7);

                MessageWriter writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, Hazel.SendOption.Reliable, -1);
                writerShot.Write(BattleRoyale.soloPlayer07.PlayerId);
                writerShot.Write(0);
                writerShot.Write(BattleRoyale.soloPlayer07mouseAngle);
                AmongUsClient.Instance.FinishRpcImmediately(writerShot);
                RPCProcedure.battleRoyaleShowShoots(BattleRoyale.soloPlayer07.PlayerId, 0, BattleRoyale.soloPlayer07mouseAngle);

                if (target == null)
                {
                    soloPlayer07KillButton.Timer = soloPlayer07KillButton.MaxTimer;
                    return;
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(BattleRoyale.soloPlayer07.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.gamemodeKills(target.PlayerId, BattleRoyale.soloPlayer07.PlayerId);

                soloPlayer07KillButton.Timer = soloPlayer07KillButton.MaxTimer;
                SoundManager.Instance.PlaySound(AssetLoader.royaleHitPlayer, false, 100f);

                target = null;
            },
            () => { return BattleRoyale.soloPlayer07 != null && BattleRoyale.soloPlayer07 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (BattleRoyale.soloPlayer07Wep == null)
                {
                    BattleRoyale.soloPlayer07Wep = new GameObject("Weapon");
                    var renderer = BattleRoyale.soloPlayer07Wep.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.Bow;
                    renderer.transform.parent = BattleRoyale.soloPlayer07.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {

                    if (PlayerControl.LocalPlayer.Data.IsDead)
                    {
                        BattleRoyale.soloPlayer07Wep.SetActive(false);
                    }

                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    BattleRoyale.soloPlayer07mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = BattleRoyale.soloPlayer07.transform.position + new Vector3(0.8f * (float)Math.Cos(BattleRoyale.soloPlayer07mouseAngle), 0.8f * (float)Math.Sin(BattleRoyale.soloPlayer07mouseAngle));
                    BattleRoyale.soloPlayer07Wep.transform.position += (targetPosition - BattleRoyale.soloPlayer07Wep.transform.position) * 0.4f;
                    BattleRoyale.soloPlayer07Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(BattleRoyale.soloPlayer07mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(BattleRoyale.soloPlayer07mouseAngle) < 0.0)
                    {
                        if (BattleRoyale.soloPlayer07Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            BattleRoyale.soloPlayer07Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (BattleRoyale.soloPlayer07Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            BattleRoyale.soloPlayer07Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                return PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { soloPlayer07KillButton.Timer = soloPlayer07KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            KeyCode.Mouse1,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // SoloPlayer08 Kill
        soloPlayer08KillButton = new CustomButton(
            () =>
            {
                PlayerControl target = BattleRoyale.GetShotPlayer(2 * 0.2f, 6, 8);

                MessageWriter writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, Hazel.SendOption.Reliable, -1);
                writerShot.Write(BattleRoyale.soloPlayer08.PlayerId);
                writerShot.Write(0);
                writerShot.Write(BattleRoyale.soloPlayer08mouseAngle);
                AmongUsClient.Instance.FinishRpcImmediately(writerShot);
                RPCProcedure.battleRoyaleShowShoots(BattleRoyale.soloPlayer08.PlayerId, 0, BattleRoyale.soloPlayer08mouseAngle);

                if (target == null)
                {
                    soloPlayer08KillButton.Timer = soloPlayer08KillButton.MaxTimer;
                    return;
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(BattleRoyale.soloPlayer08.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.gamemodeKills(target.PlayerId, BattleRoyale.soloPlayer08.PlayerId);

                soloPlayer08KillButton.Timer = soloPlayer08KillButton.MaxTimer;
                SoundManager.Instance.PlaySound(AssetLoader.royaleHitPlayer, false, 100f);

                target = null;
            },
            () => { return BattleRoyale.soloPlayer08 != null && BattleRoyale.soloPlayer08 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (BattleRoyale.soloPlayer08Wep == null)
                {
                    BattleRoyale.soloPlayer08Wep = new GameObject("Weapon");
                    var renderer = BattleRoyale.soloPlayer08Wep.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.Bow;
                    renderer.transform.parent = BattleRoyale.soloPlayer08.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {

                    if (PlayerControl.LocalPlayer.Data.IsDead)
                    {
                        BattleRoyale.soloPlayer08Wep.SetActive(false);
                    }

                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    BattleRoyale.soloPlayer08mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = BattleRoyale.soloPlayer08.transform.position + new Vector3(0.8f * (float)Math.Cos(BattleRoyale.soloPlayer08mouseAngle), 0.8f * (float)Math.Sin(BattleRoyale.soloPlayer08mouseAngle));
                    BattleRoyale.soloPlayer08Wep.transform.position += (targetPosition - BattleRoyale.soloPlayer08Wep.transform.position) * 0.4f;
                    BattleRoyale.soloPlayer08Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(BattleRoyale.soloPlayer08mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(BattleRoyale.soloPlayer08mouseAngle) < 0.0)
                    {
                        if (BattleRoyale.soloPlayer08Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            BattleRoyale.soloPlayer08Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (BattleRoyale.soloPlayer08Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            BattleRoyale.soloPlayer08Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                return PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { soloPlayer08KillButton.Timer = soloPlayer08KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            KeyCode.Mouse1,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // SoloPlayer09 Kill
        soloPlayer09KillButton = new CustomButton(
            () =>
            {
                PlayerControl target = BattleRoyale.GetShotPlayer(2 * 0.2f, 6, 9);

                MessageWriter writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, Hazel.SendOption.Reliable, -1);
                writerShot.Write(BattleRoyale.soloPlayer09.PlayerId);
                writerShot.Write(0);
                writerShot.Write(BattleRoyale.soloPlayer09mouseAngle);
                AmongUsClient.Instance.FinishRpcImmediately(writerShot);
                RPCProcedure.battleRoyaleShowShoots(BattleRoyale.soloPlayer09.PlayerId, 0, BattleRoyale.soloPlayer09mouseAngle);

                if (target == null)
                {
                    soloPlayer09KillButton.Timer = soloPlayer09KillButton.MaxTimer;
                    return;
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(BattleRoyale.soloPlayer09.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.gamemodeKills(target.PlayerId, BattleRoyale.soloPlayer09.PlayerId);

                soloPlayer09KillButton.Timer = soloPlayer09KillButton.MaxTimer;
                SoundManager.Instance.PlaySound(AssetLoader.royaleHitPlayer, false, 100f);

                target = null;
            },
            () => { return BattleRoyale.soloPlayer09 != null && BattleRoyale.soloPlayer09 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (BattleRoyale.soloPlayer09Wep == null)
                {
                    BattleRoyale.soloPlayer09Wep = new GameObject("Weapon");
                    var renderer = BattleRoyale.soloPlayer09Wep.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.Bow;
                    renderer.transform.parent = BattleRoyale.soloPlayer09.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {

                    if (PlayerControl.LocalPlayer.Data.IsDead)
                    {
                        BattleRoyale.soloPlayer09Wep.SetActive(false);
                    }

                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    BattleRoyale.soloPlayer09mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = BattleRoyale.soloPlayer09.transform.position + new Vector3(0.8f * (float)Math.Cos(BattleRoyale.soloPlayer09mouseAngle), 0.8f * (float)Math.Sin(BattleRoyale.soloPlayer09mouseAngle));
                    BattleRoyale.soloPlayer09Wep.transform.position += (targetPosition - BattleRoyale.soloPlayer09Wep.transform.position) * 0.4f;
                    BattleRoyale.soloPlayer09Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(BattleRoyale.soloPlayer09mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(BattleRoyale.soloPlayer09mouseAngle) < 0.0)
                    {
                        if (BattleRoyale.soloPlayer09Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            BattleRoyale.soloPlayer09Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (BattleRoyale.soloPlayer09Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            BattleRoyale.soloPlayer09Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                return PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { soloPlayer09KillButton.Timer = soloPlayer09KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            KeyCode.Mouse1,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // SoloPlayer10 Kill
        soloPlayer10KillButton = new CustomButton(
            () =>
            {
                PlayerControl target = BattleRoyale.GetShotPlayer(2 * 0.2f, 6, 10);

                MessageWriter writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, Hazel.SendOption.Reliable, -1);
                writerShot.Write(BattleRoyale.soloPlayer10.PlayerId);
                writerShot.Write(0);
                writerShot.Write(BattleRoyale.soloPlayer10mouseAngle);
                AmongUsClient.Instance.FinishRpcImmediately(writerShot);
                RPCProcedure.battleRoyaleShowShoots(BattleRoyale.soloPlayer10.PlayerId, 0, BattleRoyale.soloPlayer10mouseAngle);

                if (target == null)
                {
                    soloPlayer10KillButton.Timer = soloPlayer10KillButton.MaxTimer;
                    return;
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(BattleRoyale.soloPlayer10.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.gamemodeKills(target.PlayerId, BattleRoyale.soloPlayer10.PlayerId);

                soloPlayer10KillButton.Timer = soloPlayer10KillButton.MaxTimer;
                SoundManager.Instance.PlaySound(AssetLoader.royaleHitPlayer, false, 100f);

                target = null;
            },
            () => { return BattleRoyale.soloPlayer10 != null && BattleRoyale.soloPlayer10 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (BattleRoyale.soloPlayer10Wep == null)
                {
                    BattleRoyale.soloPlayer10Wep = new GameObject("Weapon");
                    var renderer = BattleRoyale.soloPlayer10Wep.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.Bow;
                    renderer.transform.parent = BattleRoyale.soloPlayer10.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {

                    if (PlayerControl.LocalPlayer.Data.IsDead)
                    {
                        BattleRoyale.soloPlayer10Wep.SetActive(false);
                    }

                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    BattleRoyale.soloPlayer10mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = BattleRoyale.soloPlayer10.transform.position + new Vector3(0.8f * (float)Math.Cos(BattleRoyale.soloPlayer10mouseAngle), 0.8f * (float)Math.Sin(BattleRoyale.soloPlayer10mouseAngle));
                    BattleRoyale.soloPlayer10Wep.transform.position += (targetPosition - BattleRoyale.soloPlayer10Wep.transform.position) * 0.4f;
                    BattleRoyale.soloPlayer10Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(BattleRoyale.soloPlayer10mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(BattleRoyale.soloPlayer10mouseAngle) < 0.0)
                    {
                        if (BattleRoyale.soloPlayer10Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            BattleRoyale.soloPlayer10Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (BattleRoyale.soloPlayer10Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            BattleRoyale.soloPlayer10Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                return PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { soloPlayer10KillButton.Timer = soloPlayer10KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            KeyCode.Mouse1,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // SoloPlayer11 Kill
        soloPlayer11KillButton = new CustomButton(
            () =>
            {
                PlayerControl target = BattleRoyale.GetShotPlayer(2 * 0.2f, 6, 11);

                MessageWriter writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, Hazel.SendOption.Reliable, -1);
                writerShot.Write(BattleRoyale.soloPlayer11.PlayerId);
                writerShot.Write(0);
                writerShot.Write(BattleRoyale.soloPlayer11mouseAngle);
                AmongUsClient.Instance.FinishRpcImmediately(writerShot);
                RPCProcedure.battleRoyaleShowShoots(BattleRoyale.soloPlayer11.PlayerId, 0, BattleRoyale.soloPlayer11mouseAngle);

                if (target == null)
                {
                    soloPlayer11KillButton.Timer = soloPlayer11KillButton.MaxTimer;
                    return;
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(BattleRoyale.soloPlayer11.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.gamemodeKills(target.PlayerId, BattleRoyale.soloPlayer11.PlayerId);

                soloPlayer11KillButton.Timer = soloPlayer11KillButton.MaxTimer;
                SoundManager.Instance.PlaySound(AssetLoader.royaleHitPlayer, false, 100f);

                target = null;
            },
            () => { return BattleRoyale.soloPlayer11 != null && BattleRoyale.soloPlayer11 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (BattleRoyale.soloPlayer11Wep == null)
                {
                    BattleRoyale.soloPlayer11Wep = new GameObject("Weapon");
                    var renderer = BattleRoyale.soloPlayer11Wep.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.Bow;
                    renderer.transform.parent = BattleRoyale.soloPlayer11.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {

                    if (PlayerControl.LocalPlayer.Data.IsDead)
                    {
                        BattleRoyale.soloPlayer11Wep.SetActive(false);
                    }

                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    BattleRoyale.soloPlayer11mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = BattleRoyale.soloPlayer11.transform.position + new Vector3(0.8f * (float)Math.Cos(BattleRoyale.soloPlayer11mouseAngle), 0.8f * (float)Math.Sin(BattleRoyale.soloPlayer11mouseAngle));
                    BattleRoyale.soloPlayer11Wep.transform.position += (targetPosition - BattleRoyale.soloPlayer11Wep.transform.position) * 0.4f;
                    BattleRoyale.soloPlayer11Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(BattleRoyale.soloPlayer11mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(BattleRoyale.soloPlayer11mouseAngle) < 0.0)
                    {
                        if (BattleRoyale.soloPlayer11Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            BattleRoyale.soloPlayer11Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (BattleRoyale.soloPlayer11Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            BattleRoyale.soloPlayer11Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                return PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { soloPlayer11KillButton.Timer = soloPlayer11KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            KeyCode.Mouse1,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // SoloPlayer12 Kill
        soloPlayer12KillButton = new CustomButton(
            () =>
            {
                PlayerControl target = BattleRoyale.GetShotPlayer(2 * 0.2f, 6, 12);

                MessageWriter writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, Hazel.SendOption.Reliable, -1);
                writerShot.Write(BattleRoyale.soloPlayer12.PlayerId);
                writerShot.Write(0);
                writerShot.Write(BattleRoyale.soloPlayer12mouseAngle);
                AmongUsClient.Instance.FinishRpcImmediately(writerShot);
                RPCProcedure.battleRoyaleShowShoots(BattleRoyale.soloPlayer12.PlayerId, 0, BattleRoyale.soloPlayer12mouseAngle);

                if (target == null)
                {
                    soloPlayer12KillButton.Timer = soloPlayer12KillButton.MaxTimer;
                    return;
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(BattleRoyale.soloPlayer12.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.gamemodeKills(target.PlayerId, BattleRoyale.soloPlayer12.PlayerId);

                soloPlayer12KillButton.Timer = soloPlayer12KillButton.MaxTimer;
                SoundManager.Instance.PlaySound(AssetLoader.royaleHitPlayer, false, 100f);

                target = null;
            },
            () => { return BattleRoyale.soloPlayer12 != null && BattleRoyale.soloPlayer12 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (BattleRoyale.soloPlayer12Wep == null)
                {
                    BattleRoyale.soloPlayer12Wep = new GameObject("Weapon");
                    var renderer = BattleRoyale.soloPlayer12Wep.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.Bow;
                    renderer.transform.parent = BattleRoyale.soloPlayer12.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {

                    if (PlayerControl.LocalPlayer.Data.IsDead)
                    {
                        BattleRoyale.soloPlayer12Wep.SetActive(false);
                    }

                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    BattleRoyale.soloPlayer12mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = BattleRoyale.soloPlayer12.transform.position + new Vector3(0.8f * (float)Math.Cos(BattleRoyale.soloPlayer12mouseAngle), 0.8f * (float)Math.Sin(BattleRoyale.soloPlayer12mouseAngle));
                    BattleRoyale.soloPlayer12Wep.transform.position += (targetPosition - BattleRoyale.soloPlayer12Wep.transform.position) * 0.4f;
                    BattleRoyale.soloPlayer12Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(BattleRoyale.soloPlayer12mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(BattleRoyale.soloPlayer12mouseAngle) < 0.0)
                    {
                        if (BattleRoyale.soloPlayer12Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            BattleRoyale.soloPlayer12Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (BattleRoyale.soloPlayer12Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            BattleRoyale.soloPlayer12Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                return PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { soloPlayer12KillButton.Timer = soloPlayer12KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            KeyCode.Mouse1,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // SoloPlayer13 Kill
        soloPlayer13KillButton = new CustomButton(
            () =>
            {
                PlayerControl target = BattleRoyale.GetShotPlayer(2 * 0.2f, 6, 13);

                MessageWriter writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, Hazel.SendOption.Reliable, -1);
                writerShot.Write(BattleRoyale.soloPlayer13.PlayerId);
                writerShot.Write(0);
                writerShot.Write(BattleRoyale.soloPlayer13mouseAngle);
                AmongUsClient.Instance.FinishRpcImmediately(writerShot);
                RPCProcedure.battleRoyaleShowShoots(BattleRoyale.soloPlayer13.PlayerId, 0, BattleRoyale.soloPlayer13mouseAngle);

                if (target == null)
                {
                    soloPlayer13KillButton.Timer = soloPlayer13KillButton.MaxTimer;
                    return;
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(BattleRoyale.soloPlayer13.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.gamemodeKills(target.PlayerId, BattleRoyale.soloPlayer13.PlayerId);

                soloPlayer13KillButton.Timer = soloPlayer13KillButton.MaxTimer;
                SoundManager.Instance.PlaySound(AssetLoader.royaleHitPlayer, false, 100f);

                target = null;
            },
            () => { return BattleRoyale.soloPlayer13 != null && BattleRoyale.soloPlayer13 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (BattleRoyale.soloPlayer13Wep == null)
                {
                    BattleRoyale.soloPlayer13Wep = new GameObject("Weapon");
                    var renderer = BattleRoyale.soloPlayer13Wep.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.Bow;
                    renderer.transform.parent = BattleRoyale.soloPlayer13.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {

                    if (PlayerControl.LocalPlayer.Data.IsDead)
                    {
                        BattleRoyale.soloPlayer13Wep.SetActive(false);
                    }

                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    BattleRoyale.soloPlayer13mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = BattleRoyale.soloPlayer13.transform.position + new Vector3(0.8f * (float)Math.Cos(BattleRoyale.soloPlayer13mouseAngle), 0.8f * (float)Math.Sin(BattleRoyale.soloPlayer13mouseAngle));
                    BattleRoyale.soloPlayer13Wep.transform.position += (targetPosition - BattleRoyale.soloPlayer13Wep.transform.position) * 0.4f;
                    BattleRoyale.soloPlayer13Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(BattleRoyale.soloPlayer13mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(BattleRoyale.soloPlayer13mouseAngle) < 0.0)
                    {
                        if (BattleRoyale.soloPlayer13Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            BattleRoyale.soloPlayer13Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (BattleRoyale.soloPlayer13Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            BattleRoyale.soloPlayer13Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                return PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { soloPlayer13KillButton.Timer = soloPlayer13KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            KeyCode.Mouse1,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // SoloPlayer14 Kill
        soloPlayer14KillButton = new CustomButton(
            () =>
            {
                PlayerControl target = BattleRoyale.GetShotPlayer(2 * 0.2f, 6, 14);

                MessageWriter writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, Hazel.SendOption.Reliable, -1);
                writerShot.Write(BattleRoyale.soloPlayer14.PlayerId);
                writerShot.Write(0);
                writerShot.Write(BattleRoyale.soloPlayer14mouseAngle);
                AmongUsClient.Instance.FinishRpcImmediately(writerShot);
                RPCProcedure.battleRoyaleShowShoots(BattleRoyale.soloPlayer14.PlayerId, 0, BattleRoyale.soloPlayer14mouseAngle);

                if (target == null)
                {
                    soloPlayer14KillButton.Timer = soloPlayer14KillButton.MaxTimer;
                    return;
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(BattleRoyale.soloPlayer14.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.gamemodeKills(target.PlayerId, BattleRoyale.soloPlayer14.PlayerId);

                soloPlayer14KillButton.Timer = soloPlayer14KillButton.MaxTimer;
                SoundManager.Instance.PlaySound(AssetLoader.royaleHitPlayer, false, 100f);

                target = null;
            },
            () => { return BattleRoyale.soloPlayer14 != null && BattleRoyale.soloPlayer14 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (BattleRoyale.soloPlayer14Wep == null)
                {
                    BattleRoyale.soloPlayer14Wep = new GameObject("Weapon");
                    var renderer = BattleRoyale.soloPlayer14Wep.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.Bow;
                    renderer.transform.parent = BattleRoyale.soloPlayer14.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {

                    if (PlayerControl.LocalPlayer.Data.IsDead)
                    {
                        BattleRoyale.soloPlayer14Wep.SetActive(false);
                    }

                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    BattleRoyale.soloPlayer14mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = BattleRoyale.soloPlayer14.transform.position + new Vector3(0.8f * (float)Math.Cos(BattleRoyale.soloPlayer14mouseAngle), 0.8f * (float)Math.Sin(BattleRoyale.soloPlayer14mouseAngle));
                    BattleRoyale.soloPlayer14Wep.transform.position += (targetPosition - BattleRoyale.soloPlayer14Wep.transform.position) * 0.4f;
                    BattleRoyale.soloPlayer14Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(BattleRoyale.soloPlayer14mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(BattleRoyale.soloPlayer14mouseAngle) < 0.0)
                    {
                        if (BattleRoyale.soloPlayer14Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            BattleRoyale.soloPlayer14Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (BattleRoyale.soloPlayer14Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            BattleRoyale.soloPlayer14Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                return PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { soloPlayer14KillButton.Timer = soloPlayer14KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            KeyCode.Mouse1,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // SoloPlayer15 Kill
        soloPlayer15KillButton = new CustomButton(
            () =>
            {
                PlayerControl target = BattleRoyale.GetShotPlayer(2 * 0.2f, 6, 15);

                MessageWriter writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, Hazel.SendOption.Reliable, -1);
                writerShot.Write(BattleRoyale.soloPlayer15.PlayerId);
                writerShot.Write(0);
                writerShot.Write(BattleRoyale.soloPlayer15mouseAngle);
                AmongUsClient.Instance.FinishRpcImmediately(writerShot);
                RPCProcedure.battleRoyaleShowShoots(BattleRoyale.soloPlayer15.PlayerId, 0, BattleRoyale.soloPlayer15mouseAngle);

                if (target == null)
                {
                    soloPlayer15KillButton.Timer = soloPlayer15KillButton.MaxTimer;
                    return;
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(BattleRoyale.soloPlayer15.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.gamemodeKills(target.PlayerId, BattleRoyale.soloPlayer15.PlayerId);

                soloPlayer15KillButton.Timer = soloPlayer15KillButton.MaxTimer;
                SoundManager.Instance.PlaySound(AssetLoader.royaleHitPlayer, false, 100f);

                target = null;
            },
            () => { return BattleRoyale.soloPlayer15 != null && BattleRoyale.soloPlayer15 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (BattleRoyale.soloPlayer15Wep == null)
                {
                    BattleRoyale.soloPlayer15Wep = new GameObject("Weapon");
                    var renderer = BattleRoyale.soloPlayer15Wep.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.Bow;
                    renderer.transform.parent = BattleRoyale.soloPlayer15.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {

                    if (PlayerControl.LocalPlayer.Data.IsDead)
                    {
                        BattleRoyale.soloPlayer15Wep.SetActive(false);
                    }

                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    BattleRoyale.soloPlayer15mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = BattleRoyale.soloPlayer15.transform.position + new Vector3(0.8f * (float)Math.Cos(BattleRoyale.soloPlayer15mouseAngle), 0.8f * (float)Math.Sin(BattleRoyale.soloPlayer15mouseAngle));
                    BattleRoyale.soloPlayer15Wep.transform.position += (targetPosition - BattleRoyale.soloPlayer15Wep.transform.position) * 0.4f;
                    BattleRoyale.soloPlayer15Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(BattleRoyale.soloPlayer15mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(BattleRoyale.soloPlayer15mouseAngle) < 0.0)
                    {
                        if (BattleRoyale.soloPlayer15Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            BattleRoyale.soloPlayer15Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (BattleRoyale.soloPlayer15Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            BattleRoyale.soloPlayer15Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                return PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { soloPlayer15KillButton.Timer = soloPlayer15KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            KeyCode.Mouse1,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // LimePlayer01 Kill
        limePlayer01KillButton = new CustomButton(
            () =>
            {
                PlayerControl target = BattleRoyale.GetLimeShotPlayer(2 * 0.2f, 6, 1);

                MessageWriter writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, Hazel.SendOption.Reliable, -1);
                writerShot.Write(BattleRoyale.limePlayer01.PlayerId);
                writerShot.Write(1);
                writerShot.Write(BattleRoyale.limePlayer01mouseAngle);
                AmongUsClient.Instance.FinishRpcImmediately(writerShot);
                RPCProcedure.battleRoyaleShowShoots(BattleRoyale.limePlayer01.PlayerId, 1, BattleRoyale.limePlayer01mouseAngle);

                if (target == null)
                {
                    limePlayer01KillButton.Timer = limePlayer01KillButton.MaxTimer;
                    return;
                }

                if (BattleRoyale.matchType == 2)
                {
                    if (BattleRoyale.pinkPlayer01 != null && target == BattleRoyale.pinkPlayer01 && BattleRoyale.pinkPlayer01IsReviving ||
                    BattleRoyale.pinkPlayer02 != null && target == BattleRoyale.pinkPlayer02 && BattleRoyale.pinkPlayer02IsReviving ||
                    BattleRoyale.pinkPlayer03 != null && target == BattleRoyale.pinkPlayer03 && BattleRoyale.pinkPlayer03IsReviving ||
                    BattleRoyale.pinkPlayer04 != null && target == BattleRoyale.pinkPlayer04 && BattleRoyale.pinkPlayer04IsReviving ||
                    BattleRoyale.pinkPlayer05 != null && target == BattleRoyale.pinkPlayer05 && BattleRoyale.pinkPlayer05IsReviving ||
                    BattleRoyale.pinkPlayer06 != null && target == BattleRoyale.pinkPlayer06 && BattleRoyale.pinkPlayer06IsReviving ||
                    BattleRoyale.pinkPlayer07 != null && target == BattleRoyale.pinkPlayer07 && BattleRoyale.pinkPlayer07IsReviving ||
                    BattleRoyale.serialKiller != null && target == BattleRoyale.serialKiller && BattleRoyale.serialKillerIsReviving)
                    {
                        limePlayer01KillButton.Timer = limePlayer01KillButton.MaxTimer;
                        return;
                    }
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(BattleRoyale.limePlayer01.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.gamemodeKills(target.PlayerId, BattleRoyale.limePlayer01.PlayerId);

                limePlayer01KillButton.Timer = limePlayer01KillButton.MaxTimer;
                SoundManager.Instance.PlaySound(AssetLoader.royaleHitPlayer, false, 100f);

                target = null;
            },
            () => { return BattleRoyale.limePlayer01 != null && BattleRoyale.limePlayer01 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (BattleRoyale.limePlayer01Wep == null)
                {
                    BattleRoyale.limePlayer01Wep = new GameObject("Weapon");
                    var renderer = BattleRoyale.limePlayer01Wep.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.Bow;
                    renderer.transform.parent = BattleRoyale.limePlayer01.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {

                    if (PlayerControl.LocalPlayer.Data.IsDead && BattleRoyale.matchType != 2)
                    {
                        BattleRoyale.limePlayer01Wep.SetActive(false);
                    }

                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    BattleRoyale.limePlayer01mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = BattleRoyale.limePlayer01.transform.position + new Vector3(0.8f * (float)Math.Cos(BattleRoyale.limePlayer01mouseAngle), 0.8f * (float)Math.Sin(BattleRoyale.limePlayer01mouseAngle));
                    BattleRoyale.limePlayer01Wep.transform.position += (targetPosition - BattleRoyale.limePlayer01Wep.transform.position) * 0.4f;
                    BattleRoyale.limePlayer01Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(BattleRoyale.limePlayer01mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(BattleRoyale.limePlayer01mouseAngle) < 0.0)
                    {
                        if (BattleRoyale.limePlayer01Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            BattleRoyale.limePlayer01Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (BattleRoyale.limePlayer01Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            BattleRoyale.limePlayer01Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                bool CanUse = true;
                if (BattleRoyale.limePlayer01IsReviving)
                {
                    CanUse = false;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { limePlayer01KillButton.Timer = limePlayer01KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            KeyCode.Mouse1,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // LimePlayer02 Kill
        limePlayer02KillButton = new CustomButton(
            () =>
            {
                PlayerControl target = BattleRoyale.GetLimeShotPlayer(2 * 0.2f, 6, 2);

                MessageWriter writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, Hazel.SendOption.Reliable, -1);
                writerShot.Write(BattleRoyale.limePlayer02.PlayerId);
                writerShot.Write(1);
                writerShot.Write(BattleRoyale.limePlayer02mouseAngle);
                AmongUsClient.Instance.FinishRpcImmediately(writerShot);
                RPCProcedure.battleRoyaleShowShoots(BattleRoyale.limePlayer02.PlayerId, 1, BattleRoyale.limePlayer02mouseAngle);

                if (target == null)
                {
                    limePlayer02KillButton.Timer = limePlayer02KillButton.MaxTimer;
                    return;
                }

                if (BattleRoyale.matchType == 2)
                {
                    if (BattleRoyale.pinkPlayer01 != null && target == BattleRoyale.pinkPlayer01 && BattleRoyale.pinkPlayer01IsReviving ||
                    BattleRoyale.pinkPlayer02 != null && target == BattleRoyale.pinkPlayer02 && BattleRoyale.pinkPlayer02IsReviving ||
                    BattleRoyale.pinkPlayer03 != null && target == BattleRoyale.pinkPlayer03 && BattleRoyale.pinkPlayer03IsReviving ||
                    BattleRoyale.pinkPlayer04 != null && target == BattleRoyale.pinkPlayer04 && BattleRoyale.pinkPlayer04IsReviving ||
                    BattleRoyale.pinkPlayer05 != null && target == BattleRoyale.pinkPlayer05 && BattleRoyale.pinkPlayer05IsReviving ||
                    BattleRoyale.pinkPlayer06 != null && target == BattleRoyale.pinkPlayer06 && BattleRoyale.pinkPlayer06IsReviving ||
                    BattleRoyale.pinkPlayer07 != null && target == BattleRoyale.pinkPlayer07 && BattleRoyale.pinkPlayer07IsReviving ||
                    BattleRoyale.serialKiller != null && target == BattleRoyale.serialKiller && BattleRoyale.serialKillerIsReviving)
                    {
                        limePlayer02KillButton.Timer = limePlayer02KillButton.MaxTimer;
                        return;
                    }
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(BattleRoyale.limePlayer02.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.gamemodeKills(target.PlayerId, BattleRoyale.limePlayer02.PlayerId);

                limePlayer02KillButton.Timer = limePlayer02KillButton.MaxTimer;
                SoundManager.Instance.PlaySound(AssetLoader.royaleHitPlayer, false, 100f);

                target = null;
            },
            () => { return BattleRoyale.limePlayer02 != null && BattleRoyale.limePlayer02 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (BattleRoyale.limePlayer02Wep == null)
                {
                    BattleRoyale.limePlayer02Wep = new GameObject("Weapon");
                    var renderer = BattleRoyale.limePlayer02Wep.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.Bow;
                    renderer.transform.parent = BattleRoyale.limePlayer02.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {

                    if (PlayerControl.LocalPlayer.Data.IsDead && BattleRoyale.matchType != 2)
                    {
                        BattleRoyale.limePlayer02Wep.SetActive(false);
                    }

                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    BattleRoyale.limePlayer02mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = BattleRoyale.limePlayer02.transform.position + new Vector3(0.8f * (float)Math.Cos(BattleRoyale.limePlayer02mouseAngle), 0.8f * (float)Math.Sin(BattleRoyale.limePlayer02mouseAngle));
                    BattleRoyale.limePlayer02Wep.transform.position += (targetPosition - BattleRoyale.limePlayer02Wep.transform.position) * 0.4f;
                    BattleRoyale.limePlayer02Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(BattleRoyale.limePlayer02mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(BattleRoyale.limePlayer02mouseAngle) < 0.0)
                    {
                        if (BattleRoyale.limePlayer02Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            BattleRoyale.limePlayer02Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (BattleRoyale.limePlayer02Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            BattleRoyale.limePlayer02Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                bool CanUse = true;
                if (BattleRoyale.limePlayer02IsReviving)
                {
                    CanUse = false;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { limePlayer02KillButton.Timer = limePlayer02KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            KeyCode.Mouse1,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // LimePlayer03 Kill
        limePlayer03KillButton = new CustomButton(
            () =>
            {
                PlayerControl target = BattleRoyale.GetLimeShotPlayer(2 * 0.2f, 6, 3);

                MessageWriter writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, Hazel.SendOption.Reliable, -1);
                writerShot.Write(BattleRoyale.limePlayer03.PlayerId);
                writerShot.Write(1);
                writerShot.Write(BattleRoyale.limePlayer03mouseAngle);
                AmongUsClient.Instance.FinishRpcImmediately(writerShot);
                RPCProcedure.battleRoyaleShowShoots(BattleRoyale.limePlayer03.PlayerId, 1, BattleRoyale.limePlayer03mouseAngle);

                if (target == null)
                {
                    limePlayer03KillButton.Timer = limePlayer03KillButton.MaxTimer;
                    return;
                }

                if (BattleRoyale.matchType == 2)
                {
                    if (BattleRoyale.pinkPlayer01 != null && target == BattleRoyale.pinkPlayer01 && BattleRoyale.pinkPlayer01IsReviving ||
                    BattleRoyale.pinkPlayer02 != null && target == BattleRoyale.pinkPlayer02 && BattleRoyale.pinkPlayer02IsReviving ||
                    BattleRoyale.pinkPlayer03 != null && target == BattleRoyale.pinkPlayer03 && BattleRoyale.pinkPlayer03IsReviving ||
                    BattleRoyale.pinkPlayer04 != null && target == BattleRoyale.pinkPlayer04 && BattleRoyale.pinkPlayer04IsReviving ||
                    BattleRoyale.pinkPlayer05 != null && target == BattleRoyale.pinkPlayer05 && BattleRoyale.pinkPlayer05IsReviving ||
                    BattleRoyale.pinkPlayer06 != null && target == BattleRoyale.pinkPlayer06 && BattleRoyale.pinkPlayer06IsReviving ||
                    BattleRoyale.pinkPlayer07 != null && target == BattleRoyale.pinkPlayer07 && BattleRoyale.pinkPlayer07IsReviving ||
                    BattleRoyale.serialKiller != null && target == BattleRoyale.serialKiller && BattleRoyale.serialKillerIsReviving)
                    {
                        limePlayer03KillButton.Timer = limePlayer03KillButton.MaxTimer;
                        return;
                    }
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(BattleRoyale.limePlayer03.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.gamemodeKills(target.PlayerId, BattleRoyale.limePlayer03.PlayerId);

                limePlayer03KillButton.Timer = limePlayer03KillButton.MaxTimer;
                SoundManager.Instance.PlaySound(AssetLoader.royaleHitPlayer, false, 100f);

                target = null;
            },
            () => { return BattleRoyale.limePlayer03 != null && BattleRoyale.limePlayer03 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (BattleRoyale.limePlayer03Wep == null)
                {
                    BattleRoyale.limePlayer03Wep = new GameObject("Weapon");
                    var renderer = BattleRoyale.limePlayer03Wep.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.Bow;
                    renderer.transform.parent = BattleRoyale.limePlayer03.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {

                    if (PlayerControl.LocalPlayer.Data.IsDead && BattleRoyale.matchType != 2)
                    {
                        BattleRoyale.limePlayer03Wep.SetActive(false);
                    }

                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    BattleRoyale.limePlayer03mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = BattleRoyale.limePlayer03.transform.position + new Vector3(0.8f * (float)Math.Cos(BattleRoyale.limePlayer03mouseAngle), 0.8f * (float)Math.Sin(BattleRoyale.limePlayer03mouseAngle));
                    BattleRoyale.limePlayer03Wep.transform.position += (targetPosition - BattleRoyale.limePlayer03Wep.transform.position) * 0.4f;
                    BattleRoyale.limePlayer03Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(BattleRoyale.limePlayer03mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(BattleRoyale.limePlayer03mouseAngle) < 0.0)
                    {
                        if (BattleRoyale.limePlayer03Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            BattleRoyale.limePlayer03Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (BattleRoyale.limePlayer03Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            BattleRoyale.limePlayer03Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                bool CanUse = true;
                if (BattleRoyale.limePlayer03IsReviving)
                {
                    CanUse = false;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { limePlayer03KillButton.Timer = limePlayer03KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            KeyCode.Mouse1,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // LimePlayer04 Kill
        limePlayer04KillButton = new CustomButton(
            () =>
            {
                PlayerControl target = BattleRoyale.GetLimeShotPlayer(2 * 0.2f, 6, 4);

                MessageWriter writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, Hazel.SendOption.Reliable, -1);
                writerShot.Write(BattleRoyale.limePlayer04.PlayerId);
                writerShot.Write(1);
                writerShot.Write(BattleRoyale.limePlayer04mouseAngle);
                AmongUsClient.Instance.FinishRpcImmediately(writerShot);
                RPCProcedure.battleRoyaleShowShoots(BattleRoyale.limePlayer04.PlayerId, 1, BattleRoyale.limePlayer04mouseAngle);

                if (target == null)
                {
                    limePlayer04KillButton.Timer = limePlayer04KillButton.MaxTimer;
                    return;
                }

                if (BattleRoyale.matchType == 2)
                {
                    if (BattleRoyale.pinkPlayer01 != null && target == BattleRoyale.pinkPlayer01 && BattleRoyale.pinkPlayer01IsReviving ||
                    BattleRoyale.pinkPlayer02 != null && target == BattleRoyale.pinkPlayer02 && BattleRoyale.pinkPlayer02IsReviving ||
                    BattleRoyale.pinkPlayer03 != null && target == BattleRoyale.pinkPlayer03 && BattleRoyale.pinkPlayer03IsReviving ||
                    BattleRoyale.pinkPlayer04 != null && target == BattleRoyale.pinkPlayer04 && BattleRoyale.pinkPlayer04IsReviving ||
                    BattleRoyale.pinkPlayer05 != null && target == BattleRoyale.pinkPlayer05 && BattleRoyale.pinkPlayer05IsReviving ||
                    BattleRoyale.pinkPlayer06 != null && target == BattleRoyale.pinkPlayer06 && BattleRoyale.pinkPlayer06IsReviving ||
                    BattleRoyale.pinkPlayer07 != null && target == BattleRoyale.pinkPlayer07 && BattleRoyale.pinkPlayer07IsReviving ||
                    BattleRoyale.serialKiller != null && target == BattleRoyale.serialKiller && BattleRoyale.serialKillerIsReviving)
                    {
                        limePlayer04KillButton.Timer = limePlayer04KillButton.MaxTimer;
                        return;
                    }
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(BattleRoyale.limePlayer04.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.gamemodeKills(target.PlayerId, BattleRoyale.limePlayer04.PlayerId);

                limePlayer04KillButton.Timer = limePlayer04KillButton.MaxTimer;
                SoundManager.Instance.PlaySound(AssetLoader.royaleHitPlayer, false, 100f);

                target = null;
            },
            () => { return BattleRoyale.limePlayer04 != null && BattleRoyale.limePlayer04 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (BattleRoyale.limePlayer04Wep == null)
                {
                    BattleRoyale.limePlayer04Wep = new GameObject("Weapon");
                    var renderer = BattleRoyale.limePlayer04Wep.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.Bow;
                    renderer.transform.parent = BattleRoyale.limePlayer04.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {

                    if (PlayerControl.LocalPlayer.Data.IsDead && BattleRoyale.matchType != 2)
                    {
                        BattleRoyale.limePlayer04Wep.SetActive(false);
                    }

                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    BattleRoyale.limePlayer04mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = BattleRoyale.limePlayer04.transform.position + new Vector3(0.8f * (float)Math.Cos(BattleRoyale.limePlayer04mouseAngle), 0.8f * (float)Math.Sin(BattleRoyale.limePlayer04mouseAngle));
                    BattleRoyale.limePlayer04Wep.transform.position += (targetPosition - BattleRoyale.limePlayer04Wep.transform.position) * 0.4f;
                    BattleRoyale.limePlayer04Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(BattleRoyale.limePlayer04mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(BattleRoyale.limePlayer04mouseAngle) < 0.0)
                    {
                        if (BattleRoyale.limePlayer04Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            BattleRoyale.limePlayer04Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (BattleRoyale.limePlayer04Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            BattleRoyale.limePlayer04Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                bool CanUse = true;
                if (BattleRoyale.limePlayer04IsReviving)
                {
                    CanUse = false;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { limePlayer04KillButton.Timer = limePlayer04KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            KeyCode.Mouse1,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // LimePlayer05 Kill
        limePlayer05KillButton = new CustomButton(
            () =>
            {
                PlayerControl target = BattleRoyale.GetLimeShotPlayer(2 * 0.2f, 6, 5);

                MessageWriter writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, Hazel.SendOption.Reliable, -1);
                writerShot.Write(BattleRoyale.limePlayer05.PlayerId);
                writerShot.Write(1);
                writerShot.Write(BattleRoyale.limePlayer05mouseAngle);
                AmongUsClient.Instance.FinishRpcImmediately(writerShot);
                RPCProcedure.battleRoyaleShowShoots(BattleRoyale.limePlayer05.PlayerId, 1, BattleRoyale.limePlayer05mouseAngle);

                if (target == null)
                {
                    limePlayer05KillButton.Timer = limePlayer05KillButton.MaxTimer;
                    return;
                }

                if (BattleRoyale.matchType == 2)
                {
                    if (BattleRoyale.pinkPlayer01 != null && target == BattleRoyale.pinkPlayer01 && BattleRoyale.pinkPlayer01IsReviving ||
                    BattleRoyale.pinkPlayer02 != null && target == BattleRoyale.pinkPlayer02 && BattleRoyale.pinkPlayer02IsReviving ||
                    BattleRoyale.pinkPlayer03 != null && target == BattleRoyale.pinkPlayer03 && BattleRoyale.pinkPlayer03IsReviving ||
                    BattleRoyale.pinkPlayer04 != null && target == BattleRoyale.pinkPlayer04 && BattleRoyale.pinkPlayer04IsReviving ||
                    BattleRoyale.pinkPlayer05 != null && target == BattleRoyale.pinkPlayer05 && BattleRoyale.pinkPlayer05IsReviving ||
                    BattleRoyale.pinkPlayer06 != null && target == BattleRoyale.pinkPlayer06 && BattleRoyale.pinkPlayer06IsReviving ||
                    BattleRoyale.pinkPlayer07 != null && target == BattleRoyale.pinkPlayer07 && BattleRoyale.pinkPlayer07IsReviving ||
                    BattleRoyale.serialKiller != null && target == BattleRoyale.serialKiller && BattleRoyale.serialKillerIsReviving)
                    {
                        limePlayer05KillButton.Timer = limePlayer05KillButton.MaxTimer;
                        return;
                    }
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(BattleRoyale.limePlayer05.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.gamemodeKills(target.PlayerId, BattleRoyale.limePlayer05.PlayerId);

                limePlayer05KillButton.Timer = limePlayer05KillButton.MaxTimer;
                SoundManager.Instance.PlaySound(AssetLoader.royaleHitPlayer, false, 100f);

                target = null;
            },
            () => { return BattleRoyale.limePlayer05 != null && BattleRoyale.limePlayer05 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (BattleRoyale.limePlayer05Wep == null)
                {
                    BattleRoyale.limePlayer05Wep = new GameObject("Weapon");
                    var renderer = BattleRoyale.limePlayer05Wep.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.Bow;
                    renderer.transform.parent = BattleRoyale.limePlayer05.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {

                    if (PlayerControl.LocalPlayer.Data.IsDead && BattleRoyale.matchType != 2)
                    {
                        BattleRoyale.limePlayer05Wep.SetActive(false);
                    }

                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    BattleRoyale.limePlayer05mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = BattleRoyale.limePlayer05.transform.position + new Vector3(0.8f * (float)Math.Cos(BattleRoyale.limePlayer05mouseAngle), 0.8f * (float)Math.Sin(BattleRoyale.limePlayer05mouseAngle));
                    BattleRoyale.limePlayer05Wep.transform.position += (targetPosition - BattleRoyale.limePlayer05Wep.transform.position) * 0.4f;
                    BattleRoyale.limePlayer05Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(BattleRoyale.limePlayer05mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(BattleRoyale.limePlayer05mouseAngle) < 0.0)
                    {
                        if (BattleRoyale.limePlayer05Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            BattleRoyale.limePlayer05Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (BattleRoyale.limePlayer05Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            BattleRoyale.limePlayer05Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                bool CanUse = true;
                if (BattleRoyale.limePlayer05IsReviving)
                {
                    CanUse = false;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { limePlayer05KillButton.Timer = limePlayer05KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            KeyCode.Mouse1,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // LimePlayer06 Kill
        limePlayer06KillButton = new CustomButton(
            () =>
            {
                PlayerControl target = BattleRoyale.GetLimeShotPlayer(2 * 0.2f, 6, 6);

                MessageWriter writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, Hazel.SendOption.Reliable, -1);
                writerShot.Write(BattleRoyale.limePlayer06.PlayerId);
                writerShot.Write(1);
                writerShot.Write(BattleRoyale.limePlayer06mouseAngle);
                AmongUsClient.Instance.FinishRpcImmediately(writerShot);
                RPCProcedure.battleRoyaleShowShoots(BattleRoyale.limePlayer06.PlayerId, 1, BattleRoyale.limePlayer06mouseAngle);

                if (target == null)
                {
                    limePlayer06KillButton.Timer = limePlayer06KillButton.MaxTimer;
                    return;
                }

                if (BattleRoyale.matchType == 2)
                {
                    if (BattleRoyale.pinkPlayer01 != null && target == BattleRoyale.pinkPlayer01 && BattleRoyale.pinkPlayer01IsReviving ||
                    BattleRoyale.pinkPlayer02 != null && target == BattleRoyale.pinkPlayer02 && BattleRoyale.pinkPlayer02IsReviving ||
                    BattleRoyale.pinkPlayer03 != null && target == BattleRoyale.pinkPlayer03 && BattleRoyale.pinkPlayer03IsReviving ||
                    BattleRoyale.pinkPlayer04 != null && target == BattleRoyale.pinkPlayer04 && BattleRoyale.pinkPlayer04IsReviving ||
                    BattleRoyale.pinkPlayer05 != null && target == BattleRoyale.pinkPlayer05 && BattleRoyale.pinkPlayer05IsReviving ||
                    BattleRoyale.pinkPlayer06 != null && target == BattleRoyale.pinkPlayer06 && BattleRoyale.pinkPlayer06IsReviving ||
                    BattleRoyale.pinkPlayer07 != null && target == BattleRoyale.pinkPlayer07 && BattleRoyale.pinkPlayer07IsReviving ||
                    BattleRoyale.serialKiller != null && target == BattleRoyale.serialKiller && BattleRoyale.serialKillerIsReviving)
                    {
                        limePlayer06KillButton.Timer = limePlayer06KillButton.MaxTimer;
                        return;
                    }
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(BattleRoyale.limePlayer06.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.gamemodeKills(target.PlayerId, BattleRoyale.limePlayer06.PlayerId);

                limePlayer06KillButton.Timer = limePlayer06KillButton.MaxTimer;
                SoundManager.Instance.PlaySound(AssetLoader.royaleHitPlayer, false, 100f);

                target = null;
            },
            () => { return BattleRoyale.limePlayer06 != null && BattleRoyale.limePlayer06 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (BattleRoyale.limePlayer06Wep == null)
                {
                    BattleRoyale.limePlayer06Wep = new GameObject("Weapon");
                    var renderer = BattleRoyale.limePlayer06Wep.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.Bow;
                    renderer.transform.parent = BattleRoyale.limePlayer06.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {

                    if (PlayerControl.LocalPlayer.Data.IsDead && BattleRoyale.matchType != 2)
                    {
                        BattleRoyale.limePlayer06Wep.SetActive(false);
                    }

                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    BattleRoyale.limePlayer06mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = BattleRoyale.limePlayer06.transform.position + new Vector3(0.8f * (float)Math.Cos(BattleRoyale.limePlayer06mouseAngle), 0.8f * (float)Math.Sin(BattleRoyale.limePlayer06mouseAngle));
                    BattleRoyale.limePlayer06Wep.transform.position += (targetPosition - BattleRoyale.limePlayer06Wep.transform.position) * 0.4f;
                    BattleRoyale.limePlayer06Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(BattleRoyale.limePlayer06mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(BattleRoyale.limePlayer06mouseAngle) < 0.0)
                    {
                        if (BattleRoyale.limePlayer06Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            BattleRoyale.limePlayer06Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (BattleRoyale.limePlayer06Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            BattleRoyale.limePlayer06Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                bool CanUse = true;
                if (BattleRoyale.limePlayer06IsReviving)
                {
                    CanUse = false;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { limePlayer06KillButton.Timer = limePlayer06KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            KeyCode.Mouse1,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // LimePlayer07 Kill
        limePlayer07KillButton = new CustomButton(
            () =>
            {
                PlayerControl target = BattleRoyale.GetLimeShotPlayer(2 * 0.2f, 6, 7);

                MessageWriter writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, Hazel.SendOption.Reliable, -1);
                writerShot.Write(BattleRoyale.limePlayer07.PlayerId);
                writerShot.Write(1);
                writerShot.Write(BattleRoyale.limePlayer07mouseAngle);
                AmongUsClient.Instance.FinishRpcImmediately(writerShot);
                RPCProcedure.battleRoyaleShowShoots(BattleRoyale.limePlayer07.PlayerId, 1, BattleRoyale.limePlayer07mouseAngle);

                if (target == null)
                {
                    limePlayer07KillButton.Timer = limePlayer07KillButton.MaxTimer;
                    return;
                }

                if (BattleRoyale.matchType == 2)
                {
                    if (BattleRoyale.pinkPlayer01 != null && target == BattleRoyale.pinkPlayer01 && BattleRoyale.pinkPlayer01IsReviving ||
                    BattleRoyale.pinkPlayer02 != null && target == BattleRoyale.pinkPlayer02 && BattleRoyale.pinkPlayer02IsReviving ||
                    BattleRoyale.pinkPlayer03 != null && target == BattleRoyale.pinkPlayer03 && BattleRoyale.pinkPlayer03IsReviving ||
                    BattleRoyale.pinkPlayer04 != null && target == BattleRoyale.pinkPlayer04 && BattleRoyale.pinkPlayer04IsReviving ||
                    BattleRoyale.pinkPlayer05 != null && target == BattleRoyale.pinkPlayer05 && BattleRoyale.pinkPlayer05IsReviving ||
                    BattleRoyale.pinkPlayer06 != null && target == BattleRoyale.pinkPlayer06 && BattleRoyale.pinkPlayer06IsReviving ||
                    BattleRoyale.pinkPlayer07 != null && target == BattleRoyale.pinkPlayer07 && BattleRoyale.pinkPlayer07IsReviving ||
                    BattleRoyale.serialKiller != null && target == BattleRoyale.serialKiller && BattleRoyale.serialKillerIsReviving)
                    {
                        limePlayer07KillButton.Timer = limePlayer07KillButton.MaxTimer;
                        return;
                    }
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(BattleRoyale.limePlayer07.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.gamemodeKills(target.PlayerId, BattleRoyale.limePlayer07.PlayerId);

                limePlayer07KillButton.Timer = limePlayer07KillButton.MaxTimer;
                SoundManager.Instance.PlaySound(AssetLoader.royaleHitPlayer, false, 100f);

                target = null;
            },
            () => { return BattleRoyale.limePlayer07 != null && BattleRoyale.limePlayer07 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (BattleRoyale.limePlayer07Wep == null)
                {
                    BattleRoyale.limePlayer07Wep = new GameObject("Weapon");
                    var renderer = BattleRoyale.limePlayer07Wep.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.Bow;
                    renderer.transform.parent = BattleRoyale.limePlayer07.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {

                    if (PlayerControl.LocalPlayer.Data.IsDead && BattleRoyale.matchType != 2)
                    {
                        BattleRoyale.limePlayer07Wep.SetActive(false);
                    }

                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    BattleRoyale.limePlayer07mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = BattleRoyale.limePlayer07.transform.position + new Vector3(0.8f * (float)Math.Cos(BattleRoyale.limePlayer07mouseAngle), 0.8f * (float)Math.Sin(BattleRoyale.limePlayer07mouseAngle));
                    BattleRoyale.limePlayer07Wep.transform.position += (targetPosition - BattleRoyale.limePlayer07Wep.transform.position) * 0.4f;
                    BattleRoyale.limePlayer07Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(BattleRoyale.limePlayer07mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(BattleRoyale.limePlayer07mouseAngle) < 0.0)
                    {
                        if (BattleRoyale.limePlayer07Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            BattleRoyale.limePlayer07Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (BattleRoyale.limePlayer07Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            BattleRoyale.limePlayer07Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                bool CanUse = true;
                if (BattleRoyale.limePlayer07IsReviving)
                {
                    CanUse = false;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { limePlayer07KillButton.Timer = limePlayer07KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            KeyCode.Mouse1,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // PinkPlayer01 Kill
        pinkPlayer01KillButton = new CustomButton(
            () =>
            {
                PlayerControl target = BattleRoyale.GetPinkShotPlayer(2 * 0.2f, 6, 1);

                MessageWriter writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, Hazel.SendOption.Reliable, -1);
                writerShot.Write(BattleRoyale.pinkPlayer01.PlayerId);
                writerShot.Write(2);
                writerShot.Write(BattleRoyale.pinkPlayer01mouseAngle);
                AmongUsClient.Instance.FinishRpcImmediately(writerShot);
                RPCProcedure.battleRoyaleShowShoots(BattleRoyale.pinkPlayer01.PlayerId, 2, BattleRoyale.pinkPlayer01mouseAngle);

                if (target == null)
                {
                    pinkPlayer01KillButton.Timer = pinkPlayer01KillButton.MaxTimer;
                    return;
                }

                if (BattleRoyale.matchType == 2)
                {
                    if (BattleRoyale.limePlayer01 != null && target == BattleRoyale.limePlayer01 && BattleRoyale.limePlayer01IsReviving ||
                    BattleRoyale.limePlayer02 != null && target == BattleRoyale.limePlayer02 && BattleRoyale.limePlayer02IsReviving ||
                    BattleRoyale.limePlayer03 != null && target == BattleRoyale.limePlayer03 && BattleRoyale.limePlayer03IsReviving ||
                    BattleRoyale.limePlayer04 != null && target == BattleRoyale.limePlayer04 && BattleRoyale.limePlayer04IsReviving ||
                    BattleRoyale.limePlayer05 != null && target == BattleRoyale.limePlayer05 && BattleRoyale.limePlayer05IsReviving ||
                    BattleRoyale.limePlayer06 != null && target == BattleRoyale.limePlayer06 && BattleRoyale.limePlayer06IsReviving ||
                    BattleRoyale.limePlayer07 != null && target == BattleRoyale.limePlayer07 && BattleRoyale.limePlayer07IsReviving ||
                    BattleRoyale.serialKiller != null && target == BattleRoyale.serialKiller && BattleRoyale.serialKillerIsReviving)
                    {
                        pinkPlayer01KillButton.Timer = pinkPlayer01KillButton.MaxTimer;
                        return;
                    }
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(BattleRoyale.pinkPlayer01.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.gamemodeKills(target.PlayerId, BattleRoyale.pinkPlayer01.PlayerId);

                pinkPlayer01KillButton.Timer = pinkPlayer01KillButton.MaxTimer;
                SoundManager.Instance.PlaySound(AssetLoader.royaleHitPlayer, false, 100f);

                target = null;
            },
            () => { return BattleRoyale.pinkPlayer01 != null && BattleRoyale.pinkPlayer01 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (BattleRoyale.pinkPlayer01Wep == null)
                {
                    BattleRoyale.pinkPlayer01Wep = new GameObject("Weapon");
                    var renderer = BattleRoyale.pinkPlayer01Wep.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.Bow;
                    renderer.transform.parent = BattleRoyale.pinkPlayer01.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {

                    if (PlayerControl.LocalPlayer.Data.IsDead && BattleRoyale.matchType != 2)
                    {
                        BattleRoyale.pinkPlayer01Wep.SetActive(false);
                    }

                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    BattleRoyale.pinkPlayer01mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = BattleRoyale.pinkPlayer01.transform.position + new Vector3(0.8f * (float)Math.Cos(BattleRoyale.pinkPlayer01mouseAngle), 0.8f * (float)Math.Sin(BattleRoyale.pinkPlayer01mouseAngle));
                    BattleRoyale.pinkPlayer01Wep.transform.position += (targetPosition - BattleRoyale.pinkPlayer01Wep.transform.position) * 0.4f;
                    BattleRoyale.pinkPlayer01Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(BattleRoyale.pinkPlayer01mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(BattleRoyale.pinkPlayer01mouseAngle) < 0.0)
                    {
                        if (BattleRoyale.pinkPlayer01Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            BattleRoyale.pinkPlayer01Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (BattleRoyale.pinkPlayer01Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            BattleRoyale.pinkPlayer01Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                bool CanUse = true;
                if (BattleRoyale.pinkPlayer01IsReviving)
                {
                    CanUse = false;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { pinkPlayer01KillButton.Timer = pinkPlayer01KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            KeyCode.Mouse1,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // PinkPlayer02 Kill
        pinkPlayer02KillButton = new CustomButton(
            () =>
            {
                PlayerControl target = BattleRoyale.GetPinkShotPlayer(2 * 0.2f, 6, 2);

                MessageWriter writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, Hazel.SendOption.Reliable, -1);
                writerShot.Write(BattleRoyale.pinkPlayer02.PlayerId);
                writerShot.Write(2);
                writerShot.Write(BattleRoyale.pinkPlayer02mouseAngle);
                AmongUsClient.Instance.FinishRpcImmediately(writerShot);
                RPCProcedure.battleRoyaleShowShoots(BattleRoyale.pinkPlayer02.PlayerId, 2, BattleRoyale.pinkPlayer02mouseAngle);

                if (target == null)
                {
                    pinkPlayer02KillButton.Timer = pinkPlayer02KillButton.MaxTimer;
                    return;
                }

                if (BattleRoyale.matchType == 2)
                {
                    if (BattleRoyale.limePlayer01 != null && target == BattleRoyale.limePlayer01 && BattleRoyale.limePlayer01IsReviving ||
                    BattleRoyale.limePlayer02 != null && target == BattleRoyale.limePlayer02 && BattleRoyale.limePlayer02IsReviving ||
                    BattleRoyale.limePlayer03 != null && target == BattleRoyale.limePlayer03 && BattleRoyale.limePlayer03IsReviving ||
                    BattleRoyale.limePlayer04 != null && target == BattleRoyale.limePlayer04 && BattleRoyale.limePlayer04IsReviving ||
                    BattleRoyale.limePlayer05 != null && target == BattleRoyale.limePlayer05 && BattleRoyale.limePlayer05IsReviving ||
                    BattleRoyale.limePlayer06 != null && target == BattleRoyale.limePlayer06 && BattleRoyale.limePlayer06IsReviving ||
                    BattleRoyale.limePlayer07 != null && target == BattleRoyale.limePlayer07 && BattleRoyale.limePlayer07IsReviving ||
                    BattleRoyale.serialKiller != null && target == BattleRoyale.serialKiller && BattleRoyale.serialKillerIsReviving)
                    {
                        pinkPlayer02KillButton.Timer = pinkPlayer02KillButton.MaxTimer;
                        return;
                    }
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(BattleRoyale.pinkPlayer02.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.gamemodeKills(target.PlayerId, BattleRoyale.pinkPlayer02.PlayerId);

                pinkPlayer02KillButton.Timer = pinkPlayer02KillButton.MaxTimer;
                SoundManager.Instance.PlaySound(AssetLoader.royaleHitPlayer, false, 100f);

                target = null;
            },
            () => { return BattleRoyale.pinkPlayer02 != null && BattleRoyale.pinkPlayer02 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (BattleRoyale.pinkPlayer02Wep == null)
                {
                    BattleRoyale.pinkPlayer02Wep = new GameObject("Weapon");
                    var renderer = BattleRoyale.pinkPlayer02Wep.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.Bow;
                    renderer.transform.parent = BattleRoyale.pinkPlayer02.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {

                    if (PlayerControl.LocalPlayer.Data.IsDead && BattleRoyale.matchType != 2)
                    {
                        BattleRoyale.pinkPlayer02Wep.SetActive(false);
                    }

                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    BattleRoyale.pinkPlayer02mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = BattleRoyale.pinkPlayer02.transform.position + new Vector3(0.8f * (float)Math.Cos(BattleRoyale.pinkPlayer02mouseAngle), 0.8f * (float)Math.Sin(BattleRoyale.pinkPlayer02mouseAngle));
                    BattleRoyale.pinkPlayer02Wep.transform.position += (targetPosition - BattleRoyale.pinkPlayer02Wep.transform.position) * 0.4f;
                    BattleRoyale.pinkPlayer02Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(BattleRoyale.pinkPlayer02mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(BattleRoyale.pinkPlayer02mouseAngle) < 0.0)
                    {
                        if (BattleRoyale.pinkPlayer02Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            BattleRoyale.pinkPlayer02Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (BattleRoyale.pinkPlayer02Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            BattleRoyale.pinkPlayer02Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                bool CanUse = true;
                if (BattleRoyale.pinkPlayer02IsReviving)
                {
                    CanUse = false;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { pinkPlayer02KillButton.Timer = pinkPlayer02KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            KeyCode.Mouse1,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // PinkPlayer03 Kill
        pinkPlayer03KillButton = new CustomButton(
            () =>
            {
                PlayerControl target = BattleRoyale.GetPinkShotPlayer(2 * 0.2f, 6, 3);

                MessageWriter writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, Hazel.SendOption.Reliable, -1);
                writerShot.Write(BattleRoyale.pinkPlayer03.PlayerId);
                writerShot.Write(2);
                writerShot.Write(BattleRoyale.pinkPlayer03mouseAngle);
                AmongUsClient.Instance.FinishRpcImmediately(writerShot);
                RPCProcedure.battleRoyaleShowShoots(BattleRoyale.pinkPlayer03.PlayerId, 2, BattleRoyale.pinkPlayer03mouseAngle);

                if (target == null)
                {
                    pinkPlayer03KillButton.Timer = pinkPlayer03KillButton.MaxTimer;
                    return;
                }

                if (BattleRoyale.matchType == 2)
                {
                    if (BattleRoyale.limePlayer01 != null && target == BattleRoyale.limePlayer01 && BattleRoyale.limePlayer01IsReviving ||
                    BattleRoyale.limePlayer02 != null && target == BattleRoyale.limePlayer02 && BattleRoyale.limePlayer02IsReviving ||
                    BattleRoyale.limePlayer03 != null && target == BattleRoyale.limePlayer03 && BattleRoyale.limePlayer03IsReviving ||
                    BattleRoyale.limePlayer04 != null && target == BattleRoyale.limePlayer04 && BattleRoyale.limePlayer04IsReviving ||
                    BattleRoyale.limePlayer05 != null && target == BattleRoyale.limePlayer05 && BattleRoyale.limePlayer05IsReviving ||
                    BattleRoyale.limePlayer06 != null && target == BattleRoyale.limePlayer06 && BattleRoyale.limePlayer06IsReviving ||
                    BattleRoyale.limePlayer07 != null && target == BattleRoyale.limePlayer07 && BattleRoyale.limePlayer07IsReviving ||
                    BattleRoyale.serialKiller != null && target == BattleRoyale.serialKiller && BattleRoyale.serialKillerIsReviving)
                    {
                        pinkPlayer03KillButton.Timer = pinkPlayer03KillButton.MaxTimer;
                        return;
                    }
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(BattleRoyale.pinkPlayer03.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.gamemodeKills(target.PlayerId, BattleRoyale.pinkPlayer03.PlayerId);

                pinkPlayer03KillButton.Timer = pinkPlayer03KillButton.MaxTimer;
                SoundManager.Instance.PlaySound(AssetLoader.royaleHitPlayer, false, 100f);

                target = null;
            },
            () => { return BattleRoyale.pinkPlayer03 != null && BattleRoyale.pinkPlayer03 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (BattleRoyale.pinkPlayer03Wep == null)
                {
                    BattleRoyale.pinkPlayer03Wep = new GameObject("Weapon");
                    var renderer = BattleRoyale.pinkPlayer03Wep.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.Bow;
                    renderer.transform.parent = BattleRoyale.pinkPlayer03.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {

                    if (PlayerControl.LocalPlayer.Data.IsDead && BattleRoyale.matchType != 2)
                    {
                        BattleRoyale.pinkPlayer03Wep.SetActive(false);
                    }

                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    BattleRoyale.pinkPlayer03mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = BattleRoyale.pinkPlayer03.transform.position + new Vector3(0.8f * (float)Math.Cos(BattleRoyale.pinkPlayer03mouseAngle), 0.8f * (float)Math.Sin(BattleRoyale.pinkPlayer03mouseAngle));
                    BattleRoyale.pinkPlayer03Wep.transform.position += (targetPosition - BattleRoyale.pinkPlayer03Wep.transform.position) * 0.4f;
                    BattleRoyale.pinkPlayer03Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(BattleRoyale.pinkPlayer03mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(BattleRoyale.pinkPlayer03mouseAngle) < 0.0)
                    {
                        if (BattleRoyale.pinkPlayer03Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            BattleRoyale.pinkPlayer03Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (BattleRoyale.pinkPlayer03Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            BattleRoyale.pinkPlayer03Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                bool CanUse = true;
                if (BattleRoyale.pinkPlayer03IsReviving)
                {
                    CanUse = false;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { pinkPlayer03KillButton.Timer = pinkPlayer03KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            KeyCode.Mouse1,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // PinkPlayer04 Kill
        pinkPlayer04KillButton = new CustomButton(
            () =>
            {
                PlayerControl target = BattleRoyale.GetPinkShotPlayer(2 * 0.2f, 6, 4);

                MessageWriter writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, Hazel.SendOption.Reliable, -1);
                writerShot.Write(BattleRoyale.pinkPlayer04.PlayerId);
                writerShot.Write(2);
                writerShot.Write(BattleRoyale.pinkPlayer04mouseAngle);
                AmongUsClient.Instance.FinishRpcImmediately(writerShot);
                RPCProcedure.battleRoyaleShowShoots(BattleRoyale.pinkPlayer04.PlayerId, 2, BattleRoyale.pinkPlayer04mouseAngle);

                if (target == null)
                {
                    pinkPlayer04KillButton.Timer = pinkPlayer04KillButton.MaxTimer;
                    return;
                }

                if (BattleRoyale.matchType == 2)
                {
                    if (BattleRoyale.limePlayer01 != null && target == BattleRoyale.limePlayer01 && BattleRoyale.limePlayer01IsReviving ||
                    BattleRoyale.limePlayer02 != null && target == BattleRoyale.limePlayer02 && BattleRoyale.limePlayer02IsReviving ||
                    BattleRoyale.limePlayer03 != null && target == BattleRoyale.limePlayer03 && BattleRoyale.limePlayer03IsReviving ||
                    BattleRoyale.limePlayer04 != null && target == BattleRoyale.limePlayer04 && BattleRoyale.limePlayer04IsReviving ||
                    BattleRoyale.limePlayer05 != null && target == BattleRoyale.limePlayer05 && BattleRoyale.limePlayer05IsReviving ||
                    BattleRoyale.limePlayer06 != null && target == BattleRoyale.limePlayer06 && BattleRoyale.limePlayer06IsReviving ||
                    BattleRoyale.limePlayer07 != null && target == BattleRoyale.limePlayer07 && BattleRoyale.limePlayer07IsReviving ||
                    BattleRoyale.serialKiller != null && target == BattleRoyale.serialKiller && BattleRoyale.serialKillerIsReviving)
                    {
                        pinkPlayer04KillButton.Timer = pinkPlayer04KillButton.MaxTimer;
                        return;
                    }
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(BattleRoyale.pinkPlayer04.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.gamemodeKills(target.PlayerId, BattleRoyale.pinkPlayer04.PlayerId);

                pinkPlayer04KillButton.Timer = pinkPlayer04KillButton.MaxTimer;
                SoundManager.Instance.PlaySound(AssetLoader.royaleHitPlayer, false, 100f);

                target = null;
            },
            () => { return BattleRoyale.pinkPlayer04 != null && BattleRoyale.pinkPlayer04 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (BattleRoyale.pinkPlayer04Wep == null)
                {
                    BattleRoyale.pinkPlayer04Wep = new GameObject("Weapon");
                    var renderer = BattleRoyale.pinkPlayer04Wep.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.Bow;
                    renderer.transform.parent = BattleRoyale.pinkPlayer04.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {

                    if (PlayerControl.LocalPlayer.Data.IsDead && BattleRoyale.matchType != 2)
                    {
                        BattleRoyale.pinkPlayer04Wep.SetActive(false);
                    }

                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    BattleRoyale.pinkPlayer04mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = BattleRoyale.pinkPlayer04.transform.position + new Vector3(0.8f * (float)Math.Cos(BattleRoyale.pinkPlayer04mouseAngle), 0.8f * (float)Math.Sin(BattleRoyale.pinkPlayer04mouseAngle));
                    BattleRoyale.pinkPlayer04Wep.transform.position += (targetPosition - BattleRoyale.pinkPlayer04Wep.transform.position) * 0.4f;
                    BattleRoyale.pinkPlayer04Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(BattleRoyale.pinkPlayer04mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(BattleRoyale.pinkPlayer04mouseAngle) < 0.0)
                    {
                        if (BattleRoyale.pinkPlayer04Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            BattleRoyale.pinkPlayer04Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (BattleRoyale.pinkPlayer04Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            BattleRoyale.pinkPlayer04Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                bool CanUse = true;
                if (BattleRoyale.pinkPlayer04IsReviving)
                {
                    CanUse = false;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { pinkPlayer04KillButton.Timer = pinkPlayer04KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            KeyCode.Mouse1,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // PinkPlayer05 Kill
        pinkPlayer05KillButton = new CustomButton(
            () =>
            {
                PlayerControl target = BattleRoyale.GetPinkShotPlayer(2 * 0.2f, 6, 5);

                MessageWriter writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, Hazel.SendOption.Reliable, -1);
                writerShot.Write(BattleRoyale.pinkPlayer05.PlayerId);
                writerShot.Write(2);
                writerShot.Write(BattleRoyale.pinkPlayer05mouseAngle);
                AmongUsClient.Instance.FinishRpcImmediately(writerShot);
                RPCProcedure.battleRoyaleShowShoots(BattleRoyale.pinkPlayer05.PlayerId, 2, BattleRoyale.pinkPlayer05mouseAngle);

                if (target == null)
                {
                    pinkPlayer05KillButton.Timer = pinkPlayer05KillButton.MaxTimer;
                    return;
                }

                if (BattleRoyale.matchType == 2)
                {
                    if (BattleRoyale.limePlayer01 != null && target == BattleRoyale.limePlayer01 && BattleRoyale.limePlayer01IsReviving ||
                    BattleRoyale.limePlayer02 != null && target == BattleRoyale.limePlayer02 && BattleRoyale.limePlayer02IsReviving ||
                    BattleRoyale.limePlayer03 != null && target == BattleRoyale.limePlayer03 && BattleRoyale.limePlayer03IsReviving ||
                    BattleRoyale.limePlayer04 != null && target == BattleRoyale.limePlayer04 && BattleRoyale.limePlayer04IsReviving ||
                    BattleRoyale.limePlayer05 != null && target == BattleRoyale.limePlayer05 && BattleRoyale.limePlayer05IsReviving ||
                    BattleRoyale.limePlayer06 != null && target == BattleRoyale.limePlayer06 && BattleRoyale.limePlayer06IsReviving ||
                    BattleRoyale.limePlayer07 != null && target == BattleRoyale.limePlayer07 && BattleRoyale.limePlayer07IsReviving ||
                    BattleRoyale.serialKiller != null && target == BattleRoyale.serialKiller && BattleRoyale.serialKillerIsReviving)
                    {
                        pinkPlayer05KillButton.Timer = pinkPlayer05KillButton.MaxTimer;
                        return;
                    }
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(BattleRoyale.pinkPlayer05.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.gamemodeKills(target.PlayerId, BattleRoyale.pinkPlayer05.PlayerId);

                pinkPlayer05KillButton.Timer = pinkPlayer05KillButton.MaxTimer;
                SoundManager.Instance.PlaySound(AssetLoader.royaleHitPlayer, false, 100f);

                target = null;
            },
            () => { return BattleRoyale.pinkPlayer05 != null && BattleRoyale.pinkPlayer05 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (BattleRoyale.pinkPlayer05Wep == null)
                {
                    BattleRoyale.pinkPlayer05Wep = new GameObject("Weapon");
                    var renderer = BattleRoyale.pinkPlayer05Wep.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.Bow;
                    renderer.transform.parent = BattleRoyale.pinkPlayer05.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {

                    if (PlayerControl.LocalPlayer.Data.IsDead && BattleRoyale.matchType != 2)
                    {
                        BattleRoyale.pinkPlayer05Wep.SetActive(false);
                    }

                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    BattleRoyale.pinkPlayer05mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = BattleRoyale.pinkPlayer05.transform.position + new Vector3(0.8f * (float)Math.Cos(BattleRoyale.pinkPlayer05mouseAngle), 0.8f * (float)Math.Sin(BattleRoyale.pinkPlayer05mouseAngle));
                    BattleRoyale.pinkPlayer05Wep.transform.position += (targetPosition - BattleRoyale.pinkPlayer05Wep.transform.position) * 0.4f;
                    BattleRoyale.pinkPlayer05Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(BattleRoyale.pinkPlayer05mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(BattleRoyale.pinkPlayer05mouseAngle) < 0.0)
                    {
                        if (BattleRoyale.pinkPlayer05Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            BattleRoyale.pinkPlayer05Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (BattleRoyale.pinkPlayer05Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            BattleRoyale.pinkPlayer05Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                bool CanUse = true;
                if (BattleRoyale.pinkPlayer05IsReviving)
                {
                    CanUse = false;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { pinkPlayer05KillButton.Timer = pinkPlayer05KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            KeyCode.Mouse1,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // PinkPlayer06 Kill
        pinkPlayer06KillButton = new CustomButton(
            () =>
            {
                PlayerControl target = BattleRoyale.GetPinkShotPlayer(2 * 0.2f, 6, 6);

                MessageWriter writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, Hazel.SendOption.Reliable, -1);
                writerShot.Write(BattleRoyale.pinkPlayer06.PlayerId);
                writerShot.Write(2);
                writerShot.Write(BattleRoyale.pinkPlayer06mouseAngle);
                AmongUsClient.Instance.FinishRpcImmediately(writerShot);
                RPCProcedure.battleRoyaleShowShoots(BattleRoyale.pinkPlayer06.PlayerId, 2, BattleRoyale.pinkPlayer06mouseAngle);

                if (target == null)
                {
                    pinkPlayer06KillButton.Timer = pinkPlayer06KillButton.MaxTimer;
                    return;
                }

                if (BattleRoyale.matchType == 2)
                {
                    if (BattleRoyale.limePlayer01 != null && target == BattleRoyale.limePlayer01 && BattleRoyale.limePlayer01IsReviving ||
                    BattleRoyale.limePlayer02 != null && target == BattleRoyale.limePlayer02 && BattleRoyale.limePlayer02IsReviving ||
                    BattleRoyale.limePlayer03 != null && target == BattleRoyale.limePlayer03 && BattleRoyale.limePlayer03IsReviving ||
                    BattleRoyale.limePlayer04 != null && target == BattleRoyale.limePlayer04 && BattleRoyale.limePlayer04IsReviving ||
                    BattleRoyale.limePlayer05 != null && target == BattleRoyale.limePlayer05 && BattleRoyale.limePlayer05IsReviving ||
                    BattleRoyale.limePlayer06 != null && target == BattleRoyale.limePlayer06 && BattleRoyale.limePlayer06IsReviving ||
                    BattleRoyale.limePlayer07 != null && target == BattleRoyale.limePlayer07 && BattleRoyale.limePlayer07IsReviving ||
                    BattleRoyale.serialKiller != null && target == BattleRoyale.serialKiller && BattleRoyale.serialKillerIsReviving)
                    {
                        pinkPlayer06KillButton.Timer = pinkPlayer06KillButton.MaxTimer;
                        return;
                    }
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(BattleRoyale.pinkPlayer06.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.gamemodeKills(target.PlayerId, BattleRoyale.pinkPlayer06.PlayerId);

                pinkPlayer06KillButton.Timer = pinkPlayer06KillButton.MaxTimer;
                SoundManager.Instance.PlaySound(AssetLoader.royaleHitPlayer, false, 100f);

                target = null;
            },
            () => { return BattleRoyale.pinkPlayer06 != null && BattleRoyale.pinkPlayer06 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (BattleRoyale.pinkPlayer06Wep == null)
                {
                    BattleRoyale.pinkPlayer06Wep = new GameObject("Weapon");
                    var renderer = BattleRoyale.pinkPlayer06Wep.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.Bow;
                    renderer.transform.parent = BattleRoyale.pinkPlayer06.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {

                    if (PlayerControl.LocalPlayer.Data.IsDead && BattleRoyale.matchType != 2)
                    {
                        BattleRoyale.pinkPlayer06Wep.SetActive(false);
                    }

                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    BattleRoyale.pinkPlayer06mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = BattleRoyale.pinkPlayer06.transform.position + new Vector3(0.8f * (float)Math.Cos(BattleRoyale.pinkPlayer06mouseAngle), 0.8f * (float)Math.Sin(BattleRoyale.pinkPlayer06mouseAngle));
                    BattleRoyale.pinkPlayer06Wep.transform.position += (targetPosition - BattleRoyale.pinkPlayer06Wep.transform.position) * 0.4f;
                    BattleRoyale.pinkPlayer06Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(BattleRoyale.pinkPlayer06mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(BattleRoyale.pinkPlayer06mouseAngle) < 0.0)
                    {
                        if (BattleRoyale.pinkPlayer06Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            BattleRoyale.pinkPlayer06Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (BattleRoyale.pinkPlayer06Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            BattleRoyale.pinkPlayer06Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                bool CanUse = true;
                if (BattleRoyale.pinkPlayer06IsReviving)
                {
                    CanUse = false;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { pinkPlayer06KillButton.Timer = pinkPlayer06KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            KeyCode.Mouse1,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // PinkPlayer07 Kill
        pinkPlayer07KillButton = new CustomButton(
            () =>
            {
                PlayerControl target = BattleRoyale.GetPinkShotPlayer(2 * 0.2f, 6, 7);

                MessageWriter writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, Hazel.SendOption.Reliable, -1);
                writerShot.Write(BattleRoyale.pinkPlayer07.PlayerId);
                writerShot.Write(2);
                writerShot.Write(BattleRoyale.pinkPlayer07mouseAngle);
                AmongUsClient.Instance.FinishRpcImmediately(writerShot);
                RPCProcedure.battleRoyaleShowShoots(BattleRoyale.pinkPlayer07.PlayerId, 2, BattleRoyale.pinkPlayer07mouseAngle);

                if (target == null)
                {
                    pinkPlayer07KillButton.Timer = pinkPlayer07KillButton.MaxTimer;
                    return;
                }

                if (BattleRoyale.matchType == 2)
                {
                    if (BattleRoyale.limePlayer01 != null && target == BattleRoyale.limePlayer01 && BattleRoyale.limePlayer01IsReviving ||
                    BattleRoyale.limePlayer02 != null && target == BattleRoyale.limePlayer02 && BattleRoyale.limePlayer02IsReviving ||
                    BattleRoyale.limePlayer03 != null && target == BattleRoyale.limePlayer03 && BattleRoyale.limePlayer03IsReviving ||
                    BattleRoyale.limePlayer04 != null && target == BattleRoyale.limePlayer04 && BattleRoyale.limePlayer04IsReviving ||
                    BattleRoyale.limePlayer05 != null && target == BattleRoyale.limePlayer05 && BattleRoyale.limePlayer05IsReviving ||
                    BattleRoyale.limePlayer06 != null && target == BattleRoyale.limePlayer06 && BattleRoyale.limePlayer06IsReviving ||
                    BattleRoyale.limePlayer07 != null && target == BattleRoyale.limePlayer07 && BattleRoyale.limePlayer07IsReviving ||
                    BattleRoyale.serialKiller != null && target == BattleRoyale.serialKiller && BattleRoyale.serialKillerIsReviving)
                    {
                        pinkPlayer07KillButton.Timer = pinkPlayer07KillButton.MaxTimer;
                        return;
                    }
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(BattleRoyale.pinkPlayer07.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.gamemodeKills(target.PlayerId, BattleRoyale.pinkPlayer07.PlayerId);

                pinkPlayer07KillButton.Timer = pinkPlayer07KillButton.MaxTimer;
                SoundManager.Instance.PlaySound(AssetLoader.royaleHitPlayer, false, 100f);

                target = null;
            },
            () => { return BattleRoyale.pinkPlayer07 != null && BattleRoyale.pinkPlayer07 == PlayerControl.LocalPlayer; },
            () =>
            {
                if (BattleRoyale.pinkPlayer07Wep == null)
                {
                    BattleRoyale.pinkPlayer07Wep = new GameObject("Weapon");
                    var renderer = BattleRoyale.pinkPlayer07Wep.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.Bow;
                    renderer.transform.parent = BattleRoyale.pinkPlayer07.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {

                    if (PlayerControl.LocalPlayer.Data.IsDead && BattleRoyale.matchType != 2)
                    {
                        BattleRoyale.pinkPlayer07Wep.SetActive(false);
                    }

                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    BattleRoyale.pinkPlayer07mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = BattleRoyale.pinkPlayer07.transform.position + new Vector3(0.8f * (float)Math.Cos(BattleRoyale.pinkPlayer07mouseAngle), 0.8f * (float)Math.Sin(BattleRoyale.pinkPlayer07mouseAngle));
                    BattleRoyale.pinkPlayer07Wep.transform.position += (targetPosition - BattleRoyale.pinkPlayer07Wep.transform.position) * 0.4f;
                    BattleRoyale.pinkPlayer07Wep.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(BattleRoyale.pinkPlayer07mouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(BattleRoyale.pinkPlayer07mouseAngle) < 0.0)
                    {
                        if (BattleRoyale.pinkPlayer07Wep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            BattleRoyale.pinkPlayer07Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (BattleRoyale.pinkPlayer07Wep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            BattleRoyale.pinkPlayer07Wep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                bool CanUse = true;
                if (BattleRoyale.pinkPlayer07IsReviving)
                {
                    CanUse = false;
                }
                return CanUse && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { pinkPlayer07KillButton.Timer = pinkPlayer07KillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            KeyCode.Mouse1,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );

        // Serial Killer Kill
        serialKillerKillButton = new CustomButton(
            () =>
            {
                PlayerControl target = BattleRoyale.GetSerialShootPlayer(2 * 0.2f, 6);

                MessageWriter writerShot = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BattleRoyaleShowShoots, Hazel.SendOption.Reliable, -1);
                writerShot.Write(BattleRoyale.serialKiller.PlayerId);
                writerShot.Write(3);
                writerShot.Write(BattleRoyale.serialKillermouseAngle);
                AmongUsClient.Instance.FinishRpcImmediately(writerShot);
                RPCProcedure.battleRoyaleShowShoots(BattleRoyale.serialKiller.PlayerId, 3, BattleRoyale.serialKillermouseAngle);

                if (target == null)
                {
                    serialKillerKillButton.Timer = serialKillerKillButton.MaxTimer;
                    return;
                }

                if (BattleRoyale.matchType == 2)
                {
                    if (BattleRoyale.limePlayer01 != null && target == BattleRoyale.limePlayer01 && BattleRoyale.limePlayer01IsReviving ||
                    BattleRoyale.limePlayer02 != null && target == BattleRoyale.limePlayer02 && BattleRoyale.limePlayer02IsReviving ||
                    BattleRoyale.limePlayer03 != null && target == BattleRoyale.limePlayer03 && BattleRoyale.limePlayer03IsReviving ||
                    BattleRoyale.limePlayer04 != null && target == BattleRoyale.limePlayer04 && BattleRoyale.limePlayer04IsReviving ||
                    BattleRoyale.limePlayer05 != null && target == BattleRoyale.limePlayer05 && BattleRoyale.limePlayer05IsReviving ||
                    BattleRoyale.limePlayer06 != null && target == BattleRoyale.limePlayer06 && BattleRoyale.limePlayer06IsReviving ||
                    BattleRoyale.limePlayer07 != null && target == BattleRoyale.limePlayer07 && BattleRoyale.limePlayer07IsReviving ||
                    BattleRoyale.pinkPlayer01 != null && target == BattleRoyale.pinkPlayer01 && BattleRoyale.pinkPlayer01IsReviving ||
                    BattleRoyale.pinkPlayer02 != null && target == BattleRoyale.pinkPlayer02 && BattleRoyale.pinkPlayer02IsReviving ||
                    BattleRoyale.pinkPlayer03 != null && target == BattleRoyale.pinkPlayer03 && BattleRoyale.pinkPlayer03IsReviving ||
                    BattleRoyale.pinkPlayer04 != null && target == BattleRoyale.pinkPlayer04 && BattleRoyale.pinkPlayer04IsReviving ||
                    BattleRoyale.pinkPlayer05 != null && target == BattleRoyale.pinkPlayer05 && BattleRoyale.pinkPlayer05IsReviving ||
                    BattleRoyale.pinkPlayer06 != null && target == BattleRoyale.pinkPlayer06 && BattleRoyale.pinkPlayer06IsReviving ||
                    BattleRoyale.pinkPlayer07 != null && target == BattleRoyale.pinkPlayer07 && BattleRoyale.pinkPlayer07IsReviving)
                    {
                        serialKillerKillButton.Timer = serialKillerKillButton.MaxTimer;
                        return;
                    }
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GamemodeKills, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                writer.Write(BattleRoyale.serialKiller.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.gamemodeKills(target.PlayerId, BattleRoyale.serialKiller.PlayerId);

                serialKillerKillButton.Timer = serialKillerKillButton.MaxTimer;
                SoundManager.Instance.PlaySound(AssetLoader.royaleHitPlayer, false, 100f);

                target = null;
            },
            () => { return BattleRoyale.serialKiller != null && BattleRoyale.serialKiller == PlayerControl.LocalPlayer; },
            () =>
            {
                if (BattleRoyale.serialKillerWep == null)
                {
                    BattleRoyale.serialKillerWep = new GameObject("Weapon");
                    var renderer = BattleRoyale.serialKillerWep.AddComponent<SpriteRenderer>();

                    renderer.sprite = AssetLoader.Bow;
                    renderer.transform.parent = BattleRoyale.serialKiller.transform;
                    renderer.color = new Color(1, 1, 1, 1);
                    renderer.transform.position = new Vector3(0, 0, -30f);
                }
                else
                {

                    if (PlayerControl.LocalPlayer.Data.IsDead && BattleRoyale.matchType != 2)
                    {
                        BattleRoyale.serialKillerWep.SetActive(false);
                    }

                    Vector3 mouseDirection = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
                    BattleRoyale.serialKillermouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x);

                    var targetPosition = BattleRoyale.serialKiller.transform.position + new Vector3(0.8f * (float)Math.Cos(BattleRoyale.serialKillermouseAngle), 0.8f * (float)Math.Sin(BattleRoyale.serialKillermouseAngle));
                    BattleRoyale.serialKillerWep.transform.position += (targetPosition - BattleRoyale.serialKillerWep.transform.position) * 0.4f;
                    BattleRoyale.serialKillerWep.GetComponent<SpriteRenderer>().transform.eulerAngles = new Vector3(0f, 0f, (float)(BattleRoyale.serialKillermouseAngle * 360f / Math.PI / 2f));
                    if (Math.Cos(BattleRoyale.serialKillermouseAngle) < 0.0)
                    {
                        if (BattleRoyale.serialKillerWep.GetComponent<SpriteRenderer>().transform.localScale.y > 0)
                            BattleRoyale.serialKillerWep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, -1f);
                    }
                    else
                    {
                        if (BattleRoyale.serialKillerWep.GetComponent<SpriteRenderer>().transform.localScale.y < 0)
                            BattleRoyale.serialKillerWep.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(1f, 1f);
                    }
                }
                bool CanUse = true;
                if (BattleRoyale.serialKillerIsReviving)
                {
                    CanUse = false;
                }
                bool canSpawnKill = true;
                foreach (GameObject spawns in BattleRoyale.serialKillerSpawns)
                {
                    if (spawns != null && BattleRoyale.matchType == 2 && Vector2.Distance(PlayerControl.LocalPlayer.transform.position, spawns.transform.position) < 3f)
                    {
                        canSpawnKill = false;
                    }
                }
                return CanUse && canSpawnKill && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.Data.IsDead;
            },
            () => { serialKillerKillButton.Timer = serialKillerKillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            __instance,
            __instance.KillButton,
            KeyCode.Mouse1,
            false,
            TranslationController.Instance.GetString(StringNames.KillLabel)
        );
    }
}