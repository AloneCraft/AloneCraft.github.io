---
title: Terrain
description: How Terrain surfaces are drawn with patches for logarithmic rendering, falling back to the native Terrain, the four-layer limit, and trees and details.
sidebar:
  order: 5
---

Omni renders the Terrain surface logarithmically with patches that are not saved. Trees and details (grass and similar) are not covered by logarithmic rendering.

## What is covered

| Target | Handling |
| --- | --- |
| The Terrain surface | Drawn with the bundled Radial Terrain shaders (TerrainLit, TerrainLitAdd, TerrainLitBase, TerrainLitBasemapGen) and patches that are not saved |
| Terrains with five or more layers | No patches are made; the native Terrain is shown |
| Trees and details (tree/detail) | Not supported by logarithmic rendering. Hidden on Radial cameras |
| Terrain Lit Shader Graphs | Not converted. Stops with a reason |

Like other materials, Terrain materials are switched to the Radial shaders by the switch to logarithmic ([Shaders and materials](../shaders-and-materials/)). Unity's default Terrain material cannot be written, so a managed copy is made in `Assets/Radial` and used instead.

## Patches that are not saved

URP's native Terrain surface cannot interpret the vertex mapping. So when a Terrain's material supports Radial, Omni draws the Terrain surface with MeshRenderer patches that are not saved.

- The TerrainData is always the source of truth. The patches are derived data for rendering, and are saved neither to the scene nor to assets.
- The native Terrain surface is hidden only after every patch has been built.
- When patches cannot be built, when they are released, and at scene boundaries, the native Terrain display comes back.
- When you change the heightmap, only the affected patches are rebuilt.
- The patches are drawn outside the Terrain, so Omni copies from the TerrainData the control texture and layer values that the TerrainLit base pass reads (`_Control`, `_Splat0` to `3`, tiling and so on).
- The patch resolution is fixed. There is no LOD switching by distance.

## The four-layer limit

The patches draw only the four layers that the TerrainLit base pass draws in one go. In the native Terrain, the fifth and later layers are added by an extra pass (TerrainLitAdd), but the patches do not draw it.

To avoid showing an image with some layers missing, a Terrain with five or more layers gets no patches and shows the native Terrain. To draw with patches, use four layers or fewer.

## Trees and details

Terrain trees and details (grass and similar) use placement and rendering paths separate from the Terrain surface. Shader conversion alone does not support them.

When drawing with a Radial camera, Omni hides the trees and details of Terrains whose tree and detail drawing (`drawTreesAndFoliage`) is enabled, for that camera's rendering only. A warning with the reason appears in the Console. Only the first warning of each kind is shown.

## Terrains under a Logical Transform

If the material of a Terrain under `Omni.Logical.Transform` does not support Radial, that Terrain is not shown on Radial cameras. A warning appears in the Console. Switch to logarithmic rendering in Omni Manager first ([Switch between standard and logarithmic rendering](../../getting-started/switch-rendering/)).

## Comparing with RadialIdentityWarp

When you add `RadialIdentityWarp` to the same GameObject as a Terrain, that Terrain is drawn with its Radial shader but with the identity mapping, even on a camera that applies the mapping. It is for comparing URP's own shader, Radial identity and Radial logarithmic side by side, with the same camera in the same frame.

- An identity Terrain gets no patches. The identity values are passed to the Terrain's own rendering.
- An identity Terrain is drawn with its real Transform. Culling uses the same test as URP.
- The trees and details of an identity Terrain are not hidden.

`RadialIdentityWarp` can also be added to renderers. For a renderer, it is removed from Scene BRG's automatic ownership and drawn through the per-renderer MaterialPropertyBlock path.

## Scene BRG

When a Terrain shader (TerrainLit, TerrainLitAdd, TerrainLitBase, TerrainLitBasemapGen) is assigned to a MeshRenderer, it is drawn with a Scene BRG sidecar. See [Scene BRG](../scene-brg/).
