using System.Collections.Generic;
using System.Text;

namespace SafeStringGenerator.Editor.Generation
{
    internal static class SceneGenerator
    {
        internal static string Generate(IEnumerable<SceneDefinition> scenes, out IReadOnlyList<string> warnings)
        {
            SceneTreeBuildResult tree = SceneTreeBuilder.Build(scenes);
            warnings = tree.Warnings;

            StringBuilder builder = new StringBuilder();
            builder.Append(GeneratedCode.Header);
            builder.Append("namespace ").Append(GeneratedCode.Namespace).Append("\n{\n");
            builder.Append("    public static class Scenes\n    {\n");
            WriteChildren(builder, tree.Root, "Scenes", 2);
            builder.Append("    }\n}\n");
            return builder.ToString();
        }

        private static void WriteChildren(StringBuilder builder, SceneNode parent, string parentIdentifier, int indent)
        {
            IReadOnlyDictionary<string, string> childIdentifiers = IdentifierUtility.Allocate(
                parent.Children.Keys,
                new[] { parentIdentifier, "Path" });
            List<SceneNode> children = new List<SceneNode>(parent.Children.Values);
            children.Sort((left, right) =>
            {
                int identifierOrder = string.CompareOrdinal(childIdentifiers[left.Segment], childIdentifiers[right.Segment]);
                return identifierOrder != 0 ? identifierOrder : string.CompareOrdinal(left.Segment, right.Segment);
            });

            string indentation = new string(' ', indent * 4);
            foreach (SceneNode child in children)
            {
                string identifier = childIdentifiers[child.Segment];
                builder.Append(indentation).Append("public static class ").Append(identifier).Append("\n");
                builder.Append(indentation).Append("{\n");

                if (child.SceneLoadPath != null)
                {
                    builder.Append(indentation).Append("    public const string Path = ")
                        .Append(CSharpLiteral.Quote(child.SceneLoadPath)).Append(";\n");
                }

                WriteChildren(builder, child, identifier, indent + 1);
                builder.Append(indentation).Append("}\n");
            }
        }
    }
}
