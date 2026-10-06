---
title: Scene BRG
description: The path that draws MeshRenderers in loaded scenes together with BatchRendererGroup, and its readiness.
sidebar:
  order: 7
---

Scene BRG is Omni's rendering path that hands the MeshRenderers in loaded scenes to the GPU together, through Unity's BatchRendererGroup (BRG). It works in both standard and logarithmic rendering.

## Two backends

The Scene BRG tab of Omni Manager shows the current backend.

| Display | Details |
| --- | --- |
| BatchRendererGroup (product) | Draws with Scene BRG. The default |
| Legacy (verification) | Draws with the original renderers. Does not use Scene BRG |

While any Game View or Scene View camera draws with a URP Forward renderer (not Forward+), Omni does not use Scene BRG and draws with Legacy. Forward passes additional lights as a per-object list, which BRG objects cannot read. When that camera is gone, Scene BRG is rebuilt.

:::caution[Forward and lighting]
Legacy Radial uses conservative bounds. With Forward, the per-object choice of Reflection Probes and Light Probes therefore differs from standard rendering. The Light Probe space conversion is not applied either ([Lighting spaces](../lighting-spaces/)). For scenes that need the same lighting in Radial as in standard rendering, use a Forward+ renderer.
:::

## Which renderers are included

Scene BRG collects MeshRenderers in loaded regular scenes automatically, including inactive objects. Renderers with no material, and renderers that do not have a mesh yet, are not collected (they are included in a rebuild after the mesh exists).

The following renderers are not put on Scene BRG; they stay on the normal Radial MeshRenderer path. Only that renderer is excluded; Scene BRG for the whole scene does not stop.

| Condition | Reason |
| --- | --- |
| Has `RadialSceneBatchRendererExclusion` | An explicit marker to exclude it |
| Has a MaterialPropertyBlock set by something other than Omni (for example, a renderer whose `material.*` is written by an Animator) | BRG cannot represent it. The MaterialPropertyBlock is kept |
| Uses a material that is not a persistent asset (for example, a copy made by a script in Edit Mode) | A BRG material cannot be made for it |
| Has `RadialIdentityWarp` | Scene BRG has no way to switch a single instance to the identity mapping |
| Is under a hidden GameObject that is not saved | Not covered by automatic collection |

The MaterialPropertyBlock check runs when Scene BRG is built. A MaterialPropertyBlock added after the build is checked in the next rebuild.

### Renderers created at runtime

MeshRenderers saved in a scene are registered automatically. Register MeshRenderers that your code creates at runtime with `RadialBatchRendererRegistry`.

| API | Details |
| --- | --- |
| `Register(renderer, provider)` | Registers the renderer and returns a handle. `provider` is optional |
| `Unregister(handle)` | Removes the registration. The renderer returns to normal rendering and is also excluded from automatic collection until it is registered again |
| `MarkDirty(handle, flags)` | Applies only the changed inputs at the next safe update |

The `flags` of `MarkDirty` (`RadialBatchDirtyFlags`) are as follows. After changing `Renderer.enabled` or the GameObject's active state, notify `Visibility`.

| Flag | What changed |
| --- | --- |
| `Transform` | The Transform or the Logical Transform |
| `GeometryOrMaterial` | Mesh, submesh, material or material property |
| `Filtering` | Layer, shadow, lightmap, motion and similar settings |
| `Visibility` | `Renderer.enabled` or the GameObject's active state |
| `MeshVertexData` | Only the vertices and bounds of the same mesh |
| `MeshBinding` | Only the mesh reference of the same renderer (materials and so on stay the same) |
| `All` | Everything |

Pass your own per-instance values with `IRadialBatchInstanceDataProvider`. Only `float` and `Vector4` values can be passed. Textures cannot be changed per instance.

A non-persistent material assigned during Play to a registered renderer is drawn by converting it to a copy of the verified BRG shader. The values of the original material are copied to that copy every frame. If a MonoBehaviour in the scene implements `IRadialBatchMaterialDependencyProvider` and declares the source materials it assigns during Play, the BRG materials for them are also made in advance.

## BRG materials (sidecars)

Scene BRG draws with a BRG material (sidecar) made for each original material of a renderer.

- Sidecars are made in `Assets/Radial/BRG` per GUID of the original material, not per scene. Every scene that uses the same material shares them.
- As long as Scene BRG draws, sidecars are needed in both standard and logarithmic rendering. Legacy does not need them.
- Even when Omni Manager is not open, sidecars for loaded scenes are prepared automatically at the next Editor update after a scene opens or closes, an asset changes, an Undo, the start of Play, or a switch between standard and logarithmic rendering.
- When a Terrain shader (TerrainLit, TerrainLitAdd, TerrainLitBase, TerrainLitBasemapGen) is assigned to a MeshRenderer, it is also drawn with a sidecar whose inputs are converted for BRG.

## Readiness

Check the state in the "Scene BRG" row at the top of Omni Manager and in the Scene BRG tab.

| State | Meaning |
| --- | --- |
| Ready | The sidecars and manifest that the loaded scenes need are all present |
| Preparing… | Preparation is scheduled or running |
| Failed (Scene BRG updates stopped) | Preparation failed. Shows the stage, source material, shader, reason and affected renderers |
| Not required (Legacy backend) | Rendering does not use sidecars. The last preparation result is shown for reference |
| Not checked yet | Not checked yet in this Editor session |

While preparation has failed, Scene BRG does not take over or update renderers until it succeeds ([Fail Closed](../../concepts/fail-closed/)). This is a separate result from the material switch.

:::note
"Ready" means the sidecars are present. It does not mean the GPU accepted every draw. If Unity rejects a BRG draw, for example as "not SRP Batcher compatible", an error appears in the Console.
:::

### Manual actions

"Actions" in the Scene BRG tab has the following buttons. Both run at the next safe Editor update. They cannot run during Play or compilation.

| Button | Details |
| --- | --- |
| Recheck / regenerate loaded Scene sidecars | Checks the sidecars of the loaded scenes, regenerates missing and stale ones, and copies the values of the original material to every sidecar again. Also use it to reflect values of original materials that a script changed before saving |
| Generate for Build Settings Scenes… | Makes sidecars for the enabled Build Settings scenes and the loaded scenes, and rebuilds the manifest for that set. It takes longer because each build scene is opened as a preview |

### When Play starts

When Play starts (when leaving Edit Mode), the same check as during Play runs while still editing. If it does not pass, Omni saves and refreshes to settle the state and remakes the sidecars (up to 2 times). If it still does not pass, Play preparation stops with a reason. The reason shows the saved hash, the current hash, the sidecar's shader, instancing and the DOTS keyword.

## Culling and layers

- The camera's Culling Mask is respected. Unity does not filter BRG draws by Culling Mask, so Scene BRG's culling checks the layer and removes them.
- Only views of cameras with Radial disabled are culled with Unity's planes.
- Views of cameras with Radial enabled, and views other than cameras (shadows, picking, selection outlines), are not plane-culled; every object is submitted. Positions after logarithmic compression cannot be tested against planes.

## Reflection Probes and Light Probes

- When drawing a baked Reflection Probe (Baked, or Custom without Dynamic Objects), only Reflection Probe Static objects are drawn. Realtime probes, probes with Dynamic Objects, and Play mode draw every object.
- Light Probe interpolation uses the position in the space where the probes were baked, not the display position. For scenes where a root has been moved for display, see [Lighting spaces](../lighting-spaces/).
