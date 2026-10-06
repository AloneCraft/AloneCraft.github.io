---
title: Shaders and materials
description: The bundled standard URP shaders, applying and restoring materials, and the Materials and Shaders tabs of Omni Manager.
sidebar:
  order: 2
---

Omni renders logarithmically by replacing the shader of each material asset with a Radial version. This page explains which shaders are covered, how materials change, and how they are restored. For the switch itself, see [Switch between standard and logarithmic rendering](../../getting-started/switch-rendering/).

## Bundled standard URP shaders

Radial versions of the standard URP shaders ship with the product. They were built with Unity `6000.6.0f1` and URP `17.6.0`, and live in `Assets/Omni/Radial/Universal Render Pipeline`. URP HLSL files that had to be changed live in `Assets/Omni/Radial/Hlsl`.

These are not regenerated in your environment. Import, Editor startup, Play and Player builds do not rebuild them either.

The material catalog registers 18 families: 17 families of hand-written shaders and URP's default Decal Shader Graph.

| Family | Original URP shaders |
| --- | --- |
| Mesh | Lit, ComplexLit, SimpleLit, BakedLit, Unlit |
| Particle | ParticlesLit, ParticlesSimpleLit, ParticlesUnlit |
| SpeedTree | SpeedTree7, SpeedTree7Billboard, SpeedTree8 |
| Terrain | TerrainLit, TerrainLitAdd, TerrainLitBase, TerrainLitBasemapGen |
| Spatial Mapping | SpatialMappingOcclusion, SpatialMappingWireframe |
| Decal | `Decal.shadergraph` (the graph used by URP's default `Decal.mat`) |

- The catalog maps each original shader and its Radial version in both directions by actual reference, not by name.
- A standard URP shader that is not in the catalog is never replaced by a guessed similar shader. The material, shader, scene, object, slot and reason are recorded in the restore ledger, and the slot shows diagnostics.
- Shader Graphs bundled with URP (AutodeskInteractive, SpeedTree8_PBRLit, SpeedTree9_URP and others) are not in the catalog. They are generated in your environment by the same converter as your own graphs. See [Shader Graph and custom shaders](../shader-graph/).

### Decal

The Radial Decal graph is `Assets/Omni/Radial/Universal Render Pipeline/Shaders/Decal.shadergraph`. Its shader name is `Omni/Radial/com.unity.render-pipelines.universal/Shaders/Decal`, which differs from URP's original `Shader Graphs/Decal`.

| Pass | Handling |
| --- | --- |
| Decal Projector passes | Vertices are not mapped. Only the position reconstructed from scene depth is inverse-transformed through Radial |
| Mesh passes (DBufferMesh and others) | Vertices are mapped, as in other graphs |

## Shaders that are not converted

The following shaders are not converted. Omni Manager shows them as "Conversion not required / Excluded".

| Shader | Reason |
| --- | --- |
| URP's standard 2D shaders (Sprite and Mesh2D Lit/Unlit, Sprite Mask, the VFX graphs for sprites) | 2D is outside logarithmic rendering. There is no Radial version |
| Hand-written Fullscreen/Blit shaders, Skybox | They have no depth ordering of object vertices |
| Shaders that implement logarithmic rendering themselves and have no standard version (such as `Omni/RadialGlobalLit`) | There is nothing to restore to and nothing to switch to |

- 2D shaders are identified by their exact path inside the package, not by name.
- Fullscreen/Blit and Skybox shaders are identified only by information that proves their use, such as every pass depending on Fullscreen, or a Skybox tag.
- A shader is not excluded just because its name contains `UI`, `2D`, `Sprite`, `Overlay` or `Screen`, or because it uses a transparent queue. Rendering that can be placed in 3D space, such as a World Space Canvas or TextMeshPro, must keep depth ordering, so it is converted as usual.
- Fullscreen Shader Graphs (fog and similar) are converted. See [Shader Graph and custom shaders](../shader-graph/).

### How 2D is displayed

2D materials stay standard.

- While a RadialCamera is enabled, 2D renderers under `Omni.Logical.Transform` are hidden for safety, on that camera only.
- 2D renderers not under `Omni.Logical.Transform` are drawn as ordinary Unity renderers, at their real distance. The logarithmic mapping is not applied.
- A Player build where a RadialCamera's camera uses the 2D Renderer stops.

:::tip[Showing 2D far away]
Draw the 2D content to a RenderTexture with a separate camera that has no RadialCamera, and composite it in the main camera.

- Limit the 2D camera's `cullingMask` to a layer used only for 2D.
- Do not put the 2D objects under `Omni.Logical.Transform`. Place them near the 2D camera.
- Composite with a Fullscreen pass (not converted by Radial) or a screen-fixed quad.

Camera Stacking cannot be used, because URP does not render when the Base and Overlay renderers are of different types. There is no depth ordering against 3D content.
:::

## Applying materials

Writable ordinary materials are not duplicated. Only the shader is switched, keeping the same `.mat` and the renderer slot references. Applying keeps the following values:

- Textures with Scale/Offset, colors, numbers
- Surface, Alpha Clip, Blend, Cull, ZWrite
- Local keywords, passes, instancing, GI
- Render queue (both default and explicit)

For materials that cannot be written, Omni makes a managed copy (managed material).

| Material | Handling |
| --- | --- |
| Unity built-in materials, materials inside packages | A copy is made in `Assets/Radial`, and references in scenes and elsewhere are redirected to the copy |
| Material Variants (inheriting the shader from a parent) | An independent copy with the parent removed is made, and references are redirected |

Applying again changes nothing. When the same unconvertible material is used in several slots, they share one diagnostic material, and the ledger records its link to the original material.

### Target scope

Switch the scope with "Target scope:" at the top of Omni Manager.

| Scope | Targets |
| --- | --- |
| Loaded Scenes | Materials in all loaded scenes, including inactive objects. Preview scenes, the Prefab Stage and Editor-only temporary objects are not included |
| Project-wide (writable Assets) | Only independent, writable `.mat` files under `Assets/` |

- In the project-wide scope, materials in `Packages/`, materials embedded in models and read-only materials are counted separately as read-only or embedded exclusions, with reasons.
- `Assets/Radial` holds Omni's generated files, so it is neither switched nor counted in either scope.
- After switching to logarithmic in the project-wide scope, scenes that were not open also show logarithmic rendering when you open them. When you switch in the loaded-scenes scope, materials used only by other scenes stay standard.

## Restoring materials

Choosing "Standard Rendering" at the top restores from the ledger.

- A value Omni changed is restored only if its current value is the same as right after applying.
- Shared properties that you changed after applying are kept. Keyword, pass and queue settings that still make sense with the original shader are also kept.
- A material using a bundled Radial shader is switched back to the standard shader in place, through the catalog's reverse mapping.
- Slots replaced by a diagnostic material are pointed back to the original material.
- "Standard Rendering" in the project-wide scope points every reference back to the original material and also deletes the copies and their ledger records. In the loaded-scenes scope, the copies are kept, because other scenes may reference them.

When the meaning cannot be kept, for example because the original shader is gone, Omni stops before changing anything. A custom shader that supports logarithmic rendering but has no established mapping to a standard shader shows, in the details, that it cannot be restored to standard, with the reason.

Applying through the final check is one unit of change. If an unexpected exception occurs, the cameras, materials, diagnostic references and ledger that were changed are restored in the same Undo group. When materials merely cannot be converted (diagnostic display), they are not rolled back automatically, so that you can see where they are used. Restore them explicitly with "Standard Rendering". After a domain reload, an Editor restart or reopening a scene, the diagnostic state is found again from the ledger.

### The restore ledger and automatic cleanup

The restore ledger is `Assets/Radial/Mappings/MaterialShaderMappings.asset`.

The ledger does not know about changes made outside Unity (overwriting folders, git checkout, deleting `Assets/Radial`). So, whenever you switch to standard or logarithmic, Omni compares the ledger with the current materials and cleans it up before analysis.

| Found state | Cleanup |
| --- | --- |
| The material is back on its original shader | Delete only the record, and convert again as usual |
| The logarithmic shader is gone (missing, or an error shader) | Return to the original shader and state, delete the record, and rebuild during the switch |
| The material switched to another logarithmic shader | Keep the original information, and update the logarithmic shader in the record |
| The material switched to some other shader | Keep your shader, delete the record, and convert again with the current shader as the new original |
| The material is gone | Delete the record |
| A managed material is missing or does not match | Point references back to the original material, and delete the managed material and the record (rebuilt on the next apply) |

- The number of cleaned-up records appears at the top of the Manager, and the details appear in the Console.
- An apply that ended midway is left to recovery at startup, and is not cleaned up.
- The material list does not rewrite the ledger. Mismatched records are shown with the same classification as after cleanup, and the details show that a stale switch record exists.

## Materials tab

This tab only shows diagnostics. It has no buttons to generate, apply or restore; those are combined into the switch at the top.

- The list updates automatically after opening or closing scenes, Hierarchy changes, material and shader changes, Undo/Redo, and the end of Play.
- The list is the union of materials used in all loaded scenes, including inactive objects. While scenes are switching, it shows the previous list and "Applying Scene changes...".
- There is one row per material reference. Different materials with the same name have separate rows.
- Non-asset runtime materials owned by VFX Graph are not in this list. They are checked separately through the VFX readiness state.

| Column | Contents |
| --- | --- |
| Material | The material |
| Shader in use | The shader currently assigned to the material |
| Current Shader state | "Standard", "Logarithmic", "Conversion error / Diagnostic display active", "Conversion not required / Excluded", "Missing reference" and others |
| Usage | How many times it is used |

When you select a row, the details show the asset path, the source and logarithmic shaders, the logarithmic version status, the generation source, whether it can be restored, and the scenes, objects and slots that use it.

### Logarithmic version status

| Category | Status |
| --- | --- |
| Custom / Standard | "Not generated", "Generated / Not in use" |
| Custom / Logarithmic | "In use" |
| URP / Standard | "Bundled / Not in use" |
| URP / Logarithmic | "In use (bundled)" |
| BuiltinRP / Standard | Can be switched only if an exactly matching logarithmic shader exists |
| Diagnostic | "Conversion error / Diagnostic display active" |
| Conversion not required / Excluded | Fullscreen/Blit confirmed to have no object space, or Skybox |

The status can also be "Update required", "In use / Update required", "Generating...", "Generation failed", "Conversion unsupported", "Manual edits / Review required" or "Missing reference". "In use" means that a logarithmic shader is assigned to the material.

Use "Display:" to narrow the list to "All", "Needs preparation" or "Problems".

### Relation to the state at the top

"Standard Rendering / Logarithmic Rendering" at the top reflects only the shader configuration of the materials in the selected scope. The readiness of cameras, VFX and renderers is handled by the checks and warnings during the switch.

- It becomes "Logarithmic" when there are zero standard and zero unresolved materials, and every remaining one is logarithmic or shows diagnostics.
- When one or more show diagnostics, it shows "Logarithmic (warnings N)".
- If standard materials remain after a switch, it is not complete. The top shows the stage that failed (pre-check, preparation, apply, post-check), the target and the reason.
- A diagnostic shader is never counted as supported, even though it implements the Radial coordinate transform.

## Shaders tab

This tab shows the shaders used by loaded scenes ("Used in Scene") or by all of `Assets` ("All Assets"), and the state of their logarithmic versions. The switch at the top does the generation, so there is no generate button.

| Status | Meaning |
| --- | --- |
| Done | Generated, or in the bundled catalog |
| Native | The shader implements the Radial transform itself and needs no generation (for example `StencilOnly.shader`). As in the switch, this is decided by the declaration of `_RadialGlobal_UseRadialTransform` |
| None | Not generated yet |
| Stale | The generated file differs from the current generator's output |
| Invalid | It references a shader with the same name as a bundled logarithmic shader but a different GUID |

"Show only shaders that need generation" narrows the list to the not-generated and stale ones.

### Camera MotionVectors and SSAO

The "Camera MotionVectors / SSAO" foldout shows whether URP's shared resources (the full-screen Camera MotionVectors shader and the SSAO shader) are Omni's versions.

- Omni's versions convert logarithmic depth back to logical positions before computing. Without them, Player builds of scenes that contain a RadialCamera stop.
- They are connected automatically when a switch to logarithmic completes. For cameras without a RadialCamera, Omni's versions compute the same as standard, so they are not removed when you switch back to standard.
- If a value other than URP's default is set, it is not overwritten. The reason appears as a warning at the top of the Manager.
- With "Install Omni Shader" and "Restore URP Shader" in the foldout, you can connect them manually or return to URP's defaults. The result appears in the tab's log.

## BuiltinRP materials and the limits of automatic support

`GUI/Text Shader` and `Legacy Shaders/Diffuse` are classified as BuiltinRP, not as custom shaders. The BuiltinRP classification does not mean supported. Without an exactly matching logarithmic shader, they cannot be converted.

- Materials in `Library/unity default resources` are not passed to the custom shader generator.
- A built-in material used by a saved object in a real scene can become a managed material only if an exact logarithmic shader can be determined from its original shader.
- Without an exact mapping, Omni shows the material name, shader, objects, slots and reason, and replaces the uses with `Omni/Radial/Diagnostics/UnsupportedConversion`.
- Editor-only temporary rendering is out of scope.

The diagnostic shader draws yellow and black diagonal stripes with a red X, double-sided, opaque and with depth writes. You can tell it apart from the magenta that Unity uses for missing shaders.

The following are outside normal automatic switching:

- Materials in read-only packages, materials embedded in models (excluded with a reason in the project-wide scope)
- Unsupported renderers
- Shaders that cannot be converted, built-in materials without an exact mapping
- Conflicts with hand-edited generated files
- Missing shader or material references
- Switching during Play

Omni never treats these as success by silently excluding them or replacing them with similar shaders. Conversion failures that can be restored go to diagnostic display; items that cannot be restored stop before anything changes. If a scene in the build still has diagnostic slots or an unrestored diagnostic ledger, the build stops with the original shader name and the reason ([Fail closed](../../concepts/fail-closed/)).

:::tip[Registering a substitute for a shader that cannot be converted]
Fixed-function and Built-in-only shaders can sometimes be converted by registering a substitute URP shader. See "Registering a substitute shader" in [Shader Graph and custom shaders](../shader-graph/).
:::

## Bake optimization

The logarithmic URP shaders are bundled, so rendering works without baking. Baking is a performance optimization that prepares caches and generated data in advance.

- After checking the rendering, the switch to logarithmic bakes only the scenes that need it.
- A failed bake does not turn the rendering switch into a failure. The result is reported as the logarithmic switch complete and the optimization incomplete.
- You can also bake manually with "Bake Active Scene" and "Bake All Loaded Scenes" in the "Bake Optimization" tab. Unloaded scenes are excluded.
- Baking needs a valid RadialCamera.

Check the readiness of Scene BRG sidecars in the "Scene BRG" tab. See [Scene BRG](../scene-brg/).

## Checking

`Assets/Omni/Verification/Functions/Shaders` contains scenes for checking shaders one at a time. Open the list from `Functions/FunctionsVerification.unity`.

| Scene | Contents |
| --- | --- |
| `Functions/Shaders/Modules` | One scene per shader for the 17 hand-written shaders in the bundled catalog. The left is the original URP shader, the center is Radial identity, and the right is the bundled Omni shader (Radial logarithmic). There are also negative examples of shaders that cannot be converted |
| `Functions/Decal.unity` | The catalog's Decal graph. Check that the positions of the projected pattern and the floor pattern do not change |
| `Functions/Shaders/Graphs` | Shader Graphs bundled with URP, made Radial. The Radial versions are generated in your environment when the scene is opened |

For the particle shaders, also check the Particle System in Play Mode. After saving the scene, check that the materials' shader references have not changed.

:::caution[Large triangles]
When one triangle spans into the range where the mapping bends strongly (beyond a logical distance of about 1/*M*), per-vertex mapping breaks its coverage and depth, and it can cover nearby objects. An example is a floor made from Unity's standard Plane scaled 50 times (each triangle is about 50 m). Build such surfaces subdivided. For *M*, see [The Radial mapping and View Scale](../../concepts/radial-mapping/).
:::

## Updating and removing Omni

### Cleaning up generated files that lost their source

When you delete or move an original shader or material, only the matching generated files remain in `Assets/Radial`. Each time a switch to logarithmic completes, Omni moves the following generated files to the OS trash and deletes their records. No confirmation dialog appears.

- Targets: generated shaders, generated materials, BRG sidecars and managed materials whose source is confirmed gone and that nothing references
- Kept: files still referenced from scenes or prefabs, and files whose source cannot be determined. The count and reason appear in the Console and at the top of the Manager
- Not touched: bake data, `Resources`, the material mapping ledger and similar

### Before updating or removing

- Before updating Omni, put `Assets/Radial` and `ProjectSettings/Omni/Generated.manifest.json` under version control or back them up.
- You do not need to rebuild the product's `Radial/Universal Render Pipeline` and `Radial/Hlsl` in your environment.
- Before removing Omni, switch back to "Standard Rendering" at the top of the Manager, and check that the target materials are back on their standard shaders.
