using AmongUs.Data.Legacy;

namespace RebuildUs.Modules;

internal abstract class CustomColors
{
    internal const int COLOR_BASE_ID_NUMBER = 50000;

    protected static Dictionary<int, string> ColorStrings = [];
    internal static List<int> LighterColors = [3, 4, 5, 7, 10, 11, 13, 14, 17];
    internal static uint PickableColors = (uint)Palette.ColorNames.Length;

    private static readonly int[] Order =
    [
        7, 37, 14, 5, 33, 41, 25,
        4, 30, 0, 35, 3, 27, 17,
        13, 23, 8, 32, 38, 1, 21,
        40, 31, 10, 34, 22, 28, 36,
        2, 11, 26, 29, 20, 19, 18,
        12, 9, 24, 16, 15, 6, 39,
    ];

    private static readonly StringBuilder ColorStringBuilder = new();

    private static bool _needsPatch;

    internal static void Load()
    {
        List<StringNames> longList = new();
        foreach (StringNames name in Palette.ColorNames) longList.Add(name);

        List<Color32> colorList = new();
        foreach (Color32 color in Palette.PlayerColors) colorList.Add(color);

        List<Color32> shadowList = new();
        foreach (Color32 shadow in Palette.ShadowColors) shadowList.Add(shadow);

        List<CustomColor> colors =
        [
            /* Custom Colors, starting with id (for ORDER) 18 */
            new()
            {
                NameKey = TrKey.Tamarind, //18
                Color = new(48, 28, 34, byte.MaxValue),
                Shadow = new(30, 11, 16, byte.MaxValue),
                IsLighterColor = true,
            },
            new()
            {
                NameKey = TrKey.Army, // 19
                Color = new(39, 45, 31, byte.MaxValue),
                Shadow = new(11, 30, 24, byte.MaxValue),
                IsLighterColor = false,
            },
            // 20
            new()
            {
                NameKey = TrKey.Olive,
                Color = new(154, 140, 61, byte.MaxValue),
                Shadow = new(104, 95, 40, byte.MaxValue),
                IsLighterColor = true,
            },
            new()
            {
                NameKey = TrKey.Turquoise,
                Color = new(22, 132, 176, byte.MaxValue),
                Shadow = new(15, 89, 117, byte.MaxValue),
                IsLighterColor = false,
            },
            new()
            {
                NameKey = TrKey.Mint,
                Color = new(111, 192, 156, byte.MaxValue),
                Shadow = new(65, 148, 111, byte.MaxValue),
                IsLighterColor = true,
            },
            new()
            {
                NameKey = TrKey.Lavender,
                Color = new(173, 126, 201, byte.MaxValue),
                Shadow = new(131, 58, 203, byte.MaxValue),
                IsLighterColor = true,
            },
            new()
            {
                NameKey = TrKey.Nougat,
                Color = new(160, 101, 56, byte.MaxValue),
                Shadow = new(115, 15, 78, byte.MaxValue),
                IsLighterColor = false,
            },
            // 25
            new()
            {
                NameKey = TrKey.Peach,
                Color = new(255, 164, 119, byte.MaxValue),
                Shadow = new(238, 128, 100, byte.MaxValue),
                IsLighterColor = true,
            },
            new()
            {
                NameKey = TrKey.Wasabi,
                Color = new(112, 143, 46, byte.MaxValue),
                Shadow = new(72, 92, 29, byte.MaxValue),
                IsLighterColor = false,
            },
            new()
            {
                NameKey = TrKey.HotPink,
                Color = new(255, 51, 102, byte.MaxValue),
                Shadow = new(232, 0, 58, byte.MaxValue),
                IsLighterColor = true,
            },
            new()
            {
                NameKey = TrKey.Petrol,
                Color = new(0, 99, 105, byte.MaxValue),
                Shadow = new(0, 61, 54, byte.MaxValue),
                IsLighterColor = false,
            },
            new()
            {
                NameKey = TrKey.Lemon,
                Color = new(0xDB, 0xFD, 0x2F, byte.MaxValue),
                Shadow = new(0x74, 0xE5, 0x10, byte.MaxValue),
                IsLighterColor = true,
            },
            // 30
            new()
            {
                NameKey = TrKey.SignalOrange,
                Color = new(0xF7, 0x44, 0x17, byte.MaxValue),
                Shadow = new(0x9B, 0x2E, 0x0F, byte.MaxValue),
                IsLighterColor = true,
            },
            new()
            {
                NameKey = TrKey.Teal,
                Color = new(0x25, 0xB8, 0xBF, byte.MaxValue),
                Shadow = new(0x12, 0x89, 0x86, byte.MaxValue),
                IsLighterColor = true,
            },
            new()
            {
                NameKey = TrKey.Blurple,
                Color = new(61, 44, 142, byte.MaxValue),
                Shadow = new(25, 14, 90, byte.MaxValue),
                IsLighterColor = false,
            },
            new()
            {
                NameKey = TrKey.Sunrise,
                Color = new(0xFF, 0xCA, 0x19, byte.MaxValue),
                Shadow = new(0xDB, 0x44, 0x42, byte.MaxValue),
                IsLighterColor = true,
            },
            new()
            {
                NameKey = TrKey.Ice,
                Color = new(0xA8, 0xDF, 0xFF, byte.MaxValue),
                Shadow = new(0x59, 0x9F, 0xC8, byte.MaxValue),
                IsLighterColor = true,
            },
            // 35
            new()
            {
                NameKey = TrKey.Fuchsia, //35 Color Credit: LaikosVK
                Color = new(164, 17, 129, byte.MaxValue),
                Shadow = new(104, 3, 79, byte.MaxValue),
                IsLighterColor = false,
            },
            new()
            {
                NameKey = TrKey.RoyalGreen, //36
                Color = new(9, 82, 33, byte.MaxValue),
                Shadow = new(0, 46, 8, byte.MaxValue),
                IsLighterColor = false,
            },
            new()
            {
                NameKey = TrKey.Slime,
                Color = new(244, 255, 188, byte.MaxValue),
                Shadow = new(167, 239, 112, byte.MaxValue),
                IsLighterColor = false,
            },
            new()
            {
                NameKey = TrKey.Navy, //38
                Color = new(9, 43, 119, byte.MaxValue),
                Shadow = new(0, 13, 56, byte.MaxValue),
                IsLighterColor = false,
            },
            new()
            {
                NameKey = TrKey.Darkness, //39
                Color = new(36, 39, 40, byte.MaxValue),
                Shadow = new(10, 10, 10, byte.MaxValue),
                IsLighterColor = false,
            },
            new()
            {
                NameKey = TrKey.Ocean, //40
                Color = new(55, 159, 218, byte.MaxValue),
                Shadow = new(62, 92, 158, byte.MaxValue),
                IsLighterColor = false,
            },
            new()
            {
                NameKey = TrKey.Sundown, // 41
                Color = new(252, 194, 100, byte.MaxValue),
                Shadow = new(197, 98, 54, byte.MaxValue),
                IsLighterColor = false,
            },
        ];
        PickableColors += (uint)colors.Count; // Colors to show in Tab

        int id = COLOR_BASE_ID_NUMBER;
        foreach (CustomColor cc in colors)
        {
            longList.Add((StringNames)id);
            ColorStrings[id++] = Tr.Get(cc.NameKey);
            colorList.Add(cc.Color);
            shadowList.Add(cc.Shadow);
            if (cc.IsLighterColor) LighterColors.Add(colorList.Count - 1);
        }

        Palette.ColorNames = longList.ToArray();
        Palette.PlayerColors = colorList.ToArray();
        Palette.ShadowColors = shadowList.ToArray();
    }

    internal static bool GetColorName(ref string __result, [HarmonyArgument(0)] StringNames name)
    {
        if ((int)name >= COLOR_BASE_ID_NUMBER && ColorStrings.TryGetValue((int)name, out string text))
        {
            if (text != null)
            {
                __result = text;
                return false;
            }
        }

        return true;
    }

    internal static bool ChatNotificationSetup(ChatNotification __instance, PlayerControl sender, string text)
    {
        if (MapUtilities.CachedShipStatus && !MapSettings.ShowChatNotifications) return false;

        __instance.timeOnScreen = 5f;
        __instance.gameObject.SetActive(true);
        __instance.SetCosmetics(sender.Data);
        string str;
        Color color;
        try
        {
            str = ColorUtility.ToHtmlStringRGB(Palette.TextColors[__instance.player.ColorId]);
            color = Palette.TextOutlineColors[__instance.player.ColorId];
        }
        catch
        {
            Color32 c = Palette.PlayerColors[__instance.player.ColorId];
            str = ColorUtility.ToHtmlStringRGB(c);

            color = c.r + c.g + c.b > 180 ? Palette.Black : Palette.White;
        }

        __instance.playerColorText.text = __instance.player.ColorBlindName;

        ColorStringBuilder.Clear();
        ColorStringBuilder.Append("<color=#").Append(str).Append('>');
        if (string.IsNullOrEmpty(sender.Data.PlayerName)) ColorStringBuilder.Append("...");
        else ColorStringBuilder.Append(sender.Data.PlayerName);

        string playerName = ColorStringBuilder.ToString();
        if (__instance.playerNameText.text != playerName) __instance.playerNameText.text = playerName;
        __instance.playerNameText.outlineColor = color;
        __instance.chatText.text = text;
        return false;
    }

    internal static void EnablePlayerTab(PlayerTab __instance)
    {
        // Replace instead
        Il2CppSystem.Collections.Generic.List<ColorChip> chips = __instance.ColorChips;

        const int cols = 7; // TODO: Design an algorithm to dynamically position chips to optimally fill space
        for (int i = 0; i < Order.Length; i++)
        {
            int pos = Order[i];
            if (pos < 0 || pos >= chips.Count)
                continue;
            ColorChip chip = chips[pos];
            int row = i / cols, col = i % cols; // Dynamically do the positioning
            chip.transform.localPosition = new(-0.975f + (col * 0.5f), 1.475f - (row * 0.5f), chip.transform.localPosition.z);
            chip.transform.localScale *= 0.76f;
        }

        for (int j = Order.Length; j < chips.Count; j++)
        {
            // If number isn't in order, hide it
            ColorChip chip = chips[j];
            chip.transform.localScale *= 0f;
            chip.enabled = false;
            chip.Button.enabled = false;
            chip.Button.OnClick.RemoveAllListeners();
        }
    }

    internal static void LoadPlayerPrefsPrefix([HarmonyArgument(0)] bool overrideLoad)
    {
        if (!LegacySaveManager.loaded || overrideLoad) _needsPatch = true;
    }

    internal static void LoadPlayerPrefsPostfix()
    {
        if (!_needsPatch) return;
        LegacySaveManager.colorConfig %= PickableColors;
        _needsPatch = false;
    }

    private static bool IsTaken(PlayerControl player, uint color)
    {
        foreach (NetworkedPlayerInfo p in GameData.Instance.AllPlayers.GetFastEnumerator())
            if (!p.Disconnected && p.PlayerId != player.PlayerId && p.DefaultOutfit.ColorId == color)
                return true;

        return false;
    }

    internal static bool CheckColor(PlayerControl __instance, [HarmonyArgument(0)] byte bodyColor)
    {
        // Fix incorrect color assignment
        uint color = bodyColor;
        if (IsTaken(__instance, color) || color >= Palette.PlayerColors.Length)
        {
            int num = 0;
            while (num++ < 50 && (color >= PickableColors || IsTaken(__instance, color))) color = (color + 1) % PickableColors;
        }

        __instance.RpcSetColor((byte)color);
        return false;
    }

    private struct CustomColor
    {
        internal TrKey NameKey;
        internal Color32 Color;
        internal Color32 Shadow;
        internal bool IsLighterColor;
    }
}