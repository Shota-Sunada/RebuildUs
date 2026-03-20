namespace RebuildUs.Modules.CustomOptions;

internal sealed class CustomTasksOption
{
    private readonly CustomNumberOption _commonTaskOption;
    private readonly CustomNumberOption _longTaskOption;
    private readonly CustomNumberOption _shortTaskOption;

    public CustomTasksOption((int commonId, int shortId, int longId) ids,
                             CustomOptionType type,
                             (int commonTasks, int shortTasks, int longTasks) defaultValue,
                             CustomOption parent = null)
    {
        _commonTaskOption = CustomOption.Normal(ids.commonId, type, TrKey.NumCommonTask, defaultValue.commonTasks, 0f, 4f, 1f, parent);
        _longTaskOption = CustomOption.Normal(ids.longId, type, TrKey.NumLongTask, defaultValue.longTasks, 0f, 23f, 1f, parent);
        _shortTaskOption = CustomOption.Normal(ids.shortId, type, TrKey.NumShortTask, defaultValue.shortTasks, 0f, 15f, 1f, parent);
    }

    internal int CommonTasksNum
    {
        get => Mathf.RoundToInt(_commonTaskOption.GetFloat());
    }

    internal int ShortTasksNum
    {
        get => Mathf.RoundToInt(_shortTaskOption.GetFloat());
    }

    internal int LongTasksNum
    {
        get => Mathf.RoundToInt(_longTaskOption.GetFloat());
    }
}