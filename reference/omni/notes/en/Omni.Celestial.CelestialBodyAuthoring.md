---
description: "The source values for one celestial body, stored in CelestialCatalogAsset. Numbers are kept as decimal strings."
---
The source values for one celestial body, stored in [CelestialCatalogAsset](../celestial-catalog-asset/). Numbers are kept as decimal strings rather than `float`, so that Inspector display precision and culture differences never lose digits of a physical constant.

The strings are validated and converted when a [CelestialCatalogSnapshot](../celestial-catalog-snapshot/) is created. Lengths, angles, times and eccentricity become [Scalar](../../logical/scalar/) values, and mass becomes [Mass](../../units/mass/). Write numbers in culture-invariant notation: the decimal separator is `.`, and exponent notation such as `5.6834e26` is accepted.

| Field | Validation rule |
| --- | --- |
| Key | Not empty and contains no whitespace. Unique within the catalog (case-sensitive) |
| Display name | Not whitespace only, and no leading or trailing whitespace |
| Diameter | Positive |
| Mass | Positive (a negative value is treated as a parse failure) |
| Axial tilt | Must parse (no sign restriction) |
| Orbit (when `HasOrbitRecord` is on) | The center key follows the key rules, exists in the catalog, and is not the body itself. Semi-major axis and period are positive. Eccentricity is at least 0 and less than 1. Inclination must parse |

<!-- members -->

### m_Key

The body key, unique within the catalog. Case-sensitive.

### m_DisplayName

The display name used in UI and credits.

### m_DiameterMeters

The equatorial diameter (m) in decimal notation.

### m_MassKilograms

The mass (kg) in decimal notation.

### m_AxialTiltDegrees

The axial tilt (degrees) in decimal notation.

### m_HasOrbitRecord

When on, the orbital elements (center body, semi-major axis, period, eccentricity, inclination) are used. When off, the orbit fields are neither validated nor used.

### m_OrbitCenterKey

The key of the body at the center of the orbit.

### m_SemiMajorAxisMeters

The semi-major axis (m) in decimal notation.

### m_OrbitalPeriodSeconds

The orbital period (seconds) in decimal notation.

### m_Eccentricity

The orbital eccentricity in decimal notation.

### m_OrbitalInclinationDegrees

The orbital inclination (degrees) in decimal notation.

### Key

Returns the body key.

### DisplayName

Returns the display name.

### DiameterMetersText

Returns the equatorial diameter (m) string.

### MassKilogramsText

Returns the mass (kg) string.

### AxialTiltDegreesText

Returns the axial tilt (degrees) string.

### HasOrbitRecord

Returns whether the orbital elements are used.

### OrbitCenterKey

Returns the key of the body at the center of the orbit.

### SemiMajorAxisMetersText

Returns the semi-major axis (m) string.

### OrbitalPeriodSecondsText

Returns the orbital period (seconds) string.

### EccentricityText

Returns the eccentricity string.

### OrbitalInclinationDegreesText

Returns the orbital inclination (degrees) string.

### CelestialBodyAuthoring()

An empty constructor for Unity's deserialization.

### CelestialBodyAuthoring(string, string, string, string, string, bool, string, string, string, string, string)

Creates a record with all values specified. The values are validated when a snapshot is created. `null` strings become empty strings.
