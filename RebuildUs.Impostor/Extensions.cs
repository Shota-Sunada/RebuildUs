using System.Security.Cryptography;

namespace RebuildUs.Impostor;

public static class Extensions
{
    /// <summary>
    /// Implementation of the Fisher-Yates shuffle.
    /// Shuffles the list in place.
    /// </summary>
    public static void Shuffle<T>(this IList<T> list)
    {
        var size = list.Count;
        for (var i = 0; i < size - 1; i++)
        {
            var j = RandomNumberGenerator.GetInt32(i, size);
            (list[j], list[i]) = (list[i], list[j]);
        }
    }
}