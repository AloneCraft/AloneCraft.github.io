---
description: "A component that collects, in one place for the whole sequence, whether baked data could be used."
---
A component that collects, in one place for the whole sequence, whether baked data could be used. Add it to the same GameObject as the `PlayableDirector`.

- [PoseBakeTrack](../pose-bake-track/) and [LightBakeTrack](../light-bake-track/) find this component on the Director's GameObject and take the GUIDs and hash to validate against, and `ExternalOverride`, from it. Every frame they report, per channel, whether baked data could be applied.
- The code that drives the sequence looks only at this one place to decide whether to use the baked data or drive everything with its own fallback computation. Splitting the decision between camera and light produces an inconsistent picture, such as a baked camera with a light computed some other way. While `AllChannelsApplied` is `false`, drive all targets the same way.

The tracks also work without this component; in that case they do not check the Scene, sequence or content hash, and do not report. See [Sequencer and Pose Bake](../../../docs/guides/sequencer/) for details.

<!-- members -->

### m_ExpectedSceneGuid

The Scene GUID to validate against. When empty, it is not checked.

### m_ExpectedSourceGuid

The sequence (Timeline asset) GUID to validate against. When empty, it is not checked.

### m_ExpectedContentHash

The content hash to validate against. When empty, it is not checked.

### ExternalOverride

Stops baked poses from being applied for external reasons. When set to anything other than `PoseBakeRejection.None`, the tracks write to no channel and report this value as the reason. Set `None` to clear it. Use it, for example, with `UserCameraOverride` while the user is controlling the camera.

### ExpectedSceneGuid

Returns the Scene GUID to validate against.

### ExpectedSourceGuid

Returns the sequence GUID to validate against.

### ExpectedContentHash

Returns the content hash to validate against.

### ConfigureExpectations

Sets the Scene GUID, sequence GUID and content hash to validate against. An empty value means that item is not checked. The values are written to serialized fields, but saving them as an asset change in the Editor is up to the caller.

### ReportChannel

Records the current state of one channel. The track mixers call it every frame. A report with the same identifier (case-sensitive) is replaced; otherwise a new one is added.

### AllChannelsApplied

Returns `true` when at least one channel has reported and baked data could be applied to all of them. Returns `false` when there are no reports.

### FirstRejection

Returns the reason of the first-reported channel that could not be applied. Returns `PoseBakeRejection.None` when all were applied or there are no reports.

### ChannelStates

Returns the latest state of each reported channel, for diagnostic display. Each element (`ChannelState`) holds the identifier `TargetPath`, whether it was applied (`Applied`), and the reason (`Rejection`).

### ResetReports

Discards all reports. Call it right before switching sequences or seeking, so that no state from the previous sequence remains. Reports are never cleared automatically, so the state of a channel whose track stopped running stays until you call this.
