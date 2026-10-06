---
description: "A high-precision quaternion of four RotationScalar values, used for rotations in Logical coordinates."
---

A quaternion with four [RotationScalar](../rotation-scalar/) (Q1.126) components, X, Y, Z and W. It is used for rotations in Logical coordinates. Multiplication, normalization and vector rotation add up all the terms of each component and then round once. Nothing is converted to Unity's `Quaternion` or `Matrix4x4` along the way.

The constructor and `TryFromFloat4` do not normalize. Normalize with `TryNormalize` before using a value as a rotation. `TryRotate` assumes a normalized quaternion. It has the same name as Unity's `Quaternion`, so an alias such as `using L = Omni.Logical;` makes them easier to tell apart.

<!-- members -->

### X

The X component of the vector part.

### Y

The Y component of the vector part.

### Z

The Z component of the vector part.

### W

The scalar part.

### Identity

The quaternion with no rotation (X, Y and Z are 0, W is 1).

### Quaternion

Creates a quaternion from four components. It does not normalize.

### TryFromFloat4

Converts from four `float` components. You can pass the components of either Unity's `Quaternion` or `Unity.Mathematics.quaternion`. It does not normalize. Returns `false` if any component is NaN, infinite, or outside the range from −2 up to but not including 2; `result` is then `Identity`.

### TryToFloat4

Converts to four `float` components, rounding each with round-half-to-even.

### TryMultiply

Computes the Hamilton product `left × right`. The component order and formula are the same as Unity's `Quaternion * Quaternion`. Returns `false` if any component falls outside the range from −2 up to but not including 2.

### TryNormalize

Normalizes to length 1. It computes the reciprocal square root of the sum of squared components and multiplies each component by it. Returns `false` when the length is 0 (including when the sum of squares is below the resolution).

### TryNlerp

Linearly interpolates two rotations component by component and normalizes the result (nlerp). When the dot product is negative it negates `to` first, so the interpolation takes the shorter arc. Returns `false` when `t` is less than 0 or greater than 1; `result` is then `Identity`. When `t` is 0 it returns `from`, and when `t` is 1 it returns `to`, as is and without normalizing.

### TryConjugate

Computes the conjugate (X, Y and Z negated, W unchanged). For a unit quaternion this is the inverse rotation. Returns `false` only when a component is −2.

### TryRotate

Rotates a [Vector3](../vector3/) by this rotation. `rotation` must already be normalized; this method does not normalize it. Returns `false` if a component of the result goes out of range.

### ToString

Returns `(X, Y, Z, W)` with every decimal digit of each component.

### Equals(Omni.Logical.Quaternion)

### Equals(object)

### GetHashCode

### operator ==

### operator !=
