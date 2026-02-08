using Object = UnityEngine.Object;

namespace RebuildUs.Modules.GameMode;

internal static partial class HotPotato
{
    public static Color IntroColor = new Color32(242, 190, 255, byte.MaxValue);

    public static bool Createdhotpotato;

    public static readonly List<PlayerControl> NOT_POTATO_TEAM = [];
    public static readonly List<PlayerControl> NOT_POTATO_TEAM_ALIVE = [];
    public static readonly List<PlayerControl> EXPLODED_POTATO_TEAM = [];

    internal static PlayerControl HotPotatoPlayer;
    private static PlayerControl _hotPotatoPlayerCurrentTarget;
    internal static PlayerControl NotPotato01;
    internal static PlayerControl NotPotato02;
    internal static PlayerControl NotPotato03;
    public static PlayerControl NotPotato04;
    public static PlayerControl NotPotato05;
    public static PlayerControl NotPotato06;
    public static PlayerControl NotPotato07;
    public static PlayerControl NotPotato08;
    public static PlayerControl NotPotato09;
    public static PlayerControl NotPotato10;
    public static PlayerControl NotPotato11;
    public static PlayerControl NotPotato12;
    public static PlayerControl NotPotato13;
    public static PlayerControl NotPotato14;

    public static PlayerControl ExplodedPotato01;
    public static PlayerControl ExplodedPotato02;
    public static PlayerControl ExplodedPotato03;
    public static PlayerControl ExplodedPotato04;
    public static PlayerControl ExplodedPotato05;
    public static PlayerControl ExplodedPotato06;
    public static PlayerControl ExplodedPotato07;
    public static PlayerControl ExplodedPotato08;
    public static PlayerControl ExplodedPotato09;
    public static PlayerControl ExplodedPotato10;
    public static PlayerControl ExplodedPotato11;
    public static PlayerControl ExplodedPotato12;
    public static PlayerControl ExplodedPotato13;
    public static PlayerControl ExplodedPotato14;

    public static GameObject HotPotatoObject;

    public static float TimeforTransfer = 15;
    public static float TransferCooldown = 10f;
    public static float SavedtimeforTransfer = 15;
    public static bool ResetTimeForTransfer = true;
    public static float IncreaseTimeIfNoReset = 5f;
    public static bool FirstPotatoTransfered;

    public static bool NotPotatoTeamAlerted;

    public static bool TriggerHotPotatoEnd;

    public static string HotpotatopointCounter = new StringBuilder(Tr.Get(TrKey.HotPotatoStatus))
                                                 .Append("<color=#808080FF></color> | ")
                                                 .Append(Tr.Get(TrKey.ColdPotatoes))
                                                 .Append("<color=#00F7FFFF>")
                                                 .Append(NOT_POTATO_TEAM.Count)
                                                 .Append("</color>")
                                                 .ToString();

    public static void ClearAndReload()
    {
        Createdhotpotato = false;

        NOT_POTATO_TEAM.Clear();
        NOT_POTATO_TEAM_ALIVE.Clear();
        HotPotatoPlayer = null;
        _hotPotatoPlayerCurrentTarget = null;
        NotPotato01 = null;
        NotPotato02 = null;
        NotPotato03 = null;
        NotPotato04 = null;
        NotPotato05 = null;
        NotPotato06 = null;
        NotPotato07 = null;
        NotPotato08 = null;
        NotPotato09 = null;
        NotPotato10 = null;
        NotPotato11 = null;
        NotPotato12 = null;
        NotPotato13 = null;
        NotPotato14 = null;

        ExplodedPotato01 = null;
        ExplodedPotato02 = null;
        ExplodedPotato03 = null;
        ExplodedPotato04 = null;
        ExplodedPotato05 = null;
        ExplodedPotato06 = null;
        ExplodedPotato07 = null;
        ExplodedPotato08 = null;
        ExplodedPotato09 = null;
        ExplodedPotato10 = null;
        ExplodedPotato11 = null;
        ExplodedPotato12 = null;
        ExplodedPotato13 = null;
        ExplodedPotato14 = null;

        TimeforTransfer = CustomOptionHolder.HotPotatoTransferLimit.GetFloat() + 10f;
        TransferCooldown = CustomOptionHolder.HotPotatoCooldown.GetFloat();
        ResetTimeForTransfer = CustomOptionHolder.HotPotatoResetTimeForTransfer.GetBool();
        IncreaseTimeIfNoReset = CustomOptionHolder.HotPotatoIncreaseTimeIfNoReset.GetFloat();
        NotPotatoTeamAlerted = false;
        TriggerHotPotatoEnd = false;
        SavedtimeforTransfer = TimeforTransfer - 10f;
        FirstPotatoTransfered = false;
        HotPotatoObject = null;

        HotpotatopointCounter = new StringBuilder(Tr.Get(TrKey.HotPotatoStatus)).Append("<color=#00F7FFFF></color> | ")
            .Append(Tr.Get(TrKey.ColdPotatoes))
            .Append("<color=#928B55FF>")
            .Append(NOT_POTATO_TEAM.Count)
            .Append("</color>")
            .ToString();
    }

    public static void CreateHp()
    {
        var hotPotatoPlayerPos = new Vector3();
        var notPotatoTeamPos = new Vector3();

        switch (GameOptionsManager.Instance.currentGameOptions.MapId)
        {
            // Skeld / Custom Skeld
            case 0:
                if (RebuildUs.ActivatedSensei)
                {
                    hotPotatoPlayerPos = new(-6.5f, -2.25f, PlayerControl.LocalPlayer.transform.position.z);
                    notPotatoTeamPos = new(12.5f, -0.25f, PlayerControl.LocalPlayer.transform.position.z);
                }
                else if (RebuildUs.ActivatedDleks)
                {
                    hotPotatoPlayerPos = new(0.75f, -7f, PlayerControl.LocalPlayer.transform.position.z);
                    notPotatoTeamPos = new(-6.25f, -3.5f, PlayerControl.LocalPlayer.transform.position.z);
                }
                else
                {
                    hotPotatoPlayerPos = new(-0.75f, -7f, PlayerControl.LocalPlayer.transform.position.z);
                    notPotatoTeamPos = new(6.25f, -3.5f, PlayerControl.LocalPlayer.transform.position.z);
                }

                break;
            // Mira HQ
            case 1:
                hotPotatoPlayerPos = new(6.15f, 6.25f, PlayerControl.LocalPlayer.transform.position.z);
                notPotatoTeamPos = new(17.75f, 11.5f, PlayerControl.LocalPlayer.transform.position.z);
                break;
            // Polus
            case 2:
                hotPotatoPlayerPos = new(20.5f, -11.75f, PlayerControl.LocalPlayer.transform.position.z);
                notPotatoTeamPos = new(12.25f, -16f, PlayerControl.LocalPlayer.transform.position.z);
                break;
            // Dleks
            case 3:
                hotPotatoPlayerPos = new(0.75f, -7f, PlayerControl.LocalPlayer.transform.position.z);
                notPotatoTeamPos = new(-6.25f, -3.5f, PlayerControl.LocalPlayer.transform.position.z);
                break;
            // Airship
            case 4:
                hotPotatoPlayerPos = new(12.25f, 2f, PlayerControl.LocalPlayer.transform.position.z);
                notPotatoTeamPos = new(6.25f, 2.5f, PlayerControl.LocalPlayer.transform.position.z);
                break;
            // Fungle
            case 5:
                hotPotatoPlayerPos = new(-10.75f, 12.75f, PlayerControl.LocalPlayer.transform.position.z);
                notPotatoTeamPos = new(-3.25f, -10.5f, PlayerControl.LocalPlayer.transform.position.z);
                break;
            // Submerged
            case 6:
                hotPotatoPlayerPos = new(-4.25f, -33.5f, PlayerControl.LocalPlayer.transform.position.z);
                notPotatoTeamPos = new(13f, -25.25f, PlayerControl.LocalPlayer.transform.position.z);
                break;
        }

        HotPotatoPlayer.MyPhysics.SetBodyType(PlayerBodyTypes.Seeker);
        HotPotatoPlayer.transform.position = hotPotatoPlayerPos;

        foreach (var player in NOT_POTATO_TEAM) player.transform.position = notPotatoTeamPos;

        if (PlayerControl.LocalPlayer == null || Createdhotpotato) return;
        PlayerControl.LocalPlayer.ClearAllTasks();

        var hotpotato = Object.Instantiate(AssetLoader.HotPotato, HotPotatoPlayer.transform);
        hotpotato.name = "HotPotato";
        hotpotato.transform.position = HotPotatoPlayer.transform.position + new Vector3(0, 0.5f, -0.25f);
        HotPotatoObject = hotpotato;

        HudManager.Instance.DangerMeter.gameObject.SetActive(true);

        Createdhotpotato = true;
    }

    public static void HotPotatoSetTarget()
    {
        if (MapSettings.GameMode is not CustomGameMode.HotPotato) return;

        if (HotPotatoPlayer != null && HotPotatoPlayer == PlayerControl.LocalPlayer)
        {
            _hotPotatoPlayerCurrentTarget = Helpers.SetTarget();
            Helpers.SetPlayerOutline(_hotPotatoPlayerCurrentTarget, Color.grey);
        }
    }

    public static void OnMurderPlayerPostfix(PlayerControl __instance, PlayerControl target)
    {
        if (HotPotatoPlayer == null || HotPotatoPlayer.PlayerId != target.PlayerId) return;
        var hpBody = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == target.PlayerId);
        hpBody.transform.position = new(50, 50, 1);

        TimeforTransfer = SavedtimeforTransfer + 4f;

        HudManager.Instance.StartCoroutine(Effects.Lerp(1, new Action<float>(p =>
        {
            // Delayed action
            if (!Mathf.Approximately(p, 1f)) return;

            if (ExplodedPotato01 == null)
            {
                ExplodedPotato01 = HotPotatoPlayer;
                EXPLODED_POTATO_TEAM.Add(ExplodedPotato01);
            }
            else if (ExplodedPotato02 == null)
            {
                ExplodedPotato02 = HotPotatoPlayer;
                EXPLODED_POTATO_TEAM.Add(ExplodedPotato02);
            }
            else if (ExplodedPotato03 == null)
            {
                ExplodedPotato03 = HotPotatoPlayer;
                EXPLODED_POTATO_TEAM.Add(ExplodedPotato03);
            }
            else if (ExplodedPotato04 == null)
            {
                ExplodedPotato04 = HotPotatoPlayer;
                EXPLODED_POTATO_TEAM.Add(ExplodedPotato04);
            }
            else if (ExplodedPotato05 == null)
            {
                ExplodedPotato05 = HotPotatoPlayer;
                EXPLODED_POTATO_TEAM.Add(ExplodedPotato05);
            }
            else if (ExplodedPotato06 == null)
            {
                ExplodedPotato06 = HotPotatoPlayer;
                EXPLODED_POTATO_TEAM.Add(ExplodedPotato06);
            }
            else if (ExplodedPotato07 == null)
            {
                ExplodedPotato07 = HotPotatoPlayer;
                EXPLODED_POTATO_TEAM.Add(ExplodedPotato07);
            }
            else if (ExplodedPotato08 == null)
            {
                ExplodedPotato08 = HotPotatoPlayer;
                EXPLODED_POTATO_TEAM.Add(ExplodedPotato08);
            }
            else if (ExplodedPotato09 == null)
            {
                ExplodedPotato09 = HotPotatoPlayer;
                EXPLODED_POTATO_TEAM.Add(ExplodedPotato09);
            }
            else if (ExplodedPotato10 == null)
            {
                ExplodedPotato10 = HotPotatoPlayer;
                EXPLODED_POTATO_TEAM.Add(ExplodedPotato10);
            }
            else if (ExplodedPotato11 == null)
            {
                ExplodedPotato11 = HotPotatoPlayer;
                EXPLODED_POTATO_TEAM.Add(ExplodedPotato11);
            }
            else if (ExplodedPotato12 == null)
            {
                ExplodedPotato12 = HotPotatoPlayer;
                EXPLODED_POTATO_TEAM.Add(ExplodedPotato12);
            }
            else if (ExplodedPotato13 == null)
            {
                ExplodedPotato13 = HotPotatoPlayer;
                EXPLODED_POTATO_TEAM.Add(ExplodedPotato13);
            }
            else if (ExplodedPotato14 == null)
            {
                ExplodedPotato14 = HotPotatoPlayer;
                EXPLODED_POTATO_TEAM.Add(ExplodedPotato14);
            }

            var notPotatosAlives = -1;
            NOT_POTATO_TEAM_ALIVE.Clear();
            foreach (var notPotato in NOT_POTATO_TEAM)
            {
                if (!notPotato.Data.IsDead)
                {
                    notPotatosAlives += 1;
                    NOT_POTATO_TEAM_ALIVE.Add(notPotato);
                }
            }

            if (notPotatosAlives < 1)
            {
                TriggerHotPotatoEnd = true;
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.HotPotatoEnd, false);
                return;
            }

            HotPotatoPlayer = NOT_POTATO_TEAM[0];

            // If hot potato timed out, assing new potato
            if (NotPotato01 != null && NOT_POTATO_TEAM[0] == NotPotato01)
                NotPotato01 = null;
            else if (NotPotato02 != null && NOT_POTATO_TEAM[0] == NotPotato02)
                NotPotato02 = null;
            else if (NotPotato03 != null && NOT_POTATO_TEAM[0] == NotPotato03)
                NotPotato03 = null;
            else if (NotPotato04 != null && NOT_POTATO_TEAM[0] == NotPotato04)
                NotPotato04 = null;
            else if (NotPotato05 != null && NOT_POTATO_TEAM[0] == NotPotato05)
                NotPotato05 = null;
            else if (NotPotato06 != null && NOT_POTATO_TEAM[0] == NotPotato06)
                NotPotato06 = null;
            else if (NotPotato07 != null && NOT_POTATO_TEAM[0] == NotPotato07)
                NotPotato07 = null;
            else if (NotPotato08 != null && NOT_POTATO_TEAM[0] == NotPotato08)
                NotPotato08 = null;
            else if (NotPotato09 != null && NOT_POTATO_TEAM[0] == NotPotato09)
                NotPotato09 = null;
            else if (NotPotato10 != null && NOT_POTATO_TEAM[0] == NotPotato10)
                NotPotato10 = null;
            else if (NotPotato11 != null && NOT_POTATO_TEAM[0] == NotPotato11)
                NotPotato11 = null;
            else if (NotPotato12 != null && NOT_POTATO_TEAM[0] == NotPotato12)
                NotPotato12 = null;
            else if (NotPotato13 != null && NOT_POTATO_TEAM[0] == NotPotato13)
                NotPotato13 = null;
            else if (NotPotato14 != null && NOT_POTATO_TEAM[0] == NotPotato14) NotPotato14 = null;

            NOT_POTATO_TEAM.RemoveAt(0);

            HotPotatoPlayer.NetTransform.Halt();
            HotPotatoPlayer.moveable = false;
            HotPotatoPlayer.MyPhysics.SetBodyType(PlayerBodyTypes.Seeker);
            HotPotatoObject.transform.position = HotPotatoPlayer.transform.position + new Vector3(0, 0.5f, -0.25f);
            HotPotatoObject.transform.parent = HotPotatoPlayer.transform;

            HudManager.Instance.StartCoroutine(Effects.Lerp(3, new Action<float>(p =>
            {
                // Delayed action
                if (p == 1f) HotPotatoPlayer.moveable = true;
            })));

            Helpers.ShowGamemodesPopUp(1, Helpers.PlayerById(HotPotatoPlayer.PlayerId));
            HotpotatopointCounter = new StringBuilder(Tr.Get(TrKey.HotPotatoStatus)).Append("<color=#808080FF>")
                .Append(HotPotatoPlayer.name)
                .Append("</color> | ")
                .Append(Tr.Get(TrKey.ColdPotatoes))
                .Append("<color=#00F7FFFF>")
                .Append(notPotatosAlives)
                .Append("</color>")
                .ToString();
        })));
    }

    public static void HotPotatoUpdate()
    {
        if (MapSettings.GameMode is not CustomGameMode.HotPotato) return;

        // Fill the Danger Metter for hotPotato and update its distance for coldpotatoes
        if (HotPotatoPlayer != null && HudManager.Instance.DangerMeter.gameObject.active)
        {
            var leftdistance = 55f;
            var rightdistance = 15f;
            var currentdistance = float.MaxValue;

            var sqrMagnitude = (HotPotatoPlayer.transform.position - PlayerControl.LocalPlayer.transform.position)
                .sqrMagnitude;
            if (sqrMagnitude < leftdistance && currentdistance > sqrMagnitude) currentdistance = sqrMagnitude;

            var dangerLevelLeft = Mathf.Clamp01((leftdistance - currentdistance) / (leftdistance - rightdistance));
            var dangerLevelRight = Mathf.Clamp01((rightdistance - currentdistance) / rightdistance);
            HudManager.Instance.DangerMeter.SetDangerValue(dangerLevelLeft, dangerLevelRight);
        }

        // Hide hot potato sprite if in vent
        if (HotPotatoPlayer != null && HotPotatoObject != null)
        {
            if (HotPotatoPlayer.inVent)
                HotPotatoObject.SetActive(false);
            else
                HotPotatoObject.SetActive(true);
        }

        // If hot potato disconnects, assing new potato and reset timer
        if (HotPotatoPlayer != null && HotPotatoPlayer.Data.Disconnected)
        {
            if (!FirstPotatoTransfered) FirstPotatoTransfered = true;

            TimeforTransfer = SavedtimeforTransfer;

            var notPotatosAlives = -1;
            NOT_POTATO_TEAM_ALIVE.Clear();
            foreach (var remainPotato in NOT_POTATO_TEAM)
            {
                if (!remainPotato.Data.IsDead)
                {
                    notPotatosAlives += 1;
                    NOT_POTATO_TEAM_ALIVE.Add(remainPotato);
                }
            }

            if (notPotatosAlives < 1)
            {
                TriggerHotPotatoEnd = true;
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.HotPotatoEnd, false);
            }

            HotPotatoPlayer = NOT_POTATO_TEAM[0];
            HotPotatoPlayer.MyPhysics.SetBodyType(PlayerBodyTypes.Seeker);
            HotPotatoObject.transform.position = HotPotatoPlayer.transform.position + new Vector3(0, 0.5f, -0.25f);
            HotPotatoObject.transform.parent = HotPotatoPlayer.transform;

            // If hot potato timed out, assing new potato
            if (NotPotato01 != null && NOT_POTATO_TEAM[0] == NotPotato01)
                NotPotato01 = null;
            else if (NotPotato02 != null && NOT_POTATO_TEAM[0] == NotPotato02)
                NotPotato02 = null;
            else if (NotPotato03 != null && NOT_POTATO_TEAM[0] == NotPotato03)
                NotPotato03 = null;
            else if (NotPotato04 != null && NOT_POTATO_TEAM[0] == NotPotato04)
                NotPotato04 = null;
            else if (NotPotato05 != null && NOT_POTATO_TEAM[0] == NotPotato05)
                NotPotato05 = null;
            else if (NotPotato06 != null && NOT_POTATO_TEAM[0] == NotPotato06)
                NotPotato06 = null;
            else if (NotPotato07 != null && NOT_POTATO_TEAM[0] == NotPotato07)
                NotPotato07 = null;
            else if (NotPotato08 != null && NOT_POTATO_TEAM[0] == NotPotato08)
                NotPotato08 = null;
            else if (NotPotato09 != null && NOT_POTATO_TEAM[0] == NotPotato09)
                NotPotato09 = null;
            else if (NotPotato10 != null && NOT_POTATO_TEAM[0] == NotPotato10)
                NotPotato10 = null;
            else if (NotPotato11 != null && NOT_POTATO_TEAM[0] == NotPotato11)
                NotPotato11 = null;
            else if (NotPotato12 != null && NOT_POTATO_TEAM[0] == NotPotato12)
                NotPotato12 = null;
            else if (NotPotato13 != null && NOT_POTATO_TEAM[0] == NotPotato13)
                NotPotato13 = null;
            else if (NotPotato14 != null && NOT_POTATO_TEAM[0] == NotPotato14) NotPotato14 = null;

            NOT_POTATO_TEAM.RemoveAt(0);

            HotPotatoButton.Timer = TransferCooldown;

            Helpers.ShowGamemodesPopUp(1, Helpers.PlayerById(HotPotatoPlayer.PlayerId));
            HotpotatopointCounter = new StringBuilder(Tr.Get(TrKey.HotPotatoStatus)).Append("<color=#808080FF>")
                .Append(HotPotatoPlayer.name)
                .Append("</color> | ")
                .Append(Tr.Get(TrKey.ColdPotatoes))
                .Append("<color=#00F7FFFF>")
                .Append(notPotatosAlives)
                .Append("</color>")
                .ToString();
        }

        // If notpotato disconnects, check number of notpotatos
        foreach (var notPotato in NOT_POTATO_TEAM)
        {
            if (notPotato.Data.Disconnected)
            {
                var notPotatosAlives = -1;
                NOT_POTATO_TEAM_ALIVE.Clear();
                foreach (var remainPotato in NOT_POTATO_TEAM)
                {
                    if (!remainPotato.Data.IsDead)
                    {
                        notPotatosAlives += 1;
                        NOT_POTATO_TEAM_ALIVE.Add(remainPotato);
                    }
                }

                if (notPotatosAlives < 1)
                {
                    TriggerHotPotatoEnd = true;
                    GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.HotPotatoEnd, false);
                }

                if (NotPotato01 != null && notPotato.PlayerId == NotPotato01.PlayerId)
                    NOT_POTATO_TEAM.Remove(NotPotato01);
                else if (NotPotato02 != null && notPotato.PlayerId == NotPotato02.PlayerId)
                    NOT_POTATO_TEAM.Remove(NotPotato02);
                else if (NotPotato03 != null && notPotato.PlayerId == NotPotato03.PlayerId)
                    NOT_POTATO_TEAM.Remove(NotPotato03);
                else if (NotPotato04 != null && notPotato.PlayerId == NotPotato04.PlayerId)
                    NOT_POTATO_TEAM.Remove(NotPotato04);
                else if (NotPotato05 != null && notPotato.PlayerId == NotPotato05.PlayerId)
                    NOT_POTATO_TEAM.Remove(NotPotato05);
                else if (NotPotato06 != null && notPotato.PlayerId == NotPotato06.PlayerId)
                    NOT_POTATO_TEAM.Remove(NotPotato06);
                else if (NotPotato07 != null && notPotato.PlayerId == NotPotato07.PlayerId)
                    NOT_POTATO_TEAM.Remove(NotPotato07);
                else if (NotPotato08 != null && notPotato.PlayerId == NotPotato08.PlayerId)
                    NOT_POTATO_TEAM.Remove(NotPotato08);
                else if (NotPotato09 != null && notPotato.PlayerId == NotPotato09.PlayerId)
                    NOT_POTATO_TEAM.Remove(NotPotato09);
                else if (NotPotato10 != null && notPotato.PlayerId == NotPotato10.PlayerId)
                    NOT_POTATO_TEAM.Remove(NotPotato10);
                else if (NotPotato11 != null && notPotato.PlayerId == NotPotato11.PlayerId)
                    NOT_POTATO_TEAM.Remove(NotPotato11);
                else if (NotPotato12 != null && notPotato.PlayerId == NotPotato12.PlayerId)
                    NOT_POTATO_TEAM.Remove(NotPotato12);
                else if (NotPotato13 != null && notPotato.PlayerId == NotPotato13.PlayerId)
                    NOT_POTATO_TEAM.Remove(NotPotato13);
                else if (NotPotato14 != null && notPotato.PlayerId == NotPotato14.PlayerId)
                    NOT_POTATO_TEAM.Remove(NotPotato14);

                HotpotatopointCounter = new StringBuilder(Tr.Get(TrKey.HotPotatoStatus)).Append("<color=#808080FF>")
                    .Append(HotPotatoPlayer.name)
                    .Append("</color> | ")
                    .Append(Tr.Get(TrKey.ColdPotatoes))
                    .Append("<color=#00F7FFFF>")
                    .Append(notPotatosAlives)
                    .Append("</color>")
                    .ToString();
                break;
            }
        }
    }
}
