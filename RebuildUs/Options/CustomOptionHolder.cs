using RebuildUs.Modules;

namespace RebuildUs.Options;

public static class CustomOptionHolder
{
    public static bool IsMapSelectionOption(CustomOption option)
    {
        // return option == CustomOptionHolder.propHuntMap && option == CustomOptionHolder.hideNSeekMap;
        return false;
    }

    public static string Cs(Color c, string s)
    {
        return string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>{4}</color>", ToByte(c.r), ToByte(c.g), ToByte(c.b), ToByte(c.a), s);
    }

    private static byte ToByte(float f)
    {
        f = Mathf.Clamp01(f);
        return (byte)(f * 255);
    }

    public static string[] Rates = ["0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%"];
    public static string[] Presets = ["Preset 1", "Preset 2", "Preset 3", "Preset 4", "Preset 5"];

    public static CustomOption PresetSelection;

    // Example foundational options
    public static CustomOption CrewmateRolesCountMin;
    public static CustomOption CrewmateRolesCountMax;
    public static CustomOption CrewmateRolesFill;
    public static CustomOption ImpostorRolesCountMin;
    public static CustomOption ImpostorRolesCountMax;
    public static CustomOption NeutralRolesCountMin;
    public static CustomOption NeutralRolesCountMax;
    public static CustomOption ModifiersCountMin;
    public static CustomOption ModifiersCountMax;
    public static CustomOption FinishTasksBeforeHauntingOrZoomingOut;
    public static CustomOption DeadImpsBlockSabotage;

    public static void Load()
    {
        PresetSelection = CustomOption.Create(0, CustomOption.CustomOptionType.General, "Preset", Presets, onChange: () =>
        {
            CustomOption.SwitchPreset(PresetSelection.Selection);
        });

        // --- General Settings ---
        CustomOption.Create(300, CustomOption.CustomOptionType.General, "Role Settings", true, isHeader: true);
        CrewmateRolesCountMin = CustomOption.Create(1, CustomOption.CustomOptionType.General, "Min Crewmate Roles", 0f, 0f, 15f, 1f);
        CrewmateRolesCountMax = CustomOption.Create(2, CustomOption.CustomOptionType.General, "Max Crewmate Roles", 0f, 0f, 15f, 1f);

        ImpostorRolesCountMin = CustomOption.Create(3, CustomOption.CustomOptionType.General, "Min Impostor Roles", 0f, 0f, 5f, 1f);
        ImpostorRolesCountMax = CustomOption.Create(4, CustomOption.CustomOptionType.General, "Max Impostor Roles", 0f, 0f, 5f, 1f);

        NeutralRolesCountMin = CustomOption.Create(5, CustomOption.CustomOptionType.General, "Min Neutral Roles", 0f, 0f, 10f, 1f);
        NeutralRolesCountMax = CustomOption.Create(6, CustomOption.CustomOptionType.General, "Max Neutral Roles", 0f, 0f, 10f, 1f);
    }
}