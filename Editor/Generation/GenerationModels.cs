namespace SafeStringGenerator.Editor.Generation
{
    internal readonly struct LayerDefinition
    {
        internal LayerDefinition(string name, int index)
        {
            Name = name;
            Index = index;
        }

        internal string Name { get; }
        internal int Index { get; }
    }

    internal readonly struct SortingLayerDefinition
    {
        internal SortingLayerDefinition(string name, int id)
        {
            Name = name;
            Id = id;
        }

        internal string Name { get; }
        internal int Id { get; }
    }

    internal readonly struct SceneDefinition
    {
        internal SceneDefinition(string assetPath, bool enabled, bool exists)
        {
            AssetPath = assetPath;
            Enabled = enabled;
            Exists = exists;
        }

        internal string AssetPath { get; }
        internal bool Enabled { get; }
        internal bool Exists { get; }
    }
}
