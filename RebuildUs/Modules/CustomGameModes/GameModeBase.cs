namespace RebuildUs.Modules.CustomGameModes;

internal abstract class GameModeBase
{
    // The internal ID/name of the game mode.
    public abstract string InternalName { get; }

    // Properties that define game rules
    public virtual bool CanCallMeeting { get; } = true;
    public virtual bool DisableNormalWinConditions { get; } = false;

    // Intro Phase Hooks
    // Return true to avoid standard crewmate/impostor processing.
    public virtual bool OnBeginIntro(IntroCutscene instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> teamToDisplay)
    {
        return false;
    }

    // Role Phase Hook
    // Return true to avoid standard 'SetupRole' processing.
    public virtual bool OnSetupRole(IntroCutscene instance)
    {
        return false;
    }

    // Win Hook
    // Return true if the game mode has ended the game (calls UncheckedEndGame inside).
    public virtual bool CheckWinCondition(PlayerStatistics stats)
    {
        return false;
    }
}
