namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
[RegisterRole(RoleType.Arsonist, RoleTeam.Neutral, typeof(SingleRoleBase<Arsonist>), nameof(Arsonist.NameColor), nameof(CustomOptionHolder.ArsonistSpawnRate))]
internal class Arsonist : SingleRoleBase<Arsonist>
{
    internal static Color NameColor = new Color32(238, 112, 46, byte.MaxValue);

    internal static bool TriggerArsonistWin;
    private static CustomButton _arsonistButton;
    private static CustomButton _arsonistIgniteButton;
    private readonly List<PlayerControl> _untargetablesCache = [];
    internal readonly List<PlayerControl> DousedPlayers = [];
    private PlayerControl _currentTarget;
    private bool _dousedEveryone;
    private PlayerControl _douseTarget;

    public Arsonist()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Arsonist;
        _dousedEveryone = false;
        _currentTarget = null;
        _douseTarget = null;
        DousedPlayers = [];
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    private static float Cooldown
    {
        get => CustomOptionHolder.ArsonistCooldown.GetFloat();
    }

    private static float Duration
    {
        get => CustomOptionHolder.ArsonistDuration.GetFloat();
    }

    internal static bool CanBeLovers
    {
        get => CustomOptionHolder.ArsonistCanBeLovers.GetBool();
    }

    private bool DousedEveryoneAlive()
    {
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p.IsRole(RoleType.Arsonist) || p.Data.IsDead || p.Data.Disconnected || p.IsGm())
            {
                continue;
            }

            var isDoused = false;
            foreach (var t in DousedPlayers)
            {
                if (t.PlayerId != p.PlayerId)
                {
                    continue;
                }
                isDoused = true;
                break;
            }

            if (!isDoused)
            {
                return false;
            }
        }

        return true;
    }

    internal override void OnMeetingStart() { }

    internal override void OnMeetingEnd()
    {
        UpdateIcons();
    }

    internal override void OnIntroEnd()
    {
        UpdateIcons();
    }

    internal override void FixedUpdate()
    {
        var local = Local;
        if (local == null)
        {
            return;
        }
        List<PlayerControl> untargetables;
        if (_douseTarget != null)
        {
            _untargetablesCache.Clear();
            foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (p.PlayerId != _douseTarget.PlayerId)
                {
                    _untargetablesCache.Add(p);
                }
            }

            untargetables = _untargetablesCache;
        }
        else
        {
            untargetables = DousedPlayers;
        }

        _currentTarget = Helpers.SetTarget(untargetablePlayers: untargetables);
        if (_currentTarget != null)
        {
            Helpers.SetPlayerOutline(_currentTarget, RoleColor);
        }
    }

    internal override void OnKill(PlayerControl target)
    {
        UpdateStatus();
    }

    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    internal static void MakeButtons(HudManager hm)
    {
        _arsonistButton = new(() =>
            {
                if (Local._currentTarget != null)
                {
                    Local._douseTarget = Local._currentTarget;
                }
            },
            () => PlayerControl.LocalPlayer.IsRole(RoleType.Arsonist) && !Local._dousedEveryone && PlayerControl.LocalPlayer.IsAlive(),
            () =>
            {
                if (!_arsonistButton.IsEffectActive || Local._douseTarget == Local._currentTarget)
                {
                    return PlayerControl.LocalPlayer.CanMove && Local._currentTarget != null;
                }
                Local._douseTarget = null;
                _arsonistButton.Timer = 0f;
                _arsonistButton.IsEffectActive = false;

                return PlayerControl.LocalPlayer.CanMove && Local._currentTarget != null;
            },
            () =>
            {
                _arsonistButton.Timer = _arsonistButton.MaxTimer;
                _arsonistButton.IsEffectActive = false;
                Local._douseTarget = null;
                Local.UpdateStatus();
            },
            AssetLoader.DouseButton,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            AbilitySlot.NeutralAbilityPrimary,
            true,
            Duration,
            () =>
            {
                if (Local._douseTarget != null)
                {
                    using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.ArsonistDouse);
                    sender.Write(Local._douseTarget.PlayerId);
                    RPCProcedure.ArsonistDouse(Local._douseTarget.PlayerId);
                }

                Local._douseTarget = null;
                Local.UpdateStatus();
                _arsonistButton.Timer = Local._dousedEveryone ? 0 : _arsonistButton.MaxTimer;

                foreach (var p in Local.DousedPlayers)
                {
                    if (MapSettings.PlayerIcons.ContainsKey(p.PlayerId))
                    {
                        MapSettings.PlayerIcons[p.PlayerId].SetSemiTransparent(false);
                    }
                }
            },
            false,
            Tr.Get(TrKey.DouseText));

        _arsonistIgniteButton = new(() =>
            {
                if (!Local._dousedEveryone)
                {
                    return;
                }
                using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.ArsonistWin);
                RPCProcedure.ArsonistWin();
            },
            () => PlayerControl.LocalPlayer.IsRole(RoleType.Arsonist) && Local._dousedEveryone && PlayerControl.LocalPlayer.IsAlive(),
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove && Local._dousedEveryone;
            },
            () => { },
            AssetLoader.IgniteButton,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            AbilitySlot.NeutralAbilityPrimary,
            false,
            Tr.Get(TrKey.IgniteText));
    }

    internal static void SetButtonCooldowns()
    {
        _arsonistButton?.MaxTimer = Cooldown;
        _arsonistIgniteButton.Timer = _arsonistIgniteButton.MaxTimer = 0f;
        Local?.UpdateStatus();
    }

    private void UpdateStatus()
    {
        _dousedEveryone = DousedEveryoneAlive();
    }

    private void UpdateIcons()
    {
        foreach (var pp in MapSettings.PlayerIcons.Values)
        {
            pp.gameObject.SetActive(false);
        }

        if (Local == null || FastDestroyableSingleton<HudManager>.Instance == null)
        {
            return;
        }
        var bottomLeft = AspectPosition.ComputePosition(AspectPosition.EdgeAlignments.LeftBottom, new(0.9f, 0.7f, -10f));
        var visibleCounter = 0;
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p.PlayerId == PlayerControl.LocalPlayer.PlayerId)
            {
                continue;
            }
            if (!MapSettings.PlayerIcons.ContainsKey(p.PlayerId))
            {
                continue;
            }

            if (p.Data.IsDead || p.Data.Disconnected)
            {
                MapSettings.PlayerIcons[p.PlayerId].gameObject.SetActive(false);
            }
            else
            {
                MapSettings.PlayerIcons[p.PlayerId].gameObject.SetActive(true);
                MapSettings.PlayerIcons[p.PlayerId].transform.localScale = Vector3.one * 0.3f;
                MapSettings.PlayerIcons[p.PlayerId].transform.localPosition = bottomLeft + Vector3.right * visibleCounter * 0.45f;
                visibleCounter++;

                var isDoused = false;
                for (var j = 0; j < DousedPlayers.Count; j++)
                {
                    if (DousedPlayers[j].PlayerId == p.PlayerId)
                    {
                        isDoused = true;
                        break;
                    }
                }

                MapSettings.PlayerIcons[p.PlayerId].SetSemiTransparent(!isDoused);
            }
        }
    }

    // write functions here

    internal static void Clear()
    {
        ModRoleManager.RemoveRole(Instance);
        Instance = null;
        TriggerArsonistWin = false;
        foreach (var p in MapSettings.PlayerIcons.Values)
        {
            if (p != null && p.gameObject != null)
            {
                p.gameObject.SetActive(false);
            }
        }
    }
}