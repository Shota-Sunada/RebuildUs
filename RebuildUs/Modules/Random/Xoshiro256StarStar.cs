using RebuildUs.Utilities;

namespace RebuildUs.Modules.Random;

internal sealed class Xoshiro256StarStar : System.Random, IDisposable
{
    private IntPtr _nativeState;

    internal Xoshiro256StarStar(int seed)
    {
        _nativeState = NativeMethods.xoshiro256ss_new((ulong)seed);
    }

    internal Xoshiro256StarStar() : this((int)DateTime.Now.Ticks) { }

    private ulong NextUInt64()
    {
        return NativeMethods.xoshiro256ss_next(_nativeState);
    }

    internal uint NextUInt32()
    {
        return (uint)(NextUInt64() >> 32);
    }

    public override int Next()
    {
        return Next(int.MaxValue);
    }

    public override int Next(int maxValue)
    {
        if (maxValue < 0) throw new ArgumentOutOfRangeException(nameof(maxValue));
        if (maxValue <= 1) return 0;
        return (int)(NextDouble() * maxValue);
    }

    public override int Next(int minValue, int maxValue)
    {
        if (minValue > maxValue) throw new ArgumentOutOfRangeException(nameof(minValue));
        if (minValue == maxValue) return minValue;
        return (int)((long)(NextDouble() * ((long)maxValue - minValue)) + minValue);
    }

    public override void NextBytes(byte[] buffer)
    {
        if (buffer == null) throw new ArgumentNullException(nameof(buffer));
        int i = 0;
        while (i < buffer.Length)
        {
            ulong r = NextUInt64();
            for (int j = 0; j < 8 && i < buffer.Length; j++)
            {
                buffer[i++] = (byte)(r & 0xFF);
                r >>= 8;
            }
        }
    }

    public override double NextDouble()
    {
        return (NextUInt64() >> 11) * (1.0 / (1UL << 53));
    }

    public void Dispose()
    {
        if (_nativeState != IntPtr.Zero)
        {
            NativeMethods.xoshiro256ss_free(_nativeState);
            _nativeState = IntPtr.Zero;
        }
    }

    ~Xoshiro256StarStar()
    {
        Dispose();
    }
}