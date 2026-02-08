using Object = UnityEngine.Object;

namespace RebuildUs.Modules.Consoles;

public static class SecurityCamera
{
    public static float CameraTimer;
    private static int _page;
    private static float _timer;

    private static readonly StringBuilder SECURITY_STRING_BUILDER = new();

    private static TextMeshPro _timeRemaining;

    public static void ResetData()
    {
        CameraTimer = 0f;
        if (_timeRemaining != null)
        {
            Object.Destroy(_timeRemaining.gameObject);
            _timeRemaining = null;
        }

        _page = 0;
        _timer = 0f;
    }

    public static void UseCameraTime()
    {
        // Don't waste network traffic if we're out of time.
        var lp = PlayerControl.LocalPlayer;
        if (MapSettings.RestrictDevices > 0 && MapSettings.RestrictCameras && MapSettings.RestrictCamerasTime > 0f && lp != null && lp.IsAlive())
        {
            using var sender = new RPCSender(lp.NetId, CustomRPC.UseCameraTime);
            sender.Write(CameraTimer);
            RPCProcedure.UseCameraTime(CameraTimer);
        }

        CameraTimer = 0f;
    }

    public static void BeginCommon()
    {
        CameraTimer = 0f;
    }

    public static void BeginPostfix(SurveillanceMinigame __instance)
    {
        // Add securityGuard cameras
        _page = 0;
        _timer = 0;
        var ship = MapUtilities.CachedShipStatus;
        if (ship != null && ship.AllCameras.Length > 4 && __instance.FilteredRooms.Length > 0)
        {
            var oldLen = __instance.textures.Length;
            var newLen = ship.AllCameras.Length;
            var newTextures = new RenderTexture[newLen];
            for (var i = 0; i < oldLen; i++) newTextures[i] = __instance.textures[i];
            __instance.textures = newTextures;

            for (var i = 4; i < ship.AllCameras.Length; i++)
            {
                var surv = ship.AllCameras[i];
                var camera = Object.Instantiate(__instance.CameraPrefab);
                camera.transform.SetParent(__instance.transform);
                camera.transform.position = new(surv.transform.position.x, surv.transform.position.y, 8f);
                camera.orthographicSize = 2.35f;
                var temporary = RenderTexture.GetTemporary(256, 256, 16, (RenderTextureFormat)0);
                __instance.textures[i] = temporary;
                camera.targetTexture = temporary;
            }
        }
    }

    public static bool Update(PlanetSurveillanceMinigame __instance)
    {
        CameraTimer += Time.deltaTime;
        if (CameraTimer > 0.1f)
            UseCameraTime();

        if (MapSettings.RestrictDevices > 0 && MapSettings.RestrictCameras)
        {
            if (_timeRemaining == null)
            {
                _timeRemaining = Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.TaskPanel.taskText, __instance.transform);
                _timeRemaining.alignment = TextAlignmentOptions.BottomRight;
                _timeRemaining.transform.position = Vector3.zero;
                _timeRemaining.transform.localPosition = new(0.95f, 4.45f);
                _timeRemaining.transform.localScale *= 1.8f;
                _timeRemaining.color = Palette.White;
            }

            if (MapSettings.RestrictCamerasTime <= 0f)
            {
                __instance.Close();
                return false;
            }

            SECURITY_STRING_BUILDER.Clear();
            var ts = TimeSpan.FromSeconds(MapSettings.RestrictCamerasTime);
            if (ts.TotalHours >= 1) SECURITY_STRING_BUILDER.Append((int)ts.TotalHours).Append(':');
            SECURITY_STRING_BUILDER.Append(ts.Minutes.ToString("D2")).Append(':').Append(ts.Seconds.ToString("D2")).Append('.').Append((ts.Milliseconds / 10).ToString("D2"));

            var timeString = SECURITY_STRING_BUILDER.ToString();
            SECURITY_STRING_BUILDER.Clear();
            SECURITY_STRING_BUILDER.Append(string.Format(Tr.Get(TrKey.TimeRemaining), timeString));
            _timeRemaining.text = SECURITY_STRING_BUILDER.ToString();
            _timeRemaining.gameObject.SetActive(true);
        }

        return true;
    }

    public static bool Update(SurveillanceMinigame __instance)
    {
        CameraTimer += Time.deltaTime;
        if (CameraTimer > 0.1f) UseCameraTime();

        if (MapSettings.RestrictDevices > 0 && MapSettings.RestrictCameras)
        {
            if (_timeRemaining == null)
            {
                _timeRemaining = Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.TaskPanel.taskText, __instance.transform);
                _timeRemaining.alignment = TextAlignmentOptions.Center;
                _timeRemaining.transform.position = Vector3.zero;
                _timeRemaining.transform.localPosition = new(0.0f, -1.7f);
                _timeRemaining.transform.localScale *= 1.8f;
                _timeRemaining.color = Palette.White;
            }

            if (MapSettings.RestrictCamerasTime <= 0f)
            {
                __instance.Close();
                return false;
            }

            SECURITY_STRING_BUILDER.Clear();
            var ts = TimeSpan.FromSeconds(MapSettings.RestrictCamerasTime);
            if (ts.TotalHours >= 1) SECURITY_STRING_BUILDER.Append((int)ts.TotalHours).Append(':');
            SECURITY_STRING_BUILDER.Append(ts.Minutes.ToString("D2")).Append(':').Append(ts.Seconds.ToString("D2")).Append('.').Append((ts.Milliseconds / 10).ToString("D2"));

            var timeString = SECURITY_STRING_BUILDER.ToString();
            SECURITY_STRING_BUILDER.Clear();
            SECURITY_STRING_BUILDER.Append(string.Format(Tr.Get(TrKey.TimeRemaining), timeString));
            _timeRemaining.text = SECURITY_STRING_BUILDER.ToString();
            _timeRemaining.gameObject.SetActive(true);
        }

        // Update normal and securityGuard cameras
        _timer += Time.deltaTime;
        var numberOfPages = Mathf.CeilToInt(MapUtilities.CachedShipStatus.AllCameras.Length / 4f);

        var update = false;

        if (_timer > 3f || Input.GetKeyDown(KeyCode.RightArrow))
        {
            update = true;
            _timer = 0f;
            _page = (_page + 1) % numberOfPages;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _page = ((_page + numberOfPages) - 1) % numberOfPages;
            update = true;
            _timer = 0f;
        }

        if ((__instance.isStatic || update) && !PlayerTask.PlayerHasTaskOfType<IHudOverrideTask>(PlayerControl.LocalPlayer))
        {
            __instance.isStatic = false;
            for (var i = 0; i < __instance.ViewPorts.Length; i++)
            {
                __instance.ViewPorts[i].sharedMaterial = __instance.DefaultMaterial;
                __instance.SabText[i].gameObject.SetActive(false);
                if ((_page * 4) + i < __instance.textures.Length)
                    __instance.ViewPorts[i].material.SetTexture("_MainTex", __instance.textures[(_page * 4) + i]);
                else
                    __instance.ViewPorts[i].sharedMaterial = __instance.StaticMaterial;
            }
        }
        else if (!__instance.isStatic && PlayerTask.PlayerHasTaskOfType<HudOverrideTask>(PlayerControl.LocalPlayer))
        {
            __instance.isStatic = true;
            for (var j = 0; j < __instance.ViewPorts.Length; j++)
            {
                __instance.ViewPorts[j].sharedMaterial = __instance.StaticMaterial;
                __instance.SabText[j].gameObject.SetActive(true);
            }
        }

        return false;
    }

    public static bool Update(SecurityLogGame __instance)
    {
        CameraTimer += Time.deltaTime;
        if (CameraTimer > 0.05f)
            UseCameraTime();

        if (MapSettings.RestrictDevices > 0)
        {
            if (_timeRemaining == null)
            {
                _timeRemaining = Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.TaskPanel.taskText, __instance.transform);
                _timeRemaining.alignment = TextAlignmentOptions.BottomRight;
                _timeRemaining.transform.position = Vector3.zero;
                _timeRemaining.transform.localPosition = new(1.0f, 4.25f);
                _timeRemaining.transform.localScale *= 1.6f;
                _timeRemaining.color = Palette.White;
            }

            if (MapSettings.RestrictCamerasTime <= 0f)
            {
                __instance.Close();
                return false;
            }

            var timeString = TimeSpan.FromSeconds(MapSettings.RestrictCamerasTime).ToString(@"mm\:ss\.ff");
            _timeRemaining.text = string.Format(Tr.Get(TrKey.TimeRemaining), timeString);
            _timeRemaining.gameObject.SetActive(true);
        }

        return true;
    }
}
