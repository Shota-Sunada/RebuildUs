namespace RebuildUs.Modules.CustomOptions;

internal sealed class CustomOptionHeader
{
    internal static readonly List<CustomOptionHeader> AllHeaders = [];
    private static readonly Dictionary<CustomOptionType, List<CustomOptionHeader>> HeadersByType = [];

    internal readonly int Id;
    internal readonly CustomOptionType Type;
    internal readonly TrKey NameKey;
    internal readonly Color Color;
    internal readonly List<CustomOption> Options = [];

    internal CustomOptionHeader(int id, CustomOptionType type, TrKey nameKey, Color? color = null)
    {
        Id = id;
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