namespace RebuildUs.Modules.CustomGameModes;

internal class HotPotatoMode : GameModeBase
{
    public override string InternalName => "HotPotato";

    // Meetings disabled in Hot Potato
    public override bool CanCallMeeting => false;

    public override bool DisableNormalWinConditions => true;

    public override bool OnBeginIntro(IntroCutscene instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> teamToDisplay)
    {
        instance.TeamTitle.text = "HOT POTATO";
        instance.TeamTitle.color = new Color(1f, 0.5f, 0f); // Orange
        instance.ImpostorText.gameObject.SetActive(false);
        instance.BackgroundBar.material.color = new Color(1f, 0.5f, 0f);
        return true;
    }

    public override bool OnSetupRole(IntroCutscene instance)
    {
        instance.RoleText.text = "Hot Potato";
        instance.RoleText.color = new Color(1f, 0.5f, 0f); // Orange
        instance.RoleBlurbText.text = "Don't hold the potato when the timer runs out!";
        return true;
    }

    public override bool CheckWinCondition(PlayerStatistics stats)
    {
        // Add hot potato specific win checks here later
        return false;
    }
}
