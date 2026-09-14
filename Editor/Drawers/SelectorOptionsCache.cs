using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace SafeStringGenerator.Editor.Drawers
{
    internal sealed class StringSelectorOptions
    {
        internal StringSelectorOptions(string[] displayNames, string[] values)
        {
            DisplayNames = displayNames;
            Values = values;
        }

        internal string[] DisplayNames { get; }
        internal string[] Values { get; }
    }

    internal sealed class IntSelectorOptions
    {
        internal IntSelectorOptions(string[] displayNames, int[] values)
        {
            DisplayNames = displayNames;
            Values = values;
        }

        internal string[] DisplayNames { get; }
        internal int[] Values { get; }
    }

    internal static class SelectorOptionsCache
    {
        private static StringSelectorOptions _tags;
        private static IntSelectorOptions _layers;
        private static StringSelectorOptions _sortingLayerNames;
        private static IntSelectorOptions _sortingLayerIds;
        private static StringSelectorOptions _scenes;

        internal static void InvalidateAll()
        {
            _tags = null;
            _layers = null;
            _sortingLayerNames = null;
            _sortingLayerIds = null;
            _scenes = null;
        }

        internal static StringSelectorOptions GetTags()
        {
            if (_tags != null)
            {
                return _tags;
            }

            string[] tags = InternalEditorUtility.tags;
            string[] displayNames = new string[tags.Length + 1];
            string[] values = new string[tags.Length + 1];
            displayNames[0] = "<None>";
            values[0] = string.Empty;
            for (int i = 0; i < tags.Length; i++)
            {
                displayNames[i + 1] = tags[i];
                values[i + 1] = tags[i];
            }

            _tags = new StringSelectorOptions(displayNames, values);
            return _tags;
        }

        internal static IntSelectorOptions GetLayers()
        {
            if (_layers != null)
            {
                return _layers;
            }

            string[] layerNames = InternalEditorUtility.layers;
            string[] displayNames = new string[layerNames.Length];
            int[] values = new int[layerNames.Length];
            for (int i = 0; i < layerNames.Length; i++)
            {
                displayNames[i] = layerNames[i];
                values[i] = LayerMask.NameToLayer(layerNames[i]);
            }

            _layers = new IntSelectorOptions(displayNames, values);
            return _layers;
        }

        internal static StringSelectorOptions GetSortingLayerNames()
        {
            EnsureSortingLayers();
            return _sortingLayerNames;
        }

        internal static IntSelectorOptions GetSortingLayerIds()
        {
            EnsureSortingLayers();
            return _sortingLayerIds;
        }

        internal static StringSelectorOptions GetScenes()
        {
            if (_scenes != null)
            {
                return _scenes;
            }

            List<string> displayNames = new List<string> { "<None>" };
            List<string> values = new List<string> { string.Empty };
            HashSet<string> addedPaths = new HashSet<string>(StringComparer.Ordinal);
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            for (int i = 0; i < scenes.Length; i++)
            {
                string path = scenes[i].path;
                if (!scenes[i].enabled || string.IsNullOrEmpty(path) ||
                    AssetDatabase.LoadAssetAtPath<SceneAsset>(path) == null ||
                    !path.EndsWith(".unity", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string loadPath = path.Substring(0, path.Length - ".unity".Length).Replace('\\', '/');
                if (!addedPaths.Add(loadPath))
                {
                    continue;
                }

                string displayName = loadPath.StartsWith("Assets/", StringComparison.Ordinal)
                    ? loadPath.Substring("Assets/".Length)
                    : loadPath;
                displayNames.Add(displayName);
                values.Add(loadPath);
            }

            _scenes = new StringSelectorOptions(displayNames.ToArray(), values.ToArray());
            return _scenes;
        }

        private static void EnsureSortingLayers()
        {
            if (_sortingLayerNames != null && _sortingLayerIds != null)
            {
                return;
            }

            SortingLayer[] layers = SortingLayer.layers;
            string[] stringDisplayNames = new string[layers.Length + 1];
            string[] stringValues = new string[layers.Length + 1];
            string[] intDisplayNames = new string[layers.Length];
            int[] intValues = new int[layers.Length];
            stringDisplayNames[0] = "<None>";
            stringValues[0] = string.Empty;

            for (int i = 0; i < layers.Length; i++)
            {
                stringDisplayNames[i + 1] = layers[i].name;
                stringValues[i + 1] = layers[i].name;
                intDisplayNames[i] = layers[i].name;
                intValues[i] = layers[i].id;
            }

            _sortingLayerNames = new StringSelectorOptions(stringDisplayNames, stringValues);
            _sortingLayerIds = new IntSelectorOptions(intDisplayNames, intValues);
        }
    }
}
