namespace RebuildUs.Modules.CustomOptions;

public class CustomTaskOption((int commonId, int shortId, int longId) ids, CustomOption.CustomOptionType type, (int commonTasks, int shortTasks, int longTasks) defaultValue, CustomOption parent = null) : CustomOption
{
    public CustomOption CommonTaskOption = CreateNormal(ids.commonId, type, "CustomOption.NumCommonTask", defaultValue.commonTasks, 0f, 4f, 1f, parent);
    public CustomOption ShortTaskOption = CreateNormal(ids.shortId, type, "CustomOption.NumShortTask", defaultValue.shortTasks, 0f, 15f, 1f, parent);
    public CustomOption LongTaskOption = CreateNormal(ids.shortId, type, "CustomOption.NumLongTask", defaultValue.longTasks, 0f, 23f, 1f, parent);

    public int CommonTasksNum { get { return Mathf.RoundToInt(CommonTaskOption.GetSelection()); } }
    public int ShortTasksNum { get { return Mathf.RoundToInt(ShortTaskOption.GetSelection()); } }
    public int LongTasksNum { get { return Mathf.RoundToInt(LongTaskOption.GetSelection()); } }
}