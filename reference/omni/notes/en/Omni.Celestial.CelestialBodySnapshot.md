---
description: "Validated, immutable catalog values for one celestial body."
---
Validated, immutable catalog values for one celestial body. Get it from `Bodies`, `GetAt` or `TryGet` on [CelestialCatalogSnapshot](../celestial-catalog-snapshot/).

Geometric quantities are held as [Scalar](../../logical/scalar/) and mass as [Mass](../../units/mass/); they are never converted to `float` or `double` ([Logical coordinates](../../../docs/concepts/logical-coordinates/)).

<!-- members -->

### Key

The body key. Case-sensitive.

### DisplayName

The display name shown on screen.

### Physical

The physical constants (`CelestialPhysicalConstants`): equatorial diameter `DiameterMeters` (m, `Scalar`), mass `Mass` (`Mass`, canonical unit kg), and axial tilt `AxialTiltDegrees` (degrees, `Scalar`). `TryGetRadiusMeters` gives the equatorial radius (m).

### Orbit

The orbital elements (`CelestialOrbitalElements`). When `IsDefined` is `false` the body has no orbit, and the other values are an empty string and zeros. The values are the center body key `CentralBodyKey`, semi-major axis `SemiMajorAxisMeters` (m), period `OrbitalPeriodSeconds` (seconds), eccentricity `Eccentricity`, and inclination `InclinationDegrees` (degrees). All numbers are `Scalar`.

### CelestialBodySnapshot

Creates a value from the given parts. `null` strings become empty strings. This constructor does not validate anything; when you need validated values, go through `CelestialCatalogSnapshot.TryCreate`.
