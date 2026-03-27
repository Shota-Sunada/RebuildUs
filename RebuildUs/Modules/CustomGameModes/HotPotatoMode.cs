namespace RebuildUs.Modules.CustomGameModes;

internal class HotPotatoMode : GameModeBase
{
    public override CustomGamemode Gamemode => CustomGamemode.HotPotato;

    // Meetings disabled in Hot Potato
    public override bool CanCallMeeting => false;
    public override Color GameModeColor => new Color32(197, 185, 64, 255);

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
}
