namespace RebuildUs.Modules;

internal static class ShowHost
{
    private static TextMeshPro _text;
    private static readonly StringBuilder Sb = new();

    internal static void Setup(MeetingHud __instance)
    {
        if (AmongUsClient.Instance.NetworkMode != NetworkModes.OnlineGame)
        {
            return;
        }

        __instance.ProceedButton.gameObject.transform.localPosition = new(-2.5f, 2.2f, 0);
        __instance.ProceedButton.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        __instance.ProceedButton.GetComponent<PassiveButton>().enabled = false;
        __instance.HostIcon.gameObject.SetActive(true);
        __instance.ProceedButton.gameObject.SetActive(true);
        _text = __instance.ProceedButton.gameObject.GetComponentInChildren<TextMeshPro>();
    }

    internal static void Update(MeetingHud __instance)
    {
        var host = GameData.Instance.GetHost();

        if (host == null || _text == null)
        {
            return;
        }
        PlayerMaterial.SetColors(host.DefaultOutfit.ColorId, __instance.HostIcon);

        Sb.Clear();
        Sb.Append(Tr.Get(TrKey.Host));
        Sb.Append(": ");
        Sb.Append(host.PlayerName);

        var newText = Sb.ToString();
        if (_text.text != newText)
        {
            _text.text = newText;
        }
    }
}