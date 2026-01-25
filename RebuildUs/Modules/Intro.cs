using BepInEx.Unity.IL2CPP.Utils.Collections;
using PowerTools;
using System.Collections;

namespace RebuildUs.Modules;

public static class Intro
{
    public static PoolablePlayer PlayerPrefab;
    public static Vector3 BottomLeft;

    public static void GenerateMiniCrewIcons(IntroCutscene __instance)
    {
        if (PlayerControl.LocalPlayer != null && FastDestroyableSingleton<HudManager>.Instance != null && __instance.PlayerPrefab != null)
        {
            BottomLeft = AspectPosition.ComputePosition(AspectPosition.EdgeAlignments.LeftBottom, new(0.9f, 0.7f, -10f));

            foreach (var p in PlayerControl.AllPlayerControls)
            {
                if (p.Data == null) continue; // Null check for p.Data
                var data = p.Data;
                var player = UnityEngine.Object.Instantiate(__instance.PlayerPrefab, FastDestroyableSingleton<HudManager>.Instance.transform);
                PlayerPrefab = __instance.PlayerPrefab;
                player.UpdateFromPlayerData(data, p.CurrentOutfitType, PlayerMaterial.MaskType.None, false);
                player.cosmetics.nameText.text = data.PlayerName;
                player.cosmetics.nameText.transform.localPosition = new(0f, -0.7f, -0.1f);
                player.SetFlipX(true);
                ModMapOptions.PlayerIcons[p.PlayerId] = player;
                player.gameObject.SetActive(false);

                // UIレイヤーに設定
                player.gameObject.layer = 5;
                foreach (var child in player.GetComponentsInChildren<Transform>(true))
                {
                    child.gameObject.layer = 5;
                }

                //  Allows the roles to have the correct position and scaling via their own UpdateIcons
                player.transform.localPosition = BottomLeft;
                player.transform.localScale = Vector3.one * 0.3f;
            }
        }

        RebuildUs.OnIntroEnd();

        // インポスター視界の場合に昇降機右の影を無効化
        if (Helpers.IsAirship && CustomOptionHolder.AirshipOptimize.GetBool() && Helpers.HasImpostorVision(PlayerControl.LocalPlayer) && ShipStatus.Instance.FastRooms.ContainsKey(SystemTypes.GapRoom))
        {
            var obj = ShipStatus.Instance.FastRooms[SystemTypes.GapRoom].gameObject;
            var oneWayShadow = obj.transform.FindChild("Shadow").FindChild("LedgeShadow").GetComponent<OneWayShadows>();
            oneWayShadow.gameObject.SetActive(false);
        }

        // ベントを追加する
        AdditionalVents.AddAdditionalVents();

        // スペシメンにバイタルを移動する
        SpecimenVital.MoveVital();

        // アーカイブのアドミンを消す
        if (Helpers.IsAirship && CustomOptionHolder.AirshipOldAdmin.GetBool())
        {
            GameObject records = ShipStatus.Instance.FastRooms[SystemTypes.Records].gameObject;
            foreach (var console in records.GetComponentsInChildren<MapConsole>())
            {
                if (console.name == "records_admin_map")
                {
                    console.gameObject.SetActive(false);
                    break;
                }
            }
        }

        if (ShipStatus.Instance.FastRooms.ContainsKey(SystemTypes.GapRoom))
        {
            var gapRoom = ShipStatus.Instance.FastRooms[SystemTypes.GapRoom].gameObject;
            // GapRoomの配電盤を消す
            if (Helpers.IsAirship && CustomOptionHolder.AirshipDisableGapSwitchBoard.GetBool())
            {
                GameObject sabotage = null;
                foreach (var console in gapRoom.GetComponentsInChildren<Console>())
                {
                    if (console.name == "task_lightssabotage (gap)")
                    {
                        sabotage = console.gameObject;
                        break;
                    }
                }

                if (sabotage != null)
                {
                    sabotage.SetActive(false);
                    var sabotageConsole = sabotage.GetComponent<Console>();
                    var newConsoles = new List<Console>();
                    foreach (var c in MapUtilities.CachedShipStatus.AllConsoles)
                    {
                        if (c != sabotageConsole) newConsoles.Add(c);
                    }
                    MapUtilities.CachedShipStatus.AllConsoles = newConsoles.ToArray();
                }
            }

            // ぬ～んを消す
            if (Helpers.IsAirship && CustomOptionHolder.AirshipDisableMovingPlatform.GetBool())
            {
                gapRoom.GetComponentInChildren<MovingPlatformBehaviour>().gameObject.SetActive(false);
                foreach (var obj in gapRoom.GetComponentsInChildren<PlatformConsole>())
                {
                    obj.gameObject.SetActive(false);
                }
            }
        }

        //タスクバグ修正
        if (Helpers.IsAirship && CustomOptionHolder.AirshipEnableWallCheck.GetBool())
        {
            foreach (var x in UnityEngine.Object.FindObjectsOfType<Console>())
            {
                if (x.name == "task_garbage1") x.checkWalls = true;
                else if (x.name == "task_garbage2") x.checkWalls = true;
                else if (x.name == "task_garbage3") x.checkWalls = true;
                else if (x.name == "task_garbage4") x.checkWalls = true;
                else if (x.name == "task_garbage5") x.checkWalls = true;
                else if (x.name == "task_shower") x.checkWalls = true;
                else if (x.name == "task_developphotos") x.checkWalls = true;
                else if (x.name == "DivertRecieve" && (x.Room == SystemTypes.Armory || x.Room == SystemTypes.MainHall)) x.checkWalls = true;
            }
        }

        // 最初から一人の場合はLast Impostorになる
        if (AmongUsClient.Instance.AmHost)
        {
            LastImpostor.PromoteToLastImpostor();
        }

        // タスクパネルの表示優先度を上げる
        var taskPanel = FastDestroyableSingleton<HudManager>.Instance.TaskStuff;
        var pos = taskPanel.transform.position;
        taskPanel.transform.position = new Vector3(pos.x, pos.y, -20);

        // マップデータのコピーを読み込み
        if (CustomOptionHolder.AirshipReplaceSafeTask.GetBool())
        {
            MapData.LoadAssets(AmongUsClient.Instance);
        }
    }

    public static void SetupIntroTeamIcons(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
    {
        // Intro solo teams
        if (PlayerControl.LocalPlayer.IsNeutral())
        {
            var soloTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            soloTeam.Add(PlayerControl.LocalPlayer);
            yourTeam = soloTeam;
        }

        // Add the Spy to the Impostor team (for the Impostors)
        if (Spy.Exists && PlayerControl.LocalPlayer.IsTeamImpostor())
        {
            var players = new List<PlayerControl>();
            foreach (var p in PlayerControl.AllPlayerControls) players.Add(p);
            players.Shuffle();

            var fakeImpostorTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>(); // The local player always has to be the first one in the list (to be displayed in the center)
            fakeImpostorTeam.Add(PlayerControl.LocalPlayer);

            foreach (var p in players)
            {
                if (PlayerControl.LocalPlayer != p && (p.IsRole(RoleType.Spy) || p.Data.Role.IsImpostor))
                {
                    fakeImpostorTeam.Add(p);
                }
            }
            yourTeam = fakeImpostorTeam;
        }
    }

    public static void SetupIntroTeam(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
    {
        var infos = RoleInfo.GetRoleInfoForPlayer(PlayerControl.LocalPlayer);
        var roleInfo = infos.FirstOrDefault(info => info.RoleType != RoleType.Lovers);
        if (roleInfo == null) return;
        // if (PlayerControl.LocalPlayer.IsNeutral() || PlayerControl.LocalPlayer.IsGM())
        if (PlayerControl.LocalPlayer.IsNeutral())
        {
            __instance.BackgroundBar.material.color = roleInfo.Color;
            __instance.TeamTitle.text = roleInfo.Name;
            __instance.TeamTitle.color = roleInfo.Color;
            __instance.ImpostorText.text = "";
        }
    }

    public static IEnumerator CoBegin(IntroCutscene __instance)
    {
        yield return WaitRoleAssign().WrapToIl2Cpp();
        // yield return __instance.CoBegin();

        Logger.LogInfo("IntroCutscene :: CoBegin() :: Starting intro cutscene");
        SoundManager.Instance.PlaySound(__instance.IntroStinger, false);

        if (Helpers.IsNormal)
        {
            Logger.LogInfo("IntroCutscene :: CoBegin() :: Game Mode: Normal");
            __instance.LogPlayerRoleData();
            __instance.HideAndSeekPanels.SetActive(false);
            __instance.CrewmateRules.SetActive(false);
            __instance.ImpostorRules.SetActive(false);
            __instance.ImpostorName.gameObject.SetActive(false);
            __instance.ImpostorTitle.gameObject.SetActive(false);
            var show = IntroCutscene.SelectTeamToShow((Func<NetworkedPlayerInfo, bool>)(pcd => !PlayerControl.LocalPlayer.IsTeamImpostor() || pcd.Role.TeamType == PlayerControl.LocalPlayer.Data.Role.TeamType));
            if (show == null || show.Count < 1)
            {
                Logger.LogError("IntroCutscene :: CoBegin() :: teamToShow is EMPTY or NULL");
            }
            if (PlayerControl.LocalPlayer.IsTeamImpostor())
            {
                __instance.ImpostorText.gameObject.SetActive(false);
            }
            else
            {
                int adjustedNumImpostors = GameManager.Instance.LogicOptions.GetAdjustedNumImpostors(GameData.Instance.PlayerCount);
                __instance.ImpostorText.text = adjustedNumImpostors == 1
                    ? DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.NumImpostorsS)
                    : DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.NumImpostorsP, adjustedNumImpostors);
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
            var show = IntroCutscene.SelectTeamToShow((Func<NetworkedPlayerInfo, bool>)(pcd => PlayerControl.LocalPlayer.IsTeamImpostor() != pcd.Role.IsImpostor));
            if (show == null || show.Count < 1)
            {
                Logger.LogError("IntroCutscene :: CoBegin() :: teamToShow is EMPTY or NULL");
            }
            PlayerControl impostor = PlayerControl.AllPlayerControls.Find((Il2CppSystem.Predicate<PlayerControl>)(pc => pc.Data.Role.IsImpostor));
            if (impostor == null)
            {
                Logger.LogError("IntroCutscene :: CoBegin() :: impostor is NULL");
            }
            GameManager.Instance.SetSpecialCosmetics(impostor);
            __instance.ImpostorName.gameObject.SetActive(true);
            __instance.ImpostorTitle.gameObject.SetActive(true);
            __instance.BackgroundBar.enabled = false;
            __instance.TeamTitle.gameObject.SetActive(false);
            __instance.ImpostorName.text = impostor == null ? impostor.Data.PlayerName : "???";
            yield return new WaitForSecondsRealtime(0.1f);
            PoolablePlayer playerSlot = null;
            if (impostor == null)
            {
                playerSlot = __instance.CreatePlayer(1, 1, impostor.Data, false);
                playerSlot.SetBodyType(PlayerBodyTypes.Normal);
                playerSlot.SetFlipX(false);
                playerSlot.transform.localPosition = __instance.impostorPos;
                playerSlot.transform.localScale = Vector3.one * __instance.impostorScale;
            }
            yield return ShipStatus.Instance.CosmeticsCache.PopulateFromPlayers();
            yield return new WaitForSecondsRealtime(6f);
            if (playerSlot == null)
            {
                playerSlot.gameObject.SetActive(false);
            }
            __instance.HideAndSeekPanels.SetActive(false);
            __instance.CrewmateRules.SetActive(false);
            __instance.ImpostorRules.SetActive(false);
            LogicOptionsHnS logicOptions = GameManager.Instance.LogicOptions as LogicOptionsHnS;
            if (GameManager.Instance.GetLogicComponent<LogicHnSMusic>() is LogicHnSMusic logicComponent)
                logicComponent.StartMusicWithIntro();
            if (PlayerControl.LocalPlayer.IsTeamImpostor())
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
                var component = poolablePlayer.GetComponent<SpriteAnim>();
                poolablePlayer.gameObject.SetActive(true);
                poolablePlayer.ToggleName(false);
                component.Play(anim);
                while ((double)crewmateLeadTime > 0.0)
                {
                    __instance.HideAndSeekTimerText.text = Mathf.RoundToInt(crewmateLeadTime).ToString();
                    crewmateLeadTime -= Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                ShipStatus.Instance.HideCountdown = logicOptions.GetCrewmateLeadTime();
                if (AprilFoolsMode.ShouldHorseAround())
                {
                    if (impostor == null)
                    {
                        impostor.AnimateCustom(__instance.HnSSeekerSpawnHorseInGameAnim);
                    }
                }
                else if (AprilFoolsMode.ShouldLongAround())
                {
                    if (impostor == null)
                    {
                        impostor.AnimateCustom(__instance.HnSSeekerSpawnLongInGameAnim);
                    }
                }
                else if (impostor == null)
                {
                    impostor.AnimateCustom(__instance.HnSSeekerSpawnAnim);
                    impostor.cosmetics.SetBodyCosmeticsVisible(false);
                }
            }
            impostor = null;
            playerSlot = null;
        }
        ShipStatus.Instance.StartSFX();
        __instance.gameObject.Destroy();

        yield break;
    }

    private static IEnumerator WaitRoleAssign()
    {
        if (!CustomOptionHolder.ActivateRoles.GetBool()) yield break;

        while (!RoleAssignment.IsAssigned)
        {
            yield return null;
        }
        yield break;
    }

    private static IEnumerator SetupRole(IntroCutscene __instance)
    {
        var infos = RoleInfo.GetRoleInfoForPlayer(PlayerControl.LocalPlayer, false, [RoleType.Lovers]);
        var roleInfo = infos.FirstOrDefault();

        Logger.LogInfo("----------Role Assign-----------", "Settings");
        foreach (var pc in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            Logger.LogInfo(string.Format("{0,-3}{1,-2}:{2}:{3}", pc.AmOwner ? "[*]" : "", pc.PlayerId, pc.Data.PlayerName.PadRightV2(20), RoleInfo.GetRolesString(pc, false, joinSeparator: " + ")), "Settings");
        }
        Logger.LogInfo("-----------Platforms------------", "Settings");
        foreach (var pc in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            Logger.LogInfo(string.Format("{0,-3}{1,-2}:{2}:{3}", pc.AmOwner ? "[*]" : "", pc.PlayerId, pc.Data.PlayerName.PadRightV2(20), pc.GetPlatform().Replace("Standalone", "")), "Settings");
        }
        Logger.LogInfo("---------Game Settings----------", "Settings");
        RebuildUs.OptionsPage = 0;
        var tmp = GameOptionsManager.Instance.CurrentGameOptions.ToHudString(GameData.Instance ? GameData.Instance.PlayerCount : 10).Split("\r\n");
        foreach (var t in tmp[1..(tmp.Length - 2)])
        {
            Logger.LogInfo(t, "Settings");
        }
        Logger.LogInfo("--------Advance Settings--------", "Settings");
        foreach (var o in CustomOption.AllOptions)
        {
            if (o.Parent == null ? !o.GetString().Equals("0%") : o.Parent.Enabled)
            {
                Logger.LogInfo(string.Format("{0}:{1}", o.Parent == null ? o.NameKey.RemoveHtml().PadRightV2(43) : $"┗ {o.NameKey.RemoveHtml().PadRightV2(41)}", o.GetString().RemoveHtml()), "Settings");
            }
        }
        Logger.LogInfo("--------------------------------", "Settings");

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
            {
                __instance.RoleText.text = Tr.Get("Modifier.Madmate");
            }
            else
            {
                __instance.RoleText.text = Tr.Get("Hud.MadmatePrefix") + __instance.RoleText.text;
            }
            __instance.YouAreText.color = Madmate.NameColor;
            __instance.RoleText.color = Madmate.NameColor;
            __instance.RoleBlurbText.text = Tr.Get("IntroDesc.Madmate");
            __instance.RoleBlurbText.color = Madmate.NameColor;
        }

        if (infos.Any(info => info.RoleType == RoleType.Lovers))
        {
            PlayerControl otherLover = PlayerControl.LocalPlayer.GetPartner();
            __instance.RoleBlurbText.text += "\n" + Helpers.Cs(Lovers.Color, string.Format(Tr.Get("IntroDesc.LoversFlavor"), otherLover?.Data?.PlayerName ?? ""));
        }

        // 従来処理
        SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer?.Data?.Role?.IntroSound, false);
        __instance.YouAreText.gameObject.SetActive(true);
        __instance.RoleText.gameObject.SetActive(true);
        __instance.RoleBlurbText.gameObject.SetActive(true);

        if (__instance.ourCrewmate == null)
        {
            __instance.ourCrewmate = __instance.CreatePlayer(0, 1, PlayerControl.LocalPlayer.Data, false);
            __instance.ourCrewmate.gameObject.SetActive(false);
        }
        __instance.ourCrewmate.gameObject.SetActive(true);
        __instance.ourCrewmate.transform.localPosition = new Vector3(0.0f, -1.05f, -18f);
        __instance.ourCrewmate.transform.localScale = new Vector3(1f, 1f, 1f);
        __instance.ourCrewmate.ToggleName(false);
        yield return new WaitForSeconds(2.5f);
        __instance.YouAreText.gameObject.SetActive(false);
        __instance.RoleText.gameObject.SetActive(false);
        __instance.RoleBlurbText.gameObject.SetActive(false);
        __instance.ourCrewmate.gameObject.SetActive(false);

        yield break;
    }
}