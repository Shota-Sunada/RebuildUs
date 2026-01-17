using RebuildUs.Players;

namespace RebuildUs.Roles;

public abstract class PlayerModifier
{
    public static List<PlayerModifier> AllModifiers = [];
    public PlayerControl Player;
    public ModifierType CurrentModifierType;

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
    public abstract void Clear();

    public virtual void ResetRole() { }
    public virtual void PostInit() { }
    public virtual string ModifyNameText(string nameText) { return nameText; }
    public virtual string ModifyRoleText(string roleText, List<RoleInfo> roleInfo, bool useColors = true, bool includeHidden = false) { return roleText; }
    public virtual string MeetingInfoText() { return ""; }

    public static void ClearAll()
    {
        AllModifiers = [];
    }
}

[HarmonyPatch]
public abstract class ModifierBase<T> : PlayerModifier where T : ModifierBase<T>, new()
{
    public static List<T> Players = [];
    public static ModifierType StaticModifierType;

    public void Init(PlayerControl player)
    {
        Player = player;
        Players.Add((T)this);
        AllModifiers.Add(this);
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

    public static T GetModifier(PlayerControl player = null)
    {
        player ??= CachedPlayer.LocalPlayer.PlayerControl;
        return Players.FirstOrDefault(x => x.Player == player);
    }

    public static bool HasModifier(PlayerControl player)
    {
        return Players.Any(x => x.Player == player);
    }

    public static T AddModifier(PlayerControl player)
    {
        T mod = new();
        mod.Init(player);
        return mod;
    }

    public static void EraseModifier(PlayerControl player)
    {
        var toRemove = new List<T>();

        foreach (var p in Players)
        {
            if (p.Player == player && p.CurrentModifierType == StaticModifierType)
            {
                toRemove.Add(p);
            }
        }
        Players.RemoveAll(x => toRemove.Contains(x));
        AllModifiers.RemoveAll(x => toRemove.Contains(x));
    }

    public static void SwapModifier(PlayerControl p1, PlayerControl p2)
    {
        var index = Players.FindIndex(x => x.Player == p1);
        if (index >= 0)
        {
            Players[index].Player = p2;
        }
    }
}