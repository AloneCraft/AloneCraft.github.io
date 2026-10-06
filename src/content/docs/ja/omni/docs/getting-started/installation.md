---
title: 導入
description: Omni をプロジェクトへ取り込み、フォルダーと生成物の置き場を知る。
sidebar:
  order: 2
---

Omni は UPM のパッケージではなく、`Assets/Omni` に入るアセットのフォルダーとして配布します。

## 手順

1. [動作環境](../requirements/)の Unity、URP、Input System のプロジェクトを用意します。
2. Project Settings の Graphics で、Render Pipeline Asset に URP の資産を設定します。
3. Omni のパッケージを取り込みます。
4. [Verification Hub](../verification/) を開き、描画を確かめます。

:::caution[デモを使う場合]
SolarSystem と CosmicScale は、地球の画像を `Demo/EarthToMoon` から、月の画像を `Demo/PlanetSurface` から読みます。デモを使うときは `Demo` フォルダー全体を取り込んでください。一部だけを取り込むと、地球と月の画像が欠けます。
:::

## フォルダー

| フォルダー | 内容 |
| --- | --- |
| `Runtime` | Logical 座標、Radial 投影、描画、Picking などの実行時のコード |
| `Editor` | Scene View との統合、設定の UI、変換と Bake のツール |
| `Converter` | Shader、Material、VFX の変換と、生成物の GUID の管理 |
| `Resources` | Omni が実行時に読み込む設定と描画の資産 |
| `Localization` | Editor の UI の翻訳 |
| `Demo` | デモの Scene と、デモが共有する部品（`PlanetSurface`、`Materials`） |
| `Verification` | 導入した環境を確かめる Hub と、機能ごとの Scene |
| `Radial/Universal Render Pipeline` | 事前に変換した URP の Shader と Shader Graph |
| `Radial/Hlsl` | Radial の座標・深度・照明のために変更した、同梱の HLSL |
| `Radial/Diagnostics` | 変換できない Material の代わりに使う診断用の Shader |

## 生成物の置き場

標準の URP Shader は、Radial 版を製品に同梱しています。利用者の環境では再生成しません。

独自の Shader、Shader Graph、VFX を変換すると、Omni はプロジェクト固有の生成物を次へ保存します。どれも購入したパッケージとは別の場所なので、パッケージを更新しても上書きされません。

| 置き場 | 内容 |
| --- | --- |
| `Assets/Radial/User` | 変換した独自の Shader と Graph |
| `Assets/Radial/Hlsl` | 変換が必要だった独自の include |
| `Assets/Radial/Mappings` | Material の Shader の切替を元に戻すための台帳 |
| `Assets/Radial` のその他 | Bake、BRG などの管理用の資産 |
| `ProjectSettings/Omni/Generated.manifest.json` | 生成物の GUID と元の Asset の対応 |

:::tip[バージョン管理]
`ProjectSettings/Omni/Generated.manifest.json` は Unity の Asset ではなく、プロジェクトの設定のデータです。Scene が生成した Material などを参照しているプロジェクトでは、このファイルをバージョン管理とバックアップに含めてください。生成物のフォルダーを消しても、このファイルが残っていれば、再生成のときに同じ GUID を復元できます。
:::
