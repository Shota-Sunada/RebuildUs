namespace RebuildUs.Modules.Random;

internal sealed class Xoshiro256PlusPlus : System.Random
{
    private ulong _s0, _s1, _s2, _s3;

    internal Xoshiro256PlusPlus(int seed)
    {
        Seed(seed);
    }

    internal Xoshiro256PlusPlus() : this((int)DateTime.Now.Ticks) { }

    private void Seed(int seed)
    {
        var s = (ulong)seed;
        _s0 = SplitMix64(ref s);
        _s1 = SplitMix64(ref s);
        _s2 = SplitMix64(ref s);
        _s3 = SplitMix64(ref s);
    }

    private static ulong SplitMix64(ref ulong x)
    {
        var z = x += 0x9e3779b97f4a7c15;
        z = (z ^ z >> 30) * 0xbf58476d1ce4e5b9;
        z = (z ^ z >> 27) * 0x94d049bb133111eb;
        return z ^ z >> 31;
    }

    private static ulong Rotl(ulong x, int k)
    {
        return x << k | x >> 64 - k;
    }

    private ulong NextUInt64()
    {
        var result = Rotl(_s0 + _s3, 23) + _s0;

        var t = _s1 << 17;

        _s2 ^= _s0;
        _s3 ^= _s1;
        _s1 ^= _s2;
        _s0 ^= _s3;

        _s2 ^= t;

        _s3 = Rotl(_s3, 45);

        return result;
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
        if (maxValue < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxValue));
        }
        if (maxValue <= 1)
        {
            return 0;
        }
        return (int)(NextDouble() * maxValue);
    }

    public override int Next(int minValue, int maxValue)
    {
        if (minValue > maxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(minValue));
        }
        if (minValue == maxValue)
        {
            return minValue;
        }
        return (int)((long)(NextDouble() * ((long)maxValue - minValue)) + minValue);
    }

    public override void NextBytes(byte[] buffer)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }
        var i = 0;
        while (i < buffer.Length)
        {
            var r = NextUInt64();
            for (var j = 0; j < 8 && i < buffer.Length; j++)
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
}