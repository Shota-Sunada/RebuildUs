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
    CommonAbilitySecondary,
}

public static class KeyBindingManager
{
    public class RebuildUsInput
    {
        public string Identifier { get; private set; }
        private readonly ConfigEntry<KeyCode> Config;
        public KeyCode Key { get; private set; }
        private readonly KeyCode DefaultKey;

        public RebuildUsInput(string identifier, KeyCode defaultKey, ConfigFile config)
        {
            this.Identifier = identifier;
            this.DefaultKey = defaultKey;
            this.Config = config.Bind("Keybindings", identifier, defaultKey);
            this.Key = this.Config.Value;
            AllInputs.Add(this);
        }

        public void ChangeKey(KeyCode newKey)
        {
            if (this.Key == newKey) return;
            this.Key = newKey;
            Config.Value = newKey;
        }

        public void Reset()
        {
            ChangeKey(DefaultKey);
        }
    }

    public static List<RebuildUsInput> AllInputs = [];

    public static RebuildUsInput CrewmateAbilityPrimary;
    public static RebuildUsInput CrewmateAbilitySecondary;
    public static RebuildUsInput ImpostorAbilityPrimary;
    public static RebuildUsInput ImpostorAbilitySecondary;
    public static RebuildUsInput NeutralAbilityPrimary;
    public static RebuildUsInput NeutralAbilitySecondary;
    public static RebuildUsInput CommonAbilityPrimary;
    public static RebuildUsInput CommonAbilitySecondary;

    public class KeyInputTexture
    {
        private readonly string Address;
        private Texture2D Texture = null;
        public Texture2D GetTexture()
        {
            if (Texture == null || !Texture)
            {
                // In RebuildUs, we use AssetLoader to get sprites/textures
                var sprite = AssetLoader.GetKeyBindTexture(Address);
                if (sprite != null) Texture = sprite.texture;
            }
            return Texture;
        }

        public KeyInputTexture(string address)
        {
            this.Address = address;
        }
    }

    public class KeyCodeData
    {
        public KeyCode KeyCode { get; private set; }
        public KeyInputTexture Texture { get; private set; }
        public int TextureNum { get; private set; }
        public string DisplayKey { get; private set; }
        private Sprite Sprite = null;
        public KeyCodeData(KeyCode keyCode, string displayKey, KeyInputTexture texture, int num)
        {
            this.KeyCode = keyCode;
            this.DisplayKey = displayKey;
            this.Texture = texture;
            this.TextureNum = num;

            AllKeyCodes[keyCode] = this;
        }

        public Sprite GetSprite()
        {
            if (Sprite == null || !Sprite)
            {
                var tex = Texture.GetTexture();
                if (tex != null)
                {
                    Sprite = Sprite.Create(tex, new Rect(0f, tex.height - 19f * (TextureNum + 1), 18f, 19f), new Vector2(0.5f, 0.5f), 100f);
                }
            }

            return Sprite;
        }
    }

    public static Dictionary<KeyCode, KeyCodeData> AllKeyCodes = [];

    public static void Initialize(ConfigFile config)
    {
        CrewmateAbilityPrimary = new RebuildUsInput(nameof(CrewmateAbilityPrimary), KeyCode.F, config);
        CrewmateAbilitySecondary = new RebuildUsInput(nameof(CrewmateAbilitySecondary), KeyCode.G, config);
        ImpostorAbilityPrimary = new RebuildUsInput(nameof(ImpostorAbilityPrimary), KeyCode.F, config);
        ImpostorAbilitySecondary = new RebuildUsInput(nameof(ImpostorAbilitySecondary), KeyCode.G, config);
        NeutralAbilityPrimary = new RebuildUsInput(nameof(NeutralAbilityPrimary), KeyCode.F, config);
        NeutralAbilitySecondary = new RebuildUsInput(nameof(NeutralAbilitySecondary), KeyCode.G, config);
        CommonAbilityPrimary = new RebuildUsInput(nameof(CommonAbilityPrimary), KeyCode.F, config);
        CommonAbilitySecondary = new RebuildUsInput(nameof(CommonAbilitySecondary), KeyCode.G, config);

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
        if (AllKeyCodes.TryGetValue(keyCode, out var data))
        {
            return data.GetSprite();
        }
        return null;
    }

    public static void Load()
    {
        KeyInputTexture kit;
        kit = new KeyInputTexture("KeyBindCharacters");
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

        kit = new KeyInputTexture("KeyBindCharacters0");
        _ = new KeyCodeData(KeyCode.UpArrow, "Up", kit, 0);
        _ = new KeyCodeData(KeyCode.DownArrow, "Down", kit, 1);
        _ = new KeyCodeData(KeyCode.LeftArrow, "Left", kit, 2);
        _ = new KeyCodeData(KeyCode.RightArrow, "Right", kit, 3);
        _ = new KeyCodeData(KeyCode.Tab, "Tab", kit, 4);
        _ = new KeyCodeData(KeyCode.Space, "Space", kit, 5);
        _ = new KeyCodeData(KeyCode.Backspace, "Back", kit, 6);
        _ = new KeyCodeData(KeyCode.Delete, "Del", kit, 7);

        kit = new KeyInputTexture("KeyBindCharacters1");
        for (KeyCode key = KeyCode.A; key <= KeyCode.Z; key++)
        {
            _ = new KeyCodeData(key, ((char)('A' + key - KeyCode.A)).ToString(), kit, key - KeyCode.A);
        }

        kit = new KeyInputTexture("KeyBindCharacters2");
        for (int i = 0; i < 12; i++)
        {
            _ = new KeyCodeData(KeyCode.F1 + i, "F" + (i + 1), kit, i);
        }
    }
}