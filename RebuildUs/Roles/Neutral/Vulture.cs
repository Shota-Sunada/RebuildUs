namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
[RegisterRole(RoleType.Vulture, RoleTeam.Neutral, typeof(SingleRoleBase<Vulture>), nameof(Vulture.NameColor), nameof(CustomOptionHolder.VultureSpawnRate))]
internal class Vulture : SingleRoleBase<Vulture>
{
    internal static Color NameColor = new Color32(139, 69, 19, byte.MaxValue);

    internal static bool TriggerVultureWin;

    private static CustomButton _vultureEatButton;
    private static TMP_Text _vultureNumCorpsesText;
    private readonly List<Arrow> _localArrows;
    private float _timeUntilUpdate;
    internal int EatenBodies;

    public Vulture()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Vulture;
        EatenBodies = 0;
        _localArrows = [];
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    private static float Cooldown
    {
        get => CustomOptionHolder.VultureCooldown.GetFloat();
    }

    internal static int NumberToWin
    {
        get => (int)CustomOptionHolder.VultureNumberToWin.GetFloat();
    }

    internal static bool CanUseVents
    {
        get => CustomOptionHolder.VultureCanUseVents.GetBool();
    }

    private static bool ShowArrows
    {
        get => CustomOptionHolder.VultureShowArrows.GetBool();
    }

    internal override void OnMeetingStart() { }
    internal override void OnMeetingEnd() { }
    internal override void OnIntroEnd() { }

    internal override void FixedUpdate()
    {
        if (_localArrows == null || !ShowArrows)
        {
            return;
        }

        Vulture local = Local;
        if (local == null)
        {
            return;
        }
        if (Player.IsDead())
        {
            foreach (Arrow t in _localArrows)
            {
                UnityObject.Destroy(t.ArrowObject);
            }

            _localArrows.Clear();
            return;
        }

        _timeUntilUpdate -= Time.fixedDeltaTime;
        if (_timeUntilUpdate > 0f)
        {
            return;
        }
        _timeUntilUpdate = 0.25f;

        DeadBody[] deadBodies = UnityObject.FindObjectsOfType<DeadBody>();
        bool arrowUpdate = _localArrows.Count != deadBodies.Length;

        if (arrowUpdate)
        {
            foreach (Arrow t in _localArrows)
            {
                UnityObject.Destroy(t.ArrowObject);
            }

            _localArrows.Clear();
        }

        for (int i = 0; i < deadBodies.Length; i++)
        {
            DeadBody db = deadBodies[i];
            if (arrowUpdate)
            {
                _localArrows.Add(new(Color.blue));
                _localArrows[i].ArrowObject.SetActive(true);
            }

            _localArrows[i]?.Update(db.transform.position);
        }
    }

    internal override void OnKill(PlayerControl target) { }
    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    internal static void MakeButtons(HudManager hm)
    {
        _vultureEatButton = new(() =>
            {
                Il2CppArrayBase<DeadBody> bodies = UnityObject.FindObjectsOfType<DeadBody>();
                PlayerControl local = PlayerControl.LocalPlayer;
                Vector2 truePosition = local.GetTruePosition();
                float maxDist = local.MaxReportDistance;

                foreach (DeadBody body in bodies)
                {
                    if (body == null || body.Reported)
                    {
                        continue;
                    }

                    Vector2 bodyPos = body.TruePosition;
                    if (!(Vector2.Distance(bodyPos, truePosition) <= maxDist)
                        || !local.CanMove
                        || PhysicsHelpers.AnythingBetween(truePosition, bodyPos, Constants.ShipAndObjectsMask, false))
                    {
                        continue;
                    }
                    NetworkedPlayerInfo playerInfo = GameData.Instance.GetPlayerById(body.ParentId);
                    if (playerInfo == null)
                    {
                        continue;
                    }
                    using RPCSender sender = new(local.NetId, CustomRPC.VultureEat);
                    sender.Write(playerInfo.PlayerId);
                    RPCProcedure.VultureEat(playerInfo.PlayerId);

                    _vultureEatButton.Timer = _vultureEatButton.MaxTimer;
                    break;
                }

                if (Local.EatenBodies < NumberToWin)
                {
                    return;
                }
                {
                    using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.VultureWin);
                    RPCProcedure.VultureWin();
                }
            },
            () => PlayerControl.LocalPlayer.IsRole(RoleType.Vulture) && PlayerControl.LocalPlayer.IsAlive(),
            () =>
            {
                _vultureNumCorpsesText?.text = string.Format(Tr.Get(TrKey.VultureCorpses), NumberToWin - Local.EatenBodies);
                return hm.ReportButton.graphic.color == Palette.EnabledColor && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                _vultureEatButton.Timer = _vultureEatButton.MaxTimer;
            },
            AssetLoader.VultureButton,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            AbilitySlot.NeutralAbilityPrimary,
            false,
            Tr.Get(TrKey.VultureText));

        _vultureNumCorpsesText = UnityObject.Instantiate(_vultureEatButton.ActionButton.cooldownTimerText,
            _vultureEatButton.ActionButton.cooldownTimerText.transform.parent);
        _vultureNumCorpsesText.text = "";
        _vultureNumCorpsesText.enableWordWrapping = false;
        _vultureNumCorpsesText.transform.localScale = Vector3.one * 0.5f;
        _vultureNumCorpsesText.transform.localPosition += new Vector3(0.0f, 0.7f, 0);
    }

    internal static void SetButtonCooldowns()
    {
        _vultureEatButton.MaxTimer = Cooldown;
    }

    internal static void Clear()
    {
        TriggerVultureWin = false;

        if (Instance is not null)
        {
            if (Instance._localArrows != null)
            {
                foreach (Arrow arrow in Instance._localArrows)
                {
                    if (arrow?.ArrowObject != null)
                    {
                        UnityObject.Destroy(arrow.ArrowObject);
                    }
                }
            }

            Instance._localArrows?.Clear();
        }

        ModRoleManager.RemoveRole(Instance);
        Instance = null;
    }
}