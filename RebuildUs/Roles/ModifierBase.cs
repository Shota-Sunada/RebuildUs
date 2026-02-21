namespace RebuildUs.Roles;

internal abstract class PlayerModifier
{
    internal static readonly List<PlayerModifier> AllModifiers = [];
    private static readonly List<PlayerModifier>[] PlayerModifierCache = new List<PlayerModifier>[256];
    internal ModifierType CurrentModifierType;
    internal PlayerControl Player;

    internal virtual Color ModifierColor
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
    internal virtual string ModifyRoleText(string roleText, List<RoleInfo> roleInfo, bool useColors = true, bool includeHidden = false) { return roleText; }
    internal virtual string MeetingInfoText() { return ""; }

    internal static void ClearAll()
    {
        AllModifiers.Clear();
        for (int i = 0; i < 256; i++) PlayerModifierCache[i] = null;
    }

    internal static void RemoveFromCache(byte playerId)
    {
        PlayerModifierCache[playerId] = null;
    }

    internal static PlayerModifier GetModifier(PlayerControl player, ModifierType type)
    {
        if (player == null) return null;
        List<PlayerModifier> list = GetModifiers(player);
        foreach (PlayerModifier t in list)
            if (t.CurrentModifierType == type)
                return t;

        return null;
    }

    internal static List<PlayerModifier> GetModifiers(PlayerControl player)
    {
        if (player == null) return [];
        if (PlayerModifierCache[player.PlayerId] != null) return PlayerModifierCache[player.PlayerId];

        List<PlayerModifier> list = new();
        foreach (PlayerModifier t in AllModifiers)
            if (t.Player == player)
                list.Add(t);

        PlayerModifierCache[player.PlayerId] = list;
        return list;
    }
}

[HarmonyPatch]
internal abstract class ModifierBase<T> : PlayerModifier where T : ModifierBase<T>, new()
{
    internal static readonly List<T> Players = [];
    internal static ModifierType StaticModifierType = ModifierType.NoModifier;

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
            foreach (T t in Players)
                list.Add(t.Player);

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
        AllModifiers.Add(this);
        RemoveFromCache(player.PlayerId);
    }

    internal static T GetModifier(PlayerControl player = null)
    {
        player ??= PlayerControl.LocalPlayer;
        if (player == null) return null;
        foreach (T t in Players)
            if (t.Player == player)
                return t;

        return null;
    }

    // ReSharper disable once MemberCanBeProtected.Global
    public static bool HasModifier(PlayerControl player)
    {
        if (player == null) return false;
        foreach (T t in Players)
            if (t.Player == player)
                return true;

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
        RemoveFromCache(player.PlayerId);

        for (int i = Players.Count - 1; i >= 0; i--)
        {
            T x = Players[i];
            if (x.Player != player || x.CurrentModifierType != StaticModifierType) continue;
            x.ResetRole(); // Assuming ResetRole exists from PlayerModifier
            Players.RemoveAt(i);
        }

        for (int i = AllModifiers.Count - 1; i >= 0; i--)
        {
            PlayerModifier x = AllModifiers[i];
            if (x.Player == player && x.CurrentModifierType == StaticModifierType) AllModifiers.RemoveAt(i);
        }
    }

    public static void SwapModifier(PlayerControl p1, PlayerControl p2)
    {
        if (p1 == null || p2 == null) return;
        RemoveFromCache(p1.PlayerId);
        RemoveFromCache(p2.PlayerId);
        foreach (T t in Players)
        {
            if (t.Player == p1)
                t.Player = p2;
            else if (t.Player == p2) t.Player = p1;
        }
    }
}