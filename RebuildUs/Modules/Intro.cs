using BepInEx.Unity.IL2CPP.Utils.Collections;
using System.Collections;

namespace RebuildUs.Modules;

public static class Intro
{
    public static PoolablePlayer PlayerPrefab;
    public static Vector3 BottomLeft;

    public static void GenerateMiniCrewIcons(IntroCutscene __instance)
    {
        // int playerCounter = 0;
        if (PlayerControl.LocalPlayer != null && FastDestroyableSingleton<HudManager>.Instance != null && __instance.PlayerPrefab != null)
        {
            float aspect = Camera.main.aspect;
            float safeOrthographicSize = CameraSafeArea.GetSafeOrthographicSize(Camera.main);
            float xpos = 1.75f - safeOrthographicSize * aspect * 1.70f;
            float ypos = 0.15f - safeOrthographicSize * 1.7f;
            BottomLeft = new Vector3(xpos / 2, ypos / 2, -61f);

            foreach (var p in PlayerControl.AllPlayerControls)
            {
                if (p.Data == null) continue; // Null check for p.Data
                var data = p.Data;
                var player = UnityEngine.Object.Instantiate(__instance.PlayerPrefab, FastDestroyableSingleton<HudManager>.Instance.transform);
                PlayerPrefab = __instance.PlayerPrefab;
                p.SetPlayerMaterialColors(player.cosmetics.currentBodySprite.BodySprite);
                player.SetSkin(data.DefaultOutfit.SkinId, data.DefaultOutfit.ColorId);
                player.cosmetics.SetHat(data.DefaultOutfit.HatId, data.DefaultOutfit.ColorId);
                // PlayerControl.SetPetImage(data.DefaultOutfit.PetId, data.DefaultOutfit.ColorId, player.PetSlot);
                player.cosmetics.nameText.text = data.PlayerName;
                player.SetFlipX(true);
                ModMapOptions.PlayerIcons[p.PlayerId] = player;
                player.gameObject.SetActive(false);
                ModMapOptions.PlayerIcons[p.PlayerId] = player;

                if (PlayerControl.LocalPlayer.IsRole(RoleType.BountyHunter))
                {
                    player.transform.localPosition = BottomLeft + new Vector3(-0.25f, 0f, 0);
                    player.transform.localScale = Vector3.one * 0.4f;
                    player.gameObject.SetActive(false);
                }

                //  This can be done for all players not just for the bounty hunter as it was before. Allows the thief to have the correct position and scaling
                player.transform.localPosition = BottomLeft;
                player.transform.localScale = Vector3.one * 0.4f;
                player.gameObject.SetActive(false);
            }
        }

        RebuildUs.OnIntroEnd();

        // インポスター視界の場合に昇降機右の影を無効化
        if (Helpers.IsAirship && CustomOptionHolder.AirshipOptions.GetBool() && Helpers.HasImpostorVision(PlayerControl.LocalPlayer))
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
            records.GetComponentsInChildren<MapConsole>().FirstOrDefault(x => x.name == "records_admin_map")?.gameObject.SetActive(false);
        }

        if (ShipStatus.Instance.FastRooms.ContainsKey(SystemTypes.GapRoom))
        {
            var gapRoom = ShipStatus.Instance.FastRooms[SystemTypes.GapRoom].gameObject;
            // GapRoomの配電盤を消す
            if (Helpers.IsAirship && CustomOptionHolder.AirshipDisableGapSwitchBoard.GetBool())
            {
                var sabotage = gapRoom.GetComponentsInChildren<Console>().FirstOrDefault(x => x.name == "task_lightssabotage (gap)")?.gameObject;
                sabotage.SetActive(false);
                MapUtilities.CachedShipStatus.AllConsoles = MapUtilities.CachedShipStatus.AllConsoles.Where(x => x != sabotage.GetComponent<Console>()).ToArray();
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
            var objects = UnityEngine.GameObject.FindObjectsOfType<Console>().ToList();
            objects.Find(x => x.name == "task_garbage1").checkWalls = true;
            objects.Find(x => x.name == "task_garbage2").checkWalls = true;
            objects.Find(x => x.name == "task_garbage3").checkWalls = true;
            objects.Find(x => x.name == "task_garbage4").checkWalls = true;
            objects.Find(x => x.name == "task_garbage5").checkWalls = true;
            objects.Find(x => x.name == "task_shower").checkWalls = true;
            objects.Find(x => x.name == "task_developphotos").checkWalls = true;
            objects.Find(x => x.name == "DivertRecieve" && x.Room == SystemTypes.Armory).checkWalls = true;
            objects.Find(x => x.name == "DivertRecieve" && x.Room == SystemTypes.MainHall).checkWalls = true;
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
        if (Spy.Exists && PlayerControl.LocalPlayer.Data.Role.IsImpostor)
        {
            var players = PlayerControl.AllPlayerControls.ToArray().ToList().OrderBy(x => Guid.NewGuid()).ToList();
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
        yield return __instance.CoBegin();
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

    public static bool ShowRole(IntroCutscene __instance, ref Il2CppSystem.Collections.IEnumerator __result)
    {
        if (!CustomOptionHolder.ActivateRoles.GetBool()) return true; // Don't override the intro of the vanilla roles
        __result = SetupRole(__instance).WrapToIl2Cpp();
        return false;
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

        // TODO: FIX LATER
        __instance.YouAreText.color = roleInfo.Color;
        __instance.RoleText.text = roleInfo.Name;
        __instance.RoleText.color = roleInfo.Color;
        __instance.RoleBlurbText.text = roleInfo.IntroDescription;
        __instance.RoleBlurbText.color = roleInfo.Color;

        // if (PlayerControl.LocalPlayer.HasModifier(EModifierType.Madmate))
        // {
        //     if (roleInfo == RoleInfo.crewmate)
        //     {
        //         __instance.RoleText.text = Tr.Get("Role.Madmate");
        //     }
        //     else
        //     {
        //         __instance.RoleText.text = Tr.Get("Option.MadmatePrefix") + __instance.RoleText.text;
        //     }
        //     __instance.YouAreText.color = Madmate.color;
        //     __instance.RoleText.color = Madmate.color;
        //     __instance.RoleBlurbText.text = Tr.Get("IntroDesc.Madmate");
        //     __instance.RoleBlurbText.color = Madmate.color;
        // }

        // if (infos.Any(info => info.RoleType == ERoleType.Lovers))
        // {
        //     PlayerControl otherLover = PlayerControl.LocalPlayer.GetPartner();
        //     __instance.RoleBlurbText.text += "\n" + Helpers.cs(Lovers.color, string.Format(Tr.Get("loversFlavor"), otherLover?.Data?.PlayerName ?? ""));
        // }

        // 従来処理
        SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.Data.Role.IntroSound, false);
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
