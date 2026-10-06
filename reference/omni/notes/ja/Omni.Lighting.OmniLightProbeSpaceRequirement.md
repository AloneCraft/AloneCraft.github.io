---
description: "表示 Root に Light Probe の焼いた空間との明示的な対応が必要なことを、Scene に保存する印の Component です。"
---
`OmniLightProbeSpaceRequirement` は、「この Root には焼いた空間との明示的な対応が必要」という印です。[OmniLightProbeSpace](../omni-light-probe-space/) と同じ GameObject に付けます（`RequireComponent`）。

- 期待する Baked Space Root を保存します。この値は Inspector には出ません。Editor の自動の修復（「確認してLight Probe空間を追加…」）か、Editor の API `Omni.Editor.BatchRendering.OmniLightProbeSpaceAuthoring.TryEnsureContract` が設定します。
- Player のビルドでは、この印と `OmniLightProbeSpace` の Baked Space Root が一致するかを確かめます。一致しないなど明示契約に違反すると、ビルドを止めます。

詳しくは [照明の空間](../../../docs/guides/lighting-spaces/) を見てください。

<!-- members -->

### ExpectedBakedSpaceRoot

期待する、焼いた空間の Root です。`null` は Unity のワールド原点を表します。
