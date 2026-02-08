namespace RebuildUs.Patches;

[HarmonyPatch]
public static class KeyboardJoystickPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
    public static void Update(KeyboardJoystick __instance)
    {
        Tr.Update();
        ShortcutCommands.HostCommands();
        ShortcutCommands.OpenAirshipToilet();
    }
}
