---
description: "表示 Root の平行移動を、URP の APV の worldOffset に反映する Component です。"
---
`OmniAdaptiveProbeVolumeSpace` は、表示 Root と、APV（Adaptive Probe Volumes）を焼いた空間の平行移動を、URP の公式の `ProbeVolumesOptions.worldOffset` で対応付けます。回転と Scale は表せません。

- 同じ GameObject に [OmniLightProbeSpace](../omni-light-probe-space/) と `Volume` が必要です。Component を付けると自動で追加されます。焼いた空間の Root には、`OmniLightProbeSpace` の Baked Space Root を使います。
- 有効な間、`Update` のたびに worldOffset を評価して `Volume` に書きます。Edit Mode でも働きます。条件を満たさない間は worldOffset の上書きを外します。
- 書き換えるのは `Volume.profile` の実行時の複製だけです。共有の Profile の Asset は変えません。
- Play 中に条件を満たさなくなると、その理由を Console にエラーで出します。同じ理由は続けて出しません。

worldOffset を適用する条件は次のとおりです。

| 条件 | 内容 |
| --- | --- |
| Volume | 有効、Global、Weight 1、Blend Distance 0、Priority が `DedicatedVolumePriority` |
| 変換 | 表示 Root と焼いた Root の回転と Scale が一致する（誤差 10⁻⁵ 以内） |
| 数 | 有効な `OmniAdaptiveProbeVolumeSpace` が 1 つだけ（worldOffset はカメラ全体で 1 つのため） |
| 焼いた Root | 表示 Root と同じ Scene にある |

一度に構成するには、Editor の API `Omni.Editor.BatchRendering.OmniAdaptiveProbeVolumeSpaceAuthoring.TryEnsureContract` を使います。詳しくは [照明の空間](../../../docs/guides/lighting-spaces/) を見てください。

<!-- members -->

### DedicatedVolumePriority

この Component が使う `Volume` に要る Priority で、`float.MaxValue` です。

### CurrentWorldOffset

今、`Volume` に書いている worldOffset です。適用していないときはゼロです。

### IsApplied

worldOffset を `Volume` に適用していれば `true` です。

### TryEvaluate

今の Scene の構成で適用できる worldOffset を求めます。Scene と `Volume` は変えません。worldOffset は「表示 Root の位置 − 焼いた Root の位置」です。適用できないときは `false` を返し、理由（英語）を `reason` に入れます。`Volume` の条件はここでは確かめません。

### TryRefreshNow

`Volume` の条件を確かめ、`TryEvaluate` の結果を worldOffset として `Volume` に書きます。成功したら `true` を返します。適用できないときは worldOffset の上書きを外し、`false` と理由（英語）を返します。
