namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class KeyboardJoystickPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
    internal static void Update(KeyboardJoystick __instance)
    {
        Tr.Update();
        ShortcutCommands.HostCommands();
        ShortcutCommands.DebugCommands();
        ShortcutCommands.OpenAirshipToilet();
    }
}
