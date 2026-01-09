using System.Text;

namespace RebuildUs;

public static class Logger
{
    private static ManualLogSource LogSource;

    public static void Initialize(ManualLogSource log)
    {
        LogSource = log;
    }

    public static void LogInfo(string text, string[] args)
    {
        LogSource.LogInfo(string.Format(text, args));
    }
    public static void LogInfo(string text, string tag, string[] args)
    {
        LogSource.LogInfo(BuildLog(string.Format(text, args), tag));
    }
    public static void LogInfo(object obj, string tag = null)
    {
        LogSource.LogInfo(tag == null ? obj : BuildLog(obj, tag));
    }

    public static void LogMessage(string text, string[] args)
    {
        LogSource.LogMessage(string.Format(text, args));
    }
    public static void LogMessage(string text, string tag, string[] args)
    {
        LogSource.LogMessage(BuildLog(string.Format(text, args), tag));
    }
    public static void LogMessage(object obj, string tag = null)
    {
        LogSource.LogMessage(tag == null ? obj : BuildLog(obj, tag));
    }

    public static void LogWarn(string text, string[] args)
    {
        LogSource.LogWarning(string.Format(text, args));
    }
    public static void LogWarn(string text, string tag, string[] args)
    {
        LogSource.LogWarning(BuildLog(string.Format(text, args), tag));
    }
    public static void LogWarn(object obj, string tag = null)
    {
        LogSource.LogWarning(tag == null ? obj : BuildLog(obj, tag));
    }

    public static void LogError(string text, string[] args)
    {
        LogSource.LogError(string.Format(text, args));
    }
    public static void LogError(string text, string tag, string[] args)
    {
        LogSource.LogError(BuildLog(string.Format(text, args), tag));
    }
    public static void LogError(object obj, string tag = null)
    {
        LogSource.LogError(tag == null ? obj : BuildLog(obj, tag));
    }

    public static void LogFatal(string text, string[] args)
    {
        LogSource.LogFatal(string.Format(text, args));
    }
    public static void LogFatal(string text, string tag, string[] args)
    {
        LogSource.LogFatal(BuildLog(string.Format(text, args), tag));
    }
    public static void LogFatal(object obj, string tag = null)
    {
        LogSource.LogFatal(tag == null ? obj : BuildLog(obj, tag));
    }

    public static void LogDebug(string text, string[] args)
    {
        LogSource.LogDebug(string.Format(text, args));
    }
    public static void LogDebug(string text, string tag, string[] args)
    {
        LogSource.LogDebug(BuildLog(string.Format(text, args), tag));
    }
    public static void LogDebug(object obj, string tag = null)
    {
        LogSource.LogDebug(tag == null ? obj : BuildLog(obj, tag));
    }

    public static string BuildLog(object text, string tag)
    {
        var sb = new StringBuilder();
        sb.Append('[').Append(tag).Append("] ").Append(text);
        return sb.ToString();
    }
}