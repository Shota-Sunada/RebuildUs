namespace RebuildUs.Modules;

public enum AbilitySlot
{
    Ability1,
    Ability2,
    Ability3,
}

public static class KeyBindingManager
{
    private static ConfigEntry<KeyCode> Ability1Key;
    private static ConfigEntry<KeyCode> Ability2Key;
    private static ConfigEntry<KeyCode> Ability3Key;

    public static void Initialize(ConfigFile config)
    {
        Ability1Key = config.Bind("Keybindings", "Ability 1", KeyCode.F, "Key to use Ability 1 button.");
        Ability2Key = config.Bind("Keybindings", "Ability 2", KeyCode.Q, "Key to use Ability 2 button.");
        Ability3Key = config.Bind("Keybindings", "Ability 3", KeyCode.G, "Key to use Ability 3 button.");
    }

    public static KeyCode GetKey(AbilitySlot slot)
    {
        return slot switch
        {
            AbilitySlot.Ability1 => Ability1Key.Value,
            AbilitySlot.Ability2 => Ability2Key.Value,
            AbilitySlot.Ability3 => Ability3Key.Value,
            _ => KeyCode.None,
        };
    }
}
