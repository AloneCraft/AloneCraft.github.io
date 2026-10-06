---
description: "A marker component that removes its MeshRenderer from Scene BRG's automatic collection and draws it on the normal Radial MeshRenderer path."
---
`RadialSceneBatchRendererExclusion` is a marker that keeps the MeshRenderer on the same GameObject out of [Scene BRG](../../../docs/guides/scene-brg/) and on the normal Radial MeshRenderer path. It has no settings. Only that Renderer is excluded; Scene BRG for the whole Scene does not stop.

- Add it to Renderers whose MaterialPropertyBlock is set by something other than Omni, for example when an Animator writes `material.*`. The MaterialPropertyBlock is kept.
- Even without the marker, Omni automatically excludes Renderers that have a MaterialPropertyBlock not set by Omni, Renderers that use a Material that is not a persistent asset, and Renderers with `RadialIdentityWarp`.
- An excluded Renderer prints a message starting with `[Omni][Scene BRG]` to the Console once.
- Enabling or disabling it rebuilds Scene BRG.

<!-- members -->

### RegisterUnmappedSceneMaterial

Registers a function that decides whether a Scene Material cannot be traced back to a persistent Material. A MeshRenderer with a slot for which the function returns `true` is excluded from Scene BRG. Omni's Editor registers this at startup. Only one function can be registered, and a call replaces the previous one. Calling it from your own code disables Omni's check, so normally you do not call it.

### RegisterExternalMaterialOwnership

Registers a function that decides which Materials must not be put on Scene BRG. A MeshRenderer that uses at least one Material for which the function returns `true` is excluded from Scene BRG. Only one function can be registered, and a call replaces the previous one. Passing `null` removes the registration.
