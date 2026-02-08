namespace RebuildUs.Patches;

[HarmonyPatch]
public static class OptionsMenuBehaviourPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Update))]
    public static bool UpdatePrefix(OptionsMenuBehaviour __instance)
    {
        if (__instance.name == "KeyBindingMenu")
        {
            KeyBindingMenu.Update();
            return false;
        }

        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Start))]
    public static void StartPostfix(OptionsMenuBehaviour __instance)
    {
        if (__instance.name == "KeyBindingMenu") return;
        if (__instance.transform.parent && __instance.transform.parent.name == "KeyBindingMenu") return;

        ClientOptions.Start(__instance);
        KeyBindingMenu.Start(__instance);
    }
}
