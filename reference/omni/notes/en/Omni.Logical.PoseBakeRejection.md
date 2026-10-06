---
description: "The reason baked samples were judged unusable."
---
The reason baked samples were judged unusable. Returned by `PoseBake.TryValidate`, [PoseBakeSampler](../pose-bake-sampler/) and the bake tracks, and collected in [BakePlaybackController](../bake-playback-controller/). Not being able to use a bake is part of normal operation (without baked data, the picture can still come from your own fallback computation), so instead of a single `bool`, the value tells which assumption failed.

`TargetMoved`, `ObserverEyeRequired` and `UserCameraOverride` are never returned by Omni's tracks themselves. You set them on `BakePlaybackController.ExternalOverride` to stop applying baked poses. See [Sequencer and Pose Bake](../../../docs/guides/sequencer/) for details.

<!-- members -->

### None

Usable; nothing was rejected.

### MissingAsset

No `PoseBake` is set on the clip. Also returned when `null` is passed to the sampler.

### FormatVersionMismatch

The data format version differs from the current implementation (`PoseBake.CurrentFormatVersion`).

### PrecisionMismatch

The fixed-point layout (total bits, fractional bits, rotation fractional bits) differs between bake time and now. The meaning of the values has changed, so the bake is rejected rather than repacked.

### SceneMismatch

The baked Scene differs from the Scene being checked against.

### SourceMismatch

The baked sequence (Timeline) differs from the sequence being checked against.

### ContentHashMismatch

The content hash differs: the sequence or the layout changed after baking.

### EmptySamples

There are no samples, the interval is 0 or less, or a channel's array lengths do not match the declaration.

### TimeOutOfRange

The track has no clip with weight at the current time. Times outside the baked range do not cause this reason; they hold at the edge sample.

### ChannelUnavailable

The `PoseBake` has no channel with the specified identifier.

### TargetUnresolved

No write target (`Omni.Logical.Transform` or `Light`) is bound to the track.

### TargetMoved

The target's position differs between bake time and now. Used by setting it on `ExternalOverride`.

### ObserverEyeRequired

An observer eye offset is required. Baked data is in absolute Logical coordinates, so it cannot be used. Used by setting it on `ExternalOverride`.

### UserCameraOverride

The user has taken control of the camera, so the sequence pose must not overwrite it. Used by setting it on `ExternalOverride`.

### InterpolationFailed

Restoring or interpolating a Logical value, or converting it to a Unity rotation, failed.
