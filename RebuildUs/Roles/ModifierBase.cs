namespace RebuildUs.Roles;

public abstract class PlayerModifier
{
    public static List<PlayerModifier> AllModifiers = [];
    public static readonly List<PlayerModifier>[] PlayerModifierCache = new List<PlayerModifier>[256];
    public PlayerControl Player;
    public ModifierType CurrentModifierType;
    public virtual Color ModifierColor => Color.white;
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

    public virtual void ResetRole() { }
    public virtual void PostInit() { }
    public virtual string ModifyNameText(string nameText) { return nameText; }
    public virtual string ModifyRoleText(string roleText, List<RoleInfo> roleInfo, bool useColors = true, bool includeHidden = false) { return roleText; }
    public virtual string MeetingInfoText() { return ""; }

    public static void ClearAll()
    {
        AllModifiers.Clear();
        for (int i = 0; i < 256; i++) PlayerModifierCache[i] = null;
    }

    public static void RemoveFromCache(byte playerId)
    {
        PlayerModifierCache[playerId] = null;
    }

    public static PlayerModifier GetModifier(PlayerControl player, ModifierType type)
    {
        if (player == null) return null;
        var list = GetModifiers(player);
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].CurrentModifierType == type) return list[i];
        }
        return null;
    }

    public static List<PlayerModifier> GetModifiers(PlayerControl player)
    {
        if (player == null) return [];
        if (PlayerModifierCache[player.PlayerId] != null) return PlayerModifierCache[player.PlayerId];

        var list = new List<PlayerModifier>();
        for (int i = 0; i < AllModifiers.Count; i++)
        {
            if (AllModifiers[i].Player == player)
            {
                list.Add(AllModifiers[i]);
            }
        }
        PlayerModifierCache[player.PlayerId] = list;
        return list;
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
        PlayerModifier.RemoveFromCache(player.PlayerId);
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

    public static T GetModifier(PlayerControl player = null)
    {
        player ??= PlayerControl.LocalPlayer;
        if (player == null) return null;
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].Player == player) return Players[i];
        }
        return null;
    }

    public static bool HasModifier(PlayerControl player)
    {
        if (player == null) return false;
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].Player == player) return true;
        }
        return false;
    }

    public static T AddModifier(PlayerControl player)
    {
        if (player == null) return null;
        T mod = new();
        mod.Init(player);
        return mod;
    }

    public static void EraseModifier(PlayerControl player)
    {
        if (player == null) return;
        PlayerModifier.RemoveFromCache(player.PlayerId);

        for (int i = Players.Count - 1; i >= 0; i--)
        {
            var x = Players[i];
            if (x.Player == player && x.CurrentModifierType == StaticModifierType)
            {
                x.ResetRole(); // Assuming ResetRole exists from PlayerModifier
                Players.RemoveAt(i);
            }
        }
        for (int i = AllModifiers.Count - 1; i >= 0; i--)
        {
            var x = AllModifiers[i];
            if (x.Player == player && x.CurrentModifierType == StaticModifierType)
            {
                AllModifiers.RemoveAt(i);
            }
        }
    }

    public static void SwapModifier(PlayerControl p1, PlayerControl p2)
    {
        if (p1 == null || p2 == null) return;
        PlayerModifier.RemoveFromCache(p1.PlayerId);
        PlayerModifier.RemoveFromCache(p2.PlayerId);
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].Player == p1)
            {
                Players[i].Player = p2;
            }
            else if (Players[i].Player == p2)
            {
                Players[i].Player = p1;
            }
        }
    }
}