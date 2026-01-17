namespace RebuildUs.Modules;

public static class ShowHost
{
    private static TextMeshPro Text = null;

    public static void Setup(MeetingHud __instance)
    {
        if (AmongUsClient.Instance.NetworkMode != NetworkModes.OnlineGame) return;

        __instance.ProceedButton.gameObject.transform.localPosition = new(-2.5f, 2.2f, 0);
        __instance.ProceedButton.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        __instance.ProceedButton.GetComponent<PassiveButton>().enabled = false;
        __instance.HostIcon.gameObject.SetActive(true);
        __instance.ProceedButton.gameObject.SetActive(true);
    }

    public static void Update(MeetingHud __instance)
    {
        var host = GameData.Instance.GetHost();

        if (host != null)
        {
            PlayerMaterial.SetColors(host.DefaultOutfit.ColorId, __instance.HostIcon);
            if (Text == null) Text = __instance.ProceedButton.gameObject.GetComponentInChildren<TextMeshPro>();
            Text.text = $"host: {host.PlayerName}";
        }
    }
}