namespace RebuildUs.Modules.GameMode;

public static partial class PoliceAndThief
{
    public static Color PolicePlayerColor = new Color32(102, 102, 153, byte.MaxValue);
    public static Color ThiefPlayerColor = new Color32(127, 76, 0, byte.MaxValue);
    public static Color IntroColor = new Color32(0, 247, 255, byte.MaxValue);

    public static bool createdpoliceandthief = false;

    public static List<PlayerControl> thiefTeam = [];
    public static PlayerControl thiefplayer01 = null;
    public static PlayerControl thiefplayer01currentTarget = null;
    public static bool thiefplayer01IsStealing = false;
    public static byte thiefplayer01JewelId = 0;
    public static bool thiefplayer01IsReviving = false;
    public static PlayerControl thiefplayer02 = null;
    public static PlayerControl thiefplayer02currentTarget = null;
    public static bool thiefplayer02IsStealing = false;
    public static byte thiefplayer02JewelId = 0;
    public static bool thiefplayer02IsReviving = false;
    public static PlayerControl thiefplayer03 = null;
    public static PlayerControl thiefplayer03currentTarget = null;
    public static bool thiefplayer03IsStealing = false;
    public static byte thiefplayer03JewelId = 0;
    public static bool thiefplayer03IsReviving = false;
    public static PlayerControl thiefplayer04 = null;
    public static PlayerControl thiefplayer04currentTarget = null;
    public static bool thiefplayer04IsStealing = false;
    public static byte thiefplayer04JewelId = 0;
    public static bool thiefplayer04IsReviving = false;
    public static PlayerControl thiefplayer05 = null;
    public static PlayerControl thiefplayer05currentTarget = null;
    public static bool thiefplayer05IsStealing = false;
    public static byte thiefplayer05JewelId = 0;
    public static bool thiefplayer05IsReviving = false;
    public static PlayerControl thiefplayer06 = null;
    public static PlayerControl thiefplayer06currentTarget = null;
    public static bool thiefplayer06IsStealing = false;
    public static byte thiefplayer06JewelId = 0;
    public static bool thiefplayer06IsReviving = false;
    public static PlayerControl thiefplayer07 = null;
    public static PlayerControl thiefplayer07currentTarget = null;
    public static bool thiefplayer07IsStealing = false;
    public static byte thiefplayer07JewelId = 0;
    public static bool thiefplayer07IsReviving = false;
    public static PlayerControl thiefplayer08 = null;
    public static PlayerControl thiefplayer08currentTarget = null;
    public static bool thiefplayer08IsStealing = false;
    public static byte thiefplayer08JewelId = 0;
    public static bool thiefplayer08IsReviving = false;
    public static PlayerControl thiefplayer09 = null;
    public static PlayerControl thiefplayer09currentTarget = null;
    public static bool thiefplayer09IsStealing = false;
    public static byte thiefplayer09JewelId = 0;
    public static bool thiefplayer09IsReviving = false;

    public static List<PlayerControl> policeTeam = [];
    public static PlayerControl policeplayer01 = null;
    public static PlayerControl policeplayer01currentTarget = null;
    public static PlayerControl policeplayer01targetedPlayer = null;
    public static float policeplayer01lightTimer = 0;
    public static bool policeplayer01IsReviving = false;
    public static PlayerControl policeplayer02 = null;
    public static PlayerControl policeplayer02currentTarget = null;
    public static float policeplayer02lightTimer = 0;
    public static bool policeplayer02IsReviving = false;
    public static GameObject policeplayer02Taser = null;
    public static float policeplayer02mouseAngle = 0f;
    public static PlayerControl policeplayer03 = null;
    public static PlayerControl policeplayer03currentTarget = null;
    public static PlayerControl policeplayer03targetedPlayer = null;
    public static float policeplayer03lightTimer = 0;
    public static bool policeplayer03IsReviving = false;
    public static PlayerControl policeplayer04 = null;
    public static PlayerControl policeplayer04currentTarget = null;
    public static float policeplayer04lightTimer = 0;
    public static bool policeplayer04IsReviving = false;
    public static GameObject policeplayer04Taser = null;
    public static float policeplayer04mouseAngle = 0f;
    public static PlayerControl policeplayer05 = null;
    public static PlayerControl policeplayer05currentTarget = null;
    public static PlayerControl policeplayer05targetedPlayer = null;
    public static float policeplayer05lightTimer = 0;
    public static bool policeplayer05IsReviving = false;
    public static PlayerControl policeplayer06 = null;
    public static PlayerControl policeplayer06currentTarget = null;
    public static PlayerControl policeplayer06targetedPlayer = null;
    public static float policeplayer06lightTimer = 0;
    public static bool policeplayer06IsReviving = false;

    public static List<PlayerControl> thiefArrested = [];
    public static List<GameObject> thiefTreasures = [];
    public static GameObject cell = null;
    public static GameObject cellbutton = null;
    public static GameObject jewelbutton = null;

    public static GameObject celltwo = null;
    public static GameObject cellbuttontwo = null;
    public static GameObject jewelbuttontwo = null;

    public static GameObject jewel01 = null;
    public static PlayerControl jewel01BeingStealed = null;
    public static GameObject jewel02 = null;
    public static PlayerControl jewel02BeingStealed = null;
    public static GameObject jewel03 = null;
    public static PlayerControl jewel03BeingStealed = null;
    public static GameObject jewel04 = null;
    public static PlayerControl jewel04BeingStealed = null;
    public static GameObject jewel05 = null;
    public static PlayerControl jewel05BeingStealed = null;
    public static GameObject jewel06 = null;
    public static PlayerControl jewel06BeingStealed = null;
    public static GameObject jewel07 = null;
    public static PlayerControl jewel07BeingStealed = null;
    public static GameObject jewel08 = null;
    public static PlayerControl jewel08BeingStealed = null;
    public static GameObject jewel09 = null;
    public static PlayerControl jewel09BeingStealed = null;
    public static GameObject jewel10 = null;
    public static PlayerControl jewel10BeingStealed = null;
    public static GameObject jewel11 = null;
    public static PlayerControl jewel11BeingStealed = null;
    public static GameObject jewel12 = null;
    public static PlayerControl jewel12BeingStealed = null;
    public static GameObject jewel13 = null;
    public static PlayerControl jewel13BeingStealed = null;
    public static GameObject jewel14 = null;
    public static PlayerControl jewel14BeingStealed = null;
    public static GameObject jewel15 = null;
    public static PlayerControl jewel15BeingStealed = null;

    public static float requiredJewels = 10;
    public static float policeTaseCooldown = 20f;
    public static float policeTaseDuration = 5f;
    public static bool policeCanSeeJewels = false;
    public static float policeCatchCooldown = 10f;
    public static float captureThiefTime = 3f;
    public static int whoCanThiefsKill = 0;

    public static float currentJewelsStoled = 0;
    public static float currentThiefsCaptured = 0;

    public static bool triggerThiefWin = false;
    public static bool triggerPoliceWin = false;

    public static string thiefpointCounter = new StringBuilder(Tr.Get(TrKey.StolenJewels)).Append("<color=#FF0000FF>").Append(currentJewelsStoled).Append(" / ").Append(requiredJewels).Append("</color> | ").Append(Tr.Get(TrKey.CapturedThieves)).Append("<color=#0000FFFF>").Append(currentThiefsCaptured).Append(" / 10</color>").ToString();

    public static List<Arrow> localThiefReleaseArrow = [];
    public static List<Arrow> localThiefDeliverArrow = [];

    public static void clearAndReload()
    {
        createdpoliceandthief = false;

        cell = null;
        cellbutton = null;
        jewelbutton = null;
        thiefTreasures.Clear();
        thiefArrested.Clear();

        celltwo = null;
        cellbuttontwo = null;
        jewelbuttontwo = null;

        thiefTeam.Clear();
        thiefplayer01 = null;
        thiefplayer01currentTarget = null;
        thiefplayer01IsStealing = false;
        thiefplayer01JewelId = 0;
        thiefplayer01IsReviving = false;
        thiefplayer02 = null;
        thiefplayer02currentTarget = null;
        thiefplayer02IsStealing = false;
        thiefplayer02JewelId = 0;
        thiefplayer02IsReviving = false;
        thiefplayer03 = null;
        thiefplayer03currentTarget = null;
        thiefplayer03IsStealing = false;
        thiefplayer03JewelId = 0;
        thiefplayer03IsReviving = false;
        thiefplayer04 = null;
        thiefplayer04currentTarget = null;
        thiefplayer04IsStealing = false;
        thiefplayer04JewelId = 0;
        thiefplayer04IsReviving = false;
        thiefplayer05 = null;
        thiefplayer05currentTarget = null;
        thiefplayer05IsStealing = false;
        thiefplayer05JewelId = 0;
        thiefplayer05IsReviving = false;
        thiefplayer06 = null;
        thiefplayer06currentTarget = null;
        thiefplayer06IsStealing = false;
        thiefplayer06JewelId = 0;
        thiefplayer06IsReviving = false;
        thiefplayer07 = null;
        thiefplayer07currentTarget = null;
        thiefplayer07IsStealing = false;
        thiefplayer07JewelId = 0;
        thiefplayer07IsReviving = false;
        thiefplayer08 = null;
        thiefplayer08currentTarget = null;
        thiefplayer08IsStealing = false;
        thiefplayer08JewelId = 0;
        thiefplayer08IsReviving = false;
        thiefplayer09 = null;
        thiefplayer09currentTarget = null;
        thiefplayer09IsStealing = false;
        thiefplayer09JewelId = 0;
        thiefplayer09IsReviving = false;

        policeTeam.Clear();
        policeplayer01 = null;
        policeplayer01currentTarget = null;
        policeplayer01targetedPlayer = null;
        policeplayer01lightTimer = 0;
        policeplayer01IsReviving = false;
        policeplayer02 = null;
        policeplayer02currentTarget = null;
        policeplayer02lightTimer = 0;
        policeplayer02IsReviving = false;
        policeplayer02Taser = null;
        policeplayer02mouseAngle = 0f;
        policeplayer03 = null;
        policeplayer03currentTarget = null;
        policeplayer03targetedPlayer = null;
        policeplayer03lightTimer = 0;
        policeplayer03IsReviving = false;
        policeplayer04 = null;
        policeplayer04currentTarget = null;
        policeplayer04lightTimer = 0;
        policeplayer04IsReviving = false;
        policeplayer04Taser = null;
        policeplayer04mouseAngle = 0f;
        policeplayer05 = null;
        policeplayer05currentTarget = null;
        policeplayer05targetedPlayer = null;
        policeplayer05lightTimer = 0;
        policeplayer05IsReviving = false;
        policeplayer06 = null;
        policeplayer06currentTarget = null;
        policeplayer06targetedPlayer = null;
        policeplayer06lightTimer = 0;
        policeplayer06IsReviving = false;

        jewel01 = null;
        jewel01BeingStealed = null;
        jewel02 = null;
        jewel02BeingStealed = null;
        jewel03 = null;
        jewel03BeingStealed = null;
        jewel04 = null;
        jewel04BeingStealed = null;
        jewel05 = null;
        jewel05BeingStealed = null;
        jewel06 = null;
        jewel06BeingStealed = null;
        jewel07 = null;
        jewel07BeingStealed = null;
        jewel08 = null;
        jewel08BeingStealed = null;
        jewel09 = null;
        jewel09BeingStealed = null;
        jewel10 = null;
        jewel10BeingStealed = null;
        jewel11 = null;
        jewel11BeingStealed = null;
        jewel12 = null;
        jewel12BeingStealed = null;
        jewel13 = null;
        jewel13BeingStealed = null;
        jewel14 = null;
        jewel14BeingStealed = null;
        jewel15 = null;
        jewel15BeingStealed = null;

        localThiefReleaseArrow = [];
        localThiefDeliverArrow = [];

        requiredJewels = CustomOptionHolder.thiefModerequiredJewels.GetFloat();
        policeTaseCooldown = CustomOptionHolder.thiefModePoliceTaseCooldown.GetFloat();
        policeTaseDuration = CustomOptionHolder.thiefModePoliceTaseDuration.GetFloat();
        policeCanSeeJewels = CustomOptionHolder.thiefModePoliceCanSeeJewels.GetBool();
        policeCatchCooldown = CustomOptionHolder.thiefModePoliceCatchCooldown.GetFloat();
        captureThiefTime = CustomOptionHolder.thiefModecaptureThiefTime.GetFloat();
        whoCanThiefsKill = CustomOptionHolder.thiefModeWhoCanThiefsKill.GetSelection();
        currentJewelsStoled = 0;
        triggerThiefWin = false;
        triggerPoliceWin = false;
        currentThiefsCaptured = 0;
        thiefpointCounter = new StringBuilder(Tr.Get(TrKey.StolenJewels)).Append("<color=#00F7FFFF>").Append(currentJewelsStoled).Append(" / ").Append(requiredJewels).Append("</color> | ").Append(Tr.Get(TrKey.CapturedThieves)).Append("<color=#928B55FF>").Append(currentThiefsCaptured).Append(" / ").Append(thiefTeam.Count).Append("</color>").ToString();
    }
    public static PlayerControl GetTasedPlayer(float shotSize, float effectiveRange, bool policeTwo)
    {
        PlayerControl result = null;
        float num = effectiveRange;
        Vector3 pos;
        float mouseAngle;
        if (policeTwo)
        {
            mouseAngle = policeplayer02mouseAngle;
        }
        else
        {
            mouseAngle = policeplayer04mouseAngle;
        }
        foreach (PlayerControl player in thiefTeam)
        {
            if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId) continue;

            if (player.Data.IsDead || player.inVent) continue;

            pos = player.transform.position - PlayerControl.LocalPlayer.transform.position;
            pos = new Vector3(
                pos.x * MathF.Cos(mouseAngle) + pos.y * MathF.Sin(mouseAngle),
                pos.y * MathF.Cos(mouseAngle) - pos.x * MathF.Sin(mouseAngle));
            if (Math.Abs(pos.y) < shotSize && (!(pos.x < 0)) && pos.x < num)
            {
                num = pos.x;
                result = player;
            }
        }
        return result;
    }

    public static void CreatePAT()
    {

        Vector3 policeTeamPos = new Vector3();
        Vector3 thiefTeamPos = new Vector3();
        Vector3 cellPos = new Vector3();
        Vector3 cellButtonPos = new Vector3();
        Vector3 jewelButtonPos = new Vector3();
        Vector3 thiefSpaceShipPos = new Vector3();
        Vector3 jewel01Pos = new Vector3();
        Vector3 jewel02Pos = new Vector3();
        Vector3 jewel03Pos = new Vector3();
        Vector3 jewel04Pos = new Vector3();
        Vector3 jewel05Pos = new Vector3();
        Vector3 jewel06Pos = new Vector3();
        Vector3 jewel07Pos = new Vector3();
        Vector3 jewel08Pos = new Vector3();
        Vector3 jewel09Pos = new Vector3();
        Vector3 jewel10Pos = new Vector3();
        Vector3 jewel11Pos = new Vector3();
        Vector3 jewel12Pos = new Vector3();
        Vector3 jewel13Pos = new Vector3();
        Vector3 jewel14Pos = new Vector3();
        Vector3 jewel15Pos = new Vector3();

        switch (GameOptionsManager.Instance.currentGameOptions.MapId)
        {
            // Skeld / Custom Skeld
            case 0:
                if (RebuildUs.activatedSensei)
                {
                    policeTeamPos = new Vector3(-12f, 5f, PlayerControl.LocalPlayer.transform.position.z);
                    thiefTeamPos = new Vector3(13.75f, -0.2f, PlayerControl.LocalPlayer.transform.position.z);
                    cellPos = new Vector3(-12f, 7.2f, 0.5f);
                    cellButtonPos = new Vector3(-12f, 4.7f, 0.5f);
                    jewelButtonPos = new Vector3(13.75f, -0.42f, 0.5f);
                    thiefSpaceShipPos = new Vector3(17f, 0f, 0.6f);
                    jewel01Pos = new Vector3(6.95f, 4.95f, 1f);
                    jewel02Pos = new Vector3(-3.75f, 5.35f, 1f);
                    jewel03Pos = new Vector3(-7.7f, 11.3f, 1f);
                    jewel04Pos = new Vector3(-19.65f, 5.3f, 1f);
                    jewel05Pos = new Vector3(-19.65f, -8, 1f);
                    jewel06Pos = new Vector3(-5.45f, -13f, 1f);
                    jewel07Pos = new Vector3(-7.65f, -4.2f, 1f);
                    jewel08Pos = new Vector3(2f, -6.75f, 1f);
                    jewel09Pos = new Vector3(8.9f, 1.45f, 1f);
                    jewel10Pos = new Vector3(4.6f, -2.25f, 1f);
                    jewel11Pos = new Vector3(-5.05f, -0.88f, 1f);
                    jewel12Pos = new Vector3(-8.25f, -0.45f, 1f);
                    jewel13Pos = new Vector3(-19.75f, -1.55f, 1f);
                    jewel14Pos = new Vector3(-12.1f, -13.15f, 1f);
                    jewel15Pos = new Vector3(7.15f, -14.45f, 1f);
                }
                else if (RebuildUs.activatedDleks)
                {
                    policeTeamPos = new Vector3(10.2f, 1.18f, PlayerControl.LocalPlayer.transform.position.z);
                    thiefTeamPos = new Vector3(1.31f, -16.25f, PlayerControl.LocalPlayer.transform.position.z);
                    cellPos = new Vector3(10.25f, 3.38f, 0.5f);
                    cellButtonPos = new Vector3(10.2f, 0.93f, 0.5f);
                    jewelButtonPos = new Vector3(-0.20f, -17.15f, 0.5f);
                    thiefSpaceShipPos = new Vector3(1.345f, -19.16f, 0.6f);
                    GameObject thiefspaceshiphatchDleks = GameObject.Instantiate(AssetLoader.thiefspaceshiphatch, PlayerControl.LocalPlayer.transform.parent);
                    thiefspaceshiphatchDleks.name = "thiefspaceshiphatch";
                    thiefspaceshiphatchDleks.transform.position = new Vector3(1.345f, -19.16f, 0.6f);
                    jewel01Pos = new Vector3(18.65f, -9.9f, 1f);
                    jewel02Pos = new Vector3(21.5f, -2, 1f);
                    jewel03Pos = new Vector3(5.9f, -8.25f, 1f);
                    jewel04Pos = new Vector3(-4.5f, -7.5f, 1f);
                    jewel05Pos = new Vector3(-7.85f, -14.45f, 1f);
                    jewel06Pos = new Vector3(-6.65f, -4.8f, 1f);
                    jewel07Pos = new Vector3(-10.5f, 2.15f, 1f);
                    jewel08Pos = new Vector3(5.5f, 3.5f, 1f);
                    jewel09Pos = new Vector3(19, -1.2f, 1f);
                    jewel10Pos = new Vector3(21.5f, -8.35f, 1f);
                    jewel11Pos = new Vector3(12.5f, -3.75f, 1f);
                    jewel12Pos = new Vector3(5.9f, -5.25f, 1f);
                    jewel13Pos = new Vector3(-2.65f, -16.5f, 1f);
                    jewel14Pos = new Vector3(-16.75f, -4.75f, 1f);
                    jewel15Pos = new Vector3(-3.8f, 3.5f, 1f);
                }
                else
                {
                    policeTeamPos = new Vector3(-10.2f, 1.18f, PlayerControl.LocalPlayer.transform.position.z);
                    thiefTeamPos = new Vector3(-1.31f, -16.25f, PlayerControl.LocalPlayer.transform.position.z);
                    cellPos = new Vector3(-10.25f, 3.38f, 0.5f);
                    cellButtonPos = new Vector3(-10.2f, 0.93f, 0.5f);
                    jewelButtonPos = new Vector3(0.20f, -17.15f, 0.5f);
                    thiefSpaceShipPos = new Vector3(1.765f, -19.16f, 0.6f);
                    GameObject thiefspaceshiphatch = GameObject.Instantiate(AssetLoader.thiefspaceshiphatch, PlayerControl.LocalPlayer.transform.parent);
                    thiefspaceshiphatch.name = "thiefspaceshiphatch";
                    thiefspaceshiphatch.transform.position = new Vector3(1.765f, -19.16f, 0.6f);
                    jewel01Pos = new Vector3(-18.65f, -9.9f, 1f);
                    jewel02Pos = new Vector3(-21.5f, -2, 1f);
                    jewel03Pos = new Vector3(-5.9f, -8.25f, 1f);
                    jewel04Pos = new Vector3(4.5f, -7.5f, 1f);
                    jewel05Pos = new Vector3(7.85f, -14.45f, 1f);
                    jewel06Pos = new Vector3(6.65f, -4.8f, 1f);
                    jewel07Pos = new Vector3(10.5f, 2.15f, 1f);
                    jewel08Pos = new Vector3(-5.5f, 3.5f, 1f);
                    jewel09Pos = new Vector3(-19, -1.2f, 1f);
                    jewel10Pos = new Vector3(-21.5f, -8.35f, 1f);
                    jewel11Pos = new Vector3(-12.5f, -3.75f, 1f);
                    jewel12Pos = new Vector3(-5.9f, -5.25f, 1f);
                    jewel13Pos = new Vector3(2.65f, -16.5f, 1f);
                    jewel14Pos = new Vector3(16.75f, -4.75f, 1f);
                    jewel15Pos = new Vector3(3.8f, 3.5f, 1f);
                }
                break;
            // Mira HQ
            case 1:
                policeTeamPos = new Vector3(1.8f, -1f, PlayerControl.LocalPlayer.transform.position.z);
                thiefTeamPos = new Vector3(17.75f, 11.5f, PlayerControl.LocalPlayer.transform.position.z);
                cellPos = new Vector3(1.75f, 1.125f, 0.5f);
                cellButtonPos = new Vector3(1.8f, -1.25f, 0.5f);
                jewelButtonPos = new Vector3(18.5f, 13.85f, 0.5f);
                thiefSpaceShipPos = new Vector3(21.4f, 14.2f, 0.6f);
                jewel01Pos = new Vector3(-4.5f, 2.5f, 1f);
                jewel02Pos = new Vector3(6.25f, 14f, 1f);
                jewel03Pos = new Vector3(9.15f, 4.75f, 1f);
                jewel04Pos = new Vector3(14.75f, 20.5f, 1f);
                jewel05Pos = new Vector3(19.5f, 17.5f, 1f);
                jewel06Pos = new Vector3(21, 24.1f, 1f);
                jewel07Pos = new Vector3(19.5f, 4.75f, 1f);
                jewel08Pos = new Vector3(28.25f, 0, 1f);
                jewel09Pos = new Vector3(2.45f, 11.25f, 1f);
                jewel10Pos = new Vector3(4.4f, 1.75f, 1f);
                jewel11Pos = new Vector3(9.25f, 13f, 1f);
                jewel12Pos = new Vector3(13.75f, 23.5f, 1f);
                jewel13Pos = new Vector3(16, 4, 1f);
                jewel14Pos = new Vector3(15.35f, -0.9f, 1f);
                jewel15Pos = new Vector3(19.5f, -1.75f, 1f);
                break;
            // Polus
            case 2:
                policeTeamPos = new Vector3(8.18f, -7.4f, PlayerControl.LocalPlayer.transform.position.z);
                thiefTeamPos = new Vector3(30f, -15.75f, PlayerControl.LocalPlayer.transform.position.z);
                cellPos = new Vector3(8.25f, -5.15f, 0.5f);
                cellButtonPos = new Vector3(8.2f, -7.5f, 0.5f);
                jewelButtonPos = new Vector3(32.25f, -15.9f, 0.5f);
                thiefSpaceShipPos = new Vector3(35.35f, -15.55f, 0.8f);
                jewel01Pos = new Vector3(16.7f, -2.65f, 0.75f);
                jewel02Pos = new Vector3(25.35f, -7.35f, 0.75f);
                jewel03Pos = new Vector3(34.9f, -9.75f, 0.75f);
                jewel04Pos = new Vector3(36.5f, -21.75f, 0.75f);
                jewel05Pos = new Vector3(17.25f, -17.5f, 0.75f);
                jewel06Pos = new Vector3(10.9f, -20.5f, -0.75f);
                jewel07Pos = new Vector3(1.5f, -20.25f, 0.75f);
                jewel08Pos = new Vector3(3f, -12f, 0.75f);
                jewel09Pos = new Vector3(30f, -7.35f, 0.75f);
                jewel10Pos = new Vector3(40.25f, -8f, 0.75f);
                jewel11Pos = new Vector3(26f, -17.15f, 0.75f);
                jewel12Pos = new Vector3(22f, -25.25f, 0.75f);
                jewel13Pos = new Vector3(20.65f, -12f, 0.75f);
                jewel14Pos = new Vector3(9.75f, -12.25f, 0.75f);
                jewel15Pos = new Vector3(2.25f, -24f, 0.75f);
                break;
            // Dleks
            case 3:
                policeTeamPos = new Vector3(10.2f, 1.18f, PlayerControl.LocalPlayer.transform.position.z);
                thiefTeamPos = new Vector3(1.31f, -16.25f, PlayerControl.LocalPlayer.transform.position.z);
                cellPos = new Vector3(10.25f, 3.38f, 0.5f);
                cellButtonPos = new Vector3(10.2f, 0.93f, 0.5f);
                jewelButtonPos = new Vector3(-0.20f, -17.15f, 0.5f);
                thiefSpaceShipPos = new Vector3(1.345f, -19.16f, 0.6f);
                GameObject thiefspaceshiphatchdleks = GameObject.Instantiate(AssetLoader.thiefspaceshiphatch, PlayerControl.LocalPlayer.transform.parent);
                thiefspaceshiphatchdleks.name = "thiefspaceshiphatch";
                thiefspaceshiphatchdleks.transform.position = new Vector3(1.345f, -19.16f, 0.6f);
                jewel01Pos = new Vector3(18.65f, -9.9f, 1f);
                jewel02Pos = new Vector3(21.5f, -2, 1f);
                jewel03Pos = new Vector3(5.9f, -8.25f, 1f);
                jewel04Pos = new Vector3(-4.5f, -7.5f, 1f);
                jewel05Pos = new Vector3(-7.85f, -14.45f, 1f);
                jewel06Pos = new Vector3(-6.65f, -4.8f, 1f);
                jewel07Pos = new Vector3(-10.5f, 2.15f, 1f);
                jewel08Pos = new Vector3(5.5f, 3.5f, 1f);
                jewel09Pos = new Vector3(19, -1.2f, 1f);
                jewel10Pos = new Vector3(21.5f, -8.35f, 1f);
                jewel11Pos = new Vector3(12.5f, -3.75f, 1f);
                jewel12Pos = new Vector3(5.9f, -5.25f, 1f);
                jewel13Pos = new Vector3(-2.65f, -16.5f, 1f);
                jewel14Pos = new Vector3(-16.75f, -4.75f, 1f);
                jewel15Pos = new Vector3(-3.8f, 3.5f, 1f);
                break;
            // Airship
            case 4:
                policeTeamPos = new Vector3(-18.5f, 0.75f, PlayerControl.LocalPlayer.transform.position.z);
                thiefTeamPos = new Vector3(7.15f, -14.5f, PlayerControl.LocalPlayer.transform.position.z);
                cellPos = new Vector3(-18.45f, 3.55f, 0.5f);
                cellButtonPos = new Vector3(-18.5f, 0.5f, 0.5f);
                jewelButtonPos = new Vector3(10.275f, -16.3f, -0.01f);
                thiefSpaceShipPos = new Vector3(13.5f, -16f, 0.6f);
                jewel01Pos = new Vector3(-23.5f, -1.5f, 1f);
                jewel02Pos = new Vector3(-14.15f, -4.85f, 1f);
                jewel03Pos = new Vector3(-13.9f, -16.25f, 1f);
                jewel04Pos = new Vector3(-0.85f, -2.5f, 1f);
                jewel05Pos = new Vector3(-5, 8.5f, 1f);
                jewel06Pos = new Vector3(19.3f, -4.15f, 1f);
                jewel07Pos = new Vector3(19.85f, 8, 1f);
                jewel08Pos = new Vector3(28.85f, -1.75f, 1f);
                jewel09Pos = new Vector3(-14.5f, -8.5f, 1f);
                jewel10Pos = new Vector3(6.3f, -2.75f, 1f);
                jewel11Pos = new Vector3(20.75f, 2.5f, 1f);
                jewel12Pos = new Vector3(29.25f, 7, 1f);
                jewel13Pos = new Vector3(37.5f, -3.5f, 1f);
                jewel14Pos = new Vector3(25.2f, -8.75f, 1f);
                jewel15Pos = new Vector3(16.3f, -11, 1f);
                break;
            // Fungle
            case 5:
                policeTeamPos = new Vector3(-22.5f, -0.5f, PlayerControl.LocalPlayer.transform.position.z);
                thiefTeamPos = new Vector3(20f, 11f, PlayerControl.LocalPlayer.transform.position.z);
                cellPos = new Vector3(-26.75f, -0.65f, 0.5f);
                cellButtonPos = new Vector3(-24f, -0.5f, 0.5f);
                jewelButtonPos = new Vector3(18f, 11.75f, 0.05f);
                thiefSpaceShipPos = new Vector3(19f, 9.25f, 0.6f);
                jewel01Pos = new Vector3(-18.25f, 5f, 1f);
                jewel02Pos = new Vector3(-22.65f, -7.15f, 1f);
                jewel03Pos = new Vector3(2, 4.35f, 1f);
                jewel04Pos = new Vector3(-3.15f, -10.5f, 0.9f);
                jewel05Pos = new Vector3(23.7f, -7.8f, 1f);
                jewel06Pos = new Vector3(-4.75f, -1.75f, 1f);
                jewel07Pos = new Vector3(8f, -10f, 1f);
                jewel08Pos = new Vector3(7f, 1.75f, 1f);
                jewel09Pos = new Vector3(13.25f, 10, 1f);
                jewel10Pos = new Vector3(22.3f, 3.3f, 1f);
                jewel11Pos = new Vector3(20.5f, 7.35f, 1f);
                jewel12Pos = new Vector3(24.15f, 14.45f, 1f);
                jewel13Pos = new Vector3(-16.12f, 0.7f, 1f);
                jewel14Pos = new Vector3(1.65f, -1.5f, 1f);
                jewel15Pos = new Vector3(10.5f, -12, 1f);
                break;
            // Submerged
            case 6:
                policeTeamPos = new Vector3(-8.45f, 27f, PlayerControl.LocalPlayer.transform.position.z);
                thiefTeamPos = new Vector3(1f, 10f, PlayerControl.LocalPlayer.transform.position.z);
                cellPos = new Vector3(-5.9f, 31.85f, 0.5f);
                cellButtonPos = new Vector3(-6f, 28.5f, 0.03f);
                jewelButtonPos = new Vector3(1f, 10f, 0.03f);
                thiefSpaceShipPos = new Vector3(14.5f, -35f, -0.011f);
                jewel01Pos = new Vector3(-15f, 17.5f, -1f);
                jewel02Pos = new Vector3(8f, 32f, -1f);
                jewel03Pos = new Vector3(-6.75f, 10f, -1f);
                jewel04Pos = new Vector3(5.15f, 8f, -1f);
                jewel05Pos = new Vector3(5f, -33.5f, -1f);
                jewel06Pos = new Vector3(-4.15f, -33.5f, -1f);
                jewel07Pos = new Vector3(-14f, -27.75f, -1f);
                jewel08Pos = new Vector3(7.8f, -23.75f, -1f);
                jewel09Pos = new Vector3(-6.75f, -42.75f, -1f);
                jewel10Pos = new Vector3(13f, -25.25f, -1f);
                jewel11Pos = new Vector3(-14f, -34.25f, -1f);
                jewel12Pos = new Vector3(0f, -33.5f, -1f);
                jewel13Pos = new Vector3(-6.5f, 14f, -1f);
                jewel14Pos = new Vector3(14.25f, 24.5f, -1f);
                jewel15Pos = new Vector3(-12.25f, 31f, -1f);

                // Add another cell and deliver point on each floor
                GameObject celltwo = GameObject.Instantiate(AssetLoader.cell, PlayerControl.LocalPlayer.transform.parent);
                celltwo.name = "celltwo";
                celltwo.transform.position = new Vector3(-14.1f, -39f, -0.01f);
                celltwo.gameObject.layer = 9;
                celltwo.transform.GetChild(0).gameObject.layer = 9;
                PoliceAndThief.celltwo = celltwo;
                GameObject cellbuttontwo = GameObject.Instantiate(AssetLoader.freethiefbutton, PlayerControl.LocalPlayer.transform.parent);
                cellbuttontwo.name = "cellbuttontwo";
                cellbuttontwo.transform.position = new Vector3(-11f, -39.35f, -0.01f);
                PoliceAndThief.cellbuttontwo = cellbuttontwo;
                GameObject jewelbuttontwo = GameObject.Instantiate(AssetLoader.jewelbutton, PlayerControl.LocalPlayer.transform.parent);
                jewelbuttontwo.name = "jewelbuttontwo";
                jewelbuttontwo.transform.position = new Vector3(13f, -32.5f, -0.01f);
                PoliceAndThief.jewelbuttontwo = jewelbuttontwo;
                GameObject thiefspaceshiptwo = GameObject.Instantiate(AssetLoader.thiefspaceship, PlayerControl.LocalPlayer.transform.parent);
                thiefspaceshiptwo.name = "thiefspaceshiptwo";
                thiefspaceshiptwo.transform.position = new Vector3(-2.75f, 9f, 0.031f);
                thiefspaceshiptwo.transform.localScale = new Vector3(-1f, 1f, 1f);
                break;
        }

        foreach (PlayerControl player in PoliceAndThief.policeTeam)
        {
            player.transform.position = policeTeamPos;
        }

        foreach (PlayerControl player in PoliceAndThief.thiefTeam)
        {
            player.transform.position = thiefTeamPos;
            if (player == PlayerControl.LocalPlayer)
            {
                // Add Arrows pointing the release and deliver point
                if (PoliceAndThief.localThiefReleaseArrow.Count == 0)
                {
                    PoliceAndThief.localThiefReleaseArrow.Add(new Arrow(Palette.PlayerColors[10]));
                    PoliceAndThief.localThiefReleaseArrow[0].ArrowObject.SetActive(true);
                    if (GameOptionsManager.Instance.currentGameOptions.MapId == 6)
                    {
                        PoliceAndThief.localThiefReleaseArrow.Add(new Arrow(Palette.PlayerColors[10]));
                        PoliceAndThief.localThiefReleaseArrow[1].ArrowObject.SetActive(true);
                    }
                }
                if (PoliceAndThief.localThiefDeliverArrow.Count == 0)
                {
                    PoliceAndThief.localThiefDeliverArrow.Add(new Arrow(Palette.PlayerColors[16]));
                    PoliceAndThief.localThiefDeliverArrow[0].ArrowObject.SetActive(true);
                    if (GameOptionsManager.Instance.currentGameOptions.MapId == 6)
                    {
                        PoliceAndThief.localThiefDeliverArrow.Add(new Arrow(Palette.PlayerColors[16]));
                        PoliceAndThief.localThiefDeliverArrow[1].ArrowObject.SetActive(true);
                    }
                }
            }
        }

        if (PlayerControl.LocalPlayer != null && !createdpoliceandthief)
        {
            Helpers.ClearAllTasks(PlayerControl.LocalPlayer);

            GameObject cell = GameObject.Instantiate(AssetLoader.cell, PlayerControl.LocalPlayer.transform.parent);
            cell.name = "cell";
            cell.transform.position = cellPos;
            cell.gameObject.layer = 9;
            cell.transform.GetChild(0).gameObject.layer = 9;
            PoliceAndThief.cell = cell;
            GameObject cellbutton = GameObject.Instantiate(AssetLoader.freethiefbutton, PlayerControl.LocalPlayer.transform.parent);
            cellbutton.name = "cellbutton";
            cellbutton.transform.position = cellButtonPos;
            PoliceAndThief.cellbutton = cellbutton;
            GameObject jewelbutton = GameObject.Instantiate(AssetLoader.jewelbutton, PlayerControl.LocalPlayer.transform.parent);
            jewelbutton.name = "jewelbutton";
            jewelbutton.transform.position = jewelButtonPos;
            PoliceAndThief.jewelbutton = jewelbutton;
            GameObject thiefspaceship = GameObject.Instantiate(AssetLoader.thiefspaceship, PlayerControl.LocalPlayer.transform.parent);
            thiefspaceship.name = "thiefspaceship";
            thiefspaceship.transform.position = thiefSpaceShipPos;

            // Spawn jewels
            GameObject jewel01 = GameObject.Instantiate(AssetLoader.jeweldiamond, PlayerControl.LocalPlayer.transform.parent);
            jewel01.transform.position = jewel01Pos;
            jewel01.name = "jewel01";
            PoliceAndThief.jewel01 = jewel01;
            GameObject jewel02 = GameObject.Instantiate(AssetLoader.jeweldiamond, PlayerControl.LocalPlayer.transform.parent);
            jewel02.transform.position = jewel02Pos;
            jewel02.name = "jewel02";
            PoliceAndThief.jewel02 = jewel02;
            GameObject jewel03 = GameObject.Instantiate(AssetLoader.jeweldiamond, PlayerControl.LocalPlayer.transform.parent);
            jewel03.transform.position = jewel03Pos;
            jewel03.name = "jewel03";
            PoliceAndThief.jewel03 = jewel03;
            GameObject jewel04 = GameObject.Instantiate(AssetLoader.jeweldiamond, PlayerControl.LocalPlayer.transform.parent);
            jewel04.transform.position = jewel04Pos;
            jewel04.name = "jewel04";
            PoliceAndThief.jewel04 = jewel04;
            GameObject jewel05 = GameObject.Instantiate(AssetLoader.jeweldiamond, PlayerControl.LocalPlayer.transform.parent);
            jewel05.transform.position = jewel05Pos;
            jewel05.name = "jewel05";
            PoliceAndThief.jewel05 = jewel05;
            GameObject jewel06 = GameObject.Instantiate(AssetLoader.jeweldiamond, PlayerControl.LocalPlayer.transform.parent);
            jewel06.transform.position = jewel06Pos;
            jewel06.name = "jewel06";
            PoliceAndThief.jewel06 = jewel06;
            GameObject jewel07 = GameObject.Instantiate(AssetLoader.jeweldiamond, PlayerControl.LocalPlayer.transform.parent);
            jewel07.transform.position = jewel07Pos;
            jewel07.name = "jewel07";
            PoliceAndThief.jewel07 = jewel07;
            GameObject jewel08 = GameObject.Instantiate(AssetLoader.jeweldiamond, PlayerControl.LocalPlayer.transform.parent);
            jewel08.transform.position = jewel08Pos;
            jewel08.name = "jewel08";
            PoliceAndThief.jewel08 = jewel08;
            GameObject jewel09 = GameObject.Instantiate(AssetLoader.jewelruby, PlayerControl.LocalPlayer.transform.parent);
            jewel09.transform.position = jewel09Pos;
            jewel09.name = "jewel09";
            PoliceAndThief.jewel09 = jewel09;
            GameObject jewel10 = GameObject.Instantiate(AssetLoader.jewelruby, PlayerControl.LocalPlayer.transform.parent);
            jewel10.transform.position = jewel10Pos;
            jewel10.name = "jewel10";
            PoliceAndThief.jewel10 = jewel10;
            GameObject jewel11 = GameObject.Instantiate(AssetLoader.jewelruby, PlayerControl.LocalPlayer.transform.parent);
            jewel11.transform.position = jewel11Pos;
            jewel11.name = "jewel11";
            PoliceAndThief.jewel11 = jewel11;
            GameObject jewel12 = GameObject.Instantiate(AssetLoader.jewelruby, PlayerControl.LocalPlayer.transform.parent);
            jewel12.transform.position = jewel12Pos;
            jewel12.name = "jewel12";
            PoliceAndThief.jewel12 = jewel12;
            GameObject jewel13 = GameObject.Instantiate(AssetLoader.jewelruby, PlayerControl.LocalPlayer.transform.parent);
            jewel13.transform.position = jewel13Pos;
            jewel13.name = "jewel13";
            PoliceAndThief.jewel13 = jewel13;
            GameObject jewel14 = GameObject.Instantiate(AssetLoader.jewelruby, PlayerControl.LocalPlayer.transform.parent);
            jewel14.transform.position = jewel14Pos;
            jewel14.name = "jewel14";
            PoliceAndThief.jewel14 = jewel14;
            GameObject jewel15 = GameObject.Instantiate(AssetLoader.jewelruby, PlayerControl.LocalPlayer.transform.parent);
            jewel15.transform.position = jewel15Pos;
            jewel15.name = "jewel15";
            PoliceAndThief.jewel15 = jewel15;
            PoliceAndThief.thiefTreasures.Add(jewel01);
            PoliceAndThief.thiefTreasures.Add(jewel02);
            PoliceAndThief.thiefTreasures.Add(jewel03);
            PoliceAndThief.thiefTreasures.Add(jewel04);
            PoliceAndThief.thiefTreasures.Add(jewel05);
            PoliceAndThief.thiefTreasures.Add(jewel06);
            PoliceAndThief.thiefTreasures.Add(jewel07);
            PoliceAndThief.thiefTreasures.Add(jewel08);
            PoliceAndThief.thiefTreasures.Add(jewel09);
            PoliceAndThief.thiefTreasures.Add(jewel10);
            PoliceAndThief.thiefTreasures.Add(jewel11);
            PoliceAndThief.thiefTreasures.Add(jewel12);
            PoliceAndThief.thiefTreasures.Add(jewel13);
            PoliceAndThief.thiefTreasures.Add(jewel14);
            PoliceAndThief.thiefTreasures.Add(jewel15);

            createdpoliceandthief = true;
        }
    }

    public static void policeandThiefSetTarget()
    {
        if (MapSettings.GameMode is not CustomGameMode.PoliceAndThieves) return;

        var untargetablePolice = new List<PlayerControl>();
        foreach (PlayerControl player in PoliceAndThief.policeTeam)
        {
            untargetablePolice.Add(player);
        }

        // Prevent killing reviving players
        if (PoliceAndThief.thiefplayer01IsReviving)
        {
            untargetablePolice.Add(PoliceAndThief.thiefplayer01);
        }
        else
        {
            untargetablePolice.Remove(PoliceAndThief.thiefplayer01);
        }
        if (PoliceAndThief.thiefplayer02IsReviving)
        {
            untargetablePolice.Add(PoliceAndThief.thiefplayer02);
        }
        else
        {
            untargetablePolice.Remove(PoliceAndThief.thiefplayer02);
        }
        if (PoliceAndThief.thiefplayer03IsReviving)
        {
            untargetablePolice.Add(PoliceAndThief.thiefplayer03);
        }
        else
        {
            untargetablePolice.Remove(PoliceAndThief.thiefplayer03);
        }
        if (PoliceAndThief.thiefplayer04IsReviving)
        {
            untargetablePolice.Add(PoliceAndThief.thiefplayer04);
        }
        else
        {
            untargetablePolice.Remove(PoliceAndThief.thiefplayer04);
        }
        if (PoliceAndThief.thiefplayer05IsReviving)
        {
            untargetablePolice.Add(PoliceAndThief.thiefplayer05);
        }
        else
        {
            untargetablePolice.Remove(PoliceAndThief.thiefplayer05);
        }
        if (PoliceAndThief.thiefplayer06IsReviving)
        {
            untargetablePolice.Add(PoliceAndThief.thiefplayer06);
        }
        else
        {
            untargetablePolice.Remove(PoliceAndThief.thiefplayer06);
        }
        if (PoliceAndThief.thiefplayer07IsReviving)
        {
            untargetablePolice.Add(PoliceAndThief.thiefplayer07);
        }
        else
        {
            untargetablePolice.Remove(PoliceAndThief.thiefplayer07);
        }
        if (PoliceAndThief.thiefplayer08IsReviving)
        {
            untargetablePolice.Add(PoliceAndThief.thiefplayer08);
        }
        else
        {
            untargetablePolice.Remove(PoliceAndThief.thiefplayer08);
        }
        if (PoliceAndThief.thiefplayer09IsReviving)
        {
            untargetablePolice.Add(PoliceAndThief.thiefplayer09);
        }
        else
        {
            untargetablePolice.Remove(PoliceAndThief.thiefplayer09);
        }

        if (PoliceAndThief.policeplayer01 != null && PoliceAndThief.policeplayer01 == PlayerControl.LocalPlayer)
        {
            PoliceAndThief.policeplayer01currentTarget = Helpers.SetTarget(untargetablePlayers: untargetablePolice);
            Helpers.SetPlayerOutline(PoliceAndThief.policeplayer01currentTarget, PolicePlayerColor);
        }
        if (PoliceAndThief.policeplayer02 != null && PoliceAndThief.policeplayer02 == PlayerControl.LocalPlayer)
        {
            PoliceAndThief.policeplayer02currentTarget = Helpers.SetTarget(untargetablePlayers: untargetablePolice);
            Helpers.SetPlayerOutline(PoliceAndThief.policeplayer02currentTarget, PolicePlayerColor);
        }
        if (PoliceAndThief.policeplayer03 != null && PoliceAndThief.policeplayer03 == PlayerControl.LocalPlayer)
        {
            PoliceAndThief.policeplayer03currentTarget = Helpers.SetTarget(untargetablePlayers: untargetablePolice);
            Helpers.SetPlayerOutline(PoliceAndThief.policeplayer03currentTarget, PolicePlayerColor);
        }
        if (PoliceAndThief.policeplayer04 != null && PoliceAndThief.policeplayer04 == PlayerControl.LocalPlayer)
        {
            PoliceAndThief.policeplayer04currentTarget = Helpers.SetTarget(untargetablePlayers: untargetablePolice);
            Helpers.SetPlayerOutline(PoliceAndThief.policeplayer04currentTarget, PolicePlayerColor);
        }
        if (PoliceAndThief.policeplayer05 != null && PoliceAndThief.policeplayer05 == PlayerControl.LocalPlayer)
        {
            PoliceAndThief.policeplayer05currentTarget = Helpers.SetTarget(untargetablePlayers: untargetablePolice);
            Helpers.SetPlayerOutline(PoliceAndThief.policeplayer05currentTarget, PolicePlayerColor);
        }
        if (PoliceAndThief.policeplayer06 != null && PoliceAndThief.policeplayer06 == PlayerControl.LocalPlayer)
        {
            PoliceAndThief.policeplayer06currentTarget = Helpers.SetTarget(untargetablePlayers: untargetablePolice);
            Helpers.SetPlayerOutline(PoliceAndThief.policeplayer06currentTarget, PolicePlayerColor);
        }

        var untargetableThiefs = new List<PlayerControl>();
        foreach (PlayerControl player in PoliceAndThief.thiefTeam)
        {
            untargetableThiefs.Add(player);
        }

        // Prevent killing reviving players
        if (PoliceAndThief.policeplayer01IsReviving)
        {
            untargetableThiefs.Add(PoliceAndThief.policeplayer01);
        }
        else
        {
            untargetableThiefs.Remove(PoliceAndThief.policeplayer01);
        }
        if (PoliceAndThief.policeplayer02IsReviving)
        {
            untargetableThiefs.Add(PoliceAndThief.policeplayer02);
        }
        else
        {
            untargetableThiefs.Remove(PoliceAndThief.policeplayer02);
        }
        if (PoliceAndThief.policeplayer03IsReviving)
        {
            untargetableThiefs.Add(PoliceAndThief.policeplayer03);
        }
        else
        {
            untargetableThiefs.Remove(PoliceAndThief.policeplayer03);
        }
        if (PoliceAndThief.policeplayer04IsReviving)
        {
            untargetableThiefs.Add(PoliceAndThief.policeplayer04);
        }
        else
        {
            untargetableThiefs.Remove(PoliceAndThief.policeplayer04);
        }
        if (PoliceAndThief.policeplayer05IsReviving)
        {
            untargetableThiefs.Add(PoliceAndThief.policeplayer05);
        }
        else
        {
            untargetableThiefs.Remove(PoliceAndThief.policeplayer05);
        }
        if (PoliceAndThief.policeplayer06IsReviving)
        {
            untargetableThiefs.Add(PoliceAndThief.policeplayer06);
        }
        else
        {
            untargetableThiefs.Remove(PoliceAndThief.policeplayer06);
        }

        if (PoliceAndThief.thiefplayer01 != null && PoliceAndThief.thiefplayer01 == PlayerControl.LocalPlayer)
        {
            PoliceAndThief.thiefplayer01currentTarget = Helpers.SetTarget(untargetablePlayers: untargetableThiefs);
            Helpers.SetPlayerOutline(PoliceAndThief.thiefplayer01currentTarget, ThiefPlayerColor);
        }
        if (PoliceAndThief.thiefplayer02 != null && PoliceAndThief.thiefplayer02 == PlayerControl.LocalPlayer)
        {
            PoliceAndThief.thiefplayer02currentTarget = Helpers.SetTarget(untargetablePlayers: untargetableThiefs);
            Helpers.SetPlayerOutline(PoliceAndThief.thiefplayer02currentTarget, ThiefPlayerColor);
        }
        if (PoliceAndThief.thiefplayer03 != null && PoliceAndThief.thiefplayer03 == PlayerControl.LocalPlayer)
        {
            PoliceAndThief.thiefplayer03currentTarget = Helpers.SetTarget(untargetablePlayers: untargetableThiefs);
            Helpers.SetPlayerOutline(PoliceAndThief.thiefplayer03currentTarget, ThiefPlayerColor);
        }
        if (PoliceAndThief.thiefplayer04 != null && PoliceAndThief.thiefplayer04 == PlayerControl.LocalPlayer)
        {
            PoliceAndThief.thiefplayer04currentTarget = Helpers.SetTarget(untargetablePlayers: untargetableThiefs);
            Helpers.SetPlayerOutline(PoliceAndThief.thiefplayer04currentTarget, ThiefPlayerColor);
        }
        if (PoliceAndThief.thiefplayer05 != null && PoliceAndThief.thiefplayer05 == PlayerControl.LocalPlayer)
        {
            PoliceAndThief.thiefplayer05currentTarget = Helpers.SetTarget(untargetablePlayers: untargetableThiefs);
            Helpers.SetPlayerOutline(PoliceAndThief.thiefplayer05currentTarget, ThiefPlayerColor);
        }
        if (PoliceAndThief.thiefplayer06 != null && PoliceAndThief.thiefplayer06 == PlayerControl.LocalPlayer)
        {
            PoliceAndThief.thiefplayer06currentTarget = Helpers.SetTarget(untargetablePlayers: untargetableThiefs);
            Helpers.SetPlayerOutline(PoliceAndThief.thiefplayer06currentTarget, ThiefPlayerColor);
        }
        if (PoliceAndThief.thiefplayer07 != null && PoliceAndThief.thiefplayer07 == PlayerControl.LocalPlayer)
        {
            PoliceAndThief.thiefplayer07currentTarget = Helpers.SetTarget(untargetablePlayers: untargetableThiefs);
            Helpers.SetPlayerOutline(PoliceAndThief.thiefplayer07currentTarget, ThiefPlayerColor);
        }
        if (PoliceAndThief.thiefplayer08 != null && PoliceAndThief.thiefplayer08 == PlayerControl.LocalPlayer)
        {
            PoliceAndThief.thiefplayer08currentTarget = Helpers.SetTarget(untargetablePlayers: untargetableThiefs);
            Helpers.SetPlayerOutline(PoliceAndThief.thiefplayer08currentTarget, ThiefPlayerColor);
        }
        if (PoliceAndThief.thiefplayer09 != null && PoliceAndThief.thiefplayer09 == PlayerControl.LocalPlayer)
        {
            PoliceAndThief.thiefplayer09currentTarget = Helpers.SetTarget(untargetablePlayers: untargetableThiefs);
            Helpers.SetPlayerOutline(PoliceAndThief.thiefplayer09currentTarget, ThiefPlayerColor);
        }
    }

    public static void OnMurderPlayerPostfix(PlayerControl __instance, PlayerControl target)
    {
        foreach (PlayerControl player in PoliceAndThief.policeTeam)
        {
            if (player.PlayerId == target.PlayerId)
            {
                var ptBody = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == target.PlayerId);
                ptBody.transform.position = new Vector3(50, 50, 1);
                if (PoliceAndThief.policeplayer01 != null && target.PlayerId == PoliceAndThief.policeplayer01.PlayerId)
                {
                    PoliceAndThief.policeplayer01IsReviving = true;
                }
                else if (PoliceAndThief.policeplayer02 != null && target.PlayerId == PoliceAndThief.policeplayer02.PlayerId)
                {
                    PoliceAndThief.policeplayer02IsReviving = true;
                }
                else if (PoliceAndThief.policeplayer03 != null && target.PlayerId == PoliceAndThief.policeplayer03.PlayerId)
                {
                    PoliceAndThief.policeplayer03IsReviving = true;
                }
                else if (PoliceAndThief.policeplayer04 != null && target.PlayerId == PoliceAndThief.policeplayer04.PlayerId)
                {
                    PoliceAndThief.policeplayer04IsReviving = true;
                }
                else if (PoliceAndThief.policeplayer05 != null && target.PlayerId == PoliceAndThief.policeplayer05.PlayerId)
                {
                    PoliceAndThief.policeplayer05IsReviving = true;
                }
                else if (PoliceAndThief.policeplayer06 != null && target.PlayerId == PoliceAndThief.policeplayer06.PlayerId)
                {
                    PoliceAndThief.policeplayer06IsReviving = true;
                }
                Helpers.alphaPlayer(true, player.PlayerId);
                HudManager.Instance.StartCoroutine(Effects.Lerp(MapSettings.gamemodeInvincibilityTime, new Action<float>((p) =>
                {
                    if (p == 1f && player != null)
                    {
                        if (PoliceAndThief.policeplayer01 != null && target.PlayerId == PoliceAndThief.policeplayer01.PlayerId)
                        {
                            PoliceAndThief.policeplayer01IsReviving = false;
                        }
                        else if (PoliceAndThief.policeplayer02 != null && target.PlayerId == PoliceAndThief.policeplayer02.PlayerId)
                        {
                            PoliceAndThief.policeplayer02IsReviving = false;
                        }
                        else if (PoliceAndThief.policeplayer03 != null && target.PlayerId == PoliceAndThief.policeplayer03.PlayerId)
                        {
                            PoliceAndThief.policeplayer03IsReviving = false;
                        }
                        else if (PoliceAndThief.policeplayer04 != null && target.PlayerId == PoliceAndThief.policeplayer04.PlayerId)
                        {
                            PoliceAndThief.policeplayer04IsReviving = false;
                        }
                        else if (PoliceAndThief.policeplayer05 != null && target.PlayerId == PoliceAndThief.policeplayer05.PlayerId)
                        {
                            PoliceAndThief.policeplayer05IsReviving = false;
                        }
                        else if (PoliceAndThief.policeplayer06 != null && target.PlayerId == PoliceAndThief.policeplayer06.PlayerId)
                        {
                            PoliceAndThief.policeplayer06IsReviving = false;
                        }
                        Helpers.alphaPlayer(false, player.PlayerId);
                    }
                })));

                HudManager.Instance.StartCoroutine(Effects.Lerp(MapSettings.gamemodeReviveTime - MapSettings.gamemodeInvincibilityTime, new Action<float>((p) =>
                {
                    if (p == 1f && player != null)
                    {
                        player.Revive();
                        switch (GameOptionsManager.Instance.currentGameOptions.MapId)
                        {
                            // Skeld
                            case 0:
                                if (RebuildUs.activatedSensei)
                                {
                                    player.transform.position = new Vector3(-12f, 5f, player.transform.position.z);
                                }
                                else if (RebuildUs.activatedDleks)
                                {
                                    player.transform.position = new Vector3(10.2f, 1.18f, player.transform.position.z);
                                }
                                else
                                {
                                    player.transform.position = new Vector3(-10.2f, 1.18f, player.transform.position.z);
                                }
                                break;
                            // MiraHQ
                            case 1:
                                player.transform.position = new Vector3(1.8f, -1f, player.transform.position.z);
                                break;
                            // Polus
                            case 2:
                                player.transform.position = new Vector3(8.18f, -7.4f, player.transform.position.z);
                                break;
                            // Dleks
                            case 3:
                                player.transform.position = new Vector3(10.2f, 1.18f, player.transform.position.z);
                                break;
                            // Airship
                            case 4:
                                player.transform.position = new Vector3(-18.5f, 0.75f, player.transform.position.z);
                                break;
                            // Fungle
                            case 5:
                                player.transform.position = new Vector3(-22.5f, -0.5f, player.transform.position.z);
                                break;
                            // Submerged
                            case 6:
                                if (player.transform.position.y > 0)
                                {
                                    player.transform.position = new Vector3(-8.45f, 27f, player.transform.position.z);
                                }
                                else
                                {
                                    player.transform.position = new Vector3(-9.25f, -41.25f, player.transform.position.z);
                                }
                                break;
                        }
                        DeadPlayer deadPlayerEntry = GameHistory.DeadPlayers.Where(x => x.Player.PlayerId == target.PlayerId).FirstOrDefault();
                        if (ptBody != null) UnityEngine.Object.Destroy(ptBody.gameObject);
                        if (deadPlayerEntry != null) GameHistory.DeadPlayers.Remove(deadPlayerEntry);
                    }

                })));

            }
        }
        foreach (PlayerControl player in PoliceAndThief.thiefTeam)
        {
            if (player.PlayerId == target.PlayerId)
            {
                var ptBody = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == target.PlayerId);
                ptBody.transform.position = new Vector3(50, 50, 1);
                if (PoliceAndThief.thiefplayer01 != null && target.PlayerId == PoliceAndThief.thiefplayer01.PlayerId)
                {
                    if (PoliceAndThief.thiefplayer01IsStealing)
                    {
                        RPCProcedure.policeandThiefRevertedJewelPosition(target.PlayerId, PoliceAndThief.thiefplayer01JewelId);
                    }
                    PoliceAndThief.thiefplayer01IsReviving = true;
                }
                else if (PoliceAndThief.thiefplayer02 != null && target.PlayerId == PoliceAndThief.thiefplayer02.PlayerId)
                {
                    if (PoliceAndThief.thiefplayer02IsStealing)
                    {
                        RPCProcedure.policeandThiefRevertedJewelPosition(target.PlayerId, PoliceAndThief.thiefplayer02JewelId);
                    }
                    PoliceAndThief.thiefplayer02IsReviving = true;
                }
                else if (PoliceAndThief.thiefplayer03 != null && target.PlayerId == PoliceAndThief.thiefplayer03.PlayerId)
                {
                    if (PoliceAndThief.thiefplayer03IsStealing)
                    {
                        RPCProcedure.policeandThiefRevertedJewelPosition(target.PlayerId, PoliceAndThief.thiefplayer03JewelId);
                    }
                    PoliceAndThief.thiefplayer03IsReviving = true;
                }
                else if (PoliceAndThief.thiefplayer04 != null && target.PlayerId == PoliceAndThief.thiefplayer04.PlayerId)
                {
                    if (PoliceAndThief.thiefplayer04IsStealing)
                    {
                        RPCProcedure.policeandThiefRevertedJewelPosition(target.PlayerId, PoliceAndThief.thiefplayer04JewelId);
                    }
                    PoliceAndThief.thiefplayer04IsReviving = true;
                }
                else if (PoliceAndThief.thiefplayer05 != null && target.PlayerId == PoliceAndThief.thiefplayer05.PlayerId)
                {
                    if (PoliceAndThief.thiefplayer05IsStealing)
                    {
                        RPCProcedure.policeandThiefRevertedJewelPosition(target.PlayerId, PoliceAndThief.thiefplayer05JewelId);
                    }
                    PoliceAndThief.thiefplayer05IsReviving = true;
                }
                else if (PoliceAndThief.thiefplayer06 != null && target.PlayerId == PoliceAndThief.thiefplayer06.PlayerId)
                {
                    if (PoliceAndThief.thiefplayer06IsStealing)
                    {
                        RPCProcedure.policeandThiefRevertedJewelPosition(target.PlayerId, PoliceAndThief.thiefplayer06JewelId);
                    }
                    PoliceAndThief.thiefplayer06IsReviving = true;
                }
                else if (PoliceAndThief.thiefplayer07 != null && target.PlayerId == PoliceAndThief.thiefplayer07.PlayerId)
                {
                    if (PoliceAndThief.thiefplayer07IsStealing)
                    {
                        RPCProcedure.policeandThiefRevertedJewelPosition(target.PlayerId, PoliceAndThief.thiefplayer07JewelId);
                    }
                    PoliceAndThief.thiefplayer07IsReviving = true;
                }
                else if (PoliceAndThief.thiefplayer08 != null && target.PlayerId == PoliceAndThief.thiefplayer08.PlayerId)
                {
                    if (PoliceAndThief.thiefplayer08IsStealing)
                    {
                        RPCProcedure.policeandThiefRevertedJewelPosition(target.PlayerId, PoliceAndThief.thiefplayer08JewelId);
                    }
                    PoliceAndThief.thiefplayer08IsReviving = true;
                }
                else if (PoliceAndThief.thiefplayer09 != null && target.PlayerId == PoliceAndThief.thiefplayer09.PlayerId)
                {
                    if (PoliceAndThief.thiefplayer09IsStealing)
                    {
                        RPCProcedure.policeandThiefRevertedJewelPosition(target.PlayerId, PoliceAndThief.thiefplayer09JewelId);
                    }
                    PoliceAndThief.thiefplayer09IsReviving = true;
                }
                Helpers.alphaPlayer(true, player.PlayerId);
                HudManager.Instance.StartCoroutine(Effects.Lerp(MapSettings.gamemodeReviveTime * 1.25f, new Action<float>((p) =>
                {
                    if (p == 1f && player != null)
                    {
                        if (PoliceAndThief.thiefplayer01 != null && target.PlayerId == PoliceAndThief.thiefplayer01.PlayerId)
                        {
                            PoliceAndThief.thiefplayer01IsReviving = false;
                        }
                        else if (PoliceAndThief.thiefplayer02 != null && target.PlayerId == PoliceAndThief.thiefplayer02.PlayerId)
                        {
                            PoliceAndThief.thiefplayer02IsReviving = false;
                        }
                        else if (PoliceAndThief.thiefplayer03 != null && target.PlayerId == PoliceAndThief.thiefplayer03.PlayerId)
                        {
                            PoliceAndThief.thiefplayer03IsReviving = false;
                        }
                        else if (PoliceAndThief.thiefplayer04 != null && target.PlayerId == PoliceAndThief.thiefplayer04.PlayerId)
                        {
                            PoliceAndThief.thiefplayer04IsReviving = false;
                        }
                        else if (PoliceAndThief.thiefplayer05 != null && target.PlayerId == PoliceAndThief.thiefplayer05.PlayerId)
                        {
                            PoliceAndThief.thiefplayer05IsReviving = false;
                        }
                        else if (PoliceAndThief.thiefplayer06 != null && target.PlayerId == PoliceAndThief.thiefplayer06.PlayerId)
                        {
                            PoliceAndThief.thiefplayer06IsReviving = false;
                        }
                        else if (PoliceAndThief.thiefplayer07 != null && target.PlayerId == PoliceAndThief.thiefplayer07.PlayerId)
                        {
                            PoliceAndThief.thiefplayer07IsReviving = false;
                        }
                        else if (PoliceAndThief.thiefplayer08 != null && target.PlayerId == PoliceAndThief.thiefplayer08.PlayerId)
                        {
                            PoliceAndThief.thiefplayer08IsReviving = false;
                        }
                        else if (PoliceAndThief.thiefplayer09 != null && target.PlayerId == PoliceAndThief.thiefplayer09.PlayerId)
                        {
                            PoliceAndThief.thiefplayer09IsReviving = false;
                        }
                        Helpers.alphaPlayer(false, player.PlayerId);
                    }
                })));

                HudManager.Instance.StartCoroutine(Effects.Lerp((MapSettings.gamemodeReviveTime * 1.25f) - MapSettings.gamemodeInvincibilityTime, new Action<float>((p) =>
                {
                    if (p == 1f && player != null)
                    {
                        player.Revive();
                        switch (GameOptionsManager.Instance.currentGameOptions.MapId)
                        {
                            // Skeld
                            case 0:
                                if (RebuildUs.activatedSensei)
                                {
                                    player.transform.position = new Vector3(13.75f, -0.2f, player.transform.position.z);
                                }
                                else if (RebuildUs.activatedDleks)
                                {
                                    player.transform.position = new Vector3(1.31f, -16.25f, player.transform.position.z);
                                }
                                else
                                {
                                    player.transform.position = new Vector3(-1.31f, -16.25f, player.transform.position.z);
                                }
                                break;
                            // MiraHQ
                            case 1:
                                player.transform.position = new Vector3(17.75f, 11.5f, player.transform.position.z);
                                break;
                            // Polus
                            case 2:
                                player.transform.position = new Vector3(30f, -15.75f, player.transform.position.z);
                                break;
                            // Dleks
                            case 3:
                                player.transform.position = new Vector3(1.31f, -16.25f, player.transform.position.z);
                                break;
                            // Airship
                            case 4:
                                player.transform.position = new Vector3(7.15f, -14.5f, player.transform.position.z);
                                break;
                            // Fungle
                            case 5:
                                player.transform.position = new Vector3(20f, 11f, player.transform.position.z);
                                break;
                            // Submerged
                            case 6:
                                if (player.transform.position.y > 0)
                                {
                                    player.transform.position = new Vector3(1f, 10f, player.transform.position.z);
                                }
                                else
                                {
                                    player.transform.position = new Vector3(12.5f, -31.75f, player.transform.position.z);
                                }
                                break;
                        }
                        DeadPlayer deadPlayerEntry = GameHistory.DeadPlayers.Where(x => x.Player.PlayerId == target.PlayerId).FirstOrDefault();
                        if (ptBody != null) UnityEngine.Object.Destroy(ptBody.gameObject);
                        if (deadPlayerEntry != null) GameHistory.DeadPlayers.Remove(deadPlayerEntry);
                    }

                })));

            }
        }
    }

    public static void policeandthiefUpdate()
    {
        if (MapSettings.GameMode is not CustomGameMode.PoliceAndThieves) return;

        // Check number of thiefs if a thief disconnects
        foreach (PlayerControl thief in PoliceAndThief.thiefTeam)
        {
            if (thief.Data.Disconnected)
            {

                if (PoliceAndThief.thiefplayer01 != null && thief.PlayerId == PoliceAndThief.thiefplayer01.PlayerId && PoliceAndThief.thiefplayer01IsStealing)
                {
                    PoliceAndThief.thiefTeam.Remove(PoliceAndThief.thiefplayer01);
                    RPCProcedure.policeandThiefRevertedJewelPosition(thief.PlayerId, PoliceAndThief.thiefplayer01JewelId);
                }
                else if (PoliceAndThief.thiefplayer02 != null && thief.PlayerId == PoliceAndThief.thiefplayer02.PlayerId && PoliceAndThief.thiefplayer02IsStealing)
                {
                    PoliceAndThief.thiefTeam.Remove(PoliceAndThief.thiefplayer02);
                    RPCProcedure.policeandThiefRevertedJewelPosition(thief.PlayerId, PoliceAndThief.thiefplayer02JewelId);
                }
                else if (PoliceAndThief.thiefplayer03 != null && thief.PlayerId == PoliceAndThief.thiefplayer03.PlayerId && PoliceAndThief.thiefplayer03IsStealing)
                {
                    PoliceAndThief.thiefTeam.Remove(PoliceAndThief.thiefplayer03);
                    RPCProcedure.policeandThiefRevertedJewelPosition(thief.PlayerId, PoliceAndThief.thiefplayer03JewelId);
                }
                else if (PoliceAndThief.thiefplayer04 != null && thief.PlayerId == PoliceAndThief.thiefplayer04.PlayerId && PoliceAndThief.thiefplayer04IsStealing)
                {
                    PoliceAndThief.thiefTeam.Remove(PoliceAndThief.thiefplayer04);
                    RPCProcedure.policeandThiefRevertedJewelPosition(thief.PlayerId, PoliceAndThief.thiefplayer04JewelId);
                }
                else if (PoliceAndThief.thiefplayer05 != null && thief.PlayerId == PoliceAndThief.thiefplayer05.PlayerId && PoliceAndThief.thiefplayer05IsStealing)
                {
                    PoliceAndThief.thiefTeam.Remove(PoliceAndThief.thiefplayer05);
                    RPCProcedure.policeandThiefRevertedJewelPosition(thief.PlayerId, PoliceAndThief.thiefplayer05JewelId);
                }
                else if (PoliceAndThief.thiefplayer06 != null && thief.PlayerId == PoliceAndThief.thiefplayer06.PlayerId && PoliceAndThief.thiefplayer06IsStealing)
                {
                    PoliceAndThief.thiefTeam.Remove(PoliceAndThief.thiefplayer06);
                    RPCProcedure.policeandThiefRevertedJewelPosition(thief.PlayerId, PoliceAndThief.thiefplayer06JewelId);
                }
                else if (PoliceAndThief.thiefplayer07 != null && thief.PlayerId == PoliceAndThief.thiefplayer07.PlayerId && PoliceAndThief.thiefplayer07IsStealing)
                {
                    PoliceAndThief.thiefTeam.Remove(PoliceAndThief.thiefplayer07);
                    RPCProcedure.policeandThiefRevertedJewelPosition(thief.PlayerId, PoliceAndThief.thiefplayer07JewelId);
                }
                else if (PoliceAndThief.thiefplayer08 != null && thief.PlayerId == PoliceAndThief.thiefplayer08.PlayerId && PoliceAndThief.thiefplayer08IsStealing)
                {
                    PoliceAndThief.thiefTeam.Remove(PoliceAndThief.thiefplayer08);
                    RPCProcedure.policeandThiefRevertedJewelPosition(thief.PlayerId, PoliceAndThief.thiefplayer08JewelId);
                }
                else if (PoliceAndThief.thiefplayer09 != null && thief.PlayerId == PoliceAndThief.thiefplayer09.PlayerId && PoliceAndThief.thiefplayer09IsStealing)
                {
                    PoliceAndThief.thiefTeam.Remove(PoliceAndThief.thiefplayer09);
                    RPCProcedure.policeandThiefRevertedJewelPosition(thief.PlayerId, PoliceAndThief.thiefplayer09JewelId);
                }

                PoliceAndThief.thiefpointCounter = new StringBuilder(Tr.Get(TrKey.CapturedThieves)).Append("<color=#00F7FFFF>").Append(PoliceAndThief.currentJewelsStoled).Append(" / ").Append(PoliceAndThief.requiredJewels).Append("</color> | ").Append(Tr.Get(TrKey.CapturedThieves)).Append("<color=#928B55FF>").Append(PoliceAndThief.currentThiefsCaptured).Append(" / ").Append(PoliceAndThief.thiefTeam.Count).Append("</color>").ToString();
                if (PoliceAndThief.currentThiefsCaptured == PoliceAndThief.thiefTeam.Count)
                {
                    PoliceAndThief.triggerPoliceWin = true;
                    GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ThiefModePoliceWin, false);
                }
                break;
            }
        }

        foreach (PlayerControl police in PoliceAndThief.policeTeam)
        {
            if (police.Data.Disconnected)
            {
                if (PoliceAndThief.policeplayer01 != null && police.PlayerId == PoliceAndThief.policeplayer01.PlayerId)
                {
                    PoliceAndThief.policeTeam.Remove(PoliceAndThief.policeplayer01);
                }
                else if (PoliceAndThief.policeplayer02 != null && police.PlayerId == PoliceAndThief.policeplayer02.PlayerId)
                {
                    PoliceAndThief.policeTeam.Remove(PoliceAndThief.policeplayer02);
                }
                else if (PoliceAndThief.policeplayer03 != null && police.PlayerId == PoliceAndThief.policeplayer03.PlayerId)
                {
                    PoliceAndThief.policeTeam.Remove(PoliceAndThief.policeplayer03);
                }
                else if (PoliceAndThief.policeplayer04 != null && police.PlayerId == PoliceAndThief.policeplayer04.PlayerId)
                {
                    PoliceAndThief.policeTeam.Remove(PoliceAndThief.policeplayer04);
                }
                else if (PoliceAndThief.policeplayer05 != null && police.PlayerId == PoliceAndThief.policeplayer05.PlayerId)
                {
                    PoliceAndThief.policeTeam.Remove(PoliceAndThief.policeplayer05);
                }
                else if (PoliceAndThief.policeplayer06 != null && police.PlayerId == PoliceAndThief.policeplayer06.PlayerId)
                {
                    PoliceAndThief.policeTeam.Remove(PoliceAndThief.policeplayer06);
                }

                if (PoliceAndThief.policeTeam.Count <= 0)
                {
                    PoliceAndThief.triggerThiefWin = true;
                    GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ThiefModeThiefWin, false);
                }
                break;
            }
        }
    }
}