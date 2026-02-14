using Object = UnityEngine.Object;

namespace RebuildUs.Modules.GameMode;

public static partial class PoliceAndThief
{
    public static Color PolicePlayerColor = new Color32(102, 102, 153, byte.MaxValue);
    public static Color ThiefPlayerColor = new Color32(127, 76, 0, byte.MaxValue);
    public static Color IntroColor = new Color32(0, 247, 255, byte.MaxValue);

    public static bool Createdpoliceandthief;

    public static List<PlayerControl> ThiefTeam = [];
    public static PlayerControl Thiefplayer01;
    public static PlayerControl Thiefplayer01CurrentTarget;
    public static bool Thiefplayer01IsStealing;
    public static byte Thiefplayer01JewelId;
    public static bool Thiefplayer01IsReviving;
    public static PlayerControl Thiefplayer02;
    public static PlayerControl Thiefplayer02CurrentTarget;
    public static bool Thiefplayer02IsStealing;
    public static byte Thiefplayer02JewelId;
    public static bool Thiefplayer02IsReviving;
    public static PlayerControl Thiefplayer03;
    public static PlayerControl Thiefplayer03CurrentTarget;
    public static bool Thiefplayer03IsStealing;
    public static byte Thiefplayer03JewelId;
    public static bool Thiefplayer03IsReviving;
    public static PlayerControl Thiefplayer04;
    public static PlayerControl Thiefplayer04CurrentTarget;
    public static bool Thiefplayer04IsStealing;
    public static byte Thiefplayer04JewelId;
    public static bool Thiefplayer04IsReviving;
    public static PlayerControl Thiefplayer05;
    public static PlayerControl Thiefplayer05CurrentTarget;
    public static bool Thiefplayer05IsStealing;
    public static byte Thiefplayer05JewelId;
    public static bool Thiefplayer05IsReviving;
    public static PlayerControl Thiefplayer06;
    public static PlayerControl Thiefplayer06CurrentTarget;
    public static bool Thiefplayer06IsStealing;
    public static byte Thiefplayer06JewelId;
    public static bool Thiefplayer06IsReviving;
    public static PlayerControl Thiefplayer07;
    public static PlayerControl Thiefplayer07CurrentTarget;
    public static bool Thiefplayer07IsStealing;
    public static byte Thiefplayer07JewelId;
    public static bool Thiefplayer07IsReviving;
    public static PlayerControl Thiefplayer08;
    public static PlayerControl Thiefplayer08CurrentTarget;
    public static bool Thiefplayer08IsStealing;
    public static byte Thiefplayer08JewelId;
    public static bool Thiefplayer08IsReviving;
    public static PlayerControl Thiefplayer09;
    public static PlayerControl Thiefplayer09CurrentTarget;
    public static bool Thiefplayer09IsStealing;
    public static byte Thiefplayer09JewelId;
    public static bool Thiefplayer09IsReviving;

    public static List<PlayerControl> PoliceTeam = [];
    public static PlayerControl Policeplayer01;
    public static PlayerControl Policeplayer01CurrentTarget;
    public static PlayerControl Policeplayer01TargetedPlayer;
    public static float Policeplayer01LightTimer;
    public static bool Policeplayer01IsReviving;
    public static PlayerControl Policeplayer02;
    public static PlayerControl Policeplayer02CurrentTarget;
    public static float Policeplayer02LightTimer;
    public static bool Policeplayer02IsReviving;
    public static GameObject Policeplayer02Taser;
    public static float Policeplayer02MouseAngle;
    public static PlayerControl Policeplayer03;
    public static PlayerControl Policeplayer03CurrentTarget;
    public static PlayerControl Policeplayer03TargetedPlayer;
    public static float Policeplayer03LightTimer;
    public static bool Policeplayer03IsReviving;
    public static PlayerControl Policeplayer04;
    public static PlayerControl Policeplayer04CurrentTarget;
    public static float Policeplayer04LightTimer;
    public static bool Policeplayer04IsReviving;
    public static GameObject Policeplayer04Taser;
    public static float Policeplayer04MouseAngle;
    public static PlayerControl Policeplayer05;
    public static PlayerControl Policeplayer05CurrentTarget;
    public static PlayerControl Policeplayer05TargetedPlayer;
    public static float Policeplayer05LightTimer;
    public static bool Policeplayer05IsReviving;
    public static PlayerControl Policeplayer06;
    public static PlayerControl Policeplayer06CurrentTarget;
    public static PlayerControl Policeplayer06TargetedPlayer;
    public static float Policeplayer06LightTimer;
    public static bool Policeplayer06IsReviving;

    public static List<PlayerControl> ThiefArrested = [];
    public static List<GameObject> ThiefTreasures = [];
    public static GameObject Cell;
    public static GameObject Cellbutton;
    public static GameObject Jewelbutton;

    public static GameObject Celltwo;
    public static GameObject Cellbuttontwo;
    public static GameObject Jewelbuttontwo;

    public static GameObject Jewel01;
    public static PlayerControl Jewel01BeingStealed;
    public static GameObject Jewel02;
    public static PlayerControl Jewel02BeingStealed;
    public static GameObject Jewel03;
    public static PlayerControl Jewel03BeingStealed;
    public static GameObject Jewel04;
    public static PlayerControl Jewel04BeingStealed;
    public static GameObject Jewel05;
    public static PlayerControl Jewel05BeingStealed;
    public static GameObject Jewel06;
    public static PlayerControl Jewel06BeingStealed;
    public static GameObject Jewel07;
    public static PlayerControl Jewel07BeingStealed;
    public static GameObject Jewel08;
    public static PlayerControl Jewel08BeingStealed;
    public static GameObject Jewel09;
    public static PlayerControl Jewel09BeingStealed;
    public static GameObject Jewel10;
    public static PlayerControl Jewel10BeingStealed;
    public static GameObject Jewel11;
    public static PlayerControl Jewel11BeingStealed;
    public static GameObject Jewel12;
    public static PlayerControl Jewel12BeingStealed;
    public static GameObject Jewel13;
    public static PlayerControl Jewel13BeingStealed;
    public static GameObject Jewel14;
    public static PlayerControl Jewel14BeingStealed;
    public static GameObject Jewel15;
    public static PlayerControl Jewel15BeingStealed;

    public static float RequiredJewels = 10;
    public static float PoliceTaseCooldown = 20f;
    public static float PoliceTaseDuration = 5f;
    public static bool PoliceCanSeeJewels;
    public static float PoliceCatchCooldown = 10f;
    public static float CaptureThiefTime = 3f;
    public static int WhoCanThiefsKill;

    public static float CurrentJewelsStoled;
    public static float CurrentThiefsCaptured;

    public static bool TriggerThiefWin;
    public static bool TriggerPoliceWin;

    public static string ThiefpointCounter = new StringBuilder(Tr.Get(TrKey.StolenJewels)).Append("<color=#FF0000FF>").Append(CurrentJewelsStoled).Append(" / ").Append(RequiredJewels).Append("</color> | ").Append(Tr.Get(TrKey.CapturedThieves)).Append("<color=#0000FFFF>").Append(CurrentThiefsCaptured).Append(" / 10</color>").ToString();

    public static List<Arrow> LocalThiefReleaseArrow = [];
    public static List<Arrow> LocalThiefDeliverArrow = [];

    public static void ClearAndReload()
    {
        Createdpoliceandthief = false;

        Cell = null;
        Cellbutton = null;
        Jewelbutton = null;
        ThiefTreasures.Clear();
        ThiefArrested.Clear();

        Celltwo = null;
        Cellbuttontwo = null;
        Jewelbuttontwo = null;

        ThiefTeam.Clear();
        Thiefplayer01 = null;
        Thiefplayer01CurrentTarget = null;
        Thiefplayer01IsStealing = false;
        Thiefplayer01JewelId = 0;
        Thiefplayer01IsReviving = false;
        Thiefplayer02 = null;
        Thiefplayer02CurrentTarget = null;
        Thiefplayer02IsStealing = false;
        Thiefplayer02JewelId = 0;
        Thiefplayer02IsReviving = false;
        Thiefplayer03 = null;
        Thiefplayer03CurrentTarget = null;
        Thiefplayer03IsStealing = false;
        Thiefplayer03JewelId = 0;
        Thiefplayer03IsReviving = false;
        Thiefplayer04 = null;
        Thiefplayer04CurrentTarget = null;
        Thiefplayer04IsStealing = false;
        Thiefplayer04JewelId = 0;
        Thiefplayer04IsReviving = false;
        Thiefplayer05 = null;
        Thiefplayer05CurrentTarget = null;
        Thiefplayer05IsStealing = false;
        Thiefplayer05JewelId = 0;
        Thiefplayer05IsReviving = false;
        Thiefplayer06 = null;
        Thiefplayer06CurrentTarget = null;
        Thiefplayer06IsStealing = false;
        Thiefplayer06JewelId = 0;
        Thiefplayer06IsReviving = false;
        Thiefplayer07 = null;
        Thiefplayer07CurrentTarget = null;
        Thiefplayer07IsStealing = false;
        Thiefplayer07JewelId = 0;
        Thiefplayer07IsReviving = false;
        Thiefplayer08 = null;
        Thiefplayer08CurrentTarget = null;
        Thiefplayer08IsStealing = false;
        Thiefplayer08JewelId = 0;
        Thiefplayer08IsReviving = false;
        Thiefplayer09 = null;
        Thiefplayer09CurrentTarget = null;
        Thiefplayer09IsStealing = false;
        Thiefplayer09JewelId = 0;
        Thiefplayer09IsReviving = false;

        PoliceTeam.Clear();
        Policeplayer01 = null;
        Policeplayer01CurrentTarget = null;
        Policeplayer01TargetedPlayer = null;
        Policeplayer01LightTimer = 0;
        Policeplayer01IsReviving = false;
        Policeplayer02 = null;
        Policeplayer02CurrentTarget = null;
        Policeplayer02LightTimer = 0;
        Policeplayer02IsReviving = false;
        Policeplayer02Taser = null;
        Policeplayer02MouseAngle = 0f;
        Policeplayer03 = null;
        Policeplayer03CurrentTarget = null;
        Policeplayer03TargetedPlayer = null;
        Policeplayer03LightTimer = 0;
        Policeplayer03IsReviving = false;
        Policeplayer04 = null;
        Policeplayer04CurrentTarget = null;
        Policeplayer04LightTimer = 0;
        Policeplayer04IsReviving = false;
        Policeplayer04Taser = null;
        Policeplayer04MouseAngle = 0f;
        Policeplayer05 = null;
        Policeplayer05CurrentTarget = null;
        Policeplayer05TargetedPlayer = null;
        Policeplayer05LightTimer = 0;
        Policeplayer05IsReviving = false;
        Policeplayer06 = null;
        Policeplayer06CurrentTarget = null;
        Policeplayer06TargetedPlayer = null;
        Policeplayer06LightTimer = 0;
        Policeplayer06IsReviving = false;

        Jewel01 = null;
        Jewel01BeingStealed = null;
        Jewel02 = null;
        Jewel02BeingStealed = null;
        Jewel03 = null;
        Jewel03BeingStealed = null;
        Jewel04 = null;
        Jewel04BeingStealed = null;
        Jewel05 = null;
        Jewel05BeingStealed = null;
        Jewel06 = null;
        Jewel06BeingStealed = null;
        Jewel07 = null;
        Jewel07BeingStealed = null;
        Jewel08 = null;
        Jewel08BeingStealed = null;
        Jewel09 = null;
        Jewel09BeingStealed = null;
        Jewel10 = null;
        Jewel10BeingStealed = null;
        Jewel11 = null;
        Jewel11BeingStealed = null;
        Jewel12 = null;
        Jewel12BeingStealed = null;
        Jewel13 = null;
        Jewel13BeingStealed = null;
        Jewel14 = null;
        Jewel14BeingStealed = null;
        Jewel15 = null;
        Jewel15BeingStealed = null;

        LocalThiefReleaseArrow = [];
        LocalThiefDeliverArrow = [];

        RequiredJewels = CustomOptionHolder.ThiefModerequiredJewels.GetFloat();
        PoliceTaseCooldown = CustomOptionHolder.ThiefModePoliceTaseCooldown.GetFloat();
        PoliceTaseDuration = CustomOptionHolder.ThiefModePoliceTaseDuration.GetFloat();
        PoliceCanSeeJewels = CustomOptionHolder.ThiefModePoliceCanSeeJewels.GetBool();
        PoliceCatchCooldown = CustomOptionHolder.ThiefModePoliceCatchCooldown.GetFloat();
        CaptureThiefTime = CustomOptionHolder.ThiefModecaptureThiefTime.GetFloat();
        WhoCanThiefsKill = CustomOptionHolder.ThiefModeWhoCanThiefsKill.GetSelection();
        CurrentJewelsStoled = 0;
        TriggerThiefWin = false;
        TriggerPoliceWin = false;
        CurrentThiefsCaptured = 0;
        ThiefpointCounter = new StringBuilder(Tr.Get(TrKey.StolenJewels)).Append("<color=#00F7FFFF>").Append(CurrentJewelsStoled).Append(" / ").Append(RequiredJewels).Append("</color> | ").Append(Tr.Get(TrKey.CapturedThieves)).Append("<color=#928B55FF>").Append(CurrentThiefsCaptured).Append(" / ").Append(ThiefTeam.Count).Append("</color>").ToString();
    }

    public static PlayerControl GetTasedPlayer(float shotSize, float effectiveRange, bool policeTwo)
    {
        PlayerControl result = null;
        var num = effectiveRange;
        Vector3 pos;
        float mouseAngle;
        if (policeTwo)
            mouseAngle = Policeplayer02MouseAngle;
        else
            mouseAngle = Policeplayer04MouseAngle;
        foreach (var player in ThiefTeam)
        {
            if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId) continue;

            if (player.Data.IsDead || player.inVent) continue;

            pos = player.transform.position - PlayerControl.LocalPlayer.transform.position;
            pos = new((pos.x * MathF.Cos(mouseAngle)) + (pos.y * MathF.Sin(mouseAngle)), (pos.y * MathF.Cos(mouseAngle)) - (pos.x * MathF.Sin(mouseAngle)));
            if (Math.Abs(pos.y) < shotSize && !(pos.x < 0) && pos.x < num)
            {
                num = pos.x;
                result = player;
            }
        }

        return result;
    }

    public static void CreatePat()
    {
        var policeTeamPos = new Vector3();
        var thiefTeamPos = new Vector3();
        var cellPos = new Vector3();
        var cellButtonPos = new Vector3();
        var jewelButtonPos = new Vector3();
        var thiefSpaceShipPos = new Vector3();
        var jewel01Pos = new Vector3();
        var jewel02Pos = new Vector3();
        var jewel03Pos = new Vector3();
        var jewel04Pos = new Vector3();
        var jewel05Pos = new Vector3();
        var jewel06Pos = new Vector3();
        var jewel07Pos = new Vector3();
        var jewel08Pos = new Vector3();
        var jewel09Pos = new Vector3();
        var jewel10Pos = new Vector3();
        var jewel11Pos = new Vector3();
        var jewel12Pos = new Vector3();
        var jewel13Pos = new Vector3();
        var jewel14Pos = new Vector3();
        var jewel15Pos = new Vector3();

        switch (GameOptionsManager.Instance.currentGameOptions.MapId)
        {
            // Skeld / Custom Skeld
            case 0:
                if (RebuildUs.ActivatedDleks)
                {
                    policeTeamPos = new(10.2f, 1.18f, PlayerControl.LocalPlayer.transform.position.z);
                    thiefTeamPos = new(1.31f, -16.25f, PlayerControl.LocalPlayer.transform.position.z);
                    cellPos = new(10.25f, 3.38f, 0.5f);
                    cellButtonPos = new(10.2f, 0.93f, 0.5f);
                    jewelButtonPos = new(-0.20f, -17.15f, 0.5f);
                    thiefSpaceShipPos = new(1.345f, -19.16f, 0.6f);
                    var thiefspaceshiphatchDleks = GameObject.Instantiate(AssetLoader.Thiefspaceshiphatch, PlayerControl.LocalPlayer.transform.parent);
                    thiefspaceshiphatchDleks.name = "thiefspaceshiphatch";
                    thiefspaceshiphatchDleks.transform.position = new(1.345f, -19.16f, 0.6f);
                    jewel01Pos = new(18.65f, -9.9f, 1f);
                    jewel02Pos = new(21.5f, -2, 1f);
                    jewel03Pos = new(5.9f, -8.25f, 1f);
                    jewel04Pos = new(-4.5f, -7.5f, 1f);
                    jewel05Pos = new(-7.85f, -14.45f, 1f);
                    jewel06Pos = new(-6.65f, -4.8f, 1f);
                    jewel07Pos = new(-10.5f, 2.15f, 1f);
                    jewel08Pos = new(5.5f, 3.5f, 1f);
                    jewel09Pos = new(19, -1.2f, 1f);
                    jewel10Pos = new(21.5f, -8.35f, 1f);
                    jewel11Pos = new(12.5f, -3.75f, 1f);
                    jewel12Pos = new(5.9f, -5.25f, 1f);
                    jewel13Pos = new(-2.65f, -16.5f, 1f);
                    jewel14Pos = new(-16.75f, -4.75f, 1f);
                    jewel15Pos = new(-3.8f, 3.5f, 1f);
                }
                else
                {
                    policeTeamPos = new(-10.2f, 1.18f, PlayerControl.LocalPlayer.transform.position.z);
                    thiefTeamPos = new(-1.31f, -16.25f, PlayerControl.LocalPlayer.transform.position.z);
                    cellPos = new(-10.25f, 3.38f, 0.5f);
                    cellButtonPos = new(-10.2f, 0.93f, 0.5f);
                    jewelButtonPos = new(0.20f, -17.15f, 0.5f);
                    thiefSpaceShipPos = new(1.765f, -19.16f, 0.6f);
                    var thiefspaceshiphatch = GameObject.Instantiate(AssetLoader.Thiefspaceshiphatch, PlayerControl.LocalPlayer.transform.parent);
                    thiefspaceshiphatch.name = "thiefspaceshiphatch";
                    thiefspaceshiphatch.transform.position = new(1.765f, -19.16f, 0.6f);
                    jewel01Pos = new(-18.65f, -9.9f, 1f);
                    jewel02Pos = new(-21.5f, -2, 1f);
                    jewel03Pos = new(-5.9f, -8.25f, 1f);
                    jewel04Pos = new(4.5f, -7.5f, 1f);
                    jewel05Pos = new(7.85f, -14.45f, 1f);
                    jewel06Pos = new(6.65f, -4.8f, 1f);
                    jewel07Pos = new(10.5f, 2.15f, 1f);
                    jewel08Pos = new(-5.5f, 3.5f, 1f);
                    jewel09Pos = new(-19, -1.2f, 1f);
                    jewel10Pos = new(-21.5f, -8.35f, 1f);
                    jewel11Pos = new(-12.5f, -3.75f, 1f);
                    jewel12Pos = new(-5.9f, -5.25f, 1f);
                    jewel13Pos = new(2.65f, -16.5f, 1f);
                    jewel14Pos = new(16.75f, -4.75f, 1f);
                    jewel15Pos = new(3.8f, 3.5f, 1f);
                }

                break;
            // Mira HQ
            case 1:
                policeTeamPos = new(1.8f, -1f, PlayerControl.LocalPlayer.transform.position.z);
                thiefTeamPos = new(17.75f, 11.5f, PlayerControl.LocalPlayer.transform.position.z);
                cellPos = new(1.75f, 1.125f, 0.5f);
                cellButtonPos = new(1.8f, -1.25f, 0.5f);
                jewelButtonPos = new(18.5f, 13.85f, 0.5f);
                thiefSpaceShipPos = new(21.4f, 14.2f, 0.6f);
                jewel01Pos = new(-4.5f, 2.5f, 1f);
                jewel02Pos = new(6.25f, 14f, 1f);
                jewel03Pos = new(9.15f, 4.75f, 1f);
                jewel04Pos = new(14.75f, 20.5f, 1f);
                jewel05Pos = new(19.5f, 17.5f, 1f);
                jewel06Pos = new(21, 24.1f, 1f);
                jewel07Pos = new(19.5f, 4.75f, 1f);
                jewel08Pos = new(28.25f, 0, 1f);
                jewel09Pos = new(2.45f, 11.25f, 1f);
                jewel10Pos = new(4.4f, 1.75f, 1f);
                jewel11Pos = new(9.25f, 13f, 1f);
                jewel12Pos = new(13.75f, 23.5f, 1f);
                jewel13Pos = new(16, 4, 1f);
                jewel14Pos = new(15.35f, -0.9f, 1f);
                jewel15Pos = new(19.5f, -1.75f, 1f);
                break;
            // Polus
            case 2:
                policeTeamPos = new(8.18f, -7.4f, PlayerControl.LocalPlayer.transform.position.z);
                thiefTeamPos = new(30f, -15.75f, PlayerControl.LocalPlayer.transform.position.z);
                cellPos = new(8.25f, -5.15f, 0.5f);
                cellButtonPos = new(8.2f, -7.5f, 0.5f);
                jewelButtonPos = new(32.25f, -15.9f, 0.5f);
                thiefSpaceShipPos = new(35.35f, -15.55f, 0.8f);
                jewel01Pos = new(16.7f, -2.65f, 0.75f);
                jewel02Pos = new(25.35f, -7.35f, 0.75f);
                jewel03Pos = new(34.9f, -9.75f, 0.75f);
                jewel04Pos = new(36.5f, -21.75f, 0.75f);
                jewel05Pos = new(17.25f, -17.5f, 0.75f);
                jewel06Pos = new(10.9f, -20.5f, -0.75f);
                jewel07Pos = new(1.5f, -20.25f, 0.75f);
                jewel08Pos = new(3f, -12f, 0.75f);
                jewel09Pos = new(30f, -7.35f, 0.75f);
                jewel10Pos = new(40.25f, -8f, 0.75f);
                jewel11Pos = new(26f, -17.15f, 0.75f);
                jewel12Pos = new(22f, -25.25f, 0.75f);
                jewel13Pos = new(20.65f, -12f, 0.75f);
                jewel14Pos = new(9.75f, -12.25f, 0.75f);
                jewel15Pos = new(2.25f, -24f, 0.75f);
                break;
            // Dleks
            case 3:
                policeTeamPos = new(10.2f, 1.18f, PlayerControl.LocalPlayer.transform.position.z);
                thiefTeamPos = new(1.31f, -16.25f, PlayerControl.LocalPlayer.transform.position.z);
                cellPos = new(10.25f, 3.38f, 0.5f);
                cellButtonPos = new(10.2f, 0.93f, 0.5f);
                jewelButtonPos = new(-0.20f, -17.15f, 0.5f);
                thiefSpaceShipPos = new(1.345f, -19.16f, 0.6f);
                var thiefspaceshiphatchdleks = GameObject.Instantiate(AssetLoader.Thiefspaceshiphatch, PlayerControl.LocalPlayer.transform.parent);
                thiefspaceshiphatchdleks.name = "thiefspaceshiphatch";
                thiefspaceshiphatchdleks.transform.position = new(1.345f, -19.16f, 0.6f);
                jewel01Pos = new(18.65f, -9.9f, 1f);
                jewel02Pos = new(21.5f, -2, 1f);
                jewel03Pos = new(5.9f, -8.25f, 1f);
                jewel04Pos = new(-4.5f, -7.5f, 1f);
                jewel05Pos = new(-7.85f, -14.45f, 1f);
                jewel06Pos = new(-6.65f, -4.8f, 1f);
                jewel07Pos = new(-10.5f, 2.15f, 1f);
                jewel08Pos = new(5.5f, 3.5f, 1f);
                jewel09Pos = new(19, -1.2f, 1f);
                jewel10Pos = new(21.5f, -8.35f, 1f);
                jewel11Pos = new(12.5f, -3.75f, 1f);
                jewel12Pos = new(5.9f, -5.25f, 1f);
                jewel13Pos = new(-2.65f, -16.5f, 1f);
                jewel14Pos = new(-16.75f, -4.75f, 1f);
                jewel15Pos = new(-3.8f, 3.5f, 1f);
                break;
            // Airship
            case 4:
                policeTeamPos = new(-18.5f, 0.75f, PlayerControl.LocalPlayer.transform.position.z);
                thiefTeamPos = new(7.15f, -14.5f, PlayerControl.LocalPlayer.transform.position.z);
                cellPos = new(-18.45f, 3.55f, 0.5f);
                cellButtonPos = new(-18.5f, 0.5f, 0.5f);
                jewelButtonPos = new(10.275f, -16.3f, -0.01f);
                thiefSpaceShipPos = new(13.5f, -16f, 0.6f);
                jewel01Pos = new(-23.5f, -1.5f, 1f);
                jewel02Pos = new(-14.15f, -4.85f, 1f);
                jewel03Pos = new(-13.9f, -16.25f, 1f);
                jewel04Pos = new(-0.85f, -2.5f, 1f);
                jewel05Pos = new(-5, 8.5f, 1f);
                jewel06Pos = new(19.3f, -4.15f, 1f);
                jewel07Pos = new(19.85f, 8, 1f);
                jewel08Pos = new(28.85f, -1.75f, 1f);
                jewel09Pos = new(-14.5f, -8.5f, 1f);
                jewel10Pos = new(6.3f, -2.75f, 1f);
                jewel11Pos = new(20.75f, 2.5f, 1f);
                jewel12Pos = new(29.25f, 7, 1f);
                jewel13Pos = new(37.5f, -3.5f, 1f);
                jewel14Pos = new(25.2f, -8.75f, 1f);
                jewel15Pos = new(16.3f, -11, 1f);
                break;
            // Fungle
            case 5:
                policeTeamPos = new(-22.5f, -0.5f, PlayerControl.LocalPlayer.transform.position.z);
                thiefTeamPos = new(20f, 11f, PlayerControl.LocalPlayer.transform.position.z);
                cellPos = new(-26.75f, -0.65f, 0.5f);
                cellButtonPos = new(-24f, -0.5f, 0.5f);
                jewelButtonPos = new(18f, 11.75f, 0.05f);
                thiefSpaceShipPos = new(19f, 9.25f, 0.6f);
                jewel01Pos = new(-18.25f, 5f, 1f);
                jewel02Pos = new(-22.65f, -7.15f, 1f);
                jewel03Pos = new(2, 4.35f, 1f);
                jewel04Pos = new(-3.15f, -10.5f, 0.9f);
                jewel05Pos = new(23.7f, -7.8f, 1f);
                jewel06Pos = new(-4.75f, -1.75f, 1f);
                jewel07Pos = new(8f, -10f, 1f);
                jewel08Pos = new(7f, 1.75f, 1f);
                jewel09Pos = new(13.25f, 10, 1f);
                jewel10Pos = new(22.3f, 3.3f, 1f);
                jewel11Pos = new(20.5f, 7.35f, 1f);
                jewel12Pos = new(24.15f, 14.45f, 1f);
                jewel13Pos = new(-16.12f, 0.7f, 1f);
                jewel14Pos = new(1.65f, -1.5f, 1f);
                jewel15Pos = new(10.5f, -12, 1f);
                break;
            // Submerged
            case 6:
                policeTeamPos = new(-8.45f, 27f, PlayerControl.LocalPlayer.transform.position.z);
                thiefTeamPos = new(1f, 10f, PlayerControl.LocalPlayer.transform.position.z);
                cellPos = new(-5.9f, 31.85f, 0.5f);
                cellButtonPos = new(-6f, 28.5f, 0.03f);
                jewelButtonPos = new(1f, 10f, 0.03f);
                thiefSpaceShipPos = new(14.5f, -35f, -0.011f);
                jewel01Pos = new(-15f, 17.5f, -1f);
                jewel02Pos = new(8f, 32f, -1f);
                jewel03Pos = new(-6.75f, 10f, -1f);
                jewel04Pos = new(5.15f, 8f, -1f);
                jewel05Pos = new(5f, -33.5f, -1f);
                jewel06Pos = new(-4.15f, -33.5f, -1f);
                jewel07Pos = new(-14f, -27.75f, -1f);
                jewel08Pos = new(7.8f, -23.75f, -1f);
                jewel09Pos = new(-6.75f, -42.75f, -1f);
                jewel10Pos = new(13f, -25.25f, -1f);
                jewel11Pos = new(-14f, -34.25f, -1f);
                jewel12Pos = new(0f, -33.5f, -1f);
                jewel13Pos = new(-6.5f, 14f, -1f);
                jewel14Pos = new(14.25f, 24.5f, -1f);
                jewel15Pos = new(-12.25f, 31f, -1f);

                // Add another cell and deliver point on each floor
                var celltwo = GameObject.Instantiate(AssetLoader.Cell, PlayerControl.LocalPlayer.transform.parent);
                celltwo.name = "celltwo";
                celltwo.transform.position = new(-14.1f, -39f, -0.01f);
                celltwo.gameObject.layer = 9;
                celltwo.transform.GetChild(0).gameObject.layer = 9;
                Celltwo = celltwo;
                var cellbuttontwo = GameObject.Instantiate(AssetLoader.FreeThiefButton, PlayerControl.LocalPlayer.transform.parent);
                cellbuttontwo.name = "cellbuttontwo";
                cellbuttontwo.transform.position = new(-11f, -39.35f, -0.01f);
                Cellbuttontwo = cellbuttontwo;
                var jewelbuttontwo = GameObject.Instantiate(AssetLoader.Jewelbutton, PlayerControl.LocalPlayer.transform.parent);
                jewelbuttontwo.name = "jewelbuttontwo";
                jewelbuttontwo.transform.position = new(13f, -32.5f, -0.01f);
                Jewelbuttontwo = jewelbuttontwo;
                var thiefspaceshiptwo = GameObject.Instantiate(AssetLoader.Thiefspaceship, PlayerControl.LocalPlayer.transform.parent);
                thiefspaceshiptwo.name = "thiefspaceshiptwo";
                thiefspaceshiptwo.transform.position = new(-2.75f, 9f, 0.031f);
                thiefspaceshiptwo.transform.localScale = new(-1f, 1f, 1f);
                break;
        }

        foreach (var player in PoliceTeam) player.transform.position = policeTeamPos;

        foreach (var player in ThiefTeam)
        {
            player.transform.position = thiefTeamPos;
            if (player == PlayerControl.LocalPlayer)
            {
                // Add Arrows pointing the release and deliver point
                if (LocalThiefReleaseArrow.Count == 0)
                {
                    LocalThiefReleaseArrow.Add(new(Palette.PlayerColors[10]));
                    LocalThiefReleaseArrow[0].ArrowObject.SetActive(true);
                    if (GameOptionsManager.Instance.currentGameOptions.MapId == 6)
                    {
                        LocalThiefReleaseArrow.Add(new(Palette.PlayerColors[10]));
                        LocalThiefReleaseArrow[1].ArrowObject.SetActive(true);
                    }
                }

                if (LocalThiefDeliverArrow.Count == 0)
                {
                    LocalThiefDeliverArrow.Add(new(Palette.PlayerColors[16]));
                    LocalThiefDeliverArrow[0].ArrowObject.SetActive(true);
                    if (GameOptionsManager.Instance.currentGameOptions.MapId == 6)
                    {
                        LocalThiefDeliverArrow.Add(new(Palette.PlayerColors[16]));
                        LocalThiefDeliverArrow[1].ArrowObject.SetActive(true);
                    }
                }
            }
        }

        if (PlayerControl.LocalPlayer != null && !Createdpoliceandthief)
        {
            PlayerControl.LocalPlayer.ClearAllTasks();

            var cell = GameObject.Instantiate(AssetLoader.Cell, PlayerControl.LocalPlayer.transform.parent);
            cell.name = "cell";
            cell.transform.position = cellPos;
            cell.gameObject.layer = 9;
            cell.transform.GetChild(0).gameObject.layer = 9;
            Cell = cell;
            var cellbutton = GameObject.Instantiate(AssetLoader.FreeThiefButton, PlayerControl.LocalPlayer.transform.parent);
            cellbutton.name = "cellbutton";
            cellbutton.transform.position = cellButtonPos;
            Cellbutton = cellbutton;
            var jewelbutton = GameObject.Instantiate(AssetLoader.Jewelbutton, PlayerControl.LocalPlayer.transform.parent);
            jewelbutton.name = "jewelbutton";
            jewelbutton.transform.position = jewelButtonPos;
            Jewelbutton = jewelbutton;
            var thiefspaceship = GameObject.Instantiate(AssetLoader.Thiefspaceship, PlayerControl.LocalPlayer.transform.parent);
            thiefspaceship.name = "thiefspaceship";
            thiefspaceship.transform.position = thiefSpaceShipPos;

            // Spawn jewels
            var jewel01 = GameObject.Instantiate(AssetLoader.Jeweldiamond, PlayerControl.LocalPlayer.transform.parent);
            jewel01.transform.position = jewel01Pos;
            jewel01.name = "jewel01";
            Jewel01 = jewel01;
            var jewel02 = GameObject.Instantiate(AssetLoader.Jeweldiamond, PlayerControl.LocalPlayer.transform.parent);
            jewel02.transform.position = jewel02Pos;
            jewel02.name = "jewel02";
            Jewel02 = jewel02;
            var jewel03 = GameObject.Instantiate(AssetLoader.Jeweldiamond, PlayerControl.LocalPlayer.transform.parent);
            jewel03.transform.position = jewel03Pos;
            jewel03.name = "jewel03";
            Jewel03 = jewel03;
            var jewel04 = GameObject.Instantiate(AssetLoader.Jeweldiamond, PlayerControl.LocalPlayer.transform.parent);
            jewel04.transform.position = jewel04Pos;
            jewel04.name = "jewel04";
            Jewel04 = jewel04;
            var jewel05 = GameObject.Instantiate(AssetLoader.Jeweldiamond, PlayerControl.LocalPlayer.transform.parent);
            jewel05.transform.position = jewel05Pos;
            jewel05.name = "jewel05";
            Jewel05 = jewel05;
            var jewel06 = GameObject.Instantiate(AssetLoader.Jeweldiamond, PlayerControl.LocalPlayer.transform.parent);
            jewel06.transform.position = jewel06Pos;
            jewel06.name = "jewel06";
            Jewel06 = jewel06;
            var jewel07 = GameObject.Instantiate(AssetLoader.Jeweldiamond, PlayerControl.LocalPlayer.transform.parent);
            jewel07.transform.position = jewel07Pos;
            jewel07.name = "jewel07";
            Jewel07 = jewel07;
            var jewel08 = GameObject.Instantiate(AssetLoader.Jeweldiamond, PlayerControl.LocalPlayer.transform.parent);
            jewel08.transform.position = jewel08Pos;
            jewel08.name = "jewel08";
            Jewel08 = jewel08;
            var jewel09 = GameObject.Instantiate(AssetLoader.Jewelruby, PlayerControl.LocalPlayer.transform.parent);
            jewel09.transform.position = jewel09Pos;
            jewel09.name = "jewel09";
            Jewel09 = jewel09;
            var jewel10 = GameObject.Instantiate(AssetLoader.Jewelruby, PlayerControl.LocalPlayer.transform.parent);
            jewel10.transform.position = jewel10Pos;
            jewel10.name = "jewel10";
            Jewel10 = jewel10;
            var jewel11 = GameObject.Instantiate(AssetLoader.Jewelruby, PlayerControl.LocalPlayer.transform.parent);
            jewel11.transform.position = jewel11Pos;
            jewel11.name = "jewel11";
            Jewel11 = jewel11;
            var jewel12 = GameObject.Instantiate(AssetLoader.Jewelruby, PlayerControl.LocalPlayer.transform.parent);
            jewel12.transform.position = jewel12Pos;
            jewel12.name = "jewel12";
            Jewel12 = jewel12;
            var jewel13 = GameObject.Instantiate(AssetLoader.Jewelruby, PlayerControl.LocalPlayer.transform.parent);
            jewel13.transform.position = jewel13Pos;
            jewel13.name = "jewel13";
            Jewel13 = jewel13;
            var jewel14 = GameObject.Instantiate(AssetLoader.Jewelruby, PlayerControl.LocalPlayer.transform.parent);
            jewel14.transform.position = jewel14Pos;
            jewel14.name = "jewel14";
            Jewel14 = jewel14;
            var jewel15 = GameObject.Instantiate(AssetLoader.Jewelruby, PlayerControl.LocalPlayer.transform.parent);
            jewel15.transform.position = jewel15Pos;
            jewel15.name = "jewel15";
            Jewel15 = jewel15;
            ThiefTreasures.Add(jewel01);
            ThiefTreasures.Add(jewel02);
            ThiefTreasures.Add(jewel03);
            ThiefTreasures.Add(jewel04);
            ThiefTreasures.Add(jewel05);
            ThiefTreasures.Add(jewel06);
            ThiefTreasures.Add(jewel07);
            ThiefTreasures.Add(jewel08);
            ThiefTreasures.Add(jewel09);
            ThiefTreasures.Add(jewel10);
            ThiefTreasures.Add(jewel11);
            ThiefTreasures.Add(jewel12);
            ThiefTreasures.Add(jewel13);
            ThiefTreasures.Add(jewel14);
            ThiefTreasures.Add(jewel15);

            Createdpoliceandthief = true;
        }
    }

    public static void PoliceandThiefSetTarget()
    {
        if (MapSettings.GameMode is not CustomGameMode.PoliceAndThieves) return;

        var untargetablePolice = new List<PlayerControl>();
        foreach (var player in PoliceTeam) untargetablePolice.Add(player);

        // Prevent killing reviving players
        if (Thiefplayer01IsReviving)
            untargetablePolice.Add(Thiefplayer01);
        else
            untargetablePolice.Remove(Thiefplayer01);
        if (Thiefplayer02IsReviving)
            untargetablePolice.Add(Thiefplayer02);
        else
            untargetablePolice.Remove(Thiefplayer02);
        if (Thiefplayer03IsReviving)
            untargetablePolice.Add(Thiefplayer03);
        else
            untargetablePolice.Remove(Thiefplayer03);
        if (Thiefplayer04IsReviving)
            untargetablePolice.Add(Thiefplayer04);
        else
            untargetablePolice.Remove(Thiefplayer04);
        if (Thiefplayer05IsReviving)
            untargetablePolice.Add(Thiefplayer05);
        else
            untargetablePolice.Remove(Thiefplayer05);
        if (Thiefplayer06IsReviving)
            untargetablePolice.Add(Thiefplayer06);
        else
            untargetablePolice.Remove(Thiefplayer06);
        if (Thiefplayer07IsReviving)
            untargetablePolice.Add(Thiefplayer07);
        else
            untargetablePolice.Remove(Thiefplayer07);
        if (Thiefplayer08IsReviving)
            untargetablePolice.Add(Thiefplayer08);
        else
            untargetablePolice.Remove(Thiefplayer08);
        if (Thiefplayer09IsReviving)
            untargetablePolice.Add(Thiefplayer09);
        else
            untargetablePolice.Remove(Thiefplayer09);

        if (Policeplayer01 != null && Policeplayer01 == PlayerControl.LocalPlayer)
        {
            Policeplayer01CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetablePolice);
            Helpers.SetPlayerOutline(Policeplayer01CurrentTarget, PolicePlayerColor);
        }

        if (Policeplayer02 != null && Policeplayer02 == PlayerControl.LocalPlayer)
        {
            Policeplayer02CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetablePolice);
            Helpers.SetPlayerOutline(Policeplayer02CurrentTarget, PolicePlayerColor);
        }

        if (Policeplayer03 != null && Policeplayer03 == PlayerControl.LocalPlayer)
        {
            Policeplayer03CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetablePolice);
            Helpers.SetPlayerOutline(Policeplayer03CurrentTarget, PolicePlayerColor);
        }

        if (Policeplayer04 != null && Policeplayer04 == PlayerControl.LocalPlayer)
        {
            Policeplayer04CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetablePolice);
            Helpers.SetPlayerOutline(Policeplayer04CurrentTarget, PolicePlayerColor);
        }

        if (Policeplayer05 != null && Policeplayer05 == PlayerControl.LocalPlayer)
        {
            Policeplayer05CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetablePolice);
            Helpers.SetPlayerOutline(Policeplayer05CurrentTarget, PolicePlayerColor);
        }

        if (Policeplayer06 != null && Policeplayer06 == PlayerControl.LocalPlayer)
        {
            Policeplayer06CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetablePolice);
            Helpers.SetPlayerOutline(Policeplayer06CurrentTarget, PolicePlayerColor);
        }

        var untargetableThiefs = new List<PlayerControl>();
        foreach (var player in ThiefTeam) untargetableThiefs.Add(player);

        // Prevent killing reviving players
        if (Policeplayer01IsReviving)
            untargetableThiefs.Add(Policeplayer01);
        else
            untargetableThiefs.Remove(Policeplayer01);
        if (Policeplayer02IsReviving)
            untargetableThiefs.Add(Policeplayer02);
        else
            untargetableThiefs.Remove(Policeplayer02);
        if (Policeplayer03IsReviving)
            untargetableThiefs.Add(Policeplayer03);
        else
            untargetableThiefs.Remove(Policeplayer03);
        if (Policeplayer04IsReviving)
            untargetableThiefs.Add(Policeplayer04);
        else
            untargetableThiefs.Remove(Policeplayer04);
        if (Policeplayer05IsReviving)
            untargetableThiefs.Add(Policeplayer05);
        else
            untargetableThiefs.Remove(Policeplayer05);
        if (Policeplayer06IsReviving)
            untargetableThiefs.Add(Policeplayer06);
        else
            untargetableThiefs.Remove(Policeplayer06);

        if (Thiefplayer01 != null && Thiefplayer01 == PlayerControl.LocalPlayer)
        {
            Thiefplayer01CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetableThiefs);
            Helpers.SetPlayerOutline(Thiefplayer01CurrentTarget, ThiefPlayerColor);
        }

        if (Thiefplayer02 != null && Thiefplayer02 == PlayerControl.LocalPlayer)
        {
            Thiefplayer02CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetableThiefs);
            Helpers.SetPlayerOutline(Thiefplayer02CurrentTarget, ThiefPlayerColor);
        }

        if (Thiefplayer03 != null && Thiefplayer03 == PlayerControl.LocalPlayer)
        {
            Thiefplayer03CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetableThiefs);
            Helpers.SetPlayerOutline(Thiefplayer03CurrentTarget, ThiefPlayerColor);
        }

        if (Thiefplayer04 != null && Thiefplayer04 == PlayerControl.LocalPlayer)
        {
            Thiefplayer04CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetableThiefs);
            Helpers.SetPlayerOutline(Thiefplayer04CurrentTarget, ThiefPlayerColor);
        }

        if (Thiefplayer05 != null && Thiefplayer05 == PlayerControl.LocalPlayer)
        {
            Thiefplayer05CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetableThiefs);
            Helpers.SetPlayerOutline(Thiefplayer05CurrentTarget, ThiefPlayerColor);
        }

        if (Thiefplayer06 != null && Thiefplayer06 == PlayerControl.LocalPlayer)
        {
            Thiefplayer06CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetableThiefs);
            Helpers.SetPlayerOutline(Thiefplayer06CurrentTarget, ThiefPlayerColor);
        }

        if (Thiefplayer07 != null && Thiefplayer07 == PlayerControl.LocalPlayer)
        {
            Thiefplayer07CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetableThiefs);
            Helpers.SetPlayerOutline(Thiefplayer07CurrentTarget, ThiefPlayerColor);
        }

        if (Thiefplayer08 != null && Thiefplayer08 == PlayerControl.LocalPlayer)
        {
            Thiefplayer08CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetableThiefs);
            Helpers.SetPlayerOutline(Thiefplayer08CurrentTarget, ThiefPlayerColor);
        }

        if (Thiefplayer09 != null && Thiefplayer09 == PlayerControl.LocalPlayer)
        {
            Thiefplayer09CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetableThiefs);
            Helpers.SetPlayerOutline(Thiefplayer09CurrentTarget, ThiefPlayerColor);
        }
    }

    public static void OnMurderPlayerPostfix(PlayerControl __instance, PlayerControl target)
    {
        foreach (var player in PoliceTeam)
        {
            if (player.PlayerId == target.PlayerId)
            {
                var ptBody = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == target.PlayerId);
                ptBody.transform.position = new(50, 50, 1);
                if (Policeplayer01 != null && target.PlayerId == Policeplayer01.PlayerId)
                    Policeplayer01IsReviving = true;
                else if (Policeplayer02 != null && target.PlayerId == Policeplayer02.PlayerId)
                    Policeplayer02IsReviving = true;
                else if (Policeplayer03 != null && target.PlayerId == Policeplayer03.PlayerId)
                    Policeplayer03IsReviving = true;
                else if (Policeplayer04 != null && target.PlayerId == Policeplayer04.PlayerId)
                    Policeplayer04IsReviving = true;
                else if (Policeplayer05 != null && target.PlayerId == Policeplayer05.PlayerId)
                    Policeplayer05IsReviving = true;
                else if (Policeplayer06 != null && target.PlayerId == Policeplayer06.PlayerId) Policeplayer06IsReviving = true;
                Helpers.AlphaPlayer(true, player.PlayerId);
                HudManager.Instance.StartCoroutine(Effects.Lerp(MapSettings.GamemodeInvincibilityTime, new Action<float>(p =>
                {
                    if (p == 1f && player != null)
                    {
                        if (Policeplayer01 != null && target.PlayerId == Policeplayer01.PlayerId)
                            Policeplayer01IsReviving = false;
                        else if (Policeplayer02 != null && target.PlayerId == Policeplayer02.PlayerId)
                            Policeplayer02IsReviving = false;
                        else if (Policeplayer03 != null && target.PlayerId == Policeplayer03.PlayerId)
                            Policeplayer03IsReviving = false;
                        else if (Policeplayer04 != null && target.PlayerId == Policeplayer04.PlayerId)
                            Policeplayer04IsReviving = false;
                        else if (Policeplayer05 != null && target.PlayerId == Policeplayer05.PlayerId)
                            Policeplayer05IsReviving = false;
                        else if (Policeplayer06 != null && target.PlayerId == Policeplayer06.PlayerId) Policeplayer06IsReviving = false;
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
                                if (RebuildUs.ActivatedDleks)
                                    player.transform.position = new(10.2f, 1.18f, player.transform.position.z);
                                else
                                    player.transform.position = new(-10.2f, 1.18f, player.transform.position.z);
                                break;
                            // MiraHQ
                            case 1:
                                player.transform.position = new(1.8f, -1f, player.transform.position.z);
                                break;
                            // Polus
                            case 2:
                                player.transform.position = new(8.18f, -7.4f, player.transform.position.z);
                                break;
                            // Dleks
                            case 3:
                                player.transform.position = new(10.2f, 1.18f, player.transform.position.z);
                                break;
                            // Airship
                            case 4:
                                player.transform.position = new(-18.5f, 0.75f, player.transform.position.z);
                                break;
                            // Fungle
                            case 5:
                                player.transform.position = new(-22.5f, -0.5f, player.transform.position.z);
                                break;
                            // Submerged
                            case 6:
                                if (player.transform.position.y > 0)
                                    player.transform.position = new(-8.45f, 27f, player.transform.position.z);
                                else
                                    player.transform.position = new(-9.25f, -41.25f, player.transform.position.z);
                                break;
                        }

                        var deadPlayerEntry = GameHistory.DEAD_PLAYERS.Where(x => x.Player.PlayerId == target.PlayerId).FirstOrDefault();
                        if (ptBody != null) Object.Destroy(ptBody.gameObject);
                        if (deadPlayerEntry != null) GameHistory.DEAD_PLAYERS.Remove(deadPlayerEntry);
                    }
                })));
            }
        }

        foreach (var player in ThiefTeam)
        {
            if (player.PlayerId == target.PlayerId)
            {
                var ptBody = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == target.PlayerId);
                ptBody.transform.position = new(50, 50, 1);
                if (Thiefplayer01 != null && target.PlayerId == Thiefplayer01.PlayerId)
                {
                    if (Thiefplayer01IsStealing) RPCProcedure.PoliceandThiefRevertedJewelPosition(target.PlayerId, Thiefplayer01JewelId);
                    Thiefplayer01IsReviving = true;
                }
                else if (Thiefplayer02 != null && target.PlayerId == Thiefplayer02.PlayerId)
                {
                    if (Thiefplayer02IsStealing) RPCProcedure.PoliceandThiefRevertedJewelPosition(target.PlayerId, Thiefplayer02JewelId);
                    Thiefplayer02IsReviving = true;
                }
                else if (Thiefplayer03 != null && target.PlayerId == Thiefplayer03.PlayerId)
                {
                    if (Thiefplayer03IsStealing) RPCProcedure.PoliceandThiefRevertedJewelPosition(target.PlayerId, Thiefplayer03JewelId);
                    Thiefplayer03IsReviving = true;
                }
                else if (Thiefplayer04 != null && target.PlayerId == Thiefplayer04.PlayerId)
                {
                    if (Thiefplayer04IsStealing) RPCProcedure.PoliceandThiefRevertedJewelPosition(target.PlayerId, Thiefplayer04JewelId);
                    Thiefplayer04IsReviving = true;
                }
                else if (Thiefplayer05 != null && target.PlayerId == Thiefplayer05.PlayerId)
                {
                    if (Thiefplayer05IsStealing) RPCProcedure.PoliceandThiefRevertedJewelPosition(target.PlayerId, Thiefplayer05JewelId);
                    Thiefplayer05IsReviving = true;
                }
                else if (Thiefplayer06 != null && target.PlayerId == Thiefplayer06.PlayerId)
                {
                    if (Thiefplayer06IsStealing) RPCProcedure.PoliceandThiefRevertedJewelPosition(target.PlayerId, Thiefplayer06JewelId);
                    Thiefplayer06IsReviving = true;
                }
                else if (Thiefplayer07 != null && target.PlayerId == Thiefplayer07.PlayerId)
                {
                    if (Thiefplayer07IsStealing) RPCProcedure.PoliceandThiefRevertedJewelPosition(target.PlayerId, Thiefplayer07JewelId);
                    Thiefplayer07IsReviving = true;
                }
                else if (Thiefplayer08 != null && target.PlayerId == Thiefplayer08.PlayerId)
                {
                    if (Thiefplayer08IsStealing) RPCProcedure.PoliceandThiefRevertedJewelPosition(target.PlayerId, Thiefplayer08JewelId);
                    Thiefplayer08IsReviving = true;
                }
                else if (Thiefplayer09 != null && target.PlayerId == Thiefplayer09.PlayerId)
                {
                    if (Thiefplayer09IsStealing) RPCProcedure.PoliceandThiefRevertedJewelPosition(target.PlayerId, Thiefplayer09JewelId);
                    Thiefplayer09IsReviving = true;
                }

                Helpers.AlphaPlayer(true, player.PlayerId);
                HudManager.Instance.StartCoroutine(Effects.Lerp(MapSettings.GamemodeReviveTime * 1.25f, new Action<float>(p =>
                {
                    if (p == 1f && player != null)
                    {
                        if (Thiefplayer01 != null && target.PlayerId == Thiefplayer01.PlayerId)
                            Thiefplayer01IsReviving = false;
                        else if (Thiefplayer02 != null && target.PlayerId == Thiefplayer02.PlayerId)
                            Thiefplayer02IsReviving = false;
                        else if (Thiefplayer03 != null && target.PlayerId == Thiefplayer03.PlayerId)
                            Thiefplayer03IsReviving = false;
                        else if (Thiefplayer04 != null && target.PlayerId == Thiefplayer04.PlayerId)
                            Thiefplayer04IsReviving = false;
                        else if (Thiefplayer05 != null && target.PlayerId == Thiefplayer05.PlayerId)
                            Thiefplayer05IsReviving = false;
                        else if (Thiefplayer06 != null && target.PlayerId == Thiefplayer06.PlayerId)
                            Thiefplayer06IsReviving = false;
                        else if (Thiefplayer07 != null && target.PlayerId == Thiefplayer07.PlayerId)
                            Thiefplayer07IsReviving = false;
                        else if (Thiefplayer08 != null && target.PlayerId == Thiefplayer08.PlayerId)
                            Thiefplayer08IsReviving = false;
                        else if (Thiefplayer09 != null && target.PlayerId == Thiefplayer09.PlayerId) Thiefplayer09IsReviving = false;
                        Helpers.AlphaPlayer(false, player.PlayerId);
                    }
                })));

                HudManager.Instance.StartCoroutine(Effects.Lerp((MapSettings.GamemodeReviveTime * 1.25f) - MapSettings.GamemodeInvincibilityTime, new Action<float>(p =>
                {
                    if (p == 1f && player != null)
                    {
                        player.Revive();
                        switch (GameOptionsManager.Instance.currentGameOptions.MapId)
                        {
                            // Skeld
                            case 0:
                                if (RebuildUs.ActivatedDleks)
                                    player.transform.position = new(1.31f, -16.25f, player.transform.position.z);
                                else
                                    player.transform.position = new(-1.31f, -16.25f, player.transform.position.z);
                                break;
                            // MiraHQ
                            case 1:
                                player.transform.position = new(17.75f, 11.5f, player.transform.position.z);
                                break;
                            // Polus
                            case 2:
                                player.transform.position = new(30f, -15.75f, player.transform.position.z);
                                break;
                            // Dleks
                            case 3:
                                player.transform.position = new(1.31f, -16.25f, player.transform.position.z);
                                break;
                            // Airship
                            case 4:
                                player.transform.position = new(7.15f, -14.5f, player.transform.position.z);
                                break;
                            // Fungle
                            case 5:
                                player.transform.position = new(20f, 11f, player.transform.position.z);
                                break;
                            // Submerged
                            case 6:
                                if (player.transform.position.y > 0)
                                    player.transform.position = new(1f, 10f, player.transform.position.z);
                                else
                                    player.transform.position = new(12.5f, -31.75f, player.transform.position.z);
                                break;
                        }

                        var deadPlayerEntry = GameHistory.DEAD_PLAYERS.Where(x => x.Player.PlayerId == target.PlayerId).FirstOrDefault();
                        if (ptBody != null) Object.Destroy(ptBody.gameObject);
                        if (deadPlayerEntry != null) GameHistory.DEAD_PLAYERS.Remove(deadPlayerEntry);
                    }
                })));
            }
        }
    }

    public static void PoliceandthiefUpdate()
    {
        if (MapSettings.GameMode is not CustomGameMode.PoliceAndThieves) return;

        // Check number of thiefs if a thief disconnects
        foreach (var thief in ThiefTeam)
        {
            if (thief.Data.Disconnected)
            {
                if (Thiefplayer01 != null && thief.PlayerId == Thiefplayer01.PlayerId && Thiefplayer01IsStealing)
                {
                    ThiefTeam.Remove(Thiefplayer01);
                    RPCProcedure.PoliceandThiefRevertedJewelPosition(thief.PlayerId, Thiefplayer01JewelId);
                }
                else if (Thiefplayer02 != null && thief.PlayerId == Thiefplayer02.PlayerId && Thiefplayer02IsStealing)
                {
                    ThiefTeam.Remove(Thiefplayer02);
                    RPCProcedure.PoliceandThiefRevertedJewelPosition(thief.PlayerId, Thiefplayer02JewelId);
                }
                else if (Thiefplayer03 != null && thief.PlayerId == Thiefplayer03.PlayerId && Thiefplayer03IsStealing)
                {
                    ThiefTeam.Remove(Thiefplayer03);
                    RPCProcedure.PoliceandThiefRevertedJewelPosition(thief.PlayerId, Thiefplayer03JewelId);
                }
                else if (Thiefplayer04 != null && thief.PlayerId == Thiefplayer04.PlayerId && Thiefplayer04IsStealing)
                {
                    ThiefTeam.Remove(Thiefplayer04);
                    RPCProcedure.PoliceandThiefRevertedJewelPosition(thief.PlayerId, Thiefplayer04JewelId);
                }
                else if (Thiefplayer05 != null && thief.PlayerId == Thiefplayer05.PlayerId && Thiefplayer05IsStealing)
                {
                    ThiefTeam.Remove(Thiefplayer05);
                    RPCProcedure.PoliceandThiefRevertedJewelPosition(thief.PlayerId, Thiefplayer05JewelId);
                }
                else if (Thiefplayer06 != null && thief.PlayerId == Thiefplayer06.PlayerId && Thiefplayer06IsStealing)
                {
                    ThiefTeam.Remove(Thiefplayer06);
                    RPCProcedure.PoliceandThiefRevertedJewelPosition(thief.PlayerId, Thiefplayer06JewelId);
                }
                else if (Thiefplayer07 != null && thief.PlayerId == Thiefplayer07.PlayerId && Thiefplayer07IsStealing)
                {
                    ThiefTeam.Remove(Thiefplayer07);
                    RPCProcedure.PoliceandThiefRevertedJewelPosition(thief.PlayerId, Thiefplayer07JewelId);
                }
                else if (Thiefplayer08 != null && thief.PlayerId == Thiefplayer08.PlayerId && Thiefplayer08IsStealing)
                {
                    ThiefTeam.Remove(Thiefplayer08);
                    RPCProcedure.PoliceandThiefRevertedJewelPosition(thief.PlayerId, Thiefplayer08JewelId);
                }
                else if (Thiefplayer09 != null && thief.PlayerId == Thiefplayer09.PlayerId && Thiefplayer09IsStealing)
                {
                    ThiefTeam.Remove(Thiefplayer09);
                    RPCProcedure.PoliceandThiefRevertedJewelPosition(thief.PlayerId, Thiefplayer09JewelId);
                }

                ThiefpointCounter = new StringBuilder(Tr.Get(TrKey.CapturedThieves)).Append("<color=#00F7FFFF>").Append(CurrentJewelsStoled).Append(" / ").Append(RequiredJewels).Append("</color> | ").Append(Tr.Get(TrKey.CapturedThieves)).Append("<color=#928B55FF>").Append(CurrentThiefsCaptured).Append(" / ").Append(ThiefTeam.Count).Append("</color>").ToString();
                if (CurrentThiefsCaptured == ThiefTeam.Count)
                {
                    TriggerPoliceWin = true;
                    GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ThiefModePoliceWin, false);
                }

                break;
            }
        }

        foreach (var police in PoliceTeam)
        {
            if (police.Data.Disconnected)
            {
                if (Policeplayer01 != null && police.PlayerId == Policeplayer01.PlayerId)
                    PoliceTeam.Remove(Policeplayer01);
                else if (Policeplayer02 != null && police.PlayerId == Policeplayer02.PlayerId)
                    PoliceTeam.Remove(Policeplayer02);
                else if (Policeplayer03 != null && police.PlayerId == Policeplayer03.PlayerId)
                    PoliceTeam.Remove(Policeplayer03);
                else if (Policeplayer04 != null && police.PlayerId == Policeplayer04.PlayerId)
                    PoliceTeam.Remove(Policeplayer04);
                else if (Policeplayer05 != null && police.PlayerId == Policeplayer05.PlayerId)
                    PoliceTeam.Remove(Policeplayer05);
                else if (Policeplayer06 != null && police.PlayerId == Policeplayer06.PlayerId) PoliceTeam.Remove(Policeplayer06);

                if (PoliceTeam.Count <= 0)
                {
                    TriggerThiefWin = true;
                    GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ThiefModeThiefWin, false);
                }

                break;
            }
        }
    }
}
