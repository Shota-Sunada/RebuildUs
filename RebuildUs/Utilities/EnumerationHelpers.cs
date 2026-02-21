using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using Object = Il2CppSystem.Object;

namespace RebuildUs.Utilities;

internal static class EnumerationHelpers
{
    internal static IEnumerable<T> GetFastEnumerator<T>(this Il2CppSystem.Collections.Generic.List<T> list) where T : Object
    {
        return new Il2CppListEnumerable<T>(list);
    }
}

internal sealed unsafe class Il2CppListEnumerable<T> : IEnumerable<T>, IEnumerator<T> where T : Object
{
    private static readonly int ElemSize;
    private static readonly int Offset;
    private static readonly Func<IntPtr, T> ObjFactory;

    private readonly IntPtr _arrayPointer;
    private readonly int _count;
    private int _index = -1;

    static Il2CppListEnumerable()
    {
        ElemSize = IntPtr.Size;
        Offset = 4 * IntPtr.Size;

        ConstructorInfo constructor = typeof(T).GetConstructor([typeof(IntPtr)]);
        ParameterExpression ptr = Expression.Parameter(typeof(IntPtr));
        NewExpression create = Expression.New(constructor!, ptr);
        Expression<Func<IntPtr, T>> lambda = Expression.Lambda<Func<IntPtr, T>>(create, ptr);
        ObjFactory = lambda.Compile();
    }

    internal Il2CppListEnumerable(Il2CppSystem.Collections.Generic.List<T> list)
    {
        Il2CppListStruct* listStruct = (Il2CppListStruct*)list.Pointer;
        _count = listStruct->Size;
        _arrayPointer = listStruct->Items;
    }

    public T Current { get; private set; }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this;
    }

    object IEnumerator.Current
    {
        get => Current;
    }

    public bool MoveNext()
    {
        if (++_index >= _count) return false;
        IntPtr refPtr = *(IntPtr*)IntPtr.Add(IntPtr.Add(_arrayPointer, Offset), _index * ElemSize);
        Current = ObjFactory(refPtr);
        return true;
    }

    public void Reset()
    {
        _index = -1;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return this;
    }

    public void Dispose() { }

    private struct Il2CppListStruct
    {
#pragma warning disable CS0169
        private readonly IntPtr _unusedPtr1;
        private readonly IntPtr _unusedPtr2;
#pragma warning restore CS0169

#pragma warning disable CS0649
        internal IntPtr Items;
        internal int Size;
#pragma warning restore CS0649
    }
}