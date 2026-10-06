---
description: "What a single bake channel drives."
---
What a single bake channel ([PoseBakeChannel](../pose-bake-channel/)) drives. The kind decides which values `PoseBakeChannel.CreateForBake` allocates.

<!-- members -->

### CameraPose

A camera pose. Holds position, rotation, camera distance, look-at offset, look-at target index, field of view and clip range. Played back by [PoseBakeTrack](../pose-bake-track/).

### LightRotation

The main light pose. Holds rotation, light intensity and color, with no position (the shading of a Directional Light depends only on its direction). Played back by [LightBakeTrack](../light-bake-track/).
