using System.Collections.Generic;
using SafeStringGenerator.Editor.Generation;
using SafeStringGenerator.Editor.Updating;
using UnityEditor;
using UnityEngine;

namespace SafeStringGenerator.Editor.Settings
{
    internal static class SafeStringGeneratorSettingsProvider
    {
        private static string _validationMessage;

        [SettingsProvider]
        internal static SettingsProvider CreateProvider()
        {
            SettingsProvider provider = new SettingsProvider("Project/Safe String Generator", SettingsScope.Project)
            {
                label = "Safe String Generator",
                keywords = new HashSet<string>
                {
                    "Safe", "String", "Generator", "Tags", "Layers", "Sorting Layers", "Scenes", "Generated"
                },
                guiHandler = DrawSettings
            };
            return provider;
        }

        private static void DrawSettings(string searchContext)
        {
            SafeStringGeneratorSettings settings = SafeStringGeneratorSettings.instance;

            EditorGUILayout.LabelField("Generated Code", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Generated C# files are placed below Assets and compiled into SafeStringGenerator.Runtime through an assembly reference.",
                MessageType.Info);

            string requestedPath = EditorGUILayout.DelayedTextField(
                new GUIContent("Generated Files Path", "A project-relative folder below Assets/."),
                settings.GeneratedFilesPath);

            if (!string.Equals(requestedPath, settings.GeneratedFilesPath, System.StringComparison.Ordinal))
            {
                if (settings.TrySetGeneratedFilesPath(requestedPath, out string error))
                {
                    _validationMessage = null;
                    GeneratorUpdateDetector.ResetSnapshot();
                }
                else
                {
                    _validationMessage = error;
                }
            }

            if (!string.IsNullOrEmpty(_validationMessage))
            {
                EditorGUILayout.HelpBox(_validationMessage, MessageType.Error);
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Generate Now", GUILayout.Width(140f)))
            {
                _validationMessage = GeneratorCoordinator.GenerateAll(true)
                    ? null
                    : "Generation failed. See the Console for details.";
                GeneratorUpdateDetector.ResetSnapshot();
            }
        }
    }
}
