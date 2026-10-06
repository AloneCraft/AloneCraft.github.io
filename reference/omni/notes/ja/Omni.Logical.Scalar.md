---
description: "位置・距離・Scale に使う Q95.96（192 bit）の固定小数点数です。"
---

位置・距離・Scale に使う 192 bit の固定小数点数です。形式は Q95.96（整数部 95 bit、小数部 96 bit、符号付き 2 の補数）で固定です。範囲は −2⁹⁵ 以上 2⁹⁵ 未満（約 ±3.96×10²⁸）、分解能は 2⁻⁹⁶（約 1.26×10⁻²⁹）です。分解能は値の大きさによらず同じです。

- 溢れる可能性のある演算は、すべて `Try` で始まるメソッドです。範囲を超えると `false` を返し、`out` の値は `Zero` になります。結果を折り返したり、飽和させたりしません。
- 加算・減算・符号反転は丸めずに正確です。乗算・除算・平方根・文字列や `float`／`double` からの変換は、分解能 2⁻⁹⁶ へ最近接偶数で丸めます。分解能の半分以下になる結果は 0 になり、失敗にはなりません。
- 演算子は比較（`==`、`<` など）だけです。四則演算は `TryAdd` などの静的メソッドで行います。

```csharp
using UnityEngine;
using L = Omni.Logical;

public static class ScalarExample
{
    public static void Run()
    {
        if (!L.Scalar.TryParse("149597870700", out L.Scalar au) || // 1 天文単位（m）
            !L.Scalar.TryParse("1e-9", out L.Scalar nm))             // 1 nm
        {
            return;
        }

        // 加算と減算は丸めないので、1 天文単位に 1 nm を足しても失われない
        if (L.Scalar.TryAdd(au, nm, out L.Scalar sum) &&
            L.Scalar.TrySubtract(sum, au, out L.Scalar back))
        {
            Debug.Log(back == nm); // True
        }

        // 範囲を超える演算は false を返す
        Debug.Log(L.Scalar.TryAdd(L.Scalar.MaxValue, L.Scalar.One, out _)); // False
    }
}
```

3 成分のベクトルは [Vector3](../vector3/)、位置は [Position3](../position3/)、Unity の Inspector で保存するには [ScalarField](../scalar-field/) を使います。絶対値や整数への丸めなどは [Math](../math/) にあります。仕組みは [Logical 座標と Proxy](../../../docs/concepts/logical-coordinates/) を見てください。

<!-- members -->

### FractionalBits

小数部のビット数です。値は 96 です。

### Raw

生の値です。192 bit の 2 の補数の整数で、数値は `Raw` ÷ 2⁹⁶ です。

### Zero

0 です。

### One

1 です（`Raw` = 2⁹⁶）。

### IsDivisionSupported

除算・逆数・平方根・逆平方根が使えるかどうかです。現在の固定の形式（Q95.96）では常に `true` です。

### MaxValue

表せる最大の値（2⁹⁵ − 2⁻⁹⁶）です。

### MinValue

表せる最小の値（−2⁹⁵）です。符号を反転した値は表せません。

### Scalar

生の値（`Raw`）から値を作ります。通常は `TryParse` や `TryFromDouble` を使います。

### IsNegative

値が負のときに `true` です。

### IsZero

値が 0 のときに `true` です。

### TryAdd

`left + right` を求めます。丸めはありません。範囲を超えると `false` を返し、`result` は `Zero` になります。

### TrySubtract

`left - right` を求めます。丸めはありません。範囲を超えると `false` を返し、`result` は `Zero` になります。

### TryNegate

符号を反転します。`MinValue` は反転できないので `false` を返します。

### TryMultiply

`left × right` を求めます。厳密な積を作ってから、分解能へ最近接偶数で 1 回だけ丸めます。範囲を超えると `false` を返します。

### TryReciprocal

`1 ÷ value` を求め、最近接偶数で丸めます。`value` が 0 のとき、または結果が範囲を超えるときは `false` を返します。

### TryDivide

`left ÷ right` を求めます。逆数を経由せずに商を直接求め、最近接偶数で 1 回だけ丸めます。`right` が 0 のとき、または結果が範囲を超えるときは `false` を返します。

### TryReciprocalSqrt

`1 ÷ √value` を Newton 法の反復で求めます。`value` が 0 以下のときは `false` を返します。

### TrySqrt

`√value` を求め、最近接偶数で丸めます。`value` が 0 または負のときは `false` を返します。√0 = 0 を成功として扱いたいときは [Math.TrySqrt](../math/) を使います。

### TryScaleByPowerOfTwo

`value × 2^exponent` を求めます。`exponent` が負なら 2 の冪で割ります。ビットをずらすだけなので、掛けるときは正確です。割るときに分解能より細かくなる桁は最近接偶数で丸め、範囲を超えるときは `false` を返します。

### TryLerp

`from + (to − from) × t` を求めます。先に差を取るので、巨大な座標どうしの補間でも差の精度が残ります。`t` が 0 未満または 1 より大きいときは `false` を返します。`t` が 0 なら `from`、1 なら `to` をそのまま返します。

### TryLerpLog2

対数の空間で補間します。両端が正なら `from^(1−t) × to^t`（等比の補間）を求めます。どちらかが 0 以下なら、通常の線形補間 `from × (1−t) + to × t` にします。`t` が 0 未満または 1 より大きいときは `false` を返します。

### TryFromFloat

`float` から変換し、分解能へ最近接偶数で丸めます。絶対値が 2⁻⁷³ 以上の `float` は正確に変換します。NaN、無限大、範囲外の値では `false` を返します。

### TryFromDouble

`double` から変換し、分解能へ最近接偶数で丸めます。NaN、無限大、範囲外の値では `false` を返します。

### TryToFloat

`float` へ変換します。有効桁（2 進 24 桁）から外れる桁は最近接偶数で丸めます。`Scalar` の範囲は `float` の有限な範囲に収まります。

### TryToDouble

`double` へ変換します。有効桁（2 進 53 桁）から外れる桁は最近接偶数で丸めます。`Scalar` の範囲は `double` の有限な範囲に収まります。

### CompareTo

### Equals(Omni.Logical.Scalar)

### Equals(object)

### GetHashCode

### ToString

10 進の全桁の文字列を返します。カルチャに依存しません。末尾の 0 は出さず、整数なら小数点も出しません（例: `1`、`0.5`）。2 進の小数は有限桁の 10 進になるので、この文字列を `TryParse` に渡すと同じ値に戻ります。

### TryParse

10 進の文字列から変換します。カルチャに依存しません。符号（`+`、`-`）、小数点、指数（例: `1.496e11`）を受け付け、前後の空白は無視します。分解能へ最近接偶数で丸め、分解能の半分以下の値は 0 になります。式、桁区切り、`Infinity` などは受け付けません。不正な文字列、4,096 文字を超える文字列、範囲外の値では `false` を返し、`result` は `Zero` になります。

### operator ==

### operator !=

### operator <

### operator >

### operator <=

### operator >=
