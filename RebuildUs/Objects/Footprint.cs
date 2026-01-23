namespace RebuildUs.Objects;

public class Footprint
{
    private static readonly List<Footprint> Footprints = [];
    private Color Color;
    private readonly GameObject FootprintObj;
    private readonly SpriteRenderer SpriteRenderer;
    private readonly PlayerControl Owner;
    private readonly bool AnonymousFootprints;

    public Footprint(float footprintDuration, bool anonymousFootprints, PlayerControl player)
    {
        Owner = player;
        this.AnonymousFootprints = anonymousFootprints;
        Color = anonymousFootprints ? (Color)Palette.PlayerColors[6] : (Color)Palette.PlayerColors[player.Data.DefaultOutfit.ColorId];

        FootprintObj = new GameObject("Footprint");
        Vector3 position = new(player.transform.position.x, player.transform.position.y, player.transform.position.z + 1f);
        FootprintObj.transform.position = position;
        FootprintObj.transform.localPosition = position;
        FootprintObj.transform.SetParent(player.transform.parent);

        FootprintObj.transform.Rotate(0.0f, 0.0f, UnityEngine.Random.Range(0.0f, 360.0f));

        SpriteRenderer = FootprintObj.AddComponent<SpriteRenderer>();
        SpriteRenderer.sprite = AssetLoader.Footprint;
        SpriteRenderer.color = Color;

        FootprintObj.SetActive(true);
        Footprints.Add(this);

        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(footprintDuration, new Action<float>((p) =>
        {
            if (SpriteRenderer == null) return;

            var c = Color;
            if (!anonymousFootprints && Owner != null)
            {
                if (Morphing.MorphTimer > 0 && Owner.IsRole(RoleType.Morphing))
                {
                    var target = Morphing.MorphTarget;
                    if (target?.Data != null)
                    {
                        c = Palette.ShadowColors[target.Data.DefaultOutfit.ColorId];
                    }
                }
                else if (Camouflager.CamouflageTimer > 0)
                {
                    c = (Color)Palette.PlayerColors[6];
                }
            }

            c.a = Mathf.Clamp01(1f - p);
            SpriteRenderer.color = c;

            if (p == 1f && FootprintObj != null)
            {
                UnityEngine.Object.Destroy(FootprintObj);
                Footprints.Remove(this);
            }
        })));
    }
}