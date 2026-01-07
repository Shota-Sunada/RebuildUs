namespace RebuildUs.Patches;

[HarmonyPatch]
public static class TranslationControllerPatch
{
    [HarmonyPrefix]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), [typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>)])]
    public static bool GetStringPrefix(ref string __result, ref StringNames id)
    {
        return CustomOption.VeryShortPatch(ref __result, ref id);
    }
}