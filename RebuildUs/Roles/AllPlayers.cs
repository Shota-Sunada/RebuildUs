namespace RebuildUs.Roles;

public static class AllPlayers
{
    public static void OnKill(PlayerControl __instance, PlayerControl target, DeadPlayer deadPlayer)
    {
        // Remove fake tasks when player dies
        if (target.HasFakeTasks())
        {
            target.ClearAllTasks();
        }

        // Seer show flash and add dead player position
        if (Seer.Exists)
        {
            if (PlayerControl.LocalPlayer.IsRole(RoleType.Seer) && PlayerControl.LocalPlayer.IsAlive() && !target.IsRole(RoleType.Seer) && Seer.Mode <= 1)
            {
                Helpers.ShowFlash(new Color(42f / 255f, 187f / 255f, 245f / 255f));
            }
            Seer.DeadBodyPositions?.Add(target.transform.position);
        }

        // // Tracker store body positions
        Tracker.DeadBodyPositions?.Add(target.transform.position);

        // Medium add body
        if (Medium.DeadBodies != null)
        {
            Medium.FeatureDeadBodies.Add(new(deadPlayer, target.transform.position));
        }

        // // Mini set adapted kill cooldown
        // if (PlayerControl.LocalPlayer.hasModifier(ModifierType.Mini) && PlayerControl.LocalPlayer.Data.Role.IsImpostor && PlayerControl.LocalPlayer == __instance)
        // {
        //     var multiplier = Mini.isGrownUp(PlayerControl.LocalPlayer) ? 0.66f : 2f;
        //     PlayerControl.LocalPlayer.SetKillTimer(PlayerControl.GameOptions.KillCooldown * multiplier);
        // }

        // Show flash on bait kill to the killer if enabled
        if (target.IsRole(RoleType.Bait) && Bait.ShowKillFlash && !__instance.IsRole(RoleType.Bait) && __instance == PlayerControl.LocalPlayer)
        {
            Helpers.ShowFlash(new Color(42f / 255f, 187f / 255f, 245f / 255f));
        }

        // // impostor promote to last impostor
        if (target.IsTeamImpostor() && AmongUsClient.Instance.AmHost)
        {
            LastImpostor.PromoteToLastImpostor();
        }
    }

    public static void Update(PlayerControl __instance)
    {
        if (__instance == null || __instance != PlayerControl.LocalPlayer) return;

        bool jackalHighlight = Engineer.HighlightForTeamJackal && (__instance.IsRole(RoleType.Jackal) || __instance.IsRole(RoleType.Sidekick));
        bool impostorHighlight = Engineer.HighlightForImpostors && __instance.IsTeamImpostor();
        bool isBait = __instance.IsRole(RoleType.Bait) && __instance.IsAlive();

        var shipStatus = MapUtilities.CachedShipStatus;
        if (shipStatus != null && shipStatus.AllVents != null)
        {
            var allVents = shipStatus.AllVents;

            // Engineer check
            bool anyEngineerInVent = false;
            if (jackalHighlight || impostorHighlight)
            {
                var engineers = Engineer.AllPlayers;
                for (int j = 0; j < engineers.Count; j++)
                {
                    if (engineers[j].inVent)
                    {
                        anyEngineerInVent = true;
                        break;
                    }
                }
            }

            // Bait check
            HashSet<int> ventsWithPlayers = [];
            bool anyPlayerInVent = false;
            if (isBait)
            {
                foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                {
                    if (player == null || !player.inVent) continue;

                    anyPlayerInVent = true;
                    var playerPos = player.GetTruePosition();
                    Vent closestVent = null;
                    float minDistance = float.MaxValue;
                    for (int j = 0; j < allVents.Length; j++)
                    {
                        var v = allVents[j];
                        if (v == null) continue;
                        float dist = Vector2.Distance(v.transform.position, playerPos);
                        if (dist < minDistance)
                        {
                            minDistance = dist;
                            closestVent = v;
                        }
                    }
                    if (closestVent != null) ventsWithPlayers.Add(closestVent.Id);
                }
            }

            for (int i = 0; i < allVents.Length; i++)
            {
                var vent = allVents[i];
                if (vent == null || vent.myRend == null) continue;

                var mat = vent.myRend.material;
                if (mat == null) continue;

                bool highlight = false;
                Color highlightColor = Color.white;

                if ((jackalHighlight || impostorHighlight) && anyEngineerInVent)
                {
                    highlight = true;
                    highlightColor = Engineer.NameColor;
                }
                else if (isBait)
                {
                    if (Bait.HighlightAllVents)
                    {
                        if (anyPlayerInVent)
                        {
                            highlight = true;
                            highlightColor = Bait.NameColor;
                        }
                    }
                    else if (ventsWithPlayers.Contains(vent.Id))
                    {
                        highlight = true;
                        highlightColor = Bait.NameColor;
                    }
                }

                if (highlight)
                {
                    mat.SetFloat("_Outline", 1f);
                    mat.SetColor("_OutlineColor", highlightColor);
                }
                else
                {
                    // Only remove outline if it's not being set by something else (Check alpha of AddColor as a proxy)
                    if (mat.HasProperty("_AddColor") && mat.GetColor("_AddColor").a == 0f)
                    {
                        mat.SetFloat("_Outline", 0f);
                    }
                }
            }
        }
    }
}