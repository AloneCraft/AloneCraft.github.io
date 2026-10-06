---
title: Installation
description: Import Omni into your project and learn where folders and generated files go.
sidebar:
  order: 2
---

Omni is distributed as an asset folder under `Assets/Omni`, not as a UPM package.

## Steps

1. Prepare a project with the Unity, URP and Input System versions in [Requirements](../requirements/).
2. In Project Settings > Graphics, set a URP asset as the Render Pipeline Asset.
3. Import the Omni package.
4. Open the [Verification Hub](../verification/) and check rendering.

:::caution[If you use the demos]
SolarSystem and CosmicScale read the Earth images from `Demo/EarthToMoon` and the Moon images from `Demo/PlanetSurface`. Import the whole `Demo` folder when you use the demos. If you import only part of it, the Earth and Moon images will be missing.
:::

## Folders

| Folder | Contents |
| --- | --- |
| `Runtime` | Run-time code for Logical coordinates, Radial projection, rendering, picking and more |
| `Editor` | Scene View integration, settings UI, conversion and bake tools |
| `Converter` | Conversion of shaders, materials and VFX, and GUID management for generated files |
| `Resources` | Settings and rendering assets that Omni loads at run time |
| `Localization` | Translations of the Editor UI |
| `Demo` | Demo scenes and parts shared between demos (`PlanetSurface`, `Materials`) |
| `Verification` | The Hub for checking your environment, and per-feature scenes |
| `Radial/Universal Render Pipeline` | Pre-converted URP shaders and Shader Graphs |
| `Radial/Hlsl` | Included HLSL modified for Radial coordinates, depth and lighting |
| `Radial/Diagnostics` | The diagnostic shader used in place of materials that cannot be converted |

## Where generated files go

Radial versions of the standard URP shaders are included in the product. They are not regenerated in your environment.

When you convert your own shaders, Shader Graphs or VFX, Omni saves project-specific output to the following places. All of them are separate from the package you bought, so updating the package never overwrites them.

| Location | Contents |
| --- | --- |
| `Assets/Radial/User` | Your converted shaders and graphs |
| `Assets/Radial/Hlsl` | Your includes that needed conversion |
| `Assets/Radial/Mappings` | The ledger used to switch materials' shaders back |
| Elsewhere in `Assets/Radial` | Assets for bake, BRG and other management |
| `ProjectSettings/Omni/Generated.manifest.json` | GUIDs of generated files and their source assets |

:::tip[Version control]
`ProjectSettings/Omni/Generated.manifest.json` is project settings data, not a Unity asset. If your scenes reference generated materials, include this file in version control and backups. Even if the generated folders are deleted, the same GUIDs are restored on regeneration as long as this file remains.
:::
