namespace RebuildUs.Modules;

public class CustomOptionBlank : CustomOption
{
    public CustomOptionBlank(CustomOption parent)
    {
        Parent = parent;
        Id = -1;
        NameKey = "";
        IsHeader = false;
        Children = [];
        Selections = new string[] { "" };
        AllOptions.Add(this);
    }

    public override int GetSelection()
    {
        return 0;
    }

    public override bool GetBool()
    {
        return true;
    }

    public override float GetFloat()
    {
        return 0f;
    }

    public override string GetString()
    {
        return "";
    }

    public override void UpdateSelection(int newSelection, RoleTypes icon, bool notifyUsers = true)
    {
        return;
    }
}