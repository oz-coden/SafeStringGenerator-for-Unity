using System;
using System.Collections.Generic;

namespace SafeStringGenerator.Editor.Generation
{
    internal sealed class SceneNode
    {
        internal SceneNode(string segment)
        {
            Segment = segment;
            Children = new Dictionary<string, SceneNode>(StringComparer.Ordinal);
        }

        internal string Segment { get; }
        internal string SceneLoadPath { get; set; }
        internal Dictionary<string, SceneNode> Children { get; }
    }

    internal sealed class SceneTreeBuildResult
    {
        internal SceneTreeBuildResult(SceneNode root, IReadOnlyList<string> warnings)
        {
            Root = root;
            Warnings = warnings;
        }

        internal SceneNode Root { get; }
        internal IReadOnlyList<string> Warnings { get; }
    }

    internal static class SceneTreeBuilder
    {
        internal static SceneTreeBuildResult Build(IEnumerable<SceneDefinition> scenes)
        {
            if (scenes == null)
            {
                throw new ArgumentNullException(nameof(scenes));
            }

            List<SceneDefinition> orderedScenes = new List<SceneDefinition>(scenes);
            orderedScenes.Sort((left, right) => string.CompareOrdinal(left.AssetPath, right.AssetPath));

            SceneNode root = new SceneNode(string.Empty);
            List<string> warnings = new List<string>();

            foreach (SceneDefinition scene in orderedScenes)
            {
                if (!scene.Enabled)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(scene.AssetPath))
                {
                    warnings.Add("An enabled Build Settings scene has an empty path and was skipped.");
                    continue;
                }

                if (!scene.Exists)
                {
                    warnings.Add("The enabled scene does not exist and was skipped: " + scene.AssetPath);
                    continue;
                }

                string normalizedPath = scene.AssetPath.Replace('\\', '/');
                if (!normalizedPath.EndsWith(".unity", StringComparison.OrdinalIgnoreCase))
                {
                    warnings.Add("The enabled scene does not have a .unity extension and was skipped: " + scene.AssetPath);
                    continue;
                }

                string loadPath = normalizedPath.Substring(0, normalizedPath.Length - ".unity".Length);
                string hierarchyPath = loadPath.StartsWith("Assets/", StringComparison.Ordinal)
                    ? loadPath.Substring("Assets/".Length)
                    : loadPath;
                string[] segments = hierarchyPath.Split('/');

                bool hasEmptySegment = segments.Length == 0;
                for (int i = 0; i < segments.Length; i++)
                {
                    if (segments[i].Length == 0)
                    {
                        hasEmptySegment = true;
                        break;
                    }
                }

                if (hasEmptySegment)
                {
                    warnings.Add("The enabled scene has an invalid path and was skipped: " + scene.AssetPath);
                    continue;
                }

                SceneNode current = root;
                for (int i = 0; i < segments.Length; i++)
                {
                    string segment = segments[i];
                    if (!current.Children.TryGetValue(segment, out SceneNode child))
                    {
                        child = new SceneNode(segment);
                        current.Children.Add(segment, child);
                    }

                    current = child;
                }

                if (current.SceneLoadPath == null)
                {
                    current.SceneLoadPath = loadPath;
                }
            }

            return new SceneTreeBuildResult(root, warnings);
        }
    }
}
