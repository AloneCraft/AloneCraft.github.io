---
description: "A rig that views the warped world seen by a RadialCamera from outside, through separate cameras (observer slots)."
---
A rig that lets you pull back and view the "warped world" that a `RadialCamera` sees, from separate cameras. It registers each observer slot's Camera as an observer whose warp center is the source `RadialCamera`, and draws it under the same contract as the Scene View comparison display: only the targets are compressed around the center C, and the observer Camera's viewpoint stays at its physical position.

- Normally you place it, prefab and all, with GameObject > Omni > Radial Observer Camera. See [Radial Observer Camera](../../../docs/guides/observer-camera/) for the structure, controls and rules.
- The rig's origin is a finite render space that corresponds to the center C. The rig itself does not move.
- The camera distance is not derived from a bounding sphere or similar; it is a fixed distance of `DistanceInScale` × S. S is the Radial scale (`RadialSettings.RadialScale`), currently the constant 1.
- By default it runs only in Play Mode. Turn on `RunInEditMode` to run it in Edit Mode.

For slot placement see [ObserverLayoutPreset](../observer-layout-preset/) and [ObserverCorner](../observer-corner/); for per-slot settings see [RadialObserverSlot](../radial-observer-slot/).

<!-- members -->

### m_Source

The `RadialCamera` whose warp center and curve are borrowed. When unset, the rig uses the MainCamera's `RadialCamera`, otherwise it searches the Scene for a `RadialCamera` (the automatic search runs at most once every 0.5 seconds). If none is found, it logs a warning to the Console. If the source is disabled, it reports this with a warning and a label at the bottom right of the screen. In both cases the slots are hidden.

### m_DistanceInScale

How many times the Radial scale S the distance from the center C to the observer camera is (5–200). Since S is currently 1, the distance in render space equals this value.

### m_Layout

How the slots are arranged. See [ObserverLayoutPreset](../observer-layout-preset/) for the values.

### m_Corner

The screen corner the slots are placed against. See [ObserverCorner](../observer-corner/) for the values.

### m_OrbitWipeArea

The side length of the orbit slot in `OrbitWipe`, as a fraction of the screen width and height. Clamped to 0.05–1.

### m_GridArea

The side length of the area used by `FixedGrid` and `Combined`, as a fraction of the screen width and height. Clamped to at least 0.05. The upper limit is 1 for `FixedGrid` and 0.8 for `Combined` (because `Combined` is 1.25 times this value wide).

### m_BaseDepth

The base value for the observer slots' `Camera.depth`. Each slot's depth is this value plus the slot index. Make it larger than the main camera's depth.

### m_IncludeUnwarpedCullingVolume

When on, the observer culling also includes the ordinary view frustum. This is the same setting as the Scene View comparison display. It is read at registration time, so a change takes effect when the source is registered again.

### m_RunInEditMode

When on, the slots are registered and shown in Edit Mode as well (when not playing). A rig with this off changes nothing in Edit Mode.

### m_Focus

The look-at Transform shared by all slots. Make it a direct child of the rig. Every update the rig moves this Transform's `localPosition` toward the center C (the rig's origin), with smoothing. If it is unset when the rig starts, the rig logs an error to the Console.

### m_Slots

The array of observer slots ([RadialObserverSlot](../radial-observer-slot/)). The rig registers, places, and enables or disables the Cameras of the slots in this array.

### Focus

Returns the look-at Transform (`m_Focus`).

### ActiveSource

Returns the source `RadialCamera` currently being observed. It is `null` while none is found and while the source is disabled.

### DistanceInScale

Gets or sets how many times S the distance from the center to the camera is. A set value is not clamped to the Inspector range (5–200). Values of 0 or less and non-finite values are treated as 40 when the frame is built.

### RunInEditMode

Gets or sets whether the rig also runs in Edit Mode. Turning it off in Edit Mode unregisters the slots and hides them on the next update.

### Layout

Gets or sets how the slots are arranged. Applied to the placement from the next update.

### Corner

Gets or sets the screen corner the slots are placed against. Applied to the placement from the next update.

### TryGetFrame

Returns the framing result (`ObserverFrame`) computed in the latest update. Returns `false` when no source was found or the source's Logical position could not be resolved. `ObserverFrame` holds the look-at point `Focus` (always the origin), the camera distance `FixedDistance` (`DistanceInScale` × S), and the radius used to choose the near and far clip planes, `BoundingRadius` (0.4 times the distance).

### Configure

Sets the focus, the slot array and the source `RadialCamera` from code. Intended for prefab generation and tests. A `null` `slots` becomes an empty array. When `source` is omitted, the source is searched for automatically.
