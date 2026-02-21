using System.Runtime.InteropServices;

namespace RebuildUs.Utilities;

internal static class NativeMethods
{
    private const string LibraryName = "rebuild_us_native";

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int add(int a, int b);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr xoshiro256ss_new(ulong seed);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern ulong xoshiro256ss_next(IntPtr ptr);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void xoshiro256ss_free(IntPtr ptr);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool calculate_md5_hash(string path, byte[] outHash);
}