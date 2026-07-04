using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(SceneSelectorAttribute))]
public class SceneSelectorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.String)
        {
            List<string> scenePaths = new List<string>();
            List<string> displayNames = new List<string>();

            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    scenePaths.Add(scene.path);
                    string name = scene.path.Replace("Assets/", "").Replace(".unity", "");
                    displayNames.Add(name);
                }
            }

            if (scenePaths.Count == 0)
            {
                EditorGUI.LabelField(position, label, new GUIContent("There are no scenes in Build Settings."));
                return;
            }

            int selectedIndex = Mathf.Max(0, scenePaths.IndexOf(property.stringValue));

            selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, displayNames.ToArray());

            property.stringValue = scenePaths[selectedIndex];
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
            Debug.LogWarning("[SceneSelector] can only be used with string variables.");
        }
    }
}