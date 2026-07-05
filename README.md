# Safe String Generator for Unity

Japanese section is [available](#目次) in below this section.

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

This tool helps you to prevents typos related errors and bugs when specifying strings such as "Tag", "Layer", "SortingLayer", and "Scene" in Unity development.  
It automatically generates constant classes and it allows you to specify and use constants from scripts, and provides a PropertyDrawer that lets you safely select them from the Inspector.

## Background of Development

Unity offers no constants to minimize errors when selecting items such as Tags, Layers, Sorting Layers, and Scenes. Instead, you must specify them as strings.  
Specifying them as strings means the compiler won't report errors, so errors caused by typos may go unnoticed.  
I made this to solve that problem.  

## Features

- Auto Generate Constant Classes  
System detects changes to TagManager and Build Settings, and automatically generates and updates constant classes.  
- Layered Scene Management  
Program analyzes the scene's folder structure and displays in scripts and the Inspector's drop-down menus and constant classes in a hierarchical format (e.g., SceneName.World1.Boss).
- Including Property Drawer  
By using attributes such as,  
  - `[TagSelector]`  
  - `[LayerSelector]`  
  - `[SortingLayerSelector]`  
  - `[SceneSelector]`  

  You can safely set values using drop-down menus in Inspector.

## Usage

1. Using In Script Code:
```C#
void Update() 
{
    // Before
    if (gameObject.CompareTag("Player"))
    SceneManager.LoadScene("Assets/Scenes/World1/Battle.unity");

    // After
    if (gameObject.CompareTag(TagName.Player))
    SceneManager.LoadScene(SceneName.Scenes.World1.Battle);
}

```

2. Using In Script Attach Variables:
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

## Requirements, Dependencies

Already Checked:
- Unity 6
- Unity 2022.3
- Unity 2021.3

## Installation

### Method 1: Using UPM (Unity Package Manager)  

1. Open the Unity menu and select Window > Package Manager.  
2. Click the + button in the upper-left corner and select Add package from git URL....  
3. Enter the following URL and click Add.  
```text
https://github.com/oz-coden/SafeStringGenerator-for-Unity.git
```

### Method 2: Using UnityPackage  

1. Download the latest .unitypackage from the GitHub Releases page.
2. Drag and drop the downloaded file into your Unity project to import it.

## Setting

If you want to change the path of the generated files, edit `SafeStringGenerator/Editor/Setting.cs`.

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

このツールは、Unity開発において "Tag"、 "Layer"、 "SortingLayer"、 "Scene"などの文字列を指定する際の入力ミスを防ぐのに役立ちます。  
このツールは定数クラスを自動的に生成し、スクリプトから定数を指定・使用できるようにするほか、インスペクターから安全に定数を選択できるプロパティを提供します。

## 開発背景

Unityには、Tag, Layer, SortingLayer, Sceneなどの選択におけるエラーを最小限に抑えるための定数が用意されていません。その代わりに、これらを文字列として指定する必要があります。  
それらを文字列として指定すると、コンパイラがエラーを報告しないため、タイプミスによるエラーが見過ごされてしまう可能性があります。  
その問題を解決するためにこれを作ることにしました。  

## 機能

- 定数クラスの自動生成  
システムは、TagManager やビルド設定の変更を検知し、定数クラスを自動的に生成・更新します。  
- 階層的なシーン管理  
プログラムはシーンのフォルダ構造を解析し、スクリプトやインスペクターのドロップダウンメニュー、および定数クラスに、階層形式（例：SceneName.World1.Battle）で表示します。  
- プロパティドロワーの組み込み  
次のような属性を使用することで、  
  - `[TagSelector]`  
  - `[LayerSelector]`  
  - `[SortingLayerSelector]`  
  - `[SceneSelector]`  

  インスペクターのドロップダウンメニューから安全に値を設定できます。

## 使い方

1. スクリプト内にコードを記述するとき：
```C#
void Update() 
{
    // Before
    if (gameObject.CompareTag("Player"))
    SceneManager.LoadScene("Assets/Scenes/World1/Battle.unity");

    // After
    if (gameObject.CompareTag(TagName.Player))
    SceneManager.LoadScene(SceneName.Scenes.World1.Battle);
}

```

2. スクリプト内にアタッチ用の変数を記述するとき：
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

## 前提・依存

確認済み：
- Unity 6
- Unity 2022.3
- Unity 2021.3

## 導入方法

### 方法 1: UPM（Unity Package Manager）を使用する  

1. Unity メニューを開き、「Window」＞「Package Manager」を選択します。  
2. 左上隅の「+」ボタンをクリックし、「Add package from git URL...」を選択します。  
3. 以下の URL を入力し、「Add」をクリックします。  
```text
https://github.com/oz-coden/SafeStringGenerator-for-Unity.git
```

### 方法 2: UnityPackage を使用する  

1. GitHub の「Releases」ページから最新の .unitypackage をダウンロードします。
2. ダウンロードしたファイルを Unity プロジェクトにドラッグ＆ドロップしてインポートします。

## 設定

生成されるファイルのパスを変更したい場合は、`SafeStringGenerator/Editor/Setting.cs` を編集してください。

## ライセンス

This project is released under the MIT License.  
