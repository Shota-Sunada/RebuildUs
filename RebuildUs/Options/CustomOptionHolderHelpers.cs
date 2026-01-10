using RebuildUs.Modules.CustomOptions;

namespace RebuildUs.Options;

public static class CustomOptionHolderHelpers
{
    public static bool IsMapSelectionOption(CustomOption option)
    {
        // return option == CustomOptionHolder.propHuntMap && option == CustomOptionHolder.hideNSeekMap;
        return false;
    }

    public static string Cs(Color c, string s)
    {
        return string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>{4}</color>", ToByte(c.r), ToByte(c.g), ToByte(c.b), ToByte(c.a), s);
    }

    private static byte ToByte(float f)
    {
        f = Mathf.Clamp01(f);
        return (byte)(f * 255);
    }
}