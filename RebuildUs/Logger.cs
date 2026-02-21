namespace RebuildUs;

internal static class Logger
{
    private static ManualLogSource _logSource;

    internal static void Initialize(ManualLogSource log)
    {
        _logSource = log;
    }

    internal static void LogInfo(string text, string[] args)
    {
        _logSource.LogInfo(string.Format(text, args));
    }

    internal static void LogInfo(string text, string tag, string[] args)
    {
        _logSource.LogInfo(BuildLog(string.Format(text, args), tag));
    }

    internal static void LogInfo(object obj, string tag = null)
    {
        _logSource.LogInfo(tag == null ? obj : BuildLog(obj, tag));
    }

    internal static void LogMessage(string text, string[] args)
    {
        _logSource.LogMessage(string.Format(text, args));
    }

    internal static void LogMessage(string text, string tag, string[] args)
    {
        _logSource.LogMessage(BuildLog(string.Format(text, args), tag));
    }

    internal static void LogMessage(object obj, string tag = null)
    {
        _logSource.LogMessage(tag == null ? obj : BuildLog(obj, tag));
    }

    internal static void LogWarn(string text, string[] args)
    {
        _logSource.LogWarning(string.Format(text, args));
    }

    internal static void LogWarn(string text, string tag, string[] args)
    {
        _logSource.LogWarning(BuildLog(string.Format(text, args), tag));
    }

    internal static void LogWarn(object obj, string tag = null)
    {
        _logSource.LogWarning(tag == null ? obj : BuildLog(obj, tag));
    }

    internal static void LogError(string text, string[] args)
    {
        _logSource.LogError(string.Format(text, args));
    }

    internal static void LogError(string text, string tag, string[] args)
    {
        _logSource.LogError(BuildLog(string.Format(text, args), tag));
    }

    internal static void LogError(object obj, string tag = null)
    {
        _logSource.LogError(tag == null ? obj : BuildLog(obj, tag));
    }

    internal static void LogFatal(string text, string[] args)
    {
        _logSource.LogFatal(string.Format(text, args));
    }

    internal static void LogFatal(string text, string tag, string[] args)
    {
        _logSource.LogFatal(BuildLog(string.Format(text, args), tag));
    }

    internal static void LogFatal(object obj, string tag = null)
    {
        _logSource.LogFatal(tag == null ? obj : BuildLog(obj, tag));
    }

    internal static void LogDebug(string text, string[] args)
    {
        _logSource.LogDebug(string.Format(text, args));
    }

    internal static void LogDebug(string text, string tag, string[] args)
    {
        _logSource.LogDebug(BuildLog(string.Format(text, args), tag));
    }

    internal static void LogDebug(object obj, string tag = null)
    {
        _logSource.LogDebug(tag == null ? obj : BuildLog(obj, tag));
    }

    internal static string BuildLog(object text, string tag)
    {
        StringBuilder sb = new();
        sb.Append('[').Append(tag).Append("] ").Append(text);
        return sb.ToString();
    }
}