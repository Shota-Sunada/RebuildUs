using Object = UnityEngine.Object;

namespace RebuildUs.Extensions;

public static class UnityEngineExtensions
{
    public static void Destroy(this Object obj)
    {
        if (obj != null) Object.Destroy(obj);
    }

    public static Transform FindEx(this Transform transform, string name)
    {
        return transform.Find(name) ?? throw new($"The Transform {name} was not found");
    }
}
