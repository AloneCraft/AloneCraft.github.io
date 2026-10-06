---
description: "A component that maps only one central point of the Renderer on the same GameObject and draws it with its shape preserved."
---
`RadialRigidAnchor` draws the Renderer on the same GameObject as a Radial rigid body. It requires a Renderer (`RequireComponent`). It has no settings and no public members.

The Radial mapping is non-linear. Mapping each vertex separately gives each vertex a different compression ratio, so the shape changes. For solid bodies such as spheres that is the correct image, but text and labels become unreadable. With `RadialRigidAnchor`, the GPU maps only the object's origin and applies the scale factor at that point (render distance ÷ logical distance) isotropically to the local shape. It does not add vertices or move the Transform.

- Use it for objects that keep their shape around one point, such as buildings, machines and UI labels.
- Enabling or disabling it reselects the render targets on the next update.

:::caution[It is an approximation]
It applies the mapping of one central point to the whole shape. It is valid only when the object spans a narrow range of distance ratios and a small solid angle as seen from the center. Do not use it for objects such as planetary terrain, where the mapping must be evaluated continuously across the surface.
:::

For all the components that change the mapping, see [The Radial mapping and View Scale](../../../docs/concepts/radial-mapping/).

<!-- members -->
