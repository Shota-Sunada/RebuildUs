using System.Collections;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using PowerTools;
using Object = UnityEngine.Object;

namespace RebuildUs.Modules;

internal static class Intro
{
    internal static void GenerateMiniCrewIcons(IntroCutscene __instance)
    {
        try
        {
            if (__instance == null) return;

            HudManager hud = FastDestroyableSingleton<HudManager>.Instance;
            ShipStatus ship = MapUtilities.CachedShipStatus ?? ShipStatus.Instance;
            PlayerControl local = PlayerControl.LocalPlayer;

            if (local != null && hud != null && __instance.PlayerPrefab != null && PlayerControl.AllPlayerControls != null)
            {
                var bottomLeft = AspectPosition.ComputePosition(AspectPosition.EdgeAlignments.LeftBottom, new(0.9f, 0.7f, -10f));

                foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                {
                    if (p?.Data == null) continue;
                    NetworkedPlayerInfo data = p.Data;
                    PoolablePlayer player = Object.Instantiate(__instance.PlayerPrefab, hud.transform);
                    if (player == null) continue;

                    player.UpdateFromPlayerData(data, p.CurrentOutfitType, PlayerMaterial.MaskType.None, false);
                    player.cosmetics.nameText.text = data.PlayerName;
                    player.cosmetics.nameText.transform.localPosition = new(0f, -0.7f, -0.1f);
                    player.SetFlipX(true);
                    MapSettings.PlayerIcons[p.PlayerId] = player;
                    player.gameObject.SetActive(false);

                    // UIレイヤーに設定
                    player.gameObject.layer = 5;
                    foreach (Transform child in player.GetComponentsInChildren<Transform>(true)) child.gameObject.layer = 5;

                    // Allows the roles to have the correct position and scaling via their own UpdateIcons
                    player.transform.localPosition = bottomLeft;
                    player.transform.localScale = Vector3.one * 0.3f;
                }
            }

            RebuildUs.OnIntroEnd();

            // インポスター視界の場合に昇降機右の影を無効化
            if (Helpers.IsAirship
                && local != null
                && ship?.FastRooms != null
                && CustomOptionHolder.AirshipOptimize.GetBool()
                && Helpers.HasImpostorVision(local)
                && ship.FastRooms.TryGetValue(SystemTypes.GapRoom, out PlainShipRoom gapRoomForShadow)
                && gapRoomForShadow != null)
            {
                GameObject obj = gapRoomForShadow.gameObject;
                Transform shadow = obj?.transform?.FindChild("Shadow");
                OneWayShadows oneWayShadow = shadow?.FindChild("LedgeShadow")?.GetComponent<OneWayShadows>();
                oneWayShadow?.gameObject.SetActive(false);
            }

            if (ship != null)
            {
                // ベントを追加する
                AdditionalVents.AddAdditionalVents();

                // スペシメンにバイタルを移動する
                SpecimenVital.MoveVital();
            }

            // アーカイブのアドミンを消す
            if (Helpers.IsAirship
                && ship?.FastRooms != null
                && CustomOptionHolder.AirshipOldAdmin.GetBool()
                && ship.FastRooms.TryGetValue(SystemTypes.Records, out PlainShipRoom recordsRoom)
                && recordsRoom != null)
            {
                GameObject records = recordsRoom.gameObject;
                foreach (MapConsole console in records.GetComponentsInChildren<MapConsole>())
                {
                    if (console.name != "records_admin_map") continue;
                    console.gameObject.SetActive(false);
                    break;
                }
            }

            if (ship?.FastRooms != null && ship.FastRooms.TryGetValue(SystemTypes.GapRoom, out PlainShipRoom gapRoomData) && gapRoomData != null)
            {
                GameObject gapRoom = gapRoomData.gameObject;
                // GapRoomの配電盤を消す
                if (Helpers.IsAirship && CustomOptionHolder.AirshipDisableGapSwitchBoard.GetBool() && gapRoom != null)
                {
                    GameObject sabotage = null;
                    foreach (Console console in gapRoom.GetComponentsInChildren<Console>())
                    {
                        if (console.name != "task_lightssabotage (gap)") continue;
                        sabotage = console.gameObject;
                        break;
                    }

                    if (sabotage != null)
                    {
                        sabotage.SetActive(false);
                        Console sabotageConsole = sabotage.GetComponent<Console>();
                        List<Console> newConsoles = new();
                        if (ship.AllConsoles != null)
                        {
                            foreach (Console c in ship.AllConsoles)
                            {
                                if (c != sabotageConsole)
                                    newConsoles.Add(c);
                            }

                            ship.AllConsoles = newConsoles.ToArray();
                        }
                    }
                }

                // ぬ～んを消す
                if (Helpers.IsAirship && CustomOptionHolder.AirshipDisableMovingPlatform.GetBool() && gapRoom != null)
                {
                    MovingPlatformBehaviour movingPlatform = gapRoom.GetComponentInChildren<MovingPlatformBehaviour>();
                    movingPlatform?.gameObject.SetActive(false);
                    foreach (PlatformConsole obj in gapRoom.GetComponentsInChildren<PlatformConsole>()) obj.gameObject.SetActive(false);
                }
            }

            // タスクバグ修正
            if (Helpers.IsAirship && CustomOptionHolder.AirshipEnableWallCheck.GetBool())
            {
                foreach (Console x in Object.FindObjectsOfType<Console>())
                {
                    if (x.name == "task_garbage1"
                        || x.name == "task_garbage2"
                        || x.name == "task_garbage3"
                        || x.name == "task_garbage4"
                        || x.name == "task_garbage5"
                        || x.name == "task_shower"
                        || x.name == "task_developphotos"
                        || (x.name == "DivertRecieve" && x.Room is SystemTypes.Armory or SystemTypes.MainHall)) x.checkWalls = true;
                }
            }

            // 最初から一人の場合はLast Impostorになる
            if (AmongUsClient.Instance?.AmHost == true) LastImpostor.PromoteToLastImpostor();

            // タスクパネルの表示優先度を上げる
            GameObject taskPanel = hud?.TaskStuff;
            if (taskPanel != null)
            {
                Vector3 pos = taskPanel.transform.position;
                taskPanel.transform.position = new(pos.x, pos.y, -20);
            }

            // マップデータのコピーを読み込み
            if (CustomOptionHolder.AirshipReplaceSafeTask.GetBool() && AmongUsClient.Instance != null) MapData.LoadAssets(AmongUsClient.Instance);
        }
        catch (Exception ex)
        {
            Logger.LogError("Intro.GenerateMiniCrewIcons failed");
            Logger.LogError(ex);
        }
    }

    internal static void SetupIntroTeamIcons(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
    {
        // Intro solo teams
        if (PlayerControl.LocalPlayer.IsNeutral())
        {
            Il2CppSystem.Collections.Generic.List<PlayerControl> soloTeam = new();
            soloTeam.Add(PlayerControl.LocalPlayer);
            yourTeam = soloTeam;
        }

        // Add the Spy to the Impostor team (for the Impostors)
        if (!Spy.Exists || !PlayerControl.LocalPlayer.IsTeamImpostor()) return;
        List<PlayerControl> players = new();
        foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator()) players.Add(p);
        players.Shuffle();

        Il2CppSystem.Collections.Generic.List<PlayerControl> fakeImpostorTeam = new(); // The local player always has to be the first one in the list (to be displayed in the center)
        fakeImpostorTeam.Add(PlayerControl.LocalPlayer);

        foreach (PlayerControl p in players)
        {
            if (PlayerControl.LocalPlayer != p && (p.IsRole(RoleType.Spy) || p.Data.Role.IsImpostor))
                fakeImpostorTeam.Add(p);
        }

        yourTeam = fakeImpostorTeam;
    }

    internal static void SetupIntroTeam(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
    {
        List<RoleInfo> infos = RoleInfo.GetRoleInfoForPlayer(PlayerControl.LocalPlayer);
        RoleInfo roleInfo = infos.FirstOrDefault(info => info.RoleType != RoleType.Lovers);
        if (roleInfo == null) return;
        // if (PlayerControl.LocalPlayer.IsNeutral() || PlayerControl.LocalPlayer.IsGM())
        if (!PlayerControl.LocalPlayer.IsNeutral()) return;
        __instance.BackgroundBar.material.color = roleInfo.Color;
        __instance.TeamTitle.text = roleInfo.Name;
        __instance.TeamTitle.color = roleInfo.Color;
        __instance.ImpostorText.text = "";
    }

    internal static IEnumerator CoBegin(IntroCutscene __instance)
    {
        if (__instance == null)
        {
            Logger.LogError("IntroCutscene :: CoBegin() aborted: __instance is null");
            yield break;
        }

        yield return WaitRoleAssign().WrapToIl2Cpp();
        yield return WaitForIntroPrerequisites().WrapToIl2Cpp();
        if (PlayerControl.LocalPlayer == null)
        {
            Logger.LogError("IntroCutscene :: CoBegin() aborted: LocalPlayer is null");
            yield break;
        }

        // yield return __instance.CoBegin();

        Logger.LogInfo("IntroCutscene :: CoBegin() :: Starting intro cutscene");
        SoundManager.Instance?.PlaySound(__instance.IntroStinger, false);

        if (Helpers.IsNormal)
        {
            Logger.LogInfo("IntroCutscene :: CoBegin() :: Game Mode: Normal");
            __instance.LogPlayerRoleData();
            __instance.HideAndSeekPanels.SetActive(false);
            __instance.CrewmateRules.SetActive(false);
            __instance.ImpostorRules.SetActive(false);
            __instance.ImpostorName.gameObject.SetActive(false);
            __instance.ImpostorTitle.gameObject.SetActive(false);
            Il2CppSystem.Collections.Generic.List<PlayerControl> show = IntroCutscene.SelectTeamToShow((Func<NetworkedPlayerInfo, bool>)(pcd => !PlayerControl.LocalPlayer.IsTeamImpostor() || pcd.Role.TeamType == PlayerControl.LocalPlayer.Data.Role.TeamType));
            if (show == null || show.Count < 1)
            {
                Logger.LogError("IntroCutscene :: CoBegin() :: teamToShow is EMPTY or NULL");
                show = new();
                show.Add(PlayerControl.LocalPlayer);
            }

            if (PlayerControl.LocalPlayer.IsTeamImpostor())
                __instance.ImpostorText.gameObject.SetActive(false);
            else
            {
                int adjustedNumImpostors = 1;
                if (GameManager.Instance?.LogicOptions != null && GameData.Instance != null)
                    adjustedNumImpostors = GameManager.Instance.LogicOptions.GetAdjustedNumImpostors(GameData.Instance.PlayerCount);
                __instance.ImpostorText.text = adjustedNumImpostors == 1 ? DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.NumImpostorsS) : DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.NumImpostorsP, adjustedNumImpostors);
                __instance.ImpostorText.text = __instance.ImpostorText.text.Replace("[FF1919FF]", "<color=#FF1919FF>");
                __instance.ImpostorText.text = __instance.ImpostorText.text.Replace("[]", "</color>");
            }

            yield return __instance.ShowTeam(show, 3f);
            // 独自処理挿入
            yield return SetupRole(__instance);
        }
        else
        {
            Logger.LogInfo("IntroCutscene :: CoBegin() :: Game Mode: Hide and Seek");
            __instance.LogPlayerRoleData();
            __instance.HideAndSeekPanels.SetActive(true);
            if (PlayerControl.LocalPlayer.IsTeamImpostor())
            {
                __instance.CrewmateRules.SetActive(false);
                __instance.ImpostorRules.SetActive(true);
            }
            else
            {
                __instance.CrewmateRules.SetActive(true);
                __instance.ImpostorRules.SetActive(false);
            }

            Il2CppSystem.Collections.Generic.List<PlayerControl> show = IntroCutscene.SelectTeamToShow((Func<NetworkedPlayerInfo, bool>)(pcd => PlayerControl.LocalPlayer.IsTeamImpostor() != pcd.Role.IsImpostor));
            if (show == null || show.Count < 1)
            {
                Logger.LogError("IntroCutscene :: CoBegin() :: teamToShow is EMPTY or NULL");
                show = new();
                show.Add(PlayerControl.LocalPlayer);
            }

            PlayerControl impostor = PlayerControl.AllPlayerControls.Find((Il2CppSystem.Predicate<PlayerControl>)(pc => pc != null && pc.Data != null && pc.Data.Role.IsImpostor));
            if (impostor == null) Logger.LogError("IntroCutscene :: CoBegin() :: impostor is NULL");

            if (impostor != null) GameManager.Instance?.SetSpecialCosmetics(impostor);
            __instance.ImpostorName.gameObject.SetActive(true);
            __instance.ImpostorTitle.gameObject.SetActive(true);
            __instance.BackgroundBar.enabled = false;
            __instance.TeamTitle.gameObject.SetActive(false);
            __instance.ImpostorName.text = impostor != null ? impostor.Data.PlayerName : "???";
            yield return new WaitForSecondsRealtime(0.1f);
            PoolablePlayer playerSlot = null;
            if (impostor != null)
            {
                playerSlot = __instance.CreatePlayer(1, 1, impostor.Data, false);
                playerSlot.SetBodyType(PlayerBodyTypes.Normal);
                playerSlot.SetFlipX(false);
                playerSlot.transform.localPosition = __instance.impostorPos;
                playerSlot.transform.localScale = Vector3.one * __instance.impostorScale;
            }

            if (MapUtilities.CachedShipStatus?.CosmeticsCache != null)
                yield return MapUtilities.CachedShipStatus.CosmeticsCache.PopulateFromPlayers();
            yield return new WaitForSecondsRealtime(6f);

            playerSlot?.gameObject.SetActive(false);

            __instance.HideAndSeekPanels.SetActive(false);
            __instance.CrewmateRules.SetActive(false);
            __instance.ImpostorRules.SetActive(false);
            LogicOptionsHnS logicOptions = GameManager.Instance.LogicOptions as LogicOptionsHnS;
            if (GameManager.Instance.GetLogicComponent<LogicHnSMusic>() is LogicHnSMusic logicComponent)
                logicComponent.StartMusicWithIntro();
            if (PlayerControl.LocalPlayer.IsTeamImpostor())
            {
                if (logicOptions != null)
                {
                    float crewmateLeadTime = logicOptions.GetCrewmateLeadTime();
                    __instance.HideAndSeekTimerText.gameObject.SetActive(true);
                    PoolablePlayer poolablePlayer;
                    AnimationClip anim;
                    if (AprilFoolsMode.ShouldHorseAround())
                    {
                        poolablePlayer = __instance.HorseWrangleVisualSuit;
                        poolablePlayer.gameObject.SetActive(true);
                        poolablePlayer.SetBodyType(PlayerBodyTypes.Seeker);
                        anim = __instance.HnSSeekerSpawnHorseAnim;
                        __instance.HorseWrangleVisualPlayer.SetBodyType(PlayerBodyTypes.Normal);
                        __instance.HorseWrangleVisualPlayer.UpdateFromPlayerData(PlayerControl.LocalPlayer.Data, PlayerControl.LocalPlayer.CurrentOutfitType, PlayerMaterial.MaskType.None, false);
                    }
                    else if (AprilFoolsMode.ShouldLongAround())
                    {
                        poolablePlayer = __instance.HideAndSeekPlayerVisual;
                        poolablePlayer.gameObject.SetActive(true);
                        poolablePlayer.SetBodyType(PlayerBodyTypes.LongSeeker);
                        anim = __instance.HnSSeekerSpawnLongAnim;
                    }
                    else
                    {
                        poolablePlayer = __instance.HideAndSeekPlayerVisual;
                        poolablePlayer.gameObject.SetActive(true);
                        poolablePlayer.SetBodyType(PlayerBodyTypes.Seeker);
                        anim = __instance.HnSSeekerSpawnAnim;
                    }

                    poolablePlayer.SetBodyCosmeticsVisible(false);
                    poolablePlayer.UpdateFromPlayerData(PlayerControl.LocalPlayer.Data, PlayerControl.LocalPlayer.CurrentOutfitType, PlayerMaterial.MaskType.None, false);
                    SpriteAnim component = poolablePlayer.GetComponent<SpriteAnim>();
                    poolablePlayer.gameObject.SetActive(true);
                    poolablePlayer.ToggleName(false);
                    component.Play(anim);
                    while (crewmateLeadTime > 0.0)
                    {
                        __instance.HideAndSeekTimerText.text = Mathf.RoundToInt(crewmateLeadTime).ToString();
                        crewmateLeadTime -= Time.deltaTime;
                        yield return null;
                    }
                }
            }
            else
            {
                if (logicOptions != null && MapUtilities.CachedShipStatus != null) MapUtilities.CachedShipStatus.HideCountdown = logicOptions.GetCrewmateLeadTime();
                if (AprilFoolsMode.ShouldHorseAround())
                {
                    if (impostor != null) impostor.AnimateCustom(__instance.HnSSeekerSpawnHorseInGameAnim);
                }
                else if (AprilFoolsMode.ShouldLongAround())
                {
                    if (impostor != null) impostor.AnimateCustom(__instance.HnSSeekerSpawnLongInGameAnim);
                }
                else if (impostor != null)
                {
                    impostor.AnimateCustom(__instance.HnSSeekerSpawnAnim);
                    impostor.cosmetics.SetBodyCosmeticsVisible(false);
                }
            }

            impostor = null;
            playerSlot = null;
        }

        MapUtilities.CachedShipStatus?.StartSFX();
        __instance.gameObject?.Destroy();
    }

    private static IEnumerator WaitForIntroPrerequisites()
    {
        const int timeoutMs = 7000;
        DateTime start = DateTime.UtcNow;
        while (PlayerControl.LocalPlayer == null)
        {
            if ((DateTime.UtcNow - start).TotalMilliseconds > timeoutMs)
            {
                Logger.LogError($"IntroCutscene :: CoBegin() timeout({timeoutMs}ms): LocalPlayer is null");
                yield break;
            }

            yield return null;
        }
    }

    private static IEnumerator WaitRoleAssign()
    {
        if (!CustomOptionHolder.ActivateRoles.GetBool()) yield break;

        while (!RoleAssignment.IsAssigned) yield return null;
    }

    private static IEnumerator SetupRole(IntroCutscene __instance)
    {
        if (__instance == null || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null)
        {
            Logger.LogError("SetupRole aborted: IntroCutscene or LocalPlayer is null.");
            yield break;
        }

        List<RoleInfo> infos = RoleInfo.GetRoleInfoForPlayer(PlayerControl.LocalPlayer, false, [RoleType.Lovers]);
        RoleInfo roleInfo = infos.FirstOrDefault();

        Logger.LogInfo("----------Role Assign-----------", "Settings");
        foreach (PlayerControl pc in PlayerControl.AllPlayerControls.GetFastEnumerator()) Logger.LogInfo($"{(pc.AmOwner ? "[*]" : ""),-3}{pc.PlayerId,-2}:{pc.Data.PlayerName.PadRightV2(20)}:{RoleInfo.GetRolesString(pc, false, joinSeparator: " + ")}", "Settings");

        Logger.LogInfo("-----------Platforms------------", "Settings");
        foreach (PlayerControl pc in PlayerControl.AllPlayerControls.GetFastEnumerator()) Logger.LogInfo($"{(pc.AmOwner ? "[*]" : ""),-3}{pc.PlayerId,-2}:{pc.Data.PlayerName.PadRightV2(20)}:{pc.GetPlatform().Replace("Standalone", "")}", "Settings");

        Logger.LogInfo("---------Game Settings----------", "Settings");
        RebuildUs.OptionsPage = 0;
        string[] tmp = GameOptionsManager.Instance.CurrentGameOptions.ToHudString(GameData.Instance ? GameData.Instance.PlayerCount : 10).Split("\r\n");
        foreach (string t in tmp[1..^2]) Logger.LogInfo(t, "Settings");

        Logger.LogInfo("--------Advance Settings--------", "Settings");
        foreach (CustomOption o in CustomOption.AllOptions)
        {
            if (o.Parent == null ? !o.GetString().Equals("0%") : o.Parent.Enabled)
                Logger.LogInfo($"{(o.Parent == null ? o.NameKey : $"┗ {o.NameKey}")}:{o.GetString().RemoveHtml()}", "Settings");
        }

        Logger.LogInfo("--------------------------------", "Settings");

        if (roleInfo != null)
        {
            __instance.YouAreText.color = roleInfo.Color;
            Logger.LogInfo(roleInfo.Color);
            __instance.RoleText.text = roleInfo.Name;
            Logger.LogInfo(roleInfo.Name);
            __instance.RoleText.color = roleInfo.Color;
            Logger.LogInfo(roleInfo.Color);
            __instance.RoleBlurbText.text = roleInfo.IntroDescription;
            Logger.LogInfo(roleInfo.IntroDescription);
            __instance.RoleBlurbText.color = roleInfo.Color;
            Logger.LogInfo(roleInfo.Color);

            if (PlayerControl.LocalPlayer.HasModifier(ModifierType.Madmate))
            {
                if (roleInfo == RoleInfo.Crewmate)
                    __instance.RoleText.text = Tr.Get(TrKey.Madmate);
                else
                    __instance.RoleText.text = Tr.Get(TrKey.MadmatePrefix) + __instance.RoleText.text;

                __instance.YouAreText.color = Madmate.NameColor;
                __instance.RoleText.color = Madmate.NameColor;
                __instance.RoleBlurbText.text = Tr.Get(TrKey.MadmateIntroDesc);
                __instance.RoleBlurbText.color = Madmate.NameColor;
            }
        }

        if (infos.Any(info => info.RoleType == RoleType.Lovers))
        {
            PlayerControl otherLover = PlayerControl.LocalPlayer.GetPartner();
            __instance.RoleBlurbText.text += "\n" + Helpers.Cs(Lovers.Color, string.Format(Tr.Get(TrKey.LoversFlavorIntroDesc), otherLover?.Data?.PlayerName ?? ""));
        }

        // 従来処理
        SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer?.Data?.Role?.IntroSound, false);
        __instance.YouAreText.gameObject.SetActive(true);
        __instance.RoleText.gameObject.SetActive(true);
        __instance.RoleBlurbText.gameObject.SetActive(true);

        if (__instance.ourCrewmate == null)
        {
            __instance.ourCrewmate = __instance.CreatePlayer(0, 1, PlayerControl.LocalPlayer.Data, false);
            if (__instance.ourCrewmate == null)
            {
                Logger.LogError("SetupRole aborted: failed to create intro player.");
                yield break;
            }

            __instance.ourCrewmate.gameObject.SetActive(false);
        }

        __instance.ourCrewmate.gameObject.SetActive(true);
        __instance.ourCrewmate.transform.localPosition = new(0.0f, -1.05f, -18f);
        __instance.ourCrewmate.transform.localScale = new(1f, 1f, 1f);
        __instance.ourCrewmate.ToggleName(false);
        yield return new WaitForSeconds(2.5f);
        __instance.YouAreText.gameObject.SetActive(false);
        __instance.RoleText.gameObject.SetActive(false);
        __instance.RoleBlurbText.gameObject.SetActive(false);
        __instance.ourCrewmate.gameObject.SetActive(false);
    }
}
