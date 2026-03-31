namespace RebuildUs.Features.CustomOptions;

internal sealed class CustomToggleOption : CustomOption<bool>
{
    internal CustomToggleOption(COID id, COType type, TrKey nameKey, bool defaultValue, CustomOption parent, bool hideIfParentEnabled, TrKey format, Color color, CustomOptionHeader header = null)
    : base(id, type, nameKey, [false, true], defaultValue, parent, hideIfParentEnabled, format, color, header)
    { }

    internal override OptionBehaviour CreateOptionBehaviour(GameOptionsMenu menu)
    {
        return UnityObject.Instantiate(menu.checkboxOrigin, menu.settingsContainer);
    }

    internal override void InitializeOptionBehaviour(OptionBehaviour optionBehaviour)
    {
        if (optionBehaviour is not ToggleOption toggleOption)
        {
            return;
        }

        toggleOption.OnValueChanged = new Action<OptionBehaviour>(_ => { });
        ApplyTitleText(toggleOption.TitleText);
        toggleOption.CheckMark.enabled = GetBool();
    }

    internal override void SyncOptionBehaviourValue()
    {
        if (_optionBehavior is not ToggleOption toggleOption)
        {
            return;
        }

        toggleOption.CheckMark.enabled = GetBool();
    }
}