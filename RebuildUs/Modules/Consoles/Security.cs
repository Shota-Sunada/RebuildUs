namespace RebuildUs.Modules.Consoles;

public static class SecurityCamera
{
    public static float CameraTimer = 0f;
    private static int Page = 0;
    private static float Timer = 0f;

    public static void ResetData()
    {
        CameraTimer = 0f;
        if (TimeRemaining != null)
        {
            UnityEngine.Object.Destroy(TimeRemaining);
            TimeRemaining = null;
        }
        Page = 0;
        Timer = 0f;
    }

    static TMPro.TextMeshPro TimeRemaining;

    public static void UseCameraTime()
    {
        // Don't waste network traffic if we're out of time.
        if (ModMapOptions.RestrictDevices > 0 && ModMapOptions.RestrictCameras && ModMapOptions.RestrictCamerasTime > 0f && CachedPlayer.LocalPlayer.PlayerControl.IsAlive())
        {
            using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.UseCameraTime);
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
        Page = 0;
        Timer = 0;
        if (MapUtilities.CachedShipStatus.AllCameras.Length > 4 && __instance.FilteredRooms.Length > 0)
        {
            __instance.textures = __instance.textures.ToList().Concat(new RenderTexture[MapUtilities.CachedShipStatus.AllCameras.Length - 4]).ToArray();
            for (int i = 4; i < MapUtilities.CachedShipStatus.AllCameras.Length; i++)
            {
                SurvCamera surv = MapUtilities.CachedShipStatus.AllCameras[i];
                Camera camera = UnityEngine.Object.Instantiate<Camera>(__instance.CameraPrefab);
                camera.transform.SetParent(__instance.transform);
                camera.transform.position = new Vector3(surv.transform.position.x, surv.transform.position.y, 8f);
                camera.orthographicSize = 2.35f;
                RenderTexture temporary = RenderTexture.GetTemporary(256, 256, 16, (RenderTextureFormat)0);
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

        if (ModMapOptions.RestrictDevices > 0 && ModMapOptions.RestrictCameras)
        {
            if (TimeRemaining == null)
            {
                TimeRemaining = UnityEngine.Object.Instantiate(FastDestroyableSingleton<TaskPanelBehaviour>.Instance.taskText, __instance.transform);
                TimeRemaining.alignment = TMPro.TextAlignmentOptions.BottomRight;
                TimeRemaining.transform.position = Vector3.zero;
                TimeRemaining.transform.localPosition = new Vector3(0.95f, 4.45f);
                TimeRemaining.transform.localScale *= 1.8f;
                TimeRemaining.color = Palette.White;
            }

            if (ModMapOptions.RestrictCamerasTime <= 0f)
            {
                __instance.Close();
                return false;
            }

            string timeString = TimeSpan.FromSeconds(ModMapOptions.RestrictCamerasTime).ToString(@"mm\:ss\.ff");
            TimeRemaining.text = String.Format(Tr.Get("Hud.TimeRemaining"), timeString);
            TimeRemaining.gameObject.SetActive(true);
        }

        return true;
    }

    public static bool Update(SurveillanceMinigame __instance)
    {
        CameraTimer += Time.deltaTime;
        if (CameraTimer > 0.1f)
        {
            UseCameraTime();
        }

        if (ModMapOptions.RestrictDevices > 0 && ModMapOptions.RestrictCameras)
        {
            if (TimeRemaining == null)
            {
                TimeRemaining = UnityEngine.Object.Instantiate(FastDestroyableSingleton<TaskPanelBehaviour>.Instance.taskText, __instance.transform);
                TimeRemaining.alignment = TMPro.TextAlignmentOptions.Center;
                TimeRemaining.transform.position = Vector3.zero;
                TimeRemaining.transform.localPosition = new Vector3(0.0f, -1.7f);
                TimeRemaining.transform.localScale *= 1.8f;
                TimeRemaining.color = Palette.White;
            }

            if (ModMapOptions.RestrictCamerasTime <= 0f)
            {
                __instance.Close();
                return false;
            }

            string timeString = TimeSpan.FromSeconds(ModMapOptions.RestrictCamerasTime).ToString(@"mm\:ss\.ff");
            TimeRemaining.text = String.Format(Tr.Get("Hud.TimeRemaining"), timeString);
            TimeRemaining.gameObject.SetActive(true);

        }

        // Update normal and securityGuard cameras
        Timer += Time.deltaTime;
        int numberOfPages = Mathf.CeilToInt(MapUtilities.CachedShipStatus.AllCameras.Length / 4f);

        bool update = false;

        if (Timer > 3f || Input.GetKeyDown(KeyCode.RightArrow))
        {
            update = true;
            Timer = 0f;
            Page = (Page + 1) % numberOfPages;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Page = (Page + numberOfPages - 1) % numberOfPages;
            update = true;
            Timer = 0f;
        }

        if ((__instance.isStatic || update) && !PlayerTask.PlayerHasTaskOfType<IHudOverrideTask>(CachedPlayer.LocalPlayer.PlayerControl))
        {
            __instance.isStatic = false;
            for (int i = 0; i < __instance.ViewPorts.Length; i++)
            {
                __instance.ViewPorts[i].sharedMaterial = __instance.DefaultMaterial;
                __instance.SabText[i].gameObject.SetActive(false);
                if (Page * 4 + i < __instance.textures.Length)
                    __instance.ViewPorts[i].material.SetTexture("_MainTex", __instance.textures[Page * 4 + i]);
                else
                    __instance.ViewPorts[i].sharedMaterial = __instance.StaticMaterial;
            }
        }
        else if (!__instance.isStatic && PlayerTask.PlayerHasTaskOfType<HudOverrideTask>(CachedPlayer.LocalPlayer.PlayerControl))
        {
            __instance.isStatic = true;
            for (int j = 0; j < __instance.ViewPorts.Length; j++)
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

        if (ModMapOptions.RestrictDevices > 0)
        {
            if (TimeRemaining == null)
            {
                TimeRemaining = UnityEngine.Object.Instantiate(FastDestroyableSingleton<TaskPanelBehaviour>.Instance.taskText, __instance.transform);
                TimeRemaining.alignment = TMPro.TextAlignmentOptions.BottomRight;
                TimeRemaining.transform.position = Vector3.zero;
                TimeRemaining.transform.localPosition = new Vector3(1.0f, 4.25f);
                TimeRemaining.transform.localScale *= 1.6f;
                TimeRemaining.color = Palette.White;
            }

            if (ModMapOptions.RestrictCamerasTime <= 0f)
            {
                __instance.Close();
                return false;
            }

            string timeString = TimeSpan.FromSeconds(ModMapOptions.RestrictCamerasTime).ToString(@"mm\:ss\.ff");
            TimeRemaining.text = String.Format(Tr.Get("Hud.TimeRemaining"), timeString);
            TimeRemaining.gameObject.SetActive(true);
        }

        return true;
    }
}
