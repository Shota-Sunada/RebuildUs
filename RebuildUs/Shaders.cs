namespace RebuildUs;

internal static class Shaders
{
    internal static readonly int BodyColor = Shader.PropertyToID("_BodyColor");
    internal static readonly int BackColor = Shader.PropertyToID("_BackColor");
    internal static readonly int VisorColor = Shader.PropertyToID("_VisorColor");
    internal static readonly int Outline = Shader.PropertyToID("_Outline");
    internal static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");
    internal static readonly int Desat = Shader.PropertyToID("_Desat");
    internal static readonly int MainTex = Shader.PropertyToID("_MainTex");
    internal static readonly int StencilComp = Shader.PropertyToID("_StencilComp");
    internal static readonly int Stencil = Shader.PropertyToID("_Stencil");
    internal static readonly int Color = Shader.PropertyToID("_Color");
    internal static readonly int AddColor = Shader.PropertyToID("_AddColor");
}