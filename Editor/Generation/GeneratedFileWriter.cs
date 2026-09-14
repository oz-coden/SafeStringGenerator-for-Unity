using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SafeStringGenerator.Editor.Generation
{
    internal readonly struct GeneratedFile
    {
        internal GeneratedFile(string assetPath, string content, bool requireCSharpOwnershipMarker)
        {
            AssetPath = assetPath;
            Content = content;
            RequireCSharpOwnershipMarker = requireCSharpOwnershipMarker;
        }

        internal string AssetPath { get; }
        internal string Content { get; }
        internal bool RequireCSharpOwnershipMarker { get; }
    }

    internal sealed class GeneratedFileWriteResult
    {
        private GeneratedFileWriteResult(bool succeeded, IReadOnlyList<string> changedAssetPaths, string error)
        {
            Succeeded = succeeded;
            ChangedAssetPaths = changedAssetPaths;
            Error = error;
        }

        internal bool Succeeded { get; }
        internal IReadOnlyList<string> ChangedAssetPaths { get; }
        internal string Error { get; }

        internal static GeneratedFileWriteResult Success(IReadOnlyList<string> changedAssetPaths)
        {
            return new GeneratedFileWriteResult(true, changedAssetPaths, null);
        }

        internal static GeneratedFileWriteResult Failure(string error)
        {
            return new GeneratedFileWriteResult(false, Array.Empty<string>(), error);
        }
    }

    internal sealed class GeneratedFileWriter
    {
        private static readonly UTF8Encoding Utf8WithoutBom = new UTF8Encoding(false);
        private readonly string _projectRoot;
        private readonly string _assetsRoot;
        private readonly StringComparison _pathComparison;

        internal GeneratedFileWriter(string projectRoot)
        {
            if (string.IsNullOrWhiteSpace(projectRoot))
            {
                throw new ArgumentException("A project root is required.", nameof(projectRoot));
            }

            _projectRoot = Path.GetFullPath(projectRoot);
            _assetsRoot = Path.GetFullPath(Path.Combine(_projectRoot, "Assets"));
            _pathComparison = Path.DirectorySeparatorChar == '\\'
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;
        }

        internal GeneratedFileWriteResult WriteAll(IReadOnlyList<GeneratedFile> files)
        {
            try
            {
                return WriteAllCore(files);
            }
            catch (Exception exception)
            {
                return GeneratedFileWriteResult.Failure("Failed to prepare generated files: " + exception.Message);
            }
        }

        private GeneratedFileWriteResult WriteAllCore(IReadOnlyList<GeneratedFile> files)
        {
            if (files == null)
            {
                return GeneratedFileWriteResult.Failure("The generated file list is null.");
            }

            List<WritePlan> plans = new List<WritePlan>();
            HashSet<string> targets = new HashSet<string>(
                _pathComparison == StringComparison.OrdinalIgnoreCase
                    ? StringComparer.OrdinalIgnoreCase
                    : StringComparer.Ordinal);

            for (int i = 0; i < files.Count; i++)
            {
                GeneratedFile file = files[i];
                if (!TryResolveAssetPath(file.AssetPath, out string fullPath, out string pathError))
                {
                    return GeneratedFileWriteResult.Failure(pathError);
                }

                if (!targets.Add(fullPath))
                {
                    return GeneratedFileWriteResult.Failure("Duplicate generated file path: " + file.AssetPath);
                }

                string extension = Path.GetExtension(fullPath);
                bool isCSharp = string.Equals(extension, ".cs", StringComparison.OrdinalIgnoreCase);
                bool isAssemblyReference = string.Equals(extension, ".asmref", StringComparison.OrdinalIgnoreCase);
                if (!isCSharp && !isAssemblyReference)
                {
                    return GeneratedFileWriteResult.Failure("Generated files must use the .cs or .asmref extension: " + file.AssetPath);
                }

                if (isCSharp && (!file.RequireCSharpOwnershipMarker ||
                    file.Content.IndexOf(GeneratedCode.OwnershipMarker, StringComparison.Ordinal) < 0))
                {
                    return GeneratedFileWriteResult.Failure("Generated C# content is missing the SafeStringGenerator ownership marker: " + file.AssetPath);
                }

                bool exists = File.Exists(fullPath);
                string previousContent = exists ? File.ReadAllText(fullPath) : null;
                if (exists && string.Equals(previousContent, file.Content, StringComparison.Ordinal))
                {
                    continue;
                }

                if (exists)
                {
                    if ((File.GetAttributes(fullPath) & FileAttributes.ReadOnly) != 0)
                    {
                        return GeneratedFileWriteResult.Failure("The generated file is read-only: " + file.AssetPath);
                    }

                    bool isOwned = isCSharp
                        ? previousContent.IndexOf(GeneratedCode.OwnershipMarker, StringComparison.Ordinal) >= 0
                        : string.Equals(previousContent, file.Content, StringComparison.Ordinal);
                    if (!isOwned)
                    {
                        return GeneratedFileWriteResult.Failure("Refusing to overwrite a file not owned by SafeStringGenerator: " + file.AssetPath);
                    }
                }

                plans.Add(new WritePlan(file.AssetPath, fullPath, file.Content, exists, previousContent));
            }

            List<WritePlan> completed = new List<WritePlan>();
            try
            {
                for (int i = 0; i < plans.Count; i++)
                {
                    WritePlan plan = plans[i];
                    string directory = Path.GetDirectoryName(plan.FullPath);
                    if (directory == null)
                    {
                        throw new IOException("Unable to determine the generated file directory.");
                    }

                    Directory.CreateDirectory(directory);
                    WriteAtomically(plan.FullPath, plan.Content, plan.Existed);
                    completed.Add(plan);
                }
            }
            catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException || exception is NotSupportedException)
            {
                string rollbackError = RollBack(completed);
                string error = "Failed to write generated files: " + exception.Message;
                if (rollbackError != null)
                {
                    error += " Rollback also failed: " + rollbackError;
                }

                return GeneratedFileWriteResult.Failure(error);
            }

            List<string> changedPaths = new List<string>(plans.Count);
            for (int i = 0; i < plans.Count; i++)
            {
                changedPaths.Add(plans[i].AssetPath);
            }

            return GeneratedFileWriteResult.Success(changedPaths);
        }

        internal bool IsOwned(GeneratedFile file, out string error)
        {
            try
            {
                error = null;
                if (!TryResolveAssetPath(file.AssetPath, out string fullPath, out error))
                {
                    return false;
                }

                if (!File.Exists(fullPath))
                {
                    return true;
                }

                string content = File.ReadAllText(fullPath);
                string extension = Path.GetExtension(fullPath);
                if (string.Equals(extension, ".cs", StringComparison.OrdinalIgnoreCase))
                {
                    if (content.IndexOf(GeneratedCode.OwnershipMarker, StringComparison.Ordinal) >= 0)
                    {
                        return true;
                    }
                }
                else if (string.Equals(extension, ".asmref", StringComparison.OrdinalIgnoreCase) &&
                         string.Equals(content, file.Content, StringComparison.Ordinal))
                {
                    return true;
                }

                error = "The file is not owned by SafeStringGenerator: " + file.AssetPath;
                return false;
            }
            catch (Exception exception)
            {
                error = "Failed to inspect generated file ownership: " + exception.Message;
                return false;
            }
        }

        private bool TryResolveAssetPath(string assetPath, out string fullPath, out string error)
        {
            fullPath = null;
            error = null;

            if (string.IsNullOrWhiteSpace(assetPath) || Path.IsPathRooted(assetPath))
            {
                error = "Generated file paths must be project-relative paths below Assets/.";
                return false;
            }

            string normalized = assetPath.Replace('\\', '/');
            string[] segments = normalized.Split('/');
            if (segments.Length < 2 || !string.Equals(segments[0], "Assets", StringComparison.Ordinal))
            {
                error = "Generated file paths must be below Assets/: " + assetPath;
                return false;
            }

            char[] invalidCharacters = Path.GetInvalidFileNameChars();
            for (int i = 0; i < segments.Length; i++)
            {
                string segment = segments[i];
                if (segment.Length == 0 || segment == "." || segment == "..")
                {
                    error = "Generated file paths cannot contain empty, . or .. segments: " + assetPath;
                    return false;
                }

                if (segment.IndexOfAny(invalidCharacters) >= 0 || segment.IndexOf(':') >= 0)
                {
                    error = "Generated file paths contain invalid characters: " + assetPath;
                    return false;
                }
            }

            fullPath = Path.GetFullPath(Path.Combine(_projectRoot, normalized.Replace('/', Path.DirectorySeparatorChar)));
            string assetsPrefix = _assetsRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
            if (!fullPath.StartsWith(assetsPrefix, _pathComparison))
            {
                error = "Generated file paths must remain below Assets/: " + assetPath;
                fullPath = null;
                return false;
            }

            string directory = Path.GetDirectoryName(fullPath);
            while (directory != null && directory.StartsWith(assetsPrefix, _pathComparison))
            {
                if (Directory.Exists(directory) &&
                    (File.GetAttributes(directory) & FileAttributes.ReparsePoint) != 0)
                {
                    error = "Generated file paths cannot pass through symbolic links or junctions: " + assetPath;
                    fullPath = null;
                    return false;
                }

                directory = Path.GetDirectoryName(directory);
            }

            return true;
        }

        private static void WriteAtomically(string fullPath, string content, bool destinationExists)
        {
            string temporaryPath = fullPath + ".ssg-" + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                File.WriteAllText(temporaryPath, content, Utf8WithoutBom);
                if (destinationExists)
                {
                    File.Replace(temporaryPath, fullPath, null);
                }
                else
                {
                    File.Move(temporaryPath, fullPath);
                }
            }
            finally
            {
                if (File.Exists(temporaryPath))
                {
                    File.Delete(temporaryPath);
                }
            }
        }

        private static string RollBack(List<WritePlan> completed)
        {
            try
            {
                for (int i = completed.Count - 1; i >= 0; i--)
                {
                    WritePlan plan = completed[i];
                    if (plan.Existed)
                    {
                        File.WriteAllText(plan.FullPath, plan.PreviousContent, Utf8WithoutBom);
                    }
                    else if (File.Exists(plan.FullPath))
                    {
                        File.Delete(plan.FullPath);
                    }
                }

                return null;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        private sealed class WritePlan
        {
            internal WritePlan(string assetPath, string fullPath, string content, bool existed, string previousContent)
            {
                AssetPath = assetPath;
                FullPath = fullPath;
                Content = content;
                Existed = existed;
                PreviousContent = previousContent;
            }

            internal string AssetPath { get; }
            internal string FullPath { get; }
            internal string Content { get; }
            internal bool Existed { get; }
            internal string PreviousContent { get; }
        }
    }
}
