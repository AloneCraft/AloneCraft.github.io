---
description: "A component that switches a RadialCamera on and off during Play or in a Player, to compare standard and logarithmic rendering from the same viewpoint."
---
Put `RadialCameraToggle` on a Camera that has a `RadialCamera`. It switches the [RadialCamera](../radial-camera/) on and off from an on-screen button, a key, or the API.

- It only changes `RadialCamera.enabled`. While disabled, Radial Materials are drawn with the identity mapping and look the same as standard rendering.
- Use it in Scenes that were switched to logarithmic rendering. While the `RadialCamera` is enabled, Renderers under an `Omni.Logical.Transform` whose Materials do not support Radial are not shown. In a Scene that still has standard Materials, switch to logarithmic rendering in Omni Manager first.
- The button is created only during Play, not in Edit Mode.

For usage, see [Switch between standard and logarithmic rendering](../../../docs/getting-started/switch-rendering/).

```csharp
using Omni;
using UnityEngine;

var toggle = Camera.main.GetComponent<RadialCameraToggle>();
toggle.RadialEnabledChanged += enabled => Debug.Log($"Radial: {enabled}");
toggle.SetRadialEnabled(false); // switch to standard rendering
```

<!-- members -->

### m_Target

The `RadialCamera` to switch. If empty, the `RadialCamera` on the same GameObject is used.

### m_ShowButton

Whether to show the toggle button at the top left of the screen during Play. The button shows "Logarithmic Rendering: ON" or "Logarithmic Rendering: OFF". When there is no target, it shows "RadialCamera not found" and cannot be pressed.

### m_Theme

The UI Toolkit theme (`ThemeStyleSheet`) for the button. The text and background colors are set explicitly so that they stay readable regardless of the theme.

### m_SortingOrder

The sorting order of the button's panel. A larger value shows it in front of other UI Toolkit screens.

### m_ToggleKey

The key that toggles on each press. `None` disables the key. This field exists only when the project has the Input System (`com.unity.inputsystem`).

### RadialEnabledChanged

Raised with the new state (`true` means logarithmic rendering) when the enabled state changes. It is not raised by calls that do not change the state.

### Target

The `RadialCamera` to switch. If `m_Target` is empty, it finds and remembers the `RadialCamera` on the same GameObject. `null` if none is found.

### RadialEnabled

`true` if the target `RadialCamera` is enabled (logarithmic rendering). `false` when there is no target.

### SetRadialEnabled

`true` sets logarithmic rendering and `false` sets standard rendering. Returns `true` if the target `RadialCamera` exists; otherwise does nothing and returns `false`.

### Toggle

Swaps logarithmic and standard rendering. Returns `false` if there is no target `RadialCamera`.
