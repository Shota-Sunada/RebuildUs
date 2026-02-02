namespace RebuildUs.Modules;

public enum SynchronizeTag
{
    PreSpawnMinigame,
}

public class SynchronizeData
{
    private readonly Dictionary<SynchronizeTag, ulong> Dic;

    public SynchronizeData()
    {
        Dic = [];
    }

    public void Synchronize(SynchronizeTag tag, byte playerId)
    {
        if (!Dic.ContainsKey(tag)) Dic[tag] = 0;

        Dic[tag] |= (ulong)1 << playerId;
    }

    public bool Align(SynchronizeTag tag, bool withGhost, bool withSurvivor = true)
    {
        if (!Dic.TryGetValue(tag, out ulong value)) return false;

        foreach (PlayerControl pc in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (pc == null || pc.Data == null || pc.Data.Disconnected) continue;

            bool shouldCheck = pc.Data.IsDead ? withGhost : withSurvivor;
            if (shouldCheck)
            {
                if ((value & ((ulong)1 << pc.PlayerId)) == 0) return false;
            }
        }

        return true;
    }

    public void Reset(SynchronizeTag tag)
    {
        Dic[tag] = 0;
    }

    public void Initialize()
    {
        Dic.Clear();
    }
}