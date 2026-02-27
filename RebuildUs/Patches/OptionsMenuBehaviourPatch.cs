namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class OptionsMenuBehaviourPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Update))]
    internal static bool UpdatePrefix(OptionsMenuBehaviour __instance)
    {
        if (__instance.name != "KeyBindingMenu")
        {
            return true;
        }
        KeyBindingMenu.Update();
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Start))]
    internal static void StartPostfix(OptionsMenuBehaviour __instance)
    {
        if (__instance.name == "KeyBindingMenu")
        {
            return;
        }
        if (__instance.transform.parent && __instance.transform.parent.name == "KeyBindingMenu")
        {
            return;
        }

        ClientOptions.Start(__instance);
        KeyBindingMenu.Start(__instance);
    }
}