using UnityEngine;
using UnityEditor;
using SafeStringGenerator;

namespace SafeStringGenerator.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(TagSelectorAttribute))]
    public sealed class TagSelectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                SelectorDrawerUtility.DrawString(position, property, label, SelectorOptionsCache.GetTags());
            }
            else
            {
                SelectorDrawerUtility.DrawTypeError(position, property, label, "a string field");
            }
        }
    }
}
