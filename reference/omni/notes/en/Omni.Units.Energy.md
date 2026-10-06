---
description: "An energy in J, stored as a decimal coefficient of up to 38 digits and a decimal exponent."
---

A non-negative energy in J (joules). Like [Mass](../mass/), it is stored as a decimal coefficient of up to 38 digits and a decimal exponent. One type covers everything from the energy of a single photon to the total energy of the universe.

It can also be created from eV, MeV, rest mass or photon frequency. The constants used for conversion are the SI defined values (1 eV = 1.602176634×10⁻¹⁹ J, c = 299792458 m/s, h = 6.62607015×10⁻³⁴ J·s). When a product has more than 38 digits, digits from the 39th onward are rounded with round-half-to-even. On failure the method returns `false`, and `energy` is `Zero`.

<!-- members -->

### Zero

An energy of 0.

### IsZero

`true` when the energy is 0.

### TryFromJoules

Creates a value from a decimal string in J (for example `3.828e26`). The string format is the same as `Mass.TryFromKilograms`. Returns `false` for an invalid string or a negative value.

### TryFromElectronVolts

Creates a value from a decimal string in eV and converts it to J. Returns `false` for an invalid string or a negative value.

### TryFromMegaElectronVolts

Creates a value from a decimal string in MeV and converts it to J. Returns `false` for an invalid string or a negative value.

### TryFromRestMass

Computes the rest-mass energy E = mc².

### TryFromPairAnnihilation

Computes the energy 2mc² released when a particle and an antiparticle of the same mass annihilate. `particleMass` is the mass of one particle.

### TryFromPhotonFrequencyHertz

Computes the energy of a single photon, E = hν, from a decimal string with the frequency in Hz. Returns `false` for an invalid string or a negative value.

### CompareTo

### Equals(Energy)

### Equals(object)

### GetHashCode

### ToString

Returns the value in J as a coefficient and an exponent. The coefficient is an integer with trailing zeros removed (for example, `1.5e3` becomes `15e2`, and 0 becomes `0`).

### operator ==

### operator !=

### operator <

### operator >

### operator <=

### operator >=
