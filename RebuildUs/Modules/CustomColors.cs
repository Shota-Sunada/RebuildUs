using AmongUs.Data.Legacy;

namespace RebuildUs.Modules;

public class CustomColors
{
    protected static Dictionary<int, string> ColorStrings = [];
    public static List<int> LighterColors = [3, 4, 5, 7, 10, 11, 13, 14, 17];
    public static uint PickableColors = (uint)Palette.ColorNames.Length;
    private static readonly int[] ORDER =
    [
        7, 37, 14, 5, 33, 41, 25,
        4, 30, 0, 35, 3, 27, 17,
        13, 23, 8, 32, 38, 1, 21,
        40, 31, 10, 34, 22, 28, 36,
        2, 11, 26, 29, 20, 19, 18,
        12, 9, 24, 16, 15, 6, 39,
    ];

    public static void Load()
    {
        List<StringNames> longList = Enumerable.ToList(Palette.ColorNames);
        List<Color32> colorList = Enumerable.ToList(Palette.PlayerColors);
        List<Color32> shadowList = Enumerable.ToList(Palette.ShadowColors);

        List<CustomColor> colors =
        [
            /* Custom Colors, starting with id (for ORDER) 18 */
            new CustomColor
            {
                NameKey = "Color.Tamarind", //18
                Color = new Color32(48, 28, 34, byte.MaxValue),
                Shadow = new Color32(30, 11, 16, byte.MaxValue),
                IsLighterColor = true
            },
            new CustomColor
            {
                NameKey = "Color.Army", // 19
                Color = new Color32(39, 45, 31, byte.MaxValue),
                Shadow = new Color32(11, 30, 24, byte.MaxValue),
                IsLighterColor = false
            },
            // 20
            new CustomColor
            {
                NameKey = "Color.Olive",
                Color = new Color32(154, 140, 61, byte.MaxValue),
                Shadow = new Color32(104, 95, 40, byte.MaxValue),
                IsLighterColor = true
            },
            new CustomColor
            {
                NameKey = "Color.Turquoise",
                Color = new Color32(22, 132, 176, byte.MaxValue),
                Shadow = new Color32(15, 89, 117, byte.MaxValue),
                IsLighterColor = false
            },
            new CustomColor
            {
                NameKey = "Color.Mint",
                Color = new Color32(111, 192, 156, byte.MaxValue),
                Shadow = new Color32(65, 148, 111, byte.MaxValue),
                IsLighterColor = true
            },
            new CustomColor
            {
                NameKey = "Color.Lavender",
                Color = new Color32(173, 126, 201, byte.MaxValue),
                Shadow = new Color32(131, 58, 203, byte.MaxValue),
                IsLighterColor = true
            },
            new CustomColor
            {
                NameKey = "Color.Nougat",
                Color = new Color32(160, 101, 56, byte.MaxValue),
                Shadow = new Color32(115, 15, 78, byte.MaxValue),
                IsLighterColor = false
            },
            // 25
            new CustomColor
            {
                NameKey = "Color.Peach",
                Color = new Color32(255, 164, 119, byte.MaxValue),
                Shadow = new Color32(238, 128, 100, byte.MaxValue),
                IsLighterColor = true
            },
            new CustomColor
            {
                NameKey = "Color.Wasabi",
                Color = new Color32(112, 143, 46, byte.MaxValue),
                Shadow = new Color32(72, 92, 29, byte.MaxValue),
                IsLighterColor = false
            },
            new CustomColor
            {
                NameKey = "Color.HotPink",
                Color = new Color32(255, 51, 102, byte.MaxValue),
                Shadow = new Color32(232, 0, 58, byte.MaxValue),
                IsLighterColor = true
            },
            new CustomColor
            {
                NameKey = "Color.Petrol",
                Color = new Color32(0, 99, 105, byte.MaxValue),
                Shadow = new Color32(0, 61, 54, byte.MaxValue),
                IsLighterColor = false
            },
            new CustomColor
            {
                NameKey = "Color.Lemon",
                Color = new Color32(0xDB, 0xFD, 0x2F, byte.MaxValue),
                Shadow = new Color32(0x74, 0xE5, 0x10, byte.MaxValue),
                IsLighterColor = true
            },
            // 30
            new CustomColor
            {
                NameKey = "Color.SignalOrange",
                Color = new Color32(0xF7, 0x44, 0x17, byte.MaxValue),
                Shadow = new Color32(0x9B, 0x2E, 0x0F, byte.MaxValue),
                IsLighterColor = true
            },
            new CustomColor
            {
                NameKey = "Color.Teal",
                Color = new Color32(0x25, 0xB8, 0xBF, byte.MaxValue),
                Shadow = new Color32(0x12, 0x89, 0x86, byte.MaxValue),
                IsLighterColor = true
            },
            new CustomColor
            {
                NameKey = "Color.Blurple",
                Color = new Color32(61, 44, 142, byte.MaxValue),
                Shadow = new Color32(25, 14, 90, byte.MaxValue),
                IsLighterColor = false
            },
            new CustomColor
            {
                NameKey = "Color.Sunrise",
                Color = new Color32(0xFF, 0xCA, 0x19, byte.MaxValue),
                Shadow = new Color32(0xDB, 0x44, 0x42, byte.MaxValue),
                IsLighterColor = true
            },
            new CustomColor
            {
                NameKey = "Color.Ice",
                Color = new Color32(0xA8, 0xDF, 0xFF, byte.MaxValue),
                Shadow = new Color32(0x59, 0x9F, 0xC8, byte.MaxValue),
                IsLighterColor = true
            },
            // 35
            new CustomColor
            {
                NameKey = "Color.Fuchsia", //35 Color Credit: LaikosVK
                Color = new Color32(164, 17, 129, byte.MaxValue),
                Shadow = new Color32(104, 3, 79, byte.MaxValue),
                IsLighterColor = false
            },
            new CustomColor
            {
                NameKey = "Color.RoyalGreen", //36
                Color = new Color32(9, 82, 33, byte.MaxValue),
                Shadow = new Color32(0, 46, 8, byte.MaxValue),
                IsLighterColor = false
            },
            new CustomColor
            {
                NameKey = "Color.Slime",
                Color = new Color32(244, 255, 188, byte.MaxValue),
                Shadow = new Color32(167, 239, 112, byte.MaxValue),
                IsLighterColor = false
            },
            new CustomColor
            {
                NameKey = "Color.Navy", //38
                Color = new Color32(9, 43, 119, byte.MaxValue),
                Shadow = new Color32(0, 13, 56, byte.MaxValue),
                IsLighterColor = false
            },
            new CustomColor
            {
                NameKey = "Color.Darkness", //39
                Color = new Color32(36, 39, 40, byte.MaxValue),
                Shadow = new Color32(10, 10, 10, byte.MaxValue),
                IsLighterColor = false
            },
            new CustomColor
            {
                NameKey = "Color.Ocean", //40
                Color = new Color32(55, 159, 218, byte.MaxValue),
                Shadow = new Color32(62, 92, 158, byte.MaxValue),
                IsLighterColor = false
            },
            new CustomColor
            {
                NameKey = "Color.Sundown", // 41
                Color = new Color32(252, 194, 100, byte.MaxValue),
                Shadow = new Color32(197, 98, 54, byte.MaxValue),
                IsLighterColor = false
            },
        ];
        PickableColors += (uint)colors.Count; // Colors to show in Tab

        /** Hidden Colors **/
        /** Add Colors **/
        int id = 50000;
        foreach (CustomColor cc in colors)
        {
            longList.Add((StringNames)id);
            ColorStrings[id++] = Tr.Get(cc.NameKey);
            colorList.Add(cc.Color);
            shadowList.Add(cc.Shadow);
            if (cc.IsLighterColor)
            {
                LighterColors.Add(colorList.Count - 1);
            }
        }

        Palette.ColorNames = longList.ToArray();
        Palette.PlayerColors = colorList.ToArray();
        Palette.ShadowColors = shadowList.ToArray();
    }

    protected internal struct CustomColor
    {
        public string NameKey;
        public Color32 Color;
        public Color32 Shadow;
        public bool IsLighterColor;
    }

    public static bool GetColorName(ref string __result, [HarmonyArgument(0)] StringNames name)
    {
        if ((int)name >= 50000)
        {
            string text = ColorStrings[(int)name];
            if (text != null)
            {
                __result = text;
                return false;
            }
        }
        return true;
    }

    public static bool ChatNotificationSetup(ChatNotification __instance, PlayerControl sender, string text)
    {
        if (ShipStatus.Instance && !ModMapOptions.ShowChatNotifications)
        {
            return false;
        }
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
        __instance.playerNameText.text = "<color=#" + str + ">" + (string.IsNullOrEmpty(sender.Data.PlayerName) ? "..." : sender.Data.PlayerName);
        __instance.playerNameText.outlineColor = color;
        __instance.chatText.text = text;
        return false;
    }

    public static void EnablePlayerTab(PlayerTab __instance)
    { // Replace instead
        Il2CppArrayBase<ColorChip> chips = __instance.ColorChips.ToArray();

        int cols = 7; // TODO: Design an algorithm to dynamically position chips to optimally fill space
        for (int i = 0; i < ORDER.Length; i++)
        {
            int pos = ORDER[i];
            if (pos < 0 || pos > chips.Length)
                continue;
            ColorChip chip = chips[pos];
            int row = i / cols, col = i % cols; // Dynamically do the positioning
            chip.transform.localPosition = new Vector3(-0.975f + (col * 0.5f), 1.475f - (row * 0.5f), chip.transform.localPosition.z);
            chip.transform.localScale *= 0.76f;
        }
        for (int j = ORDER.Length; j < chips.Length; j++)
        { // If number isn't in order, hide it
            ColorChip chip = chips[j];
            chip.transform.localScale *= 0f;
            chip.enabled = false;
            chip.Button.enabled = false;
            chip.Button.OnClick.RemoveAllListeners();
        }
    }

    private static bool NeedsPatch = false;
    public static void LoadPlayerPrefsPrefix([HarmonyArgument(0)] bool overrideLoad)
    {
        if (!LegacySaveManager.loaded || overrideLoad)
        {
            NeedsPatch = true;
        }
    }
    public static void LoadPlayerPrefsPostfix()
    {
        if (!NeedsPatch) return;
        LegacySaveManager.colorConfig %= PickableColors;
        NeedsPatch = false;
    }

    private static bool IsTaken(PlayerControl player, uint color)
    {
        foreach (var p in GameData.Instance.AllPlayers.GetFastEnumerator())
        {
            if (!p.Disconnected && p.PlayerId != player.PlayerId && p.DefaultOutfit.ColorId == color)
            {
                return true;
            }
        }

        return false;
    }

    public static bool CheckColor(PlayerControl __instance, [HarmonyArgument(0)] byte bodyColor)
    {
        // Fix incorrect color assignment
        uint color = bodyColor;
        if (IsTaken(__instance, color) || color >= Palette.PlayerColors.Length)
        {
            int num = 0;
            while (num++ < 50 && (color >= PickableColors || IsTaken(__instance, color)))
            {
                color = (color + 1) % PickableColors;
            }
        }
        __instance.RpcSetColor((byte)color);
        return false;
    }
}