---
title: Logical coordinates and proxies
description: How Omni keeps the true position in Logical coordinates and treats the Unity Transform as a finite copy.
sidebar:
  order: 1
---

## Why float is not enough

A Unity Transform stores positions as 32-bit `float`. A `float` has 24 binary digits of precision, so the representable step grows in proportion to the size of the value.

| Distance from the origin | Step a `float` can represent (approx.) |
| --- | --- |
| 1 m | 1.2×10⁻⁷ m (0.12 µm) |
| 10 km | 1 mm |
| 1,000 km | 6 cm |
| 1 astronomical unit (about 1.5×10¹¹ m) | 16 km |
| 1 light-year (about 9.5×10¹⁵ m) | 1.1×10⁹ m |

At astronomical scales you cannot place objects more finely than this step. Objects jitter, overlap, or cannot be placed at all.

Omni stores positions in fixed-point **Logical coordinates**. Fixed point has the same resolution at any distance from the origin.

## Source of truth and proxy

- `Omni.Logical.Transform` (Add Component: `Omni/Logical Transform`) is the source of truth for position, rotation and scale.
- Parent-child composition and distance tests are done in the Logical types.
- For rendering, Omni computes the difference from the camera in Logical values, makes it camera-relative, and only then converts it to finite values. Huge absolute coordinates are never narrowed to `float` first.
- The Unity Transform is a finite display copy (a proxy). The Logical Transform writes to the Unity Transform in one direction (`SyncUnityTransform()`).
- On a GameObject with a Logical Transform, the Unity Transform inspector is read-only and shows "This Transform is managed by LogicalTransform."

### Safety range and proxy values

What goes into the Unity Transform depends on the size of the Logical value. The default safety range is 10,000 m.

| Logical value | Value in the Unity Transform |
| --- | --- |
| Inside the safety range | The Logical value converted to `float` |
| Outside the safety range, representable as `float` | The actual value at reduced precision. Not treated as the source of truth |
| World position beyond 10¹⁸ m, or not representable as `float` | A placeholder with position 0 and scale 1 |

When the object is outside the safety range, the Logical Transform inspector shows one of these:

- "Transform: outside the real-space extent (object), so this holds a reduced-precision real value."
- "Transform: outside the real-space extent (object) and unrepresentable, so this is a placeholder (position 0, scale 1)."

When a camera with a RadialCamera is outside the safety range, the Unity camera is placed at the origin and only its rotation follows the Logical value. Rendering is built camera-relative from the Logical camera position.

:::caution[Do not read the Unity Transform as the source of truth]
Outside the safety range, do not judge the position from the Unity Transform values. A placeholder has position 0, so by its value alone it looks as if it were inside the safety range. Check the state with `IsPoseNeutralizedForFiniteProxy` and `IsUnityTransformPlaceholder`.
:::

### Moving the Transform in Edit Mode

- Inside the safety range, moving the Unity Transform in the Scene View captures that value into the Logical value. The captured value has `float` precision.
- Outside the safety range, it is not captured. Omni writes the Logical value back and logs a warning to the Console. Edit position, rotation and scale with the Logical Transform in the Inspector.

## Number formats

| Value | Type | Format | Range and resolution |
| --- | --- | --- | --- |
| Position, distance, scale | `Scalar` | Q95.96 (192-bit fixed point) | ±2⁹⁵ m (about ±3.96×10²⁸ m), resolution 2⁻⁹⁶ m (about 1.26×10⁻²⁹ m) |
| Rotation (each quaternion component) | `RotationScalar` | Q1.126 (128-bit fixed point) | The position error caused by rotation rounding is about 2×10⁻³⁸ times the distance |

The formats are fixed. They do not change at runtime or through settings. Scale is a signed XYZ value stored as `Scalar`.

### Operations that can overflow are Try APIs

Every operation that can overflow is an API whose name starts with `Try`. When the result is out of range, it returns `false`. Results are never silently wrapped or clamped.

- Multiplication, division and parsing round to the resolution 2⁻⁹⁶ with round-half-to-even.
- Conversions to and from `float` and `double` (`TryFromFloat`, `TryToFloat` and so on) fail for non-finite and out-of-range values.
- `Transform.SetWorld` changes nothing and returns `false` when the parent cannot be inverted, for example because its scale is 0.

## Using it from code

`Transform`, `Vector3` and `Quaternion` in `Omni.Logical` have the same names as Unity's types. An alias makes them easier to tell apart.

```csharp
using UnityEngine;
using L = Omni.Logical;

public sealed class PlaceAtOneAu : MonoBehaviour
{
    private void Start()
    {
        var logical = GetComponent<L.Transform>();
        if (logical == null ||
            !L.Scalar.TryParse("149597870700", out L.Scalar au))
        {
            return;
        }

        var position = new L.Position3(new L.Vector3(au, L.Scalar.Zero, L.Scalar.Zero));
        if (!logical.SetWorld(position, L.Quaternion.Identity, L.Vector3.One))
        {
            Debug.LogError("SetWorld failed.");
            return;
        }

        // Bring the Unity Transform (proxy) up to date
        logical.SyncUnityTransform();

        if (logical.TryGetWorld(out L.Position3 world, out L.Quaternion _, out L.Scalar _))
        {
            Debug.Log(world); // printed with every decimal digit
        }
    }
}
```

| API | Description |
| --- | --- |
| `LocalPosition`, `LocalRotation`, `LocalScale` | Local values. Read and written in fixed point |
| `TryGetWorld`, `TryGetWorldTrs`, `TryGetWorldAffine` | World values with the parents composed |
| `SetWorld`, `TranslateWorld` | Set world values. Add a world-space delta |
| `SyncUnityTransform()` | Write to the Unity Transform |
| `Position3.TryDifference`, `Vector3.TryLength` | Difference and length between two points at Logical precision |
| `Scalar.TryParse`, `Scalar.ToString()` | Conversion to and from decimal strings. `ToString()` prints every digit |

:::tip[ScalarField.FromString]
`ScalarField.FromString` returns zero for a string it cannot convert. Use `Scalar.TryParse` when you need to detect the failure.
:::

## Hierarchy rules

If a Logical Transform has a Logical Transform ancestor, that ancestor must be its **direct parent**.

| Hierarchy | Allowed |
| --- | --- |
| Logical Transform | Yes |
| Logical Transform → plain Transform | Yes |
| Logical Transform → plain Transform → plain Transform | Yes |
| Logical Transform → Logical Transform (direct parent and child) | Yes |
| Logical Transform → plain Transform → Logical Transform | **Forbidden** |

In the forbidden placement, the local transform of the plain Transform in between drops out of the Logical composition, while the Unity hierarchy still applies it. The Logical value and the rendered position drift apart.

When Omni finds a forbidden placement, it does the following:

- It logs an error to the Console. The Inspector shows "Invalid hierarchy (LogicalTransform → ordinary Transform → LogicalTransform resumes)."
- That Logical Transform stops composing with its parent and is placed by its own values alone. A broken composition result is never produced.

To fix it, make the GameObject a direct child of the Logical Transform, or add a Logical Transform to the Transforms in between too. You can test this with `IsInForbiddenHierarchyPlacement()` and `TransformHierarchy.IsValid`.

:::note[Adding to a child]
When you add a Logical Transform to a GameObject whose ancestors lack one, Omni asks "The parent object has no LogicalTransform. Attach LogicalTransform up to the root?". "OK (Add)" also adds it to the ancestors up to the root, as a single Undo step. "Cancel" removes the Logical Transform you just added.
:::

## Do not let other writers compete

The Logical Transform is the only writer of its Transform. If any of the following is on the same GameObject, Omni logs an error to the Console when the component is enabled:

- `CinemachineBrain`, `CinemachineCamera`
- `CharacterController`
- A non-kinematic `Rigidbody`

In the Editor and in Development Builds, Omni also detects when the Transform is changed through another path after Omni wrote it. When it does, it logs an error to the Console and the Inspector shows "Transform: diverges from the logical value (another writer was detected)."

## Removing a Logical Transform

| Method | Description |
| --- | --- |
| `TryDetach(out DetachResult)` | Bakes the current local values into the Unity Transform, then removes the Logical Transform. Direct Logical children move up one level and keep their world pose. Undoable in the Editor |
| `TryBakeToUnityTransformLossy(out DetachResult)` | Writes the current local values to the Unity Transform without removing anything |
| Remove Component directly | Writes the last local values to the Unity Transform. Values that do not fit in `float` are limited to ±`float.MaxValue`, with an error |

When `TryDetach` fails, it changes nothing and reports the reason in `DetachResult.Status`.

| `DetachStatus` | Meaning |
| --- | --- |
| `Success` | Bake and removal completed |
| `AlreadyDetached` | Already removed |
| `FloatRangeExceeded` | Position or scale is outside the `float` range |
| `InvalidHierarchy` | In a forbidden placement |
| `SingularParent` | The parent cannot be inverted, for example because its scale is 0 |
| `ConversionFailed` | Converting the Logical values failed |

## Entering values in the Inspector

### Logical Transform

- Position and Scale are shown and edited as decimal strings with every digit. Decimal and scientific notation (for example `1.496e11`) are accepted.
- Input that cannot be converted is ignored and the previous value is kept.
- Dragging the X, Y and Z labels changes the value, as with the standard Transform.
- While the lock button next to Scale is ON, changing one axis applies the same multiplier to the other axes.
- Rotation is edited as `float` Euler angles.

### ScalarField fields (RadialCamera Near, Far and others)

`ScalarField` fields accept expressions as well as numbers. Only the evaluated value is saved; the expression is not.

| Kind | Available |
| --- | --- |
| Operators | `+`, `-`, `*`, `/`, `^` (integer exponent, absolute value up to one million), parentheses |
| Constants | `pi`, `tau`, `e`, `phi`, `sqrt2` |
| Functions | `abs`, `sqrt`, `floor`, `ceil`, `round`, `min`, `max`, `clamp`, `lerp` |

- Evaluation uses Logical precision. `float` is not used along the way.
- An expression that goes out of range, divides by zero, uses an unknown name and so on is not applied. An error and its position are shown below the field, and the previous value is kept.
- Pressing Esc while editing cancels the input.

## Compatibility of saved data

- Position and scale are saved as 3 × 64 bits (24 bytes) per value. Rotation is saved as 2 × 64 bits per component.
- Values saved in the earlier format (position and scale as 2 × 64 bits, Q95.32) are read as the same numbers.
- Data of any other length is read as zero, and a warning is logged to the Console once per Logical Transform.
