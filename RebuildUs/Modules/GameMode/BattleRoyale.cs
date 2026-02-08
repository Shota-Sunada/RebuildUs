using Object = UnityEngine.Object;

namespace RebuildUs.Modules.GameMode;

public static partial class BattleRoyale
{
    public static Color SoloPlayerColor = new Color32(248, 205, 70, byte.MaxValue);
    public static Color LimeTeamColor = new Color32(0, 198, 66, byte.MaxValue);
    public static Color PinkTeamColor = new Color32(242, 190, 255, byte.MaxValue);
    public static Color SerialKillerColor = new Color32(128, 128, 128, byte.MaxValue);
    public static Color IntroColor = new Color32(0, 159, 87, byte.MaxValue);

    private static bool _createdbattleroyale;
    private static int _howmanyBattleRoyaleplayers;

    public static List<PlayerControl> SoloPlayerTeam = [];
    public static PlayerControl SoloPlayer01;
    public static float SoloPlayer01MouseAngle;
    public static GameObject SoloPlayer01Wep;
    public static float SoloPlayer01Lifes = 3;
    public static PlayerControl SoloPlayer02;
    public static float SoloPlayer02MouseAngle;
    public static GameObject SoloPlayer02Wep;
    public static float SoloPlayer02Lifes = 3;
    public static PlayerControl SoloPlayer03;
    public static float SoloPlayer03MouseAngle;
    public static GameObject SoloPlayer03Wep;
    public static float SoloPlayer03Lifes = 3;
    public static PlayerControl SoloPlayer04;
    public static float SoloPlayer04MouseAngle;
    public static GameObject SoloPlayer04Wep;
    public static float SoloPlayer04Lifes = 3;
    public static PlayerControl SoloPlayer05;
    public static float SoloPlayer05MouseAngle;
    public static GameObject SoloPlayer05Wep;
    public static float SoloPlayer05Lifes = 3;
    public static PlayerControl SoloPlayer06;
    public static float SoloPlayer06MouseAngle;
    public static GameObject SoloPlayer06Wep;
    public static float SoloPlayer06Lifes = 3;
    public static PlayerControl SoloPlayer07;
    public static float SoloPlayer07MouseAngle;
    public static GameObject SoloPlayer07Wep;
    public static float SoloPlayer07Lifes = 3;
    public static PlayerControl SoloPlayer08;
    public static float SoloPlayer08MouseAngle;
    public static GameObject SoloPlayer08Wep;
    public static float SoloPlayer08Lifes = 3;
    public static PlayerControl SoloPlayer09;
    public static float SoloPlayer09MouseAngle;
    public static GameObject SoloPlayer09Wep;
    public static float SoloPlayer09Lifes = 3;
    public static PlayerControl SoloPlayer10;
    public static float SoloPlayer10MouseAngle;
    public static GameObject SoloPlayer10Wep;
    public static float SoloPlayer10Lifes = 3;
    public static PlayerControl SoloPlayer11;
    public static float SoloPlayer11MouseAngle;
    public static GameObject SoloPlayer11Wep;
    public static float SoloPlayer11Lifes = 3;
    public static PlayerControl SoloPlayer12;
    public static float SoloPlayer12MouseAngle;
    public static GameObject SoloPlayer12Wep;
    public static float SoloPlayer12Lifes = 3;
    public static PlayerControl SoloPlayer13;
    public static float SoloPlayer13MouseAngle;
    public static GameObject SoloPlayer13Wep;
    public static float SoloPlayer13Lifes = 3;
    public static PlayerControl SoloPlayer14;
    public static float SoloPlayer14MouseAngle;
    public static GameObject SoloPlayer14Wep;
    public static float SoloPlayer14Lifes = 3;
    public static PlayerControl SoloPlayer15;
    public static float SoloPlayer15MouseAngle;
    public static GameObject SoloPlayer15Wep;
    public static float SoloPlayer15Lifes = 3;

    public static List<PlayerControl> LimeTeam = [];
    public static PlayerControl LimePlayer01;
    public static float LimePlayer01MouseAngle;
    public static GameObject LimePlayer01Wep;
    public static float LimePlayer01Lifes = 3;
    public static bool LimePlayer01IsReviving;
    public static PlayerControl LimePlayer02;
    public static float LimePlayer02MouseAngle;
    public static GameObject LimePlayer02Wep;
    public static float LimePlayer02Lifes = 3;
    public static bool LimePlayer02IsReviving;
    public static PlayerControl LimePlayer03;
    public static float LimePlayer03MouseAngle;
    public static GameObject LimePlayer03Wep;
    public static float LimePlayer03Lifes = 3;
    public static bool LimePlayer03IsReviving;
    public static PlayerControl LimePlayer04;
    public static float LimePlayer04MouseAngle;
    public static GameObject LimePlayer04Wep;
    public static float LimePlayer04Lifes = 3;
    public static bool LimePlayer04IsReviving;
    public static PlayerControl LimePlayer05;
    public static float LimePlayer05MouseAngle;
    public static GameObject LimePlayer05Wep;
    public static float LimePlayer05Lifes = 3;
    public static bool LimePlayer05IsReviving;
    public static PlayerControl LimePlayer06;
    public static float LimePlayer06MouseAngle;
    public static GameObject LimePlayer06Wep;
    public static float LimePlayer06Lifes = 3;
    public static bool LimePlayer06IsReviving;
    public static PlayerControl LimePlayer07;
    public static float LimePlayer07MouseAngle;
    public static GameObject LimePlayer07Wep;
    public static float LimePlayer07Lifes = 3;
    public static bool LimePlayer07IsReviving;

    public static List<PlayerControl> PinkTeam = [];
    public static PlayerControl PinkPlayer01;
    public static float PinkPlayer01MouseAngle;
    public static GameObject PinkPlayer01Wep;
    public static float PinkPlayer01Lifes = 3;
    public static bool PinkPlayer01IsReviving;
    public static PlayerControl PinkPlayer02;
    public static float PinkPlayer02MouseAngle;
    public static GameObject PinkPlayer02Wep;
    public static float PinkPlayer02Lifes = 3;
    public static bool PinkPlayer02IsReviving;
    public static PlayerControl PinkPlayer03;
    public static float PinkPlayer03MouseAngle;
    public static GameObject PinkPlayer03Wep;
    public static float PinkPlayer03Lifes = 3;
    public static bool PinkPlayer03IsReviving;
    public static PlayerControl PinkPlayer04;
    public static float PinkPlayer04MouseAngle;
    public static GameObject PinkPlayer04Wep;
    public static float PinkPlayer04Lifes = 3;
    public static bool PinkPlayer04IsReviving;
    public static PlayerControl PinkPlayer05;
    public static float PinkPlayer05MouseAngle;
    public static GameObject PinkPlayer05Wep;
    public static float PinkPlayer05Lifes = 3;
    public static bool PinkPlayer05IsReviving;
    public static PlayerControl PinkPlayer06;
    public static float PinkPlayer06MouseAngle;
    public static GameObject PinkPlayer06Wep;
    public static float PinkPlayer06Lifes = 3;
    public static bool PinkPlayer06IsReviving;
    public static PlayerControl PinkPlayer07;
    public static float PinkPlayer07MouseAngle;
    public static GameObject PinkPlayer07Wep;
    public static float PinkPlayer07Lifes = 3;
    public static bool PinkPlayer07IsReviving;

    public static List<PlayerControl> SerialKillerTeam = [];
    public static PlayerControl SerialKiller;
    public static float SerialKillermouseAngle;
    public static GameObject SerialKillerWep;
    public static float SerialKillerLifes = 3;
    public static float SerialKillerCooldown = 3;
    public static bool SerialKillerIsReviving;
    public static List<GameObject> SerialKillerSpawns = [];

    public static List<Vector3> SoloPlayersSpawnPositions = [];
    public static bool BattleRoyaleSenseiMapMode;
    public static bool BattleRoyaleDleksMap;

    public static float KillCooldown = 1;
    public static float FighterLifes = 3f;
    public static int MatchType;
    public static float RequiredScore;

    public static int LimePoints;
    public static int PinkPoints;
    public static int SerialKillerPoints;

    public static bool TriggerSoloWin;
    public static bool TriggerTimeWin;
    public static bool TriggerLimeTeamWin;
    public static bool TriggerPinkTeamWin;
    public static bool TriggerSerialKillerWin;
    public static bool TriggerDrawWin;

    public static string BattleRoyalePointCounter = "";

    public static void ClearAndReload()
    {
        FighterLifes = CustomOptionHolder.BattleRoyaleLifes.GetFloat();

        _howmanyBattleRoyaleplayers = 0;
        _createdbattleroyale = false;

        SoloPlayersSpawnPositions.Clear();
        SoloPlayerTeam.Clear();
        LimeTeam.Clear();
        PinkTeam.Clear();
        SerialKillerTeam.Clear();
        SoloPlayer01 = null;
        SoloPlayer01MouseAngle = 0;
        SoloPlayer01Wep = null;
        SoloPlayer01Lifes = FighterLifes;
        SoloPlayer02 = null;
        SoloPlayer02MouseAngle = 0;
        SoloPlayer02Wep = null;
        SoloPlayer02Lifes = FighterLifes;
        SoloPlayer03 = null;
        SoloPlayer03MouseAngle = 0;
        SoloPlayer03Wep = null;
        SoloPlayer03Lifes = FighterLifes;
        SoloPlayer04 = null;
        SoloPlayer04MouseAngle = 0;
        SoloPlayer04Wep = null;
        SoloPlayer04Lifes = FighterLifes;
        SoloPlayer05 = null;
        SoloPlayer05MouseAngle = 0;
        SoloPlayer05Wep = null;
        SoloPlayer05Lifes = FighterLifes;
        SoloPlayer06 = null;
        SoloPlayer06MouseAngle = 0;
        SoloPlayer06Wep = null;
        SoloPlayer06Lifes = FighterLifes;
        SoloPlayer07 = null;
        SoloPlayer07MouseAngle = 0;
        SoloPlayer07Wep = null;
        SoloPlayer07Lifes = FighterLifes;
        SoloPlayer08 = null;
        SoloPlayer08MouseAngle = 0;
        SoloPlayer08Wep = null;
        SoloPlayer08Lifes = FighterLifes;
        SoloPlayer09 = null;
        SoloPlayer09MouseAngle = 0;
        SoloPlayer09Wep = null;
        SoloPlayer09Lifes = FighterLifes;
        SoloPlayer10 = null;
        SoloPlayer10MouseAngle = 0;
        SoloPlayer10Wep = null;
        SoloPlayer10Lifes = FighterLifes;
        SoloPlayer11 = null;
        SoloPlayer11MouseAngle = 0;
        SoloPlayer11Wep = null;
        SoloPlayer11Lifes = FighterLifes;
        SoloPlayer12 = null;
        SoloPlayer12MouseAngle = 0;
        SoloPlayer12Wep = null;
        SoloPlayer12Lifes = FighterLifes;
        SoloPlayer13 = null;
        SoloPlayer13MouseAngle = 0;
        SoloPlayer13Wep = null;
        SoloPlayer13Lifes = FighterLifes;
        SoloPlayer14 = null;
        SoloPlayer14MouseAngle = 0;
        SoloPlayer14Wep = null;
        SoloPlayer14Lifes = FighterLifes;
        SoloPlayer15 = null;
        SoloPlayer15MouseAngle = 0;
        SoloPlayer15Wep = null;
        SoloPlayer15Lifes = FighterLifes;

        LimePlayer01 = null;
        LimePlayer01MouseAngle = 0;
        LimePlayer01Wep = null;
        LimePlayer01Lifes = FighterLifes;
        LimePlayer01IsReviving = false;
        LimePlayer02 = null;
        LimePlayer02MouseAngle = 0;
        LimePlayer02Wep = null;
        LimePlayer02Lifes = FighterLifes;
        LimePlayer02IsReviving = false;
        LimePlayer03 = null;
        LimePlayer03MouseAngle = 0;
        LimePlayer03Wep = null;
        LimePlayer03Lifes = FighterLifes;
        LimePlayer03IsReviving = false;
        LimePlayer04 = null;
        LimePlayer04MouseAngle = 0;
        LimePlayer04Wep = null;
        LimePlayer04Lifes = FighterLifes;
        LimePlayer04IsReviving = false;
        LimePlayer05 = null;
        LimePlayer05MouseAngle = 0;
        LimePlayer05Wep = null;
        LimePlayer05Lifes = FighterLifes;
        LimePlayer05IsReviving = false;
        LimePlayer06 = null;
        LimePlayer06MouseAngle = 0;
        LimePlayer06Wep = null;
        LimePlayer06Lifes = FighterLifes;
        LimePlayer06IsReviving = false;
        LimePlayer07 = null;
        LimePlayer07MouseAngle = 0;
        LimePlayer07Wep = null;
        LimePlayer07Lifes = FighterLifes;
        LimePlayer07IsReviving = false;
        PinkPlayer01 = null;
        PinkPlayer01MouseAngle = 0;
        PinkPlayer01Wep = null;
        PinkPlayer01Lifes = FighterLifes;
        PinkPlayer01IsReviving = false;
        PinkPlayer02 = null;
        PinkPlayer02MouseAngle = 0;
        PinkPlayer02Wep = null;
        PinkPlayer02Lifes = FighterLifes;
        PinkPlayer02IsReviving = false;
        PinkPlayer03 = null;
        PinkPlayer03MouseAngle = 0;
        PinkPlayer03Wep = null;
        PinkPlayer03Lifes = FighterLifes;
        PinkPlayer03IsReviving = false;
        PinkPlayer04 = null;
        PinkPlayer04MouseAngle = 0;
        PinkPlayer04Wep = null;
        PinkPlayer04Lifes = FighterLifes;
        PinkPlayer04IsReviving = false;
        PinkPlayer05 = null;
        PinkPlayer05MouseAngle = 0;
        PinkPlayer05Wep = null;
        PinkPlayer05Lifes = FighterLifes;
        PinkPlayer05IsReviving = false;
        PinkPlayer06 = null;
        PinkPlayer06MouseAngle = 0;
        PinkPlayer06Wep = null;
        PinkPlayer06Lifes = FighterLifes;
        PinkPlayer06IsReviving = false;
        PinkPlayer07 = null;
        PinkPlayer07MouseAngle = 0;
        PinkPlayer07Wep = null;
        PinkPlayer07Lifes = FighterLifes;
        PinkPlayer07IsReviving = false;
        SerialKiller = null;
        SerialKillermouseAngle = 0;
        SerialKillerWep = null;
        SerialKillerIsReviving = false;
        SerialKillerSpawns.Clear();

        KillCooldown = CustomOptionHolder.BattleRoyaleKillCooldown.GetFloat();
        BattleRoyaleSenseiMapMode = CustomOptionHolder.CustomSkeldMap.GetSelection() == 2;
        BattleRoyaleDleksMap = CustomOptionHolder.CustomSkeldMap.GetSelection() == 1;
        MatchType = CustomOptionHolder.BattleRoyaleMatchType.GetSelection();
        if (PlayerControl.AllPlayerControls.Count >= 11)
        {
            SerialKillerCooldown = KillCooldown / 3;
            SerialKillerLifes = FighterLifes * 3;
        }
        else
        {
            SerialKillerCooldown = KillCooldown / 2;
            SerialKillerLifes = FighterLifes * 2;
        }

        RequiredScore = CustomOptionHolder.BattleRoyaleScoreNeeded.GetFloat();

        LimePoints = 0;
        PinkPoints = 0;
        SerialKillerPoints = 0;

        TriggerSoloWin = false;
        TriggerTimeWin = false;
        TriggerLimeTeamWin = false;
        TriggerPinkTeamWin = false;
        TriggerSerialKillerWin = false;
        TriggerDrawWin = false;

        switch (MatchType)
        {
            case 0:
                BattleRoyalePointCounter = new StringBuilder(Tr.Get(TrKey.BattleRoyaleFighters)).Append("<color=#009F57FF>").Append(SoloPlayerTeam.Count).Append("</color>").ToString();
                break;
            case 1:
                if (SerialKiller != null)
                {
                    var sb = new StringBuilder(Tr.Get(TrKey.BattleRoyaleLimeTeam)).Append("<color=#39FF14FF>").Append(LimeTeam.Count).Append("</color> | ").Append(Tr.Get(TrKey.BattleRoyalePinkTeam)).Append("<color=#F2BEFFFF>").Append(PinkTeam.Count).Append("</color> | ").Append(Tr.Get(TrKey.SerialKiller)).Append("<color=#808080FF>").Append(SerialKillerTeam.Count).Append("</color>");
                    BattleRoyalePointCounter = sb.ToString();
                }
                else
                {
                    var sb = new StringBuilder(Tr.Get(TrKey.BattleRoyaleLimeTeam)).Append("<color=#39FF14FF>").Append(LimeTeam.Count).Append("</color> | ").Append(Tr.Get(TrKey.BattleRoyalePinkTeam)).Append("<color=#F2BEFFFF>").Append(PinkTeam.Count).Append("</color>");
                    BattleRoyalePointCounter = sb.ToString();
                }

                break;
            case 2:
                if (SerialKiller != null)
                {
                    var sb = new StringBuilder(Tr.Get(TrKey.BattleRoyaleGoal)).Append(RequiredScore).Append(" | <color=#39FF14FF>").Append(Tr.Get(TrKey.BattleRoyaleLimeTeam)).Append(LimePoints).Append("</color> | <color=#F2BEFFFF>").Append(Tr.Get(TrKey.BattleRoyalePinkTeam)).Append(PinkPoints).Append("</color> | <color=#808080FF>").Append(Tr.Get(TrKey.SerialKiller)).Append(SerialKillerPoints).Append("</color>");
                    BattleRoyalePointCounter = sb.ToString();
                }
                else
                {
                    var sb = new StringBuilder(Tr.Get(TrKey.BattleRoyaleGoal)).Append(RequiredScore).Append(" | <color=#39FF14FF>").Append(Tr.Get(TrKey.BattleRoyaleLimeTeam)).Append(LimePoints).Append("</color> | <color=#F2BEFFFF>").Append(Tr.Get(TrKey.BattleRoyalePinkTeam)).Append(PinkPoints).Append("</color>");
                    BattleRoyalePointCounter = sb.ToString();
                }

                break;
        }

        switch (GameOptionsManager.Instance.currentGameOptions.MapId)
        {
            case 0:
                if (BattleRoyaleSenseiMapMode)
                {
                    SoloPlayersSpawnPositions.Add(new(-6.8f, 11f, 0f)); // secutiry
                    SoloPlayersSpawnPositions.Add(new(10f, -2.15f, 0f)); // wep shields
                    SoloPlayersSpawnPositions.Add(new(-19f, 5.5f, 0f)); // upper engine
                    SoloPlayersSpawnPositions.Add(new(7f, -14f, 0f)); // nav
                    SoloPlayersSpawnPositions.Add(new(-6.25f, -1.5f, 0f)); // medbey
                    SoloPlayersSpawnPositions.Add(new(-18.85f, -8f, 0f)); // lower engine
                    SoloPlayersSpawnPositions.Add(new(4.75f, -8.5f, 0f)); // admin
                    SoloPlayersSpawnPositions.Add(new(-0.75f, -1.5f, 0f)); // cafete
                    SoloPlayersSpawnPositions.Add(new(-12f, 7f, 0f)); // empty room
                    SoloPlayersSpawnPositions.Add(new(-5.5f, -13.15f, 0f)); // elect
                    SoloPlayersSpawnPositions.Add(new(6.75f, 4.75f, 0f)); // comms
                    SoloPlayersSpawnPositions.Add(new(-8.4f, -0.4f, 0f)); // o2
                    SoloPlayersSpawnPositions.Add(new(-12f, -12.75f, 0f)); // storage
                    SoloPlayersSpawnPositions.Add(new(-3.75f, 5f, 0f)); // hidden cafeteria room
                    SoloPlayersSpawnPositions.Add(new(-19.5f, -1.5f, 0f)); // reactor
                }
                else if (BattleRoyaleDleksMap)
                {
                    SoloPlayersSpawnPositions.Add(new(8.75f, -8.5f, 0f)); // elec
                    SoloPlayersSpawnPositions.Add(new(9.15f, -4.75f, 0f)); // medbey
                    SoloPlayersSpawnPositions.Add(new(-6f, -3.5f, 0f)); // o2
                    SoloPlayersSpawnPositions.Add(new(-6.25f, -8.5f, 0f)); // admin
                    SoloPlayersSpawnPositions.Add(new(17.75f, 2.5f, 0f)); // upper engine
                    SoloPlayersSpawnPositions.Add(new(-2.75f, -15.25f, 0f)); // comms
                    SoloPlayersSpawnPositions.Add(new(17.75f, -13.25f, 0f)); // lower engine
                    SoloPlayersSpawnPositions.Add(new(-9.75f, 2.75f, 0f)); // weapons
                    SoloPlayersSpawnPositions.Add(new(13.5f, -6.75f, 0f)); // seguridad
                    SoloPlayersSpawnPositions.Add(new(-9.5f, -12.25f, 0f)); // shields
                    SoloPlayersSpawnPositions.Add(new(21.5f, -2.5f, 0f)); // reactor
                    SoloPlayersSpawnPositions.Add(new(-16.5f, -3.5f, 0f)); // nav
                    SoloPlayersSpawnPositions.Add(new(0.75f, 5.25f, 0f)); // caftereria upper
                    SoloPlayersSpawnPositions.Add(new(1.75f, -16f, 0f)); // stoage
                    SoloPlayersSpawnPositions.Add(new(0.75f, -2.75f, 0f)); // caftereria lower
                }
                else
                {
                    SoloPlayersSpawnPositions.Add(new(-8.75f, -8.5f, 0f)); // elec
                    SoloPlayersSpawnPositions.Add(new(-9.15f, -4.75f, 0f)); // medbey
                    SoloPlayersSpawnPositions.Add(new(6f, -3.5f, 0f)); // o2
                    SoloPlayersSpawnPositions.Add(new(6.25f, -8.5f, 0f)); // admin
                    SoloPlayersSpawnPositions.Add(new(-17.75f, 2.5f, 0f)); // upper engine
                    SoloPlayersSpawnPositions.Add(new(2.75f, -15.25f, 0f)); // comms
                    SoloPlayersSpawnPositions.Add(new(-17.75f, -13.25f, 0f)); // lower engine
                    SoloPlayersSpawnPositions.Add(new(9.75f, 2.75f, 0f)); // weapons
                    SoloPlayersSpawnPositions.Add(new(-13.5f, -6.75f, 0f)); // seguridad
                    SoloPlayersSpawnPositions.Add(new(9.5f, -12.25f, 0f)); // shields
                    SoloPlayersSpawnPositions.Add(new(-21.5f, -2.5f, 0f)); // reactor
                    SoloPlayersSpawnPositions.Add(new(16.5f, -3.5f, 0f)); // nav
                    SoloPlayersSpawnPositions.Add(new(-0.75f, 5.25f, 0f)); // caftereria upper
                    SoloPlayersSpawnPositions.Add(new(-1.75f, -16f, 0f)); // stoage
                    SoloPlayersSpawnPositions.Add(new(-0.75f, -2.75f, 0f)); // caftereria lower
                }

                break;
            case 1:
                SoloPlayersSpawnPositions.Add(new(19.5f, 4.65f, 0f)); // storage
                SoloPlayersSpawnPositions.Add(new(11.25f, 10.5f, 0f)); // lab
                SoloPlayersSpawnPositions.Add(new(14.75f, 20.5f, 0f)); // office
                SoloPlayersSpawnPositions.Add(new(15.5f, -0.5f, 0f)); // medbey
                SoloPlayersSpawnPositions.Add(new(27.5f, -1.75f, 0f)); // balcony
                SoloPlayersSpawnPositions.Add(new(2.5f, 13.15f, 0f)); // reactor
                SoloPlayersSpawnPositions.Add(new(15.5f, 4f, 0f)); // comms
                SoloPlayersSpawnPositions.Add(new(21f, 20.5f, 0f)); // admin
                SoloPlayersSpawnPositions.Add(new(27, 4.75f, 0f)); // cafeteria
                SoloPlayersSpawnPositions.Add(new(6.15f, 6.5f, 0f)); // decom
                SoloPlayersSpawnPositions.Add(new(5f, -1.25f, 0f)); // long hallway
                SoloPlayersSpawnPositions.Add(new(16.15f, 24.25f, 0f)); // greenhouse
                SoloPlayersSpawnPositions.Add(new(-4.35f, 3.25f, 0f)); // launch pad
                SoloPlayersSpawnPositions.Add(new(9.5f, 1.25f, 0f)); // lockroom
                SoloPlayersSpawnPositions.Add(new(18f, 11.5f, 0f)); // midway
                break;
            case 2:
                SoloPlayersSpawnPositions.Add(new(9.75f, -12.15f, 0f)); // elec
                SoloPlayersSpawnPositions.Add(new(40.5f, -7.75f, 0f)); // right lab
                SoloPlayersSpawnPositions.Add(new(11f, -23f, 0f)); // weapons
                SoloPlayersSpawnPositions.Add(new(36.5f, -21.5f, 0f)); // specifmen
                SoloPlayersSpawnPositions.Add(new(1f, -16.5f, 0f)); // up o2
                SoloPlayersSpawnPositions.Add(new(27.75f, -7.5f, 0f)); // left lab
                SoloPlayersSpawnPositions.Add(new(26.5f, -17f, 0f)); // right office
                SoloPlayersSpawnPositions.Add(new(12.5f, -16.5f, 0f)); // comms
                SoloPlayersSpawnPositions.Add(new(16.75f, -1f, 0f)); // launch pad
                SoloPlayersSpawnPositions.Add(new(22f, -25.15f, 0f)); // admin
                SoloPlayersSpawnPositions.Add(new(1.75f, -23.75f, 0f)); // low o2
                SoloPlayersSpawnPositions.Add(new(17.15f, -17f, 0f)); // left office
                SoloPlayersSpawnPositions.Add(new(3.75f, -12f, 0f)); // secutiry
                SoloPlayersSpawnPositions.Add(new(20.75f, -12f, 0f)); // storage
                SoloPlayersSpawnPositions.Add(new(1.5f, -20f, 0f)); // mid o2
                break;
            case 3:
                SoloPlayersSpawnPositions.Add(new(8.75f, -8.5f, 0f)); // elec
                SoloPlayersSpawnPositions.Add(new(9.15f, -4.75f, 0f)); // medbey
                SoloPlayersSpawnPositions.Add(new(-6f, -3.5f, 0f)); // o2
                SoloPlayersSpawnPositions.Add(new(-6.25f, -8.5f, 0f)); // admin
                SoloPlayersSpawnPositions.Add(new(17.75f, 2.5f, 0f)); // upper engine
                SoloPlayersSpawnPositions.Add(new(-2.75f, -15.25f, 0f)); // comms
                SoloPlayersSpawnPositions.Add(new(17.75f, -13.25f, 0f)); // lower engine
                SoloPlayersSpawnPositions.Add(new(-9.75f, 2.75f, 0f)); // weapons
                SoloPlayersSpawnPositions.Add(new(13.5f, -6.75f, 0f)); // seguridad
                SoloPlayersSpawnPositions.Add(new(-9.5f, -12.25f, 0f)); // shields
                SoloPlayersSpawnPositions.Add(new(21.5f, -2.5f, 0f)); // reactor
                SoloPlayersSpawnPositions.Add(new(-16.5f, -3.5f, 0f)); // nav
                SoloPlayersSpawnPositions.Add(new(0.75f, 5.25f, 0f)); // caftereria upper
                SoloPlayersSpawnPositions.Add(new(1.75f, -16f, 0f)); // stoage
                SoloPlayersSpawnPositions.Add(new(0.75f, -2.75f, 0f)); // caftereria lower
                break;
            case 4:
                SoloPlayersSpawnPositions.Add(new(-0.5f, -1, 0f));
                break;
            case 5:
                SoloPlayersSpawnPositions.Add(new(-18.25f, 5f, 0f));
                SoloPlayersSpawnPositions.Add(new(-22.65f, -7.15f, 0f));
                SoloPlayersSpawnPositions.Add(new(2, 4.35f, 0f));
                SoloPlayersSpawnPositions.Add(new(-3.15f, -10.5f, 0f));
                SoloPlayersSpawnPositions.Add(new(23.7f, -7.8f, 0f));
                SoloPlayersSpawnPositions.Add(new(-4.75f, -1.75f, 0f));
                SoloPlayersSpawnPositions.Add(new(8f, -10f, 0f));
                SoloPlayersSpawnPositions.Add(new(7f, 1.75f, 0f));
                SoloPlayersSpawnPositions.Add(new(13.25f, 10, 0f));
                SoloPlayersSpawnPositions.Add(new(22.3f, 3.3f, 0f));
                SoloPlayersSpawnPositions.Add(new(20.5f, 7.35f, 0f));
                SoloPlayersSpawnPositions.Add(new(24.15f, 14.45f, 0f));
                SoloPlayersSpawnPositions.Add(new(-16.12f, 0.7f, 0f));
                SoloPlayersSpawnPositions.Add(new(1.65f, -1.5f, 0f));
                SoloPlayersSpawnPositions.Add(new(10.5f, -12, 0f));
                break;
            case 6:
                SoloPlayersSpawnPositions.Add(new(3.75f, -26.5f, 0f));
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
                playerMouse = SoloPlayer01MouseAngle;
                originPlayer = SoloPlayer01.GetTruePosition();
                break;
            case 2:
                playerMouse = SoloPlayer02MouseAngle;
                originPlayer = SoloPlayer02.GetTruePosition();
                break;
            case 3:
                playerMouse = SoloPlayer03MouseAngle;
                originPlayer = SoloPlayer03.GetTruePosition();
                break;
            case 4:
                playerMouse = SoloPlayer04MouseAngle;
                originPlayer = SoloPlayer04.GetTruePosition();
                break;
            case 5:
                playerMouse = SoloPlayer05MouseAngle;
                originPlayer = SoloPlayer05.GetTruePosition();
                break;
            case 6:
                playerMouse = SoloPlayer06MouseAngle;
                originPlayer = SoloPlayer06.GetTruePosition();
                break;
            case 7:
                playerMouse = SoloPlayer07MouseAngle;
                originPlayer = SoloPlayer07.GetTruePosition();
                break;
            case 8:
                playerMouse = SoloPlayer08MouseAngle;
                originPlayer = SoloPlayer08.GetTruePosition();
                break;
            case 9:
                playerMouse = SoloPlayer09MouseAngle;
                originPlayer = SoloPlayer09.GetTruePosition();
                break;
            case 10:
                playerMouse = SoloPlayer10MouseAngle;
                originPlayer = SoloPlayer10.GetTruePosition();
                break;
            case 11:
                playerMouse = SoloPlayer11MouseAngle;
                originPlayer = SoloPlayer11.GetTruePosition();
                break;
            case 12:
                playerMouse = SoloPlayer12MouseAngle;
                originPlayer = SoloPlayer12.GetTruePosition();
                break;
            case 13:
                playerMouse = SoloPlayer13MouseAngle;
                originPlayer = SoloPlayer13.GetTruePosition();
                break;
            case 14:
                playerMouse = SoloPlayer14MouseAngle;
                originPlayer = SoloPlayer14.GetTruePosition();
                break;
            case 15:
                playerMouse = SoloPlayer15MouseAngle;
                originPlayer = SoloPlayer15.GetTruePosition();
                break;
        }

        PlayerControl result = null;
        var num = effectiveRange;
        Vector3 pos;
        var mouseAngle = playerMouse;
        foreach (var player in SoloPlayerTeam)
        {
            if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId) continue;

            if (player.Data.IsDead || player.inVent) continue;

            pos = player.transform.position - PlayerControl.LocalPlayer.transform.position;
            pos = new((pos.x * MathF.Cos(mouseAngle)) + (pos.y * MathF.Sin(mouseAngle)), (pos.y * MathF.Cos(mouseAngle)) - (pos.x * MathF.Sin(mouseAngle)));
            if (Math.Abs(pos.y) < shotSize && !(pos.x < 0) && pos.x < num)
            {
                num = pos.x;
                if (!PhysicsHelpers.AnythingBetween(originPlayer, player.GetTruePosition(), Constants.ShipOnlyMask, false))
                    result = player;
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
                playerMouse = LimePlayer01MouseAngle;
                originPlayer = LimePlayer01.GetTruePosition();
                break;
            case 2:
                playerMouse = LimePlayer02MouseAngle;
                originPlayer = LimePlayer02.GetTruePosition();
                break;
            case 3:
                playerMouse = LimePlayer03MouseAngle;
                originPlayer = LimePlayer03.GetTruePosition();
                break;
            case 4:
                playerMouse = LimePlayer04MouseAngle;
                originPlayer = LimePlayer04.GetTruePosition();
                break;
            case 5:
                playerMouse = LimePlayer05MouseAngle;
                originPlayer = LimePlayer05.GetTruePosition();
                break;
            case 6:
                playerMouse = LimePlayer06MouseAngle;
                originPlayer = LimePlayer06.GetTruePosition();
                break;
            case 7:
                playerMouse = LimePlayer07MouseAngle;
                originPlayer = LimePlayer07.GetTruePosition();
                break;
        }

        PlayerControl result = null;
        var num = effectiveRange;
        Vector3 pos;
        var mouseAngle = playerMouse;
        foreach (var player in PinkTeam)
        {
            if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId) continue;

            if (player.Data.IsDead || player.inVent) continue;

            pos = player.transform.position - PlayerControl.LocalPlayer.transform.position;
            pos = new((pos.x * MathF.Cos(mouseAngle)) + (pos.y * MathF.Sin(mouseAngle)), (pos.y * MathF.Cos(mouseAngle)) - (pos.x * MathF.Sin(mouseAngle)));
            if (Math.Abs(pos.y) < shotSize && !(pos.x < 0) && pos.x < num)
            {
                num = pos.x;
                if (!PhysicsHelpers.AnythingBetween(originPlayer, player.GetTruePosition(), Constants.ShipOnlyMask, false))
                    result = player;
            }
        }

        foreach (var player in SerialKillerTeam)
        {
            if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId) continue;

            if (player.Data.IsDead || player.inVent) continue;

            pos = player.transform.position - PlayerControl.LocalPlayer.transform.position;
            pos = new((pos.x * MathF.Cos(mouseAngle)) + (pos.y * MathF.Sin(mouseAngle)), (pos.y * MathF.Cos(mouseAngle)) - (pos.x * MathF.Sin(mouseAngle)));
            if (Math.Abs(pos.y) < shotSize && !(pos.x < 0) && pos.x < num)
            {
                num = pos.x;
                if (!PhysicsHelpers.AnythingBetween(originPlayer, player.GetTruePosition(), Constants.ShipOnlyMask, false))
                    result = player;
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
                playerMouse = PinkPlayer01MouseAngle;
                originPlayer = PinkPlayer01.GetTruePosition();
                break;
            case 2:
                playerMouse = PinkPlayer02MouseAngle;
                originPlayer = PinkPlayer02.GetTruePosition();
                break;
            case 3:
                playerMouse = PinkPlayer03MouseAngle;
                originPlayer = PinkPlayer03.GetTruePosition();
                break;
            case 4:
                playerMouse = PinkPlayer04MouseAngle;
                originPlayer = PinkPlayer04.GetTruePosition();
                break;
            case 5:
                playerMouse = PinkPlayer05MouseAngle;
                originPlayer = PinkPlayer05.GetTruePosition();
                break;
            case 6:
                playerMouse = PinkPlayer06MouseAngle;
                originPlayer = PinkPlayer06.GetTruePosition();
                break;
            case 7:
                playerMouse = PinkPlayer07MouseAngle;
                originPlayer = PinkPlayer07.GetTruePosition();
                break;
        }

        PlayerControl result = null;
        var num = effectiveRange;
        Vector3 pos;
        var mouseAngle = playerMouse;
        foreach (var player in LimeTeam)
        {
            if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId) continue;

            if (player.Data.IsDead || player.inVent) continue;

            pos = player.transform.position - PlayerControl.LocalPlayer.transform.position;
            pos = new((pos.x * MathF.Cos(mouseAngle)) + (pos.y * MathF.Sin(mouseAngle)), (pos.y * MathF.Cos(mouseAngle)) - (pos.x * MathF.Sin(mouseAngle)));
            if (Math.Abs(pos.y) < shotSize && !(pos.x < 0) && pos.x < num)
            {
                num = pos.x;
                if (!PhysicsHelpers.AnythingBetween(originPlayer, player.GetTruePosition(), Constants.ShipOnlyMask, false))
                    result = player;
            }
        }

        foreach (var player in SerialKillerTeam)
        {
            if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId) continue;

            if (player.Data.IsDead || player.inVent) continue;

            pos = player.transform.position - PlayerControl.LocalPlayer.transform.position;
            pos = new((pos.x * MathF.Cos(mouseAngle)) + (pos.y * MathF.Sin(mouseAngle)), (pos.y * MathF.Cos(mouseAngle)) - (pos.x * MathF.Sin(mouseAngle)));
            if (Math.Abs(pos.y) < shotSize && !(pos.x < 0) && pos.x < num)
            {
                num = pos.x;
                if (!PhysicsHelpers.AnythingBetween(originPlayer, player.GetTruePosition(), Constants.ShipOnlyMask, false))
                    result = player;
            }
        }

        return result;
    }

    public static PlayerControl GetSerialShootPlayer(float shotSize, float effectiveRange)
    {
        PlayerControl result = null;
        var num = effectiveRange;
        Vector3 pos;
        var mouseAngle = SerialKillermouseAngle;
        foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId) continue;

            if (player.Data.IsDead) continue;

            pos = player.transform.position - PlayerControl.LocalPlayer.transform.position;
            pos = new((pos.x * MathF.Cos(mouseAngle)) + (pos.y * MathF.Sin(mouseAngle)), (pos.y * MathF.Cos(mouseAngle)) - (pos.x * MathF.Sin(mouseAngle)));
            if (Math.Abs(pos.y) < shotSize && !(pos.x < 0) && pos.x < num)
            {
                num = pos.x;
                if (!PhysicsHelpers.AnythingBetween(SerialKiller.GetTruePosition(), player.GetTruePosition(), Constants.ShipOnlyMask, false))
                    result = player;
            }
        }

        return result;
    }

    public static void CreateBR()
    {
        var serialKillerPos = new Vector3();
        var limeTeamPos = new Vector3();
        var pinkTeamPos = new Vector3();
        var limeTeamFloorPos = new Vector3();
        var pinkTeamFloorPos = new Vector3();

        switch (GameOptionsManager.Instance.currentGameOptions.MapId)
        {
            // Skeld / Custom Skeld
            case 0:
                if (RebuildUs.ActivatedSensei)
                {
                    serialKillerPos = new(-3.65f, 5f, PlayerControl.LocalPlayer.transform.position.z);
                    limeTeamPos = new(-17.5f, -1.15f, PlayerControl.LocalPlayer.transform.position.z);
                    pinkTeamPos = new(7.7f, -0.95f, PlayerControl.LocalPlayer.transform.position.z);
                    limeTeamFloorPos = new(-17.5f, -1.15f, 0.5f);
                    pinkTeamFloorPos = new(7.7f, -0.95f, 0.5f);
                }
                else if (RebuildUs.ActivatedDleks)
                {
                    serialKillerPos = new(-6.35f, -7.5f, PlayerControl.LocalPlayer.transform.position.z);
                    limeTeamPos = new(17f, -5.5f, PlayerControl.LocalPlayer.transform.position.z);
                    pinkTeamPos = new(-12f, -4.75f, PlayerControl.LocalPlayer.transform.position.z);
                    limeTeamFloorPos = new(17f, -5.5f, 0.5f);
                    pinkTeamFloorPos = new(-12f, -4.75f, 0.5f);
                }
                else
                {
                    serialKillerPos = new(6.35f, -7.5f, PlayerControl.LocalPlayer.transform.position.z);
                    limeTeamPos = new(-17f, -5.5f, PlayerControl.LocalPlayer.transform.position.z);
                    pinkTeamPos = new(12f, -4.75f, PlayerControl.LocalPlayer.transform.position.z);
                    limeTeamFloorPos = new(-17f, -5.5f, 0.5f);
                    pinkTeamFloorPos = new(12f, -4.75f, 0.5f);
                }

                break;
            // Mira HQ
            case 1:
                serialKillerPos = new(16.25f, 24.5f, PlayerControl.LocalPlayer.transform.position.z);
                limeTeamPos = new(6.15f, 13.25f, PlayerControl.LocalPlayer.transform.position.z);
                pinkTeamPos = new(22.25f, 3f, PlayerControl.LocalPlayer.transform.position.z);
                limeTeamFloorPos = new(6.15f, 13.25f, 0.5f);
                pinkTeamFloorPos = new(22.25f, 3f, 0.5f);
                break;
            // Polus
            case 2:
                serialKillerPos = new(22.3f, -19.15f, PlayerControl.LocalPlayer.transform.position.z);
                limeTeamPos = new(2.35f, -23.75f, PlayerControl.LocalPlayer.transform.position.z);
                pinkTeamPos = new(36.35f, -8f, PlayerControl.LocalPlayer.transform.position.z);
                limeTeamFloorPos = new(2.35f, -23.75f, 0.5f);
                pinkTeamFloorPos = new(36.35f, -8f, 0.5f);
                break;
            // Dleks
            case 3:
                serialKillerPos = new(-6.35f, -7.5f, PlayerControl.LocalPlayer.transform.position.z);
                limeTeamPos = new(17f, -5.5f, PlayerControl.LocalPlayer.transform.position.z);
                pinkTeamPos = new(-12f, -4.75f, PlayerControl.LocalPlayer.transform.position.z);
                limeTeamFloorPos = new(17f, -5.5f, 0.5f);
                pinkTeamFloorPos = new(-12f, -4.75f, 0.5f);
                break;
            // Airship
            case 4:
                serialKillerPos = new(12.25f, 2f, PlayerControl.LocalPlayer.transform.position.z);
                limeTeamPos = new(-13.9f, -14.45f, PlayerControl.LocalPlayer.transform.position.z);
                pinkTeamPos = new(37.35f, -3.25f, PlayerControl.LocalPlayer.transform.position.z);
                limeTeamFloorPos = new(-13.9f, -14.45f, 0.5f);
                pinkTeamFloorPos = new(37.35f, -3.25f, 0.5f);
                break;
            // Fungle
            case 5:
                serialKillerPos = new(9.35f, -9.85f, PlayerControl.LocalPlayer.transform.position.z);
                limeTeamPos = new(1.6f, -1.65f, PlayerControl.LocalPlayer.transform.position.z);
                pinkTeamPos = new(6.75f, 2f, PlayerControl.LocalPlayer.transform.position.z);
                limeTeamFloorPos = new(1.6f, -1.65f, 0.5f);
                pinkTeamFloorPos = new(6.75f, 2, 0.5f);
                break;
            // Submerged
            case 6:
                serialKillerPos = new(5.75f, 31.25f, PlayerControl.LocalPlayer.transform.position.z);
                limeTeamPos = new(-12.25f, 18.5f, PlayerControl.LocalPlayer.transform.position.z);
                pinkTeamPos = new(-8.5f, -39.5f, PlayerControl.LocalPlayer.transform.position.z);
                limeTeamFloorPos = new(-12.25f, 18.5f, 0.03f);
                pinkTeamFloorPos = new(-8.5f, -39.5f, -0.01f);
                var limeTeamFloorTwo = GameObject.Instantiate(AssetLoader.Greenfloor, PlayerControl.LocalPlayer.transform.parent);
                limeTeamFloorTwo.name = "LimeTeamFloorTwo";
                limeTeamFloorTwo.transform.position = new(-14.5f, -34.35f, -0.01f);
                var pinkTeamFloorTwo = GameObject.Instantiate(AssetLoader.Redfloor, PlayerControl.LocalPlayer.transform.parent);
                pinkTeamFloorTwo.name = "PinkTeamFloorTwo";
                pinkTeamFloorTwo.transform.position = new(0f, 33.5f, 0.03f);
                SerialKillerSpawns.Add(limeTeamFloorTwo);
                SerialKillerSpawns.Add(pinkTeamFloorTwo);
                break;
        }

        if (MatchType == 0)
        {
            foreach (var soloPlayer in SoloPlayerTeam)
            {
                soloPlayer.transform.position = new(SoloPlayersSpawnPositions[_howmanyBattleRoyaleplayers].x, SoloPlayersSpawnPositions[_howmanyBattleRoyaleplayers].y, PlayerControl.LocalPlayer.transform.position.z);
                _howmanyBattleRoyaleplayers += 1;
            }
        }
        else
        {
            if (SerialKiller != null)
            {
                SerialKiller.MyPhysics.SetBodyType(PlayerBodyTypes.Seeker);
                SerialKiller.transform.position = serialKillerPos;
            }

            foreach (var player in LimeTeam) player.transform.position = limeTeamPos;

            foreach (var player in PinkTeam) player.transform.position = pinkTeamPos;
        }

        if (PlayerControl.LocalPlayer != null && !_createdbattleroyale)
        {
            PlayerControl.LocalPlayer.ClearAllTasks();

            if (MatchType != 0)
            {
                var limeteamfloor = GameObject.Instantiate(AssetLoader.Greenfloor, PlayerControl.LocalPlayer.transform.parent);
                limeteamfloor.name = "limeteamfloor";
                limeteamfloor.transform.position = limeTeamFloorPos;
                var pinkteamfloor = GameObject.Instantiate(AssetLoader.Redfloor, PlayerControl.LocalPlayer.transform.parent);
                pinkteamfloor.name = "pinkteamfloor";
                pinkteamfloor.transform.position = pinkTeamFloorPos;
                SerialKillerSpawns.Add(limeteamfloor);
                SerialKillerSpawns.Add(pinkteamfloor);
            }

            _createdbattleroyale = true;
        }
    }

    public static void OnMurderPlayerPostfix(PlayerControl __instance, PlayerControl target)
    {
        var brBody = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == target.PlayerId);
        brBody.transform.position = new(50, 50, 1);

        if (MatchType == 2)
        {
            if (SerialKiller != null && SerialKiller.PlayerId == target.PlayerId)
            {
                SerialKillerIsReviving = true;
                SerialKiller.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
                Helpers.AlphaPlayer(true, SerialKiller.PlayerId);
                HudManager.Instance.StartCoroutine(Effects.Lerp(MapSettings.GamemodeReviveTime, new Action<float>(p =>
                {
                    if (p == 1f && SerialKiller != null)
                    {
                        SerialKillerIsReviving = false;
                        if (PlayerControl.AllPlayerControls.Count >= 11)
                            SerialKillerLifes = FighterLifes * 3;
                        else
                            SerialKillerLifes = FighterLifes * 2;
                        Helpers.AlphaPlayer(false, SerialKiller.PlayerId);
                        SerialKiller.MyPhysics.SetBodyType(PlayerBodyTypes.Seeker);
                    }
                })));
                HudManager.Instance.StartCoroutine(Effects.Lerp(MapSettings.GamemodeReviveTime - MapSettings.GamemodeInvincibilityTime, new Action<float>(p =>
                {
                    if (p == 1f && SerialKiller != null)
                    {
                        SerialKiller.Revive();
                        switch (GameOptionsManager.Instance.currentGameOptions.MapId)
                        {
                            // Skeld
                            case 0:
                                if (RebuildUs.ActivatedSensei)
                                    SerialKiller.transform.position = new(-3.65f, 5f, PlayerControl.LocalPlayer.transform.position.z);
                                else if (RebuildUs.ActivatedDleks)
                                    SerialKiller.transform.position = new(-6.35f, -7.5f, PlayerControl.LocalPlayer.transform.position.z);
                                else
                                    SerialKiller.transform.position = new(6.35f, -7.5f, PlayerControl.LocalPlayer.transform.position.z);

                                break;
                            // MiraHQ
                            case 1:
                                SerialKiller.transform.position = new(16.25f, 24.5f, PlayerControl.LocalPlayer.transform.position.z);
                                break;
                            // Polus
                            case 2:
                                SerialKiller.transform.position = new(22.3f, -19.15f, PlayerControl.LocalPlayer.transform.position.z);
                                break;
                            // Dleks
                            case 3:
                                SerialKiller.transform.position = new(-6.35f, -7.5f, PlayerControl.LocalPlayer.transform.position.z);
                                break;
                            // Airship
                            case 4:
                                SerialKiller.transform.position = new(12.25f, 2f, PlayerControl.LocalPlayer.transform.position.z);
                                break;
                            // Fungle
                            case 5:
                                SerialKiller.transform.position = new(9.35f, -9.85f, PlayerControl.LocalPlayer.transform.position.z);
                                break;
                            // Submerged
                            case 6:
                                if (SerialKiller.transform.position.y > 0)
                                    SerialKiller.transform.position = new(5.75f, 31.25f, SerialKiller.transform.position.z);
                                else
                                    SerialKiller.transform.position = new(-4.25f, -33.5f, SerialKiller.transform.position.z);

                                break;
                        }

                        var deadPlayerEntry = GameHistory.DEAD_PLAYERS.Where(x => x.Player.PlayerId == target.PlayerId).FirstOrDefault();
                        if (brBody != null)
                            Object.Destroy(brBody.gameObject);
                        if (deadPlayerEntry != null) GameHistory.DEAD_PLAYERS.Remove(deadPlayerEntry);
                    }
                })));
            }

            foreach (var player in LimeTeam)
            {
                if (player.PlayerId == target.PlayerId)
                {
                    if (LimePlayer01 != null && target.PlayerId == LimePlayer01.PlayerId)
                        LimePlayer01IsReviving = true;
                    else if (LimePlayer02 != null && target.PlayerId == LimePlayer02.PlayerId)
                        LimePlayer02IsReviving = true;
                    else if (LimePlayer03 != null && target.PlayerId == LimePlayer03.PlayerId)
                        LimePlayer03IsReviving = true;
                    else if (LimePlayer04 != null && target.PlayerId == LimePlayer04.PlayerId)
                        LimePlayer04IsReviving = true;
                    else if (LimePlayer05 != null && target.PlayerId == LimePlayer05.PlayerId)
                        LimePlayer05IsReviving = true;
                    else if (LimePlayer06 != null && target.PlayerId == LimePlayer06.PlayerId)
                        LimePlayer06IsReviving = true;
                    else if (LimePlayer07 != null && target.PlayerId == LimePlayer07.PlayerId)
                        LimePlayer07IsReviving = true;
                    Helpers.AlphaPlayer(true, player.PlayerId);
                    HudManager.Instance.StartCoroutine(Effects.Lerp(MapSettings.GamemodeReviveTime, new Action<float>(p =>
                    {
                        if (p == 1f && player != null)
                        {
                            if (LimePlayer01 != null && target.PlayerId == LimePlayer01.PlayerId)
                            {
                                LimePlayer01IsReviving = false;
                                LimePlayer01Lifes = FighterLifes;
                            }
                            else if (LimePlayer02 != null && target.PlayerId == LimePlayer02.PlayerId)
                            {
                                LimePlayer02IsReviving = false;
                                LimePlayer02Lifes = FighterLifes;
                            }
                            else if (LimePlayer03 != null && target.PlayerId == LimePlayer03.PlayerId)
                            {
                                LimePlayer03IsReviving = false;
                                LimePlayer03Lifes = FighterLifes;
                            }
                            else if (LimePlayer04 != null && target.PlayerId == LimePlayer04.PlayerId)
                            {
                                LimePlayer04IsReviving = false;
                                LimePlayer04Lifes = FighterLifes;
                            }
                            else if (LimePlayer05 != null && target.PlayerId == LimePlayer05.PlayerId)
                            {
                                LimePlayer05IsReviving = false;
                                LimePlayer05Lifes = FighterLifes;
                            }
                            else if (LimePlayer06 != null && target.PlayerId == LimePlayer06.PlayerId)
                            {
                                LimePlayer06IsReviving = false;
                                LimePlayer06Lifes = FighterLifes;
                            }
                            else if (LimePlayer07 != null && target.PlayerId == LimePlayer07.PlayerId)
                            {
                                LimePlayer07IsReviving = false;
                                LimePlayer07Lifes = FighterLifes;
                            }

                            Helpers.AlphaPlayer(false, player.PlayerId);
                        }
                    })));
                    HudManager.Instance.StartCoroutine(Effects.Lerp(MapSettings.GamemodeReviveTime - MapSettings.GamemodeInvincibilityTime, new Action<float>(p =>
                    {
                        if (p == 1f && player != null)
                        {
                            player.Revive();
                            switch (GameOptionsManager.Instance.currentGameOptions.MapId)
                            {
                                // Skeld
                                case 0:
                                    if (RebuildUs.ActivatedSensei)
                                        player.transform.position = new(-17.5f, -1.15f, player.transform.position.z);
                                    else if (RebuildUs.ActivatedDleks)
                                        player.transform.position = new(17f, -5.5f, player.transform.position.z);
                                    else
                                        player.transform.position = new(-17f, -5.5f, player.transform.position.z);

                                    break;
                                // MiraHQ
                                case 1:
                                    player.transform.position = new(6.15f, 13.25f, player.transform.position.z);
                                    break;
                                // Polus
                                case 2:
                                    player.transform.position = new(2.35f, -23.75f, player.transform.position.z);
                                    break;
                                // Dleks
                                case 3:
                                    player.transform.position = new(17f, -5.5f, player.transform.position.z);
                                    break;
                                // Airship
                                case 4:
                                    player.transform.position = new(-13.9f, -14.45f, player.transform.position.z);
                                    break;
                                // Fungle
                                case 5:
                                    player.transform.position = new(1.6f, -1.65f, player.transform.position.z);
                                    break;
                                // Submerged
                                case 6:
                                    if (player.transform.position.y > 0)
                                        player.transform.position = new(-12.25f, 18.5f, player.transform.position.z);
                                    else
                                        player.transform.position = new(-14.5f, -34.35f, player.transform.position.z);

                                    break;
                            }

                            var deadPlayerEntry = GameHistory.DEAD_PLAYERS.Where(x => x.Player.PlayerId == target.PlayerId).FirstOrDefault();
                            if (brBody != null)
                                Object.Destroy(brBody.gameObject);
                            if (deadPlayerEntry != null) GameHistory.DEAD_PLAYERS.Remove(deadPlayerEntry);
                        }
                    })));
                }
            }

            foreach (var player in PinkTeam)
            {
                if (player.PlayerId == target.PlayerId)
                {
                    if (PinkPlayer01 != null && target.PlayerId == PinkPlayer01.PlayerId)
                        PinkPlayer01IsReviving = true;
                    else if (PinkPlayer02 != null && target.PlayerId == PinkPlayer02.PlayerId)
                        PinkPlayer02IsReviving = true;
                    else if (PinkPlayer03 != null && target.PlayerId == PinkPlayer03.PlayerId)
                        PinkPlayer03IsReviving = true;
                    else if (PinkPlayer04 != null && target.PlayerId == PinkPlayer04.PlayerId)
                        PinkPlayer04IsReviving = true;
                    else if (PinkPlayer05 != null && target.PlayerId == PinkPlayer05.PlayerId)
                        PinkPlayer05IsReviving = true;
                    else if (PinkPlayer06 != null && target.PlayerId == PinkPlayer06.PlayerId)
                        PinkPlayer06IsReviving = true;
                    else if (PinkPlayer01 != null && target.PlayerId == PinkPlayer07.PlayerId)
                        PinkPlayer07IsReviving = true;
                    Helpers.AlphaPlayer(true, player.PlayerId);
                    HudManager.Instance.StartCoroutine(Effects.Lerp(MapSettings.GamemodeReviveTime, new Action<float>(p =>
                    {
                        if (p == 1f && player != null)
                        {
                            if (PinkPlayer01 != null && target.PlayerId == PinkPlayer01.PlayerId)
                            {
                                PinkPlayer01IsReviving = false;
                                PinkPlayer01Lifes = FighterLifes;
                            }
                            else if (PinkPlayer02 != null && target.PlayerId == PinkPlayer02.PlayerId)
                            {
                                PinkPlayer02IsReviving = false;
                                PinkPlayer02Lifes = FighterLifes;
                            }
                            else if (PinkPlayer03 != null && target.PlayerId == PinkPlayer03.PlayerId)
                            {
                                PinkPlayer03IsReviving = false;
                                PinkPlayer03Lifes = FighterLifes;
                            }
                            else if (PinkPlayer04 != null && target.PlayerId == PinkPlayer04.PlayerId)
                            {
                                PinkPlayer04IsReviving = false;
                                PinkPlayer04Lifes = FighterLifes;
                            }
                            else if (PinkPlayer05 != null && target.PlayerId == PinkPlayer05.PlayerId)
                            {
                                PinkPlayer05IsReviving = false;
                                PinkPlayer05Lifes = FighterLifes;
                            }
                            else if (PinkPlayer06 != null && target.PlayerId == PinkPlayer06.PlayerId)
                            {
                                PinkPlayer06IsReviving = false;
                                PinkPlayer06Lifes = FighterLifes;
                            }
                            else if (PinkPlayer01 != null && target.PlayerId == PinkPlayer07.PlayerId)
                            {
                                PinkPlayer07IsReviving = false;
                                PinkPlayer07Lifes = FighterLifes;
                            }

                            Helpers.AlphaPlayer(false, player.PlayerId);
                        }
                    })));

                    HudManager.Instance.StartCoroutine(Effects.Lerp(MapSettings.GamemodeReviveTime - MapSettings.GamemodeInvincibilityTime, new Action<float>(p =>
                    {
                        if (p == 1f && player != null)
                        {
                            player.Revive();
                            switch (GameOptionsManager.Instance.currentGameOptions.MapId)
                            {
                                // Skeld
                                case 0:
                                    if (RebuildUs.ActivatedSensei)
                                        player.transform.position = new(7.7f, -0.95f, player.transform.position.z);
                                    else if (RebuildUs.ActivatedDleks)
                                        player.transform.position = new(-12f, -4.75f, player.transform.position.z);
                                    else
                                        player.transform.position = new(12f, -4.75f, player.transform.position.z);

                                    break;
                                // MiraHQ
                                case 1:
                                    player.transform.position = new(22.25f, 3f, player.transform.position.z);
                                    break;
                                // Polus
                                case 2:
                                    player.transform.position = new(36.35f, -8f, player.transform.position.z);
                                    break;
                                // Dleks
                                case 3:
                                    player.transform.position = new(-12f, -4.75f, player.transform.position.z);
                                    break;
                                // Airship
                                case 4:
                                    player.transform.position = new(37.35f, -3.25f, player.transform.position.z);
                                    break;
                                // Fungle
                                case 5:
                                    player.transform.position = new(6.75f, 2f, player.transform.position.z);
                                    break;
                                // Submerged
                                case 6:
                                    if (player.transform.position.y > 0)
                                        player.transform.position = new(0f, 33.5f, player.transform.position.z);
                                    else
                                        player.transform.position = new(-8.5f, -39.5f, player.transform.position.z);

                                    break;
                            }

                            var deadPlayerEntry = GameHistory.DEAD_PLAYERS.Where(x => x.Player.PlayerId == target.PlayerId).FirstOrDefault();
                            if (brBody != null)
                                Object.Destroy(brBody.gameObject);
                            if (deadPlayerEntry != null) GameHistory.DEAD_PLAYERS.Remove(deadPlayerEntry);
                        }
                    })));
                }
            }
        }
    }

    public static void BattleRoyaleUpdate()
    {
        if (MapSettings.GameMode is not CustomGameMode.BattleRoyale) return;

        if (MatchType == 0)
        {
            // If solo player disconnects, check number of players
            foreach (var soloPlayer in SoloPlayerTeam)
            {
                if (soloPlayer.Data.Disconnected)
                {
                    if (SoloPlayer01 != null && soloPlayer.PlayerId == SoloPlayer01.PlayerId)
                        SoloPlayerTeam.Remove(SoloPlayer01);
                    else if (SoloPlayer02 != null && soloPlayer.PlayerId == SoloPlayer02.PlayerId)
                        SoloPlayerTeam.Remove(SoloPlayer02);
                    else if (SoloPlayer03 != null && soloPlayer.PlayerId == SoloPlayer03.PlayerId)
                        SoloPlayerTeam.Remove(SoloPlayer03);
                    else if (SoloPlayer04 != null && soloPlayer.PlayerId == SoloPlayer04.PlayerId)
                        SoloPlayerTeam.Remove(SoloPlayer04);
                    else if (SoloPlayer05 != null && soloPlayer.PlayerId == SoloPlayer05.PlayerId)
                        SoloPlayerTeam.Remove(SoloPlayer05);
                    else if (SoloPlayer06 != null && soloPlayer.PlayerId == SoloPlayer06.PlayerId)
                        SoloPlayerTeam.Remove(SoloPlayer06);
                    else if (SoloPlayer07 != null && soloPlayer.PlayerId == SoloPlayer07.PlayerId)
                        SoloPlayerTeam.Remove(SoloPlayer07);
                    else if (SoloPlayer08 != null && soloPlayer.PlayerId == SoloPlayer08.PlayerId)
                        SoloPlayerTeam.Remove(SoloPlayer08);
                    else if (SoloPlayer09 != null && soloPlayer.PlayerId == SoloPlayer09.PlayerId)
                        SoloPlayerTeam.Remove(SoloPlayer09);
                    else if (SoloPlayer10 != null && soloPlayer.PlayerId == SoloPlayer10.PlayerId)
                        SoloPlayerTeam.Remove(SoloPlayer10);
                    else if (SoloPlayer11 != null && soloPlayer.PlayerId == SoloPlayer11.PlayerId)
                        SoloPlayerTeam.Remove(SoloPlayer11);
                    else if (SoloPlayer12 != null && soloPlayer.PlayerId == SoloPlayer12.PlayerId)
                        SoloPlayerTeam.Remove(SoloPlayer12);
                    else if (SoloPlayer13 != null && soloPlayer.PlayerId == SoloPlayer13.PlayerId)
                        SoloPlayerTeam.Remove(SoloPlayer13);
                    else if (SoloPlayer14 != null && soloPlayer.PlayerId == SoloPlayer14.PlayerId)
                        SoloPlayerTeam.Remove(SoloPlayer14);
                    else if (SoloPlayer15 != null && soloPlayer.PlayerId == SoloPlayer15.PlayerId)
                        SoloPlayerTeam.Remove(SoloPlayer15);

                    var soloPlayersAlives = 0;

                    foreach (var remainPlayer in SoloPlayerTeam)
                    {
                        if (!remainPlayer.Data.IsDead)
                            soloPlayersAlives += 1;
                    }

                    BattleRoyalePointCounter = new StringBuilder(Tr.Get(TrKey.BattleRoyaleFighters)).Append("<color=#009F57FF>").Append(soloPlayersAlives).Append("</color>").ToString();

                    if (soloPlayersAlives <= 1)
                    {
                        TriggerSoloWin = true;
                        GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleSoloWin, false);
                    }

                    break;
                }
            }
        }
        else
        {
            // lime Team disconnects
            foreach (var limePlayer in LimeTeam)
            {
                if (limePlayer.Data.Disconnected)
                {
                    if (LimePlayer01 != null && limePlayer.PlayerId == LimePlayer01.PlayerId)
                        LimeTeam.Remove(LimePlayer01);
                    else if (LimePlayer02 != null && limePlayer.PlayerId == LimePlayer02.PlayerId)
                        LimeTeam.Remove(LimePlayer02);
                    else if (LimePlayer03 != null && limePlayer.PlayerId == LimePlayer03.PlayerId)
                        LimeTeam.Remove(LimePlayer03);
                    else if (LimePlayer04 != null && limePlayer.PlayerId == LimePlayer04.PlayerId)
                        LimeTeam.Remove(LimePlayer04);
                    else if (LimePlayer05 != null && limePlayer.PlayerId == LimePlayer05.PlayerId)
                        LimeTeam.Remove(LimePlayer05);
                    else if (LimePlayer06 != null && limePlayer.PlayerId == LimePlayer06.PlayerId)
                        LimeTeam.Remove(LimePlayer06);
                    else if (LimePlayer07 != null && limePlayer.PlayerId == LimePlayer07.PlayerId)
                        LimeTeam.Remove(LimePlayer07);

                    var limePlayersAlive = 0;

                    foreach (var remainingLimePlayer in LimeTeam)
                    {
                        if (!remainingLimePlayer.Data.IsDead)
                            limePlayersAlive += 1;
                    }

                    var pinkPlayersAlive = 0;

                    foreach (var remainingPinkPlayer in PinkTeam)
                    {
                        if (!remainingPinkPlayer.Data.IsDead)
                            pinkPlayersAlive += 1;
                    }

                    if (SerialKiller != null)
                    {
                        var serialKillerAlive = 0;

                        foreach (var serialKiller in SerialKillerTeam)
                        {
                            if (!serialKiller.Data.IsDead)
                                serialKillerAlive += 1;
                        }

                        if (MatchType == 1)
                        {
                            var sb = new StringBuilder(Tr.Get(TrKey.BattleRoyaleLimeTeam)).Append("<color=#39FF14FF>").Append(limePlayersAlive).Append("</color> | ").Append(Tr.Get(TrKey.BattleRoyalePinkTeam)).Append("<color=#F2BEFFFF>").Append(pinkPlayersAlive).Append("</color> | ").Append(Tr.Get(TrKey.BattleRoyaleSerialKiller)).Append("<color=#808080FF>").Append(serialKillerAlive).Append("</color>");
                            BattleRoyalePointCounter = sb.ToString();
                            if (limePlayersAlive <= 0 && pinkPlayersAlive <= 0 && !SerialKiller.Data.IsDead)
                            {
                                TriggerSerialKillerWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleSerialKillerWin, false);
                            }
                            else if (pinkPlayersAlive <= 0 && SerialKiller.Data.IsDead)
                            {
                                TriggerLimeTeamWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleLimeTeamWin, false);
                            }
                            else if (limePlayersAlive <= 0 && SerialKiller.Data.IsDead)
                            {
                                TriggerPinkTeamWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyalePinkTeamWin, false);
                            }
                        }
                    }
                    else
                    {
                        if (MatchType == 1)
                        {
                            var sb = new StringBuilder(Tr.Get(TrKey.BattleRoyaleLimeTeam)).Append("<color=#39FF14FF>").Append(limePlayersAlive).Append("</color> | ").Append(Tr.Get(TrKey.BattleRoyalePinkTeam)).Append("<color=#F2BEFFFF>").Append(pinkPlayersAlive).Append("</color>");
                            BattleRoyalePointCounter = sb.ToString();
                            if (pinkPlayersAlive <= 0)
                            {
                                TriggerLimeTeamWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleLimeTeamWin, false);
                            }
                            else if (limePlayersAlive <= 0)
                            {
                                TriggerPinkTeamWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyalePinkTeamWin, false);
                            }
                        }
                    }

                    break;
                }
            }

            // Pink Team disconnects
            foreach (var pinkPlayer in PinkTeam)
            {
                if (pinkPlayer.Data.Disconnected)
                {
                    if (PinkPlayer01 != null && pinkPlayer.PlayerId == PinkPlayer01.PlayerId)
                        PinkTeam.Remove(PinkPlayer01);
                    else if (PinkPlayer02 != null && pinkPlayer.PlayerId == PinkPlayer02.PlayerId)
                        PinkTeam.Remove(PinkPlayer02);
                    else if (PinkPlayer03 != null && pinkPlayer.PlayerId == PinkPlayer03.PlayerId)
                        PinkTeam.Remove(PinkPlayer03);
                    else if (PinkPlayer04 != null && pinkPlayer.PlayerId == PinkPlayer04.PlayerId)
                        PinkTeam.Remove(PinkPlayer04);
                    else if (PinkPlayer05 != null && pinkPlayer.PlayerId == PinkPlayer05.PlayerId)
                        PinkTeam.Remove(PinkPlayer05);
                    else if (PinkPlayer06 != null && pinkPlayer.PlayerId == PinkPlayer06.PlayerId)
                        PinkTeam.Remove(PinkPlayer06);
                    else if (PinkPlayer07 != null && pinkPlayer.PlayerId == PinkPlayer07.PlayerId)
                        PinkTeam.Remove(PinkPlayer07);

                    var limePlayersAlive = 0;

                    foreach (var remainingLimePlayer in LimeTeam)
                    {
                        if (!remainingLimePlayer.Data.IsDead)
                            limePlayersAlive += 1;
                    }

                    var pinkPlayersAlive = 0;

                    foreach (var remainingPinkPlayer in PinkTeam)
                    {
                        if (!remainingPinkPlayer.Data.IsDead)
                            pinkPlayersAlive += 1;
                    }

                    if (SerialKiller != null)
                    {
                        var serialKillerAlive = 0;

                        foreach (var serialKiller in SerialKillerTeam)
                        {
                            if (!serialKiller.Data.IsDead)
                                serialKillerAlive += 1;
                        }

                        if (MatchType == 1)
                        {
                            var sb = new StringBuilder(Tr.Get(TrKey.BattleRoyaleLimeTeam)).Append("<color=#39FF14FF>").Append(limePlayersAlive).Append("</color> | ").Append(Tr.Get(TrKey.BattleRoyalePinkTeam)).Append("<color=#F2BEFFFF>").Append(pinkPlayersAlive).Append("</color> | ").Append(Tr.Get(TrKey.BattleRoyaleSerialKiller)).Append("<color=#808080FF>").Append(serialKillerAlive).Append("</color>");
                            BattleRoyalePointCounter = sb.ToString();
                            if (limePlayersAlive <= 0 && pinkPlayersAlive <= 0 && !SerialKiller.Data.IsDead)
                            {
                                TriggerSerialKillerWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleSerialKillerWin, false);
                            }
                            else if (pinkPlayersAlive <= 0 && SerialKiller.Data.IsDead)
                            {
                                TriggerLimeTeamWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleLimeTeamWin, false);
                            }
                            else if (limePlayersAlive <= 0 && SerialKiller.Data.IsDead)
                            {
                                TriggerPinkTeamWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyalePinkTeamWin, false);
                            }
                        }
                    }
                    else
                    {
                        if (MatchType == 1)
                        {
                            var sb = new StringBuilder(Tr.Get(TrKey.BattleRoyaleLimeTeam)).Append("<color=#39FF14FF>").Append(limePlayersAlive).Append("</color> | ").Append(Tr.Get(TrKey.BattleRoyalePinkTeam)).Append("<color=#F2BEFFFF>").Append(pinkPlayersAlive).Append("</color>");
                            BattleRoyalePointCounter = sb.ToString();
                            if (pinkPlayersAlive <= 0)
                            {
                                TriggerLimeTeamWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleLimeTeamWin, false);
                            }
                            else if (limePlayersAlive <= 0)
                            {
                                TriggerPinkTeamWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyalePinkTeamWin, false);
                            }
                        }
                    }

                    break;
                }
            }

            // Serial Killer disconnects
            if (SerialKiller != null && SerialKiller.Data.Disconnected)
            {
                SerialKillerTeam.Remove(SerialKiller);

                var limePlayersAlive = 0;

                foreach (var limePlayer in LimeTeam)
                {
                    if (!limePlayer.Data.IsDead)
                        limePlayersAlive += 1;
                }

                var pinkPlayersAlive = 0;

                foreach (var pinkPlayer in PinkTeam)
                {
                    if (!pinkPlayer.Data.IsDead)
                        pinkPlayersAlive += 1;
                }

                var serialKillerAlive = 0;

                if (MatchType == 1)
                {
                    var sb = new StringBuilder(Tr.Get(TrKey.BattleRoyaleLimeTeam)).Append("<color=#39FF14FF>").Append(limePlayersAlive).Append("</color> | ").Append(Tr.Get(TrKey.BattleRoyalePinkTeam)).Append("<color=#F2BEFFFF>").Append(pinkPlayersAlive).Append("</color> | ").Append(Tr.Get(TrKey.BattleRoyaleSerialKiller)).Append("<color=#808080FF>").Append(serialKillerAlive).Append("</color>");
                    BattleRoyalePointCounter = sb.ToString();
                    if (pinkPlayersAlive <= 0)
                    {
                        TriggerLimeTeamWin = true;
                        GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleLimeTeamWin, false);
                    }
                    else if (limePlayersAlive <= 0)
                    {
                        TriggerPinkTeamWin = true;
                        GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyalePinkTeamWin, false);
                    }
                }
            }
        }
    }
}
