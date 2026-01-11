using RebuildUs.Extensions;
using RebuildUs.Options;
using AmongUs.GameOptions;

namespace RebuildUs.Modules.CustomOptions;

public partial class CustomOption
{
    public class HudSettingsManager
    {
        private static readonly TextMeshPro GameSettings;

        public static float
            MinX,/*-5.3F*/
            OriginalY = 2.9F,
            MinY = 2.9F;

        public static Scroller Scroller;
        private static Vector3 LastPosition;
        private static float LastAspect;
        private static bool SetLastPosition = false;

        public static void UpdateScrollerPosition(HudManager __instance)
        {
            if (GameSettings?.transform == null) return;

            // Sets the MinX position to the left edge of the screen + 0.1 units
            Rect safeArea = Screen.safeArea;
            float aspect = Mathf.Min(Camera.main.aspect, safeArea.width / safeArea.height);
            float safeOrthographicSize = CameraSafeArea.GetSafeOrthographicSize(Camera.main);
            MinX = 0.1f - safeOrthographicSize * aspect;

            if (!SetLastPosition || aspect != LastAspect)
            {
                LastPosition = new Vector3(MinX, MinY);
                LastAspect = aspect;
                SetLastPosition = true;
                Scroller?.ContentXBounds = new FloatRange(MinX, MinX);
            }

            CreateScroller(__instance);

            Scroller.gameObject.SetActive(GameSettings.gameObject.activeSelf);

            if (!Scroller.gameObject.active) return;

            var rows = GameSettings.text.Count(c => c == '\n');
            float lobbyTextRowHeight = 0.06F;
            var maxY = Mathf.Max(MinY, rows * lobbyTextRowHeight + (rows - 38) * lobbyTextRowHeight);

            Scroller.ContentYBounds = new FloatRange(MinY, maxY);

            // Prevent scrolling when the player is interacting with a menu
            if (PlayerControl.LocalPlayer.CanMove != true)
            {
                GameSettings.transform.localPosition = LastPosition;

                return;
            }

            if (GameSettings.transform.localPosition.x != MinX ||
                GameSettings.transform.localPosition.y < MinY)
            {
                return;
            }

            LastPosition = GameSettings.transform.localPosition;
        }

        private static void CreateScroller(HudManager __instance)
        {
            if (Scroller != null) return;

            Transform target = GameSettings.transform;

            Scroller = new GameObject("SettingsScroller").AddComponent<Scroller>();
            Scroller.transform.SetParent(GameSettings.transform.parent);
            Scroller.gameObject.layer = 5;

            Scroller.transform.localScale = Vector3.one;
            Scroller.allowX = false;
            Scroller.allowY = true;
            Scroller.active = true;
            Scroller.velocity = new Vector2(0, 0);
            Scroller.ScrollbarYBounds = new FloatRange(0, 0);
            Scroller.ContentXBounds = new FloatRange(MinX, MinX);
            Scroller.enabled = true;

            Scroller.Inner = target;
            target.SetParent(Scroller.transform);
        }

        public static void UpdateHudSettings(HudManager __instance)
        {
            if (!SettingsTMPs[0]) return;
            foreach (var tmp in SettingsTMPs) tmp.text = "";
            var settingsString = BuildAllOptions(hideExtras: true);
            var blocks = settingsString.Split("\n\n", StringSplitOptions.RemoveEmptyEntries); ;
            string curString = "";
            string curBlock;
            int j = 0;
            for (int i = 0; i < blocks.Length; i++)
            {
                curBlock = blocks[i];
                if (Helpers.LineCount(curBlock) + Helpers.LineCount(curString) < 43)
                {
                    curString += curBlock + "\n\n";
                }
                else
                {
                    SettingsTMPs[j].text = curString;
                    j++;

                    curString = "\n" + curBlock + "\n\n";
                    if (curString.Substring(0, 2) != "\n\n") curString = "\n" + curString;
                }
            }
            if (j < SettingsTMPs.Length) SettingsTMPs[j].text = curString;
            int blockCount = 0;
            foreach (var tmp in SettingsTMPs)
            {
                if (tmp.text != "")
                    blockCount++;
            }
            for (int i = 0; i < blockCount; i++)
            {
                SettingsTMPs[i].transform.localPosition = new Vector3(-blockCount * 1.2f + 2.7f * i, 2.2f, -500f);
            }
        }

        private static readonly TMPro.TextMeshPro[] SettingsTMPs = new TMPro.TextMeshPro[4];
        private static GameObject SettingsBackground;
        public static void OpenSettings(HudManager __instance)
        {
            if (__instance.FullScreen == null || MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) return;
            if (SummaryTMP)
            {
                CloseSummary();
            }
            SettingsBackground = UnityEngine.Object.Instantiate(__instance.FullScreen.gameObject, __instance.transform);
            SettingsBackground.SetActive(true);
            var renderer = SettingsBackground.GetComponent<SpriteRenderer>();
            renderer.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
            renderer.enabled = true;

            for (int i = 0; i < SettingsTMPs.Length; i++)
            {
                SettingsTMPs[i] = UnityEngine.Object.Instantiate(__instance.KillButton.cooldownTimerText, __instance.transform);
                SettingsTMPs[i].alignment = TextAlignmentOptions.TopLeft;
                SettingsTMPs[i].enableWordWrapping = false;
                SettingsTMPs[i].transform.localScale = Vector3.one * 0.25f;
                SettingsTMPs[i].gameObject.SetActive(true);
            }
        }

        public static void CloseSettings()
        {
            foreach (var tmp in SettingsTMPs)
                if (tmp) tmp.gameObject.Destroy();

            if (SettingsBackground) SettingsBackground.Destroy();
        }

        public static void ToggleSettings(HudManager __instance)
        {
            if (SettingsTMPs[0]) CloseSettings();
            else OpenSettings(__instance);
        }

        public static void UpdateEndGameSummary(HudManager __instance)
        {
            if (!SummaryTMP) return;
            SummaryTMP.text = Helpers.PreviousEndGameSummary;
            SummaryTMP.transform.localPosition = new Vector3(-3 * 1.2f, 2.2f, -500f);
        }

        private static TextMeshPro SummaryTMP = null;
        private static GameObject SummaryBackground;
        public static void OpenSummary(HudManager __instance)
        {
            if (__instance.FullScreen == null || MapBehaviour.Instance && MapBehaviour.Instance.IsOpen || Helpers.PreviousEndGameSummary.IsNullOrWhiteSpace()) return;
            if (SettingsTMPs[0])
            {
                CloseSettings();
            }
            SummaryBackground = UnityEngine.Object.Instantiate(__instance.FullScreen.gameObject, __instance.transform);
            SummaryBackground.SetActive(true);
            var renderer = SummaryBackground.GetComponent<SpriteRenderer>();
            renderer.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
            renderer.enabled = true;

            SummaryTMP = UnityEngine.Object.Instantiate(__instance.KillButton.cooldownTimerText, __instance.transform);
            SummaryTMP.alignment = TextAlignmentOptions.TopLeft;
            SummaryTMP.enableWordWrapping = false;
            SummaryTMP.transform.localScale = Vector3.one * 0.3f;
            SummaryTMP.gameObject.SetActive(true);
        }

        public static void CloseSummary()
        {
            SummaryTMP?.gameObject.Destroy();
            SummaryTMP = null;
            if (SummaryBackground) SummaryBackground.Destroy();
        }

        public static void ToggleSummary(HudManager __instance)
        {
            if (SummaryTMP) CloseSummary();
            else OpenSummary(__instance);
        }

        static PassiveButton ToggleSettingsButton;
        static GameObject ToggleSettingsButtonObject;

        static PassiveButton ToggleSummaryButton;
        static GameObject ToggleSummaryButtonObject;

        static GameObject ToggleZoomButtonObject;
        static PassiveButton ToggleZoomButton;

        public static void UpdateHudButtons(HudManager __instance)
        {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
            if (!ToggleSettingsButton || !ToggleSettingsButtonObject)
            {
                // add a special button for settings viewing:
                ToggleSettingsButtonObject = UnityEngine.Object.Instantiate(__instance.MapButton.gameObject, __instance.MapButton.transform.parent);
                ToggleSettingsButtonObject.transform.localPosition = __instance.MapButton.transform.localPosition + new Vector3(0, -1.25f, -500f);
                ToggleSettingsButtonObject.name = "TOGGLESETTINGSBUTTON";
                SpriteRenderer renderer = ToggleSettingsButtonObject.transform.Find("Inactive").GetComponent<SpriteRenderer>();
                SpriteRenderer rendererActive = ToggleSettingsButtonObject.transform.Find("Active").GetComponent<SpriteRenderer>();
                ToggleSettingsButtonObject.transform.Find("Background").localPosition = Vector3.zero;
                renderer.sprite = Helpers.LoadSpriteFromResources("RebuildUs.Resources.Settings_Button.png", 100f);
                rendererActive.sprite = Helpers.LoadSpriteFromResources("RebuildUs.Resources.Settings_ButtonActive.png", 100);
                ToggleSettingsButton = ToggleSettingsButtonObject.GetComponent<PassiveButton>();
                ToggleSettingsButton.OnClick.RemoveAllListeners();
                ToggleSettingsButton.OnClick.AddListener((Action)(() => ToggleSettings(__instance)));
            }
            ToggleSettingsButtonObject.SetActive(__instance.MapButton.gameObject.active && !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) && GameOptionsManager.Instance.currentGameOptions.GameMode != GameModes.HideNSeek);
            ToggleSettingsButtonObject.transform.localPosition = __instance.MapButton.transform.localPosition + new Vector3(0, -0.8f, -500f);

            if (!ToggleZoomButton || !ToggleZoomButtonObject)
            {
                // add a special button for settings viewing:
                ToggleZoomButtonObject = UnityEngine.Object.Instantiate(__instance.MapButton.gameObject, __instance.MapButton.transform.parent);
                ToggleZoomButtonObject.transform.localPosition = __instance.MapButton.transform.localPosition + new Vector3(0, -1.25f, -500f);
                ToggleZoomButtonObject.name = "TOGGLEZOOMBUTTON";
                SpriteRenderer tZRenderer = ToggleZoomButtonObject.transform.Find("Inactive").GetComponent<SpriteRenderer>();
                SpriteRenderer tZRrenderer = ToggleZoomButtonObject.transform.Find("Active").GetComponent<SpriteRenderer>();
                ToggleZoomButtonObject.transform.Find("Background").localPosition = Vector3.zero;
                tZRenderer.sprite = Helpers.LoadSpriteFromResources("RebuildUs.Resources.Minus_Button.png", 100f);
                tZRrenderer.sprite = Helpers.LoadSpriteFromResources("RebuildUs.Resources.Minus_ButtonActive.png", 100);
                ToggleZoomButton = ToggleZoomButtonObject.GetComponent<PassiveButton>();
                ToggleZoomButton.OnClick.RemoveAllListeners();
                ToggleZoomButton.OnClick.AddListener((Action)(() => Helpers.ToggleZoom()));
            }
            var (playerCompleted, playerTotal) = TasksHandler.TaskInfo(PlayerControl.LocalPlayer.Data);
            int numberOfLeftTasks = playerTotal - playerCompleted;
            bool zoomButtonActive = !(PlayerControl.LocalPlayer == null || !PlayerControl.LocalPlayer.Data.IsDead || (PlayerControl.LocalPlayer.Data.Role.IsImpostor && !CustomOptionHolder.BlockSabotageFromDeadImpostors.GetBool()) || MeetingHud.Instance);
            zoomButtonActive &= numberOfLeftTasks <= 0 || !CustomOptionHolder.FinishTasksBeforeHauntingOrZoomingOut.GetBool();
            ToggleZoomButtonObject.SetActive(zoomButtonActive);
            var posOffset = Helpers.ZoomOutStatus ? new Vector3(-1.27f, -7.92f, -52f) : new Vector3(0, -1.6f, -52f);
            ToggleZoomButtonObject.transform.localPosition = HudManager.Instance.MapButton.transform.localPosition + posOffset;
        }

        public static void ToggleSummaryButtonHandler(HudManager __instance)
        {
            if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
            {
                if (ToggleSummaryButtonObject != null)
                {
                    ToggleSummaryButtonObject.SetActive(false);
                    ToggleSummaryButtonObject.Destroy();
                    ToggleSummaryButton.Destroy();
                }
                return;
            }
            if (!ToggleSummaryButton || !ToggleSummaryButtonObject)
            {
                // add a special button for settings viewing:
                ToggleSummaryButtonObject = UnityEngine.Object.Instantiate(__instance.MapButton.gameObject, __instance.MapButton.transform.parent);
                ToggleSummaryButtonObject.transform.localPosition = __instance.MapButton.transform.localPosition + new Vector3(0, -1.25f, -500f);
                ToggleSummaryButtonObject.name = "TOGGLESUMMARYSBUTTON";
                SpriteRenderer renderer = ToggleSummaryButtonObject.transform.Find("Inactive").GetComponent<SpriteRenderer>();
                SpriteRenderer rendererActive = ToggleSummaryButtonObject.transform.Find("Active").GetComponent<SpriteRenderer>();
                ToggleSummaryButtonObject.transform.Find("Background").localPosition = Vector3.zero;
                renderer.sprite = Helpers.LoadSpriteFromResources("RebuildUs.Resources.Endscreen.png", 100f);
                rendererActive.sprite = Helpers.LoadSpriteFromResources("RebuildUs.Resources.EndscreenActive.png", 100f);
                ToggleSummaryButton = ToggleSummaryButtonObject.GetComponent<PassiveButton>();
                ToggleSummaryButton.OnClick.RemoveAllListeners();
                ToggleSummaryButton.OnClick.AddListener((Action)(() => ToggleSummary(__instance)));
            }
            ToggleSummaryButtonObject.SetActive(__instance.SettingsButton.gameObject.active && LobbyBehaviour.Instance && !Helpers.PreviousEndGameSummary.IsNullOrWhiteSpace() && GameOptionsManager.Instance.currentGameOptions.GameMode != GameModes.HideNSeek
                && AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started);
            ToggleSummaryButtonObject.transform.localPosition = __instance.SettingsButton.transform.localPosition + new Vector3(-1.45f, 0.03f, -500f);
        }
    }
}