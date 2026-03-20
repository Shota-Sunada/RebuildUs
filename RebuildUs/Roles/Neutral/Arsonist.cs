namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
[RegisterRole(RoleType.Arsonist, RoleTeam.Neutral, typeof(SingleRoleBase<Arsonist>), nameof(CustomOptionHolder.ArsonistSpawnRate))]
internal class Arsonist : SingleRoleBase<Arsonist>
{
    internal static Color Color = new Color32(238, 112, 46, byte.MaxValue);

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

    [CustomEvent(CustomEventType.OnMeetingEnd)]
    internal void OnMeetingEnd()
    {
        UpdateIcons();
    }

    [CustomEvent(CustomEventType.OnIntroEnd)]
    internal void OnIntroEnd()
    {
        UpdateIcons();
    }

    [CustomEvent(CustomEventType.FixedUpdate)]
    internal void FixedUpdate()
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

    [CustomEvent(CustomEventType.OnKill)]
    internal void OnKill(PlayerControl target)
    {
        UpdateStatus();
    }



    [RegisterCustomButton]
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
                    ArsonistDouse(PlayerControl.LocalPlayer, Local._douseTarget.PlayerId);
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
                ArsonistWin(PlayerControl.LocalPlayer);
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

    [RegisterCustomButton]
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

    [MethodRpc((uint)CustomRPC.ArsonistDouse)]
    internal static void ArsonistDouse(PlayerControl sender, byte playerId)
    {
        var arsonist = Instance;
        if (arsonist == null)
        {
            return;
        }
        var target = Helpers.PlayerById(playerId);
        if (target != null)
        {
            arsonist.DousedPlayers.Add(target);
        }
    }

    [MethodRpc((uint)CustomRPC.ArsonistWin)]
    internal static void ArsonistWin(PlayerControl sender)
    {
        TriggerArsonistWin = true;
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p == null || !p.IsAlive() || p.IsRole(RoleType.Arsonist))
            {
                continue;
            }
            p.Exiled();
            GameHistory.FinalStatuses[p.PlayerId] = FinalStatus.Torched;
        }
    }
}