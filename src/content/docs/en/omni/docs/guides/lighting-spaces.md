---
title: Lighting spaces
description: Use baked Light Probes and APV at the correct positions even in scenes where a display root has been moved.
sidebar:
  order: 6
---

Omni's Radial Lit computes lighting at physical positions, not at positions after the mapping. Baked data such as Light Probes, APV, Lightmaps and Reflection Probes, on the other hand, is made in Unity's finite coordinates. When you move a root for display, the display coordinates and the coordinates at bake time no longer match. The components on this page make the correspondence between those two spaces explicit in the scene.

## What Omni keeps for lighting

- Lighting is computed at physical positions. The mapping keeps view directions, so within the range that standard rendering can show, the lighting, shadows and lightmaps of the Radial image are the same as standard rendering.
- Reflection and view directions are not taken from interpolated values; they are recomputed per pixel from the position on the physical surface.
- SSAO is computed from the physical surface position and depth, not from the mapped position on screen.
- The ShadowCaster and Meta passes are not deformed by the Radial mapping; they stay in physical space.
- A Lit path that cannot receive correct physical lighting data does not draw ([Fail Closed](../../concepts/fail-closed/)).
- The mapping, depth and lighting data do not change with quality level or build target.

:::note[SSAO]
URP's standard SSAO reads the depth after the mapping as if it were physical depth, so it cannot be used as is with a Radial camera. In the Shaders tab of Omni Manager, check that the SSAO shader is connected to Omni's version ([Check with the Verification Hub](../../getting-started/verification/)).
:::

## Which component to use

| Baked data | Component | Transform it can map |
| --- | --- | --- |
| Light Probes (Light Probe Groups) | `OmniLightProbeSpace`, `OmniLightProbeSpaceRequirement` | The display root's Transform (translation, rotation, scale) |
| APV (Adaptive Probe Volumes) | `OmniAdaptiveProbeVolumeSpace`, `OmniAdaptiveProbeVolumeSpaceRequirement` | Translation only |
| Converting between Logical positions and baked-space positions | `OmniLightingSpace` | A conversion used from scripts |

In a scene where the display coordinates and the baked coordinates are the same (a scene whose roots have not been moved), add nothing. Omni does not add unnecessary conversions to such scenes.

## Light Probe space

`OmniLightProbeSpace` maps a root moved for display to the Light Probe space baked near the origin.

1. Add `OmniLightProbeSpace` to the display root that you move after baking.
2. In the Inspector, set Baked Space Root to the root used when the Light Probes were baked. If you baked at Unity's world origin, leave it empty.

Renderers drawn by Scene BRG use the position in the baked space, not the display position, for Light Probe interpolation. That position is converted by the `OmniLightProbeSpace` on the renderer's parent: local coordinates under the display root are read as coordinates under Baked Space Root.

- The interpolation position is the renderer's origin. If the renderer has a `probeAnchor`, its position is used without conversion.
- Renderers with Light Probe usage set to Off are not affected.

`OmniLightProbeSpaceRequirement` stores in the scene a marker that says "this root needs an explicit mapping to the baked space". Add it to the same GameObject as `OmniLightProbeSpace`. The expected Baked Space Root is not shown in the Inspector. It is set by the automatic fix below, or by the Editor API `Omni.Editor.BatchRendering.OmniLightProbeSpaceAuthoring.TryEnsureContract`.

:::caution[The Legacy path]
The Legacy path keeps Unity's standard Light Probe interpolation and does not apply the `OmniLightProbeSpace` conversion. In a scene where the display root has been moved, the lighting of objects lit by Light Probes changes. While any camera draws with a URP Forward renderer (not Forward+), Omni does not use Scene BRG and draws with Legacy. For scenes that need the same lighting in Radial as in standard rendering, use a Forward+ renderer ([Scene BRG](../scene-brg/)).
:::

### Automatic diagnosis

The Editor diagnoses the active scene when a scene is opened, and after scene changes, Undo, completion of a lightmap bake and similar events. It covers scenes that have an enabled RadialCamera and are drawn by Scene BRG.

When it finds a problem, it logs a warning to the Console and opens the "Omni Light Probe Space" window. For the same state, the window opens automatically only once per Editor session.

| Status | Meaning |
| --- | --- |
| Not applicable | The diagnosis conditions are not met (no RadialCamera or Scene BRG, no baked Light Probes, and so on) |
| Valid | The interpolation positions are within the range of the baked probes |
| Missing (safely fixable) | A mapping is missing, and there is exactly one way to fix it |
| Missing candidate (needs manual check) | A mapping may be missing, but the root to add it to cannot be narrowed down to one |
| Contract violation | The added components contradict each other. Including this scene in a Player stops the build |

For "Missing (safely fixable)", you can fix it with the "確認してLight Probe空間を追加…" (confirm and add Light Probe space) button. It adds `OmniLightProbeSpace` and `OmniLightProbeSpaceRequirement` to the candidate root, with Unity's world origin as the baked space. It is a single operation that can be undone, and the scene is not saved automatically. This fix is offered only when returning a single top-level root to the world-origin space puts every interpolation position within the probe range.

For "Missing candidate (needs manual check)", nothing is added automatically. Check the baked space and the display root, and set up the components yourself.

To diagnose again, press Validate in the Inspector of the `OmniLightProbeSpace` on the display root. Validate only reads; it does not change the scene.

### Build checks

When building a Player, Omni checks the following for each scene. It does not stop Play from starting.

| Result | Handling |
| --- | --- |
| Contract violation | Stops the build |
| Missing, missing candidate | Logs a warning and continues the build |

A contract violation occurs, for example, when:

- There is an `OmniLightProbeSpaceRequirement` but no `OmniLightProbeSpace`
- The expected Baked Space Root does not match the Baked Space Root of `OmniLightProbeSpace`
- Baked Space Root is in another scene
- The display root cannot be converted to a finite position in the baked space

## APV space

`OmniAdaptiveProbeVolumeSpace` maps the translation of the display root through URP APV's official `ProbeVolumesOptions.worldOffset`. Rotation and scale cannot be represented.

| Condition | Details |
| --- | --- |
| Same GameObject | `OmniLightProbeSpace` and `Volume` are required (they are added automatically when you add the component) |
| Volume | Enabled, Global, Weight 1, Blend Distance 0, and Priority set to the reserved maximum (`float.MaxValue`) |
| Transform | The display root and the baked root have the same rotation and scale |
| Count | Only one enabled `OmniAdaptiveProbeVolumeSpace` in the loaded scenes (worldOffset is one value for the whole camera) |
| Baked root | In the same scene as the display root |

While the conditions are not met, worldOffset is not applied. During Play, the reason is logged to the Console once. Only a runtime copy of the Volume profile is changed. The shared profile asset is not changed.

To set all of this up at once, use the Editor API `Omni.Editor.BatchRendering.OmniAdaptiveProbeVolumeSpaceAuthoring.TryEnsureContract`. It also sets up the Light Probe space mapping on the same root, and sets the Volume values to the conditions above. The scene is marked unsaved but not saved. It does not set anything up, and returns the reason, when:

- The display root is inactive
- The display root already has a Volume for another purpose
- Another enabled `OmniAdaptiveProbeVolumeSpace` exists in the loaded scenes
- The rotation or scale of the display root and the baked root differ

In a scene that has `OmniAdaptiveProbeVolumeSpaceRequirement`, a Player build checks the conditions above and the combination of components. If they are not met, the build stops.

## Converting between Logical positions and the baked space

`OmniLightingSpace` maps an `Omni.Logical.Transform` used as the Logical origin (Logical Origin) to the origin of a finite baked space (Lighting Space Root; itself if not set).

| API | Details |
| --- | --- |
| `TryToLightingPosition` | Converts a Logical position to a finite position in the baked space |
| `TryToLogicalPosition` | Converts a position in the baked space back to a Logical position |

It does not convert huge absolute values to `float`; it takes the difference from the Logical origin first and then converts to finite values. When a conversion is not possible, it returns `false`.

## Reflection Probes

A baked Reflection Probe (Baked, or Custom without Dynamic Objects) captures only Reflection Probe Static renderers, and its shadows come only from static casters. Scene BRG draws with the same rule. Realtime probes, probes with Dynamic Objects, and Play mode capture every renderer.

In a scene where the display root has been moved, bake Reflection Probes while Scene BRG is drawing. On the Legacy path, the Light Probe lighting changes.

## Checking

`Assets/Omni/Verification/Functions/Lighting` contains scenes for checking Lightmaps, Realtime Shadows and Reflection Probes. These scenes do not switch to a dedicated URP asset; they render with the URP asset active in your project.

| Scene | Required URP asset settings |
| --- | --- |
| `Lightmaps.unity` | Light Probe System set to "Light Probe Groups", with Mixed Lighting enabled |
| `ReflectionProbes.unity` | Reflection Probe Blending and Box Projection enabled |

Missing settings are reported as a warning in the Console when the scene is opened. Capturing stops and returns that reason.
