---
description: "Logical Transform の Detach と Bake の結果の分類です。"
---
`DetachStatus` は、[DetachResult](../detach-result/) の `Status` の値です。[Logical Transform](../logical-transform/) の `TryDetach` と `TryBakeToUnityTransformLossy` が返します。

失敗の値のとき、`TryDetach` は Scene を元のままにします。

<!-- members -->

### Success

成功しました。`TryDetach` では、Unity の Transform への Bake と、Logical Transform の削除が終わっています。

### AlreadyDetached

対象はすでに削除されています。

### FloatRangeExceeded

位置または Scale が `float` で表せません。

### InvalidHierarchy

禁止の配置（Logical Transform → 通常の Transform → Logical Transform）にあります。

### SingularParent

直接の子の Logical Transform を 1 段上へ移すときに、その world の姿勢を保てませんでした（親の Scale が 0 などで、逆変換できない）。

### ConversionFailed

直接の子の Logical Transform の world の姿勢を求められませんでした。

