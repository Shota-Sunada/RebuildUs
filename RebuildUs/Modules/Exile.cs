using RebuildUs.Roles.Neutral;

namespace RebuildUs.Modules;

public static class Exile
{
    public static void WrapUpPostfix(PlayerControl exiled)
    {
        CustomButton.MeetingEndedUpdate();

        if (exiled != null && exiled.IsRole(ERoleType.Jester))
        {
            Jester.TriggerJesterWin = true;
        }
    }

    public static void ExileMessage(ref string __result, StringNames id)
    {
        try
        {
            if (ExileController.Instance != null && ExileController.Instance.initData != null)
            {
                PlayerControl player = ExileController.Instance.initData.networkedPlayer.Object;
                if (player == null) return;
                // Exile role text
                if (id == StringNames.ExileTextPN || id == StringNames.ExileTextSN || id == StringNames.ExileTextPP || id == StringNames.ExileTextSP)
                {
                    __result = player.Data.PlayerName + " was The " + string.Join(" ", [.. RoleInfo.GetRoleInfoForPlayer(player, false).Select(x => x.Name)]);
                }
                if (id == StringNames.ImpostorsRemainP || id == StringNames.ImpostorsRemainS)
                {
                    if (player.IsRole(ERoleType.Jester)) __result = "";
                }
            }
        }
        catch
        {
            // pass - Hopefully prevent leaving while exiling to soft lock game
        }
    }
}