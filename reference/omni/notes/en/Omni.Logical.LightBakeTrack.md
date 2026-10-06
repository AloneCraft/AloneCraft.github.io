---
description: "A Timeline track that drives a Light with baked rotation, intensity and color."
---
A Timeline track that drives a `Light` with baked rotation, intensity and color. Its binding type is `Light`, and its clips are [PoseBakeClip](../pose-bake-clip/). It uses channels of the `LightRotation` kind ([PoseBakeChannelKind](../pose-bake-channel-kind/)).

Every frame it validates the [PoseBake](../pose-bake/) of the clip with the highest weight, interpolates the values at the sequence time, and writes:

- The rotation of the `Light`'s Transform (world rotation). The Logical rotation is converted to a Unity rotation only at this point.
- The intensity, only when the interpolated value is greater than 0.
- The color.

It does not write a position, because the shading of a Directional Light depends only on its direction. When the bake cannot be used it writes nothing and reports the reason to [BakePlaybackController](../bake-playback-controller/). `CreateMixerPlayable`, which creates the mixer, is protected virtual; override it to use a derived mixer.

See [Sequencer and Pose Bake](../../../docs/guides/sequencer/) for the playback rules.

<!-- members -->

### m_ChannelTargetPath

The identifier of the channel this track drives (`"KeyLight"`, for example). Used when the clip's Channel Target Path is empty.

### ChannelTargetPath

Gets or sets the identifier of the channel this track drives. A new value is used from the next time the mixer is created.

### CreateTrackMixer

Called by Timeline when it builds the graph. Writes each clip's start time in the sequence (`PoseBakeClip.ClipStart`), then creates the mixer. The mixer receives the track's identifier and the `BakePlaybackController` on the Director's GameObject, if there is one.
