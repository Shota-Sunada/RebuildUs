namespace RebuildUs.Patches;

public static class AbilityButtonPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static bool DoClickPrefix(AbilityButton __instance)
    {
        if (MapSettings.GameMode is not CustomGameMode.Roles && !HotPotato.hotPotato) return false;

        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.Update))]
    public static void Postfix()
    {

        if (MapSettings.GameMode is not CustomGameMode.Roles && !HotPotato.hotPotato)
        {
            HudManager.Instance.AbilityButton.Hide();
        }
    }
}