namespace RebuildUs.Modules.CustomGameModes;

internal class BattleRoyaleMode : GameModeBase
{
    public override string InternalName => "BattleRoyale";

    // Meetings disabled in Battle Royale
    public override bool CanCallMeeting => false;

    public override bool DisableNormalWinConditions => true; // Needs custom win condition

    public override bool OnBeginIntro(IntroCutscene instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> teamToDisplay)
    {
        instance.TeamTitle.text = "BATTLE ROYALE";
        instance.TeamTitle.color = Palette.ImpostorRed;
        instance.ImpostorText.gameObject.SetActive(false);
        instance.BackgroundBar.material.color = Palette.ImpostorRed;
        return true; // We handled intro text
    }

    public override bool OnSetupRole(IntroCutscene instance)
    {
        instance.RoleText.text = "Battle Royale";
        instance.RoleText.color = Palette.ImpostorRed;
        instance.RoleBlurbText.text = "Be the last one standing!";
        return true;
    }

    public override bool CheckWinCondition(PlayerStatistics stats)
    {
        // Add battle royale specific win checks here later
        return false;
    }
}
