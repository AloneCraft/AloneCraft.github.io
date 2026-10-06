---
description: "A marker component that stores in the scene that a display root needs an explicit mapping to the baked Light Probe space."
---
`OmniLightProbeSpaceRequirement` is a marker that says "this root needs an explicit mapping to the baked space". Add it to the same GameObject as [OmniLightProbeSpace](../omni-light-probe-space/) (`RequireComponent`).

- It stores the expected Baked Space Root. This value is not shown in the Inspector. It is set by the Editor's automatic fix (the "確認してLight Probe空間を追加…" button, confirm and add Light Probe space) or by the Editor API `Omni.Editor.BatchRendering.OmniLightProbeSpaceAuthoring.TryEnsureContract`.
- When building a Player, Omni checks whether this marker and the Baked Space Root of `OmniLightProbeSpace` agree. A contract violation, such as a mismatch, stops the build.

For details, see [Lighting spaces](../../../docs/guides/lighting-spaces/).

<!-- members -->

### ExpectedBakedSpaceRoot

The expected root of the baked space. `null` means Unity's world origin.
