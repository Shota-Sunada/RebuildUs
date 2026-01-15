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
    }
}