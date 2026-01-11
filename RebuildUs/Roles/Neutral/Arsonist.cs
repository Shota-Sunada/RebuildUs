namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
public class Arsonist : RoleBase<Arsonist>
{
    public static Color RoleColor = new Color32(238, 112, 46, byte.MaxValue);
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
        return PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().All(x => { return x.IsRole(ERoleType.Arsonist) || x.Data.IsDead || x.Data.Disconnected || x.IsGM() || DousedPlayers.Any(y => y.PlayerId == x.PlayerId); });
    }

    public Arsonist()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = ERoleType.Arsonist;
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
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.Arsonist))
        {
            List<PlayerControl> untargetables;
            if (DouseTarget != null)
            {
                untargetables = [.. PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().Where(x => x.PlayerId != DouseTarget.PlayerId)];
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
                () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.Arsonist) && !Local.DousedEveryone && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
                () =>
                {
                    if (ArsonistButton.IsEffectActive && Local.DouseTarget != Local.CurrentTarget)
                    {
                        Local.DouseTarget = null;
                        ArsonistButton.Timer = 0f;
                        ArsonistButton.IsEffectActive = false;
                    }

                    return CachedPlayer.LocalPlayer.PlayerControl.CanMove && Local.CurrentTarget != null;
                },
                () =>
                {
                    ArsonistButton.Timer = ArsonistButton.MaxTimer;
                    ArsonistButton.IsEffectActive = false;
                    Local.DouseTarget = null;
                    Local.UpdateStatus();
                },
                GetDouseSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                hm,
                hm.KillButton,
                KeyCode.F,
                true,
                Duration,
                () =>
                {
                    if (Local.DouseTarget != null)
                    {
                        using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.ArsonistDouse);
                        sender.Write(Local.DouseTarget.PlayerId);
                        sender.Write(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                        RPCProcedure.ArsonistDouse(Local.DouseTarget.PlayerId, CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                    }

                    Local.DouseTarget = null;
                    Local.UpdateStatus();
                    ArsonistButton.Timer = Local.DousedEveryone ? 0 : ArsonistButton.MaxTimer;

                    foreach (var p in Local.DousedPlayers)
                    {
                        if (MapOptions.PlayerIcons.ContainsKey(p.PlayerId))
                        {
                            MapOptions.PlayerIcons[p.PlayerId].SetSemiTransparent(false);
                        }
                    }
                }
            )
        {
            ButtonText = Tr.Get("RoleText.DouseText")
        };

        ArsonistIgniteButton = new CustomButton(
            () =>
            {
                if (Local.DousedEveryone)
                {
                    using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.ArsonistWin);
                    sender.Write(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                    RPCProcedure.ArsonistWin(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                }
            },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.Arsonist) && Local.DousedEveryone && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove && Local.DousedEveryone; },
            () => { },
            AssetLoader.IgniteButton,
            new Vector3(-1.8f, -0.06f, 0),
            hm,
            hm.KillButton,
            KeyCode.Q
        )
        {
            ButtonText = Tr.Get("RoleText.IgniteText")
        };
    }
    public static void SetButtonCooldowns()
    {
        ArsonistButton.MaxTimer = Cooldown;
        ArsonistButton.EffectDuration = Duration;
        ArsonistIgniteButton.MaxTimer = 0f;
        ArsonistIgniteButton.Timer = 0f;

    }

    public void UpdateStatus()
    {
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.Arsonist))
        {
            DousedEveryone = DousedEveryoneAlive();
        }
    }

    public void UpdateIcons()
    {
        foreach (var pp in MapOptions.PlayerIcons.Values)
        {
            pp.gameObject.SetActive(false);
        }

        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.Arsonist))
        {
            var visibleCounter = 0;
            var bottomLeft = FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition;
            bottomLeft.x *= -1;
            bottomLeft += new Vector3(-0.25f, -0.25f, 0);

            foreach (var p in CachedPlayer.AllPlayers)
            {
                if (p.PlayerId == CachedPlayer.LocalPlayer.PlayerControl.PlayerId) continue;
                if (!MapOptions.PlayerIcons.ContainsKey(p.PlayerId)) continue;

                if (p.Data.IsDead || p.Data.Disconnected)
                {
                    MapOptions.PlayerIcons[p.PlayerId].gameObject.SetActive(false);
                }
                else
                {
                    MapOptions.PlayerIcons[p.PlayerId].gameObject.SetActive(true);
                    MapOptions.PlayerIcons[p.PlayerId].transform.localScale = Vector3.one * 0.25f;
                    MapOptions.PlayerIcons[p.PlayerId].transform.localPosition = bottomLeft + Vector3.right * visibleCounter * 0.45f;
                    visibleCounter++;
                }
                var isDoused = DousedPlayers.Any(x => x.PlayerId == p.PlayerId);
                MapOptions.PlayerIcons[p.PlayerId].SetSemiTransparent(!isDoused);
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
        foreach (var p in MapOptions.PlayerIcons.Values)
        {
            if (p != null && p.gameObject != null)
            {
                p.gameObject.SetActive(false);
            }
        }
    }
}