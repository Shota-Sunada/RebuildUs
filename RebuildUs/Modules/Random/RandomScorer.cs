namespace RebuildUs.Modules.Random;

internal static class RandomScorer
{
    internal static string MeasureQuality(System.Random random, int sampleCount = 100000)
    {
        if (random == null)
        {
            return "Random instance is null.";
        }

        int[] frequencies = new int[10];
        long start = DateTime.Now.Ticks;

        for (int i = 0; i < sampleCount; i++)
        {
            frequencies[random.Next(0, 10)]++;
        }

        long end = DateTime.Now.Ticks;
        double durationSeconds = TimeSpan.FromTicks(end - start).TotalSeconds;
        double samplesPerSecond = sampleCount / durationSeconds;

        double expected = sampleCount / 10.0;
        double chiSquare = 0;
        for (int i = 0; i < 10; i++)
        {
            double diff = frequencies[i] - expected;
            chiSquare += diff * diff / expected;
        }

        StringBuilder sb = new();
        sb.AppendLine("--- Random Quality Score ---");
        sb.Append("Algorithm: ").Append(random.GetType().Name).AppendLine();
        sb.Append("Samples: ").Append(sampleCount).AppendLine();
        sb.Append("Speed: ").Append(samplesPerSecond.ToString("N0")).Append(" samples/sec").AppendLine();
        sb.Append("Chi-Square: ").Append(chiSquare.ToString("F4")).Append(" (df=9)").AppendLine();

        switch (chiSquare)
        {
            // Critical values for df=9:
            // 16.919 (p=0.05)
            // 21.666 (p=0.01)
            case < 16.919:
                sb.AppendLine("Result: Uniform distribution (Pass)");
                break;
            case < 21.666:
                sb.AppendLine("Result: Potentially non-uniform (Warning)");
                break;
            default:
                sb.AppendLine("Result: Non-uniform distribution (Fail)");
                break;
        }

        sb.Append("Frequencies: [");
        for (int i = 0; i < 10; i++)
        {
            sb.Append(frequencies[i]);
            if (i < 9)
            {
                sb.Append(", ");
            }
        }

        sb.Append(']');

        return sb.ToString();
    }
}