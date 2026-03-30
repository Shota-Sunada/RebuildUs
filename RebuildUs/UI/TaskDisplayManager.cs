#nullable enable
namespace RebuildUs.UI;

using System.Text;

internal static class TaskDisplayManager
{
    private static readonly StringBuilder InfoStringBuilder = new();

    internal static string GetTaskInfoText(byte playerId)
    {
        var player = Helpers.PlayerById(playerId);
        return GetTaskInfoText(player);
    }

    internal static string GetTaskInfoText(PlayerControl player)
    {
        if (player?.Data == null)
        {
            return string.Empty;
        }

        var data = player.Data;
        var (completed, total) = TasksHandler.TaskInfo(data);
        var statusText = GetSpecialRoleStatus(player);

        if (total <= 0)
        {
            return statusText;
        }

        InfoStringBuilder.Clear();
        if (!string.IsNullOrEmpty(statusText))
        {
            InfoStringBuilder.Append(statusText).Append(' ');
        }

        var commsActive = false;
        if (MapUtilities.CachedShipStatus != null && MapUtilities.Systems.TryGetValue(SystemTypes.Comms, out var comms))
        {
            var activatable = comms.CastFast<IActivatable>();
            if (activatable != null)
            {
                commsActive = activatable.IsActive;
            }
        }

        if (commsActive)
        {
            InfoStringBuilder.Append("<color=#808080FF>(?/?)</color>");
        }
        else
        {
            var color = completed == total ? "#00FF00FF" : "#FAD934FF";
            InfoStringBuilder
                .Append("<color=")
                .Append(color)
                .Append(">(")
                .Append(completed)
                .Append('/')
                .Append(total)
                .Append(")</color>");
        }

        return InfoStringBuilder.ToString().Trim();
    }

    private static string GetSpecialRoleStatus(PlayerControl player)
    {
        if (player.IsRole(RoleType.Arsonist))
        {
            var role = Arsonist.Instance;
            if (role != null)
            {
                var dousedSurvivors = 0;
                foreach (var dousedPlayer in role.DousedPlayers)
                {
                    if (dousedPlayer?.Data != null && !dousedPlayer.Data.IsDead && !dousedPlayer.Data.Disconnected)
                    {
                        dousedSurvivors++;
                    }
                }

                var totalSurvivors = 0;
                foreach (var targetPlayer in PlayerControl.AllPlayerControls.GetFastEnumerator())
                {
                    if (targetPlayer?.Data != null
                        && !targetPlayer.Data.IsDead
                        && !targetPlayer.Data.Disconnected
                        && !targetPlayer.IsRole(RoleType.Arsonist)
                        && !targetPlayer.IsGm())
                    {
                        totalSurvivors++;
                    }
                }

                return Helpers.Cs(Arsonist.Color, string.Format(" ({0}/{1})", dousedSurvivors, totalSurvivors));
            }
        }
        else if (player.IsRole(RoleType.Vulture))
        {
            var role = Vulture.Instance;
            if (role != null)
            {
                return Helpers.Cs(Vulture.Color, string.Format(" ({0}/{1})", role.EatenBodies, Vulture.NumberToWin));
            }
        }

        return string.Empty;
    }
}
