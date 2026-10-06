---
title: Requirements
description: The environment Omni 0.1 was built and verified with, and the build target support table.
sidebar:
  order: 1
---

## Built and verified with

| Item | Version |
| --- | --- |
| Unity | `6000.6.0f1` |
| Universal Render Pipeline | `17.6.0` |
| Input System | `1.20.0` |

Omni's code also uses Timeline (`com.unity.timeline`) ([Sequencer](../../guides/sequencer/)). Add Timeline if your project does not have it.

The main verification environment is the Windows Editor and Windows Player. If you use other versions, first check rendering with the [Verification Hub](../verification/).

## Renderers

| Renderer | Status |
| --- | --- |
| Universal Renderer, Forward and Forward+ | Supported |
| Deferred, Deferred+, custom renderers | Not supported. Player builds that contain a RadialCamera stop |
| 2D Renderer | Not supported for a RadialCamera's camera. The build stops |

The Deferred check covers not only the renderer the camera uses but every renderer of every URP asset referenced by the build target's quality levels. If even one Deferred renderer exists, URP puts the GBuffer pass of every shader into the Player, even when it is unused.

## Build targets

Radial shaders require Shader Model 4.5 (equivalent to OpenGL ES 3.1).

| Build target and Graphics API | Status |
| --- | --- |
| Windows (Direct3D 12) | Supported (accepted) |
| Web (WebGPU only) | Supported (accepted with demos). Turn off Auto Graphics API and remove WebGL 2 from the list |
| Windows (Direct3D 11), macOS (Metal), Linux (Vulkan, OpenGL Core), consoles | Not verified (the build is not stopped) |
| Android, iOS, tvOS, visionOS | Not supported (the build stops) |
| WebGL 2, OpenGL ES 3.0, OpenGL on macOS | Not supported (below Shader Model 4.5; the build stops) |

The Player has no way to switch materials back to the original URP shaders. Turning Radial off at run time therefore does not fix the display on an unsupported platform.

## Optional packages

Omni compiles without the following packages. Without them, only the features that use them are unavailable.

| Package | Used for |
| --- | --- |
| `com.unity.visualeffectgraph` | VFX Graph conversion, the Hub's VFX paths |
| `com.unity.entities`, `com.unity.entities.graphics` | The Hub's ECS paths (see the note below) |
| `com.unity.cinemachine` | The Radial observer camera (`GameObject/Omni/Radial Observer Camera`) |

:::caution[DOTS / Entities Graphics]
Omni 0.1 does not include DOTS / Entities Graphics rendering in logarithmic rendering. If a loaded scene has a SubScene, or there are Entities Graphics render entities, switching to logarithmic rendering stops with the reason before changing anything. A Player build that contains both a scene with an active RadialCamera and a scene with a SubScene also stops.
:::

## Quality settings

Omni has no quality profiles or quality keywords of its own. Adjust quality with your project's URP assets and Quality Settings. The Radial mapping and depth do not change with the quality level.

If you use several quality levels, make the differences with the following settings so that shader variants do not increase.

- Render Scale
- Shadow resolution, distance and cascade count
- Soft Shadows quality
- MSAA
- HDR precision

Keep the following settings the same across the quality levels of one build target. URP keeps the union of variants of all URP assets of the build target's quality levels, so differences increase variants and may hit Omni's limit (8,192 per pass and stage).

- Soft Shadows on or off
- Additional light shadows
- Light Layers
- Terrain Holes
- LOD Cross Fade
- The renderer's Rendering Path and Renderer Features (SSAO, Decal, and so on)
