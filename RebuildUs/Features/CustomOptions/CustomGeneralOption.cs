namespace RebuildUs.Features.CustomOptions;

internal class CustomGeneralOption<T> : CustomOption<T>
{
    internal CustomGeneralOption(COID id, COType type, TrKey nameKey, T[] selections, T defaultValue, CustomOption parent, bool hideIfParentEnabled, TrKey format, Color color, CustomOptionHeader header = null) :
    base(id, type, nameKey, selections, defaultValue, parent, hideIfParentEnabled, format, color, header)
    { }

    internal CustomGeneralOption(COID id, COType type, TrKey nameKey, T[] selections, int defaultSelection, CustomOption parent, bool hideIfParentEnabled, TrKey format, Color color, CustomOptionHeader header = null) :
    base(id, type, nameKey, selections, defaultSelection, parent, hideIfParentEnabled, format, color, header)
    { }
}