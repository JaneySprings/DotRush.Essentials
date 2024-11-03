
using System.Xml;
using DotRush.Essentials.Tools.Logging;
using DotRush.Essentials.Tools.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Range = DotRush.Essentials.Tools.Models.Range;

namespace DotRush.Essentials.Tools;

public static class TestTools {
    
    public static IEnumerable<TestModel> DiscoverTests(string[] workspaceFolders) {
        return workspaceFolders.SelectMany(DiscoverTests);
    }
    public static IEnumerable<TestModel> DiscoverTests(string workspaceFolder) {
        var result = new List<TestModel>();
        var testProjectFiles = Directory.GetFiles(workspaceFolder, "*.*Tests.*", SearchOption.AllDirectories);
        foreach (var testProjectFile in testProjectFiles) {
            var testProjectDirectory = Path.GetDirectoryName(testProjectFile)!;
            var testFixtures = GetFixtures(testProjectDirectory);
            if (!testFixtures.Any())
                continue;

            result.Add(new TestModel {
                Name = Path.GetFileNameWithoutExtension(testProjectFile),
                FilePath = testProjectFile,
                Children = testFixtures
            });
        }
        return result;
    }

    public static IEnumerable<TestResultModel> ProcessTrxFile(string reportFilePath) {
        var result = new List<TestResultModel>();
        var doc = new XmlDocument();
        doc.Load(reportFilePath);
        var testResults = doc.GetElementsByTagName("UnitTestResult");
        if (testResults == null)
            return result;

        foreach (XmlNode testResult in testResults) {
            try {
                result.Add(new TestResultModel {
                    FullName = testResult.Attributes["testName"].Value,
                    State = testResult.Attributes["outcome"].Value,
                    Duration = testResult.Attributes["duration"].Value,
                    ErrorMessage = testResult.SelectSingleNode("*[local-name()='Output']/*[local-name()='ErrorInfo']/*[local-name()='Message']")?.InnerText,
                });
            } catch (Exception e) {
                CurrentSessionLogger.Error(e);
            }
        }
        return result;
    }

    private static List<TestModel> GetFixtures(string projectPath) {
        var result = new List<TestModel>();
        var documentPaths = Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories);
        foreach (var documentPath in documentPaths) {
            var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(documentPath));
            var walker = new TestDiscoverySyntaxWalker(documentPath);
            walker.Visit(tree.GetRoot());
            foreach (var fixture in walker.Fixtures) {
                if (fixture.Children != null && fixture.Children.Any())
                    result.Add(fixture);
            }
        }
        return result;
    }
}

public class TestDiscoverySyntaxWalker : CSharpSyntaxWalker {
    private string currentNamespace;
    private string currentClass;
    private string filePath;
    private TestModel? currentFixture;

    public List<TestModel> Fixtures { get; }

    public TestDiscoverySyntaxWalker(string documentPath) {
        Fixtures = new List<TestModel>();   
        currentNamespace = string.Empty;
        currentClass = string.Empty;
        currentFixture = null;
        filePath = documentPath;
    }

    public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node) {
        currentNamespace = node.Name.ToString();
        base.VisitNamespaceDeclaration(node);
    }
    public override void VisitFileScopedNamespaceDeclaration(FileScopedNamespaceDeclarationSyntax node) {
        currentNamespace = node.Name.ToString();
        base.VisitFileScopedNamespaceDeclaration(node);
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node) {
        currentClass = node.Identifier.Text;
        currentFixture = new TestModel {
            Name = currentClass,
            FullName = $"{currentNamespace}.{currentClass}",
            FilePath = filePath,
            Children = new List<TestModel>(),
            Range = GetRange(node)
        };
        Fixtures.Add(currentFixture);
        base.VisitClassDeclaration(node);
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node) {
        base.VisitMethodDeclaration(node);
        if (node.AttributeLists.Any(p => p.Attributes.Any(a => a.Name.ToString().EndsWith("Fact")))) {
            var testModel = new TestModel {
                Name = node.Identifier.Text,
                FullName = $"{currentNamespace}.{currentClass}.{node.Identifier.Text}",
                FilePath = filePath,
                Range = GetRange(node)
            };
            currentFixture?.Children?.Add(testModel);
        }
    }

    private Range GetRange(SyntaxNode node) {
        return new Range {
            Start = new Position {
                Line = node.GetLocation().GetLineSpan().StartLinePosition.Line,
                Character = node.GetLocation().GetLineSpan().StartLinePosition.Character
            },
            End = new Position {
                Line = node.GetLocation().GetLineSpan().EndLinePosition.Line,
                Character = node.GetLocation().GetLineSpan().EndLinePosition.Character
            }
        };
    }
}