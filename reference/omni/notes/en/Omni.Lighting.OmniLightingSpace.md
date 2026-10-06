---
description: "A component that converts, from scripts, between huge Logical positions and positions in the finite space used by baked lighting data."
---
`OmniLightingSpace` maps a [Logical Transform](../../components/logical-transform/) used as the Logical origin (Logical Origin) to the origin of a finite baked space (Lighting Space Root). Baked data such as Reflection Probes, APV and lightmaps is made in Unity's finite coordinates, so a huge Logical position cannot be turned into `float` as is. This component first computes the difference from the Logical origin in fixed point, then converts it to a finite position.

- You call the conversions from scripts. Omni's rendering does not use this component automatically.
- Only the world position of Logical Origin is used. Its rotation and scale are not.
- The rotation and scale of Lighting Space Root do apply to the conversion.

For details, see [Lighting spaces](../../../docs/guides/lighting-spaces/).

```csharp
using Omni.Lighting;
using UnityEngine;
using L = Omni.Logical;

var space = GetComponent<OmniLightingSpace>();
if (space.TryToLightingPosition(logicalPosition, out Vector3 finite))
{
    // finite is the finite position in the baked space
}
```

<!-- members -->

### logicalOrigin

The `Omni.Logical.Transform` used as the Logical origin. When empty, no conversion is possible.

### lightingSpaceRoot

The Transform at the origin of the baked space. When empty, this GameObject's Transform is used.

### LogicalOrigin

The `Omni.Logical.Transform` used as the Logical origin.

### LightingSpaceRoot

The origin of the baked space. If `lightingSpaceRoot` is empty, returns this GameObject's Transform.

### Configure

Sets Logical Origin and Lighting Space Root. Not recorded in Undo.

### TryToLightingPosition

Converts a Logical position to a finite world position in the baked space. It turns the difference from Logical Origin into `float` and converts it back to world as coordinates under Lighting Space Root. Returns `false` if Logical Origin is empty, its world position cannot be computed, the difference cannot be represented as `float`, or the result is not finite.

### TryToLogicalPosition

Converts a world position in the baked space back to a Logical position. The inverse of `TryToLightingPosition`. Returns `false` if the input is not finite, Logical Origin is empty, or the result goes out of range.
