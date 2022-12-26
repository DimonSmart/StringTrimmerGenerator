using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DimonSmart.StringTrimmerGenerator
{
    public class StringTrimmerGeneratorContextReceiver : ISyntaxContextReceiver
    {
        public class PropertyTrimDescriptor
        {
            public PropertyTrimDescriptor(string propertyName, string propertyType)
            {
                PropertyName = propertyName;
                PropertyType = propertyType;
            }

            public string PropertyName { get; set; }
            public string PropertyType { get; set; }
        }

        public class ClassDescriptor
        {
            public ClassDescriptor(string className, string fullClassName, List<PropertyTrimDescriptor> properties)
            {
                ClassName = className;
                FullClassName = fullClassName;
                Properties = properties;
            }

            public string NameSpace { get { return string.Join(".", FullClassName.Split('.').Reverse().Skip(1).Reverse()); } }
            public string ClassName { get; set; }
            public string FullClassName { get; set; }
            public List<PropertyTrimDescriptor> Properties { get; set; }
        }

        public Dictionary<string, ClassDescriptor> MyProperties = new();
        public List<Diagnostic> Diagnostics = new();
        private ClassDeclarationSyntax? CurrentClass = null;
        private const string GenerateStringTrimmerAttributeFullName = "DimonSmart.StringTrimmer.GenerateStringTrimmerAttribute";

        public void OnVisitSyntaxNode(GeneratorSyntaxContext syntaxNode)
        {
            if (syntaxNode.Node is ClassDeclarationSyntax classDeclarationSyntax)
            {
                VisitClassDeclarationSyntax(syntaxNode, classDeclarationSyntax);
                return;
            }

            if (CurrentClass != null && syntaxNode.Node is PropertyDeclarationSyntax propertyDeclarationSyntax)
            {
                VisitPropertyDeclarationSyntax(syntaxNode, propertyDeclarationSyntax);
                return;
            }
        }

        private void VisitPropertyDeclarationSyntax(GeneratorSyntaxContext syntaxNode, PropertyDeclarationSyntax propertyDeclarationSyntax)
        {
            if (CurrentClass == null)
            {
                return;
            }

            var className = CurrentClass.Identifier.ValueText;
            var fullClassName = GetFullClassName(CurrentClass);
            var propertyName = propertyDeclarationSyntax.Identifier.ValueText;
            var propertyType = syntaxNode
                .SemanticModel
                .GetDeclaredSymbol(propertyDeclarationSyntax)!
                .Type.ToDisplayString();
            var propertyAccessibility = syntaxNode
                .SemanticModel
                .GetDeclaredSymbol(propertyDeclarationSyntax)!
                .DeclaredAccessibility;

            if (propertyAccessibility != Accessibility.Public && propertyAccessibility != Accessibility.Internal)
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

                if ((propertySymbol.GetMethod!.DeclaredAccessibility & (Accessibility.Public | Accessibility.Internal)) == 0)
                {
                    ReportUnaccessibleMethod(className, propertyName, "get", propertySymbol.GetMethod.DeclaredAccessibility);
                    return;
                }

                if ((propertySymbol.SetMethod!.DeclaredAccessibility & (Accessibility.Public | Accessibility.Internal)) == 0)
                {
                    ReportUnaccessibleMethod(className, propertyName, "set", propertySymbol.SetMethod.DeclaredAccessibility);
                    return;
                }
            }

            var propertyDescriptor = new PropertyTrimDescriptor(propertyName, propertyType);

            if (MyProperties.TryGetValue(fullClassName, out var targetClass))
            {
                targetClass.Properties.Add(propertyDescriptor);
            }
            else
            {
                MyProperties.Add(fullClassName,
                    new ClassDescriptor(
                        className,
                        fullClassName,
                        new List<PropertyTrimDescriptor> { propertyDescriptor }));
            }
        }

        private void ReportUnaccessibleMethod(string className, string propertyName, string methodName, Accessibility accessibility)
        {
            Diagnostics.Add(Diagnostic.Create(new DiagnosticDescriptor(
            id: "TRIMMER002",
            title: "Class property isn't fully accessible",
            messageFormat: "Class '{0}' has unaccessible property '{1}' with '{2}' accessibility as '{3}'",
            category: "StringTrimmerGenerator",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true), Location.None, className, propertyName, methodName, Enum.GetName(typeof(Accessibility), accessibility)));
        }

        private void VisitClassDeclarationSyntax(GeneratorSyntaxContext syntaxNode, ClassDeclarationSyntax classDeclarationSyntax)
        {
            var generateStringTrimmerAttribute = syntaxNode
                .SemanticModel
                .Compilation
                //.GetTypeByMetadataName(typeof(GenerateStringTrimmerAttribute).FullName);
                .GetTypeByMetadataName("DimonSmart.StringTrimmer.GenerateStringTrimmerAttribute");


            CurrentClass = classDeclarationSyntax;

            var markerAttributes =
               syntaxNode
               .SemanticModel
               .GetDeclaredSymbol(classDeclarationSyntax)!
               .GetAttributes()
               .Where(i => i.AttributeClass!.OriginalDefinition.ToDisplayString() == GenerateStringTrimmerAttributeFullName)
               .ToList();

            if (markerAttributes.Any())
            {
                CurrentClass = classDeclarationSyntax;
            }
            else
            {
                CurrentClass = null;
            }

            return;
        }

        private void ReportPrivateField(string className, string propertyName)
        {
            Diagnostics.Add(Diagnostic.Create(new DiagnosticDescriptor(
            id: "TRIMMER001",
            title: "Class contain a private property",
            messageFormat: "Class '{0}' contains a private field '{1}'",
            category: "StringTrimmerGenerator",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true), Location.None, className, propertyName));
        }

        private static string GetFullClassName(ClassDeclarationSyntax varClassDec)
        {
            SyntaxNode? tempCurCls = varClassDec;
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
