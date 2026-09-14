using UnityEngine;
using UnityEditor;
using SafeStringGenerator;

namespace SafeStringGenerator.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(SortingLayerSelectorAttribute))]
    public sealed class SortingLayerSelectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Integer)
            {
                SelectorDrawerUtility.DrawInt(position, property, label, SelectorOptionsCache.GetSortingLayerIds());
            }
            else if (property.propertyType == SerializedPropertyType.String)
            {
                SelectorDrawerUtility.DrawString(position, property, label, SelectorOptionsCache.GetSortingLayerNames());
            }
            else
            {
                SelectorDrawerUtility.DrawTypeError(position, property, label, "a string or int field");
            }
        }
    }
}
