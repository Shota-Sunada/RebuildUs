namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class UseButtonPatch
{
    private static readonly int Desat = Shader.PropertyToID("_Desat");

    [HarmonyPrefix]
    [HarmonyPatch(typeof(UseButton), nameof(UseButton.SetTarget))]
    internal static bool SetTargetPrefix(UseButton __instance, [HarmonyArgument(0)] IUsable target)
    {
        var pc = PlayerControl.LocalPlayer;
        __instance.enabled = true;

        if (Helpers.IsBlocked(target, pc))
        {
            __instance.currentTarget = null;
            __instance.buttonLabelText.text = Tr.Get(TrKey.ButtonBlocked);
            __instance.enabled = false;
            __instance.graphic.color = Palette.DisabledClear;
            __instance.graphic.material.SetFloat(Desat, 0f);
            return false;
        }

        __instance.currentTarget = target;
        return true;
    }
}