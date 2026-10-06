---
description: "A static class that checks the hierarchy rule of Logical Transforms."
---
`TransformHierarchy` checks the hierarchy rule of [Logical Transform](../logical-transform/). Omni's Runtime and Editor all use this check.

Rule: if a Logical Transform has a Logical Transform ancestor, that ancestor must be its **direct parent**.

| Hierarchy | Allowed |
| --- | --- |
| Logical Transform → plain Transform → plain Transform | Yes |
| Logical Transform → Logical Transform (direct parent and child) | Yes |
| Logical Transform → plain Transform → Logical Transform | **Forbidden** |

In the forbidden placement, the local transform of the plain Transform in between drops out of the Logical composition, while the Unity hierarchy still applies it, so the Logical value and the rendered position drift apart. A Logical Transform in the forbidden placement stops composing with its parent and positions itself from its own values only. For details, see "Hierarchy rules" in [Logical coordinates and proxies](../../../docs/concepts/logical-coordinates/).

<!-- members -->

### IsValid

Returns `true` if the hierarchy of `logicalTransform` follows the rule. Otherwise returns `false` and puts the ancestor Logical Transform that is not the direct parent into `offendingAncestor`. Returns `true` if `logicalTransform` is `null`.

### IsRoot

Returns `true` if `logicalTransform` is a starting point of the Logical composition: it has no direct parent, or its direct parent has no Logical Transform. Also `true` in the forbidden placement. Returns `false` if `logicalTransform` is `null`.

### DescribeViolation

Returns a human-readable text describing the violation and how to fix it, in Omni's display language. A `null` argument is shown with the name "(unknown)".
