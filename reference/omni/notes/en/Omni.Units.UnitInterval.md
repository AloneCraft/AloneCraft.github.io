---
description: "A ratio from 0 to 1, stored in fixed point with 126 fractional bits."
---

A fixed-point type that holds a ratio from 0 to 1 inclusive. It has 126 fractional bits, so the resolution is 2⁻¹²⁶ (about 1.18×10⁻³⁸). `TryParse` does not accept values outside 0 to 1.

<!-- members -->

### FractionalBits

The number of fractional bits. The value is 126.

### Zero

Zero.

### One

One.

### TryParse

Creates a value from a decimal string (exponent notation allowed). The string format is the same as `Scalar.TryParse`, and the value is rounded to the resolution with round-half-to-even. Returns `false` for an invalid string, a negative value, or a value greater than 1; `result` is then `Zero`.

### FromQ32

Creates a value from a 32-bit integer. In the current version the result is `value` ÷ 2⁹⁴. Note that it is not `value` ÷ 2³² (a Q32 ratio).

### TryToDouble

Converts to a `double`, rounded with round-half-to-even.

### CompareTo

### Equals(UnitInterval)

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
