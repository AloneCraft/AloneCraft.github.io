---
description: "Provides the shared CelestialCatalogAsset in Resources as a validated snapshot."
---
A static class that reads the shared [CelestialCatalogAsset](../celestial-catalog-asset/) from `Resources` and provides it as a validated, immutable [CelestialCatalogSnapshot](../celestial-catalog-snapshot/). The bundled demos (EarthToMoon, SolarSystem) read celestial values from here.

```csharp
using Omni.Celestial;

if (CelestialCatalogRuntime.TryGetSnapshot(out CelestialCatalogSnapshot catalog, out string error) &&
    catalog.TryGet("Moon", out CelestialBodySnapshot moon))
{
    // use moon.Physical.DiameterMeters and so on
}
else
{
    Debug.LogError(error);
}
```

:::caution[Reused results]
While the asset and its `Revision` stay the same, the previous result (a successful snapshot or a failure's error) is returned as is. After changing the asset's values in the Editor, increase `Revision` or call `InvalidateCacheForEditor`.
:::

<!-- members -->

### ResourcePath

The path relative to `Resources` that is loaded in the Player (`"Omni/CelestialCatalog"`).

### AssetPath

The asset path of the shared catalog (`"Assets/Omni/Resources/Omni/CelestialCatalog.asset"`).

### TryGetSnapshot

Loads the shared catalog with `Resources.Load` and returns a validated snapshot. When the asset is missing or its content is invalid, returns `false` with `snapshot` set to `null` and the reason in `error`. On success `error` is an empty string. The leading sentence of `error` is fixed Japanese text. For the same asset and the same revision number, the previous result is returned.

### GetSnapshotOrThrow

Returns a snapshot in the same way as `TryGetSnapshot`. On failure, throws an `InvalidOperationException` that contains the reason.

### InvalidateCacheForEditor

Discards the remembered result so that the next `TryGetSnapshot` loads the asset again. Available only in the Editor (`UNITY_EDITOR`). Call it right after updating the asset without changing its revision number.
