---
title: Omni Manager
description: The Editor window that switches between standard and logarithmic rendering and shows the state of materials, shaders, bakes and Scene BRG.
sidebar:
  order: 2
---

Omni Manager is the Editor window that switches between standard and logarithmic rendering and shows the state of what the switch covers. For the switch workflow, see [Switch between standard and logarithmic rendering](../../../docs/getting-started/switch-rendering/). For how materials change and what each state means, see [Shaders and materials](../../../docs/guides/shaders-and-materials/).

## Opening it

Open it with the "Omni Manager" checkbox at the top of Omni's Scene View overlay.

- The checkbox and the window stay in sync. Clearing the checkbox closes every open Omni Manager window.
- There is no Unity menu item or shortcut that opens Omni Manager.
- While Omni Manager is not ready yet, for example right after the Editor starts, the checkbox cannot be used.

For the overlay, see [Scene View overlay](../scene-view-overlay/).

## Window layout

From top to bottom, the window has three parts.

| Part | Details |
| --- | --- |
| "Material Rendering Switch" section | The rendering switch and the current state. Shown whichever tab is selected |
| Tab bar | "Materials", "Shaders", "Bake Optimization", "Scene BRG" |
| Tab content | What the selected tab shows and does |

The refresh button at the right end of the tab bar (tooltip "Refresh current tab") reloads the list of the selected tab. After a scene is opened or closed, the Hierarchy or Project changes, Undo/Redo, or Play ends, Omni Manager reloads the "Materials", "Shaders" and "Bake Optimization" tabs automatically.

The display language follows the Unity Editor language (Japanese or English).

## "Material Rendering Switch" section

| Row | Details |
| --- | --- |
| Target scope: | Choose "Loaded Scenes" or "Project-wide (writable Assets)". For what each covers, see "Target scope" in [Shaders and materials](../../../docs/guides/shaders-and-materials/) |
| Rendering mode: | The "Standard Rendering" and "Logarithmic Rendering" radio buttons. Choosing one starts the switch |
| Current: | The actual state (table below) |
| State: | Material counts in the target scope, split into "Standard", "Logarithmic", "Diagnostic", "Unresolved" and "Excluded" |
| Target: | With "Loaded Scenes", the number and names of the loaded scenes. With "Project-wide", the number of writable materials under `Assets` and the number excluded |
| Cameras ・ Materials | The state of cameras and material slots in loaded scenes (see below) |
| Scene BRG: | The readiness of Scene BRG. For what each state means, see [Scene BRG](../../../docs/guides/scene-brg/) |
| Orphaned generated assets: | The result of cleaning up generated assets whose source was lost, after the last switch to logarithmic. Not shown if there was nothing to clean up |
| Auto-add RadialCamera (opened Scenes / Play / Build) | The checkbox for the recorded rendering mode (see below) |
| Cleaned up N stale switch record(s) | How many stale records in the restore ledger the last switch cleaned up. Not shown if there were none |

### Rendering mode radio buttons

- After you click, the switch runs on the next Editor update.
- The radio buttons cannot be used during Play, while compiling, while assets are updating, while a switch is running, or while the list is being rebuilt after the target scope changes.
- When standard and logarithmic are mixed, neither radio button is shown as selected.
- "Logarithmic Rendering" is shown as selected only when every target material is logarithmic (including diagnostics) and every target camera in the loaded scenes has an enabled `RadialCamera`.
- With "Project-wide", choosing "Logarithmic Rendering" opens the "Confirm Project Material Switch" dialog. Check the counts of change targets, diagnostic candidates before generation, and read-only or embedded exclusions, then press "Continue" to start.
- With "Project-wide", the cameras in the loaded scenes are also switched after the materials. Logarithmic adds and enables `RadialCamera`; standard disables it.

### "Current:" values

| Display | Condition |
| --- | --- |
| Processing... | A switch is running |
| Scanning... | The list is being rebuilt after the target scope changed |
| Logarithmic | Every target material is logarithmic, and every target camera in the loaded scenes has an enabled `RadialCamera` |
| Logarithmic (warnings N) | Same as "Logarithmic", but N materials show diagnostics |
| Standard | Every target material is standard |
| Mixed / Error | None of the above. This is also shown when there are no target materials, and when runtime materials were detected so the state cannot be determined |

### Cameras and materials row

It has the form `Cameras a/b ・ Materials c/d ・ VFX …`.

| Part | Details |
| --- | --- |
| Cameras a/b | Cameras with an enabled `RadialCamera` / target cameras |
| Materials c/d | Slots that actually use a logarithmic shader / target slots |
| (N unwarped) | Slots that point to a generated asset but reference a shader that does not map. Shown only when N is 1 or more |
| VFX | The VFX backend, and ready / required assets |
| (N camera(s) disabled) | Cameras that have a `RadialCamera` that is disabled. Shown only when N is 1 or more |

### Warnings and errors

Below the section, the following appear only when needed.

| Display | Details |
| --- | --- |
| Cannot switch during Play Mode | Play is running. The tooltip gives the reason |
| ⚠ N unresolved | Some slots could not resolve their source material. The tooltip lists up to 10 reasons |
| ⚠ VFX …: ready a/b | The VFX backend is not ready. The tooltip gives the reason |
| Switched to logarithmic rendering. Materials that cannot be converted are shown with the diagnostic Shader: … | The switch completed, but materials that cannot be converted were replaced with diagnostics |
| Rendering switch did not complete at …: … | The switch did not complete. Shows the stage ("pre-check", "preparation (analysis and Shader generation)", "apply" or "post-check"), the target and the reason. If the reason has several lines, open the full text with "Details" |
| Scene BRG preparation failed | Shows the stage, the source material and the reason. "Open Scene BRG tab" moves to the "Scene BRG" tab |
| Camera MotionVectors and SSAO warnings | Why the switch to logarithmic could not connect URP's shared resources to the Omni versions |
| GUID manifestの保存は保留中です。… | Saving `ProjectSettings/Omni/Generated.manifest.json` is pending. The "manifest保存を再試行" button retries the save. This warning and button are shown in Japanese in every language |
| Orphaned generated assets (warning) | Some assets were kept because they are referenced, or could not be deleted. The paths and referrers are in the Console |

The switch failure and warning messages are cleared when a scene is opened or closed, when the active scene changes, and when the target scope changes.

### Automatic RadialCamera

"Auto-add RadialCamera (opened Scenes / Play / Build)" shows the rendering mode recorded in `ProjectSettings/Omni/RenderingMode.json`. It is checked when logarithmic is recorded.

- When a switch completes, Omni records the mode automatically. A switch to logarithmic turns it on; a switch to standard turns it off.
- Checking it immediately adds and enables `RadialCamera` on the target cameras of the loaded scenes.
- Clearing it removes the `RadialCamera` components that Omni added automatically, and disables again the ones it enabled automatically. A `RadialCamera` added by the switch (saved in the scene) stays. "Standard Rendering" is what disables it.

For when and to which cameras it is added, see "What is recorded" in [Switch between standard and logarithmic rendering](../../../docs/getting-started/switch-rendering/). For the API, see [OmniRenderingModeSettings](../omni-rendering-mode-settings/).

## Materials tab

This tab shows the state of the materials the switch covers. It has no buttons to generate, apply or restore; the switch at the top does those.

| Part | Details |
| --- | --- |
| Display: | Filter the list with "All", "Needs preparation" or "Problems" |
| Search field | Filter by text contained in the asset path or the shader name |
| Warnings | Shown when runtime materials are detected, when some materials cannot finish the logarithmic switch automatically, and while scene changes are being applied ("Applying Scene changes...") |
| Switch target | What the scope covers (the number and names of scenes, or the number of materials and exclusions), and the counts of "Renderer", "Materials", "Cameras" and "Radial Cameras" in the loaded scenes. "Radial Cameras" turns green when every camera has a `RadialCamera` |
| Material List | Columns "Material", "Shader in use", "Current Shader state" and "Usage" |
| Details | For the selected row: "Asset path", "Source Shader", "Logarithmic Shader", "Logarithmic version status", "Generation source", notes on restore, exclusion and problems, and the usage in loaded scenes ("Scene / Object / Slot") |

- Clicking a material name highlights that material in the Project window.
- Double-clicking a row opens the source material and the matching Omni-side material (such as a managed material) side by side.
- Drag the border between the list and the details to change the height of the details.

"Shader in use" has two lines: the category and the shader name.

| Category | Details |
| --- | --- |
| URP / Standard, URP / Logarithmic | A URP shader, and its bundled logarithmic version |
| Custom / Standard, Custom / Logarithmic | Your own shader, and its logarithmic version |
| BuiltinRP / Standard | A BuiltinRP shader |
| Conversion error / Diagnostic display | Could not be converted, and was replaced with a diagnostic material |
| Conversion not required / Excluded | A shader that logarithmic rendering does not cover |
| Missing reference | The shader reference is missing |

For the values of "Current Shader state" and "Logarithmic version status", see "Materials tab" in [Shaders and materials](../../../docs/guides/shaders-and-materials/).

## Shaders tab

This tab shows shaders and the state of their logarithmic versions. It has no generate button; the switch at the top does the generation.

| Part | Details |
| --- | --- |
| Diagnostics | Opens the "Omni Shader Diagnostic" window (see below) |
| Used in Scene / All Assets | Choose the shaders used by loaded scenes, or all shaders in `Assets` |
| Show only shaders that need generation | Show only "None" (not generated) and "Stale" |
| Search field | Filter by text contained in the shader path |
| Camera MotionVectors / SSAO | A foldout that shows whether URP's shared resources are the Omni versions (see below) |
| Log | Results of the buttons in the foldout. "Clear" empties it |
| Scene Summary | "Total", "Hand-Written", "ShaderGraph", and counts per state ("Generated", "Not Generated", "Needs Regeneration", "Invalid Reference", "Skipped") |
| Shader List | Columns "Status", "Source Shader", "Relative Path" and "Usage" |

- The "Status" values are "Done", "Native", "None", "Stale", "Invalid" and "Skip". For their meaning, see "Shaders tab" in [Shaders and materials](../../../docs/guides/shaders-and-materials/).
- Clicking a name in "Source Shader" highlights that shader in the Project window.
- The "S" and "D" buttons in "Relative Path" highlight the source shader and the converted shader in the Project window.
- Double-clicking a row opens the source shader and the converted shader side by side.
- Selecting an "Invalid" row shows the reason, the "Current Shader" and the "Expected Shader".

### Camera MotionVectors / SSAO

The foldout header shows "Omni version" or "Not installed" for each of Camera MotionVectors and SSAO. When a switch to logarithmic completes, Omni connects them automatically.

| Button | Details |
| --- | --- |
| Install Omni Shader | Connects URP's shared resource to the Omni version of the shader |
| Restore URP Shader | Returns it to URP's standard shader |

The Camera MotionVectors row and the SSAO row each have these two buttons.

### Omni Shader Diagnostic

This window analyzes the selected shader, or the shader of the selected material. It analyzes again when the selection changes. Nothing is sent outside. The window looks the same whatever the Editor language is.

| Button | Details |
| --- | --- |
| Refresh | Analyzes the selected shader again |
| Open Offline Guide | Opens `Assets/Omni/Documentation/ShaderAndMaterialWorkflow.md`, which ships with the product |
| Export Report | Writes the result to `Assets/Radial/Diagnostics/<shader name>.omni-shader-diagnostic.json` |
| Copy Japanese AI Prompt | Copies instructions in Japanese for fixing the shader to the clipboard |

## Bake Optimization tab

This tab runs bakes and shows the bake state of the active scene. Baking is an optional performance optimization; standard and logarithmic rendering work without it. For details, see "Bake optimization" in [Shaders and materials](../../../docs/guides/shaders-and-materials/).

| Part | Details |
| --- | --- |
| Bake Active Scene | Bakes the active scene. Fails if there is no valid `RadialCamera` |
| Bake All Loaded Scenes | Bakes every loaded scene. Unloaded scenes are excluded |
| Active Scene | The bake result of the active scene: the number of target renderers (split into NearField / MidField / FarField), the number of far-field groups, the maximum estimated reflection probe error, the warp settings at bake time, and the bake data asset |
| Select | Highlights the bake data in the Project window |
| Log | Bake results. "Clear" empties it |

## Scene BRG tab

This tab shows the readiness of Scene BRG sidecars, and lets you request a recheck or regeneration. For what each state means and how sidecars work, see [Scene BRG](../../../docs/guides/scene-brg/).

| Part | Details |
| --- | --- |
| Backend: | The current backend: "BatchRendererGroup (product)" or "Legacy (verification)" |
| Status: | "Ready", "Preparing…", "Failed (Scene BRG updates stopped)", "Not required (Legacy backend)" or "Not checked yet" |
| Actions | Two buttons (table below). They cannot be used during Play or while compiling |
| Last check | The number, time, trigger and scope of the last check, the number of materials, the number of updated sidecars, the backend at the time of the check, and the scenes |
| Failure | Shown only on failure: "Stage", "Source material", "Source shader", "BRG base shader", "Reason" and "Affected renderers". "Ping" highlights the source material in the Project window |

| Button | Details |
| --- | --- |
| Recheck / regenerate loaded Scene sidecars | Checks the sidecars of the loaded scenes, rebuilds missing and stale ones, and copies the values of the source material into every sidecar again |
| Generate for Build Settings Scenes… | After a confirmation dialog, creates sidecars for the enabled Build Settings scenes and the loaded scenes, and rebuilds the manifest for that set. This takes longer because each build scene is opened as a preview |

Neither button runs immediately; the request runs on the next safe Editor update. While it waits, "A request is waiting for the next safe editor update." is shown.

The trigger in "Last check" is one of "Omni Manager", "Build Settings generation", "Play Mode start", "Automation" or "Automatic".

## Related pages

- [Switch between standard and logarithmic rendering](../../../docs/getting-started/switch-rendering/)
- [Shaders and materials](../../../docs/guides/shaders-and-materials/)
- [Scene BRG](../../../docs/guides/scene-brg/)
- [Files Omni creates](../../files/project-files/)
