using PowerTools;
using RebuildUs.Modules.Cosmetics.Extensions;

namespace RebuildUs.Modules.Cosmetics.Patches;

[HarmonyPatch(typeof(HatParent))]
internal static class HatParentPatches
{
    [HarmonyPatch(nameof(HatParent.SetHat), typeof(int))]
    [HarmonyPriority(Priority.High)]
    [HarmonyPrefix]
    private static void SetHatPrefix(HatParent __instance)
    {
        SetCustomHat(__instance);
    }

    [HarmonyPatch(nameof(HatParent.SetHat), typeof(HatData), typeof(int))]
    [HarmonyPrefix]
    private static bool SetHatPrefix(HatParent __instance, HatData hat, int color)
    {
        if (SetCustomHat(__instance))
        {
            return true;
        }
        __instance.PopulateFromViewData();
        __instance.SetMaterialColor(color);
        return false;
    }

    [HarmonyPatch(nameof(HatParent.SetHat), typeof(int))]
    [HarmonyPrefix]
    private static bool SetHatPrefix(HatParent __instance, int color)
    {
        if (!__instance.IsCached())
        {
            return true;
        }
        __instance.viewAsset = null;
        __instance.PopulateFromViewData();
        __instance.SetMaterialColor(color);
        return false;
    }

    [HarmonyPatch(nameof(HatParent.UpdateMaterial))]
    [HarmonyPrefix]
    private static bool UpdateMaterialPrefix(HatParent __instance)
    {
        try
        {
            if (__instance == null || !__instance)
            {
                return false;
            }
            if (!__instance.TryGetCached(out var asset))
            {
                return true;
            }
            if (__instance.FrontLayer == null || !__instance.FrontLayer)
            {
                return false;
            }

            var hatManager = DestroyableSingleton<HatManager>.Instance;
            if (hatManager == null)
            {
                return false;
            }
            if (__instance.Hat == null)
            {
                return false;
            }

            var extend = __instance.Hat.GetHatExtension();
            var targetMaterial = asset
                                      && extend is
                                      {
                                          Adaptive: true,
                                      }
                ? hatManager.PlayerMaterial
                : hatManager.DefaultShader;
            if (targetMaterial == null)
            {
                return false;
            }

            __instance.FrontLayer.sharedMaterial = targetMaterial;
            if (__instance.BackLayer && __instance.BackLayer)
            {
                __instance.BackLayer.sharedMaterial = targetMaterial;
            }

            var colorId = __instance.matProperties.ColorId;
            PlayerMaterial.SetColors(colorId, __instance.FrontLayer);
            if (__instance.BackLayer && __instance.BackLayer)
            {
                PlayerMaterial.SetColors(colorId, __instance.BackLayer);
            }

            __instance.FrontLayer.material?.SetInt(PlayerMaterial.MaskLayer, __instance.matProperties.MaskLayer);
            if (__instance.BackLayer && __instance.BackLayer && __instance.BackLayer.material != null)
            {
                __instance.BackLayer.material.SetInt(PlayerMaterial.MaskLayer, __instance.matProperties.MaskLayer);
            }

            var maskType = __instance.matProperties.MaskType;
            switch (maskType)
            {
                case PlayerMaterial.MaskType.ScrollingUI:
                    if (__instance.FrontLayer && __instance.FrontLayer)
                    {
                        __instance.FrontLayer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                    }
                    if (__instance.BackLayer && __instance.BackLayer)
                    {
                        __instance.BackLayer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                    }
                    break;
                case PlayerMaterial.MaskType.Exile:
                    if (__instance.FrontLayer && __instance.FrontLayer)
                    {
                        __instance.FrontLayer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                    }
                    if (__instance.BackLayer && __instance.BackLayer)
                    {
                        __instance.BackLayer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                    }
                    break;
                default:
                    if (__instance.FrontLayer && __instance.FrontLayer)
                    {
                        __instance.FrontLayer.maskInteraction = SpriteMaskInteraction.None;
                    }
                    if (__instance.BackLayer && __instance.BackLayer)
                    {
                        __instance.BackLayer.maskInteraction = SpriteMaskInteraction.None;
                    }
                    break;
            }

            if (__instance.matProperties.MaskLayer > 0)
            {
                return false;
            }
            PlayerMaterial.SetMaskLayerBasedOnLocalPlayer(__instance.FrontLayer, __instance.matProperties.IsLocalPlayer);
            if (!__instance.BackLayer || !__instance.BackLayer)
            {
                return false;
            }
            PlayerMaterial.SetMaskLayerBasedOnLocalPlayer(__instance.BackLayer, __instance.matProperties.IsLocalPlayer);

            return false;
        }
        catch (Exception ex)
        {
            Logger.LogError("HatParentPatches.UpdateMaterialPrefix failed.");
            Logger.LogError(ex);
            return false;
        }
    }

    [HarmonyPatch(nameof(HatParent.LateUpdate))]
    [HarmonyPrefix]
    private static bool LateUpdatePrefix(HatParent __instance)
    {
        if (!__instance.Parent || !__instance.Hat)
        {
            return false;
        }
        if (!__instance.TryGetCached(out var hatViewData))
        {
            return true;
        }
        if (__instance.FrontLayer.sprite != hatViewData.ClimbImage && __instance.FrontLayer.sprite != hatViewData.FloorImage)
        {
            if ((__instance.Hat.InFront || hatViewData.BackImage) && hatViewData.LeftMainImage)
            {
                __instance.FrontLayer.sprite = __instance.Parent.flipX ? hatViewData.LeftMainImage : hatViewData.MainImage;
            }

            if (hatViewData.BackImage && hatViewData.LeftBackImage)
            {
                __instance.BackLayer.sprite = __instance.Parent.flipX ? hatViewData.LeftBackImage : hatViewData.BackImage;
                return false;
            }

            if (hatViewData.BackImage || __instance.Hat.InFront || !hatViewData.LeftMainImage)
            {
                return false;
            }
            __instance.BackLayer.sprite = __instance.Parent.flipX ? hatViewData.LeftMainImage : hatViewData.MainImage;
        }
        else if (__instance.FrontLayer.sprite == hatViewData.ClimbImage || __instance.FrontLayer.sprite == hatViewData.LeftClimbImage)
        {
            var spriteAnimNodeSync = __instance.SpriteSyncNode ?? __instance.GetComponent<SpriteAnimNodeSync>();
            if (spriteAnimNodeSync)
            {
                spriteAnimNodeSync.NodeId = 0;
            }
        }

        return false;
    }

    [HarmonyPatch(nameof(HatParent.SetFloorAnim))]
    [HarmonyPrefix]
    private static bool SetFloorAnimPrefix(HatParent __instance)
    {
        if (!__instance.TryGetCached(out var hatViewData))
        {
            return true;
        }
        __instance.BackLayer.enabled = false;
        __instance.FrontLayer.enabled = true;
        __instance.FrontLayer.sprite = hatViewData.FloorImage;
        return false;
    }

    [HarmonyPatch(nameof(HatParent.SetIdleAnim))]
    [HarmonyPrefix]
    private static bool SetIdleAnimPrefix(HatParent __instance, int colorId)
    {
        if (!__instance.Hat)
        {
            return false;
        }
        if (!__instance.IsCached())
        {
            return true;
        }
        __instance.viewAsset = null;
        __instance.PopulateFromViewData();
        __instance.SetMaterialColor(colorId);
        return false;
    }

    [HarmonyPatch(nameof(HatParent.SetClimbAnim))]
    [HarmonyPrefix]
    private static bool SetClimbAnimPrefix(HatParent __instance)
    {
        if (!__instance.TryGetCached(out var hatViewData))
        {
            return true;
        }
        if (!__instance.options.ShowForClimb)
        {
            return false;
        }
        __instance.BackLayer.enabled = false;
        __instance.FrontLayer.enabled = true;
        __instance.FrontLayer.sprite = hatViewData.ClimbImage;
        return false;
    }

    [HarmonyPatch(nameof(HatParent.PopulateFromViewData))]
    [HarmonyPrefix]
    private static bool PopulateFromHatViewDataPrefix(HatParent __instance)
    {
        if (!__instance.TryGetCached(out var asset))
        {
            return true;
        }
        __instance.UpdateMaterial();

        var spriteAnimNodeSync = __instance.SpriteSyncNode ? __instance.SpriteSyncNode : __instance.GetComponent<SpriteAnimNodeSync>();
        if (spriteAnimNodeSync)
        {
            spriteAnimNodeSync.NodeId = __instance.Hat.NoBounce ? 1 : 0;
        }

        if (__instance.Hat.InFront)
        {
            __instance.BackLayer.enabled = false;
            __instance.FrontLayer.enabled = true;
            __instance.FrontLayer.sprite = asset.MainImage;
        }
        else if (asset.BackImage)
        {
            __instance.BackLayer.enabled = true;
            __instance.FrontLayer.enabled = true;
            __instance.BackLayer.sprite = asset.BackImage;
            __instance.FrontLayer.sprite = asset.MainImage;
        }
        else
        {
            __instance.BackLayer.enabled = true;
            __instance.FrontLayer.enabled = false;
            __instance.FrontLayer.sprite = null;
            __instance.BackLayer.sprite = asset.MainImage;
        }

        if ( /*!__instance.options.Initialized ||*/ !__instance.HideHat())
        {
            return false;
        }
        __instance.FrontLayer.enabled = false;
        __instance.BackLayer.enabled = false;
        return false;
    }

    private static bool SetCustomHat(HatParent hatParent)
    {
        var dirPath = Path.Combine(CustomHatManager.HatsDirectory, "Test");
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        if (!FastDestroyableSingleton<TutorialManager>.InstanceExists)
        {
            return true;
        }
        DirectoryInfo d = new(dirPath);
        var files = d.GetFiles("*.png");
        var filePaths = new string[files.Length];
        for (var i = 0; i < files.Length; i++)
        {
            filePaths[i] = files[i].FullName;
        }
        var hats = CustomHatManager.CreateHatDetailsFromFileNames(filePaths, true);
        if (hats.Count <= 0)
        {
            return false;
        }
        try
        {
            hatParent.Hat = CustomHatManager.CreateHatBehaviour(hats[0], true);
        }
        catch (Exception err)
        {
            Logger.LogWarn($"Unable to create test hat \n{err}");
            return true;
        }

        return false;
    }
}