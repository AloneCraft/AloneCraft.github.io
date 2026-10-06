---
description: "A Timeline clip that says which channel of which PoseBake drives one span of sequence time."
---
A Timeline clip that says which channel of which [PoseBake](../pose-bake/) drives one span of sequence time. Place it on [PoseBakeTrack](../pose-bake-track/) and [LightBakeTrack](../light-bake-track/).

- The clip itself holds no samples. It points to a single `PoseBake` for the whole sequence, and baked data is looked up by sequence time (the clip start time plus the elapsed time within the clip). Splitting samples per shot would require duplicate samples at the boundaries and a rebake every time a clip is moved.
- Clips neither blend nor loop (`clipCaps` is `None`). At a shot boundary the pose switches.

See [Sequencer and Pose Bake](../../../docs/guides/sequencer/) for the playback rules.

<!-- members -->

### m_Bake

The `PoseBake` this clip refers to. When unset, the track writes nothing and reports `MissingAsset`. Choosing a fallback pose is up to the code that drives the sequence.

### m_ChannelTargetPath

The identifier of the channel to drive (`"Camera"`, `"KeyLight"`, and so on). When empty, the track's Channel Target Path is used.

### Bake

Gets or sets the `PoseBake` to refer to.

### ChannelTargetPath

Gets or sets the identifier of the channel to drive.

### ClipStart

The time (seconds) at which this clip starts in the sequence. The track writes it every time it creates its mixer. It is not serialized, so dragging the clip does not invalidate the baked data.

### clipCaps

The clip's capabilities. Always returns `ClipCaps.None`, allowing neither blending nor looping. A pose halfway between the camera poses of two shots has no meaning.

### CreatePlayable

Called by Timeline when it creates the clip's Playable. Passes `Bake`, `ChannelTargetPath` and `ClipStart` to the Playable. The track's mixer is what writes the values.
