namespace RebuildUs.Core;

internal static class PlayerStore
{
    internal static readonly Dictionary<byte, PlayerData> AllPlayerDataOnStarted = [];

    internal static void OnGameStarted()
    {
        AllPlayerDataOnStarted.Clear();

        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            AllPlayerDataOnStarted.Add(p.PlayerId, new(p.Data.PlayerName, p.GetRoleName(), p.CurrentOutfit.HatId, p.CurrentOutfit.PetId, p.CurrentOutfit.SkinId, p.CurrentOutfit.VisorId));
        }
    }
}

internal struct PlayerData
{
    internal string Name;
    internal string Roles;

    internal string HatId;
    internal string PetId;
    internal string SkinId;
    internal string VisorId;

    internal PlayerData(string name, string roles, string hatId, string petId, string skinId, string visorId)
    {
        Name = name;
        Roles = roles;
        HatId = hatId;
        PetId = petId;
        SkinId = skinId;
        VisorId = visorId;
    }
}