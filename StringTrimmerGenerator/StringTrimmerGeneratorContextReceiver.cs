using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DimonSmart.StringTrimmerGenerator
{
    class StringTrimmerGeneratorContextReceiver : ISyntaxContextReceiver
    {
        public class PropertyTrimDescriptor
        {
            public string PropertyName { get; set; }
            public TrimType TrimType { get; set; }
        }

        public class ClassDescriptor
        {
            public string NameSpace { get { return string.Join(".", FullClassName.Split('.').Reverse().Skip(1).Reverse()); } }
            public string ClassName { get; set; }
            public string FullClassName { get; set; }
            public List<PropertyTrimDescriptor> Properties { get; set; } = new List<PropertyTrimDescriptor>();
        }

        public Dictionary<string, ClassDescriptor> MyProperties = new();
        public List<Diagnostic> Diagnostics = new List<Diagnostic>();
        private ClassDeclarationSyntax CurrentClass = null;

        /// <summary>
        /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
        /// </summary>
        public void OnVisitSyntaxNode(GeneratorSyntaxContext syntaxNode)
        {
            if (syntaxNode.Node is ClassDeclarationSyntax { AttributeLists.Count: > 0 } classDeclarationSyntax)
            {
                if (!Debugger.IsAttached)
                {
                    Debugger.Launch();
                }
                CurrentClass = classDeclarationSyntax;
                return;
            }

            if (syntaxNode.Node is PropertyDeclarationSyntax propertyDeclarationSyntax)
            {
                var className = CurrentClass.Identifier.ValueText;
                var fullClassName = GetFullClassName(CurrentClass);
                var propertyName = propertyDeclarationSyntax.Identifier.ValueText;

                var propertyAccessibility = syntaxNode.SemanticModel.GetDeclaredSymbol(propertyDeclarationSyntax).DeclaredAccessibility;

                // Skip for non public properties
                if (propertyAccessibility != Accessibility.Public)
                {
                    ReportPrivateField(className, propertyName);
                    return;
                }

                if (syntaxNode.SemanticModel.GetDeclaredSymbol(propertyDeclarationSyntax) is IPropertySymbol propertySymbol)
                {
                    if (propertySymbol.IsWriteOnly || propertySymbol.IsReadOnly)
                    {
                        return;
                    }
                }


                var propertyDescriptor = new PropertyTrimDescriptor { PropertyName = propertyName, TrimType = TrimType.All };

                if (MyProperties.TryGetValue(className, out var targetClass))
                {
                    targetClass.Properties.Add(propertyDescriptor);
                }
                else
                {
                    MyProperties.Add(className, new ClassDescriptor
                    {
                        ClassName = className,
                        FullClassName = fullClassName,
                        Properties = new List<PropertyTrimDescriptor> { propertyDescriptor }
                    });
                }
            }
        }

        private void ReportPrivateField(string className, string propertyName)
        {
            Diagnostics.Add(Diagnostic.Create(new DiagnosticDescriptor(
            id: "TRIMMER001",
            title: "Class contain a private field",
            messageFormat: "Class '{0}' contains a private field '{1}'",
            category: "StringTrimmerGenerator",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true), Location.None, className, propertyName));
        }

        public static string GetFullClassName(ClassDeclarationSyntax varClassDec)
        {
            SyntaxNode tempCurCls = varClassDec;
            var tempFullName = new Stack<string>();

            do
            {
                if (tempCurCls.IsKind(SyntaxKind.ClassDeclaration))
                {
                    tempFullName.Push(((ClassDeclarationSyntax)tempCurCls).Identifier.ToString());
                }
                else if (tempCurCls.IsKind(SyntaxKind.NamespaceDeclaration))
                {
                    tempFullName.Push(((NamespaceDeclarationSyntax)tempCurCls).Name.ToString());
                }

                tempCurCls = tempCurCls.Parent;
            } while (tempCurCls != null);

            return string.Join(".", tempFullName);
        }
    }

}
