using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SafeStringGenerator
{
    [CustomPropertyDrawer(typeof(SortingLayerSelectorAttribute))]
    public class SortingLayerSelectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Integer || property.propertyType == SerializedPropertyType.String)
            {
                SortingLayer[] sortingLayers = SortingLayer.layers;
                string[] names = new string[sortingLayers.Length];
                int selectedIndex = 0;

                for (int i = 0; i < sortingLayers.Length; i++)
                {
                    names[i] = sortingLayers[i].name;

                    if (property.propertyType == SerializedPropertyType.Integer && sortingLayers[i].id == property.intValue)
                    {
                        selectedIndex = i;
                    }
                    else if (property.propertyType == SerializedPropertyType.String && sortingLayers[i].name == property.stringValue)
                    {
                        selectedIndex = i;
                    }
                }

                selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, names);

                if (property.propertyType == SerializedPropertyType.Integer)
                {
                    property.intValue = sortingLayers[selectedIndex].id;
                }
                else
                {
                    property.stringValue = sortingLayers[selectedIndex].name;
                }
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
                Debug.LogWarning("[SortingLayerSelector] can only be used with string variables.");
            }
        }
    }
}
