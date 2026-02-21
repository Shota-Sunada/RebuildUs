using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace RebuildUs.Objects;

internal abstract class MapData
{
    private static ShipStatus _airShip;
    private static ShipStatus _skeldShip;
    private static ShipStatus _miraHq;
    internal static ShipStatus PolusShip;

    internal static void LoadAssets(AmongUsClient __instance)
    {
        AssetReference assetReference;
        AsyncOperationHandle<GameObject> asset;
        // Skeld
        if (!_skeldShip)
        {
            assetReference = __instance.ShipPrefabs[0];
            asset = assetReference.LoadAsset<GameObject>();
            asset.WaitForCompletion();
            _skeldShip = assetReference.Asset.Cast<GameObject>().GetComponent<ShipStatus>();
        }

        // Mira
        if (!_miraHq)
        {
            assetReference = __instance.ShipPrefabs[1];
            asset = assetReference.LoadAsset<GameObject>();
            asset.WaitForCompletion();
            _miraHq = assetReference.Asset.Cast<GameObject>().GetComponent<ShipStatus>();
        }

        // Polus
        if (!PolusShip)
        {
            assetReference = __instance.ShipPrefabs[2];
            asset = assetReference.LoadAsset<GameObject>();
            asset.WaitForCompletion();
            PolusShip = assetReference.Asset.Cast<GameObject>().GetComponent<ShipStatus>();
        }

        // AirShip
        if (!_airShip)
        {
            assetReference = __instance.ShipPrefabs[4];
            asset = assetReference.LoadAsset<GameObject>();
            asset.WaitForCompletion();
            _airShip = assetReference.Asset.Cast<GameObject>().GetComponent<ShipStatus>();
        }
    }
}