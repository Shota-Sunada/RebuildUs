namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class MainMenuManagerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    internal static void StartPostfix(MainMenuManager __instance)
    {
        ModManager.Instance.ShowModStamp();

        ClientOptions.Start(__instance);

        {
            // Edit Main Menu
            GameObject ruLogo = new("RULogo");
            ruLogo.transform.SetParent(GameObject.Find("RightPanel").transform, false);
            ruLogo.transform.localPosition = new(-0.4f, 1f, 5f);

            GameObject credits = new("RUModCredits");
            TextMeshPro text = credits.AddComponent<TextMeshPro>();
            text.SetText($"<color=#1684B0>{RebuildUs.MOD_NAME}</color> v{RebuildUs.MOD_VERSION}\n<size=70%>By {RebuildUs.MOD_DEVELOPER}</size>");
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize *= 0.07f;

            text.transform.SetParent(ruLogo.transform);
            text.transform.localPosition = Vector3.down * 1.25f;

            PassiveButton howToPlayButton = __instance.howToPlayButton;
            PassiveButton freePlayButton = __instance.freePlayButton;
            howToPlayButton.gameObject.SetActive(false);
            freePlayButton.gameObject.SetActive(false);

            PassiveButton createGameButton = __instance.createGameButton;
            // var enterCodeButtons = __instance.enterCodeButtons;
            Transform enterCodeButtons = createGameButton.transform.parent.Find("Enter Code Button");
            FindGameButton findGameButton = __instance.findGameButton;

            // remove line
            findGameButton.gameObject.transform.parent.Find("Line")?.gameObject.SetActive(false);
            findGameButton.gameObject.SetActive(false);

            createGameButton.transform.SetLocalX(0);
            enterCodeButtons?.transform.SetLocalX(0);
        }
    }
}