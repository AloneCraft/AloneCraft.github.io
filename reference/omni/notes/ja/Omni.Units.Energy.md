---
description: "エネルギーを J で持つ型です。10 進の係数（最大 38 桁）と 10 進の指数で持ちます。"
---

エネルギーを J（ジュール）で持つ、負にならない型です。[Mass](../mass/) と同じく、10 進の係数（最大 38 桁）と 10 進の指数で持ちます。光子 1 個のエネルギーから宇宙規模の総エネルギーまでを同じ型で表せます。

eV、MeV、静止質量、光子の周波数からも作れます。変換に使う定数は SI の定義値です（1 eV = 1.602176634×10⁻¹⁹ J、c = 299792458 m/s、h = 6.62607015×10⁻³⁴ J·s）。積が 38 桁を超えるときは、39 桁目以降を最近接偶数で丸めます。失敗すると `false` を返し、`energy` は `Zero` になります。

<!-- members -->

### Zero

エネルギー 0 です。

### IsZero

エネルギーが 0 のときに `true` です。

### TryFromJoules

J の 10 進の文字列（例: `3.828e26`）から作ります。文字列の形式は `Mass.TryFromKilograms` と同じです。不正な文字列、負の値では `false` を返します。

### TryFromElectronVolts

eV の 10 進の文字列から作り、J に直します。不正な文字列、負の値では `false` を返します。

### TryFromMegaElectronVolts

MeV の 10 進の文字列から作り、J に直します。不正な文字列、負の値では `false` を返します。

### TryFromRestMass

静止質量のエネルギー E = mc² を求めます。

### TryFromPairAnnihilation

同じ質量の粒子と反粒子が対消滅したときのエネルギー 2mc² を求めます。`particleMass` は粒子 1 個の質量です。

### TryFromPhotonFrequencyHertz

周波数（Hz）の 10 進の文字列から、光子 1 個のエネルギー E = hν を求めます。不正な文字列、負の値では `false` を返します。

### CompareTo

### Equals(Energy)

### Equals(object)

### GetHashCode

### ToString

J の値を、係数と指数の形で返します。係数は末尾の 0 を除いた整数です（例: `1.5e3` は `15e2`、0 は `0`）。

### operator ==

### operator !=

### operator <

### operator >

### operator <=

### operator >=
