using SafeStringGenerator.Editor.Generation;
using UnityEditor;
using UnityEngine;

namespace SafeStringGenerator.Editor.Settings
{
    [FilePath("ProjectSettings/SafeStringGeneratorSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    internal sealed class SafeStringGeneratorSettings : ScriptableSingleton<SafeStringGeneratorSettings>
    {
        [SerializeField]
        private string _generatedFilesPath = GeneratedPathUtility.DefaultDirectory;

        internal string GeneratedFilesPath
        {
            get
            {
                return GeneratedPathUtility.TryNormalizeDirectory(
                    _generatedFilesPath,
                    out string normalized,
                    out _)
                    ? normalized
                    : GeneratedPathUtility.DefaultDirectory;
            }
        }

        internal bool TrySetGeneratedFilesPath(string value, out string error)
        {
            if (!GeneratedPathUtility.TryNormalizeDirectory(value, out string normalized, out error))
            {
                return false;
            }

            string previous = GeneratedFilesPath;
            if (string.Equals(previous, normalized, System.StringComparison.Ordinal))
            {
                return true;
            }

            if (!GeneratorCoordinator.TryMoveOwnedOutputs(previous, normalized, out error))
            {
                return false;
            }

            _generatedFilesPath = normalized;
            Save(true);

            if (!GeneratorCoordinator.GenerateAll(true))
            {
                error = "The path was saved, but generation failed. See the Console for details.";
                return false;
            }

            return true;
        }
    }
}
