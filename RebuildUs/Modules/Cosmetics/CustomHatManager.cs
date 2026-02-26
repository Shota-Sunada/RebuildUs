using System.Security.Cryptography;

namespace RebuildUs.Modules.Cosmetics;

internal static class CustomHatManager
{
    private const string RESOURCES_DIRECTORY = "RebuildHats";
    internal const string INNERSLOTH_PACKAGE_NAME = "Innersloth Hats";
    internal const string DEVELOPER_PACKAGE_NAME = "Developer Hats";

    internal static readonly string[] Repositories =
    [
        "https://raw.githubusercontent.com/TheOtherRolesAU/TheOtherHats/master",
        "https://raw.githubusercontent.com/hinakkyu/TheOtherHats/master",
        "https://raw.githubusercontent.com/yukinogatari/TheOtherHats-GM/main",
    ];

    internal static readonly List<CustomHat> UnregisteredHats = [];
    private static readonly Dictionary<string, HatViewData> ViewDataCache = [];
    internal static readonly Dictionary<string, HatViewData> ViewDataCacheByName = [];
    internal static readonly Dictionary<string, HatExtension> ExtensionCache = [];

    internal static readonly HatsLoader Loader;

    static CustomHatManager()
    {
        Loader = RebuildUs.Instance?.AddComponent<HatsLoader>();
    }

    private static string CustomSkinsDirectory
    {
        get => Path.Combine(Path.GetDirectoryName(Application.dataPath) ?? string.Empty, RESOURCES_DIRECTORY);
    }

    internal static string HatsDirectory
    {
        get => CustomSkinsDirectory;
    }

    internal static HatExtension TestExtension { get; private set; }

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

    internal static HatData CreateHatBehaviour(CustomHat ch, bool testOnly = false)
    {
        HatViewData viewData = ScriptableObject.CreateInstance<HatViewData>();
        HatData hat = ScriptableObject.CreateInstance<HatData>();

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

        HatExtension extend = new()
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
            ExtensionCache[hat.name] = extend;
            ViewDataCache[hat.name] = viewData;
            ViewDataCacheByName[hat.name] = viewData;
        }

        hat.ViewDataRef = new(viewData.Pointer);
        hat.CreateAddressableAsset();
        return hat;
    }

    private static Sprite CreateHatSprite(string path)
    {
        Texture2D texture = Helpers.LoadTextureFromDisk(Path.Combine(HatsDirectory, path));
        if (texture == null) return null;
        Sprite sprite = Sprite.Create(texture, new(0, 0, texture.width, texture.height), new(0.53f, 0.575f), texture.width * 0.375f);
        if (sprite == null) return null;
        texture.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
        sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;

        return sprite;
    }

    internal static List<CustomHat> CreateHatDetailsFromFileNames(string[] fileNames, bool fromDisk = false)
    {
        Dictionary<string, CustomHat> fronts = new();
        Dictionary<string, string> backs = new();
        Dictionary<string, string> flips = new();
        Dictionary<string, string> backFlips = new();
        Dictionary<string, string> climbs = new();

        foreach (string fileName in fileNames)
        {
            int index = fileName.LastIndexOf("\\", StringComparison.InvariantCulture) + 1;
            string s = fromDisk ? fileName[index..].Split('.')[0] : fileName.Split('.')[3];
            string[] p = s.Split('_');
            HashSet<string> options = new(p);
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

        List<CustomHat> hats = new();

        foreach ((string k, CustomHat hat) in fronts)
        {
            backs.TryGetValue(k, out string backResource);
            climbs.TryGetValue(k, out string climbResource);
            flips.TryGetValue(k, out string flipResource);
            backFlips.TryGetValue(k, out string backFlipResource);
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
        foreach (CustomHat hat in response.Hats)
        {
            hat.Resource = SanitizeFileName(hat.Resource);
            hat.BackResource = SanitizeFileName(hat.BackResource);
            hat.ClimbResource = SanitizeFileName(hat.ClimbResource);
            hat.FlipResource = SanitizeFileName(hat.FlipResource);
            hat.BackFlipResource = SanitizeFileName(hat.BackFlipResource);
        }

        return response.Hats;
    }

    internal static string SanitizeFileName(string path)
    {
        if (path == null) return null;
        if (!path.EndsWith(".png", StringComparison.OrdinalIgnoreCase)) return null;

        StringBuilder sb = new();
        // We only want to sanitize the part before the .png extension
        int nameLen = path.Length - 4;
        for (int i = 0; i < nameLen; i++)
        {
            char c = path[i];
            if (c is '\\' or '/' or '*' or '.')
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
        string filePath = Path.Combine(HatsDirectory, resFile);
        if (resHash == null || !File.Exists(filePath)) return true;

        using FileStream stream = File.OpenRead(filePath);
        string hash = BitConverter.ToString(algorithm.ComputeHash(stream)).Replace("-", string.Empty).ToLowerInvariant();
        return !resHash.Equals(hash);
    }

    internal static List<string> GenerateDownloadList(List<CustomHat> hats)
    {
        using MD5 algorithm = MD5.Create();
        List<string> toDownload = new();

        foreach (CustomHat hat in hats)
        {
            if (hat.Resource != null && ResourceRequireDownload(hat.Resource, hat.ResHashA, algorithm)) toDownload.Add(hat.Resource);
            if (hat.BackResource != null && ResourceRequireDownload(hat.BackResource, hat.ResHashB, algorithm)) toDownload.Add(hat.BackResource);
            if (hat.ClimbResource != null && ResourceRequireDownload(hat.ClimbResource, hat.ResHashC, algorithm)) toDownload.Add(hat.ClimbResource);
            if (hat.FlipResource != null && ResourceRequireDownload(hat.FlipResource, hat.ResHashF, algorithm)) toDownload.Add(hat.FlipResource);
            if (hat.BackFlipResource != null && ResourceRequireDownload(hat.BackFlipResource, hat.ResHashBf, algorithm)) toDownload.Add(hat.BackFlipResource);
        }

        return toDownload;
    }

    extension(HatData hat)
    {
        internal bool TryGetCached(out HatViewData asset)
        {
            if (hat != null) return ViewDataCache.TryGetValue(hat.name, out asset);
            asset = null;
            return false;
        }

        private bool IsCached()
        {
            return hat != null && ViewDataCache.ContainsKey(hat.name);
        }
    }

    extension(HatParent hatParent)
    {
        internal bool IsCached()
        {
            return hatParent != null && hatParent.Hat != null && hatParent.Hat.IsCached();
        }
    }
}