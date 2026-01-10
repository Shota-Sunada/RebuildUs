using BepInEx.Unity.IL2CPP.Utils.Collections;
using RebuildUs.Players;
using RebuildUs.Roles;
using RebuildUs.Utilities;
using System.Collections;

namespace RebuildUs.Modules;

public static class Intro
{
    public static PoolablePlayer playerPrefab;
    public static void GenerateMiniCrewIcons(IntroCutscene __instance)
    {
        // int playerCounter = 0;
        if (CachedPlayer.LocalPlayer.PlayerControl != null && FastDestroyableSingleton<HudManager>.Instance != null)
        {
            float aspect = Camera.main.aspect;
            float safeOrthographicSize = CameraSafeArea.GetSafeOrthographicSize(Camera.main);
            float xpos = 1.75f - safeOrthographicSize * aspect * 1.70f;
            float ypos = 0.15f - safeOrthographicSize * 1.7f;
            var bottomLeft = new Vector3(xpos / 2, ypos / 2, -61f);

            foreach (PlayerControl p in PlayerControl.AllPlayerControls)
            {
                NetworkedPlayerInfo data = p.Data;
                var player = UnityEngine.Object.Instantiate(__instance.PlayerPrefab, FastDestroyableSingleton<HudManager>.Instance.transform);
                playerPrefab = __instance.PlayerPrefab;
                p.SetPlayerMaterialColors(player.cosmetics.currentBodySprite.BodySprite);
                player.SetSkin(data.DefaultOutfit.SkinId, data.DefaultOutfit.ColorId);
                player.cosmetics.SetHat(data.DefaultOutfit.HatId, data.DefaultOutfit.ColorId);
                // PlayerControl.SetPetImage(data.DefaultOutfit.PetId, data.DefaultOutfit.ColorId, player.PetSlot);
                player.cosmetics.nameText.text = data.PlayerName;
                player.SetFlipX(true);
                MapOptions.PlayerIcons[p.PlayerId] = player;
                player.gameObject.SetActive(false);

                // if (PlayerControl.LocalPlayer == Arsonist.arsonist && p != Arsonist.arsonist)
                // {
                //     player.transform.localPosition = bottomLeft + new Vector3(-0.25f, -0.25f, 0) + Vector3.right * playerCounter++ * 0.35f;
                //     player.transform.localScale = Vector3.one * 0.2f;
                //     player.SetSemiTransparent(true);
                //     player.gameObject.SetActive(true);
                // }
                // else
                {
                    //  This can be done for all players not just for the bounty hunter as it was before. Allows the thief to have the correct position and scaling
                    player.transform.localPosition = bottomLeft;
                    player.transform.localScale = Vector3.one * 0.4f;
                    player.gameObject.SetActive(false);
                }
            }
        }
    }

    public static void SetupIntroTeamIcons(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
    {
        // Intro solo teams
        if (CachedPlayer.LocalPlayer.PlayerControl.IsNeutral())
        {
            var soloTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            soloTeam.Add(PlayerControl.LocalPlayer);
            yourTeam = soloTeam;
        }

        // Add the Spy to the Impostor team (for the Impostors)
        // if (Spy.spy != null && PlayerControl.LocalPlayer.Data.Role.IsImpostor)
        // {
        //     var players = PlayerControl.AllPlayerControls.ToArray().ToList().OrderBy(x => Guid.NewGuid()).ToList();
        //     var fakeImpostorTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>(); // The local player always has to be the first one in the list (to be displayed in the center)
        //     fakeImpostorTeam.Add(PlayerControl.LocalPlayer);
        //     foreach (var p in players)
        //     {
        //         if (PlayerControl.LocalPlayer != p && (p == Spy.spy || p.Data.Role.IsImpostor))
        //         {
        //             fakeImpostorTeam.Add(p);
        //         }
        //     }
        //     yourTeam = fakeImpostorTeam;
        // }
    }

    public static void SetupIntroTeam(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
    {
        var infos = RoleInfo.getRoleInfoForPlayer(CachedPlayer.LocalPlayer.PlayerControl);
        var roleInfo = infos.Where(info => info.roleType != RoleType.Lovers).FirstOrDefault();
        if (roleInfo == null) return;
        // if (CachedPlayer.LocalPlayer.PlayerControl.IsNeutral() || CachedPlayer.LocalPlayer.PlayerControl.isGM())
        if (CachedPlayer.LocalPlayer.PlayerControl.IsNeutral())
        {
            __instance.BackgroundBar.material.color = roleInfo.color;
            __instance.TeamTitle.text = roleInfo.name;
            __instance.TeamTitle.color = roleInfo.color;
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
        if (!CustomOptionHolder.ActivateRoles.getBool()) yield break;

        while (!RoleAssignmentPatch.isAssigned)
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
        List<RoleInfo> infos = RoleInfo.GetRoleInfoForPlayer(CachedPlayer.LocalPlayer.PlayerControl, new ERoleType[] { ERoleType.Lovers });
        RoleInfo roleInfo = infos.FirstOrDefault();

        // Logger.LogInfo("----------Role Assign-----------", "Settings");
        // foreach (var pc in PlayerControl.AllPlayerControls.GetFastEnumerator())
        // {
        //     Logger.LogInfo(string.Format("{0,-3}{1,-2}:{2}:{3}", pc.AmOwner ? "[*]" : "", pc.PlayerId, pc.Data.PlayerName.PadRightV2(20), RoleInfo.GetRolesString(pc, false, joinSeparator: " + ")), "Settings");
        // }
        // Logger.LogInfo("-----------Platforms------------", "Settings");
        // foreach (var pc in PlayerControl.AllPlayerControls.GetFastEnumerator())
        // {
        //     Logger.LogInfo(string.Format("{0,-3}{1,-2}:{2}:{3}", pc.AmOwner ? "[*]" : "", pc.PlayerId, pc.Data.PlayerName.PadRightV2(20), pc.getPlatform().Replace("Standalone", "")), "Settings");
        // }
        // Logger.LogInfo("---------Game Settings----------", "Settings");
        // RebuildUs.OptionsPage = 0;
        // var tmp = PlayerControl.GameOptions.ToHudString(GameData.Instance ? GameData.Instance.PlayerCount : 10).Split("\r\n");
        // foreach (var t in tmp[1..(tmp.Length - 2)])
        // {
        //     Logger.LogInfo(t, "Settings");
        // }
        // Logger.LogInfo("--------Advance Settings--------", "Settings");
        // foreach (var o in CustomOption.AllOptions)
        // {
        //     if (o.Parent == null ? !o.GetString().Equals("0%") : o.Parent.Enabled)
        //     {
        //         Logger.LogInfo(string.Format("{0}:{1}", o.Parent == null ? o.name.removeHtml().PadRightV2(43) : $"┗ {o.name.removeHtml().PadRightV2(41)}", o.getString().removeHtml()), "Settings");
        //     }
        // }
        // Logger.LogInfo("--------------------------------", "Settings");

        __instance.YouAreText.color = roleInfo.Color;
        __instance.RoleText.text = roleInfo.Name;
        __instance.RoleText.color = roleInfo.Color;
        __instance.RoleBlurbText.text = roleInfo.IntroDescription;
        __instance.RoleBlurbText.color = roleInfo.Color;

        // if (CachedPlayer.LocalPlayer.PlayerControl.HasModifier(EModifierType.Madmate))
        // {
        //     if (roleInfo == RoleInfo.crewmate)
        //     {
        //         __instance.RoleText.text = ModTranslation.getString("madmate");
        //     }
        //     else
        //     {
        //         __instance.RoleText.text = ModTranslation.getString("madmatePrefix") + __instance.RoleText.text;
        //     }
        //     __instance.YouAreText.color = Madmate.color;
        //     __instance.RoleText.color = Madmate.color;
        //     __instance.RoleBlurbText.text = ModTranslation.getString("madmateIntroDesc");
        //     __instance.RoleBlurbText.color = Madmate.color;
        // }

        // if (infos.Any(info => info.roleType == RoleType.Lovers))
        // {
        //     PlayerControl otherLover = CachedPlayer.LocalPlayer.PlayerControl.getPartner();
        //     __instance.RoleBlurbText.text += "\n" + Helpers.cs(Lovers.color, String.Format(ModTranslation.getString("loversFlavor"), otherLover?.Data?.PlayerName ?? ""));
        // }

        // 従来処理
        SoundManager.Instance.PlaySound(CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IntroSound, false, 1f);
        __instance.YouAreText.gameObject.SetActive(true);
        __instance.RoleText.gameObject.SetActive(true);
        __instance.RoleBlurbText.gameObject.SetActive(true);

        if (__instance.ourCrewmate == null)
        {
            __instance.ourCrewmate = __instance.CreatePlayer(0, 1, CachedPlayer.LocalPlayer.PlayerControl.Data, false);
            __instance.ourCrewmate.gameObject.SetActive(false);
        }
        __instance.ourCrewmate.gameObject.SetActive(true);
        __instance.ourCrewmate.transform.localPosition = new Vector3(0f, -1.05f, -18f);
        __instance.ourCrewmate.transform.localScale = new Vector3(1f, 1f, 1f);
        yield return new WaitForSeconds(2.5f);
        __instance.YouAreText.gameObject.SetActive(false);
        __instance.RoleText.gameObject.SetActive(false);
        __instance.RoleBlurbText.gameObject.SetActive(false);
        __instance.ourCrewmate.gameObject.SetActive(false);

        yield break;
    }
}