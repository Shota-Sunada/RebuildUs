using Object = Il2CppSystem.Object;

namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class TranslationControllerPatch
{
    [HarmonyPrefix]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(StringNames), typeof(Il2CppReferenceArray<Object>))]
    internal static bool GetStringPrefix(ref string __result, StringNames id)
    {
        if ((int)id < CustomOption.CUSTOM_OPTION_PRE_ID) return true;

        // For now only do this in custom options.
        int idInt = (int)id - CustomOption.CUSTOM_OPTION_PRE_ID;
        CustomOption opt = null;
        foreach (CustomOption o in CustomOption.AllOptions)
        {
            if (o.Id != idInt) continue;
            opt = o;
            break;
        }

        if (opt == null)
        {
            __result = "Unknown Option";
            return false;
        }

        string ourString = Helpers.Cs(opt.Color, Tr.Get(opt.NameKey)) ?? "";

        __result = ourString;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(StringNames), typeof(Il2CppReferenceArray<Object>))]
    internal static bool GetColorNamePrefix(ref string __result, [HarmonyArgument(0)] StringNames name)
    {
        return CustomColors.GetColorName(ref __result, name);
    }

    private static readonly StringBuilder ExileStringBuilder = new();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(StringNames), typeof(Il2CppReferenceArray<Object>))]
    internal static void GetStringPostfix(ref string __result, [HarmonyArgument(0)] StringNames id)
    {
        try
        {
            if (ExileController.Instance == null || ExileController.Instance.initData == null) return;
            NetworkedPlayerInfo netPlayer = ExileController.Instance.initData.networkedPlayer;
            if (netPlayer == null) return;
            PlayerControl player = netPlayer.Object;
            if (player == null) return;

            switch (id)
            {
                // Exile role text
                case StringNames.ExileTextPN or StringNames.ExileTextSN or StringNames.ExileTextPP or StringNames.ExileTextSP:
                {
                    ExileStringBuilder.Clear();
                    ExileStringBuilder.Append(player.Data.PlayerName).Append(" was The ");
                    List<RoleInfo> roleInfos = RoleInfo.GetRoleInfoForPlayer(player, false);
                    for (int i = 0; i < roleInfos.Count; i++)
                    {
                        if (i > 0) ExileStringBuilder.Append(' ');
                        ExileStringBuilder.Append(roleInfos[i].Name);
                    }

                    __result = ExileStringBuilder.ToString();
                    break;
                }
                case StringNames.ImpostorsRemainP or StringNames.ImpostorsRemainS:
                {
                    if (player.IsRole(RoleType.Jester)) __result = string.Empty;
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