---
title: 動作環境
description: Omni 0.1 を作成・検証した環境と、ビルド対象の対応表。
sidebar:
  order: 1
---

## 作成・検証した環境

| 項目 | 版 |
| --- | --- |
| Unity | `6000.6.0f1` |
| Universal Render Pipeline | `17.6.0` |
| Input System | `1.20.0` |

Omni のコードは Timeline（`com.unity.timeline`）も使います（[Sequencer](../../guides/sequencer/)）。Timeline が入っていないプロジェクトでは追加してください。

主な検証環境は Windows の Editor と Windows の Player です。別の版へ導入する場合は、まず [Verification Hub](../verification/) で描画を確かめてください。

## Renderer

| Renderer | 扱い |
| --- | --- |
| Universal Renderer の Forward、Forward+ | 対応 |
| Deferred、Deferred+、独自の Renderer | 非対応。RadialCamera を含む Player のビルドを止める |
| 2D Renderer | RadialCamera のカメラでは非対応。ビルドを止める |

Deferred の検査は、カメラが使う Renderer だけでなく、ビルド対象の品質レベルが参照する URP 資産の全 Renderer に及びます。URP は Deferred の Renderer が 1 つでもあると、使わなくても全 Shader の GBuffer の Pass を Player に入れるためです。

## ビルド対象

Radial の Shader は Shader Model 4.5（OpenGL ES 3.1 相当）を必要とします。

| ビルド対象と Graphics API | 扱い |
| --- | --- |
| Windows（Direct3D 12） | 対応（受入済み） |
| Web（WebGPU だけ） | 対応（デモで受入済み）。Auto Graphics API を外し、WebGL 2 を一覧から外す |
| Windows（Direct3D 11）、macOS（Metal）、Linux（Vulkan、OpenGL Core）、コンソール | 未検証（ビルドは止めない） |
| Android、iOS、tvOS、visionOS | 非対応（ビルドを止める） |
| WebGL 2、OpenGL ES 3.0、macOS の OpenGL | 非対応（Shader Model 4.5 未満のため、ビルドを止める） |

Player には Material を URP の元の Shader へ戻す仕組みがありません。そのため、実行時に Radial を止めても、非対応の環境で表示が直ることはありません。

## 任意のパッケージ

次のパッケージは無くてもコンパイルできます。無い場合、その機能を使う部分だけが使えなくなります。

| パッケージ | 使う機能 |
| --- | --- |
| `com.unity.visualeffectgraph` | VFX Graph の変換、Hub の VFX の経路 |
| `com.unity.entities`、`com.unity.entities.graphics` | Hub の ECS の経路（下の注意を参照） |
| `com.unity.cinemachine` | Radial 観察カメラ（`GameObject/Omni/Radial Observer Camera`） |

:::caution[DOTS／Entities Graphics]
Omni 0.1 は、DOTS／Entities Graphics の描画を対数描画の対象にしません。読み込んだ Scene に SubScene、または Entities Graphics の描画 Entity があると、対数描画への切替は何も変更する前に理由を示して止まります。有効な RadialCamera を含む Scene と SubScene を含む Scene を同じ Player に入れるビルドも止まります。
:::

## 品質の設定

Omni は独自の品質プロファイルや品質の keyword を持ちません。品質はプロジェクトの URP 資産と Quality Settings で調整します。Radial の写像と深度は、品質レベルによって変わりません。

品質レベルを複数使う場合は、次の設定で段の差を作ると、Shader variant が増えません。

- Render Scale
- 影の解像度・距離・Cascade の数
- Soft Shadows の品質
- MSAA
- HDR の精度

次の設定は、同じビルド対象の品質レベルの間でそろえることを勧めます。URP はビルド対象のすべての品質レベルの URP 資産の和で variant を残すので、そろえないと variant が増え、Omni の上限（1 つの Pass・stage あたり 8,192）に当たることがあります。

- Soft Shadows の有無
- 追加ライトの影
- Light Layers
- Terrain Holes
- LOD Cross Fade
- Renderer の Rendering Path と Renderer Feature（SSAO、Decal など）
