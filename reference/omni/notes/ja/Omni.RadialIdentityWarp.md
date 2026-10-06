---
description: "同じ GameObject の Renderer または Terrain を、Radial の Shader のまま恒等の写像で描く Component です。"
---
`RadialIdentityWarp` は、URP の元の Shader、Radial の恒等、Radial の対数を、同じカメラ・同じフレームで並べて比べるための部品です。同じ GameObject の Renderer または Terrain に効きます。設定の項目はありません。

- 恒等の Renderer は、実際の Transform と Unity の Bounds で描きます。Culling は URP と同じ判定です。
- [Scene BRG](../../../docs/guides/scene-brg/) の対象から外し、Renderer ごとの経路（MaterialPropertyBlock）で描きます。Scene BRG には、Instance ごとに恒等の写像へ切り替える仕組みが無いためです。
- Terrain では、保存しない複製を作らず、Terrain の本来の描画へ恒等の値を渡します。
- 有効と無効を切り替えると、描画の対象を次の更新で選び直します。

写像を変える部品の全体は [Radial の写像と View Scale](../../../docs/concepts/radial-mapping/) を見てください。

<!-- members -->

### IsApplied(Renderer)

`renderer` と同じ GameObject に、有効な `RadialIdentityWarp` があれば `true` を返します。GameObject が非アクティブなとき、Component が無効なとき、`renderer` が `null` のときは `false` です。

### IsApplied(Terrain)

`terrain` と同じ GameObject に、有効な `RadialIdentityWarp` があれば `true` を返します。GameObject が非アクティブなとき、Component が無効なとき、`terrain` が `null` のときは `false` です。
