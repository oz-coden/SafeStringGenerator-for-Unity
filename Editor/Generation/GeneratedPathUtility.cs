using System;
using System.IO;

namespace SafeStringGenerator.Editor.Generation
{
    internal static class GeneratedPathUtility
    {
        internal const string DefaultDirectory = "Assets/Scripts/SafeStringGenerator";

        internal static bool TryNormalizeDirectory(string value, out string normalized, out string error)
        {
            normalized = null;
            error = null;

            if (string.IsNullOrWhiteSpace(value))
            {
                error = "The generated files path cannot be empty.";
                return false;
            }

            if (Path.IsPathRooted(value))
            {
                error = "The generated files path must be project-relative, not absolute.";
                return false;
            }

            string candidate = value.Replace('\\', '/').TrimEnd('/');
            string[] segments = candidate.Split('/');
            if (segments.Length < 2 || !string.Equals(segments[0], "Assets", StringComparison.Ordinal))
            {
                error = "The generated files path must be a folder below Assets/.";
                return false;
            }

            char[] invalidCharacters = Path.GetInvalidFileNameChars();
            for (int i = 0; i < segments.Length; i++)
            {
                string segment = segments[i];
                if (segment.Length == 0 || segment == "." || segment == "..")
                {
                    error = "The generated files path contains an invalid path segment.";
                    return false;
                }

                if (segment.IndexOfAny(invalidCharacters) >= 0 || segment.IndexOf(':') >= 0)
                {
                    error = "The generated files path contains invalid characters.";
                    return false;
                }
            }

            normalized = string.Join("/", segments);
            return true;
        }

        internal static string Combine(string directory, string fileName)
        {
            return directory.TrimEnd('/') + "/" + fileName;
        }
    }
}
