---
description: "A gravitational parameter μ (m³/s²), stored as a decimal coefficient of up to 38 digits and a decimal exponent."
---

A type that holds the gravitational parameter μ = GM in m³/s². Like [Mass](../mass/), it is stored as a decimal coefficient of up to 38 digits and a decimal exponent. Only positive values can be created.

For orbit calculations, convert it to a [Scalar](../../logical/scalar/) with `TryToLogicalScalar` and pass it to [AngularVelocity.TryFromKepler](../angular-velocity/) and similar methods.

<!-- members -->

### IsZero

`true` when the value is 0. `TryFromMetersCubedPerSecondSquared` never creates 0, so only the `default` value is 0.

### TryFromMetersCubedPerSecondSquared

Creates a value from a decimal string in m³/s² (for example `3.986004418e14`). The string format is the same as `Mass.TryFromKilograms`. Returns `false` for an invalid string, 0, or a negative value.

### TryToLogicalScalar

Converts to a [Scalar](../../logical/scalar/) (Q95.96), rounded to the resolution 2⁻⁹⁶ with round-half-to-even. Returns `false` for values of 2⁹⁵ (about 3.96×10²⁸) or more.

### TryToDouble

Converts to a `double`.

### CompareTo

### Equals(GravitationalParameter)

### Equals(object)

### GetHashCode

### ToString

Returns the value in m³/s² as a coefficient and an exponent. The coefficient is an integer with trailing zeros removed (for example, `3.986004418e14` becomes `3986004418e5`).

### operator ==

### operator !=

### operator <

### operator >
