---
description: "RotationScalar を 4 つ持つ高精度の Quaternion です。Logical 座標の回転に使います。"
---

[RotationScalar](../rotation-scalar/)（Q1.126）を X、Y、Z、W の 4 つ持つ Quaternion です。Logical 座標の回転に使います。乗算・正規化・ベクトルの回転は、各成分の項をすべて足し合わせてから 1 回だけ丸めます。途中で Unity の `Quaternion` や `Matrix4x4` へ変換しません。

コンストラクターと `TryFromFloat4` は正規化しません。回転として使う前に `TryNormalize` で正規化してください。`TryRotate` は、正規化済みの Quaternion を前提にしています。Unity の `Quaternion` と同じ名前なので、`using L = Omni.Logical;` のように別名を付けると区別しやすくなります。

<!-- members -->

### X

ベクトル部の X 成分です。

### Y

ベクトル部の Y 成分です。

### Z

ベクトル部の Z 成分です。

### W

スカラー部の成分です。

### Identity

回転しない Quaternion（X、Y、Z が 0、W が 1）です。

### Quaternion

4 つの成分から作ります。正規化はしません。

### TryFromFloat4

`float` の 4 成分から変換します。Unity の `Quaternion` と `Unity.Mathematics.quaternion` のどちらの成分も渡せます。正規化はしません。どれかの成分が NaN、無限大、または −2 以上 2 未満の範囲外だと `false` を返し、`result` は `Identity` になります。

### TryToFloat4

`float` の 4 成分へ変換します。各成分は最近接偶数で丸めます。

### TryMultiply

Hamilton 積 `left × right` を求めます。成分の並びと式は Unity の `Quaternion * Quaternion` と同じです。どれかの成分が −2 以上 2 未満の範囲を超えると `false` を返します。

### TryNormalize

長さが 1 になるように正規化します。成分の 2 乗の和から逆平方根を求め、各成分に掛けます。長さが 0 のとき（2 乗の和が分解能に満たないときを含む）は `false` を返します。

### TryNlerp

2 つの回転を成分ごとに線形補間し、正規化して返します（nlerp）。内積が負なら `to` の符号を反転してから補間するので、短い方の弧を通ります。`t` が 0 未満または 1 より大きいときは `false` を返し、`result` は `Identity` になります。`t` が 0 なら `from`、1 なら `to` を、正規化せずにそのまま返します。

### TryConjugate

共役（X、Y、Z の符号を反転し、W はそのまま）を求めます。単位 Quaternion なら逆回転です。成分が −2 のときだけ `false` を返します。

### TryRotate

[Vector3](../vector3/) をこの回転で回します。`rotation` は正規化済みである必要があり、このメソッドは正規化しません。結果の成分が範囲を超えると `false` を返します。

### ToString

`(X, Y, Z, W)` の形で、各成分を 10 進の全桁で返します。

### Equals(Omni.Logical.Quaternion)

### Equals(object)

### GetHashCode

### operator ==

### operator !=
