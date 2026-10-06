---
title: 照明の空間
description: 表示用の Root を動かした Scene でも、焼いた Light Probe と APV を正しい位置で使う。
sidebar:
  order: 6
---

Omni の Radial の Lit は、写像した後の位置ではなく、物理的な位置で照明を計算します。一方、Light Probe、APV、Lightmap、Reflection Probe などの焼いたデータは、Unity の有限な座標で作ります。表示のために Root を動かすと、表示の座標と、焼いたときの座標がずれます。このページの Component は、その 2 つの空間の対応を Scene に明示します。

## 照明について Omni が守ること

- 照明は物理的な位置で計算します。写像は視線の向きを保つので、標準の描画で表せる範囲では、Radial の画像の照明・影・Lightmap は標準の描画と同じです。
- 反射と視線の向きは、補間した値を使わず、画素ごとに物理的な面の位置から求め直します。
- SSAO は、画面上の写像した位置ではなく、物理的な面の位置と深度から求めます。
- ShadowCaster と Meta の Pass は、Radial の写像で変形せず、物理的な空間のまま扱います。
- 物理的な照明のデータを正しく渡せない Lit の経路は、描画しません（[Fail Closed](../../concepts/fail-closed/)）。
- 写像、深度、照明のデータは、品質レベルやビルド対象によって変わりません。

:::note[SSAO]
URP の標準の SSAO は、写像した後の深度を物理的な深度として読むため、Radial のカメラにそのまま使えません。Omni Manager の「シェーダー」タブで、SSAO の Shader が Omni 版につながっていることを確かめてください（[Verification Hub で確かめる](../../getting-started/verification/)）。
:::

## どの Component を使うか

| 焼いたデータ | Component | 対応付けられる変換 |
| --- | --- | --- |
| Light Probe（Light Probe Groups） | `OmniLightProbeSpace`、`OmniLightProbeSpaceRequirement` | 表示 Root の Transform（移動・回転・Scale） |
| APV（Adaptive Probe Volumes） | `OmniAdaptiveProbeVolumeSpace`、`OmniAdaptiveProbeVolumeSpaceRequirement` | 平行移動だけ |
| Logical の位置と焼いた空間の位置の変換 | `OmniLightingSpace` | スクリプトから使う変換 |

表示の座標と焼いた座標が同じ Scene（Root を動かしていない Scene）には、何も付けません。Omni は、そのような Scene に不要な変換を加えません。

## Light Probe の空間

`OmniLightProbeSpace` は、表示用に動かした Root と、原点の近くで焼いた Light Probe の空間を対応付けます。

1. 焼いた後に動かす表示用の Root に、`OmniLightProbeSpace` を付けます。
2. Inspector の Baked Space Root に、Light Probe を焼いたときの Root を設定します。Unity のワールド原点で焼いた場合は空にします。

Scene BRG が描く Renderer は、Light Probe の補間に、表示の位置ではなく焼いた空間の位置を使います。その位置は、Renderer の親にある `OmniLightProbeSpace` で変換します。表示 Root のローカル座標を、Baked Space Root の座標として読み替えます。

- 補間の位置には Renderer の原点を使います。Renderer に `probeAnchor` を設定している場合は、その位置を変換せずに使います。
- Light Probe の使用を Off にした Renderer は対象外です。

`OmniLightProbeSpaceRequirement` は、「この Root には焼いた空間との明示的な対応が必要」という印を Scene に保存します。`OmniLightProbeSpace` と同じ GameObject に付けます。期待する Baked Space Root は Inspector には出ません。下の自動の修復か、Editor の API `Omni.Editor.BatchRendering.OmniLightProbeSpaceAuthoring.TryEnsureContract` が設定します。

:::caution[Legacy の経路]
Legacy の経路は Unity の標準の Light Probe 補間のままで、`OmniLightProbeSpace` の変換を行いません。表示 Root を動かした Scene では、Light Probe で照らす物体の照明が変わります。URP の Forward（Forward+ ではない）の Renderer で描くカメラがある間は、Scene BRG を使わず Legacy で描きます。Radial で標準と同じ照明を求める Scene では、Forward+ の Renderer を使ってください（[Scene BRG](../scene-brg/)）。
:::

### 自動の診断

Editor は、Scene を開いたとき、Scene の変更、Undo、Lightmap の Bake の完了などの後に、作業中の Scene を診断します。対象は、有効な RadialCamera があり、Scene BRG で描く Scene です。

問題が見つかると、Console に警告を出し、「Omni Light Probe Space」ウィンドウを開きます。同じ状態での自動の表示は、Editor の 1 回の起動の間に 1 度だけです。

| 状態 | 意味 |
| --- | --- |
| 対象外 | 診断の条件に当たらない（RadialCamera や Scene BRG が無い、焼いた Light Probe が無い、など） |
| 正常 | 補間の位置が、焼いた Probe の範囲に入っている |
| 不足（安全に修復可能） | 対応が足りない。修復の方法が 1 つに決まる |
| 不足候補（要手動確認） | 対応が足りない可能性があるが、追加先の Root を 1 つに決められない |
| 明示契約違反 | 付けた Component どうしが矛盾している。この Scene を Player に入れるとビルドが止まる |

「不足（安全に修復可能）」のときは、「確認してLight Probe空間を追加…」で修復できます。Unity のワールド原点を焼いた空間として、候補の Root に `OmniLightProbeSpace` と `OmniLightProbeSpaceRequirement` を追加します。Undo できる 1 つの操作で、Scene は自動で保存しません。この修復を出すのは、1 つの最上位の Root をワールド原点の空間へ戻すと、すべての補間の位置が Probe の範囲に入るときだけです。

「不足候補（要手動確認）」のときは自動で追加しません。焼いた空間と表示 Root を確かめて、Component を自分で構成してください。

もう一度診断するには、表示 Root の `OmniLightProbeSpace` の Inspector で Validate を押します。Validate は読むだけで、Scene を変えません。

### ビルドの検査

Player のビルドでは、Scene ごとに次を確かめます。Play の開始は止めません。

| 結果 | 扱い |
| --- | --- |
| 明示契約違反 | ビルドを止める |
| 不足、不足候補 | 警告を出してビルドを続ける |

明示契約違反になるのは、たとえば次の場合です。

- `OmniLightProbeSpaceRequirement` があるのに、`OmniLightProbeSpace` が無い
- 期待する Baked Space Root と、`OmniLightProbeSpace` の Baked Space Root が一致しない
- Baked Space Root が、別の Scene にある
- 表示 Root を、有限な焼いた空間の位置へ変換できない

## APV の空間

`OmniAdaptiveProbeVolumeSpace` は、表示 Root の平行移動を、URP の APV の公式の `ProbeVolumesOptions.worldOffset` で対応付けます。回転と Scale は表せません。

| 条件 | 内容 |
| --- | --- |
| 同じ GameObject | `OmniLightProbeSpace` と `Volume` が必要（Component を付けると自動で追加される） |
| Volume | 有効、Global、Weight 1、Blend Distance 0、Priority は予約の最大値（`float.MaxValue`） |
| 変換 | 表示 Root と焼いた Root の回転と Scale が一致する |
| 数 | 読み込んだ Scene の中で、有効な `OmniAdaptiveProbeVolumeSpace` は 1 つだけ（worldOffset はカメラ全体で 1 つのため） |
| 焼いた Root | 表示 Root と同じ Scene にある |

条件を満たさない間は、worldOffset を適用しません。Play 中は、その理由を Console に 1 度出します。Volume の Profile は実行時の複製だけを変えます。共有の Profile の Asset は変えません。

これらを一度に構成するには、Editor の API `Omni.Editor.BatchRendering.OmniAdaptiveProbeVolumeSpaceAuthoring.TryEnsureContract` を使います。Light Probe の空間の対応も同じ Root に構成し、Volume の値を上の条件に合わせます。Scene は未保存の状態になりますが、保存はしません。次の場合は構成せず、理由を返します。

- 表示 Root が非アクティブ
- 表示 Root に、別の目的の Volume がすでにある
- 読み込んだ Scene に、別の有効な `OmniAdaptiveProbeVolumeSpace` がある
- 表示 Root と焼いた Root の回転や Scale が違う

`OmniAdaptiveProbeVolumeSpaceRequirement` がある Scene では、Player のビルドで、上の条件と Component の組み合わせを確かめます。満たさない場合はビルドを止めます。

## Logical の位置と焼いた空間の変換

`OmniLightingSpace` は、Logical の原点として使う `Omni.Logical.Transform`（Logical Origin）と、有限な焼いた空間の原点（Lighting Space Root。未指定なら自身）を対応付けます。

| API | 内容 |
| --- | --- |
| `TryToLightingPosition` | Logical の位置を、焼いた空間の有限な位置へ変換する |
| `TryToLogicalPosition` | 焼いた空間の位置を、Logical の位置へ戻す |

巨大な絶対値を `float` にせず、Logical の原点との差を求めてから有限な値へ変換します。変換できないときは `false` を返します。

## Reflection Probe

焼く Reflection Probe（Baked、または Dynamic Objects を含まない Custom）には、Reflection Probe Static の Renderer だけを写し、影も static な caster だけから作ります。Scene BRG も同じ規則で描きます。Realtime の Probe、Dynamic Objects を含む Probe、Play 中は、すべての Renderer を写します。

表示 Root を動かした Scene では、Reflection Probe を Scene BRG で描く状態で焼いてください。Legacy の経路では Light Probe の照明が変わるためです。

## 確かめる

`Assets/Omni/Verification/Functions/Lighting` に、Lightmap、Realtime Shadow、Reflection Probe を確かめる Scene があります。これらの Scene は、専用の URP 資産へ切り替えず、プロジェクトで有効な URP 資産で描きます。

| Scene | 必要な URP 資産の設定 |
| --- | --- |
| `Lightmaps.unity` | Light Probe System が「Light Probe Groups」で、Mixed Lighting が有効 |
| `ReflectionProbes.unity` | Reflection Probe の Blending と Box Projection が有効 |

足りない設定は、Scene を開いたときに Console に警告します。撮影は、その理由を返して止まります。
