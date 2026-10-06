---
description: "A wrapping angle that splits one turn into 2⁶⁴ steps (turns in Q0.64)."
---

A wrapping angle that splits one turn into 2⁶⁴ steps. The format is turns in Q0.64, and the range is from 0 up to but not including one turn. Values beyond one turn become the remainder after dividing by one turn. The resolution is 2⁻⁶⁴ turns (about 3.4×10⁻¹⁹ rad).

Adding angles is exact, so no error builds up no matter how many times you add. To get an angle from an angular velocity and a time, use [AngularVelocity.TryMultiply](../angular-velocity/). Comparisons (`<` and so on) compare the values from 0 to one turn and ignore wrapping.

<!-- members -->

### Zero

Zero turns.

### RawTurns

The raw value: an integer where one turn is 2⁶⁴ (from 0 up to but not including 2⁶⁴).

### FromSector

Creates an angle from a position inside sector number `sectorIndex` when the circle is split into `sectorCount` sectors. `phaseWithinSectorQ32` is the position inside the sector, where 2³² is one whole sector. The result is (`sectorIndex` + `phaseWithinSectorQ32` ÷ 2³²) ÷ `sectorCount` turns, truncated. Throws `ArgumentOutOfRangeException` when `sectorCount` is 0 or less, or `sectorIndex` is outside 0 to `sectorCount − 1`.

### Add

Adds two angles. Anything beyond one turn wraps around. It never fails.

### ToTurns

Returns the number of turns as a `double` (from 0 to 1 inclusive). A value very close to one turn may become 1.0 because of `double` rounding.

### ToRadians

Returns radians as a `double` (`ToTurns()` × 2π).

### GetCellIndex

Splits the circle into `cellCount` equal cells and returns the number of the cell that contains this angle (0 to `cellCount − 1`). It uses integers only, so boundary decisions have no rounding error. Throws `ArgumentOutOfRangeException` when `cellCount` is 0 or less.

### TrySinCos

Computes sin and cos as [Scalar](../../logical/scalar/) values. It runs a 32-iteration CORDIC with integers only and uses no floating point. The precision is set by the iteration count and is much coarser than the `Scalar` resolution (2⁻⁹⁶). Always returns `true`.

### CompareTo

### Equals(Angle)

### Equals(object)

### GetHashCode

### ToString

Returns the number of turns (the value of `ToTurns()`) as a culture-independent `double` string.

### operator ==

### operator !=

### operator <

### operator >

### operator <=

### operator >=
