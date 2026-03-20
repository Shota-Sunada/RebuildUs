namespace RebuildUs.Modules.CustomOptions;

internal class CustomGeneralOption<T> : CustomOption<T>
{
    internal CustomGeneralOption(int id,
                                 CustomOptionType type,
                                 TrKey nameKey,
                                 T[] selections,
                                 T defaultValue,
                                 CustomOption parent,
                                 bool hideIfParentEnabled,
                                 string format,
                                 Color color,
                                 CustomOptionHeader header = null)
        : base(id, type, nameKey, selections, defaultValue, parent, hideIfParentEnabled, format, color, header) { }

    internal CustomGeneralOption(int id,
                                 CustomOptionType type,
                                 TrKey nameKey,
                                 T[] selections,
                                 int defaultSelection,
                                 CustomOption parent,
                                 bool hideIfParentEnabled,
                                 string format,
                                 Color color,
                                 CustomOptionHeader header = null)
        : base(id, type, nameKey, selections, defaultSelection, parent, hideIfParentEnabled, format, color, header) { }
}