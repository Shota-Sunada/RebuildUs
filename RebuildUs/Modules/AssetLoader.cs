using System.Reflection;
using Object = UnityEngine.Object;

namespace RebuildUs.Modules;

internal static class AssetLoader
{
    private static bool _isLoaded;

    internal static void LoadAssets()
    {
        if (_isLoaded) return;
        _isLoaded = true;

        LoadButtonAssets();
        LoadAnimationAssets();
        LoadSpriteAssets();
        LoadLocationAssets();
        LoadKeyBindAssets();
    }

    private static byte[] ReadFully(this Stream input)
    {
        using MemoryStream ms = new();
        input.CopyTo(ms);
        return ms.ToArray();
    }

#nullable enable
    private static T? LoadAsset<T>(this AssetBundle assetBundle, string name) where T : Object
    {
        return assetBundle.LoadAsset(name, Il2CppType.Of<T>())?.Cast<T>() ?? throw new($"The asset was not found: {name}");
    }
#nullable disable

    private static Sprite Resize(this Sprite sprite, float pixelsPerUnit)
    {
        if (Mathf.Approximately(pixelsPerUnit, 100f)) return sprite.DontUnload();

        return sprite == null ? null : Sprite.Create(sprite.texture, sprite.rect, new(sprite.pivot.x / sprite.rect.width, sprite.pivot.y / sprite.rect.height), pixelsPerUnit, 0, SpriteMeshType.FullRect, sprite.border).DontUnload();
    }

    private static T DontUnload<T>(this T obj) where T : Object
    {
        obj.hideFlags |= HideFlags.DontUnloadUnusedAsset;

        return obj;
    }

#region Animations

    internal static readonly List<Sprite> TricksterAnimations = [];

    private static void LoadAnimationAssets()
    {
        Stream resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("RebuildUs.Resources.tricksteranimation");
        AssetBundle ab = AssetBundle.LoadFromMemory(resource.ReadFully());

        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0001.png").Resize(175f));
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0002.png").Resize(175f));
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0003.png").Resize(175f));
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0004.png").Resize(175f));
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0005.png").Resize(175f));
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0006.png").Resize(175f));
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0007.png").Resize(175f));
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0008.png").Resize(175f));
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0009.png").Resize(175f));
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0010.png").Resize(175f));
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0011.png").Resize(175f));
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0012.png").Resize(175f));
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0013.png").Resize(175f));
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0014.png").Resize(175f));
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0015.png").Resize(175f));
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0016.png").Resize(175f));
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0017.png").Resize(175f));
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0018.png").Resize(175f));
    }

#endregion

#region Buttons

    internal static Sprite DouseButton;
    internal static Sprite EmergencyButton;
    internal static Sprite VultureButton;
    internal static Sprite SidekickButton;
    internal static Sprite IgniteButton;
    internal static Sprite HackerButton;
    internal static Sprite LighterButton;
    internal static Sprite ShieldButton;
    internal static Sprite MediumButton;
    internal static Sprite TimeShieldButton;
    internal static Sprite TrackerButton;
    internal static Sprite PathfindButton;
    internal static Sprite EraserButton;
    internal static Sprite CleanButton;
    internal static Sprite PlaceJackInTheBoxButton;
    internal static Sprite LightsOutButton;
    internal static Sprite PlaceCameraButton;
    internal static Sprite CloseVentButton;
    internal static Sprite CurseButton;
    internal static Sprite CurseKillButton;
    internal static Sprite MorphButton;
    internal static Sprite SampleButton;
    internal static Sprite SpellButton;
    internal static Sprite SpellButtonMeeting;
    internal static Sprite CamouflageButton;
    internal static Sprite SwapperCheck;
    internal static Sprite GarlicButton;
    internal static Sprite VampireButton;
    internal static Sprite ShiftButton;
    internal static Sprite TricksterVentButton;
    internal static Sprite RepairButton;

    private static void LoadButtonAssets()
    {
        Stream resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("RebuildUs.Resources.buttons");
        AssetBundle ab = AssetBundle.LoadFromMemory(resource.ReadFully());

        DouseButton = ab.LoadAsset<Sprite>("DouseButton.png").Resize(115f);
        EmergencyButton = ab.LoadAsset<Sprite>("EmergencyButton.png").Resize(550f);
        VultureButton = ab.LoadAsset<Sprite>("VultureButton.png").Resize(115f);
        SidekickButton = ab.LoadAsset<Sprite>("SidekickButton.png").Resize(115f);
        IgniteButton = ab.LoadAsset<Sprite>("IgniteButton.png").Resize(115f);
        HackerButton = ab.LoadAsset<Sprite>("HackerButton.png").Resize(115f);
        LighterButton = ab.LoadAsset<Sprite>("LighterButton.png").Resize(115f);
        ShieldButton = ab.LoadAsset<Sprite>("ShieldButton.png").Resize(115f);
        MediumButton = ab.LoadAsset<Sprite>("MediumButton.png").Resize(115f);
        TimeShieldButton = ab.LoadAsset<Sprite>("TimeShieldButton.png").Resize(115f);
        TrackerButton = ab.LoadAsset<Sprite>("TrackerButton.png").Resize(115f);
        PathfindButton = ab.LoadAsset<Sprite>("PathfindButton.png").Resize(115f);
        EraserButton = ab.LoadAsset<Sprite>("EraserButton.png").Resize(115f);
        CleanButton = ab.LoadAsset<Sprite>("CleanButton.png").Resize(115f);
        PlaceJackInTheBoxButton = ab.LoadAsset<Sprite>("PlaceJackInTheBoxButton.png").Resize(115f);
        LightsOutButton = ab.LoadAsset<Sprite>("LightsOutButton.png").Resize(115f);
        PlaceCameraButton = ab.LoadAsset<Sprite>("PlaceCameraButton.png").Resize(115f);
        CloseVentButton = ab.LoadAsset<Sprite>("CloseVentButton.png").Resize(115f);
        CurseButton = ab.LoadAsset<Sprite>("CurseButton.png").Resize(115f);
        CurseKillButton = ab.LoadAsset<Sprite>("CurseKillButton.png").Resize(115f);
        MorphButton = ab.LoadAsset<Sprite>("MorphButton.png").Resize(115f);
        SampleButton = ab.LoadAsset<Sprite>("SampleButton.png").Resize(115f);
        SpellButton = ab.LoadAsset<Sprite>("SpellButton.png").Resize(115f);
        SpellButtonMeeting = ab.LoadAsset<Sprite>("SpellButtonMeeting.png").Resize(225f);
        CamouflageButton = ab.LoadAsset<Sprite>("CamouflageButton.png").Resize(115f);
        SwapperCheck = ab.LoadAsset<Sprite>("SwapperCheck.png").Resize(150f);
        GarlicButton = ab.LoadAsset<Sprite>("GarlicButton.png").Resize(115f);
        VampireButton = ab.LoadAsset<Sprite>("VampireButton.png").Resize(115f);
        ShiftButton = ab.LoadAsset<Sprite>("ShiftButton.png").Resize(115f);
        TricksterVentButton = ab.LoadAsset<Sprite>("TricksterVentButton.png").Resize(115f);
        RepairButton = ab.LoadAsset<Sprite>("RepairButton.png").Resize(115f);
    }

#endregion

#region Sprites

    internal static Sprite Arrow;
    internal static Sprite Garlic;
    internal static Sprite GarlicBackground;
    internal static Sprite Ladder;
    internal static Sprite LadderBackground;
    internal static Sprite AirshipFence;
    internal static Sprite AirshipDownloadG;
    internal static Sprite CorpseIcon;
    internal static Sprite Cross;
    internal static Sprite AdminCockpit;
    internal static Sprite AdminRecords;
    internal static Sprite Footprint;
    internal static Sprite Soul;
    internal static Sprite AnimatedVentSealed;
    internal static Sprite AnimatedVentSealedSubmerged;
    internal static Sprite StaticVentSealed;
    internal static Sprite FungleVentSealed;
    internal static Sprite CentralUpperBlocked;
    internal static Sprite CentralLowerBlocked;
    internal static Sprite TargetIcon;
    internal static Sprite White;

    private static void LoadSpriteAssets()
    {
        Stream resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("RebuildUs.Resources.sprites");
        AssetBundle ab = AssetBundle.LoadFromMemory(resource.ReadFully());

        Arrow = ab.LoadAsset<Sprite>("Arrow.png").Resize(200f);
        Garlic = ab.LoadAsset<Sprite>("Garlic.png").Resize(300f);
        GarlicBackground = ab.LoadAsset<Sprite>("GarlicBackground.png").Resize(60f);
        Ladder = ab.LoadAsset<Sprite>("Ladder.png").Resize(100f);
        LadderBackground = ab.LoadAsset<Sprite>("LadderBackground.png").Resize(100f);
        AirshipFence = ab.LoadAsset<Sprite>("AirshipFence.png").Resize(100f);
        AirshipDownloadG = ab.LoadAsset<Sprite>("AirshipDownloadG.png").Resize(100f);
        CorpseIcon = ab.LoadAsset<Sprite>("CorpseIcon.png").Resize(115f);
        Cross = ab.LoadAsset<Sprite>("Cross.png").Resize(300f);
        AdminCockpit = ab.LoadAsset<Sprite>("AdminCockpit.png").Resize(100f);
        AdminRecords = ab.LoadAsset<Sprite>("AdminRecords.png").Resize(100f);
        Footprint = ab.LoadAsset<Sprite>("Footprint.png").Resize(600f);
        Soul = ab.LoadAsset<Sprite>("Soul.png").Resize(500f);
        AnimatedVentSealed = ab.LoadAsset<Sprite>("AnimatedVentSealed.png").Resize(185f);
        AnimatedVentSealedSubmerged = ab.LoadAsset<Sprite>("AnimatedVentSealed.png").Resize(120f);
        StaticVentSealed = ab.LoadAsset<Sprite>("StaticVentSealed.png").Resize(160f);
        FungleVentSealed = ab.LoadAsset<Sprite>("FungleVentSealed.png").Resize(160f);
        CentralUpperBlocked = ab.LoadAsset<Sprite>("CentralUpperBlocked.png").Resize(145f);
        CentralLowerBlocked = ab.LoadAsset<Sprite>("CentralLowerBlocked.png").Resize(145f);
        TargetIcon = ab.LoadAsset<Sprite>("TargetIcon.png").Resize(150f);
        White = ab.LoadAsset<Sprite>("White.png").Resize(100f);
    }

#endregion

#region Locations

    internal static Sprite ArmoryButton;
    internal static Sprite CockpitButton;
    internal static Sprite CommunicationsButton;
    internal static Sprite ElectricalButton;
    internal static Sprite GapButton;
    internal static Sprite LoungeButton;
    internal static Sprite MedicalButton;
    internal static Sprite MeetingButton;
    internal static Sprite SecurityButton;
    internal static Sprite ShowersButton;
    internal static Sprite VaultButton;
    internal static Sprite ViewingButton;

    private static void LoadLocationAssets()
    {
        Stream resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("RebuildUs.Resources.locations");
        AssetBundle ab = AssetBundle.LoadFromMemory(resource.ReadFully());

        ArmoryButton = ab.LoadAsset<Sprite>("ArmoryButton.png").Resize(100f);
        CockpitButton = ab.LoadAsset<Sprite>("CockpitButton.png").Resize(100f);
        CommunicationsButton = ab.LoadAsset<Sprite>("CommunicationsButton.png").Resize(100f);
        ElectricalButton = ab.LoadAsset<Sprite>("ElectricalButton.png").Resize(100f);
        GapButton = ab.LoadAsset<Sprite>("GapButton.png").Resize(100f);
        LoungeButton = ab.LoadAsset<Sprite>("LoungeButton.png").Resize(100f);
        MedicalButton = ab.LoadAsset<Sprite>("MedicalButton.png").Resize(100f);
        MeetingButton = ab.LoadAsset<Sprite>("MeetingButton.png").Resize(100f);
        SecurityButton = ab.LoadAsset<Sprite>("SecurityButton.png").Resize(100f);
        ShowersButton = ab.LoadAsset<Sprite>("ShowersButton.png").Resize(100f);
        VaultButton = ab.LoadAsset<Sprite>("VaultButton.png").Resize(100f);
        ViewingButton = ab.LoadAsset<Sprite>("ViewingButton.png").Resize(100f);
    }

#endregion

#region KeyBinds

    private static readonly Dictionary<string, Sprite> KeyBindSprites = [];
    internal static Sprite KeyBindBackground;

    internal static Sprite GetKeyBindTexture(string address)
    {
        return KeyBindSprites.GetValueOrDefault(address);
    }

    private static void LoadKeyBindAssets()
    {
        Stream resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("RebuildUs.Resources.keybinds");
        if (resource == null) return;
        AssetBundle ab = AssetBundle.LoadFromMemory(resource.ReadFully());
        if (ab == null) return;

        KeyBindSprites["KeyBindCharacters"] = ab.LoadAsset<Sprite>("KeyBindCharacters.png").DontUnload();
        KeyBindSprites["KeyBindCharacters0"] = ab.LoadAsset<Sprite>("KeyBindCharacters0.png").DontUnload();
        KeyBindSprites["KeyBindCharacters1"] = ab.LoadAsset<Sprite>("KeyBindCharacters1.png").DontUnload();
        KeyBindSprites["KeyBindCharacters2"] = ab.LoadAsset<Sprite>("KeyBindCharacters2.png").DontUnload();
        KeyBindSprites["KeyBindCharacters3"] = ab.LoadAsset<Sprite>("KeyBindCharacters3.png").DontUnload();
        KeyBindSprites["KeyBindCharacters4"] = ab.LoadAsset<Sprite>("KeyBindCharacters4.png").DontUnload();
        KeyBindSprites["KeyBindCharacters5"] = ab.LoadAsset<Sprite>("KeyBindCharacters5.png").DontUnload();
        KeyBindBackground = ab.LoadAsset<Sprite>("KeyBindBackground.png").DontUnload();
    }

#endregion
}