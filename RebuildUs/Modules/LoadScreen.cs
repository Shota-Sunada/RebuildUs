using Il2CppInterop.Runtime.Attributes;
using Object = UnityEngine.Object;

namespace RebuildUs.Modules;

public static class LoadScreen
{
    private static GameObject _uiContainer;
    private static TextMeshPro _titleTMPro;
    private static TextMeshPro _statusTMPro;

    private static Sprite _whiteSprite;

    public static string StatusText { get; set; } = "";
    public static float Progress { get; set; }
    public static string ProgressDetailText { get; set; } = "";

    [HideFromIl2Cpp]
    public static void Create()
    {
        if (_uiContainer != null) return;

        if (_whiteSprite == null)
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            _whiteSprite = Sprite.Create(tex, new(0, 0, 1, 1), new(0.5f, 0.5f));
        }

        _uiContainer = new("RebuildUsLoadScreen");
        Object.DontDestroyOnLoad(_uiContainer);

        // Background
        var bg = new GameObject("Background");
        bg.transform.SetParent(_uiContainer.transform);
        var bgRenderer = bg.AddComponent<SpriteRenderer>();
        bgRenderer.sprite = _whiteSprite;
        bgRenderer.color = new(0.05f, 0.05f, 0.1f, 1f); // Dark Blue-ish Black
        bg.transform.localScale = new(100f, 100f, 1f);
        bg.transform.localPosition = new(0, 0, 20f);

        // Title
        var titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(_uiContainer.transform);
        _titleTMPro = titleObj.AddComponent<TextMeshPro>();
        _titleTMPro.alignment = TextAlignmentOptions.Center;
        _titleTMPro.fontSize = 5f;
        _titleTMPro.text = "<size=70%>LOADING</size>\n<color=#1684B0>REBUILD US</color>";
        _titleTMPro.fontStyle = FontStyles.Bold | FontStyles.Italic;
        titleObj.transform.localPosition = new(0, 1.5f, 5f);

        // Status
        var statusObj = new GameObject("StatusText");
        statusObj.transform.SetParent(_uiContainer.transform);
        _statusTMPro = statusObj.AddComponent<TextMeshPro>();
        _statusTMPro.alignment = TextAlignmentOptions.Center;
        _statusTMPro.fontSize = 2f;
        _statusTMPro.text = "Initializing...";
        statusObj.transform.localPosition = new(0, 0.2f, 5f);

        _uiContainer.transform.position = new(0, 0, -50f);
    }

    [HideFromIl2Cpp]
    public static void Update()
    {
        if (_statusTMPro != null && _statusTMPro.text != StatusText) _statusTMPro.text = StatusText;
    }

    [HideFromIl2Cpp]
    public static void Destroy()
    {
        if (_uiContainer != null)
        {
            Object.Destroy(_uiContainer);
            _uiContainer = null;
        }
    }
}
