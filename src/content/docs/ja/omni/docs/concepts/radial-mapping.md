---
title: Radial の写像と View Scale
description: 目からの距離を対数で写す式、View Scale の決まり方、写像を変える部品。
sidebar:
  order: 2
---

## 写像の式

Radial は、目（写像の中心）からの距離 *d* を、次の式で写してから描きます。

```text
F(d) = S · log2(1 + d · M / S)    S = 1
```

| 記号 | 意味 |
| --- | --- |
| *d* | 目からの論理距離（m） |
| *F*(*d*) | 描画空間での距離（Unity の単位） |
| *S* | 定数の 1。設定はありません |
| *M* | View Scale。調整できるのはこれだけです |

*S* = 1 なので、式は `F(d) = log2(1 + d · M)` と同じです。

曲線は 1 つだけです。線形の範囲も、曲線や *S* の選択もありません。近くの物も曲線を通して写すので、写像の扱いの誤りがカメラの近くでも見えます。

### 曲線の形

| 範囲 | 振る舞い |
| --- | --- |
| *d* が 1/*M* よりずっと小さい | *F*(*d*) ≈ *d* · *M* / ln 2。ほぼ比例する（傾きは約 1.44 × *M*） |
| *d* が 1/*M* よりずっと大きい | *F*(*d*) ≈ log2(*d* · *M*)。距離が 2 倍になるごとに、描画距離が 1 進む |

曲がり始めは、論理距離でおよそ 1/*M* です。*M* = 1 でも写像は恒等になりません。原点の近くの傾きは *M* / ln 2 です。

*M* = 1 のときの値の例です。

| 論理距離 *d* | *F*(*d*) |
| --- | --- |
| 1 m | 1 |
| 1 km | 約 10.0 |
| 1 天文単位（約 1.5×10¹¹ m） | 約 37.1 |
| 1 光年（約 9.5×10¹⁵ m） | 約 53.1 |
| 2⁹⁵ m（`Scalar` の上限、約 3.96×10²⁸ m） | 約 95 |

遠方では、*M* を変えると描画距離におよそ log2 *M* が足されます。Logical 座標で表せる最も遠い点も、描画空間では有限の距離に収まります。

## 向きを保つ写像

写像は距離だけを変え、視線の向きを変えません。また、*F* は距離とともに増えるので、前後の関係も変わりません。

そのため、目が写像の中心にあるカメラ（Game View、Scene View の主観の視点）では、次のことが成り立ちます。

- 標準の描画で表せる範囲の画像は、標準の描画と同じです。位置、遮蔽、照明、影、Lightmap が一致します。
- それより遠いものは、標準の描画では表せません。Radial の描画だけが、同じ画面に圧縮して描きます。
- 深度（`SV_Depth`）、影、Culling、Picking、Motion Vectors も、標準の描画と同じ空間の約束を守ります。

写像は目からの距離の関数です。そのため、1 台のカメラが見るすべての対象に同じ曲線をかけます。目が写像の中心にない観察用の表示は、[Radial 観察カメラ](../../guides/observer-camera/)を見てください。

### 照明は物理的な位置で計算する

照明は、写像した後の位置ではなく、物理的な位置で計算します。法線、光源、視線の向き、影の情報も物理的な空間で評価し、写像した位置と混ぜません。Light Probe と APV の空間については [照明の空間](../../guides/lighting-spaces/) を見てください。

## View Scale *M*

*M* は次の式で決まります。

```text
M = Camera の Near ÷ RadialCamera の Near（論理 Near）
```

- Camera の Near（`Camera.nearClipPlane`）は、描画空間の距離です。Inspector で設定した値をそのまま使います。
- RadialCamera の Near（`nearClip`）は、その Near の面が論理空間で何 m を表すかです。
- Omni は描画の間だけ Camera の Near と Far を一時的に変え、描画の後に元へ戻します。*M* の計算には、一時的な値ではなく、設定した Near を使います。

例えば、Camera の Near が 0.1 で、論理 Near が 10⁻⁹ m（1 nm）なら、*M* = 10⁸ です。Camera の Near を下げずに、微小な対象を描けます。

RadialCamera の Near の既定値は 0.3 m で、Unity の Camera の Near の既定値と同じです。Camera の Near も 0.3 なら、*M* = 1 です。

:::note[*M* で変わるもの]
写像は向きを保つので、*M* を変えても、画面の上の位置は変わりません。*M* が決めるのは、Camera の Near の面が論理で何 m に当たるかと、圧縮が始まる論理距離です。
:::

## RadialCamera

`RadialCamera` は Camera に付けるデータの入れ物です。論理 Near と論理 Far を持ちます。通常は Omni Manager の対数描画への切替が追加します（[標準描画と対数描画を切り替える](../../getting-started/switch-rendering/)）。

| メンバー | 内容 |
| --- | --- |
| `nearClip`（`ScalarField`） | 論理 Near（m）。Inspector の Clipping Planes の Near |
| `farClip`（`ScalarField`） | 論理 Far（m）。既定値は `Scalar` の上限 |
| `EffectiveNearClip`、`EffectiveFarClip` | 描画に使う値。0 以下のときは既定値 |
| `EffectiveLog2ViewScale` | log2(*M*)。Camera に設定した Near と論理 Near から求める。求められないときは 0 |
| `SetViewScale(float)` | 論理 Near を「Camera の Near ÷ 指定した *M*」にする。*M* が正で有限でなければ *M* = 1 にする |
| `SetIdentityViewScale()` | *M* = 1 にする。写像を恒等にはしない |
| `RadialCamera.GetOrAdd(Camera)` | Camera の RadialCamera を返す。無ければ追加する |

Inspector の診断の表示は [Near と Far](../near-far/) で説明します。

## CPU で同じ写像を使う

`RadialSettings` は、GPU の写像と同じ曲線を CPU で計算します。`RadialSettings.FromDriver(radialCamera)` で、その RadialCamera の *M* と論理 Far を取り込みます。

| メソッド | 内容 |
| --- | --- |
| `TryMap(in Vector3, out float3)` | 写像の中心からの Logical の相対位置を、描画空間の相対位置へ写す。向きは保つ |
| `TryUnmap(in float3, out Vector3)` | `TryMap` の逆 |
| `TryMapDistance(in Scalar, out float)` | 距離だけを写す |
| `TryUnmapDistance(float, out Scalar)` | 描画距離を論理距離へ戻す |
| `TryGetLog2Scale(in Scalar, out float)` | その距離での倍率（描画距離 ÷ 論理距離）の log2 |

どれも、入力または結果が有限でないときは `false` を返します。

```csharp
using Omni;
using L = Omni.Logical;

RadialSettings settings = RadialSettings.FromDriver(radialCamera);
if (L.Scalar.TryParse("149597870700", out L.Scalar au) &&
    settings.TryMapDistance(au, out float rendered))
{
    // rendered は 1 天文単位の描画空間での距離
}
```

## 写像を変える部品

Renderer ごとに、写像のかけ方を変える Component があります。

| Component | 内容 |
| --- | --- |
| `RadialIdentityWarp` | 同じ GameObject の Renderer または Terrain を、Radial の Shader のまま恒等の写像で描く |
| `Omni.Logical.RadialRigidAnchor` | 同じ GameObject の Renderer を、中心の 1 点だけ写像し、形を保ったまま描く |

### RadialIdentityWarp

URP の元の Shader、Radial の恒等、Radial の対数を、同じカメラ・同じフレームで並べて比べるための部品です。

- 実際の Transform と Unity の Bounds で描きます。Culling は URP と同じ判定です。
- Scene BRG の対象から外し、Renderer ごとの経路で描きます。
- Terrain では、Terrain の本来の描画へ恒等の値を渡します。

### RadialRigidAnchor

写像は非線形なので、頂点ごとに写すと、頂点ごとに圧縮の率が変わり、形が変わります。球のような実体では、その変形が正しい絵です。一方、文字やラベルは読めなくなります。

`RadialRigidAnchor` は、中心の 1 点だけを写像し、局所の形にはその点での倍率を等方にかけます。頂点を足したり、Transform を動かしたりはしません。

:::caution[近似です]
中心の 1 点の写像を、形全体へ当てはめる近似です。中心から見て、対象が狭い距離の比と立体角に収まっているときだけ妥当です。惑星の地形のように、面の中でも写像を連続して評価すべき対象には使いません。
:::
