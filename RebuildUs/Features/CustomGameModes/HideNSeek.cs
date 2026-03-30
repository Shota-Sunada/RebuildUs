namespace RebuildUs.Features.CustomGameModes;

internal class HideNSeekMode : GameModeBase
{
    public override CustomGamemode Gamemode => CustomGamemode.HideNSeek;

    internal const float CREWMATE_LEAD_TIME = 10f;
}