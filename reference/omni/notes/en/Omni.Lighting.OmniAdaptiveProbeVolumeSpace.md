---
description: "A component that reflects the translation of a display root in URP's APV worldOffset."
---
`OmniAdaptiveProbeVolumeSpace` maps the translation between a display root and the space where APV (Adaptive Probe Volumes) was baked, using URP's official `ProbeVolumesOptions.worldOffset`. Rotation and scale cannot be expressed.

- The same GameObject needs [OmniLightProbeSpace](../omni-light-probe-space/) and a `Volume`. They are added automatically when you add the component. The root of the baked space is the Baked Space Root of `OmniLightProbeSpace`.
- While enabled, it evaluates the worldOffset and writes it to the `Volume` on every `Update`. It also works in Edit Mode. While the conditions are not met, it removes the worldOffset override.
- It changes only the runtime copy of `Volume.profile`. The shared Profile asset is not changed.
- When the conditions stop being met during Play, it logs the reason to the Console as an error. The same reason is not logged twice in a row.

The conditions for applying the worldOffset are:

| Condition | Details |
| --- | --- |
| Volume | Enabled, Global, Weight 1, Blend Distance 0, Priority equal to `DedicatedVolumePriority` |
| Transform | The rotation and scale of the display root and the baked root match (within 10⁻⁵) |
| Count | Only one enabled `OmniAdaptiveProbeVolumeSpace` (the worldOffset is a single value for all cameras) |
| Baked root | In the same scene as the display root |

To set all of this up at once, use the Editor API `Omni.Editor.BatchRendering.OmniAdaptiveProbeVolumeSpaceAuthoring.TryEnsureContract`. For details, see [Lighting spaces](../../../docs/guides/lighting-spaces/).

<!-- members -->

### DedicatedVolumePriority

The Priority that the `Volume` used by this component must have: `float.MaxValue`.

### CurrentWorldOffset

The worldOffset currently written to the `Volume`. Zero when not applied.

### IsApplied

`true` if the worldOffset is applied to the `Volume`.

### TryEvaluate

Computes the worldOffset that can be applied with the current scene setup, without changing the scene or the `Volume`. The worldOffset is "display root position − baked root position". When it cannot be applied, returns `false` and puts the reason (in English) in `reason`. It does not check the `Volume` conditions.

### TryRefreshNow

Checks the `Volume` conditions and writes the result of `TryEvaluate` to the `Volume` as the worldOffset. Returns `true` on success. When it cannot be applied, it removes the worldOffset override and returns `false` with the reason (in English).
