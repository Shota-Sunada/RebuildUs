namespace RebuildUs.Objects;

public class SpecimenVital
{
    public static Vector3 Pos = new(35.39f, -22.10f, 1.0f);
    public static bool Flag;

    public static void ClearAndReload()
    {
        Flag = false;
    }

    public static void MoveVital()
    {
        if (Flag) return;
        if (Helpers.IsPolus && CustomOptionHolder.PolusSpecimenVital.GetBool())
        {
            var panel = GameObject.Find("panel_vitals");
            if (panel != null)
            {
                var transform = panel.GetComponent<Transform>();
                transform.SetPositionAndRotation(Pos, transform.rotation);
                Flag = true;
            }
        }
    }
}
