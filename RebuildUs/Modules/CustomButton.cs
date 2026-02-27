using UnityEngine.Events;

namespace RebuildUs.Modules;

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

    internal static bool StopCountdown = true;
    private static readonly int Desat = Shader.PropertyToID("_Desat");
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

    private string _lastButtonText;

    private bool _lastIsActive;
    internal string ButtonText;
    internal float EffectDuration;
    internal bool HasEffect;
    internal bool IsEffectActive;
    internal Vector3 LocalScale;
    internal float MaxTimer = float.MaxValue;
    internal Vector3 PositionOffset;
    internal Sprite Sprite;
    internal float Timer;

    private CustomButton(Action onClick,
                         Func<bool> hasButton,
                         Func<bool> couldUse,
                         Action onMeetingEnds,
                         Sprite sprite,
                         ButtonPosition position,
                         HudManager hudManager,
                         ActionButton textTemplate,
                         KeyCode? hotkey,
                         bool hasEffect,
                         float effectDuration,
                         Action onEffectEnds,
                         bool mirror = false,
                         string buttonText = "") : this(onClick,
        hasButton,
        couldUse,
        onMeetingEnds,
        sprite,
        position,
        hudManager,
        textTemplate,
        hotkey,
        null,
        hasEffect,
        effectDuration,
        onEffectEnds,
        mirror,
        buttonText) { }

    internal CustomButton(Action onClick,
                          Func<bool> hasButton,
                          Func<bool> couldUse,
                          Action onMeetingEnds,
                          Sprite sprite,
                          ButtonPosition position,
                          HudManager hudManager,
                          ActionButton textTemplate,
                          AbilitySlot? slot,
                          bool hasEffect,
                          float effectDuration,
                          Action onEffectEnds,
                          bool mirror = false,
                          string buttonText = "") : this(onClick,
        hasButton,
        couldUse,
        onMeetingEnds,
        sprite,
        position,
        hudManager,
        textTemplate,
        null,
        slot,
        hasEffect,
        effectDuration,
        onEffectEnds,
        mirror,
        buttonText) { }

    internal CustomButton(Action onClick,
                          Func<bool> hasButton,
                          Func<bool> couldUse,
                          Action onMeetingEnds,
                          Sprite sprite,
                          ButtonPosition position,
                          HudManager hudManager,
                          ActionButton textTemplate,
                          AbilitySlot? slot,
                          bool mirror = false,
                          string buttonText = "") : this(onClick,
        hasButton,
        couldUse,
        onMeetingEnds,
        sprite,
        position,
        hudManager,
        textTemplate,
        null,
        slot,
        false,
        0f,
        () => { },
        mirror,
        buttonText) { }

    private CustomButton(Action onClick,
                         Func<bool> hasButton,
                         Func<bool> couldUse,
                         Action onMeetingEnds,
                         Sprite sprite,
                         ButtonPosition position,
                         HudManager hudManager,
                         ActionButton textTemplate,
                         KeyCode? hotkey,
                         AbilitySlot? slot,
                         bool hasEffect,
                         float effectDuration,
                         Action onEffectEnds,
                         bool mirror = false,
                         string buttonText = "")
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
        ActionButton.gameObject.name = "CustomButton";

        // Add Key Bind Guide
        GameObject baseObj = new("KeyBindGuide");
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

        PassiveButton button = ActionButton.GetComponent<PassiveButton>();
        button.OnClick = new();
        button.OnClick.AddListener((UnityAction)OnClickEvent);

        if (ActionButton.GetComponent<TextTranslatorTMP>())
        {
            ActionButton.GetComponent<TextTranslatorTMP>().Destroy();
        }

        LocalScale = ActionButton.transform.localScale;
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
        _lastButtonText = ButtonText;

        if (_useLayout)
        {
            ActionButton.transform.SetParent(hudManager.AbilityButton.transform.parent, false);
        }

        SetActive(false);
    }

#nullable enable
    internal CustomButton(Action onClick,
                          Func<bool> hasButton,
                          Func<bool> couldUse,
                          Action onMeetingEnds,
                          Sprite sprite,
                          ButtonPosition position,
                          HudManager hudManager,
                          ActionButton? textTemplate,
                          KeyCode? hotkey,
                          bool mirror = false,
                          string buttonText = "") : this(onClick,
        hasButton,
        couldUse,
        onMeetingEnds,
        sprite,
        position,
        hudManager,
        textTemplate,
        hotkey,
        false,
        0f,
        () => { },
        mirror,
        buttonText) { }
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
        for (int i = Buttons.Count - 1; i >= 0; i--)
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

    internal static void MeetingEndedUpdate()
    {
        for (int i = Buttons.Count - 1; i >= 0; i--)
        {
            if (Buttons[i].ActionButton == null)
            {
                Buttons.RemoveAt(i);
                continue;
            }

            try
            {
                if (Buttons[i]._hasButton != null && Buttons[i]._hasButton())
                {
                    Buttons[i]._onMeetingEnds?.Invoke();
                }

                Buttons[i].Update();
            }
            catch (Exception ex)
            {
                Logger.LogError($"[CustomButton] MeetingEndedUpdate error: {ex}");
            }
        }
    }

    internal static void ResetAllCooldowns()
    {
        foreach (CustomButton t in Buttons)
        {
            try
            {
                t.Timer = t.MaxTimer;
                t.Update();
            }
            catch (Exception ex)
            {
                Logger.LogError($"[CustomButton] ResetAllCooldowns error: {ex}");
            }
        }
    }

    internal void SetActive(bool isActive)
    {
        if (_lastIsActive == isActive)
        {
            return;
        }
        _lastIsActive = isActive;
        if (ActionButton != null && ActionButton.gameObject != null)
        {
            ActionButton.gameObject.SetActive(isActive);
            ActionButton.graphic.enabled = isActive;
        }
    }

    private void Update()
    {
        if (PlayerControl.LocalPlayer?.Data == null || MeetingHud.Instance || ExileController.Instance || _hasButton == null || !_hasButton())
        {
            SetActive(false);
            return;
        }

        bool useActive = _hudManager?.UseButton != null && _hudManager.UseButton.isActiveAndEnabled;
        bool petActive = _hudManager?.PetButton != null && _hudManager.PetButton.isActiveAndEnabled;
        SetActive(useActive || petActive);

        if (ActionButton?.graphic != null && ActionButton.graphic.sprite != Sprite)
        {
            ActionButton.graphic.sprite = Sprite;
        }
        if (SHOW_BUTTON_TEXT && _lastButtonText != ButtonText && ActionButton != null)
        {
            ActionButton.OverrideText(ButtonText);
            _lastButtonText = ButtonText;
        }

        if (ActionButton?.buttonLabelText != null)
        {
            ActionButton.buttonLabelText.enabled = SHOW_BUTTON_TEXT;
        }

        if (_hudManager?.UseButton != null && ActionButton != null)
        {
            if (!_useLayout)
            {
                Transform useTransform = _hudManager.UseButton.transform;
                Vector3 pos = useTransform.localPosition;
                if (_mirror)
                {
                    float aspect = Camera.main != null ? Camera.main.aspect : 1.77f;
                    float safeOrthographicSize = Camera.main != null ? CameraSafeArea.GetSafeOrthographicSize(Camera.main) : 3f;
                    float xpos = 0.05f - safeOrthographicSize * aspect * 1.70f;
                    pos = new(xpos, pos.y, pos.z);
                }

                ActionButton.transform.localPosition = pos + PositionOffset;
            }

            ActionButton.transform.localScale = LocalScale;
        }

        bool couldUse = _couldUse != null && _couldUse();
        Color targetColor = couldUse ? Palette.EnabledColor : Palette.DisabledClear;
        float targetDesat = couldUse ? 0f : 1f;

        if (ActionButton?.graphic != null)
        {
            if (ActionButton.graphic.color != targetColor)
            {
                ActionButton.graphic.color = targetColor;
                ActionButton.buttonLabelText?.color = targetColor;
            }

            if (ActionButton.graphic.material != null
                && ActionButton.graphic.material.HasProperty(Desat)
                && !Mathf.Approximately(ActionButton.graphic.material.GetFloat(Desat), targetDesat))
            {
                ActionButton.graphic.material.SetFloat(Desat, targetDesat);
            }
        }

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
            if (ActionButton?.cooldownTimerText != null)
            {
                ActionButton.cooldownTimerText.color = Palette.EnabledColor;
            }
            _onEffectEnds?.Invoke();
        }

        ActionButton?.SetCoolDown(Timer, HasEffect && IsEffectActive ? EffectDuration : MaxTimer);

        // Update Key Guide
        KeyCode? activeKey = _hotkey ?? (_slot.HasValue ? KeyBindingManager.GetKey(_slot.Value) : null);
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