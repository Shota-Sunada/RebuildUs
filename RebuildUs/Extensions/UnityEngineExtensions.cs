namespace RebuildUs.Extensions;

internal static class UnityEngineExtensions
{
    internal static void Destroy(this UnityObject obj)
    {
        if (obj != null)
        {
            UnityObject.Destroy(obj);
        }
    }

    internal static Transform FindEx(this Transform transform, string name)
    {
        return transform.Find(name) ?? throw new($"The Transform {name} was not found");
    }
}