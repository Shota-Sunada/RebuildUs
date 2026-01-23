namespace RebuildUs.Roles.Impostor;

public static class Impostor
{
    private static readonly List<PlayerControl> _untargetableCache = [];

    public static void ImpostorSetTarget()
    {
        var local = PlayerControl.LocalPlayer;
        if (!local.IsTeamImpostor() || !local.CanMove || local.Data.IsDead)
        {
            FastDestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
            return;
        }

        _untargetableCache.Clear();

        // 複数のwasSpyがtrueな状態のSidekickやJackalがいるかチェック
        bool containsWasSpy = Spy.Exists;
        if (!containsWasSpy)
        {
            var sidekickPlayers = Sidekick.Players;
            for (var i = 0; i < sidekickPlayers.Count; i++)
            {
                if (sidekickPlayers[i].WasSpy)
                {
                    containsWasSpy = true;
                    break;
                }
            }
            if (!containsWasSpy)
            {
                var jackalPlayers = Jackal.Players;
                for (var i = 0; i < jackalPlayers.Count; i++)
                {
                    if (jackalPlayers[i].WasSpy)
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
                if (Spy.Exists)
                {
                    var spyPlayers = Spy.AllPlayers;
                    for (var i = 0; i < spyPlayers.Count; i++)
                    {
                        _untargetableCache.Add(spyPlayers[i]);
                    }
                }

                var sidekickPlayers = Sidekick.Players;
                for (var i = 0; i < sidekickPlayers.Count; i++)
                {
                    var s = sidekickPlayers[i];
                    if (s.WasTeamRed) _untargetableCache.Add(s.Player);
                }
                var jackalPlayers = Jackal.Players;
                for (var i = 0; i < jackalPlayers.Count; i++)
                {
                    var j = jackalPlayers[i];
                    if (j.WasTeamRed) _untargetableCache.Add(j.Player);
                }
                target = Helpers.SetTarget(true, true, _untargetableCache);
            }
        }
        else
        {
            var sidekickPlayers = Sidekick.Players;
            for (var i = 0; i < sidekickPlayers.Count; i++)
            {
                var s = sidekickPlayers[i];
                if (s.WasImpostor) _untargetableCache.Add(s.Player);
            }
            var jackalPlayers = Jackal.Players;
            for (var i = 0; i < jackalPlayers.Count; i++)
            {
                var j = jackalPlayers[i];
                if (j.WasImpostor) _untargetableCache.Add(j.Player);
            }
            target = Helpers.SetTarget(true, true, _untargetableCache);
        }

        FastDestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(target); // Includes setPlayerOutline(target, Palette.ImpostorRed);
    }
}