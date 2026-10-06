---
description: "表示のために動かした Root と、原点の近くで焼いた Light Probe の空間を対応付ける Component です。"
---
`OmniLightProbeSpace` は、焼いた後に表示のために動かす Root に付けます。Scene BRG が描く Renderer は、Light Probe の補間に、表示の位置ではなく焼いた空間の位置を使います。その位置は、Renderer の親にある `OmniLightProbeSpace` で変換します。

- 表示 Root のローカル座標を、Baked Space Root の座標として読み替えます。移動・回転・Scale を対応付けられます。
- 補間の位置には Renderer の原点を使います。Renderer に `probeAnchor` を設定している場合は、その位置を変換せずに使います。
- Legacy の経路（Scene BRG を使わない描画）は、この変換を行いません。
- Inspector の Validate で、Scene の補間の位置が焼いた Probe の範囲に入っているかを診断できます。Validate は Scene を変えません。

構成の手順、自動の診断、ビルドの検査は [照明の空間](../../../docs/guides/lighting-spaces/) を見てください。対応が必要なことを Scene に保存するには [OmniLightProbeSpaceRequirement](../omni-light-probe-space-requirement/) を併せて付けます。

<!-- members -->

### bakedSpaceRoot

Light Probe を焼いたときの Root です。Unity のワールド原点で焼いた場合は空にします。

### LightProbeSpaceChanged

いずれかの `OmniLightProbeSpace` が有効・無効になったとき、Inspector で値が変わったとき、`Configure` で Root が変わったときに呼ばれる静的なイベントです。Scene BRG は、これを受けて Light Probe の位置を計算し直します。

### BakedSpaceRoot

焼いた空間の Root です。`null` は Unity のワールド原点を表します。

### Configure

焼いた空間の Root を設定します。`null` は Unity のワールド原点を表します。値が変わったときだけ `LightProbeSpaceChanged` を呼びます。Undo には記録しません。

### TryToBakedPosition

表示空間の world の位置を、Light Probe を焼いた空間の位置へ変換します。この GameObject のローカル座標にしてから、`BakedSpaceRoot` の座標として world へ戻します。入力または結果が有限でないときは `false` を返します。
