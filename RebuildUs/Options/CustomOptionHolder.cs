using RebuildUs.Modules;
using RebuildUs.Modules.CustomOptions;

namespace RebuildUs.Options;

public static class CustomOptionHolder
{
    public static string[] Rates = ["0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%"];
    public static string[] Presets = ["Preset 1", "Preset 2", "Preset 3", "Preset 4", "Preset 5"];

    public static CustomOption PresetSelection;

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
        PresetSelection = CustomOption.CreateHeader(0, CustomOption.CustomOptionType.General, "Preset", Presets, () =>
        {
            CustomOption.SwitchPreset(PresetSelection.Selection);
        });

    }
}