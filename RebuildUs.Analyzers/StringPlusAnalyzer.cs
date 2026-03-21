using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RebuildUs.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class StringPlusAnalyzer : DiagnosticAnalyzer
{
    public const string DIAGNOSTIC_ID = "RUS001";
    private static readonly string Title = "String concatenation using + is forbidden";
    private static readonly string MessageFormat = "Do not use + for string concatenation. Use StringBuilder or string.Format instead.";
    private static readonly string Description = "Concatenating strings with + operator in hot paths causes unnecessary GC allocations in IL2CPP.";
    private const string CATEGORY = "Performance";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        DIAGNOSTIC_ID,
        Title,
        MessageFormat,
        CATEGORY,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.AddExpression);
    }

    private void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var binaryExpression = (BinaryExpressionSyntax)context.Node;

        // 数値の足し算などを除外するため、型を確認
        var typeInfo = context.SemanticModel.GetTypeInfo(binaryExpression);
        if (typeInfo.Type?.SpecialType == SpecialType.System_String)
        {
            var diagnostic = Diagnostic.Create(Rule, binaryExpression.OperatorToken.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
