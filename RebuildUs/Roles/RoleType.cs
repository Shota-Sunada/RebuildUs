namespace RebuildUs.Roles;

internal enum RoleType : byte
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

    // Impostor
    Impostor,
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
    EvilSwapper,

    // Neutral
    Lovers,
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
    Gm,

    // don't put anything below this
    NoRole = byte.MaxValue,
}