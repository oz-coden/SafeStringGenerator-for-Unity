using System.Collections.Generic;
using System.Text;

namespace SafeStringGenerator.Editor.Generation
{
    internal static class TagGenerator
    {
        internal static string Generate(IEnumerable<string> tags)
        {
            IReadOnlyDictionary<string, string> names = IdentifierUtility.Allocate(tags, new[] { "Tags" });
            List<string> values = new List<string>(names.Keys);
            values.Sort((left, right) => string.CompareOrdinal(names[left], names[right]));

            StringBuilder builder = new StringBuilder();
            builder.Append(GeneratedCode.Header);
            builder.Append("namespace ").Append(GeneratedCode.Namespace).Append("\n{\n");
            builder.Append("    public static class Tags\n    {\n");

            foreach (string value in values)
            {
                builder.Append("        public const string ").Append(names[value]).Append(" = ")
                    .Append(CSharpLiteral.Quote(value)).Append(";\n");
            }

            builder.Append("    }\n}\n");
            return builder.ToString();
        }
    }
}
