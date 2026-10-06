---
description: "The camera packet read by Radial shaders, the property that marks Radial support, and the defines that change include behavior."
---
Global values and defines read by Radial shaders.

- `_OmniRadialCamera_*` is the camera packet. Omni writes it per camera before rendering (in SRP, in order into the camera's command stream). Scene BRG draws read values with the same names from per-camera instance data, so the values of a Game View and a Scene View drawn in the same frame never mix. They are declared in `RadialInject.hlsl`.
- Position values never hold absolute float coordinates. A Logical difference is split into a mantissa (xyz) and a power-of-two exponent (w); the value is `xyz × 2^w`.
- `Previous*` are the values of the packet previously written for the same camera. The first time a camera is written they equal the current values, so a missing history never produces a large fake velocity. Object motion vectors use them.
- When Omni releases its rendering system, it sets `_OmniRadialCamera_State` and `_OmniRadialCamera_PreviousState` to 0.

The component meanings are an internal Omni contract. In your own shaders, reading them through the functions in `RadialInject.hlsl` is recommended. See [Include files](../includes/) for which include to use and [Radial mapping](../../../docs/concepts/radial-mapping/) for the mapping.

<!-- members -->

### _OmniRadialCamera_State

The camera state. x is 1 when the Radial mapping is active for this camera and 0 otherwise (it is 0 if the warp is disabled, the mapping is identity, there are no Radial targets, or the warp center cannot be resolved). y, z and w are internal Omni flags (label visibility, eye handling, and whether camera-relative instance data is used).

### _OmniRadialCamera_Curve

The mapping curve. x = S (always 1; shaders do not read it), y = whether to map (0 for `OMNI_RADIAL_MODE_SMOOTHLOG` or −1 for `OMNI_RADIAL_MODE_IDENTITY`), z = reserved (0; shaders do not read it), w = log2(M) (the base-2 logarithm of the View Scale M).

### _OmniRadialCamera_EyeRelative

The eye's offset from the Logical warp center (eye − center). xyz is the mantissa and w the power-of-two exponent. It is 0 when the camera itself is the warp center (the camera with the `RadialCamera`).

### _OmniRadialCamera_OriginRelative

The Logical world origin's offset from the warp center (origin − center). xyz is the mantissa and w the power-of-two exponent. It is 0 when State.x is 0.

### _OmniRadialCamera_ViewRow0

The first row of the world-to-view rotation. It contains no translation, and includes the same Z flip as Unity's view matrix.

### _OmniRadialCamera_ViewRow1

The second row of the world-to-view rotation.

### _OmniRadialCamera_ViewRow2

The third row of the world-to-view rotation. It is the negated camera forward direction.

### _OmniRadialCamera_PreviousState

The previously written `_OmniRadialCamera_State`. While its x is 0 (the first frame, or right after switching from non-Radial), object motion vectors output zero velocity.

### _OmniRadialCamera_PreviousCurve

The previously written `_OmniRadialCamera_Curve`.

### _OmniRadialCamera_PreviousEyeRelative

The previously written `_OmniRadialCamera_EyeRelative`.

### _OmniRadialCamera_PreviousOriginRelative

The previously written `_OmniRadialCamera_OriginRelative`. Even when the warp center moves, the difference between the current and previous origin offsets gives the previous frame's position without going back to absolute coordinates.

### _OmniRadialCamera_PreviousViewRow0

The previously written `_OmniRadialCamera_ViewRow0`.

### _OmniRadialCamera_PreviousViewRow1

The previously written `_OmniRadialCamera_ViewRow1`.

### _OmniRadialCamera_PreviousViewRow2

The previously written `_OmniRadialCamera_ViewRow2`.

### _RadialGlobal_UseRadialTransform

The shader property that marks Radial support. Omni decides whether a shader supports Radial by whether its Properties declare this name (`RadialMaterialContract`). A stale value left on a Material does not count. Omni sets the value per renderer: 1 is the precise Logical transform, −1 combines the ordinary Unity Transform with the camera packet, and 0 means no mapping (Unity's standard transform). Use 0 as the default in Properties.

### OMNI_RADIAL_MODE_SMOOTHLOG

The mode value for mapped rendering (0.0). Defined in `RadialWarp.hlsl`, and matches the value in C# `RadialGlobalRenderSystem`.

### OMNI_RADIAL_MODE_IDENTITY

The mode value for unmapped rendering (−1.0). Omni sends it when the warp is disabled, when there is no `RadialCamera`, and for orthographic projection. Defined in `RadialWarp.hlsl`.

### OMNI_RADIAL_HOOKS_DISABLED

When defined, `RadialVertexHooks.hlsl` does not replace URP's vertex functions, and the pass renders with URP's standard vertex transform. `RadialVertexInject.hlsl` defines it automatically in the ShadowCaster and Meta passes, and `RadialWarpNode.hlsl` in the ShadowCaster, Meta and Decal Projector passes and in Fullscreen graphs. To render a custom pass in physical space, define it before the include.

### OMNI_RADIAL_EXPECT_ACTIVE

When defined before `RadialVertexInject.hlsl`, including it before URP's `Core.hlsl` is a compile error. Without `Core.hlsl` the vertex hooks are silently disabled, so define it to catch include-order mistakes. The bundled Radial shaders (for example `Lit.shader`) define it as `1` right before `RadialVertexInject.hlsl`.
