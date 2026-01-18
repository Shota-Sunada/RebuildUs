using RebuildUs.Roles.Crewmate;
using RebuildUs.Roles.Neutral;

namespace RebuildUs.Roles.Impostor;

public static class Impostor
{
    private static readonly List<PlayerControl> _untargetableCache = [];

    public static void ImpostorSetTarget()
    {
        if (!PlayerControl.LocalPlayer.IsTeamImpostor() || !PlayerControl.LocalPlayer.CanMove || PlayerControl.LocalPlayer.Data.IsDead)
        {
            FastDestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
            return;
        }

        _untargetableCache.Clear();

        // 複数のwasSpyがtrueな状態のSidekickやJackalがいるかチェック
        bool containsWasSpy = Spy.Exists;
        if (!containsWasSpy)
        {
            foreach (var s in Sidekick.Players)
            {
                if (s.WasSpy)
                {
                    containsWasSpy = true;
                    break;
                }
            }
            if (!containsWasSpy)
            {
                foreach (var j in Jackal.Players)
                {
                    if (j.WasSpy)
                    {
                        containsWasSpy = true;
                        break;
                    }
                }
            }
        }

        PlayerControl target;
        if (containsWasSpy)
        {
            if (Spy.ImpostorsCanKillAnyone)
            {
                target = Helpers.SetTarget(false, true);
            }
            else
            {
                if (Spy.Exists) _untargetableCache.AddRange(Spy.AllPlayers);

                foreach (var s in Sidekick.Players)
                {
                    if (s.WasTeamRed) _untargetableCache.Add(s.Player);
                }
                foreach (var j in Jackal.Players)
                {
                    if (j.WasTeamRed) _untargetableCache.Add(j.Player);
                }
                target = Helpers.SetTarget(true, true, _untargetableCache);
            }
        }
        else
        {
            foreach (var s in Sidekick.Players)
            {
                if (s.WasImpostor) _untargetableCache.Add(s.Player);
            }
            foreach (var j in Jackal.Players)
            {
                if (j.WasImpostor) _untargetableCache.Add(j.Player);
            }
            target = Helpers.SetTarget(true, true, _untargetableCache);
        }

        FastDestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(target); // Includes setPlayerOutline(target, Palette.ImpostorRed);
    }
}
