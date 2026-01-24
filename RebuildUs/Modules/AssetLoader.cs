using System.Reflection;

namespace RebuildUs.Modules;

public static class AssetLoader
{
    private static bool IsLoaded = false;
    public static void LoadAssets()
    {
        if (IsLoaded) return;
        IsLoaded = true;

        LoadButtonAssets();
        LoadAnimationAssets();
        LoadSpriteAssets();
        LoadLocationAssets();
    }

    #region Animations
    public static List<Sprite> TricksterAnimations = [];

    private static void LoadAnimationAssets()
    {
        var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("RebuildUs.Resources.tricksteranimation");
        var ab = AssetBundle.LoadFromMemory(resource.ReadFully());

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
    public static Sprite DouseButton;
    public static Sprite EmergencyButton;
    public static Sprite VultureButton;
    public static Sprite SidekickButton;
    public static Sprite IgniteButton;
    public static Sprite HackerButton;
    public static Sprite LighterButton;
    public static Sprite ShieldButton;
    public static Sprite MediumButton;
    public static Sprite TimeShieldButton;
    public static Sprite TrackerButton;
    public static Sprite PathfindButton;
    public static Sprite EraserButton;
    public static Sprite CleanButton;
    public static Sprite PlaceJackInTheBoxButton;
    public static Sprite LightsOutButton;
    public static Sprite PlaceCameraButton;
    public static Sprite CloseVentButton;
    public static Sprite CurseButton;
    public static Sprite CurseKillButton;
    public static Sprite MorphButton;
    public static Sprite SampleButton;
    public static Sprite SpellButton;
    public static Sprite SpellButtonMeeting;
    public static Sprite CamouflageButton;
    public static Sprite SwapperCheck;
    public static Sprite GarlicButton;
    public static Sprite VampireButton;
    public static Sprite ShiftButton;
    public static Sprite TricksterVentButton;
    public static Sprite RepairButton;

    private static void LoadButtonAssets()
    {
        var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("RebuildUs.Resources.buttons");
        var ab = AssetBundle.LoadFromMemory(resource.ReadFully());

        DouseButton = ab.LoadAsset<Sprite>("DouseButton.png").Resize(115f);
        EmergencyButton = ab.LoadAsset<Sprite>("EmergencyButton.png").Resize(550f);
        VultureButton = ab.LoadAsset<Sprite>("VultureButton.png").Resize(115f);
        SidekickButton = ab.LoadAsset<Sprite>("SidekickButton.png").Resize(115f);
        IgniteButton = ab.LoadAsset<Sprite>("IgniteButton.png").Resize(115f);
        HackerButton = ab.LoadAsset<Sprite>("HackerButton.png").Resize(115f);
        LighterButton = ab.LoadAsset<Sprite>("LighterButton.png").Resize(115f);
        ShieldButton = ab.LoadAsset<Sprite>("ShieldButton.png").Resize(115f);
        MediumButton = ab.LoadAsset<Sprite>("MediumButton.png").Resize(115f);
        TimeShieldButton = ab.LoadAsset<Sprite>("MediumButton.png").Resize(115f);
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
    public static Sprite Arrow;
    public static Sprite Garlic;
    public static Sprite GarlicBackground;
    public static Sprite Ladder;
    public static Sprite LadderBackground;
    public static Sprite AirshipFence;
    public static Sprite AirshipDownloadG;
    public static Sprite CorpseIcon;
    public static Sprite Cross;
    public static Sprite AdminCockpit;
    public static Sprite AdminRecords;
    public static Sprite Footprint;
    public static Sprite Soul;
    public static Sprite AnimatedVentSealed;
    public static Sprite AnimatedVentSealedSubmerged;
    public static Sprite StaticVentSealed;
    public static Sprite FungleVentSealed;
    public static Sprite CentralUpperBlocked;
    public static Sprite CentralLowerBlocked;
    public static Sprite TargetIcon;
    public static Sprite White;

    private static void LoadSpriteAssets()
    {
        var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("RebuildUs.Resources.sprites");
        var ab = AssetBundle.LoadFromMemory(resource.ReadFully());

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
    public static Sprite ArmoryButton;
    public static Sprite CockpitButton;
    public static Sprite CommunicationsButton;
    public static Sprite ElectricalButton;
    public static Sprite GapButton;
    public static Sprite LoungeButton;
    public static Sprite MedicalButton;
    public static Sprite MeetingButton;
    public static Sprite SecurityButton;
    public static Sprite ShowersButton;
    public static Sprite VaultButton;
    public static Sprite ViewingButton;

    private static void LoadLocationAssets()
    {
        var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("RebuildUs.Resources.locations");
        var ab = AssetBundle.LoadFromMemory(resource.ReadFully());

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

    public static byte[] ReadFully(this Stream input)
    {
        using var ms = new MemoryStream();
        input.CopyTo(ms);
        return ms.ToArray();
    }

#nullable enable
    private static T? LoadAsset<T>(this AssetBundle assetBundle, string name) where T : UnityEngine.Object
    {
        return assetBundle.LoadAsset(name, Il2CppType.Of<T>())?.Cast<T>() ?? throw new Exception($"The asset was not found: {name}");
    }
#nullable disable

    private static Sprite Resize(this Sprite sprite, float pixelsPerUnit)
    {
        if (pixelsPerUnit == 100f) return sprite.DontUnload();

        return sprite == null
            ? null
            : Sprite.Create(
            sprite.texture,
            sprite.rect,
            new Vector2(sprite.pivot.x / sprite.rect.width, sprite.pivot.y / sprite.rect.height),
            pixelsPerUnit,
            0,
            SpriteMeshType.FullRect,
            sprite.border
        ).DontUnload();
    }

    private static T DontUnload<T>(this T obj) where T : UnityEngine.Object
    {
        obj.hideFlags |= HideFlags.DontUnloadUnusedAsset;

        return obj;
    }
}