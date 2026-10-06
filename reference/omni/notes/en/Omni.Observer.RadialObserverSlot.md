---
description: "Settings for one observer slot: its Camera, layout slot index, kind, angles and field of view."
---
Settings for one observer slot. It holds the slot's Camera and its slot index in the layout. [RadialObserverRig](../radial-observer-rig/) decides the on-screen placement and visibility, and [RadialObserverOrbit](../radial-observer-orbit/) drives the camera distance and angles through Cinemachine.

The slots in the bundled prefab are:

| Index | Slot | Kind | Yaw | Pitch |
| --- | --- | --- | --- | --- |
| 0 | Orbit | `Orbit` | 0 | 25 |
| 1 | Front | `Fixed` | 0 | 0 |
| 2 | Right | `Fixed` | 90 | 0 |
| 3 | Top | `Fixed` | 0 | 89 |
| 4 | Iso | `Fixed` | 45 | 35.264 |

Orthographic Cameras are not used. With an orthographic projection Radial is always the identity mapping, so the warped world cannot be observed. If an orthographic Camera is assigned, a warning is logged to the Console when a value is changed in the Inspector. See [Radial Observer Camera](../../../docs/guides/observer-camera/) for details.

<!-- members -->

### m_Camera

This slot's Camera. Do not add a `RadialCamera` to it. The rig logs a warning and does not register a Camera that has one.

### m_Index

The slot index in the layout (0–4). 0 is the orbit slot position, and 1–4 are the fixed slot positions (Front, Right, Top, Iso).

### m_Kind

The slot kind. `Orbit` orbits automatically and can be controlled with the mouse. `Fixed` keeps a fixed angle.

### m_Yaw

The horizontal angle of a fixed slot (degrees, −180 to 180). Used as the value of the Cinemachine OrbitalFollow horizontal axis. Not used by `Orbit` slots.

### m_Pitch

The elevation angle of a fixed slot (degrees, −89 to 89). Used as the value of the Cinemachine OrbitalFollow vertical axis. Not used by `Orbit` slots.

### m_FieldOfView

The vertical field of view of this slot's camera (degrees, 5–120).

### Camera

Returns this slot's Camera. When unset, it finds the first Camera on this GameObject or its children (including inactive ones) and remembers it.

### Index

Returns the slot index in the layout.

### Kind

Returns the slot kind.

### Yaw

Returns the horizontal angle of a fixed slot (degrees).

### Pitch

Returns the elevation angle of a fixed slot (degrees).

### FieldOfView

Returns the field of view (degrees).

### Configure

Sets the Camera, slot index, kind, horizontal angle, elevation angle and field of view from code. Intended for prefab generation and tests. The slot index is clamped to 0–4; the other values are not clamped.
