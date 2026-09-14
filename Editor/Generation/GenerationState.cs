using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SafeStringGenerator.Editor.Generation
{
    internal sealed class GenerationState
    {
        internal GenerationState(
            IReadOnlyList<string> tags,
            IReadOnlyList<LayerDefinition> layers,
            IReadOnlyList<SortingLayerDefinition> sortingLayers,
            IReadOnlyList<SceneDefinition> scenes)
        {
            Tags = tags;
            Layers = layers;
            SortingLayers = sortingLayers;
            Scenes = scenes;
        }

        internal IReadOnlyList<string> Tags { get; }
        internal IReadOnlyList<LayerDefinition> Layers { get; }
        internal IReadOnlyList<SortingLayerDefinition> SortingLayers { get; }
        internal IReadOnlyList<SceneDefinition> Scenes { get; }

        internal string CreateSignature(string generatedDirectory)
        {
            StringBuilder builder = new StringBuilder();
            Append(builder, "D", generatedDirectory);

            for (int i = 0; i < Tags.Count; i++)
            {
                Append(builder, "T", Tags[i]);
            }

            for (int i = 0; i < Layers.Count; i++)
            {
                Append(builder, "L", Layers[i].Name);
                Append(builder, "I", Layers[i].Index.ToString(CultureInfo.InvariantCulture));
            }

            for (int i = 0; i < SortingLayers.Count; i++)
            {
                Append(builder, "S", SortingLayers[i].Name);
                Append(builder, "I", SortingLayers[i].Id.ToString(CultureInfo.InvariantCulture));
            }

            for (int i = 0; i < Scenes.Count; i++)
            {
                Append(builder, "P", Scenes[i].AssetPath);
                builder.Append(Scenes[i].Enabled ? '1' : '0');
                builder.Append(Scenes[i].Exists ? '1' : '0');
                builder.Append(';');
            }

            return builder.ToString();
        }

        private static void Append(StringBuilder builder, string kind, string value)
        {
            string safeValue = value ?? string.Empty;
            builder.Append(kind);
            builder.Append(safeValue.Length.ToString(CultureInfo.InvariantCulture));
            builder.Append(':');
            builder.Append(safeValue);
            builder.Append(';');
        }
    }

    internal static class GenerationDecision
    {
        internal static bool ShouldGenerate(string previousSignature, string currentSignature, bool allOutputsExist, bool force)
        {
            return force || !allOutputsExist || previousSignature == null ||
                   !string.Equals(previousSignature, currentSignature, System.StringComparison.Ordinal);
        }
    }
}
