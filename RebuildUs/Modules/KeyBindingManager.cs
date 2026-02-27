namespace RebuildUs.Modules;

internal enum AbilitySlot
{
    CrewmateAbilityPrimary,
    CrewmateAbilitySecondary,
    ImpostorAbilityPrimary,
    ImpostorAbilitySecondary,
    NeutralAbilityPrimary,
    NeutralAbilitySecondary,
    CommonAbilityPrimary,
    CommonAbilitySecondary,
}

internal static class KeyBindingManager
{
    internal static List<RebuildUsInput> AllInputs = [];

    internal static RebuildUsInput CrewmateAbilityPrimary;
    internal static RebuildUsInput CrewmateAbilitySecondary;
    internal static RebuildUsInput ImpostorAbilityPrimary;
    internal static RebuildUsInput ImpostorAbilitySecondary;
    internal static RebuildUsInput NeutralAbilityPrimary;
    internal static RebuildUsInput NeutralAbilitySecondary;
    internal static RebuildUsInput CommonAbilityPrimary;
    internal static RebuildUsInput CommonAbilitySecondary;

    internal static Dictionary<KeyCode, KeyCodeData> AllKeyCodes = [];

    internal static void Initialize(ConfigFile config)
    {
        CrewmateAbilityPrimary = new(nameof(CrewmateAbilityPrimary), KeyCode.F, config);
        CrewmateAbilitySecondary = new(nameof(CrewmateAbilitySecondary), KeyCode.G, config);
        ImpostorAbilityPrimary = new(nameof(ImpostorAbilityPrimary), KeyCode.F, config);
        ImpostorAbilitySecondary = new(nameof(ImpostorAbilitySecondary), KeyCode.G, config);
        NeutralAbilityPrimary = new(nameof(NeutralAbilityPrimary), KeyCode.F, config);
        NeutralAbilitySecondary = new(nameof(NeutralAbilitySecondary), KeyCode.G, config);
        CommonAbilityPrimary = new(nameof(CommonAbilityPrimary), KeyCode.F, config);
        CommonAbilitySecondary = new(nameof(CommonAbilitySecondary), KeyCode.G, config);

        Load();
    }

    internal static KeyCode GetKey(AbilitySlot slot)
    {
        return slot switch
        {
            AbilitySlot.CrewmateAbilityPrimary => CrewmateAbilityPrimary.Key,
            AbilitySlot.CrewmateAbilitySecondary => CrewmateAbilitySecondary.Key,
            AbilitySlot.ImpostorAbilityPrimary => ImpostorAbilityPrimary.Key,
            AbilitySlot.ImpostorAbilitySecondary => ImpostorAbilitySecondary.Key,
            AbilitySlot.NeutralAbilityPrimary => NeutralAbilityPrimary.Key,
            AbilitySlot.NeutralAbilitySecondary => NeutralAbilitySecondary.Key,
            AbilitySlot.CommonAbilityPrimary => CommonAbilityPrimary.Key,
            AbilitySlot.CommonAbilitySecondary => CommonAbilitySecondary.Key,
            _ => KeyCode.None,
        };
    }

    internal static Sprite GetKeySprite(KeyCode keyCode)
    {
        return AllKeyCodes.TryGetValue(keyCode, out KeyCodeData data) ? data.GetSprite() : null;
    }

    internal static void Load()
    {
        KeyInputTexture kit = new("KeyBindCharacters");
        for (int i = 0; i < 10; i++)
        {
            _ = new KeyCodeData(KeyCode.Alpha0 + i, i.ToString(), kit, i);
        }

        _ = new KeyCodeData(KeyCode.Mouse0, "Mouse Left", kit, 10);
        _ = new KeyCodeData(KeyCode.Mouse1, "Mouse Right", kit, 11);
        _ = new KeyCodeData(KeyCode.Mouse2, "Mouse Middle", kit, 12);
        _ = new KeyCodeData(KeyCode.LeftShift, "Shift", kit, 13);
        _ = new KeyCodeData(KeyCode.LeftControl, "Ctrl", kit, 14);
        _ = new KeyCodeData(KeyCode.LeftAlt, "Alt", kit, 15);

        kit = new("KeyBindCharacters0");
        _ = new KeyCodeData(KeyCode.UpArrow, "Up", kit, 0);
        _ = new KeyCodeData(KeyCode.DownArrow, "Down", kit, 1);
        _ = new KeyCodeData(KeyCode.LeftArrow, "Left", kit, 2);
        _ = new KeyCodeData(KeyCode.RightArrow, "Right", kit, 3);
        _ = new KeyCodeData(KeyCode.Tab, "Tab", kit, 4);
        _ = new KeyCodeData(KeyCode.Space, "Space", kit, 5);
        _ = new KeyCodeData(KeyCode.Backspace, "Back", kit, 6);
        _ = new KeyCodeData(KeyCode.Delete, "Del", kit, 7);

        kit = new("KeyBindCharacters1");
        for (KeyCode key = KeyCode.A; key <= KeyCode.Z; key++)
        {
            _ = new KeyCodeData(key, ((char)('A' + key - KeyCode.A)).ToString(), kit, key - KeyCode.A);
        }

        kit = new("KeyBindCharacters2");
        for (int i = 0; i < 12; i++)
        {
            _ = new KeyCodeData(KeyCode.F1 + i, "F" + (i + 1), kit, i);
        }
    }

    internal sealed class RebuildUsInput
    {
        private readonly ConfigEntry<KeyCode> _config;
        private readonly KeyCode _defaultKey;

        internal RebuildUsInput(string identifier, KeyCode defaultKey, ConfigFile config)
        {
            Identifier = identifier;
            _defaultKey = defaultKey;
            _config = config.Bind("Keybindings", identifier, defaultKey);
            Key = _config.Value;
            AllInputs.Add(this);
        }

        internal string Identifier { get; private set; }
        internal KeyCode Key { get; private set; }

        internal void ChangeKey(KeyCode newKey)
        {
            if (Key == newKey)
            {
                return;
            }
            Key = newKey;
            _config.Value = newKey;
        }

        internal void Reset()
        {
            ChangeKey(_defaultKey);
        }
    }

    internal sealed class KeyInputTexture
    {
        private readonly string _address;
        private Texture2D _texture;

        internal KeyInputTexture(string address)
        {
            _address = address;
        }

        internal Texture2D GetTexture()
        {
            if (_texture == null || !_texture)
            {
                // In RebuildUs, we use AssetLoader to get sprites/textures
                Sprite sprite = AssetLoader.GetKeyBindTexture(_address);
                if (sprite != null)
                {
                    _texture = sprite.texture;
                }
            }

            return _texture;
        }
    }

    internal sealed class KeyCodeData
    {
        private Sprite _sprite;

        internal KeyCodeData(KeyCode keyCode, string displayKey, KeyInputTexture texture, int num)
        {
            KeyCode = keyCode;
            DisplayKey = displayKey;
            Texture = texture;
            TextureNum = num;

            AllKeyCodes[keyCode] = this;
        }

        internal KeyCode KeyCode { get; private set; }
        private KeyInputTexture Texture { get; }
        private int TextureNum { get; }
        internal string DisplayKey { get; private set; }

        internal Sprite GetSprite()
        {
            if (_sprite != null && _sprite)
            {
                return _sprite;
            }
            Texture2D tex = Texture.GetTexture();
            if (tex != null)
            {
                _sprite = Sprite.Create(tex, new(0f, tex.height - 19f * (TextureNum + 1), 18f, 19f), new(0.5f, 0.5f), 100f);
            }

            return _sprite;
        }
    }
}