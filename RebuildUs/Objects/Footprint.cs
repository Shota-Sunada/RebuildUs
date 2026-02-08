using System.Collections.Concurrent;
using Il2CppInterop.Runtime.Attributes;

namespace RebuildUs.Objects;

public sealed class FootprintHolder : MonoBehaviour
{
    private static FootprintHolder _instance;

    private static readonly float UPDATE_DT = 0.10f;
    private readonly List<Footprint> _activeFootprints = [];

    private readonly ConcurrentBag<Footprint> _pool = [];
    private readonly List<Footprint> _toRemove = [];

    static FootprintHolder()
    {
        ClassInjector.RegisterTypeInIl2Cpp<FootprintHolder>();
    }

    public FootprintHolder(IntPtr ptr) : base(ptr) { }

    public static FootprintHolder Instance
    {
        get => _instance ? _instance : _instance = new GameObject("FootprintHolder").AddComponent<FootprintHolder>();
        set => _instance = value;
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
        InvokeRepeating(nameof(FootprintUpdate), UPDATE_DT, UPDATE_DT);
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    [HideFromIl2Cpp]
    public void MakeFootprint(PlayerControl player)
    {
        if (!_pool.TryTake(out var print)) print = new();

        print.Lifetime = FootprintDuration;

        var pos = player.transform.position;
        pos.z = (pos.y / 1000f) + 0.001f;
        print.Transform.SetPositionAndRotation(pos, Quaternion.EulerRotation(0, 0, RebuildUs.Instance.Rnd.Next(0, 360)));
        print.GameObject.SetActive(true);
        print.Owner = player;
        print.Data = player.Data;
        _activeFootprints.Add(print);
    }

    private void FootprintUpdate()
    {
        var dt = UPDATE_DT;
        _toRemove.Clear();
        foreach (var activeFootprint in _activeFootprints)
        {
            var p = activeFootprint.Lifetime / FootprintDuration;

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

        foreach (var footprint in _toRemove)
        {
            footprint.GameObject.SetActive(false);
            _activeFootprints.Remove(footprint);
            _pool.Add(footprint);
        }
    }

    private class Footprint
    {
        public readonly GameObject GameObject;
        public readonly SpriteRenderer Renderer;
        public readonly Transform Transform;
        public NetworkedPlayerInfo Data;
        public float Lifetime;
        public PlayerControl Owner;

        public Footprint()
        {
            GameObject = new("Footprint") { layer = 8 };
            Transform = GameObject.transform;
            Renderer = GameObject.AddComponent<SpriteRenderer>();
            Renderer.sprite = AssetLoader.Footprint;
            Renderer.color = Color.clear;
            GameObject.AddSubmergedComponent(SubmergedCompatibility.ELEVATOR_MOVER);
        }
    }
}
