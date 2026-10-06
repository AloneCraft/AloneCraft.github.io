---
description: "Logical Transform の Detach と Bake の結果を返す、変更できない値です。"
---
`DetachResult` は、[Logical Transform](../logical-transform/) の `TryDetach` と `TryBakeToUnityTransformLossy` が `out` で返す結果です。成否の分類は `Status`（[DetachStatus](../detach-status/)）で判定します。

```csharp
using UnityEngine;
using L = Omni.Logical;

if (!logical.TryDetach(out L.DetachResult result))
{
    Debug.LogWarning($"Detach failed: {result.Status}");
}
```

<!-- members -->

### Status

結果の分類です。成功なら `DetachStatus.Success` です。

### Message

補足のメッセージです。今の Omni は常に空の文字列を返します。判定には `Status` を使ってください。

### DetachResult

分類とメッセージを指定して作ります。`message` が `null` なら空の文字列にします。
