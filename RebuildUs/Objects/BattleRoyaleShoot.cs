using Object = UnityEngine.Object;

namespace RebuildUs.Objects;

public sealed class BattleRoyaleShoot
{
    private static List<BattleRoyaleShoot> _shoots = [];
    private static Sprite _sprite;
    private readonly Color _color;
    private readonly Vector3 _position;
    private readonly GameObject _shoot;
    private readonly SpriteRenderer _spriteRenderer;

    public BattleRoyaleShoot(PlayerControl player, int whichColor, float angle)
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

        _shoot = new("BattleRoyaleShoot");
        _shoot.AddSubmergedComponent(SubmergedCompatibility.ELEVATOR_MOVER);
        if (GameOptionsManager.Instance.currentGameOptions.MapId == 6)
            _position = new(player.transform.position.x, player.transform.position.y, -0.5f);
        else
            _position = new(player.transform.position.x, player.transform.position.y, player.transform.position.z + 1f);
        _shoot.transform.position = _position;
        _shoot.transform.localPosition = _position;
        _shoot.transform.SetParent(player.transform.parent);
        _shoot.transform.eulerAngles = new(0f, 0f, (float)((angle * 360f) / Math.PI / 2f));

        _spriteRenderer = _shoot.AddComponent<SpriteRenderer>();
        _spriteRenderer.sprite = GetShootSprite();
        _spriteRenderer.color = _color;

        _shoots.Add(this);
        _shoot.SetActive(true);

        HudManager.Instance.StartCoroutine(Effects.Lerp(0.5f, new Action<float>(p =>
        {
            if (p == 1f && _shoot != null)
            {
                Object.Destroy(_shoot);
                _shoots.Remove(this);
            }
        })));
    }

    public static Sprite GetShootSprite()
    {
        if (_sprite) return _sprite;
        _sprite = AssetLoader.RoyaleShoot.GetComponent<SpriteRenderer>().sprite;
        return _sprite;
    }

    public static void ClearBattleRoyaleShoots()
    {
        _shoots = [];
    }
}
