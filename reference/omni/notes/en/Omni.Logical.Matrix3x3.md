---
description: "A 3×3 matrix of nine Scalar values, used for bases that combine rotation, scale and shear."
---

A 3×3 matrix of nine [Scalar](../scalar/) (Q95.96) values. Components are in row-major order: `M01` is row 1, column 2. It represents a basis that combines rotation, signed scale and shear. `TryGetWorldAffine` of [Logical Transform](../../components/logical-transform/) returns the world basis as this type.

Matrix-vector and matrix-matrix products add up the three products of each component and then round once. They return `false` when a result is out of range.

<!-- members -->

### M00

The component at row 1, column 1.

### M01

The component at row 1, column 2.

### M02

The component at row 1, column 3.

### M10

The component at row 2, column 1.

### M11

The component at row 2, column 2.

### M12

The component at row 2, column 3.

### M20

The component at row 3, column 1.

### M21

The component at row 3, column 2.

### M22

The component at row 3, column 3.

### Identity

The identity matrix.

### Matrix3x3

Creates a matrix from nine components in row-major order (`m00`, `m01`, `m02`, `m10`, …).

### TryMultiplyVector

Computes the matrix-vector product (`matrix × vector`, with the vector as a column vector). Returns `false` if any component goes out of range; `result` is then `Vector3.Zero`.

### TryMultiply

Computes the matrix product (`left × right`). Returns `false` if any component goes out of range; `result` is then `Identity`.

### TryInverse

Computes the inverse as the adjugate (transposed cofactor matrix) divided by the determinant. Matrices with negative scale or shear are handled too. Returns `false` when the determinant is 0 (no inverse exists) or an intermediate value goes out of range; `result` is then `Identity`.

### Equals(Omni.Logical.Matrix3x3)

### Equals(object)

### GetHashCode

### operator ==

### operator !=
