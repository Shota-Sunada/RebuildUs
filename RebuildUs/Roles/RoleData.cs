namespace RebuildUs.Roles;

internal enum RoleTeam
{
    Crewmate,
    Impostor,
    Neutral,
}

internal static class RoleData
{
    internal static readonly RoleRegistration[] Roles =
    [
        // Crewmate
        new(RoleType.Mayor, RoleTeam.Crewmate, typeof(MultiRoleBase<Mayor>), () => Mayor.NameColor, () => CustomOptionHolder.MayorSpawnRate),
        new(RoleType.Engineer, RoleTeam.Crewmate, typeof(MultiRoleBase<Engineer>), () => Engineer.NameColor, () => CustomOptionHolder.EngineerSpawnRate),
        new(RoleType.Sheriff, RoleTeam.Crewmate, typeof(MultiRoleBase<Sheriff>), () => Sheriff.NameColor, () => CustomOptionHolder.SheriffSpawnRate),
        new(RoleType.Lighter, RoleTeam.Crewmate, typeof(MultiRoleBase<Lighter>), () => Lighter.NameColor, () => CustomOptionHolder.LighterSpawnRate),
        new(RoleType.Detective, RoleTeam.Crewmate, typeof(MultiRoleBase<Detective>), () => Detective.NameColor, () => CustomOptionHolder.DetectiveSpawnRate),
        new(RoleType.TimeMaster, RoleTeam.Crewmate, typeof(SingleRoleBase<TimeMaster>), () => TimeMaster.NameColor, () => CustomOptionHolder.TimeMasterSpawnRate),
        new(RoleType.Medic, RoleTeam.Crewmate, typeof(SingleRoleBase<Medic>), () => Medic.NameColor, () => CustomOptionHolder.MedicSpawnRate),
        new(RoleType.Seer, RoleTeam.Crewmate, typeof(MultiRoleBase<Seer>), () => Seer.NameColor, () => CustomOptionHolder.SeerSpawnRate),
        new(RoleType.Hacker, RoleTeam.Crewmate, typeof(MultiRoleBase<Hacker>), () => Hacker.NameColor, () => CustomOptionHolder.HackerSpawnRate),
        new(RoleType.Tracker, RoleTeam.Crewmate, typeof(MultiRoleBase<Tracker>), () => Tracker.NameColor, () => CustomOptionHolder.TrackerSpawnRate),
        new(RoleType.Snitch, RoleTeam.Crewmate, typeof(SingleRoleBase<Snitch>), () => Snitch.NameColor, () => CustomOptionHolder.SnitchSpawnRate),
        new(RoleType.Spy, RoleTeam.Crewmate, typeof(SingleRoleBase<Spy>), () => Spy.NameColor, () => CustomOptionHolder.SpySpawnRate),
        new(RoleType.SecurityGuard, RoleTeam.Crewmate, typeof(SingleRoleBase<SecurityGuard>), () => SecurityGuard.NameColor, () => CustomOptionHolder.SecurityGuardSpawnRate),
        new(RoleType.Bait, RoleTeam.Crewmate, typeof(MultiRoleBase<Bait>), () => Bait.NameColor, () => CustomOptionHolder.BaitSpawnRate),
        new(RoleType.Medium, RoleTeam.Crewmate, typeof(MultiRoleBase<Medium>), () => Medium.NameColor, () => CustomOptionHolder.MediumSpawnRate),
        new(RoleType.Shifter, RoleTeam.Crewmate, typeof(MultiRoleBase<Shifter>), () => Shifter.NameColor, () => CustomOptionHolder.ShifterSpawnRate),
        new(RoleType.Madmate, RoleTeam.Crewmate, typeof(MultiRoleBase<MadmateRole>), () => MadmateRole.NameColor, () => CustomOptionHolder.MadmateRoleSpawnRate),
        new(RoleType.Suicider, RoleTeam.Crewmate, typeof(MultiRoleBase<Suicider>), () => Suicider.NameColor, () => CustomOptionHolder.SuiciderSpawnRate),

        // Impostor
        new(RoleType.BountyHunter, RoleTeam.Impostor, typeof(SingleRoleBase<BountyHunter>), () => BountyHunter.NameColor, () => CustomOptionHolder.BountyHunterSpawnRate),
        new(RoleType.Godfather, RoleTeam.Impostor, typeof(SingleRoleBase<Mafia.Godfather>), () => Mafia.NameColor, () => CustomOptionHolder.MafiaSpawnRate),
        new(RoleType.Mafioso, RoleTeam.Impostor, typeof(SingleRoleBase<Mafia.Mafioso>), () => Mafia.NameColor, () => CustomOptionHolder.MafiaSpawnRate),
        new(RoleType.Janitor, RoleTeam.Impostor, typeof(SingleRoleBase<Mafia.Janitor>), () => Mafia.NameColor, () => CustomOptionHolder.MafiaSpawnRate),
        new(RoleType.Morphing, RoleTeam.Impostor, typeof(MultiRoleBase<Morphing>), () => Morphing.NameColor, () => CustomOptionHolder.MorphingSpawnRate),
        new(RoleType.Camouflager, RoleTeam.Impostor, typeof(SingleRoleBase<Camouflager>), () => Camouflager.NameColor, () => CustomOptionHolder.CamouflagerSpawnRate),
        new(RoleType.Vampire, RoleTeam.Impostor, typeof(MultiRoleBase<Vampire>), () => Vampire.NameColor, () => CustomOptionHolder.VampireSpawnRate),
        new(RoleType.Eraser, RoleTeam.Impostor, typeof(MultiRoleBase<Eraser>), () => Eraser.NameColor, () => CustomOptionHolder.EraserSpawnRate),
        new(RoleType.Trickster, RoleTeam.Impostor, typeof(SingleRoleBase<Trickster>), () => Trickster.NameColor, () => CustomOptionHolder.TricksterSpawnRate),
        new(RoleType.Cleaner, RoleTeam.Impostor, typeof(MultiRoleBase<Cleaner>), () => Cleaner.NameColor, () => CustomOptionHolder.CleanerSpawnRate),
        new(RoleType.Warlock, RoleTeam.Impostor, typeof(MultiRoleBase<Warlock>), () => Warlock.NameColor, () => CustomOptionHolder.WarlockSpawnRate),
        new(RoleType.Witch, RoleTeam.Impostor, typeof(MultiRoleBase<Witch>), () => Witch.NameColor, () => CustomOptionHolder.WitchSpawnRate),
        new(RoleType.EvilHacker, RoleTeam.Impostor, typeof(MultiRoleBase<EvilHacker>), () => EvilHacker.NameColor, () => CustomOptionHolder.EvilHackerSpawnRate),
        new(RoleType.EvilTracker, RoleTeam.Impostor, typeof(MultiRoleBase<EvilTracker>), () => EvilTracker.NameColor, () => CustomOptionHolder.EvilTrackerSpawnRate),

        // Neutral
        new(RoleType.Jester, RoleTeam.Neutral, typeof(SingleRoleBase<Jester>), () => Jester.NameColor, () => CustomOptionHolder.JesterSpawnRate),
        new(RoleType.Arsonist, RoleTeam.Neutral, typeof(SingleRoleBase<Arsonist>), () => Arsonist.NameColor, () => CustomOptionHolder.ArsonistSpawnRate),
        new(RoleType.Vulture, RoleTeam.Neutral, typeof(SingleRoleBase<Vulture>), () => Vulture.NameColor, () => CustomOptionHolder.VultureSpawnRate),
        new(RoleType.Jackal, RoleTeam.Neutral, typeof(SingleRoleBase<Jackal>), () => Jackal.NameColor, () => CustomOptionHolder.JackalSpawnRate),
        new(RoleType.Sidekick, RoleTeam.Neutral, typeof(SingleRoleBase<Sidekick>), () => Jackal.NameColor, () => CustomOptionHolder.JackalSpawnRate),
        new(RoleType.Lovers, RoleTeam.Neutral, null, () => Lovers.Color, () => CustomOptionHolder.LoversSpawnRate),

        new(RoleType.NiceGuesser, RoleTeam.Crewmate, typeof(SingleRoleBase<Guesser.NiceGuesser>), () => Guesser.NiceGuesser.NameColor, () => CustomOptionHolder.GuesserSpawnRate),
        new(RoleType.EvilGuesser, RoleTeam.Impostor, typeof(SingleRoleBase<Guesser.EvilGuesser>), () => Guesser.EvilGuesser.NameColor, () => CustomOptionHolder.GuesserSpawnRate),
        new(RoleType.NiceSwapper, RoleTeam.Crewmate, typeof(SingleRoleBase<Swapper>), () => Swapper.NameColor, () => CustomOptionHolder.SwapperSpawnRate),
        new(RoleType.EvilSwapper, RoleTeam.Impostor, typeof(SingleRoleBase<Swapper>), () => Palette.ImpostorRed, () => CustomOptionHolder.SwapperSpawnRate),
    ];

    internal sealed record RoleRegistration(RoleType RoleType, RoleTeam RoleTeam, Type ClassType, Func<Color> GetColor, Func<CustomOption> GetOption);
}