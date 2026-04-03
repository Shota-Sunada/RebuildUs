namespace RebuildUs.Core;

internal static class PlayerStore
{
    internal static readonly Dictionary<byte, PlayerData> AllPlayerDataOnStarted = [];

    internal static void OnGameStarted()
    {
        AllPlayerDataOnStarted.Clear();

        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            AllPlayerDataOnStarted.Add(p.PlayerId, new(p.Data.PlayerName, RoleInfo.GetRolesString(p, true, true, [], true)));
        }
    }
}

internal struct PlayerData
{
    internal string Name;
    internal string Roles;

    internal PlayerData(string name, string roles)
    {
        Name = name;
        Roles = roles;
    }
}