namespace RebuildUs.Objects;

public class Footprint
{
    private static readonly List<Footprint> footprints = [];
    private Color color;
    private readonly GameObject footprint;
    private readonly SpriteRenderer spriteRenderer;
    private readonly PlayerControl owner;
    private readonly bool anonymousFootprints;

    public Footprint(float footprintDuration, bool anonymousFootprints, PlayerControl player)
    {
        owner = player;
        this.anonymousFootprints = anonymousFootprints;
        color = anonymousFootprints ? (Color)Palette.PlayerColors[6] : (Color)Palette.PlayerColors[player.Data.DefaultOutfit.ColorId];

        footprint = new GameObject("Footprint");
        Vector3 position = new(player.transform.position.x, player.transform.position.y, player.transform.position.z + 1f);
        footprint.transform.position = position;
        footprint.transform.localPosition = position;
        footprint.transform.SetParent(player.transform.parent);

        footprint.transform.Rotate(0.0f, 0.0f, UnityEngine.Random.Range(0.0f, 360.0f));

        spriteRenderer = footprint.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = AssetLoader.Footprint;
        spriteRenderer.color = color;

        footprint.SetActive(true);
        footprints.Add(this);

        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(footprintDuration, new Action<float>((p) =>
        {
            var c = color;
            if (!anonymousFootprints && owner != null)
            {
                if (owner.IsRole(RoleType.Morphing))
                {
                    if (Morphing.morphTimer > 0 && Morphing.morphTarget?.Data != null)
                    {
                        c = Palette.ShadowColors[Morphing.morphTarget.Data.DefaultOutfit.ColorId];
                    }
                }
                else if (Camouflager.camouflageTimer > 0)
                {
                    c = Palette.PlayerColors[6];
                }
            }

            if (spriteRenderer) spriteRenderer.color = new Color(c.r, c.g, c.b, Mathf.Clamp01(1 - p));

            if (p == 1f && footprint != null)
            {
                UnityEngine.Object.Destroy(footprint);
                footprints.Remove(this);
            }
        })));
    }
}