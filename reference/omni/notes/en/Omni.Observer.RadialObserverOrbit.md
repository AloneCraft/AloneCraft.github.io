---
description: "Drives the Cinemachine Virtual Camera of one observer slot every frame: distance, angles and clip range."
---
Drives the Cinemachine Virtual Camera (`CinemachineCamera` and `CinemachineOrbitalFollow`) of one observer slot. It uses Cinemachine (`com.unity.cinemachine` 3.0.0 or later) and the Input System (`com.unity.inputsystem` 1.0.0 or later), so it is excluded from compilation when either is missing.

Every frame it does the following:

- Distance: moves toward the fixed framing distance of the [RadialObserverRig](../radial-observer-rig/), smoothing in log space, and writes it to the OrbitalFollow `Radius`.
- Lens: writes the slot's `FieldOfView` as the field of view. The near and far clip planes are chosen from the distance and the framing radius.
- Angles: for a fixed slot, writes the slot's `Yaw` and `Pitch` to the horizontal and vertical axes. For the orbit slot, adds the automatic orbit and the mouse input to the axes.

While the rig has no framing result (for example when there is no source), it writes nothing. This component does not have `[ExecuteAlways]`, so it does not run in Edit Mode. See [Radial Observer Camera](../../../docs/guides/observer-camera/) for the controls.

<!-- members -->

### m_Rig

The [RadialObserverRig](../radial-observer-rig/) whose framing result is read.

### m_Slot

The [RadialObserverSlot](../radial-observer-slot/) this Virtual Camera belongs to. The kind, angles, field of view, and the Camera used to test the mouse position are read from it.

### m_VirtualCamera

The `CinemachineCamera` whose lens (field of view, near clip, far clip) is written. When unset, the lens is not written.

### m_Orbital

The `CinemachineOrbitalFollow` whose distance and angles are written. When unset, nothing is done.

### m_SpinDegreesPerSecond

The automatic orbit speed of the orbit slot (degrees per second). Used only by the orbit slot. Time is measured with `Time.unscaledDeltaTime`, so `Time.timeScale` has no effect.

### m_ResumeDelaySeconds

For the orbit slot, the time (seconds) after the last input before the automatic orbit resumes.

### m_DragDegreesPerPixel

For the orbit slot, the rotation angle (degrees) per pixel of right-button drag. Dragging only works while the mouse is inside the slot.

### m_ZoomPerScrollUnit

For the orbit slot, the zoom strength per unit of wheel scroll. The OrbitalFollow radial axis value is multiplied by `exp(−scroll × this value)`.

### m_DistanceSmoothSeconds

The smoothing time (seconds, 0.01 or more) used to move the distance toward its target. SmoothDamp is applied to the logarithm of the distance, so the camera moves smoothly between distances of different orders of magnitude.

### Configure

Sets the rig, slot, `CinemachineCamera` and `CinemachineOrbitalFollow` from code. Intended for prefab generation and tests.
