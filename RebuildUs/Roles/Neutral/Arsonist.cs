namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
internal class Arsonist : RoleBase<Arsonist>
{
    internal static Color NameColor = new Color32(238, 112, 46, byte.MaxValue);

    internal static bool TriggerArsonistWin;
    internal static CustomButton ArsonistButton;
    internal static CustomButton ArsonistIgniteButton;
    private readonly List<PlayerControl> _untargetablesCache = [];
    internal PlayerControl CurrentTarget;
    internal bool DousedEveryone;
    internal List<PlayerControl> DousedPlayers = [];
    internal PlayerControl DouseTarget;

    public Arsonist()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Arsonist;
        DousedEveryone = false;
        CurrentTarget = null;
        DouseTarget = null;
        DousedPlayers = [];
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    internal static float Cooldown { get => CustomOptionHolder.ArsonistCooldown.GetFloat(); }
    internal static float Duration { get => CustomOptionHolder.ArsonistDuration.GetFloat(); }
    internal static bool CanBeLovers { get => CustomOptionHolder.ArsonistCanBeLovers.GetBool(); }

    internal bool DousedEveryoneAlive()
    {
        foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p.IsRole(RoleType.Arsonist) || p.Data.IsDead || p.Data.Disconnected || p.IsGm()) continue;

            bool isDoused = false;
            for (int j = 0; j < DousedPlayers.Count; j++)
            {
                if (DousedPlayers[j].PlayerId == p.PlayerId)
                {
                    isDoused = true;
                    break;
                }
            }

            if (!isDoused) return false;
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
        Arsonist local = Local;
        if (local != null)
        {
            List<PlayerControl> untargetables;
            if (DouseTarget != null)
            {
                _untargetablesCache.Clear();
                foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                {
                    if (p.PlayerId != DouseTarget.PlayerId)
                        _untargetablesCache.Add(p);
                }

                untargetables = _untargetablesCache;
            }
            else
                untargetables = DousedPlayers;

            CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetables);
            if (CurrentTarget != null) Helpers.SetPlayerOutline(CurrentTarget, RoleColor);
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
        ArsonistButton = new(() =>
        {
            if (Local.CurrentTarget != null) Local.DouseTarget = Local.CurrentTarget;
        }, () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Arsonist) && !Local.DousedEveryone && PlayerControl.LocalPlayer.IsAlive(); }, () =>
        {
            if (ArsonistButton.IsEffectActive && Local.DouseTarget != Local.CurrentTarget)
            {
                Local.DouseTarget = null;
                ArsonistButton.Timer = 0f;
                ArsonistButton.IsEffectActive = false;
            }

            return PlayerControl.LocalPlayer.CanMove && Local.CurrentTarget != null;
        }, () =>
        {
            ArsonistButton.Timer = ArsonistButton.MaxTimer;
            ArsonistButton.IsEffectActive = false;
            Local.DouseTarget = null;
            Local.UpdateStatus();
        }, AssetLoader.DouseButton, ButtonPosition.Layout, hm, hm.KillButton, AbilitySlot.NeutralAbilityPrimary, true, Duration, () =>
        {
            if (Local.DouseTarget != null)
            {
                using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.ArsonistDouse);
                sender.Write(Local.DouseTarget.PlayerId);
                sender.Write(PlayerControl.LocalPlayer.PlayerId);
                RPCProcedure.ArsonistDouse(Local.DouseTarget.PlayerId, PlayerControl.LocalPlayer.PlayerId);
            }

            Local.DouseTarget = null;
            Local.UpdateStatus();
            ArsonistButton.Timer = Local.DousedEveryone ? 0 : ArsonistButton.MaxTimer;

            foreach (PlayerControl p in Local.DousedPlayers)
            {
                if (MapSettings.PlayerIcons.ContainsKey(p.PlayerId))
                    MapSettings.PlayerIcons[p.PlayerId].SetSemiTransparent(false);
            }
        }, false, Tr.Get(TrKey.DouseText));

        ArsonistIgniteButton = new(() =>
        {
            if (Local.DousedEveryone)
            {
                using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.ArsonistWin);
                sender.Write(PlayerControl.LocalPlayer.PlayerId);
                RPCProcedure.ArsonistWin(PlayerControl.LocalPlayer.PlayerId);
            }
        }, () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Arsonist) && Local.DousedEveryone && PlayerControl.LocalPlayer.IsAlive(); }, () => { return PlayerControl.LocalPlayer.CanMove && Local.DousedEveryone; }, () => { }, AssetLoader.IgniteButton, ButtonPosition.Layout, hm, hm.KillButton, AbilitySlot.NeutralAbilityPrimary, false, Tr.Get(TrKey.IgniteText));
    }

    internal static void SetButtonCooldowns()
    {
        ArsonistButton?.MaxTimer = Cooldown;
        ArsonistIgniteButton.Timer = ArsonistIgniteButton.MaxTimer = 0f;
        Local?.UpdateStatus();
    }

    internal void UpdateStatus()
    {
        DousedEveryone = DousedEveryoneAlive();
    }

    internal void UpdateIcons()
    {
        foreach (PoolablePlayer pp in MapSettings.PlayerIcons.Values) pp.gameObject.SetActive(false);

        if (Local != null && FastDestroyableSingleton<HudManager>.Instance != null)
        {
            Vector3 bottomLeft = AspectPosition.ComputePosition(AspectPosition.EdgeAlignments.LeftBottom, new(0.9f, 0.7f, -10f));
            int visibleCounter = 0;
            foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (p.PlayerId == PlayerControl.LocalPlayer.PlayerId) continue;
                if (!MapSettings.PlayerIcons.ContainsKey(p.PlayerId)) continue;

                if (p.Data.IsDead || p.Data.Disconnected)
                    MapSettings.PlayerIcons[p.PlayerId].gameObject.SetActive(false);
                else
                {
                    MapSettings.PlayerIcons[p.PlayerId].gameObject.SetActive(true);
                    MapSettings.PlayerIcons[p.PlayerId].transform.localScale = Vector3.one * 0.3f;
                    MapSettings.PlayerIcons[p.PlayerId].transform.localPosition = bottomLeft + (Vector3.right * visibleCounter * 0.45f);
                    visibleCounter++;

                    bool isDoused = false;
                    for (int j = 0; j < DousedPlayers.Count; j++)
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
    }

    // write functions here

    internal static void Clear()
    {
        // reset configs here
        Players.Clear();
        TriggerArsonistWin = false;
        foreach (PoolablePlayer p in MapSettings.PlayerIcons.Values)
        {
            if (p != null && p.gameObject != null)
                p.gameObject.SetActive(false);
        }
    }
}