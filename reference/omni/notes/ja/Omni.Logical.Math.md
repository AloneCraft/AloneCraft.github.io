---
description: "Scalar の定数と、絶対値・整数への丸め・平方根・累乗などの関数です。"
---

[Scalar](../scalar/)（Q95.96）の定数と関数です。計算はすべて固定長の整数で行い、途中で `float` や `double` を使いません。範囲を超える可能性のある関数は `Try` で始まり、失敗すると `false` を返します。丸めは最近接偶数です。

[ScalarField](../scalar-field/) の Inspector の式（`pi`、`sqrt`、`floor` など）は、この型の定数と関数で計算します。

<!-- members -->

### Pi

円周率 π を Q95.96 で表した値です。

### Tau

τ = 2π を Q95.96 で表した値です。

### E

ネイピア数 e を Q95.96 で表した値です。

### Phi

黄金比 φ を Q95.96 で表した値です。

### Sqrt2

√2 を Q95.96 で表した値です。

### TryAbs

絶対値を求めます。`Scalar.MinValue` の絶対値は表せないので `false` を返します。

### Min

小さい方の値を返します。

### Max

大きい方の値を返します。

### TryClamp

`value` を `min` 以上 `max` 以下に収めます。`min` が `max` より大きいときは `false` を返し、`result` は `Zero` になります。

### TryFloor

負の無限大の方向へ整数に丸めます（床関数）。常に成功します。

### TryCeiling

正の無限大の方向へ整数に丸めます（天井関数）。結果が範囲を超えると `false` を返します。

### TryRound

最も近い整数に丸めます。ちょうど中間の値は偶数の方へ丸めます（例: 2.5 は 2、3.5 は 4）。結果が範囲を超えると `false` を返します。

### TryDivideRounded

`numerator ÷ denominator` を、逆数を経由せずに求めます。誤差は分解能の半分以内です。`denominator` が 0 のとき、または結果が範囲を超えるときは `false` を返します。`Scalar.TryDivide` と同じ計算です。

### TrySqrt

平方根を求めます。√0 は 0 として成功します。負の値では `false` を返します。`Scalar.TrySqrt` と違い、0 で失敗しません。

### TryPowInteger

`value` の整数乗を求めます。各乗算は最近接偶数で丸めます。負の指数は、先に逆数を丸めて求めてから掛けます。0⁰、0 の負の指数乗、途中の値が範囲を超えるときは `false` を返します。値が 2 の冪なら丸めずに求めます。

### TryLerp

`Scalar.TryLerp` と同じです。`a + (b − a) × t` を求め、`t` が 0 未満または 1 より大きいときは `false` を返します。
