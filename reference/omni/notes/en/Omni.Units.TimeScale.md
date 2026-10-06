---
description: "A time scale factor, stored in fixed point with 96 fractional bits."
---

A signed fixed-point type that holds a time scale factor (dimensionless). It has 96 fractional bits, so the resolution is 2⁻⁹⁶. The raw value is 192 bits, the same as [Scalar](../../logical/scalar/), so the range is about ±2⁹⁵.

Pass it to [Duration.TryScale](../duration/) or [AngularVelocity.TryScale](../angular-velocity/) to scale a time or an angular velocity.

<!-- members -->

### FractionalBits

The number of fractional bits. The value is 96.

### Zero

A factor of 0.

### One

A factor of 1.

### IsNegative

`true` when the value is negative.

### IsZero

`true` when the value is zero.

### TryParse

Creates a value from a decimal string (exponent notation allowed). The string format is the same as `Scalar.TryParse`, and the value is rounded to the resolution with round-half-to-even. Returns `false` for an invalid string or an out-of-range value; `result` is then `Zero`.

### TryFromDouble

Creates a value from a `double`, rounded to the resolution with round-half-to-even. Returns `false` for NaN, infinity and out-of-range values.

### TryToDouble

Converts to a `double`, rounded with round-half-to-even.

### CompareTo

### Equals(TimeScale)

### Equals(object)

### GetHashCode

### ToString

Returns every decimal digit, independent of the culture, with trailing zeros omitted.

### operator ==

### operator !=

### operator <

### operator >

### operator <=

### operator >=
