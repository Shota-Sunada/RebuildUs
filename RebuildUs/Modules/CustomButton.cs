using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace RebuildUs;

public struct ButtonPosition
{
    public Vector3 Offset;
    public bool UseLayout;

    public ButtonPosition(Vector3 offset)
    {
        Offset = offset;
        UseLayout = false;
    }

    public static ButtonPosition Layout
    {
        get => new() { UseLayout = true };
    }

    public static implicit operator ButtonPosition(Vector3 offset)
    {
        return new(offset);
    }
}

public sealed class CustomButton
{
    public static List<CustomButton> Buttons = [];

    public static bool StopCountdown = true;
    private readonly SpriteRenderer _keyBackground;

    private readonly SpriteRenderer _keyGuide;
    private readonly Action _onClick;
    private readonly Action _onEffectEnds;
    private readonly Action _onMeetingEnds;

    private string _lastButtonText = "";

    private bool _lastIsActive;
    public ActionButton ActionButton;
    public string ButtonText;
    public Func<bool> CouldUse;
    public bool EffectCancellable = false;
    public float EffectDuration;
    public Func<bool> HasButton;
    public bool HasEffect;
    public KeyCode? Hotkey;
    public HudManager HudManager;
    public bool IsEffectActive;
    public Vector3 LocalScale = Vector3.one;
    public float MaxTimer = float.MaxValue;
    public bool Mirror;
    public Vector3 PositionOffset;
    public bool ShowButtonText = true;
    public AbilitySlot? Slot;
    public Sprite Sprite;
    public float Timer;
    public bool UseLayout;

    public CustomButton(Action onClick, Func<bool> hasButton, Func<bool> couldUse, Action onMeetingEnds, Sprite sprite, ButtonPosition position, HudManager hudManager, ActionButton textTemplate, KeyCode? hotkey, bool hasEffect, float effectDuration, Action onEffectEnds, bool mirror = false, string buttonText = "") : this(onClick, hasButton, couldUse, onMeetingEnds, sprite, position, hudManager, textTemplate, hotkey, null, hasEffect, effectDuration, onEffectEnds, mirror, buttonText) { }

    public CustomButton(Action onClick, Func<bool> hasButton, Func<bool> couldUse, Action onMeetingEnds, Sprite sprite, ButtonPosition position, HudManager hudManager, ActionButton textTemplate, AbilitySlot? slot, bool hasEffect, float effectDuration, Action onEffectEnds, bool mirror = false, string buttonText = "") : this(onClick, hasButton, couldUse, onMeetingEnds, sprite, position, hudManager, textTemplate, null, slot, hasEffect, effectDuration, onEffectEnds, mirror, buttonText) { }

    public CustomButton(Action onClick, Func<bool> hasButton, Func<bool> couldUse, Action onMeetingEnds, Sprite sprite, ButtonPosition position, HudManager hudManager, ActionButton textTemplate, AbilitySlot? slot, bool mirror = false, string buttonText = "") : this(onClick, hasButton, couldUse, onMeetingEnds, sprite, position, hudManager, textTemplate, null, slot, false, 0f, () => { }, mirror, buttonText) { }

    public CustomButton(Action onClick, Func<bool> hasButton, Func<bool> couldUse, Action onMeetingEnds, Sprite sprite, ButtonPosition position, HudManager hudManager, ActionButton textTemplate, KeyCode? hotkey, AbilitySlot? slot, bool hasEffect, float effectDuration, Action onEffectEnds, bool mirror = false, string buttonText = "")
    {
        HudManager = hudManager;
        _onClick = onClick;
        HasButton = hasButton;
        CouldUse = couldUse;
        PositionOffset = position.Offset;
        _onMeetingEnds = onMeetingEnds;
        HasEffect = hasEffect;
        EffectDuration = effectDuration;
        _onEffectEnds = onEffectEnds;
        Sprite = sprite;
        Mirror = mirror;
        Hotkey = hotkey;
        Slot = slot;
        ButtonText = buttonText;
        UseLayout = position.UseLayout;
        Timer = 16.2f;
        Buttons.Add(this);
        ActionButton = Object.Instantiate(hudManager.KillButton, hudManager.KillButton.transform.parent);
        ActionButton.gameObject.name = "CustomButton";

        // Add Key Bind Guide
        var baseObj = new GameObject("KeyBindGuide");
        baseObj.transform.SetParent(ActionButton.transform);
        baseObj.transform.localScale = Vector3.one;
        baseObj.transform.localPosition = new(-0.35f, -0.35f, -1f);

        _keyBackground = baseObj.AddComponent<SpriteRenderer>();
        _keyBackground.sprite = AssetLoader.KeyBindBackground;
        if (ActionButton.graphic != null)
        {
            _keyBackground.sortingLayerID = ActionButton.graphic.sortingLayerID;
            _keyBackground.sortingOrder = ActionButton.graphic.sortingOrder + 1;
        }

        var guideObj = new GameObject("Guide");
        guideObj.transform.SetParent(baseObj.transform);
        guideObj.transform.localScale = Vector3.one;
        guideObj.transform.localPosition = new(0f, 0f, -0.1f);
        _keyGuide = guideObj.AddComponent<SpriteRenderer>();
        if (ActionButton.graphic != null)
        {
            _keyGuide.sortingLayerID = ActionButton.graphic.sortingLayerID;
            _keyGuide.sortingOrder = ActionButton.graphic.sortingOrder + 2;
        }

        var button = ActionButton.GetComponent<PassiveButton>();
        button.OnClick = new();
        button.OnClick.AddListener((UnityAction)OnClickEvent);

        if (ActionButton.GetComponent<TextTranslatorTMP>()) ActionButton.GetComponent<TextTranslatorTMP>().Destroy();

        LocalScale = ActionButton.transform.localScale;
        if (textTemplate)
        {
            Object.Destroy(ActionButton.buttonLabelText);
            ActionButton.buttonLabelText = Object.Instantiate(textTemplate.buttonLabelText, ActionButton.transform);
        }

        if (ActionButton.buttonLabelText.GetComponent<TextTranslatorTMP>()) ActionButton.buttonLabelText.GetComponent<TextTranslatorTMP>().Destroy();

        ActionButton.OverrideText(ButtonText);
        _lastButtonText = ButtonText;

        if (UseLayout) ActionButton.transform.SetParent(hudManager.AbilityButton.transform.parent, false);

        SetActive(false);
    }

#nullable enable
    public CustomButton(Action onClick, Func<bool> hasButton, Func<bool> couldUse, Action onMeetingEnds, Sprite sprite, ButtonPosition position, HudManager hudManager, ActionButton? textTemplate, KeyCode? hotkey, bool mirror = false, string buttonText = "") : this(onClick, hasButton, couldUse, onMeetingEnds, sprite, position, hudManager, textTemplate, hotkey, false, 0f, () => { }, mirror, buttonText) { }
#nullable disable

    public void OnClickEvent()
    {
        if ((HasEffect && IsEffectActive && EffectCancellable) || (Timer < 0f && HasButton() && CouldUse()))
        {
            ActionButton.graphic.color = new(1f, 1f, 1f, 0.3f);
            _onClick();

            if (HasEffect && !IsEffectActive)
            {
                Timer = EffectDuration;
                ActionButton.cooldownTimerText.color = new(0F, 0.8F, 0F);
                IsEffectActive = true;
            }
        }
    }

    public static void HudUpdate()
    {
        for (var i = Buttons.Count - 1; i >= 0; i--)
        {
            if (Buttons[i].ActionButton == null)
            {
                Buttons.RemoveAt(i);
                continue;
            }

            try
            {
                Buttons[i].Update();
            }
            catch (Exception ex)
            {
                Logger.LogError($"[CustomButton] HudUpdate error: {ex}");
            }
        }
    }

    public static void MeetingEndedUpdate()
    {
        for (var i = Buttons.Count - 1; i >= 0; i--)
        {
            if (Buttons[i].ActionButton == null)
            {
                Buttons.RemoveAt(i);
                continue;
            }

            try
            {
                if (Buttons[i].HasButton != null && Buttons[i].HasButton()) Buttons[i]._onMeetingEnds?.Invoke();
                Buttons[i].Update();
            }
            catch (Exception ex)
            {
                Logger.LogError($"[CustomButton] MeetingEndedUpdate error: {ex}");
            }
        }
    }

    public static void ResetAllCooldowns()
    {
        for (var i = 0; i < Buttons.Count; i++)
        {
            try
            {
                Buttons[i].Timer = Buttons[i].MaxTimer;
                Buttons[i].Update();
            }
            catch (Exception ex)
            {
                Logger.LogError($"[CustomButton] ResetAllCooldowns error: {ex}");
            }
        }
    }

    public void SetActive(bool isActive)
    {
        if (_lastIsActive == isActive) return;
        _lastIsActive = isActive;
        if (ActionButton != null && ActionButton.gameObject != null)
        {
            ActionButton.gameObject.SetActive(isActive);
            ActionButton.graphic.enabled = isActive;
        }
    }

    public void Update()
    {
        if (PlayerControl.LocalPlayer?.Data == null || MeetingHud.Instance || ExileController.Instance || HasButton == null || !HasButton())
        {
            SetActive(false);
            return;
        }

        var useActive = HudManager?.UseButton != null && HudManager.UseButton.isActiveAndEnabled;
        var petActive = HudManager?.PetButton != null && HudManager.PetButton.isActiveAndEnabled;
        SetActive(useActive || petActive);

        if (ActionButton?.graphic != null && ActionButton.graphic.sprite != Sprite) ActionButton.graphic.sprite = Sprite;
        if (ShowButtonText && _lastButtonText != ButtonText && ActionButton != null)
        {
            ActionButton.OverrideText(ButtonText);
            _lastButtonText = ButtonText;
        }

        if (ActionButton?.buttonLabelText != null) ActionButton.buttonLabelText.enabled = ShowButtonText;

        if (HudManager?.UseButton != null && ActionButton != null)
        {
            if (UseLayout)
                ActionButton.transform.localScale = LocalScale;
            else
            {
                var useTransform = HudManager.UseButton.transform;
                var pos = useTransform.localPosition;
                if (Mirror)
                {
                    var aspect = Camera.main != null ? Camera.main.aspect : 1.77f;
                    var safeOrthographicSize = Camera.main != null ? CameraSafeArea.GetSafeOrthographicSize(Camera.main) : 3f;
                    var xpos = 0.05f - (safeOrthographicSize * aspect * 1.70f);
                    pos = new(xpos, pos.y, pos.z);
                }

                ActionButton.transform.localPosition = pos + PositionOffset;
                ActionButton.transform.localScale = LocalScale;
            }
        }

        var couldUse = CouldUse != null && CouldUse();
        var targetColor = couldUse ? Palette.EnabledColor : Palette.DisabledClear;
        var targetDesat = couldUse ? 0f : 1f;

        if (ActionButton?.graphic != null)
        {
            if (ActionButton.graphic.color != targetColor)
            {
                ActionButton.graphic.color = targetColor;
                ActionButton.buttonLabelText?.color = targetColor;
            }

            if (ActionButton.graphic.material != null && ActionButton.graphic.material.HasProperty("_Desat") && ActionButton.graphic.material.GetFloat("_Desat") != targetDesat) ActionButton.graphic.material.SetFloat("_Desat", targetDesat);
        }

        if (Timer >= 0 && !StopCountdown)
        {
            // Make sure role draft has finished or isn't running
            if (HasEffect && IsEffectActive)
                Timer -= Time.deltaTime;
            else if (PlayerControl.LocalPlayer != null && !PlayerControl.LocalPlayer.inVent && PlayerControl.LocalPlayer.moveable) Timer -= Time.deltaTime;
        }

        if (Timer <= 0 && HasEffect && IsEffectActive)
        {
            IsEffectActive = false;
            if (ActionButton?.cooldownTimerText != null) ActionButton.cooldownTimerText.color = Palette.EnabledColor;
            _onEffectEnds?.Invoke();
        }

        ActionButton?.SetCoolDown(Timer, HasEffect && IsEffectActive ? EffectDuration : MaxTimer);

        // Update Key Guide
        var activeKey = Hotkey ?? (Slot.HasValue ? KeyBindingManager.GetKey(Slot.Value) : null);
        if (activeKey.HasValue && activeKey.Value != KeyCode.None)
        {
            _keyBackground.gameObject.SetActive(true);
            _keyGuide.sprite = KeyBindingManager.GetKeySprite(activeKey.Value);

            _keyBackground.color = targetColor;
            _keyGuide.color = targetColor;
        }
        else
            _keyBackground?.gameObject.SetActive(false);

        // Trigger OnClickEvent if the hotkey is being pressed down
        if (Hotkey.HasValue && Input.GetKeyDown(Hotkey.Value)) OnClickEvent();
        if (Slot.HasValue && Input.GetKeyDown(KeyBindingManager.GetKey(Slot.Value))) OnClickEvent();
    }
}
