namespace RebuildUs.Modules;

internal static class Airship
{
    internal static void AwakePostfix()
    {
        if (!Helpers.IsAirship) return;

        if (CustomOptionHolder.AirshipAdditionalWireTask.GetBool())
        {
            ActivateWiring("task_wiresHallway2", 2);
            ActivateWiring("task_electricalside2", 3).Room = SystemTypes.Armory;
            ActivateWiring("task_wireShower", 4);
            ActivateWiring("taks_wiresLounge", 5);
            ActivateWiring("panel_wireHallwayL", 6);
            ActivateWiring("task_wiresStorage", 7);
            ActivateWiring("task_electricalSide", 8).Room = SystemTypes.VaultRoom;
            ActivateWiring("task_wiresMeeting", 9);
        }

        if (CustomOptionHolder.AirshipOptimize.GetBool())
        {
            var obj = MapUtilities.CachedShipStatus.FastRooms[SystemTypes.GapRoom].gameObject;
            // 昇降機右に影を追加
            var oneWayShadow = obj.transform.FindChild("Shadow").FindChild("LedgeShadow").GetComponent<OneWayShadows>();
            oneWayShadow.enabled = false;
            if (PlayerControl.LocalPlayer.IsTeamImpostor())
            {
                oneWayShadow.gameObject.SetActive(false);
            }

            GameObject fence = new("ModFence")
            {
                layer = LayerMask.NameToLayer("Ship"),
            };
            fence.transform.SetParent(obj.transform);
            fence.transform.localPosition = new(4.2f, 0.15f, 0.5f);
            fence.transform.localScale = new(1f, 1f, 1f);
            fence.SetActive(true);
            var collider = fence.AddComponent<EdgeCollider2D>();
            collider.points = new Vector2[]
            {
                new(1.5f, -0.2f), new(-1.5f, -0.2f), new(-1.5f, 1.5f),
            };
            collider.enabled = true;
            var renderer = fence.AddComponent<SpriteRenderer>();
            renderer.sprite = AssetLoader.AirshipFence;

            // GameObject pole = new("DownloadPole")
            // {
            //     layer = LayerMask.NameToLayer("Ship")
            // };
            // pole.transform.SetParent(obj.transform);
            // pole.transform.localPosition = new Vector3(4.1f, 0.75f, 0.8f);
            // pole.transform.localScale = new Vector3(1f, 1f, 1f);
            // renderer = pole.AddComponent<SpriteRenderer>();
            // renderer.sprite = AssetLoader.AirshipDownloadG;

            var panel = obj.transform.FindChild("panel_data");
            panel.localPosition = new(4.52f, -3.95f, 0.1f);
            // panel.gameObject.GetComponent<Console>().usableDistance = 0.9f;
        }

        {
            // Add Ladder
            var meetingRoom = MapUtilities.CachedShipStatus.FastRooms[SystemTypes.MeetingRoom].gameObject;
            var gapRoom = MapUtilities.CachedShipStatus.FastRooms[SystemTypes.GapRoom].gameObject;

            var meetingRenderers = meetingRoom.GetComponentsInChildren<SpriteRenderer>();
            GameObject ladder = null;
            foreach (var renderer in meetingRenderers)
            {
                if (renderer.name != "ladder_meeting")
                {
                    continue;
                }
                ladder = renderer.gameObject;
                break;
            }

            if (CustomOptionHolder.AirshipAdditionalLadder.GetBool())
            {
                // 梯子追加
                if (ladder != null)
                {
                    var newLadder = UnityObject.Instantiate(ladder, ladder.transform.parent);
                    var ladders = newLadder.GetComponentsInChildren<Ladder>();
                    var id = 100;
                    foreach (var l in ladders)
                    {
                        if (l.name == "LadderBottom")
                        {
                            l.gameObject.SetActive(false);
                        }
                        l.Id = (byte)id;
                        FastDestroyableSingleton<AirshipStatus>.Instance.Ladders.AddItem(l);
                        id++;
                    }

                    newLadder.transform.position = new(15.442f, 12.18f, 0.1f);
                    newLadder.GetComponentInChildren<SpriteRenderer>().sprite = AssetLoader.Ladder;
                }

                // 梯子の周りの影を消す
                foreach (var x in gapRoom.GetComponentsInChildren<EdgeCollider2D>())
                {
                    if (!(Math.Abs(x.points[0].x + 6.2984f) < 0.1))
                    {
                        continue;
                    }
                    UnityObject.Destroy(x);
                    break;
                }

                EdgeCollider2D collider = null;
                foreach (var x in meetingRoom.GetComponentsInChildren<EdgeCollider2D>())
                {
                    if (x.pointCount != 46)
                    {
                        continue;
                    }
                    collider = x;
                    break;
                }

                if (collider != null)
                {
                    Il2CppSystem.Collections.Generic.List<Vector2> points = new();
                    var newCollider = collider.gameObject.AddComponent<EdgeCollider2D>();
                    var newCollider2 = collider.gameObject.AddComponent<EdgeCollider2D>();
                    points.Add(collider.points[45]);
                    points.Add(collider.points[44]);
                    points.Add(collider.points[43]);
                    points.Add(collider.points[42]);
                    points.Add(collider.points[41]);
                    newCollider.SetPoints(points);
                    points.Clear();
                    for (var i = 0; i < 41; i++)
                    {
                        points.Add(collider.points[i]);
                    }

                    newCollider2.SetPoints(points);
                    UnityObject.DestroyObject(collider);
                }

                // 梯子の背景を変更
                SpriteRenderer side = null;
                foreach (var r in meetingRenderers)
                {
                    if (r.name != "meeting_side")
                    {
                        continue;
                    }
                    side = r;
                    break;
                }

                if (side != null)
                {
                    var bg = UnityObject.Instantiate(side, side.transform.parent);
                    bg.sprite = AssetLoader.LadderBackground;
                    bg.transform.localPosition = new(9.57f, -3.355f, 4.9f);
                }
            }

            if (!CustomOptionHolder.AirshipOneWayLadder.GetBool())
            {
                return;
            }

            {
                if (ladder == null)
                {
                    return;
                }
                foreach (var l in ladder.GetComponentsInChildren<Ladder>())
                {
                    if (l.name != "LadderTop")
                    {
                        continue;
                    }
                    l.gameObject.SetActive(false);
                    break;
                }
            }
        }
    }

    internal static Console ActivateWiring(string consoleName, int consoleId)
    {
        var console = ActivateConsole(consoleName);

        if (console == null)
        {
            Logger.LogError($"Console \"{consoleName}\" is null", "ActivateWiring");
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