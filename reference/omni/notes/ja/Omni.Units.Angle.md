---
description: "1 回転を 2⁶⁴ に分けた循環する角度（Q0.64 の turn）です。"
---

1 回転を 2⁶⁴ に分けて持つ、循環する角度です。形式は Q0.64 の turn（回転数）で、範囲は 0 以上 1 回転未満です。1 回転を超える値は、1 回転で割った余りになります。分解能は 2⁻⁶⁴ 回転（約 3.4×10⁻¹⁹ rad）です。

角度の加算は正確で、何回足しても誤差がたまりません。角速度と時間から角度を求めるには [AngularVelocity.TryMultiply](../angular-velocity/) を使います。比較（`<` など）は 0 から 1 回転までの値の大小で、循環を考えません。

<!-- members -->

### Zero

0 回転です。

### RawTurns

生の値です。1 回転を 2⁶⁴ とした整数（0 以上 2⁶⁴ 未満）です。

### FromSector

円を `sectorCount` 個の扇形に分けたときの、`sectorIndex` 番目の扇形の中の位置から角度を作ります。`phaseWithinSectorQ32` は扇形の中の位置で、2³² が扇形 1 つ分です。結果は (`sectorIndex` + `phaseWithinSectorQ32` ÷ 2³²) ÷ `sectorCount` 回転で、端数は切り捨てます。`sectorCount` が 0 以下、または `sectorIndex` が 0 から `sectorCount − 1` の外のときは `ArgumentOutOfRangeException` を投げます。

### Add

2 つの角度を足します。1 回転を超えた分は折り返します。失敗はありません。

### ToTurns

回転数を `double` で返します（0 以上 1 以下）。1 回転に非常に近い値は、`double` の丸めで 1.0 になることがあります。

### ToRadians

ラジアンを `double` で返します（`ToTurns()` × 2π）。

### GetCellIndex

円を `cellCount` 個のセルに等分したとき、この角度が入るセルの番号（0 から `cellCount − 1`）を返します。整数だけで計算するので、境界の判定に丸め誤差がありません。`cellCount` が 0 以下のときは `ArgumentOutOfRangeException` を投げます。

### TrySinCos

sin と cos を [Scalar](../../logical/scalar/) で求めます。32 回反復の CORDIC を整数だけで計算し、浮動小数点を使いません。精度は反復の回数で決まり、`Scalar` の分解能（2⁻⁹⁶）よりずっと粗くなります。常に `true` を返します。

### CompareTo

### Equals(Angle)

### Equals(object)

### GetHashCode

### ToString

回転数（`ToTurns()` の値）を、カルチャに依存しない `double` の文字列で返します。

### operator ==

### operator !=

### operator <

### operator >

### operator <=

### operator >=
