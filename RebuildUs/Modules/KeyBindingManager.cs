namespace RebuildUs.Modules;

public enum AbilitySlot
{
    CrewmateAbilityPrimary,
    CrewmateAbilitySecondary,
    ImpostorAbilityPrimary,
    ImpostorAbilitySecondary,
    NeutralAbilityPrimary,
    NeutralAbilitySecondary,
    CommonAbilityPrimary,
    CommonAbilitySecondary
}

public static class KeyBindingManager
{
    public static List<RebuildUsInput> AllInputs = [];

    public static RebuildUsInput CrewmateAbilityPrimary;
    public static RebuildUsInput CrewmateAbilitySecondary;
    public static RebuildUsInput ImpostorAbilityPrimary;
    public static RebuildUsInput ImpostorAbilitySecondary;
    public static RebuildUsInput NeutralAbilityPrimary;
    public static RebuildUsInput NeutralAbilitySecondary;
    public static RebuildUsInput CommonAbilityPrimary;
    public static RebuildUsInput CommonAbilitySecondary;

    public static Dictionary<KeyCode, KeyCodeData> AllKeyCodes = [];

    public static void Initialize(ConfigFile config)
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

    public static KeyCode GetKey(AbilitySlot slot)
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

    public static Sprite GetKeySprite(KeyCode keyCode)
    {
        if (AllKeyCodes.TryGetValue(keyCode, out var data)) return data.GetSprite();
        return null;
    }

    public static void Load()
    {
        KeyInputTexture kit;
        kit = new("KeyBindCharacters");
        for (var i = 0; i < 10; i++) _ = new KeyCodeData(KeyCode.Alpha0 + i, i.ToString(), kit, i);
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
        for (var key = KeyCode.A; key <= KeyCode.Z; key++) _ = new KeyCodeData(key, ((char)(('A' + key) - KeyCode.A)).ToString(), kit, key - KeyCode.A);

        kit = new("KeyBindCharacters2");
        for (var i = 0; i < 12; i++) _ = new KeyCodeData(KeyCode.F1 + i, "F" + (i + 1), kit, i);
    }

    public sealed class RebuildUsInput
    {
        private readonly ConfigEntry<KeyCode> _config;
        private readonly KeyCode _defaultKey;

        public RebuildUsInput(string identifier, KeyCode defaultKey, ConfigFile config)
        {
            Identifier = identifier;
            _defaultKey = defaultKey;
            _config = config.Bind("Keybindings", identifier, defaultKey);
            Key = _config.Value;
            AllInputs.Add(this);
        }

        public string Identifier { get; private set; }
        public KeyCode Key { get; private set; }

        public void ChangeKey(KeyCode newKey)
        {
            if (Key == newKey) return;
            Key = newKey;
            _config.Value = newKey;
        }

        public void Reset()
        {
            ChangeKey(_defaultKey);
        }
    }

    public sealed class KeyInputTexture
    {
        private readonly string _address;
        private Texture2D _texture;

        public KeyInputTexture(string address)
        {
            _address = address;
        }

        public Texture2D GetTexture()
        {
            if (_texture == null || !_texture)
            {
                // In RebuildUs, we use AssetLoader to get sprites/textures
                var sprite = AssetLoader.GetKeyBindTexture(_address);
                if (sprite != null) _texture = sprite.texture;
            }

            return _texture;
        }
    }

    public sealed class KeyCodeData
    {
        private Sprite _sprite;

        public KeyCodeData(KeyCode keyCode, string displayKey, KeyInputTexture texture, int num)
        {
            KeyCode = keyCode;
            DisplayKey = displayKey;
            Texture = texture;
            TextureNum = num;

            AllKeyCodes[keyCode] = this;
        }

        public KeyCode KeyCode { get; private set; }
        public KeyInputTexture Texture { get; }
        public int TextureNum { get; }
        public string DisplayKey { get; private set; }

        public Sprite GetSprite()
        {
            if (_sprite == null || !_sprite)
            {
                var tex = Texture.GetTexture();
                if (tex != null) _sprite = Sprite.Create(tex, new(0f, tex.height - (19f * (TextureNum + 1)), 18f, 19f), new(0.5f, 0.5f), 100f);
            }

            return _sprite;
        }
    }
}
