namespace RebuildUs.Roles;

// 役職のインスタンス情報を保持する純粋な抽象クラス
internal abstract class PlayerRole
{
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
}

// 役職のグローバルな管理（登録・検索など）を担当するクラス
internal static class ModRoleManager
{
    internal static readonly List<PlayerRole> AllRoles = [];
    private static readonly PlayerRole[] PlayerRoleCache = new PlayerRole[256];

    internal static void ClearAll()
    {
        AllRoles.Clear();
        for (int i = 0; i < 256; i++) PlayerRoleCache[i] = null;
    }

    internal static void RemoveFromCache(byte playerId)
    {
        PlayerRoleCache[playerId] = null;
    }

    internal static PlayerRole GetRole(PlayerControl player)
    {
        if (player == null) return null;
        var cached = PlayerRoleCache[player.PlayerId];
        if (cached != null) return cached;

        foreach (PlayerRole t in AllRoles)
        {
            if (t.Player != player) continue;
            PlayerRoleCache[player.PlayerId] = t;
            return t;
        }

        return null;
    }

    internal static void AddRole(PlayerRole role)
    {
        AllRoles.Add(role);
        if (role.Player != null) PlayerRoleCache[role.Player.PlayerId] = null;
    }

    internal static void RemoveRole(PlayerRole role)
    {
        if (role == null) return;
        if (role.Player != null) PlayerRoleCache[role.Player.PlayerId] = null;
        AllRoles.Remove(role);
    }
}

// 最大一人しか割り当てられない役職のベースクラス
[HarmonyPatch]
internal abstract class SingleRoleBase<T> : PlayerRole where T : SingleRoleBase<T>, new()
{
    internal static T Instance;

    // ReSharper disable once StaticMemberInGenericType
    internal static RoleType StaticRoleType;

    internal static PlayerControl PlayerControl
    {
        get => Instance?.Player;
    }

    internal static T Local
    {
        get => IsRole(PlayerControl.LocalPlayer) ? Instance : null;
    }

    internal static bool Exists
    {
        get => Helpers.RolesEnabled && Instance != null;
    }

    private void Init(PlayerControl player)
    {
        Player = player;
        Instance = (T)this;
        ModRoleManager.AddRole(this);
    }

    // ReSharper disable once MemberCanBeProtected.Global
    // ReSharper disable once MemberCanBePrivate.Global
    public static bool IsRole(PlayerControl player)
    {
        return player != null && Instance != null && Instance.Player == player;
    }

    // ReSharper disable once UnusedMember.Global
    public static void SetRole(PlayerControl player)
    {
        if (player == null || IsRole(player)) return;
        if (Instance != null) EraseRole(Instance.Player);
        T role = new();
        role.Init(player);
    }

    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once MemberCanBePrivate.Global
    public static void EraseRole(PlayerControl player)
    {
        if (player == null || Instance == null || Instance.Player != player) return;
        Instance.ResetRole();
        ModRoleManager.RemoveRole(Instance);
        Instance = null;
    }

    // ReSharper disable once UnusedMember.Global
    public static void SwapRole(PlayerControl p1, PlayerControl p2)
    {
        if (p1 == null || p2 == null || Instance == null) return;
        if (Instance.Player != p1 && Instance.Player != p2) return;

        ModRoleManager.RemoveFromCache(p1.PlayerId);
        ModRoleManager.RemoveFromCache(p2.PlayerId);

        Instance.Player = Instance.Player == p1 ? p2 : p1;
    }
}

// 複数人割り当てられる可能性のある役職のベースクラス
[HarmonyPatch]
internal abstract class MultiRoleBase<T> : PlayerRole where T : MultiRoleBase<T>, new()
{
    internal static readonly List<T> Players = [];

    // ReSharper disable once StaticMemberInGenericType
    internal static RoleType StaticRoleType;

    internal static T Local
    {
        get
        {
            PlayerControl local = PlayerControl.LocalPlayer;
            if (local == null) return null;
            foreach (T t in Players)
            {
                if (t.Player == local)
                    return t;
            }

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
        ModRoleManager.AddRole(this);
    }

    internal static T GetRole(PlayerControl player = null)
    {
        player ??= PlayerControl.LocalPlayer;
        if (player == null) return null;
        foreach (T t in Players)
        {
            if (t.Player == player)
                return t;
        }

        return null;
    }

    // ReSharper disable once MemberCanBeProtected.Global
    public static bool IsRole(PlayerControl player)
    {
        if (player == null) return false;
        foreach (var t in Players)
        {
            if (t.Player == player)
                return true;
        }

        return false;
    }

    // ReSharper disable once UnusedMember.Global
    public static void SetRole(PlayerControl player)
    {
        if (player == null || IsRole(player)) return;
        T role = new();
        role.Init(player);
    }

    // ReSharper disable once UnusedMember.Global
    public static void EraseRole(PlayerControl player)
    {
        if (player == null) return;
        ModRoleManager.RemoveFromCache(player.PlayerId);
        for (int i = 0; i < Players.Count; i++)
        {
            T x = Players[i];
            if (x.Player != player || x.CurrentRoleType != StaticRoleType) continue;
            x.ResetRole();
            ModRoleManager.RemoveRole(x);
            Players.RemoveAt(i);
            break;
        }
    }

    // ReSharper disable once UnusedMember.Global
    public static void SwapRole(PlayerControl p1, PlayerControl p2)
    {
        if (p1 == null || p2 == null) return;
        ModRoleManager.RemoveFromCache(p1.PlayerId);
        ModRoleManager.RemoveFromCache(p2.PlayerId);
        foreach (var t in Players)
        {
            if (t.Player == p1)
                t.Player = p2;
            else if (t.Player == p2) t.Player = p1;
        }
    }
}