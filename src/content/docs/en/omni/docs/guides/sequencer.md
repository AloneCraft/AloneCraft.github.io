---
title: Sequencer and Pose Bake
description: Bake the camera and light poses of a fixed sequence into time samples in Logical form, and play them back with Timeline.
sidebar:
  order: 9
---

`PoseBake` is an asset that stores the poses of a fixed sequence as samples at equal time intervals, kept in Logical form. When a sequence is fixed, finding targets, redoing parent-child composition, rebuilding the camera distance and look-at point, and deriving the key light direction all depend only on time. Once baked, playback does not need to compute them every frame.

`PoseBake` is not tied to a specific scene. Any scene that drives `Omni.Logical.Transform` and `Light` can use the same format.

## What you need

The code for this feature uses Unity's Timeline (`com.unity.timeline`).

:::note[How to create a bake]
Omni 0.1 has no Editor window or menu for creating a `PoseBake`. Create a `PoseBake` from a script ([Baking from a script](#baking-from-a-script)).
:::

## Parts

All types are in the `Omni.Logical` namespace.

| Type | Role |
| --- | --- |
| `PoseBake` | A ScriptableObject that holds the samples and the information used to check whether they can be used |
| `PoseBakeChannel` | The time samples for one target (a camera, the key light, and so on) |
| `PoseBakeTrack` | A Timeline track that drives an `Omni.Logical.Transform` with baked poses |
| `LightBakeTrack` | A Timeline track that drives a `Light` with baked rotation, intensity and color |
| `PoseBakeClip` | A clip that says which channel of which `PoseBake` drives one span of sequence time |
| `BakePlaybackController` | A component that gathers, in one place for the whole sequence, whether baked data could be used |
| `PoseBakeSampler` | Functions that compute values at any time from the samples |
| `PoseBakeRejection` | The reason baked data could not be used |

## What is stored

| Value | How it is stored |
| --- | --- |
| Position, camera distance, look-at offset | As raw Logical values. Not converted to `float` or `double` |
| Rotation | As a Logical quaternion |
| Field of view, clip range, light intensity, color | As `float` (they are not spatial quantities) |
| Time | The time of the first sample and the interval (seconds, `double`), and the number of samples |
| Check information | The GUID of the baked scene, the GUID of the sequence (the Timeline asset), a content hash, the format version, and the fixed-point configuration |

There are two channel kinds (`PoseBakeChannelKind`).

| Kind | Values held |
| --- | --- |
| `CameraPose` | Position, rotation, field of view, clip range, look-at target index, camera distance, look-at offset |
| `LightRotation` | Rotation, intensity, color. No position (a Directional Light's shading depends only on its direction) |

## Baking from a script

1. Create a channel with `PoseBakeChannel.CreateForBake(targetPath, kind, sampleCount)`. `targetPath` is a stable identifier for the target, such as `"Camera"` or `"KeyLight"`.
2. Write values for each sample. For `CameraPose`, use `WritePosition`, `WriteRotation`, `WriteCameraSettings`, `WriteDistance` and `WriteLookOffset`. For `LightRotation`, use `WriteRotation` and `WriteLightShading`.
3. Create a `PoseBake`, set the check information, timing and channels with `SetData`, and save it as an asset. `SetData` also records the current format version and fixed-point configuration.

```csharp
using UnityEditor;
using UnityEngine;
using L = Omni.Logical;

static class MyShowBaker
{
    // EvaluateShow is your own function that evaluates the sequence at a given time.
    public static L.PoseBake Bake(
        double duration, string sceneGuid, string timelineGuid, string contentHash, string path)
    {
        const int rate = 120;
        double interval = 1.0 / rate;
        int count = (int)(duration * rate) + 1;

        var camera = L.PoseBakeChannel.CreateForBake(
            "Camera", L.PoseBakeChannelKind.CameraPose, count);
        for (int i = 0; i < count; i++)
        {
            EvaluateShow(i * interval, out L.Position3 position, out L.Quaternion rotation);
            camera.WritePosition(i, position);
            camera.WriteRotation(i, rotation);
            camera.WriteCameraSettings(i, 0, 55f, 0.03f, 1000f);
        }

        var bake = ScriptableObject.CreateInstance<L.PoseBake>();
        bake.SetData(
            sceneGuid, timelineGuid, contentHash, System.DateTime.UtcNow.ToString("o"),
            0.0, interval, count, new[] { "MyTarget" }, new[] { camera });
        AssetDatabase.CreateAsset(bake, path);
        return bake;
    }

    static void EvaluateShow(double time, out L.Position3 position, out L.Quaternion rotation)
    {
        // Evaluate your own sequence here
        position = L.Position3.Zero;
        rotation = L.Quaternion.Identity;
    }
}
```

The `targetIndex` of `WriteCameraSettings(index, targetIndex, fieldOfView, nearClip, farClip)` is an index into the table of look-at target IDs (`targetIds`) passed to `SetData`.

## Playing back with Timeline

1. Add `BakePlaybackController` to the GameObject that has the `PlayableDirector`.
2. Add a `PoseBakeTrack` to the Timeline and bind the `Omni.Logical.Transform` to drive. For a light, add a `LightBakeTrack` and bind the `Light`.
3. Put a `PoseBakeClip` on the track, and set Bake to the `PoseBake` and Channel Target Path to the channel identifier. When the clip has no identifier, the track's Channel Target Path is used.

Playback follows these rules.

- Baked data is looked up by sequence time (the clip's start time plus the time elapsed within the clip). A clip points to one `PoseBake` for the whole sequence. Dragging a clip does not invalidate the baked data.
- Clips do not blend or loop. Shot boundaries switch. The clip with the largest weight is used.
- At times outside the baked range, the end samples are held.
- Positions are interpolated linearly in Logical form, and rotations with normalized linear interpolation. Clip ranges are interpolated logarithmically.
- `PoseBakeTrack` writes the `LocalPosition` and `LocalRotation` of the `Omni.Logical.Transform` and syncs them to the Unity Transform. Timeline's Animation Track, which writes the Unity Transform directly, cannot be used because it would disagree with the Logical Transform's values.
- `LightBakeTrack` writes the rotation of the `Light`'s Transform, its intensity (only when greater than 0) and its color.

:::caution[Field of view and clip range]
The mixer that `PoseBakeTrack` creates (`PoseBakeMixerBehaviour`) writes the field of view and clip range to a camera only when its `Camera` property is set. The standard `PoseBakeTrack` does not set this property. With the standard track, therefore, only the pose is written.
:::

## When baked data cannot be used

Each frame, a track checks whether the baked data can be used. When it cannot, the track does not write to its target and reports the reason to `BakePlaybackController`. Deciding the replacement pose, so that something is still shown without baked data, is the job of your code that runs the sequence.

| `BakePlaybackController` API | Details |
| --- | --- |
| `AllChannelsApplied` | `true` when at least one channel has reported and baked data was applied to all of them |
| `FirstRejection` | The first reason among channels that could not be applied |
| `ChannelStates` | The latest state per channel (for diagnostics) |
| `ExternalOverride` | Stops applying baked data for an outside reason. Release it with `PoseBakeRejection.None` |
| `ConfigureExpectations(sceneGuid, sourceGuid, contentHash)` | Sets the scene, sequence and content hash to check against. Empty values are not checked |
| `ResetReports()` | Discards reports. Call it right before switching sequences or seeking |

If you decide separately per camera and per light, you get mismatched images, such as a baked camera with a light computed another way. While `AllChannelsApplied` is `false`, drive every target the same way. The EarthToMoon demo computes and writes the pose from the same sequence definition only while `AllChannelsApplied` is `false`. While the user controls the camera, it sets `ExternalOverride` to `UserCameraOverride`.

### Reasons (`PoseBakeRejection`)

These are the reasons the tracks decide by themselves.

| Value | Meaning |
| --- | --- |
| `MissingAsset` | No `PoseBake` is set on the clip |
| `FormatVersionMismatch` | The data format version differs from the current implementation |
| `PrecisionMismatch` | The fixed-point configuration (total bits, fractional bits) differs between bake time and now. The values are not repacked and are not used |
| `SceneMismatch` | The baked scene differs from the current scene |
| `SourceMismatch` | The baked sequence (Timeline) differs |
| `ContentHashMismatch` | The sequence or placement changed after baking |
| `EmptySamples` | There are no samples, or an array length does not match what was declared |
| `TimeOutOfRange` | There is no active clip |
| `ChannelUnavailable` | The `PoseBake` has no channel with the given identifier |
| `TargetUnresolved` | Nothing is bound to the track as the write target |
| `InterpolationFailed` | Restoring or interpolating a Logical value failed |

The following values are reasons that you set in `ExternalOverride`.

| Value | Meaning |
| --- | --- |
| `UserCameraOverride` | The user is controlling the camera, so the sequence pose must not overwrite it |
| `ObserverEyeRequired` | An observer viewpoint offset is needed. Baked data is in absolute Logical coordinates, so it cannot be used |
| `TargetMoved` | The target's position differs between bake time and now |
