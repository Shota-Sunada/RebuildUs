namespace RebuildUs.Modules.Random;

public static class RandomMain
{
    public static System.Random Rnd { get; private set; } = new MersenneTwister((int)System.DateTime.Now.Ticks);

    public static void RefreshRnd(int seed)
    {
        Rnd = CustomOptionHolder.RandomNumberAlgorithm.Selection switch
        {
            0 => new System.Random(seed),
            1 => new MersenneTwister(seed),
            2 => new Xoshiro256PlusPlus(seed),
            3 => new Xoshiro256StarStar(seed),
            4 => new Pcg64(seed),
            _ => new Xoshiro256PlusPlus(seed)
        };
    }

    public static void LogScore()
    {
        Logger.LogInfo(RandomScorer.MeasureQuality(Rnd), "RandomScorer");
    }
}
