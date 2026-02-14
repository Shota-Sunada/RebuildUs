namespace RebuildUs.Modules.GameMode;

public static partial class HotPotato
{
    public static Color IntroColor = new Color32(242, 190, 255, byte.MaxValue);

    public static bool createdhotpotato = false;

    public static List<PlayerControl> notPotatoTeam = [];
    public static List<PlayerControl> notPotatoTeamAlive = [];
    public static List<PlayerControl> explodedPotatoTeam = [];

    public static PlayerControl hotPotatoPlayer = null;
    public static PlayerControl hotPotatoPlayerCurrentTarget = null;
    public static PlayerControl notPotato01 = null;
    public static PlayerControl notPotato02 = null;
    public static PlayerControl notPotato03 = null;
    public static PlayerControl notPotato04 = null;
    public static PlayerControl notPotato05 = null;
    public static PlayerControl notPotato06 = null;
    public static PlayerControl notPotato07 = null;
    public static PlayerControl notPotato08 = null;
    public static PlayerControl notPotato09 = null;
    public static PlayerControl notPotato10 = null;
    public static PlayerControl notPotato11 = null;
    public static PlayerControl notPotato12 = null;
    public static PlayerControl notPotato13 = null;
    public static PlayerControl notPotato14 = null;

    public static PlayerControl explodedPotato01 = null;
    public static PlayerControl explodedPotato02 = null;
    public static PlayerControl explodedPotato03 = null;
    public static PlayerControl explodedPotato04 = null;
    public static PlayerControl explodedPotato05 = null;
    public static PlayerControl explodedPotato06 = null;
    public static PlayerControl explodedPotato07 = null;
    public static PlayerControl explodedPotato08 = null;
    public static PlayerControl explodedPotato09 = null;
    public static PlayerControl explodedPotato10 = null;
    public static PlayerControl explodedPotato11 = null;
    public static PlayerControl explodedPotato12 = null;
    public static PlayerControl explodedPotato13 = null;
    public static PlayerControl explodedPotato14 = null;

    public static GameObject hotPotato = null;

    public static float timeforTransfer = 15;
    public static float transferCooldown = 10f;
    public static float savedtimeforTransfer = 15;
    public static bool resetTimeForTransfer = true;
    public static float increaseTimeIfNoReset = 5f;
    public static bool firstPotatoTransfered = false;

    public static bool notPotatoTeamAlerted = false;

    public static bool triggerHotPotatoEnd = false;

    public static string hotpotatopointCounter = new StringBuilder(Tr.Get(TrKey.HotPotatoStatus)).Append("<color=#808080FF></color> | ").Append(Tr.Get(TrKey.ColdPotatoes)).Append("<color=#00F7FFFF>").Append(notPotatoTeam.Count).Append("</color>").ToString();

    public static void clearAndReload()
    {
        createdhotpotato = false;

        notPotatoTeam.Clear();
        notPotatoTeamAlive.Clear();
        hotPotatoPlayer = null;
        hotPotatoPlayerCurrentTarget = null;
        notPotato01 = null;
        notPotato02 = null;
        notPotato03 = null;
        notPotato04 = null;
        notPotato05 = null;
        notPotato06 = null;
        notPotato07 = null;
        notPotato08 = null;
        notPotato09 = null;
        notPotato10 = null;
        notPotato11 = null;
        notPotato12 = null;
        notPotato13 = null;
        notPotato14 = null;

        explodedPotato01 = null;
        explodedPotato02 = null;
        explodedPotato03 = null;
        explodedPotato04 = null;
        explodedPotato05 = null;
        explodedPotato06 = null;
        explodedPotato07 = null;
        explodedPotato08 = null;
        explodedPotato09 = null;
        explodedPotato10 = null;
        explodedPotato11 = null;
        explodedPotato12 = null;
        explodedPotato13 = null;
        explodedPotato14 = null;

        timeforTransfer = CustomOptionHolder.hotPotatoTransferLimit.GetFloat() + 10f;
        transferCooldown = CustomOptionHolder.hotPotatoCooldown.GetFloat();
        resetTimeForTransfer = CustomOptionHolder.hotPotatoResetTimeForTransfer.GetBool();
        increaseTimeIfNoReset = CustomOptionHolder.hotPotatoIncreaseTimeIfNoReset.GetFloat();
        notPotatoTeamAlerted = false;
        triggerHotPotatoEnd = false;
        savedtimeforTransfer = timeforTransfer - 10f;
        firstPotatoTransfered = false;
        hotPotato = null;

        hotpotatopointCounter = new StringBuilder(Tr.Get(TrKey.HotPotatoStatus)).Append("<color=#00F7FFFF></color> | ").Append(Tr.Get(TrKey.ColdPotatoes)).Append("<color=#928B55FF>").Append(notPotatoTeam.Count).Append("</color>").ToString();
    }

    public static void CreateHP()
    {

        Vector3 hotPotatoPlayerPos = new Vector3();
        Vector3 notPotatoTeamPos = new Vector3();

        switch (GameOptionsManager.Instance.currentGameOptions.MapId)
        {
            // Skeld / Custom Skeld
            case 0:
                if (RebuildUs.activatedSensei)
                {
                    hotPotatoPlayerPos = new Vector3(-6.5f, -2.25f, PlayerControl.LocalPlayer.transform.position.z);
                    notPotatoTeamPos = new Vector3(12.5f, -0.25f, PlayerControl.LocalPlayer.transform.position.z);
                }
                else if (RebuildUs.activatedDleks)
                {
                    hotPotatoPlayerPos = new Vector3(0.75f, -7f, PlayerControl.LocalPlayer.transform.position.z);
                    notPotatoTeamPos = new Vector3(-6.25f, -3.5f, PlayerControl.LocalPlayer.transform.position.z);
                }
                else
                {
                    hotPotatoPlayerPos = new Vector3(-0.75f, -7f, PlayerControl.LocalPlayer.transform.position.z);
                    notPotatoTeamPos = new Vector3(6.25f, -3.5f, PlayerControl.LocalPlayer.transform.position.z);
                }
                break;
            // Mira HQ
            case 1:
                hotPotatoPlayerPos = new Vector3(6.15f, 6.25f, PlayerControl.LocalPlayer.transform.position.z);
                notPotatoTeamPos = new Vector3(17.75f, 11.5f, PlayerControl.LocalPlayer.transform.position.z);
                break;
            // Polus
            case 2:
                hotPotatoPlayerPos = new Vector3(20.5f, -11.75f, PlayerControl.LocalPlayer.transform.position.z);
                notPotatoTeamPos = new Vector3(12.25f, -16f, PlayerControl.LocalPlayer.transform.position.z);
                break;
            // Dleks
            case 3:
                hotPotatoPlayerPos = new Vector3(0.75f, -7f, PlayerControl.LocalPlayer.transform.position.z);
                notPotatoTeamPos = new Vector3(-6.25f, -3.5f, PlayerControl.LocalPlayer.transform.position.z);
                break;
            // Airship
            case 4:
                hotPotatoPlayerPos = new Vector3(12.25f, 2f, PlayerControl.LocalPlayer.transform.position.z);
                notPotatoTeamPos = new Vector3(6.25f, 2.5f, PlayerControl.LocalPlayer.transform.position.z);
                break;
            // Fungle
            case 5:
                hotPotatoPlayerPos = new Vector3(-10.75f, 12.75f, PlayerControl.LocalPlayer.transform.position.z);
                notPotatoTeamPos = new Vector3(-3.25f, -10.5f, PlayerControl.LocalPlayer.transform.position.z);
                break;
            // Submerged
            case 6:
                hotPotatoPlayerPos = new Vector3(-4.25f, -33.5f, PlayerControl.LocalPlayer.transform.position.z);
                notPotatoTeamPos = new Vector3(13f, -25.25f, PlayerControl.LocalPlayer.transform.position.z);
                break;
        }

        HotPotato.hotPotatoPlayer.MyPhysics.SetBodyType(PlayerBodyTypes.Seeker);
        HotPotato.hotPotatoPlayer.transform.position = hotPotatoPlayerPos;

        foreach (PlayerControl player in HotPotato.notPotatoTeam)
        {
            player.transform.position = notPotatoTeamPos;
        }

        if (PlayerControl.LocalPlayer != null && !createdhotpotato)
        {
            Helpers.ClearAllTasks(PlayerControl.LocalPlayer);

            GameObject hotpotato = GameObject.Instantiate(AssetLoader.hotPotato, HotPotato.hotPotatoPlayer.transform);
            hotpotato.name = "HotPotato";
            hotpotato.transform.position = HotPotato.hotPotatoPlayer.transform.position + new Vector3(0, 0.5f, -0.25f);
            HotPotato.hotPotato = hotpotato;

            HudManager.Instance.DangerMeter.gameObject.SetActive(true);

            createdhotpotato = true;
        }
    }

    public static void hotPotatoSetTarget()
    {
        if (MapSettings.GameMode is not CustomGameMode.HotPotato) return;

        if (HotPotato.hotPotatoPlayer != null && HotPotato.hotPotatoPlayer == PlayerControl.LocalPlayer)
        {
            HotPotato.hotPotatoPlayerCurrentTarget = Helpers.SetTarget();
            Helpers.SetPlayerOutline(HotPotato.hotPotatoPlayerCurrentTarget, Color.grey);
        }
    }

    public static void OnMurderPlayerPostfix(PlayerControl __instance, PlayerControl target)
    {
        if (HotPotato.hotPotatoPlayer != null && HotPotato.hotPotatoPlayer.PlayerId == target.PlayerId)
        {

            var hpBody = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == target.PlayerId);
            hpBody.transform.position = new Vector3(50, 50, 1);

            HotPotato.timeforTransfer = HotPotato.savedtimeforTransfer + 4f;

            HudManager.Instance.StartCoroutine(Effects.Lerp(1, new Action<float>((p) =>
            { // Delayed action
                if (p == 1f)
                {

                    if (HotPotato.explodedPotato01 == null)
                    {
                        HotPotato.explodedPotato01 = HotPotato.hotPotatoPlayer;
                        HotPotato.explodedPotatoTeam.Add(HotPotato.explodedPotato01);
                    }
                    else if (HotPotato.explodedPotato02 == null)
                    {
                        HotPotato.explodedPotato02 = HotPotato.hotPotatoPlayer;
                        HotPotato.explodedPotatoTeam.Add(HotPotato.explodedPotato02);
                    }
                    else if (HotPotato.explodedPotato03 == null)
                    {
                        HotPotato.explodedPotato03 = HotPotato.hotPotatoPlayer;
                        HotPotato.explodedPotatoTeam.Add(HotPotato.explodedPotato03);
                    }
                    else if (HotPotato.explodedPotato04 == null)
                    {
                        HotPotato.explodedPotato04 = HotPotato.hotPotatoPlayer;
                        HotPotato.explodedPotatoTeam.Add(HotPotato.explodedPotato04);
                    }
                    else if (HotPotato.explodedPotato05 == null)
                    {
                        HotPotato.explodedPotato05 = HotPotato.hotPotatoPlayer;
                        HotPotato.explodedPotatoTeam.Add(HotPotato.explodedPotato05);
                    }
                    else if (HotPotato.explodedPotato06 == null)
                    {
                        HotPotato.explodedPotato06 = HotPotato.hotPotatoPlayer;
                        HotPotato.explodedPotatoTeam.Add(HotPotato.explodedPotato06);
                    }
                    else if (HotPotato.explodedPotato07 == null)
                    {
                        HotPotato.explodedPotato07 = HotPotato.hotPotatoPlayer;
                        HotPotato.explodedPotatoTeam.Add(HotPotato.explodedPotato07);
                    }
                    else if (HotPotato.explodedPotato08 == null)
                    {
                        HotPotato.explodedPotato08 = HotPotato.hotPotatoPlayer;
                        HotPotato.explodedPotatoTeam.Add(HotPotato.explodedPotato08);
                    }
                    else if (HotPotato.explodedPotato09 == null)
                    {
                        HotPotato.explodedPotato09 = HotPotato.hotPotatoPlayer;
                        HotPotato.explodedPotatoTeam.Add(HotPotato.explodedPotato09);
                    }
                    else if (HotPotato.explodedPotato10 == null)
                    {
                        HotPotato.explodedPotato10 = HotPotato.hotPotatoPlayer;
                        HotPotato.explodedPotatoTeam.Add(HotPotato.explodedPotato10);
                    }
                    else if (HotPotato.explodedPotato11 == null)
                    {
                        HotPotato.explodedPotato11 = HotPotato.hotPotatoPlayer;
                        HotPotato.explodedPotatoTeam.Add(HotPotato.explodedPotato11);
                    }
                    else if (HotPotato.explodedPotato12 == null)
                    {
                        HotPotato.explodedPotato12 = HotPotato.hotPotatoPlayer;
                        HotPotato.explodedPotatoTeam.Add(HotPotato.explodedPotato12);
                    }
                    else if (HotPotato.explodedPotato13 == null)
                    {
                        HotPotato.explodedPotato13 = HotPotato.hotPotatoPlayer;
                        HotPotato.explodedPotatoTeam.Add(HotPotato.explodedPotato13);
                    }
                    else if (HotPotato.explodedPotato14 == null)
                    {
                        HotPotato.explodedPotato14 = HotPotato.hotPotatoPlayer;
                        HotPotato.explodedPotatoTeam.Add(HotPotato.explodedPotato14);
                    }

                    int notPotatosAlives = -1;
                    HotPotato.notPotatoTeamAlive.Clear();
                    foreach (PlayerControl notPotato in HotPotato.notPotatoTeam)
                    {
                        if (!notPotato.Data.IsDead)
                        {
                            notPotatosAlives += 1;
                            HotPotato.notPotatoTeamAlive.Add(notPotato);
                        }
                    }

                    if (notPotatosAlives < 1)
                    {
                        HotPotato.triggerHotPotatoEnd = true;
                        GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.HotPotatoEnd, false);
                        return;
                    }

                    HotPotato.hotPotatoPlayer = HotPotato.notPotatoTeam[0];

                    // If hot potato timed out, assing new potato
                    if (HotPotato.notPotato01 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato01)
                    {
                        HotPotato.notPotato01 = null;
                    }
                    else if (HotPotato.notPotato02 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato02)
                    {
                        HotPotato.notPotato02 = null;
                    }
                    else if (HotPotato.notPotato03 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato03)
                    {
                        HotPotato.notPotato03 = null;
                    }
                    else if (HotPotato.notPotato04 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato04)
                    {
                        HotPotato.notPotato04 = null;
                    }
                    else if (HotPotato.notPotato05 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato05)
                    {
                        HotPotato.notPotato05 = null;
                    }
                    else if (HotPotato.notPotato06 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato06)
                    {
                        HotPotato.notPotato06 = null;
                    }
                    else if (HotPotato.notPotato07 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato07)
                    {
                        HotPotato.notPotato07 = null;
                    }
                    else if (HotPotato.notPotato08 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato08)
                    {
                        HotPotato.notPotato08 = null;
                    }
                    else if (HotPotato.notPotato09 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato09)
                    {
                        HotPotato.notPotato09 = null;
                    }
                    else if (HotPotato.notPotato10 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato10)
                    {
                        HotPotato.notPotato10 = null;
                    }
                    else if (HotPotato.notPotato11 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato11)
                    {
                        HotPotato.notPotato11 = null;
                    }
                    else if (HotPotato.notPotato12 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato12)
                    {
                        HotPotato.notPotato12 = null;
                    }
                    else if (HotPotato.notPotato13 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato13)
                    {
                        HotPotato.notPotato13 = null;
                    }
                    else if (HotPotato.notPotato14 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato14)
                    {
                        HotPotato.notPotato14 = null;
                    }

                    HotPotato.notPotatoTeam.RemoveAt(0);

                    HotPotato.hotPotatoPlayer.NetTransform.Halt();
                    HotPotato.hotPotatoPlayer.moveable = false;
                    HotPotato.hotPotatoPlayer.MyPhysics.SetBodyType(PlayerBodyTypes.Seeker);
                    HotPotato.hotPotato.transform.position = HotPotato.hotPotatoPlayer.transform.position + new Vector3(0, 0.5f, -0.25f);
                    HotPotato.hotPotato.transform.parent = HotPotato.hotPotatoPlayer.transform;

                    HudManager.Instance.StartCoroutine(Effects.Lerp(3, new Action<float>((p) =>
                    { // Delayed action
                        if (p == 1f)
                        {
                            HotPotato.hotPotatoPlayer.moveable = true;
                        }
                    })));

                    Helpers.showGamemodesPopUp(1, Helpers.PlayerById(HotPotato.hotPotatoPlayer.PlayerId));
                    HotPotato.hotpotatopointCounter = new StringBuilder(Tr.Get(TrKey.HotPotatoStatus)).Append("<color=#808080FF>").Append(HotPotato.hotPotatoPlayer.name).Append("</color> | ").Append(Tr.Get(TrKey.ColdPotatoes)).Append("<color=#00F7FFFF>").Append(notPotatosAlives).Append("</color>").ToString();
                }
            })));
        }
    }

    public static void hotPotatoUpdate()
    {
        if (MapSettings.GameMode is not CustomGameMode.HotPotato) return;

        // Fill the Danger Metter for hotPotato and update its distance for coldpotatoes
        if (HotPotato.hotPotatoPlayer != null && HudManager.Instance.DangerMeter.gameObject.active)
        {
            float leftdistance = 55f;
            float rightdistance = 15f;
            float currentdistance = float.MaxValue;

            float sqrMagnitude = (HotPotato.hotPotatoPlayer.transform.position - PlayerControl.LocalPlayer.transform.position).sqrMagnitude;
            if (sqrMagnitude < leftdistance && currentdistance > sqrMagnitude)
            {
                currentdistance = sqrMagnitude;
            }

            float dangerLevelLeft = Mathf.Clamp01((leftdistance - currentdistance) / (leftdistance - rightdistance));
            float dangerLevelRight = Mathf.Clamp01((rightdistance - currentdistance) / rightdistance);
            HudManager.Instance.DangerMeter.SetDangerValue(dangerLevelLeft, dangerLevelRight);
        }

        // Hide hot potato sprite if in vent
        if (HotPotato.hotPotatoPlayer != null && HotPotato.hotPotato != null)
        {
            if (HotPotato.hotPotatoPlayer.inVent)
            {
                HotPotato.hotPotato.SetActive(false);
            }
            else
            {
                HotPotato.hotPotato.SetActive(true);
            }
        }

        // If hot potato disconnects, assing new potato and reset timer
        if (HotPotato.hotPotatoPlayer != null && HotPotato.hotPotatoPlayer.Data.Disconnected)
        {

            if (!HotPotato.firstPotatoTransfered)
            {
                HotPotato.firstPotatoTransfered = true;
            }

            HotPotato.timeforTransfer = HotPotato.savedtimeforTransfer;

            int notPotatosAlives = -1;
            HotPotato.notPotatoTeamAlive.Clear();
            foreach (PlayerControl remainPotato in HotPotato.notPotatoTeam)
            {
                if (!remainPotato.Data.IsDead)
                {
                    notPotatosAlives += 1;
                    HotPotato.notPotatoTeamAlive.Add(remainPotato);
                }
            }

            if (notPotatosAlives < 1)
            {
                HotPotato.triggerHotPotatoEnd = true;
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.HotPotatoEnd, false);
            }

            HotPotato.hotPotatoPlayer = HotPotato.notPotatoTeam[0];
            HotPotato.hotPotatoPlayer.MyPhysics.SetBodyType(PlayerBodyTypes.Seeker);
            HotPotato.hotPotato.transform.position = HotPotato.hotPotatoPlayer.transform.position + new Vector3(0, 0.5f, -0.25f);
            HotPotato.hotPotato.transform.parent = HotPotato.hotPotatoPlayer.transform;

            // If hot potato timed out, assing new potato
            if (HotPotato.notPotato01 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato01)
            {
                HotPotato.notPotato01 = null;
            }
            else if (HotPotato.notPotato02 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato02)
            {
                HotPotato.notPotato02 = null;
            }
            else if (HotPotato.notPotato03 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato03)
            {
                HotPotato.notPotato03 = null;
            }
            else if (HotPotato.notPotato04 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato04)
            {
                HotPotato.notPotato04 = null;
            }
            else if (HotPotato.notPotato05 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato05)
            {
                HotPotato.notPotato05 = null;
            }
            else if (HotPotato.notPotato06 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato06)
            {
                HotPotato.notPotato06 = null;
            }
            else if (HotPotato.notPotato07 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato07)
            {
                HotPotato.notPotato07 = null;
            }
            else if (HotPotato.notPotato08 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato08)
            {
                HotPotato.notPotato08 = null;
            }
            else if (HotPotato.notPotato09 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato09)
            {
                HotPotato.notPotato09 = null;
            }
            else if (HotPotato.notPotato10 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato10)
            {
                HotPotato.notPotato10 = null;
            }
            else if (HotPotato.notPotato11 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato11)
            {
                HotPotato.notPotato11 = null;
            }
            else if (HotPotato.notPotato12 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato12)
            {
                HotPotato.notPotato12 = null;
            }
            else if (HotPotato.notPotato13 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato13)
            {
                HotPotato.notPotato13 = null;
            }
            else if (HotPotato.notPotato14 != null && HotPotato.notPotatoTeam[0] == HotPotato.notPotato14)
            {
                HotPotato.notPotato14 = null;
            }

            HotPotato.notPotatoTeam.RemoveAt(0);

            hotPotatoButton.Timer = HotPotato.transferCooldown;

            Helpers.showGamemodesPopUp(1, Helpers.PlayerById(HotPotato.hotPotatoPlayer.PlayerId));
            HotPotato.hotpotatopointCounter = new StringBuilder(Tr.Get(TrKey.HotPotatoStatus)).Append("<color=#808080FF>").Append(HotPotato.hotPotatoPlayer.name).Append("</color> | ").Append(Tr.Get(TrKey.ColdPotatoes)).Append("<color=#00F7FFFF>").Append(notPotatosAlives).Append("</color>").ToString();
        }

        // If notpotato disconnects, check number of notpotatos
        foreach (PlayerControl notPotato in HotPotato.notPotatoTeam)
        {
            if (notPotato.Data.Disconnected)
            {

                int notPotatosAlives = -1;
                HotPotato.notPotatoTeamAlive.Clear();
                foreach (PlayerControl remainPotato in HotPotato.notPotatoTeam)
                {
                    if (!remainPotato.Data.IsDead)
                    {
                        notPotatosAlives += 1;
                        HotPotato.notPotatoTeamAlive.Add(remainPotato);
                    }
                }

                if (notPotatosAlives < 1)
                {
                    HotPotato.triggerHotPotatoEnd = true;
                    GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.HotPotatoEnd, false);
                }

                if (HotPotato.notPotato01 != null && notPotato.PlayerId == HotPotato.notPotato01.PlayerId)
                {
                    HotPotato.notPotatoTeam.Remove(HotPotato.notPotato01);
                }
                else if (HotPotato.notPotato02 != null && notPotato.PlayerId == HotPotato.notPotato02.PlayerId)
                {
                    HotPotato.notPotatoTeam.Remove(HotPotato.notPotato02);
                }
                else if (HotPotato.notPotato03 != null && notPotato.PlayerId == HotPotato.notPotato03.PlayerId)
                {
                    HotPotato.notPotatoTeam.Remove(HotPotato.notPotato03);
                }
                else if (HotPotato.notPotato04 != null && notPotato.PlayerId == HotPotato.notPotato04.PlayerId)
                {
                    HotPotato.notPotatoTeam.Remove(HotPotato.notPotato04);
                }
                else if (HotPotato.notPotato05 != null && notPotato.PlayerId == HotPotato.notPotato05.PlayerId)
                {
                    HotPotato.notPotatoTeam.Remove(HotPotato.notPotato05);
                }
                else if (HotPotato.notPotato06 != null && notPotato.PlayerId == HotPotato.notPotato06.PlayerId)
                {
                    HotPotato.notPotatoTeam.Remove(HotPotato.notPotato06);
                }
                else if (HotPotato.notPotato07 != null && notPotato.PlayerId == HotPotato.notPotato07.PlayerId)
                {
                    HotPotato.notPotatoTeam.Remove(HotPotato.notPotato07);
                }
                else if (HotPotato.notPotato08 != null && notPotato.PlayerId == HotPotato.notPotato08.PlayerId)
                {
                    HotPotato.notPotatoTeam.Remove(HotPotato.notPotato08);
                }
                else if (HotPotato.notPotato09 != null && notPotato.PlayerId == HotPotato.notPotato09.PlayerId)
                {
                    HotPotato.notPotatoTeam.Remove(HotPotato.notPotato09);
                }
                else if (HotPotato.notPotato10 != null && notPotato.PlayerId == HotPotato.notPotato10.PlayerId)
                {
                    HotPotato.notPotatoTeam.Remove(HotPotato.notPotato10);
                }
                else if (HotPotato.notPotato11 != null && notPotato.PlayerId == HotPotato.notPotato11.PlayerId)
                {
                    HotPotato.notPotatoTeam.Remove(HotPotato.notPotato11);
                }
                else if (HotPotato.notPotato12 != null && notPotato.PlayerId == HotPotato.notPotato12.PlayerId)
                {
                    HotPotato.notPotatoTeam.Remove(HotPotato.notPotato12);
                }
                else if (HotPotato.notPotato13 != null && notPotato.PlayerId == HotPotato.notPotato13.PlayerId)
                {
                    HotPotato.notPotatoTeam.Remove(HotPotato.notPotato13);
                }
                else if (HotPotato.notPotato14 != null && notPotato.PlayerId == HotPotato.notPotato14.PlayerId)
                {
                    HotPotato.notPotatoTeam.Remove(HotPotato.notPotato14);
                }

                HotPotato.hotpotatopointCounter = new StringBuilder(Tr.Get(TrKey.HotPotatoStatus)).Append("<color=#808080FF>").Append(HotPotato.hotPotatoPlayer.name).Append("</color> | ").Append(Tr.Get(TrKey.ColdPotatoes)).Append("<color=#00F7FFFF>").Append(notPotatosAlives).Append("</color>").ToString();
                break;
            }
        }
    }
}