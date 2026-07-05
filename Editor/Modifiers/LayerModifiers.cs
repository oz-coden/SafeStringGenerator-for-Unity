using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using System.Text;

namespace SafeStringGenerator
{
    public static class LayerModifiers
    {
        private const string EXPORT_PATH = SafeStringGeneratorSetting.LAYER_EXPORT_PATH;

        [MenuItem("Tools/Generate Constants/Layer")]
        public static void Generate()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("// This is a generated file. Do not modify it manually.");
            builder.AppendLine("public static class LayerName");
            builder.AppendLine("{");
            foreach (string layer in InternalEditorUtility.layers)
            {
                string safeName = layer.Replace(" ", "").Replace("/", "");
                int layerIndex = LayerMask.NameToLayer(layer);
                builder.AppendLine($"    public const int {safeName} = {layerIndex};");
            }
            builder.AppendLine("}");

            string directory = Path.GetDirectoryName(EXPORT_PATH);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(EXPORT_PATH, builder.ToString());

            AssetDatabase.Refresh();
            Debug.Log("Layer constants class has been generated automatically.");
        }
    }
}
