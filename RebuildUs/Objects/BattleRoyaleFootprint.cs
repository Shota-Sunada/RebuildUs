namespace RebuildUs.Objects;

public class BattleRoyaleFootprint
{
    private static List<BattleRoyaleFootprint> footprints = [];
    private Color color;
    private readonly GameObject footprint;
    private readonly SpriteRenderer spriteRenderer;
    private Vector3 position;

    public BattleRoyaleFootprint(PlayerControl player, int whichColor)
    {

        switch (whichColor)
        {
            case 0:
                color = Palette.PlayerColors[(int)player.Data.DefaultOutfit.ColorId];
                break;
            case 1:
                color = Palette.PlayerColors[11];
                break;
            case 2:
                color = Palette.PlayerColors[13];
                break;
            case 3:
                color = Palette.PlayerColors[15];
                break;
        }

        footprint = new GameObject("BattleRoyaleFootprint");
        footprint.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
        if (GameOptionsManager.Instance.currentGameOptions.MapId == 6)
        {
            position = new Vector3(player.transform.position.x, player.transform.position.y, -0.5f);
        }
        else
        {
            position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z + 1f);
        }
        footprint.transform.position = position;
        footprint.transform.localPosition = position;
        footprint.transform.Rotate(0.0f, 0.0f, UnityEngine.Random.Range(0.0f, 360.0f));
        footprint.transform.SetParent(player.transform.parent);

        spriteRenderer = footprint.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = AssetLoader.BattleRoyaleFootprint;
        spriteRenderer.color = color;

        footprints.Add(this);
        footprint.SetActive(true);

        if (BattleRoyale.matchType == 2)
        {
            HudManager.Instance.StartCoroutine(Effects.Lerp(5, new Action<float>((p) =>
            {

                if (p == 1f && footprint != null)
                {
                    UnityEngine.Object.Destroy(footprint);
                    footprints.Remove(this);
                }
            })));
        }
        else
        {
            HudManager.Instance.StartCoroutine(Effects.Lerp(420, new Action<float>((p) =>
            {

                if (p == 1f && footprint != null)
                {
                    UnityEngine.Object.Destroy(footprint);
                    footprints.Remove(this);
                }
            })));
        }
    }
    public static void ClearBattleRoyaleFootprints()
    {
        footprints = [];
    }
}