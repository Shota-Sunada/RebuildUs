using System.Collections.Generic;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using RebuildUs.Modules;
using UnityEngine;
using TMPro;

namespace RebuildUs.Patches
{
    [HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Start))]
    public static class OptionsMenuBehaviourPatch
    {
        private static void SetLayerRecursively(GameObject obj, int layer)
        {
            obj.layer = layer;
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                SetLayerRecursively(obj.transform.GetChild(i).gameObject, layer);
            }
        }

        public static void Postfix(OptionsMenuBehaviour __instance)
        {
            int uiLayer = __instance.gameObject.layer;

            var tabs = new List<TabGroup>();
            for (int i = 0; i < __instance.Tabs.Count; i++)
            {
                tabs.Add(__instance.Tabs[i]);
            }

            GameObject serverTab = new GameObject("ServerTab");
            serverTab.transform.SetParent(__instance.transform);
            serverTab.transform.localScale = Vector3.one;
            serverTab.transform.localPosition = Vector3.zero;
            serverTab.layer = uiLayer;
            serverTab.SetActive(false);

            var newTabBtn = GameObject.Instantiate(tabs[1].gameObject, null).GetComponent<TabGroup>();
            tabs.Add(newTabBtn);

            newTabBtn.gameObject.name = "ServerTabButton";
            newTabBtn.transform.SetParent(tabs[0].transform.parent);
            newTabBtn.transform.localScale = Vector3.one;
            newTabBtn.Content = serverTab;

            var textObj = newTabBtn.transform.FindChild("Text_TMP").gameObject;
            var translator = textObj.GetComponent<TextTranslatorTMP>();
            if (translator != null) translator.enabled = false;
            textObj.GetComponent<TextMeshPro>().text = "Server";

            var passiveButton = newTabBtn.gameObject.GetComponent<PassiveButton>();
            passiveButton.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            System.Action onClickAction = () => { __instance.OpenTabGroup(tabs.Count - 1); };
            passiveButton.OnClick.AddListener((UnityEngine.Events.UnityAction)onClickAction);

            float y = tabs[0].transform.localPosition.y;
            float z = tabs[0].transform.localPosition.z;
            for (int i = 0; i < tabs.Count; i++)
            {
                tabs[i].transform.localPosition = new Vector3(1.6f * (i - (tabs.Count - 1) / 2f), y, z);
            }

            var newArray = new TabGroup[tabs.Count];
            for (int i = 0; i < tabs.Count; i++) newArray[i] = tabs[i];
            __instance.Tabs = new Il2CppReferenceArray<TabGroup>(newArray);

            TextBoxTMP templateTextBox = null;
            var allTextBoxes = UnityEngine.Object.FindObjectsOfType<TextBoxTMP>(true);
            for (int i = 0; i < allTextBoxes.Count; i++)
            {
                if (allTextBoxes[i] != null && allTextBoxes[i].gameObject != null)
                {
                    var bg = allTextBoxes[i].Background;
                    if (bg != null && bg.sprite != null)
                    {
                        templateTextBox = allTextBoxes[i];
                        break;
                    }
                }
            }

            if (templateTextBox == null)
            {
                Logger.LogError("No valid TextBoxTMP found!", "OptionsMenu");
                return;
            }

            // IP Label
            var ipLabelGo = new GameObject("IpLabel");
            ipLabelGo.layer = uiLayer;
            ipLabelGo.transform.SetParent(serverTab.transform);
            ipLabelGo.transform.localPosition = new Vector3(-2.8f, 1f, -1f);
            var ipLabelTxt = ipLabelGo.AddComponent<TextMeshPro>();
            ipLabelTxt.text = "Custom Server IP";
            ipLabelTxt.fontSize = 2f;
            ipLabelTxt.alignment = TextAlignmentOptions.Right;

            // IP Input
            var ipTextGo = GameObject.Instantiate(templateTextBox.gameObject, serverTab.transform);
            ipTextGo.name = "IpInput";
            ipTextGo.transform.localPosition = new Vector3(0.5f, 1.25f, -1.5f);
            ipTextGo.transform.localScale = Vector3.one;
            SetLayerRecursively(ipTextGo, uiLayer);
            ipTextGo.SetActive(true);

            var ipTextBox = ipTextGo.GetComponent<TextBoxTMP>();
            // Reset colliders to UI space
            var col1 = ipTextBox.GetComponent<BoxCollider2D>();
            if (col1) col1.enabled = true;

            ipTextBox.characterLimit = 30;
            ipTextBox.IpMode = true;
            ipTextBox.AllowSymbols = true;
            ipTextBox.text = global::RebuildUs.RebuildUs.Ip.Value;
            ipTextBox.SetText(global::RebuildUs.RebuildUs.Ip.Value);
            ipTextBox.OnChange = new UnityEngine.UI.Button.ButtonClickedEvent();
            System.Action ipChangeAction = () =>
            {
                global::RebuildUs.RebuildUs.Ip.Value = ipTextBox.text;
                global::RebuildUs.RebuildUs.UpdateRegions();
            };
            ipTextBox.OnChange.AddListener((UnityEngine.Events.UnityAction)ipChangeAction);

            // Port Label
            var portLabelGo = new GameObject("PortLabel");
            portLabelGo.layer = uiLayer;
            portLabelGo.transform.SetParent(serverTab.transform);
            portLabelGo.transform.localPosition = new Vector3(-2.8f, 0f, -1f);
            var portLabelTxt = portLabelGo.AddComponent<TextMeshPro>();
            portLabelTxt.text = "Custom Server Port";
            portLabelTxt.fontSize = 2f;
            portLabelTxt.alignment = TextAlignmentOptions.Right;

            // Port Input
            var portTextGo = GameObject.Instantiate(templateTextBox.gameObject, serverTab.transform);
            portTextGo.name = "PortInput";
            portTextGo.transform.localPosition = new Vector3(0.5f, 0.25f, -1.5f);
            portTextGo.transform.localScale = Vector3.one;
            SetLayerRecursively(portTextGo, uiLayer);
            portTextGo.SetActive(true);

            var portTextBox = portTextGo.GetComponent<TextBoxTMP>();
            var col2 = portTextBox.GetComponent<BoxCollider2D>();
            if (col2) col2.enabled = true;

            portTextBox.characterLimit = 10;
            portTextBox.AllowSymbols = true;
            portTextBox.text = global::RebuildUs.RebuildUs.Port.Value.ToString();
            portTextBox.SetText(global::RebuildUs.RebuildUs.Port.Value.ToString());
            portTextBox.OnChange = new UnityEngine.UI.Button.ButtonClickedEvent();
            System.Action portChangeAction = () =>
            {
                if (ushort.TryParse(portTextBox.text, out ushort newPort))
                {
                    global::RebuildUs.RebuildUs.Port.Value = newPort;
                    global::RebuildUs.RebuildUs.UpdateRegions();
                }
            };
            portTextBox.OnChange.AddListener((UnityEngine.Events.UnityAction)portChangeAction);

            Logger.LogInfo("Server UI generated successfully.", "OptionsMenu");
        }
    }
}
