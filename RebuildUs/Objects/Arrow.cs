namespace RebuildUs.Objects;

internal sealed class Arrow
{
    private readonly ArrowBehaviour _arrowBehaviour;
    internal readonly GameObject ArrowObject;
    internal readonly SpriteRenderer Image;
    private Vector3 _oldTarget;
    internal float Perc = 0.925f;

    internal Arrow(Color color)
    {
        ArrowObject = new("Arrow")
        {
            layer = 5,
        };
        Image = ArrowObject.AddComponent<SpriteRenderer>();
        Image.sprite = AssetLoader.Arrow;
        Image.color = color;
        _arrowBehaviour = ArrowObject.AddComponent<ArrowBehaviour>();
        _arrowBehaviour.image = Image;
    }

    internal void Update()
    {
        Vector3 target = _oldTarget;
        Update(target);
    }

    internal void Update(Vector3 target, Color? color = null)
    {
        if (ArrowObject == null)
        {
            return;
        }
        _oldTarget = target;

        if (color.HasValue)
        {
            Image.color = color.Value;
        }

        _arrowBehaviour.target = target;
        _arrowBehaviour.Update();
    }
}