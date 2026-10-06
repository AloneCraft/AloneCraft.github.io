---
description: "Logical 座標の固定小数点の形式（Q95.96 と Q1.126）を表す定数です。"
---

Logical 座標の数値の形式を表す定数です。位置・距離・Scale の [Scalar](../scalar/) は Q95.96（192 bit）、回転の [RotationScalar](../rotation-scalar/) は Q1.126（128 bit）です。形式は固定で、実行時や設定で変わりません。

<!-- members -->

### TotalBits

`Scalar` の全体のビット数です。値は 192 です。

### FractionalBits

`Scalar` の小数部のビット数です。値は 96 です。

### IntegerBits

`Scalar` の整数部のビット数（符号を除く）です。値は 95 で、範囲はおよそ ±2⁹⁵ です。

### RotationFractionalBits

`RotationScalar` の小数部のビット数です。値は 126 です。

### LimbCount

`Scalar` の生の値を 64 bit ずつに分けた数です。値は 3 です。

### WideBits

乗算の途中で使う整数のビット数です。値は 384（`TotalBits` の 2 倍）です。

### WideLimbCount

乗算の途中で使う整数を 64 bit ずつに分けた数です。値は 6 です。

### CurrentResolution

`Scalar` の最小の正の値（2⁻⁹⁶、約 1.26×10⁻²⁹）です。

### TryGetPowerOfTwoExponent

`value` がちょうど 2 の冪なら、その指数を返します（`value` = 2^`binaryExponent`）。`value` が 0 以下、または 2 の冪でないときは `false` を返し、`binaryExponent` は 0 になります。
