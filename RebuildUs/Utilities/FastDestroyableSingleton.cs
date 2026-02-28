namespace RebuildUs.Utilities;

internal static unsafe class FastDestroyableSingleton<T> where T : MonoBehaviour
{
    private static readonly IntPtr FieldPtr;
    private static readonly Func<IntPtr, T> CreateObject;

    static FastDestroyableSingleton()
    {
        FieldPtr = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<DestroyableSingleton<T>>.NativeClassPtr, nameof(DestroyableSingleton<T>._instance));
        ConstructorInfo constructor = typeof(T).GetConstructor([typeof(IntPtr)]);
        ParameterExpression ptr = Expression.Parameter(typeof(IntPtr));
        NewExpression create = Expression.New(constructor!, ptr);
        Expression<Func<IntPtr, T>> lambda = Expression.Lambda<Func<IntPtr, T>>(create, ptr);
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
}