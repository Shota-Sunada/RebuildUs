namespace RebuildUs.Utilities;

internal static unsafe class FastDestroyableSingleton<T> where T : MonoBehaviour
{
    private static readonly IntPtr FieldPtr;
    private static readonly Func<IntPtr, T> CreateObject;

    static FastDestroyableSingleton()
    {
        FieldPtr = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<DestroyableSingleton<T>>.NativeClassPtr, nameof(DestroyableSingleton<T>._instance));
        var constructor = typeof(T).GetConstructor([typeof(IntPtr)]);
        var ptr = Expression.Parameter(typeof(IntPtr));
        var create = Expression.New(constructor!, ptr);
        var lambda = Expression.Lambda<Func<IntPtr, T>>(create, ptr);
        CreateObject = lambda.Compile();
    }

    internal static T Instance
    {
        get
        {
            IntPtr objectPointer;
            IL2CPP.il2cpp_field_static_get_value(FieldPtr, &objectPointer);
            return objectPointer == IntPtr.Zero ? DestroyableSingleton<T>.Instance : CreateObject(objectPointer);
        }
    }

    internal static bool InstanceExists
    {
        get
        {
            IntPtr objectPointer;
            IL2CPP.il2cpp_field_static_get_value(FieldPtr, &objectPointer);
            return objectPointer != IntPtr.Zero;
        }
    }
}