using System.Text;
using BepInEx.Logging;

namespace RebuildUs;

internal class Logger
{
    private static ManualLogSource _logSource = null!;
    private readonly string _tag;

    internal static void Initialize(ManualLogSource log)
    {
        _logSource = log;
    }

    internal Logger(string tag)
    {
        _tag = tag;
    }

    private string Format(string text, params object[] args)
    {
        StringBuilder sb = new();
        sb.Append('[').Append(_tag).Append("] ");
        if (args != null && args.Length > 0)
        {
            sb.AppendFormat(text, args);
        }
        else
        {
            sb.Append(text);
        }
        return sb.ToString();
    }

#pragma warning disable RS0030 // 禁止 API を使用しない
    internal void LogInfo(string text, params object[] args)
    {
        _logSource.LogInfo(Format(text, args));
    }

    internal void LogMessage(string text, params object[] args)
    {
        _logSource.LogMessage(Format(text, args));
    }

    internal void LogWarn(string text, params object[] args)
    {
        _logSource.LogWarning(Format(text, args));
    }

    internal void LogError(string text, params object[] args)
    {
        _logSource.LogError(Format(text, args));
    }

    internal void LogFatal(string text, params object[] args)
    {
        _logSource.LogFatal(Format(text, args));
    }

    internal void LogDebug(string text, params object[] args)
    {
        _logSource.LogDebug(Format(text, args));
    }
#pragma warning restore RS0030 // 禁止 API を使用しない
}
