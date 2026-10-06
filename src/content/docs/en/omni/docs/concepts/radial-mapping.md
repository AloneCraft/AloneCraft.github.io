---
title: The Radial mapping and View Scale
description: The formula that maps distance from the eye logarithmically, how View Scale is determined, and components that change the mapping.
sidebar:
  order: 2
---

## The formula

Radial maps the distance *d* from the eye (the center of the mapping) with the following formula before drawing.

```text
F(d) = S · log2(1 + d · M / S)    S = 1
```

| Symbol | Meaning |
| --- | --- |
| *d* | Logical distance from the eye (m) |
| *F*(*d*) | Distance in render space (Unity units) |
| *S* | The constant 1. There is no setting for it |
| *M* | View Scale. This is the only parameter you can adjust |

Because *S* = 1, the formula is the same as `F(d) = log2(1 + d · M)`.

There is only one curve. There is no linear region, and no choice of curve or *S*. Nearby objects are also mapped through the curve, so mistakes in handling the mapping show up close to the camera too.

### Shape of the curve

| Range | Behavior |
| --- | --- |
| *d* much smaller than 1/*M* | *F*(*d*) ≈ *d* · *M* / ln 2. Nearly proportional (slope about 1.44 × *M*) |
| *d* much larger than 1/*M* | *F*(*d*) ≈ log2(*d* · *M*). Each doubling of distance adds 1 to the render distance |

The curve starts to bend at a logical distance of about 1/*M*. Even with *M* = 1 the mapping is not the identity. The slope near the origin is *M* / ln 2.

Example values with *M* = 1:

| Logical distance *d* | *F*(*d*) |
| --- | --- |
| 1 m | 1 |
| 1 km | about 10.0 |
| 1 astronomical unit (about 1.5×10¹¹ m) | about 37.1 |
| 1 light-year (about 9.5×10¹⁵ m) | about 53.1 |
| 2⁹⁵ m (the `Scalar` limit, about 3.96×10²⁸ m) | about 95 |

Far away, changing *M* adds about log2 *M* to the render distance. Even the farthest point that Logical coordinates can represent ends up at a finite distance in render space.

## A mapping that keeps directions

The mapping changes only distances, not view directions. *F* also grows with distance, so front-to-back order does not change.

As a result, for a camera whose eye is at the center of the mapping (the Game View, and the first-person view of the Scene View), the following hold:

- Within the range that standard rendering can show, the image is the same as standard rendering. Positions, occlusion, lighting, shadows and lightmaps match.
- Anything farther cannot be shown by standard rendering. Only Radial rendering draws it, compressed into the same frame.
- Depth (`SV_Depth`), shadows, culling, picking and motion vectors follow the same spatial contract as standard rendering.

The mapping is a function of the distance from the eye. One camera therefore applies the same curve to everything it sees. For observer views whose eye is not at the center of the mapping, see [Radial observer camera](../../guides/observer-camera/).

### Lighting uses the physical position

Lighting is computed at the physical position, not at the mapped one. Normals, lights, view directions and shadow information are also evaluated in physical space and are not mixed with mapped positions. For Light Probe and APV spaces, see [Lighting spaces](../../guides/lighting-spaces/).

## View Scale *M*

*M* is determined by the following formula.

```text
M = Camera Near ÷ RadialCamera Near (logical Near)
```

- The Camera Near (`Camera.nearClipPlane`) is a distance in render space. The value set in the Inspector is used as is.
- The RadialCamera Near (`nearClip`) is how many meters that near plane represents in logical space.
- Omni changes the Camera's Near and Far temporarily only while rendering and restores them afterwards. *M* is computed from the Near you set, not from the temporary value.

For example, with a Camera Near of 0.1 and a logical Near of 10⁻⁹ m (1 nm), *M* = 10⁸. You can draw tiny objects without lowering the Camera Near.

The default RadialCamera Near is 0.3 m, the same as the default Near of a Unity Camera. If the Camera Near is also 0.3, *M* = 1.

:::note[What *M* changes]
Because the mapping keeps directions, changing *M* does not change positions on the screen. *M* decides how many logical meters the Camera's near plane stands for, and the logical distance where compression starts.
:::

## RadialCamera

`RadialCamera` is a data holder you add to a Camera. It holds the logical Near and the logical Far. Normally the switch to logarithmic rendering in Omni Manager adds it ([Switch between standard and logarithmic rendering](../../getting-started/switch-rendering/)).

| Member | Description |
| --- | --- |
| `nearClip` (`ScalarField`) | Logical Near (m). Near under Clipping Planes in the Inspector |
| `farClip` (`ScalarField`) | Logical Far (m). The default is the `Scalar` limit |
| `EffectiveNearClip`, `EffectiveFarClip` | The values used for rendering. The defaults are used when a value is 0 or less |
| `EffectiveLog2ViewScale` | log2(*M*), computed from the Camera's authored Near and the logical Near. 0 when it cannot be computed |
| `SetViewScale(float)` | Sets the logical Near to "Camera Near ÷ the given *M*". Uses *M* = 1 if the value is not positive and finite |
| `SetIdentityViewScale()` | Sets *M* = 1. Does not make the mapping the identity |
| `RadialCamera.GetOrAdd(Camera)` | Returns the Camera's RadialCamera, adding one if there is none |

The diagnostics shown in the Inspector are explained in [Near and far](../near-far/).

## Using the same mapping on the CPU

`RadialSettings` computes the same curve as the GPU mapping, on the CPU. `RadialSettings.FromDriver(radialCamera)` takes the *M* and logical Far of that RadialCamera.

| Method | Description |
| --- | --- |
| `TryMap(in Vector3, out float3)` | Maps a Logical position relative to the mapping center into a render-space relative position. Keeps the direction |
| `TryUnmap(in float3, out Vector3)` | The inverse of `TryMap` |
| `TryMapDistance(in Scalar, out float)` | Maps a distance only |
| `TryUnmapDistance(float, out Scalar)` | Maps a render distance back to a logical distance |
| `TryGetLog2Scale(in Scalar, out float)` | log2 of the scale factor (render distance ÷ logical distance) at that distance |

All of them return `false` when the input or the result is not finite.

```csharp
using Omni;
using L = Omni.Logical;

RadialSettings settings = RadialSettings.FromDriver(radialCamera);
if (L.Scalar.TryParse("149597870700", out L.Scalar au) &&
    settings.TryMapDistance(au, out float rendered))
{
    // rendered is the render-space distance of 1 astronomical unit
}
```

## Components that change the mapping

Some components change how the mapping is applied, per renderer.

| Component | Description |
| --- | --- |
| `RadialIdentityWarp` | Draws the Renderer or Terrain on the same GameObject with its Radial shader, but with an identity mapping |
| `Omni.Logical.RadialRigidAnchor` | Maps only the center point of the Renderer on the same GameObject and draws it with its shape kept |

### RadialIdentityWarp

This is for comparing URP's original shader, Radial identity and Radial logarithmic side by side, with the same camera in the same frame.

- It draws with the actual Transform and Unity's bounds. Culling uses the same test as URP.
- It is excluded from Scene BRG and drawn through the per-renderer path.
- For Terrain, it passes identity values to the Terrain's own rendering.

### RadialRigidAnchor

The mapping is non-linear, so mapping each vertex makes the compression rate differ from vertex to vertex, and the shape changes. For a solid body such as a sphere, that deformation is the correct image. Text and labels, however, become unreadable.

`RadialRigidAnchor` maps only the center point and applies the scale factor at that point isotropically to the local shape. It does not add vertices or move the Transform.

:::caution[It is an approximation]
It applies the mapping of one center point to the whole shape. It is valid only when the object spans a narrow distance ratio and solid angle as seen from the center of the mapping. Do not use it for objects such as planetary terrain, where the mapping should be evaluated continuously across the surface.
:::
