namespace RebuildUs.Modules;

public static class ShowHost
{
    private static TextMeshPro _text = null;
    private static readonly StringBuilder _sb = new();

    public static void Setup(MeetingHud __instance)
    {
        if (AmongUsClient.Instance.NetworkMode != NetworkModes.OnlineGame) return;

        __instance.ProceedButton.gameObject.transform.localPosition = new(-2.5f, 2.2f, 0);
        __instance.ProceedButton.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        __instance.ProceedButton.GetComponent<PassiveButton>().enabled = false;
        __instance.HostIcon.gameObject.SetActive(true);
        __instance.ProceedButton.gameObject.SetActive(true);
        _text = __instance.ProceedButton.gameObject.GetComponentInChildren<TextMeshPro>();
    }

    public static void Update(MeetingHud __instance)
    {
        var host = GameData.Instance.GetHost();

        if (host != null && _text != null)
        {
            PlayerMaterial.SetColors(host.DefaultOutfit.ColorId, __instance.HostIcon);

            _sb.Clear();
            _sb.Append(Tr.Get(TranslateKey.Host));
            _sb.Append(": ");
            _sb.Append(host.PlayerName);

            string newText = _sb.ToString();
            if (_text.text != newText)
            {
                _text.text = newText;
            }
        }
    }
}

