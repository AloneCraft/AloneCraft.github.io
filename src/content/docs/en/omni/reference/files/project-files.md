---
title: Files Omni creates
description: The files Omni creates, reads and rewrites in your project, and how to treat them in version control.
sidebar:
  order: 1
---

Omni itself lives in `Assets/Omni`. This page lists the files Omni creates outside that folder, and the project files it rewrites. For installation, see [Installation](../../../docs/getting-started/installation/).

## Overview

| Location | Contents | Version control |
| --- | --- | --- |
| `Assets/Radial/` | Generated files such as converted shaders, managed materials, the restore ledger, bakes and Scene BRG sidecars | Commit |
| `ProjectSettings/Omni/Generated.manifest.json` | The mapping between generated GUIDs and source assets | Commit |
| `ProjectSettings/Omni/RenderingMode.json` | The chosen rendering mode (standard or logarithmic) | Commit |
| `ProjectSettings/OmniEditorSettings.asset` | Omni's Scene View settings | Commit if you want to share them |
| `Logs/Omni…` | Results and captured images from the verification scenes | Do not commit |
| `Library/OmniRadialVfxPatch.hash` | A marker that VFX assets were checked | Do not commit |

`Assets/Radial` and `ProjectSettings/Omni` are separate from Omni itself, so updating Omni does not overwrite them. Before updating Omni, put both in version control or a backup.

## Assets/Radial

The root for files Omni generates in your project. Materials under `Assets/Radial` are never targets of the standard/logarithmic switch, and are not counted in Omni Manager.

| Folder or file | Written by | Contents |
| --- | --- | --- |
| `User/` | Switching to logarithmic (shader generation) | Converted custom shaders and Shader Graphs. Keeps the path they had under `Assets/` or `Packages/` |
| `Hlsl/` | Switching to logarithmic (shader generation) | Custom includes (HLSL) that had to be converted. Keeps the original logical path |
| `Project/`, `Packages/` | Scene BRG preparation and others | Logarithmic versions of materials, made from source materials. Placed under `Project/` if the source is under `Assets/`, or `Packages/` if it is under `Packages/`, keeping the source path |
| `ManagedBuiltInMaterials/` | The switch (apply) | Managed materials: copies of materials that cannot be rewritten (Unity built-in, inside a package, Material Variants). Named `BuiltIn_<hash>.mat` |
| `DiagnosticMaterials/` | The switch (apply) | Diagnostic materials used in place of materials that cannot be converted. Named `Unsupported_<hash>.mat` |
| `Mappings/MaterialShaderMappings.asset` | The switch (apply and restore) | The restore ledger: each material's original shader and values, and the references that were replaced |
| `Bake/<scene name>.RadialBake.asset` | Bake | Bake data for each scene |
| `Resources/RadialBakeRegistry.asset` | Bake | The mapping from scene GUID to bake data. Loaded from `Resources` at runtime |
| `BRG/Materials/`, `BRG/Shaders/` | Scene BRG preparation | Sidecar materials and shaders per source material GUID (`<GUID>_BRG.mat`, `<GUID>_BRG.shader`) |
| `BRG/Hlsl/` | Scene BRG preparation | HLSL that converts the inputs of Terrain shaders for BRG |
| `BRG/Editor/RadialBrgMaterialManifest.asset` | Scene BRG preparation | The mapping between source materials and sidecars |
| `Diagnostics/` | "Export Report" in "Omni Shader Diagnostic" | Shader diagnostic results (`<shader name>.omni-shader-diagnostic.json`). Created only on request |

- Each time a switch to logarithmic completes, generated files whose source is confirmed gone and that nothing references are moved to the OS trash. Version control shows them as deleted. For what is removed and what is kept, see "Cleaning up generated files that lost their source" in [Shaders and materials](../../../docs/guides/shaders-and-materials/).
- Switching back to "Standard Rendering" points references back to the original materials and deletes the diagnostic materials. With the "Project-wide" scope, managed materials are deleted too. With the "Loaded Scenes" scope, managed materials are kept, because other scenes may still reference them.
- `Assets/Radial/Generated.manifest.json`, used by older versions, is read only for the first migration. Omni never rewrites or deletes it.

### Version control

Commit all of `Assets/Radial`, including the `.meta` files.

- Scenes and prefabs reference managed materials and diagnostic materials by GUID, and scenes reference bake data by GUID. Without these files, the references break.
- Without the restore ledger (`Mappings/`), "Standard Rendering" cannot restore your materials.
- Converted shaders and sidecars can be regenerated, but keeping the same GUIDs on regeneration depends on `Generated.manifest.json`. It is safer to keep generated files from the same point in time as the ledger and the scenes.
- You do not need to commit the diagnostic results in `Diagnostics/`.

## ProjectSettings/Omni/Generated.manifest.json

Records the path and GUID of each generated file under `Assets/Radial`, and its source asset. It is not a Unity asset, and Unity does not import it.

| Item | Details |
| --- | --- |
| Written by | Operations that generate files (shader and material generation, bakes, Scene BRG preparation and others) |
| Contents | JSON: `version`, and for each generated file its `path`, `guid`, and the source asset's `sourceGuid`, `sourceLocalId` and `sourceName` |
| Use | When a generated file is recreated, Omni writes a `.meta` with the recorded GUID first, so the same GUID comes back |
| Version control | Commit. Even if `Assets/Radial` is deleted, this file lets Omni regenerate with the same GUIDs |

Omni writes a temporary file in the same folder first, then replaces the manifest with it. If saving fails, the top of Omni Manager shows that the save is pending, with a retry button ([Omni Manager](../../editor/omni-manager/)). If the record for a newly generated file cannot be saved, that conversion is not treated as a success.

## ProjectSettings/Omni/RenderingMode.json

Records the rendering mode chosen with the standard/logarithmic switch.

| Item | Details |
| --- | --- |
| Written by | Completion of a switch, and the "Auto-add RadialCamera (opened Scenes / Play / Build)" checkbox in Omni Manager |
| Contents | JSON: `{ "mode": "Logarithmic" }` or `{ "mode": "Standard" }` |
| Use | While it is `Logarithmic`, `RadialCamera` is added automatically to the target cameras when a scene is opened, when Play starts, and when a Player is built |
| If the file is missing | Nothing happens (unset) |
| Version control | Commit. Without it, `RadialCamera` is not added automatically in other environments |

For when and to which cameras it is added, see "What is recorded" in [Switch between standard and logarithmic rendering](../../../docs/getting-started/switch-rendering/). For the API, see [OmniRenderingModeSettings](../../editor/omni-rendering-mode-settings/).

## ProjectSettings/OmniEditorSettings.asset

Omni's Scene View settings, shared across the project. Omni saves the file when a setting changes.

| Item | Details |
| --- | --- |
| Written by | The "Custom Navigation" section of "Controls and Display" in Omni's Scene View overlay, and the "RMB Rotation Basis" field |
| Contents | Settings such as flight speed, approach, wheel sensitivity, and the basis and inversion of right-button look, plus the Scene View defaults Omni uses |
| Version control | Commit if your team should share the same settings. It also holds personal preferences such as movement speed |

For the overlay settings, see [Scene View overlay](../../editor/scene-view-overlay/).

## Logs

Results from the verification scenes are saved under the project's `Logs` folder. `Logs` is Unity's log folder, so do not commit it.

| Folder | Written by |
| --- | --- |
| `Logs/OmniRenderingRegression/<Run ID>/` | "Capture" in the Verification Hub: the result JSON (`rendering-regression.json`), JUnit-compatible XML (`rendering-regression.xml`) and images |
| `Logs/OmniVfxModules/<Run ID>/` | Capture and scoring in the VFX module scenes |
| `Logs/OmniShaderGraphModules/<Run ID>/`, `Logs/OmniShaderGraphSamples/<Run ID>/` | Capture and scoring in the Shader Graph verification scenes |

When the Verification Hub runs in a Player, results go to `Application.persistentDataPath/OmniRenderingRegression`. For how to use it, see [Check with the Verification Hub](../../../docs/getting-started/verification/).

## Library

`Library/OmniRadialVfxPatch.hash` is a marker that Omni's current VFX patch has reached every VFX Graph asset in the project. If the marker is missing, or Omni's VFX patch has changed, Omni checks the VFX Graph assets once the Editor is idle and reimports the ones without the patch. `Library` is Unity's cache, so do not commit it.

## Project files Omni rewrites

Besides its generated files, Omni rewrites the following project files. Commit them as usual.

| File | Rewritten by | Change |
| --- | --- | --- |
| Materials under `Assets/` (`.mat`) | The standard/logarithmic switch | Changes the material's shader to the logarithmic version, or back, keeping the same `.mat` |
| Scenes, and other assets that reference materials (such as prefabs) | The standard/logarithmic switch | Replaces references to materials that cannot be rewritten with managed materials, and references to materials that cannot be converted with diagnostic materials. With "Loaded Scenes", the references in loaded scenes are replaced; with "Project-wide", the references in the files of assets that reference the material. "Standard Rendering" restores them |
| Scenes | The standard/logarithmic switch | Adds and enables `RadialCamera` on the target cameras (standard disables it). Recorded in Undo, and kept in the scene when you save |
| Scenes | Bake | Creates a `RadialBakeBinding` GameObject that references the bake data |
| URP Global Settings asset | Switching to logarithmic, and "Install Omni Shader" / "Restore URP Shader" in the Shaders tab | The shader references of the shared Camera MotionVectors and SSAO resources |

The `RadialCamera` that Omni adds automatically while `RenderingMode.json` is logarithmic is not written to scene files ([Switch between standard and logarithmic rendering](../../../docs/getting-started/switch-rendering/)).

## Before removing Omni

In Omni Manager, switch back to "Standard Rendering" at the top, and check that the target materials are back on their standard shaders. For details, see "Updating and removing Omni" in [Shaders and materials](../../../docs/guides/shaders-and-materials/).
