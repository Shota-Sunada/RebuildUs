namespace RebuildUs.Modules.Consoles;

internal static class SecurityCamera
{
    private static float _cameraTimer;
    private static int _page;
    private static float _timer;

    private static readonly StringBuilder SecurityStringBuilder = new();

    private static TextMeshPro _timeRemaining;
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");

    internal static void ResetData()
    {
        _cameraTimer = 0f;
        if (_timeRemaining != null)
        {
            UnityObject.Destroy(_timeRemaining.gameObject);
            _timeRemaining = null;
        }

        _page = 0;
        _timer = 0f;
    }

    internal static void UseCameraTime()
    {
        // Don't waste network traffic if we're out of time.
        var lp = PlayerControl.LocalPlayer;
        if (MapSettings.RestrictDevices > 0 && MapSettings.RestrictCameras && MapSettings.RestrictCamerasTime > 0f && lp != null && lp.IsAlive())
        {
            using RPCSender sender = new(lp.NetId, CustomRPC.UseCameraTime);
            sender.Write(_cameraTimer);
            RPCProcedure.UseCameraTime(_cameraTimer);
        }

        _cameraTimer = 0f;
    }

    internal static void BeginCommon()
    {
        _cameraTimer = 0f;
    }

    internal static void BeginPostfix(SurveillanceMinigame __instance)
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
            for (var i = 0; i < oldLen; i++)
            {
                newTextures[i] = __instance.textures[i];
            }
            __instance.textures = newTextures;

            for (var i = 4; i < ship.AllCameras.Length; i++)
            {
                var surv = ship.AllCameras[i];
                var camera = UnityObject.Instantiate(__instance.CameraPrefab, __instance.transform, true);
                camera.transform.position = new(surv.transform.position.x, surv.transform.position.y, 8f);
                camera.orthographicSize = 2.35f;
                var temporary = RenderTexture.GetTemporary(256, 256, 16, (RenderTextureFormat)0);
                __instance.textures[i] = temporary;
                camera.targetTexture = temporary;
            }
        }
    }

    internal static bool Update(PlanetSurveillanceMinigame __instance)
    {
        _cameraTimer += Time.deltaTime;
        if (_cameraTimer > 0.1f)
        {
            UseCameraTime();
        }

        if (MapSettings.RestrictDevices <= 0 || !MapSettings.RestrictCameras)
        {
            return true;
        }
        if (_timeRemaining == null)
        {
            _timeRemaining = UnityObject.Instantiate(FastDestroyableSingleton<HudManager>.Instance.TaskPanel.taskText, __instance.transform);
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

        SecurityStringBuilder.Clear();
        var ts = TimeSpan.FromSeconds(MapSettings.RestrictCamerasTime);
        if (ts.TotalHours >= 1)
        {
            SecurityStringBuilder.Append((int)ts.TotalHours).Append(':');
        }
        SecurityStringBuilder
            .Append(ts.Minutes.ToString("D2"))
            .Append(':')
            .Append(ts.Seconds.ToString("D2"))
            .Append('.')
            .Append((ts.Milliseconds / 10).ToString("D2"));

        var timeString = SecurityStringBuilder.ToString();
        SecurityStringBuilder.Clear();
        SecurityStringBuilder.Append(string.Format(Tr.Get(TrKey.TimeRemaining), timeString));
        _timeRemaining.text = SecurityStringBuilder.ToString();
        _timeRemaining.gameObject.SetActive(true);

        return true;
    }

    internal static bool Update(SurveillanceMinigame __instance)
    {
        _cameraTimer += Time.deltaTime;
        if (_cameraTimer > 0.1f)
        {
            UseCameraTime();
        }

        if (MapSettings.RestrictDevices > 0 && MapSettings.RestrictCameras)
        {
            if (_timeRemaining == null)
            {
                _timeRemaining = UnityObject.Instantiate(FastDestroyableSingleton<HudManager>.Instance.TaskPanel.taskText, __instance.transform);
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

            SecurityStringBuilder.Clear();
            var ts = TimeSpan.FromSeconds(MapSettings.RestrictCamerasTime);
            if (ts.TotalHours >= 1)
            {
                SecurityStringBuilder.Append((int)ts.TotalHours).Append(':');
            }
            SecurityStringBuilder
                .Append(ts.Minutes.ToString("D2"))
                .Append(':')
                .Append(ts.Seconds.ToString("D2"))
                .Append('.')
                .Append((ts.Milliseconds / 10).ToString("D2"));

            var timeString = SecurityStringBuilder.ToString();
            SecurityStringBuilder.Clear();
            SecurityStringBuilder.Append(string.Format(Tr.Get(TrKey.TimeRemaining), timeString));
            _timeRemaining.text = SecurityStringBuilder.ToString();
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
            _page = (_page + numberOfPages - 1) % numberOfPages;
            update = true;
            _timer = 0f;
        }

        if (__instance.isStatic || update)
        {
            if (PlayerTask.PlayerHasTaskOfType<IHudOverrideTask>(PlayerControl.LocalPlayer))
            {
                return false;
            }
            __instance.isStatic = false;
            for (var i = 0; i < __instance.ViewPorts.Length; i++)
            {
                __instance.ViewPorts[i].sharedMaterial = __instance.DefaultMaterial;
                __instance.SabText[i].gameObject.SetActive(false);
                if (_page * 4 + i < __instance.textures.Length)
                {
                    __instance.ViewPorts[i].material.SetTexture(MainTex, __instance.textures[_page * 4 + i]);
                }
                else
                {
                    __instance.ViewPorts[i].sharedMaterial = __instance.StaticMaterial;
                }
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

    internal static bool Update(SecurityLogGame __instance)
    {
        _cameraTimer += Time.deltaTime;
        if (_cameraTimer > 0.05f)
        {
            UseCameraTime();
        }

        if (MapSettings.RestrictDevices <= 0)
        {
            return true;
        }
        if (_timeRemaining == null)
        {
            _timeRemaining = UnityObject.Instantiate(FastDestroyableSingleton<HudManager>.Instance.TaskPanel.taskText, __instance.transform);
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

        return true;
    }
}