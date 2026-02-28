namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class TranslationControllerPatch
{
    private static readonly StringBuilder ExileStringBuilder = new();

    [HarmonyPrefix]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(TranslationController),
        nameof(TranslationController.GetString),
        typeof(StringNames),
        typeof(Il2CppReferenceArray<CppObject>))]
    internal static bool GetStringPrefix(ref string __result, StringNames id)
    {
        if ((int)id < CustomOption.CUSTOM_OPTION_PRE_ID)
        {
            return true;
        }

        // For now only do this in custom options.
        var idInt = (int)id - CustomOption.CUSTOM_OPTION_PRE_ID;
        CustomOption opt = null;
        foreach (var o in CustomOption.AllOptions)
        {
            if (o.Id != idInt)
            {
                continue;
            }
            opt = o;
            break;
        }

        if (opt == null)
        {
            __result = "Unknown Option";
            return false;
        }

        var ourString = Helpers.Cs(opt.Color, Tr.Get(opt.NameKey)) ?? "";

        __result = ourString;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(TranslationController),
        nameof(TranslationController.GetString),
        typeof(StringNames),
        typeof(Il2CppReferenceArray<CppObject>))]
    internal static bool GetColorNamePrefix(ref string __result, [HarmonyArgument(0)] StringNames name)
    {
        return CustomColors.GetColorName(ref __result, name);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(TranslationController),
        nameof(TranslationController.GetString),
        typeof(StringNames),
        typeof(Il2CppReferenceArray<CppObject>))]
    internal static void GetStringPostfix(ref string __result, [HarmonyArgument(0)] StringNames id)
    {
        try
        {
            if (ExileController.Instance == null || ExileController.Instance.initData == null)
            {
                return;
            }
            var netPlayer = ExileController.Instance.initData.networkedPlayer;
            if (netPlayer == null)
            {
                return;
            }
            var player = netPlayer.Object;
            if (player == null)
            {
                return;
            }

            switch (id)
            {
                // Exile role text
                case StringNames.ExileTextPN or StringNames.ExileTextSN or StringNames.ExileTextPP or StringNames.ExileTextSP:
                    {
                        ExileStringBuilder.Clear();
                        ExileStringBuilder.Append(player.Data.PlayerName).Append(" was The ");
                        var roleInfos = RoleInfo.GetRoleInfoForPlayer(player, false);
                        for (var i = 0; i < roleInfos.Count; i++)
                        {
                            if (i > 0)
                            {
                                ExileStringBuilder.Append(' ');
                            }
                            ExileStringBuilder.Append(roleInfos[i].Name);
                        }

                        __result = ExileStringBuilder.ToString();
                        break;
                    }
                case StringNames.ImpostorsRemainP or StringNames.ImpostorsRemainS:
                    {
                        if (player.IsRole(RoleType.Jester))
                        {
                            __result = string.Empty;
                        }
                        break;
                    }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "ExileMessage");
        }
    }
}