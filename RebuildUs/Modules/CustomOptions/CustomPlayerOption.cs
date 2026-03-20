namespace RebuildUs.Modules.CustomOptions;

internal sealed class CustomPlayerOption : CustomOption<int>
{
    internal CustomPlayerOption(int id,
                                CustomOptionType type,
                                TrKey nameKey,
                                int[] playerIds,
                                int defaultSelection,
                                CustomOption parent,
                                bool hideIfParentEnabled,
                                string format,
                                Color color,
                                CustomOptionHeader header = null) : base(id,
        type,
        nameKey,
        playerIds,
        defaultSelection,
        parent,
        hideIfParentEnabled,
        format,
        color,
        header)
    { }

    internal override OptionBehaviour CreateOptionBehaviour(GameOptionsMenu menu)
    {
        return UnityObject.Instantiate(menu.playerOptionOrigin, menu.settingsContainer);
    }

    internal override void InitializeOptionBehaviour(OptionBehaviour optionBehaviour)
    {
        if (optionBehaviour is not PlayerOption playerOption)
        {
            return;
        }

        playerOption.OnValueChanged = new Action<OptionBehaviour>(_ => { });
        ApplyTitleText(playerOption.TitleText);
        var pid = (int)GetValue();
        playerOption.Value = pid;
        var player = GameData.Instance.GetPlayerById((byte)pid);
        playerOption.ValueText.text = player != null ? player.PlayerName : "Disconnected";
    }

    internal override void SyncOptionBehaviourValue()
    {
        if (_optionBehavior is not PlayerOption playerOption)
        {
            return;
        }

        var pid = (int)GetValue();
        playerOption.Value = pid;
        var player = GameData.Instance.GetPlayerById((byte)pid);
        playerOption.ValueText.text = player != null ? player.PlayerName : "Disconnected";
    }
}