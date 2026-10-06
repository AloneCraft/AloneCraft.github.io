---
title: Fail closed
description: Where Omni stops on paths it cannot confirm render correctly, and what to do then.
sidebar:
  order: 4
---

When Omni cannot confirm that a path renders correctly, it stops instead of showing a plausible image. This is called failing closed.

Even when the image looks right, depth, lighting, culling or picking may be breaking the spatial contract. Omni does not present such a state as success. It shows why it stopped and keeps the original state.

## Where it stops

| Stage | How it stops |
| --- | --- |
| Player build | Stops the build at the start, or while processing a scene, with the reason |
| Switching to logarithmic rendering | Stops before changing anything. Or puts only the parts that cannot be converted into a diagnostic display and does not report success |
| Rendering | Does not draw objects that cannot be drawn correctly |
| Input and picking | Does not treat a result it could not confirm as an empty click |

## What stops a Player build

The build error states the reason. In most cases it also names the camera and scene, and how to fix it.

### Build target and graphics API

These are checked when building a scene that contains a RadialCamera. Radial shaders need Shader Model 4.5 (equivalent to OpenGL ES 3.1). The support table is in [Requirements](../../getting-started/requirements/).

| Condition | What to do |
| --- | --- |
| Android, iOS, tvOS, visionOS | Build for Windows, macOS, Linux, or the web with WebGPU |
| On the web, Auto Graphics API is on, or there is an API other than WebGPU | In Player Settings > Web > Other Settings, turn off Auto Graphics API and keep only WebGPU |
| OpenGL ES 3.0 is in the graphics API list | Remove it from the list |
| OpenGL Core is used on macOS | Use Metal |

A Player has no way to switch materials back to URP's original shaders. Turning Radial off at runtime does not fix the display, so the build is stopped up front.

### URP renderers

| Condition | What to do |
| --- | --- |
| Any URP asset referenced by the build target's quality levels has a Deferred or Deferred+ renderer | Set that renderer to Forward+ (or Forward) |
| The RadialCamera's camera uses the 2D Renderer | Use a Universal Renderer set to Forward or Forward+. Draw 2D with a separate camera that has no RadialCamera |
| The RadialCamera's camera renderer is not a Universal Renderer, or cannot be resolved | Use a Universal Renderer set to Forward or Forward+ |
| A Render Objects override material or shader is not compatible with Scene BRG | Fix the override as the error describes |

The Deferred check covers not only the renderer the camera uses but every renderer in those URP assets. If there is even one Deferred renderer, URP puts the GBuffer pass of every shader into the Player, even if it is never used.

### Connecting Omni's shaders

| Condition | What to do |
| --- | --- |
| The Camera MotionVectors shader is not connected to Omni's version | Press "Install Omni Shader" in the Camera MotionVectors section of Omni Manager's "Shaders" tab |
| SSAO is active and the SSAO shader is not connected to Omni's version | Press "Install Omni Shader" in the SSAO section of the same tab |
| The VFX used by a Radial scene is not ready | Check the state and reason in the error and fix the VFX conversion ([VFX Graph](../../guides/vfx/)) |

The switch to logarithmic rendering connects Camera MotionVectors and SSAO automatically. If a value other than the URP default has been set, it is not overwritten, so connect it by hand.

### Scene setup

| Condition | What to do |
| --- | --- |
| A scene with an active RadialCamera and a scene with a DOTS SubScene go into the same Player | Split them into separate Players. Omni 0.1 does not render DOTS / Entities Graphics content logarithmically |
| On a display root with `OmniLightProbeSpaceRequirement`, the Light Probe space contract is not met | Revalidate with "Validate" in the Omni Light Probe Space inspector on the display root ([Lighting spaces](../../guides/lighting-spaces/)) |
| On a display root with `OmniAdaptiveProbeVolumeSpaceRequirement`, the APV space contract is not met | Check `OmniAdaptiveProbeVolumeSpace` and `OmniAdaptiveProbeVolumeSpaceRequirement` on the display root |

For Light Probe spaces, the build stops only for roots that have a Requirement. Mismatches that are only inferred stay as warnings.

### Every Player build

The following checks run on every Player build of a project with Omni installed, whether or not it contains a RadialCamera.

| Condition | What to do |
| --- | --- |
| The current render pipeline is not URP | Use a URP asset in the Graphics settings |
| A diagnostic display from conversion errors has not been restored | Choose "Standard Rendering" in Omni Manager to restore it |
| Strip Unused Variants in URP Global Settings is off | Turn it on |
| The variants of one shader pass and stage exceed 8,192, or the total exceeds 100,000 | Align settings across quality levels ("Quality settings" in [Requirements](../../getting-started/requirements/)) |

## What stops the switch to logarithmic rendering

The full flow of the switch is in [Switch between standard and logarithmic rendering](../../getting-started/switch-rendering/).

| Kind | Example | Result |
| --- | --- | --- |
| Cannot guarantee a safe restore | Prefab Stage, a conflict in the restore ledger, a missing diagnostic shader, a failed asset write | Stops before changing anything |
| DOTS / Entities Graphics | A loaded scene has a SubScene or Entities Graphics renderable entities | Stops before changing anything |
| A material that cannot be converted | No logarithmic shader, or generation or compilation failed | Only that slot is replaced with a diagnostic material, and the result is `CompletedWithConversionErrors` |

The diagnostic material has yellow and black diagonal stripes with red X marks. The reference to the original material is kept. The top of Omni Manager shows "Conversion error / Diagnostic display active" with a count.

The converter does not convert a Lit path into which it cannot inject physical lighting data. For a shader, this is treated as a conversion failure. For VFX, that VFX is marked unsupported with a reason, and the Player build stops.

## What is not drawn

| Target | Not drawn when | What you see |
| --- | --- | --- |
| A Renderer under a Logical Transform | A RadialCamera is active and the material does not support Radial | A warning in the Console. Generate and apply it in Omni Manager's "Materials" / "Shaders" tabs |
| VFX outside the safety range | On non-Radial cameras, in shadows, and in Decal outputs | Not drawn |
| Camera MotionVectors | On the first frame, or when the previous frame's data is missing | Velocity is set to zero |
| A Logical Transform in a forbidden hierarchy placement | Logical Transform → plain Transform → Logical Transform | An error in the Console. It stops composing with its parent |

The hierarchy rules are in [Logical coordinates and proxies](../logical-coordinates/).

## Picking and input

Scene View picking returns one of three results: `Hit`, `Miss` or `Unavailable`.

| Result | Meaning |
| --- | --- |
| `Hit` | An object was hit |
| `Miss` | Checked, and nothing was hit |
| `Unavailable` | Could not be checked |

- Only a real `Miss` without modifier keys requests clearing the selection.
- `Unavailable`, input over UI, and a `Miss` with modifier keys do not clear the selection.
- When the hover display cannot be produced, the result is `Unavailable` and no outline is shown.
- A middle-button drag does not start on `Unavailable`.

The detailed controls are in [Working in the Scene View](../../guides/scene-view/).

## What errs on the side of drawing everything

Failing closed is for cases where correctness cannot be confirmed. Decisions that only reduce the amount of drawing go the other way: they avoid removing objects that should be visible.

- For views from a camera with Radial active, and for shadows, picking and selection outlines, Scene BRG does not remove objects by culling planes. Positions after the mapping cannot be tested against planes.
- The hardware near is chosen so that nothing beyond the logical near is clipped ([Near and far](../near-far/)).

## When something stops

1. Read the reason shown in the build error, the Console, or at the top of Omni Manager. It names the camera, scene, renderer or material.
2. Fix the setting as the tables above describe.
3. Build again, or switch again.

You can check that the display is correct after the fix with the [Verification Hub](../../getting-started/verification/).
