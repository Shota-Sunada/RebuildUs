using Object = UnityEngine.Object;

namespace RebuildUs.Objects;

public sealed class Tased
{
    public static List<Tased> Tasers = new();
    private static Sprite _sprite;
    private readonly Color _color;
    private readonly SpriteRenderer _spriteRenderer;
    private readonly GameObject _taser;

    public Tased(float taserDuration, PlayerControl player)
    {
        _color = new(1f, 1f, 1f, 1f);

        _taser = new("Tased");
        _taser.AddSubmergedComponent(SubmergedCompatibility.ELEVATOR_MOVER);
        var position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z - 1f);
        _taser.transform.position = position;
        _taser.transform.localPosition = position;
        _taser.transform.SetParent(player.transform);

        _spriteRenderer = _taser.AddComponent<SpriteRenderer>();
        _spriteRenderer.sprite = GetTaserSprite();
        _spriteRenderer.color = _color;

        _taser.SetActive(true);
        Tasers.Add(this);

        HudManager.Instance.StartCoroutine(Effects.Lerp(taserDuration, new Action<float>(p =>
        {
            player.moveable = false;
            player.NetTransform.Halt(); // Stop current movement
            if (p == 1f)
            {
                player.moveable = true;
                Object.Destroy(_taser);
                Tasers.Remove(this);
            }
        })));
    }

    public static Sprite GetTaserSprite()
    {
        if (_sprite) return _sprite;
        _sprite = AssetLoader.PoliceParalyze.GetComponent<SpriteRenderer>().sprite;
        return _sprite;
    }
}
