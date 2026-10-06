---
description: "Constants and functions for Scalar: absolute value, rounding to integers, square root, powers and more."
---

Constants and functions for [Scalar](../scalar/) (Q95.96). Everything is computed with fixed-width integers; `float` and `double` are never used along the way. Functions that can go out of range start with `Try` and return `false` on failure. Rounding is round-half-to-even.

Expressions in the Inspector of [ScalarField](../scalar-field/) (`pi`, `sqrt`, `floor` and so on) are evaluated with the constants and functions of this type.

<!-- members -->

### Pi

π in Q95.96.

### Tau

τ = 2π in Q95.96.

### E

Euler's number e in Q95.96.

### Phi

The golden ratio φ in Q95.96.

### Sqrt2

√2 in Q95.96.

### TryAbs

Computes the absolute value. The absolute value of `Scalar.MinValue` cannot be represented, so it returns `false`.

### Min

Returns the smaller value.

### Max

Returns the larger value.

### TryClamp

Limits `value` to the range from `min` to `max`. Returns `false` when `min` is greater than `max`; `result` is then `Zero`.

### TryFloor

Rounds toward negative infinity to an integer (floor). Always succeeds.

### TryCeiling

Rounds toward positive infinity to an integer (ceiling). Returns `false` when the result is out of range.

### TryRound

Rounds to the nearest integer. Values exactly halfway round to the even integer (for example, 2.5 becomes 2 and 3.5 becomes 4). Returns `false` when the result is out of range.

### TryDivideRounded

Computes `numerator ÷ denominator` without going through a reciprocal. The error is at most half the resolution. Returns `false` when `denominator` is 0 or the result is out of range. This is the same computation as `Scalar.TryDivide`.

### TrySqrt

Computes the square root. √0 succeeds as 0. Returns `false` for negative values. Unlike `Scalar.TrySqrt`, it does not fail for 0.

### TryPowInteger

Raises `value` to an integer power. Each multiplication rounds with round-half-to-even. For a negative exponent, the reciprocal is rounded first and then multiplied. Returns `false` for 0⁰, for 0 to a negative power, and when an intermediate value goes out of range. If the value is a power of two, the result is computed without rounding.

### TryLerp

The same as `Scalar.TryLerp`. Computes `a + (b − a) × t` and returns `false` when `t` is less than 0 or greater than 1.
