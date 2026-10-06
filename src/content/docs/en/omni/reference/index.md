---
title: Omni reference
description: Omni's public API, Editor controls, shader contracts, and files.
---

This is the list of Omni's public API. The lists of types and members (signatures, attributes, default values) are generated automatically from the Omni source. The descriptions are written by hand.

For workflows, read the [guides](../docs/guides/scene-view/). For how it works, read the [concepts](../docs/concepts/logical-coordinates/).

## Categories

| Category | Contents |
| --- | --- |
| Components | Components you add to GameObjects, such as `RadialCamera`, `RadialCameraToggle` and Logical Transform, and related types |
| Lighting | Components that link the baked space to the displayed space (Light Probes, APV, Lightmaps, Reflection Probes) |
| Observer camera | `RadialObserverRig`, which looks at the warped world from outside, and its parts |
| Logical types | Numeric types such as the 192-bit fixed-point `Scalar`, the position `Position3` and the rotation `Quaternion` |
| Unit types | Fixed-point types with units, such as angle, time, angular velocity, mass and energy |
| Sequencer | `PoseBake`, which bakes Logical poses and plays them back in Timeline, and its playback system |
| Celestial catalog | A catalog of the size, mass and orbit of celestial bodies |
| Editor | The rendering mode record (`OmniRenderingModeSettings`), Omni Manager, the overlay, shortcuts and menus |
| Shader | Include files, Shader Graph Custom Functions, global properties and defines |
| Files | Files that Omni creates, and project settings files |

## What is included

- The reference covers only the types intended for users. Types used inside Omni are not included, even when they are `public`.
- Members include the `public` ones and the saved fields shown in the Inspector (those with `[SerializeField]`).
- Omni 0.1's public API may change in future versions.

## A note on names

The `Omni.Logical` namespace has types with the same names as Unity types (`Transform`, `Vector3`, `Quaternion`). In a file that uses both, an alias such as `using L = Omni.Logical;` keeps them apart.
