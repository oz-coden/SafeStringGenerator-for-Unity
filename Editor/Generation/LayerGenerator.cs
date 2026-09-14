using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SafeStringGenerator.Editor.Generation
{
    internal static class LayerGenerator
    {
        internal static string Generate(IEnumerable<LayerDefinition> layers)
        {
            SortedDictionary<string, int> uniqueLayers = new SortedDictionary<string, int>(System.StringComparer.Ordinal);
            foreach (LayerDefinition layer in layers)
            {
                if (layer.Name == null)
                {
                    continue;
                }

                if (!uniqueLayers.TryGetValue(layer.Name, out int existing) || layer.Index < existing)
                {
                    uniqueLayers[layer.Name] = layer.Index;
                }
            }

            IReadOnlyDictionary<string, string> names = IdentifierUtility.Allocate(uniqueLayers.Keys, new[] { "Layers" });
            List<string> values = new List<string>(uniqueLayers.Keys);
            values.Sort((left, right) => string.CompareOrdinal(names[left], names[right]));

            StringBuilder builder = new StringBuilder();
            builder.Append(GeneratedCode.Header);
            builder.Append("namespace ").Append(GeneratedCode.Namespace).Append("\n{\n");
            builder.Append("    public static class Layers\n    {\n");

            foreach (string value in values)
            {
                builder.Append("        public const int ").Append(names[value]).Append(" = ")
                    .Append(uniqueLayers[value].ToString(CultureInfo.InvariantCulture)).Append(";\n");
            }

            builder.Append("    }\n}\n");
            return builder.ToString();
        }
    }
}
