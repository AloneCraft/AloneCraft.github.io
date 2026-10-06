---
description: "角速度を固定小数点で持つ型です。入出力は rad/s、中身は turn/s です。"
---

角速度を固定小数点で持つ型です。入出力は rad/s（ラジアン毎秒）です。中身は turn/s（回転毎秒）で持ち、小数部は 96 bit です。生の値は [Scalar](../../logical/scalar/) と同じ 192 bit なので、範囲はおよそ ±2⁹⁵ turn/s です。中身を turn で持つので、[Duration](../duration/) を掛けて [Angle](../angle/) を求めるときに、2π の丸めが角度に入りません。

rad/s から作るときは、2π（分解能 2⁻⁹⁶ に丸めた値）で割り、最近接偶数で丸めます。範囲を超える演算は `false` を返し、`out` の値は `Zero` になります。

<!-- members -->

### FractionalBits

中身（turn/s）の小数部のビット数です。値は 96 です。

### Zero

0 rad/s です。

### IsNegative

値が負（逆向きの回転）のときに `true` です。

### IsZero

値が 0 のときに `true` です。

### TryFromRadiansPerSecond(string, out AngularVelocity)

rad/s の 10 進の文字列（指数の表記も可）から作ります。文字列の形式は `Scalar.TryParse` と同じです。不正な文字列や範囲外の値では `false` を返します。

### TryFromRadiansPerSecond(double, out AngularVelocity)

rad/s の `double` から作ります。NaN、無限大、範囲外の値では `false` を返します。

### TryFromKepler

重力パラメータ μ（m³/s²）と軌道半径 r（m）から、円軌道の角速度 √(μ ÷ r³) を求めます。計算はすべて固定小数点で行います。μ または r が 0 以下のとき、または途中の値が範囲を超えるときは `false` を返します。μ は [GravitationalParameter.TryToLogicalScalar](../gravitational-parameter/) で `Scalar` にできます。

### TryApplyOrbitClassQ16

角速度に 0.9 + 0.1 × (`orbitClassQ16` ÷ 65536) を掛けます。係数は 0.9 以上 1.0 未満です。結果が範囲を超えると `false` を返します。

### TryLerpQ16

`from + (to − from) × ratioQ16 ÷ 65536` を求めます。比率は 0 以上 1 未満です。結果が範囲を超えると `false` を返します。

### TryLerpQ32

`from + (to − from) × ratioQ32 ÷ 2³²` を求めます。比率は 0 以上 1 未満です。結果が範囲を超えると `false` を返します。

### TryCalculateHalfCellWindow

円を `angularCellCount` 個のセルに分けたとき、`maximumVelocity` で半セル（1 ÷ (2 × `angularCellCount`) 回転）を回る時間を [Duration](../duration/) で返します。`angularCellCount` が 2 未満、`maximumVelocity` が 0 以下、または結果が 0 に丸められるときは `false` を返します。

### TryToRadiansPerSecond

rad/s の `double` に変換します。`double` へは最近接偶数で丸めます。

### TryScale

角速度に [TimeScale](../time-scale/) を掛けます。最近接偶数で丸め、範囲を超えると `false` を返します。

### TryMultiply

角速度 × 時間（[Duration](../duration/)）で回った角度を [Angle](../angle/) で返します。角度は 1 回転で折り返します。負の結果も 0 から 1 回転の範囲に折り返します。途中の値が範囲を超えるときだけ `false` を返します。

### CompareTo

### Equals(AngularVelocity)

### Equals(object)

### GetHashCode

### ToString

rad/s の値を 10 進で返します。rad/s への変換が範囲を超えるときは `"0"` を返します。

### operator ==

### operator !=

### operator <

### operator >

### operator <=

### operator >=
