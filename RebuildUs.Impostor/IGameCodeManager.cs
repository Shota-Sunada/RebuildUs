using Impostor.Api.Games;

namespace RebuildUs.Impostor;

public interface IGameCodeManager
{
    int SixCharCodes { get; }
    int FourCharCodes { get; }
    string Path { get; }
    GameCode Get();
    void Release(GameCode code);
    bool IsInUse(string code);
    bool AnyInUse();
}