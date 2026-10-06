---
description: "A validated, read-only celestial catalog created from a CelestialCatalogAsset."
---
A validated, read-only celestial catalog created from a [CelestialCatalogAsset](../celestial-catalog-asset/). It never changes after creation and contains no Unity Objects. Bodies are stored re-sorted by key (ascending ordinal comparison).

The same source content always produces the same `Fingerprint`, which you can use to check which catalog content other data was built from. Normally you get one from [CelestialCatalogRuntime](../celestial-catalog-runtime/) or `CelestialCatalogAsset.TryCreateSnapshot`.

<!-- members -->

### FingerprintSchemaVersion

The version of the content layout used to compute `Fingerprint` (currently 2). When it changes, `Fingerprint` changes even for the same content.

### Revision

The revision number of the source asset.

### Fingerprint

A SHA-256 hex string (64 lowercase characters) representing the catalog content. Computed from the layout version, the revision number and every value of every body.

### Count

Returns the number of bodies.

### Bodies

Returns a read-only list of the bodies ([CelestialBodySnapshot](../celestial-body-snapshot/)), in ascending key order.

### GetAt

Returns the body at an index in ascending key order. Throws `ArgumentOutOfRangeException` when the index is out of range.

### TryGet

Finds a body by key. The comparison is case-sensitive. Returns `false` when not found or when the key is `null` or empty, with `body` set to its default value.

### TryCreate

Validates a revision number and body records and creates a snapshot only if everything is valid. The revision number must be 1 or more, and at least one record is required. On failure returns `false` with `snapshot` set to `null`. `validation` receives every error found. See [CelestialBodyAuthoring](../celestial-body-authoring/) for the validation rules.
