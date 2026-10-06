---
description: "A position in Logical coordinates, kept distinct from difference vectors (Vector3)."
---

A position in Logical coordinates. It wraps a [Vector3](../vector3/) whose components are [Scalar](../scalar/) (Q95.96) values. It is a separate type so that positions are not confused with difference or direction vectors. Use `TryDifference` to get the difference of two positions as a `Vector3`, and `TryAdd` to offset a position.

`SetWorld` and `TryGetWorld` of [Logical Transform](../../components/logical-transform/) exchange world positions as this type.

<!-- members -->

### Value

The wrapped vector.

### Zero

The origin of Logical coordinates.

### Position3

Creates a position from a vector.

### TryToFloat3

Converts to `Unity.Mathematics.float3`. Each component is rounded to `float` with round-half-to-even, so precision is lost for positions far from the origin.

### TryFromFloat3

Converts from `Unity.Mathematics.float3`. Returns `false` if any component is NaN, infinite or out of range.

### ToString

Returns `(X, Y, Z)` with every decimal digit of each component.

### TryAdd

Returns the position moved by `offset`, with no rounding. Returns `false` if any component goes out of range.

### TryDifference

Returns the difference of two positions (`left − right`) as a `Vector3`, with no rounding. Returns `false` if any component goes out of range.

### Equals(Omni.Logical.Position3)

### Equals(object)

### GetHashCode

### operator ==

### operator !=
