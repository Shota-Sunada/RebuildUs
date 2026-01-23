namespace RebuildUs.Roles;

public abstract class PlayerRole
{
    public static List<PlayerRole> AllRoles = [];
    private static readonly Dictionary<byte, PlayerRole> PlayerRoleCache = [];
    public PlayerControl Player;
    public RoleType CurrentRoleType;
    public virtual Color RoleColor => Color.white;
    public virtual string NameTag => "";

    public virtual void OnUpdateNameColors() { }
    public virtual void OnUpdateNameTags() { }

    public abstract void OnMeetingStart();
    public abstract void OnMeetingEnd();
    public abstract void OnIntroEnd();
    public abstract void FixedUpdate();
    public abstract void OnKill(PlayerControl target);
    public abstract void OnDeath(PlayerControl killer = null);
    public abstract void OnFinishShipStatusBegin();
    public abstract void HandleDisconnect(PlayerControl player, DisconnectReasons reason);
    public abstract void MakeButtons(HudManager hm);
    public abstract void SetButtonCooldowns();

    public virtual void ResetRole() { }
    public virtual void PostInit() { }
    public virtual string ModifyNameText(string nameText) { return nameText; }
    public virtual string MeetingInfoText() { return ""; }

    public static void ClearAll()
    {
        AllRoles.Clear();
        PlayerRoleCache.Clear();
    }

    public static PlayerRole GetRole(PlayerControl player)
    {
        if (player == null) return null;
        if (PlayerRoleCache.TryGetValue(player.PlayerId, out var role)) return role;

        for (int i = 0; i < AllRoles.Count; i++)
        {
            if (AllRoles[i].Player == player)
            {
                PlayerRoleCache[player.PlayerId] = AllRoles[i];
                return AllRoles[i];
            }
        }
        return null;
    }
}

[HarmonyPatch]
public abstract class RoleBase<T> : PlayerRole where T : RoleBase<T>, new()
{
    public static List<T> Players = [];
    public static RoleType StaticRoleType;

    public void Init(PlayerControl player)
    {
        Player = player;
        Players.Add((T)this);
        AllRoles.Add(this);
    }

    public static T Local
    {
        get
        {
            var local = PlayerControl.LocalPlayer;
            if (local == null) return null;
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].Player == local) return Players[i];
            }
            return null;
        }
    }

    public static List<PlayerControl> AllPlayers
    {
        get
        {
            var list = new List<PlayerControl>(Players.Count);
            for (int i = 0; i < Players.Count; i++) list.Add(Players[i].Player);
            return list;
        }
    }

    public static List<PlayerControl> LivingPlayers
    {
        get
        {
            var list = new List<PlayerControl>(Players.Count);
            for (int i = 0; i < Players.Count; i++)
            {
                var p = Players[i].Player;
                if (p.IsAlive()) list.Add(p);
            }
            return list;
        }
    }

    public static List<PlayerControl> DeadPlayers
    {
        get
        {
            var list = new List<PlayerControl>(Players.Count);
            for (int i = 0; i < Players.Count; i++)
            {
                var p = Players[i].Player;
                if (!p.IsAlive()) list.Add(p);
            }
            return list;
        }
    }

    public static bool Exists
    {
        get { return Helpers.RolesEnabled && Players.Count > 0; }
    }

    public static new T GetRole(PlayerControl player = null)
    {
        player ??= PlayerControl.LocalPlayer;
        if (player == null) return null;
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].Player == player) return Players[i];
        }
        return null;
    }

    public static bool IsRole(PlayerControl player)
    {
        if (player == null) return false;
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].Player == player) return true;
        }
        return false;
    }

    public static void SetRole(PlayerControl player)
    {
        if (player != null && !IsRole(player))
        {
            T role = new();
            role.Init(player);
        }
    }

    public static void EraseRole(PlayerControl player)
    {
        if (player == null) return;
        for (int i = Players.Count - 1; i >= 0; i--)
        {
            var x = Players[i];
            if (x.Player == player && x.CurrentRoleType == StaticRoleType)
            {
                x.ResetRole();
                Players.RemoveAt(i);
            }
        }
        for (int i = AllRoles.Count - 1; i >= 0; i--)
        {
            var x = AllRoles[i];
            if (x.Player == player && x.CurrentRoleType == StaticRoleType)
            {
                AllRoles.RemoveAt(i);
            }
        }
    }

    public static void SwapRole(PlayerControl p1, PlayerControl p2)
    {
        if (p1 == null || p2 == null) return;
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].Player == p1)
            {
                Players[i].Player = p2;
                break;
            }
        }
    }
}