using System.Collections.Concurrent;
using Il2CppInterop.Runtime.Attributes;

namespace RebuildUs.Objects;

internal sealed class FootprintHolder : MonoBehaviour
{
    private static readonly float UpdateDt = 0.10f;
    private readonly List<Footprint> _activeFootprints = [];

    private readonly ConcurrentBag<Footprint> _pool = [];
    private readonly List<Footprint> _toRemove = [];

    static FootprintHolder()
    {
        ClassInjector.RegisterTypeInIl2Cpp<FootprintHolder>();
    }

    internal FootprintHolder(IntPtr ptr) : base(ptr) { }

    internal static FootprintHolder Instance
    {
        get => field ? field : field = new GameObject("FootprintHolder").AddComponent<FootprintHolder>();
        set;
    }

    private static bool AnonymousFootprints
    {
        get => Detective.AnonymousFootprints;
    }

    private static float FootprintDuration
    {
        get => Detective.FootprintDuration;
    }

    private void Start()
    {
        InvokeRepeating(nameof(FootprintUpdate), UpdateDt, UpdateDt);
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    [HideFromIl2Cpp]
    internal void MakeFootprint(PlayerControl player)
    {
        if (!_pool.TryTake(out Footprint print)) print = new();

        print.Lifetime = FootprintDuration;

        Vector3 pos = player.transform.position;
        pos.z = (pos.y / 1000f) + 0.001f;
        print.Transform.SetPositionAndRotation(pos, Quaternion.EulerRotation(0, 0, RebuildUs.Rnd.Next(0, 360)));
        print.GameObject.SetActive(true);
        print.Owner = player;
        print.Data = player.Data;
        _activeFootprints.Add(print);
    }

    private void FootprintUpdate()
    {
        float dt = UpdateDt;
        _toRemove.Clear();
        foreach (Footprint activeFootprint in _activeFootprints)
        {
            float p = activeFootprint.Lifetime / FootprintDuration;

            if (activeFootprint.Lifetime <= 0)
            {
                _toRemove.Add(activeFootprint);
                continue;
            }

            Color color;
            if (AnonymousFootprints || Camouflager.CamouflageTimer > 0 || Helpers.MushroomSabotageActive())
                color = Palette.PlayerColors[6];
            else if (activeFootprint.Owner.IsRole(RoleType.Morphing) && Morphing.MorphTimer > 0 && Morphing.MorphTarget && Morphing.MorphTarget.Data != null)
                color = Palette.PlayerColors[Morphing.MorphTarget.Data.DefaultOutfit.ColorId];
            else
                color = Palette.PlayerColors[activeFootprint.Data.DefaultOutfit.ColorId];

            color.a = Math.Clamp(p, 0f, 1f);
            activeFootprint.Renderer.color = color;

            activeFootprint.Lifetime -= dt;
        }

        foreach (Footprint footprint in _toRemove)
        {
            footprint.GameObject.SetActive(false);
            _activeFootprints.Remove(footprint);
            _pool.Add(footprint);
        }
    }

    private sealed class Footprint
    {
        internal readonly GameObject GameObject;
        internal readonly SpriteRenderer Renderer;
        internal readonly Transform Transform;
        internal NetworkedPlayerInfo Data;
        internal float Lifetime;
        internal PlayerControl Owner;

        internal Footprint()
        {
            GameObject = new("Footprint") { layer = 8 };
            Transform = GameObject.transform;
            Renderer = GameObject.AddComponent<SpriteRenderer>();
            Renderer.sprite = AssetLoader.Footprint;
            Renderer.color = Color.clear;
            GameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ELEVATOR_MOVER);
        }
    }
}