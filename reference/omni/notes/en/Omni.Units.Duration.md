---
description: "A time in seconds, stored in fixed point with 64 fractional bits."
---

A signed fixed-point type that holds a time in seconds. It has 64 fractional bits, so the resolution is 2⁻⁶⁴ seconds (about 5.4×10⁻²⁰ seconds). The raw value is 192 bits, the same as [Scalar](../../logical/scalar/), so the range is about ±2¹²⁷ seconds.

Addition, subtraction and integer multiplication are exact, with no rounding. Error does not build up the way it does when you keep adding `double` values. An operation that goes out of range returns `false`, and the `out` value is `Zero`.

<!-- members -->

### FractionalBits

The number of fractional bits. The value is 64.

### Zero

Zero seconds.

### OneSecond

One second.

### IsNegative

`true` when the value is negative.

### IsZero

`true` when the value is zero.

### TryFromSeconds(string, out Duration)

Creates a value from a decimal string in seconds (exponent notation allowed). The string format is the same as `Scalar.TryParse`, and the value is rounded to the resolution with round-half-to-even. Returns `false` for an invalid string or an out-of-range value.

### TryFromSeconds(double, out Duration)

Creates a value from a `double` in seconds, rounded to the resolution with round-half-to-even. Returns `false` for NaN, infinity and out-of-range values.

### TryToSeconds

Converts to a `double` in seconds, rounded with round-half-to-even.

### TryToSecondsFloat

Converts to a `float` in seconds, rounded with round-half-to-even.

### TryAdd

Computes `left + right` with no rounding. Returns `false` when the result is out of range.

### TrySubtract

Computes `left − right` with no rounding. Returns `false` when the result is out of range.

### TryScale

Multiplies the time by a [TimeScale](../time-scale/) factor, rounded to the resolution with round-half-to-even. Returns `false` when the result is out of range.

### TryDivideToFloor

Returns the quotient `numerator ÷ denominator`, truncated, as a `long` (for example, how many whole periods have elapsed). Returns `false` when either value is negative, `denominator` is 0, or the quotient does not fit in a `long` (it must be less than 2⁶³); `quotient` is then 0.

### TryMultiply

Multiplies the time by an integer, with no rounding. Returns `false` when the result is out of range.

### CompareTo

### Equals(Duration)

### Equals(object)

### GetHashCode

### ToString

Returns every decimal digit of the value in seconds, independent of the culture, with trailing zeros omitted.

### operator ==

### operator !=

### operator <

### operator >

### operator <=

### operator >=
