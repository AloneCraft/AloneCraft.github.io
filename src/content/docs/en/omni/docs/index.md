---
title: Omni overview
description: What Omni solves and how it renders.
---

Omni is a Unity asset that keeps positions and orientations, from the scale of a proton to the size of the observable universe, in high-precision **Logical coordinates**, and hands them to Unity's Scene View, Game View, Player and GPU as finite-precision values.

:::note[About 0.1]
This documentation covers Omni 0.1.0. The product has not been released yet, so details may change.
:::

## The problem

A Unity Transform stores positions as 32-bit `float`. The farther you go from the origin, the coarser the representable steps become, so at astronomical scales objects jitter, overlap, or cannot be placed at all. The ratio between a camera's near and far planes is also limited, so one camera cannot draw near and far objects at the same time.

Omni solves these two problems separately.

| Problem | Omni's approach |
| --- | --- |
| Position precision | Keep the true position in 192-bit Logical coordinates. Convert to `float` only right before rendering |
| Distance range | Draw through a logarithmic mapping of the distance from the eye (Radial). One camera draws from near to far |

## Logical coordinates

- `Omni.Logical.Transform` (Add Component: `Omni/Logical Transform`) is the source of truth for position, rotation and scale.
- The position value `Scalar` is a 192-bit Q95.96 fixed-point number. Its range is ±3.96×10²⁸ m and its resolution is 2⁻⁹⁶ m (about 1.26×10⁻²⁹ m).
- Rotations are 128-bit Q1.126.
- The Unity Transform is a finite display copy (a proxy). It is not the source of truth.
- Every operation that can overflow is a `Try` API. Results are never silently rounded or clamped.

## Radial (logarithmic) rendering

The distance *d* from the eye is mapped by the following formula before drawing.

```text
F(d) = log2(1 + d · M)
```

- *M* is the View Scale: the camera's near plane divided by the logical near distance (`RadialCamera`). The curve starts to bend at a logical distance of about 1/*M*. *M* is the only parameter.
- The mapping keeps view directions. Within the range that standard rendering can show, the image is the same as standard rendering. Anything farther is compressed into the same frame.
- Not only color but depth (`SV_Depth`), shadows, culling, picking and motion vectors follow the same spatial contract as standard rendering.
- Lighting uses the physical position, not the mapped one.

## Fail closed

When Omni cannot confirm that a path renders correctly, it stops instead of showing a plausible image. For example:

- A Lit path that cannot receive physical lighting data is not drawn.
- Building a scene that contains a RadialCamera for an unsupported target or renderer stops at the start of the build, with the reason.
- A material that cannot be converted is replaced by a diagnostic material while the original is kept, and the result is not reported as success.

## Read next

| Goal | Pages |
| --- | --- |
| Install and run | [Requirements](getting-started/requirements/), [Installation](getting-started/installation/), [Check with the Verification Hub](getting-started/verification/), [Switch between standard and logarithmic rendering](getting-started/switch-rendering/) |
| Understand how it works | [Logical coordinates and proxies](concepts/logical-coordinates/), [The Radial mapping and View Scale](concepts/radial-mapping/), [Near and far](concepts/near-far/), [Fail closed](concepts/fail-closed/) |
| Look up the API | [Omni reference](../reference/) |
| Use each feature | [Working in the Scene View](guides/scene-view/), [Shaders and materials](guides/shaders-and-materials/), [Shader Graph and custom shaders](guides/shader-graph/), [VFX Graph](guides/vfx/), [Terrain](guides/terrain/), [Lighting spaces](guides/lighting-spaces/), [Scene BRG](guides/scene-brg/), [Radial observer camera](guides/observer-camera/), [Sequencer and Pose Bake](guides/sequencer/) |
