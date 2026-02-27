using Object = UnityEngine.Object;

namespace RebuildUs.Extensions;

internal static class PlayerControlExtensions
{
    private static readonly StringBuilder RoleStringBuilder = new();
    private static readonly StringBuilder InfoStringBuilder = new();
    private static readonly Dictionary<byte, PlayerVoteArea> VoteAreaStates = [];

    private static readonly Vector3 ColorBlindMeetingPos = new(0.3384f, 0.23334f, -0.11f);
    private static readonly Vector3 ColorBlindMeetingScale = new(0.72f, 0.8f, 0.8f);

    private static List<byte> GenerateTasks(int numCommon, int numShort, int numLong)
    {
        if (numCommon + numShort + numLong <= 0)
        {
            numShort = 1;
        }

        Il2CppSystem.Collections.Generic.List<byte> tasks = new();
        Il2CppSystem.Collections.Generic.HashSet<TaskTypes> hashSet = new();

        List<NormalPlayerTask> commonTasks = [.. MapUtilities.CachedShipStatus.CommonTasks];
        commonTasks.Shuffle();

        List<NormalPlayerTask> shortTasks = [.. MapUtilities.CachedShipStatus.ShortTasks];
        shortTasks.Shuffle();

        List<NormalPlayerTask> longTasks = [.. MapUtilities.CachedShipStatus.LongTasks];
        longTasks.Shuffle();

        int start = 0;
        Il2CppSystem.Collections.Generic.List<NormalPlayerTask> commonTasksIl2Cpp = new();
        foreach (NormalPlayerTask t in commonTasks)
        {
            commonTasksIl2Cpp.Add(t);
        }
        MapUtilities.CachedShipStatus.AddTasksFromList(ref start, numCommon, tasks, hashSet, commonTasksIl2Cpp);

        start = 0;
        Il2CppSystem.Collections.Generic.List<NormalPlayerTask> shortTasksIl2Cpp = new();
        foreach (NormalPlayerTask t in shortTasks)
        {
            shortTasksIl2Cpp.Add(t);
        }
        MapUtilities.CachedShipStatus.AddTasksFromList(ref start, numShort, tasks, hashSet, shortTasksIl2Cpp);

        start = 0;
        Il2CppSystem.Collections.Generic.List<NormalPlayerTask> longTasksIl2Cpp = new();
        foreach (NormalPlayerTask t in longTasks)
        {
            longTasksIl2Cpp.Add(t);
        }
        MapUtilities.CachedShipStatus.AddTasksFromList(ref start, numLong, tasks, hashSet, longTasksIl2Cpp);

        return [.. tasks];
    }

    private static TextMeshPro GetOrCreateLabel(TextMeshPro source, string name, float yOffset, float fontScale)
    {
        if (source?.transform?.parent == null)
        {
            return null;
        }

        Transform labelTransform = source.transform.parent.Find(name);
        TextMeshPro label = labelTransform?.GetComponent<TextMeshPro>();

        if (label == null)
        {
            label = Object.Instantiate(source, source.transform.parent);
            if (label == null)
            {
                return null;
            }
            label.transform.localPosition += Vector3.up * yOffset;
            label.fontSize *= fontScale;
            label.gameObject.name = name;
            label.color = label.color.SetAlpha(1f);
        }

        return label;
    }

    extension(PlayerControl player)
    {
        internal void UpdatePlayerInfo()
        {
            if (player?.Data == null)
            {
                return;
            }

            MeetingHud meeting = MeetingHud.Instance;
            bool hasMeeting = meeting?.playerStates != null;
            if (hasMeeting)
            {
                VoteAreaStates.Clear();
                foreach (PlayerVoteArea s in meeting.playerStates)
                {
                    if (s != null)
                    {
                        VoteAreaStates[s.TargetPlayerId] = s;
                    }
                }
            }

            foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (p?.Data == null || p.cosmetics == null)
                {
                    continue;
                }

                PlayerVoteArea pva = null;
                if (hasMeeting)
                {
                    VoteAreaStates.TryGetValue(p.PlayerId, out pva);
                }

                // Colorblind Text Handling
                if (pva?.ColorBlindName != null && pva.ColorBlindName.gameObject.active)
                {
                    pva.ColorBlindName.transform.localPosition = ColorBlindMeetingPos;
                    pva.ColorBlindName.transform.localScale = ColorBlindMeetingScale;
                }

                if (p.cosmetics.colorBlindText != null && p.cosmetics.showColorBlindText && p.cosmetics.colorBlindText.gameObject.active)
                {
                    p.cosmetics.colorBlindText.transform.localPosition = new(0, -0.25f, 0f);
                }

                p.cosmetics.nameText?.transform.parent?.SetLocalZ(-0.0001f);

                if (p == player || player.Data.IsDead)
                {
                    TextMeshPro label = GetOrCreateLabel(p.cosmetics.nameText, "Info", 0.225f, 0.75f);
                    if (label == null)
                    {
                        continue;
                    }

                    TextMeshPro meetingLabel = null;
                    if (pva != null)
                    {
                        meetingLabel = GetOrCreateLabel(pva.NameText, "Info", -0.2f, 0.60f);
                        if (meetingLabel != null && pva.NameText != null)
                        {
                            pva.NameText.transform.localPosition = new(0.3384f, 0.0311f, -0.1f);
                        }
                    }

                    (int completed, int total) = TasksHandler.TaskInfo(p.Data);
                    string roleBase = RoleInfo.GetRolesString(p, true, false);
                    string roleGhost = RoleInfo.GetRolesString(p, true, MapSettings.GhostsSeeModifier);

                    string statusText = "";
                    if (p == player || player.Data.IsDead && MapSettings.GhostsSeeInformation)
                    {
                        if (p.IsRole(RoleType.Arsonist))
                        {
                            Arsonist role = Arsonist.Instance;
                            if (role != null)
                            {
                                int dousedSurvivors = 0;
                                foreach (PlayerControl dousedPlayer in role.DousedPlayers)
                                {
                                    if (dousedPlayer?.Data != null && !dousedPlayer.Data.IsDead && !dousedPlayer.Data.Disconnected)
                                    {
                                        dousedSurvivors++;
                                    }
                                }

                                int totalSurvivors = 0;
                                foreach (PlayerControl targetPlayer in PlayerControl.AllPlayerControls.GetFastEnumerator())
                                {
                                    if (targetPlayer?.Data != null
                                        && !targetPlayer.Data.IsDead
                                        && !targetPlayer.Data.Disconnected
                                        && !targetPlayer.IsRole(RoleType.Arsonist)
                                        && !targetPlayer.IsGm())
                                    {
                                        totalSurvivors++;
                                    }
                                }

                                statusText = Helpers.Cs(Arsonist.NameColor, $" ({dousedSurvivors}/{totalSurvivors})");
                            }
                        }
                        else if (p.IsRole(RoleType.Vulture))
                        {
                            Vulture role = Vulture.Instance;
                            if (role != null)
                            {
                                statusText = Helpers.Cs(Vulture.NameColor, $" ({role.EatenBodies}/{Vulture.NumberToWin})");
                            }
                        }
                    }

                    string taskText = "";
                    if (total > 0)
                    {
                        InfoStringBuilder.Clear();

                        bool commsActive = false;
                        if (MapUtilities.CachedShipStatus != null && MapUtilities.Systems.TryGetValue(SystemTypes.Comms, out UnityObject comms))
                        {
                            IActivatable activatable = comms.TryCast<IActivatable>();
                            if (activatable != null)
                            {
                                commsActive = activatable.IsActive;
                            }
                        }

                        if (commsActive)
                        {
                            InfoStringBuilder.Append("<color=#808080FF>(?/?)</color>");
                        }
                        else
                        {
                            string color = completed == total ? "#00FF00FF" : "#FAD934FF";
                            InfoStringBuilder
                                .Append("<color=")
                                .Append(color)
                                .Append(">(")
                                .Append(completed)
                                .Append('/')
                                .Append(total)
                                .Append(")</color>");
                        }

                        taskText = InfoStringBuilder.ToString();
                    }

                    string pInfo = "";
                    string mInfo = "";
                    if (p == player)
                    {
                        string roles = (p.Data.IsDead ? roleGhost : roleBase) + statusText;
                        if (p.IsRole(RoleType.NiceSwapper))
                        {
                            InfoStringBuilder.Clear();
                            InfoStringBuilder.Append(roles).Append("<color=#FAD934FF> (").Append(Swapper.RemainSwaps).Append(")</color>");
                            pInfo = InfoStringBuilder.ToString();
                        }
                        else
                        {
                            if (p.IsTeamCrewmate() || p.IsRole(RoleType.Arsonist) || p.IsRole(RoleType.Vulture))
                            {
                                InfoStringBuilder.Clear();
                                InfoStringBuilder.Append(roles).Append(' ').Append(taskText);
                                pInfo = InfoStringBuilder.ToString();
                            }
                            else
                            {
                                pInfo = roles;
                            }
                        }

                        if (HudManager.Instance?.TaskPanel?.tab != null)
                        {
                            Transform tabTextObj = HudManager.Instance.TaskPanel.tab.transform.Find("TabText_TMP");
                            if (tabTextObj != null)
                            {
                                TextMeshPro tabText = tabTextObj.GetComponent<TextMeshPro>();
                                if (tabText != null)
                                {
                                    InfoStringBuilder.Clear();
                                    InfoStringBuilder
                                        .Append(TranslationController.Instance.GetString(StringNames.Tasks))
                                        .Append(' ')
                                        .Append(taskText);
                                    tabText.SetText(InfoStringBuilder.ToString());
                                }
                            }
                        }

                        InfoStringBuilder.Clear();
                        InfoStringBuilder.Append(roles).Append(' ').Append(taskText);
                        mInfo = InfoStringBuilder.ToString().Trim();
                    }
                    else if (MapSettings.GhostsSeeRoles && MapSettings.GhostsSeeInformation)
                    {
                        InfoStringBuilder.Clear();
                        InfoStringBuilder.Append(roleGhost).Append(statusText).Append(' ').Append(taskText);
                        pInfo = InfoStringBuilder.ToString().Trim();
                        mInfo = pInfo;
                    }
                    else if (MapSettings.GhostsSeeInformation)
                    {
                        InfoStringBuilder.Clear();
                        InfoStringBuilder.Append(taskText).Append(statusText);
                        pInfo = InfoStringBuilder.ToString().Trim();
                        mInfo = pInfo;
                    }
                    else if (MapSettings.GhostsSeeRoles)
                    {
                        pInfo = roleGhost;
                        mInfo = pInfo;
                    }

                    label.text = pInfo;
                    label.gameObject.SetActive(p.Visible);
                    meetingLabel?.text = meeting != null && meeting.state == MeetingHud.VoteStates.Results ? "" : mInfo;
                }
            }
        }

        internal void RefreshRoleDescription()
        {
            if (player == null)
            {
                return;
            }

            List<RoleInfo> infos = RoleInfo.GetRoleInfoForPlayer(player);
            List<PlayerTask> toRemove = [];

            foreach (PlayerTask t in player.myTasks.GetFastEnumerator())
            {
                ImportantTextTask textTask = t.TryCast<ImportantTextTask>();
                if (textTask == null)
                {
                    continue;
                }
                bool found = false;
                for (int i = 0; i < infos.Count; i++)
                {
                    if (!textTask.Text.StartsWith(infos[i].Name))
                    {
                        continue;
                    }
                    infos.RemoveAt(i);
                    found = true;
                    break;
                }

                if (!found)
                {
                    toRemove.Add(t);
                }
            }

            foreach (PlayerTask t in toRemove)
            {
                t.OnRemove();
                player.myTasks.Remove(t);
                Object.Destroy(t.gameObject);
            }

            foreach (RoleInfo roleInfo in infos)
            {
                ImportantTextTask task = new GameObject("RoleTask").AddComponent<ImportantTextTask>();
                task.transform.SetParent(player.transform, false);

                InfoStringBuilder.Clear();
                InfoStringBuilder.Append(roleInfo.Name).Append(": ");
                if (roleInfo.RoleType == RoleType.Jackal)
                {
                    InfoStringBuilder.Append(Jackal.CanCreateSidekick ? Tr.Get(TrKey.JackalWithSidekick) : Tr.Get(TrKey.JackalShortDesc));
                }
                else
                {
                    InfoStringBuilder.Append(roleInfo.ShortDescription);
                }

                task.Text = Helpers.Cs(roleInfo.Color, InfoStringBuilder.ToString());
                player.myTasks.Insert(0, task);
            }

            if (player.HasModifier(ModifierType.Madmate) || player.HasModifier(ModifierType.CreatedMadmate))
            {
                ImportantTextTask task = new GameObject("RoleTask").AddComponent<ImportantTextTask>();
                task.transform.SetParent(player.transform, false);

                InfoStringBuilder.Clear();
                InfoStringBuilder.Append(Madmate.FullName).Append(": ").Append(Tr.Get(TrKey.MadmateShortDesc));
                task.Text = Helpers.Cs(Madmate.NameColor, InfoStringBuilder.ToString());
                player.myTasks.Insert(0, task);
            }
        }

        internal void SetPetVisibility()
        {
            bool localDead = player.Data.IsDead;
            foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                p.cosmetics.SetPetVisible(localDead && !p.Data.IsDead || !localDead);
            }
        }

        internal void GenerateAndAssignTasks(int numCommon, int numShort, int numLong)
        {
            if (player == null)
            {
                return;
            }

            List<byte> taskTypeIds = GenerateTasks(numCommon, numShort, numLong);
            using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.UncheckedSetTasks);
            sender.Write(player.PlayerId);
            sender.WriteBytesAndSize(taskTypeIds.ToArray());
            RPCProcedure.UncheckedSetTasks(player.PlayerId, [.. taskTypeIds]);
        }

        internal void ClearAllTasks()
        {
            if (player == null)
            {
                return;
            }
            foreach (PlayerTask t in player.myTasks)
            {
                t.OnRemove();
                Object.Destroy(t.gameObject);
            }

            player.myTasks.Clear();
            player.Data?.Tasks?.Clear();
        }

        internal void SetKillTimerUnchecked(float time, float max = float.NegativeInfinity)
        {
            if (float.IsNegativeInfinity(max))
            {
                max = time;
            }
            player.killTimer = time;
            FastDestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(time, max);
        }

        internal bool HasAliveKillingLover()
        {
            return Lovers.ExistingAndAlive(player) && Lovers.ExistingWithKiller(player) && player != null && player.IsLovers();
        }

        internal bool IsDead()
        {
            if (player == null)
            {
                return true;
            }
            NetworkedPlayerInfo data = player.Data;
            if (data == null || data.IsDead || data.Disconnected)
            {
                return true;
            }

            return GameHistory.FinalStatuses != null
                   && GameHistory.FinalStatuses.TryGetValue(player.PlayerId, out FinalStatus status)
                   && status != FinalStatus.Alive;
        }

        internal bool IsAlive()
        {
            return !player.IsDead();
        }

        internal bool IsTeamCrewmate()
        {
            return player != null && !player.IsTeamImpostor() && !player.IsNeutral() && !player.IsGm();
        }

        internal bool IsTeamImpostor()
        {
            return player?.Data?.Role != null && player.Data.Role.IsImpostor;
        }

        internal bool IsGm()
        {
            return false;
            // return GM.gm != null && player == GM.gm;
        }

        internal bool IsLovers()
        {
            return player != null && Lovers.IsLovers(player);
        }

        internal PlayerControl GetPartner()
        {
            return Lovers.GetPartner(player);
        }

        internal bool CanBeErased()
        {
            return !player.IsRole(RoleType.Jackal) && !player.IsRole(RoleType.Sidekick) && !Jackal.FormerJackals.Contains(player);
        }

        internal ClientData GetClient()
        {
            if (player == null)
            {
                return null;
            }
            foreach (ClientData cd in AmongUsClient.Instance.allClients.GetFastEnumerator())
            {
                if (cd?.Character != null && cd.Character.PlayerId == player.PlayerId)
                {
                    return cd;
                }
            }

            return null;
        }

        internal string GetPlatform()
        {
            ClientData client = player.GetClient();
            return client != null ? client.PlatformData.Platform.ToString() : "Unknown";
        }

        internal string GetRoleName()
        {
            return RoleInfo.GetRolesString(player, false, joinSeparator: " + ");
        }

        internal string GetNameWithRole()
        {
            if (player == null || player.Data == null)
            {
                return "";
            }
            RoleStringBuilder.Clear();
            string name = player.Data.PlayerName;
            if (Camouflager.CamouflageTimer > 0f)
            {
                if (string.IsNullOrEmpty(name))
                {
                    name = player.Data.DefaultOutfit.PlayerName;
                }
            }
            else if (player.CurrentOutfitType == PlayerOutfitType.Shapeshifted)
            {
                name = $"{player.CurrentOutfit.PlayerName} ({player.Data.DefaultOutfit.PlayerName})";
            }

            RoleStringBuilder.Append(name).Append(" (").Append(player.GetRoleName()).Append(')');
            return RoleStringBuilder.ToString();
        }

        internal void MurderPlayer(PlayerControl target)
        {
            player.MurderPlayer(target, MurderResultFlags.Succeeded);
        }

        internal void SetBasePlayerOutlines()
        {
            foreach (PlayerControl target in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (target?.cosmetics?.currentBodySprite?.BodySprite == null)
                {
                    continue;
                }

                bool isMorphedMorphing = target.IsRole(RoleType.Morphing) && Morphing.MorphTarget != null && Morphing.MorphTimer > 0f;
                bool hasVisibleShield = false;
                if (Camouflager.CamouflageTimer <= 0f
                    && Medic.Shielded != null
                    && (target == Medic.Shielded && !isMorphedMorphing || isMorphedMorphing && Morphing.MorphTarget == Medic.Shielded))
                {
                    hasVisibleShield = Medic.ShowShielded switch
                    {
                        0 => true, // Everyone
                        1 => player == Medic.Shielded || player.IsRole(RoleType.Medic), // Shielded + Medic
                        2 => player.IsRole(RoleType.Medic), // Medic only
                        _ => false,
                    };
                }

                Material mat = target.cosmetics.currentBodySprite.BodySprite.material;
                if (hasVisibleShield)
                {
                    mat.SetFloat("_Outline", 1f);
                    mat.SetColor("_OutlineColor", Medic.ShieldedColor);
                }
                else
                {
                    mat.SetFloat("_Outline", 0f);
                }
            }
        }
    }
}