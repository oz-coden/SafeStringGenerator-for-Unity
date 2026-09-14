using System;
using UnityEditor;
using UnityEngine;

namespace SafeStringGenerator.Editor.Drawers
{
    internal static class SelectorDrawerUtility
    {
        internal static void DrawTypeError(
            Rect position,
            SerializedProperty property,
            GUIContent label,
            string expectedType)
        {
            EditorGUI.BeginProperty(position, label, property);
            try
            {
                EditorGUI.LabelField(position, label, new GUIContent("Requires " + expectedType));
            }
            finally
            {
                EditorGUI.EndProperty();
            }
        }

        internal static void DrawString(
            Rect position,
            SerializedProperty property,
            GUIContent label,
            StringSelectorOptions options)
        {
            EditorGUI.BeginProperty(position, label, property);
            bool previousMixedValue = EditorGUI.showMixedValue;
            try
            {
                bool mixed = property.hasMultipleDifferentValues;
                EditorGUI.showMixedValue = mixed;

                int selectedIndex = mixed ? -1 : IndexOf(options.Values, property.stringValue);
                string[] displayNames = options.DisplayNames;
                string[] values = options.Values;
                if (!mixed && selectedIndex < 0)
                {
                    displayNames = Prepend("<Missing: " + property.stringValue + ">", displayNames);
                    values = Prepend(property.stringValue, values);
                    selectedIndex = 0;
                }

                EditorGUI.BeginChangeCheck();
                Rect controlPosition = EditorGUI.PrefixLabel(position, label);
                int newIndex = EditorGUI.Popup(controlPosition, selectedIndex, displayNames);
                bool changed = EditorGUI.EndChangeCheck();
                ApplyStringSelection(property, changed, newIndex, values);
            }
            finally
            {
                EditorGUI.showMixedValue = previousMixedValue;
                EditorGUI.EndProperty();
            }
        }

        internal static void DrawInt(
            Rect position,
            SerializedProperty property,
            GUIContent label,
            IntSelectorOptions options)
        {
            EditorGUI.BeginProperty(position, label, property);
            bool previousMixedValue = EditorGUI.showMixedValue;
            try
            {
                bool mixed = property.hasMultipleDifferentValues;
                EditorGUI.showMixedValue = mixed;

                int selectedIndex = mixed ? -1 : IndexOf(options.Values, property.intValue);
                string[] displayNames = options.DisplayNames;
                int[] values = options.Values;
                if (!mixed && selectedIndex < 0)
                {
                    displayNames = Prepend("<Missing: " + property.intValue + ">", displayNames);
                    values = Prepend(property.intValue, values);
                    selectedIndex = 0;
                }

                EditorGUI.BeginChangeCheck();
                Rect controlPosition = EditorGUI.PrefixLabel(position, label);
                int newIndex = EditorGUI.Popup(controlPosition, selectedIndex, displayNames);
                bool changed = EditorGUI.EndChangeCheck();
                ApplyIntSelection(property, changed, newIndex, values);
            }
            finally
            {
                EditorGUI.showMixedValue = previousMixedValue;
                EditorGUI.EndProperty();
            }
        }

        internal static void ApplyStringSelection(SerializedProperty property, bool changed, int selectedIndex, string[] values)
        {
            if (changed && selectedIndex >= 0 && selectedIndex < values.Length)
            {
                property.stringValue = values[selectedIndex];
            }
        }

        internal static void ApplyIntSelection(SerializedProperty property, bool changed, int selectedIndex, int[] values)
        {
            if (changed && selectedIndex >= 0 && selectedIndex < values.Length)
            {
                property.intValue = values[selectedIndex];
            }
        }

        private static int IndexOf(string[] values, string value)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (string.Equals(values[i], value, StringComparison.Ordinal))
                {
                    return i;
                }
            }

            return -1;
        }

        private static int IndexOf(int[] values, int value)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == value)
                {
                    return i;
                }
            }

            return -1;
        }

        private static string[] Prepend(string value, string[] source)
        {
            string[] result = new string[source.Length + 1];
            result[0] = value;
            Array.Copy(source, 0, result, 1, source.Length);
            return result;
        }

        private static int[] Prepend(int value, int[] source)
        {
            int[] result = new int[source.Length + 1];
            result[0] = value;
            Array.Copy(source, 0, result, 1, source.Length);
            return result;
        }
    }
}
