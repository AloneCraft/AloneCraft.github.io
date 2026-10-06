---
description: "A mass in kg, stored as a decimal coefficient of up to 38 digits and a decimal exponent."
---

A non-negative mass in kg. It uses a different format from the fixed point of coordinates: a decimal coefficient of up to 38 digits and a decimal exponent. This lets one type cover everything from particle masses to galaxy masses. Digits from the 39th onward are rounded with round-half-to-even.

The mass of a [CelestialBodyAuthoring](../../celestial/celestial-body-authoring/) (a string in kg) is converted to this type with `TryFromKilograms` when the catalog snapshot is built.

<!-- members -->

### Zero

A mass of 0.

### IsZero

`true` when the mass is 0.

### TryFromKilograms

Creates a value from a decimal string in kg (for example `5.972e24`). It accepts leading and trailing white space, a sign, a decimal point and an exponent (`e` or `E`). Returns `false` for an invalid string, a negative value, or an exponent outside the `int` range; `mass` is then `Zero`.

### TryFromGrams

Creates a value from a decimal string in g and converts it to kg (multiplying by 10⁻³ only shifts the exponent, so it is exact). It fails under the same conditions as `TryFromKilograms`.

### CompareTo

### Equals(Mass)

### Equals(object)

### GetHashCode

### ToString

Returns the value in kg as a coefficient and an exponent. The coefficient is an integer with trailing zeros removed (for example, `5.972e24` becomes `5972e21`, and 0 becomes `0`).

### operator ==

### operator !=

### operator <

### operator >

### operator <=

### operator >=
