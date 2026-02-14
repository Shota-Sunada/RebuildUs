using Il2CppInterop.Runtime.Attributes;

namespace RebuildUs.Modules;

public static class LoadScreen
{
    private static GameObject UIContainer;
    private static TextMeshPro TitleTMPro;
    private static TextMeshPro StatusTMPro;

    public static string StatusText { get; set; } = "";
    public static float Progress { get; set; }
    public static string ProgressDetailText { get; set; } = "";

    private static Sprite WhiteSprite;

    [HideFromIl2Cpp]
    public static void Create()
    {
        if (UIContainer != null) return;

        if (WhiteSprite == null)
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            WhiteSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
        }

        UIContainer = new GameObject("RebuildUsLoadScreen");
        UnityEngine.Object.DontDestroyOnLoad(UIContainer);

        // Background
        var bg = new GameObject("Background");
        bg.transform.SetParent(UIContainer.transform);
        var bgRenderer = bg.AddComponent<SpriteRenderer>();
        bgRenderer.sprite = WhiteSprite;
        bgRenderer.color = new Color(0.05f, 0.05f, 0.1f, 1f); // Dark Blue-ish Black
        bg.transform.localScale = new Vector3(100f, 100f, 1f);
        bg.transform.localPosition = new Vector3(0, 0, 20f);

        // Title
        var titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(UIContainer.transform);
        TitleTMPro = titleObj.AddComponent<TextMeshPro>();
        TitleTMPro.alignment = TextAlignmentOptions.Center;
        TitleTMPro.fontSize = 5f;
        TitleTMPro.text = "<size=70%>LOADING</size>\n<color=#1684B0>REBUILD US</color>";
        TitleTMPro.fontStyle = FontStyles.Bold | FontStyles.Italic;
        titleObj.transform.localPosition = new Vector3(0, 1.5f, 5f);

        // Status
        var statusObj = new GameObject("StatusText");
        statusObj.transform.SetParent(UIContainer.transform);
        StatusTMPro = statusObj.AddComponent<TextMeshPro>();
        StatusTMPro.alignment = TextAlignmentOptions.Center;
        StatusTMPro.fontSize = 2f;
        StatusTMPro.text = "Initializing...";
        statusObj.transform.localPosition = new Vector3(0, 0.2f, 5f);

        UIContainer.transform.position = new Vector3(0, 0, -50f);
    }

    [HideFromIl2Cpp]
    public static void Update()
    {
        if (StatusTMPro != null && StatusTMPro.text != StatusText)
        {
            StatusTMPro.text = StatusText;
        }
    }

    [HideFromIl2Cpp]
    public static void Destroy()
    {
        if (UIContainer != null)
        {
            UnityEngine.Object.Destroy(UIContainer);
            UIContainer = null;
        }
    }
}