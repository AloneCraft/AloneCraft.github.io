---
description: "A Timeline track that drives an Omni.Logical.Transform with baked Logical poses."
---
A Timeline track that drives an [Omni.Logical.Transform](../../components/logical-transform/) with baked Logical poses. Its binding type is `Omni.Logical.Transform`, and its clips are [PoseBakeClip](../pose-bake-clip/).

Every frame it validates the [PoseBake](../pose-bake/) of the clip with the highest weight, interpolates position and rotation at the sequence time, writes them to `LocalPosition` and `LocalRotation`, and syncs the Unity Transform. When the bake cannot be used it writes nothing and reports the reason to [BakePlaybackController](../bake-playback-controller/).

- Timeline's Animation Track, which writes the Unity Transform directly, cannot be used. It breaks the rule that the Logical Transform is the only writer of the Transform, and at large coordinates the pose is rounded to `float` and disagrees with the Logical value.
- The standard `PoseBakeTrack` does not set the mixer's (`PoseBakeMixerBehaviour`) `Camera`. So the baked field of view and clip range are not written to the camera; only the pose is written.
- `CreateMixerPlayable`, which creates the mixer, is protected virtual. Override it to use a derived mixer, for example one that adds Profiler markers.

See [Sequencer and Pose Bake](../../../docs/guides/sequencer/) for the playback rules.

<!-- members -->

### m_ChannelTargetPath

The identifier of the channel this track drives. Used when the clip's Channel Target Path is empty. If neither the clip nor the track has an identifier, a channel with an empty identifier is looked up.

### ChannelTargetPath

Gets or sets the identifier of the channel this track drives. A new value is used from the next time the mixer is created.

### CreateTrackMixer

Called by Timeline when it builds the graph. Writes each clip's start time in the sequence (`PoseBakeClip.ClipStart`), then creates the mixer. The mixer receives the track's identifier and the `BakePlaybackController` on the Director's GameObject, if there is one.
