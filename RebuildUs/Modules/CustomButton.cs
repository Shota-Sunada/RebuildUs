using Rewired;
using UnityEngine.UI;

namespace RebuildUs;

public class CustomButton
{
    public static List<CustomButton> buttons = [];
    public ActionButton actionButton;
    public Vector3 PositionOffset;
    public Vector3 LocalScale = Vector3.one;
    public float MaxTimer = float.MaxValue;
    public float Timer = 0f;
    public bool effectCancellable = false;
    private Action OnClick;
    private Action OnMeetingEnds;
    public Func<bool> HasButton;
    public Func<bool> CouldUse;
    private Action OnEffectEnds;
    public bool HasEffect;
    public bool isEffectActive = false;
    public bool showButtonText = true;
    public string buttonText;
    public float EffectDuration;
    public Sprite Sprite;
    public HudManager hudManager;
    public bool mirror;
    public KeyCode? hotkey;

    public static class ButtonPositions
    {
        public static readonly Vector3 lowerRowRight = new(-2f, -0.06f, 0);  // Not usable for imps because of new button positions!
        public static readonly Vector3 lowerRowCenter = new(-3f, -0.06f, 0);
        public static readonly Vector3 lowerRowLeft = new(-4f, -0.06f, 0);
        public static readonly Vector3 upperRowRight = new(0f, 1f, 0f);  // Not usable for imps because of new button positions!
        public static readonly Vector3 upperRowCenter = new(-1f, 1f, 0f);  // Not usable for imps because of new button positions!
        public static readonly Vector3 upperRowLeft = new(-2f, 1f, 0f);
        public static readonly Vector3 upperRowFarLeft = new(-3f, 1f, 0f);
        public static readonly Vector3 highRowRight = new(0f, 2.06f, 0f);
    }

    public CustomButton(
        Action OnClick,
        Func<bool> HasButton,
        Func<bool> CouldUse,
        Action OnMeetingEnds,
        Sprite Sprite,
        Vector3 PositionOffset,
        HudManager hudManager,
        ActionButton textTemplate,
        KeyCode? hotkey,
        bool HasEffect,
        float EffectDuration,
        Action OnEffectEnds,
        bool mirror = false,
        string buttonText = ""
    )
    {
        this.hudManager = hudManager;
        this.OnClick = OnClick;
        this.HasButton = HasButton;
        this.CouldUse = CouldUse;
        this.PositionOffset = PositionOffset;
        this.OnMeetingEnds = OnMeetingEnds;
        this.HasEffect = HasEffect;
        this.EffectDuration = EffectDuration;
        this.OnEffectEnds = OnEffectEnds;
        this.Sprite = Sprite;
        this.mirror = mirror;
        this.hotkey = hotkey;
        this.buttonText = buttonText;
        Timer = 16.2f;
        buttons.Add(this);
        actionButton = UnityEngine.Object.Instantiate(hudManager.KillButton, hudManager.KillButton.transform.parent);
        PassiveButton button = actionButton.GetComponent<PassiveButton>();
        button.OnClick = new Button.ButtonClickedEvent();
        button.OnClick.AddListener((UnityEngine.Events.UnityAction)onClickEvent);

        LocalScale = actionButton.transform.localScale;
        if (textTemplate)
        {
            UnityEngine.Object.Destroy(actionButton.buttonLabelText);
            actionButton.buttonLabelText = UnityEngine.Object.Instantiate(textTemplate.buttonLabelText, actionButton.transform);
        }

        setActive(false);
    }

#nullable enable
    public CustomButton(
        Action OnClick,
        Func<bool> HasButton,
        Func<bool> CouldUse,
        Action OnMeetingEnds,
        Sprite Sprite,
        Vector3 PositionOffset,
        HudManager hudManager,
        ActionButton? textTemplate,
        KeyCode? hotkey,
        bool mirror = false,
        string buttonText = ""
    )
    : this(
        OnClick,
        HasButton,
        CouldUse,
        OnMeetingEnds,
        Sprite,
        PositionOffset,
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

    public void onClickEvent()
    {
        if ((HasEffect && isEffectActive && effectCancellable) || (Timer < 0f && HasButton() && CouldUse()))
        {
            actionButton.graphic.color = new Color(1f, 1f, 1f, 0.3f);
            OnClick();

            if (HasEffect && !isEffectActive)
            {
                Timer = EffectDuration;
                actionButton.cooldownTimerText.color = new Color(0F, 0.8F, 0F);
                isEffectActive = true;
            }
        }
    }

    public static void HudUpdate()
    {
        buttons.RemoveAll(item => item.actionButton == null);

        for (int i = 0; i < buttons.Count; i++)
        {
            try
            {
                buttons[i].Update();
            }
            catch (NullReferenceException)
            {
                Logger.LogWarn("[WARNING] NullReferenceException from HudUpdate().HasButton(), if theres only one warning its fine");
            }
        }
    }

    public static void MeetingEndedUpdate()
    {
        buttons.RemoveAll(item => item.actionButton == null);
        for (int i = 0; i < buttons.Count; i++)
        {
            try
            {
                buttons[i].OnMeetingEnds();
                buttons[i].Update();
            }
            catch (NullReferenceException)
            {
                Logger.LogWarn("[WARNING] NullReferenceException from MeetingEndedUpdate().HasButton(), if theres only one warning its fine");
            }
        }
    }

    public static void ResetAllCooldowns()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            try
            {
                buttons[i].Timer = buttons[i].MaxTimer;
                buttons[i].Update();
            }
            catch (NullReferenceException)
            {
                Logger.LogWarn("[WARNING] NullReferenceException from MeetingEndedUpdate().HasButton(), if theres only one warning its fine");
            }
        }
    }

    public void setActive(bool isActive)
    {
        if (isActive)
        {
            actionButton.gameObject.SetActive(true);
            actionButton.graphic.enabled = true;
        }
        else
        {
            actionButton.gameObject.SetActive(false);
            actionButton.graphic.enabled = false;
        }
    }

    public void Update()
    {
        if (CachedPlayer.LocalPlayer.PlayerControl.Data == null || MeetingHud.Instance || ExileController.Instance || !HasButton())
        {
            setActive(false);
            return;
        }
        setActive(hudManager.UseButton.isActiveAndEnabled || hudManager.PetButton.isActiveAndEnabled);

        actionButton.graphic.sprite = Sprite;
        if (showButtonText && buttonText != "")
        {
            actionButton.OverrideText(buttonText);
        }
        actionButton.buttonLabelText.enabled = showButtonText; // Only show the text if it's a kill button

        if (hudManager.UseButton != null)
        {
            Vector3 pos = hudManager.UseButton.transform.localPosition;
            if (mirror)
            {
                float aspect = Camera.main.aspect;
                float safeOrthographicSize = CameraSafeArea.GetSafeOrthographicSize(Camera.main);
                float xpos = 0.05f - safeOrthographicSize * aspect * 1.70f;
                pos = new Vector3(xpos, pos.y, pos.z);
            }
            actionButton.transform.localPosition = pos + PositionOffset;
            actionButton.transform.localScale = LocalScale;
        }
        if (CouldUse())
        {
            actionButton.graphic.color = actionButton.buttonLabelText.color = Palette.EnabledColor;
            actionButton.graphic.material.SetFloat("_Desat", 0f);
        }
        else
        {
            actionButton.graphic.color = actionButton.buttonLabelText.color = Palette.DisabledClear;
            actionButton.graphic.material.SetFloat("_Desat", 1f);
        }

        if (Timer >= 0)
        {
            // Make sure role draft has finished or isn't running
            if (HasEffect && isEffectActive)
            {
                Timer -= Time.deltaTime;
            }
            else if (!CachedPlayer.LocalPlayer.PlayerControl.inVent && CachedPlayer.LocalPlayer.PlayerControl.moveable)
            {
                Timer -= Time.deltaTime;
            }
        }

        if (Timer <= 0 && HasEffect && isEffectActive)
        {
            isEffectActive = false;
            actionButton.cooldownTimerText.color = Palette.EnabledColor;
            OnEffectEnds();
        }

        actionButton.SetCoolDown(Timer, (HasEffect && isEffectActive) ? EffectDuration : MaxTimer);

        // Trigger OnClickEvent if the hotkey is being pressed down
        if (hotkey.HasValue && Input.GetKeyDown(hotkey.Value)) onClickEvent();
    }
}