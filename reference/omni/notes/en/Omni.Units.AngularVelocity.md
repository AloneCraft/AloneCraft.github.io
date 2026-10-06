---
description: "An angular velocity in fixed point. Input and output are in rad/s; the stored value is in turns/s."
---

An angular velocity in fixed point. Input and output are in rad/s (radians per second). The stored value is in turns/s (turns per second) with 96 fractional bits. The raw value is 192 bits, the same as [Scalar](../../logical/scalar/), so the range is about ±2⁹⁵ turns/s. Because the stored value is in turns, multiplying by a [Duration](../duration/) to get an [Angle](../angle/) adds no rounding of 2π to the angle.

When created from rad/s, the value is divided by 2π (rounded to the resolution 2⁻⁹⁶) and rounded with round-half-to-even. An operation that goes out of range returns `false`, and the `out` value is `Zero`.

<!-- members -->

### FractionalBits

The number of fractional bits of the stored value (turns/s). The value is 96.

### Zero

Zero rad/s.

### IsNegative

`true` when the value is negative (rotation in the opposite direction).

### IsZero

`true` when the value is zero.

### TryFromRadiansPerSecond(string, out AngularVelocity)

Creates a value from a decimal string in rad/s (exponent notation allowed). The string format is the same as `Scalar.TryParse`. Returns `false` for an invalid string or an out-of-range value.

### TryFromRadiansPerSecond(double, out AngularVelocity)

Creates a value from a `double` in rad/s. Returns `false` for NaN, infinity and out-of-range values.

### TryFromKepler

Computes the angular velocity of a circular orbit, √(μ ÷ r³), from the gravitational parameter μ (m³/s²) and the orbit radius r (m). Everything is computed in fixed point. Returns `false` when μ or r is 0 or less, or an intermediate value goes out of range. You can turn μ into a `Scalar` with [GravitationalParameter.TryToLogicalScalar](../gravitational-parameter/).

### TryApplyOrbitClassQ16

Multiplies the angular velocity by 0.9 + 0.1 × (`orbitClassQ16` ÷ 65536). The factor is at least 0.9 and less than 1.0. Returns `false` when the result is out of range.

### TryLerpQ16

Computes `from + (to − from) × ratioQ16 ÷ 65536`. The ratio is at least 0 and less than 1. Returns `false` when the result is out of range.

### TryLerpQ32

Computes `from + (to − from) × ratioQ32 ÷ 2³²`. The ratio is at least 0 and less than 1. Returns `false` when the result is out of range.

### TryCalculateHalfCellWindow

Splits the circle into `angularCellCount` cells and returns, as a [Duration](../duration/), the time it takes to turn through half a cell (1 ÷ (2 × `angularCellCount`) turns) at `maximumVelocity`. Returns `false` when `angularCellCount` is less than 2, `maximumVelocity` is 0 or less, or the result rounds to 0.

### TryToRadiansPerSecond

Converts to a `double` in rad/s, rounded with round-half-to-even.

### TryScale

Multiplies the angular velocity by a [TimeScale](../time-scale/), with round-half-to-even. Returns `false` when the result is out of range.

### TryMultiply

Returns the angle turned through, angular velocity × time ([Duration](../duration/)), as an [Angle](../angle/). The angle wraps at one turn; negative results also wrap into the range from 0 to one turn. Returns `false` only when an intermediate value goes out of range.

### CompareTo

### Equals(AngularVelocity)

### Equals(object)

### GetHashCode

### ToString

Returns the value in rad/s as a decimal string. Returns `"0"` when the conversion to rad/s goes out of range.

### operator ==

### operator !=

### operator <

### operator >

### operator <=

### operator >=
