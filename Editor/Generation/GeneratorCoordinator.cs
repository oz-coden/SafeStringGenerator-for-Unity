using System;
using System.Collections.Generic;
using System.IO;
using SafeStringGenerator.Editor.Settings;
using SafeStringGenerator.Editor.Updating;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace SafeStringGenerator.Editor.Generation
{
    internal static class GeneratorCoordinator
    {
        internal const string TagsFileName = "Tags.g.cs";
        internal const string LayersFileName = "Layers.g.cs";
        internal const string SortingLayersFileName = "SortingLayers.g.cs";
        internal const string ScenesFileName = "Scenes.g.cs";
        internal const string AssemblyReferenceFileName = "SafeStringGenerator.Generated.asmref";
        internal const string AssemblyReferenceContent = "{\n    \"reference\": \"GUID:9506df4bc967a9d41999a4af89f51b8b\"\n}\n";

        private static bool _isGenerating;

        internal static bool IsGenerating => _isGenerating;

        [MenuItem("Tools/Safe String Generator/Generate All")]
        private static void GenerateAllFromMenu()
        {
            GenerateAll(true);
        }

        internal static GenerationState CaptureState()
        {
            string[] sourceTags = InternalEditorUtility.tags;
            List<string> tags = new List<string>(sourceTags.Length);
            for (int i = 0; i < sourceTags.Length; i++)
            {
                tags.Add(sourceTags[i]);
            }

            string[] sourceLayers = InternalEditorUtility.layers;
            List<LayerDefinition> layers = new List<LayerDefinition>(sourceLayers.Length);
            for (int i = 0; i < sourceLayers.Length; i++)
            {
                string layerName = sourceLayers[i];
                layers.Add(new LayerDefinition(layerName, LayerMask.NameToLayer(layerName)));
            }

            SortingLayer[] sourceSortingLayers = SortingLayer.layers;
            List<SortingLayerDefinition> sortingLayers = new List<SortingLayerDefinition>(sourceSortingLayers.Length);
            for (int i = 0; i < sourceSortingLayers.Length; i++)
            {
                sortingLayers.Add(new SortingLayerDefinition(sourceSortingLayers[i].name, sourceSortingLayers[i].id));
            }

            EditorBuildSettingsScene[] sourceScenes = EditorBuildSettings.scenes;
            List<SceneDefinition> scenes = new List<SceneDefinition>(sourceScenes.Length);
            for (int i = 0; i < sourceScenes.Length; i++)
            {
                string path = sourceScenes[i].path;
                bool exists = !string.IsNullOrEmpty(path) && AssetDatabase.LoadAssetAtPath<SceneAsset>(path) != null;
                scenes.Add(new SceneDefinition(path, sourceScenes[i].enabled, exists));
            }

            return new GenerationState(tags, layers, sortingLayers, scenes);
        }

        internal static bool GenerateAll(bool logResult)
        {
            return GenerateAll(CaptureState(), SafeStringGeneratorSettings.instance.GeneratedFilesPath, logResult);
        }

        internal static bool GenerateAll(GenerationState state, string generatedDirectory, bool logResult)
        {
            if (_isGenerating)
            {
                return false;
            }

            _isGenerating = true;
            try
            {
                IReadOnlyList<GeneratedFile> files = CreateGeneratedFiles(state, generatedDirectory, out IReadOnlyList<string> warnings);
                string projectRoot = Directory.GetParent(Application.dataPath)?.FullName;
                if (projectRoot == null)
                {
                    Debug.LogError("SafeStringGenerator could not determine the Unity project root.");
                    return false;
                }

                GeneratedFileWriter writer = new GeneratedFileWriter(projectRoot);
                GeneratedFileWriteResult writeResult = writer.WriteAll(files);
                if (!writeResult.Succeeded)
                {
                    Debug.LogError("SafeStringGenerator: " + writeResult.Error);
                    return false;
                }

                for (int i = 0; i < warnings.Count; i++)
                {
                    Debug.LogWarning("SafeStringGenerator: " + warnings[i]);
                }

                if (writeResult.ChangedAssetPaths.Count > 0)
                {
                    ImportChangedAssets(writeResult.ChangedAssetPaths);
                    if (logResult)
                    {
                        Debug.Log("SafeStringGenerator generated " + writeResult.ChangedAssetPaths.Count + " changed file(s).");
                    }
                }
                else if (logResult)
                {
                    Debug.Log("SafeStringGenerator generated files are already up to date.");
                }

                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError("SafeStringGenerator failed: " + exception);
                return false;
            }
            finally
            {
                _isGenerating = false;
            }
        }

        internal static IReadOnlyList<GeneratedFile> CreateGeneratedFiles(
            GenerationState state,
            string generatedDirectory,
            out IReadOnlyList<string> warnings)
        {
            if (!GeneratedPathUtility.TryNormalizeDirectory(generatedDirectory, out string normalizedDirectory, out string pathError))
            {
                throw new ArgumentException(pathError, nameof(generatedDirectory));
            }

            string sceneSource = SceneGenerator.Generate(state.Scenes, out warnings);
            return new[]
            {
                new GeneratedFile(
                    GeneratedPathUtility.Combine(normalizedDirectory, AssemblyReferenceFileName),
                    AssemblyReferenceContent,
                    false),
                new GeneratedFile(
                    GeneratedPathUtility.Combine(normalizedDirectory, TagsFileName),
                    TagGenerator.Generate(state.Tags),
                    true),
                new GeneratedFile(
                    GeneratedPathUtility.Combine(normalizedDirectory, LayersFileName),
                    LayerGenerator.Generate(state.Layers),
                    true),
                new GeneratedFile(
                    GeneratedPathUtility.Combine(normalizedDirectory, SortingLayersFileName),
                    SortingLayerGenerator.Generate(state.SortingLayers),
                    true),
                new GeneratedFile(
                    GeneratedPathUtility.Combine(normalizedDirectory, ScenesFileName),
                    sceneSource,
                    true)
            };
        }

        internal static bool AllOutputsExist(string generatedDirectory)
        {
            if (!GeneratedPathUtility.TryNormalizeDirectory(generatedDirectory, out string normalizedDirectory, out _))
            {
                return false;
            }

            string projectRoot = Directory.GetParent(Application.dataPath)?.FullName;
            if (projectRoot == null)
            {
                return false;
            }

            string[] fileNames =
            {
                AssemblyReferenceFileName,
                TagsFileName,
                LayersFileName,
                SortingLayersFileName,
                ScenesFileName
            };

            for (int i = 0; i < fileNames.Length; i++)
            {
                string assetPath = GeneratedPathUtility.Combine(normalizedDirectory, fileNames[i]);
                string fullPath = Path.Combine(projectRoot, assetPath.Replace('/', Path.DirectorySeparatorChar));
                if (!File.Exists(fullPath))
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool IsGeneratedAssetPath(string assetPath)
        {
            string directory = SafeStringGeneratorSettings.instance.GeneratedFilesPath.TrimEnd('/') + "/";
            return !string.IsNullOrEmpty(assetPath) && assetPath.StartsWith(directory, StringComparison.Ordinal);
        }

        internal static bool TryMoveOwnedOutputs(string oldDirectory, string newDirectory, out string error)
        {
            error = null;
            if (!GeneratedPathUtility.TryNormalizeDirectory(oldDirectory, out string normalizedOld, out error) ||
                !GeneratedPathUtility.TryNormalizeDirectory(newDirectory, out string normalizedNew, out error))
            {
                return false;
            }

            if (string.Equals(normalizedOld, normalizedNew, StringComparison.Ordinal))
            {
                return true;
            }

            GenerationState state = CaptureState();
            IReadOnlyList<GeneratedFile> oldFiles = CreateGeneratedFiles(state, normalizedOld, out _);
            IReadOnlyList<GeneratedFile> newFiles = CreateGeneratedFiles(state, normalizedNew, out _);
            string projectRoot = Directory.GetParent(Application.dataPath)?.FullName;
            if (projectRoot == null)
            {
                error = "Unable to determine the Unity project root.";
                return false;
            }

            GeneratedFileWriter writer = new GeneratedFileWriter(projectRoot);
            for (int i = 0; i < oldFiles.Count; i++)
            {
                if (!writer.IsOwned(oldFiles[i], out error))
                {
                    return false;
                }

                string oldFullPath = Path.Combine(
                    projectRoot,
                    oldFiles[i].AssetPath.Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(oldFullPath) && AssetDatabase.LoadMainAssetAtPath(oldFiles[i].AssetPath) == null)
                {
                    error = "The existing generated file is not registered in the AssetDatabase: " + oldFiles[i].AssetPath;
                    return false;
                }

                if (AssetDatabase.LoadMainAssetAtPath(newFiles[i].AssetPath) != null ||
                    File.Exists(Path.Combine(projectRoot, newFiles[i].AssetPath.Replace('/', Path.DirectorySeparatorChar))))
                {
                    error = "The new generated files folder already contains: " + newFiles[i].AssetPath;
                    return false;
                }
            }

            if (!EnsureAssetFolder(normalizedNew, out error))
            {
                return false;
            }

            List<int> movedIndices = new List<int>();
            for (int i = 0; i < oldFiles.Count; i++)
            {
                if (AssetDatabase.LoadMainAssetAtPath(oldFiles[i].AssetPath) == null)
                {
                    continue;
                }

                string moveError = AssetDatabase.MoveAsset(oldFiles[i].AssetPath, newFiles[i].AssetPath);
                if (!string.IsNullOrEmpty(moveError))
                {
                    for (int rollback = movedIndices.Count - 1; rollback >= 0; rollback--)
                    {
                        int movedIndex = movedIndices[rollback];
                        AssetDatabase.MoveAsset(newFiles[movedIndex].AssetPath, oldFiles[movedIndex].AssetPath);
                    }

                    error = "Failed to move generated files: " + moveError;
                    return false;
                }

                movedIndices.Add(i);
            }

            return true;
        }

        private static bool EnsureAssetFolder(string assetFolder, out string error)
        {
            error = null;
            string[] segments = assetFolder.Split('/');
            string current = segments[0];
            for (int i = 1; i < segments.Length; i++)
            {
                string next = current + "/" + segments[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    string guid = AssetDatabase.CreateFolder(current, segments[i]);
                    if (string.IsNullOrEmpty(guid))
                    {
                        error = "Failed to create generated files folder: " + next;
                        return false;
                    }
                }

                current = next;
            }

            return true;
        }

        private static void ImportChangedAssets(IReadOnlyList<string> assetPaths)
        {
            AssetDatabase.StartAssetEditing();
            try
            {
                for (int i = 0; i < assetPaths.Count; i++)
                {
                    AssetDatabase.ImportAsset(assetPaths[i], ImportAssetOptions.Default);
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }
    }
}
