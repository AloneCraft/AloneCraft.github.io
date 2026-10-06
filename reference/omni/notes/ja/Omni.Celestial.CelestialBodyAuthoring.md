---
description: "CelestialCatalogAsset に保存する、天体 1 つ分の元の値です。数値は 10 進の文字列で持ちます。"
---
[CelestialCatalogAsset](../celestial-catalog-asset/) に保存する、天体 1 つ分の元の値です。数値は `float` ではなく 10 進の文字列で持ちます。Inspector の表示の精度やカルチャの違いで、物理定数の桁を失わないためです。

文字列は、[CelestialCatalogSnapshot](../celestial-catalog-snapshot/) を作るときに検証して変換します。長さ、角度、時間、離心率は [Scalar](../../logical/scalar/)、質量は [Mass](../../units/mass/) になります。数値はカルチャに依存しない表記で書きます。小数点は `.` で、`5.6834e26` のような指数の表記も使えます。

| 項目 | 検証の規則 |
| --- | --- |
| キー | 空でなく、空白を含まない。カタログの中で一意（大文字小文字を区別） |
| 表示名 | 空白だけでなく、前後に空白を持たない |
| 直径 | 正 |
| 質量 | 正（負の値は解析の失敗として扱う） |
| 自転軸の傾き | 解析できればよい（符号の制限なし） |
| 軌道（`HasOrbitRecord` がオンのとき） | 中心のキーはキーと同じ規則で、カタログにあり、自分自身ではない。軌道長半径と周期は正。離心率は 0 以上 1 未満。軌道傾斜角は解析できればよい |

<!-- members -->

### m_Key

カタログの中で一意な天体のキーです。大文字小文字を区別します。

### m_DisplayName

UI やクレジットで使う表示名です。

### m_DiameterMeters

赤道の直径（m）の 10 進の表記です。

### m_MassKilograms

質量（kg）の 10 進の表記です。

### m_AxialTiltDegrees

自転軸の傾き（度）の 10 進の表記です。

### m_HasOrbitRecord

オンにすると、軌道要素（中心の天体、軌道長半径、周期、離心率、軌道傾斜角）を使います。オフなら軌道の項目は検証もせず、使いません。

### m_OrbitCenterKey

軌道の中心の天体のキーです。

### m_SemiMajorAxisMeters

軌道長半径（m）の 10 進の表記です。

### m_OrbitalPeriodSeconds

公転の周期（秒）の 10 進の表記です。

### m_Eccentricity

軌道の離心率の 10 進の表記です。

### m_OrbitalInclinationDegrees

軌道傾斜角（度）の 10 進の表記です。

### Key

天体のキーを返します。

### DisplayName

表示名を返します。

### DiameterMetersText

赤道の直径（m）の文字列を返します。

### MassKilogramsText

質量（kg）の文字列を返します。

### AxialTiltDegreesText

自転軸の傾き（度）の文字列を返します。

### HasOrbitRecord

軌道要素を使うかを返します。

### OrbitCenterKey

軌道の中心の天体のキーを返します。

### SemiMajorAxisMetersText

軌道長半径（m）の文字列を返します。

### OrbitalPeriodSecondsText

公転の周期（秒）の文字列を返します。

### EccentricityText

離心率の文字列を返します。

### OrbitalInclinationDegreesText

軌道傾斜角（度）の文字列を返します。

### CelestialBodyAuthoring()

Unity のシリアライズの復元のための、空のコンストラクターです。

### CelestialBodyAuthoring(string, string, string, string, string, bool, string, string, string, string, string)

すべての値を指定して作ります。値の検証は、Snapshot を作るときに行います。`null` の文字列は空の文字列にします。
