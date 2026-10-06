---
description: "How the observer slots are arranged."
---
How the observer slots are arranged. Chosen with `Layout` on [RadialObserverRig](../radial-observer-rig/). The slots stay inside the screen and never overlap. The Cameras of slots that are not shown are disabled.

<!-- members -->

### OrbitWipe

Places the single orbit slot (slot 0) small in the `Corner` corner. Its side length is `OrbitWipeArea`. Intended for spotting anomalies that change over time.

### FixedGrid

Places the four fixed slots (slots 1–4: Front, Right, Top, Iso) as a 2×2 grid in the `Corner` corner. The whole grid's side length is `GridArea`. Intended for still checks per angle and image comparison.

### Combined

Places the orbit slot with side length `GridArea`, and stacks the four fixed slots vertically beside it (toward the screen center) with side length `GridArea` / 4. `GridArea` is clamped to at most 0.8.
