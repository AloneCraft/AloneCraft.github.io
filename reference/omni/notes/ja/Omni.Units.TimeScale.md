---
description: "時間の倍率を小数部 96 bit の固定小数点で持つ型です。"
---

時間の倍率（単位なし）を持つ、符号付きの固定小数点の型です。小数部は 96 bit で、分解能は 2⁻⁹⁶ です。生の値は [Scalar](../../logical/scalar/) と同じ 192 bit なので、範囲はおよそ ±2⁹⁵ です。

[Duration.TryScale](../duration/) と [AngularVelocity.TryScale](../angular-velocity/) に渡して、時間や角速度に倍率を掛けます。

<!-- members -->

### FractionalBits

小数部のビット数です。値は 96 です。

### Zero

倍率 0 です。

### One

倍率 1 です。

### IsNegative

値が負のときに `true` です。

### IsZero

値が 0 のときに `true` です。

### TryParse

10 進の文字列（指数の表記も可）から作ります。文字列の形式は `Scalar.TryParse` と同じで、分解能へ最近接偶数で丸めます。不正な文字列や範囲外の値では `false` を返し、`result` は `Zero` になります。

### TryFromDouble

`double` から作り、分解能へ最近接偶数で丸めます。NaN、無限大、範囲外の値では `false` を返します。

### TryToDouble

`double` に変換します。`double` へは最近接偶数で丸めます。

### CompareTo

### Equals(TimeScale)

### Equals(object)

### GetHashCode

### ToString

10 進の全桁で返します。カルチャに依存せず、末尾の 0 は出しません。

### operator ==

### operator !=

### operator <

### operator >

### operator <=

### operator >=
