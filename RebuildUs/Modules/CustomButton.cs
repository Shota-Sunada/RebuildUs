using UnityEngine.UI;

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

    public static ButtonPosition Layout => new() { UseLayout = true };
    public static implicit operator ButtonPosition(Vector3 offset) => new(offset);
}

public class CustomButton
{
    public static List<CustomButton> Buttons = [];
    public ActionButton ActionButton;
    public Vector3 PositionOffset;
    public Vector3 LocalScale = Vector3.one;
    public float MaxTimer = float.MaxValue;
    public float Timer = 0f;
    public bool EffectCancellable = false;
    private readonly Action OnClick;
    private readonly Action OnMeetingEnds;
    public Func<bool> HasButton;
    public Func<bool> CouldUse;
    private readonly Action OnEffectEnds;
    public bool HasEffect;
    public bool IsEffectActive = false;
    public bool ShowButtonText = true;
    public string ButtonText;
    public float EffectDuration;
    public Sprite Sprite;
    public HudManager HudManager;
    public bool Mirror;
    public KeyCode? Hotkey;
    public bool UseLayout = false;

    public static bool StopCountdown = true;

    public CustomButton(
        Action onClick,
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
        string buttonText = ""
    )
    {
        this.HudManager = hudManager;
        this.OnClick = onClick;
        this.HasButton = hasButton;
        this.CouldUse = couldUse;
        this.PositionOffset = position.Offset;
        this.OnMeetingEnds = onMeetingEnds;
        this.HasEffect = hasEffect;
        this.EffectDuration = effectDuration;
        this.OnEffectEnds = onEffectEnds;
        this.Sprite = sprite;
        this.Mirror = mirror;
        this.Hotkey = hotkey;
        this.ButtonText = buttonText;
        this.UseLayout = position.UseLayout;
        Timer = 16.2f;
        Buttons.Add(this);
        ActionButton = UnityEngine.Object.Instantiate(hudManager.KillButton, hudManager.KillButton.transform.parent);
        ActionButton.gameObject.name = "CustomButton";
        PassiveButton button = ActionButton.GetComponent<PassiveButton>();
        button.OnClick = new Button.ButtonClickedEvent();
        button.OnClick.AddListener((UnityEngine.Events.UnityAction)OnClickEvent);

        LocalScale = ActionButton.transform.localScale;
        if (textTemplate)
        {
            UnityEngine.Object.Destroy(ActionButton.buttonLabelText);
            ActionButton.buttonLabelText = UnityEngine.Object.Instantiate(textTemplate.buttonLabelText, ActionButton.transform);
        }

        if (UseLayout)
        {
            ActionButton.transform.SetParent(hudManager.AbilityButton.transform.parent, false);
        }

        SetActive(false);
    }

#nullable enable
    public CustomButton(
        Action onClick,
        Func<bool> hasButton,
        Func<bool> couldUse,
        Action onMeetingEnds,
        Sprite sprite,
        ButtonPosition position,
        HudManager hudManager,
        ActionButton? textTemplate,
        KeyCode? hotkey,
        bool mirror = false,
        string buttonText = ""
    )
    : this(
        onClick,
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
        buttonText
    )
    { }
#nullable disable

    public void OnClickEvent()
    {
        if ((HasEffect && IsEffectActive && EffectCancellable) || (Timer < 0f && HasButton() && CouldUse()))
        {
            ActionButton.graphic.color = new Color(1f, 1f, 1f, 0.3f);
            OnClick();

            if (HasEffect && !IsEffectActive)
            {
                Timer = EffectDuration;
                ActionButton.cooldownTimerText.color = new Color(0F, 0.8F, 0F);
                IsEffectActive = true;
            }
        }
    }

    public static void HudUpdate()
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
            catch (NullReferenceException)
            {
                Logger.LogWarn("[WARNING] NullReferenceException from HudUpdate().HasButton()");
            }
        }
    }

    public static void MeetingEndedUpdate()
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
                Buttons[i].OnMeetingEnds();
                Buttons[i].Update();
            }
            catch (NullReferenceException)
            {
                Logger.LogWarn("[WARNING] NullReferenceException from MeetingEndedUpdate().HasButton()");
            }
        }
    }

    public static void ResetAllCooldowns()
    {
        for (int i = 0; i < Buttons.Count; i++)
        {
            try
            {
                Buttons[i].Timer = Buttons[i].MaxTimer;
                Buttons[i].Update();
            }
            catch (NullReferenceException)
            {
                Logger.LogWarn("[WARNING] NullReferenceException from MeetingEndedUpdate().HasButton(), if theres only one warning its fine");
            }
        }
    }

    private bool LastIsActive = false;
    public void SetActive(bool isActive)
    {
        if (LastIsActive == isActive) return;
        LastIsActive = isActive;
        if (ActionButton != null && ActionButton.gameObject != null)
        {
            ActionButton.gameObject.SetActive(isActive);
            ActionButton.graphic.enabled = isActive;
        }
    }

    private string LastButtonText = "";

    public void Update()
    {
        if (PlayerControl.LocalPlayer.Data == null || MeetingHud.Instance || ExileController.Instance || !HasButton())
        {
            SetActive(false);
            return;
        }
        SetActive(HudManager.UseButton.isActiveAndEnabled || HudManager.PetButton.isActiveAndEnabled);

        if (ActionButton.graphic.sprite != Sprite) ActionButton.graphic.sprite = Sprite;
        if (ShowButtonText && ButtonText != "" && LastButtonText != ButtonText)
        {
            ActionButton.OverrideText(ButtonText);
            LastButtonText = ButtonText;
        }
        ActionButton.buttonLabelText.enabled = ShowButtonText; // Only show the text if it's a kill button

        if (HudManager.UseButton != null)
        {
            if (UseLayout)
            {
                ActionButton.transform.localScale = LocalScale;
            }
            else
            {
                Transform useTransform = HudManager.UseButton.transform;
                Vector3 pos = useTransform.localPosition;
                if (Mirror)
                {
                    float aspect = Camera.main.aspect;
                    float safeOrthographicSize = CameraSafeArea.GetSafeOrthographicSize(Camera.main);
                    float xpos = 0.05f - safeOrthographicSize * aspect * 1.70f;
                    pos = new Vector3(xpos, pos.y, pos.z);
                }
                ActionButton.transform.localPosition = pos + PositionOffset;
                ActionButton.transform.localScale = LocalScale;
            }
        }

        bool couldUse = CouldUse();
        Color targetColor = couldUse ? Palette.EnabledColor : Palette.DisabledClear;
        float targetDesat = couldUse ? 0f : 1f;

        if (ActionButton.graphic.color != targetColor)
        {
            ActionButton.graphic.color = ActionButton.buttonLabelText.color = targetColor;
        }
        if (ActionButton.graphic.material.GetFloat("_Desat") != targetDesat)
        {
            ActionButton.graphic.material.SetFloat("_Desat", targetDesat);
        }

        if (Timer >= 0 && !StopCountdown)
        {
            // Make sure role draft has finished or isn't running
            if (HasEffect && IsEffectActive)
            {
                Timer -= Time.deltaTime;
            }
            else if (!PlayerControl.LocalPlayer.inVent && PlayerControl.LocalPlayer.moveable)
            {
                Timer -= Time.deltaTime;
            }
        }

        if (Timer <= 0 && HasEffect && IsEffectActive)
        {
            IsEffectActive = false;
            ActionButton.cooldownTimerText.color = Palette.EnabledColor;
            OnEffectEnds();
        }

        ActionButton.SetCoolDown(Timer, (HasEffect && IsEffectActive) ? EffectDuration : MaxTimer);

        // Trigger OnClickEvent if the hotkey is being pressed down
        if (Hotkey.HasValue && Input.GetKeyDown(Hotkey.Value)) OnClickEvent();
    }
}