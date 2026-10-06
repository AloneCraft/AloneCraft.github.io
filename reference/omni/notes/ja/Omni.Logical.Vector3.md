---
description: "Scalar を 3 つ持つベクトルです。差分・方向・Scale に使います。"
---

[Scalar](../scalar/)（Q95.96）を X、Y、Z の 3 つ持つベクトルです。差分・方向・Scale に使います。位置には、この型を包んだ [Position3](../position3/) を使います。Unity の `Vector3` と同じ名前なので、`using L = Omni.Logical;` のように別名を付けると区別しやすくなります。

成分ごとの演算は `Scalar` と同じ規則です。範囲を超えると `false` を返し、`out` の値は `Zero` になります。内積・外積は、複数の積を足し合わせてから 1 回だけ丸めます。

<!-- members -->

### X

X 成分です。

### Y

Y 成分です。

### Z

Z 成分です。

### Zero

すべての成分が 0 のベクトルです。

### One

すべての成分が 1 のベクトルです。

### Vector3

3 つの成分から作ります。

### TryToFloat3

`Unity.Mathematics.float3` へ変換します。各成分は最近接偶数で `float` に丸めるので、精度が落ちます。描画など `float` が必要な境界で使います。

### TryFromFloat3

`Unity.Mathematics.float3` から変換します。各成分は `Scalar.TryFromFloat` と同じ規則で変換します。NaN、無限大、範囲外の成分があると `false` を返します。

### TryAdd

成分ごとに足します。丸めはありません。どれかの成分が範囲を超えると `false` を返します。

### TrySubtract

成分ごとに引きます（`left − right`）。丸めはありません。どれかの成分が範囲を超えると `false` を返します。

### TryNegate

各成分の符号を反転します。どれかの成分が `Scalar.MinValue` だと `false` を返します。

### TryScale

各成分に `scalar` を掛けます。成分ごとに最近接偶数で丸めます。

### TryLerp

成分ごとに `Scalar.TryLerp` で補間します。先に差を取るので、巨大な座標どうしの補間でも差の精度が残ります。`t` が 0 未満または 1 より大きいときは `false` を返します。

### TryDot

内積を求めます。3 つの積を足してから 1 回だけ丸めます。

### TryCross

外積を求めます。各成分は 2 つの積の差を求めてから 1 回だけ丸めます。

### TrySqrLength

長さの 2 乗を求めます。`TryDot(value, value)` と同じです。2 乗は範囲を超えやすく、長さがおよそ 2×10¹⁴（2⁴⁷·⁵）以上だと失敗します。長いベクトルには `TryLength` を使います。

### TryLength

長さを求めます。最大の成分で割ってから計算するので、長さが表せる範囲なら、2 乗が範囲を超える巨大なベクトルでも求められます。ゼロベクトルでは 0 を返して成功します。

### TryNormalize

長さが 1 になるように割ります。ゼロベクトルでは `false` を返します。

### ToString

`(X, Y, Z)` の形で、各成分を 10 進の全桁で返します。

### Equals(Omni.Logical.Vector3)

### Equals(object)

### GetHashCode

### operator ==

### operator !=
