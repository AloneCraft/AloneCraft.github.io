---
description: "Separates Cinemachine channels so that an observer slot's Virtual Camera is picked up only by that slot's Brain."
---
Makes an observer slot's Virtual Camera visible only to that slot's `CinemachineBrain`. A `CinemachineBrain` Channel Mask includes all channels by default, so if another Brain in the Scene (such as the game's main camera) picks up an observer Virtual Camera, the main camera moves to the observer position. This component prevents that.

When enabled, it does the following:

1. Sets the Virtual Camera's Output Channel to the dedicated channel for the slot index (Channel01–05).
2. Sets the slot Brain's Channel Mask to that channel only.
3. Removes that channel from the Channel Mask of every other Brain present in the Scene at that moment (excluding Brains on inactive GameObjects).

When disabled, it adds the channel back to the Brains changed in step 3. It does not restore the settings of the slot's own Brain and Virtual Camera.

:::caution
Brains created after the component was enabled are not covered. Remove the observer channels from those Brains' Channel Mask yourself.
:::

It is excluded from compilation when Cinemachine or the Input System is missing. See [Radial Observer Camera](../../../docs/guides/observer-camera/) for details.

<!-- members -->

### m_Slot

The slot ([RadialObserverSlot](../radial-observer-slot/)) that decides the channel. If any of `m_Slot`, `m_Brain` or `m_VirtualCamera` is unset, nothing is done.

### m_Brain

The `CinemachineBrain` of this slot's Camera. Its Channel Mask is set to the slot's dedicated channel only.

### m_VirtualCamera

This slot's `CinemachineCamera`. Its Output Channel is set to the slot's dedicated channel.

### ChannelForSlot

Returns the dedicated observer channel for a slot index. Slot 0 is Channel01 and slot 4 is Channel05. The index is clamped to 0–4.

### Configure

Sets the slot, Brain and Virtual Camera from code. Intended for prefab generation and tests. The channels are configured the next time the component is enabled.
