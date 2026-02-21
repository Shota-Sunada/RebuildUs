using InnerNet;
using Object = UnityEngine.Object;

namespace RebuildUs.Objects;

internal sealed class AdditionalVents
{
    private static List<AdditionalVents> _allVents = [];
    private static bool _flag;
    private readonly Vent _vent;

    private AdditionalVents(Vector3 p)
    {
        // Create the vent
        Vent referenceVent = Object.FindObjectOfType<Vent>();
        _vent = Object.Instantiate(referenceVent);
        _vent.transform.position = p;
        _vent.Left = null;
        _vent.Right = null;
        _vent.Center = null;
        Vent tmp = MapUtilities.CachedShipStatus.AllVents[0];
        _vent.EnterVentAnim = tmp.EnterVentAnim;
        _vent.ExitVentAnim = tmp.ExitVentAnim;
        _vent.Offset = new(0f, 0.25f, 0f);

        int maxId = -1;
        Il2CppReferenceArray<Vent> allVents = MapUtilities.CachedShipStatus.AllVents;
        for (int i = 0; i < allVents.Length; i++)
            if (allVents[i].Id > maxId)
                maxId = allVents[i].Id;

        _vent.Id = maxId + 1; // Make sure we have a unique id

        Vent[] newVents = new Vent[allVents.Length + 1];
        for (int i = 0; i < allVents.Length; i++) newVents[i] = allVents[i];
        newVents[^1] = _vent;
        MapUtilities.CachedShipStatus.AllVents = newVents;

        _vent.gameObject.SetActive(true);
        _vent.name = "AdditionalVent_" + _vent.Id;
        _allVents.Add(this);
    }

    internal static void AddAdditionalVents()
    {
        if (_flag) return;
        _flag = true;
        if (AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started) return;
        Logger.LogMessage("AddAdditionalVents");

        // Polusにベントを追加する
        if (!Helpers.IsPolus || !CustomOptionHolder.PolusAdditionalVents.GetBool()) return;
        AdditionalVents vents1 = new(new(36.54f, -21.77f, PlayerControl.LocalPlayer.transform.position.z + 1f)); // Specimen
        AdditionalVents vents2 = new(new(16.64f, -2.46f, PlayerControl.LocalPlayer.transform.position.z + 1f)); // InitialSpawn
        AdditionalVents vents3 = new(new(26.67f, -17.54f, PlayerControl.LocalPlayer.transform.position.z + 1f)); // Vital
        vents1._vent.Left = vents3._vent; // Specimen - Vital
        vents2._vent.Center = vents3._vent; // InitialSpawn - Vital
        vents3._vent.Right = vents1._vent; // Vital - Specimen
        vents3._vent.Left = vents2._vent; // Vital - InitialSpawn

        // AirShipにベントを追加する
        // if(PlayerControl.GameOptions.MapId == 4 && CustomOptionHolder.additionalVents.getBool()){
        //     AdditionalVents vents1 = new AdditionalVents(new Vector3(17.086f, 15.24f, PlayerControl.LocalPlayer.transform.position.z + 1f)); // MeetingRoom
        //     AdditionalVents vents2 = new AdditionalVents(new Vector3(19.137f, -11.32f, PlayerControl.LocalPlayer.transform.position.z + 1f)); // Electrical
        //     vents1.vent.Right = vents2.vent;
        //     vents2.vent.Left = vents1.vent;
        // }
    }

    internal static void ClearAndReload()
    {
        Logger.LogMessage("additionalVentsClearAndReload");
        _flag = false;
        _allVents = [];
    }
}