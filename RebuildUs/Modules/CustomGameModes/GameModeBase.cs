namespace RebuildUs.Modules.CustomGameModes;

internal abstract class GameModeBase
{
    // The internal ID/name of the game mode.
    public abstract CustomGamemode Gamemode { get; }

    // Properties that define game rules
    public virtual bool CanCallMeeting { get; } = true;

    public virtual bool SkipShowRole { get; } = false;

    // Whether to use the custom timer bar.
    public virtual bool UseTimerBar { get; } = false;

    public virtual Color GameModeColor { get; } = Color.white;

    // Game Lifecycle Hooks

    // Called during role assignment phase.
    public virtual void AssignRoles() { }

    // Called when the game truly starts (after ShipStatus.Begin).
    public virtual void OnGameStart() { }

    // Called every frame for each player.
    public virtual void OnPlayerUpdate(PlayerControl player) { }

    // Called when the game ends.
    public virtual void OnGameEnd() { }

    // Called every frame to update the timer bar if UseTimerBar is true.
    public virtual void OnTimerBarUpdate(float deltaTime) { }

    public virtual void OnKill(PlayerControl target) { }

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

    public virtual void OnIntroDestroyed() { }
}
