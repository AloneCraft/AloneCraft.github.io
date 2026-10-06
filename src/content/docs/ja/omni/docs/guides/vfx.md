---
title: VFX Graph
description: VFX Graph の出力を対数描画に対応させる仕組み、対応する出力、照明、原点から遠い VFX。
sidebar:
  order: 4
---

Omni は、VFX Graph の表示用の Shader に Radial の処理を入れて、VFX を対数描画で描きます。VFX Graph の機能を使うには、`com.unity.visualeffectgraph` が必要です。無い場合は、VFX を使う部分だけが使えなくなります。

## 対応する出力

次の出力は、`Assets/Omni/Verification/Functions/Vfx/Modules` の Scene で 1 つずつ確かめています。

| 種類 | 出力 |
| --- | --- |
| Unlit | Quad、Triangle、Octagon、Mesh、Cube、Quad Strip、Line、Line Strip、Point、Soft Particle |
| Shader Graph | Quad、Strip |
| Static Mesh | Static Mesh |
| URP Lit | Quad、Mesh、Quad Strip |
| URP Decal | Decal |

次は、この版には確かめる対象がありません。理由は `Assets/Omni/Verification/Generated/VFX/Modules/modules.json` に残しています。

| 出力 | 理由 |
| --- | --- |
| Six-way | VFX Graph 17.6／URP 17.6 に、Six-way の出力の Context が無い |
| Sphere | Sphere の出力はテンプレート（ParticleSpheres）だけで、Context の型が無い |

URP の 2D の VFX の Sprite 用の Graph は、Radial へ変換しません（[Shader と Material](../shaders-and-materials/)）。

## 変換の仕組み

- Omni は、プロジェクトのすべての VFX の表示用の Shader に、取り込みのたびに Radial のパッチを当てます。
- パッチの処理は、Radial のカメラで描くときだけ効きます。標準のカメラの描画は変わりません。
- フックより前に取り込まれ、パッチの無い VFX は、対数への切替が取り込み直します。
- Omni の注入の Shader が変わったときも、VFX を取り込み直させます。
- VFX Graph が持つ、Asset ではない実行時の Material は、マテリアルの一覧に入れません。VFX の準備状態で確かめます。

Omni Manager の上部に、VFX の状態と、準備のできた数と必要な数が出ます。

| 状態 | 意味 |
| --- | --- |
| `Ready` | 必要な VFX の描画 Shader に、すべてパッチが当たっている |
| `NeedsGeneration`、`Generating` | パッチが必要、またはパッチを当てている |
| `Stale`、`Faulted` | パッチが古い、または当てられない。RadialCamera を含む Player のビルドは止まる |

VFX Graph の内部の呼び出しが見つからない版では、`Faulted` になります（[Fail Closed](../../concepts/fail-closed/)）。

## 空間と照明

VFX の空間の約束は、メッシュの Radial の Lit と同じです。

| 処理 | 使う位置 |
| --- | --- |
| ラスター（clip、view、前のフレームの clip） | 写像した後の、カメラ相対の位置 |
| 照明 | 物理の world の位置（URP が期待する絶対の world） |
| World 空間の距離のソート | 写像した後の「対象 − 視点」の距離 |
| Scene の深度を使う処理（Soft Particle、Decal の出力） | 写像した後の深度を物理の距離へ戻してから比べる |

- Lit の出力（URP Lit の Quad、Mesh、Quad Strip）は、物理の world の位置で照らします。URP の `VFXLit.hlsl` が、この位置を照明、影の座標、追加のライト、APV、反射に使います。視線は URP の本来の計算です。
- URP の VFX Lit を使う Pass に Omni の注入が無いと、パッチを当てず、対応外として理由を出します。物理の照明のデータが無いまま照らすことはしません。
- Decal の出力は、メッシュの Decal と同じ約束です。体積の頂点を写像し、受ける面の位置の復元を、逆の写像で物理の world へ戻します。

:::note[Soft Particle の縁]
頂点だけを写像する出力は、四角形の内側の深度が、写像した後の曲面に厳密には沿いません。そのため、Soft Particle の縁（受ける面との交線）で、色が少しずれます。
:::

照明の空間について詳しくは、[照明の空間](../lighting-spaces/)を見てください。

## 制約

- Renderer の MaterialPropertyBlock は、VFX の出力の Shader に届きません。Instancing の有無によりません。
- VFXRenderer は、`Renderer.bounds` と `localBounds` の上書きを受け付けません。カリングは常に、VFX 自身の Bounds（Graph の Bounds を Unity の Transform で写したもの）で行います。

## 原点から遠い VFX

VFX と同じ GameObject の `Omni.Logical.Transform` が安全域の外にあるとき、Omni はその VFX を、メッシュの精密な経路と同じ写像で描きます。

- 1 つの VFX ごとに表の番号を割り当てます。最大は 32 個です。
- 代わりに使う Unity の Transform の world の位置は、番号を表す印として、原点から 0.5 m 以内の点になります。回転は論理の姿勢のままで、world の倍率は 1 です。
- カメラが描く直前に、各番号の位置の情報を配ります。出力の Shader はこの情報から写像します。カメラに正対する向きも、この情報を使います。

| 条件 | 扱い |
| --- | --- |
| Radial でないカメラ、影、Decal の出力 | 描かない（Fail Closed） |
| VFX の GameObject 自身に `Omni.Logical.Transform` が無い（親だけにある場合を含む） | 対応しない |
| 33 個目以降 | 対応しない |
| World 空間のシステムの重力など、Unity の座標を使う計算 | 対応しない |
| カメラの位置を向く向き（VFX の共通の関数が使うもの） | 対応しない |

:::caution
- 代わりに使う Unity の Transform を読む、ほかの部品（音源など）も、位置が合いません。
- 印を原点の近くに置くのは、カリングが VFX 自身の Bounds で行われるためです。安全域の中にいて原点から離れた Radial のカメラからは、原点が錐台に入らないと描かれないことがあります。
:::

安全域と Logical 座標については、[Logical 座標と Proxy](../../concepts/logical-coordinates/) を見てください。

## 確かめる

| Scene | 内容 |
| --- | --- |
| `Functions/Vfx/Modules` | VFX の出力を 1 つずつ分けた Scene。近い群で、標準と Radial の画像が一致することを確かめる。遠い群で、同じ場所の参照のメッシュと形と明るさが一致することを確かめる |
| `Functions/Vfx/FarLogicalAccretionDisk.unity` | 原点から 1e20 m の Logical の位置にある、粒子の降着円盤。位置、深度、レンズの像を確かめる。Radial を切ると描かれない |

- 近い群で画像が一致するのは、写像が視線の向きを変えないからです。照明が物理の位置で計算されていれば、違うのは深度だけです。
- Module の Scene では、Scene の操作の「撮影して採点」で、開いている Module を標準と Radial で撮影して採点します。結果は `Logs/OmniVfxModules` に保存します。
- どちらも VFX Graph が必要です。
- Verification Hub では、**Case** の `VFX` の系列と、**Execution Path** の VFX の経路で確かめられます（[Verification Hub で確かめる](../../getting-started/verification/)）。
