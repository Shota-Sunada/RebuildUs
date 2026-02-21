namespace RebuildUs.Modules.Random;

internal static class RandomMain
{
    internal static System.Random Rnd { get; private set; } = new MersenneTwister((int)DateTime.Now.Ticks);

    internal static void RefreshRnd(int seed)
    {
        Rnd = CustomOptionHolder.RandomNumberAlgorithm.Selection switch
        {
            0 => new(seed),
            1 => new MersenneTwister(seed),
            2 => new Xoshiro256PlusPlus(seed),
            3 => new Xoshiro256StarStar(seed),
            4 => new Pcg64(seed),
            _ => new Xoshiro256PlusPlus(seed),
        };
    }

    internal static void LogScore()
    {
        Logger.LogInfo(RandomScorer.MeasureQuality(Rnd), "RandomScorer");
    }
}