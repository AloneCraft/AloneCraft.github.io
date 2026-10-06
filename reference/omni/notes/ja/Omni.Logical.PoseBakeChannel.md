---
description: "1 つの対象（カメラ、主光源など）の、時刻のサンプル列です。"
---
1 つの対象（カメラ、主光源など）の、時刻のサンプル列です。[PoseBake](../pose-bake/) の中に保存します。

Logical の値は、サンプルごとの [ScalarField](../../logical/scalar-field/) ではなく、チャンネルごとの平らな `ulong` の配列に、固定小数点の raw の値（limb）のまま詰めます。シリアライズのたびの小さな配列の確保を避け、Asset の行数も小さくするためです。画角、クリップの範囲、光の強さ、色は空間の量ではないので、`float` と `Color` のまま持ちます。

持つ値は種類（[PoseBakeChannelKind](../pose-bake-channel-kind/)）で決まります。

| 種類 | 持つ値 |
| --- | --- |
| `CameraPose` | 位置、回転、カメラの距離、注視点のオフセット、注視対象の索引、画角、クリップの範囲 |
| `LightRotation` | 回転、光の強さ、色 |

`CreateForBake` で作り、`Write*` でサンプルを書きます。`Write*` は索引を確かめません。範囲の外の索引や、その種類が持たない値を書くと例外になります。読むときは `TryRead*` か [PoseBakeSampler](../pose-bake-sampler/) を使います。手順は [Sequencer と Pose Bake](../../../docs/guides/sequencer/) を見てください。

<!-- members -->

### m_TargetPath

対象を指す安定した識別子です（`"Camera"`、`"KeyLight"` など）。Clip と Track の Channel Target Path と照合します。

### m_Kind

このチャンネルが動かす対象の種類です。

### m_SampleCount

サンプルの数です。

### m_PositionLimbs

位置のサンプルです。1 サンプルあたり X、Y、Z の 3 成分の raw の値を並べます。

### m_RotationLimbs

回転のサンプルです。1 サンプルあたり X、Y、Z、W の 4 成分の raw の値（回転の 128 ビットの形式）を並べます。

### m_DistanceLimbs

カメラの距離のサンプルです。1 サンプルあたり 1 つの raw の値です。

### m_LookOffsetLimbs

注視点の Logical のオフセットのサンプルです。1 サンプルあたり 3 成分の raw の値です。

### m_TargetIndices

注視対象の索引のサンプルです。`PoseBake.TargetIds` の索引です。

### m_FieldOfView

画角（度）のサンプルです。

### m_NearClip

近いクリップの距離のサンプルです。

### m_FarClip

遠いクリップの距離のサンプルです。

### m_LightIntensity

光の強さのサンプルです。

### m_LightColor

光の色のサンプルです。

### OnBeforeSerialize

何もしません。

### OnAfterDeserialize

Unity が読み込んだ直後に呼びます。値ごとに 2 limb だった旧形式（Logical が 128 ビットだったころ）の位置、距離、注視点のオフセットを、同じ数値の今の形式へ広げます。回転の形式は変わっていないので、そのままです。

### TargetPath

対象を指す安定した識別子を返します。

### Kind

このチャンネルが動かす対象の種類を返します。

### SampleCount

サンプルの数を返します。

### HasPosition

位置のサンプルを、宣言した数だけ持つときに `true` を返します。サンプルの数が 0 なら `false` です。

### HasRotation

回転のサンプルを、宣言した数だけ持つときに `true` を返します。

### HasFieldOfView

画角のサンプルを、宣言した数だけ持つときに `true` を返します。

### HasClipRange

近いクリップと遠いクリップのサンプルを、どちらも宣言した数だけ持つときに `true` を返します。

### HasLightShading

光の強さと色のサンプルを、どちらも宣言した数だけ持つときに `true` を返します。

### HasTargetIndices

注視対象の索引のサンプルを、宣言した数だけ持つときに `true` を返します。

### HasDistance

カメラの距離のサンプルを、宣言した数だけ持つときに `true` を返します。

### HasLookOffset

注視点のオフセットのサンプルを、宣言した数だけ持つときに `true` を返します。

### CreateForBake

焼くときに、種類に合わせてサンプルの配列を確保したチャンネルを作ります。回転は両方の種類が持ちます。`CameraPose` は位置、距離、注視点のオフセット、注視対象の索引、画角、クリップの範囲を、`LightRotation` は光の強さと色を確保します。値はすべて 0 で始まります。

### WritePosition

`index` 番のサンプルの位置を書きます。`CameraPose` だけが持つ値です。

### WriteRotation

`index` 番のサンプルの回転を書きます。正規化はしません。

### WriteDistance

`index` 番のサンプルのカメラの距離を書きます。`CameraPose` だけが持つ値です。

### WriteLookOffset

`index` 番のサンプルの注視点の Logical のオフセットを書きます。`CameraPose` だけが持つ値です。

### WriteCameraSettings

`index` 番のサンプルの、注視対象の索引、画角（度）、近いクリップ、遠いクリップを書きます。`CameraPose` だけが持つ値です。

### WriteLightShading

`index` 番のサンプルの光の強さと色を書きます。`LightRotation` だけが持つ値です。

### TryReadPosition

`index` 番のサンプルの位置を読みます。位置のサンプルが無いか、索引が範囲の外なら `false` を返し、`value` は原点です。

### TryReadRotation

`index` 番のサンプルの回転を読みます。保存した raw の値と一致させるため、正規化はしません。回転のサンプルが無いか、索引が範囲の外なら `false` を返し、`value` は恒等の回転です。

### TryReadDistance

`index` 番のサンプルのカメラの距離を読みます。読めないときは `false` を返し、`value` は 0 です。

### TryReadLookOffset

`index` 番のサンプルの注視点のオフセットを読みます。読めないときは `false` を返し、`value` は 0 のベクトルです。

### TryReadTargetIndex

`index` 番のサンプルの注視対象の索引を読みます。読めないときは `false` を返し、`targetIndex` は −1 です。

### TryReadFieldOfView

`index` 番のサンプルの画角（度）を読みます。読めないときは `false` を返し、`fieldOfView` は 0 です。

### TryReadClipRange

`index` 番のサンプルの近いクリップと遠いクリップを読みます。読めないときは `false` を返し、どちらも 0 です。

### TryReadLightShading

`index` 番のサンプルの光の強さと色を読みます。読めないときは `false` を返し、`intensity` は 0、`color` は白です。

### IsWellFormed

宣言したサンプルの数と、配列の長さが合っているかを返します。サンプルの数が正で、回転と、その種類が持つすべての値の配列の長さが正しいときに `true` です。壊れたデータを途中まで読んで、別のサンプルの値を混ぜてしまうことを防ぎます。`PoseBake.TryValidate` がこれを使います。
