using UnityEngine;

namespace RebuildUs.Objects;

public class Arrow
{
    public float Perc = 0.925f;
    public SpriteRenderer Image;
    public GameObject ArrowObject;
    private Vector3 OldTarget;
    private readonly ArrowBehaviour ArrowBehaviour;

    private static Sprite Sprite;
    public static Sprite GetSprite()
    {
        if (Sprite) return Sprite;
        Sprite = AssetLoader.Arrow;
        return Sprite;
    }

    public Arrow(Color color)
    {
        ArrowObject = new GameObject("Arrow")
        {
            layer = 5
        };
        Image = ArrowObject.AddComponent<SpriteRenderer>();
        Image.sprite = GetSprite();
        Image.color = color;
        ArrowBehaviour = ArrowObject.AddComponent<ArrowBehaviour>();
        ArrowBehaviour.image = Image;
    }

    public void Update()
    {
        var target = OldTarget;
        Update(target);
    }

    public void Update(Vector3 target, Color? color = null)
    {
        if (ArrowObject == null) return;
        OldTarget = target;

        if (color.HasValue) Image.color = color.Value;

        ArrowBehaviour.target = target;
        ArrowBehaviour.Update();
    }
}