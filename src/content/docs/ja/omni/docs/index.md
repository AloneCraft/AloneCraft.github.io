---
title: Omni の概要
description: Omni が何を解決するか、どういう仕組みで描くか。
---

Omni は、陽子ほどの小さな尺度から観測可能な宇宙の大きさまでの位置と姿勢を高精度の **Logical 座標**で持ち、Unity の Scene View、Game View、Player、GPU へ有限な精度の値として渡すための Unity アセットです。

:::note[0.1 について]
このドキュメントは Omni 0.1.0 を対象にしています。販売前のため、内容は変わることがあります。
:::

## 解決する問題

Unity の Transform は位置を 32 bit の `float` で持ちます。原点から離れるほど表せる間隔が粗くなるので、天体規模の空間では物体が震えたり、重なったり、置けなかったりします。また、カメラの Near と Far の比には限りがあるので、近くの物体と遠くの物体を 1 台のカメラで同時に描けません。

Omni はこの 2 つを別々の方法で解きます。

| 問題 | Omni の方法 |
| --- | --- |
| 位置の精度 | 位置の正本を 192 bit の Logical 座標で持つ。`float` へは描画の直前でだけ変換する |
| 距離の範囲 | 目からの距離を対数で写して描く（Radial）。1 台のカメラで近景から遠景までを描く |

## Logical 座標

- `Omni.Logical.Transform`（Add Component の `Omni/Logical Transform`）が、位置・回転・Scale の正本です。
- 位置の値 `Scalar` は Q95.96 の 192 bit 固定小数点です。範囲は ±3.96×10²⁸ m、分解能は 2⁻⁹⁶ m（約 1.26×10⁻²⁹ m）です。
- 回転は Q1.126 の 128 bit です。
- Unity の Transform は、有限な値へ変換した表示用の写し（Proxy）です。正本ではありません。
- 溢れる可能性のある演算は、すべて `Try` で始まる API です。結果を黙って丸めたり、飽和させたりしません。

## Radial（対数）描画

目からの距離 *d* を、次の式で写してから描きます。

```text
F(d) = log2(1 + d · M)
```

- *M* は View Scale で、カメラの Near ÷ 論理 Near です（`RadialCamera`）。写像の曲がり始めは、論理距離でおよそ 1/*M* です。調整できるのは *M* だけです。
- 写像は視線の向きを変えません。そのため、標準の描画で表せる範囲の画像は、標準の描画と同じになります。それより遠いものは圧縮されて、同じ画面に収まります。
- 色だけでなく、深度（`SV_Depth`）、影、Culling、Picking、Motion Vectors も、標準の描画と同じ空間の約束を守ります。
- 照明は、写像した後の位置ではなく、物理的な位置で計算します。

## Fail Closed

Omni は、正しく描けると確かめられない経路では、もっともらしい絵を出さずに止まります。例を挙げます。

- 物理的な照明のデータを渡せない Lit の経路は、描画しません。
- 対応していないビルド対象や Renderer で RadialCamera を含む Scene をビルドしようとすると、ビルドの開始時に理由を示して止まります。
- 変換できない Material は、元の Material を残したまま診断用の Material に置き換え、結果を成功とは表示しません。

## 次に読むもの

| 目的 | ページ |
| --- | --- |
| 導入して動かす | [動作環境](getting-started/requirements/)、[導入](getting-started/installation/)、[Verification Hub で確かめる](getting-started/verification/)、[標準描画と対数描画を切り替える](getting-started/switch-rendering/) |
| 仕組みを知る | [Logical 座標と Proxy](concepts/logical-coordinates/)、[Radial の写像と View Scale](concepts/radial-mapping/)、[Near と Far](concepts/near-far/)、[Fail Closed](concepts/fail-closed/) |
| API を調べる | [Omni リファレンス](../reference/) |
| 機能ごとの使い方 | [Scene View の操作](guides/scene-view/)、[Shader と Material](guides/shaders-and-materials/)、[Shader Graph と独自の Shader](guides/shader-graph/)、[VFX Graph](guides/vfx/)、[Terrain](guides/terrain/)、[照明の空間](guides/lighting-spaces/)、[Scene BRG](guides/scene-brg/)、[Radial 観察カメラ](guides/observer-camera/)、[Sequencer と Pose Bake](guides/sequencer/) |
