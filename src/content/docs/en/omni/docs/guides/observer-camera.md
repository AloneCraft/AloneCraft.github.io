---
title: Radial observer camera
description: Pull back with a separate camera to view the warped world that a RadialCamera sees.
sidebar:
  order: 8
---

`RadialObserverRig` is a rig for pulling back with a separate camera to view the "warped world" that a `RadialCamera` sees. It brings the idea of the Scene View's "Objective" comparison display ([Working in the Scene View](../scene-view/)) to Game View and Player cameras.

## What you need

| Package | Version |
| --- | --- |
| `com.unity.cinemachine` | 3.0.0 or later |
| `com.unity.inputsystem` | 1.0.0 or later |

Cinemachine is not a required package for Omni. Without Cinemachine, the parts of the observer camera that use Cinemachine are excluded from compilation.

## Placing it in a scene

1. Choose GameObject > Omni > Radial Observer Camera.
2. If Cinemachine is not installed, a dialog asks whether to install it from the Package Manager. After installing, wait for scripts to recompile, then choose the same menu again.
3. The rig prefab (`Assets/Omni/Runtime/Observer/Prefabs/RadialObserverRig.prefab`) is placed in the scene.

By default, the rig runs only in Play Mode.

## Structure

```text
RadialObserverRig       RadialObserverRig (registration, frame placement)
 ├─ Focus               The look-at point shared by all frames
 └─ Slot_*              5 frames: Orbit, Front, Right, Top, Iso
     ├─ Camera          Camera, UniversalAdditionalCameraData, CinemachineBrain
     └─ VCam            CinemachineCamera, OrbitalFollow (Sphere), RotationComposer,
                        RadialObserverOrbit, RadialObserverChannelGuard
```

The rig's origin is a finite rendering space that corresponds to the warp center (the position of the observed `RadialCamera`). The rig itself does not move. Following the center is done on the Logical side.

## Rig settings

These are the Inspector items of `RadialObserverRig` (Add Component: `Omni/Radial Observer Rig`).

| Item | Details | Default |
| --- | --- | --- |
| Source | The `RadialCamera` whose warp center and curve are used. If not set, the one on the MainCamera is used; if there is none, an enabled `RadialCamera` in the scene is searched for | None |
| Distance In Scale | The distance from the center to the camera, as a multiple of the Radial scale S (5–200). S is the constant 1 (`RadialSettings.RadialScale`), so this value is the Unity distance in meters | 40 |
| Layout | How the frames are arranged | `OrbitWipe` |
| Corner | The screen corner the frames are placed against (`BottomRight`, `BottomLeft`, `TopRight`, `TopLeft`) | `BottomRight` |
| Orbit Wipe Area | The side length of the orbit frame in `OrbitWipe`, as a ratio of the screen width and height | 0.3 |
| Grid Area | The side length of the area used by `FixedGrid` and `Combined`, as a ratio of the screen width and height | 0.6 |
| Base Depth | The base Camera depth of the observer frames. Make it larger than the main camera's | 10 |
| Include Unwarped Culling Volume | Also includes the normal view frustum in observer culling (the same setting as the Scene View comparison display) | On |
| Run In Edit Mode | Registers and shows the observer frames in Edit Mode too | Off |

Because the distance is proportional to the scale S, the view looks the same regardless of the value of S.

### Layouts

| `Layout` | Details |
| --- | --- |
| `OrbitWipe` | One small orbit frame in a corner of the screen. For finding anomalies that change over time |
| `FixedGrid` | The 4 fixed frames (Front, Right, Top, Iso) large in a 2×2 grid. For static checks per angle and image comparison |
| `Combined` | A large orbit frame, with the 4 fixed frames stacked vertically beside it |

The frames stay within the screen and do not overlap.

### Per-frame settings

In each frame's `RadialObserverSlot`, set the kind (`Orbit` or `Fixed`), the horizontal angle (Yaw) and elevation (Pitch) of a fixed frame, and the field of view (Field Of View, 50 by default).

## Controls

| Frame | Movement |
| --- | --- |
| Orbit frame | Orbits automatically (12°/s by default). Right-drag inside the frame to rotate by hand, and use the wheel to zoom. It returns to orbiting 3 seconds after you stop |
| Fixed frames | The angle is fixed |

The near and far clip distances are set automatically from the camera distance.

## Rules

- Do not add a `RadialCamera` to an observer frame's camera. If you do, that camera itself becomes the warp center, and you cannot see the main camera's warped world. A camera that has one is not registered, and a warning is logged to the Console. Omni Manager's switch to logarithmic rendering also does not add `RadialCamera` to observer frame cameras.
- Do not put `Omni.Logical.Transform` under the rig. It would write the same Transform as Cinemachine.
- Do not use orthographic cameras. Radial is always the identity mapping in orthographic projection, so the warped world cannot be seen. The fixed-angle frames are also perspective.

### Cinemachine channels

Each observer frame's Virtual Camera is placed on its own Output Channel (Channel01–05), and the frame's Brain listens only to that channel. That channel is removed from the other Brains in the scene at the moment the rig is enabled (and restored when it is disabled). This prevents the main camera's Brain from picking up an observer Virtual Camera and moving to the observer position.

:::caution
Brains created after the rig is enabled are not affected. In that case, remove the observer channels from that Brain's Channel Mask.
:::

## When frames do not appear

While the observed `RadialCamera` is disabled, destroyed or not found, the registration is removed and all frame cameras are disabled (the frames disappear).

| Situation | Display |
| --- | --- |
| No `RadialCamera` found | A warning in the Console |
| `RadialCamera` is disabled | A warning in the Console, and "Radial is OFF - enable it (F8) to show the observer view." at the bottom right of the screen |

While Radial is turned off with `RadialCameraToggle`, there is no warped world to observe, so the frames also disappear ([Switch between standard and logarithmic rendering](../../getting-started/switch-rendering/)).

## Running in Edit Mode

Only rigs with `Run In Edit Mode` turned on register and show frames in Edit Mode. A rig with it off changes nothing in Edit Mode. Turning it off in Edit Mode removes the registration and hides the frames.

In Edit Mode, updates happen only when the Editor's player loop runs. To run every frame, call `EditorApplication.QueuePlayerLoopUpdate` from your side.

:::caution[What is verified in 0.1]
Omni 0.1 verifies frame placement, layouts, registration and the prefab structure. The GPU rendering result of the observer cameras (such as matching the Scene View's Objective display) has not been verified yet.
:::

## Demo

In the CosmicScale demo, the O key shows an overview in the lower half of the screen. It uses `RadialObserverRig` and a Cinemachine camera to look down on the Radial-compressed world from above the main camera.
