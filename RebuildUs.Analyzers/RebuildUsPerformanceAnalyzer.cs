using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RebuildUs.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RebuildUsPerformanceAnalyzer : DiagnosticAnalyzer
{
    private const string CATEGORY = "Performance";

    // RUS001: String +
    public static readonly DiagnosticDescriptor RUS001 = new(
        "RUS001",
        "String concatenation using + is forbidden",
        "Do not use + for string concatenation. Use StringBuilder or string.Format instead.",
        CATEGORY,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    // RUS002: Interpolated string $""
    public static readonly DiagnosticDescriptor RUS002 = new(
        "RUS002",
        "String interpolation is forbidden",
        "Do not use string interpolation ($). Use StringBuilder or string.Format instead.",
        CATEGORY,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    // RUS003: Allocation in hot paths (Update/FixedUpdate)
    public static readonly DiagnosticDescriptor RUS003 = new(
        "RUS003",
        "Object allocation in hot path",
        "Do not allocate objects in Update/FixedUpdate. Reuse static buffers or fields.",
        CATEGORY,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    // RUS004: Standard Cast<T> usage
    public static readonly DiagnosticDescriptor RUS004 = new(
        "RUS004",
        "Standard Cast<T> is slow",
        "Use CastFast<T>() from Il2CppHelpers instead of standard Cast<T>()",
        CATEGORY,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    // RUS005: Standard Singleton usage
    public static readonly DiagnosticDescriptor RUS005 = new(
        "RUS005",
        "Standard Singleton access is slow",
        "Use FastDestroyableSingleton<T>.Instance instead of DestroyableSingleton<T>.Instance",
        CATEGORY,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(RUS001, RUS002, RUS003, RUS004, RUS005);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeAddExpression, SyntaxKind.AddExpression);
        context.RegisterSyntaxNodeAction(AnalyzeInterpolatedString, SyntaxKind.InterpolatedStringExpression);
        context.RegisterSyntaxNodeAction(AnalyzeObjectCreation, SyntaxKind.ObjectCreationExpression);
        context.RegisterSyntaxNodeAction(AnalyzeMemberAccess, SyntaxKind.SimpleMemberAccessExpression);
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private void AnalyzeAddExpression(SyntaxNodeAnalysisContext context)
    {
        var binary = (BinaryExpressionSyntax)context.Node;
        var typeInfo = context.SemanticModel.GetTypeInfo(binary);
        if (typeInfo.Type?.SpecialType == SpecialType.System_String)
        {
            context.ReportDiagnostic(Diagnostic.Create(RUS001, binary.OperatorToken.GetLocation()));
        }
    }

    private void AnalyzeInterpolatedString(SyntaxNodeAnalysisContext context)
    {
        context.ReportDiagnostic(Diagnostic.Create(RUS002, context.Node.GetLocation()));
    }

    private void AnalyzeObjectCreation(SyntaxNodeAnalysisContext context)
    {
        var method = GetEnclosingMethod(context.Node);
        if (method != null && IsHotPath(method))
        {
            context.ReportDiagnostic(Diagnostic.Create(RUS003, context.Node.GetLocation()));
        }
    }

    private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        var symbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;

        if (symbol?.Name == "Cast" && symbol.ContainingType.Name == "Il2CppObjectBase")
        {
            context.ReportDiagnostic(Diagnostic.Create(RUS004, invocation.GetLocation()));
        }

        // Hot path allocations for arrays etc
        if (invocation.Expression is MemberAccessExpressionSyntax memberAccess && memberAccess.Name.Identifier.Text == "ToArray")
        {
            var method = GetEnclosingMethod(context.Node);
            if (method != null && IsHotPath(method))
            {
                context.ReportDiagnostic(Diagnostic.Create(RUS003, invocation.GetLocation()));
            }
        }
    }

    private void AnalyzeMemberAccess(SyntaxNodeAnalysisContext context)
    {
        var memberAccess = (MemberAccessExpressionSyntax)context.Node;
        if (memberAccess.Name.Identifier.Text == "Instance")
        {
            var symbol = context.SemanticModel.GetSymbolInfo(memberAccess).Symbol;
            if (symbol?.ContainingType.Name == "DestroyableSingleton")
            {
                context.ReportDiagnostic(Diagnostic.Create(RUS005, memberAccess.GetLocation()));
            }
        }
    }

    private static MethodDeclarationSyntax? GetEnclosingMethod(SyntaxNode node)
    {
        return node.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
    }

    private static bool IsHotPath(MethodDeclarationSyntax method)
    {
        var name = method.Identifier.Text;
        return name == "Update" || name == "FixedUpdate" || name == "LateUpdate";
    }
}
