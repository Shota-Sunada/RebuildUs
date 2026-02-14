namespace RebuildUs.Modules.GameMode;

public static partial class BattleRoyale
{
    public static Color SoloPlayerColor = new Color32(248, 205, 70, byte.MaxValue);
    public static Color LimeTeamColor = new Color32(0, 198, 66, byte.MaxValue);
    public static Color PinkTeamColor = new Color32(242, 190, 255, byte.MaxValue);
    public static Color SerialKillerColor = new Color32(128, 128, 128, byte.MaxValue);
    public static Color IntroColor = new Color32(0, 159, 87, byte.MaxValue);

    private static bool createdbattleroyale = false;
    private static int howmanyBattleRoyaleplayers = 0;

    public static List<PlayerControl> soloPlayerTeam = [];
    public static PlayerControl soloPlayer01 = null;
    public static float soloPlayer01mouseAngle = 0f;
    public static GameObject soloPlayer01Wep = null;
    public static float soloPlayer01Lifes = 3;
    public static PlayerControl soloPlayer02 = null;
    public static float soloPlayer02mouseAngle = 0f;
    public static GameObject soloPlayer02Wep = null;
    public static float soloPlayer02Lifes = 3;
    public static PlayerControl soloPlayer03 = null;
    public static float soloPlayer03mouseAngle = 0f;
    public static GameObject soloPlayer03Wep = null;
    public static float soloPlayer03Lifes = 3;
    public static PlayerControl soloPlayer04 = null;
    public static float soloPlayer04mouseAngle = 0f;
    public static GameObject soloPlayer04Wep = null;
    public static float soloPlayer04Lifes = 3;
    public static PlayerControl soloPlayer05 = null;
    public static float soloPlayer05mouseAngle = 0f;
    public static GameObject soloPlayer05Wep = null;
    public static float soloPlayer05Lifes = 3;
    public static PlayerControl soloPlayer06 = null;
    public static float soloPlayer06mouseAngle = 0f;
    public static GameObject soloPlayer06Wep = null;
    public static float soloPlayer06Lifes = 3;
    public static PlayerControl soloPlayer07 = null;
    public static float soloPlayer07mouseAngle = 0f;
    public static GameObject soloPlayer07Wep = null;
    public static float soloPlayer07Lifes = 3;
    public static PlayerControl soloPlayer08 = null;
    public static float soloPlayer08mouseAngle = 0f;
    public static GameObject soloPlayer08Wep = null;
    public static float soloPlayer08Lifes = 3;
    public static PlayerControl soloPlayer09 = null;
    public static float soloPlayer09mouseAngle = 0f;
    public static GameObject soloPlayer09Wep = null;
    public static float soloPlayer09Lifes = 3;
    public static PlayerControl soloPlayer10 = null;
    public static float soloPlayer10mouseAngle = 0f;
    public static GameObject soloPlayer10Wep = null;
    public static float soloPlayer10Lifes = 3;
    public static PlayerControl soloPlayer11 = null;
    public static float soloPlayer11mouseAngle = 0f;
    public static GameObject soloPlayer11Wep = null;
    public static float soloPlayer11Lifes = 3;
    public static PlayerControl soloPlayer12 = null;
    public static float soloPlayer12mouseAngle = 0f;
    public static GameObject soloPlayer12Wep = null;
    public static float soloPlayer12Lifes = 3;
    public static PlayerControl soloPlayer13 = null;
    public static float soloPlayer13mouseAngle = 0f;
    public static GameObject soloPlayer13Wep = null;
    public static float soloPlayer13Lifes = 3;
    public static PlayerControl soloPlayer14 = null;
    public static float soloPlayer14mouseAngle = 0f;
    public static GameObject soloPlayer14Wep = null;
    public static float soloPlayer14Lifes = 3;
    public static PlayerControl soloPlayer15 = null;
    public static float soloPlayer15mouseAngle = 0f;
    public static GameObject soloPlayer15Wep = null;
    public static float soloPlayer15Lifes = 3;

    public static List<PlayerControl> limeTeam = [];
    public static PlayerControl limePlayer01 = null;
    public static float limePlayer01mouseAngle = 0f;
    public static GameObject limePlayer01Wep = null;
    public static float limePlayer01Lifes = 3;
    public static bool limePlayer01IsReviving = false;
    public static PlayerControl limePlayer02 = null;
    public static float limePlayer02mouseAngle = 0f;
    public static GameObject limePlayer02Wep = null;
    public static float limePlayer02Lifes = 3;
    public static bool limePlayer02IsReviving = false;
    public static PlayerControl limePlayer03 = null;
    public static float limePlayer03mouseAngle = 0f;
    public static GameObject limePlayer03Wep = null;
    public static float limePlayer03Lifes = 3;
    public static bool limePlayer03IsReviving = false;
    public static PlayerControl limePlayer04 = null;
    public static float limePlayer04mouseAngle = 0f;
    public static GameObject limePlayer04Wep = null;
    public static float limePlayer04Lifes = 3;
    public static bool limePlayer04IsReviving = false;
    public static PlayerControl limePlayer05 = null;
    public static float limePlayer05mouseAngle = 0f;
    public static GameObject limePlayer05Wep = null;
    public static float limePlayer05Lifes = 3;
    public static bool limePlayer05IsReviving = false;
    public static PlayerControl limePlayer06 = null;
    public static float limePlayer06mouseAngle = 0f;
    public static GameObject limePlayer06Wep = null;
    public static float limePlayer06Lifes = 3;
    public static bool limePlayer06IsReviving = false;
    public static PlayerControl limePlayer07 = null;
    public static float limePlayer07mouseAngle = 0f;
    public static GameObject limePlayer07Wep = null;
    public static float limePlayer07Lifes = 3;
    public static bool limePlayer07IsReviving = false;

    public static List<PlayerControl> pinkTeam = [];
    public static PlayerControl pinkPlayer01 = null;
    public static float pinkPlayer01mouseAngle = 0f;
    public static GameObject pinkPlayer01Wep = null;
    public static float pinkPlayer01Lifes = 3;
    public static bool pinkPlayer01IsReviving = false;
    public static PlayerControl pinkPlayer02 = null;
    public static float pinkPlayer02mouseAngle = 0f;
    public static GameObject pinkPlayer02Wep = null;
    public static float pinkPlayer02Lifes = 3;
    public static bool pinkPlayer02IsReviving = false;
    public static PlayerControl pinkPlayer03 = null;
    public static float pinkPlayer03mouseAngle = 0f;
    public static GameObject pinkPlayer03Wep = null;
    public static float pinkPlayer03Lifes = 3;
    public static bool pinkPlayer03IsReviving = false;
    public static PlayerControl pinkPlayer04 = null;
    public static float pinkPlayer04mouseAngle = 0f;
    public static GameObject pinkPlayer04Wep = null;
    public static float pinkPlayer04Lifes = 3;
    public static bool pinkPlayer04IsReviving = false;
    public static PlayerControl pinkPlayer05 = null;
    public static float pinkPlayer05mouseAngle = 0f;
    public static GameObject pinkPlayer05Wep = null;
    public static float pinkPlayer05Lifes = 3;
    public static bool pinkPlayer05IsReviving = false;
    public static PlayerControl pinkPlayer06 = null;
    public static float pinkPlayer06mouseAngle = 0f;
    public static GameObject pinkPlayer06Wep = null;
    public static float pinkPlayer06Lifes = 3;
    public static bool pinkPlayer06IsReviving = false;
    public static PlayerControl pinkPlayer07 = null;
    public static float pinkPlayer07mouseAngle = 0f;
    public static GameObject pinkPlayer07Wep = null;
    public static float pinkPlayer07Lifes = 3;
    public static bool pinkPlayer07IsReviving = false;

    public static List<PlayerControl> serialKillerTeam = [];
    public static PlayerControl serialKiller = null;
    public static float serialKillermouseAngle = 0f;
    public static GameObject serialKillerWep = null;
    public static float serialKillerLifes = 3;
    public static float serialKillerCooldown = 3;
    public static bool serialKillerIsReviving = false;
    public static List<GameObject> serialKillerSpawns = [];

    public static List<Vector3> soloPlayersSpawnPositions = [];
    public static bool battleRoyaleSenseiMapMode = false;
    public static bool battleRoyaleDleksMap = false;

    public static float killCooldown = 1;
    public static float fighterLifes = 3f;
    public static int matchType = 0;
    public static float requiredScore = 0;

    public static int limePoints = 0;
    public static int pinkPoints = 0;
    public static int serialKillerPoints = 0;

    public static bool triggerSoloWin = false;
    public static bool triggerTimeWin = false;
    public static bool triggerLimeTeamWin = false;
    public static bool triggerPinkTeamWin = false;
    public static bool triggerSerialKillerWin = false;
    public static bool triggerDrawWin = false;

    public static string battleRoyalePointCounter = "";

    public static void clearAndReload()
    {
        fighterLifes = CustomOptionHolder.battleRoyaleLifes.GetFloat();

        howmanyBattleRoyaleplayers = 0;
        createdbattleroyale = false;

        soloPlayersSpawnPositions.Clear();
        soloPlayerTeam.Clear();
        limeTeam.Clear();
        pinkTeam.Clear();
        serialKillerTeam.Clear();
        soloPlayer01 = null;
        soloPlayer01mouseAngle = 0;
        soloPlayer01Wep = null;
        soloPlayer01Lifes = fighterLifes;
        soloPlayer02 = null;
        soloPlayer02mouseAngle = 0;
        soloPlayer02Wep = null;
        soloPlayer02Lifes = fighterLifes;
        soloPlayer03 = null;
        soloPlayer03mouseAngle = 0;
        soloPlayer03Wep = null;
        soloPlayer03Lifes = fighterLifes;
        soloPlayer04 = null;
        soloPlayer04mouseAngle = 0;
        soloPlayer04Wep = null;
        soloPlayer04Lifes = fighterLifes;
        soloPlayer05 = null;
        soloPlayer05mouseAngle = 0;
        soloPlayer05Wep = null;
        soloPlayer05Lifes = fighterLifes;
        soloPlayer06 = null;
        soloPlayer06mouseAngle = 0;
        soloPlayer06Wep = null;
        soloPlayer06Lifes = fighterLifes;
        soloPlayer07 = null;
        soloPlayer07mouseAngle = 0;
        soloPlayer07Wep = null;
        soloPlayer07Lifes = fighterLifes;
        soloPlayer08 = null;
        soloPlayer08mouseAngle = 0;
        soloPlayer08Wep = null;
        soloPlayer08Lifes = fighterLifes;
        soloPlayer09 = null;
        soloPlayer09mouseAngle = 0;
        soloPlayer09Wep = null;
        soloPlayer09Lifes = fighterLifes;
        soloPlayer10 = null;
        soloPlayer10mouseAngle = 0;
        soloPlayer10Wep = null;
        soloPlayer10Lifes = fighterLifes;
        soloPlayer11 = null;
        soloPlayer11mouseAngle = 0;
        soloPlayer11Wep = null;
        soloPlayer11Lifes = fighterLifes;
        soloPlayer12 = null;
        soloPlayer12mouseAngle = 0;
        soloPlayer12Wep = null;
        soloPlayer12Lifes = fighterLifes;
        soloPlayer13 = null;
        soloPlayer13mouseAngle = 0;
        soloPlayer13Wep = null;
        soloPlayer13Lifes = fighterLifes;
        soloPlayer14 = null;
        soloPlayer14mouseAngle = 0;
        soloPlayer14Wep = null;
        soloPlayer14Lifes = fighterLifes;
        soloPlayer15 = null;
        soloPlayer15mouseAngle = 0;
        soloPlayer15Wep = null;
        soloPlayer15Lifes = fighterLifes;

        limePlayer01 = null;
        limePlayer01mouseAngle = 0;
        limePlayer01Wep = null;
        limePlayer01Lifes = fighterLifes;
        limePlayer01IsReviving = false;
        limePlayer02 = null;
        limePlayer02mouseAngle = 0;
        limePlayer02Wep = null;
        limePlayer02Lifes = fighterLifes;
        limePlayer02IsReviving = false;
        limePlayer03 = null;
        limePlayer03mouseAngle = 0;
        limePlayer03Wep = null;
        limePlayer03Lifes = fighterLifes;
        limePlayer03IsReviving = false;
        limePlayer04 = null;
        limePlayer04mouseAngle = 0;
        limePlayer04Wep = null;
        limePlayer04Lifes = fighterLifes;
        limePlayer04IsReviving = false;
        limePlayer05 = null;
        limePlayer05mouseAngle = 0;
        limePlayer05Wep = null;
        limePlayer05Lifes = fighterLifes;
        limePlayer05IsReviving = false;
        limePlayer06 = null;
        limePlayer06mouseAngle = 0;
        limePlayer06Wep = null;
        limePlayer06Lifes = fighterLifes;
        limePlayer06IsReviving = false;
        limePlayer07 = null;
        limePlayer07mouseAngle = 0;
        limePlayer07Wep = null;
        limePlayer07Lifes = fighterLifes;
        limePlayer07IsReviving = false;
        pinkPlayer01 = null;
        pinkPlayer01mouseAngle = 0;
        pinkPlayer01Wep = null;
        pinkPlayer01Lifes = fighterLifes;
        pinkPlayer01IsReviving = false;
        pinkPlayer02 = null;
        pinkPlayer02mouseAngle = 0;
        pinkPlayer02Wep = null;
        pinkPlayer02Lifes = fighterLifes;
        pinkPlayer02IsReviving = false;
        pinkPlayer03 = null;
        pinkPlayer03mouseAngle = 0;
        pinkPlayer03Wep = null;
        pinkPlayer03Lifes = fighterLifes;
        pinkPlayer03IsReviving = false;
        pinkPlayer04 = null;
        pinkPlayer04mouseAngle = 0;
        pinkPlayer04Wep = null;
        pinkPlayer04Lifes = fighterLifes;
        pinkPlayer04IsReviving = false;
        pinkPlayer05 = null;
        pinkPlayer05mouseAngle = 0;
        pinkPlayer05Wep = null;
        pinkPlayer05Lifes = fighterLifes;
        pinkPlayer05IsReviving = false;
        pinkPlayer06 = null;
        pinkPlayer06mouseAngle = 0;
        pinkPlayer06Wep = null;
        pinkPlayer06Lifes = fighterLifes;
        pinkPlayer06IsReviving = false;
        pinkPlayer07 = null;
        pinkPlayer07mouseAngle = 0;
        pinkPlayer07Wep = null;
        pinkPlayer07Lifes = fighterLifes;
        pinkPlayer07IsReviving = false;
        serialKiller = null;
        serialKillermouseAngle = 0;
        serialKillerWep = null;
        serialKillerIsReviving = false;
        serialKillerSpawns.Clear();

        killCooldown = CustomOptionHolder.battleRoyaleKillCooldown.GetFloat();
        battleRoyaleSenseiMapMode = CustomOptionHolder.CustomSkeldMap.GetSelection() == 2;
        battleRoyaleDleksMap = CustomOptionHolder.CustomSkeldMap.GetSelection() == 1;
        matchType = CustomOptionHolder.battleRoyaleMatchType.GetSelection();
        if (PlayerControl.AllPlayerControls.Count >= 11)
        {
            serialKillerCooldown = killCooldown / 3;
            serialKillerLifes = fighterLifes * 3;
        }
        else
        {
            serialKillerCooldown = killCooldown / 2;
            serialKillerLifes = fighterLifes * 2;
        }
        requiredScore = CustomOptionHolder.battleRoyaleScoreNeeded.GetFloat();

        limePoints = 0;
        pinkPoints = 0;
        serialKillerPoints = 0;

        triggerSoloWin = false;
        triggerTimeWin = false;
        triggerLimeTeamWin = false;
        triggerPinkTeamWin = false;
        triggerSerialKillerWin = false;
        triggerDrawWin = false;

        switch (matchType)
        {
            case 0:
                battleRoyalePointCounter = new StringBuilder(Tr.Get(TrKey.BattleRoyaleFighters)).Append("<color=#009F57FF>").Append(soloPlayerTeam.Count).Append("</color>").ToString();
                break;
            case 1:
                if (serialKiller != null)
                {
                    var sb = new StringBuilder(Tr.Get(TrKey.BattleRoyaleLimeTeam))
                                         .Append("<color=#39FF14FF>")
                                         .Append(limeTeam.Count)
                                         .Append("</color> | ")
                                         .Append(Tr.Get(TrKey.BattleRoyalePinkTeam))
                                         .Append("<color=#F2BEFFFF>")
                                         .Append(pinkTeam.Count)
                                         .Append("</color> | ")
                                         .Append(Tr.Get(TrKey.SerialKiller))
                                         .Append("<color=#808080FF>")
                                         .Append(serialKillerTeam.Count)
                                         .Append("</color>");
                    battleRoyalePointCounter = sb.ToString();
                }
                else
                {
                    var sb = new StringBuilder(Tr.Get(TrKey.BattleRoyaleLimeTeam))
                                         .Append("<color=#39FF14FF>")
                                         .Append(limeTeam.Count)
                                         .Append("</color> | ")
                                         .Append(Tr.Get(TrKey.BattleRoyalePinkTeam))
                                         .Append("<color=#F2BEFFFF>")
                                         .Append(pinkTeam.Count)
                                         .Append("</color>");
                    battleRoyalePointCounter = sb.ToString();
                }
                break;
            case 2:
                if (serialKiller != null)
                {
                    var sb = new StringBuilder(Tr.Get(TrKey.BattleRoyaleGoal))
                                       .Append(requiredScore)
                                       .Append(" | <color=#39FF14FF>")
                                       .Append(Tr.Get(TrKey.BattleRoyaleLimeTeam))
                                       .Append(limePoints)
                                       .Append("</color> | <color=#F2BEFFFF>")
                                       .Append(Tr.Get(TrKey.BattleRoyalePinkTeam))
                                       .Append(pinkPoints)
                                       .Append("</color> | <color=#808080FF>")
                                       .Append(Tr.Get(TrKey.SerialKiller))
                                       .Append(serialKillerPoints)
                                       .Append("</color>");
                    battleRoyalePointCounter = sb.ToString();
                }
                else
                {
                    var sb = new StringBuilder(Tr.Get(TrKey.BattleRoyaleGoal))
                                       .Append(requiredScore)
                                       .Append(" | <color=#39FF14FF>")
                                       .Append(Tr.Get(TrKey.BattleRoyaleLimeTeam))
                                       .Append(limePoints)
                                       .Append("</color> | <color=#F2BEFFFF>")
                                       .Append(Tr.Get(TrKey.BattleRoyalePinkTeam))
                                       .Append(pinkPoints)
                                       .Append("</color>");
                    battleRoyalePointCounter = sb.ToString();
                }
                break;
        }

        switch (GameOptionsManager.Instance.currentGameOptions.MapId)
        {
            case 0:
                if (battleRoyaleSenseiMapMode)
                {
                    soloPlayersSpawnPositions.Add(new Vector3(-6.8f, 11f, 0f)); // secutiry
                    soloPlayersSpawnPositions.Add(new Vector3(10f, -2.15f, 0f)); // wep shields
                    soloPlayersSpawnPositions.Add(new Vector3(-19f, 5.5f, 0f)); // upper engine
                    soloPlayersSpawnPositions.Add(new Vector3(7f, -14f, 0f)); // nav
                    soloPlayersSpawnPositions.Add(new Vector3(-6.25f, -1.5f, 0f)); // medbey
                    soloPlayersSpawnPositions.Add(new Vector3(-18.85f, -8f, 0f)); // lower engine
                    soloPlayersSpawnPositions.Add(new Vector3(4.75f, -8.5f, 0f)); // admin
                    soloPlayersSpawnPositions.Add(new Vector3(-0.75f, -1.5f, 0f)); // cafete
                    soloPlayersSpawnPositions.Add(new Vector3(-12f, 7f, 0f)); // empty room
                    soloPlayersSpawnPositions.Add(new Vector3(-5.5f, -13.15f, 0f)); // elect
                    soloPlayersSpawnPositions.Add(new Vector3(6.75f, 4.75f, 0f)); // comms
                    soloPlayersSpawnPositions.Add(new Vector3(-8.4f, -0.4f, 0f)); // o2
                    soloPlayersSpawnPositions.Add(new Vector3(-12f, -12.75f, 0f)); // storage
                    soloPlayersSpawnPositions.Add(new Vector3(-3.75f, 5f, 0f)); // hidden cafeteria room
                    soloPlayersSpawnPositions.Add(new Vector3(-19.5f, -1.5f, 0f)); // reactor
                }
                else if (battleRoyaleDleksMap)
                {
                    soloPlayersSpawnPositions.Add(new Vector3(8.75f, -8.5f, 0f)); // elec
                    soloPlayersSpawnPositions.Add(new Vector3(9.15f, -4.75f, 0f)); // medbey
                    soloPlayersSpawnPositions.Add(new Vector3(-6f, -3.5f, 0f)); // o2
                    soloPlayersSpawnPositions.Add(new Vector3(-6.25f, -8.5f, 0f)); // admin
                    soloPlayersSpawnPositions.Add(new Vector3(17.75f, 2.5f, 0f)); // upper engine
                    soloPlayersSpawnPositions.Add(new Vector3(-2.75f, -15.25f, 0f)); // comms
                    soloPlayersSpawnPositions.Add(new Vector3(17.75f, -13.25f, 0f)); // lower engine
                    soloPlayersSpawnPositions.Add(new Vector3(-9.75f, 2.75f, 0f)); // weapons
                    soloPlayersSpawnPositions.Add(new Vector3(13.5f, -6.75f, 0f)); // seguridad
                    soloPlayersSpawnPositions.Add(new Vector3(-9.5f, -12.25f, 0f)); // shields
                    soloPlayersSpawnPositions.Add(new Vector3(21.5f, -2.5f, 0f)); // reactor
                    soloPlayersSpawnPositions.Add(new Vector3(-16.5f, -3.5f, 0f)); // nav
                    soloPlayersSpawnPositions.Add(new Vector3(0.75f, 5.25f, 0f)); // caftereria upper
                    soloPlayersSpawnPositions.Add(new Vector3(1.75f, -16f, 0f)); // stoage
                    soloPlayersSpawnPositions.Add(new Vector3(0.75f, -2.75f, 0f)); // caftereria lower
                }
                else
                {
                    soloPlayersSpawnPositions.Add(new Vector3(-8.75f, -8.5f, 0f)); // elec
                    soloPlayersSpawnPositions.Add(new Vector3(-9.15f, -4.75f, 0f)); // medbey
                    soloPlayersSpawnPositions.Add(new Vector3(6f, -3.5f, 0f)); // o2
                    soloPlayersSpawnPositions.Add(new Vector3(6.25f, -8.5f, 0f)); // admin
                    soloPlayersSpawnPositions.Add(new Vector3(-17.75f, 2.5f, 0f)); // upper engine
                    soloPlayersSpawnPositions.Add(new Vector3(2.75f, -15.25f, 0f)); // comms
                    soloPlayersSpawnPositions.Add(new Vector3(-17.75f, -13.25f, 0f)); // lower engine
                    soloPlayersSpawnPositions.Add(new Vector3(9.75f, 2.75f, 0f)); // weapons
                    soloPlayersSpawnPositions.Add(new Vector3(-13.5f, -6.75f, 0f)); // seguridad
                    soloPlayersSpawnPositions.Add(new Vector3(9.5f, -12.25f, 0f)); // shields
                    soloPlayersSpawnPositions.Add(new Vector3(-21.5f, -2.5f, 0f)); // reactor
                    soloPlayersSpawnPositions.Add(new Vector3(16.5f, -3.5f, 0f)); // nav
                    soloPlayersSpawnPositions.Add(new Vector3(-0.75f, 5.25f, 0f)); // caftereria upper
                    soloPlayersSpawnPositions.Add(new Vector3(-1.75f, -16f, 0f)); // stoage
                    soloPlayersSpawnPositions.Add(new Vector3(-0.75f, -2.75f, 0f)); // caftereria lower
                }
                break;
            case 1:
                soloPlayersSpawnPositions.Add(new Vector3(19.5f, 4.65f, 0f)); // storage
                soloPlayersSpawnPositions.Add(new Vector3(11.25f, 10.5f, 0f)); // lab
                soloPlayersSpawnPositions.Add(new Vector3(14.75f, 20.5f, 0f)); // office
                soloPlayersSpawnPositions.Add(new Vector3(15.5f, -0.5f, 0f)); // medbey
                soloPlayersSpawnPositions.Add(new Vector3(27.5f, -1.75f, 0f)); // balcony
                soloPlayersSpawnPositions.Add(new Vector3(2.5f, 13.15f, 0f)); // reactor
                soloPlayersSpawnPositions.Add(new Vector3(15.5f, 4f, 0f)); // comms
                soloPlayersSpawnPositions.Add(new Vector3(21f, 20.5f, 0f)); // admin
                soloPlayersSpawnPositions.Add(new Vector3(27, 4.75f, 0f)); // cafeteria
                soloPlayersSpawnPositions.Add(new Vector3(6.15f, 6.5f, 0f)); // decom
                soloPlayersSpawnPositions.Add(new Vector3(5f, -1.25f, 0f)); // long hallway
                soloPlayersSpawnPositions.Add(new Vector3(16.15f, 24.25f, 0f)); // greenhouse
                soloPlayersSpawnPositions.Add(new Vector3(-4.35f, 3.25f, 0f)); // launch pad
                soloPlayersSpawnPositions.Add(new Vector3(9.5f, 1.25f, 0f)); // lockroom
                soloPlayersSpawnPositions.Add(new Vector3(18f, 11.5f, 0f)); // midway
                break;
            case 2:
                soloPlayersSpawnPositions.Add(new Vector3(9.75f, -12.15f, 0f)); // elec
                soloPlayersSpawnPositions.Add(new Vector3(40.5f, -7.75f, 0f)); // right lab
                soloPlayersSpawnPositions.Add(new Vector3(11f, -23f, 0f)); // weapons
                soloPlayersSpawnPositions.Add(new Vector3(36.5f, -21.5f, 0f)); // specifmen
                soloPlayersSpawnPositions.Add(new Vector3(1f, -16.5f, 0f)); // up o2
                soloPlayersSpawnPositions.Add(new Vector3(27.75f, -7.5f, 0f)); // left lab
                soloPlayersSpawnPositions.Add(new Vector3(26.5f, -17f, 0f)); // right office
                soloPlayersSpawnPositions.Add(new Vector3(12.5f, -16.5f, 0f)); // comms
                soloPlayersSpawnPositions.Add(new Vector3(16.75f, -1f, 0f)); // launch pad
                soloPlayersSpawnPositions.Add(new Vector3(22f, -25.15f, 0f)); // admin
                soloPlayersSpawnPositions.Add(new Vector3(1.75f, -23.75f, 0f)); // low o2
                soloPlayersSpawnPositions.Add(new Vector3(17.15f, -17f, 0f)); // left office
                soloPlayersSpawnPositions.Add(new Vector3(3.75f, -12f, 0f)); // secutiry
                soloPlayersSpawnPositions.Add(new Vector3(20.75f, -12f, 0f)); // storage
                soloPlayersSpawnPositions.Add(new Vector3(1.5f, -20f, 0f)); // mid o2
                break;
            case 3:
                soloPlayersSpawnPositions.Add(new Vector3(8.75f, -8.5f, 0f)); // elec
                soloPlayersSpawnPositions.Add(new Vector3(9.15f, -4.75f, 0f)); // medbey
                soloPlayersSpawnPositions.Add(new Vector3(-6f, -3.5f, 0f)); // o2
                soloPlayersSpawnPositions.Add(new Vector3(-6.25f, -8.5f, 0f)); // admin
                soloPlayersSpawnPositions.Add(new Vector3(17.75f, 2.5f, 0f)); // upper engine
                soloPlayersSpawnPositions.Add(new Vector3(-2.75f, -15.25f, 0f)); // comms
                soloPlayersSpawnPositions.Add(new Vector3(17.75f, -13.25f, 0f)); // lower engine
                soloPlayersSpawnPositions.Add(new Vector3(-9.75f, 2.75f, 0f)); // weapons
                soloPlayersSpawnPositions.Add(new Vector3(13.5f, -6.75f, 0f)); // seguridad
                soloPlayersSpawnPositions.Add(new Vector3(-9.5f, -12.25f, 0f)); // shields
                soloPlayersSpawnPositions.Add(new Vector3(21.5f, -2.5f, 0f)); // reactor
                soloPlayersSpawnPositions.Add(new Vector3(-16.5f, -3.5f, 0f)); // nav
                soloPlayersSpawnPositions.Add(new Vector3(0.75f, 5.25f, 0f)); // caftereria upper
                soloPlayersSpawnPositions.Add(new Vector3(1.75f, -16f, 0f)); // stoage
                soloPlayersSpawnPositions.Add(new Vector3(0.75f, -2.75f, 0f)); // caftereria lower
                break;
            case 4:
                soloPlayersSpawnPositions.Add(new Vector3(-0.5f, -1, 0f));
                break;
            case 5:
                soloPlayersSpawnPositions.Add(new Vector3(-18.25f, 5f, 0f));
                soloPlayersSpawnPositions.Add(new Vector3(-22.65f, -7.15f, 0f));
                soloPlayersSpawnPositions.Add(new Vector3(2, 4.35f, 0f));
                soloPlayersSpawnPositions.Add(new Vector3(-3.15f, -10.5f, 0f));
                soloPlayersSpawnPositions.Add(new Vector3(23.7f, -7.8f, 0f));
                soloPlayersSpawnPositions.Add(new Vector3(-4.75f, -1.75f, 0f));
                soloPlayersSpawnPositions.Add(new Vector3(8f, -10f, 0f));
                soloPlayersSpawnPositions.Add(new Vector3(7f, 1.75f, 0f));
                soloPlayersSpawnPositions.Add(new Vector3(13.25f, 10, 0f));
                soloPlayersSpawnPositions.Add(new Vector3(22.3f, 3.3f, 0f));
                soloPlayersSpawnPositions.Add(new Vector3(20.5f, 7.35f, 0f));
                soloPlayersSpawnPositions.Add(new Vector3(24.15f, 14.45f, 0f));
                soloPlayersSpawnPositions.Add(new Vector3(-16.12f, 0.7f, 0f));
                soloPlayersSpawnPositions.Add(new Vector3(1.65f, -1.5f, 0f));
                soloPlayersSpawnPositions.Add(new Vector3(10.5f, -12, 0f));
                break;
            case 6:
                soloPlayersSpawnPositions.Add(new Vector3(3.75f, -26.5f, 0f));
                break;
        }
    }

    public static PlayerControl GetShotPlayer(float shotSize, float effectiveRange, int whichPlayerShot)
    {
        float playerMouse = 0;
        var originPlayer = new Vector2(0, 0);
        switch (whichPlayerShot)
        {
            case 1:
                playerMouse = soloPlayer01mouseAngle;
                originPlayer = soloPlayer01.GetTruePosition();
                break;
            case 2:
                playerMouse = soloPlayer02mouseAngle;
                originPlayer = soloPlayer02.GetTruePosition();
                break;
            case 3:
                playerMouse = soloPlayer03mouseAngle;
                originPlayer = soloPlayer03.GetTruePosition();
                break;
            case 4:
                playerMouse = soloPlayer04mouseAngle;
                originPlayer = soloPlayer04.GetTruePosition();
                break;
            case 5:
                playerMouse = soloPlayer05mouseAngle;
                originPlayer = soloPlayer05.GetTruePosition();
                break;
            case 6:
                playerMouse = soloPlayer06mouseAngle;
                originPlayer = soloPlayer06.GetTruePosition();
                break;
            case 7:
                playerMouse = soloPlayer07mouseAngle;
                originPlayer = soloPlayer07.GetTruePosition();
                break;
            case 8:
                playerMouse = soloPlayer08mouseAngle;
                originPlayer = soloPlayer08.GetTruePosition();
                break;
            case 9:
                playerMouse = soloPlayer09mouseAngle;
                originPlayer = soloPlayer09.GetTruePosition();
                break;
            case 10:
                playerMouse = soloPlayer10mouseAngle;
                originPlayer = soloPlayer10.GetTruePosition();
                break;
            case 11:
                playerMouse = soloPlayer11mouseAngle;
                originPlayer = soloPlayer11.GetTruePosition();
                break;
            case 12:
                playerMouse = soloPlayer12mouseAngle;
                originPlayer = soloPlayer12.GetTruePosition();
                break;
            case 13:
                playerMouse = soloPlayer13mouseAngle;
                originPlayer = soloPlayer13.GetTruePosition();
                break;
            case 14:
                playerMouse = soloPlayer14mouseAngle;
                originPlayer = soloPlayer14.GetTruePosition();
                break;
            case 15:
                playerMouse = soloPlayer15mouseAngle;
                originPlayer = soloPlayer15.GetTruePosition();
                break;
        }
        PlayerControl result = null;
        float num = effectiveRange;
        Vector3 pos;
        float mouseAngle = playerMouse;
        foreach (PlayerControl player in soloPlayerTeam)
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
                if (!PhysicsHelpers.AnythingBetween(
                        originPlayer,
                        player.GetTruePosition(),
                        Constants.ShipOnlyMask,
                        false
                    ))
                {
                    result = player;
                }
            }
        }
        return result;
    }
    public static PlayerControl GetLimeShotPlayer(float shotSize, float effectiveRange, int whichPlayerShot)
    {
        float playerMouse = 0;
        var originPlayer = new Vector2(0, 0);
        switch (whichPlayerShot)
        {
            case 1:
                playerMouse = limePlayer01mouseAngle;
                originPlayer = limePlayer01.GetTruePosition();
                break;
            case 2:
                playerMouse = limePlayer02mouseAngle;
                originPlayer = limePlayer02.GetTruePosition();
                break;
            case 3:
                playerMouse = limePlayer03mouseAngle;
                originPlayer = limePlayer03.GetTruePosition();
                break;
            case 4:
                playerMouse = limePlayer04mouseAngle;
                originPlayer = limePlayer04.GetTruePosition();
                break;
            case 5:
                playerMouse = limePlayer05mouseAngle;
                originPlayer = limePlayer05.GetTruePosition();
                break;
            case 6:
                playerMouse = limePlayer06mouseAngle;
                originPlayer = limePlayer06.GetTruePosition();
                break;
            case 7:
                playerMouse = limePlayer07mouseAngle;
                originPlayer = limePlayer07.GetTruePosition();
                break;
        }
        PlayerControl result = null;
        float num = effectiveRange;
        Vector3 pos;
        float mouseAngle = playerMouse;
        foreach (PlayerControl player in pinkTeam)
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
                if (!PhysicsHelpers.AnythingBetween(
                        originPlayer,
                        player.GetTruePosition(),
                        Constants.ShipOnlyMask,
                        false
                    ))
                {
                    result = player;
                }
            }
        }
        foreach (PlayerControl player in serialKillerTeam)
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
                if (!PhysicsHelpers.AnythingBetween(
                        originPlayer,
                        player.GetTruePosition(),
                        Constants.ShipOnlyMask,
                        false
                    ))
                {
                    result = player;
                }
            }
        }
        return result;
    }
    public static PlayerControl GetPinkShotPlayer(float shotSize, float effectiveRange, int whichPlayerShot)
    {
        float playerMouse = 0;
        var originPlayer = new Vector2(0, 0);
        switch (whichPlayerShot)
        {
            case 1:
                playerMouse = pinkPlayer01mouseAngle;
                originPlayer = pinkPlayer01.GetTruePosition();
                break;
            case 2:
                playerMouse = pinkPlayer02mouseAngle;
                originPlayer = pinkPlayer02.GetTruePosition();
                break;
            case 3:
                playerMouse = pinkPlayer03mouseAngle;
                originPlayer = pinkPlayer03.GetTruePosition();
                break;
            case 4:
                playerMouse = pinkPlayer04mouseAngle;
                originPlayer = pinkPlayer04.GetTruePosition();
                break;
            case 5:
                playerMouse = pinkPlayer05mouseAngle;
                originPlayer = pinkPlayer05.GetTruePosition();
                break;
            case 6:
                playerMouse = pinkPlayer06mouseAngle;
                originPlayer = pinkPlayer06.GetTruePosition();
                break;
            case 7:
                playerMouse = pinkPlayer07mouseAngle;
                originPlayer = pinkPlayer07.GetTruePosition();
                break;
        }
        PlayerControl result = null;
        float num = effectiveRange;
        Vector3 pos;
        float mouseAngle = playerMouse;
        foreach (PlayerControl player in limeTeam)
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
                if (!PhysicsHelpers.AnythingBetween(
                        originPlayer,
                        player.GetTruePosition(),
                        Constants.ShipOnlyMask,
                        false
                    ))
                {
                    result = player;
                }
            }
        }
        foreach (PlayerControl player in serialKillerTeam)
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
                if (!PhysicsHelpers.AnythingBetween(
                        originPlayer,
                        player.GetTruePosition(),
                        Constants.ShipOnlyMask,
                        false
                    ))
                {
                    result = player;
                }
            }
        }
        return result;
    }
    public static PlayerControl GetSerialShootPlayer(float shotSize, float effectiveRange)
    {
        PlayerControl result = null;
        float num = effectiveRange;
        Vector3 pos;
        float mouseAngle = serialKillermouseAngle;
        foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId) continue;

            if (player.Data.IsDead) continue;

            pos = player.transform.position - PlayerControl.LocalPlayer.transform.position;
            pos = new Vector3(
                pos.x * MathF.Cos(mouseAngle) + pos.y * MathF.Sin(mouseAngle),
                pos.y * MathF.Cos(mouseAngle) - pos.x * MathF.Sin(mouseAngle));
            if (Math.Abs(pos.y) < shotSize && (!(pos.x < 0)) && pos.x < num)
            {
                num = pos.x;
                if (!PhysicsHelpers.AnythingBetween(
                        serialKiller.GetTruePosition(),
                        player.GetTruePosition(),
                        Constants.ShipOnlyMask,
                        false
                    ))
                {
                    result = player;
                }
            }
        }
        return result;
    }

    public static void CreateBR()
    {

        Vector3 serialKillerPos = new Vector3();
        Vector3 limeTeamPos = new Vector3();
        Vector3 pinkTeamPos = new Vector3();
        Vector3 limeTeamFloorPos = new Vector3();
        Vector3 pinkTeamFloorPos = new Vector3();

        switch (GameOptionsManager.Instance.currentGameOptions.MapId)
        {
            // Skeld / Custom Skeld
            case 0:
                if (RebuildUs.activatedSensei)
                {
                    serialKillerPos = new Vector3(-3.65f, 5f, PlayerControl.LocalPlayer.transform.position.z);
                    limeTeamPos = new Vector3(-17.5f, -1.15f, PlayerControl.LocalPlayer.transform.position.z);
                    pinkTeamPos = new Vector3(7.7f, -0.95f, PlayerControl.LocalPlayer.transform.position.z);
                    limeTeamFloorPos = new Vector3(-17.5f, -1.15f, 0.5f);
                    pinkTeamFloorPos = new Vector3(7.7f, -0.95f, 0.5f);
                }
                else if (RebuildUs.activatedDleks)
                {
                    serialKillerPos = new Vector3(-6.35f, -7.5f, PlayerControl.LocalPlayer.transform.position.z);
                    limeTeamPos = new Vector3(17f, -5.5f, PlayerControl.LocalPlayer.transform.position.z);
                    pinkTeamPos = new Vector3(-12f, -4.75f, PlayerControl.LocalPlayer.transform.position.z);
                    limeTeamFloorPos = new Vector3(17f, -5.5f, 0.5f);
                    pinkTeamFloorPos = new Vector3(-12f, -4.75f, 0.5f);
                }
                else
                {
                    serialKillerPos = new Vector3(6.35f, -7.5f, PlayerControl.LocalPlayer.transform.position.z);
                    limeTeamPos = new Vector3(-17f, -5.5f, PlayerControl.LocalPlayer.transform.position.z);
                    pinkTeamPos = new Vector3(12f, -4.75f, PlayerControl.LocalPlayer.transform.position.z);
                    limeTeamFloorPos = new Vector3(-17f, -5.5f, 0.5f);
                    pinkTeamFloorPos = new Vector3(12f, -4.75f, 0.5f);
                }
                break;
            // Mira HQ
            case 1:
                serialKillerPos = new Vector3(16.25f, 24.5f, PlayerControl.LocalPlayer.transform.position.z);
                limeTeamPos = new Vector3(6.15f, 13.25f, PlayerControl.LocalPlayer.transform.position.z);
                pinkTeamPos = new Vector3(22.25f, 3f, PlayerControl.LocalPlayer.transform.position.z);
                limeTeamFloorPos = new Vector3(6.15f, 13.25f, 0.5f);
                pinkTeamFloorPos = new Vector3(22.25f, 3f, 0.5f);
                break;
            // Polus
            case 2:
                serialKillerPos = new Vector3(22.3f, -19.15f, PlayerControl.LocalPlayer.transform.position.z);
                limeTeamPos = new Vector3(2.35f, -23.75f, PlayerControl.LocalPlayer.transform.position.z);
                pinkTeamPos = new Vector3(36.35f, -8f, PlayerControl.LocalPlayer.transform.position.z);
                limeTeamFloorPos = new Vector3(2.35f, -23.75f, 0.5f);
                pinkTeamFloorPos = new Vector3(36.35f, -8f, 0.5f);
                break;
            // Dleks
            case 3:
                serialKillerPos = new Vector3(-6.35f, -7.5f, PlayerControl.LocalPlayer.transform.position.z);
                limeTeamPos = new Vector3(17f, -5.5f, PlayerControl.LocalPlayer.transform.position.z);
                pinkTeamPos = new Vector3(-12f, -4.75f, PlayerControl.LocalPlayer.transform.position.z);
                limeTeamFloorPos = new Vector3(17f, -5.5f, 0.5f);
                pinkTeamFloorPos = new Vector3(-12f, -4.75f, 0.5f);
                break;
            // Airship
            case 4:
                serialKillerPos = new Vector3(12.25f, 2f, PlayerControl.LocalPlayer.transform.position.z);
                limeTeamPos = new Vector3(-13.9f, -14.45f, PlayerControl.LocalPlayer.transform.position.z);
                pinkTeamPos = new Vector3(37.35f, -3.25f, PlayerControl.LocalPlayer.transform.position.z);
                limeTeamFloorPos = new Vector3(-13.9f, -14.45f, 0.5f);
                pinkTeamFloorPos = new Vector3(37.35f, -3.25f, 0.5f);
                break;
            // Fungle
            case 5:
                serialKillerPos = new Vector3(9.35f, -9.85f, PlayerControl.LocalPlayer.transform.position.z);
                limeTeamPos = new Vector3(1.6f, -1.65f, PlayerControl.LocalPlayer.transform.position.z);
                pinkTeamPos = new Vector3(6.75f, 2f, PlayerControl.LocalPlayer.transform.position.z);
                limeTeamFloorPos = new Vector3(1.6f, -1.65f, 0.5f);
                pinkTeamFloorPos = new Vector3(6.75f, 2, 0.5f);
                break;
            // Submerged
            case 6:
                serialKillerPos = new Vector3(5.75f, 31.25f, PlayerControl.LocalPlayer.transform.position.z);
                limeTeamPos = new Vector3(-12.25f, 18.5f, PlayerControl.LocalPlayer.transform.position.z);
                pinkTeamPos = new Vector3(-8.5f, -39.5f, PlayerControl.LocalPlayer.transform.position.z);
                limeTeamFloorPos = new Vector3(-12.25f, 18.5f, 0.03f);
                pinkTeamFloorPos = new Vector3(-8.5f, -39.5f, -0.01f);
                GameObject limeTeamFloorTwo = GameObject.Instantiate(AssetLoader.greenfloor, PlayerControl.LocalPlayer.transform.parent);
                limeTeamFloorTwo.name = "LimeTeamFloorTwo";
                limeTeamFloorTwo.transform.position = new Vector3(-14.5f, -34.35f, -0.01f);
                GameObject pinkTeamFloorTwo = GameObject.Instantiate(AssetLoader.redfloor, PlayerControl.LocalPlayer.transform.parent);
                pinkTeamFloorTwo.name = "PinkTeamFloorTwo";
                pinkTeamFloorTwo.transform.position = new Vector3(0f, 33.5f, 0.03f);
                BattleRoyale.serialKillerSpawns.Add(limeTeamFloorTwo);
                BattleRoyale.serialKillerSpawns.Add(pinkTeamFloorTwo);
                break;
        }

        if (BattleRoyale.matchType == 0)
        {
            foreach (PlayerControl soloPlayer in BattleRoyale.soloPlayerTeam)
            {
                soloPlayer.transform.position = new Vector3(BattleRoyale.soloPlayersSpawnPositions[howmanyBattleRoyaleplayers].x, BattleRoyale.soloPlayersSpawnPositions[howmanyBattleRoyaleplayers].y, PlayerControl.LocalPlayer.transform.position.z);
                howmanyBattleRoyaleplayers += 1;
            }
        }
        else
        {
            if (BattleRoyale.serialKiller != null)
            {
                BattleRoyale.serialKiller.MyPhysics.SetBodyType(PlayerBodyTypes.Seeker);
                BattleRoyale.serialKiller.transform.position = serialKillerPos;
            }

            foreach (PlayerControl player in BattleRoyale.limeTeam)
            {
                player.transform.position = limeTeamPos;
            }

            foreach (PlayerControl player in BattleRoyale.pinkTeam)
            {
                player.transform.position = pinkTeamPos;
            }
        }

        if (PlayerControl.LocalPlayer != null && !createdbattleroyale)
        {
            Helpers.ClearAllTasks(PlayerControl.LocalPlayer);

            if (BattleRoyale.matchType != 0)
            {
                GameObject limeteamfloor = GameObject.Instantiate(AssetLoader.greenfloor, PlayerControl.LocalPlayer.transform.parent);
                limeteamfloor.name = "limeteamfloor";
                limeteamfloor.transform.position = limeTeamFloorPos;
                GameObject pinkteamfloor = GameObject.Instantiate(AssetLoader.redfloor, PlayerControl.LocalPlayer.transform.parent);
                pinkteamfloor.name = "pinkteamfloor";
                pinkteamfloor.transform.position = pinkTeamFloorPos;
                BattleRoyale.serialKillerSpawns.Add(limeteamfloor);
                BattleRoyale.serialKillerSpawns.Add(pinkteamfloor);
            }

            createdbattleroyale = true;
        }
    }

    public static void OnMurderPlayerPostfix(PlayerControl __instance, PlayerControl target)
    {
        var brBody = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == target.PlayerId);
        brBody.transform.position = new Vector3(50, 50, 1);

        if (BattleRoyale.matchType == 2)
        {
            if (BattleRoyale.serialKiller != null && BattleRoyale.serialKiller.PlayerId == target.PlayerId)
            {
                BattleRoyale.serialKillerIsReviving = true;
                BattleRoyale.serialKiller.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                Helpers.alphaPlayer(true, BattleRoyale.serialKiller.PlayerId);
                HudManager.Instance.StartCoroutine(Effects.Lerp(MapSettings.gamemodeReviveTime, new Action<float>((p) =>
                {
                    if (p == 1f && BattleRoyale.serialKiller != null)
                    {
                        BattleRoyale.serialKillerIsReviving = false;
                        if (PlayerControl.AllPlayerControls.Count >= 11)
                        {
                            BattleRoyale.serialKillerLifes = BattleRoyale.fighterLifes * 3;
                        }
                        else
                        {
                            BattleRoyale.serialKillerLifes = BattleRoyale.fighterLifes * 2;
                        }
                        Helpers.alphaPlayer(false, BattleRoyale.serialKiller.PlayerId);
                        BattleRoyale.serialKiller.MyPhysics.SetBodyType(PlayerBodyTypes.Seeker);
                    }
                })));
                HudManager.Instance.StartCoroutine(Effects.Lerp(MapSettings.gamemodeReviveTime - MapSettings.gamemodeInvincibilityTime, new Action<float>((p) =>
                {
                    if (p == 1f && BattleRoyale.serialKiller != null)
                    {
                        BattleRoyale.serialKiller.Revive();
                        switch (GameOptionsManager.Instance.currentGameOptions.MapId)
                        {
                            // Skeld
                            case 0:
                                if (RebuildUs.activatedSensei)
                                {
                                    BattleRoyale.serialKiller.transform.position = new Vector3(-3.65f, 5f, PlayerControl.LocalPlayer.transform.position.z);
                                }
                                else if (RebuildUs.activatedDleks)
                                {
                                    BattleRoyale.serialKiller.transform.position = new Vector3(-6.35f, -7.5f, PlayerControl.LocalPlayer.transform.position.z);
                                }
                                else
                                {
                                    BattleRoyale.serialKiller.transform.position = new Vector3(6.35f, -7.5f, PlayerControl.LocalPlayer.transform.position.z);
                                }
                                break;
                            // MiraHQ
                            case 1:
                                BattleRoyale.serialKiller.transform.position = new Vector3(16.25f, 24.5f, PlayerControl.LocalPlayer.transform.position.z);
                                break;
                            // Polus
                            case 2:
                                BattleRoyale.serialKiller.transform.position = new Vector3(22.3f, -19.15f, PlayerControl.LocalPlayer.transform.position.z);
                                break;
                            // Dleks
                            case 3:
                                BattleRoyale.serialKiller.transform.position = new Vector3(-6.35f, -7.5f, PlayerControl.LocalPlayer.transform.position.z);
                                break;
                            // Airship
                            case 4:
                                BattleRoyale.serialKiller.transform.position = new Vector3(12.25f, 2f, PlayerControl.LocalPlayer.transform.position.z);
                                break;
                            // Fungle
                            case 5:
                                BattleRoyale.serialKiller.transform.position = new Vector3(9.35f, -9.85f, PlayerControl.LocalPlayer.transform.position.z);
                                break;
                            // Submerged
                            case 6:
                                if (BattleRoyale.serialKiller.transform.position.y > 0)
                                {
                                    BattleRoyale.serialKiller.transform.position = new Vector3(5.75f, 31.25f, BattleRoyale.serialKiller.transform.position.z);
                                }
                                else
                                {
                                    BattleRoyale.serialKiller.transform.position = new Vector3(-4.25f, -33.5f, BattleRoyale.serialKiller.transform.position.z);
                                }
                                break;
                        }
                        DeadPlayer deadPlayerEntry = GameHistory.DeadPlayers.Where(x => x.Player.PlayerId == target.PlayerId).FirstOrDefault();
                        if (brBody != null) UnityEngine.Object.Destroy(brBody.gameObject);
                        if (deadPlayerEntry != null) GameHistory.DeadPlayers.Remove(deadPlayerEntry);
                    }
                })));

            }

            foreach (PlayerControl player in BattleRoyale.limeTeam)
            {
                if (player.PlayerId == target.PlayerId)
                {

                    if (BattleRoyale.limePlayer01 != null && target.PlayerId == BattleRoyale.limePlayer01.PlayerId)
                    {
                        BattleRoyale.limePlayer01IsReviving = true;
                    }
                    else if (BattleRoyale.limePlayer02 != null && target.PlayerId == BattleRoyale.limePlayer02.PlayerId)
                    {
                        BattleRoyale.limePlayer02IsReviving = true;
                    }
                    else if (BattleRoyale.limePlayer03 != null && target.PlayerId == BattleRoyale.limePlayer03.PlayerId)
                    {
                        BattleRoyale.limePlayer03IsReviving = true;
                    }
                    else if (BattleRoyale.limePlayer04 != null && target.PlayerId == BattleRoyale.limePlayer04.PlayerId)
                    {
                        BattleRoyale.limePlayer04IsReviving = true;
                    }
                    else if (BattleRoyale.limePlayer05 != null && target.PlayerId == BattleRoyale.limePlayer05.PlayerId)
                    {
                        BattleRoyale.limePlayer05IsReviving = true;
                    }
                    else if (BattleRoyale.limePlayer06 != null && target.PlayerId == BattleRoyale.limePlayer06.PlayerId)
                    {
                        BattleRoyale.limePlayer06IsReviving = true;
                    }
                    else if (BattleRoyale.limePlayer07 != null && target.PlayerId == BattleRoyale.limePlayer07.PlayerId)
                    {
                        BattleRoyale.limePlayer07IsReviving = true;
                    }
                    Helpers.alphaPlayer(true, player.PlayerId);
                    HudManager.Instance.StartCoroutine(Effects.Lerp(MapSettings.gamemodeReviveTime, new Action<float>((p) =>
                    {
                        if (p == 1f && player != null)
                        {
                            if (BattleRoyale.limePlayer01 != null && target.PlayerId == BattleRoyale.limePlayer01.PlayerId)
                            {
                                BattleRoyale.limePlayer01IsReviving = false;
                                BattleRoyale.limePlayer01Lifes = BattleRoyale.fighterLifes;
                            }
                            else if (BattleRoyale.limePlayer02 != null && target.PlayerId == BattleRoyale.limePlayer02.PlayerId)
                            {
                                BattleRoyale.limePlayer02IsReviving = false;
                                BattleRoyale.limePlayer02Lifes = BattleRoyale.fighterLifes;
                            }
                            else if (BattleRoyale.limePlayer03 != null && target.PlayerId == BattleRoyale.limePlayer03.PlayerId)
                            {
                                BattleRoyale.limePlayer03IsReviving = false;
                                BattleRoyale.limePlayer03Lifes = BattleRoyale.fighterLifes;
                            }
                            else if (BattleRoyale.limePlayer04 != null && target.PlayerId == BattleRoyale.limePlayer04.PlayerId)
                            {
                                BattleRoyale.limePlayer04IsReviving = false;
                                BattleRoyale.limePlayer04Lifes = BattleRoyale.fighterLifes;
                            }
                            else if (BattleRoyale.limePlayer05 != null && target.PlayerId == BattleRoyale.limePlayer05.PlayerId)
                            {
                                BattleRoyale.limePlayer05IsReviving = false;
                                BattleRoyale.limePlayer05Lifes = BattleRoyale.fighterLifes;
                            }
                            else if (BattleRoyale.limePlayer06 != null && target.PlayerId == BattleRoyale.limePlayer06.PlayerId)
                            {
                                BattleRoyale.limePlayer06IsReviving = false;
                                BattleRoyale.limePlayer06Lifes = BattleRoyale.fighterLifes;
                            }
                            else if (BattleRoyale.limePlayer07 != null && target.PlayerId == BattleRoyale.limePlayer07.PlayerId)
                            {
                                BattleRoyale.limePlayer07IsReviving = false;
                                BattleRoyale.limePlayer07Lifes = BattleRoyale.fighterLifes;
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
                                        player.transform.position = new Vector3(-17.5f, -1.15f, player.transform.position.z);
                                    }
                                    else if (RebuildUs.activatedDleks)
                                    {
                                        player.transform.position = new Vector3(17f, -5.5f, player.transform.position.z);
                                    }
                                    else
                                    {
                                        player.transform.position = new Vector3(-17f, -5.5f, player.transform.position.z);
                                    }
                                    break;
                                // MiraHQ
                                case 1:
                                    player.transform.position = new Vector3(6.15f, 13.25f, player.transform.position.z);
                                    break;
                                // Polus
                                case 2:
                                    player.transform.position = new Vector3(2.35f, -23.75f, player.transform.position.z);
                                    break;
                                // Dleks
                                case 3:
                                    player.transform.position = new Vector3(17f, -5.5f, player.transform.position.z);
                                    break;
                                // Airship
                                case 4:
                                    player.transform.position = new Vector3(-13.9f, -14.45f, player.transform.position.z);
                                    break;
                                // Fungle
                                case 5:
                                    player.transform.position = new Vector3(1.6f, -1.65f, player.transform.position.z);
                                    break;
                                // Submerged
                                case 6:
                                    if (player.transform.position.y > 0)
                                    {
                                        player.transform.position = new Vector3(-12.25f, 18.5f, player.transform.position.z);
                                    }
                                    else
                                    {
                                        player.transform.position = new Vector3(-14.5f, -34.35f, player.transform.position.z);
                                    }
                                    break;
                            }
                            DeadPlayer deadPlayerEntry = GameHistory.DeadPlayers.Where(x => x.Player.PlayerId == target.PlayerId).FirstOrDefault();
                            if (brBody != null) UnityEngine.Object.Destroy(brBody.gameObject);
                            if (deadPlayerEntry != null) GameHistory.DeadPlayers.Remove(deadPlayerEntry);
                        }
                    })));

                }
            }
            foreach (PlayerControl player in BattleRoyale.pinkTeam)
            {
                if (player.PlayerId == target.PlayerId)
                {

                    if (BattleRoyale.pinkPlayer01 != null && target.PlayerId == BattleRoyale.pinkPlayer01.PlayerId)
                    {
                        BattleRoyale.pinkPlayer01IsReviving = true;
                    }
                    else if (BattleRoyale.pinkPlayer02 != null && target.PlayerId == BattleRoyale.pinkPlayer02.PlayerId)
                    {
                        BattleRoyale.pinkPlayer02IsReviving = true;
                    }
                    else if (BattleRoyale.pinkPlayer03 != null && target.PlayerId == BattleRoyale.pinkPlayer03.PlayerId)
                    {
                        BattleRoyale.pinkPlayer03IsReviving = true;
                    }
                    else if (BattleRoyale.pinkPlayer04 != null && target.PlayerId == BattleRoyale.pinkPlayer04.PlayerId)
                    {
                        BattleRoyale.pinkPlayer04IsReviving = true;
                    }
                    else if (BattleRoyale.pinkPlayer05 != null && target.PlayerId == BattleRoyale.pinkPlayer05.PlayerId)
                    {
                        BattleRoyale.pinkPlayer05IsReviving = true;
                    }
                    else if (BattleRoyale.pinkPlayer06 != null && target.PlayerId == BattleRoyale.pinkPlayer06.PlayerId)
                    {
                        BattleRoyale.pinkPlayer06IsReviving = true;
                    }
                    else if (BattleRoyale.pinkPlayer01 != null && target.PlayerId == BattleRoyale.pinkPlayer07.PlayerId)
                    {
                        BattleRoyale.pinkPlayer07IsReviving = true;
                    }
                    Helpers.alphaPlayer(true, player.PlayerId);
                    HudManager.Instance.StartCoroutine(Effects.Lerp(MapSettings.gamemodeReviveTime, new Action<float>((p) =>
                    {
                        if (p == 1f && player != null)
                        {
                            if (BattleRoyale.pinkPlayer01 != null && target.PlayerId == BattleRoyale.pinkPlayer01.PlayerId)
                            {
                                BattleRoyale.pinkPlayer01IsReviving = false;
                                BattleRoyale.pinkPlayer01Lifes = BattleRoyale.fighterLifes;
                            }
                            else if (BattleRoyale.pinkPlayer02 != null && target.PlayerId == BattleRoyale.pinkPlayer02.PlayerId)
                            {
                                BattleRoyale.pinkPlayer02IsReviving = false;
                                BattleRoyale.pinkPlayer02Lifes = BattleRoyale.fighterLifes;
                            }
                            else if (BattleRoyale.pinkPlayer03 != null && target.PlayerId == BattleRoyale.pinkPlayer03.PlayerId)
                            {
                                BattleRoyale.pinkPlayer03IsReviving = false;
                                BattleRoyale.pinkPlayer03Lifes = BattleRoyale.fighterLifes;
                            }
                            else if (BattleRoyale.pinkPlayer04 != null && target.PlayerId == BattleRoyale.pinkPlayer04.PlayerId)
                            {
                                BattleRoyale.pinkPlayer04IsReviving = false;
                                BattleRoyale.pinkPlayer04Lifes = BattleRoyale.fighterLifes;
                            }
                            else if (BattleRoyale.pinkPlayer05 != null && target.PlayerId == BattleRoyale.pinkPlayer05.PlayerId)
                            {
                                BattleRoyale.pinkPlayer05IsReviving = false;
                                BattleRoyale.pinkPlayer05Lifes = BattleRoyale.fighterLifes;
                            }
                            else if (BattleRoyale.pinkPlayer06 != null && target.PlayerId == BattleRoyale.pinkPlayer06.PlayerId)
                            {
                                BattleRoyale.pinkPlayer06IsReviving = false;
                                BattleRoyale.pinkPlayer06Lifes = BattleRoyale.fighterLifes;
                            }
                            else if (BattleRoyale.pinkPlayer01 != null && target.PlayerId == BattleRoyale.pinkPlayer07.PlayerId)
                            {
                                BattleRoyale.pinkPlayer07IsReviving = false;
                                BattleRoyale.pinkPlayer07Lifes = BattleRoyale.fighterLifes;
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
                                        player.transform.position = new Vector3(7.7f, -0.95f, player.transform.position.z);
                                    }
                                    else if (RebuildUs.activatedDleks)
                                    {
                                        player.transform.position = new Vector3(-12f, -4.75f, player.transform.position.z);
                                    }
                                    else
                                    {
                                        player.transform.position = new Vector3(12f, -4.75f, player.transform.position.z);
                                    }
                                    break;
                                // MiraHQ
                                case 1:
                                    player.transform.position = new Vector3(22.25f, 3f, player.transform.position.z);
                                    break;
                                // Polus
                                case 2:
                                    player.transform.position = new Vector3(36.35f, -8f, player.transform.position.z);
                                    break;
                                // Dleks
                                case 3:
                                    player.transform.position = new Vector3(-12f, -4.75f, player.transform.position.z);
                                    break;
                                // Airship
                                case 4:
                                    player.transform.position = new Vector3(37.35f, -3.25f, player.transform.position.z);
                                    break;
                                // Fungle
                                case 5:
                                    player.transform.position = new Vector3(6.75f, 2f, player.transform.position.z);
                                    break;
                                // Submerged
                                case 6:
                                    if (player.transform.position.y > 0)
                                    {
                                        player.transform.position = new Vector3(0f, 33.5f, player.transform.position.z);
                                    }
                                    else
                                    {
                                        player.transform.position = new Vector3(-8.5f, -39.5f, player.transform.position.z);
                                    }
                                    break;
                            }
                            DeadPlayer deadPlayerEntry = GameHistory.DeadPlayers.Where(x => x.Player.PlayerId == target.PlayerId).FirstOrDefault();
                            if (brBody != null) UnityEngine.Object.Destroy(brBody.gameObject);
                            if (deadPlayerEntry != null) GameHistory.DeadPlayers.Remove(deadPlayerEntry);
                        }
                    })));

                }
            }
        }
    }

    public static void battleRoyaleUpdate()
    {
        if (MapSettings.GameMode is not CustomGameMode.BattleRoyale) return;

        if (BattleRoyale.matchType == 0)
        {
            // If solo player disconnects, check number of players
            foreach (PlayerControl soloPlayer in BattleRoyale.soloPlayerTeam)
            {
                if (soloPlayer.Data.Disconnected)
                {

                    if (BattleRoyale.soloPlayer01 != null && soloPlayer.PlayerId == BattleRoyale.soloPlayer01.PlayerId)
                    {
                        BattleRoyale.soloPlayerTeam.Remove(BattleRoyale.soloPlayer01);
                    }
                    else if (BattleRoyale.soloPlayer02 != null && soloPlayer.PlayerId == BattleRoyale.soloPlayer02.PlayerId)
                    {
                        BattleRoyale.soloPlayerTeam.Remove(BattleRoyale.soloPlayer02);
                    }
                    else if (BattleRoyale.soloPlayer03 != null && soloPlayer.PlayerId == BattleRoyale.soloPlayer03.PlayerId)
                    {
                        BattleRoyale.soloPlayerTeam.Remove(BattleRoyale.soloPlayer03);
                    }
                    else if (BattleRoyale.soloPlayer04 != null && soloPlayer.PlayerId == BattleRoyale.soloPlayer04.PlayerId)
                    {
                        BattleRoyale.soloPlayerTeam.Remove(BattleRoyale.soloPlayer04);
                    }
                    else if (BattleRoyale.soloPlayer05 != null && soloPlayer.PlayerId == BattleRoyale.soloPlayer05.PlayerId)
                    {
                        BattleRoyale.soloPlayerTeam.Remove(BattleRoyale.soloPlayer05);
                    }
                    else if (BattleRoyale.soloPlayer06 != null && soloPlayer.PlayerId == BattleRoyale.soloPlayer06.PlayerId)
                    {
                        BattleRoyale.soloPlayerTeam.Remove(BattleRoyale.soloPlayer06);
                    }
                    else if (BattleRoyale.soloPlayer07 != null && soloPlayer.PlayerId == BattleRoyale.soloPlayer07.PlayerId)
                    {
                        BattleRoyale.soloPlayerTeam.Remove(BattleRoyale.soloPlayer07);
                    }
                    else if (BattleRoyale.soloPlayer08 != null && soloPlayer.PlayerId == BattleRoyale.soloPlayer08.PlayerId)
                    {
                        BattleRoyale.soloPlayerTeam.Remove(BattleRoyale.soloPlayer08);
                    }
                    else if (BattleRoyale.soloPlayer09 != null && soloPlayer.PlayerId == BattleRoyale.soloPlayer09.PlayerId)
                    {
                        BattleRoyale.soloPlayerTeam.Remove(BattleRoyale.soloPlayer09);
                    }
                    else if (BattleRoyale.soloPlayer10 != null && soloPlayer.PlayerId == BattleRoyale.soloPlayer10.PlayerId)
                    {
                        BattleRoyale.soloPlayerTeam.Remove(BattleRoyale.soloPlayer10);
                    }
                    else if (BattleRoyale.soloPlayer11 != null && soloPlayer.PlayerId == BattleRoyale.soloPlayer11.PlayerId)
                    {
                        BattleRoyale.soloPlayerTeam.Remove(BattleRoyale.soloPlayer11);
                    }
                    else if (BattleRoyale.soloPlayer12 != null && soloPlayer.PlayerId == BattleRoyale.soloPlayer12.PlayerId)
                    {
                        BattleRoyale.soloPlayerTeam.Remove(BattleRoyale.soloPlayer12);
                    }
                    else if (BattleRoyale.soloPlayer13 != null && soloPlayer.PlayerId == BattleRoyale.soloPlayer13.PlayerId)
                    {
                        BattleRoyale.soloPlayerTeam.Remove(BattleRoyale.soloPlayer13);
                    }
                    else if (BattleRoyale.soloPlayer14 != null && soloPlayer.PlayerId == BattleRoyale.soloPlayer14.PlayerId)
                    {
                        BattleRoyale.soloPlayerTeam.Remove(BattleRoyale.soloPlayer14);
                    }
                    else if (BattleRoyale.soloPlayer15 != null && soloPlayer.PlayerId == BattleRoyale.soloPlayer15.PlayerId)
                    {
                        BattleRoyale.soloPlayerTeam.Remove(BattleRoyale.soloPlayer15);
                    }

                    int soloPlayersAlives = 0;

                    foreach (PlayerControl remainPlayer in BattleRoyale.soloPlayerTeam)
                    {

                        if (!remainPlayer.Data.IsDead)
                        {
                            soloPlayersAlives += 1;
                        }
                    }

                    BattleRoyale.battleRoyalePointCounter = new StringBuilder(Tr.Get(TrKey.BattleRoyaleFighters)).Append("<color=#009F57FF>").Append(soloPlayersAlives).Append("</color>").ToString();

                    if (soloPlayersAlives <= 1)
                    {
                        BattleRoyale.triggerSoloWin = true;
                        GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleSoloWin, false);
                    }
                    break;
                }
            }
        }
        else
        {
            // lime Team disconnects
            foreach (PlayerControl limePlayer in BattleRoyale.limeTeam)
            {
                if (limePlayer.Data.Disconnected)
                {

                    if (BattleRoyale.limePlayer01 != null && limePlayer.PlayerId == BattleRoyale.limePlayer01.PlayerId)
                    {
                        BattleRoyale.limeTeam.Remove(BattleRoyale.limePlayer01);
                    }
                    else if (BattleRoyale.limePlayer02 != null && limePlayer.PlayerId == BattleRoyale.limePlayer02.PlayerId)
                    {
                        BattleRoyale.limeTeam.Remove(BattleRoyale.limePlayer02);
                    }
                    else if (BattleRoyale.limePlayer03 != null && limePlayer.PlayerId == BattleRoyale.limePlayer03.PlayerId)
                    {
                        BattleRoyale.limeTeam.Remove(BattleRoyale.limePlayer03);
                    }
                    else if (BattleRoyale.limePlayer04 != null && limePlayer.PlayerId == BattleRoyale.limePlayer04.PlayerId)
                    {
                        BattleRoyale.limeTeam.Remove(BattleRoyale.limePlayer04);
                    }
                    else if (BattleRoyale.limePlayer05 != null && limePlayer.PlayerId == BattleRoyale.limePlayer05.PlayerId)
                    {
                        BattleRoyale.limeTeam.Remove(BattleRoyale.limePlayer05);
                    }
                    else if (BattleRoyale.limePlayer06 != null && limePlayer.PlayerId == BattleRoyale.limePlayer06.PlayerId)
                    {
                        BattleRoyale.limeTeam.Remove(BattleRoyale.limePlayer06);
                    }
                    else if (BattleRoyale.limePlayer07 != null && limePlayer.PlayerId == BattleRoyale.limePlayer07.PlayerId)
                    {
                        BattleRoyale.limeTeam.Remove(BattleRoyale.limePlayer07);
                    }

                    int limePlayersAlive = 0;

                    foreach (PlayerControl remainingLimePlayer in BattleRoyale.limeTeam)
                    {

                        if (!remainingLimePlayer.Data.IsDead)
                        {
                            limePlayersAlive += 1;
                        }
                    }

                    int pinkPlayersAlive = 0;

                    foreach (PlayerControl remainingPinkPlayer in BattleRoyale.pinkTeam)
                    {

                        if (!remainingPinkPlayer.Data.IsDead)
                        {
                            pinkPlayersAlive += 1;
                        }
                    }

                    if (BattleRoyale.serialKiller != null)
                    {

                        int serialKillerAlive = 0;

                        foreach (PlayerControl serialKiller in BattleRoyale.serialKillerTeam)
                        {

                            if (!serialKiller.Data.IsDead)
                            {
                                serialKillerAlive += 1;
                            }
                        }

                        if (BattleRoyale.matchType == 1)
                        {
                            var sb = new StringBuilder(Tr.Get(TrKey.BattleRoyaleLimeTeam))
                                               .Append("<color=#39FF14FF>")
                                               .Append(limePlayersAlive)
                                               .Append("</color> | ")
                                               .Append(Tr.Get(TrKey.BattleRoyalePinkTeam))
                                               .Append("<color=#F2BEFFFF>")
                                               .Append(pinkPlayersAlive)
                                               .Append("</color> | ")
                                               .Append(Tr.Get(TrKey.BattleRoyaleSerialKiller))
                                               .Append("<color=#808080FF>")
                                               .Append(serialKillerAlive)
                                               .Append("</color>");
                            BattleRoyale.battleRoyalePointCounter = sb.ToString();
                            if (limePlayersAlive <= 0 && pinkPlayersAlive <= 0 && !BattleRoyale.serialKiller.Data.IsDead)
                            {
                                BattleRoyale.triggerSerialKillerWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleSerialKillerWin, false);
                            }
                            else if (pinkPlayersAlive <= 0 && BattleRoyale.serialKiller.Data.IsDead)
                            {
                                BattleRoyale.triggerLimeTeamWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleLimeTeamWin, false);
                            }
                            else if (limePlayersAlive <= 0 && BattleRoyale.serialKiller.Data.IsDead)
                            {
                                BattleRoyale.triggerPinkTeamWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyalePinkTeamWin, false);
                            }
                        }
                    }
                    else
                    {
                        if (BattleRoyale.matchType == 1)
                        {
                            var sb = new StringBuilder(Tr.Get(TrKey.BattleRoyaleLimeTeam))
                                               .Append("<color=#39FF14FF>")
                                               .Append(limePlayersAlive)
                                               .Append("</color> | ")
                                               .Append(Tr.Get(TrKey.BattleRoyalePinkTeam))
                                               .Append("<color=#F2BEFFFF>")
                                               .Append(pinkPlayersAlive)
                                               .Append("</color>");
                            BattleRoyale.battleRoyalePointCounter = sb.ToString();
                            if (pinkPlayersAlive <= 0)
                            {
                                BattleRoyale.triggerLimeTeamWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleLimeTeamWin, false);
                            }
                            else if (limePlayersAlive <= 0)
                            {
                                BattleRoyale.triggerPinkTeamWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyalePinkTeamWin, false);
                            }
                        }
                    }
                    break;
                }
            }
            // Pink Team disconnects
            foreach (PlayerControl pinkPlayer in BattleRoyale.pinkTeam)
            {
                if (pinkPlayer.Data.Disconnected)
                {

                    if (BattleRoyale.pinkPlayer01 != null && pinkPlayer.PlayerId == BattleRoyale.pinkPlayer01.PlayerId)
                    {
                        BattleRoyale.pinkTeam.Remove(BattleRoyale.pinkPlayer01);
                    }
                    else if (BattleRoyale.pinkPlayer02 != null && pinkPlayer.PlayerId == BattleRoyale.pinkPlayer02.PlayerId)
                    {
                        BattleRoyale.pinkTeam.Remove(BattleRoyale.pinkPlayer02);
                    }
                    else if (BattleRoyale.pinkPlayer03 != null && pinkPlayer.PlayerId == BattleRoyale.pinkPlayer03.PlayerId)
                    {
                        BattleRoyale.pinkTeam.Remove(BattleRoyale.pinkPlayer03);
                    }
                    else if (BattleRoyale.pinkPlayer04 != null && pinkPlayer.PlayerId == BattleRoyale.pinkPlayer04.PlayerId)
                    {
                        BattleRoyale.pinkTeam.Remove(BattleRoyale.pinkPlayer04);
                    }
                    else if (BattleRoyale.pinkPlayer05 != null && pinkPlayer.PlayerId == BattleRoyale.pinkPlayer05.PlayerId)
                    {
                        BattleRoyale.pinkTeam.Remove(BattleRoyale.pinkPlayer05);
                    }
                    else if (BattleRoyale.pinkPlayer06 != null && pinkPlayer.PlayerId == BattleRoyale.pinkPlayer06.PlayerId)
                    {
                        BattleRoyale.pinkTeam.Remove(BattleRoyale.pinkPlayer06);
                    }
                    else if (BattleRoyale.pinkPlayer07 != null && pinkPlayer.PlayerId == BattleRoyale.pinkPlayer07.PlayerId)
                    {
                        BattleRoyale.pinkTeam.Remove(BattleRoyale.pinkPlayer07);
                    }

                    int limePlayersAlive = 0;

                    foreach (PlayerControl remainingLimePlayer in BattleRoyale.limeTeam)
                    {

                        if (!remainingLimePlayer.Data.IsDead)
                        {
                            limePlayersAlive += 1;
                        }
                    }

                    int pinkPlayersAlive = 0;

                    foreach (PlayerControl remainingPinkPlayer in BattleRoyale.pinkTeam)
                    {

                        if (!remainingPinkPlayer.Data.IsDead)
                        {
                            pinkPlayersAlive += 1;
                        }
                    }

                    if (BattleRoyale.serialKiller != null)
                    {

                        int serialKillerAlive = 0;

                        foreach (PlayerControl serialKiller in BattleRoyale.serialKillerTeam)
                        {

                            if (!serialKiller.Data.IsDead)
                            {
                                serialKillerAlive += 1;
                            }
                        }

                        if (BattleRoyale.matchType == 1)
                        {
                            var sb = new StringBuilder(Tr.Get(TrKey.BattleRoyaleLimeTeam))
                                               .Append("<color=#39FF14FF>")
                                               .Append(limePlayersAlive)
                                               .Append("</color> | ")
                                               .Append(Tr.Get(TrKey.BattleRoyalePinkTeam))
                                               .Append("<color=#F2BEFFFF>")
                                               .Append(pinkPlayersAlive)
                                               .Append("</color> | ")
                                               .Append(Tr.Get(TrKey.BattleRoyaleSerialKiller))
                                               .Append("<color=#808080FF>")
                                               .Append(serialKillerAlive)
                                               .Append("</color>");
                            BattleRoyale.battleRoyalePointCounter = sb.ToString();
                            if (limePlayersAlive <= 0 && pinkPlayersAlive <= 0 && !BattleRoyale.serialKiller.Data.IsDead)
                            {
                                BattleRoyale.triggerSerialKillerWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleSerialKillerWin, false);
                            }
                            else if (pinkPlayersAlive <= 0 && BattleRoyale.serialKiller.Data.IsDead)
                            {
                                BattleRoyale.triggerLimeTeamWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleLimeTeamWin, false);
                            }
                            else if (limePlayersAlive <= 0 && BattleRoyale.serialKiller.Data.IsDead)
                            {
                                BattleRoyale.triggerPinkTeamWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyalePinkTeamWin, false);
                            }
                        }
                    }
                    else
                    {
                        if (BattleRoyale.matchType == 1)
                        {
                            var sb = new StringBuilder(Tr.Get(TrKey.BattleRoyaleLimeTeam))
                                               .Append("<color=#39FF14FF>")
                                               .Append(limePlayersAlive)
                                               .Append("</color> | ")
                                               .Append(Tr.Get(TrKey.BattleRoyalePinkTeam))
                                               .Append("<color=#F2BEFFFF>")
                                               .Append(pinkPlayersAlive)
                                               .Append("</color>");
                            BattleRoyale.battleRoyalePointCounter = sb.ToString();
                            if (pinkPlayersAlive <= 0)
                            {
                                BattleRoyale.triggerLimeTeamWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleLimeTeamWin, false);
                            }
                            else if (limePlayersAlive <= 0)
                            {
                                BattleRoyale.triggerPinkTeamWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyalePinkTeamWin, false);
                            }
                        }
                    }
                    break;
                }
            }
            // Serial Killer disconnects
            if (BattleRoyale.serialKiller != null && BattleRoyale.serialKiller.Data.Disconnected)
            {

                BattleRoyale.serialKillerTeam.Remove(BattleRoyale.serialKiller);

                int limePlayersAlive = 0;

                foreach (PlayerControl limePlayer in BattleRoyale.limeTeam)
                {

                    if (!limePlayer.Data.IsDead)
                    {
                        limePlayersAlive += 1;
                    }
                }

                int pinkPlayersAlive = 0;

                foreach (PlayerControl pinkPlayer in BattleRoyale.pinkTeam)
                {

                    if (!pinkPlayer.Data.IsDead)
                    {
                        pinkPlayersAlive += 1;
                    }
                }

                int serialKillerAlive = 0;

                if (BattleRoyale.matchType == 1)
                {
                    var sb = new StringBuilder(Tr.Get(TrKey.BattleRoyaleLimeTeam))
                                       .Append("<color=#39FF14FF>")
                                       .Append(limePlayersAlive)
                                       .Append("</color> | ")
                                       .Append(Tr.Get(TrKey.BattleRoyalePinkTeam))
                                       .Append("<color=#F2BEFFFF>")
                                       .Append(pinkPlayersAlive)
                                       .Append("</color> | ")
                                       .Append(Tr.Get(TrKey.BattleRoyaleSerialKiller))
                                       .Append("<color=#808080FF>")
                                       .Append(serialKillerAlive)
                                       .Append("</color>");
                    BattleRoyale.battleRoyalePointCounter = sb.ToString();
                    if (pinkPlayersAlive <= 0)
                    {
                        BattleRoyale.triggerLimeTeamWin = true;
                        GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleLimeTeamWin, false);
                    }
                    else if (limePlayersAlive <= 0)
                    {
                        BattleRoyale.triggerPinkTeamWin = true;
                        GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyalePinkTeamWin, false);
                    }
                }
            }
        }
    }
}