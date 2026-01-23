using System.Collections;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using UnityEngine;
using HarmonyLib;
using RebuildUs.Modules.Cosmetics;

namespace RebuildUs.Patches;

[HarmonyPatch(typeof(SplashManager), nameof(SplashManager.Update))]
public static class SplashManagerPatch
{
    private static bool IsProcessing = false;
    private static bool IsDone = false;

    [HarmonyPrefix]
    public static bool Prefix(SplashManager __instance)
    {
        if (IsDone) return true;

        if (__instance.doneLoadingRefdata && !__instance.startedSceneLoad && Time.time - __instance.startTime > __instance.minimumSecondsBeforeSceneChange)
        {
            if (!IsProcessing)
            {
                IsProcessing = true;
                __instance.StartCoroutine(CoProcess(__instance).WrapToIl2Cpp());
            }
            return false;
        }

        return true;
    }

    private static IEnumerator CoProcess(SplashManager __instance)
    {
        CustomHatManager.LoadHats();

        while (CustomHatManager.Loader.IsRunning)
        {
            yield return null;
        }

        IsDone = true;
    }
}
