using UnityEditor;

public class GeneratorUpdateDetector : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string path in importedAssets)
        {
            if (path.Equals("ProjectSettings/TagManager.asset"))
            {
                TagModifiers.Generate();
                LayerModifiers.Generate();
                SortingLayerModifiers.Generate();
            }
            if (path.Equals("ProjectSettings/EditorBuildSettings.asset"))
            {
                SceneModifiers.Generate();
            }
        }
    }

    [MenuItem("Tools/Generate Constants/Generate All")]
    public static void GenerateAll()
    {
        TagModifiers.Generate();
        LayerModifiers.Generate();
        SortingLayerModifiers.Generate();
        SceneModifiers.Generate();
    }
}