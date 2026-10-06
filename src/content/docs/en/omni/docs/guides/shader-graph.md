---
title: Shader Graph and custom shaders
description: How custom ShaderLab/HLSL and Shader Graph are converted to Radial, the Custom Function entry points, the depth contract, and the samples.
sidebar:
  order: 3
---

Omni converts your project's custom shaders and Shader Graphs for Radial (logarithmic) rendering in your environment. You do not need to generate anything in advance. The switch to logarithmic rendering does the needed generation and re-checks ([Switch between standard and logarithmic rendering](../../getting-started/switch-rendering/)).

:::tip[Start with Shader Graph]
The conversion is built from the graph's structure (which node uses which value). This makes it more reliable than adapting a hand-written `.shader` to Radial. When you make your own effects, we recommend starting with Shader Graph.
:::

## Where generated files go

| Generated files | Location |
| --- | --- |
| Converted custom shaders and Shader Graphs | `Assets/Radial/User` |
| Custom includes (HLSL) that had to be converted | `Assets/Radial/Hlsl`, keeping the original logical path |

- Package includes that were not changed are not duplicated. The original `Packages/...` is referenced.
- Regeneration keeps the GUID, scene and prefab references, RenderQueue, keywords, passes and properties. A conversion that cannot keep them is not treated as success.
- If you edit a generated file by hand, its output hash no longer matches, so Omni stops overwriting it. The material list shows "Manual edits / Review required".

## Converting ShaderLab/HLSL

The conversion proceeds in this order.

1. Classify the source as ShaderLab or Shader Graph
2. Check the target SubShaders, passes, vertex and fragment connections, includes and properties at syntactic boundaries
3. Rewrite the HLSL, inject Radial, and create the needed include copies and records
4. Check that the generated files have the required passes, properties, tags, and depth and lighting contracts

- Relative includes are resolved from the logical location of the original shader.
- `UsePass`, `Fallback`, `Dependency` and `CustomEditor` are also analyzed.
- Forward, Depth, DepthNormals, ShadowCaster, MotionVectors, Picking, Meta and custom passes are diagnosed one pass at a time.
- ShadowCaster and Meta do not get the rendering camera's Radial projection. They stay in physical space.

## Converting Shader Graph

Shader Graphs are generated in your environment by the same converter, regardless of where they come from (your project, URP or other packages).

| Graph | Handling |
| --- | --- |
| URP Lit, Unlit | Converted (Radial is injected into the vertex Position) |
| URP Fullscreen | Converted through a dedicated path (see below) |
| URP Decal | Converted through a dedicated path |
| Canvas/UI, 2D Sprite, Six-way, Terrain Lit, graphs with only non-URP targets | Not converted. Stops with a reason |

- Radial is injected into vertices only for the URP Lit and Unlit SubTargets, whose physical lighting contract has been checked.
- Graphs in the format before Shader Graph 10 (the Master Node format) are upgraded to the current format the same way Shader Graph import does, and then converted.

### Fullscreen graphs

This path is for effects such as fog that compute distance from the depth buffer. With Radial, the depth buffer holds depth after the mapping. Without conversion, distant fog would be thinner than in standard rendering.

- Vertices are not converted.
- The Scene Depth node, and the world position reconstructed from it, return depth converted back from mapped depth to physical depth. The same applies when a Custom Function calls `SampleSceneDepth`.
- In graphs with Depth Write enabled, the physical depth the graph writes is converted to mapped depth just before the depth block (Eye, Linear01, Raw). The mapping does not change view directions, so screen positions stay the same.
- The "far" markers for pixels that a ray misses are written as they are: 1e20 m or more for Eye Depth, 1 or more for Linear01 Depth, and the far value for Raw Depth.
- Materials embedded in a graph and referenced by a FullScreenPass cannot have their shader rewritten, so the reference is redirected to a managed copy.

:::caution
Effects in which the graph reads depth from its own texture property (such as depth copied to an RTHandle) are outside this replacement. They do not match standard rendering.
:::

:::caution[Far with Eye Depth]
Do not use an Eye Depth equal to the camera's far plane as the "far" marker. With Radial it cannot be distinguished from the mapped far plane, and it is corrected to a nearer depth.
:::

## Custom Function entry points

`Assets/Omni/Runtime/Sharders/RadialWarpNode.hlsl` is the Custom Function entry point for Shader Graph. The nodes that the converter adds to graphs use the following functions. Each has a `_float` and a `_half` version.

| Function | Input → output | Role |
| --- | --- | --- |
| `RadialKeepObjectPosition` | Object-space position and Radial Blackboard values → object-space position | For the vertex Position. Returns the position unchanged; the vertex hook does the actual Radial projection. It is connected to keep the Radial Blackboard properties in the graph |
| `RadialGetLogicalViewDirection` | Object-space position → logical view direction | Returns the logical view direction passed to fragment lighting |
| `RadialPreserveEmission` | Logical view direction, original emission → original emission | Passes emission through unchanged. Stores the fragment's logical view direction before PBR initialization |
| `RadialPreserveDecalAlpha` | Alpha → the same alpha | Does not change the value. Keeps this include in Decal graphs and enables the inverse transform of positions reconstructed from depth in Projector passes |
| `RadialPreserveFullscreenColor` | Color → the same color | Does not change the value. Keeps this include in Fullscreen graphs and enables the Scene Depth replacement |
| `RadialFullscreenDepth` | Screen position, depth, Mode → depth | In Fullscreen graphs with Depth Write, converts physical depth to mapped depth. Mode 0 is Eye, 1 is Linear01, 2 is Raw |

- The Shader Graph include is expanded into every pass. In ShadowCaster and Meta, the vertex hooks are disabled automatically.
- The vertex hooks are also disabled in Decal Projector passes.

:::note[View Direction in the vertex stage]
In the Shader Graph vertex stage, URP evaluates vertex-side work (such as Custom Interpolators) before computing the vertex position. The View Direction node at that point is the physical view direction (camera position − vertex world position), the same as standard URP. The Fog node is not affected, because URP's standard computation gets the distance from the physical world position and the View matrix.
:::

## Hand-written shaders

If you write an ordinary URP shader, the switch to logarithmic converts it. To write a shader that supports Radial directly, use the public includes in `Assets/Omni/Runtime/Sharders`.

| Include | Purpose |
| --- | --- |
| `RadialVertexInject.hlsl` | The vertex entry point for hand-written and generated URP shaders. Reads `RadialInject.hlsl` and `RadialVertexHooks.hlsl`. Disables the vertex hooks in ShadowCaster and Meta |
| `RadialInject.hlsl` | The shared implementation of the Radial packet, coordinate transforms and depth reconstruction |
| `RadialVertexHooks.hlsl` | Read after `RadialInject.hlsl`. Connects URP's vertex entry points from object space to camera-relative mapped clip space. Do not include it on its own |
| `RadialObjectMotionVectors.hlsl` | A dedicated entry point that builds motion vectors from the object's current and previous-frame positions. Do not use it for ordinary Forward passes |

Include `RadialVertexInject.hlsl` after `Core.hlsl` and the material inputs (such as `LitInput.hlsl`), and before each pass's implementation.

```hlsl
#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
#define OMNI_RADIAL_EXPECT_ACTIVE 1
#include "Assets/Omni/Runtime/Sharders/RadialVertexInject.hlsl"
// Include the pass implementation after this
```

With `OMNI_RADIAL_EXPECT_ACTIVE` defined, including it before `Core.hlsl` is a compile error. The bundled Radial versions (for example `Assets/Omni/Radial/Universal Render Pipeline/Shaders/Lit.shader`) show the property declarations and the include order in practice.

Omni decides whether a shader supports Radial by whether it declares `_RadialGlobal_UseRadialTransform`. A Radial shader with no standard version shows as "Native" in the Shaders tab and "Conversion not required / Excluded" in the Materials tab, and is not restored to standard.

## The depth contract

Radial shaders keep the same spatial contract as standard rendering for depth as well as color.

| Contract | Details |
| --- | --- |
| Vertex transform | From object space to Radial clip space. Forward, Depth, DepthNormals, Picking and MotionVectors use the same spatial contract |
| Fragment depth | The depth of the mapped physical point (`SV_Depth`), written in Forward, DepthOnly, DepthNormals and Picking. To solve for depth, the fragment evaluates the same mapping per pixel |
| Object motion vectors | Built from the current and previous-frame Radial clip positions. No fake velocity when there is no history |
| Lighting data | Lit Forward and Forward+ use the Radial raster position and physical lighting data. Paths that cannot receive physical lighting data are not drawn ([Fail closed](../../concepts/fail-closed/)) |

Every pass that writes depth must write the same depth at the same position. The default renderer (Forward+ with Depth Priming set to Forced) writes depth first in a DepthOnly or DepthNormals prepass, and draws the color pass with ZTest Equal. If the depth differs between passes, color pixels drop out. Looking correct only with Depth Priming turned off is not evidence of support.

- The depth and color passes of converted shaders need Shader Model 4.5 (equivalent to OpenGL ES 3.1).
- The mapping, depth and lighting-data contracts do not change with the quality level.

### Depth in Shader Graph

URP's Lit and Unlit Shader Graphs have no block that outputs per-pixel depth (`SV_Depth`). Omni's converter adds the Radial-mapped per-pixel depth to the passes of converted graphs that write depth (color, DepthOnly, DepthNormals, and scene selection and Picking).

- This is the same contract as for converted hand-written shaders, so intersections between large surfaces and Depth Priming give depth ordering close to standard rendering.
- You cannot set depth with graph nodes. Depth comes from the surface of the mesh being drawn.

## Shader Graph samples

`Assets/Omni/Verification/Functions/Shaders/GraphSamples` contains samples that rebuild, in Shader Graph, the same effects as the hand-written shaders of the CosmicScale demo. You can compare side by side how to build graphs, and the depth limits that graphs cannot express.

| Scene | Graph | Contents |
| --- | --- | --- |
| `BlackHole.unity` | `OmniGraphBlackHole` (Unlit, transparent Premultiply, back faces) | Gravitational lensing of a black hole: a bent starfield, the photon ring, the accretion disk and Doppler colors |
| `Nebula.unity` | `OmniGraphNebula` (Unlit, transparent Premultiply, back faces) | A ring-shaped planetary nebula: a procedural volume that adds emission while stepping evenly through the shell |
| `Star.unity` | `OmniGraphStar` (Unlit, opaque) | A star's surface: granulation, sunspots and faculae, flare loops, limb darkening, differential rotation |
| `Planet.unity` | `OmniGraphPlanet` (Unlit, opaque) | The Moon: a Custom Function for UVs, Sample Texture 2D, and a Custom Function for lighting |
| `DepthMarch.unity` | `OmniGraphDepthMarch` (Fullscreen, Depth Write) | Spheres and rings drawn by full-screen ray tracing intersect the floor, pillar and box meshes in depth |

| Location | Contents |
| --- | --- |
| `Assets/Omni/Verification/Shaders/GraphSamples/` | The graphs and the Custom Function HLSL |
| `Assets/Omni/Verification/Generated/Materials/GraphSamples/` | The sample materials |
| `Assets/Omni/Demo/CosmicScale/Shaders/` | The hand-written reference shaders |

### How to read the scenes

The object samples (BlackHole, Nebula, Star, Planet) have four columns. Every column uses the same sphere mesh, and the column headings appear above the objects.

| Column | Contents |
| --- | --- |
| Shader Graph (URP) | The graph before conversion, rendered by standard URP |
| Radial identity | The same graph made Radial by Omni's converter, drawn with the identity mapping |
| Radial logarithmic | The converted graph, drawn with the logarithmic mapping |
| Hand-written (CosmicScale) | The hand-written shader for the same effect (the reference for what it should look like) |

1. If **Shader Graph and Radial identity** look the same, the conversion has not changed the look.
2. The difference between **Radial identity and Radial logarithmic** is the effect of the logarithmic mapping alone. Shape and position look the same; only the depth and lighting computations are affected by the mapping.
3. The difference between **Radial logarithmic and hand-written** comes from moving the effect into a graph. The nebula's pattern noise uses a baked 3D noise in the hand-written version, while the graph version computes the same kind of noise, so the details do not match.
4. The columns sit side by side, so each is seen from a slightly different angle. This is why the Moon's phase differs between columns.

- The scenes save only the materials before conversion. When you open a scene in the Editor, the two Radial columns are switched to the Radial versions generated in your environment (under `Assets/Radial/User/`). They are switched back just before saving, so the scene is not modified.
- If the conversion fails, that column is not drawn, and the reason appears in the Console.
- The samples use the same exposure and tone mapping as CosmicScale. Strongly emissive effects blow out to white without tone mapping.
- The on-screen panel switches between Radial and standard, toggles back-and-forth motion and shows motion vectors. In Edit Mode, the "Omni Verification" overlay in the Scene View and the main camera's Inspector have the same controls and an action that opens the document (labeled "文書を開く").

### Depth limits of object graphs

Effects that draw their contents by ray tracing are drawn on the mesh of an outer shell, so the depth of that surface is not enough.

| Sample | Difference |
| --- | --- |
| Black hole | The hand-written version writes, with `SV_Depth`, the depth of the point on the pixel's ray at the same distance as the center, and compares with that depth. The graph version is transparent without depth writes and compares with the back face of the shell. Objects inside the shell behind the center should be hidden, but are not hidden in the graph version |
| Nebula | The graph version adds light along the whole segment where the ray passes through the shell. Even with an object inside the shell, the light behind it is laid over the object |
| Star, Moon | Opaque surfaces, so they sort correctly with the same depth as ordinary meshes |

To avoid this, make the effect's shell a size that does not overlap other objects. If overlap cannot be avoided, use a hand-written shader or a full-screen graph.

### Full-screen graphs can write depth

In URP's Fullscreen Shader Graph, enabling **Depth Write** in the graph settings adds an **Eye Depth**, **Linear01 Depth** or **Raw Depth** block to the output (chosen with Depth Write Mode), and you can write per-pixel depth. `DepthMarch.unity` is the sample of this.

- For pixels hit by a ray, write the eye depth from the camera (the distance along the camera's forward direction).
- For pixels not hit, set alpha to 0 and write the "far" depth. With the depth test (Less Equal), objects already drawn are not overwritten.
- Write "far" as a very large value such as 1e30 for Eye Depth (as in the sample), 1 for Linear01 Depth, or the far value (`UNITY_RAW_FAR_CLIP_VALUE`) for Raw Depth.
- Draw the full-screen graph with a URP Renderer Feature (Full Screen Pass) or a scene component (in the sample, `GraphSampleFullscreenPass`, which affects only that scene's camera).
- To read the floor's depth inside the ray tracing, enable the camera's depth texture.

### Making your own graph

1. Write heavy work (such as ray tracing with loops) in the HLSL of a Custom Function node (Type = File). In the graph, connect the input nodes (Position, Camera, Screen Position, Scene Depth, Time, Normal Vector, View Direction) and properties.
2. Define the Custom Function as `FunctionName_float`, and set the graph's precision to Single.
3. Start object-space ray tracing from Position (Object) and the camera's Position moved into object space with Transform (World → Object, Position).
4. Read depth with the Scene Depth node. Calling `SampleSceneDepth` inside Custom Function HLSL is also replaced, but you need a Scene Depth node in the graph so that it requests the depth texture.
5. For translucent effects that output emission and alpha separately, set Blending Mode to Premultiply.
6. Do not use the globals of hand-written shaders from graphs. Make them material properties.

:::note[Not checked]
These samples do not check whether graphs that use the Transform node's matrices render correctly for objects farther than the safe area (10 km).
:::

## Registering a substitute shader

For shaders Omni cannot convert, such as fixed-function or Built-in-only shaders, you can register a substitute URP shader.

1. In the Project window, choose **Create > Omni > Radial Shader Substitutes** to create a registry (`OmniRadialShaderSubstitutes`).
2. Add an entry, and set the original shader (`source`) and the substitute URP shader (`substitute`).

- Only when switching a material to Radial, Omni generates a Radial version from the substitute and uses it. The original shader and its file are not changed. Switching back to standard returns to the original shader.
- The substitute must declare every property of the original shader, with the same name and type. Float and Range count as the same type.
- The substitute must have a name that does not start with `Hidden/`, and must pass the normal checks as a URP shader.
- You can have several registries in the project. If registries disagree for the same original shader, or properties are missing, Omni does not convert and shows the reason.

## What cannot be converted

| Target | Handling |
| --- | --- |
| Fixed-function and Built-in-only shaders | Not converted. You can register a substitute shader |
| Canvas/UI, 2D Sprite, Six-way, Terrain Lit Shader Graphs, and graphs with only non-URP targets | Stops with a reason |
| Fullscreen effects that read depth from their own texture | Do not match standard rendering |
| GBuffer passes for Deferred and Deferred+ | Not supported. Player builds that contain a RadialCamera stop |
| XR Camera MotionVectors | Not within the supported scope |
| Terrain trees and details | Not supported by shader conversion alone ([Terrain](../terrain/)) |

A material that cannot be converted is never replaced by a guessed similar shader. The reference to the original material is kept, the slot shows diagnostics, and the result is not reported as success ([Shaders and materials](../shaders-and-materials/)).
