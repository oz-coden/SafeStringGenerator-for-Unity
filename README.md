# Safe String Generator for Unity

Japanese section is [available](#目次) below this section.

## Table of Contents

1. [Summary](#summary)
2. [Background of Development](#background-of-development)
3. [Features](#features)
4. [Usage](#usage)
5. [Requirements, Dependencies](#requirements-dependencies)
6. [Installation](#installation)
7. [Setting](#setting)
8. [LICENSE](#license)

## Summary

Safe String Generator helps reduce errors caused by directly specifying Unity Tags, Layers, Sorting Layers, and Scenes as strings or numeric values.

It automatically generates safe C# constants and provides PropertyDrawers that allow values to be selected from the Inspector.

Generated constants are placed in the `SafeStringGenerator.Generated` namespace.

## Background of Development

Unity commonly requires Tags, Layers, Sorting Layers, and Scenes to be specified using strings or numeric values.

When these values are written directly in scripts, mistakes such as typos, renamed settings, or invalid references may not be detected by the compiler.

Safe String Generator was created to reduce these problems by automatically generating C# constants from Unity project settings and by providing safe Inspector selectors.

## Features

- Automatic Constant Generation  
Generates constants for:
  - Tags
  - Layers
  - Sorting Layers
  - Enabled Scenes

- Hierarchical Scene Generation  
Analyzes the Scene paths in `EditorBuildSettings.scenes` and generates a nested API based on the folder structure.

For example:

```text
Assets/Scenes/World1/Battle.unity
```

is generated as:

```csharp
Scenes.Scenes.World1.Battle.Path
```

The first `Scenes` is the generated API root, and the second `Scenes` represents the `Assets/Scenes` folder.

* Safe Sorting Layer API
  Sorting Layer names and IDs are generated separately:

```csharp
SortingLayers.Names.Default
SortingLayers.Ids.Default
```

* Automatic Regeneration
  Automatically regenerates generated constants when relevant Unity settings, Scene lists, project state, or script reload events change.

* Safe Generated File Handling
  Only files owned by Safe String Generator are updated.
  Identical files are not rewritten, and existing `.meta` GUIDs are preserved.

* Property Drawers
  The following attributes are provided:

  * `[TagSelector]`
  * `[LayerSelector]`
  * `[SortingLayerSelector]`
  * `[SceneSelector]`

These allow values to be safely selected from drop-down menus in the Inspector.

## Usage

### Using Generated Constants

Generated constants are available in the `SafeStringGenerator.Generated` namespace.

```C#
using SafeStringGenerator.Generated;
using UnityEngine.SceneManagement;

gameObject.CompareTag(Tags.Player);
gameObject.layer = Layers.Default;

renderer.sortingLayerName = SortingLayers.Names.Default;
renderer.sortingLayerID = SortingLayers.Ids.Default;

SceneManager.LoadScene(Scenes.Scenes.World1.Battle.Path);
```

Scene constants use extensionless Build Settings paths and can be passed directly to `SceneManager.LoadScene`.

### Using Inspector Selectors

Selector attributes are available in the `SafeStringGenerator` namespace.

```C#
using SafeStringGenerator;
using UnityEngine;

public class Example : MonoBehaviour
{
    [TagSelector] public string targetTag;
    [LayerSelector] public int targetLayer;
    [SortingLayerSelector] public string sortingLayerName;
    [SortingLayerSelector] public int sortingLayerId;
    [SceneSelector] public string nextScene;
}
```

Missing or mixed values are not changed automatically.
Values are modified only when the user explicitly selects a new value from the Inspector.

### Manual Generation

You can manually regenerate all generated constants from:

```text
Tools > Safe String Generator > Generate All
```

### Custom asmdef Projects

If your project uses custom asmdefs, reference:

```text
SafeStringGenerator.Runtime
```

Generated constants are compiled into the same runtime assembly through the generated `.asmref`, so no additional reference to a generated assembly is required.

## Requirements, Dependencies

Already Checked:

* Unity 2021.3 LTS
* Unity 2022.3 LTS
* Unity 6

No external package dependencies are required.

Only enabled Scenes in the current `EditorBuildSettings.scenes` list are generated.

In Unity 6, this corresponds to the active Build Profile / platform Scene list.

## Installation

### Method 1: Using UPM (Unity Package Manager)

1. Open the Unity menu and select **Window > Package Manager**.
2. Click the **+** button in the upper-left corner and select **Add package from git URL...**.
3. Enter the following URL and click **Add**.

```text
https://github.com/oz-coden/SafeStringGenerator-for-Unity.git
```

### Method 2: Using UnityPackage

1. Download the latest `.unitypackage` from the GitHub Releases page.
2. Drag and drop the downloaded file into your Unity project to import it.

## Setting

Open:

```text
Edit > Project Settings > Safe String Generator
```

to change the generated files path.

The default path is:

```text
Assets/Scripts/SafeStringGenerator
```

Only subfolders of `Assets/` can be used.

The following paths are rejected:

* Absolute paths
* Paths containing `..`
* Paths outside `Assets/`
* `Packages/`
* `PackageCache/`
* Invalid paths

Settings are stored per project in:

```text
ProjectSettings/SafeStringGeneratorSettings.asset
```

and can be version controlled.

Generated files contain an ownership marker and should not be edited manually.
If a destination file already exists and is not owned by Safe String Generator, generation stops without overwriting the file.

## LICENSE

This project is released under the MIT License.

---

# Safe String Generator for Unity

## 目次

1. [概要](#概要)
2. [開発背景](#開発背景)
3. [機能](#機能)
4. [使い方](#使い方)
5. [前提・依存](#前提依存)
6. [導入方法](#導入方法)
7. [設定](#設定)
8. [ライセンス](#ライセンス)

## 概要

Safe String Generatorは、UnityのTag、Layer、Sorting Layer、Sceneを文字列や数値で直接指定することによるミスを減らすためのEditor Toolです。

Unity Projectの設定から安全なC#定数を自動生成し、Inspectorから値を選択できるPropertyDrawerも提供します。

生成された定数は`SafeStringGenerator.Generated` namespace内に配置されます。

## 開発背景

Unityでは、Tag、Layer、Sorting Layer、Sceneなどを文字列や数値で指定する場面が多くあります。

これらをスクリプト内へ直接記述すると、タイプミスや設定名の変更、無効な参照などがあってもコンパイラでは検出できない場合があります。

Safe String Generatorは、Unity Projectの設定からC#定数を自動生成し、Inspectorから安全に値を選択できるようにすることで、これらの問題を減らすために作成しました。

## 機能

* 定数クラスの自動生成
  以下の定数を自動生成します。

  * Tag
  * Layer
  * Sorting Layer
  * 有効なScene

* 階層的なScene生成
  `EditorBuildSettings.scenes`のScene pathを解析し、folder構造を反映した階層APIを生成します。

例えば、

```text
Assets/Scenes/World1/Battle.unity
```

は、

```csharp
Scenes.Scenes.World1.Battle.Path
```

として生成されます。

最初の`Scenes`は生成APIのrootで、次の`Scenes`は`Assets/Scenes` folderを表します。

* Sorting Layerの安全なAPI
  Sorting LayerのNameとIDは分けて生成されます。

```csharp
SortingLayers.Names.Default
SortingLayers.Ids.Default
```

* 自動再生成
  Unity設定、Scene list、Project状態、Script Reloadなどの変更に応じて、自動的に生成内容を更新します。

* 安全な生成ファイル管理
  Safe String Generatorが所有する生成ファイルのみを更新します。
  内容が同一の場合は書き込まず、既存の`.meta` GUIDも維持します。

* PropertyDrawerの提供
  以下のAttributeを利用できます。

  * `[TagSelector]`
  * `[LayerSelector]`
  * `[SortingLayerSelector]`
  * `[SceneSelector]`

Inspectorのプルダウンから安全に値を選択できます。

## 使い方

### 生成された定数を使用する

生成された定数は`SafeStringGenerator.Generated` namespace内にあります。

```C#
using SafeStringGenerator.Generated;
using UnityEngine.SceneManagement;

gameObject.CompareTag(Tags.Player);
gameObject.layer = Layers.Default;

renderer.sortingLayerName = SortingLayers.Names.Default;
renderer.sortingLayerID = SortingLayers.Ids.Default;

SceneManager.LoadScene(Scenes.Scenes.World1.Battle.Path);
```

Sceneの定数値は`.unity`拡張子を除いたBuild Settings pathになっており、`SceneManager.LoadScene`へ直接渡すことができます。

### Inspectorから値を選択する

Selector Attributeは`SafeStringGenerator` namespace内にあります。

```C#
using SafeStringGenerator;
using UnityEngine;

public class Example : MonoBehaviour
{
    [TagSelector] public string targetTag;
    [LayerSelector] public int targetLayer;
    [SortingLayerSelector] public string sortingLayerName;
    [SortingLayerSelector] public int sortingLayerId;
    [SceneSelector] public string nextScene;
}
```

削除済みの値やMixed Valueは自動的に変更されません。
ユーザーがInspectorから明示的に新しい値を選択した場合のみ変更されます。

### 手動生成

すべての定数を手動で再生成する場合は、

```text
Tools > Safe String Generator > Generate All
```

を使用してください。

### custom asmdefを使用する場合

custom asmdefを使用しているProjectでは、

```text
SafeStringGenerator.Runtime
```

を参照してください。

生成された定数は`.asmref`によって同じRuntime assemblyへコンパイルされるため、生成用assemblyを追加で参照する必要はありません。

## 前提・依存

確認済み：

* Unity 2021.3 LTS
* Unity 2022.3 LTS
* Unity 6

外部packageへの依存はありません。

生成対象となるSceneは、現在の`EditorBuildSettings.scenes`に含まれている有効なSceneのみです。

Unity 6では、active Build Profile / platformのScene listが対象になります。

## 導入方法

### 方法 1: UPM（Unity Package Manager）を使用する

1. Unityメニューを開き、**Window > Package Manager**を選択します。
2. 左上の **+** ボタンをクリックし、**Add package from git URL...**を選択します。
3. 以下のURLを入力し、**Add**をクリックします。

```text
https://github.com/oz-coden/SafeStringGenerator-for-Unity.git
```

### 方法 2: UnityPackageを使用する

1. GitHub Releasesページから最新の`.unitypackage`をダウンロードします。
2. ダウンロードしたファイルをUnity Projectへドラッグ＆ドロップしてインポートします。

## 設定

以下を開くことで、生成ファイルの保存先を変更できます。

```text
Edit > Project Settings > Safe String Generator
```

既定値は以下です。

```text
Assets/Scripts/SafeStringGenerator
```

指定できるのは`Assets/`配下のfolderのみです。

以下のpathは使用できません。

* 絶対path
* `..`を含むpath
* `Assets/`外
* `Packages/`
* `PackageCache/`
* 不正なpath

設定はProject単位で、

```text
ProjectSettings/SafeStringGeneratorSettings.asset
```

に保存され、Version Controlで共有できます。

生成ファイルには所有markerが付与されます。
生成ファイルを手動で編集しないでください。

同じ出力先にSafe String Generatorが所有していない既存ファイルがある場合、そのファイルは上書きせず生成を停止します。

## ライセンス

This project is released under the MIT License.
