namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class TranslationControllerPatch
{
    private static readonly StringBuilder ExileStringBuilder = new();

    [HarmonyPrefix]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(StringNames), typeof(Il2CppReferenceArray<CppObject>))]
    internal static bool GetStringPrefix(ref string __result, StringNames id)
    {
        var idInt = (int)id;
        if (idInt >= CustomOption.CUSTOM_OPTION_PRE_ID)
        {
            var optId = (COID)(idInt - CustomOption.CUSTOM_OPTION_PRE_ID);
            foreach (var o in CustomOption.AllOptions)
            {
                if (o.Id == optId)
                {
                    __result = Helpers.Cs(o.Color, Tr.Get(o.NameKey)) ?? "";
                    return false;
                }
            }

            __result = "Unknown Option";
            return false;
        }

        if (idInt >= CustomColors.COLOR_BASE_ID_NUMBER)
        {
            if (CustomColors.ColorStrings.TryGetValue(idInt, out var key))
            {
                __result = Tr.Get(key) ?? "Unknown Color";
                return false;
            }
        }

        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(StringNames), typeof(Il2CppReferenceArray<CppObject>))]
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
            Logger.LogError("[ExileMessage] Error occurred: {0}", ex.Message);
        }
    }
}