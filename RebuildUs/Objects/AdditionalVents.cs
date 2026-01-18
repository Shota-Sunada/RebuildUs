namespace RebuildUs.Objects;

public class AdditionalVents
{
    public Vent Vent;
    public static System.Collections.Generic.List<AdditionalVents> AllVents = new();
    public static bool Flag = false;
    public AdditionalVents(Vector3 p)
    {
        // Create the vent
        var referenceVent = UnityEngine.Object.FindObjectOfType<Vent>();
        Vent = UnityEngine.Object.Instantiate<Vent>(referenceVent);
        Vent.transform.position = p;
        Vent.Left = null;
        Vent.Right = null;
        Vent.Center = null;
        Vent tmp = MapUtilities.CachedShipStatus.AllVents[0];
        Vent.EnterVentAnim = tmp.EnterVentAnim;
        Vent.ExitVentAnim = tmp.ExitVentAnim;
        Vent.Offset = new Vector3(0f, 0.25f, 0f);
        Vent.Id = MapUtilities.CachedShipStatus.AllVents.Select(x => x.Id).Max() + 1; // Make sure we have a unique id
        var allVentsList = MapUtilities.CachedShipStatus.AllVents.ToList();
        allVentsList.Add(Vent);
        MapUtilities.CachedShipStatus.AllVents = allVentsList.ToArray();
        Vent.gameObject.SetActive(true);
        Vent.name = "AdditionalVent_" + Vent.Id;
        AllVents.Add(this);
    }

    public static void AddAdditionalVents()
    {
        if (AdditionalVents.Flag) return;
        AdditionalVents.Flag = true;
        if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
        System.Console.WriteLine("AddAdditionalVents");

        // Polusにベントを追加する
        if (Helpers.IsPolus && CustomOptionHolder.PolusAdditionalVents.GetBool())
        {
            AdditionalVents vents1 = new(new Vector3(36.54f, -21.77f, PlayerControl.LocalPlayer.transform.position.z + 1f)); // Specimen
            AdditionalVents vents2 = new(new Vector3(16.64f, -2.46f, PlayerControl.LocalPlayer.transform.position.z + 1f)); // InitialSpawn
            AdditionalVents vents3 = new(new Vector3(26.67f, -17.54f, PlayerControl.LocalPlayer.transform.position.z + 1f)); // Vital
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
        System.Console.WriteLine("additionalVentsClearAndReload");
        Flag = false;
        AllVents = new List<AdditionalVents>();
    }
}