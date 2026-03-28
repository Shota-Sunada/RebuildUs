using UnityEngine.Events;

namespace RebuildUs.Objects;

internal struct ButtonPosition
{
    internal Vector3 Offset;
    internal bool UseLayout;

    private ButtonPosition(Vector3 offset)
    {
        Offset = offset;
        UseLayout = false;
    }

    internal static ButtonPosition Layout
    {
        get => new()
        {
            UseLayout = true,
        };
    }

    public static implicit operator ButtonPosition(Vector3 offset)
    {
        return new(offset);
    }
}

internal sealed class CustomButton
{
    private const bool EFFECT_CANCELLABLE = false;
    private const bool SHOW_BUTTON_TEXT = true;
    internal static readonly List<CustomButton> Buttons = [];

    internal string Name = "";
    internal static bool StopCountdown = true;
    private readonly Func<bool> _couldUse;
    private readonly Func<bool> _hasButton;
    private readonly KeyCode? _hotkey;
    private readonly HudManager _hudManager;
    private readonly SpriteRenderer _keyBackground;

    private readonly SpriteRenderer _keyGuide;
    private readonly bool _mirror;
    private readonly Action _onClick;
    private readonly Action _onEffectEnds;
    private readonly Action _onMeetingEnds;
    private readonly AbilitySlot? _slot;
    private readonly bool _useLayout;
    internal readonly ActionButton ActionButton;

    internal string ButtonText;
    internal float EffectDuration;
    internal bool HasEffect;
    internal bool IsEffectActive;
    internal float MaxTimer = float.MaxValue;
    internal Vector3 PositionOffset;
    internal Sprite Sprite;
    internal float Timer;

    private CustomButton(string name, Action onClick, Func<bool> hasButton, Func<bool> couldUse, Action onMeetingEnds, Sprite sprite, ButtonPosition position, HudManager hudManager, ActionButton textTemplate, KeyCode? hotkey, bool hasEffect, float effectDuration, Action onEffectEnds, bool mirror = false, string buttonText = "")
    : this(name, onClick, hasButton, couldUse, onMeetingEnds, sprite, position, hudManager, textTemplate, hotkey, null, hasEffect, effectDuration, onEffectEnds, mirror, buttonText)
    { }

    internal CustomButton(string name, Action onClick, Func<bool> hasButton, Func<bool> couldUse, Action onMeetingEnds, Sprite sprite, ButtonPosition position, HudManager hudManager, ActionButton textTemplate, AbilitySlot? slot, bool hasEffect, float effectDuration, Action onEffectEnds, bool mirror = false, string buttonText = "")
    : this(name, onClick, hasButton, couldUse, onMeetingEnds, sprite, position, hudManager, textTemplate, null, slot, hasEffect, effectDuration, onEffectEnds, mirror, buttonText)
    { }

    internal CustomButton(string name, Action onClick, Func<bool> hasButton, Func<bool> couldUse, Action onMeetingEnds, Sprite sprite, ButtonPosition position, HudManager hudManager, ActionButton textTemplate, AbilitySlot? slot, bool mirror = false, string buttonText = "")
    : this(name, onClick, hasButton, couldUse, onMeetingEnds, sprite, position, hudManager, textTemplate, null, slot, false, 0f, () => { }, mirror, buttonText)
    { }

    private CustomButton(string name, Action onClick, Func<bool> hasButton, Func<bool> couldUse, Action onMeetingEnds, Sprite sprite, ButtonPosition position, HudManager hudManager, ActionButton textTemplate, KeyCode? hotkey, AbilitySlot? slot, bool hasEffect, float effectDuration, Action onEffectEnds, bool mirror = false, string buttonText = "")
    {
        _hudManager = hudManager;
        _onClick = onClick;
        _hasButton = hasButton;
        _couldUse = couldUse;
        PositionOffset = position.Offset;
        _onMeetingEnds = onMeetingEnds;
        HasEffect = hasEffect;
        EffectDuration = effectDuration;
        _onEffectEnds = onEffectEnds;
        Sprite = sprite;
        _mirror = mirror;
        _hotkey = hotkey;
        _slot = slot;
        ButtonText = buttonText;
        _useLayout = position.UseLayout;
        Timer = 16.2f;
        Buttons.Add(this);
        ActionButton = UnityObject.Instantiate(hudManager.KillButton, hudManager.KillButton.transform.parent);
        Name = ActionButton.gameObject.name = string.IsNullOrEmpty(name) ? "Unnamed CustomButton" : name;

        // Add Key Bind Guide
        GameObject baseObj = new("KeyBindGuide");
        baseObj.transform.SetParent(ActionButton.transform);
        baseObj.transform.localScale = Vector3.one;
        baseObj.transform.localPosition = new(-0.55f, -0.35f, -1f);

        _keyBackground = baseObj.AddComponent<SpriteRenderer>();
        _keyBackground.sprite = AssetLoader.KeyBindBackground;
        if (ActionButton.graphic != null)
        {
            _keyBackground.sortingLayerID = ActionButton.graphic.sortingLayerID;
            _keyBackground.sortingOrder = ActionButton.graphic.sortingOrder + 1;
        }

        GameObject guideObj = new("Guide");
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

        if (ActionButton.GetComponent<TextTranslatorTMP>())
        {
            ActionButton.GetComponent<TextTranslatorTMP>().Destroy();
        }

        if (textTemplate)
        {
            UnityObject.Destroy(ActionButton.buttonLabelText);
            ActionButton.buttonLabelText = UnityObject.Instantiate(textTemplate.buttonLabelText, ActionButton.transform);
        }

        if (ActionButton.buttonLabelText.GetComponent<TextTranslatorTMP>())
        {
            ActionButton.buttonLabelText.GetComponent<TextTranslatorTMP>().Destroy();
        }

        ActionButton.OverrideText(ButtonText);

        if (_useLayout)
        {
            ActionButton.transform.SetParent(hudManager.AbilityButton.transform.parent, false);
        }

        Update();
    }

#nullable enable
    internal CustomButton(string name, Action onClick, Func<bool> hasButton, Func<bool> couldUse, Action onMeetingEnds, Sprite sprite, ButtonPosition position, HudManager hudManager, ActionButton? textTemplate, KeyCode? hotkey, bool mirror = false, string buttonText = "")
    : this(name, onClick, hasButton, couldUse, onMeetingEnds, sprite, position, hudManager, textTemplate, hotkey, false, 0f, () => { }, mirror, buttonText)
    { }
#nullable disable

    private void OnClickEvent()
    {
        if ((!HasEffect || !IsEffectActive || !EFFECT_CANCELLABLE) && (!(Timer < 0f) || !_hasButton() || !_couldUse()))
        {
            return;
        }
        ActionButton.graphic.color = new(1f, 1f, 1f, 0.3f);
        _onClick();

        if (!HasEffect || IsEffectActive)
        {
            return;
        }
        Timer = EffectDuration;
        ActionButton.cooldownTimerText.color = new(0F, 0.8F, 0F);
        IsEffectActive = true;
    }

    internal static void HudUpdate()
    {
        for (var i = 0; i < Buttons.Count; i++)
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
                Logger.LogError("[CustomButton] HudUpdate error in button '{0}': {1}", Buttons[i].Name, ex.Message);
            }
        }
    }

    internal static void MeetingEndedUpdate()
    {
        for (var i = 0; i < Buttons.Count; i++)
        {
            if (Buttons[i].ActionButton == null)
            {
                Buttons.RemoveAt(i);
                continue;
            }

            try
            {
                Buttons[i]._onMeetingEnds();
                Buttons[i].Update();
            }
            catch (Exception ex)
            {
                Logger.LogError("[CustomButton] MeetingEndedUpdate error in button '{0}': {1}", Buttons[i].Name, ex.Message);
            }
        }
    }

    internal static void ResetAllCooldowns()
    {
        foreach (var t in Buttons)
        {
            try
            {
                t.Timer = t.MaxTimer;
                t.Update();
            }
            catch (Exception ex)
            {
                Logger.LogError("[CustomButton] ResetAllCooldowns error in button '{0}': {1}", t.Name, ex.Message);
            }
        }
    }

    internal static void DestroyAllButtons()
    {
        for (var i = 0; i < Buttons.Count; i++)
        {
            try
            {
                if (Buttons[i].ActionButton != null && Buttons[i].ActionButton.gameObject != null)
                {
                    UnityObject.Destroy(Buttons[i].ActionButton.gameObject);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("[CustomButton] DestroyAllButtons error in button '{0}': {1}", Buttons[i].Name, ex.Message);
            }
        }

        Buttons.Clear();
    }

    internal void SetActive(bool isActive)
    {
        if (ActionButton != null && ActionButton.gameObject != null)
        {
            ActionButton.gameObject.SetActive(isActive);
            ActionButton.graphic.enabled = isActive;
        }
    }

    private void Update()
    {
        if (PlayerControl.LocalPlayer?.Data == null || MeetingHud.Instance || ExileController.Instance || !_hasButton())
        {
            SetActive(false);
            return;
        }
        SetActive(_hudManager.UseButton.isActiveAndEnabled || _hudManager.PetButton.isActiveAndEnabled);

        ActionButton.graphic.sprite = Sprite;

        if (SHOW_BUTTON_TEXT && ActionButton != null)
        {
            ActionButton.OverrideText(ButtonText);
        }
        ActionButton.buttonLabelText.enabled = SHOW_BUTTON_TEXT;

        if (_hudManager?.UseButton != null && ActionButton != null)
        {
            if (!_useLayout)
            {
                var useTransform = _hudManager.UseButton.transform;
                var pos = useTransform.localPosition;
                if (_mirror)
                {
                    var aspect = Camera.main != null ? Camera.main.aspect : 1.77f;
                    var safeOrthographicSize = Camera.main != null ? CameraSafeArea.GetSafeOrthographicSize(Camera.main) : 3f;
                    var xpos = 0.05f - safeOrthographicSize * aspect * 1.70f;
                    pos = new(xpos, pos.y, pos.z);
                }

                ActionButton.transform.localPosition = pos + PositionOffset;
            }
        }

        var targetColor = _couldUse() ? Palette.EnabledColor : Palette.DisabledClear;
        var targetDesat = _couldUse() ? 0f : 1f;
        ActionButton.graphic.color = ActionButton.buttonLabelText.color = targetColor;
        ActionButton.graphic.material.SetFloat("_Desat", targetDesat);

        if (Timer >= 0 && !StopCountdown)
        {
            // Make sure role draft has finished or isn't running
            if (HasEffect && IsEffectActive)
            {
                Timer -= Time.deltaTime;
            }
            else if (PlayerControl.LocalPlayer != null && !PlayerControl.LocalPlayer.inVent && PlayerControl.LocalPlayer.moveable)
            {
                Timer -= Time.deltaTime;
            }
        }

        if (Timer <= 0 && HasEffect && IsEffectActive)
        {
            IsEffectActive = false;
            ActionButton.cooldownTimerText.color = Palette.EnabledColor;
            _onEffectEnds();
        }

        ActionButton?.SetCoolDown(Timer, HasEffect && IsEffectActive ? EffectDuration : MaxTimer);

        // Update Key Guide
        var activeKey = _hotkey ?? (_slot.HasValue ? KeyBindingManager.GetKey(_slot.Value) : null);
        if (activeKey.HasValue && activeKey.Value != KeyCode.None)
        {
            _keyBackground.gameObject.SetActive(true);
            _keyGuide.sprite = KeyBindingManager.GetKeySprite(activeKey.Value);

            _keyBackground.color = targetColor;
            _keyGuide.color = targetColor;
        }
        else
        {
            _keyBackground?.gameObject.SetActive(false);
        }

        // Trigger OnClickEvent if the hotkey is being pressed down
        if (_hotkey.HasValue && Input.GetKeyDown(_hotkey.Value))
        {
            OnClickEvent();
        }
        if (_slot.HasValue && Input.GetKeyDown(KeyBindingManager.GetKey(_slot.Value)))
        {
            OnClickEvent();
        }
    }
}