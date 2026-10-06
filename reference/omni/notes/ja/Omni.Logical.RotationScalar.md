---
description: "Quaternion の各成分に使う Q1.126（128 bit）の固定小数点数です。"
---

[Quaternion](../quaternion/) の各成分に使う 128 bit の固定小数点数です。形式は Q1.126（整数部 1 bit、小数部 126 bit、符号付き 2 の補数）で固定です。範囲は −2 以上 2 未満、分解能は 2⁻¹²⁶（約 1.18×10⁻³⁸）です。単位 Quaternion の成分は −1 から 1 に収まるので、整数部は 1 bit です。

回転の丸めが位置に与える誤差は、距離のおよそ 2×10⁻³⁸ 倍です。位置の [Scalar](../scalar/)（192 bit）とは別の型です。この型の公開 API は、変換・符号反転・比較だけです。回転の計算は `Quaternion` のメソッドで行います。

<!-- members -->

### FractionalBits

小数部のビット数です。値は 126 です。

### Raw

生の値です。128 bit の 2 の補数の整数で、数値は `Raw` ÷ 2¹²⁶ です。

### Zero

0 です。

### One

1 です（`Raw` = 2¹²⁶）。

### RotationScalar

生の値（`Raw`）から値を作ります。通常は `TryFromDouble` や `TryFromFloat` を使います。

### IsNegative

値が負のときに `true` です。

### IsZero

値が 0 のときに `true` です。

### TryNegate

符号を反転します。最小の値 −2 は反転できないので `false` を返し、`result` は `Zero` になります。

### TryFromFloat

`float` から変換し、分解能 2⁻¹²⁶ へ最近接偶数で丸めます。NaN、無限大、−2 未満または 2 以上の値では `false` を返します。

### TryFromDouble

`double` から変換し、分解能 2⁻¹²⁶ へ最近接偶数で丸めます。NaN、無限大、−2 未満または 2 以上の値では `false` を返します。

### TryToFloat

`float` へ変換します。有効桁から外れる桁は最近接偶数で丸めます。

### CompareTo

### Equals(Omni.Logical.RotationScalar)

### Equals(object)

### GetHashCode

### ToString

10 進の全桁の文字列を返します。カルチャに依存せず、末尾の 0 は出しません。

### operator ==

### operator !=

### operator <

### operator >

### operator <=

### operator >=
