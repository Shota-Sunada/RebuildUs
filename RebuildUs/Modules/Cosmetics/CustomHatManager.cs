using System.Security.Cryptography;

namespace RebuildUs.Modules.Cosmetics;

public static class CustomHatManager
{
    public const string RESOURCES_DIRECTORY = "RebuildHats";
    public const string INNERSLOTH_PACKAGE_NAME = "Innersloth Hats";
    public const string DEVELOPER_PACKAGE_NAME = "Developer Hats";

    internal static readonly string[] REPOSITORIES = ["https://raw.githubusercontent.com/TheOtherRolesAU/TheOtherHats/master", "https://raw.githubusercontent.com/hinakkyu/TheOtherHats/master", "https://raw.githubusercontent.com/yukinogatari/TheOtherHats-GM/main"];

    internal static List<CustomHat> UnregisteredHats = [];
    internal static readonly Dictionary<string, HatViewData> VIEW_DATA_CACHE = [];
    internal static readonly Dictionary<string, HatViewData> VIEW_DATA_CACHE_BY_NAME = [];
    internal static readonly Dictionary<string, HatExtension> EXTENSION_CACHE = [];

    public static readonly HatsLoader LOADER;

    static CustomHatManager()
    {
        LOADER = RebuildUs.Instance?.AddComponent<HatsLoader>();
    }

    internal static string CustomSkinsDirectory
    {
        get => Path.Combine(Path.GetDirectoryName(Application.dataPath), RESOURCES_DIRECTORY);
    }

    internal static string HatsDirectory
    {
        get => CustomSkinsDirectory;
    }

    internal static HatExtension TestExtension { get; private set; }

    internal static void LoadHats()
    {
        LOADER?.FetchHats();
    }

    internal static bool TryGetCached(this HatParent hatParent, out HatViewData asset)
    {
        if (hatParent != null && hatParent.Hat != null) return VIEW_DATA_CACHE.TryGetValue(hatParent.Hat.name, out asset);
        asset = null;
        return false;
    }

    internal static bool TryGetCached(this HatData hat, out HatViewData asset)
    {
        if (hat == null)
        {
            asset = null;
            return false;
        }

        return VIEW_DATA_CACHE.TryGetValue(hat.name, out asset);
    }

    internal static bool IsCached(this HatData hat)
    {
        return hat != null && VIEW_DATA_CACHE.ContainsKey(hat.name);
    }

    internal static bool IsCached(this HatParent hatParent)
    {
        return hatParent != null && hatParent.Hat != null && hatParent.Hat.IsCached();
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
        hat.ChipOffset = new(0f, 0.2f);
        hat.Free = true;

        var extend = new HatExtension
        {
            Author = ch.Author ?? "Unknown",
            Package = ch.Package ?? "Misc.",
            Condition = ch.Condition ?? "none",
            Adaptive = ch.Adaptive,
        };

        if (ch.FlipResource != null) extend.FlipImage = CreateHatSprite(ch.FlipResource);

        if (ch.BackFlipResource != null) extend.BackFlipImage = CreateHatSprite(ch.BackFlipResource);

        if (testOnly)
        {
            TestExtension = extend;
            TestExtension.Condition = hat.name;
        }
        else
        {
            EXTENSION_CACHE[hat.name] = extend;
            VIEW_DATA_CACHE[hat.name] = viewData;
            VIEW_DATA_CACHE_BY_NAME[hat.name] = viewData;
        }

        hat.ViewDataRef = new(viewData.Pointer);
        hat.CreateAddressableAsset();
        return hat;
    }

    private static Sprite CreateHatSprite(string path)
    {
        var texture = Helpers.LoadTextureFromDisk(Path.Combine(HatsDirectory, path));
        if (texture == null) return null;
        var sprite = Sprite.Create(texture, new(0, 0, texture.width, texture.height), new(0.53f, 0.575f), texture.width * 0.375f);
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
                backFlips[p[0]] = fileName;
            else if (options.Contains("climb"))
                climbs[p[0]] = fileName;
            else if (options.Contains("back"))
                backs[p[0]] = fileName;
            else if (options.Contains("flip"))
                flips[p[0]] = fileName;
            else
            {
                fronts[p[0]] = new()
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
        var nameLen = path.Length - 4;
        for (var i = 0; i < nameLen; i++)
        {
            var c = path[i];
            if (c == '\\' || c == '/' || c == '*' || c == '.')
            {
                // Skip these characters or handle '..'
                if (c == '.' && i + 1 < nameLen && path[i + 1] == '.') i++; // skip second dot
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
        if (resHash == null || !File.Exists(filePath)) return true;
        using var stream = File.OpenRead(filePath);
        var hash = BitConverter.ToString(algorithm.ComputeHash(stream)).Replace("-", string.Empty).ToLowerInvariant();
        return !resHash.Equals(hash);
    }

    internal static List<string> GenerateDownloadList(List<CustomHat> hats)
    {
        using var algorithm = MD5.Create();
        var toDownload = new List<string>();

        for (var i = 0; i < hats.Count; i++)
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
