namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class VentButtonPatch
{
    private static Sprite _defaultVentSprite;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(VentButton), nameof(VentButton.SetTarget))]
    internal static void SetTargetPostfix(VentButton __instance)
    {
        // Trickster render special vent button
        PlayerControl lp = PlayerControl.LocalPlayer;
        if (!lp.IsRole(RoleType.Trickster)) return;
        if (_defaultVentSprite == null) _defaultVentSprite = __instance.graphic.sprite;
        bool isSpecialVent = __instance.currentTarget != null && __instance.currentTarget.gameObject != null && __instance.currentTarget.gameObject.name.StartsWith("JackInTheBoxVent_");
        __instance.graphic.sprite = isSpecialVent ? AssetLoader.TricksterVentButton : _defaultVentSprite;
        __instance.buttonLabelText.enabled = !isSpecialVent;
    }
}