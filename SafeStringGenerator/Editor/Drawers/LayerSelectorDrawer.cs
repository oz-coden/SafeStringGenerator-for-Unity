using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(LayerSelectorAttribute))]
public class LayerSelectorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.Integer)
        {
            property.intValue = EditorGUI.LayerField(position, label, property.intValue);
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
            Debug.LogWarning("[LayerSelector] can only be used with int variables.");
        }
    }
}
