namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
[RegisterRole(RoleType.Arsonist, RoleTeam.Neutral, typeof(SingleRoleBase<Arsonist>), nameof(CustomOptionHolder.ArsonistSpawnRate))]
internal class Arsonist : SingleRoleBase<Arsonist>
{
    public static Color Color = new Color32(238, 112, 46, byte.MaxValue);

    internal static bool TriggerArsonistWin;
    private static CustomButton ArsonistButton;
    private static CustomButton ArsonistIgniteButton;
    private readonly List<PlayerControl> _untargetablesCache = [];
    internal readonly List<PlayerControl> DousedPlayers = [];
    private PlayerControl _currentTarget;
    private bool _dousedEveryone;
    private PlayerControl _douseTarget;

    public Arsonist()
    {
        StaticRoleType = CurrentRoleType = RoleType.Arsonist;
        _dousedEveryone = false;
        _currentTarget = null;
        _douseTarget = null;
        DousedPlayers = [];
    }

    private static float Cooldown { get => CustomOptionHolder.ArsonistCooldown.GetFloat(); }
    private static float Duration { get => CustomOptionHolder.ArsonistDuration.GetFloat(); }
    internal static bool CanBeLovers { get => CustomOptionHolder.ArsonistCanBeLovers.GetBool(); }

    private bool DousedEveryoneAlive()
    {
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p.IsRole(RoleType.Arsonist) || p.IsDead() || p.IsGm())
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
        if (Local == null)
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
        ArsonistButton = new(
            nameof(ArsonistButton),
            () =>
            {
                if (Local._currentTarget != null)
                {
                    Local._douseTarget = Local._currentTarget;
                }
            },
            () => PlayerControl.LocalPlayer.IsRole(RoleType.Arsonist) && !Local._dousedEveryone && PlayerControl.LocalPlayer.IsAlive(),
            () =>
            {
                if (ArsonistButton?.IsEffectActive == true && Local._douseTarget != Local._currentTarget)
                {
                    Local._douseTarget = null;
                    ArsonistButton?.Timer = 0f;
                    ArsonistButton?.IsEffectActive = false;
                }

                return PlayerControl.LocalPlayer.CanMove && Local._currentTarget != null;
            },
            () =>
            {
                ArsonistButton.Timer = ArsonistButton.MaxTimer;
                ArsonistButton.IsEffectActive = false;
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
                ArsonistButton.Timer = Local._dousedEveryone ? 0 : ArsonistButton.MaxTimer;

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

        ArsonistIgniteButton = new(
            nameof(ArsonistIgniteButton),
            () =>
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
                return PlayerControl.LocalPlayer.CanMove && Local._dousedEveryone && !TriggerArsonistWin;
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

    [SetCustomButtonTimer]
    internal static void SetButtonCooldowns()
    {
        ArsonistButton.MaxTimer = Cooldown;
        ArsonistIgniteButton.Timer = ArsonistIgniteButton.MaxTimer = 0f;
        Local?.UpdateStatus();
    }

    private static bool TEST(object obj)
    {
        Logger.LogInfo("Object is null: {0}", obj == null);

        return obj == null;
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