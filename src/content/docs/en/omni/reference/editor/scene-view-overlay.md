---
title: Scene View Overlay
description: Every item in the Omni overlay of the Scene View, with when it appears, its choices and its default.
sidebar:
  order: 3
---

This page lists the items in the Scene View overlay "Omni", from top to bottom. For workflows, read [Working in the Scene View](../../../docs/guides/scene-view/).

## Showing the overlay

- The overlay is named "Omni". Show or hide it from the Scene View's overlay menu. It is shown by default.
- Each Scene View keeps its own item states. Multiple Scene Views do not share their RadialCamera or viewpoint selections.
- When you hide the overlay, that view returns to standard display. For details, read [When you hide the overlay](../../../docs/guides/scene-view/#when-you-hide-the-overlay) in the guide.
- When you open "Controls and Display" and "Debug" and the items no longer fit, the inside of the overlay scrolls.

## Item layout

| Position | Item | When it appears |
| --- | --- | --- |
| Top | [RMB Rotation Basis](#rmb-rotation-basis) | Always |
| Below | [RadialCamera and the items under it](#items-in-perspective) | Perspective Scene View |
| Below | [Logarithmic Grid, Data Type](#items-in-orthographic) | Orthographic Scene View |
| Scroll area | [Omni Manager](#omni-manager) | Always |
| Scroll area | [Warnings, the Nav line, Logical position and angle](#logical-position-and-angle) | Always |
| Scroll area | [Controls and Display](#controls-and-display) | Always. Collapsed by default |
| Scroll area | [Debug](#debug) | Always. Collapsed by default |

## RMB Rotation Basis

A dropdown that selects the basis for looking around with a right-drag.

| Choice | Behavior |
| --- | --- |
| World (default) | Uses the world Y axis as the basis for horizontal look. Near a straight up or down view, switches smoothly to the camera up axis captured when the right button was pressed, to prevent screen roll |
| View | Always uses the camera up axis captured when the right button is pressed |

- Switching to "World" keeps the eye position and returns the view direction to a level world orientation.
- It cannot be changed in 2D mode or while rotation is locked.
- The selected value is saved in the project settings (`ProjectSettings/OmniEditorSettings.asset`).

## Items in perspective

### RadialCamera

Selects the `RadialCamera` whose compression settings (mode, radius, threshold, Logical near/far and view scale) the view uses. The button shows the name of the selected camera. Press it to open a menu of choices.

| Choice | Details |
| --- | --- |
| Standard Camera | Draws this view with standard Scene View rendering |
| `camera name — scene name › parent › …` | Draws this view in Radial with that camera's `RadialCamera` settings |

- The choices are the cameras in loaded scenes that have an enabled `RadialCamera`. Cameras in prefab assets, preview scenes and objects that are not saved are not listed.
- The order is: closer to the Hierarchy root first, then earlier loaded scenes, then higher in the Hierarchy.
- A new Scene View selects the first camera in that order once, automatically. While there are no candidates, it selects one when a candidate appears. After a selection has been made once (including "Standard Camera"), it does not select again automatically.
- When the selected camera is gone, or its `RadialCamera` is disabled, the view returns to "Standard Camera". It does not switch to another camera automatically.
- The selected camera does not need to render a Game View.
- The selection is kept per Scene View until you close the Editor.

The following items appear under RadialCamera only while something other than "Standard Camera" is selected. With "Standard Camera" they are all hidden.

### Viewpoint

| Choice | Details |
| --- | --- |
| Objective | Uses the selected `RadialCamera`'s position as the compression center, and views that compressed space from this Scene View (comparison display) |
| Subjective (default) | Applies the selected `RadialCamera`'s settings to this Scene View, and views the Logical space centered on the Scene View itself |

The selection is kept per Scene View until you close the Editor. For the difference, read [Subjective and Objective](../../../docs/guides/scene-view/#subjective-and-objective) in the guide.

### Open Source Inspector

When checked, opens the source `RadialCamera` in a Property Editor window locked to that target. Unchecking it closes the window. The Hierarchy selection does not change. If you close the window by hand, the check is cleared too.

### Align to Source Camera

A button that, only at the moment you press it, aligns the Scene View's position, orientation and vertical field of view to the source camera. It does not keep them aligned.

- When the source camera is orthographic, only the position and orientation are aligned. The field of view is not.
- When the source camera uses part of a split screen, the vertical framing matches, but the horizontal framing does not.

### Before Warp, After Warp, Connect Corresponding Points

Comparison display settings. They appear only when Viewpoint is "Objective".

| Item | Choices | Default |
| --- | --- | --- |
| Before Warp | How the positions before the mapping are shown: None (hidden), Wireframe or Shading | None |
| After Warp | How the appearance after the mapping is shown: None, Wireframe or Shading | Shading |
| Connect Corresponding Points | When on, draws lines between corresponding vertices before and after the warp | Off |

When the comparison display cannot be drawn, or cannot be drawn in full, one of the following lines appears below the items. It appears only while at least one item is not None or off.

| Message | Meaning |
| --- | --- |
| ⚠ Comparison display is not rendered because no reference camera with a valid RadialCamera was found. | There is no source camera |
| ⚠ This scene has no targets with a Radial material. Apply a radial material in Omni Manager. | There are no comparison targets |
| No comparison targets are on screen (all are outside the view frustum). | All targets are off screen |
| {0} items are not rendered due to the load limit (drawn nearest-first). | Items over the limit are not drawn |

## Items in orthographic

In an orthographic Scene View, the following items appear instead of RadialCamera and the items under it. This is a separate feature from the perspective settings and shares no values with them.

| Item | Details | Default |
| --- | --- | --- |
| Logarithmic Grid | Shows a grid where the center 50% of the screen is an undistorted rectangle and the outside is logarithmically compressed. While on, the objects in the scene are also drawn with the same projection | Off |
| Data Type | The value the screen edge corresponds to. `Float` is `float.MaxValue`; `Logic` is the maximum Logical value | `Float` |

The amount of compression is derived automatically from the camera's `orthographicSize` and aspect ratio. There are no items to adjust it.

## Omni Manager

Checking it opens the Omni Manager window; unchecking it closes the open windows. If you open or close the window another way, the check follows the window. While Omni Manager is not ready to open, such as during Editor startup, it cannot be changed. For the window's contents, read [Omni Manager](../omni-manager/).

## Logical position and angle

Fields that show the Logical position and angle of the Scene View's viewpoint. They are not the values of the Unity Transform.

| Item | Details |
| --- | --- |
| Position X / Y / Z | The viewpoint's Logical position (m). Enter a value and press Enter to move there. Accepts decimal and scientific notation (such as `1.5e12`) |
| Angle X / Y / Z | The viewpoint's orientation as world Euler angles (degrees). Enter a value and press Enter to change the orientation while keeping the position |

- While you are not editing them, the position fields show three significant digits in scientific notation. When you select a field, it shows the full-precision value.
- When you rewrite one axis, the other two axes keep their full-precision values. The display rounding never moves the viewpoint.
- The angle fields are not yaw, pitch and roll controls.
- While you are editing a field, it is not overwritten even if the viewpoint moves.

Above the fields, the following lines appear only when needed.

| Line | Details |
| --- | --- |
| ⚠ Could not move the position (enter a finite Logical decimal or scientific value). | The value in a position field was not accepted. It disappears the next time you commit a position or angle, or start editing a field |
| ⚠ Could not change the orientation (enter a finite angle within the floating-point representable range). | The value in an angle field was not accepted |
| ⚠ The high-precision viewport is disabled ({0}). Outside the real-space extent (viewpoint), rendering will be incorrect. | In Radial display, something is disabling high-precision rendering. {0} shows the reason |
| ⚠ This position cannot be represented as a float (outside the real-space extent (viewpoint)). It is only rendered correctly in the logarithmic (Radial) Scene View. | The viewpoint is at a position that `float` cannot represent. The position is not restricted |
| The Nav line | See the table below |

The Nav line shows the distance to the reference that the wheel approaches, and the speed of flying with right-drag + W/A/S/D/Q/E.

| Message | When it appears |
| --- | --- |
| Nav: {0} to {1}'s surface ｜ flight speed {2} | The distance to the selected target's surface can be determined |
| Nav: {0} to pivot ｜ flight speed {1} | There is no selection, or the distance to the surface cannot be determined |
| Nav: unavailable ｜ flight speed {0} | The distance to the pivot cannot be determined |
| Navigator: orthographic ready | Orthographic Scene View |

During an operation, the Nav line may change to show the state of that operation.

## Controls and Display

A collapsible section. Its contents appear in the following order.

### Render Path, Real-Space Extent (Object)

| Item | Details | Default |
| --- | --- | --- |
| Render Path | `Auto` uses the original Renderer only for targets whose full AABB is within the real-space extent (object). `Force Proxy` uses the bounded-render proxy even for targets within it | `Auto` |
| Real-Space Extent (Object) | The ± threshold (m) for the full AABB, including the mesh asset bounds. There is no hysteresis. This is separate from the real-space extent (viewpoint) on the viewpoint side | 10000 m |

These two values are shared by all Scene Views. They are kept until you close the Editor.

### Safety Boundary

| Item | Details | Default |
| --- | --- | --- |
| Show Safety Boundary Faces | Shows the translucent X/Y/Z faces of the finite Scene View safety boundary. The boundary edges are shown even when this is off. Leave it off for normal editing | Off |

### Grid

| Item | Details | Default |
| --- | --- | --- |
| Grid Follows Camera | When off, the three planes are fixed at the origin (X=0, Y=0, Z=0). When on, they snap to the grid step at the camera position and follow it, so the grid stays on screen even far from the origin | Off |
| Grid Base (10^n m) | The base size of one grid cell, as 10 to the n meters. It is a lower bound; the grid is thinned out by powers of 10 according to the apparent spacing on screen. The range is from −4 up to the limit that Logical numbers can represent | 0 (1 m) |
| X row | When checked, draws the plane where X is constant (the YZ plane). The color field sets its color and opacity (alpha) | On, reddish |
| Y row | When checked, draws the plane where Y is constant (the XZ plane). The color field works the same way | On, greenish |
| Z row | When checked, draws the plane where Z is constant (the XY plane). The color field works the same way | On, bluish |
| Reset Grid to Defaults | Resets Grid Follows Camera, plane visibility and colors to their defaults. Does not change Grid Base | — |

The grid settings are shared by standard and Radial display.

### Custom Navigation

Settings for flight speed and wheel approach. The values are saved in the project settings (`ProjectSettings/OmniEditorSettings.asset`).

| Item | Details | Default |
| --- | --- | --- |
| Auto Speed Seconds | The base flight speed is the speed that covers the distance to the selected target's surface (or to the pivot when nothing is selected) in about this many seconds | 2.5 s |
| Fallback Speed | The speed (m/s) used only when the base speed cannot be determined automatically (for example, inside the target) | 10 m/s |
| Flight Speed Step Ratio | The multiplier per step of right button + wheel. There is no limit to the number of steps | 2 |
| Approach Shrink Ratio | How much of the remaining distance one wheel step closes. Choose from Reduce by 5% (default), Reduce by 10%, Reduce by 20%, Reduce by 30%, Reduce by 50% and Reduce by 70% | Reduce by 5% |
| Approach Minimum Distance | The lower bound (m) of the remaining distance to the surface when approaching with the wheel | 0.000001 m |
| Wheel Sensitivity | Advances one step each time the wheel amount crosses this value | 1 |
| Max Steps Per Event | The maximum number of steps one wheel event can advance during a fast wheel spin | 8 |
| Invert Look X | Inverts the horizontal direction of the right-drag look | Off |
| Invert Look Y | Inverts the vertical direction of the right-drag look | Off |

### View Editing

Buttons that move the viewpoint while keeping the focus point and distance.

| Item | Details |
| --- | --- |
| Editing scale | The size of the target last focused with F |
| Left 15°, Right 15°, Up 15°, Down 15° | Rotates the viewpoint by 15° while keeping the focus point and distance |
| One step closer, One step further | Multiplies the distance to the focus point by 5/6 or 6/5. In orthographic, multiplies the view size by 1/1.2 or 1.2 |
| Return to the view before focus | Returns to the viewpoint before you focused with F. Up to 16 history entries are kept; each press goes back one |

- These buttons work only in Radial display with Viewpoint set to "Subjective".
- The 15° buttons do not work in 2D mode or while rotation is locked.
- "Return to the view before focus" does not work when there is no history.
- The same operations are in the Shortcut Manager as `Omni/Editing/*`. They have no default keys. Read [Shortcuts and Menus](../shortcuts-and-menus/).
- Apart from the section heading, the text in this section (buttons, editing scale, notice) is shown in Japanese regardless of the Editor language setting. The English names above are translations: 左15° / 右15° / 上15° / 下15°, 1段近づく / 1段離れる, フォーカス前の視点へ戻る, 編集尺度.

## Debug

A collapsible section of diagnostics, for cases such as missing objects. Keep it closed for normal editing.

| Item | Details |
| --- | --- |
| Use Culling | When off, all Omni-originated culling and hiding are disabled (fail-open) so that you can find the cause of missing objects. The bounded-render proxy stays active because it is a safety mechanism for drawing. On by default. Shared by all Scene Views and kept until you close the Editor |
| Culling is disabled — … | A warning shown only while Use Culling is off |
| Selected | For the Renderer of the selected GameObject: its render path, reason, hiding layer, frustum test, real distance and render depth, and culling bounds state |
| Bounds overrides: {0}  Unsupported: {1} | The number of Renderers whose culling bounds are overridden, and the number set aside as unsupported |
| Culling range | The culling range used in the last frame (render near/far and the real distances they correspond to) |
| Logical Near Clip | The Logical near clip and the render near used in the last frame |
| Warp path | The mapping path in the last frame: logarithmic (Radial), or identity (no warp) with its cause |
| Eye override | Whether the eye position was overridden to the warp center in the last frame |
| Trackball Orbit | The last Alt + left-drag orbit input and the Logical pivot held during it |
| Invariants | The result for each monitored invariant. A violation keeps the state from when it first occurred |
| Clear Invariant Records | Clears the kept violation records and the first-log suppression state |
| Hidden this frame | The number of objects each layer hid in the last frame |
| Unsupported Renderer/Material | The list of Renderers and Materials set aside as unsupported |
| Materials are not auto-generated. … | A note that Materials are made in Omni Manager's Materials/Shaders tabs |

The Debug contents are updated only while the section is open.
