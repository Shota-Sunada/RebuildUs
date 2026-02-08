namespace RebuildUs.Modules.CustomOptions;

public sealed class CustomTasksOption((int commonId, int shortId, int longId) ids, CustomOptionType type, (int commonTasks, int shortTasks, int longTasks) defaultValue, CustomOption parent = null) : CustomOption
{
    public CustomOption CommonTaskOption = Normal(ids.commonId, type, TrKey.NumCommonTask, defaultValue.commonTasks, 0f, 4f, 1f, parent);
    public CustomOption LongTaskOption = Normal(ids.longId, type, TrKey.NumLongTask, defaultValue.longTasks, 0f, 23f, 1f, parent);
    public CustomOption ShortTaskOption = Normal(ids.shortId, type, TrKey.NumShortTask, defaultValue.shortTasks, 0f, 15f, 1f, parent);

    public int CommonTasksNum
    {
        get => Mathf.RoundToInt(CommonTaskOption.GetSelection());
    }

    public int ShortTasksNum
    {
        get => Mathf.RoundToInt(ShortTaskOption.GetSelection());
    }

    public int LongTasksNum
    {
        get => Mathf.RoundToInt(LongTaskOption.GetSelection());
    }
}
