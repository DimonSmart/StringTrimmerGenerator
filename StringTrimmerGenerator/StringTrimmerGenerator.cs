using System.Collections.Immutable;
using System.Linq;
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
            //    if (!Debugger.IsAttached)
            //    {
            //        Debugger.Launch();
            //    }

            context.RegisterForPostInitialization((i) =>
            {
                i.AddSource("GenerateStringTrimmerAttribute.g.cs", GenerateStringTrimmerAttributeText);
                i.AddSource("StringTrimmerExtensionClass.g.cs", StringTrimmerExtensionClassText);
            });

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

            var classesToTrim = receiver.MyProperties.Keys.ToArray();

            var sb = new StringBuilder();

            foreach (var cls in receiver.MyProperties.Keys)
            {
                var classDescriptor = receiver.MyProperties[cls];
                sb.AppendLine($"namespace {receiver.MyProperties[cls].NameSpace}");
                sb.AppendLine(@"{");

                sb.AppendLine($"public static class {classDescriptor.ClassName}_StringTrimmerExtension");
                sb.AppendLine(@"{");

                sb.AppendLine($"  public static {classDescriptor.ClassName} Trim(this {classDescriptor.ClassName} classToTrim)");
                sb.AppendLine("  {");
                foreach (var prop in receiver.MyProperties[cls].Properties)
                {
                    if (prop.PropertyType != "string" && !classesToTrim.Contains(prop.PropertyType))
                    {
                        continue;
                    }

                    // TODO: Handle options
                    sb.AppendLine($"    classToTrim.{prop.PropertyName} = classToTrim.{prop.PropertyName}.Trim();");
                }
                sb.AppendLine("    return classToTrim;");
                sb.AppendLine("  }");

                sb.AppendLine("}");
                sb.AppendLine("}");
            }

            context.AddSource("StringTrimmerExtension.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8, SourceHashAlgorithm.Sha256));
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
    [Flags]
    public enum TrimType
    {
        None,
        Left = 1,
        Right = 2,
        LeftAndRignt = Left | Right,
        Seq = 4,
        All = LeftAndRignt | Seq,
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    [System.Diagnostics.Conditional(""StringTrimmerGenerator_DEBUG"")]
    sealed class GenerateStringTrimmerAttribute : Attribute
{
	TrimType _trimType;
	public GenerateStringTrimmerAttribute(TrimType trimType = TrimType.All)
	{
		_trimType = trimType;
	}
}
}
";
    }
}
