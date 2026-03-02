namespace RebuildUs.Modules;

internal static class Airship
{
    internal static Console ActivateWiring(string consoleName, int consoleId)
    {
        var console = ActivateConsole(consoleName);

        if (console == null)
        {
            Logger.LogError($"consoleName \"{consoleName}\" is null", "ActivateWiring");
            return null;
        }

        var hasWiringText = false;
        foreach (var taskType in console.TaskTypes)
        {
            if (taskType != TaskTypes.FixWiring)
            {
                continue;
            }
            hasWiringText = true;
            break;
        }

        if (!hasWiringText)
        {
            var newTasks = new TaskTypes[console.TaskTypes.Length + 1];
            for (var i = 0; i < console.TaskTypes.Length; i++)
            {
                newTasks[i] = console.TaskTypes[i];
            }
            newTasks[console.TaskTypes.Length] = TaskTypes.FixWiring;
            console.TaskTypes = newTasks;
        }

        console.ConsoleId = consoleId;
        return console;
    }

    private static Console ActivateConsole(string objectName)
    {
        var obj = GameObject.Find(objectName);
        if (obj == null)
        {
            Logger.LogError($"Object \"{objectName}\" was not found!", "ActivateConsole");
            return null;
        }

        obj.layer = LayerMask.NameToLayer("ShortObjects");
        var console = obj.GetComponent<Console>();
        var button = obj.GetComponent<PassiveButton>();
        var collider = obj.GetComponent<CircleCollider2D>();
        if (!console)
        {
            console = obj.AddComponent<Console>();
            console.checkWalls = true;
            console.usableDistance = 0.7f;
            console.TaskTypes = Array.Empty<TaskTypes>();
            console.ValidTasks = new(0);

            var oldConsoles = MapUtilities.CachedShipStatus.AllConsoles;
            Il2CppReferenceArray<Console> newConsoles = new(oldConsoles.Length + 1);
            for (var i = 0; i < oldConsoles.Length; i++)
            {
                newConsoles[i] = oldConsoles[i];
            }
            newConsoles[oldConsoles.Length] = console;
            MapUtilities.CachedShipStatus.AllConsoles = newConsoles;
        }

        if (console.Image == null)
        {
            console.Image = obj.GetComponent<SpriteRenderer>();
            console.Image.material = new(MapUtilities.CachedShipStatus.AllConsoles[0].Image.material);
        }

        if (!button)
        {
            button = obj.AddComponent<PassiveButton>();
            button.OnMouseOut = new();
            button.OnMouseOver = new();
            button._CachedZ_k__BackingField = 0.1f;
            button.CachedZ = 0.1f;
        }

        if (collider)
        {
            return console;
        }
        collider = obj.AddComponent<CircleCollider2D>();
        collider.radius = 0.4f;
        collider.isTrigger = true;

        return console;
    }
}