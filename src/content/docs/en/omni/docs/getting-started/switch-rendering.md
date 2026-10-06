---
title: Switch between standard and logarithmic rendering
description: Switch scenes to logarithmic rendering with Omni Manager, and compare during Play.
sidebar:
  order: 4
---

## Open Omni Manager

Open Omni Manager with the "Omni Manager" checkbox in Omni's Scene View overlay. The checkbox and the Manager window stay in sync.

## Switch to logarithmic rendering

The "Standard Rendering / Logarithmic Rendering" switch at the top of Omni Manager is the normal way to switch. Choose the targets with "Target scope:" at the top.

| Target scope | Targets |
| --- | --- |
| Loaded Scenes | All loaded scenes |
| Project-wide (writable Assets) | The writable assets in the project |

In both scopes, `RadialCamera` is added to the cameras of the loaded scenes.

When you choose logarithmic rendering, Omni does the following in one run.

1. Analyze cameras, renderers, terrains, decals, VFX and materials
2. Generate only the custom shaders and VFX that are needed
3. Re-check that the generated files compile and meet the logarithmic rendering contract
4. Only after every target is ready, add and enable `RadialCamera` on cameras, and apply logarithmic shaders to materials
5. Check cameras, materials, VFX and the restore ledger, and update the completion state
6. Bake only the scenes that need it, for optimization

Choosing logarithmic is not by itself a success. The "Current:" field at the top shows "Processing..." while processing, and the actual state only after the check.

| Display | Meaning |
| --- | --- |
| Standard | Every target is standard |
| Logarithmic | Every target material is logarithmic, and every target camera in the loaded scenes has an enabled `RadialCamera` |
| Logarithmic (warnings N) | Logarithmic, but N materials are shown as diagnostics (see below) |
| Mixed / Error | Standard and logarithmic are mixed, or there is a problem |

:::caution[Materials are shared]
The switch rewrites material assets. Shader changes can therefore also affect unloaded scenes and prefabs that use the same material. The details of the Materials tab show the objects and slots that use the material in loaded scenes. Uses in unloaded scenes and prefabs are not shown there.
:::

### When it stops, and when it shows diagnostics

- **Stops before applying**: problems where a safe restore cannot be guaranteed, such as a Prefab Stage, a conflict in the restore ledger, a missing diagnostic shader, or a failure to write an asset.
- **Continues with diagnostics**: conversion failures where the reference to the original material can be kept safely, such as no exact logarithmic shader, or a failed generation or compile. Valid targets become logarithmic, and only the slots that cannot be converted are replaced with a diagnostic material. The result is `CompletedWithConversionErrors`, which is not success. "Current:" at the top becomes "Logarithmic (warnings N)", and the Materials tab shows "Conversion error / Diagnostic display" for the affected materials. Choosing "Standard Rendering" restores them.

### What is recorded

When the switch finishes, the chosen rendering mode is recorded in `ProjectSettings/Omni/RenderingMode.json` (`Logarithmic` or `Standard`).

A `RadialCamera` added to a camera is a scene change, so it is lost unless saved. While logarithmic is recorded, Omni therefore adds and enables `RadialCamera` on the target cameras automatically at these times:

- When a scene is opened, after a domain reload, and when you return to editing after Play
- When Play starts, and when a Player is built

An automatically added `RadialCamera` is not recorded in Undo, does not mark the scene as unsaved, and is not written to the scene file.

The cameras of `RadialObserverRig` and hidden cameras that are not saved are excluded. If an observer camera had a `RadialCamera`, that camera would itself become the center of the mapping, and observation would not work.

## Compare during Play

In a scene switched to logarithmic rendering, add `Omni/Radial Camera Toggle` (`RadialCameraToggle`) to a camera that has a `RadialCamera`. You can then switch between standard and logarithmic rendering from the same viewpoint during Play and in the Player.

| Control | Details |
| --- | --- |
| On-screen button | Appears at the top left (can be hidden in the Inspector) |
| Key | F8 by default (when Input System is present; can be changed or disabled in the Inspector) |
| API | `SetRadialEnabled(bool)`, `Toggle()`, and the `RadialEnabledChanged` event |

Only `RadialCamera` is enabled or disabled. When it is disabled, Radial materials draw with the identity mapping and look the same as standard rendering.

:::note
While `RadialCamera` is enabled, renderers under `Omni.Logical.Transform` whose materials do not support Radial are hidden for safety. In a scene that still uses standard materials, switch to logarithmic rendering in Omni Manager first.
:::

## Switch in the Scene View

In Omni's Scene View overlay you can switch each view between standard and Radial. Choose "Standard Camera" in the `RadialCamera` field to show that view normally, or choose a `RadialCamera` to show it in Radial with that camera's settings. Multiple Scene Views do not share their selection.
