namespace RebuildUs.Roles;

internal abstract class PlayerRole
{
    internal static readonly List<PlayerRole> AllRoles = [];
    private static readonly Dictionary<byte, PlayerRole> PlayerRoleCache = [];
    internal RoleType CurrentRoleType;
    internal PlayerControl Player;

    internal virtual Color RoleColor
    {
        get => Color.white;
    }

    internal virtual string NameTag
    {
        get => "";
    }

    internal virtual void OnUpdateNameColors() { }
    internal virtual void OnUpdateNameTags() { }

    internal abstract void OnMeetingStart();
    internal abstract void OnMeetingEnd();
    internal abstract void OnIntroEnd();
    internal abstract void FixedUpdate();
    internal abstract void OnKill(PlayerControl target);
    internal abstract void OnDeath(PlayerControl killer = null);
    internal abstract void OnFinishShipStatusBegin();
    internal abstract void HandleDisconnect(PlayerControl player, DisconnectReasons reason);

    internal virtual void ResetRole() { }
    internal virtual void PostInit() { }
    internal virtual string ModifyNameText(string nameText) { return nameText; }
    internal virtual string MeetingInfoText() { return ""; }

    internal static void ClearAll()
    {
        AllRoles.Clear();
        PlayerRoleCache.Clear();
    }

    internal static void RemoveFromCache(byte playerId)
    {
        PlayerRoleCache.Remove(playerId);
    }

    internal static PlayerRole GetRole(PlayerControl player)
    {
        if (player == null) return null;
        if (PlayerRoleCache.TryGetValue(player.PlayerId, out PlayerRole role)) return role;

        foreach (PlayerRole t in AllRoles)
        {
            if (t.Player != player) continue;
            PlayerRoleCache[player.PlayerId] = t;
            return t;
        }

        return null;
    }
}

[HarmonyPatch]
internal abstract class RoleBase<T> : PlayerRole where T : RoleBase<T>, new()
{
    internal static readonly List<T> Players = [];
    internal static RoleType StaticRoleType;

    internal static T Local
    {
        get
        {
            PlayerControl local = PlayerControl.LocalPlayer;
            if (local == null) return null;
            foreach (T t in Players)
                if (t.Player == local)
                    return t;

            return null;
        }
    }

    internal static List<PlayerControl> AllPlayers
    {
        get
        {
            List<PlayerControl> list = new(Players.Count);
            foreach (T t in Players) list.Add(t.Player);

            return list;
        }
    }

    internal static List<PlayerControl> LivingPlayers
    {
        get
        {
            List<PlayerControl> list = new(Players.Count);
            foreach (T t in Players)
            {
                PlayerControl p = t.Player;
                if (p.IsAlive()) list.Add(p);
            }

            return list;
        }
    }

    internal static List<PlayerControl> DeadPlayers
    {
        get
        {
            List<PlayerControl> list = new(Players.Count);
            foreach (T t in Players)
            {
                PlayerControl p = t.Player;
                if (!p.IsAlive()) list.Add(p);
            }

            return list;
        }
    }

    internal static bool Exists
    {
        get => Helpers.RolesEnabled && Players.Count > 0;
    }

    private void Init(PlayerControl player)
    {
        Player = player;
        Players.Add((T)this);
        AllRoles.Add(this);
        RemoveFromCache(player.PlayerId);
    }

    internal new static T GetRole(PlayerControl player = null)
    {
        player ??= PlayerControl.LocalPlayer;
        if (player == null) return null;
        foreach (T t in Players)
            if (t.Player == player)
                return t;

        return null;
    }

    // ReSharper disable once MemberCanBeProtected.Global
    public static bool IsRole(PlayerControl player)
    {
        if (player == null) return false;
        for (int i = 0; i < Players.Count; i++)
            if (Players[i].Player == player)
                return true;

        return false;
    }

    public static void SetRole(PlayerControl player)
    {
        if (player == null || IsRole(player)) return;
        T role = new();
        role.Init(player);
    }

    public static void EraseRole(PlayerControl player)
    {
        if (player == null) return;
        RemoveFromCache(player.PlayerId);
        for (int i = 0; i < Players.Count; i++)
        {
            T x = Players[i];
            if (x.Player != player || x.CurrentRoleType != StaticRoleType) continue;
            x.ResetRole();
            Players.RemoveAt(i);
        }

        for (int i = 0; i < AllRoles.Count; i++)
        {
            PlayerRole x = AllRoles[i];
            if (x.Player == player && x.CurrentRoleType == StaticRoleType) AllRoles.RemoveAt(i);
        }
    }

    public static void SwapRole(PlayerControl p1, PlayerControl p2)
    {
        if (p1 == null || p2 == null) return;
        RemoveFromCache(p1.PlayerId);
        RemoveFromCache(p2.PlayerId);
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].Player == p1)
                Players[i].Player = p2;
            else if (Players[i].Player == p2) Players[i].Player = p1;
        }
    }
}