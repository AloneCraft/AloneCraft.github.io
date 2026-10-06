---
description: "A vector of three Scalar values, used for differences, directions and scale."
---

A vector with three [Scalar](../scalar/) (Q95.96) components, X, Y and Z. It is used for differences, directions and scale. For positions, use [Position3](../position3/), which wraps this type. It has the same name as Unity's `Vector3`, so an alias such as `using L = Omni.Logical;` makes them easier to tell apart.

Component-wise operations follow the same rules as `Scalar`. When a result is out of range, the method returns `false` and the `out` value is `Zero`. Dot and cross products add up several products and then round once.

<!-- members -->

### X

The X component.

### Y

The Y component.

### Z

The Z component.

### Zero

The vector whose components are all 0.

### One

The vector whose components are all 1.

### Vector3

Creates a vector from three components.

### TryToFloat3

Converts to `Unity.Mathematics.float3`. Each component is rounded to `float` with round-half-to-even, so precision is lost. Use it at boundaries that need `float`, such as rendering.

### TryFromFloat3

Converts from `Unity.Mathematics.float3`. Each component is converted with the same rules as `Scalar.TryFromFloat`. Returns `false` if any component is NaN, infinite or out of range.

### TryAdd

Adds component by component, with no rounding. Returns `false` if any component goes out of range.

### TrySubtract

Subtracts component by component (`left − right`), with no rounding. Returns `false` if any component goes out of range.

### TryNegate

Negates each component. Returns `false` if any component is `Scalar.MinValue`.

### TryScale

Multiplies each component by `scalar`, rounding each component with round-half-to-even.

### TryLerp

Interpolates each component with `Scalar.TryLerp`. Because the difference is taken first, the precision of the difference is kept even between huge coordinates. Returns `false` when `t` is less than 0 or greater than 1.

### TryDot

Computes the dot product. The three products are added and then rounded once.

### TryCross

Computes the cross product. Each component is the difference of two products, rounded once.

### TrySqrLength

Computes the squared length; the same as `TryDot(value, value)`. The square goes out of range easily: it fails when the length is about 2×10¹⁴ (2⁴⁷·⁵) or more. Use `TryLength` for long vectors.

### TryLength

Computes the length. It divides by the largest component first, so it works even for huge vectors whose square is out of range, as long as the length itself is representable. A zero vector succeeds with 0.

### TryNormalize

Divides the vector so that its length is 1. Returns `false` for a zero vector.

### ToString

Returns `(X, Y, Z)` with every decimal digit of each component.

### Equals(Omni.Logical.Vector3)

### Equals(object)

### GetHashCode

### operator ==

### operator !=
