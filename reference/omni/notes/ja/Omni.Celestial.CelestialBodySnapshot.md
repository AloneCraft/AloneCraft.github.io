---
description: "天体 1 つ分の、検証済みで変わらないカタログの値です。"
---
天体 1 つ分の、検証済みで変わらないカタログの値です。[CelestialCatalogSnapshot](../celestial-catalog-snapshot/) の `Bodies`、`GetAt`、`TryGet` から得ます。

幾何の量は [Scalar](../../logical/scalar/)、質量は [Mass](../../units/mass/) で持ちます。`float` や `double` にはしません（[Logical 座標](../../../docs/concepts/logical-coordinates/)）。

<!-- members -->

### Key

天体のキーです。大文字小文字を区別します。

### DisplayName

画面に出す表示名です。

### Physical

物理定数（`CelestialPhysicalConstants`）です。赤道の直径 `DiameterMeters`（m、`Scalar`）、質量 `Mass`（`Mass`、正規の単位は kg）、自転軸の傾き `AxialTiltDegrees`（度、`Scalar`）を持ちます。`TryGetRadiusMeters` で赤道の半径（m）を得られます。

### Orbit

軌道要素（`CelestialOrbitalElements`）です。`IsDefined` が `false` なら軌道を持たず、ほかの値は空の文字列と 0 です。持つ値は、中心の天体のキー `CentralBodyKey`、軌道長半径 `SemiMajorAxisMeters`（m）、周期 `OrbitalPeriodSeconds`（秒）、離心率 `Eccentricity`、軌道傾斜角 `InclinationDegrees`（度）です。数値はすべて `Scalar` です。

### CelestialBodySnapshot

値を指定して作ります。`null` の文字列は空の文字列にします。このコンストラクターは値を検証しません。検証済みの値が必要なときは、`CelestialCatalogSnapshot.TryCreate` を通してください。
