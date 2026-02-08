namespace RebuildUs.Modules.GameMode;

public static partial class CaptureTheFlag
{
    public static Color IntroColor = new Color32(248, 205, 70, byte.MaxValue);

    public static bool createdcapturetheflag = false;

    public static List<PlayerControl> redteamFlag = [];
    public static PlayerControl redplayer01 = null;
    public static bool redplayer01IsReviving = false;
    public static PlayerControl redplayer01currentTarget = null;
    public static PlayerControl redplayer02 = null;
    public static bool redplayer02IsReviving = false;
    public static PlayerControl redplayer02currentTarget = null;
    public static PlayerControl redplayer03 = null;
    public static bool redplayer03IsReviving = false;
    public static PlayerControl redplayer03currentTarget = null;
    public static PlayerControl redplayer04 = null;
    public static bool redplayer04IsReviving = false;
    public static PlayerControl redplayer04currentTarget = null;
    public static PlayerControl redplayer05 = null;
    public static bool redplayer05IsReviving = false;
    public static PlayerControl redplayer05currentTarget = null;
    public static PlayerControl redplayer06 = null;
    public static bool redplayer06IsReviving = false;
    public static PlayerControl redplayer06currentTarget = null;
    public static PlayerControl redplayer07 = null;
    public static bool redplayer07IsReviving = false;
    public static PlayerControl redplayer07currentTarget = null;

    public static List<PlayerControl> blueteamFlag = [];
    public static PlayerControl blueplayer01 = null;
    public static bool blueplayer01IsReviving = false;
    public static PlayerControl blueplayer01currentTarget = null;
    public static PlayerControl blueplayer02 = null;
    public static bool blueplayer02IsReviving = false;
    public static PlayerControl blueplayer02currentTarget = null;
    public static PlayerControl blueplayer03 = null;
    public static bool blueplayer03IsReviving = false;
    public static PlayerControl blueplayer03currentTarget = null;
    public static PlayerControl blueplayer04 = null;
    public static bool blueplayer04IsReviving = false;
    public static PlayerControl blueplayer04currentTarget = null;
    public static PlayerControl blueplayer05 = null;
    public static bool blueplayer05IsReviving = false;
    public static PlayerControl blueplayer05currentTarget = null;
    public static PlayerControl blueplayer06 = null;
    public static bool blueplayer06IsReviving = false;
    public static PlayerControl blueplayer06currentTarget = null;
    public static PlayerControl blueplayer07 = null;
    public static bool blueplayer07IsReviving = false;
    public static PlayerControl blueplayer07currentTarget = null;
    public static PlayerControl stealerPlayer = null;
    public static bool stealerPlayerIsReviving = false;
    public static PlayerControl stealerPlayercurrentTarget = null;
    public static List<GameObject> stealerSpawns = [];

    public static float requiredFlags = 3;

    public static GameObject redflag = null;
    public static GameObject redflagbase = null;
    public static bool redflagtaken = false;
    public static PlayerControl redPlayerWhoHasBlueFlag = null;
    public static float currentRedTeamPoints = 0;
    public static List<Arrow> localRedFlagArrow = [];

    public static GameObject blueflag = null;
    public static GameObject blueflagbase = null;
    public static bool blueflagtaken = false;
    public static PlayerControl bluePlayerWhoHasRedFlag = null;
    public static float currentBlueTeamPoints = 0;
    public static List<Arrow> localBlueFlagArrow = [];

    public static bool triggerRedTeamWin = false;
    public static bool triggerBlueTeamWin = false;
    public static bool triggerDrawWin = false;

    public static string flagpointCounter = new StringBuilder(Tr.Get(TrKey.Score)).Append("<color=#FF0000FF>").Append(currentRedTeamPoints).Append("</color> - <color=#0000FFFF>").Append(currentBlueTeamPoints).Append("</color>").ToString();

    public static void clearAndReload()
    {
        createdcapturetheflag = false;

        redteamFlag.Clear();
        redplayer01 = null;
        redplayer01currentTarget = null;
        redplayer01IsReviving = false;
        redplayer02 = null;
        redplayer02IsReviving = false;
        redplayer02currentTarget = null;
        redplayer03 = null;
        redplayer03IsReviving = false;
        redplayer03currentTarget = null;
        redplayer04 = null;
        redplayer04IsReviving = false;
        redplayer04currentTarget = null;
        redplayer05 = null;
        redplayer05IsReviving = false;
        redplayer05currentTarget = null;
        redplayer06 = null;
        redplayer06IsReviving = false;
        redplayer06currentTarget = null;
        redplayer07 = null;
        redplayer07IsReviving = false;
        redplayer07currentTarget = null;
        blueteamFlag.Clear();
        blueplayer01 = null;
        blueplayer01IsReviving = false;
        blueplayer01currentTarget = null;
        blueplayer02 = null;
        blueplayer02IsReviving = false;
        blueplayer02currentTarget = null;
        blueplayer03 = null;
        blueplayer03IsReviving = false;
        blueplayer03currentTarget = null;
        blueplayer04 = null;
        blueplayer04IsReviving = false;
        blueplayer04currentTarget = null;
        blueplayer05 = null;
        blueplayer05IsReviving = false;
        blueplayer05currentTarget = null;
        blueplayer06 = null;
        blueplayer06IsReviving = false;
        blueplayer06currentTarget = null;
        blueplayer07 = null;
        blueplayer07IsReviving = false;
        blueplayer07currentTarget = null;
        stealerPlayer = null;
        stealerPlayerIsReviving = false;
        stealerPlayercurrentTarget = null;
        stealerSpawns.Clear();

        requiredFlags = CustomOptionHolder.requiredFlags.GetFloat();
        redflag = null;
        redflagbase = null;
        redflagtaken = false;
        redPlayerWhoHasBlueFlag = null;
        currentRedTeamPoints = 0;
        blueflag = null;
        blueflagbase = null;
        blueflagtaken = false;
        bluePlayerWhoHasRedFlag = null;
        triggerRedTeamWin = false;
        triggerBlueTeamWin = false;
        triggerDrawWin = false;
        currentBlueTeamPoints = 0;
        localRedFlagArrow = [];
        localBlueFlagArrow = [];
        flagpointCounter = new StringBuilder(Tr.Get(TrKey.Score)).Append("<color=#FF0000FF>").Append(currentRedTeamPoints).Append("</color> - <color=#0000FFFF>").Append(currentBlueTeamPoints).Append("</color>").ToString();
    }

    public static void CreateCTF()
    {
        Vector3 stealerPlayerPos = new Vector3();
        Vector3 redTeamPos = new Vector3();
        Vector3 blueTeamPos = new Vector3();
        Vector3 redFlagPos = new Vector3();
        Vector3 redFlagBasePos = new Vector3();
        Vector3 blueFlagPos = new Vector3();
        Vector3 blueFlagBasePos = new Vector3();

        switch (GameOptionsManager.Instance.currentGameOptions.MapId)
        {
            // Skeld / Custom Skeld
            case 0:
                if (RebuildUs.activatedSensei)
                {
                    stealerPlayerPos = new Vector3(-3.65f, 5f, PlayerControl.LocalPlayer.transform.position.z);
                    redTeamPos = new Vector3(-17.5f, -1.15f, PlayerControl.LocalPlayer.transform.position.z);
                    blueTeamPos = new Vector3(7.7f, -0.95f, PlayerControl.LocalPlayer.transform.position.z);
                    redFlagPos = new Vector3(-17.5f, -1.35f, 0.5f);
                    redFlagBasePos = new Vector3(-17.5f, -1.4f, 1f);
                    blueFlagPos = new Vector3(7.7f, -1.15f, 0.5f);
                    blueFlagBasePos = new Vector3(7.7f, -1.2f, 1f);
                }
                else if (RebuildUs.activatedDleks)
                {
                    stealerPlayerPos = new Vector3(-6.35f, -7.5f, PlayerControl.LocalPlayer.transform.position.z);
                    redTeamPos = new Vector3(20.5f, -5.15f, PlayerControl.LocalPlayer.transform.position.z);
                    blueTeamPos = new Vector3(-16.5f, -4.45f, PlayerControl.LocalPlayer.transform.position.z);
                    redFlagPos = new Vector3(20.5f, -5.35f, 0.5f);
                    redFlagBasePos = new Vector3(20.5f, -5.4f, 1f);
                    blueFlagPos = new Vector3(-16.5f, -4.65f, 0.5f);
                    blueFlagBasePos = new Vector3(-16.5f, -4.7f, 1f);
                }
                else
                {
                    stealerPlayerPos = new Vector3(6.35f, -7.5f, PlayerControl.LocalPlayer.transform.position.z);
                    redTeamPos = new Vector3(-20.5f, -5.15f, PlayerControl.LocalPlayer.transform.position.z);
                    blueTeamPos = new Vector3(16.5f, -4.45f, PlayerControl.LocalPlayer.transform.position.z);
                    redFlagPos = new Vector3(-20.5f, -5.35f, 0.5f);
                    redFlagBasePos = new Vector3(-20.5f, -5.4f, 1f);
                    blueFlagPos = new Vector3(16.5f, -4.65f, 0.5f);
                    blueFlagBasePos = new Vector3(16.5f, -4.7f, 1f);
                }
                break;
            // Mira HQ
            case 1:
                stealerPlayerPos = new Vector3(17.75f, 24f, PlayerControl.LocalPlayer.transform.position.z);
                redTeamPos = new Vector3(2.53f, 10.75f, PlayerControl.LocalPlayer.transform.position.z);
                blueTeamPos = new Vector3(23.25f, 5.25f, PlayerControl.LocalPlayer.transform.position.z);
                redFlagPos = new Vector3(2.525f, 10.55f, 0.5f);
                redFlagBasePos = new Vector3(2.53f, 10.5f, 1f);
                blueFlagPos = new Vector3(23.25f, 5.05f, 0.5f);
                blueFlagBasePos = new Vector3(23.25f, 5f, 1f);
                break;
            // Polus
            case 2:
                stealerPlayerPos = new Vector3(31.75f, -13f, PlayerControl.LocalPlayer.transform.position.z);
                redTeamPos = new Vector3(36.4f, -21.5f, PlayerControl.LocalPlayer.transform.position.z);
                blueTeamPos = new Vector3(5.4f, -9.45f, PlayerControl.LocalPlayer.transform.position.z);
                redFlagPos = new Vector3(36.4f, -21.7f, 0.5f);
                redFlagBasePos = new Vector3(36.4f, -21.75f, 1f);
                blueFlagPos = new Vector3(5.4f, -9.65f, 0.5f);
                blueFlagBasePos = new Vector3(5.4f, -9.7f, 1f);
                break;
            // Dleks
            case 3:
                stealerPlayerPos = new Vector3(-6.35f, -7.5f, PlayerControl.LocalPlayer.transform.position.z);
                redTeamPos = new Vector3(20.5f, -5.15f, PlayerControl.LocalPlayer.transform.position.z);
                blueTeamPos = new Vector3(-16.5f, -4.45f, PlayerControl.LocalPlayer.transform.position.z);
                redFlagPos = new Vector3(20.5f, -5.35f, 0.5f);
                redFlagBasePos = new Vector3(20.5f, -5.4f, 1f);
                blueFlagPos = new Vector3(-16.5f, -4.65f, 0.5f);
                blueFlagBasePos = new Vector3(-16.5f, -4.7f, 1f);
                break;
            // Airship
            case 4:
                stealerPlayerPos = new Vector3(10.25f, -15.35f, PlayerControl.LocalPlayer.transform.position.z);
                redTeamPos = new Vector3(-17.5f, -1f, PlayerControl.LocalPlayer.transform.position.z);
                blueTeamPos = new Vector3(33.6f, 1.45f, PlayerControl.LocalPlayer.transform.position.z);
                redFlagPos = new Vector3(-17.5f, -1.2f, 0.5f);
                redFlagBasePos = new Vector3(-17.5f, -1.25f, 1f);
                blueFlagPos = new Vector3(33.6f, 1.25f, 0.5f);
                blueFlagBasePos = new Vector3(33.6f, 1.2f, 1f);
                break;
            // Fungle
            case 5:
                stealerPlayerPos = new Vector3(2.85f, -5.75f, PlayerControl.LocalPlayer.transform.position.z);
                redTeamPos = new Vector3(-23f, -0.45f, PlayerControl.LocalPlayer.transform.position.z);
                blueTeamPos = new Vector3(19.25f, 2.35f, PlayerControl.LocalPlayer.transform.position.z);
                redFlagPos = new Vector3(-23f, -0.65f, 0.5f);
                redFlagBasePos = new Vector3(-23, -0.7f, 1f);
                blueFlagPos = new Vector3(19.25f, 2.15f, 0.5f);
                blueFlagBasePos = new Vector3(19.25f, 2.1f, 1f);
                break;
            // Submerged
            case 6:
                stealerPlayerPos = new Vector3(1f, 10f, PlayerControl.LocalPlayer.transform.position.z);
                redTeamPos = new Vector3(-8.35f, 28.25f, PlayerControl.LocalPlayer.transform.position.z);
                blueTeamPos = new Vector3(12.5f, -31.25f, PlayerControl.LocalPlayer.transform.position.z);
                redFlagPos = new Vector3(-8.35f, 28.05f, 0.03f);
                redFlagBasePos = new Vector3(-8.35f, 28, 0.031f);
                blueFlagPos = new Vector3(12.5f, -31.45f, -0.011f);
                blueFlagBasePos = new Vector3(12.5f, -31.5f, -0.01f);

                // Add another respawn on each floor
                GameObject redteamfloor = GameObject.Instantiate(AssetLoader.redfloor, PlayerControl.LocalPlayer.transform.parent);
                redteamfloor.name = "redteamfloor";
                redteamfloor.transform.position = new Vector3(-14f, -27.5f, -0.01f);
                GameObject blueteamfloor = GameObject.Instantiate(AssetLoader.bluefloor, PlayerControl.LocalPlayer.transform.parent);
                blueteamfloor.name = "blueteamfloor";
                blueteamfloor.transform.position = new Vector3(14.25f, 24.25f, 0.03f);
                break;
        }

        foreach (PlayerControl player in CaptureTheFlag.redteamFlag)
        {
            player.transform.position = redTeamPos;
        }

        foreach (PlayerControl player in CaptureTheFlag.blueteamFlag)
        {
            player.transform.position = blueTeamPos;
        }

        if (PlayerControl.LocalPlayer != null && !createdcapturetheflag)
        {
            Helpers.ClearAllTasks(PlayerControl.LocalPlayer);

            GameObject redflag = GameObject.Instantiate(AssetLoader.redflag, PlayerControl.LocalPlayer.transform.parent);
            redflag.name = "redflag";
            redflag.transform.position = redFlagPos;
            CaptureTheFlag.redflag = redflag;
            GameObject redflagbase = GameObject.Instantiate(AssetLoader.redflagbase, PlayerControl.LocalPlayer.transform.parent);
            redflagbase.name = "redflagbase";
            redflagbase.transform.position = redFlagBasePos;
            CaptureTheFlag.redflagbase = redflagbase;
            GameObject blueflag = GameObject.Instantiate(AssetLoader.blueflag, PlayerControl.LocalPlayer.transform.parent);
            blueflag.name = "blueflag";
            blueflag.transform.position = blueFlagPos;
            CaptureTheFlag.blueflag = blueflag;
            GameObject blueflagbase = GameObject.Instantiate(AssetLoader.blueflagbase, PlayerControl.LocalPlayer.transform.parent);
            blueflagbase.name = "blueflagbase";
            blueflagbase.transform.position = blueFlagBasePos;
            CaptureTheFlag.blueflagbase = blueflagbase;

            if (CaptureTheFlag.stealerPlayer != null)
            {
                CaptureTheFlag.stealerPlayer.MyPhysics.SetBodyType(PlayerBodyTypes.Seeker);
                CaptureTheFlag.stealerPlayer.transform.position = stealerPlayerPos;
                CaptureTheFlag.stealerSpawns.Add(redflagbase);
                CaptureTheFlag.stealerSpawns.Add(blueflagbase);
            }

            createdcapturetheflag = true;
        }
    }

    public static void setTarget()
    {
        if (MapSettings.GameMode is not CustomGameMode.CaptureTheFlag) return;

        var untargetableAllPlayers = new List<PlayerControl>();

        var untargetableRedPlayers = new List<PlayerControl>();
        foreach (PlayerControl player in CaptureTheFlag.redteamFlag)
        {
            untargetableRedPlayers.Add(player);
        }

        var untargetableBluePlayers = new List<PlayerControl>();
        foreach (PlayerControl player in CaptureTheFlag.blueteamFlag)
        {
            untargetableBluePlayers.Add(player);
        }

        // Prevent killing reviving players
        if (CaptureTheFlag.blueplayer01IsReviving)
        {
            untargetableRedPlayers.Add(CaptureTheFlag.blueplayer01);
            untargetableAllPlayers.Add(CaptureTheFlag.blueplayer01);
        }
        else
        {
            untargetableRedPlayers.Remove(CaptureTheFlag.blueplayer01);
            untargetableAllPlayers.Remove(CaptureTheFlag.blueplayer01);
        }
        if (CaptureTheFlag.blueplayer02IsReviving)
        {
            untargetableRedPlayers.Add(CaptureTheFlag.blueplayer02);
            untargetableAllPlayers.Add(CaptureTheFlag.blueplayer02);
        }
        else
        {
            untargetableRedPlayers.Remove(CaptureTheFlag.blueplayer02);
            untargetableAllPlayers.Remove(CaptureTheFlag.blueplayer02);
        }
        if (CaptureTheFlag.blueplayer03IsReviving)
        {
            untargetableRedPlayers.Add(CaptureTheFlag.blueplayer03);
            untargetableAllPlayers.Add(CaptureTheFlag.blueplayer03);
        }
        else
        {
            untargetableRedPlayers.Remove(CaptureTheFlag.blueplayer03);
            untargetableAllPlayers.Remove(CaptureTheFlag.blueplayer03);
        }
        if (CaptureTheFlag.blueplayer04IsReviving)
        {
            untargetableRedPlayers.Add(CaptureTheFlag.blueplayer04);
            untargetableAllPlayers.Add(CaptureTheFlag.blueplayer04);
        }
        else
        {
            untargetableRedPlayers.Remove(CaptureTheFlag.blueplayer04);
            untargetableAllPlayers.Remove(CaptureTheFlag.blueplayer04);
        }
        if (CaptureTheFlag.blueplayer05IsReviving)
        {
            untargetableRedPlayers.Add(CaptureTheFlag.blueplayer05);
            untargetableAllPlayers.Add(CaptureTheFlag.blueplayer05);
        }
        else
        {
            untargetableRedPlayers.Remove(CaptureTheFlag.blueplayer05);
            untargetableAllPlayers.Remove(CaptureTheFlag.blueplayer05);
        }
        if (CaptureTheFlag.blueplayer06IsReviving)
        {
            untargetableRedPlayers.Add(CaptureTheFlag.blueplayer06);
            untargetableAllPlayers.Add(CaptureTheFlag.blueplayer06);
        }
        else
        {
            untargetableRedPlayers.Remove(CaptureTheFlag.blueplayer06);
            untargetableAllPlayers.Remove(CaptureTheFlag.blueplayer06);
        }
        if (CaptureTheFlag.blueplayer07IsReviving)
        {
            untargetableRedPlayers.Add(CaptureTheFlag.blueplayer07);
            untargetableAllPlayers.Add(CaptureTheFlag.blueplayer07);
        }
        else
        {
            untargetableRedPlayers.Remove(CaptureTheFlag.blueplayer07);
            untargetableAllPlayers.Remove(CaptureTheFlag.blueplayer07);
        }
        if (CaptureTheFlag.stealerPlayerIsReviving)
        {
            untargetableRedPlayers.Add(CaptureTheFlag.stealerPlayer);
            untargetableBluePlayers.Add(CaptureTheFlag.stealerPlayer);
        }
        else
        {
            untargetableRedPlayers.Remove(CaptureTheFlag.stealerPlayer);
            untargetableBluePlayers.Remove(CaptureTheFlag.stealerPlayer);
        }

        if (CaptureTheFlag.redplayer01 != null && CaptureTheFlag.redplayer01 == PlayerControl.LocalPlayer)
        {
            CaptureTheFlag.redplayer01currentTarget = Helpers.SetTarget(untargetablePlayers: untargetableRedPlayers);
            Helpers.SetPlayerOutline(CaptureTheFlag.redplayer01currentTarget, Palette.ImpostorRed);
        }
        if (CaptureTheFlag.redplayer02 != null && CaptureTheFlag.redplayer02 == PlayerControl.LocalPlayer)
        {
            CaptureTheFlag.redplayer02currentTarget = Helpers.SetTarget(untargetablePlayers: untargetableRedPlayers);
            Helpers.SetPlayerOutline(CaptureTheFlag.redplayer02currentTarget, Palette.ImpostorRed);
        }
        if (CaptureTheFlag.redplayer03 != null && CaptureTheFlag.redplayer03 == PlayerControl.LocalPlayer)
        {
            CaptureTheFlag.redplayer03currentTarget = Helpers.SetTarget(untargetablePlayers: untargetableRedPlayers);
            Helpers.SetPlayerOutline(CaptureTheFlag.redplayer03currentTarget, Palette.ImpostorRed);
        }
        if (CaptureTheFlag.redplayer04 != null && CaptureTheFlag.redplayer04 == PlayerControl.LocalPlayer)
        {
            CaptureTheFlag.redplayer04currentTarget = Helpers.SetTarget(untargetablePlayers: untargetableRedPlayers);
            Helpers.SetPlayerOutline(CaptureTheFlag.redplayer04currentTarget, Palette.ImpostorRed);
        }
        if (CaptureTheFlag.redplayer05 != null && CaptureTheFlag.redplayer05 == PlayerControl.LocalPlayer)
        {
            CaptureTheFlag.redplayer05currentTarget = Helpers.SetTarget(untargetablePlayers: untargetableRedPlayers);
            Helpers.SetPlayerOutline(CaptureTheFlag.redplayer05currentTarget, Palette.ImpostorRed);
        }
        if (CaptureTheFlag.redplayer06 != null && CaptureTheFlag.redplayer06 == PlayerControl.LocalPlayer)
        {
            CaptureTheFlag.redplayer06currentTarget = Helpers.SetTarget(untargetablePlayers: untargetableRedPlayers);
            Helpers.SetPlayerOutline(CaptureTheFlag.redplayer06currentTarget, Palette.ImpostorRed);
        }
        if (CaptureTheFlag.redplayer07 != null && CaptureTheFlag.redplayer07 == PlayerControl.LocalPlayer)
        {
            CaptureTheFlag.redplayer07currentTarget = Helpers.SetTarget(untargetablePlayers: untargetableRedPlayers);
            Helpers.SetPlayerOutline(CaptureTheFlag.redplayer07currentTarget, Palette.ImpostorRed);
        }

        // Prevent killing reviving players
        if (CaptureTheFlag.redplayer01IsReviving)
        {
            untargetableBluePlayers.Add(CaptureTheFlag.redplayer01);
            untargetableAllPlayers.Add(CaptureTheFlag.redplayer01);
        }
        else
        {
            untargetableBluePlayers.Remove(CaptureTheFlag.redplayer01);
            untargetableAllPlayers.Remove(CaptureTheFlag.redplayer01);
        }
        if (CaptureTheFlag.redplayer02IsReviving)
        {
            untargetableBluePlayers.Add(CaptureTheFlag.redplayer02);
            untargetableAllPlayers.Add(CaptureTheFlag.redplayer02);
        }
        else
        {
            untargetableBluePlayers.Remove(CaptureTheFlag.redplayer02);
            untargetableAllPlayers.Remove(CaptureTheFlag.redplayer02);
        }
        if (CaptureTheFlag.redplayer03IsReviving)
        {
            untargetableBluePlayers.Add(CaptureTheFlag.redplayer03);
            untargetableAllPlayers.Add(CaptureTheFlag.redplayer03);
        }
        else
        {
            untargetableBluePlayers.Remove(CaptureTheFlag.redplayer03);
            untargetableAllPlayers.Remove(CaptureTheFlag.redplayer03);
        }
        if (CaptureTheFlag.redplayer04IsReviving)
        {
            untargetableBluePlayers.Add(CaptureTheFlag.redplayer04);
            untargetableAllPlayers.Add(CaptureTheFlag.redplayer04);
        }
        else
        {
            untargetableBluePlayers.Remove(CaptureTheFlag.redplayer04);
            untargetableAllPlayers.Remove(CaptureTheFlag.redplayer04);
        }
        if (CaptureTheFlag.redplayer05IsReviving)
        {
            untargetableBluePlayers.Add(CaptureTheFlag.redplayer05);
            untargetableAllPlayers.Add(CaptureTheFlag.redplayer05);
        }
        else
        {
            untargetableBluePlayers.Remove(CaptureTheFlag.redplayer05);
            untargetableAllPlayers.Remove(CaptureTheFlag.redplayer05);
        }
        if (CaptureTheFlag.redplayer06IsReviving)
        {
            untargetableBluePlayers.Add(CaptureTheFlag.redplayer06);
            untargetableAllPlayers.Add(CaptureTheFlag.redplayer06);
        }
        else
        {
            untargetableBluePlayers.Remove(CaptureTheFlag.redplayer06);
            untargetableAllPlayers.Remove(CaptureTheFlag.redplayer06);
        }
        if (CaptureTheFlag.redplayer07IsReviving)
        {
            untargetableBluePlayers.Add(CaptureTheFlag.redplayer07);
            untargetableAllPlayers.Add(CaptureTheFlag.redplayer07);
        }
        else
        {
            untargetableBluePlayers.Remove(CaptureTheFlag.redplayer07);
            untargetableAllPlayers.Remove(CaptureTheFlag.redplayer07);
        }

        if (CaptureTheFlag.blueplayer01 != null && CaptureTheFlag.blueplayer01 == PlayerControl.LocalPlayer)
        {
            CaptureTheFlag.blueplayer01currentTarget = Helpers.SetTarget(untargetablePlayers: untargetableBluePlayers);
            Helpers.SetPlayerOutline(CaptureTheFlag.blueplayer01currentTarget, Color.blue);
        }
        if (CaptureTheFlag.blueplayer02 != null && CaptureTheFlag.blueplayer02 == PlayerControl.LocalPlayer)
        {
            CaptureTheFlag.blueplayer02currentTarget = Helpers.SetTarget(untargetablePlayers: untargetableBluePlayers);
            Helpers.SetPlayerOutline(CaptureTheFlag.blueplayer02currentTarget, Color.blue);
        }
        if (CaptureTheFlag.blueplayer03 != null && CaptureTheFlag.blueplayer03 == PlayerControl.LocalPlayer)
        {
            CaptureTheFlag.blueplayer03currentTarget = Helpers.SetTarget(untargetablePlayers: untargetableBluePlayers);
            Helpers.SetPlayerOutline(CaptureTheFlag.blueplayer03currentTarget, Color.blue);
        }
        if (CaptureTheFlag.blueplayer04 != null && CaptureTheFlag.blueplayer04 == PlayerControl.LocalPlayer)
        {
            CaptureTheFlag.blueplayer04currentTarget = Helpers.SetTarget(untargetablePlayers: untargetableBluePlayers);
            Helpers.SetPlayerOutline(CaptureTheFlag.blueplayer04currentTarget, Color.blue);
        }
        if (CaptureTheFlag.blueplayer05 != null && CaptureTheFlag.blueplayer05 == PlayerControl.LocalPlayer)
        {
            CaptureTheFlag.blueplayer05currentTarget = Helpers.SetTarget(untargetablePlayers: untargetableBluePlayers);
            Helpers.SetPlayerOutline(CaptureTheFlag.blueplayer05currentTarget, Color.blue);
        }
        if (CaptureTheFlag.blueplayer06 != null && CaptureTheFlag.blueplayer06 == PlayerControl.LocalPlayer)
        {
            CaptureTheFlag.blueplayer06currentTarget = Helpers.SetTarget(untargetablePlayers: untargetableBluePlayers);
            Helpers.SetPlayerOutline(CaptureTheFlag.blueplayer06currentTarget, Color.blue);
        }
        if (CaptureTheFlag.blueplayer07 != null && CaptureTheFlag.blueplayer07 == PlayerControl.LocalPlayer)
        {
            CaptureTheFlag.blueplayer07currentTarget = Helpers.SetTarget(untargetablePlayers: untargetableBluePlayers);
            Helpers.SetPlayerOutline(CaptureTheFlag.blueplayer07currentTarget, Color.blue);
        }
        if (CaptureTheFlag.stealerPlayer != null && CaptureTheFlag.stealerPlayer == PlayerControl.LocalPlayer)
        {
            CaptureTheFlag.stealerPlayercurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetableAllPlayers);
            Helpers.SetPlayerOutline(CaptureTheFlag.stealerPlayercurrentTarget, Color.grey);
        }
    }

    public static void OnMurderPlayerPostfix(PlayerControl __instance, PlayerControl target)
    {
        if (CaptureTheFlag.redPlayerWhoHasBlueFlag != null && target == CaptureTheFlag.redPlayerWhoHasBlueFlag)
        {
            CaptureTheFlag.blueflagtaken = false;
            Helpers.showGamemodesPopUp(0, Helpers.PlayerById(CaptureTheFlag.redPlayerWhoHasBlueFlag.PlayerId));
            CaptureTheFlag.redPlayerWhoHasBlueFlag = null;
            CaptureTheFlag.blueflag.transform.parent = CaptureTheFlag.blueflagbase.transform.parent;
            switch (GameOptionsManager.Instance.currentGameOptions.MapId)
            {
                // Skeld
                case 0:
                    if (RebuildUs.activatedSensei)
                    {
                        CaptureTheFlag.blueflag.transform.position = new Vector3(7.7f, -1.15f, 0.5f);
                    }
                    else if (RebuildUs.activatedDleks)
                    {
                        CaptureTheFlag.blueflag.transform.position = new Vector3(-16.5f, -4.65f, 0.5f);
                    }
                    else
                    {
                        CaptureTheFlag.blueflag.transform.position = new Vector3(16.5f, -4.65f, 0.5f);
                    }
                    break;
                // MiraHQ
                case 1:
                    CaptureTheFlag.blueflag.transform.position = new Vector3(23.25f, 5.05f, 0.5f);
                    break;
                // Polus
                case 2:
                    CaptureTheFlag.blueflag.transform.position = new Vector3(5.4f, -9.65f, 0.5f);
                    break;
                // Dleks
                case 3:
                    CaptureTheFlag.blueflag.transform.position = new Vector3(-16.5f, -4.65f, 0.5f);
                    break;
                // Airship
                case 4:
                    CaptureTheFlag.blueflag.transform.position = new Vector3(33.6f, 1.25f, 0.5f);
                    break;
                // Fungle
                case 5:
                    CaptureTheFlag.blueflag.transform.position = new Vector3(19.25f, 2.15f, 0.5f);
                    break;
                // Submerged
                case 6:
                    CaptureTheFlag.blueflag.transform.position = new Vector3(12.5f, -31.45f, -0.011f);
                    break;
            }
        }

        if (CaptureTheFlag.bluePlayerWhoHasRedFlag != null && target == CaptureTheFlag.bluePlayerWhoHasRedFlag)
        {
            CaptureTheFlag.redflagtaken = false;
            Helpers.showGamemodesPopUp(0, Helpers.PlayerById(CaptureTheFlag.bluePlayerWhoHasRedFlag.PlayerId));
            CaptureTheFlag.bluePlayerWhoHasRedFlag = null;
            CaptureTheFlag.redflag.transform.parent = CaptureTheFlag.redflagbase.transform.parent;
            switch (GameOptionsManager.Instance.currentGameOptions.MapId)
            {
                // Skeld
                case 0:
                    if (RebuildUs.activatedSensei)
                    {
                        CaptureTheFlag.redflag.transform.position = new Vector3(-17.5f, -1.35f, 0.5f);
                    }
                    else if (RebuildUs.activatedDleks)
                    {
                        CaptureTheFlag.redflag.transform.position = new Vector3(20.5f, -5.35f, 0.5f);
                    }
                    else
                    {
                        CaptureTheFlag.redflag.transform.position = new Vector3(-20.5f, -5.35f, 0.5f);
                    }
                    break;
                // MiraHQ
                case 1:
                    CaptureTheFlag.redflag.transform.position = new Vector3(2.525f, 10.55f, 0.5f);
                    break;
                // Polus
                case 2:
                    CaptureTheFlag.redflag.transform.position = new Vector3(36.4f, -21.7f, 0.5f);
                    break;
                // Dlesk
                case 3:
                    CaptureTheFlag.redflag.transform.position = new Vector3(20.5f, -5.35f, 0.5f);
                    break;
                // Airship
                case 4:
                    CaptureTheFlag.redflag.transform.position = new Vector3(-17.5f, -1.2f, 0.5f);
                    break;
                // Fungle
                case 5:
                    CaptureTheFlag.redflag.transform.position = new Vector3(-23f, -0.65f, 0.5f);
                    break;
                // Submerged
                case 6:
                    CaptureTheFlag.redflag.transform.position = new Vector3(-8.35f, 28.05f, 0.03f);
                    break;
            }
        }

        // Capture the flag revive player
        if (CaptureTheFlag.stealerPlayer != null && CaptureTheFlag.stealerPlayer.PlayerId == target.PlayerId)
        {
            var ctfBody = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == target.PlayerId);
            ctfBody.transform.position = new Vector3(50, 50, 1);
            CaptureTheFlag.stealerPlayerIsReviving = true;
            CaptureTheFlag.stealerPlayer.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
            Helpers.alphaPlayer(true, CaptureTheFlag.stealerPlayer.PlayerId);
            HudManager.Instance.StartCoroutine(Effects.Lerp(MapSettings.gamemodeReviveTime, new Action<float>((p) =>
            {
                if (p == 1f && CaptureTheFlag.stealerPlayer != null)
                {
                    CaptureTheFlag.stealerPlayerIsReviving = false;
                    Helpers.alphaPlayer(false, CaptureTheFlag.stealerPlayer.PlayerId);
                    CaptureTheFlag.stealerPlayer.MyPhysics.SetBodyType(PlayerBodyTypes.Seeker);
                }
            })));
            HudManager.Instance.StartCoroutine(Effects.Lerp(MapSettings.gamemodeReviveTime - MapSettings.gamemodeInvincibilityTime, new Action<float>((p) =>
            {
                if (p == 1f && CaptureTheFlag.stealerPlayer != null)
                {
                    CaptureTheFlag.stealerPlayer.Revive();
                    switch (GameOptionsManager.Instance.currentGameOptions.MapId)
                    {
                        // Skeld
                        case 0:
                            if (RebuildUs.activatedSensei)
                            {
                                CaptureTheFlag.stealerPlayer.transform.position = new Vector3(-3.65f, 5f, CaptureTheFlag.stealerPlayer.transform.position.z);
                            }
                            else if (RebuildUs.activatedDleks)
                            {
                                CaptureTheFlag.stealerPlayer.transform.position = new Vector3(-6.35f, -7.5f, CaptureTheFlag.stealerPlayer.transform.position.z);
                            }
                            else
                            {
                                CaptureTheFlag.stealerPlayer.transform.position = new Vector3(6.35f, -7.5f, CaptureTheFlag.stealerPlayer.transform.position.z);
                            }
                            break;
                        // MiraHQ
                        case 1:
                            CaptureTheFlag.stealerPlayer.transform.position = new Vector3(17.75f, 24f, CaptureTheFlag.stealerPlayer.transform.position.z);
                            break;
                        // Polus
                        case 2:
                            CaptureTheFlag.stealerPlayer.transform.position = new Vector3(31.75f, -13f, CaptureTheFlag.stealerPlayer.transform.position.z);
                            break;
                        // Dleks
                        case 3:
                            CaptureTheFlag.stealerPlayer.transform.position = new Vector3(-6.35f, -7.5f, CaptureTheFlag.stealerPlayer.transform.position.z);
                            break;
                        // Airship
                        case 4:
                            CaptureTheFlag.stealerPlayer.transform.position = new Vector3(10.25f, -15.35f, CaptureTheFlag.stealerPlayer.transform.position.z);
                            break;
                        // Fungle
                        case 5:
                            CaptureTheFlag.stealerPlayer.transform.position = new Vector3(2.85f, -5.75f, CaptureTheFlag.stealerPlayer.transform.position.z);
                            break;
                        // Submerged
                        case 6:
                            if (CaptureTheFlag.stealerPlayer.transform.position.y > 0)
                            {
                                CaptureTheFlag.stealerPlayer.transform.position = new Vector3(1f, 10f, CaptureTheFlag.stealerPlayer.transform.position.z);
                            }
                            else
                            {
                                CaptureTheFlag.stealerPlayer.transform.position = new Vector3(0f, -33.5f, CaptureTheFlag.stealerPlayer.transform.position.z);
                            }
                            break;
                    }
                    DeadPlayer deadPlayerEntry = GameHistory.DeadPlayers.Where(x => x.Player.PlayerId == target.PlayerId).FirstOrDefault();
                    if (ctfBody != null) UnityEngine.Object.Destroy(ctfBody.gameObject);
                    if (deadPlayerEntry != null) GameHistory.DeadPlayers.Remove(deadPlayerEntry);
                }

            })));

        }

        foreach (PlayerControl player in CaptureTheFlag.redteamFlag)
        {
            if (player.PlayerId == target.PlayerId)
            {
                var ctfBody = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == target.PlayerId);
                ctfBody.transform.position = new Vector3(50, 50, 1);
                if (CaptureTheFlag.redplayer01 != null && target.PlayerId == CaptureTheFlag.redplayer01.PlayerId)
                {
                    CaptureTheFlag.redplayer01IsReviving = true;
                }
                else if (CaptureTheFlag.redplayer02 != null && target.PlayerId == CaptureTheFlag.redplayer02.PlayerId)
                {
                    CaptureTheFlag.redplayer02IsReviving = true;
                }
                else if (CaptureTheFlag.redplayer03 != null && target.PlayerId == CaptureTheFlag.redplayer03.PlayerId)
                {
                    CaptureTheFlag.redplayer03IsReviving = true;
                }
                else if (CaptureTheFlag.redplayer04 != null && target.PlayerId == CaptureTheFlag.redplayer04.PlayerId)
                {
                    CaptureTheFlag.redplayer04IsReviving = true;
                }
                else if (CaptureTheFlag.redplayer05 != null && target.PlayerId == CaptureTheFlag.redplayer05.PlayerId)
                {
                    CaptureTheFlag.redplayer05IsReviving = true;
                }
                else if (CaptureTheFlag.redplayer06 != null && target.PlayerId == CaptureTheFlag.redplayer06.PlayerId)
                {
                    CaptureTheFlag.redplayer06IsReviving = true;
                }
                else if (CaptureTheFlag.redplayer07 != null && target.PlayerId == CaptureTheFlag.redplayer07.PlayerId)
                {
                    CaptureTheFlag.redplayer07IsReviving = true;
                }
                Helpers.alphaPlayer(true, player.PlayerId);
                HudManager.Instance.StartCoroutine(Effects.Lerp(MapSettings.gamemodeReviveTime, new Action<float>((p) =>
                {
                    if (p == 1f && player != null)
                    {
                        if (CaptureTheFlag.redplayer01 != null && target.PlayerId == CaptureTheFlag.redplayer01.PlayerId)
                        {
                            CaptureTheFlag.redplayer01IsReviving = false;
                        }
                        else if (CaptureTheFlag.redplayer02 != null && target.PlayerId == CaptureTheFlag.redplayer02.PlayerId)
                        {
                            CaptureTheFlag.redplayer02IsReviving = false;
                        }
                        else if (CaptureTheFlag.redplayer03 != null && target.PlayerId == CaptureTheFlag.redplayer03.PlayerId)
                        {
                            CaptureTheFlag.redplayer03IsReviving = false;
                        }
                        else if (CaptureTheFlag.redplayer04 != null && target.PlayerId == CaptureTheFlag.redplayer04.PlayerId)
                        {
                            CaptureTheFlag.redplayer04IsReviving = false;
                        }
                        else if (CaptureTheFlag.redplayer05 != null && target.PlayerId == CaptureTheFlag.redplayer05.PlayerId)
                        {
                            CaptureTheFlag.redplayer05IsReviving = false;
                        }
                        else if (CaptureTheFlag.redplayer06 != null && target.PlayerId == CaptureTheFlag.redplayer06.PlayerId)
                        {
                            CaptureTheFlag.redplayer06IsReviving = false;
                        }
                        else if (CaptureTheFlag.redplayer07 != null && target.PlayerId == CaptureTheFlag.redplayer07.PlayerId)
                        {
                            CaptureTheFlag.redplayer07IsReviving = false;
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
                                    player.transform.position = new Vector3(20.5f, -5.15f, player.transform.position.z);
                                }
                                else
                                {
                                    player.transform.position = new Vector3(-20.5f, -5.15f, player.transform.position.z);
                                }
                                break;
                            // MiraHQ
                            case 1:
                                player.transform.position = new Vector3(2.53f, 10.75f, player.transform.position.z);
                                break;
                            // Polus
                            case 2:
                                player.transform.position = new Vector3(36.4f, -21.5f, player.transform.position.z);
                                break;
                            // Dleks
                            case 3:
                                player.transform.position = new Vector3(20.5f, -5.15f, player.transform.position.z);
                                break;
                            // Airship
                            case 4:
                                player.transform.position = new Vector3(-17.5f, -1.1f, player.transform.position.z);
                                break;
                            // Fungle
                            case 5:
                                player.transform.position = new Vector3(-23f, -0.45f, player.transform.position.z);
                                break;
                            // Submerged
                            case 6:
                                if (player.transform.position.y > 0)
                                {
                                    player.transform.position = new Vector3(-8.35f, 28.25f, player.transform.position.z);
                                }
                                else
                                {
                                    player.transform.position = new Vector3(-14f, -27.5f, player.transform.position.z);
                                }
                                break;
                        }
                        DeadPlayer deadPlayerEntry = GameHistory.DeadPlayers.Where(x => x.Player.PlayerId == target.PlayerId).FirstOrDefault();
                        if (ctfBody != null) UnityEngine.Object.Destroy(ctfBody.gameObject);
                        if (deadPlayerEntry != null) GameHistory.DeadPlayers.Remove(deadPlayerEntry);
                    }

                })));

            }
        }
        foreach (PlayerControl player in CaptureTheFlag.blueteamFlag)
        {
            if (player.PlayerId == target.PlayerId)
            {
                var ctfBody = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == target.PlayerId);
                ctfBody.transform.position = new Vector3(50, 50, 1);
                if (CaptureTheFlag.blueplayer01 != null && target.PlayerId == CaptureTheFlag.blueplayer01.PlayerId)
                {
                    CaptureTheFlag.blueplayer01IsReviving = true;
                }
                else if (CaptureTheFlag.blueplayer02 != null && target.PlayerId == CaptureTheFlag.blueplayer02.PlayerId)
                {
                    CaptureTheFlag.blueplayer02IsReviving = true;
                }
                else if (CaptureTheFlag.blueplayer03 != null && target.PlayerId == CaptureTheFlag.blueplayer03.PlayerId)
                {
                    CaptureTheFlag.blueplayer03IsReviving = true;
                }
                else if (CaptureTheFlag.blueplayer04 != null && target.PlayerId == CaptureTheFlag.blueplayer04.PlayerId)
                {
                    CaptureTheFlag.blueplayer04IsReviving = true;
                }
                else if (CaptureTheFlag.blueplayer05 != null && target.PlayerId == CaptureTheFlag.blueplayer05.PlayerId)
                {
                    CaptureTheFlag.blueplayer05IsReviving = true;
                }
                else if (CaptureTheFlag.blueplayer06 != null && target.PlayerId == CaptureTheFlag.blueplayer06.PlayerId)
                {
                    CaptureTheFlag.blueplayer06IsReviving = true;
                }
                else if (CaptureTheFlag.blueplayer07 != null && target.PlayerId == CaptureTheFlag.blueplayer07.PlayerId)
                {
                    CaptureTheFlag.blueplayer07IsReviving = true;
                }
                Helpers.alphaPlayer(true, player.PlayerId);
                HudManager.Instance.StartCoroutine(Effects.Lerp(MapSettings.gamemodeReviveTime, new Action<float>((p) =>
                {
                    if (p == 1f && player != null)
                    {
                        if (CaptureTheFlag.blueplayer01 != null && target.PlayerId == CaptureTheFlag.blueplayer01.PlayerId)
                        {
                            CaptureTheFlag.blueplayer01IsReviving = false;
                        }
                        else if (CaptureTheFlag.blueplayer02 != null && target.PlayerId == CaptureTheFlag.blueplayer02.PlayerId)
                        {
                            CaptureTheFlag.blueplayer02IsReviving = false;
                        }
                        else if (CaptureTheFlag.blueplayer03 != null && target.PlayerId == CaptureTheFlag.blueplayer03.PlayerId)
                        {
                            CaptureTheFlag.blueplayer03IsReviving = false;
                        }
                        else if (CaptureTheFlag.blueplayer04 != null && target.PlayerId == CaptureTheFlag.blueplayer04.PlayerId)
                        {
                            CaptureTheFlag.blueplayer04IsReviving = false;
                        }
                        else if (CaptureTheFlag.blueplayer05 != null && target.PlayerId == CaptureTheFlag.blueplayer05.PlayerId)
                        {
                            CaptureTheFlag.blueplayer05IsReviving = false;
                        }
                        else if (CaptureTheFlag.blueplayer06 != null && target.PlayerId == CaptureTheFlag.blueplayer06.PlayerId)
                        {
                            CaptureTheFlag.blueplayer06IsReviving = false;
                        }
                        else if (CaptureTheFlag.blueplayer07 != null && target.PlayerId == CaptureTheFlag.blueplayer07.PlayerId)
                        {
                            CaptureTheFlag.blueplayer07IsReviving = false;
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
                                    player.transform.position = new Vector3(-16.5f, -4.45f, player.transform.position.z);
                                }
                                else
                                {
                                    player.transform.position = new Vector3(16.5f, -4.45f, player.transform.position.z);
                                }
                                break;
                            // MiraHQ
                            case 1:
                                player.transform.position = new Vector3(23.25f, 5.25f, player.transform.position.z);
                                break;
                            // Polus
                            case 2:
                                player.transform.position = new Vector3(5.4f, -9.45f, player.transform.position.z);
                                break;
                            // Dleks
                            case 3:
                                player.transform.position = new Vector3(-16.5f, -4.45f, player.transform.position.z);
                                break;
                            // Airship
                            case 4:
                                player.transform.position = new Vector3(33.6f, 1.45f, player.transform.position.z);
                                break;
                            // Fungle
                            case 5:
                                player.transform.position = new Vector3(19.25f, 2.35f, player.transform.position.z);
                                break;
                            // Submerged
                            case 6:
                                if (player.transform.position.y > 0)
                                {
                                    player.transform.position = new Vector3(14.25f, 24.25f, player.transform.position.z);
                                }
                                else
                                {
                                    player.transform.position = new Vector3(12.5f, -31.25f, player.transform.position.z);
                                }
                                break;
                        }
                        DeadPlayer deadPlayerEntry = GameHistory.DeadPlayers.Where(x => x.Player.PlayerId == target.PlayerId).FirstOrDefault();
                        if (ctfBody != null) UnityEngine.Object.Destroy(ctfBody.gameObject);
                        if (deadPlayerEntry != null) GameHistory.DeadPlayers.Remove(deadPlayerEntry);
                    }

                })));

            }
        }
    }

    public static void captureTheFlagUpdate()
    {
        if (MapSettings.GameMode is not CustomGameMode.CaptureTheFlag) return;

        if (CaptureTheFlag.redPlayerWhoHasBlueFlag != null && CaptureTheFlag.redPlayerWhoHasBlueFlag.Data.Disconnected)
        {
            CaptureTheFlag.blueflag.transform.parent = CaptureTheFlag.blueflagbase.transform.parent;
            switch (GameOptionsManager.Instance.currentGameOptions.MapId)
            {
                // Skeld
                case 0:
                    if (RebuildUs.activatedSensei)
                    {
                        CaptureTheFlag.blueflag.transform.position = new Vector3(7.7f, -1.15f, 0.5f);
                    }
                    else if (RebuildUs.activatedDleks)
                    {
                        CaptureTheFlag.blueflag.transform.position = new Vector3(-16.5f, -4.65f, 0.5f);
                    }
                    else
                    {
                        CaptureTheFlag.blueflag.transform.position = new Vector3(16.5f, -4.65f, 0.5f);
                    }
                    break;
                // MiraHQ
                case 1:
                    CaptureTheFlag.blueflag.transform.position = new Vector3(23.25f, 5.05f, 0.5f);
                    break;
                // Polus
                case 2:
                    CaptureTheFlag.blueflag.transform.position = new Vector3(5.4f, -9.65f, 0.5f);
                    break;
                // Dleks
                case 3:
                    CaptureTheFlag.blueflag.transform.position = new Vector3(-16.5f, -4.65f, 0.5f);
                    break;
                // Airship
                case 4:
                    CaptureTheFlag.blueflag.transform.position = new Vector3(33.6f, 1.25f, 0.5f);
                    break;
                // Fungle
                case 5:
                    CaptureTheFlag.blueflag.transform.position = new Vector3(19.25f, 2.15f, 0.5f);
                    break;
                // Submerged
                case 6:
                    CaptureTheFlag.blueflag.transform.position = new Vector3(12.5f, -31.45f, -0.011f);
                    break;
            }
            CaptureTheFlag.blueflagtaken = false;
            CaptureTheFlag.redPlayerWhoHasBlueFlag = null;
        }

        if (CaptureTheFlag.bluePlayerWhoHasRedFlag != null && CaptureTheFlag.bluePlayerWhoHasRedFlag.Data.Disconnected)
        {
            CaptureTheFlag.redflag.transform.parent = CaptureTheFlag.redflagbase.transform.parent;
            switch (GameOptionsManager.Instance.currentGameOptions.MapId)
            {
                // Skeld
                case 0:
                    if (RebuildUs.activatedSensei)
                    {
                        CaptureTheFlag.redflag.transform.position = new Vector3(-17.5f, -1.35f, 0.5f);
                    }
                    else if (RebuildUs.activatedDleks)
                    {
                        CaptureTheFlag.redflag.transform.position = new Vector3(20.5f, -5.35f, 0.5f);
                    }
                    else
                    {
                        CaptureTheFlag.redflag.transform.position = new Vector3(-20.5f, -5.35f, 0.5f);
                    }
                    break;
                // MiraHQ
                case 1:
                    CaptureTheFlag.redflag.transform.position = new Vector3(2.525f, 10.55f, 0.5f);
                    break;
                // Polus
                case 2:
                    CaptureTheFlag.redflag.transform.position = new Vector3(36.4f, -21.7f, 0.5f);
                    break;
                // Dleks
                case 3:
                    CaptureTheFlag.redflag.transform.position = new Vector3(20.5f, -5.35f, 0.5f);
                    break;
                // Airship
                case 4:
                    CaptureTheFlag.redflag.transform.position = new Vector3(-17.5f, -1.2f, 0.5f);
                    break;
                // Fungle
                case 5:
                    CaptureTheFlag.redflag.transform.position = new Vector3(-23f, -0.65f, 0.5f);
                    break;
                // Submerged
                case 6:
                    CaptureTheFlag.redflag.transform.position = new Vector3(-8.35f, 28.05f, 0.03f);
                    break;
            }
            CaptureTheFlag.redflagtaken = false;
            CaptureTheFlag.bluePlayerWhoHasRedFlag = null;
        }
    }
}