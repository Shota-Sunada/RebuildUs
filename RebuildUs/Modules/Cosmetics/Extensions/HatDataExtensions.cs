namespace RebuildUs.Modules.Cosmetics.Extensions;

internal static class HatDataExtensions
{
    internal static HatExtension GetHatExtension(this HatData hat)
    {
        if (hat == null)
        {
            return null;
        }
        if (CustomHatManager.TestExtension != null && CustomHatManager.TestExtension.Condition.Equals(hat.name))
        {
            return CustomHatManager.TestExtension;
        }

        return CustomHatManager.ExtensionCache.GetValueOrDefault(hat.name);
    }
}