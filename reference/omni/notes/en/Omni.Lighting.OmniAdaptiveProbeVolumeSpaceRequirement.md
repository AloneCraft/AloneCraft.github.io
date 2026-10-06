---
description: "A marker component that stores in the scene that a display root needs an explicit mapping to the baked APV space."
---
`OmniAdaptiveProbeVolumeSpaceRequirement` is a marker that says "this root needs an explicit mapping to the baked APV space". The same GameObject needs [OmniAdaptiveProbeVolumeSpace](../omni-adaptive-probe-volume-space/) and [OmniLightProbeSpaceRequirement](../omni-light-probe-space-requirement/) (`RequireComponent`).

- It stores the expected root of the baked space. This value is not shown in the Inspector and is set by the Editor setup API.
- In a scene with this marker, a Player build checks the conditions of `OmniAdaptiveProbeVolumeSpace` and the combination of components. If they are not met, the build stops.

For details, see [Lighting spaces](../../../docs/guides/lighting-spaces/).

<!-- members -->

### ExpectedBakedSpaceRoot

The expected root of the baked space. `null` means Unity's world origin.
