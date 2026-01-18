namespace RebuildUs.Roles.Modifier;

[HarmonyPatch]
public class LastImpostor : ModifierBase<LastImpostor>
{
    public static Color NameColor = Palette.ImpostorRed;
    public override Color ModifierColor => NameColor;
    public enum DivineResults
    {
        BlackWhite,
        Team,
        Role,
    }

    public static bool isEnable { get { return CustomOptionHolder.lastImpostorEnable.GetBool(); } }
    public static int killCounter = 0;
    public static int maxKillCounter { get { return (int)CustomOptionHolder.lastImpostorNumKills.GetFloat(); } }
    public static int numUsed = 0;
    public static int remainingShots = 0;
    public static int selectedFunction { get { return CustomOptionHolder.lastImpostorFunctions.GetSelection(); } }
    public static DivineResults divineResult { get { return (DivineResults)CustomOptionHolder.lastImpostorResults.GetSelection(); } }

    public static string postfix
    {
        get
        {
            return Tr.Get("lastImpostorPostfix");
        }
    }
    public static string fullName
    {
        get
        {
            return Tr.Get("lastImpostor");
        }
    }

    public static List<CustomButton> lastImpostorButtons = [];
    static Dictionary<byte, PoolablePlayer> playerIcons = [];

    public LastImpostor()
    {
        // write value init here
        StaticModifierType = CurrentModifierType = ModifierType.LastImpostor;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate() { }
    public override void OnKill(PlayerControl target)
    {
        killCounter += 1;
    }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void MakeButtons(HudManager hm)
    {
        lastImpostorButtons = [];

        Vector3 lastImpostorCalcPos(byte index)
        {
            //return new Vector3(-0.25f, -0.25f, 0) + Vector3.right * index * 0.55f;
            return new Vector3(-0.25f, -0.15f, 0) + Vector3.right * index * 0.55f;
        }

        Action lastImpostorButtonOnClick(byte index)
        {
            return () =>
            {
                if (selectedFunction == 1) return;
                PlayerControl p = Helpers.PlayerById(index);
                LastImpostor.divine(p);
            };
        }
        ;

        Func<bool> lastImpostorHasButton(byte index)
        {
            return () =>
            {
                if (selectedFunction == 1) return false;
                var p = CachedPlayer.LocalPlayer.PlayerControl;
                if (!p.HasModifier(ModifierType.LastImpostor)) return false;
                if (p.HasModifier(ModifierType.LastImpostor) && p.CanMove && p.IsAlive() & p.PlayerId != index
                    && ModMapOptions.PlayerIcons.ContainsKey(index) && numUsed < 1 && isCounterMax())
                {
                    return true;
                }
                else
                {
                    if (playerIcons.ContainsKey(index))
                    {
                        playerIcons[index].gameObject.SetActive(false);
                        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.BountyHunter))
                            setBountyIconPos(Vector3.zero);
                    }
                    if (lastImpostorButtons.Count > index)
                    {
                        lastImpostorButtons[index].SetActive(false);
                    }
                    return false;
                }
            };
        }

        void setButtonPos(byte index)
        {
            Vector3 pos = lastImpostorCalcPos(index);
            Vector3 scale = new(0.4f, 0.8f, 1.0f);

            Vector3 iconBase = hm.UseButton.transform.localPosition;
            iconBase.x *= -1;
            if (lastImpostorButtons[index].PositionOffset != pos)
            {
                lastImpostorButtons[index].PositionOffset = pos;
                lastImpostorButtons[index].LocalScale = scale;
                playerIcons[index].transform.localPosition = iconBase + pos;
            }
        }

        void setIconStatus(byte index, bool transparent)
        {
            playerIcons[index].transform.localScale = Vector3.one * 0.25f;
            playerIcons[index].gameObject.SetActive(CachedPlayer.LocalPlayer.PlayerControl.CanMove);
            playerIcons[index].SetSemiTransparent(transparent);
        }

        void setBountyIconPos(Vector3 offset)
        {
            Vector3 bottomLeft = new(-FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.x, FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.y, FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.z);
            PoolablePlayer icon = ModMapOptions.PlayerIcons[BountyHunter.Bounty.PlayerId];
            icon.transform.localPosition = bottomLeft + new Vector3(-0.25f, 0f, 0) + offset;
            BountyHunter.CooldownText.transform.localPosition = bottomLeft + new Vector3(-0.25f, 0f, -1f) + offset;
        }

        Func<bool> lastImpostorCouldUse(byte index)
        {
            return () =>
            {
                if (selectedFunction == 1) return false;

                //　ラストインポスター以外の場合、リソースがない場合はボタンを表示しない
                var p = Helpers.PlayerById(index);
                if (!playerIcons.ContainsKey(index) ||
                    !CachedPlayer.LocalPlayer.PlayerControl.HasModifier(ModifierType.LastImpostor) ||
                    !isCounterMax())
                {
                    return false;
                }

                // ボタンの位置を変更
                setButtonPos(index);

                // ボタンにテキストを設定
                lastImpostorButtons[index].ButtonText = CachedPlayer.LocalPlayer.PlayerControl.IsAlive() ? "生存" : "死亡";

                // アイコンの位置と透明度を変更
                setIconStatus(index, false);

                // Bounty Hunterの場合賞金首の位置をずらして表示する
                if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.BountyHunter))
                {
                    Vector3 offset = new(0f, 1f, 0f);
                    setBountyIconPos(offset);
                }

                return CachedPlayer.LocalPlayer.PlayerControl.CanMove && numUsed < 1;
            };
        }

        for (byte i = 0; i < 15; i++)
        {
            CustomButton lastImpostorButton = new(
                lastImpostorButtonOnClick(i),
                lastImpostorHasButton(i),
                lastImpostorCouldUse(i),
                () => { },
                null,
                Vector3.zero,
                hm,
                null,
                KeyCode.None,
                true
            )
            {
                Timer = 0.0f,
                MaxTimer = 0.0f
            };

            lastImpostorButtons.Add(lastImpostorButton);
        }
    }
    public override void SetButtonCooldowns() { }

    // write functions here

    public override void Clear()
    {
        // reset configs here
        Players.Clear();
        killCounter = 0;
        numUsed = 0;
        remainingShots = (int)CustomOptionHolder.lastImpostorNumShots.GetFloat();
        playerIcons = [];
    }

    public static bool isCounterMax()
    {
        if (maxKillCounter <= killCounter) return true;
        return false;
    }

    public static bool canGuess()
    {
        return remainingShots > 0 && selectedFunction == 1 && isCounterMax();
    }

    public static void promoteToLastImpostor()
    {
        if (!isEnable) return;

        var impList = new List<PlayerControl>();
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p.IsTeamImpostor() && p.IsAlive()) impList.Add(p);
        }
        if (impList.Count == 1)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ImpostorPromotesToLastImpostor, Hazel.SendOption.Reliable, -1);
            writer.Write(impList[0].PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.impostorPromotesToLastImpostor(impList[0].PlayerId);
        }
    }
    public static void divine(PlayerControl p)
    {
        // FortuneTeller.divine(p, resultIsCrewOrNot);
        string msgBase = "";
        string msgInfo = "";
        Color color = Color.white;

        if (divineResult == DivineResults.BlackWhite)
        {
            if (p.IsTeamCrewmate())
            {
                msgBase = "divineMessageIsCrew";
                color = Color.white;
            }
            else
            {
                msgBase = "divineMessageIsntCrew";
                color = Palette.ImpostorRed;
            }
        }

        else if (divineResult == DivineResults.Team)
        {
            msgBase = "divineMessageTeam";
            if (p.IsTeamCrewmate())
            {
                msgInfo = Tr.Get("divineCrew");
                color = Color.white;
            }
            else if (p.IsNeutral())
            {
                msgInfo = Tr.Get("divineNeutral");
                color = Color.yellow;
            }
            else
            {
                msgInfo = Tr.Get("divineImpostor");
                color = Palette.ImpostorRed;
            }
        }

        else if (divineResult == DivineResults.Role)
        {
            msgBase = "divineMessageRole";
            msgInfo = string.Join(" ", [.. RoleInfo.GetRoleInfoForPlayer(p).Select(x => Helpers.Cs(x.Color, x.Name))]);
        }

        string msg = string.Format(Tr.Get(msgBase), p.name, msgInfo);
        if (!string.IsNullOrWhiteSpace(msg))
        {
            // TODO: FortuneTeller.fortuneTellerMessage(msg, 5f, color);
        }

        if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(FastDestroyableSingleton<HudManager>.Instance.TaskCompleteSound, false, 0.8f);

        // 占いを実行したことで発火される処理を他クライアントに通知
        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.FortuneTellerUsedDivine, Hazel.SendOption.Reliable, -1);
        writer.Write(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
        writer.Write(p.PlayerId);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
        RPCProcedure.fortuneTellerUsedDivine(CachedPlayer.LocalPlayer.PlayerControl.PlayerId, p.PlayerId);
    }

    public static void OnIntroDestroy(IntroCutscene __instance)
    {
        if (CachedPlayer.LocalPlayer.PlayerControl != null && FastDestroyableSingleton<HudManager>.Instance != null)
        {
            foreach (PlayerControl p in CachedPlayer.AllPlayers)
            {
                var player = UnityEngine.Object.Instantiate(__instance.PlayerPrefab, FastDestroyableSingleton<HudManager>.Instance.transform);
                player.UpdateFromPlayerOutfit(p.Data.DefaultOutfit, PlayerMaterial.MaskType.ComplexUI, p.Data.IsDead, true);
                player.SetFlipX(true);
                player.cosmetics.currentPet?.gameObject.SetActive(false);
                player.cosmetics.nameText.text = p.Data.DefaultOutfit.PlayerName;
                player.gameObject.SetActive(false);
                playerIcons[p.PlayerId] = player;
            }
        }
    }
}