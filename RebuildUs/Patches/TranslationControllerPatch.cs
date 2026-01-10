using System.CodeDom.Compiler;
using RebuildUs.Modules;

namespace RebuildUs.Patches;

[HarmonyPatch]
public static class TranslationControllerPatch
{
    [HarmonyPrefix]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), [typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>)])]
    public static bool GetStringPrefix(ref string __result, ref StringNames id)
    {
        return true;
    }

    [HarmonyPrefix]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), [typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>)])]
    public static bool GetColorNamePrefix(ref string __result, [HarmonyArgument(0)] StringNames name)
    {
        if (!CustomColors.GetColorName(ref __result, name))
        {
            return false;
        }

        return true;
    }
}