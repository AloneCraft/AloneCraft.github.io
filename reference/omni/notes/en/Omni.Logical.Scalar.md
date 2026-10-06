---
description: "A Q95.96 (192-bit) fixed-point number used for positions, distances and scale."
---

A 192-bit fixed-point number used for positions, distances and scale. The format is fixed at Q95.96 (95 integer bits, 96 fractional bits, signed two's complement). The range is from −2⁹⁵ up to but not including 2⁹⁵ (about ±3.96×10²⁸), and the resolution is 2⁻⁹⁶ (about 1.26×10⁻²⁹). The resolution is the same regardless of the size of the value.

- Every operation that can overflow is a method whose name starts with `Try`. When the result is out of range, it returns `false` and the `out` value is `Zero`. Results are never wrapped or clamped.
- Addition, subtraction and negation are exact, with no rounding. Multiplication, division, square roots and conversions from strings, `float` and `double` round to the resolution 2⁻⁹⁶ with round-half-to-even. A result of half the resolution or less becomes 0; this is not a failure.
- The only operators are comparisons (`==`, `<` and so on). Arithmetic uses static methods such as `TryAdd`.

```csharp
using UnityEngine;
using L = Omni.Logical;

public static class ScalarExample
{
    public static void Run()
    {
        if (!L.Scalar.TryParse("149597870700", out L.Scalar au) || // 1 astronomical unit (m)
            !L.Scalar.TryParse("1e-9", out L.Scalar nm))             // 1 nm
        {
            return;
        }

        // Addition and subtraction do not round, so 1 nm added to 1 au is not lost
        if (L.Scalar.TryAdd(au, nm, out L.Scalar sum) &&
            L.Scalar.TrySubtract(sum, au, out L.Scalar back))
        {
            Debug.Log(back == nm); // True
        }

        // An operation that goes out of range returns false
        Debug.Log(L.Scalar.TryAdd(L.Scalar.MaxValue, L.Scalar.One, out _)); // False
    }
}
```

Use [Vector3](../vector3/) for three-component vectors, [Position3](../position3/) for positions, and [ScalarField](../scalar-field/) to save a value through Unity's Inspector. Absolute values, rounding to integers and similar functions are in [Math](../math/). For how it all fits together, see [Logical coordinates and proxies](../../../docs/concepts/logical-coordinates/).

<!-- members -->

### FractionalBits

The number of fractional bits. The value is 96.

### Raw

The raw value: a 192-bit two's complement integer. The number is `Raw` ÷ 2⁹⁶.

### Zero

Zero.

### One

One (`Raw` = 2⁹⁶).

### IsDivisionSupported

Whether division, reciprocal, square root and reciprocal square root are available. With the current fixed format (Q95.96) this is always `true`.

### MaxValue

The largest representable value (2⁹⁵ − 2⁻⁹⁶).

### MinValue

The smallest representable value (−2⁹⁵). Its negation cannot be represented.

### Scalar

Creates a value from a raw value (`Raw`). Normally you use `TryParse` or `TryFromDouble` instead.

### IsNegative

`true` when the value is negative.

### IsZero

`true` when the value is zero.

### TryAdd

Computes `left + right` with no rounding. Returns `false` when the result is out of range; `result` is then `Zero`.

### TrySubtract

Computes `left - right` with no rounding. Returns `false` when the result is out of range; `result` is then `Zero`.

### TryNegate

Negates the value. `MinValue` cannot be negated, so it returns `false`.

### TryMultiply

Computes `left × right`. It forms the exact product and then rounds once to the resolution with round-half-to-even. Returns `false` when the result is out of range.

### TryReciprocal

Computes `1 ÷ value` with round-half-to-even. Returns `false` when `value` is 0 or the result is out of range.

### TryDivide

Computes `left ÷ right`. The quotient is computed directly, not through a reciprocal, and rounded once with round-half-to-even. Returns `false` when `right` is 0 or the result is out of range.

### TryReciprocalSqrt

Computes `1 ÷ √value` by Newton iteration. Returns `false` when `value` is 0 or negative.

### TrySqrt

Computes `√value` with round-half-to-even. Returns `false` when `value` is 0 or negative. Use [Math.TrySqrt](../math/) if you want √0 = 0 to succeed.

### TryScaleByPowerOfTwo

Computes `value × 2^exponent`; a negative `exponent` divides by a power of two. It only shifts bits, so multiplying is exact. When dividing, digits finer than the resolution are rounded with round-half-to-even. Returns `false` when the result is out of range.

### TryLerp

Computes `from + (to − from) × t`. Because the difference is taken first, the precision of the difference is kept even when interpolating between huge coordinates. Returns `false` when `t` is less than 0 or greater than 1. When `t` is 0 it returns `from`, and when `t` is 1 it returns `to`, unchanged.

### TryLerpLog2

Interpolates in log space. When both ends are positive it computes `from^(1−t) × to^t` (geometric interpolation). When either end is 0 or negative it falls back to plain linear interpolation, `from × (1−t) + to × t`. Returns `false` when `t` is less than 0 or greater than 1.

### TryFromFloat

Converts from `float`, rounding to the resolution with round-half-to-even. A `float` whose absolute value is 2⁻⁷³ or more converts exactly. Returns `false` for NaN, infinity and out-of-range values.

### TryFromDouble

Converts from `double`, rounding to the resolution with round-half-to-even. Returns `false` for NaN, infinity and out-of-range values.

### TryToFloat

Converts to `float`. Digits beyond the significand (24 binary digits) are rounded with round-half-to-even. The range of `Scalar` fits within the finite range of `float`.

### TryToDouble

Converts to `double`. Digits beyond the significand (53 binary digits) are rounded with round-half-to-even. The range of `Scalar` fits within the finite range of `double`.

### CompareTo

### Equals(Omni.Logical.Scalar)

### Equals(object)

### GetHashCode

### ToString

Returns every decimal digit of the value. The output does not depend on the culture. Trailing zeros are omitted, and integers have no decimal point (for example `1`, `0.5`). A binary fraction always has a finite decimal expansion, so passing this string to `TryParse` gives back the same value.

### TryParse

Converts from a decimal string, independent of the culture. It accepts a sign (`+`, `-`), a decimal point and an exponent (for example `1.496e11`), and ignores leading and trailing white space. The value is rounded to the resolution with round-half-to-even; a value of half the resolution or less becomes 0. Expressions, digit-group separators, `Infinity` and the like are not accepted. Returns `false` for an invalid string, a string longer than 4,096 characters, or an out-of-range value; `result` is then `Zero`.

### operator ==

### operator !=

### operator <

### operator >

### operator <=

### operator >=
