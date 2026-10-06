---
description: "質量を kg で持つ型です。10 進の係数（最大 38 桁）と 10 進の指数で持ちます。"
---

質量を kg で持つ、負にならない型です。座標の固定小数点とは別の形式で、10 進の係数（最大 38 桁）と 10 進の指数で持ちます。そのため、素粒子の質量から銀河の質量までを同じ型で表せます。39 桁目以降の桁は最近接偶数で丸めます。

[CelestialBodyAuthoring](../../celestial/celestial-body-authoring/) の質量（kg の文字列）は、カタログの Snapshot を作るときに `TryFromKilograms` でこの型へ変換します。

<!-- members -->

### Zero

質量 0 です。

### IsZero

質量が 0 のときに `true` です。

### TryFromKilograms

kg の 10 進の文字列（例: `5.972e24`）から作ります。前後の空白、符号、小数点、指数（`e` または `E`）を受け付けます。不正な文字列、負の値、指数が `int` の範囲を超える値では `false` を返し、`mass` は `Zero` になります。

### TryFromGrams

g の 10 進の文字列から作り、kg に直します（10⁻³ 倍は指数をずらすだけなので正確です）。失敗の条件は `TryFromKilograms` と同じです。

### CompareTo

### Equals(Mass)

### Equals(object)

### GetHashCode

### ToString

kg の値を、係数と指数の形で返します。係数は末尾の 0 を除いた整数です（例: `5.972e24` は `5972e21`、0 は `0`）。

### operator ==

### operator !=

### operator <

### operator >

### operator <=

### operator >=
