using System.Reflection;

namespace RebuildUs;

internal static class ModEventDispatcher
{
    private sealed class EventDelegates<TBase>
    {
        public Action<TBase> OnMeetingStart;
        public Action<TBase> OnMeetingEnd;
        public Action<TBase> OnIntroEnd;
        public Action<TBase> FixedUpdate;
        public Action<TBase, PlayerControl> OnKill;
        public Action<TBase, PlayerControl> OnDeath;
        public Action<TBase> OnFinishShipStatusBegin;
        public Action<TBase, PlayerControl, DisconnectReasons> HandleDisconnect;
    }

    private static readonly Dictionary<Type, EventDelegates<PlayerRole>> RoleDelegatesCache = [];
    private static readonly Dictionary<Type, EventDelegates<PlayerModifier>> ModifierDelegatesCache = [];

    // Cache for creating buttons in specific contexts
    internal static readonly List<Action<HudManager>> GlobalButtonRegistrations = [];
    internal static readonly Dictionary<RoleType, List<Action<HudManager>>> RoleButtonRegistrations = [];
    internal static readonly Dictionary<ModifierType, List<Action<HudManager>>> ModifierButtonRegistrations = [];

    internal static readonly List<Action> GlobalButtonTimers = [];
    internal static readonly Dictionary<RoleType, List<Action>> RoleButtonTimers = [];
    internal static readonly Dictionary<ModifierType, List<Action>> ModifierButtonTimers = [];

    internal static readonly HashSet<RoleType> GeneratedRoleButtons = [];
    internal static readonly HashSet<ModifierType> GeneratedModifierButtons = [];

    internal static void Initialize()
    {
        RoleDelegatesCache.Clear();
        ModifierDelegatesCache.Clear();

        GlobalButtonRegistrations.Clear();
        RoleButtonRegistrations.Clear();
        ModifierButtonRegistrations.Clear();

        GlobalButtonTimers.Clear();
        RoleButtonTimers.Clear();
        ModifierButtonTimers.Clear();

        GeneratedRoleButtons.Clear();
        GeneratedModifierButtons.Clear();

        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (type.IsAbstract) continue;

            var roleAttrs = new List<RegisterRoleAttribute>(type.GetCustomAttributes<RegisterRoleAttribute>());
            var modAttrs = new List<RegisterModifierAttribute>(type.GetCustomAttributes<RegisterModifierAttribute>());

            if (typeof(PlayerRole).IsAssignableFrom(type))
            {
                RoleDelegatesCache[type] = CreateDelegates<PlayerRole>(type);
            }

            if (typeof(PlayerModifier).IsAssignableFrom(type))
            {
                ModifierDelegatesCache[type] = CreateDelegates<PlayerModifier>(type);
            }

            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                var hasCustomButton = false;
                foreach (var _ in method.GetCustomAttributes<RegisterCustomButtonAttribute>()) { hasCustomButton = true; break; }
                if (hasCustomButton)
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length == 1 && parameters[0].ParameterType == typeof(HudManager))
                    {
                        var action = (Action<HudManager>)Delegate.CreateDelegate(typeof(Action<HudManager>), method);
                        if (roleAttrs.Count > 0)
                        {
                            foreach (var roleAttr in roleAttrs)
                            {
                                if (!RoleButtonRegistrations.ContainsKey(roleAttr.RoleType))
                                    RoleButtonRegistrations[roleAttr.RoleType] = [];
                                RoleButtonRegistrations[roleAttr.RoleType].Add(action);
                            }
                        }
                        else if (modAttrs.Count > 0)
                        {
                            foreach (var modAttr in modAttrs)
                            {
                                if (!ModifierButtonRegistrations.ContainsKey(modAttr.ModifierType))
                                    ModifierButtonRegistrations[modAttr.ModifierType] = [];
                                ModifierButtonRegistrations[modAttr.ModifierType].Add(action);
                            }
                        }
                        else
                        {
                            GlobalButtonRegistrations.Add(action);
                        }
                    }
                }

                var hasCustomButtonTimer = false;
                foreach (var _ in method.GetCustomAttributes<SetCustomButtonTimerAttribute>()) { hasCustomButtonTimer = true; break; }
                if (hasCustomButtonTimer)
                {
                    if (method.GetParameters().Length == 0)
                    {
                        var action = (Action)Delegate.CreateDelegate(typeof(Action), method);
                        if (roleAttrs.Count > 0)
                        {
                            foreach (var roleAttr in roleAttrs)
                            {
                                if (!RoleButtonTimers.ContainsKey(roleAttr.RoleType))
                                    RoleButtonTimers[roleAttr.RoleType] = [];
                                RoleButtonTimers[roleAttr.RoleType].Add(action);
                            }
                        }
                        else if (modAttrs.Count > 0)
                        {
                            foreach (var modAttr in modAttrs)
                            {
                                if (!ModifierButtonTimers.ContainsKey(modAttr.ModifierType))
                                    ModifierButtonTimers[modAttr.ModifierType] = [];
                                ModifierButtonTimers[modAttr.ModifierType].Add(action);
                            }
                        }
                        else
                        {
                            GlobalButtonTimers.Add(action);
                        }
                    }
                }
            }
        }
    }

    private static EventDelegates<TBase> CreateDelegates<TBase>(Type concreteType)
    {
        var delegates = new EventDelegates<TBase>();
        var methods = concreteType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        foreach (var method in methods)
        {
            CustomEventAttribute attr = null;
            foreach (var a in method.GetCustomAttributes<CustomEventAttribute>()) { attr = a; break; }
            if (attr == null) continue;

            var parameters = method.GetParameters();

            switch (attr.EventType)
            {
                case CustomEventType.OnMeetingStart:
                case CustomEventType.OnMeetingEnd:
                case CustomEventType.OnIntroEnd:
                case CustomEventType.FixedUpdate:
                case CustomEventType.OnFinishShipStatusBegin:
                    if (parameters.Length != 0) continue;
                    var parameterlessAction = CompileAction<TBase>(concreteType, method);
                    switch (attr.EventType)
                    {
                        case CustomEventType.OnMeetingStart: delegates.OnMeetingStart += parameterlessAction; break;
                        case CustomEventType.OnMeetingEnd: delegates.OnMeetingEnd += parameterlessAction; break;
                        case CustomEventType.OnIntroEnd: delegates.OnIntroEnd += parameterlessAction; break;
                        case CustomEventType.FixedUpdate: delegates.FixedUpdate += parameterlessAction; break;
                        case CustomEventType.OnFinishShipStatusBegin: delegates.OnFinishShipStatusBegin += parameterlessAction; break;
                    }
                    break;

                case CustomEventType.OnKill:
                    if (parameters.Length == 1 && parameters[0].ParameterType == typeof(PlayerControl))
                    {
                        delegates.OnKill += CompileAction<TBase, PlayerControl>(concreteType, method);
                    }
                    break;

                case CustomEventType.OnDeath:
                    if (parameters.Length == 1 && parameters[0].ParameterType == typeof(PlayerControl))
                    {
                        delegates.OnDeath += CompileAction<TBase, PlayerControl>(concreteType, method);
                    }
                    break;

                case CustomEventType.HandleDisconnect:
                    if (parameters.Length == 2
                        && parameters[0].ParameterType == typeof(PlayerControl)
                        && parameters[1].ParameterType == typeof(DisconnectReasons))
                    {
                        delegates.HandleDisconnect += CompileAction<TBase, PlayerControl, DisconnectReasons>(concreteType, method);
                    }
                    break;
            }
        }

        return delegates;
    }

    // Expression-compiled delegate factories (compiled once at init, zero overhead at dispatch)

    private static Action<TBase> CompileAction<TBase>(Type concreteType, MethodInfo method)
    {
        var param = Expression.Parameter(typeof(TBase));
        var call = Expression.Call(Expression.Convert(param, concreteType), method);
        return Expression.Lambda<Action<TBase>>(call, param).Compile();
    }

    private static Action<TBase, T1> CompileAction<TBase, T1>(Type concreteType, MethodInfo method)
    {
        var p0 = Expression.Parameter(typeof(TBase));
        var p1 = Expression.Parameter(typeof(T1));
        var call = Expression.Call(Expression.Convert(p0, concreteType), method, p1);
        return Expression.Lambda<Action<TBase, T1>>(call, p0, p1).Compile();
    }

    private static Action<TBase, T1, T2> CompileAction<TBase, T1, T2>(Type concreteType, MethodInfo method)
    {
        var p0 = Expression.Parameter(typeof(TBase));
        var p1 = Expression.Parameter(typeof(T1));
        var p2 = Expression.Parameter(typeof(T2));
        var call = Expression.Call(Expression.Convert(p0, concreteType), method, p1, p2);
        return Expression.Lambda<Action<TBase, T1, T2>>(call, p0, p1, p2).Compile();
    }

    // --- Dispatch Methods for PlayerRole ---

    internal static void DispatchOnMeetingStart(PlayerRole role) => RoleDelegatesCache.GetValueOrDefault(role.GetType())?.OnMeetingStart?.Invoke(role);
    internal static void DispatchOnMeetingEnd(PlayerRole role) => RoleDelegatesCache.GetValueOrDefault(role.GetType())?.OnMeetingEnd?.Invoke(role);
    internal static void DispatchOnIntroEnd(PlayerRole role) => RoleDelegatesCache.GetValueOrDefault(role.GetType())?.OnIntroEnd?.Invoke(role);
    internal static void DispatchFixedUpdate(PlayerRole role) => RoleDelegatesCache.GetValueOrDefault(role.GetType())?.FixedUpdate?.Invoke(role);
    internal static void DispatchOnKill(PlayerRole role, PlayerControl target) => RoleDelegatesCache.GetValueOrDefault(role.GetType())?.OnKill?.Invoke(role, target);
    internal static void DispatchOnDeath(PlayerRole role, PlayerControl killer) => RoleDelegatesCache.GetValueOrDefault(role.GetType())?.OnDeath?.Invoke(role, killer);
    internal static void DispatchOnFinishShipStatusBegin(PlayerRole role) => RoleDelegatesCache.GetValueOrDefault(role.GetType())?.OnFinishShipStatusBegin?.Invoke(role);
    internal static void DispatchHandleDisconnect(PlayerRole role, PlayerControl player, DisconnectReasons reason) => RoleDelegatesCache.GetValueOrDefault(role.GetType())?.HandleDisconnect?.Invoke(role, player, reason);

    // --- Dispatch Methods for PlayerModifier ---

    internal static void DispatchOnMeetingStart(PlayerModifier mod) => ModifierDelegatesCache.GetValueOrDefault(mod.GetType())?.OnMeetingStart?.Invoke(mod);
    internal static void DispatchOnMeetingEnd(PlayerModifier mod) => ModifierDelegatesCache.GetValueOrDefault(mod.GetType())?.OnMeetingEnd?.Invoke(mod);
    internal static void DispatchOnIntroEnd(PlayerModifier mod) => ModifierDelegatesCache.GetValueOrDefault(mod.GetType())?.OnIntroEnd?.Invoke(mod);
    internal static void DispatchFixedUpdate(PlayerModifier mod) => ModifierDelegatesCache.GetValueOrDefault(mod.GetType())?.FixedUpdate?.Invoke(mod);
    internal static void DispatchOnKill(PlayerModifier mod, PlayerControl target) => ModifierDelegatesCache.GetValueOrDefault(mod.GetType())?.OnKill?.Invoke(mod, target);
    internal static void DispatchOnDeath(PlayerModifier mod, PlayerControl killer) => ModifierDelegatesCache.GetValueOrDefault(mod.GetType())?.OnDeath?.Invoke(mod, killer);
    internal static void DispatchOnFinishShipStatusBegin(PlayerModifier mod) => ModifierDelegatesCache.GetValueOrDefault(mod.GetType())?.OnFinishShipStatusBegin?.Invoke(mod);
    internal static void DispatchHandleDisconnect(PlayerModifier mod, PlayerControl player, DisconnectReasons reason) => ModifierDelegatesCache.GetValueOrDefault(mod.GetType())?.HandleDisconnect?.Invoke(mod, player, reason);

    // --- Dynamic Button Generation ---

    internal static void TryCreateButtonsForRole(RoleType roleType)
    {
        var hudManager = FastDestroyableSingleton<HudManager>.Instance;
        if (hudManager == null) return;
        if (GeneratedRoleButtons.Contains(roleType)) return;

        GeneratedRoleButtons.Add(roleType);

        if (RoleButtonRegistrations.TryGetValue(roleType, out var registrations))
        {
            foreach (var act in registrations)
            {
                try
                {
                    act(hudManager);
                }
                catch (Exception e)
                {
                    Logger.LogError("[ModEventDispatcher] Error generating Role Button for {0}: {1}", roleType.ToString(), e.Message);
                }
            }
        }

        if (RoleButtonTimers.TryGetValue(roleType, out var timers))
        {
            foreach (var timerAct in timers)
            {
                try
                {
                    timerAct();
                }
                catch (Exception e)
                {
                    Logger.LogError("[ModEventDispatcher] Error setting Role Button Timer for {0}: {1}", roleType.ToString(), e.Message);
                }
            }
        }
    }

    internal static void TryCreateButtonsForModifier(ModifierType modType)
    {
        var hudManager = FastDestroyableSingleton<HudManager>.Instance;
        if (hudManager == null) return;
        if (GeneratedModifierButtons.Contains(modType)) return;

        GeneratedModifierButtons.Add(modType);

        if (ModifierButtonRegistrations.TryGetValue(modType, out var registrations))
        {
            foreach (var act in registrations)
            {
                try
                {
                    act(hudManager);
                }
                catch (Exception e)
                {
                    Logger.LogError("[ModEventDispatcher] Error generating Modifier Button for {0}: {1}", modType.ToString(), e.Message);
                }
            }
        }

        if (ModifierButtonTimers.TryGetValue(modType, out var timers))
        {
            foreach (var timerAct in timers)
            {
                try
                {
                    timerAct();
                }
                catch (Exception e)
                {
                    Logger.LogError("[ModEventDispatcher] Error setting Modifier Button Timer for {0}: {1}", modType.ToString(), e.Message);
                }
            }
        }
    }

    internal static void ClearGeneratedButtons()
    {
        GeneratedRoleButtons.Clear();
        GeneratedModifierButtons.Clear();
        CustomButton.DestroyAllButtons();
    }
}