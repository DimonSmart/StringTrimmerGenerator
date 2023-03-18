using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace DimonSmart.StringTrimmerGenerator
{

    [Generator]
    public partial class StringTrimmerGenerator : 
        ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            #region DebuggerLauncher
            // if (!Debugger.IsAttached)
            // {
            //     Debugger.Launch();
            // }
            #endregion

            context.RegisterForPostInitialization((i) =>
            {
                i.AddSource("GenerateStringTrimmerAttribute.g.cs", GenerateStringTrimmerAttributeText);
                i.AddSource("StringTrimmerExtensionClass.g.cs", StringTrimmerExtensionClassText);
            });

            context.RegisterForSyntaxNotifications(
                () => new StringTrimmerGeneratorContextReceiver());
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
            var functions = new List<(string fnName, string fn, string comment)> {
                ("Trim", "Trim()", "Removes all leading and trailing white-space characters from all class string properties" ),
                ("TrimEnd", "TrimEnd()", "Removes all trailing white-space characters from all class strings properties"),
                ("TrimStart", "TrimStart()", "Removes all leading white-space characters from all class string properties"),
                ("TrimExtraSpaces", "TrimExtraSpaces()", "Removes all sequental white-space characters and replace them with only one from all class string properties"),
                ("TrimAll", "Trim()?.TrimExtraSpaces()", "Removes all leading and trailing white-space characters as well as any sequential white-space characters from all class string properties and replaces them with only one space.") };

            var sb = new TabbedStringBuilder();
            sb.AppendLine($"using DimonSmart.StringTrimmer;");
            foreach (var cls in receiver.MyProperties.Keys)
            {
                var classDescriptor = receiver.MyProperties[cls];

                sb.AppendLine($"namespace {receiver.MyProperties[cls].NameSpace}");
                sb.AppendLine(@"{");
                using (new Indent(sb))
                {
                    sb.AppendLine($"public static class {classDescriptor.ClassName}_StringTrimmerExtension");
                    sb.AppendLine(@"{");

                    foreach (var (fnName, fn, comment) in functions)
                    {
                        using (new Indent(sb))
                        {
                            sb.AppendLine($"/// <summary>");
                            sb.AppendLine($"/// {comment}");
                            sb.AppendLine($"/// </summary>");
                            sb.AppendLine($"/// <param name=\"classToTrim\"></param>");
                            sb.AppendLine($"/// <returns>classToTrim</returns>");

                            sb.AppendLine($"public static {classDescriptor.ClassName} {fnName}(this {classDescriptor.ClassName} classToTrim)");
                            sb.AppendLine("{");
                            using (new Indent(sb))
                            {
                                foreach (var prop in receiver.MyProperties[cls].Properties)
                                {
                                    if (prop.PropertyType != "string" && !classesToTrim.Contains(prop.PropertyType))
                                    {
                                        continue;
                                    }

                                    sb.AppendLine($"classToTrim.{prop.PropertyName} = classToTrim?.{prop.PropertyName}?.{fn};");
                                }
                                sb.AppendLine("return classToTrim;");
                            }
                            sb.AppendLine("}");
                        }
                    }
                    sb.AppendLine("}");
                }
                sb.AppendLine("}");
                sb.AppendLine(string.Empty);
            }

            context.AddSource("StringTrimmerExtension.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8, SourceHashAlgorithm.Sha256));
        }

        public const string StringTrimmerExtensionClassText = @"
namespace DimonSmart.StringTrimmer
{
    public static class StringTrimmerExtension
    {
        public static string Trim(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }
            return s.Trim();
        }
        public static string TrimStart(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }
            return s.TrimStart();
        }
        public static string TrimEnd(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }
            return s.TrimEnd();
        }

        public static string TrimExtraSpaces(this string s)
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
