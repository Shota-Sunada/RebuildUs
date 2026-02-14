using Object = UnityEngine.Object;

namespace RebuildUs.Modules.GameMode;

public static partial class CaptureTheFlag
{
    public static Color IntroColor = new Color32(248, 205, 70, byte.MaxValue);

    public static bool Createdcapturetheflag;

    public static List<PlayerControl> RedteamFlag = [];
    public static PlayerControl Redplayer01;
    public static bool Redplayer01IsReviving;
    public static PlayerControl Redplayer01CurrentTarget;
    public static PlayerControl Redplayer02;
    public static bool Redplayer02IsReviving;
    public static PlayerControl Redplayer02CurrentTarget;
    public static PlayerControl Redplayer03;
    public static bool Redplayer03IsReviving;
    public static PlayerControl Redplayer03CurrentTarget;
    public static PlayerControl Redplayer04;
    public static bool Redplayer04IsReviving;
    public static PlayerControl Redplayer04CurrentTarget;
    public static PlayerControl Redplayer05;
    public static bool Redplayer05IsReviving;
    public static PlayerControl Redplayer05CurrentTarget;
    public static PlayerControl Redplayer06;
    public static bool Redplayer06IsReviving;
    public static PlayerControl Redplayer06CurrentTarget;
    public static PlayerControl Redplayer07;
    public static bool Redplayer07IsReviving;
    public static PlayerControl Redplayer07CurrentTarget;

    public static List<PlayerControl> BlueteamFlag = [];
    public static PlayerControl Blueplayer01;
    public static bool Blueplayer01IsReviving;
    public static PlayerControl Blueplayer01CurrentTarget;
    public static PlayerControl Blueplayer02;
    public static bool Blueplayer02IsReviving;
    public static PlayerControl Blueplayer02CurrentTarget;
    public static PlayerControl Blueplayer03;
    public static bool Blueplayer03IsReviving;
    public static PlayerControl Blueplayer03CurrentTarget;
    public static PlayerControl Blueplayer04;
    public static bool Blueplayer04IsReviving;
    public static PlayerControl Blueplayer04CurrentTarget;
    public static PlayerControl Blueplayer05;
    public static bool Blueplayer05IsReviving;
    public static PlayerControl Blueplayer05CurrentTarget;
    public static PlayerControl Blueplayer06;
    public static bool Blueplayer06IsReviving;
    public static PlayerControl Blueplayer06CurrentTarget;
    public static PlayerControl Blueplayer07;
    public static bool Blueplayer07IsReviving;
    public static PlayerControl Blueplayer07CurrentTarget;
    public static PlayerControl StealerPlayer;
    public static bool StealerPlayerIsReviving;
    public static PlayerControl StealerPlayercurrentTarget;
    public static List<GameObject> StealerSpawns = [];

    public static float RequiredFlags = 3;

    public static GameObject Redflag;
    public static GameObject Redflagbase;
    public static bool Redflagtaken;
    public static PlayerControl RedPlayerWhoHasBlueFlag;
    public static float CurrentRedTeamPoints;
    public static List<Arrow> LocalRedFlagArrow = [];

    public static GameObject Blueflag;
    public static GameObject Blueflagbase;
    public static bool Blueflagtaken;
    public static PlayerControl BluePlayerWhoHasRedFlag;
    public static float CurrentBlueTeamPoints;
    public static List<Arrow> LocalBlueFlagArrow = [];

    public static bool TriggerRedTeamWin;
    public static bool TriggerBlueTeamWin;
    public static bool TriggerDrawWin;

    public static string FlagpointCounter = new StringBuilder(Tr.Get(TrKey.Score)).Append("<color=#FF0000FF>").Append(CurrentRedTeamPoints).Append("</color> - <color=#0000FFFF>").Append(CurrentBlueTeamPoints).Append("</color>").ToString();

    public static void ClearAndReload()
    {
        Createdcapturetheflag = false;

        RedteamFlag.Clear();
        Redplayer01 = null;
        Redplayer01CurrentTarget = null;
        Redplayer01IsReviving = false;
        Redplayer02 = null;
        Redplayer02IsReviving = false;
        Redplayer02CurrentTarget = null;
        Redplayer03 = null;
        Redplayer03IsReviving = false;
        Redplayer03CurrentTarget = null;
        Redplayer04 = null;
        Redplayer04IsReviving = false;
        Redplayer04CurrentTarget = null;
        Redplayer05 = null;
        Redplayer05IsReviving = false;
        Redplayer05CurrentTarget = null;
        Redplayer06 = null;
        Redplayer06IsReviving = false;
        Redplayer06CurrentTarget = null;
        Redplayer07 = null;
        Redplayer07IsReviving = false;
        Redplayer07CurrentTarget = null;
        BlueteamFlag.Clear();
        Blueplayer01 = null;
        Blueplayer01IsReviving = false;
        Blueplayer01CurrentTarget = null;
        Blueplayer02 = null;
        Blueplayer02IsReviving = false;
        Blueplayer02CurrentTarget = null;
        Blueplayer03 = null;
        Blueplayer03IsReviving = false;
        Blueplayer03CurrentTarget = null;
        Blueplayer04 = null;
        Blueplayer04IsReviving = false;
        Blueplayer04CurrentTarget = null;
        Blueplayer05 = null;
        Blueplayer05IsReviving = false;
        Blueplayer05CurrentTarget = null;
        Blueplayer06 = null;
        Blueplayer06IsReviving = false;
        Blueplayer06CurrentTarget = null;
        Blueplayer07 = null;
        Blueplayer07IsReviving = false;
        Blueplayer07CurrentTarget = null;
        StealerPlayer = null;
        StealerPlayerIsReviving = false;
        StealerPlayercurrentTarget = null;
        StealerSpawns.Clear();

        RequiredFlags = CustomOptionHolder.RequiredFlags.GetFloat();
        Redflag = null;
        Redflagbase = null;
        Redflagtaken = false;
        RedPlayerWhoHasBlueFlag = null;
        CurrentRedTeamPoints = 0;
        Blueflag = null;
        Blueflagbase = null;
        Blueflagtaken = false;
        BluePlayerWhoHasRedFlag = null;
        TriggerRedTeamWin = false;
        TriggerBlueTeamWin = false;
        TriggerDrawWin = false;
        CurrentBlueTeamPoints = 0;
        LocalRedFlagArrow = [];
        LocalBlueFlagArrow = [];
        FlagpointCounter = new StringBuilder(Tr.Get(TrKey.Score)).Append("<color=#FF0000FF>").Append(CurrentRedTeamPoints).Append("</color> - <color=#0000FFFF>").Append(CurrentBlueTeamPoints).Append("</color>").ToString();
    }

    public static void CreateCtf()
    {
        var stealerPlayerPos = new Vector3();
        var redTeamPos = new Vector3();
        var blueTeamPos = new Vector3();
        var redFlagPos = new Vector3();
        var redFlagBasePos = new Vector3();
        var blueFlagPos = new Vector3();
        var blueFlagBasePos = new Vector3();

        switch (GameOptionsManager.Instance.currentGameOptions.MapId)
        {
            // Skeld / Custom Skeld
            case 0:
                if (RebuildUs.ActivatedSensei)
                {
                    stealerPlayerPos = new(-3.65f, 5f, PlayerControl.LocalPlayer.transform.position.z);
                    redTeamPos = new(-17.5f, -1.15f, PlayerControl.LocalPlayer.transform.position.z);
                    blueTeamPos = new(7.7f, -0.95f, PlayerControl.LocalPlayer.transform.position.z);
                    redFlagPos = new(-17.5f, -1.35f, 0.5f);
                    redFlagBasePos = new(-17.5f, -1.4f, 1f);
                    blueFlagPos = new(7.7f, -1.15f, 0.5f);
                    blueFlagBasePos = new(7.7f, -1.2f, 1f);
                }
                else if (RebuildUs.ActivatedDleks)
                {
                    stealerPlayerPos = new(-6.35f, -7.5f, PlayerControl.LocalPlayer.transform.position.z);
                    redTeamPos = new(20.5f, -5.15f, PlayerControl.LocalPlayer.transform.position.z);
                    blueTeamPos = new(-16.5f, -4.45f, PlayerControl.LocalPlayer.transform.position.z);
                    redFlagPos = new(20.5f, -5.35f, 0.5f);
                    redFlagBasePos = new(20.5f, -5.4f, 1f);
                    blueFlagPos = new(-16.5f, -4.65f, 0.5f);
                    blueFlagBasePos = new(-16.5f, -4.7f, 1f);
                }
                else
                {
                    stealerPlayerPos = new(6.35f, -7.5f, PlayerControl.LocalPlayer.transform.position.z);
                    redTeamPos = new(-20.5f, -5.15f, PlayerControl.LocalPlayer.transform.position.z);
                    blueTeamPos = new(16.5f, -4.45f, PlayerControl.LocalPlayer.transform.position.z);
                    redFlagPos = new(-20.5f, -5.35f, 0.5f);
                    redFlagBasePos = new(-20.5f, -5.4f, 1f);
                    blueFlagPos = new(16.5f, -4.65f, 0.5f);
                    blueFlagBasePos = new(16.5f, -4.7f, 1f);
                }

                break;
            // Mira HQ
            case 1:
                stealerPlayerPos = new(17.75f, 24f, PlayerControl.LocalPlayer.transform.position.z);
                redTeamPos = new(2.53f, 10.75f, PlayerControl.LocalPlayer.transform.position.z);
                blueTeamPos = new(23.25f, 5.25f, PlayerControl.LocalPlayer.transform.position.z);
                redFlagPos = new(2.525f, 10.55f, 0.5f);
                redFlagBasePos = new(2.53f, 10.5f, 1f);
                blueFlagPos = new(23.25f, 5.05f, 0.5f);
                blueFlagBasePos = new(23.25f, 5f, 1f);
                break;
            // Polus
            case 2:
                stealerPlayerPos = new(31.75f, -13f, PlayerControl.LocalPlayer.transform.position.z);
                redTeamPos = new(36.4f, -21.5f, PlayerControl.LocalPlayer.transform.position.z);
                blueTeamPos = new(5.4f, -9.45f, PlayerControl.LocalPlayer.transform.position.z);
                redFlagPos = new(36.4f, -21.7f, 0.5f);
                redFlagBasePos = new(36.4f, -21.75f, 1f);
                blueFlagPos = new(5.4f, -9.65f, 0.5f);
                blueFlagBasePos = new(5.4f, -9.7f, 1f);
                break;
            // Dleks
            case 3:
                stealerPlayerPos = new(-6.35f, -7.5f, PlayerControl.LocalPlayer.transform.position.z);
                redTeamPos = new(20.5f, -5.15f, PlayerControl.LocalPlayer.transform.position.z);
                blueTeamPos = new(-16.5f, -4.45f, PlayerControl.LocalPlayer.transform.position.z);
                redFlagPos = new(20.5f, -5.35f, 0.5f);
                redFlagBasePos = new(20.5f, -5.4f, 1f);
                blueFlagPos = new(-16.5f, -4.65f, 0.5f);
                blueFlagBasePos = new(-16.5f, -4.7f, 1f);
                break;
            // Airship
            case 4:
                stealerPlayerPos = new(10.25f, -15.35f, PlayerControl.LocalPlayer.transform.position.z);
                redTeamPos = new(-17.5f, -1f, PlayerControl.LocalPlayer.transform.position.z);
                blueTeamPos = new(33.6f, 1.45f, PlayerControl.LocalPlayer.transform.position.z);
                redFlagPos = new(-17.5f, -1.2f, 0.5f);
                redFlagBasePos = new(-17.5f, -1.25f, 1f);
                blueFlagPos = new(33.6f, 1.25f, 0.5f);
                blueFlagBasePos = new(33.6f, 1.2f, 1f);
                break;
            // Fungle
            case 5:
                stealerPlayerPos = new(2.85f, -5.75f, PlayerControl.LocalPlayer.transform.position.z);
                redTeamPos = new(-23f, -0.45f, PlayerControl.LocalPlayer.transform.position.z);
                blueTeamPos = new(19.25f, 2.35f, PlayerControl.LocalPlayer.transform.position.z);
                redFlagPos = new(-23f, -0.65f, 0.5f);
                redFlagBasePos = new(-23, -0.7f, 1f);
                blueFlagPos = new(19.25f, 2.15f, 0.5f);
                blueFlagBasePos = new(19.25f, 2.1f, 1f);
                break;
            // Submerged
            case 6:
                stealerPlayerPos = new(1f, 10f, PlayerControl.LocalPlayer.transform.position.z);
                redTeamPos = new(-8.35f, 28.25f, PlayerControl.LocalPlayer.transform.position.z);
                blueTeamPos = new(12.5f, -31.25f, PlayerControl.LocalPlayer.transform.position.z);
                redFlagPos = new(-8.35f, 28.05f, 0.03f);
                redFlagBasePos = new(-8.35f, 28, 0.031f);
                blueFlagPos = new(12.5f, -31.45f, -0.011f);
                blueFlagBasePos = new(12.5f, -31.5f, -0.01f);

                // Add another respawn on each floor
                var redteamfloor = GameObject.Instantiate(AssetLoader.Redfloor, PlayerControl.LocalPlayer.transform.parent);
                redteamfloor.name = "redteamfloor";
                redteamfloor.transform.position = new(-14f, -27.5f, -0.01f);
                var blueteamfloor = GameObject.Instantiate(AssetLoader.Bluefloor, PlayerControl.LocalPlayer.transform.parent);
                blueteamfloor.name = "blueteamfloor";
                blueteamfloor.transform.position = new(14.25f, 24.25f, 0.03f);
                break;
        }

        foreach (var player in RedteamFlag) player.transform.position = redTeamPos;

        foreach (var player in BlueteamFlag) player.transform.position = blueTeamPos;

        if (PlayerControl.LocalPlayer != null && !Createdcapturetheflag)
        {
            PlayerControl.LocalPlayer.ClearAllTasks();

            var redflag = GameObject.Instantiate(AssetLoader.Redflag, PlayerControl.LocalPlayer.transform.parent);
            redflag.name = "redflag";
            redflag.transform.position = redFlagPos;
            Redflag = redflag;
            var redflagbase = GameObject.Instantiate(AssetLoader.Redflagbase, PlayerControl.LocalPlayer.transform.parent);
            redflagbase.name = "redflagbase";
            redflagbase.transform.position = redFlagBasePos;
            Redflagbase = redflagbase;
            var blueflag = GameObject.Instantiate(AssetLoader.Blueflag, PlayerControl.LocalPlayer.transform.parent);
            blueflag.name = "blueflag";
            blueflag.transform.position = blueFlagPos;
            Blueflag = blueflag;
            var blueflagbase = GameObject.Instantiate(AssetLoader.Blueflagbase, PlayerControl.LocalPlayer.transform.parent);
            blueflagbase.name = "blueflagbase";
            blueflagbase.transform.position = blueFlagBasePos;
            Blueflagbase = blueflagbase;

            if (StealerPlayer != null)
            {
                StealerPlayer.MyPhysics.SetBodyType(PlayerBodyTypes.Seeker);
                StealerPlayer.transform.position = stealerPlayerPos;
                StealerSpawns.Add(redflagbase);
                StealerSpawns.Add(blueflagbase);
            }

            Createdcapturetheflag = true;
        }
    }

    public static void SetTarget()
    {
        if (MapSettings.GameMode is not CustomGameMode.CaptureTheFlag) return;

        var untargetableAllPlayers = new List<PlayerControl>();

        var untargetableRedPlayers = new List<PlayerControl>();
        foreach (var player in RedteamFlag) untargetableRedPlayers.Add(player);

        var untargetableBluePlayers = new List<PlayerControl>();
        foreach (var player in BlueteamFlag) untargetableBluePlayers.Add(player);

        // Prevent killing reviving players
        if (Blueplayer01IsReviving)
        {
            untargetableRedPlayers.Add(Blueplayer01);
            untargetableAllPlayers.Add(Blueplayer01);
        }
        else
        {
            untargetableRedPlayers.Remove(Blueplayer01);
            untargetableAllPlayers.Remove(Blueplayer01);
        }

        if (Blueplayer02IsReviving)
        {
            untargetableRedPlayers.Add(Blueplayer02);
            untargetableAllPlayers.Add(Blueplayer02);
        }
        else
        {
            untargetableRedPlayers.Remove(Blueplayer02);
            untargetableAllPlayers.Remove(Blueplayer02);
        }

        if (Blueplayer03IsReviving)
        {
            untargetableRedPlayers.Add(Blueplayer03);
            untargetableAllPlayers.Add(Blueplayer03);
        }
        else
        {
            untargetableRedPlayers.Remove(Blueplayer03);
            untargetableAllPlayers.Remove(Blueplayer03);
        }

        if (Blueplayer04IsReviving)
        {
            untargetableRedPlayers.Add(Blueplayer04);
            untargetableAllPlayers.Add(Blueplayer04);
        }
        else
        {
            untargetableRedPlayers.Remove(Blueplayer04);
            untargetableAllPlayers.Remove(Blueplayer04);
        }

        if (Blueplayer05IsReviving)
        {
            untargetableRedPlayers.Add(Blueplayer05);
            untargetableAllPlayers.Add(Blueplayer05);
        }
        else
        {
            untargetableRedPlayers.Remove(Blueplayer05);
            untargetableAllPlayers.Remove(Blueplayer05);
        }

        if (Blueplayer06IsReviving)
        {
            untargetableRedPlayers.Add(Blueplayer06);
            untargetableAllPlayers.Add(Blueplayer06);
        }
        else
        {
            untargetableRedPlayers.Remove(Blueplayer06);
            untargetableAllPlayers.Remove(Blueplayer06);
        }

        if (Blueplayer07IsReviving)
        {
            untargetableRedPlayers.Add(Blueplayer07);
            untargetableAllPlayers.Add(Blueplayer07);
        }
        else
        {
            untargetableRedPlayers.Remove(Blueplayer07);
            untargetableAllPlayers.Remove(Blueplayer07);
        }

        if (StealerPlayerIsReviving)
        {
            untargetableRedPlayers.Add(StealerPlayer);
            untargetableBluePlayers.Add(StealerPlayer);
        }
        else
        {
            untargetableRedPlayers.Remove(StealerPlayer);
            untargetableBluePlayers.Remove(StealerPlayer);
        }

        if (Redplayer01 != null && Redplayer01 == PlayerControl.LocalPlayer)
        {
            Redplayer01CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetableRedPlayers);
            Helpers.SetPlayerOutline(Redplayer01CurrentTarget, Palette.ImpostorRed);
        }

        if (Redplayer02 != null && Redplayer02 == PlayerControl.LocalPlayer)
        {
            Redplayer02CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetableRedPlayers);
            Helpers.SetPlayerOutline(Redplayer02CurrentTarget, Palette.ImpostorRed);
        }

        if (Redplayer03 != null && Redplayer03 == PlayerControl.LocalPlayer)
        {
            Redplayer03CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetableRedPlayers);
            Helpers.SetPlayerOutline(Redplayer03CurrentTarget, Palette.ImpostorRed);
        }

        if (Redplayer04 != null && Redplayer04 == PlayerControl.LocalPlayer)
        {
            Redplayer04CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetableRedPlayers);
            Helpers.SetPlayerOutline(Redplayer04CurrentTarget, Palette.ImpostorRed);
        }

        if (Redplayer05 != null && Redplayer05 == PlayerControl.LocalPlayer)
        {
            Redplayer05CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetableRedPlayers);
            Helpers.SetPlayerOutline(Redplayer05CurrentTarget, Palette.ImpostorRed);
        }

        if (Redplayer06 != null && Redplayer06 == PlayerControl.LocalPlayer)
        {
            Redplayer06CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetableRedPlayers);
            Helpers.SetPlayerOutline(Redplayer06CurrentTarget, Palette.ImpostorRed);
        }

        if (Redplayer07 != null && Redplayer07 == PlayerControl.LocalPlayer)
        {
            Redplayer07CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetableRedPlayers);
            Helpers.SetPlayerOutline(Redplayer07CurrentTarget, Palette.ImpostorRed);
        }

        // Prevent killing reviving players
        if (Redplayer01IsReviving)
        {
            untargetableBluePlayers.Add(Redplayer01);
            untargetableAllPlayers.Add(Redplayer01);
        }
        else
        {
            untargetableBluePlayers.Remove(Redplayer01);
            untargetableAllPlayers.Remove(Redplayer01);
        }

        if (Redplayer02IsReviving)
        {
            untargetableBluePlayers.Add(Redplayer02);
            untargetableAllPlayers.Add(Redplayer02);
        }
        else
        {
            untargetableBluePlayers.Remove(Redplayer02);
            untargetableAllPlayers.Remove(Redplayer02);
        }

        if (Redplayer03IsReviving)
        {
            untargetableBluePlayers.Add(Redplayer03);
            untargetableAllPlayers.Add(Redplayer03);
        }
        else
        {
            untargetableBluePlayers.Remove(Redplayer03);
            untargetableAllPlayers.Remove(Redplayer03);
        }

        if (Redplayer04IsReviving)
        {
            untargetableBluePlayers.Add(Redplayer04);
            untargetableAllPlayers.Add(Redplayer04);
        }
        else
        {
            untargetableBluePlayers.Remove(Redplayer04);
            untargetableAllPlayers.Remove(Redplayer04);
        }

        if (Redplayer05IsReviving)
        {
            untargetableBluePlayers.Add(Redplayer05);
            untargetableAllPlayers.Add(Redplayer05);
        }
        else
        {
            untargetableBluePlayers.Remove(Redplayer05);
            untargetableAllPlayers.Remove(Redplayer05);
        }

        if (Redplayer06IsReviving)
        {
            untargetableBluePlayers.Add(Redplayer06);
            untargetableAllPlayers.Add(Redplayer06);
        }
        else
        {
            untargetableBluePlayers.Remove(Redplayer06);
            untargetableAllPlayers.Remove(Redplayer06);
        }

        if (Redplayer07IsReviving)
        {
            untargetableBluePlayers.Add(Redplayer07);
            untargetableAllPlayers.Add(Redplayer07);
        }
        else
        {
            untargetableBluePlayers.Remove(Redplayer07);
            untargetableAllPlayers.Remove(Redplayer07);
        }

        if (Blueplayer01 != null && Blueplayer01 == PlayerControl.LocalPlayer)
        {
            Blueplayer01CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetableBluePlayers);
            Helpers.SetPlayerOutline(Blueplayer01CurrentTarget, Color.blue);
        }

        if (Blueplayer02 != null && Blueplayer02 == PlayerControl.LocalPlayer)
        {
            Blueplayer02CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetableBluePlayers);
            Helpers.SetPlayerOutline(Blueplayer02CurrentTarget, Color.blue);
        }

        if (Blueplayer03 != null && Blueplayer03 == PlayerControl.LocalPlayer)
        {
            Blueplayer03CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetableBluePlayers);
            Helpers.SetPlayerOutline(Blueplayer03CurrentTarget, Color.blue);
        }

        if (Blueplayer04 != null && Blueplayer04 == PlayerControl.LocalPlayer)
        {
            Blueplayer04CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetableBluePlayers);
            Helpers.SetPlayerOutline(Blueplayer04CurrentTarget, Color.blue);
        }

        if (Blueplayer05 != null && Blueplayer05 == PlayerControl.LocalPlayer)
        {
            Blueplayer05CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetableBluePlayers);
            Helpers.SetPlayerOutline(Blueplayer05CurrentTarget, Color.blue);
        }

        if (Blueplayer06 != null && Blueplayer06 == PlayerControl.LocalPlayer)
        {
            Blueplayer06CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetableBluePlayers);
            Helpers.SetPlayerOutline(Blueplayer06CurrentTarget, Color.blue);
        }

        if (Blueplayer07 != null && Blueplayer07 == PlayerControl.LocalPlayer)
        {
            Blueplayer07CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetableBluePlayers);
            Helpers.SetPlayerOutline(Blueplayer07CurrentTarget, Color.blue);
        }

        if (StealerPlayer != null && StealerPlayer == PlayerControl.LocalPlayer)
        {
            StealerPlayercurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetableAllPlayers);
            Helpers.SetPlayerOutline(StealerPlayercurrentTarget, Color.grey);
        }
    }

    public static void OnMurderPlayerPostfix(PlayerControl __instance, PlayerControl target)
    {
        if (RedPlayerWhoHasBlueFlag != null && target == RedPlayerWhoHasBlueFlag)
        {
            Blueflagtaken = false;
            Helpers.ShowGamemodesPopUp(0, Helpers.PlayerById(RedPlayerWhoHasBlueFlag.PlayerId));
            RedPlayerWhoHasBlueFlag = null;
            Blueflag.transform.parent = Blueflagbase.transform.parent;
            switch (GameOptionsManager.Instance.currentGameOptions.MapId)
            {
                // Skeld
                case 0:
                    if (RebuildUs.ActivatedSensei)
                        Blueflag.transform.position = new(7.7f, -1.15f, 0.5f);
                    else if (RebuildUs.ActivatedDleks)
                        Blueflag.transform.position = new(-16.5f, -4.65f, 0.5f);
                    else
                        Blueflag.transform.position = new(16.5f, -4.65f, 0.5f);
                    break;
                // MiraHQ
                case 1:
                    Blueflag.transform.position = new(23.25f, 5.05f, 0.5f);
                    break;
                // Polus
                case 2:
                    Blueflag.transform.position = new(5.4f, -9.65f, 0.5f);
                    break;
                // Dleks
                case 3:
                    Blueflag.transform.position = new(-16.5f, -4.65f, 0.5f);
                    break;
                // Airship
                case 4:
                    Blueflag.transform.position = new(33.6f, 1.25f, 0.5f);
                    break;
                // Fungle
                case 5:
                    Blueflag.transform.position = new(19.25f, 2.15f, 0.5f);
                    break;
                // Submerged
                case 6:
                    Blueflag.transform.position = new(12.5f, -31.45f, -0.011f);
                    break;
            }
        }

        if (BluePlayerWhoHasRedFlag != null && target == BluePlayerWhoHasRedFlag)
        {
            Redflagtaken = false;
            Helpers.ShowGamemodesPopUp(0, Helpers.PlayerById(BluePlayerWhoHasRedFlag.PlayerId));
            BluePlayerWhoHasRedFlag = null;
            Redflag.transform.parent = Redflagbase.transform.parent;
            switch (GameOptionsManager.Instance.currentGameOptions.MapId)
            {
                // Skeld
                case 0:
                    if (RebuildUs.ActivatedSensei)
                        Redflag.transform.position = new(-17.5f, -1.35f, 0.5f);
                    else if (RebuildUs.ActivatedDleks)
                        Redflag.transform.position = new(20.5f, -5.35f, 0.5f);
                    else
                        Redflag.transform.position = new(-20.5f, -5.35f, 0.5f);
                    break;
                // MiraHQ
                case 1:
                    Redflag.transform.position = new(2.525f, 10.55f, 0.5f);
                    break;
                // Polus
                case 2:
                    Redflag.transform.position = new(36.4f, -21.7f, 0.5f);
                    break;
                // Dlesk
                case 3:
                    Redflag.transform.position = new(20.5f, -5.35f, 0.5f);
                    break;
                // Airship
                case 4:
                    Redflag.transform.position = new(-17.5f, -1.2f, 0.5f);
                    break;
                // Fungle
                case 5:
                    Redflag.transform.position = new(-23f, -0.65f, 0.5f);
                    break;
                // Submerged
                case 6:
                    Redflag.transform.position = new(-8.35f, 28.05f, 0.03f);
                    break;
            }
        }

        // Capture the flag revive player
        if (StealerPlayer != null && StealerPlayer.PlayerId == target.PlayerId)
        {
            var ctfBody = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == target.PlayerId);
            ctfBody.transform.position = new(50, 50, 1);
            StealerPlayerIsReviving = true;
            StealerPlayer.MyPhysics.SetBodyType(PlayerBodyTypes.Normal);
            Helpers.AlphaPlayer(true, StealerPlayer.PlayerId);
            HudManager.Instance.StartCoroutine(Effects.Lerp(MapSettings.GamemodeReviveTime, new Action<float>(p =>
            {
                if (p == 1f && StealerPlayer != null)
                {
                    StealerPlayerIsReviving = false;
                    Helpers.AlphaPlayer(false, StealerPlayer.PlayerId);
                    StealerPlayer.MyPhysics.SetBodyType(PlayerBodyTypes.Seeker);
                }
            })));
            HudManager.Instance.StartCoroutine(Effects.Lerp(MapSettings.GamemodeReviveTime - MapSettings.GamemodeInvincibilityTime, new Action<float>(p =>
            {
                if (p == 1f && StealerPlayer != null)
                {
                    StealerPlayer.Revive();
                    switch (GameOptionsManager.Instance.currentGameOptions.MapId)
                    {
                        // Skeld
                        case 0:
                            if (RebuildUs.ActivatedSensei)
                                StealerPlayer.transform.position = new(-3.65f, 5f, StealerPlayer.transform.position.z);
                            else if (RebuildUs.ActivatedDleks)
                                StealerPlayer.transform.position = new(-6.35f, -7.5f, StealerPlayer.transform.position.z);
                            else
                                StealerPlayer.transform.position = new(6.35f, -7.5f, StealerPlayer.transform.position.z);
                            break;
                        // MiraHQ
                        case 1:
                            StealerPlayer.transform.position = new(17.75f, 24f, StealerPlayer.transform.position.z);
                            break;
                        // Polus
                        case 2:
                            StealerPlayer.transform.position = new(31.75f, -13f, StealerPlayer.transform.position.z);
                            break;
                        // Dleks
                        case 3:
                            StealerPlayer.transform.position = new(-6.35f, -7.5f, StealerPlayer.transform.position.z);
                            break;
                        // Airship
                        case 4:
                            StealerPlayer.transform.position = new(10.25f, -15.35f, StealerPlayer.transform.position.z);
                            break;
                        // Fungle
                        case 5:
                            StealerPlayer.transform.position = new(2.85f, -5.75f, StealerPlayer.transform.position.z);
                            break;
                        // Submerged
                        case 6:
                            if (StealerPlayer.transform.position.y > 0)
                                StealerPlayer.transform.position = new(1f, 10f, StealerPlayer.transform.position.z);
                            else
                                StealerPlayer.transform.position = new(0f, -33.5f, StealerPlayer.transform.position.z);
                            break;
                    }

                    var deadPlayerEntry = GameHistory.DEAD_PLAYERS.Where(x => x.Player.PlayerId == target.PlayerId).FirstOrDefault();
                    if (ctfBody != null) Object.Destroy(ctfBody.gameObject);
                    if (deadPlayerEntry != null) GameHistory.DEAD_PLAYERS.Remove(deadPlayerEntry);
                }
            })));
        }

        foreach (var player in RedteamFlag)
        {
            if (player.PlayerId == target.PlayerId)
            {
                var ctfBody = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == target.PlayerId);
                ctfBody.transform.position = new(50, 50, 1);
                if (Redplayer01 != null && target.PlayerId == Redplayer01.PlayerId)
                    Redplayer01IsReviving = true;
                else if (Redplayer02 != null && target.PlayerId == Redplayer02.PlayerId)
                    Redplayer02IsReviving = true;
                else if (Redplayer03 != null && target.PlayerId == Redplayer03.PlayerId)
                    Redplayer03IsReviving = true;
                else if (Redplayer04 != null && target.PlayerId == Redplayer04.PlayerId)
                    Redplayer04IsReviving = true;
                else if (Redplayer05 != null && target.PlayerId == Redplayer05.PlayerId)
                    Redplayer05IsReviving = true;
                else if (Redplayer06 != null && target.PlayerId == Redplayer06.PlayerId)
                    Redplayer06IsReviving = true;
                else if (Redplayer07 != null && target.PlayerId == Redplayer07.PlayerId) Redplayer07IsReviving = true;
                Helpers.AlphaPlayer(true, player.PlayerId);
                HudManager.Instance.StartCoroutine(Effects.Lerp(MapSettings.GamemodeReviveTime, new Action<float>(p =>
                {
                    if (p == 1f && player != null)
                    {
                        if (Redplayer01 != null && target.PlayerId == Redplayer01.PlayerId)
                            Redplayer01IsReviving = false;
                        else if (Redplayer02 != null && target.PlayerId == Redplayer02.PlayerId)
                            Redplayer02IsReviving = false;
                        else if (Redplayer03 != null && target.PlayerId == Redplayer03.PlayerId)
                            Redplayer03IsReviving = false;
                        else if (Redplayer04 != null && target.PlayerId == Redplayer04.PlayerId)
                            Redplayer04IsReviving = false;
                        else if (Redplayer05 != null && target.PlayerId == Redplayer05.PlayerId)
                            Redplayer05IsReviving = false;
                        else if (Redplayer06 != null && target.PlayerId == Redplayer06.PlayerId)
                            Redplayer06IsReviving = false;
                        else if (Redplayer07 != null && target.PlayerId == Redplayer07.PlayerId) Redplayer07IsReviving = false;
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
                                    player.transform.position = new(20.5f, -5.15f, player.transform.position.z);
                                else
                                    player.transform.position = new(-20.5f, -5.15f, player.transform.position.z);
                                break;
                            // MiraHQ
                            case 1:
                                player.transform.position = new(2.53f, 10.75f, player.transform.position.z);
                                break;
                            // Polus
                            case 2:
                                player.transform.position = new(36.4f, -21.5f, player.transform.position.z);
                                break;
                            // Dleks
                            case 3:
                                player.transform.position = new(20.5f, -5.15f, player.transform.position.z);
                                break;
                            // Airship
                            case 4:
                                player.transform.position = new(-17.5f, -1.1f, player.transform.position.z);
                                break;
                            // Fungle
                            case 5:
                                player.transform.position = new(-23f, -0.45f, player.transform.position.z);
                                break;
                            // Submerged
                            case 6:
                                if (player.transform.position.y > 0)
                                    player.transform.position = new(-8.35f, 28.25f, player.transform.position.z);
                                else
                                    player.transform.position = new(-14f, -27.5f, player.transform.position.z);
                                break;
                        }

                        var deadPlayerEntry = GameHistory.DEAD_PLAYERS.Where(x => x.Player.PlayerId == target.PlayerId).FirstOrDefault();
                        if (ctfBody != null) Object.Destroy(ctfBody.gameObject);
                        if (deadPlayerEntry != null) GameHistory.DEAD_PLAYERS.Remove(deadPlayerEntry);
                    }
                })));
            }
        }

        foreach (var player in BlueteamFlag)
        {
            if (player.PlayerId == target.PlayerId)
            {
                var ctfBody = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == target.PlayerId);
                ctfBody.transform.position = new(50, 50, 1);
                if (Blueplayer01 != null && target.PlayerId == Blueplayer01.PlayerId)
                    Blueplayer01IsReviving = true;
                else if (Blueplayer02 != null && target.PlayerId == Blueplayer02.PlayerId)
                    Blueplayer02IsReviving = true;
                else if (Blueplayer03 != null && target.PlayerId == Blueplayer03.PlayerId)
                    Blueplayer03IsReviving = true;
                else if (Blueplayer04 != null && target.PlayerId == Blueplayer04.PlayerId)
                    Blueplayer04IsReviving = true;
                else if (Blueplayer05 != null && target.PlayerId == Blueplayer05.PlayerId)
                    Blueplayer05IsReviving = true;
                else if (Blueplayer06 != null && target.PlayerId == Blueplayer06.PlayerId)
                    Blueplayer06IsReviving = true;
                else if (Blueplayer07 != null && target.PlayerId == Blueplayer07.PlayerId) Blueplayer07IsReviving = true;
                Helpers.AlphaPlayer(true, player.PlayerId);
                HudManager.Instance.StartCoroutine(Effects.Lerp(MapSettings.GamemodeReviveTime, new Action<float>(p =>
                {
                    if (p == 1f && player != null)
                    {
                        if (Blueplayer01 != null && target.PlayerId == Blueplayer01.PlayerId)
                            Blueplayer01IsReviving = false;
                        else if (Blueplayer02 != null && target.PlayerId == Blueplayer02.PlayerId)
                            Blueplayer02IsReviving = false;
                        else if (Blueplayer03 != null && target.PlayerId == Blueplayer03.PlayerId)
                            Blueplayer03IsReviving = false;
                        else if (Blueplayer04 != null && target.PlayerId == Blueplayer04.PlayerId)
                            Blueplayer04IsReviving = false;
                        else if (Blueplayer05 != null && target.PlayerId == Blueplayer05.PlayerId)
                            Blueplayer05IsReviving = false;
                        else if (Blueplayer06 != null && target.PlayerId == Blueplayer06.PlayerId)
                            Blueplayer06IsReviving = false;
                        else if (Blueplayer07 != null && target.PlayerId == Blueplayer07.PlayerId) Blueplayer07IsReviving = false;
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
                                    player.transform.position = new(-16.5f, -4.45f, player.transform.position.z);
                                else
                                    player.transform.position = new(16.5f, -4.45f, player.transform.position.z);
                                break;
                            // MiraHQ
                            case 1:
                                player.transform.position = new(23.25f, 5.25f, player.transform.position.z);
                                break;
                            // Polus
                            case 2:
                                player.transform.position = new(5.4f, -9.45f, player.transform.position.z);
                                break;
                            // Dleks
                            case 3:
                                player.transform.position = new(-16.5f, -4.45f, player.transform.position.z);
                                break;
                            // Airship
                            case 4:
                                player.transform.position = new(33.6f, 1.45f, player.transform.position.z);
                                break;
                            // Fungle
                            case 5:
                                player.transform.position = new(19.25f, 2.35f, player.transform.position.z);
                                break;
                            // Submerged
                            case 6:
                                if (player.transform.position.y > 0)
                                    player.transform.position = new(14.25f, 24.25f, player.transform.position.z);
                                else
                                    player.transform.position = new(12.5f, -31.25f, player.transform.position.z);
                                break;
                        }

                        var deadPlayerEntry = GameHistory.DEAD_PLAYERS.Where(x => x.Player.PlayerId == target.PlayerId).FirstOrDefault();
                        if (ctfBody != null) Object.Destroy(ctfBody.gameObject);
                        if (deadPlayerEntry != null) GameHistory.DEAD_PLAYERS.Remove(deadPlayerEntry);
                    }
                })));
            }
        }
    }

    public static void CaptureTheFlagUpdate()
    {
        if (MapSettings.GameMode is not CustomGameMode.CaptureTheFlag) return;

        if (RedPlayerWhoHasBlueFlag != null && RedPlayerWhoHasBlueFlag.Data.Disconnected)
        {
            Blueflag.transform.parent = Blueflagbase.transform.parent;
            switch (GameOptionsManager.Instance.currentGameOptions.MapId)
            {
                // Skeld
                case 0:
                    if (RebuildUs.ActivatedSensei)
                        Blueflag.transform.position = new(7.7f, -1.15f, 0.5f);
                    else if (RebuildUs.ActivatedDleks)
                        Blueflag.transform.position = new(-16.5f, -4.65f, 0.5f);
                    else
                        Blueflag.transform.position = new(16.5f, -4.65f, 0.5f);
                    break;
                // MiraHQ
                case 1:
                    Blueflag.transform.position = new(23.25f, 5.05f, 0.5f);
                    break;
                // Polus
                case 2:
                    Blueflag.transform.position = new(5.4f, -9.65f, 0.5f);
                    break;
                // Dleks
                case 3:
                    Blueflag.transform.position = new(-16.5f, -4.65f, 0.5f);
                    break;
                // Airship
                case 4:
                    Blueflag.transform.position = new(33.6f, 1.25f, 0.5f);
                    break;
                // Fungle
                case 5:
                    Blueflag.transform.position = new(19.25f, 2.15f, 0.5f);
                    break;
                // Submerged
                case 6:
                    Blueflag.transform.position = new(12.5f, -31.45f, -0.011f);
                    break;
            }

            Blueflagtaken = false;
            RedPlayerWhoHasBlueFlag = null;
        }

        if (BluePlayerWhoHasRedFlag != null && BluePlayerWhoHasRedFlag.Data.Disconnected)
        {
            Redflag.transform.parent = Redflagbase.transform.parent;
            switch (GameOptionsManager.Instance.currentGameOptions.MapId)
            {
                // Skeld
                case 0:
                    if (RebuildUs.ActivatedSensei)
                        Redflag.transform.position = new(-17.5f, -1.35f, 0.5f);
                    else if (RebuildUs.ActivatedDleks)
                        Redflag.transform.position = new(20.5f, -5.35f, 0.5f);
                    else
                        Redflag.transform.position = new(-20.5f, -5.35f, 0.5f);
                    break;
                // MiraHQ
                case 1:
                    Redflag.transform.position = new(2.525f, 10.55f, 0.5f);
                    break;
                // Polus
                case 2:
                    Redflag.transform.position = new(36.4f, -21.7f, 0.5f);
                    break;
                // Dleks
                case 3:
                    Redflag.transform.position = new(20.5f, -5.35f, 0.5f);
                    break;
                // Airship
                case 4:
                    Redflag.transform.position = new(-17.5f, -1.2f, 0.5f);
                    break;
                // Fungle
                case 5:
                    Redflag.transform.position = new(-23f, -0.65f, 0.5f);
                    break;
                // Submerged
                case 6:
                    Redflag.transform.position = new(-8.35f, 28.05f, 0.03f);
                    break;
            }

            Redflagtaken = false;
            BluePlayerWhoHasRedFlag = null;
        }
    }
}
