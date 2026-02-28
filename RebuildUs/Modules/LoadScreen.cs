using Il2CppInterop.Runtime.Attributes;

namespace RebuildUs.Modules;

internal static class LoadScreen
{
    private static GameObject _uiContainer;
    private static TextMeshPro _titleTMPro;
    private static TextMeshPro _statusTMPro;

    private static Sprite _whiteSprite;

    internal static string StatusText { get; set; } = "";
    internal static float Progress { get; set; }
    internal static string ProgressDetailText { get; set; } = "";

    [HideFromIl2Cpp]
    internal static void Create()
    {
        if (_uiContainer != null)
        {
            return;
        }

        if (_whiteSprite == null)
        {
            Texture2D tex = new(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            _whiteSprite = Sprite.Create(tex, new(0, 0, 1, 1), new(0.5f, 0.5f));
        }

        _uiContainer = new("RebuildUsLoadScreen");
        UnityObject.DontDestroyOnLoad(_uiContainer);

        // Background
        GameObject bg = new("Background");
        bg.transform.SetParent(_uiContainer.transform);
        var bgRenderer = bg.AddComponent<SpriteRenderer>();
        bgRenderer.sprite = _whiteSprite;
        bgRenderer.color = new(0.05f, 0.05f, 0.1f, 1f); // Dark Blue-ish Black
        bg.transform.localScale = new(100f, 100f, 1f);
        bg.transform.localPosition = new(0, 0, 20f);

        // Title
        GameObject titleObj = new("TitleText");
        titleObj.transform.SetParent(_uiContainer.transform);
        _titleTMPro = titleObj.AddComponent<TextMeshPro>();
        _titleTMPro.alignment = TextAlignmentOptions.Center;
        _titleTMPro.fontSize = 5f;
        _titleTMPro.text = "<size=70%>LOADING</size>\n<color=#1684B0>REBUILD US</color>";
        _titleTMPro.fontStyle = FontStyles.Bold | FontStyles.Italic;
        titleObj.transform.localPosition = new(0, 1.5f, 5f);

        // Status
        GameObject statusObj = new("StatusText");
        statusObj.transform.SetParent(_uiContainer.transform);
        _statusTMPro = statusObj.AddComponent<TextMeshPro>();
        _statusTMPro.alignment = TextAlignmentOptions.Center;
        _statusTMPro.fontSize = 2f;
        _statusTMPro.text = "Initializing...";
        statusObj.transform.localPosition = new(0, 0.2f, 5f);

        _uiContainer.transform.position = new(0, 0, -50f);
    }

    [HideFromIl2Cpp]
    internal static void Update()
    {
        if (_statusTMPro != null && _statusTMPro.text != StatusText)
        {
            _statusTMPro.text = StatusText;
        }
    }

    [HideFromIl2Cpp]
    internal static void Destroy()
    {
        if (_uiContainer == null)
        {
            return;
        }
        UnityObject.Destroy(_uiContainer);
        _uiContainer = null;
    }
}