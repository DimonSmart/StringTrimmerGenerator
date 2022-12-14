using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace DimonSmart.StringTrimmerGenerator
{
    [Generator]
    public partial class StringTrimmerGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // Register the attribute source
            context.RegisterForPostInitialization((i) =>
            {
                i.AddSource("GenerateStringTrimmerAttribute.g.cs", GenerateStringTrimmerAttributeText);
                i.AddSource("StringTrimmerExtensionClass.g.cs", StringTrimmerExtensionClassText);
            });

            // Register a syntax receiver that will be created for each generation pass
            context.RegisterForSyntaxNotifications(() => new StringTrimmerGeneratorContextReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxContextReceiver is StringTrimmerGeneratorContextReceiver receiver))
                return;

            foreach (var diagnostic in receiver.Diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }

            var sb = new StringBuilder();

            foreach (var cls in receiver.MyProperties.Keys)
            {
                sb.AppendLine($"namespace {receiver.MyProperties[cls].NameSpace}");
                sb.AppendLine(@"{");

                sb.AppendLine($"public static class {cls}_StringTrimmerExtension");
                sb.AppendLine(@"{");

                sb.AppendLine($"  public static {cls} Trim(this {cls} classToTrim)");
                sb.AppendLine("  {");
                foreach (var prop in receiver.MyProperties[cls].Properties)
                {
                    // TODO: Handle options
                    sb.AppendLine($"    classToTrim.{prop.PropertyName} = classToTrim.{prop.PropertyName}.Trim();");
                }
                sb.AppendLine("    return classToTrim;");
                sb.AppendLine("  }");

                sb.AppendLine("}");
                sb.AppendLine("}");
            }

            context.AddSource("StringTrimmerExtension.cs", SourceText.From(sb.ToString(), Encoding.UTF8, SourceHashAlgorithm.Sha256));
        }

        public const string StringTrimmerExtensionClassText = @"
namespace DimonSmart.StringTrimmer
{
public static class StringTrimmerExtension
{
    public static string RemoveConsecutiveSpaces(this string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return s;
        }

        var lenght = s.Length;
        var characters = s.ToCharArray();
        int lastChar = 0;
        var spaceAdded = false;
        for (int i = 0; i < lenght; i++)
        {
            var ch = characters[i];

            if (char.IsWhiteSpace(ch))
            {
                if (spaceAdded)
                {
                    continue;
                }
                spaceAdded = true;
            }
            else
            {
                spaceAdded = false;
            }

            characters[lastChar++] = ch;

        }
        return new string(characters, 0, lastChar);
    }
}
}
";

        public const string GenerateStringTrimmerAttributeText = @"
using System;
namespace DimonSmart.StringTrimmer
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    [System.Diagnostics.Conditional(""StringTrimmerGenerator_DEBUG"")]
    sealed class GenerateStringTrimmerAttribute : Attribute
    {
        public GenerateStringTrimmerAttribute()
        {
        }
    }
}
";
    }
}
