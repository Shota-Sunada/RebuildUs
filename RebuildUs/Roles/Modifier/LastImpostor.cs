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

    public static bool IsEnable { get { return CustomOptionHolder.LastImpostorEnable.GetBool(); } }
    public static int KillCounter = 0;
    public static int MaxKillCounter { get { return (int)CustomOptionHolder.LastImpostorNumKills.GetFloat(); } }
    public static int NumUsed = 0;
    public static int RemainingShots = 0;
    public static int SelectedFunction { get { return CustomOptionHolder.LastImpostorFunctions.GetSelection(); } }
    public static DivineResults DivineResult { get { return (DivineResults)CustomOptionHolder.LastImpostorResults.GetSelection(); } }

    public static string Postfix
    {
        get
        {
            return Tr.Get("Hud.LastImpostorPostfix");
        }
    }
    public static string FullName
    {
        get
        {
            return Tr.Get("Modifier.LastImpostor");
        }
    }

    public static List<CustomButton> LastImpostorButtons = [];
    static Dictionary<byte, PoolablePlayer> PlayerIcons = [];

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
        KillCounter += 1;
    }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public static void MakeButtons(HudManager hm)
    {
        LastImpostorButtons = [];

        Vector3 LastImpostorCalcPos(byte index)
        {
            //return new Vector3(-0.25f, -0.25f, 0) + Vector3.right * index * 0.55f;
            return new Vector3(-0.25f, -0.15f, 0) + Vector3.right * index * 0.55f;
        }

        Action LastImpostorButtonOnClick(byte index)
        {
            return () =>
            {
                if (SelectedFunction == 1) return;
                PlayerControl p = Helpers.PlayerById(index);
                LastImpostor.Divine(p);
            };
        }
        ;

        Func<bool> LastImpostorHasButton(byte index)
        {
            return () =>
            {
                if (SelectedFunction == 1) return false;
                var p = PlayerControl.LocalPlayer;
                if (!p.HasModifier(ModifierType.LastImpostor)) return false;
                if (p.HasModifier(ModifierType.LastImpostor) && p.CanMove && p.IsAlive() & p.PlayerId != index
                    && ModMapOptions.PlayerIcons.ContainsKey(index) && NumUsed < 1 && IsCounterMax())
                {
                    return true;
                }
                else
                {
                    if (PlayerIcons.ContainsKey(index))
                    {
                        PlayerIcons[index].gameObject.SetActive(false);
                        if (PlayerControl.LocalPlayer.IsRole(RoleType.BountyHunter))
                            SetBountyIconPos(Vector3.zero);
                    }
                    if (LastImpostorButtons.Count > index)
                    {
                        LastImpostorButtons[index].SetActive(false);
                    }
                    return false;
                }
            };
        }

        void SetButtonPos(byte index)
        {
            Vector3 pos = LastImpostorCalcPos(index);
            Vector3 scale = new(0.4f, 0.8f, 1.0f);

            Vector3 iconBase = hm.UseButton.transform.localPosition;
            iconBase.x *= -1;
            if (LastImpostorButtons[index].PositionOffset != pos)
            {
                LastImpostorButtons[index].PositionOffset = pos;
                LastImpostorButtons[index].LocalScale = scale;
                PlayerIcons[index].transform.localPosition = iconBase + pos;
            }
        }

        void SetIconStatus(byte index, bool transparent)
        {
            PlayerIcons[index].transform.localScale = Vector3.one * 0.25f;
            PlayerIcons[index].gameObject.SetActive(PlayerControl.LocalPlayer.CanMove);
            PlayerIcons[index].SetSemiTransparent(transparent);
        }

        void SetBountyIconPos(Vector3 offset)
        {
            Vector3 bottomLeft = new(-FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.x, FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.y, FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.z);
            PoolablePlayer icon = ModMapOptions.PlayerIcons[BountyHunter.Bounty.PlayerId];
            icon.transform.localPosition = bottomLeft + new Vector3(-0.25f, 0f, 0) + offset;
            BountyHunter.CooldownText.transform.localPosition = bottomLeft + new Vector3(-0.25f, 0f, -1f) + offset;
        }

        Func<bool> LastImpostorCouldUse(byte index)
        {
            return () =>
            {
                if (SelectedFunction == 1) return false;

                //　ラストインポスター以外の場合、リソースがない場合はボタンを表示しない
                var p = Helpers.PlayerById(index);
                if (!PlayerIcons.ContainsKey(index) ||
                    !PlayerControl.LocalPlayer.HasModifier(ModifierType.LastImpostor) ||
                    !IsCounterMax())
                {
                    return false;
                }

                // ボタンの位置を変更
                SetButtonPos(index);

                // ボタンにテキストを設定
                LastImpostorButtons[index].ButtonText = PlayerControl.LocalPlayer.IsAlive() ? "生存" : "死亡";

                // アイコンの位置と透明度を変更
                SetIconStatus(index, false);

                // Bounty Hunterの場合賞金首の位置をずらして表示する
                if (PlayerControl.LocalPlayer.IsRole(RoleType.BountyHunter))
                {
                    Vector3 offset = new(0f, 1f, 0f);
                    SetBountyIconPos(offset);
                }

                return PlayerControl.LocalPlayer.CanMove && NumUsed < 1;
            };
        }

        for (byte i = 0; i < 15; i++)
        {
            CustomButton lastImpostorButton = new(
                LastImpostorButtonOnClick(i),
                LastImpostorHasButton(i),
                LastImpostorCouldUse(i),
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

            LastImpostorButtons.Add(lastImpostorButton);
        }
    }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
        KillCounter = 0;
        NumUsed = 0;
        RemainingShots = (int)CustomOptionHolder.LastImpostorNumShots.GetFloat();
        PlayerIcons = [];
    }

    public static bool IsCounterMax()
    {
        if (MaxKillCounter <= KillCounter) return true;
        return false;
    }

    public static bool CanGuess()
    {
        return RemainingShots > 0 && SelectedFunction == 1 && IsCounterMax();
    }

    public static void PromoteToLastImpostor()
    {
        if (!IsEnable) return;

        var impList = new List<PlayerControl>();
        var allPlayers = PlayerControl.AllPlayerControls;
        for (var i = 0; i < allPlayers.Count; i++)
        {
            var p = allPlayers[i];
            if (p.IsTeamImpostor() && p.IsAlive()) impList.Add(p);
        }
        if (impList.Count == 1)
        {
            {
                using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.ImpostorPromotesToLastImpostor);
                sender.Write(impList[0].PlayerId);
            }
            RPCProcedure.ImpostorPromotesToLastImpostor(impList[0].PlayerId);
        }
    }
    public static void Divine(PlayerControl p)
    {
        // FortuneTeller.divine(p, resultIsCrewOrNot);
        string msgBase = "";
        string msgInfo = "";
        Color color = Color.white;

        if (DivineResult == DivineResults.BlackWhite)
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

        else if (DivineResult == DivineResults.Team)
        {
            msgBase = "Option.DivineMessageTeam";
            if (p.IsTeamCrewmate())
            {
                msgInfo = Tr.Get("Option.DivineCrew");
                color = Color.white;
            }
            else if (p.IsNeutral())
            {
                msgInfo = Tr.Get("Option.DivineNeutral");
                color = Color.yellow;
            }
            else
            {
                msgInfo = Tr.Get("Option.DivineImpostor");
                color = Palette.ImpostorRed;
            }
        }

        else if (DivineResult == DivineResults.Role)
        {
            msgBase = "Option.DivineMessageRole";
            var roleInfos = RoleInfo.GetRoleInfoForPlayer(p);
            var sb = new StringBuilder();
            for (var i = 0; i < roleInfos.Count; i++)
            {
                if (i > 0) sb.Append(' ');
                var info = roleInfos[i];
                sb.Append(Helpers.Cs(info.Color, info.Name));
            }
            msgInfo = sb.ToString();
        }

        string msg = string.Format(Tr.Get(msgBase), p.name, msgInfo);
        if (!string.IsNullOrWhiteSpace(msg))
        {
            // TODO: FortuneTeller.fortuneTellerMessage(msg, 5f, color);
        }

        if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(FastDestroyableSingleton<HudManager>.Instance.TaskCompleteSound, false, 0.8f);

        // 占いを実行したことで発火される処理を他クライアントに通知
        {
            using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.FortuneTellerUsedDivine);
            sender.Write(PlayerControl.LocalPlayer.PlayerId);
            sender.Write(p.PlayerId);
        }
        RPCProcedure.FortuneTellerUsedDivine(PlayerControl.LocalPlayer.PlayerId, p.PlayerId);
    }

    public static void OnIntroDestroy(IntroCutscene __instance)
    {
        if (PlayerControl.LocalPlayer != null && FastDestroyableSingleton<HudManager>.Instance != null)
        {
            var allPlayers = PlayerControl.AllPlayerControls;
            for (var i = 0; i < allPlayers.Count; i++)
            {
                var p = allPlayers[i];
                var player = UnityEngine.Object.Instantiate(__instance.PlayerPrefab, FastDestroyableSingleton<HudManager>.Instance.transform);
                player.UpdateFromPlayerOutfit(p.Data.DefaultOutfit, PlayerMaterial.MaskType.ComplexUI, p.Data.IsDead, true);
                player.SetFlipX(true);
                player.cosmetics.currentPet?.gameObject.SetActive(false);
                player.cosmetics.nameText.text = p.Data.DefaultOutfit.PlayerName;
                player.gameObject.SetActive(false);
                PlayerIcons[p.PlayerId] = player;
            }
        }
    }
}