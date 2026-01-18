namespace RebuildUs.Roles;

public enum RoleTeam
{
    Crewmate,
    Impostor,
    Neutral
}

public static class RoleData
{
    public record RoleRegistration(
        RoleType roleType,
        RoleTeam roleTeam,
        Type classType,
        Func<Color> getColor,
        Func<CustomOption> getOption
    );

    public static readonly RoleRegistration[] Roles =
    [
        // Crewmate
        new(RoleType.Mayor, RoleTeam.Crewmate, typeof(Mayor), () => Mayor.NameColor, () => CustomOptionHolder.MayorSpawnRate),
        new(RoleType.Engineer, RoleTeam.Crewmate, typeof(Engineer), () => Engineer.NameColor, () => CustomOptionHolder.EngineerSpawnRate),
        new(RoleType.Sheriff, RoleTeam.Crewmate, typeof(Sheriff), () => Sheriff.NameColor, () => CustomOptionHolder.sheriffSpawnRate),
        new(RoleType.Lighter, RoleTeam.Crewmate, typeof(Lighter), () => Lighter.NameColor, () => CustomOptionHolder.LighterSpawnRate),
        new(RoleType.Detective, RoleTeam.Crewmate, typeof(Detective), () => Detective.NameColor, () => CustomOptionHolder.DetectiveSpawnRate),
        new(RoleType.TimeMaster, RoleTeam.Crewmate, typeof(TimeMaster), () => TimeMaster.NameColor, () => CustomOptionHolder.TimeMasterSpawnRate),
        new(RoleType.Medic, RoleTeam.Crewmate, typeof(Medic), () => Medic.NameColor, () => CustomOptionHolder.MedicSpawnRate),
        new(RoleType.Swapper, RoleTeam.Crewmate, typeof(Swapper), () => Swapper.NameColor, () => CustomOptionHolder.SwapperSpawnRate),
        new(RoleType.Seer, RoleTeam.Crewmate, typeof(Seer), () => Seer.NameColor, () => CustomOptionHolder.SeerSpawnRate),
        new(RoleType.Hacker, RoleTeam.Crewmate, typeof(Hacker), () => Hacker.NameColor, () => CustomOptionHolder.HackerSpawnRate),
        new(RoleType.Tracker, RoleTeam.Crewmate, typeof(Tracker), () => Tracker.NameColor, () => CustomOptionHolder.TrackerSpawnRate),
        new(RoleType.Snitch, RoleTeam.Crewmate, typeof(Snitch), () => Snitch.NameColor, () => CustomOptionHolder.SnitchSpawnRate),
        new(RoleType.Spy, RoleTeam.Crewmate, typeof(Spy), () => Spy.NameColor, () => CustomOptionHolder.SpySpawnRate),
        new(RoleType.SecurityGuard, RoleTeam.Crewmate, typeof(SecurityGuard), () => SecurityGuard.NameColor, () => CustomOptionHolder.SecurityGuardSpawnRate),
        new(RoleType.Bait, RoleTeam.Crewmate, typeof(Bait), () => Bait.NameColor, () => CustomOptionHolder.BaitSpawnRate),
        new(RoleType.Medium, RoleTeam.Crewmate, typeof(Medium), () => Medium.NameColor, () => CustomOptionHolder.MediumSpawnRate),
        new(RoleType.Shifter, RoleTeam.Crewmate, typeof(Shifter), () => Shifter.NameColor, () => CustomOptionHolder.ShifterSpawnRate),

        // Impostor
        new(RoleType.BountyHunter, RoleTeam.Impostor, typeof(BountyHunter), () => BountyHunter.NameColor, () => CustomOptionHolder.BountyHunterSpawnRate),
        new(RoleType.Godfather, RoleTeam.Impostor, typeof(Mafia.Godfather), () => Mafia.NameColor, () => CustomOptionHolder.MafiaSpawnRate),
        new(RoleType.Mafioso, RoleTeam.Impostor, typeof(Mafia.Mafioso), () => Mafia.NameColor, () => CustomOptionHolder.MafiaSpawnRate),
        new(RoleType.Janitor, RoleTeam.Impostor, typeof(Mafia.Janitor), () => Mafia.NameColor, () => CustomOptionHolder.MafiaSpawnRate),
        new(RoleType.Morphing, RoleTeam.Impostor, typeof(Morphing), () => Morphing.NameColor, () => CustomOptionHolder.MorphingSpawnRate),
        new(RoleType.Camouflager, RoleTeam.Impostor, typeof(Camouflager), () => Camouflager.NameColor, () => CustomOptionHolder.CamouflagerSpawnRate),
        new(RoleType.Vampire, RoleTeam.Impostor, typeof(Vampire), () => Vampire.NameColor, () => CustomOptionHolder.VampireSpawnRate),
        new(RoleType.Eraser, RoleTeam.Impostor, typeof(Eraser), () => Eraser.NameColor, () => CustomOptionHolder.EraserSpawnRate),
        new(RoleType.Trickster, RoleTeam.Impostor, typeof(Trickster), () => Trickster.NameColor, () => CustomOptionHolder.TricksterSpawnRate),
        new(RoleType.Cleaner, RoleTeam.Impostor, typeof(Cleaner), () => Cleaner.NameColor, () => CustomOptionHolder.CleanerSpawnRate),
        new(RoleType.Warlock, RoleTeam.Impostor, typeof(Warlock), () => Warlock.NameColor, () => CustomOptionHolder.WarlockSpawnRate),
        new(RoleType.Witch, RoleTeam.Impostor, typeof(Witch), () => Witch.NameColor, () => CustomOptionHolder.WitchSpawnRate),
        new(RoleType.EvilHacker, RoleTeam.Impostor, typeof(EvilHacker), () => EvilHacker.NameColor, () => CustomOptionHolder.EvilHackerSpawnRate),
        new(RoleType.EvilTracker, RoleTeam.Impostor, typeof(EvilTracker), () => EvilTracker.NameColor, () => CustomOptionHolder.EvilTrackerSpawnRate),

        // Neutral
        new(RoleType.Jester, RoleTeam.Neutral, typeof(Jester), () => Jester.NameColor, () => CustomOptionHolder.JesterSpawnRate),
        new(RoleType.Arsonist, RoleTeam.Neutral, typeof(Arsonist), () => Arsonist.NameColor, () => CustomOptionHolder.ArsonistSpawnRate),
        new(RoleType.Vulture, RoleTeam.Neutral, typeof(Vulture), () => Vulture.NameColor, () => CustomOptionHolder.VultureSpawnRate),
        new(RoleType.Jackal, RoleTeam.Neutral, typeof(Jackal), () => Jackal.NameColor, () => CustomOptionHolder.JackalSpawnRate),
        new(RoleType.Sidekick, RoleTeam.Neutral, typeof(Sidekick), () => Jackal.NameColor, () => CustomOptionHolder.JackalSpawnRate),
        new(RoleType.NiceGuesser, RoleTeam.Neutral, typeof(Guesser.NiceGuesser), () => Guesser.NiceGuesser.NameColor, () => CustomOptionHolder.GuesserSpawnRate),
        new(RoleType.EvilGuesser, RoleTeam.Neutral, typeof(Guesser.EvilGuesser), () => Guesser.EvilGuesser.NameColor, () => CustomOptionHolder.GuesserSpawnRate),
        new(RoleType.Lovers, RoleTeam.Neutral, typeof(Lovers), () => Lovers.Color, () => CustomOptionHolder.LoversSpawnRate),
    ];

    public static (RoleType RoleType, Type Type)[] AllRoleTypes => [.. Roles.Select(r => (r.roleType, r.classType))];
}