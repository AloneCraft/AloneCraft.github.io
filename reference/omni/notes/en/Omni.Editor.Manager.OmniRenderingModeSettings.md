---
description: "An Editor static class that records in the project the rendering mode (logarithmic or standard) chosen with the Omni Manager switch."
---
`OmniRenderingModeSettings` records the rendering mode chosen with "Standard Rendering / Logarithmic Rendering" in [Omni Manager](../omni-manager/) to `ProjectSettings/Omni/RenderingMode.json` ([Files Omni creates](../../files/project-files/)). It is Editor-only.

A `RadialCamera` added to a Camera is a scene change, so it is lost unless saved. Therefore, while logarithmic is recorded, Omni automatically adds and enables a `RadialCamera` on the target Cameras at these times:

- When a scene is opened, after a domain reload, and when returning to Edit Mode after Play
- When Play starts, when a scene is loaded during Play, and when a Player is built

A `RadialCamera` added automatically is not recorded in Undo, does not mark the scene as modified, and is not written to the scene file. When the record is no longer logarithmic, the automatically added ones are removed. Even when standard is recorded, `RadialCamera` components saved in a scene are not removed.

The values of `Mode` (`OmniRenderingModeSettings.RenderingMode`) are:

| Value | Meaning |
| --- | --- |
| `Unset` | Nothing recorded. The file does not exist or cannot be read. Nothing happens automatically |
| `Standard` | Standard rendering is recorded |
| `Logarithmic` | Logarithmic rendering is recorded. `RadialCamera` is added automatically |

For the switching flow, see [Switch between standard and logarithmic rendering](../../../docs/getting-started/switch-rendering/).

<!-- members -->

### Mode

The recorded rendering mode. It is read from the file the first time and the remembered value is returned after that. `Unset` when the file does not exist or cannot be read (a warning is logged to the Console when it cannot be read).

### AutoApplyActive

`true` when the automatic addition of `RadialCamera` is active: only when `Mode` is `Logarithmic` and it is not stopped by `Suspend`.

### SetMode

Records the rendering mode and applies it to the loaded scenes right away: with logarithmic it adds `RadialCamera`, otherwise it removes the automatically added ones. It only records; it does not perform the switch itself, such as converting Materials. It does nothing when the value is the same as now or while stopped by `Suspend`.

### Suspend

Temporarily stops the automatic addition, for example while tests run. Calls are counted, so call `Resume` the same number of times. While stopped, `SetMode` does not change the record either.

### Resume

Cancels one `Suspend`. When every `Suspend` is cancelled, the automatic addition works again. Does nothing if `Suspend` has not been called.
