using System;
using System.IO;
using System.Threading;
using SafeStringGenerator.Editor.Drawers;
using SafeStringGenerator.Editor.Generation;
using SafeStringGenerator.Editor.Settings;
using UnityEditor;

namespace SafeStringGenerator.Editor.Updating
{
    [InitializeOnLoad]
    internal sealed class GeneratorUpdateDetector : AssetPostprocessor
    {
        private static bool _scheduled;
        private static bool _forceRequested;
        private static string _lastSignature;
        private static readonly SynchronizationContext EditorContext;
        private static FileSystemWatcher _projectSettingsWatcher;

        static GeneratorUpdateDetector()
        {
            EditorContext = SynchronizationContext.Current;
            EditorBuildSettings.sceneListChanged += OnSceneListChanged;
            EditorApplication.projectChanged += OnProjectChanged;
            AssemblyReloadEvents.beforeAssemblyReload += DisposeProjectSettingsWatcher;
            EditorApplication.quitting += DisposeProjectSettingsWatcher;
            StartProjectSettingsWatcher();
            RequestGeneration(false);
        }

        internal static void ResetSnapshot()
        {
            _lastSignature = null;
        }

        internal static void RequestGeneration(bool force)
        {
            _forceRequested |= force;
            if (_scheduled)
            {
                return;
            }

            _scheduled = true;
            EditorApplication.delayCall += RunScheduledGeneration;
        }

        private static void OnSceneListChanged()
        {
            SelectorOptionsCache.InvalidateAll();
            RequestGeneration(false);
        }

        private static void OnProjectChanged()
        {
            SelectorOptionsCache.InvalidateAll();
            RequestGeneration(false);
        }

        private static void RunScheduledGeneration()
        {
            _scheduled = false;
            bool force = _forceRequested;
            _forceRequested = false;

            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                return;
            }

            try
            {
                GenerationState state = GeneratorCoordinator.CaptureState();
                string directory = SafeStringGeneratorSettings.instance.GeneratedFilesPath;
                string signature = state.CreateSignature(directory);
                bool outputsExist = GeneratorCoordinator.AllOutputsExist(directory);

                if (!GenerationDecision.ShouldGenerate(_lastSignature, signature, outputsExist, force))
                {
                    return;
                }

                if (GeneratorCoordinator.GenerateAll(state, directory, false))
                {
                    _lastSignature = signature;
                }
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogError("SafeStringGenerator update detection failed: " + exception);
            }
        }

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths,
            bool didDomainReload)
        {
            bool relevantSettingsChange = Contains(importedAssets, "ProjectSettings/TagManager.asset") ||
                                          Contains(importedAssets, "ProjectSettings/EditorBuildSettings.asset");
            bool generatedOutputChanged = ContainsGeneratedPath(importedAssets) ||
                                          ContainsGeneratedPath(deletedAssets) ||
                                          ContainsGeneratedPath(movedAssets) ||
                                          ContainsGeneratedPath(movedFromAssetPaths);

            if (GeneratorCoordinator.IsGenerating)
            {
                generatedOutputChanged = false;
            }

            if (didDomainReload || relevantSettingsChange || generatedOutputChanged)
            {
                SelectorOptionsCache.InvalidateAll();
                RequestGeneration(generatedOutputChanged);
            }
        }

        private static void StartProjectSettingsWatcher()
        {
            try
            {
                string projectRoot = Directory.GetParent(UnityEngine.Application.dataPath)?.FullName;
                if (projectRoot == null)
                {
                    return;
                }

                string projectSettingsPath = Path.Combine(projectRoot, "ProjectSettings");
                if (!Directory.Exists(projectSettingsPath))
                {
                    return;
                }

                _projectSettingsWatcher = new FileSystemWatcher(projectSettingsPath, "*.asset")
                {
                    IncludeSubdirectories = false,
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size
                };
                _projectSettingsWatcher.Changed += OnProjectSettingsFileChanged;
                _projectSettingsWatcher.Created += OnProjectSettingsFileChanged;
                _projectSettingsWatcher.Renamed += OnProjectSettingsFileRenamed;
                _projectSettingsWatcher.EnableRaisingEvents = true;
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogWarning("SafeStringGenerator could not watch ProjectSettings changes: " + exception.Message);
                DisposeProjectSettingsWatcher();
            }
        }

        private static void OnProjectSettingsFileChanged(object sender, FileSystemEventArgs args)
        {
            if (IsRelevantProjectSettingsFile(args.Name))
            {
                PostProjectSettingsChange();
            }
        }

        private static void OnProjectSettingsFileRenamed(object sender, RenamedEventArgs args)
        {
            if (IsRelevantProjectSettingsFile(args.Name) || IsRelevantProjectSettingsFile(args.OldName))
            {
                PostProjectSettingsChange();
            }
        }

        private static bool IsRelevantProjectSettingsFile(string fileName)
        {
            return string.Equals(fileName, "TagManager.asset", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(fileName, "EditorBuildSettings.asset", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(fileName, "SafeStringGeneratorSettings.asset", StringComparison.OrdinalIgnoreCase);
        }

        private static void PostProjectSettingsChange()
        {
            EditorContext?.Post(_ =>
            {
                SelectorOptionsCache.InvalidateAll();
                RequestGeneration(false);
            }, null);
        }

        private static void DisposeProjectSettingsWatcher()
        {
            if (_projectSettingsWatcher == null)
            {
                return;
            }

            _projectSettingsWatcher.EnableRaisingEvents = false;
            _projectSettingsWatcher.Dispose();
            _projectSettingsWatcher = null;
        }

        private static bool Contains(string[] paths, string expected)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                if (string.Equals(paths[i], expected, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ContainsGeneratedPath(string[] paths)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                if (GeneratorCoordinator.IsGeneratedAssetPath(paths[i]))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
