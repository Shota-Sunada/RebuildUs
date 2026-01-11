namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
public class Arsonist : RoleBase<Arsonist>
{
    public static Color RoleColor = new Color32(238, 112, 46, byte.MaxValue);
    public static bool triggerArsonistWin = false;
    public bool dousedEveryone = false;
    public PlayerControl currentTarget;
    public PlayerControl douseTarget;
    public List<PlayerControl> dousedPlayers = [];
    public static CustomButton arsonistButton;
    public static CustomButton arsonistIgniteButton;

    // write configs here
    public static float cooldown { get { return CustomOptionHolder.arsonistCooldown.GetFloat(); } }
    public static float duration { get { return CustomOptionHolder.arsonistDuration.GetFloat(); } }
    public static bool canBeLovers { get { return CustomOptionHolder.arsonistCanBeLovers.GetBool(); } }

    public bool dousedEveryoneAlive()
    {
        return PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().All(x => { return x.IsRole(ERoleType.Arsonist) || x.Data.IsDead || x.Data.Disconnected || x.IsGM() || dousedPlayers.Any(y => y.PlayerId == x.PlayerId); });
    }

    public Arsonist()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = ERoleType.Arsonist;
        dousedEveryone = false;
        currentTarget = null;
        douseTarget = null;
        dousedPlayers = [];
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd()
    {
        updateIcons();
    }
    public override void OnIntroEnd()
    {
        updateIcons();
    }
    public override void FixedUpdate()
    {
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.Arsonist))
        {
            List<PlayerControl> untargetables;
            if (douseTarget != null)
            {
                untargetables = [.. PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().Where(x => x.PlayerId != douseTarget.PlayerId)];
            }
            else
            {
                untargetables = dousedPlayers;
            }
            currentTarget = setTarget(untargetablePlayers: untargetables);
            if (currentTarget != null) setPlayerOutline(currentTarget, RoleColor);
        }
    }
    public override void OnKill(PlayerControl target)
    {
        updateStatus();
    }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    public static void MakeButtons(HudManager hm)
    {
        arsonistButton = new CustomButton(
                () =>
                {
                    if (Local.currentTarget != null)
                    {
                        Local.douseTarget = Local.currentTarget;
                    }
                },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.Arsonist) && !Local.dousedEveryone && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
                () =>
                {
                    if (arsonistButton.isEffectActive && Local.douseTarget != Local.currentTarget)
                    {
                        Local.douseTarget = null;
                        arsonistButton.Timer = 0f;
                        arsonistButton.isEffectActive = false;
                    }

                    return CachedPlayer.LocalPlayer.PlayerControl.CanMove && Local.currentTarget != null;
                },
                () =>
                {
                    arsonistButton.Timer = arsonistButton.MaxTimer;
                    arsonistButton.isEffectActive = false;
                    Local.douseTarget = null;
                    Local.updateStatus();
                },
                getDouseSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                hm,
                hm.KillButton,
                KeyCode.F,
                true,
                duration,
                () =>
                {
                    if (Local.douseTarget != null)
                    {
                        using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.ArsonistDouse);
                        sender.Write(Local.douseTarget.PlayerId);
                        sender.Write(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                        RPCProcedure.arsonistDouse(Local.douseTarget.PlayerId, CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                    }

                    Local.douseTarget = null;
                    Local.updateStatus();
                    arsonistButton.Timer = Local.dousedEveryone ? 0 : arsonistButton.MaxTimer;

                    foreach (var p in Local.dousedPlayers)
                    {
                        if (MapOptions.PlayerIcons.ContainsKey(p.PlayerId))
                        {
                            MapOptions.PlayerIcons[p.PlayerId].SetSemiTransparent(false);
                        }
                    }
                }
            )
        {
            buttonText = Tr.Get("DouseText")
        };

        arsonistIgniteButton = new CustomButton(
            () =>
            {
                if (Local.dousedEveryone)
                {
                    using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.ArsonistWin);
                    sender.Write(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                    RPCProcedure.arsonistWin(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                }
            },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.Arsonist) && Local.dousedEveryone && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.CanMove && Local.dousedEveryone; },
            () => { },
            getIgniteSprite(),
            new Vector3(-1.8f, -0.06f, 0),
            hm,
            hm.KillButton,
            KeyCode.Q
        )
        {
            buttonText = Tr.Get("IgniteText")
        };
    }
    public static void SetButtonCooldowns()
    {
        arsonistButton.MaxTimer = cooldown;
        arsonistButton.EffectDuration = duration;
        arsonistIgniteButton.MaxTimer = 0f;
        arsonistIgniteButton.Timer = 0f;

    }

    public void updateStatus()
    {
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.Arsonist))
        {
            dousedEveryone = dousedEveryoneAlive();
        }
    }

    public void updateIcons()
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
                var isDoused = dousedPlayers.Any(x => x.PlayerId == p.PlayerId);
                MapOptions.PlayerIcons[p.PlayerId].SetSemiTransparent(!isDoused);
            }
        }
    }

    // write functions here
    private static Sprite douseSprite;
    public static Sprite getDouseSprite()
    {
        if (douseSprite) return douseSprite;
        douseSprite = Helpers.LoadSpriteFromResources("RebuildUs.Resources.DouseButton.png", 115f);
        return douseSprite;
    }

    private static Sprite igniteSprite;
    public static Sprite getIgniteSprite()
    {
        if (igniteSprite) return igniteSprite;
        igniteSprite = Helpers.LoadSpriteFromResources("RebuildUs.Resources.IgniteButton.png", 115f);
        return igniteSprite;
    }

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
        triggerArsonistWin = false;
        foreach (var p in MapOptions.PlayerIcons.Values)
        {
            if (p != null && p.gameObject != null)
            {
                p.gameObject.SetActive(false);
            }
        }
    }
}