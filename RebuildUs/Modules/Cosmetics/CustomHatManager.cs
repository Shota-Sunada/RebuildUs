using System.Text;
using System.Security.Cryptography;
using UnityEngine.AddressableAssets;

namespace RebuildUs.Modules.Cosmetics;

public static class CustomHatManager
{
    public const string ResourcesDirectory = "RebuildHats";
    public const string InnerslothPackageName = "Innersloth Hats";
    public const string DeveloperPackageName = "Developer Hats";

    internal static readonly string[] Repositories =
    [
        "https://raw.githubusercontent.com/TheOtherRolesAU/TheOtherHats/master",
        "https://raw.githubusercontent.com/hinakkyu/TheOtherHats/master",
        "https://raw.githubusercontent.com/yukinogatari/TheOtherHats-GM/main"
    ];

    internal static string CustomSkinsDirectory => Path.Combine(Path.GetDirectoryName(Application.dataPath), ResourcesDirectory);
    internal static string HatsDirectory => CustomSkinsDirectory;

    internal static List<CustomHat> UnregisteredHats = [];
    internal static readonly Dictionary<string, HatViewData> ViewDataCache = [];
    internal static readonly Dictionary<string, HatViewData> ViewDataCacheByName = [];
    internal static readonly Dictionary<string, HatExtension> ExtensionCache = [];

    public static readonly HatsLoader Loader;

    internal static HatExtension TestExtension { get; private set; }

    static CustomHatManager()
    {
        Loader = RebuildUs.Instance?.AddComponent<HatsLoader>();
    }

    internal static void LoadHats()
    {
        Loader?.FetchHats();
    }

    internal static bool TryGetCached(this HatParent hatParent, out HatViewData asset)
    {
        if (hatParent != null && hatParent.Hat != null) return ViewDataCache.TryGetValue(hatParent.Hat.name, out asset);
        asset = null;
        return false;
    }

    internal static bool TryGetCached(this HatData hat, out HatViewData asset)
    {
        if (hat == null) { asset = null; return false; }
        return ViewDataCache.TryGetValue(hat.name, out asset);
    }

    internal static bool IsCached(this HatData hat)
    {
        return hat != null && ViewDataCache.ContainsKey(hat.name);
    }

    internal static bool IsCached(this HatParent hatParent)
    {
        return hatParent != null && hatParent.Hat != null && IsCached(hatParent.Hat);
    }

    internal static HatData CreateHatBehaviour(CustomHat ch, bool testOnly = false)
    {
        var viewData = ScriptableObject.CreateInstance<HatViewData>();
        var hat = ScriptableObject.CreateInstance<HatData>();

        viewData.MainImage = CreateHatSprite(ch.Resource) ?? throw new FileNotFoundException("File not downloaded yet");
        viewData.FloorImage = viewData.MainImage;
        if (ch.BackResource != null)
        {
            viewData.BackImage = CreateHatSprite(ch.BackResource);
            ch.Behind = true;
        }

        if (ch.ClimbResource != null)
        {
            viewData.ClimbImage = CreateHatSprite(ch.ClimbResource);
            viewData.LeftClimbImage = viewData.ClimbImage;
        }

        hat.name = ch.Name;
        hat.displayOrder = 99;
        hat.ProductId = "hat_" + ch.Name.Replace(' ', '_');
        hat.InFront = !ch.Behind;
        hat.NoBounce = !ch.Bounce;
        hat.ChipOffset = new Vector2(0f, 0.2f);
        hat.Free = true;

        var extend = new HatExtension
        {
            Author = ch.Author ?? "Unknown",
            Package = ch.Package ?? "Misc.",
            Condition = ch.Condition ?? "none",
            Adaptive = ch.Adaptive,
        };

        if (ch.FlipResource != null)
        {
            extend.FlipImage = CreateHatSprite(ch.FlipResource);
        }

        if (ch.BackFlipResource != null)
        {
            extend.BackFlipImage = CreateHatSprite(ch.BackFlipResource);
        }

        if (testOnly)
        {
            TestExtension = extend;
            TestExtension.Condition = hat.name;
        }
        else
        {
            ExtensionCache[hat.name] = extend;
            ViewDataCache[hat.name] = viewData;
            ViewDataCacheByName[hat.name] = viewData;
        }

        hat.ViewDataRef = new AssetReference(viewData.Pointer);
        hat.CreateAddressableAsset();
        return hat;
    }

    private static Sprite CreateHatSprite(string path)
    {
        var texture = Helpers.LoadTextureFromDisk(System.IO.Path.Combine(HatsDirectory, path));
        if (texture == null) return null;
        var sprite = UnityEngine.Sprite.Create(texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.53f, 0.575f),
            texture.width * 0.375f);
        if (sprite == null) return null;
        texture.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
        sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;

        return sprite;
    }

    public static List<CustomHat> CreateHatDetailsFromFileNames(string[] fileNames, bool fromDisk = false)
    {
        var fronts = new Dictionary<string, CustomHat>();
        var backs = new Dictionary<string, string>();
        var flips = new Dictionary<string, string>();
        var backFlips = new Dictionary<string, string>();
        var climbs = new Dictionary<string, string>();

        foreach (var fileName in fileNames)
        {
            var index = fileName.LastIndexOf("\\", StringComparison.InvariantCulture) + 1;
            var s = fromDisk ? fileName[index..].Split('.')[0] : fileName.Split('.')[3];
            var p = s.Split('_');
            var options = new HashSet<string>(p);
            if (options.Contains("back") && options.Contains("flip"))
            {
                backFlips[p[0]] = fileName;
            }
            else if (options.Contains("climb"))
            {
                climbs[p[0]] = fileName;
            }
            else if (options.Contains("back"))
            {
                backs[p[0]] = fileName;
            }
            else if (options.Contains("flip"))
            {
                flips[p[0]] = fileName;
            }
            else
            {
                fronts[p[0]] = new CustomHat
                {
                    Resource = fileName,
                    Name = p[0].Replace('-', ' '),
                    Bounce = options.Contains("bounce"),
                    Adaptive = options.Contains("adaptive"),
                    Behind = options.Contains("behind"),
                };
            }
        }

        var hats = new List<CustomHat>();

        foreach (var frontKvP in fronts)
        {
            var k = frontKvP.Key;
            var hat = frontKvP.Value;
            backs.TryGetValue(k, out var backResource);
            climbs.TryGetValue(k, out var climbResource);
            flips.TryGetValue(k, out var flipResource);
            backFlips.TryGetValue(k, out var backFlipResource);
            if (backResource != null) hat.BackResource = backResource;
            if (climbResource != null) hat.ClimbResource = climbResource;
            if (flipResource != null) hat.FlipResource = flipResource;
            if (backFlipResource != null) hat.BackFlipResource = backFlipResource;
            if (hat.BackResource != null) hat.Behind = true;
            hats.Add(hat);
        }

        return hats;
    }

    internal static List<CustomHat> SanitizeHats(SkinsConfigFile response)
    {
        foreach (var hat in response.Hats)
        {
            hat.Resource = SanitizeFileName(hat.Resource);
            hat.BackResource = SanitizeFileName(hat.BackResource);
            hat.ClimbResource = SanitizeFileName(hat.ClimbResource);
            hat.FlipResource = SanitizeFileName(hat.FlipResource);
            hat.BackFlipResource = SanitizeFileName(hat.BackFlipResource);
        }

        return response.Hats;
    }

    public static string SanitizeFileName(string path)
    {
        if (path == null) return null;
        if (!path.EndsWith(".png", StringComparison.OrdinalIgnoreCase)) return null;

        var sb = new StringBuilder();
        // We only want to sanitize the part before the .png extension
        int nameLen = path.Length - 4;
        for (int i = 0; i < nameLen; i++)
        {
            char c = path[i];
            if (c == '\\' || c == '/' || c == '*' || c == '.')
            {
                // Skip these characters or handle '..'
                if (c == '.' && i + 1 < nameLen && path[i + 1] == '.')
                {
                    i++; // skip second dot
                }
                continue;
            }
            sb.Append(c);
        }

        sb.Append(".png");
        return sb.ToString();
    }

    private static bool ResourceRequireDownload(string resFile, string resHash, HashAlgorithm algorithm)
    {
        var filePath = Path.Combine(HatsDirectory, resFile);
        if (resHash == null || !File.Exists(filePath))
        {
            return true;
        }
        using var stream = File.OpenRead(filePath);
        var hash = BitConverter.ToString(algorithm.ComputeHash(stream))
            .Replace("-", string.Empty)
            .ToLowerInvariant();
        return !resHash.Equals(hash);
    }

    internal static List<string> GenerateDownloadList(List<CustomHat> hats)
    {
        using var algorithm = MD5.Create();
        var toDownload = new List<string>();

        for (int i = 0; i < hats.Count; i++)
        {
            var hat = hats[i];

            if (hat.Resource != null && ResourceRequireDownload(hat.Resource, hat.ResHashA, algorithm)) toDownload.Add(hat.Resource);
            if (hat.BackResource != null && ResourceRequireDownload(hat.BackResource, hat.ResHashB, algorithm)) toDownload.Add(hat.BackResource);
            if (hat.ClimbResource != null && ResourceRequireDownload(hat.ClimbResource, hat.ResHashC, algorithm)) toDownload.Add(hat.ClimbResource);
            if (hat.FlipResource != null && ResourceRequireDownload(hat.FlipResource, hat.ResHashF, algorithm)) toDownload.Add(hat.FlipResource);
            if (hat.BackFlipResource != null && ResourceRequireDownload(hat.BackFlipResource, hat.ResHashBf, algorithm)) toDownload.Add(hat.BackFlipResource);
        }

        return toDownload;
    }
}