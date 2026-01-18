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

        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0001.png").DontUnload());
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0002.png").DontUnload());
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0003.png").DontUnload());
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0004.png").DontUnload());
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0005.png").DontUnload());
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0006.png").DontUnload());
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0007.png").DontUnload());
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0008.png").DontUnload());
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0009.png").DontUnload());
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0010.png").DontUnload());
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0011.png").DontUnload());
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0012.png").DontUnload());
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0013.png").DontUnload());
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0014.png").DontUnload());
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0015.png").DontUnload());
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0016.png").DontUnload());
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0017.png").DontUnload());
        TricksterAnimations.Add(ab.LoadAsset<Sprite>("trickster_box_0018.png").DontUnload());
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

    private static void LoadButtonAssets()
    {
        var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("RebuildUs.Resources.buttons");
        var ab = AssetBundle.LoadFromMemory(resource.ReadFully());

        DouseButton = ab.LoadAsset<Sprite>("DouseButton.png").DontUnload();
        EmergencyButton = ab.LoadAsset<Sprite>("EmergencyButton.png").DontUnload();
        VultureButton = ab.LoadAsset<Sprite>("VultureButton.png").DontUnload();
        SidekickButton = ab.LoadAsset<Sprite>("SidekickButton.png").DontUnload();
        IgniteButton = ab.LoadAsset<Sprite>("IgniteButton.png").DontUnload();
        HackerButton = ab.LoadAsset<Sprite>("HackerButton.png").DontUnload();
        LighterButton = ab.LoadAsset<Sprite>("LighterButton.png").DontUnload();
        ShieldButton = ab.LoadAsset<Sprite>("ShieldButton.png").DontUnload();
        MediumButton = ab.LoadAsset<Sprite>("MediumButton.png").DontUnload();
        TimeShieldButton = ab.LoadAsset<Sprite>("MediumButton.png").DontUnload();
        TrackerButton = ab.LoadAsset<Sprite>("TrackerButton.png").DontUnload();
        PathfindButton = ab.LoadAsset<Sprite>("PathfindButton.png").DontUnload();
        EraserButton = ab.LoadAsset<Sprite>("EraserButton.png").DontUnload();
        CleanButton = ab.LoadAsset<Sprite>("CleanButton.png").DontUnload();
        PlaceJackInTheBoxButton = ab.LoadAsset<Sprite>("PlaceJackInTheBoxButton.png").DontUnload();
        LightsOutButton = ab.LoadAsset<Sprite>("LightsOutButton.png").DontUnload();
        PlaceCameraButton = ab.LoadAsset<Sprite>("PlaceCameraButton.png").DontUnload();
        CloseVentButton = ab.LoadAsset<Sprite>("CloseVentButton.png").DontUnload();
        CurseButton = ab.LoadAsset<Sprite>("CurseButton.png").DontUnload();
        CurseKillButton = ab.LoadAsset<Sprite>("CurseKillButton.png").DontUnload();
        MorphButton = ab.LoadAsset<Sprite>("MorphButton.png").DontUnload();
        SampleButton = ab.LoadAsset<Sprite>("SampleButton.png").DontUnload();
        SpellButton = ab.LoadAsset<Sprite>("SpellButton.png").DontUnload();
        SpellButtonMeeting = ab.LoadAsset<Sprite>("SpellButtonMeeting.png").DontUnload();
        CamouflageButton = ab.LoadAsset<Sprite>("CamouflageButton.png").DontUnload();
        SwapperCheck = ab.LoadAsset<Sprite>("SwapperCheck.png").DontUnload();
        GarlicButton = ab.LoadAsset<Sprite>("GarlicButton.png").DontUnload();
        VampireButton = ab.LoadAsset<Sprite>("VampireButton.png").DontUnload();
        ShiftButton = ab.LoadAsset<Sprite>("ShiftButton.png").DontUnload();
        TricksterVentButton = ab.LoadAsset<Sprite>("TricksterVentButton.png").DontUnload();
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

        Arrow = ab.LoadAsset<Sprite>("Arrow.png").DontUnload();
        Garlic = ab.LoadAsset<Sprite>("Garlic.png").DontUnload();
        GarlicBackground = ab.LoadAsset<Sprite>("GarlicBackground.png").DontUnload();
        Ladder = ab.LoadAsset<Sprite>("Ladder.png").DontUnload();
        LadderBackground = ab.LoadAsset<Sprite>("LadderBackground.png").DontUnload();
        AirshipFence = ab.LoadAsset<Sprite>("AirshipFence.png").DontUnload();
        AirshipDownloadG = ab.LoadAsset<Sprite>("AirshipDownloadG.png").DontUnload();
        CorpseIcon = ab.LoadAsset<Sprite>("CorpseIcon.png").DontUnload();
        Cross = ab.LoadAsset<Sprite>("Cross.png").DontUnload();
        AdminCockpit = ab.LoadAsset<Sprite>("AdminCockpit.png").DontUnload();
        AdminRecords = ab.LoadAsset<Sprite>("AdminRecords.png").DontUnload();
        Footprint = ab.LoadAsset<Sprite>("Footprint.png").DontUnload();
        Soul = ab.LoadAsset<Sprite>("Soul.png").DontUnload();
        AnimatedVentSealed = ab.LoadAsset<Sprite>("AnimatedVentSealed.png").DontUnload();
        StaticVentSealed = ab.LoadAsset<Sprite>("StaticVentSealed.png").DontUnload();
        FungleVentSealed = ab.LoadAsset<Sprite>("FungleVentSealed.png").DontUnload();
        CentralUpperBlocked = ab.LoadAsset<Sprite>("CentralUpperBlocked.png").DontUnload();
        CentralLowerBlocked = ab.LoadAsset<Sprite>("CentralLowerBlocked.png").DontUnload();
        TargetIcon = ab.LoadAsset<Sprite>("TargetIcon.png").DontUnload();
        White = ab.LoadAsset<Sprite>("White.png").DontUnload();
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

        ArmoryButton = ab.LoadAsset<Sprite>("ArmoryButton.png").DontUnload();
        CockpitButton = ab.LoadAsset<Sprite>("CockpitButton.png").DontUnload();
        CommunicationsButton = ab.LoadAsset<Sprite>("CommunicationsButton.png").DontUnload();
        ElectricalButton = ab.LoadAsset<Sprite>("ElectricalButton.png").DontUnload();
        GapButton = ab.LoadAsset<Sprite>("GapButton.png").DontUnload();
        LoungeButton = ab.LoadAsset<Sprite>("LoungeButton.png").DontUnload();
        MedicalButton = ab.LoadAsset<Sprite>("MedicalButton.png").DontUnload();
        MeetingButton = ab.LoadAsset<Sprite>("MeetingButton.png").DontUnload();
        SecurityButton = ab.LoadAsset<Sprite>("SecurityButton.png").DontUnload();
        ShowersButton = ab.LoadAsset<Sprite>("ShowersButton.png").DontUnload();
        VaultButton = ab.LoadAsset<Sprite>("VaultButton.png").DontUnload();
        ViewingButton = ab.LoadAsset<Sprite>("ViewingButton.png").DontUnload();
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

    private static T DontUnload<T>(this T obj) where T : UnityEngine.Object
    {
        obj.hideFlags |= HideFlags.DontUnloadUnusedAsset;

        return obj;
    }
}