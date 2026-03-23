using BepInEx.Unity.IL2CPP.Utils;

namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
[RegisterRole(RoleType.Bait, RoleTeam.Crewmate, typeof(MultiRoleBase<Bait>), nameof(CustomOptionHolder.BaitSpawnRate))]
internal class Bait : MultiRoleBase<Bait>
{
    public static Color Color = new Color32(0, 247, 255, byte.MaxValue);
    private float _delay;

    private bool _reported;
    private CustomMessage _warningMessage;

    public Bait()
    {
        StaticRoleType = CurrentRoleType = RoleType.Bait;
        _delay = ReportDelay;
    }

    internal static bool HighlightAllVents { get => CustomOptionHolder.BaitHighlightAllVents.GetBool(); }
    internal static float ReportDelay { get => CustomOptionHolder.BaitReportDelay.GetFloat(); }
    internal static bool NotifyToMurder { get => CustomOptionHolder.BaitShowKillFlash.GetBool(); }

    [CustomEvent(CustomEventType.OnDeath)]
    internal void OnDeath(PlayerControl killer)
    {
        if (killer == null)
        {
            return;
        }

        if (Player == PlayerControl.LocalPlayer)
        {
            if (NotifyToMurder)
            {
                using var sender = new RPCSender(Player.NetId, CustomRPC.BaitOnKilled);
                sender.Write(killer.PlayerId);
                RPCProcedure.BaitOnKilled(killer.PlayerId);
            }

            // 通報コルーチンを開始
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(ReportCoroutine(killer));
        }
    }

    private IEnumerator ReportCoroutine(PlayerControl killer)
    {
        yield return new WaitForSeconds(ReportDelay);

        if (_reported || Helpers.IsGameOver)
        {
            yield break;
        }

        Helpers.HandleVampireBiteOnBodyReport(); // Manually call Vampire handling, since the CmdReportDeadBody Prefix won't be called

        var reporter = killer.PlayerId;
        if (Player.HasModifier(ModifierType.Madmate))
        {
            List<PlayerControl> candidates = [];
            foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (p != null && p.IsAlive() && !p.IsTeamImpostor() && !p.isDummy)
                {
                    candidates.Add(p);
                }
            }

            if (candidates.Count > 0)
            {
                var i = RebuildUs.Rnd.Next(0, candidates.Count);
                reporter = candidates[i].PlayerId;
            }
        }

        {
            using RPCSender sender = new(Player.NetId, CustomRPC.UncheckedCmdReportDeadBody);
            sender.Write(reporter);
            sender.Write(Player.PlayerId);
        }
        RPCProcedure.UncheckedCmdReportDeadBody(reporter, Player.PlayerId);
        _reported = true;
        _warningMessage = null;
    }

    internal static void Clear()
    {
        Players.Clear();
    }
}