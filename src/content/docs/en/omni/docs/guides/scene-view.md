---
title: Working in the Scene View
description: Use Omni's overlay to show the Scene View in Radial and move the viewpoint in Logical coordinates.
sidebar:
  order: 1
---

Omni adds an overlay named "Omni" to the Scene View. With this overlay you switch each view between standard and Radial display, and handle the viewpoint's position and angle in Logical coordinates. The Scene View's Unity camera is a display copy; the source of truth for the viewpoint is its Logical position and orientation.

## Overlay items

In a perspective Scene View, the overlay shows the following items.

| Item | Details |
| --- | --- |
| RMB Rotation Basis | The basis for looking around with a right-drag. Choose "World" or "View" |
| RadialCamera | Choose the `RadialCamera` in the scene whose compression settings the view uses. Choose "Standard Camera" to show the view normally |
| Viewpoint | Choose "Subjective" or "Objective". Shown only while a RadialCamera is selected |
| Open Source Inspector | Opens or closes the source `RadialCamera` in a Property Editor locked to it. The Hierarchy selection does not change |
| Align to Source Camera | At the moment you press it, aligns the Scene View's position, orientation and vertical field of view to the source camera |
| Before Warp, After Warp, Connect Corresponding Points | Comparison display settings. Shown only in "Objective" |
| Omni Manager | Opens or closes the Omni Manager window. Stays in sync with the window |
| Position, Angle | The viewpoint's Logical position and angle. Enter a value and press Enter to move to that position or orientation |
| Controls and Display | Settings for the render path, view editing, safety boundary, grid and navigation |
| Debug | Diagnostic displays |

"World" in RMB Rotation Basis uses the world Y axis as the basis for horizontal look. Near a straight up or down view, it switches smoothly to the camera up axis captured when the right button was pressed, so the screen does not roll. "View" always uses the camera up axis captured when the right button is pressed.

The coordinate fields show the Logical position and angle, not the values of the Unity Transform. The angle fields are world Euler angles in degrees; they are not yaw, pitch and roll controls.

:::note
Multiple Scene Views do not share their RadialCamera or viewpoint selections.
:::

### When you hide the overlay

When you hide the overlay, that view returns to standard display and any operation in progress ends. The Logical position, view direction, pitch, yaw and zoom are kept. Only the screen roll returns to Unity's standard up direction. When you show the overlay again, you can continue with the same RadialCamera selection and RMB Rotation Basis.

## Subjective and Objective

| Viewpoint | What you see |
| --- | --- |
| Subjective (default) | Applies the selected `RadialCamera`'s settings to the Scene View and draws in Radial centered on the Scene View itself |
| Objective | Uses the selected `RadialCamera`'s position as the compression center, and views that compressed space from the Scene View's viewpoint (comparison display) |

In Subjective, the mapping keeps view directions. So within the range that standard rendering can show, the image is the same as standard rendering. Anything farther is visible only in the Radial display.

"Align to Source Camera" is not a continuous sync. After aligning, you can keep operating by hand as usual.

- When the source camera is orthographic, only the position and orientation are aligned; the field of view is not.
- When the source camera uses part of a split screen, the vertical framing matches, but the horizontal framing does not, because its aspect ratio differs from the Scene View window.

### Comparison display

In Objective, you compare the shapes before and after the mapping from the same Scene View viewpoint.

| Item | Details | Default |
| --- | --- | --- |
| Before Warp | How positions before the mapping are shown: None (hidden), Wireframe or Shading | None |
| After Warp | How positions after the mapping are shown: None, Wireframe or Shading | Shading |
| Connect Corresponding Points | Draws lines between corresponding vertices before and after the warp | Off |

- The comparison display is a state of the Scene View. It does not change the Game View, the Player, saved scene data or the Selection. It also does not change renderer enabled states, GameObject active states or the Hierarchy visibility icon.
- When both Before Warp and After Warp are None and Connect Corresponding Points is off, there are zero comparison targets. The background, grid and camera/light gizmos remain.
- Before Warp's Shading draws up to 256 objects, nearest first. The number not drawn appears in the overlay as "{0} items are not rendered due to the load limit (drawn nearest-first)." This does not affect selection.

When the comparison display cannot be drawn, the overlay shows the reason.

| Message | What to do |
| --- | --- |
| ⚠ Comparison display is not rendered because no reference camera with a valid RadialCamera was found. | Prepare a camera with an enabled `RadialCamera` and choose it in the RadialCamera field |
| ⚠ This scene has no targets with a Radial material. (rest omitted) | Switch to logarithmic rendering in Omni Manager ([Switch between standard and logarithmic rendering](../../getting-started/switch-rendering/)) |
| No comparison targets are on screen (all are outside the view frustum). | Point the view at the targets |

## Moving the viewpoint

Omni's viewpoint controls work in Logical coordinates. Even far from the origin, movement is not lost to `float` rounding.

| Control | Movement |
| --- | --- |
| F, double-click in the Hierarchy | Frames the selected object. Repeating on the same object alternates between a far and a near framing |
| Wheel | Moves toward or away from the selected object. The step is proportional to the remaining distance to its surface |
| Middle-button drag | Pans so that the surface point under the cursor follows the cursor 1:1. Objects do not move |
| Alt + left-drag | Orbits around the current pivot |
| Right-drag | Looks around. The eye position does not move |
| Right-drag + W/A/S/D/Q/E | Flies. Turn the wheel while holding the right button to change the speed multiplier |
| Hover | Outlines the frontmost renderer under the pointer in cyan. The selection does not change |

- F is `Omni/Frame Logical Selection` in Edit > Shortcuts. It works in standard display, Objective and Subjective.
- When you select a parent and press F, the view is framed on the center and size of the region that encloses all child renderers. When you select a single child, only that child is framed.
- When a middle-button drag grabs the background, it does a normal pan around the current pivot.
- Flight speed follows the distance to the selected object's surface. With nothing selected, it is 4 times the speed derived from the distance to the visible objects.
- The overlay shows the distance to the pivot or to the selected object's surface and the flight speed, in the form "Nav: {0} to pivot ｜ flight speed {1}".
- Look and Orbit do not change the behavior of 2D mode or rotation lock.

After you switch scenes, the view frames the new scene automatically, once, only when nothing from the new scene is on screen and the viewpoint is far enough away. If something is already visible, or an operation is in progress, the viewpoint does not change.

### View editing helpers

"View Editing" in "Controls and Display" has buttons that rotate the view by 15° (left, right, up, down) while keeping the pivot and distance, buttons that move closer or farther by a fixed ratio, and a button that returns to the viewpoint before the last focus. These work only in the Subjective Radial display.

The same actions are also in Edit > Shortcuts under `Omni/Editing`. No keys are assigned by default. Assign combinations that do not overlap Unity's existing keys.

### Navigation settings

"Custom Navigation" in "Controls and Display" has the following settings.

| Item | Details |
| --- | --- |
| Auto Speed Seconds | The base speed covers the distance to the selected object's surface (or to the pivot) in about this many seconds |
| Fallback Speed | Used only when the base speed cannot be determined automatically (for example, inside the object) |
| Flight Speed Step Ratio | The multiplier per step of right-button + wheel. There is no limit to the number of steps |
| Approach Shrink Ratio | The fraction of the remaining distance left after one wheel step. By default 95% remains |
| Approach Minimum Distance | The lower bound for the distance to the surface when approaching with the wheel |
| Wheel Sensitivity | One step advances each time the wheel amount crosses this value |
| Max Steps Per Event | Limits the number of steps in one event when the wheel is spun fast |
| Invert Look X, Invert Look Y | Invert the direction of the right-drag look |

## Selection

- A click has one of three results: Hit, Miss or Unavailable. Only a real Miss without modifier keys clears the selection. Unavailable, clicks on UI, and a Miss with modifier keys do not clear it.
- Hover and click are decided on the GPU. No colliders are needed.
- For renderers that overlap at the same position and depth (LOD0, LOD1, LOD2 and so on), hover and click may point to different renderers.
- To show the selection outline of Entities Graphics entities, turn on `Selection Outline` in the `Gizmos` menu at the top of the Scene View. When it is off, the outline is not shown.

## Orthographic views and the logarithmic grid

In an orthographic Scene View, the overlay shows the following items instead of the RadialCamera field.

| Item | Details |
| --- | --- |
| Logarithmic Grid | Shows a grid where the center 50% of the screen is an undistorted rectangle and the outside is logarithmically compressed. While it is on, the objects in the scene are drawn with the same projection |
| Data Type | The value the screen edge corresponds to. `float` is `float.MaxValue`; `Logic` is the maximum Logical value |

## Other display settings

"Controls and Display" also has the following settings.

| Item | Details |
| --- | --- |
| Render Path, Real-Space Extent (Object) | `Auto` draws with the original renderer only objects whose full AABB is within the real-space extent (object). `ForceProxy` uses a finite render proxy even within it |
| Safety Boundary | The Scene View's finite safety boundary. Its edges are always shown. "Show Safety Boundary Faces" also shows translucent faces. Leave it off for normal editing |
| Grid | "Grid Follows Camera", "Grid Base (10^n m)", visibility and color per plane, and "Reset Grid to Defaults" |
