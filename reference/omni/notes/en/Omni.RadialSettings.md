---
description: "A struct that holds the Radial (logarithmic) mapping settings and computes the same mapping as the GPU on the CPU."
---
`RadialSettings` is a value for computing the Radial mapping `F(d) = log2(1 + d · M)` on the CPU. It uses the same curve as the GPU mapping. It holds only log2(*M*) and the logical Far.

- Take the current values of a [RadialCamera](../radial-camera/) with `RadialSettings.FromDriver(radialCamera)`.
- The mapping applies to positions relative to the mapping center (normally the eye). It keeps the direction and changes only the distance.
- Methods starting with `Try` return `false` when an input or the result is not finite. Do not use the `out` values when they return `false`.

For the formula and the meaning of *M*, see [The Radial mapping and View Scale](../../../docs/concepts/radial-mapping/).

```csharp
using Omni;
using L = Omni.Logical;

RadialSettings settings = RadialSettings.FromDriver(radialCamera);
if (L.Scalar.TryParse("149597870700", out L.Scalar au) &&
    settings.TryMapDistance(au, out float rendered) &&
    settings.TryUnmapDistance(rendered, out L.Scalar back))
{
    // rendered is the render-space distance of 1 astronomical unit; back maps it back to a logical distance
}
```

<!-- members -->

### RadialScale

The *S* of the mapping, a constant 1. There is no setting.

### DefaultLog2ViewScale

The default log2(*M*), 0 (*M* = 1). Even with *M* = 1 the mapping is not the identity.

### DefaultFarClip

The default logical Far. The same maximum of `Scalar` (about 3.96×10²⁸ m) as `RadialCamera.DefaultFarClip`.

### m_Log2ViewScale

The stored log2(*M*). A non-finite value is treated as 0 when read.

### m_FarClip

The stored logical Far (m). A value of 0 or less (unset) is treated as `DefaultFarClip` when read.

### RadialSettings(float)

Creates settings with the given log2(*M*). The logical Far is `DefaultFarClip`. If `log2ViewScale` is not finite, 0 is used.

### RadialSettings(float, in Omni.Logical.Scalar)

Creates settings with the given log2(*M*) and logical Far. If `log2ViewScale` is not finite, 0 is used; if `logicalFarClip` is not positive, `DefaultFarClip` is used.

### Default

The default settings: log2(*M*) is 0 and the logical Far is `DefaultFarClip`.

### Log2ViewScale

log2(*M*). *M* itself is never turned into a `float`, so very large and very small *M* can be represented.

### FarClip

The logical Far (m). Always returns a positive value.

### TryMap

Maps a Logical position relative to the mapping center to a render-space relative position. It keeps the direction and maps only the distance. A zero vector input returns zero and succeeds.

### TryUnmap

Maps a render-space relative position back to a Logical position relative to the mapping center. The inverse of `TryMap`. Fails when the input is not finite or when the recovered distance becomes 0.

### TryMapDistance

Maps only a logical distance, with the same curve as `TryMap`. Fails when the distance is negative. A distance of 0 returns 0 and succeeds.

### TryUnmapDistance

Maps a render distance back to a logical distance. The inverse of `TryMapDistance`. Fails when the render distance is negative or not finite.

### TryProject

Maps a relative position and also returns the log2 of the scale factor (render distance ÷ logical distance) at that position. This computes the mapping once, instead of calling `TryMap` and `TryGetLog2Scale` separately. A zero vector input returns the log2 of the scale factor at the origin (*M* / ln 2).

### TryGetLog2Scale

Returns the log2 of the scale factor (render distance ÷ logical distance) at the given logical distance. A distance of 0 returns the log2 of the scale factor at the origin (*M* / ln 2). Fails when the distance is negative.

### ApplyTo

Writes these settings to a `RadialCamera`. It sets the logical Near to "Camera Near ÷ *M*" and the logical Far to `FarClip`. Without a Camera, the Near is taken as 0.3. Returns `false` without changing anything when `driver` is `null` or the logical Near cannot be derived as a positive value.

### FromDriver

Creates settings from the current log2(*M*) (`EffectiveLog2ViewScale`) and logical Far (`EffectiveFarClip`) of a `RadialCamera`. Returns `Default` when `driver` is `null`.

### Equals(RadialSettings)

Returns whether log2(*M*) and the logical Far are equal.

### Equals(object)

Returns whether the object is a `RadialSettings` with equal log2(*M*) and logical Far.

### GetHashCode

### ToString

Returns a string of the form `SmoothLog S=1 log2(M)=…`. The logical Far is not included.
