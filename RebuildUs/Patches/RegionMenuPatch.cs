// Adapted from https://github.com/MoltenMods/Unify
/*
MIT License

Copyright (c) 2021 Daemon

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using UnityEngine.Events;
using UnityEngine.UI;

namespace RebuildUs.Patches;

[HarmonyPatch(typeof(RegionMenu), nameof(RegionMenu.Open))]
public static class RegionMenuOpenPatch
{
    private static GameObject IpField;
    private static GameObject PortField;

    public static void Postfix(RegionMenu __instance)
    {
        var template = GameObject.Find("NormalMenu/JoinGameButton/JoinGameMenu/GameIdText");
        if (template == null) return;

        if (IpField == null || IpField.gameObject == null)
        {
            IpField = UnityEngine.Object.Instantiate(template.gameObject, __instance.transform);
            IpField.gameObject.name = "IpTextBox";
            var arrow = IpField.transform.FindChild("arrowEnter");
            if (arrow == null || arrow.gameObject == null) return;
            UnityEngine.Object.DestroyImmediate(arrow.gameObject);

            IpField.transform.localPosition = new Vector3(0, -1f, -100f);

            var ipTextBox = IpField.GetComponent<TextBoxTMP>();
            ipTextBox.characterLimit = 30;
            ipTextBox.AllowSymbols = true;
            ipTextBox.ForceUppercase = false;
            ipTextBox.SetText(RebuildUs.Ip.Value);
            __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>((p) =>
            {
                ipTextBox.outputText.SetText(RebuildUs.Ip.Value);
                ipTextBox.SetText(RebuildUs.Ip.Value);
            })));

            ipTextBox.ClearOnFocus = false;
            ipTextBox.OnEnter = ipTextBox.OnChange = new Button.ButtonClickedEvent();
            ipTextBox.OnFocusLost = new Button.ButtonClickedEvent();
            ipTextBox.OnChange.AddListener((UnityAction)OnEnterOrIpChange);
            ipTextBox.OnFocusLost.AddListener((UnityAction)OnFocusLost);

        }

        if (PortField == null || PortField.gameObject == null)
        {
            PortField = UnityEngine.Object.Instantiate(template.gameObject, __instance.transform);
            PortField.gameObject.name = "PortTextBox";
            var arrow = PortField.transform.FindChild("arrowEnter");
            if (arrow == null || arrow.gameObject == null) return;
            UnityEngine.Object.DestroyImmediate(arrow.gameObject);

            PortField.transform.localPosition = new Vector3(0, -1.75f, -100f);

            var portTextBox = PortField.GetComponent<TextBoxTMP>();
            portTextBox.characterLimit = 5;
            portTextBox.SetText(RebuildUs.Port.Value.ToString());
            __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>((p) =>
            {
                portTextBox.outputText.SetText(RebuildUs.Port.Value.ToString());
                portTextBox.SetText(RebuildUs.Port.Value.ToString());
            })));

            portTextBox.ClearOnFocus = false;
            portTextBox.OnEnter = portTextBox.OnChange = new Button.ButtonClickedEvent();
            portTextBox.OnFocusLost = new Button.ButtonClickedEvent();
            portTextBox.OnChange.AddListener((UnityAction)OnEnterOrPortFieldChange);
            portTextBox.OnFocusLost.AddListener((UnityAction)OnFocusLost);
        }

        void OnEnterOrPortFieldChange()
        {
            var portTextBox = PortField.GetComponent<TextBoxTMP>();
            if (ushort.TryParse(portTextBox.text, out ushort port))
            {
                RebuildUs.Port.Value = port;
                portTextBox.outputText.color = Color.white;
            }
            else
            {
                portTextBox.outputText.color = Color.red;
            }
        }

        void OnEnterOrIpChange()
        {
            RebuildUs.Ip.Value = IpField.GetComponent<TextBoxTMP>().text;
        }

        void OnFocusLost()
        {
            RebuildUs.UpdateRegions();
            __instance.ChooseOption(ServerManager.DefaultRegions[ServerManager.DefaultRegions.Length - 1]);
        }
    }
}