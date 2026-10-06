---
title: Check with the Verification Hub
description: Check rendering, depth, scale, VFX and cameras in your project with one set of controls.
sidebar:
  order: 3
---

`Assets/Omni/Verification/Hub.unity` is the entry point for checking rendering, depth, scale, VFX, ECS/BRG and camera state in a project where Omni is installed, all with the same controls.

## What you need

- The Hub renders with Omni's own URP asset (`Verification/Settings/OmniVerification_URP.asset`). It switches the active URP asset to this one only during Play and switches it back when Play ends. It does not change your project's Quality and Graphics settings.
- The ECS paths need `com.unity.entities`. Without it, these paths report `Unsupported` with a reason.
- The paths that draw VFX need `com.unity.visualeffectgraph`. Without it, these paths report `Unsupported` with a reason.

## How to use it

1. Open `Hub.unity` and enter Play Mode.
2. Choose what to check with **Case**. The series are `DEP`, `SHD`, `DEC`, `BUF`, `MOT`, `ALP`, `LIT`, `SCL`, `VFX`, `ECS`, `CAM` and `ED`.
3. Switch scale, distance, camera conditions and the fixed time step with **Preset**.
4. Switch between the GameObject, ECS/Omni BRG and VFX paths with **Execution Path**.
5. Compare Forward and Forward+ with **Renderer Request**. The status shows the requested renderer and the renderer actually selected, separately.
6. Control the deterministic clock with **Play**, **Pause**, **Rewind** and **Step**.
7. **A** turns on every required feature, and **B** turns off only the selected diagnostic feature, so you can compare them at the same time.
8. **Capture** saves the current image and a result JSON.

Results are saved to the project's `Logs/OmniRenderingRegression` in the Editor, and to `Application.persistentDataPath/OmniRenderingRegression` in the Player.

## Reading results

| Status | Meaning |
| --- | --- |
| `Passed` | The independent measurements defined for the case are within tolerance |
| `Unsupported` | The selected renderer or execution path cannot be checked in the current environment |
| `Failed` | The fixture, rendered samples, renderer selection or a measured value does not meet the conditions |

:::caution
Do not judge by the image alone. Also check `status`, `reason`, the requested and actual renderer, and the number of rendered samples (`renderedSampleCount`). An image where only the background matches is not evidence of a pass.
:::

## Checking in a Player

Before building the Hub into a Player, do the following.

1. Switch to logarithmic rendering in Omni Manager ([Switch between standard and logarithmic rendering](../switch-rendering/)).
2. In the "Camera MotionVectors / SSAO" section of Omni Manager's Shaders tab, check that the Camera MotionVectors and SSAO shaders are connected to Omni's versions. The switch in step 1 connects them automatically. However, if a value other than the URP default has been set, it is not overwritten. In that case, check the warning at the top of the Manager and press "Install Omni Shader" in that section. Without this, building a scene that contains a RadialCamera stops with the reason.
3. In Project Settings > Quality, add a quality level and set its Render Pipeline Asset to `Verification/Settings/OmniVerification_URP.asset`. URP strips shader variants by looking only at the URP assets of the build target's quality levels. Without this, variants for features of the dedicated asset that the Hub switches to during Play (such as Decal) are stripped, and Decals appear magenta in the Player.
4. Add `Hub.unity` to Build Settings and build.

In the Player, use the panel at the top left of the screen.

## Per-feature scenes

`Verification/Functions` contains scenes for narrowing down Hub results to small setups. Open the list from `Functions/FunctionsVerification.unity`. Each scene can be opened on its own and checked in both Scene View and Play. Start overall checks from the Hub.
