using UnityEngine.UI;

namespace RebuildUs;

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

    public static bool StopCountdown = true;

    public static class ButtonPositions
    {
        public static readonly Vector3 LowerRowRight = new(-2f, -0.06f, 0);  // Not usable for imps because of new button positions!
        public static readonly Vector3 LowerRowCenter = new(-3f, -0.06f, 0);
        public static readonly Vector3 LowerRowLeft = new(-4f, -0.06f, 0);
        public static readonly Vector3 UpperRowRight = new(0f, 1f, 0f);  // Not usable for imps because of new button positions!
        public static readonly Vector3 UpperRowCenter = new(-1f, 1f, 0f);  // Not usable for imps because of new button positions!
        public static readonly Vector3 UpperRowLeft = new(-2f, 1f, 0f);
        public static readonly Vector3 UpperRowFarLeft = new(-3f, 1f, 0f);
        public static readonly Vector3 HighRowRight = new(0f, 2.06f, 0f);
    }

    public CustomButton(
        Action onClick,
        Func<bool> hasButton,
        Func<bool> couldUse,
        Action onMeetingEnds,
        Sprite sprite,
        Vector3 positionOffset,
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
        this.PositionOffset = positionOffset;
        this.OnMeetingEnds = onMeetingEnds;
        this.HasEffect = hasEffect;
        this.EffectDuration = effectDuration;
        this.OnEffectEnds = onEffectEnds;
        this.Sprite = sprite;
        this.Mirror = mirror;
        this.Hotkey = hotkey;
        this.ButtonText = buttonText;
        Timer = 16.2f;
        Buttons.Add(this);
        ActionButton = UnityEngine.Object.Instantiate(hudManager.KillButton, hudManager.KillButton.transform.parent);
        PassiveButton button = ActionButton.GetComponent<PassiveButton>();
        button.OnClick = new Button.ButtonClickedEvent();
        button.OnClick.AddListener((UnityEngine.Events.UnityAction)OnClickEvent);

        LocalScale = ActionButton.transform.localScale;
        if (textTemplate)
        {
            UnityEngine.Object.Destroy(ActionButton.buttonLabelText);
            ActionButton.buttonLabelText = UnityEngine.Object.Instantiate(textTemplate.buttonLabelText, ActionButton.transform);
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
        Vector3 positionOffset,
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
        positionOffset,
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

    private bool lastIsActive = false;
    public void SetActive(bool isActive)
    {
        if (lastIsActive == isActive) return;
        lastIsActive = isActive;
        if (ActionButton != null && ActionButton.gameObject != null)
        {
            ActionButton.gameObject.SetActive(isActive);
            ActionButton.graphic.enabled = isActive;
        }
    }

    private string lastButtonText = "";

    public void Update()
    {
        if (PlayerControl.LocalPlayer.Data == null || MeetingHud.Instance || ExileController.Instance || !HasButton())
        {
            SetActive(false);
            return;
        }
        SetActive(HudManager.UseButton.isActiveAndEnabled || HudManager.PetButton.isActiveAndEnabled);

        if (ActionButton.graphic.sprite != Sprite) ActionButton.graphic.sprite = Sprite;
        if (ShowButtonText && ButtonText != "" && lastButtonText != ButtonText)
        {
            ActionButton.OverrideText(ButtonText);
            lastButtonText = ButtonText;
        }
        ActionButton.buttonLabelText.enabled = ShowButtonText; // Only show the text if it's a kill button

        if (HudManager.UseButton != null)
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