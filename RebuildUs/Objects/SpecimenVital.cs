namespace RebuildUs.Objects;

internal abstract class SpecimenVital
{
    private static readonly Vector3 Pos = new(35.39f, -22.10f, 1.0f);
    private static bool _flag;

    internal static void ClearAndReload()
    {
        _flag = false;
    }

    internal static void MoveVital()
    {
        if (_flag) return;
        if (!Helpers.IsPolus || !CustomOptionHolder.PolusSpecimenVital.GetBool()) return;
        GameObject panel = GameObject.Find("panel_vitals");
        if (panel == null) return;
        Transform transform = panel.GetComponent<Transform>();
        transform.SetPositionAndRotation(Pos, transform.rotation);
        _flag = true;
    }
}