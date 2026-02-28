using Object = UnityEngine.Object;

namespace RebuildUs.Modules;

internal static class DeathPopup
{
    private const int RESOLVE_RETRY_INTERVAL_FRAMES = 120;

    internal const int RESULT_SUCCESS = 0;
    private const int RESULT_INVALID_DEAD_PLAYER = 1 << 0;
    private const int RESULT_INVALID_DEATH_INDEX = 1 << 1;
    private const int RESULT_MISSING_PREFAB = 1 << 2;
    private const int RESULT_MISSING_PARENT = 1 << 3;
    private const int RESULT_FALLBACK_UNAVAILABLE = 1 << 4;
    private const int RESULT_INSTANTIATION_FAILED = 1 << 5;

    private static readonly FieldInfo AnyDeathPopupPrefabField = typeof(HideAndSeekManager)
                                                                 .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                                                 .FirstOrDefault(f => typeof(HideAndSeekDeathPopup).IsAssignableFrom(f.FieldType));

    private static readonly FieldInfo LogicPopupPrefabField = typeof(LogicHnSDeathPopup)
                                                              .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                                              .FirstOrDefault(f => typeof(HideAndSeekDeathPopup).IsAssignableFrom(f.FieldType));
    private static int _nextDeathIndex;
    private static int _lastPrefabResolveFrame = -RESOLVE_RETRY_INTERVAL_FRAMES;
    private static HideAndSeekDeathPopup _cachedPrefab;
    private static Transform _cachedParent;

    internal static void Reset()
    {
        _nextDeathIndex = 0;
        _cachedPrefab = null;
        _cachedParent = null;
        _lastPrefabResolveFrame = -RESOLVE_RETRY_INTERVAL_FRAMES;
    }

    internal static int TryShow(PlayerControl deadPlayer)
    {
        return TryShow(deadPlayer, out _);
    }

    internal static int TryShow(PlayerControl deadPlayer, out HideAndSeekDeathPopup popupInstance)
    {
        popupInstance = null;
        if (deadPlayer?.Data == null)
        {
            return RESULT_INVALID_DEAD_PLAYER;
        }

        int deathIndex = _nextDeathIndex++;
        return TryShow(deadPlayer, deathIndex, out popupInstance);
    }

    internal static int TryShow(PlayerControl deadPlayer, int deathIndex)
    {
        return TryShow(deadPlayer, deathIndex, out _);
    }

    internal static int TryShow(PlayerControl deadPlayer, int deathIndex, out HideAndSeekDeathPopup popupInstance)
    {
        popupInstance = null;

        if (deadPlayer?.Data == null)
        {
            return RESULT_INVALID_DEAD_PLAYER;
        }
        if (deathIndex < 0)
        {
            return RESULT_INVALID_DEATH_INDEX;
        }

        HideAndSeekDeathPopup prefab = GetOrResolvePrefab();
        Transform parent = GetOrResolveParent();

        int resolveError = RESULT_SUCCESS;
        if (prefab == null)
        {
            resolveError |= RESULT_MISSING_PREFAB;
        }
        if (parent == null)
        {
            resolveError |= RESULT_MISSING_PARENT;
        }

        if (prefab != null && parent != null)
        {
            popupInstance = Object.Instantiate(prefab, parent);
            if (popupInstance == null)
            {
                return RESULT_INSTANTIATION_FAILED;
            }

            popupInstance.Show(deadPlayer, deathIndex);
            return RESULT_SUCCESS;
        }

        // Last resort: rely on vanilla logic if direct prefab/parent resolution failed.
        if (deadPlayer != null && GameManager.Instance is HideAndSeekManager hnsManager && hnsManager.LogicDeathPopup != null)
        {
            hnsManager.LogicDeathPopup.OnPlayerDeath(deadPlayer);
            return RESULT_SUCCESS;
        }

        return resolveError | RESULT_FALLBACK_UNAVAILABLE;
    }

    internal static HideAndSeekDeathPopup ResolvePrefab(HideAndSeekManager hideAndSeekManager = null,
                                                        HideAndSeekManager hideAndSeekManagerPrefab = null)
    {
        if (hideAndSeekManager != null)
        {
            HideAndSeekDeathPopup popup = ResolvePrefabFromManager(hideAndSeekManager);
            if (popup != null)
            {
                return popup;
            }
        }

        if (hideAndSeekManagerPrefab != null)
        {
            HideAndSeekDeathPopup popup = ResolvePrefabFromManager(hideAndSeekManagerPrefab);
            if (popup != null)
            {
                return popup;
            }
        }

        if (GameManager.Instance is HideAndSeekManager hnsManager)
        {
            HideAndSeekDeathPopup popup = ResolvePrefabFromManager(hnsManager);
            if (popup != null)
            {
                return popup;
            }
        }

        HideAndSeekManager creatorPrefab = GameManagerCreator.Instance?.HideAndSeekManagerPrefab;
        if (creatorPrefab != null)
        {
            HideAndSeekDeathPopup popup = ResolvePrefabFromManager(creatorPrefab);
            if (popup != null)
            {
                return popup;
            }
        }

        Object[] popups = Resources.FindObjectsOfTypeAll(Il2CppType.Of<HideAndSeekDeathPopup>());
        foreach (Object obj in popups)
        {
            HideAndSeekDeathPopup popup = obj.TryCast<HideAndSeekDeathPopup>();
            if (popup != null)
            {
                return popup;
            }
        }

        return null;
    }

    internal static Transform ResolveParent(HudManager hudManager = null)
    {
        HudManager hud = hudManager ?? FastDestroyableSingleton<HudManager>.Instance;
        if (hud == null)
        {
            return null;
        }
        return hud.transform.parent ?? hud.transform;
    }

    private static HideAndSeekDeathPopup GetOrResolvePrefab()
    {
        if (_cachedPrefab != null)
        {
            return _cachedPrefab;
        }

        int frame = Time.frameCount;
        if (frame - _lastPrefabResolveFrame < RESOLVE_RETRY_INTERVAL_FRAMES)
        {
            return null;
        }
        _lastPrefabResolveFrame = frame;

        _cachedPrefab = ResolvePrefab();
        return _cachedPrefab;
    }

    private static Transform GetOrResolveParent()
    {
        if (_cachedParent != null)
        {
            return _cachedParent;
        }
        _cachedParent = ResolveParent();
        return _cachedParent;
    }

    private static HideAndSeekDeathPopup ResolvePrefabFromManager(HideAndSeekManager manager)
    {
        if (manager == null)
        {
            return null;
        }

        if (manager.LogicDeathPopup != null && LogicPopupPrefabField != null)
        {
            HideAndSeekDeathPopup fromLogic = LogicPopupPrefabField.GetValue(manager.LogicDeathPopup) as HideAndSeekDeathPopup;
            if (fromLogic != null)
            {
                return fromLogic;
            }
        }

        if (AnyDeathPopupPrefabField != null)
        {
            HideAndSeekDeathPopup byType = AnyDeathPopupPrefabField.GetValue(manager) as HideAndSeekDeathPopup;
            if (byType != null)
            {
                return byType;
            }
        }

        return null;
    }

    internal static string ExplainResult(int result)
    {
        if (result == RESULT_SUCCESS)
        {
            return "success";
        }

        StringBuilder sb = new();
        if ((result & RESULT_INVALID_DEAD_PLAYER) != 0)
        {
            sb.Append("invalid-dead-player,");
        }
        if ((result & RESULT_INVALID_DEATH_INDEX) != 0)
        {
            sb.Append("invalid-death-index,");
        }
        if ((result & RESULT_MISSING_PREFAB) != 0)
        {
            sb.Append("missing-prefab,");
        }
        if ((result & RESULT_MISSING_PARENT) != 0)
        {
            sb.Append("missing-parent,");
        }
        if ((result & RESULT_FALLBACK_UNAVAILABLE) != 0)
        {
            sb.Append("fallback-unavailable,");
        }
        if ((result & RESULT_INSTANTIATION_FAILED) != 0)
        {
            sb.Append("instantiation-failed,");
        }
        if (sb.Length > 0)
        {
            sb.Length--;
        }
        return sb.ToString();
    }
}