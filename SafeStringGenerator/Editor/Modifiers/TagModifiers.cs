using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using System.Text;
using System.Linq;

public static class TagModifiers
{
    private const string EXPORT_PATH = "Assets/Scripts/Modifiers/TagName.cs";

    [MenuItem("Tools/Generate Constants/Tag")]
    public static void Generate()
    {
        StringBuilder builder = new StringBuilder();

        builder.AppendLine("// This is a generated file. Do not modify it manually.");
        builder.AppendLine("public static class TagName");
        builder.AppendLine("{");
        foreach (string tag in InternalEditorUtility.tags)
        {
            string safeName = tag.Replace(" ", "").Replace("/", "");
            builder.AppendLine($"    public const string {safeName} = \"{tag}\";");
        }
        builder.AppendLine("}");
        
        string directory = Path.GetDirectoryName(EXPORT_PATH);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(EXPORT_PATH, builder.ToString());
        
        AssetDatabase.Refresh();
        Debug.Log("// Tag constants class has been generated automatically.");
    }
}
