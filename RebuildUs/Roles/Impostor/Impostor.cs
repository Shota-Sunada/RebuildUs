using RebuildUs.Roles.Crewmate;
using RebuildUs.Roles.Neutral;

namespace RebuildUs.Roles.Impostor;

public static class Impostor
{
    private static readonly List<PlayerControl> _untargetableCache = [];

    public static void impostorSetTarget()
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
                if (s.wasSpy)
                {
                    containsWasSpy = true;
                    break;
                }
            }
            if (!containsWasSpy)
            {
                foreach (var j in Jackal.Players)
                {
                    if (j.wasSpy)
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
            if (Spy.impostorsCanKillAnyone)
            {
                target = Helpers.setTarget(false, true);
            }
            else
            {
                if (Spy.Exists) _untargetableCache.AddRange(Spy.AllPlayers);

                foreach (var s in Sidekick.Players)
                {
                    if (s.wasTeamRed) _untargetableCache.Add(s.Player);
                }
                foreach (var j in Jackal.Players)
                {
                    if (j.wasTeamRed) _untargetableCache.Add(j.Player);
                }
                target = Helpers.setTarget(true, true, _untargetableCache);
            }
        }
        else
        {
            foreach (var s in Sidekick.Players)
            {
                if (s.wasImpostor) _untargetableCache.Add(s.Player);
            }
            foreach (var j in Jackal.Players)
            {
                if (j.wasImpostor) _untargetableCache.Add(j.Player);
            }
            target = Helpers.setTarget(true, true, _untargetableCache);
        }

        FastDestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(target); // Includes setPlayerOutline(target, Palette.ImpostorRed);
    }
}