namespace RebuildUs.Roles;

internal static class RoleColorRegistry
{
    private const string CLOSE_TAG = "</color>";

    private static readonly Color[] RoleColors = new Color[256];
    private static readonly bool[] HasRoleColors = new bool[256];
    private static readonly string[] RoleOpenTags = new string[256];

    private static readonly Color[] ModifierColors = new Color[256];
    private static readonly bool[] HasModifierColors = new bool[256];
    private static readonly string[] ModifierOpenTags = new string[256];

    internal static void RegisterRoleColor(RoleType roleType, Color color)
    {
        var index = (byte)roleType;
        RoleColors[index] = color;
        HasRoleColors[index] = true;
        RoleOpenTags[index] = BuildOpenTag(color);
    }

    internal static void RegisterModifierColor(ModifierType modifierType, Color color)
    {
        var index = (byte)modifierType;
        ModifierColors[index] = color;
        HasModifierColors[index] = true;
        ModifierOpenTags[index] = BuildOpenTag(color);
    }

    internal static Color GetRoleColor(RoleType roleType, Color fallback = default)
    {
        var index = (byte)roleType;
        if (HasRoleColors[index])
        {
            return RoleColors[index];
        }

        if (fallback != default)
        {
            RegisterRoleColor(roleType, fallback);
            return fallback;
        }

        return Color.white;
    }

    internal static Color GetModifierColor(ModifierType modifierType, Color fallback = default)
    {
        var index = (byte)modifierType;
        if (HasModifierColors[index])
        {
            return ModifierColors[index];
        }

        if (fallback != default)
        {
            RegisterModifierColor(modifierType, fallback);
            return fallback;
        }

        return Color.white;
    }

    internal static string WrapRoleText(RoleType roleType, string text, Color fallbackColor)
    {
        var index = (byte)roleType;
        if (!HasRoleColors[index])
        {
            RegisterRoleColor(roleType, fallbackColor);
        }

        return string.Concat(RoleOpenTags[index], text, CLOSE_TAG);
    }

    internal static string WrapModifierText(ModifierType modifierType, string text, Color fallbackColor)
    {
        var index = (byte)modifierType;
        if (!HasModifierColors[index])
        {
            RegisterModifierColor(modifierType, fallbackColor);
        }

        return string.Concat(ModifierOpenTags[index], text, CLOSE_TAG);
    }

    private static string BuildOpenTag(Color color)
    {
        var r = ToByte(color.r);
        var g = ToByte(color.g);
        var b = ToByte(color.b);
        var a = ToByte(color.a);
        return string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>", r, g, b, a);
    }

    private static byte ToByte(float f)
    {
        return (byte)(Mathf.Clamp01(f) * 255f);
    }
}