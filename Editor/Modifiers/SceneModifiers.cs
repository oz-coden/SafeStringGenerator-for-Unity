using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SafeStringGenerator
{
    public static class SceneModifiers
    {
        private const string EXPORT_PATH = SafeStringGeneratorSetting.SCENE_EXPORT_PATH;

        private class Node
        {
            public string name;
            public Dictionary<string, Node> children = new Dictionary<string, Node>();
            public string scenePath = null;
        }

        [MenuItem("Tools/Generate Constants/Scene")]
        public static void Generate()
        {
            Node root = new Node { name = "SceneName" };

            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (!scene.enabled) continue;

                string path = scene.path;
                string pathWithoutExt = path.Replace(".unity", "");

                if (pathWithoutExt.StartsWith("Assets/"))
                {
                    pathWithoutExt = pathWithoutExt.Substring(7);
                }

                string[] parts = pathWithoutExt.Split('/');
                Node current = root;

                for (int i = 0; i < parts.Length; i++)
                {
                    string safeName = Regex.Replace(parts[i], @"[^a-zA-Z0-9_]", "");

                    if (Regex.IsMatch(safeName, @"^\d")) safeName = "_" + safeName;

                    if (!current.children.ContainsKey(safeName))
                    {
                        current.children[safeName] = new Node { name = safeName };
                    }
                    current = current.children[safeName];

                    if (i == parts.Length - 1)
                    {
                        current.scenePath = path;
                    }
                }
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("// This is a generated file. Do not modify it manually.");

            WriteNode(builder, root, 0);

            string directory = Path.GetDirectoryName(EXPORT_PATH);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            File.WriteAllText(EXPORT_PATH, builder.ToString());

            AssetDatabase.Refresh();
            Debug.Log("Snece constants class has been generated automatically.");
        }

        private static void WriteNode(StringBuilder builder, Node node, int indent, string parentName = "")
        {
            string indentStr = new string(' ', indent * 4);

            if (node.children.Count > 0 || indent == 0)
            {
                builder.AppendLine($"{indentStr}public static class {node.name}");
                builder.AppendLine($"{indentStr}{{");

                if (node.scenePath != null)
                {
                    builder.AppendLine($"{indentStr}    public const string Path = \"{node.scenePath}\";");
                }

                foreach (var child in node.children.Values)
                {
                    WriteNode(builder, child, indent + 1, node.name);
                }
                builder.AppendLine($"{indentStr}}}");
            }
            else if (node.scenePath != null)
            {
                string fieldName = node.name;
                if (fieldName == parentName) fieldName += "_Scene";

                builder.AppendLine($"{indentStr}public const string {fieldName} = \"{node.scenePath}\";");
            }
        }
    }
}
