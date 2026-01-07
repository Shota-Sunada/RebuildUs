using System.Reflection;

namespace RebuildUs;

public static class Helpers
{
    public static Dictionary<string, Sprite> CachedSprites = new();

    public static Sprite LoadSpriteFromResources(string path, float pixelsPerUnit, bool cache = true)
    {
        try
        {
            if (cache && CachedSprites.TryGetValue(path + pixelsPerUnit, out var sprite)) return sprite;
            Texture2D texture = LoadTextureFromResources(path);
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            if (cache) sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;
            if (!cache) return sprite;
            return CachedSprites[path + pixelsPerUnit] = sprite;
        }
        catch
        {
            System.Console.WriteLine("Error loading sprite from path: " + path);
        }
        return null;
    }

    public static unsafe Texture2D LoadTextureFromResources(string path)
    {
        try
        {
            Texture2D texture = new(2, 2, TextureFormat.ARGB32, true);
            var assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(path);
            var length = stream.Length;
            var byteTexture = new Il2CppStructArray<byte>(length);
            stream.Read(new Span<byte>(IntPtr.Add(byteTexture.Pointer, IntPtr.Size * 4).ToPointer(), (int)length));
            if (path.Contains("HorseHats"))
            {
                byteTexture = new Il2CppStructArray<byte>([.. byteTexture.Reverse()]);
            }
            ImageConversion.LoadImage(texture, byteTexture, false);
            return texture;
        }
        catch
        {
            System.Console.WriteLine("Error loading texture from resources: " + path);
        }
        return null;
    }

    public static Texture2D LoadTextureFromDisk(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                Texture2D texture = new(2, 2, TextureFormat.ARGB32, true);
                var byteTexture = Il2CppSystem.IO.File.ReadAllBytes(path);
                ImageConversion.LoadImage(texture, byteTexture, false);
                return texture;
            }
        }
        catch
        {
            RebuildUs.Instance.Logger.LogError("Error loading texture from disk: " + path);
        }
        return null;
    }

    public static object TryCast(this Il2CppObjectBase self, Type type)
    {
        return AccessTools.Method(self.GetType(), nameof(Il2CppObjectBase.TryCast)).MakeGenericMethod(type).Invoke(self, []);
    }

    public static string Cs(Color c, string s)
    {
        return string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>{4}</color>", ToByte(c.r), ToByte(c.g), ToByte(c.b), ToByte(c.a), s);
    }

    private static byte ToByte(float f)
    {
        f = Mathf.Clamp01(f);
        return (byte)(f * 255);
    }

    public static int LineCount(string text)
    {
        return text.Count(c => c == '\n');
    }
    public static string PreviousEndGameSummary = "";

    public static bool HasFakeTasks(this PlayerControl player)
    {
        // return (player == Jester.jester || player == Jackal.jackal || player == Sidekick.sidekick || player == Arsonist.arsonist || player == Vulture.vulture || Jackal.formerJackals.Any(x => x == player));
        return false;
    }
    public static bool ZoomOutStatus = false;
    public static void ToggleZoom(bool reset = false)
    {
        float orthographicSize = reset || ZoomOutStatus ? 3f : 12f;

        ZoomOutStatus = !ZoomOutStatus && !reset;
        Camera.main.orthographicSize = orthographicSize;
        foreach (var cam in Camera.allCameras)
        {
            if (cam != null && cam.gameObject.name == "UI Camera") cam.orthographicSize = orthographicSize;  // The UI is scaled too, else we cant click the buttons. Downside: map is super small.
        }

        var tzGO = GameObject.Find("TOGGLEZOOMBUTTON");
        if (tzGO != null)
        {
            var rend = tzGO.transform.Find("Inactive").GetComponent<SpriteRenderer>();
            var rendActive = tzGO.transform.Find("Active").GetComponent<SpriteRenderer>();
            rend.sprite = ZoomOutStatus ? Helpers.LoadSpriteFromResources("TheOtherRoles.Resources.Plus_Button.png", 100f) : Helpers.LoadSpriteFromResources("TheOtherRoles.Resources.Minus_Button.png", 100f);
            rendActive.sprite = ZoomOutStatus ? Helpers.LoadSpriteFromResources("TheOtherRoles.Resources.Plus_ButtonActive.png", 100f) : Helpers.LoadSpriteFromResources("TheOtherRoles.Resources.Minus_ButtonActive.png", 100f);
            tzGO.transform.localScale = new Vector3(1.2f, 1.2f, 1f) * (ZoomOutStatus ? 4 : 1);
        }

        ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen); // This will move button positions to the correct position.
    }
}