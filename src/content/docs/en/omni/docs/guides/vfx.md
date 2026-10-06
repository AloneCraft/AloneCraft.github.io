---
title: VFX Graph
description: How VFX Graph outputs are made to support logarithmic rendering, the supported outputs, lighting, and VFX far from the origin.
sidebar:
  order: 4
---

Omni draws VFX with logarithmic rendering by injecting Radial into VFX Graph's display shaders. The VFX Graph features need `com.unity.visualeffectgraph`. Without it, only the parts that use VFX are unavailable.

## Supported outputs

The following outputs are checked one at a time in the scenes in `Assets/Omni/Verification/Functions/Vfx/Modules`.

| Kind | Outputs |
| --- | --- |
| Unlit | Quad, Triangle, Octagon, Mesh, Cube, Quad Strip, Line, Line Strip, Point, Soft Particle |
| Shader Graph | Quad, Strip |
| Static Mesh | Static Mesh |
| URP Lit | Quad, Mesh, Quad Strip |
| URP Decal | Decal |

The following have nothing to check in this version. The reasons are kept in `Assets/Omni/Verification/Generated/VFX/Modules/modules.json`.

| Output | Reason |
| --- | --- |
| Six-way | VFX Graph 17.6 / URP 17.6 have no Six-way output context |
| Sphere | The Sphere output exists only as a template (ParticleSpheres), with no context type |

URP's 2D VFX graphs for sprites are not converted to Radial ([Shaders and materials](../shaders-and-materials/)).

## How the conversion works

- Omni applies a Radial patch to the display shaders of every VFX in the project, each time it is imported.
- The patch takes effect only when drawing with a Radial camera. Rendering with standard cameras does not change.
- VFX that were imported before the hook and have no patch are reimported by the switch to logarithmic.
- When Omni's injected shader code changes, the VFX are reimported as well.
- Non-asset runtime materials owned by VFX Graph are not in the material list. They are checked through the VFX readiness state.

The top of Omni Manager shows the VFX state, with the number ready and the number required.

| State | Meaning |
| --- | --- |
| `Ready` | Every required VFX render shader has the patch |
| `NeedsGeneration`, `Generating` | A patch is needed, or is being applied |
| `Stale`, `Faulted` | The patch is out of date, or cannot be applied. Player builds that contain a RadialCamera stop |

In a VFX Graph version where the internal callbacks cannot be found, the state is `Faulted` ([Fail closed](../../concepts/fail-closed/)).

## Space and lighting

The spatial contract of VFX is the same as that of Radial Lit meshes.

| Processing | Position used |
| --- | --- |
| Rasterization (clip, view, previous-frame clip) | The mapped, camera-relative position |
| Lighting | The physical world position (the absolute world position URP expects) |
| World-space distance sorting | The mapped "target − viewpoint" distance |
| Processing that uses scene depth (Soft Particle, Decal output) | Mapped depth is converted back to physical distance before comparing |

- Lit outputs (URP Lit Quad, Mesh, Quad Strip) are lit at the physical world position. URP's `VFXLit.hlsl` uses this position for lighting, shadow coordinates, additional lights, APV and reflections. The view direction is URP's own computation.
- If a pass that uses URP's VFX Lit lacks Omni's injection, the patch is not applied, and it is reported as unsupported with the reason. Omni never lights without physical lighting data.
- The Decal output follows the same contract as mesh decals. The volume's vertices are mapped, and the reconstructed position of the receiving surface is mapped back to the physical world.

:::note[Soft Particle edges]
In outputs that map only vertices, depth inside a quad does not follow the mapped curved surface exactly. This shifts the color slightly at Soft Particle edges (where they meet the receiving surface).
:::

For more on lighting spaces, see [Lighting spaces](../lighting-spaces/).

## Constraints

- A renderer's MaterialPropertyBlock does not reach VFX output shaders, with or without instancing.
- VFXRenderer does not accept overrides of `Renderer.bounds` and `localBounds`. Culling always uses the VFX's own bounds (the graph's bounds transformed by the Unity Transform).

## VFX far from the origin

When the `Omni.Logical.Transform` on the same GameObject as a VFX is outside the safe area, Omni draws that VFX with the same mapping as the precise mesh path.

- Each VFX is assigned a table number. The maximum is 32.
- The world position of the Unity Transform used as its proxy becomes a marker for the number: a point within 0.5 m of the origin. Rotation stays at the logical orientation, and world scale is 1.
- Just before each camera renders, Omni distributes the position data for each number. The output shader maps with this data. Camera-facing orientations also use it.

| Condition | Handling |
| --- | --- |
| Non-Radial cameras, shadows, Decal outputs | Not drawn (fail closed) |
| The VFX GameObject itself has no `Omni.Logical.Transform` (including when only a parent has one) | Not supported |
| The 33rd and later | Not supported |
| Computations that use Unity coordinates, such as gravity in world-space systems | Not supported |
| Orientations facing the camera position (used by VFX's common functions) | Not supported |

:::caution
- Other components that read the proxy Unity Transform (such as audio sources) are also not positioned correctly.
- The marker sits near the origin because culling uses the VFX's own bounds. From a Radial camera that is inside the safe area but far from the origin, the VFX may not be drawn unless the origin is inside the frustum.
:::

For the safe area and Logical coordinates, see [Logical coordinates and proxies](../../concepts/logical-coordinates/).

## Checking

| Scene | Contents |
| --- | --- |
| `Functions/Vfx/Modules` | One scene per VFX output. In the near group, check that the standard and Radial images match. In the far group, check that shape and brightness match a reference mesh at the same place |
| `Functions/Vfx/FarLogicalAccretionDisk.unity` | A particle accretion disk at a Logical position 1e20 m from the origin. Check position, depth and the lensed image. It is not drawn when Radial is turned off |

- The images in the near group match because the mapping does not change view directions. If lighting uses physical positions, only depth differs.
- In a module scene, the scene action labeled "撮影して採点" (capture and score) captures and scores the open module in standard and Radial. Results are saved to `Logs/OmniVfxModules`.
- Both need VFX Graph.
- In the Verification Hub, you can check with the `VFX` series under **Case** and the VFX path under **Execution Path** ([Check with the Verification Hub](../../getting-started/verification/)).
