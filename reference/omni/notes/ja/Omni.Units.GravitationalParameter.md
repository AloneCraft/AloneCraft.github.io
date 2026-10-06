---
description: "重力パラメータ μ（m³/s²）を持つ型です。10 進の係数（最大 38 桁）と 10 進の指数で持ちます。"
---

重力パラメータ μ = GM を m³/s² で持つ型です。[Mass](../mass/) と同じく、10 進の係数（最大 38 桁）と 10 進の指数で持ちます。作れるのは正の値だけです。

軌道の計算に使うときは `TryToLogicalScalar` で [Scalar](../../logical/scalar/) に直し、[AngularVelocity.TryFromKepler](../angular-velocity/) などに渡します。

<!-- members -->

### IsZero

値が 0 のときに `true` です。`TryFromMetersCubedPerSecondSquared` は 0 を作らないので、`default` の値だけが 0 です。

### TryFromMetersCubedPerSecondSquared

m³/s² の 10 進の文字列（例: `3.986004418e14`）から作ります。文字列の形式は `Mass.TryFromKilograms` と同じです。不正な文字列、0、負の値では `false` を返します。

### TryToLogicalScalar

[Scalar](../../logical/scalar/)（Q95.96）に変換し、分解能 2⁻⁹⁶ へ最近接偶数で丸めます。2⁹⁵（約 3.96×10²⁸）以上の値では `false` を返します。

### TryToDouble

`double` に変換します。

### CompareTo

### Equals(GravitationalParameter)

### Equals(object)

### GetHashCode

### ToString

m³/s² の値を、係数と指数の形で返します。係数は末尾の 0 を除いた整数です（例: `3.986004418e14` は `3986004418e5`）。

### operator ==

### operator !=

### operator <

### operator >
