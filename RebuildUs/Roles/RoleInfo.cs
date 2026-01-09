using RebuildUs.Localization;

namespace RebuildUs.Roles;

public class RoleInfo
{
    public Color Color;
    public virtual string Name { get { return Tr.Get(NameKey); } }
    public virtual string NameColored { get { return Helpers.Cs(Color, Name); } }
    public virtual string IntroDescription { get { return Tr.Get(("IntroDesc", NameKey)); } }
    public virtual string ShortDescription { get { return Tr.Get(("ShortDesc", NameKey)); } }
    public virtual string FullDescription { get { return Tr.Get(("FullDesc", NameKey)); } }
    // public virtual string RoleOptions { get { return CustomOption.}}
    public bool Enabled { get { return Helpers.RolesEnabled && (BaseOption == null /*|| BaseOption.Enabled*/); } }

    public string NameKey;
    public ERoleType RoleType;
    private CustomOption BaseOption;

    public RoleInfo(string nameKey, Color color, CustomOption baseOption, ERoleType roleType)
    {
        NameKey = nameKey;
        Color = color;
        BaseOption = baseOption;
        RoleType = roleType;
    }
}