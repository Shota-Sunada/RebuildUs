using System.Collections;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using RebuildUs.Modules.Cosmetics;

namespace RebuildUs.Patches;

[HarmonyPatch(typeof(SplashManager), nameof(SplashManager.Update))]
public static class SplashManagerPatch
{
    private static bool _isProcessing;
    private static bool _isDone;

    [HarmonyPrefix]
    public static bool Prefix(SplashManager __instance)
    {
        if (_isDone) return true;

        if (__instance.doneLoadingRefdata && !__instance.startedSceneLoad && Time.time - __instance.startTime > __instance.minimumSecondsBeforeSceneChange)
        {
            if (!_isProcessing)
            {
                _isProcessing = true;
                __instance.StartCoroutine(CoProcess(__instance).WrapToIl2Cpp());
            }

            return false;
        }

        return true;
    }

    private static IEnumerator CoProcess(SplashManager __instance)
    {
        CustomHatManager.LoadHats();

        while (CustomHatManager.LOADER.IsRunning) yield return null;

        _isDone = true;
    }
}
