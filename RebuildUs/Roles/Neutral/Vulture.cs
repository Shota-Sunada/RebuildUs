namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
[RegisterRole(RoleType.Vulture, RoleTeam.Neutral, typeof(SingleRoleBase<Vulture>), nameof(CustomOptionHolder.VultureSpawnRate))]
internal class Vulture : SingleRoleBase<Vulture>
{
    public static Color Color = new Color32(139, 69, 19, byte.MaxValue);

    internal static bool TriggerVultureWin;

    private static CustomButton VultureEatButton;
    private static TMP_Text _vultureNumCorpsesText;
    private readonly List<Arrow> _localArrows;
    private float _timeUntilUpdate;
    internal int EatenBodies;

    public Vulture()
    {
        StaticRoleType = CurrentRoleType = RoleType.Vulture;
        EatenBodies = 0;
        _localArrows = [];
    }

    private static float Cooldown { get => CustomOptionHolder.VultureCooldown.GetFloat(); }
    internal static int NumberToWin { get => (int)CustomOptionHolder.VultureNumberToWin.GetFloat(); }
    internal static bool CanUseVents { get => CustomOptionHolder.VultureCanUseVents.GetBool(); }
    private static bool ShowArrows { get => CustomOptionHolder.VultureShowArrows.GetBool(); }

    [CustomEvent(CustomEventType.FixedUpdate)]
    internal void FixedUpdate()
    {
        if (_localArrows == null || !ShowArrows)
        {
            return;
        }

        if (Local == null)
        {
            return;
        }
        if (Player.IsDead())
        {
            foreach (var t in _localArrows)
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
        var arrowUpdate = _localArrows.Count != deadBodies.Length;

        if (arrowUpdate)
        {
            foreach (var t in _localArrows)
            {
                UnityObject.Destroy(t.ArrowObject);
            }

            _localArrows.Clear();
        }

        for (var i = 0; i < deadBodies.Length; i++)
        {
            var db = deadBodies[i];
            if (arrowUpdate)
            {
                _localArrows.Add(new(Color.blue));
                _localArrows[i].ArrowObject.SetActive(true);
            }

            _localArrows[i]?.Update(db.transform.position);
        }
    }

    [RegisterCustomButton]
    internal static void MakeButtons(HudManager hm)
    {
        VultureEatButton = new(
            nameof(VultureEatButton),
            () =>
            {
                var bodies = UnityObject.FindObjectsOfType<DeadBody>();
                var local = PlayerControl.LocalPlayer;
                var truePosition = local.GetTruePosition();
                var maxDist = local.MaxReportDistance;

                foreach (var body in bodies)
                {
                    if (body == null || body.Reported)
                    {
                        continue;
                    }

                    var bodyPos = body.TruePosition;
                    if (!(Vector2.Distance(bodyPos, truePosition) <= maxDist)
                        || !local.CanMove
                        || PhysicsHelpers.AnythingBetween(truePosition, bodyPos, Constants.ShipAndObjectsMask, false))
                    {
                        continue;
                    }
                    var playerInfo = GameData.Instance.GetPlayerById(body.ParentId);
                    if (playerInfo == null)
                    {
                        continue;
                    }
                    using RPCSender sender = new(local.NetId, CustomRPC.VultureEat);
                    sender.Write(playerInfo.PlayerId);
                    RPCProcedure.VultureEat(playerInfo.PlayerId);

                    VultureEatButton.Timer = VultureEatButton.MaxTimer;
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
                VultureEatButton.Timer = VultureEatButton.MaxTimer;
            },
            AssetLoader.VultureButton,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            AbilitySlot.NeutralAbilityPrimary,
            false,
            Tr.Get(TrKey.VultureText));

        _vultureNumCorpsesText = UnityObject.Instantiate(VultureEatButton.ActionButton.cooldownTimerText, VultureEatButton.ActionButton.cooldownTimerText.transform.parent);
        _vultureNumCorpsesText.text = "";
        _vultureNumCorpsesText.enableWordWrapping = false;
        _vultureNumCorpsesText.transform.localScale = Vector3.one * 0.5f;
        _vultureNumCorpsesText.transform.localPosition += new Vector3(0.0f, 0.7f, 0);
    }

    [SetCustomButtonTimer]
    internal static void SetButtonCooldowns()
    {
        VultureEatButton.MaxTimer = Cooldown;
    }

    internal static void Clear()
    {
        TriggerVultureWin = false;

        if (Instance is not null)
        {
            if (Instance._localArrows != null)
            {
                foreach (var arrow in Instance._localArrows)
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