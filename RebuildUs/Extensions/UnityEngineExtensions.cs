namespace RebuildUs.Extensions;

public static class UnityEngineExtensions
{
    public static void Destroy(this UnityEngine.Object obj)
    {
        UnityEngine.Object.Destroy(obj);
    }

    public static Transform FindEx(this Transform transform, string name)
    {
        return transform.Find(name) ?? throw new Exception($"The Transform {name} was not found");
    }
}