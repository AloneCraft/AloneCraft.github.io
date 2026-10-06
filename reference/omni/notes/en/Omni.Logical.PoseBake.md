---
description: "An asset that stores the poses of a fixed sequence as evenly spaced time samples, kept in Logical form."
---
A ScriptableObject that stores the camera and light poses of a fixed sequence as evenly spaced time samples, kept in Logical form. Spatial values such as positions are stored as raw fixed-point values and are never converted to `float` or `double`. `double` is used only for time (the first sample time and the interval).

- Omni 0.1 has no Editor window or menu for creating a `PoseBake`. Write samples into [PoseBakeChannel](../pose-bake-channel/) objects, set them with `SetData`, and save the asset from a script.
- Playback is done by the Timeline tracks [PoseBakeTrack](../pose-bake-track/) and [LightBakeTrack](../light-bake-track/). Every playback frame they check with `TryValidate` whether the bake can be used as is.

See [Sequencer and Pose Bake](../../../docs/guides/sequencer/) for how to create one and the playback rules.

<!-- members -->

### CurrentFormatVersion

The data format version the current implementation can read (currently 1). A bake whose `FormatVersion` differs is rejected by `TryValidate` with `FormatVersionMismatch`.

### m_FormatVersion

The data format version. `SetData` records `CurrentFormatVersion`.

### m_TotalBits

The total bit count of the Logical fixed-point format at bake time. Recorded by `SetData`.

### m_FractionalBits

The fractional bit count of the Logical fixed-point format at bake time. Recorded by `SetData`.

### m_RotationFractionalBits

The fractional bit count of the rotation fixed-point format at bake time. Recorded by `SetData`.

### m_SceneGuid

The asset GUID of the Scene that was baked.

### m_SourceGuid

The GUID of the sequence (Timeline asset) that was baked.

### m_SourceContentHash

A hash of the sequence content and layout at bake time. Used to detect changes made after baking.

### m_BakedUtc

The bake time as a string (intended as ISO 8601, UTC). For diagnostic display only; not used for validation.

### m_StartTimeSeconds

The time of the first sample (seconds), in sequence time.

### m_SampleIntervalSeconds

The interval between samples (seconds).

### m_SampleCount

The number of samples. Every channel has the same number of samples.

### m_TargetIds

The table of stable IDs of look-at targets. The look-at target indices in channels refer to this table.

### m_Channels

The array of baked channels ([PoseBakeChannel](../pose-bake-channel/)).

### FormatVersion

Returns the data format version.

### SceneGuid

Returns the asset GUID of the Scene that was baked.

### SourceGuid

Returns the GUID of the sequence (Timeline asset) that was baked.

### SourceContentHash

Returns the hash of the sequence content and layout at bake time.

### BakedUtc

Returns the bake time string.

### StartTimeSeconds

Returns the time of the first sample (seconds).

### SampleIntervalSeconds

Returns the interval between samples (seconds).

### SampleCount

Returns the number of samples.

### EndTimeSeconds

Returns the time of the last sample (seconds): `StartTimeSeconds + SampleIntervalSeconds × (SampleCount − 1)`. Returns `StartTimeSeconds` when there are no samples.

### TargetIds

Returns the look-at target ID table. The internal array itself is returned.

### Channels

Returns the channel array. The internal array itself is returned.

### SetData

Sets the bake result. Called from your baking code. It sets the validation information, time, ID table and channels, and also records the current format version (`CurrentFormatVersion`) and the fixed-point layout (the `Precision` total bits, fractional bits and rotation fractional bits). A `null` `targetIds` or `channels` becomes an empty array. The values are not validated.

### TryGetChannel

Finds the channel whose target path (`PoseBakeChannel.TargetPath`) matches. The comparison is case-sensitive. Returns `false` when none is found, with `channel` set to `null`.

### TryGetTargetId

Looks up a look-at target ID by index. Returns `false` when the index is out of range, with `targetId` set to `null`.

### TryValidate

Checks whether this bake can be used as is. When it cannot, returns `false` and puts the first reason found in `rejection`. The checks run in this order: format version (`FormatVersionMismatch`), fixed-point layout (`PrecisionMismatch`), Scene (`SceneMismatch`), sequence (`SourceMismatch`), content hash (`ContentHashMismatch`), samples (`EmptySamples`). An expected value that is empty is not checked. For the samples, it checks that the count and interval are positive, and that every channel has the same sample count and correctly sized arrays. The fixed-point layout does not change at run time or through settings, but a bake made with an Omni version that uses a different layout cannot be used until it is baked again.
