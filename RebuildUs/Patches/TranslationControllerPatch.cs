namespace RebuildUs.Patches;

[HarmonyPatch]
public static class TranslationControllerPatch
{
    [HarmonyPrefix]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), [typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>)])]
    public static bool GetStringPrefix(ref string __result, StringNames id)
    {
        if ((int)id < CustomOption.CUSTOM_OPTION_PRE_ID)
        {
            return true;
        }
        string ourString = "";

        // For now only do this in custom options.
        int idInt = (int)id - CustomOption.CUSTOM_OPTION_PRE_ID;
        var opt = CustomOption.AllOptions.FirstOrDefault(x => x.Id == idInt);
        ourString = opt?.NameKey;

        __result = ourString;
        return false;
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

    [HarmonyPostfix]
    [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), [typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>)])]
    public static void GetStringPostfix(ref string __result, [HarmonyArgument(0)] StringNames id)
    {
        Exile.ExileMessage(ref __result, id);
    }
}