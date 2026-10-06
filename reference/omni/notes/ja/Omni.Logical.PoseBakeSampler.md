---
description: "焼いたサンプル列から、任意の時刻の値を Logical のまま求める関数です。"
---
焼いたサンプル列（[PoseBake](../pose-bake/) と [PoseBakeChannel](../pose-bake-channel/)）から、任意の時刻の値を Logical のまま求める静的な関数の集まりです。Scene にも Component にも触れないので、Edit Mode のテストからそのまま使えます。実行時のメモリの確保はありません。

使い方は 2 段です。

1. `TryResolveSample` で、時刻から前後のサンプルの索引と区間の中の比率を求めます。
2. `TrySample*` に索引と比率を渡して、値ごとに補間します。

```csharp
using L = Omni.Logical;

if (L.PoseBakeSampler.TryResolveSample(bake, time, out int i, out int next, out L.Scalar t, out var rejection) &&
    L.PoseBakeSampler.TrySamplePosition(channel, i, next, t, out L.Position3 position) &&
    L.PoseBakeSampler.TrySampleRotation(channel, i, next, t, out L.Quaternion rotation))
{
    // position と rotation を使う
}
```

`float` の比率を取る関数（画角、クリップの範囲、光）には、`t` を `float` にして（`t.TryToFloat(out float f)`）渡します。焼いた範囲の外の時刻は棄却せず、端のサンプルで止まります。範囲の端や丸めでわずかに外へ出た瞬間だけ別の経路に切り替わると、絵が跳ねるためです。詳しくは [Sequencer と Pose Bake](../../../docs/guides/sequencer/) を見てください。

<!-- members -->

### TryResolveSample

時刻 `timeSeconds`（秒、シーケンスの時刻）から、区間の始まりのサンプルの索引 `index`、終わりの索引 `nextIndex`、区間の中の比率 `fraction`（0〜1 の Logical の値）を求めます。最初のサンプルより前は先頭、最後のサンプルより後は末尾に止まり、そのとき `index` と `nextIndex` は同じで、`fraction` は 0 です。`bake` が `null` なら `MissingAsset`、サンプルの数か間隔が 0 以下なら `EmptySamples`、比率を Logical の値にできなければ `InterpolationFailed` を `rejection` に入れて `false` を返します。

### TrySamplePosition

前後のサンプルの位置を、Logical のまま線形に補間します。`channel` が `null`、位置のサンプルが無い、索引が範囲の外、または補間が範囲を超えたときは `false` を返します。`index` と `nextIndex` が同じか、比率が 0 なら、補間せずに `index` の値を返します。

### TrySampleRotation

前後のサンプルの回転を、正規化した線形補間（nlerp）で補間します。読めないとき、または正規化できないときは `false` を返します。`index` と `nextIndex` が同じか、比率が 0 なら、補間も正規化もせずに `index` の値を返します。

### TrySampleDistance

前後のサンプルのカメラの距離を、Logical のまま線形に補間します。読めないとき、または補間が範囲を超えたときは `false` を返します。

### TrySampleLookOffset

前後のサンプルの注視点のオフセットを、Logical のまま線形に補間します。読めないとき、または補間が範囲を超えたときは `false` を返します。

### TrySampleFieldOfView

前後のサンプルの画角（度）を、`float` で線形に補間します。画角は空間の量ではないので、`float` のまま扱います。読めないときは `false` を返します。

### TrySampleClipRange

前後のサンプルの近いクリップと遠いクリップを補間します。クリップの距離は桁で変わるので、線形ではなく対数で補間します。どちらかの値が 0 以下なら線形の補間にします。読めないときは `false` を返します。

### TrySampleLightShading

前後のサンプルの光の強さと色を、線形に補間します。読めないときは `false` を返し、`intensity` は 0、`color` は白です。

### TryFractionTo

0〜1 の `double` の比率を、Logical の値へ取り込みます。0 以下は 0、1 以上は 1 にします。`float` 1 回で受けると仮数が 24 ビットに落ちるので、上位と残差の 2 つの `float` に分けて取り込み、約 48 ビットの分解能を保ちます。取り込めないときは `false` を返し、`result` は 0 です。
