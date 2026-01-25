namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
public class Arsonist : RoleBase<Arsonist>
{
    public static Color NameColor = new Color32(238, 112, 46, byte.MaxValue);
    public override Color RoleColor => NameColor;
    public static bool TriggerArsonistWin = false;
    public bool DousedEveryone = false;
    public PlayerControl CurrentTarget;
    public PlayerControl DouseTarget;
    public List<PlayerControl> DousedPlayers = [];
    public static CustomButton ArsonistButton;
    public static CustomButton ArsonistIgniteButton;

    // write configs here
    public static float Cooldown { get { return CustomOptionHolder.ArsonistCooldown.GetFloat(); } }
    public static float Duration { get { return CustomOptionHolder.ArsonistDuration.GetFloat(); } }
    public static bool CanBeLovers { get { return CustomOptionHolder.ArsonistCanBeLovers.GetBool(); } }

    public bool DousedEveryoneAlive()
    {
        var allPlayers = PlayerControl.AllPlayerControls;
        for (var i = 0; i < allPlayers.Count; i++)
        {
            var p = allPlayers[i];
            if (p.IsRole(RoleType.Arsonist) || p.Data.IsDead || p.Data.Disconnected || p.IsGM()) continue;

            bool isDoused = false;
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

    public Arsonist()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Arsonist;
        DousedEveryone = false;
        CurrentTarget = null;
        DouseTarget = null;
        DousedPlayers = [];
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
                untargetables = [];
                var allPlayers = PlayerControl.AllPlayerControls;
                for (var i = 0; i < allPlayers.Count; i++)
                {
                    var p = allPlayers[i];
                    if (p.PlayerId != DouseTarget.PlayerId)
                    {
                        untargetables.Add(p);
                    }
                }
            }
            else
            {
                untargetables = DousedPlayers;
            }
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
        ArsonistButton = new CustomButton(
            () =>
            {
                if (Local.CurrentTarget != null)
                {
                    Local.DouseTarget = Local.CurrentTarget;
                }
            },
            () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Arsonist) && !Local.DousedEveryone && PlayerControl.LocalPlayer.IsAlive(); },
            () =>
            {
                if (ArsonistButton.IsEffectActive && Local.DouseTarget != Local.CurrentTarget)
                {
                    Local.DouseTarget = null;
                    ArsonistButton.Timer = 0f;
                    ArsonistButton.IsEffectActive = false;
                }

                return PlayerControl.LocalPlayer.CanMove && Local.CurrentTarget != null;
            },
            () =>
            {
                ArsonistButton.Timer = ArsonistButton.MaxTimer;
                ArsonistButton.IsEffectActive = false;
                Local.DouseTarget = null;
                Local.UpdateStatus();
            },
            GetDouseSprite(),
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            KeyCode.F,
            true,
            Duration,
            () =>
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
                    if (ModMapOptions.PlayerIcons.ContainsKey(p.PlayerId))
                    {
                        ModMapOptions.PlayerIcons[p.PlayerId].SetSemiTransparent(false);
                    }
                }
            },
            false,
            Tr.Get("Hud.DouseText")
        );

        ArsonistIgniteButton = new CustomButton(
            () =>
            {
                if (Local.DousedEveryone)
                {
                    using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.ArsonistWin);
                    sender.Write(PlayerControl.LocalPlayer.PlayerId);
                    RPCProcedure.ArsonistWin(PlayerControl.LocalPlayer.PlayerId);
                }
            },
            () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Arsonist) && Local.DousedEveryone && PlayerControl.LocalPlayer.IsAlive(); },
            () => { return PlayerControl.LocalPlayer.CanMove && Local.DousedEveryone; },
            () => { },
            AssetLoader.IgniteButton,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            KeyCode.Q,
            false,
            Tr.Get("Hud.IgniteText")
        );
    }
    public static void SetButtonCooldowns()
    {
        ArsonistButton?.MaxTimer = Cooldown;
        Local?.UpdateStatus();
    }

    public void UpdateStatus()
    {
        DousedEveryone = DousedEveryoneAlive();
    }

    public void UpdateIcons()
    {
        foreach (var pp in ModMapOptions.PlayerIcons.Values)
        {
            pp.gameObject.SetActive(false);
        }

        if (Local != null)
        {
            var visibleCounter = 0;
            var bottomLeft = FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition;
            bottomLeft.x *= -1;
            bottomLeft += new Vector3(-0.25f, -0.25f, 0);

            var allPlayers = PlayerControl.AllPlayerControls;
            for (var i = 0; i < allPlayers.Count; i++)
            {
                var p = allPlayers[i];
                if (p.PlayerId == PlayerControl.LocalPlayer.PlayerId) continue;
                if (!ModMapOptions.PlayerIcons.ContainsKey(p.PlayerId)) continue;

                if (p.Data.IsDead || p.Data.Disconnected)
                {
                    ModMapOptions.PlayerIcons[p.PlayerId].gameObject.SetActive(false);
                }
                else
                {
                    ModMapOptions.PlayerIcons[p.PlayerId].gameObject.SetActive(true);
                    ModMapOptions.PlayerIcons[p.PlayerId].transform.localScale = Vector3.one * 0.25f;
                    ModMapOptions.PlayerIcons[p.PlayerId].transform.localPosition = bottomLeft + Vector3.right * visibleCounter * 0.45f;
                    visibleCounter++;

                    bool isDoused = false;
                    for (var j = 0; j < DousedPlayers.Count; j++)
                    {
                        if (DousedPlayers[j].PlayerId == p.PlayerId)
                        {
                            isDoused = true;
                            break;
                        }
                    }
                    ModMapOptions.PlayerIcons[p.PlayerId].SetSemiTransparent(!isDoused);
                }
            }
        }
    }

    // write functions here
    private static Sprite DouseSprite;
    public static Sprite GetDouseSprite()
    {
        if (DouseSprite) return DouseSprite;
        DouseSprite = AssetLoader.DouseButton;
        return DouseSprite;
    }

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
        TriggerArsonistWin = false;
        foreach (var p in ModMapOptions.PlayerIcons.Values)
        {
            if (p != null && p.gameObject != null)
            {
                p.gameObject.SetActive(false);
            }
        }
    }
}