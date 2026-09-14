# Safe String Generator for Unity

Safe String Generator replaces error-prone Unity string and numeric references with generated C# constants and safe Inspector selectors.

[日本語](#日本語)

## Features

- Generates constants for Tags, Layers, Sorting Layers, and enabled Scenes.
- Mirrors Scene folders as a deterministic nested API.
- Uses the active `EditorBuildSettings.scenes` list (including the active Build Profile in Unity 6); disabled or missing Scenes are not generated.
- Regenerates after relevant Unity settings, Scene-list, project, or script-reload events without per-frame polling.
- Writes only marked generated files, skips identical content, and preserves existing `.meta` GUIDs.
- Provides `[TagSelector]`, `[LayerSelector]`, `[SortingLayerSelector]`, and `[SceneSelector]` PropertyDrawers.

## Usage

Generated constants are in `SafeStringGenerator.Generated`:

```csharp
using SafeStringGenerator.Generated;
using UnityEngine.SceneManagement;

gameObject.CompareTag(Tags.Player);
gameObject.layer = Layers.Default;
sortingLayer.sortingLayerName = SortingLayers.Names.Default;
sortingLayer.sortingLayerID = SortingLayers.Ids.Default;
SceneManager.LoadScene(Scenes.Scenes.World1.Battle.Path);
```

The first `Scenes` is the generated API root. The second one in this example is the `Assets/Scenes` folder. Every Scene is represented by a nested class with a `Path` constant, so a Scene can safely share a name with a folder. Scene values are extensionless Build Settings paths that can be passed directly to `SceneManager.LoadScene`.

Inspector selectors are in the `SafeStringGenerator` namespace:

```csharp
using SafeStringGenerator;
using UnityEngine;

public sealed class Example : MonoBehaviour
{
    [TagSelector] public string targetTag;
    [LayerSelector] public int targetLayer;
    [SortingLayerSelector] public string sortingLayerName;
    [SortingLayerSelector] public int sortingLayerId;
    [SceneSelector] public string nextScene;
}
```

Missing and mixed values remain unchanged until the user explicitly selects a new value.

Projects that use custom asmdefs should reference `SafeStringGenerator.Runtime`. The generated `.asmref` compiles the constants into that same runtime assembly, so no separate generated assembly reference is required.

Use **Tools > Safe String Generator > Generate All** to request generation manually.

## Requirements

- Unity 2021.3 LTS or later
- Unity 2022.3 LTS and Unity 6 are supported targets
- No external package dependency

## Installation

In **Window > Package Manager**, select **Add package from git URL...** and enter:

```text
https://github.com/oz-coden/SafeStringGenerator-for-Unity.git
```

Alternatively, import a `.unitypackage` from the GitHub Releases page.

## Settings

Open **Edit > Project Settings > Safe String Generator** to change **Generated Files Path**. The default is:

```text
Assets/Generated/SafeStringGenerator
```

Only a subfolder of `Assets/` is accepted. Absolute paths, `..`, `Packages/`, `PackageCache/`, and non-generated destination files are rejected. Settings are stored per project in `ProjectSettings/SafeStringGeneratorSettings.asset` and can be version controlled.

Generated files contain an ownership marker and must not be edited manually. If a destination file is not owned by Safe String Generator, generation stops without overwriting it.

## License

[MIT License](LICENSE.md)

---

## 日本語

Safe String Generatorは、UnityのTag、Layer、Sorting Layer、Sceneの直接指定を、生成されたC#定数と安全なInspector選択欄へ置き換える小さなEditor Toolです。

### Features

- Tag、Layer、Sorting Layer、enabled Sceneの定数を生成します。
- Sceneのfolder構造を決定的な階層APIとして生成します。
- 生成対象は現在の`EditorBuildSettings.scenes`（Unity 6ではactive Build Profileを含む）のenabled Sceneだけです。disabled／missing Sceneは生成しません。
- Unity設定、Scene list、project、script reloadのイベントを契機に更新し、毎frame pollingは行いません。
- 所有marker付きの生成fileだけを更新し、同一内容なら書き込まず、既存の`.meta` GUIDを維持します。
- `[TagSelector]`、`[LayerSelector]`、`[SortingLayerSelector]`、`[SceneSelector]`を提供します。

### Usage

生成定数は`SafeStringGenerator.Generated` namespaceにあります。

```csharp
using SafeStringGenerator.Generated;
using UnityEngine.SceneManagement;

gameObject.CompareTag(Tags.Player);
gameObject.layer = Layers.Default;
sortingLayer.sortingLayerName = SortingLayers.Names.Default;
sortingLayer.sortingLayerID = SortingLayers.Ids.Default;
SceneManager.LoadScene(Scenes.Scenes.World1.Battle.Path);
```

例の先頭の`Scenes`は生成API root、次の`Scenes`は`Assets/Scenes` folderです。Sceneは常に`Path`を持つnested classとして生成されるため、folderとSceneが同名でも安全です。値は`.unity`を除いたBuild Settings pathで、`SceneManager.LoadScene`へ直接渡せます。

Selector attributeは`SafeStringGenerator` namespaceにあります。

```csharp
using SafeStringGenerator;
using UnityEngine;

public sealed class Example : MonoBehaviour
{
    [TagSelector] public string targetTag;
    [LayerSelector] public int targetLayer;
    [SortingLayerSelector] public string sortingLayerName;
    [SortingLayerSelector] public int sortingLayerId;
    [SceneSelector] public string nextScene;
}
```

削除済みの値やMixed Valueは、ユーザーが明示的に選択するまで変更されません。

custom asmdefを使うprojectでは`SafeStringGenerator.Runtime`を参照してください。生成先の`.asmref`により定数も同じRuntime assemblyへ入るため、生成assemblyへの追加参照は不要です。

手動生成は **Tools > Safe String Generator > Generate All** です。

### Requirements

- Unity 2021.3 LTS以降
- 対応対象: Unity 2021.3 LTS、2022.3 LTS、Unity 6
- 外部package依存なし

### Installation

**Window > Package Manager > Add package from git URL...** で次を指定します。

```text
https://github.com/oz-coden/SafeStringGenerator-for-Unity.git
```

またはGitHub Releasesの`.unitypackage`をimportします。

### Settings

**Edit > Project Settings > Safe String Generator** で **Generated Files Path** を変更できます。既定値は`Assets/Generated/SafeStringGenerator`です。

`Assets/`配下のfolderだけを指定できます。absolute path、`..`、`Packages/`、`PackageCache/`、未所有の同名fileは拒否されます。設定は`ProjectSettings/SafeStringGeneratorSettings.asset`へProject単位で保存され、Version Controlできます。

生成fileを手動編集しないでください。所有markerがない既存fileは上書きせず、生成を停止します。

### License

[MIT License](LICENSE.md)
