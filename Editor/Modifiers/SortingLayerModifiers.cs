using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using System.Text;
using System.Linq;

public class SortingLayerModifiers
{
    private const string EXPORT_PATH = "Assets/Scripts/Modifiers/SortingLayerName.cs";

    [MenuItem("Tools/Generate Constants/SortingLayerName")]
    public static void Generate()
    {
        StringBuilder builder = new StringBuilder();

        builder.AppendLine("// This is a generated file. Do not modify it manually.");
        builder.AppendLine("public static class SortingLayerName");
        builder.AppendLine("{");
        foreach (SortingLayer sortingLayer in SortingLayer.layers)
        {
            string safeName = sortingLayer.name.Replace(" ", "").Replace("/", "");
            
            builder.AppendLine($"    public const string {safeName} = \"{sortingLayer.name}\";");
            
            builder.AppendLine($"    public const int {safeName}_ID = {sortingLayer.id};");
        }
        builder.AppendLine("}");
        
        string directory = Path.GetDirectoryName(EXPORT_PATH);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(EXPORT_PATH, builder.ToString());
        
        AssetDatabase.Refresh();
        Debug.Log("// SortingLayer constants class has been generated automatically.");
    }
}
