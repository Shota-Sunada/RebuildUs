namespace RebuildUs.Features.CustomOptions;

internal sealed class CustomNumberOption : CustomOption<float>
{
    internal CustomNumberOption(int id,
                                CustomOptionType type,
                                TrKey nameKey,
                                float[] selections,
                                float defaultValue,
                                CustomOption parent,
                                bool hideIfParentEnabled,
                                TrKey format,
                                Color color,
                                CustomOptionHeader header = null) :
        base(id,
        type,
        nameKey,
        selections,
        defaultValue,
        parent,
        hideIfParentEnabled,
        format,
        color,
        header)
    { }

    internal override OptionBehaviour CreateOptionBehaviour(GameOptionsMenu menu)
    {
        return UnityObject.Instantiate(menu.numberOptionOrigin, menu.settingsContainer);
    }

    internal override void InitializeOptionBehaviour(OptionBehaviour optionBehaviour)
    {
        if (optionBehaviour is not NumberOption numberOption)
        {
            return;
        }

        numberOption.OnValueChanged = new Action<OptionBehaviour>(_ => { });
        ApplyTitleText(numberOption.TitleText);
        numberOption.Value = GetFloat();
        numberOption.ValueText.text = string.Format("{0}{1}", GetValue()?.ToString() ?? string.Empty, Format == TrKey.None ? string.Empty : Tr.Get(Format));
    }

    internal override void SyncOptionBehaviourValue()
    {
        if (_optionBehavior is not NumberOption numberOption)
        {
            return;
        }

        numberOption.Value = GetFloat();
        numberOption.ValueText.text = string.Format("{0}{1}", GetValue()?.ToString() ?? string.Empty, Format == TrKey.None ? string.Empty : Tr.Get(Format));
    }
}