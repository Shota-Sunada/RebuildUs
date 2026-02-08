using System.Linq.Expressions;

namespace RebuildUs.Utilities;

internal static unsafe class FastDestroyableSingleton<T> where T : MonoBehaviour
{
    private static readonly IntPtr FIELD_PTR;
    private static readonly Func<IntPtr, T> CREATE_OBJECT;

    static FastDestroyableSingleton()
    {
        FIELD_PTR = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<DestroyableSingleton<T>>.NativeClassPtr, nameof(DestroyableSingleton<T>._instance));
        var constructor = typeof(T).GetConstructor([typeof(IntPtr)]);
        var ptr = Expression.Parameter(typeof(IntPtr));
        var create = Expression.New(constructor!, ptr);
        var lambda = Expression.Lambda<Func<IntPtr, T>>(create, ptr);
        CREATE_OBJECT = lambda.Compile();
    }

    public static T Instance
    {
        get
        {
            IntPtr objectPointer;
            IL2CPP.il2cpp_field_static_get_value(FIELD_PTR, &objectPointer);
            if (objectPointer == IntPtr.Zero) return DestroyableSingleton<T>.Instance;
            return CREATE_OBJECT(objectPointer);
        }
    }
}
