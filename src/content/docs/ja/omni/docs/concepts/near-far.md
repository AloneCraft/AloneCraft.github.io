---
title: Near と Far
description: 論理 Near、ハードウェアの Near、Culling の Near の違いと、RadialCamera の Inspector の診断の読み方。
sidebar:
  order: 3
---

Radial の描画では、「Near」と呼ぶ値が複数あります。役割が違うので、同じ値として扱いません。

| 値 | 役割 |
| --- | --- |
| 論理 Near | Logical のカメラ空間で、近すぎる物を判定する条件。RadialCamera の Near |
| ハードウェアの Near | Unity の投影行列に渡す Near の面。描画の間だけ Omni が決める |
| Culling の Near | 描画の候補を CPU で外すときの条件 |
| View Scale *M* | 論理の距離と描画空間の距離をつなぐ倍率 |

## 論理 Near は平面

論理 Near は、カメラからの球ではなく、カメラの前方に垂直な平面です。

```text
logicalViewDepth = dot(P - C, F)
見える条件         logicalViewDepth >= nearClip
```

*P* は対象の位置、*C* はカメラの位置、*F* はカメラの前方の単位ベクトルです。計算は Logical の精度で行います。

## 透視投影では、中央が少し手前まで残る

写像は Near の近くでも恒等ではありません。そのため、透視投影では、論理 Near の平面を 1 枚のハードウェアの Near の面で表せません。

Omni は、画面の四隅で切り過ぎないハードウェアの Near を選びます。その結果、画面の中央では、論理 Near より少し手前の物が残ります。これを **Center Under-clip** と呼びます。量は RadialCamera の Inspector に出ます。

:::note[切り過ぎない側に倒す]
ハードウェアの Near は、常に安全な側に倒します。論理 Near より奥にある物を切ることはありません。
:::

平行投影（Orthographic）のカメラでは、対数の写像をかけません。論理 Near をそのまま投影の Near に使うので、中央も四隅も一致します。

## Far

RadialCamera の Far は、描く論理距離の上限です。ここより遠い対象は描きません。

- CPU の判定と、写像の後の形を包む Bounds に使います。Far より遠い対象は、Radial の判定で非表示になります。
- GPU のハードウェアの Far にも使います。論理 Far を写した距離が収まるように、カメラごとの描画の間だけ Camera の Near、Far、投影行列を合わせ、描画の後に元へ戻します。

対数の写像は無限遠を有限の描画距離へ畳みます。上限を置かないと、どこまで見えるかが、誰も指定していない内部の値で決まってしまいます。Far はその上限を明示する値です。既定値は `Scalar` の上限（約 3.96×10²⁸ m）です。

## Far と Near の比の上限

32 bit の `float` の透視投影は、Far と Near の比が極端に大きいと壊れます。Far の面の法線がゼロになり、計算が NaN や無限大を生みます。

Omni は、ハードウェアの Far と Near の比を 2²⁰（1,048,576）以下に保ちます。超える場合は、Near を保ち、Far だけを縮めます。

Radial の写像は遠方を対数で縮めるので、通常はこの上限に届きません。例えば *M* = 1 なら、`Scalar` の上限の距離も、描画空間ではおよそ 95 です。

## Culling の Near

CPU の Culling は、論理 Near を Near の条件に使います。論理 Near が 10⁹ m を超える場合、Culling には 10⁹ m を使います。Culling の Near が論理 Near より奥になることはありません。

## RadialCamera の Inspector

### Clipping Planes

| 項目 | 内容 |
| --- | --- |
| Near | Camera の Near の面が表す論理距離（m）。小さいほど近くを大きく描く |
| Far | 描く論理距離の上限（m） |

どちらも `ScalarField` の欄です。式を入力できます（[Logical 座標と Proxy](../logical-coordinates/) の「Inspector で値を入れる」）。単位は論理の m で、Camera の Clipping Planes（描画空間）とは別の値です。

次の場合は警告が出ます。

- 「Near が0以下です。」「Far が0以下です。」: 正の値でないと写像が成り立ちません。描画には既定値を使います。
- 「Far は Near より大きくしてください。」: 描く論理距離の範囲が空です。

### 診断情報

| 項目 | 内容 |
| --- | --- |
| 小数部ビット数、固定小数点の最小単位 | `Scalar` の形式（96 bit、2⁻⁹⁶ m） |
| View Scale (M)、log2(View Scale) | 今の *M* |
| View Scaleの算出元 | M = Camera Near ÷ Logical Near |
| Camera Near Clip、Logical Near、Camera Far Clip | 設定した値 |
| Camera Near Clip の論理距離、Camera Far Clip の論理距離 | Camera の Near と Far を論理距離へ戻した値 |
| Far に必要な描画 Far | 論理 Far を描くのに必要な描画空間の Far |
| 圧縮が始まる論理距離（S / M） | 曲線が曲がり始める論理距離 |
| 使用中の Camera | 対象の Camera |
| 論理Near Clip平面 | 論理 Near の値 |
| Safe Render Near（内部値） | ハードウェアの Near に使う値 |
| クリップ方式 | 「Hardware平面が論理値と一致」（平行投影）または「保守的」（透視投影） |
| Frustum Corner cos | 前方と、視錐台の四隅の向きのなす角の cos |
| 中心のunder-clip | 画面の中央で、論理 Near より手前に残る量（%） |

透視投影のカメラでは、次の説明が出ます。「写像はごく近い範囲も圧縮するため、論理 Near 平面を1枚のハードウェアの near 平面では表せません。描画で切り過ぎることはありませんが、画面中央では Near より少し手前の形が残ります（Center Under-clip を参照）。」

:::tip[設定した距離で切れないと感じたら]
Camera の Near（描画空間）と RadialCamera の Near（論理空間）は、*M* でつながった別の値です。まず診断情報の「Logical Near」「Safe Render Near（内部値）」「中心のunder-clip」を確かめてください。
:::
