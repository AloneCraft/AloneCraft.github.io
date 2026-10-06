---
description: "The time samples for one target (a camera, the main light, and so on)."
---
The time samples for one target (a camera, the main light, and so on). Stored inside a [PoseBake](../pose-bake/).

Logical values are not stored as one [ScalarField](../../logical/scalar-field/) per sample; they are packed per channel into flat `ulong` arrays as raw fixed-point values (limbs). This avoids allocating many small arrays on every serialization and keeps the asset small in line count. Field of view, clip range, light intensity and color are not spatial quantities, so they stay as `float` and `Color`.

The values a channel holds depend on its kind ([PoseBakeChannelKind](../pose-bake-channel-kind/)).

| Kind | Values |
| --- | --- |
| `CameraPose` | Position, rotation, camera distance, look-at offset, look-at target index, field of view, clip range |
| `LightRotation` | Rotation, light intensity, color |

Create one with `CreateForBake` and write samples with the `Write*` methods. The `Write*` methods do not check the index; writing an out-of-range index, or a value the kind does not hold, throws an exception. To read, use the `TryRead*` methods or [PoseBakeSampler](../pose-bake-sampler/). See [Sequencer and Pose Bake](../../../docs/guides/sequencer/) for the steps.

<!-- members -->

### m_TargetPath

A stable identifier for the target (`"Camera"`, `"KeyLight"`, and so on). Matched against the Channel Target Path of clips and tracks.

### m_Kind

The kind of target this channel drives.

### m_SampleCount

The number of samples.

### m_PositionLimbs

Position samples. Each sample holds the raw values of the three components X, Y and Z.

### m_RotationLimbs

Rotation samples. Each sample holds the raw values of the four components X, Y, Z and W (in the 128-bit rotation format).

### m_DistanceLimbs

Camera distance samples. One raw value per sample.

### m_LookOffsetLimbs

Samples of the Logical look-at offset. Three raw component values per sample.

### m_TargetIndices

Look-at target index samples. These are indices into `PoseBake.TargetIds`.

### m_FieldOfView

Field of view samples (degrees).

### m_NearClip

Near clip distance samples.

### m_FarClip

Far clip distance samples.

### m_LightIntensity

Light intensity samples.

### m_LightColor

Light color samples.

### OnBeforeSerialize

Does nothing.

### OnAfterDeserialize

Called right after Unity loads the data. Widens the position, distance and look-at offset stored in the old format (two limbs per value, from when Logical was 128-bit) to the current format with the same numeric value. The rotation format has not changed, so rotations are kept as they are.

### TargetPath

Returns the stable identifier for the target.

### Kind

Returns the kind of target this channel drives.

### SampleCount

Returns the number of samples.

### HasPosition

Returns `true` when the channel holds position samples for the declared sample count. Returns `false` when the sample count is 0.

### HasRotation

Returns `true` when the channel holds rotation samples for the declared sample count.

### HasFieldOfView

Returns `true` when the channel holds field of view samples for the declared sample count.

### HasClipRange

Returns `true` when the channel holds both near and far clip samples for the declared sample count.

### HasLightShading

Returns `true` when the channel holds both light intensity and color samples for the declared sample count.

### HasTargetIndices

Returns `true` when the channel holds look-at target index samples for the declared sample count.

### HasDistance

Returns `true` when the channel holds camera distance samples for the declared sample count.

### HasLookOffset

Returns `true` when the channel holds look-at offset samples for the declared sample count.

### CreateForBake

Creates a channel for baking, with sample arrays allocated for its kind. Both kinds hold rotation. `CameraPose` also allocates position, distance, look-at offset, look-at target index, field of view and clip range; `LightRotation` allocates light intensity and color. All values start at 0.

### WritePosition

Writes the position of sample `index`. Only `CameraPose` holds this value.

### WriteRotation

Writes the rotation of sample `index`. It is not normalized.

### WriteDistance

Writes the camera distance of sample `index`. Only `CameraPose` holds this value.

### WriteLookOffset

Writes the Logical look-at offset of sample `index`. Only `CameraPose` holds this value.

### WriteCameraSettings

Writes the look-at target index, field of view (degrees), near clip and far clip of sample `index`. Only `CameraPose` holds these values.

### WriteLightShading

Writes the light intensity and color of sample `index`. Only `LightRotation` holds these values.

### TryReadPosition

Reads the position of sample `index`. Returns `false` when there are no position samples or the index is out of range, with `value` set to the origin.

### TryReadRotation

Reads the rotation of sample `index`. It is not normalized, so that it matches the stored raw values. Returns `false` when there are no rotation samples or the index is out of range, with `value` set to the identity rotation.

### TryReadDistance

Reads the camera distance of sample `index`. Returns `false` when it cannot be read, with `value` set to 0.

### TryReadLookOffset

Reads the look-at offset of sample `index`. Returns `false` when it cannot be read, with `value` set to the zero vector.

### TryReadTargetIndex

Reads the look-at target index of sample `index`. Returns `false` when it cannot be read, with `targetIndex` set to −1.

### TryReadFieldOfView

Reads the field of view (degrees) of sample `index`. Returns `false` when it cannot be read, with `fieldOfView` set to 0.

### TryReadClipRange

Reads the near and far clip of sample `index`. Returns `false` when they cannot be read, with both set to 0.

### TryReadLightShading

Reads the light intensity and color of sample `index`. Returns `false` when they cannot be read, with `intensity` set to 0 and `color` set to white.

### IsWellFormed

Returns whether the declared sample count matches the array lengths. It is `true` when the sample count is positive and the arrays for rotation and for every value the kind holds have the correct length. This prevents reading corrupted data part way and mixing in values from other samples. `PoseBake.TryValidate` uses it.
