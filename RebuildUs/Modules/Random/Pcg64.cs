namespace RebuildUs.Modules.Random;

internal sealed class Pcg64 : System.Random
{
    private ulong _inc;
    private ulong _state;

    internal Pcg64(int seed)
    {
        Seed(seed);
    }

    internal Pcg64() : this((int)DateTime.Now.Ticks) { }

    private void Seed(int seed)
    {
        _state = 0U;
        _inc = ((ulong)seed << 1) | 1U;
        NextUInt64();
        _state += (ulong)seed;
        NextUInt64();
    }

    private ulong NextUInt64()
    {
        ulong oldstate = _state;
        _state = (oldstate * 6364136223846793005UL) + _inc;

        // RXS-M-XS output function
        ulong word = ((oldstate >> (int)((oldstate >> 59) + 5)) ^ oldstate) * 12605985483714317049UL;
        return (word >> 43) ^ word;
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
}