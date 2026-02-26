namespace RebuildUs.Roles.Modifier;

[HarmonyPatch]
internal class LastImpostor : ModifierBase<LastImpostor>
{
    internal static Color NameColor = Palette.ImpostorRed;
    internal static int KillCounter;
    internal static int NumUsed;
    internal static int RemainingShots;

    internal static List<CustomButton> LastImpostorButtons = [];
    private static Dictionary<byte, PoolablePlayer> _playerIcons = [];

    public LastImpostor()
    {
        // write value init here
        StaticModifierType = CurrentModifierType = ModifierType.LastImpostor;
    }

    internal override Color ModifierColor
    {
        get => NameColor;
    }

    internal static bool IsEnable { get => CustomOptionHolder.LastImpostorEnable.GetBool(); }
    internal static int MaxKillCounter { get => (int)CustomOptionHolder.LastImpostorNumKills.GetFloat(); }
    internal static int SelectedFunction { get => CustomOptionHolder.LastImpostorFunctions.GetSelection(); }
    internal static DivineResults DivineResult { get => (DivineResults)CustomOptionHolder.LastImpostorResults.GetSelection(); }

    internal static string Postfix
    {
        get => Tr.Get(TrKey.LastImpostorPostfix);
    }

    internal static string FullName
    {
        get => Tr.Get(TrKey.LastImpostor);
    }

    internal override void OnMeetingStart() { }
    internal override void OnMeetingEnd() { }
    internal override void OnIntroEnd() { }
    internal override void FixedUpdate() { }

    internal override void OnKill(PlayerControl target)
    {
        KillCounter += 1;
    }

    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    internal static void MakeButtons(HudManager hm)
    {
        LastImpostorButtons = [];

        Vector3 LastImpostorCalcPos(byte index)
        {
            //return new Vector3(-0.25f, -0.25f, 0) + Vector3.right * index * 0.55f;
            return new Vector3(-0.25f, -0.15f, 0) + (Vector3.right * index * 0.55f);
        }

        Action LastImpostorButtonOnClick(byte index)
        {
            return () =>
            {
                if (SelectedFunction == 1) return;
                PlayerControl p = Helpers.PlayerById(index);
                Divine(p);
            };
        }

        ;

        Func<bool> LastImpostorHasButton(byte index)
        {
            return () =>
            {
                if (SelectedFunction == 1) return false;
                PlayerControl p = PlayerControl.LocalPlayer;
                if (!p.HasModifier(ModifierType.LastImpostor)) return false;
                if (p.HasModifier(ModifierType.LastImpostor) && p.CanMove && p.IsAlive() & (p.PlayerId != index) && MapSettings.PlayerIcons.ContainsKey(index) && NumUsed < 1 && IsCounterMax()) return true;

                if (_playerIcons.ContainsKey(index))
                {
                    _playerIcons[index].gameObject.SetActive(false);
                    if (PlayerControl.LocalPlayer.IsRole(RoleType.BountyHunter))
                        SetBountyIconPos(Vector3.zero);
                }

                if (LastImpostorButtons.Count > index) LastImpostorButtons[index].SetActive(false);

                return false;
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
                _playerIcons[index].transform.localPosition = iconBase + pos;
            }
        }

        void SetIconStatus(byte index, bool transparent)
        {
            _playerIcons[index].transform.localScale = Vector3.one * 0.25f;
            _playerIcons[index].gameObject.SetActive(PlayerControl.LocalPlayer.CanMove);
            _playerIcons[index].SetSemiTransparent(transparent);
        }

        void SetBountyIconPos(Vector3 offset)
        {
            Vector3 bottomLeft = AspectPosition.ComputePosition(AspectPosition.EdgeAlignments.LeftBottom, new(0.9f, 0.7f, -10f));
            PoolablePlayer icon = MapSettings.PlayerIcons[BountyHunter.Bounty.PlayerId];
            icon.transform.localPosition = bottomLeft + new Vector3(-0.25f, 0f, 0) + offset;
            BountyHunter.CooldownText.transform.localPosition = bottomLeft + new Vector3(-0.25f, 0f, -0.1f) + offset;
        }

        Func<bool> LastImpostorCouldUse(byte index)
        {
            return () =>
            {
                if (SelectedFunction == 1) return false;

                //　ラストインポスター以外の場合、リソースがない場合はボタンを表示しない
                PlayerControl p = Helpers.PlayerById(index);
                if (!_playerIcons.ContainsKey(index) || !PlayerControl.LocalPlayer.HasModifier(ModifierType.LastImpostor) || !IsCounterMax()) return false;

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
            CustomButton lastImpostorButton = new(LastImpostorButtonOnClick(i), LastImpostorHasButton(i), LastImpostorCouldUse(i), () => { }, null, Vector3.zero, hm, null, KeyCode.None, true)
            {
                Timer = 0.0f,
                MaxTimer = 0.0f,
            };

            LastImpostorButtons.Add(lastImpostorButton);
        }
    }

    // write functions here

    internal static void Clear()
    {
        // reset configs here
        Players.Clear();
        KillCounter = 0;
        NumUsed = 0;
        RemainingShots = (int)CustomOptionHolder.LastImpostorNumShots.GetFloat();
        _playerIcons = [];
    }

    internal static bool IsCounterMax()
    {
        if (MaxKillCounter <= KillCounter) return true;
        return false;
    }

    internal static bool CanGuess()
    {
        return RemainingShots > 0 && SelectedFunction == 1 && IsCounterMax();
    }

    internal static void PromoteToLastImpostor()
    {
        if (!IsEnable) return;

        List<PlayerControl> impList = [];
        foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p.IsTeamImpostor() && p.IsAlive())
                impList.Add(p);
        }

        if (impList.Count == 1)
        {
            {
                using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.ImpostorPromotesToLastImpostor);
                sender.Write(impList[0].PlayerId);
            }
            RPCProcedure.ImpostorPromotesToLastImpostor(impList[0].PlayerId);
        }
    }

    internal static void Divine(PlayerControl p)
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
            msgBase = "DivineMessageTeam";
            if (p.IsTeamCrewmate())
            {
                msgInfo = Tr.Get(TrKey.DivineCrew);
                color = Color.white;
            }
            else if (p.IsNeutral())
            {
                msgInfo = Tr.Get(TrKey.DivineNeutral);
                color = Color.yellow;
            }
            else
            {
                msgInfo = Tr.Get(TrKey.DivineImpostor);
                color = Palette.ImpostorRed;
            }
        }

        else if (DivineResult == DivineResults.Role)
        {
            msgBase = "DivineMessageRole";
            List<RoleInfo> roleInfos = RoleInfo.GetRoleInfoForPlayer(p);
            StringBuilder sb = new();
            for (int i = 0; i < roleInfos.Count; i++)
            {
                if (i > 0) sb.Append(' ');
                RoleInfo info = roleInfos[i];
                sb.Append(Helpers.Cs(info.Color, info.Name));
            }

            msgInfo = sb.ToString();
        }

        string msg = string.Format(Tr.GetDynamic(msgBase), p.name, msgInfo);
        if (!string.IsNullOrWhiteSpace(msg))
        {
            // TODO: FortuneTeller.fortuneTellerMessage(msg, 5f, color);
        }

        if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(FastDestroyableSingleton<HudManager>.Instance.TaskCompleteSound, false, 0.8f);

        // 占いを実行したことで発火される処理を他クライアントに通知
        {
            using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.FortuneTellerUsedDivine);
            sender.Write(PlayerControl.LocalPlayer.PlayerId);
            sender.Write(p.PlayerId);
        }
        RPCProcedure.FortuneTellerUsedDivine(PlayerControl.LocalPlayer.PlayerId, p.PlayerId);
    }

    internal static void OnIntroDestroy(IntroCutscene __instance)
    {
        if (PlayerControl.LocalPlayer != null && FastDestroyableSingleton<HudManager>.Instance != null)
        {
            foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                PoolablePlayer player = UnityObject.Instantiate(__instance.PlayerPrefab, FastDestroyableSingleton<HudManager>.Instance.transform);
                player.UpdateFromPlayerOutfit(p.Data.DefaultOutfit, PlayerMaterial.MaskType.ComplexUI, p.Data.IsDead, true);
                player.SetFlipX(true);
                player.cosmetics.currentPet?.gameObject.SetActive(false);
                player.cosmetics.nameText.text = p.Data.DefaultOutfit.PlayerName;
                player.gameObject.SetActive(false);
                _playerIcons[p.PlayerId] = player;
            }
        }
    }

    internal enum DivineResults
    {
        BlackWhite,
        Team,
        Role,
    }
}