using InnerNet;
using Object = UnityEngine.Object;

namespace RebuildUs.Objects;

public sealed class AdditionalVents
{
    public static List<AdditionalVents> AllVents = [];
    public static bool Flag;
    public Vent Vent;

    public AdditionalVents(Vector3 p)
    {
        // Create the vent
        var referenceVent = Object.FindObjectOfType<Vent>();
        Vent = Object.Instantiate(referenceVent);
        Vent.transform.position = p;
        Vent.Left = null;
        Vent.Right = null;
        Vent.Center = null;
        var tmp = MapUtilities.CachedShipStatus.AllVents[0];
        Vent.EnterVentAnim = tmp.EnterVentAnim;
        Vent.ExitVentAnim = tmp.ExitVentAnim;
        Vent.Offset = new(0f, 0.25f, 0f);

        var maxId = -1;
        var allVents = MapUtilities.CachedShipStatus.AllVents;
        for (var i = 0; i < allVents.Length; i++)
        {
            if (allVents[i].Id > maxId)
                maxId = allVents[i].Id;
        }

        Vent.Id = maxId + 1; // Make sure we have a unique id

        var newVents = new Vent[allVents.Length + 1];
        for (var i = 0; i < allVents.Length; i++) newVents[i] = allVents[i];
        newVents[newVents.Length - 1] = Vent;
        MapUtilities.CachedShipStatus.AllVents = newVents;

        Vent.gameObject.SetActive(true);
        Vent.name = "AdditionalVent_" + Vent.Id;
        AllVents.Add(this);
    }

    public static void AddAdditionalVents()
    {
        if (Flag) return;
        Flag = true;
        if (AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started) return;
        Logger.LogMessage("AddAdditionalVents");

        // Polusにベントを追加する
        if (Helpers.IsPolus && CustomOptionHolder.PolusAdditionalVents.GetBool())
        {
            AdditionalVents vents1 = new(new(36.54f, -21.77f, PlayerControl.LocalPlayer.transform.position.z + 1f)); // Specimen
            AdditionalVents vents2 = new(new(16.64f, -2.46f, PlayerControl.LocalPlayer.transform.position.z + 1f)); // InitialSpawn
            AdditionalVents vents3 = new(new(26.67f, -17.54f, PlayerControl.LocalPlayer.transform.position.z + 1f)); // Vital
            vents1.Vent.Left = vents3.Vent; // Specimen - Vital
            vents2.Vent.Center = vents3.Vent; // InitialSpawn - Vital
            vents3.Vent.Right = vents1.Vent; // Vital - Specimen
            vents3.Vent.Left = vents2.Vent; // Vital - InitialSpawn
        }

        // AirShipにベントを追加する
        // if(PlayerControl.GameOptions.MapId == 4 && CustomOptionHolder.additionalVents.getBool()){
        //     AdditionalVents vents1 = new AdditionalVents(new Vector3(17.086f, 15.24f, PlayerControl.LocalPlayer.transform.position.z + 1f)); // MeetingRoom
        //     AdditionalVents vents2 = new AdditionalVents(new Vector3(19.137f, -11.32f, PlayerControl.LocalPlayer.transform.position.z + 1f)); // Electrical
        //     vents1.vent.Right = vents2.vent;
        //     vents2.vent.Left = vents1.vent;
        // }
    }

    public static void ClearAndReload()
    {
        Logger.LogMessage("additionalVentsClearAndReload");
        Flag = false;
        AllVents = [];
    }
}
