---
description: "表示 Root に APV の焼いた空間との明示的な対応が必要なことを、Scene に保存する印の Component です。"
---
`OmniAdaptiveProbeVolumeSpaceRequirement` は、「この Root には APV の焼いた空間との明示的な対応が必要」という印です。同じ GameObject に [OmniAdaptiveProbeVolumeSpace](../omni-adaptive-probe-volume-space/) と [OmniLightProbeSpaceRequirement](../omni-light-probe-space-requirement/) が必要です（`RequireComponent`）。

- 期待する焼いた空間の Root を保存します。この値は Inspector には出ず、Editor の構成の API が設定します。
- この印がある Scene では、Player のビルドで、`OmniAdaptiveProbeVolumeSpace` の条件と Component の組み合わせを確かめます。満たさない場合はビルドを止めます。

詳しくは [照明の空間](../../../docs/guides/lighting-spaces/) を見てください。

<!-- members -->

### ExpectedBakedSpaceRoot

期待する、焼いた空間の Root です。`null` は Unity のワールド原点を表します。
