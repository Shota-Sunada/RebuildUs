namespace RebuildUs.Modules;

public enum SynchronizeTag
{
    PreSpawnMinigame,
}

public class SynchronizeData
{
    private readonly Dictionary<SynchronizeTag, ulong> dic;

    public SynchronizeData()
    {
        dic = new Dictionary<SynchronizeTag, ulong>();
    }

    public void Synchronize(SynchronizeTag tag, byte playerId)
    {
        if (!dic.ContainsKey(tag)) dic[tag] = 0;

        dic[tag] |= (ulong)1 << playerId;
    }

    public bool Align(SynchronizeTag tag, bool withGhost, bool withSurvivor = true)
    {
        bool result = true;

        dic.TryGetValue(tag, out ulong value);

        foreach (PlayerControl pc in CachedPlayer.AllPlayers)
        {
            if (pc.Data.IsDead ? withGhost : withSurvivor)
                result &= (value & ((ulong)1 << pc.PlayerId)) != 0;
        }

        return result;
    }

    public void Reset(SynchronizeTag tag)
    {
        dic[tag] = 0;
    }

    public void Initialize()
    {
        dic.Clear();
    }
}