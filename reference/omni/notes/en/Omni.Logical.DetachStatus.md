---
description: "The categories of the result of detaching or baking a Logical Transform."
---
`DetachStatus` is the value of `Status` in [DetachResult](../detach-result/). It is returned by [Logical Transform](../logical-transform/)'s `TryDetach` and `TryBakeToUnityTransformLossy`.

For the failure values, `TryDetach` leaves the Scene as it was.

<!-- members -->

### Success

Succeeded. For `TryDetach`, the bake to the Unity Transform and the removal of the Logical Transform are complete.

### AlreadyDetached

The target has already been removed.

### FloatRangeExceeded

The position or scale cannot be represented as `float`.

### InvalidHierarchy

The target is in a forbidden placement (Logical Transform → plain Transform → Logical Transform).

### SingularParent

When moving a direct child Logical Transform up one level, its world pose could not be kept (the parent cannot be inverted, for example because its scale is 0).

### ConversionFailed

The world pose of a direct child Logical Transform could not be computed.

