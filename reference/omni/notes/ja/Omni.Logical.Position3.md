---
description: "Logical 座標の位置を表す型です。差分のベクトル（Vector3）と区別します。"
---

Logical 座標の位置を表す型です。中身は [Vector3](../vector3/) で、各成分は [Scalar](../scalar/)（Q95.96）です。差分や方向の `Vector3` と型で区別するために分けてあります。位置どうしの差は `TryDifference` で `Vector3` として求め、位置にずれを足すときは `TryAdd` を使います。

[Logical Transform](../../components/logical-transform/) の `SetWorld` や `TryGetWorld` が、この型で world の位置をやり取りします。

<!-- members -->

### Value

中身のベクトルです。

### Zero

Logical 座標の原点です。

### Position3

ベクトルから位置を作ります。

### TryToFloat3

`Unity.Mathematics.float3` へ変換します。各成分は最近接偶数で `float` に丸めるので、原点から遠い位置では精度が落ちます。

### TryFromFloat3

`Unity.Mathematics.float3` から変換します。NaN、無限大、範囲外の成分があると `false` を返します。

### ToString

`(X, Y, Z)` の形で、各成分を 10 進の全桁で返します。

### TryAdd

位置にずれ（`offset`）を足した位置を求めます。丸めはありません。どれかの成分が範囲を超えると `false` を返します。

### TryDifference

2 つの位置の差（`left − right`）を `Vector3` で求めます。丸めはありません。どれかの成分が範囲を超えると `false` を返します。

### Equals(Omni.Logical.Position3)

### Equals(object)

### GetHashCode

### operator ==

### operator !=
