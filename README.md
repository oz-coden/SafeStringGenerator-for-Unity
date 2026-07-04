# Safe String Generator for Unity

Unityを用いた開発における、TagやLayer、SortingLayer、Sceneなどの文字列指定によるタイポを防ぎ、バグやエラーを減らすための開発環境を構築するためのツールです。
Tag, Layer, SortingLayer, Sceneの定数クラスを自動生成し、スクリプトから定数を指定して利用することができたり、インスペクターから安全に選択できるPropertyDrawerを提供します。

## 特徴

- **自動生成:** `TagManager` や `Build Settings` の変更を検知し、定数クラスを自動で生成・更新します。
- **階層化されたシーン管理:** シーンのフォルダ構造を解析し、インスペクターのプルダウンや定数クラスを階層（例: `SceneName.World1.Boss`）で出力します。
- **PropertyDrawer付属:** `[TagSelector]` や `[SceneSelector]` 属性を使うことで、インスペクター上で安全にドロップダウンを使用して値を設定できます。
- **シンプル:** UnityPackageまたはUPM（Unity Package Manager）から簡単に導入できます。

## 導入方法

### 方法1: UPM (Unity Package Manager) を使う場合【おすすめ】
1. Unityメニューの `Window > Package Manager` を開きます。
2. 左上の `+` ボタンをクリックし、`Add package from git URL...` を選択します。
3. 以下のURLを入力し、`Add` をクリックします。
   ```text
   https://github.com/oz-coden/SafeStringGenerator-for-Unity.git
   ```

### 方法2: UnityPackage を使う場合
GitHubの Releases ページから最新の .unitypackage をダウンロードします。

ダウンロードしたファイルをUnityプロジェクトにドラッグ＆ドロップしてインポートします。

## 使い方
1. 定数クラスの利用（コード内でのハードコード防止）
TagやSceneを追加してプロジェクトを保存すると、自動的に `Assets/Scripts/Modifiers/` の中に `TagName.cs` や `SceneName.cs` が生成されます。（手動で生成する場合は、メニューの `Tools > Generate Constants` などを実行してください）。

```C#
// 変更前
if (gameObject.CompareTag("Player"))
SceneManager.LoadScene("Assets/Scenes/World1/Boss.unity");

// 変更後
if (gameObject.CompareTag(TagName.Player))
SceneManager.LoadScene(SceneName.Scenes.World1.Boss);
```

2. インスペクターでの利用（PropertyDrawer）
提供されている属性（Attribute）を変数に付けるだけで、インスペクターが専用のプルダウンメニューに変化します。

```C#
using UnityEngine;

public class Example : MonoBehaviour
{
    [TagSelector] public string targetTag;
    [LayerSelector] public int targetLayer;
    [SortingLayerSelector] public int targetSortingLayer;
    [SceneSelector] public string nextScene; 
}
```
