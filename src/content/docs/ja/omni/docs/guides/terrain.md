---
title: Terrain
description: Terrain の表面を対数描画で描く patch の仕組み、Native Terrain への切替、4 レイヤーの上限、木と細部の扱い。
sidebar:
  order: 5
---

Omni は、Terrain の表面を、保存しない patch で対数描画にします。木と細部（草など）は、対数描画の対象に含みません。

## 対応の範囲

| 対象 | 扱い |
| --- | --- |
| Terrain の表面 | 同梱の Radial 版の Terrain の Shader（TerrainLit、TerrainLitAdd、TerrainLitBase、TerrainLitBasemapGen）と、保存しない patch で描く |
| 5 枚以上のレイヤーを持つ Terrain | patch を作らず、Native Terrain を表示する |
| 木と細部（tree／detail） | 対数描画に対応しない。Radial のカメラでは非表示 |
| Terrain Lit の Shader Graph | 変換しない。理由を示して止める |

Terrain の Material は、ほかの Material と同じく、対数への切替が Radial 版の Shader へ切り替えます（[Shader と Material](../shaders-and-materials/)）。Unity 既定の Terrain の Material は書き換えられないので、`Assets/Radial` に管理用の写しを作って使います。

## 保存しない patch

URP の Native Terrain の表面は、頂点の写像を解釈できません。そこで、Terrain の Material が Radial に対応している場合、Omni は Terrain の表面を、保存しない MeshRenderer の patch で描きます。

- TerrainData が常に正本です。patch は描画のための派生物で、Scene にもアセットにも保存しません。
- Native Terrain の表面を隠すのは、すべての patch を作り終えた後だけです。
- patch を作れなかったとき、patch を解除したとき、Scene の境界では、Native Terrain の表示へ戻します。
- 高さ（Heightmap）を変えると、影響する patch だけを作り直します。
- patch は Terrain の外で描くので、TerrainLit の基本の Pass が読む制御のテクスチャとレイヤーの値（`_Control`、`_Splat0`〜`3`、タイリングなど）を、Omni が TerrainData から写します。
- patch の細かさは固定です。距離に応じた LOD の切替はありません。

## 4 レイヤーの上限

patch は、TerrainLit の基本の Pass が 1 回で描く 4 枚のレイヤーだけを描きます。5 枚目以降のレイヤーは、Native Terrain では追加の Pass（TerrainLitAdd）で重ねますが、patch はこれを描きません。

一部のレイヤーが欠けた絵を出さないために、5 枚以上のレイヤーを持つ Terrain は patch を作らず、Native Terrain を表示します。patch で描くには、レイヤーを 4 枚以下にします。

## 木と細部

Terrain の木と細部（草など）は、Terrain の表面とは別の配置と描画の経路です。Shader の変換だけでは対応しません。

Radial のカメラで描くとき、Omni は木と細部を描く設定（`drawTreesAndFoliage`）が有効な Terrain について、そのカメラの描画でだけ木と細部を隠します。Console に理由を示す警告を出します。同じ種類の警告は、最初の 1 件だけを出します。

## Logical Transform の下の Terrain

`Omni.Logical.Transform` の下にある Terrain の Material が Radial に対応していない場合、その Terrain は Radial のカメラでは表示されません。Console に警告が出ます。先に Omni Manager で対数描画に切り替えてください（[標準描画と対数描画を切り替える](../../getting-started/switch-rendering/)）。

## RadialIdentityWarp で比べる

Terrain と同じ GameObject に `RadialIdentityWarp` を付けると、写像を行うカメラでも、その Terrain を Radial の Shader のまま、写像を恒等にして描きます。URP の本来の Shader、Radial の恒等、Radial の対数を、同じカメラ、同じフレームで並べて比べるためのものです。

- 恒等の Terrain は patch を作りません。Terrain の本来の描画に、恒等の値を渡します。
- 恒等の Terrain は、実際の Transform で描きます。カリングは URP と同じ判定です。
- 恒等の Terrain の木と細部は隠しません。

`RadialIdentityWarp` は Renderer にも付けられます。Renderer の場合は、Scene BRG の自動の所有から外し、Renderer ごとの MaterialPropertyBlock の経路で描きます。

## Scene BRG

Terrain の Shader（TerrainLit、TerrainLitAdd、TerrainLitBase、TerrainLitBasemapGen）を MeshRenderer に割り当てた場合は、Scene BRG の sidecar で描きます。詳しくは [Scene BRG](../scene-brg/) を見てください。
