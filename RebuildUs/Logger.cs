namespace RebuildUs;

internal static class Logger
{
#pragma warning disable RS0030 // 禁止 API を使用しない
    private static ManualLogSource _logSource;

    internal static void Initialize(ManualLogSource log)
    {
        _logSource = log;
    }

    internal static void LogInfo(string text, params object[] args)
    {
        _logSource.LogInfo(string.Format(text, args));
    }

    internal static void LogMessage(string text, params object[] args)
    {
        _logSource.LogMessage(string.Format(text, args));
    }

    internal static void LogWarn(string text, params object[] args)
    {
        _logSource.LogWarning(string.Format(text, args));
    }

    internal static void LogError(string text, params object[] args)
    {
        _logSource.LogError(string.Format(text, args));
    }

    internal static void LogFatal(string text, params object[] args)
    {
        _logSource.LogFatal(string.Format(text, args));
    }

    internal static void LogDebug(string text, params object[] args)
    {
        _logSource.LogDebug(string.Format(text, args));
    }

    internal static string BuildLog(object text, string tag)
    {
        StringBuilder sb = new();
        sb.Append('[').Append(tag).Append("] ").Append(text);
        return sb.ToString();
    }
#pragma warning restore RS0030 // 禁止 API を使用しない
}