namespace RebuildUs.Modules.Cosmetics.Extensions;

internal static class HatDataExtensions
{
    public static HatExtension GetHatExtension(this HatData hat)
    {
        if (hat == null) return null;
        if (CustomHatManager.TestExtension != null && CustomHatManager.TestExtension.Condition.Equals(hat.name)) return CustomHatManager.TestExtension;

        return CustomHatManager.EXTENSION_CACHE.TryGetValue(hat.name, out var extension) ? extension : null;
    }
}
