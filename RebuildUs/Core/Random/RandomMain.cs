namespace RebuildUs.Core.Random;

internal static class RandomMain
{
    internal static System.Random Rnd { get; private set; } = new MersenneTwister((int)DateTime.Now.Ticks);
    internal static readonly TrKey[] RNAs = [TrKey.RndDotnet, TrKey.RndMT, TrKey.RndXoshiro256, TrKey.RndXoshiro256Ss, TrKey.RndPcg64];

    internal static TrKey CurrentAlgorithm = TrKey.RndDotnet;

    internal static void RefreshRnd(TrKey rnd, int seed)
    {
        CurrentAlgorithm = rnd;
        Rnd = rnd switch
        {
            TrKey.RndDotnet => new(seed),
            TrKey.RndMT => new MersenneTwister(seed),
            TrKey.RndXoshiro256 => new Xoshiro256PlusPlus(seed),
            TrKey.RndXoshiro256Ss => new Xoshiro256StarStar(seed),
            TrKey.RndPcg64 => new Pcg64(seed),
            _ => new Xoshiro256PlusPlus(seed),
        };
    }

    internal static void LogScore()
    {
        Logger.LogInfo("[RandomScorer] {0}", RandomScorer.MeasureQuality(Rnd));
    }
}