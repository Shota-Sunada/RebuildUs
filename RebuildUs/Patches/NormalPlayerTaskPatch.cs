namespace RebuildUs.Patches;

[HarmonyPatch]
public static class NormalPlayerTaskPatch
{
    private static int NumWireTask { get { return (int)CustomOptionHolder.NumWireTask.GetFloat(); } }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(NormalPlayerTask), nameof(NormalPlayerTask.PickRandomConsoles), [typeof(TaskTypes), typeof(Il2CppStructArray<byte>)])]
    public static void PickRandomConsolesPostfix(NormalPlayerTask __instance, TaskTypes taskType, byte[] consoleIds)
    {
        if (taskType != TaskTypes.FixWiring || !CustomOptionHolder.RandomWireTask.GetBool()) return;
        List<Console> orgList = [.. ShipStatus.Instance.AllConsoles.Where((Console t) => t.TaskTypes.Contains(taskType))];
        List<Console> list = [.. orgList];

        __instance.MaxStep = NumWireTask;
        __instance.Data = new byte[NumWireTask];
        for (int i = 0; i < __instance.Data.Length; i++)
        {
            if (list.Count == 0)
            {
                list = [.. orgList];
            }
            int index = UnityEngine.Random.Range(0, list.Count);
            __instance.Data[i] = (byte)list[index].ConsoleId;
            list.RemoveAt(index);
        }
    }
}