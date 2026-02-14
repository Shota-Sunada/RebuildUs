namespace RebuildUs.Roles;

public enum RoleType : byte
{
    // Crewmate
    Crewmate = 0,
    Shifter,
    Mayor,
    Engineer,
    Sheriff,
    Lighter,
    Detective,
    TimeMaster,
    Medic,
    NiceSwapper,
    Seer,
    Hacker,
    Tracker,
    Snitch,
    Spy,
    SecurityGuard,
    Bait,
    Medium,
    FortuneTeller,
    Suicider,

    // Impostor
    Impostor = 100,
    Godfather,
    Mafioso,
    Janitor,
    Morphing,
    Camouflager,
    Vampire,
    Eraser,
    Trickster,
    Cleaner,
    Warlock,
    BountyHunter,
    Witch,
    Madmate,
    EvilSwapper,

    // Neutral
    Lovers = 150,
    EvilGuesser,
    NiceGuesser,
    Jester,
    Arsonist,
    Jackal,
    Sidekick,
    // Opportunist,
    Vulture,
    // PlagueDoctor,
    // Watcher,
    EvilTracker,
    EvilHacker,

    // Others
    GM = 200,

    // don't put anything below this
    NoRole = byte.MaxValue
}