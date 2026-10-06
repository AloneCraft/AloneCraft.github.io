---
description: "0 以上 1 以下の比率を小数部 126 bit の固定小数点で持つ型です。"
---

0 以上 1 以下の比率を持つ固定小数点の型です。小数部は 126 bit で、分解能は 2⁻¹²⁶（約 1.18×10⁻³⁸）です。`TryParse` は 0 から 1 の外の値を受け付けません。

<!-- members -->

### FractionalBits

小数部のビット数です。値は 126 です。

### Zero

0 です。

### One

1 です。

### TryParse

10 進の文字列（指数の表記も可）から作ります。文字列の形式は `Scalar.TryParse` と同じで、分解能へ最近接偶数で丸めます。不正な文字列、負の値、1 より大きい値では `false` を返し、`result` は `Zero` になります。

### FromQ32

32 bit の整数から作ります。今の版では、結果は `value` ÷ 2⁹⁴ です。`value` ÷ 2³²（Q32 の比率）にはならないので注意してください。

### TryToDouble

`double` に変換します。`double` へは最近接偶数で丸めます。

### CompareTo

### Equals(UnitInterval)

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
