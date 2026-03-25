namespace RebuildUs.Objects;

internal sealed class Garlic
{
    private static CustomButton _garlicButton;
    internal static List<Garlic> Garlics = [];
    private readonly GameObject _background;

    internal readonly GameObject GarlicObject;

    [RegisterCustomButton]
    internal static void MakeButtons(HudManager hm)
    {
        _garlicButton = new(
            () =>
            {
                Roles.Impostor.Vampire.PlayerPlacedGarlic = true;
                var pos = PlayerControl.LocalPlayer.transform.position;

                using (RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.PlaceGarlic))
                {
                    sender.Write(pos.x);
                    sender.Write(pos.y);
                }

                RPCProcedure.PlaceGarlic(pos.x, pos.y);
            },
            () =>
            {
                return !Roles.Impostor.Vampire.PlayerPlacedGarlic && PlayerControl.LocalPlayer.IsAlive() && Roles.Impostor.Vampire.GarlicsActive && !PlayerControl.LocalPlayer.IsGm();
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove && !Roles.Impostor.Vampire.PlayerPlacedGarlic;
            },
            () => { },
            AssetLoader.GarlicButton,
            ButtonPosition.Layout,
            hm,
            hm.UseButton,
            AbilitySlot.CommonAbilityPrimary,
            true,
            Tr.Get(TrKey.GarlicText));
    }

    [SetCustomButtonTimer]
    internal static void SetButtonCooldowns()
    {
        _garlicButton.Timer = _garlicButton.MaxTimer = 0f;
    }

    internal Garlic(Vector2 p)
    {
        GarlicObject = new("Garlic")
        {
            layer = 11,
        };
        GarlicObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ELEVATOR_MOVER);
        _background = new("Background")
        {
            layer = 11,
        };
        _background.transform.SetParent(GarlicObject.transform);
        Vector3 position = new(p.x, p.y, p.y / 1000 + 0.001f); // just behind player
        GarlicObject.transform.position = position;
        _background.transform.localPosition = new(0, 0, -1f); // before player

        var garlicRenderer = GarlicObject.AddComponent<SpriteRenderer>();
        garlicRenderer.sprite = AssetLoader.Garlic;
        var backgroundRenderer = _background.AddComponent<SpriteRenderer>();
        backgroundRenderer.sprite = AssetLoader.GarlicBackground;

        GarlicObject.SetActive(true);
        Garlics.Add(this);
    }

    internal static void ClearGarlics()
    {
        Garlics = [];
    }

    internal static void UpdateAll()
    {
        foreach (var t in Garlics)
        {
            t?.Update();
        }
    }

    private void Update()
    {
        _background?.transform.Rotate(Vector3.forward * 6 * Time.fixedDeltaTime);
    }
}