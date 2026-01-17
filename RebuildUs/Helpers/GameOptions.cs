namespace RebuildUs.Helpers;

public static class GameOptions
{
    public static bool IsHideNSeekMode
    {
        get
        {
            return GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek;
        }
    }
    public static bool IsNormalMode
    {
        get
        {
            return GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal;
        }
    }

    public static int Get(Int32OptionNames opt)
    {
        return GameOptionsManager.Instance.CurrentGameOptions.GetInt(opt);
    }

    public static int[] Get(Int32ArrayOptionNames opt)
    {
        return GameOptionsManager.Instance.CurrentGameOptions.GetIntArray(opt);
    }

    public static float Get(FloatOptionNames opt)
    {
        return GameOptionsManager.Instance.CurrentGameOptions.GetFloat(opt);
    }

    public static float[] Get(FloatArrayOptionNames opt)
    {
        return GameOptionsManager.Instance.CurrentGameOptions.GetFloatArray(opt);
    }

    public static bool Get(BoolOptionNames opt)
    {
        return GameOptionsManager.Instance.CurrentGameOptions.GetBool(opt);
    }

    public static byte Get(ByteOptionNames opt)
    {
        return GameOptionsManager.Instance.CurrentGameOptions.GetByte(opt);
    }
}