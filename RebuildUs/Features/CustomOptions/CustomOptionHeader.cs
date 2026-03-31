namespace RebuildUs.Features.CustomOptions;

internal sealed class CustomOptionHeader
{
    internal static readonly List<CustomOptionHeader> AllHeaders = [];
    private static readonly Dictionary<COType, List<CustomOptionHeader>> HeadersByType = [];

    internal readonly COType Type;
    internal readonly TrKey NameKey;
    internal readonly Color Color;
    internal readonly List<CustomOption> Options = [];

    internal CustomOptionHeader(COType type, TrKey nameKey, Color? color = null)
    {
        Type = type;
        NameKey = nameKey;
        Color = color ?? Color.white;

        if (!HeadersByType.TryGetValue(type, out var headers))
        {
            headers = [];
            HeadersByType[type] = headers;
        }

        headers.Add(this);
        AllHeaders.Add(this);
    }

    internal void AddOption(CustomOption option)
    {
        if (option == null)
        {
            return;
        }

        Options.Add(option);
    }

    internal string GetTitleText()
    {
        return Helpers.Cs(Color, Tr.Get(NameKey));
    }
}