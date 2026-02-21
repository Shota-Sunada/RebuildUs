namespace RebuildUs.Roles;

internal static class AllPlayers
{
    private static readonly int Outline = Shader.PropertyToID("_Outline");
    private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");
    private static readonly int AddColor = Shader.PropertyToID("_AddColor");

    internal static void OnKill(PlayerControl __instance, PlayerControl target, DeadPlayer deadPlayer)
    {
        // Remove fake tasks when player dies
        if (target.HasFakeTasks()) target.ClearAllTasks();

        // Seer show flash and add dead player position
        if (Seer.Exists)
        {
            if (PlayerControl.LocalPlayer.IsRole(RoleType.Seer) && PlayerControl.LocalPlayer.IsAlive() && !target.IsRole(RoleType.Seer) && Seer.Mode <= 1) Helpers.ShowFlash(new(42f / 255f, 187f / 255f, 245f / 255f));

            Seer.DeadBodyPositions?.Add(target.transform.position);
        }

        // // Tracker store body positions
        Tracker.DeadBodyPositions?.Add(target.transform.position);

        // Medium add body
        if (Medium.DeadBodies != null) Medium.FeatureDeadBodies.Add(new(deadPlayer, target.transform.position));

        // // Mini set adapted kill cooldown
        // if (PlayerControl.LocalPlayer.hasModifier(ModifierType.Mini) && PlayerControl.LocalPlayer.Data.Role.IsImpostor && PlayerControl.LocalPlayer == __instance)
        // {
        //     var multiplier = Mini.isGrownUp(PlayerControl.LocalPlayer) ? 0.66f : 2f;
        //     PlayerControl.LocalPlayer.SetKillTimer(PlayerControl.GameOptions.KillCooldown * multiplier);
        // }

        // Show flash on bait kill to the killer if enabled
        if (target.IsRole(RoleType.Bait) && Bait.ShowKillFlash && !__instance.IsRole(RoleType.Bait) && __instance == PlayerControl.LocalPlayer) Helpers.ShowFlash(new(42f / 255f, 187f / 255f, 245f / 255f));

        // // impostor promote to last impostor
        if (target.IsTeamImpostor() && AmongUsClient.Instance.AmHost) LastImpostor.PromoteToLastImpostor();
    }

    internal static void Update(PlayerControl __instance)
    {
        if (__instance == null || __instance != PlayerControl.LocalPlayer) return;

        bool jackalHighlight = Engineer.HighlightForTeamJackal && (__instance.IsRole(RoleType.Jackal) || __instance.IsRole(RoleType.Sidekick));
        bool impostorHighlight = Engineer.HighlightForImpostors && __instance.IsTeamImpostor();
        bool isBait = __instance.IsRole(RoleType.Bait) && __instance.IsAlive();

        ShipStatus shipStatus = MapUtilities.CachedShipStatus;
        if (shipStatus != null && shipStatus.AllVents != null)
        {
            Il2CppReferenceArray<Vent> allVents = shipStatus.AllVents;

            // Engineer check
            bool anyEngineerInVent = false;
            if (jackalHighlight || impostorHighlight)
            {
                List<PlayerControl> engineers = Engineer.AllPlayers;
                foreach (PlayerControl t in engineers)
                {
                    if (t.inVent) continue;
                    anyEngineerInVent = true;
                    break;
                }
            }

            // Bait check
            HashSet<int> ventsWithPlayers = [];
            bool anyPlayerInVent = false;
            if (isBait)
            {
                foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                {
                    if (player == null || !player.inVent) continue;

                    anyPlayerInVent = true;
                    Vector2 playerPos = player.GetTruePosition();
                    Vent closestVent = null;
                    float minDistance = float.MaxValue;
                    foreach (Vent v in allVents)
                    {
                        if (v == null) continue;
                        float dist = Vector2.Distance(v.transform.position, playerPos);
                        if (!(dist < minDistance)) continue;
                        minDistance = dist;
                        closestVent = v;
                    }

                    if (closestVent != null) ventsWithPlayers.Add(closestVent.Id);
                }
            }

            foreach (Vent vent in allVents)
            {
                if (vent == null || vent.myRend == null) continue;

                Material mat = vent.myRend.material;
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
                    mat.SetFloat(Outline, 1f);
                    mat.SetColor(OutlineColor, highlightColor);
                }
                else
                {
                    // Only remove outline if it's not being set by something else (Check alpha of AddColor as a proxy)
                    if (mat.HasProperty(AddColor) && mat.GetColor(AddColor).a == 0f) mat.SetFloat(Outline, 0f);
                }
            }
        }
    }
}