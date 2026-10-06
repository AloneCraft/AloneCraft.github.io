---
description: "秒を小数部 64 bit の固定小数点で持つ時間の型です。"
---

時間を秒で持つ、符号付きの固定小数点の型です。小数部は 64 bit で、分解能は 2⁻⁶⁴ 秒（約 5.4×10⁻²⁰ 秒）です。生の値は [Scalar](../../logical/scalar/) と同じ 192 bit なので、範囲はおよそ ±2¹²⁷ 秒です。

加算・減算・整数倍は丸めずに正確です。`double` を足し続けたときのような誤差がたまりません。範囲を超える演算は `false` を返し、`out` の値は `Zero` になります。

<!-- members -->

### FractionalBits

小数部のビット数です。値は 64 です。

### Zero

0 秒です。

### OneSecond

1 秒です。

### IsNegative

値が負のときに `true` です。

### IsZero

値が 0 のときに `true` です。

### TryFromSeconds(string, out Duration)

秒の 10 進の文字列（指数の表記も可）から作ります。文字列の形式は `Scalar.TryParse` と同じで、分解能へ最近接偶数で丸めます。不正な文字列や範囲外の値では `false` を返します。

### TryFromSeconds(double, out Duration)

秒の `double` から作り、分解能へ最近接偶数で丸めます。NaN、無限大、範囲外の値では `false` を返します。

### TryToSeconds

秒の `double` に変換します。`double` へは最近接偶数で丸めます。

### TryToSecondsFloat

秒の `float` に変換します。`float` へは最近接偶数で丸めます。

### TryAdd

`left + right` を求めます。丸めはありません。範囲を超えると `false` を返します。

### TrySubtract

`left − right` を求めます。丸めはありません。範囲を超えると `false` を返します。

### TryScale

時間に [TimeScale](../time-scale/) の倍率を掛けます。分解能へ最近接偶数で丸め、範囲を超えると `false` を返します。

### TryDivideToFloor

`numerator ÷ denominator` の商を切り捨てて `long` で返します（例: 経過時間が周期の何回分か）。どちらかが負、`denominator` が 0、または商が `long` の範囲（2⁶³ 未満）を超えるときは `false` を返し、`quotient` は 0 になります。

### TryMultiply

時間を整数倍します。丸めはありません。範囲を超えると `false` を返します。

### CompareTo

### Equals(Duration)

### Equals(object)

### GetHashCode

### ToString

秒の値を 10 進の全桁で返します。カルチャに依存せず、末尾の 0 は出しません。

### operator ==

### operator !=

### operator <

### operator >

### operator <=

### operator >=
