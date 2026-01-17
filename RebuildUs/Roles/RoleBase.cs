using RebuildUs.Players;

namespace RebuildUs.Roles;

public abstract class PlayerRole
{
    public static List<PlayerRole> AllRoles = [];
    public PlayerControl Player;
    public RoleType CurrentRoleType;

    public abstract void OnMeetingStart();
    public abstract void OnMeetingEnd();
    public abstract void OnIntroEnd();
    public abstract void FixedUpdate();
    public abstract void OnKill(PlayerControl target);
    public abstract void OnDeath(PlayerControl killer = null);
    public abstract void OnFinishShipStatusBegin();
    public abstract void HandleDisconnect(PlayerControl player, DisconnectReasons reason);

    public virtual void ResetRole() { }
    public virtual void PostInit() { }
    public virtual string ModifyNameText(string nameText) { return nameText; }
    public virtual string MeetingInfoText() { return ""; }

    public static void ClearAll()
    {
        AllRoles = [];
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
            return Players.FirstOrDefault(x => x.Player == CachedPlayer.LocalPlayer.PlayerControl);
        }
    }

    public static List<PlayerControl> AllPlayers
    {
        get
        {
            return [.. Players.Select(x => x.Player)];
        }
    }

    public static List<PlayerControl> LivingPlayers
    {
        get
        {
            return [.. Players.Select(x => x.Player).Where(x => x.IsAlive())];
        }
    }

    public static List<PlayerControl> DeadPlayers
    {
        get
        {
            return [.. Players.Select(x => x.Player).Where(x => !x.IsAlive())];
        }
    }

    public static bool Exists
    {
        get { return Helpers.RolesEnabled && Players.Count > 0; }
    }

    public static T GetRole(PlayerControl player = null)
    {
        player ??= CachedPlayer.LocalPlayer.PlayerControl;
        return Players.FirstOrDefault(x => x.Player == player);
    }

    public static bool IsRole(PlayerControl player)
    {
        return Players.Any(x => x.Player == player);
    }

    public static void SetRole(PlayerControl player)
    {
        if (!IsRole(player))
        {
            T role = new();
            role.Init(player);
        }
    }

    public static void EraseRole(PlayerControl player)
    {
        Players.DoIf(x => x.Player == player, x => x.ResetRole());
        Players.RemoveAll(x => x.Player == player && x.CurrentRoleType == StaticRoleType);
        AllRoles.RemoveAll(x => x.Player == player && x.CurrentRoleType == StaticRoleType);
    }

    public static void SwapRole(PlayerControl p1, PlayerControl p2)
    {
        var index = Players.FindIndex(x => x.Player == p1);
        if (index >= 0)
        {
            Players[index].Player = p2;
        }
    }
}