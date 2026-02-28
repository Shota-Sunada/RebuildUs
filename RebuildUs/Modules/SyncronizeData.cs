namespace RebuildUs.Modules;

internal enum SynchronizeTag
{
    PreSpawnMinigame,
}

internal sealed class SynchronizeData
{
    private readonly Dictionary<SynchronizeTag, ulong> _dic;

    internal SynchronizeData()
    {
        _dic = [];
    }

    internal void Synchronize(SynchronizeTag tag, byte playerId)
    {
        _dic.TryAdd(tag, 0);

        _dic[tag] |= (ulong)1 << playerId;
    }

    internal bool Align(SynchronizeTag tag, bool withGhost, bool withSurvivor = true)
    {
        if (!_dic.TryGetValue(tag, out var value))
        {
            return false;
        }

        foreach (var pc in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (pc == null || pc.Data == null || pc.Data.Disconnected)
            {
                continue;
            }

            var shouldCheck = pc.Data.IsDead ? withGhost : withSurvivor;
            if (!shouldCheck)
            {
                continue;
            }
            if ((value & (ulong)1 << pc.PlayerId) == 0)
            {
                return false;
            }
        }

        return true;
    }

    internal void Reset(SynchronizeTag tag)
    {
        _dic[tag] = 0;
    }

    internal void Initialize()
    {
        _dic.Clear();
    }
}