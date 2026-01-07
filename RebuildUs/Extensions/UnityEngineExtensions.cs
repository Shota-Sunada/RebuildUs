namespace RebuildUs.Extensions;

public static class UnityEngineExtensions
{
    public static void Destroy(this UnityEngine.Object obj)
    {
        UnityEngine.Object.Destroy(obj);
    }
}