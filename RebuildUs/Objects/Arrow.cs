namespace RebuildUs.Objects;

public sealed class Arrow
{
    private readonly ArrowBehaviour _arrowBehaviour;
    private Vector3 _oldTarget;
    public GameObject ArrowObject;
    public SpriteRenderer Image;
    public float Perc = 0.925f;

    public Arrow(Color color)
    {
        ArrowObject = new("Arrow") { layer = 5 };
        Image = ArrowObject.AddComponent<SpriteRenderer>();
        Image.sprite = AssetLoader.Arrow;
        Image.color = color;
        _arrowBehaviour = ArrowObject.AddComponent<ArrowBehaviour>();
        _arrowBehaviour.image = Image;
    }

    public void Update()
    {
        var target = _oldTarget;
        Update(target);
    }

    public void Update(Vector3 target, Color? color = null)
    {
        if (ArrowObject == null) return;
        _oldTarget = target;

        if (color.HasValue) Image.color = color.Value;

        _arrowBehaviour.target = target;
        _arrowBehaviour.Update();
    }
}
