---
description: "Scalar を 9 つ持つ 3×3 行列です。回転・Scale・Shear を合成した基底に使います。"
---

[Scalar](../scalar/)（Q95.96）を 9 つ持つ 3×3 の行列です。成分は行優先で、`M01` は 1 行 2 列目です。回転・符号付きの Scale・Shear を合成した基底を表します。[Logical Transform](../../components/logical-transform/) の `TryGetWorldAffine` が、world の基底をこの型で返します。

ベクトルとの積と行列どうしの積は、各成分の 3 つの積を足してから 1 回だけ丸めます。範囲を超えると `false` を返します。

<!-- members -->

### M00

1 行 1 列目の成分です。

### M01

1 行 2 列目の成分です。

### M02

1 行 3 列目の成分です。

### M10

2 行 1 列目の成分です。

### M11

2 行 2 列目の成分です。

### M12

2 行 3 列目の成分です。

### M20

3 行 1 列目の成分です。

### M21

3 行 2 列目の成分です。

### M22

3 行 3 列目の成分です。

### Identity

単位行列です。

### Matrix3x3

9 つの成分を行優先（`m00`、`m01`、`m02`、`m10`、…）で渡して作ります。

### TryMultiplyVector

行列とベクトルの積（`matrix × vector`、ベクトルは列ベクトル）を求めます。どれかの成分が範囲を超えると `false` を返し、`result` は `Vector3.Zero` になります。

### TryMultiply

行列どうしの積（`left × right`）を求めます。どれかの成分が範囲を超えると `false` を返し、`result` は `Identity` になります。

### TryInverse

逆行列を、余因子行列の転置 ÷ 行列式で求めます。負の Scale や Shear を含む行列も扱えます。行列式が 0（逆行列が無い）のとき、または途中で範囲を超えたときは `false` を返し、`result` は `Identity` になります。

### Equals(Omni.Logical.Matrix3x3)

### Equals(object)

### GetHashCode

### operator ==

### operator !=
