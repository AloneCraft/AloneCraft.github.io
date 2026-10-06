---
description: "A wrapper that lets Unity serialization save a Scalar."
---

A wrapper that lets Unity serialization save a [Scalar](../scalar/). `Scalar` has no writable fields, so Unity cannot save it directly. `ScalarField` saves the value as an array of 3 × 64 bits and turns it back into a `Scalar` when loaded. It converts implicitly to and from `Scalar`, so you can read and write it like an ordinary `Scalar`.

The `nearClip` and `farClip` of [RadialCamera](../../components/radial-camera/) use this type.

```csharp
using UnityEngine;
using L = Omni.Logical;

public sealed class OrbitRadius : MonoBehaviour
{
    [SerializeField]
    private L.ScalarField m_Radius = L.ScalarField.FromString("149597870700");

    public L.Scalar Radius => m_Radius; // implicit conversion to Scalar
}
```

The Inspector field shows every decimal digit. Besides numbers, it accepts expressions (`+`, `*`, `pi`, `sqrt` and so on). Only the evaluated value is saved. Expressions work only in the Editor's Inspector; `FromString` and `Scalar.TryParse` in scripts accept numbers only. For the available expressions, see [Logical coordinates and proxies](../../../docs/concepts/logical-coordinates/).

<!-- members -->

### m_Limbs

The saved array. It holds the raw 192 bits of the value as three 64-bit parts, lowest first. You do not edit it directly.

### implicit operator Omni.Logical.Scalar

Converts a `ScalarField` to a `Scalar`.

### implicit operator Omni.Logical.ScalarField

Converts a `Scalar` to a `ScalarField`.

### FromString

Creates a field from a decimal string (exponent notation allowed). The conversion is the same as `Scalar.TryParse`. Returns zero for a string it cannot convert. Use `Scalar.TryParse` when you need to detect the failure.

### OnBeforeSerialize

Called by Unity before saving. Writes the current value to the 3 × 64-bit array.

### OnAfterDeserialize

Called by Unity after loading. A 3 × 64-bit array is read as the current format. A 2 × 64-bit array is read as the earlier format (Q95.32) and converted to the same number. A missing array, or an array of any other length, gives 0. `ScalarField` itself does not log a warning.
