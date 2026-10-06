---
title: Near and far
description: The difference between the logical near, the hardware near and the culling near, and how to read the RadialCamera inspector diagnostics.
sidebar:
  order: 3
---

Radial rendering has several values called "near". Their roles differ, so they are not treated as the same value.

| Value | Role |
| --- | --- |
| Logical near | The condition that decides, in Logical camera space, what is too close. The RadialCamera Near |
| Hardware near | The near plane passed to Unity's projection matrix. Omni decides it only while rendering |
| Culling near | The condition used when removing render candidates on the CPU |
| View Scale *M* | The factor that links logical distance and render-space distance |

## The logical near is a plane

The logical near is not a sphere around the camera. It is a plane perpendicular to the camera's forward direction.

```text
logicalViewDepth = dot(P - C, F)
visible when       logicalViewDepth >= nearClip
```

*P* is the position of the object, *C* the position of the camera, and *F* the camera's forward unit vector. The computation uses Logical precision.

## With perspective, the center keeps a little more

The mapping is not the identity even close to the near plane. With perspective projection, the logical near plane therefore cannot be represented by one hardware near plane.

Omni chooses a hardware near that does not over-clip at the four corners of the screen. As a result, at the center of the screen, objects slightly closer than the logical near remain. This is called **Center Under-clip**. The amount is shown in the RadialCamera inspector.

:::note[Always on the safe side]
The hardware near is always chosen on the safe side. Nothing beyond the logical near is clipped.
:::

Orthographic cameras do not apply the logarithmic mapping. The logical near is used directly as the projection's near, so it matches at the center and at the corners.

## Far

The RadialCamera Far is the upper limit of the logical distance to draw. Objects beyond it are not drawn.

- It is used for the CPU test and for the bounds that enclose the shape after mapping. Objects beyond Far are hidden by the Radial test.
- It is also used for the GPU hardware far. Only while each camera renders, Omni adjusts the Camera's near, far and projection matrix so that the mapped logical Far fits, and restores them afterwards.

The logarithmic mapping folds infinity into a finite render distance. Without an upper limit, how far you can see would be decided by an internal value that nobody specified. Far states that limit explicitly. The default is the `Scalar` limit (about 3.96×10²⁸ m).

## Limit on the far/near ratio

A 32-bit `float` perspective projection breaks down when the far/near ratio is extremely large. The normal of the far plane becomes zero, and the computation produces NaN or infinity.

Omni keeps the ratio of the hardware far to the hardware near at 2²⁰ (1,048,576) or less. When it would be exceeded, Omni keeps the near and shrinks only the far.

The Radial mapping compresses distant space logarithmically, so this limit is normally not reached. For example, with *M* = 1, even the distance at the `Scalar` limit is about 95 in render space.

## Culling near

CPU culling uses the logical near as its near condition. When the logical near exceeds 10⁹ m, culling uses 10⁹ m. The culling near is never farther than the logical near.

## RadialCamera inspector

### Clipping Planes

| Item | Description |
| --- | --- |
| Near | The logical distance (m) represented by the Camera's near plane. Smaller values magnify nearby objects more |
| Far | The upper limit of the logical distance to draw (m) |

Both are `ScalarField` fields and accept expressions (see "Entering values in the Inspector" in [Logical coordinates and proxies](../logical-coordinates/)). The unit is logical meters, separate from the Camera's Clipping Planes (render space).

Warnings appear in these cases:

- "Near is 0 or less." / "Far is 0 or less.": the mapping is not valid unless the value is positive. Rendering uses the default value.
- "Far must be greater than Near.": the range of logical distance to draw is empty.

### Diagnostics

| Item | Description |
| --- | --- |
| Fractional Bits, Minimum Fixed-Point Unit | The `Scalar` format (96 bits, 2⁻⁹⁶ m) |
| View Scale (M), log2(View Scale) | The current *M* |
| View Scale Source | M = Camera Near ÷ Logical Near |
| Camera Near Clip, Logical Near, Camera Far Clip | The values you set |
| Logical Distance of Camera Near Clip, Logical Distance of Camera Far Clip | The Camera's near and far mapped back to logical distance |
| Render Far Required for Far | The render-space far needed to draw the logical Far |
| Logical Distance Where Compression Starts (S / M) | The logical distance where the curve starts to bend |
| Active Camera | The target Camera |
| Logical Near Clip Plane | The logical near value |
| Safe Render Near (internal) | The value used as the hardware near |
| Clip Regime | "Hardware plane matches Logical" (orthographic) or "Conservative" (perspective) |
| Frustum Corner cos | The cosine of the angle between the forward direction and the directions to the frustum corners |
| Center Under-clip | How much remains in front of the logical near at the center of the screen (%) |

For a perspective camera, this explanation is shown: "The mapping compresses even the closest range, so the logical Near plane cannot be represented by one matching hardware near plane. Rendering never over-clips, but the screen center keeps a little geometry closer than Near (see Center Under-clip)."

:::tip[If objects do not clip at the distance you set]
The Camera Near (render space) and the RadialCamera Near (logical space) are different values linked by *M*. First check "Logical Near", "Safe Render Near (internal)" and "Center Under-clip" in Diagnostics.
:::
