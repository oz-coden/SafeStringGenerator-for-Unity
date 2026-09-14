using UnityEngine;
using UnityEditor;
using SafeStringGenerator;

namespace SafeStringGenerator.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(LayerSelectorAttribute))]
    public sealed class LayerSelectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Integer)
            {
                SelectorDrawerUtility.DrawInt(position, property, label, SelectorOptionsCache.GetLayers());
            }
            else
            {
                SelectorDrawerUtility.DrawTypeError(position, property, label, "an int field");
            }
        }
    }
}
