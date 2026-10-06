---
description: "巨大な Logical の位置と、焼いた照明のデータが使う有限な空間の位置を、スクリプトから変換する Component です。"
---
`OmniLightingSpace` は、Logical の原点として使う [Logical Transform](../../components/logical-transform/)（Logical Origin）と、有限な焼いた空間の原点（Lighting Space Root）を対応付けます。Reflection Probe、APV、Lightmap などの焼いたデータは Unity の有限な座標で作るので、巨大な Logical の位置をそのまま `float` にできません。この Component は、Logical の原点との差を固定小数点のまま求めてから、有限な位置へ変換します。

- 変換はスクリプトから呼びます。Omni の描画は、この Component を自動では使いません。
- Logical Origin は world の位置だけを使います。回転と Scale は使いません。
- Lighting Space Root の回転と Scale は変換にかかります。

詳しくは [照明の空間](../../../docs/guides/lighting-spaces/) を見てください。

```csharp
using Omni.Lighting;
using UnityEngine;
using L = Omni.Logical;

var space = GetComponent<OmniLightingSpace>();
if (space.TryToLightingPosition(logicalPosition, out Vector3 finite))
{
    // finite は焼いた空間の有限な位置
}
```

<!-- members -->

### logicalOrigin

Logical の原点として使う `Omni.Logical.Transform` です。空のときは変換できません。

### lightingSpaceRoot

焼いた空間の原点の Transform です。空のときは、この GameObject の Transform を使います。

### LogicalOrigin

Logical の原点として使う `Omni.Logical.Transform` です。

### LightingSpaceRoot

焼いた空間の原点です。`lightingSpaceRoot` が空なら、この GameObject の Transform を返します。

### Configure

Logical Origin と Lighting Space Root を設定します。Undo には記録しません。

### TryToLightingPosition

Logical の位置を、焼いた空間の有限な world の位置へ変換します。Logical Origin との差を `float` にし、Lighting Space Root の座標として world へ戻します。Logical Origin が空、world の位置を求められない、差が `float` で表せない、または結果が有限でないときは `false` を返します。

### TryToLogicalPosition

焼いた空間の world の位置を、Logical の位置へ戻します。`TryToLightingPosition` の逆です。入力が有限でない、Logical Origin が空、または結果が範囲を超えるときは `false` を返します。
