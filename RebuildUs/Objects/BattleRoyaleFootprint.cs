using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace RebuildUs.Objects;

public sealed class BattleRoyaleFootprint
{
    private static List<BattleRoyaleFootprint> _footprints = [];
    private readonly Color _color;
    private readonly GameObject _footprint;
    private readonly Vector3 _position;
    private readonly SpriteRenderer _spriteRenderer;

    public BattleRoyaleFootprint(PlayerControl player, int whichColor)
    {
        switch (whichColor)
        {
            case 0:
                _color = Palette.PlayerColors[player.Data.DefaultOutfit.ColorId];
                break;
            case 1:
                _color = Palette.PlayerColors[11];
                break;
            case 2:
                _color = Palette.PlayerColors[13];
                break;
            case 3:
                _color = Palette.PlayerColors[15];
                break;
        }

        _footprint = new("BattleRoyaleFootprint");
        _footprint.AddSubmergedComponent(SubmergedCompatibility.ELEVATOR_MOVER);
        if (GameOptionsManager.Instance.currentGameOptions.MapId == 6)
            _position = new(player.transform.position.x, player.transform.position.y, -0.5f);
        else
            _position = new(player.transform.position.x, player.transform.position.y, player.transform.position.z + 1f);
        _footprint.transform.position = _position;
        _footprint.transform.localPosition = _position;
        _footprint.transform.Rotate(0.0f, 0.0f, Random.Range(0.0f, 360.0f));
        _footprint.transform.SetParent(player.transform.parent);

        _spriteRenderer = _footprint.AddComponent<SpriteRenderer>();
        _spriteRenderer.sprite = AssetLoader.BattleRoyaleFootprint;
        _spriteRenderer.color = _color;

        _footprints.Add(this);
        _footprint.SetActive(true);

        if (BattleRoyale.MatchType == 2)
        {
            HudManager.Instance.StartCoroutine(Effects.Lerp(5, new Action<float>(p =>
            {
                if (p == 1f && _footprint != null)
                {
                    Object.Destroy(_footprint);
                    _footprints.Remove(this);
                }
            })));
        }
        else
        {
            HudManager.Instance.StartCoroutine(Effects.Lerp(420, new Action<float>(p =>
            {
                if (p == 1f && _footprint != null)
                {
                    Object.Destroy(_footprint);
                    _footprints.Remove(this);
                }
            })));
        }
    }

    public static void ClearBattleRoyaleFootprints()
    {
        _footprints = [];
    }
}
