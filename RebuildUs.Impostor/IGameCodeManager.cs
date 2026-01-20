using Impostor.Api.Games;

namespace RebuildUs.Impostor;

public interface IGameCodeManager
{
    public int SixCharCodes { get; }
    public int FourCharCodes { get; }
    public string Path { get; }
    public GameCode Get();
    public void Release(GameCode code);
}