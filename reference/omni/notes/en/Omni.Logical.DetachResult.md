---
description: "An immutable value that reports the result of detaching or baking a Logical Transform."
---
`DetachResult` is the result that [Logical Transform](../logical-transform/)'s `TryDetach` and `TryBakeToUnityTransformLossy` return through `out`. Use `Status` ([DetachStatus](../detach-status/)) to tell success from failure.

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

The category of the result. `DetachStatus.Success` on success.

### Message

A supplementary message. The current Omni always returns an empty string. Use `Status` to decide.

### DetachResult

Creates a result with the given category and message. A `null` `message` becomes an empty string.
