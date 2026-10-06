---
description: "The Shader Graph Custom Functions in RadialWarpNode.hlsl: inputs and outputs, spaces, and where the converter adds them."
---
The Shader Graph Custom Function (Type = File) functions in `Assets/Omni/Runtime/Sharders/RadialWarpNode.hlsl`. Every function has a `_float` and a `_half` version. The converter adds these nodes to graphs when you switch to logarithmic rendering; you do not need to place them yourself.

| Graph | Nodes the converter adds |
| --- | --- |
| URP Lit, Unlit | `RadialKeepObjectPosition`. Graphs with an Emission block also get `RadialGetLogicalViewDirection` and `RadialPreserveEmission` |
| URP Decal | `RadialPreserveDecalAlpha` |
| URP Fullscreen | `RadialPreserveFullscreenColor`. Graphs with Depth Write enabled also get `RadialFullscreenDepth` |

Some functions only pass their value through. They exist to keep this include in the graph, which enables the replacements inside it (vertex hooks, depth reconstruction). See [Shader Graph and custom shaders](../../../docs/guides/shader-graph/) for the whole conversion and [Include files](../includes/) for the role of each include.

<!-- members -->

### RadialKeepObjectPosition

The function for the vertex Position. `Out` returns the object-space `PositionOS` unchanged. The actual Radial projection is done by the vertex hooks (`RadialVertexHooks.hlsl`). Going through Absolute World and back to object space would break vertex precision at large Logical coordinates, so no coordinate transform is done. The other inputs (`EyeRelative`, `EyeRelativeExponent`, `S`, `Mode`, `LinearR`, `Log2ViewScale`, `ViewRow0`–`ViewRow2`) are connected to the `_RadialGlobal_*` Blackboard properties and exist only to keep those properties in the graph; their values are not read. The converter inserts this node in front of the Vertex Position block in every Lit and Unlit graph it converts. The original Position input (vertex deformation such as wind or waves) is reconnected to `PositionOS`, so it is not lost.

### RadialGetLogicalViewDirection

Returns the Logical view direction of the vertex from its object-space position `PositionOS`. `Out` is a normalized world-space direction that points from the surface toward the camera, like URP's View Direction. For renderers that do not receive the Radial transform it returns URP's standard view direction. In passes where the vertex hooks are disabled (ShadowCaster, Meta, and so on) it returns `(0, 0, 1)`. In graphs with an Emission block, the converter feeds this node's output into a vertex Custom Interpolator (`OmniLogicalViewDirection`) to carry it to the fragment stage.

### RadialPreserveEmission

`Out` returns the original emission (`OriginalEmission`) unchanged. At the same time it stores the `LogicalViewDirection` received through the Custom Interpolator as the Logical view direction of that fragment, so that the PBR lighting setup uses it. The stored value is valid only within the same fragment invocation. The converter inserts this node in front of the Emission block and connects the original emission input to `OriginalEmission`.

### RadialPreserveDecalAlpha

`Out` returns the original alpha (`OriginalAlpha`) unchanged. This node keeps the include in a Decal graph, which enables the Decal Projector passes to map positions reconstructed from depth back through the inverse Radial mapping. The converter inserts it in front of the Alpha block of URP Decal graphs. Graphs that have neither Affect BaseColor nor Affect Emission enabled are not converted, because URP does not generate their alpha.

### RadialPreserveFullscreenColor

`Out` returns the original color (`OriginalColor`) unchanged. This node keeps the include in a Fullscreen graph, which enables the Scene Depth replacement (turning mapped depth back into physical depth). The converter inserts it in front of the Base Color block of Fullscreen graphs.

### RadialFullscreenDepth

In a Fullscreen graph with Depth Write enabled, converts the physical depth the graph writes into the mapped depth used by the depth buffer. The inputs are the screen position `ScreenPosition` (Screen Position node, Default mode), the depth `Depth`, and `Mode` (0 = Eye, 1 = Linear01, 2 = Raw). `Out` is a depth in the same encoding as `Depth`. The mapping keeps view directions, so only the depth along the same pixel's ray is replaced. When Radial is inactive, the projection is orthographic, or the graph is not a Fullscreen graph, `Depth` is returned unchanged. "Far" markers for pixels that hit nothing (Eye 1e20 or more, Linear01 1 or more, Raw equal to the far value) are also returned unchanged. The converter inserts this node in front of the depth block that matches the graph's Depth Write Mode and sets `Mode` to that mode.
