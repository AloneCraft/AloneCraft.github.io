---
description: "A Q1.126 (128-bit) fixed-point number used for each component of a Quaternion."
---

A 128-bit fixed-point number used for each component of a [Quaternion](../quaternion/). The format is fixed at Q1.126 (1 integer bit, 126 fractional bits, signed two's complement). The range is from −2 up to but not including 2, and the resolution is 2⁻¹²⁶ (about 1.18×10⁻³⁸). The components of a unit quaternion lie between −1 and 1, so one integer bit is enough.

The position error caused by rotation rounding is about 2×10⁻³⁸ times the distance. This is a separate type from the position type [Scalar](../scalar/) (192 bits). Its public API covers only conversion, negation and comparison; rotation math uses the methods of `Quaternion`.

<!-- members -->

### FractionalBits

The number of fractional bits. The value is 126.

### Raw

The raw value: a 128-bit two's complement integer. The number is `Raw` ÷ 2¹²⁶.

### Zero

Zero.

### One

One (`Raw` = 2¹²⁶).

### RotationScalar

Creates a value from a raw value (`Raw`). Normally you use `TryFromDouble` or `TryFromFloat` instead.

### IsNegative

`true` when the value is negative.

### IsZero

`true` when the value is zero.

### TryNegate

Negates the value. The smallest value, −2, cannot be negated, so it returns `false`; `result` is then `Zero`.

### TryFromFloat

Converts from `float`, rounding to the resolution 2⁻¹²⁶ with round-half-to-even. Returns `false` for NaN, infinity, and values below −2 or at least 2.

### TryFromDouble

Converts from `double`, rounding to the resolution 2⁻¹²⁶ with round-half-to-even. Returns `false` for NaN, infinity, and values below −2 or at least 2.

### TryToFloat

Converts to `float`. Digits beyond the significand are rounded with round-half-to-even.

### CompareTo

### Equals(Omni.Logical.RotationScalar)

### Equals(object)

### GetHashCode

### ToString

Returns every decimal digit of the value, independent of the culture, with trailing zeros omitted.

### operator ==

### operator !=

### operator <

### operator >

### operator <=

### operator >=
