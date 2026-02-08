namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
public class Arsonist : RoleBase<Arsonist>
{
    public static Color NameColor = new Color32(238, 112, 46, byte.MaxValue);

    public static bool TriggerArsonistWin;
    public static CustomButton ArsonistButton;
    public static CustomButton ArsonistIgniteButton;
    private readonly List<PlayerControl> _untargetablesCache = [];
    public PlayerControl CurrentTarget;
    public bool DousedEveryone;
    public List<PlayerControl> DousedPlayers = [];
    public PlayerControl DouseTarget;

    public Arsonist()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Arsonist;
        DousedEveryone = false;
        CurrentTarget = null;
        DouseTarget = null;
        DousedPlayers = [];
    }

    public override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    public static float Cooldown
    {
        get => CustomOptionHolder.ArsonistCooldown.GetFloat();
    }

    public static float Duration
    {
        get => CustomOptionHolder.ArsonistDuration.GetFloat();
    }

    public static bool CanBeLovers
    {
        get => CustomOptionHolder.ArsonistCanBeLovers.GetBool();
    }

    public bool DousedEveryoneAlive()
    {
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p.IsRole(RoleType.Arsonist) || p.Data.IsDead || p.Data.Disconnected || p.IsGm()) continue;

            var isDoused = false;
            for (var j = 0; j < DousedPlayers.Count; j++)
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

    public override void OnMeetingStart() { }

    public override void OnMeetingEnd()
    {
        UpdateIcons();
    }

    public override void OnIntroEnd()
    {
        UpdateIcons();
    }

    public override void FixedUpdate()
    {
        var local = Local;
        if (local != null)
        {
            List<PlayerControl> untargetables;
            if (DouseTarget != null)
            {
                _untargetablesCache.Clear();
                foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
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

    public override void OnKill(PlayerControl target)
    {
        UpdateStatus();
    }

    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    public static void MakeButtons(HudManager hm)
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
                using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.ArsonistDouse);
                sender.Write(Local.DouseTarget.PlayerId);
                sender.Write(PlayerControl.LocalPlayer.PlayerId);
                RPCProcedure.ArsonistDouse(Local.DouseTarget.PlayerId, PlayerControl.LocalPlayer.PlayerId);
            }

            Local.DouseTarget = null;
            Local.UpdateStatus();
            ArsonistButton.Timer = Local.DousedEveryone ? 0 : ArsonistButton.MaxTimer;

            foreach (var p in Local.DousedPlayers)
            {
                if (MapSettings.PlayerIcons.ContainsKey(p.PlayerId))
                    MapSettings.PlayerIcons[p.PlayerId].SetSemiTransparent(false);
            }
        }, false, Tr.Get(TrKey.DouseText));

        ArsonistIgniteButton = new(() =>
        {
            if (Local.DousedEveryone)
            {
                using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.ArsonistWin);
                sender.Write(PlayerControl.LocalPlayer.PlayerId);
                RPCProcedure.ArsonistWin(PlayerControl.LocalPlayer.PlayerId);
            }
        }, () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Arsonist) && Local.DousedEveryone && PlayerControl.LocalPlayer.IsAlive(); }, () => { return PlayerControl.LocalPlayer.CanMove && Local.DousedEveryone; }, () => { }, AssetLoader.IgniteButton, ButtonPosition.Layout, hm, hm.KillButton, AbilitySlot.NeutralAbilityPrimary, false, Tr.Get(TrKey.IgniteText));
    }

    public static void SetButtonCooldowns()
    {
        ArsonistButton?.MaxTimer = Cooldown;
        ArsonistIgniteButton.Timer = ArsonistIgniteButton.MaxTimer = 0f;
        Local?.UpdateStatus();
    }

    public void UpdateStatus()
    {
        DousedEveryone = DousedEveryoneAlive();
    }

    public void UpdateIcons()
    {
        foreach (var pp in MapSettings.PlayerIcons.Values) pp.gameObject.SetActive(false);

        if (Local != null && FastDestroyableSingleton<HudManager>.Instance != null)
        {
            var bottomLeft = AspectPosition.ComputePosition(AspectPosition.EdgeAlignments.LeftBottom, new(0.9f, 0.7f, -10f));
            var visibleCounter = 0;
            foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
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
    }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
        TriggerArsonistWin = false;
        foreach (var p in MapSettings.PlayerIcons.Values)
        {
            if (p != null && p.gameObject != null)
                p.gameObject.SetActive(false);
        }
    }
}
