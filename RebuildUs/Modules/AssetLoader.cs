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
    }

    // Animations
    public static List<Sprite> TricksterAnimations = [];

    // Buttons
    public static Sprite DouseButton;
    public static Sprite EmergencyButton;
    public static Sprite VultureButton;
    public static Sprite SidekickButton;

    public static Sprite IgniteButton;
    public static Sprite Minus_Button;
    public static Sprite Minus_ButtonActive;
    public static Sprite Plus_Button;
    public static Sprite Plus_ButtonActive;
    public static Sprite Settings_Button;
    public static Sprite Settings_ButtonActive;

    // Sprites
    public static Sprite Arrow;
    public static Sprite Endscreen;
    public static Sprite EndscreenActive;
    public static Sprite Garlic;
    public static Sprite GarlicBackground;

    private static void LoadButtonAssets()
    {
        var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("RebuildUs.Resources.buttons");
        var ab = AssetBundle.LoadFromMemory(resource.ReadFully());

        DouseButton = ab.LoadAsset<Sprite>("DouseButton.png").DontUnload();
        EmergencyButton = ab.LoadAsset<Sprite>("EmergencyButton.png").DontUnload();
        VultureButton = ab.LoadAsset<Sprite>("VultureButton.png").DontUnload();
        SidekickButton = ab.LoadAsset<Sprite>("SidekickButton.png").DontUnload();

        IgniteButton = ab.LoadAsset<Sprite>("IgniteButton.png").DontUnload();
        Minus_Button = ab.LoadAsset<Sprite>("Minus_Button.png").DontUnload();
        Minus_ButtonActive = ab.LoadAsset<Sprite>("Minus_ButtonActive.png").DontUnload();
        Plus_Button = ab.LoadAsset<Sprite>("Plus_Button.png").DontUnload();
        Plus_ButtonActive = ab.LoadAsset<Sprite>("Plus_ButtonActive.png").DontUnload();
        Settings_Button = ab.LoadAsset<Sprite>("Settings_Button.png").DontUnload();
        Settings_ButtonActive = ab.LoadAsset<Sprite>("Settings_ButtonActive.png").DontUnload();
    }

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

    private static void LoadSpriteAssets()
    {
        var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("RebuildUs.Resources.sprites");
        var ab = AssetBundle.LoadFromMemory(resource.ReadFully());

        Arrow = ab.LoadAsset<Sprite>("Arrow.png").DontUnload();
        Endscreen = ab.LoadAsset<Sprite>("Endscreen.png").DontUnload();
        EndscreenActive = ab.LoadAsset<Sprite>("EndscreenActive.png").DontUnload();
        Garlic = ab.LoadAsset<Sprite>("Garlic.png").DontUnload();
        GarlicBackground = ab.LoadAsset<Sprite>("GarlicBackground.png").DontUnload();
    }

    public static byte[] ReadFully(this Stream input)
    {
        using var ms = new MemoryStream();
        input.CopyTo(ms);
        return ms.ToArray();
    }

#nullable enable
    private static T? LoadAsset<T>(this AssetBundle assetBundle, string name) where T : UnityEngine.Object
    {
        return assetBundle.LoadAsset(name, Il2CppType.Of<T>())?.Cast<T>();
    }
#nullable disable

    private static T DontUnload<T>(this T obj) where T : UnityEngine.Object
    {
        obj.hideFlags |= HideFlags.DontUnloadUnusedAsset;

        return obj;
    }
}