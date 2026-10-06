---
description: "The component that is the source of truth for a pose, holding Logical position, rotation and signed XYZ scale in fixed point."
---
`Omni.Logical.Transform` (Add Component: `Omni/Logical Transform`) is the source of truth for position, rotation and scale. Position and scale are stored as `Scalar` (Q95.96, 192 bit), and rotation as a Q1.126 (128 bit) `Quaternion`. The Unity Transform is a finite display copy (a proxy).

- Parent-child composition happens only with an `Omni.Logical.Transform` on the direct parent. For the hierarchy rule, see [TransformHierarchy](../transform-hierarchy/).
- Changing the properties from code does not update the Unity Transform immediately. Call `SyncUnityTransform()` to bring it in line. Omni does this itself on Inspector edits, Undo, and enabling.
- In Edit Mode, if you move the Unity Transform of an object inside the safety range in the Scene View, Omni captures that value at `float` precision. Outside the safety range it does not capture it and writes the Logical value back.
- `Transform` in `Omni.Logical` has the same name as Unity's `Transform`. An alias such as `using L = Omni.Logical;` keeps them apart.

For how it works and examples, see [Logical coordinates and proxies](../../../docs/concepts/logical-coordinates/).

```csharp
using UnityEngine;
using L = Omni.Logical;

var logical = GetComponent<L.Transform>();
if (L.Scalar.TryParse("149597870700", out L.Scalar au) &&
    logical.SetWorld(new L.Position3(new L.Vector3(au, L.Scalar.Zero, L.Scalar.Zero)),
                     L.Quaternion.Identity, L.Vector3.One))
{
    logical.SyncUnityTransform(); // bring the Unity Transform (proxy) in line with the current values
}
```

<!-- members -->

### m_PositionX

Stores the X of the local position as raw fixed-point data (64 bit × 3). In the Inspector you edit it through the Position field, not this array.

### m_PositionY

Stored data for the Y of the local position. Same format as `m_PositionX`.

### m_PositionZ

Stored data for the Z of the local position. Same format as `m_PositionX`.

### m_RotationX

Stores the X of the local rotation (Quaternion) as raw fixed-point data (64 bit × 2). In the Inspector you edit it through the Rotation field (Euler angles).

### m_RotationY

Stored data for the Y of the local rotation. Same format as `m_RotationX`.

### m_RotationZ

Stored data for the Z of the local rotation. Same format as `m_RotationX`.

### m_RotationW

Stored data for the W of the local rotation. Same format as `m_RotationX`.

### m_ScaleX

Stores the X of the local scale as raw fixed-point data (64 bit × 3). It is signed. In the Inspector you edit it through the Scale field.

### m_ScaleY

Stored data for the Y of the local scale. Same format as `m_ScaleX`.

### m_ScaleZ

Stored data for the Z of the local scale. Same format as `m_ScaleX`.

### HasDetectedConflictingWriter

`true` if Omni has detected that something else rewrote this Unity Transform after Omni wrote it. This property exists only in the Editor and in Development Builds. Wrap code that uses it in `#if UNITY_EDITOR || DEVELOPMENT_BUILD`.

### LocalPosition

The local position, read and written in fixed point.

### LocalRotation

The local rotation. A value you set is stored normalized. Setting a value that cannot be normalized results in the identity rotation.

### LocalScale

The local signed XYZ scale, read and written in fixed point.

### TryGetWorld

Returns the world position and rotation composed with the parents, plus a scale. `worldScale` is the largest absolute value of the XYZ world scale. Use `TryGetWorldTrs` when you need per-axis values. Returns `false` when it cannot be computed.

### TryGetWorldTrs

Returns the world position, rotation and signed XYZ scale composed with the parents. Returns `false` when an operation goes out of range, the scale cannot be decomposed from the basis, the parent chain is deeper than 256, and so on.

### TryGetWorldAffine

Returns the world position composed with the parents and a 3×3 basis that includes rotation, non-uniform scale and shear. The scale of `TryGetWorldTrs` is decomposed from the basis and cannot express shear. Use this method to handle poses with shear exactly.

### SetWorld

Sets the world position, rotation and scale, converting them to local values relative to the parent. Returns `false` without changing anything if the rotation cannot be normalized, the parent cannot be inverted (for example a zero parent scale), or an operation goes out of range. It does not change the Unity Transform; call `SyncUnityTransform()` if needed.

### TranslateWorld

Converts the world delta `worldDelta` into a local delta through the parent's basis and adds it to the position. This moves an object without turning a huge absolute position into `float`. Returns `false` without changing anything if the parent's basis cannot be inverted or the result goes out of range.

### CaptureFromTransform

Captures the local position, rotation and scale (`float`) of `source` as this Logical Transform's local values. The captured values have `float` precision. Returns `false` without changing anything if a value is not finite or the rotation cannot be normalized. Throws `ArgumentNullException` if `source` is `null`.

### SyncUnityTransform

Writes the local Logical values to the Unity Transform (proxy). Even outside the safety range it writes the value while it is representable as `float` (it is not treated as the source of truth). When the value cannot be represented as `float`, for example a world value beyond 10¹⁸ m, it writes a placeholder with position 0 and scale 1. When a Camera with a `RadialCamera` is outside the safety range, it puts the position at the origin and matches only the rotation. It does not write when the values are already equal.

### IsPoseNeutralizedForFiniteProxy

`true` if, at the last `SyncUnityTransform()`, the object was outside the safety range and the Unity Transform therefore holds a value that is not the source of truth (a reduced-precision value, the camera origin proxy, or a placeholder). Use this property, not the Unity Transform values, to tell whether the object is outside the safety range.

### IsUnityTransformPlaceholder

`true` if the last `SyncUnityTransform()` wrote a placeholder (position 0, scale 1) to the Unity Transform. If `IsPoseNeutralizedForFiniteProxy` is `true` and this is `false`, the Unity Transform holds a reduced-precision value.

### IsInForbiddenHierarchyPlacement

Returns `true` if this object is in the forbidden placement (Logical Transform → plain Transform → Logical Transform). Same test as `TransformHierarchy.IsValid`.

### TryBakeToUnityTransformLossy

Writes the current local values to the Unity Transform while keeping the Logical Transform. If the position or scale cannot be represented as `float`, it returns `false` without changing anything, and `result.Status` is `FloatRangeExceeded`.

### TryDetach

Writes the current local values to the Unity Transform, then removes this Logical Transform. Direct child Logical Transforms move up one level and keep their world pose. Undoable in Edit Mode. On failure it leaves the Scene as it was, returns `false`, and reports the reason in `result.Status` ([DetachStatus](../detach-status/)).

### OnBeforeSerialize

Called by Unity's serialization. Does nothing. You do not need to call it.

### OnAfterDeserialize

Called by Unity's deserialization. Records that the instance was loaded. You do not need to call it. Only an instance newly created with Add Component captures the Unity Transform values when it is enabled.
