---
description: "Constants that describe the fixed-point formats of Logical coordinates (Q95.96 and Q1.126)."
---

Constants that describe the number formats of Logical coordinates. [Scalar](../scalar/), used for positions, distances and scale, is Q95.96 (192 bits). [RotationScalar](../rotation-scalar/), used for rotations, is Q1.126 (128 bits). The formats are fixed and do not change at runtime or through settings.

<!-- members -->

### TotalBits

The total number of bits in a `Scalar`. The value is 192.

### FractionalBits

The number of fractional bits in a `Scalar`. The value is 96.

### IntegerBits

The number of integer bits in a `Scalar`, excluding the sign. The value is 95, so the range is about ±2⁹⁵.

### RotationFractionalBits

The number of fractional bits in a `RotationScalar`. The value is 126.

### LimbCount

The number of 64-bit parts in the raw value of a `Scalar`. The value is 3.

### WideBits

The number of bits of the integers used inside multiplication. The value is 384 (twice `TotalBits`).

### WideLimbCount

The number of 64-bit parts of the integers used inside multiplication. The value is 6.

### CurrentResolution

The smallest positive `Scalar` (2⁻⁹⁶, about 1.26×10⁻²⁹).

### TryGetPowerOfTwoExponent

If `value` is exactly a power of two, returns its exponent (`value` = 2^`binaryExponent`). Returns `false` when `value` is 0 or less, or not a power of two; `binaryExponent` is then 0.
