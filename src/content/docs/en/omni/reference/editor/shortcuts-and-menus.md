---
title: Shortcuts and Menus
description: The shortcuts, GameObject menu items, Add Component paths and Create menu items that Omni adds.
sidebar:
  order: 4
---

This page lists the shortcuts and menu items that Omni adds to the Unity Editor. Menu paths and shortcut IDs stay in English regardless of the Editor language.

## Shortcuts

These entries appear in Edit > Shortcuts (the Shortcut Manager). You can change their keys in the Shortcut Manager.

| ID | Default key | Context | Action |
| --- | --- | --- | --- |
| `Omni/Frame Logical Selection` | F | Scene View | Frames the selected target in Logical coordinates |
| `Omni/Editing/OrbitLeft` | None | Scene View | Rotates the viewpoint 15° to the left while keeping the focus point and distance |
| `Omni/Editing/OrbitRight` | None | Scene View | Likewise, 15° to the right |
| `Omni/Editing/OrbitUp` | None | Scene View | Likewise, 15° up |
| `Omni/Editing/OrbitDown` | None | Scene View | Likewise, 15° down |
| `Omni/Editing/Closer` | None | Scene View | Multiplies the distance to the focus point by 5/6. In orthographic, multiplies the view size by 1/1.2 |
| `Omni/Editing/Further` | None | Scene View | Multiplies the distance to the focus point by 6/5. In orthographic, multiplies the view size by 1.2 |
| `Omni/Editing/Restore` | None | Scene View | Goes back one step to the viewpoint before you focused with F |

`Omni/Editing/*` are the same operations as the "View Editing" buttons in the overlay's "Controls and Display". For details, read [Scene View Overlay](../scene-view-overlay/#view-editing).

### When they work

None of the shortcuts work while:

- the Scene View does not have keyboard focus
- a text field is being edited
- a mouse drag is in progress
- Unity's own flythrough is in progress

`Omni/Frame Logical Selection` works in standard display, Objective and Subjective. Because it is a Scene View context shortcut, it takes priority over Unity's global shortcut on the same F key (Frame Selected in Window under Cursor). F in the Hierarchy, Edit > Frame Selected and double-clicking in the Hierarchy also lead to the same Logical framing.

`Omni/Editing/*` have these additional conditions.

| ID | Condition |
| --- | --- |
| All | Radial display with Viewpoint set to "Subjective" |
| `OrbitLeft`, `OrbitRight`, `OrbitUp`, `OrbitDown`, `Restore` | Not in 2D mode and rotation is not locked |
| `Restore` | There is focus history |

`Omni/Editing/*` have no default keys so that Unity's existing keys take priority. When you assign keys, choose combinations that do not overlap existing ones.

## GameObject menu

| Path | Action |
| --- | --- |
| `GameObject/Omni/Radial Observer Camera` | Places the observer camera rig prefab (`Assets/Omni/Runtime/Observer/Prefabs/RadialObserverRig.prefab`) in the scene |

- If Cinemachine is not installed, a dialog asks whether to install `com.unity.cinemachine` from the Package Manager. After installing it, wait for scripts to recompile, then choose the same menu item again.
- If the menu was opened on a GameObject, the rig is placed as its child. Otherwise it is placed in the active scene. In that case, if the scene has a component named `SceneMetaData` with its `Root` set, the rig is placed as a child of `Root`.
- You can undo the placement. After placing, the rig is selected.

For usage, read [Radial observer camera](../../../docs/guides/observer-camera/).

## Opening windows

Omni adds no user-facing items to the Window menu. Open the Omni Manager window with the "Omni Manager" check in the Scene View overlay ([Scene View Overlay](../scene-view-overlay/#omni-manager)).

Omni adds no items to the right-click menu of components in the Inspector (`CONTEXT/...`).

## Add Component menu

The following components appear under `Omni` in Add Component.

| Path | Component | Requires |
| --- | --- | --- |
| `Omni/Logical Transform` | [`Omni.Logical.Transform`](../../components/logical-transform/) | — |
| `Omni/Radial Camera Toggle` | [`RadialCameraToggle`](../../components/radial-camera-toggle/) | — |
| `Omni/Radial Observer Rig` | [`RadialObserverRig`](../../observer/radial-observer-rig/) | — |
| `Omni/Radial Observer Slot` | [`RadialObserverSlot`](../../observer/radial-observer-slot/) | — |
| `Omni/Radial Observer Orbit` | [`RadialObserverOrbit`](../../observer/radial-observer-orbit/) | Cinemachine 3.0.0 or later, Input System 1.0.0 or later |
| `Omni/Radial Observer Channel Guard` | [`RadialObserverChannelGuard`](../../observer/radial-observer-channel-guard/) | Cinemachine 3.0.0 or later, Input System 1.0.0 or later |

`RadialObserverOrbit` and `RadialObserverChannelGuard` are compiled only when both Cinemachine and Input System are installed. If either is missing, they do not appear in the menu.

The following components do not appear under `Omni`. Add them by typing the type name into the Add Component search field.

| Component | Category |
| --- | --- |
| [`RadialCamera`](../../components/radial-camera/) | Components |
| [`RadialIdentityWarp`](../../components/radial-identity-warp/) | Components |
| [`RadialRigidAnchor`](../../components/radial-rigid-anchor/) | Components |
| [`RadialSceneBatchRendererExclusion`](../../components/radial-scene-batch-renderer-exclusion/) | Components |
| [`OmniLightingSpace`](../../lighting/omni-lighting-space/) | Lighting |
| [`OmniLightProbeSpace`](../../lighting/omni-light-probe-space/) | Lighting |
| [`OmniLightProbeSpaceRequirement`](../../lighting/omni-light-probe-space-requirement/) | Lighting |
| [`OmniAdaptiveProbeVolumeSpace`](../../lighting/omni-adaptive-probe-volume-space/) | Lighting |
| [`OmniAdaptiveProbeVolumeSpaceRequirement`](../../lighting/omni-adaptive-probe-volume-space-requirement/) | Lighting |
| [`BakePlaybackController`](../../sequencer/bake-playback-controller/) | Sequencer |

## Create menu

The following assets appear under `Omni` in the Project window's Create menu (Assets > Create).

| Path | Asset | Default file name |
| --- | --- | --- |
| `Omni/Celestial/Catalog` | [`CelestialCatalogAsset`](../../celestial/celestial-catalog-asset/) | `CelestialCatalog` |
| `Omni/Radial Shader Substitutes` | `OmniRadialShaderSubstitutes` | `OmniRadialShaderSubstitutes` |

`OmniRadialShaderSubstitutes` is a table that registers URP shaders to use in place of shaders that Omni cannot convert. For usage, read [Shader Graph and custom shaders](../../../docs/guides/shader-graph/).
