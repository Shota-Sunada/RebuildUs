namespace RebuildUs;

internal static class Il2CppHelpers
{
    internal static T CastFast<T>(this Il2CppObjectBase obj) where T : Il2CppObjectBase
    {
        if (obj is T casted)
        {
            return casted;
        }
        return obj.Pointer.CastFast<T>();
    }

    internal static T CastFast<T>(this IntPtr ptr) where T : Il2CppObjectBase
    {
        return CastHelper<T>.Cast(ptr);
    }

    private static class CastHelper<T> where T : Il2CppObjectBase
    {
        internal static readonly Func<IntPtr, T> Cast;

        static CastHelper()
        {
            var constructor = typeof(T).GetConstructor([typeof(IntPtr)]);
            var ptr = Expression.Parameter(typeof(IntPtr));
            var create = Expression.New(constructor!, ptr);
            Expression<Func<IntPtr, T>> lambda = Expression.Lambda<Func<IntPtr, T>>(create, ptr);
            Cast = lambda.Compile();
        }
    }
}