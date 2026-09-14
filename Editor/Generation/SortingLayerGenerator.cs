using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SafeStringGenerator.Editor.Generation
{
    internal static class SortingLayerGenerator
    {
        internal static string Generate(IEnumerable<SortingLayerDefinition> layers)
        {
            SortedDictionary<string, int> uniqueLayers = new SortedDictionary<string, int>(System.StringComparer.Ordinal);
            foreach (SortingLayerDefinition layer in layers)
            {
                if (layer.Name == null)
                {
                    continue;
                }

                if (!uniqueLayers.TryGetValue(layer.Name, out int existing) || layer.Id < existing)
                {
                    uniqueLayers[layer.Name] = layer.Id;
                }
            }

            IReadOnlyDictionary<string, string> names = IdentifierUtility.Allocate(
                uniqueLayers.Keys,
                new[] { "Names", "Ids" });
            List<string> values = new List<string>(uniqueLayers.Keys);
            values.Sort((left, right) => string.CompareOrdinal(names[left], names[right]));

            StringBuilder builder = new StringBuilder();
            builder.Append(GeneratedCode.Header);
            builder.Append("namespace ").Append(GeneratedCode.Namespace).Append("\n{\n");
            builder.Append("    public static class SortingLayers\n    {\n");
            builder.Append("        public static class Names\n        {\n");

            foreach (string value in values)
            {
                builder.Append("            public const string ").Append(names[value]).Append(" = ")
                    .Append(CSharpLiteral.Quote(value)).Append(";\n");
            }

            builder.Append("        }\n\n        public static class Ids\n        {\n");
            foreach (string value in values)
            {
                builder.Append("            public const int ").Append(names[value]).Append(" = ")
                    .Append(uniqueLayers[value].ToString(CultureInfo.InvariantCulture)).Append(";\n");
            }

            builder.Append("        }\n    }\n}\n");
            return builder.ToString();
        }
    }
}
