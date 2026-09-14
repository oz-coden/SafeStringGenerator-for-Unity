using UnityEngine;
using UnityEditor;
using SafeStringGenerator;

namespace SafeStringGenerator.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(SceneSelectorAttribute))]
    public sealed class SceneSelectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                SelectorDrawerUtility.DrawString(position, property, label, SelectorOptionsCache.GetScenes());
            }
            else
            {
                SelectorDrawerUtility.DrawTypeError(position, property, label, "a string field");
            }
        }
    }
}
