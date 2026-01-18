using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Formatting;

namespace RefactorTool
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: RefactorTool <solution-path>");
                return;
            }

            string solutionPath = args[0];
            using var workspace = MSBuildWorkspace.Create();

            workspace.WorkspaceFailed += (sender, e) =>
            {
                if (e.Diagnostic.Kind == WorkspaceDiagnosticKind.Failure)
                    Console.WriteLine($"Workspace error: {e.Diagnostic.Message}");
            };

            Console.WriteLine($"Loading solution: {solutionPath}");
            var solution = await workspace.OpenSolutionAsync(solutionPath);
            Console.WriteLine("Solution loaded.");

            var newSolution = solution;

            foreach (var projectId in solution.ProjectIds)
            {
                var project = newSolution.GetProject(projectId);
                if (project == null || project.Name == "RefactorTool") continue;

                Console.WriteLine($"Processing project: {project.Name}");

                var compilation = await project.GetCompilationAsync();
                if (compilation == null) continue;

                var symbolsToRename = new List<ISymbol>();

                foreach (var document in project.Documents)
                {
                    var semanticModel = await document.GetSemanticModelAsync();
                    var root = await document.GetSyntaxRootAsync();
                    if (semanticModel == null || root == null) continue;

                    var nodes = root.DescendantNodes();
                    foreach (var node in nodes)
                    {
                        ISymbol symbol = semanticModel.GetDeclaredSymbol(node);

                        if (symbol != null && ShouldRename(symbol))
                        {
                            if (!symbolsToRename.Any(s => s.Name == symbol.Name && s.Kind == symbol.Kind && s.ContainingSymbol?.Name == symbol.ContainingSymbol?.Name))
                            {
                                symbolsToRename.Add(symbol);
                            }
                        }
                    }
                }

                // Sort symbols: Local, Parameter, Field, Property, Method
                var sortedSymbols = symbolsToRename.OrderBy(s => s.Kind switch
                {
                    SymbolKind.Local => 0,
                    SymbolKind.Parameter => 1,
                    SymbolKind.Field => 2,
                    SymbolKind.Property => 3,
                    SymbolKind.Method => 4,
                    _ => 5
                }).ToList();

                Console.WriteLine($"Found {sortedSymbols.Count} symbols to rename in {project.Name}");

                foreach (var symbol in sortedSymbols)
                {
                    string oldName = symbol.Name;
                    string newName;
                    if (symbol.Kind == SymbolKind.Local || symbol.Kind == SymbolKind.Parameter)
                    {
                        newName = char.ToLower(oldName[0]) + oldName.Substring(1);
                    }
                    else
                    {
                        newName = char.ToUpper(oldName[0]) + oldName.Substring(1);
                    }

                    if (oldName == newName) continue;

                    Console.WriteLine($"Renaming {oldName} to {newName} in {symbol.ContainingSymbol?.Name}");

                    try
                    {
                        ISymbol currentSymbol = await SymbolFinder.FindSourceDefinitionAsync(symbol, newSolution, default);
                        if (currentSymbol == null)
                        {
                            Console.WriteLine($"Could not find current definition for {oldName}");
                            continue;
                        }

                        newSolution = await Renamer.RenameSymbolAsync(newSolution, currentSymbol, newName, null);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error renaming {oldName}: {ex.Message}");
                    }
                }
            }

            Console.WriteLine("Cleaning up and formatting documents...");
            foreach (var projectId in newSolution.ProjectIds)
            {
                var project = newSolution.GetProject(projectId);
                if (project == null || project.Name == "RefactorTool") continue;

                foreach (var documentId in project.DocumentIds)
                {
                    var document = newSolution.GetDocument(documentId);
                    if (document == null) continue;

                    document = await CleanupDocumentAsync(document);
                    newSolution = document.Project.Solution;
                }
            }

            if (workspace.TryApplyChanges(newSolution))
            {
                Console.WriteLine("Changes applied successfully.");
            }
            else
            {
                Console.WriteLine("Failed to apply changes.");
            }
        }

        static bool ShouldRename(ISymbol symbol)
        {
            if (string.IsNullOrEmpty(symbol.Name)) return false;

            // Only rename variables, fields, parameters, methods, properties
            bool isValidKind = symbol.Kind == SymbolKind.Local ||
                               symbol.Kind == SymbolKind.Field ||
                               symbol.Kind == SymbolKind.Parameter ||
                               symbol.Kind == SymbolKind.Method ||
                               symbol.Kind == SymbolKind.Property;
            if (!isValidKind) return false;

            bool isLocalOrParam = symbol.Kind == SymbolKind.Local || symbol.Kind == SymbolKind.Parameter;
            bool startsWithUpper = char.IsUpper(symbol.Name[0]);
            bool startsWithLower = char.IsLower(symbol.Name[0]);

            // If it's a local/param and starts with Upper, it needs to be Lower.
            // If it's a field/prop/method and starts with Lower, it needs to be Upper.
            bool needsChange = (isLocalOrParam && startsWithUpper) || (!isLocalOrParam && startsWithLower);
            if (!needsChange) return false;

            // Exclude external symbols
            if (symbol.Locations.Any(l => !l.IsInSource)) return false;

            // Exclude overrides
            if (symbol is IMethodSymbol method && method.IsOverride) return false;
            if (symbol is IPropertySymbol prop && prop.IsOverride) return false;

            // Exclude property accessors and other special names
            if (symbol is IMethodSymbol m && (m.MethodKind != MethodKind.Ordinary && m.MethodKind != MethodKind.LocalFunction)) return false;

            // Exclude implicitly declared symbols
            if (symbol.IsImplicitlyDeclared) return false;

            return true;
        }

        static async Task<Document> CleanupDocumentAsync(Document document)
        {
            var root = await document.GetSyntaxRootAsync();
            var model = await document.GetSemanticModelAsync();
            if (root != null && model != null)
            {
                // Remove unused usings (IDE0005)
                var diagnostics = model.GetDiagnostics();
                var unusedUsings = diagnostics
                    .Where(d => d.Id == "IDE0005" || d.Descriptor.CustomTags.Contains(WellKnownDiagnosticTags.Unnecessary))
                    .Select(d => root.FindNode(d.Location.SourceSpan))
                    .OfType<UsingDirectiveSyntax>()
                    .Distinct()
                    .ToList();

                if (unusedUsings.Any())
                {
                    root = root.RemoveNodes(unusedUsings, SyntaxRemoveOptions.KeepNoTrivia);
                    if (root != null)
                    {
                        document = document.WithSyntaxRoot(root);
                    }
                }
            }

            // Format (Respects .editorconfig for {} blank lines and consecutive empty lines)
            document = await Formatter.FormatAsync(document);
            return document;
        }
    }
}