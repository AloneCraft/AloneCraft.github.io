---
description: "Radial（対数）の写像の設定と、GPU と同じ写像を CPU で計算するメソッドを持つ構造体です。"
---
`RadialSettings` は、Radial の写像 `F(d) = log2(1 + d · M)` を CPU で計算するための値です。GPU の写像と同じ曲線を使います。持つ値は log2(*M*) と論理 Far だけです。

- [RadialCamera](../radial-camera/) の今の値は `RadialSettings.FromDriver(radialCamera)` で取り込みます。
- 写像は、写像の中心（通常は目）からの相対位置に対して行います。向きを保ち、距離だけを変えます。
- `Try` で始まるメソッドは、入力または結果が有限でないときに `false` を返します。`false` のときの `out` の値は使わないでください。

式と *M* の意味は [Radial の写像と View Scale](../../../docs/concepts/radial-mapping/) を見てください。

```csharp
using Omni;
using L = Omni.Logical;

RadialSettings settings = RadialSettings.FromDriver(radialCamera);
if (L.Scalar.TryParse("149597870700", out L.Scalar au) &&
    settings.TryMapDistance(au, out float rendered) &&
    settings.TryUnmapDistance(rendered, out L.Scalar back))
{
    // rendered は 1 天文単位の描画空間での距離。back はそれを論理距離へ戻した値
}
```

<!-- members -->

### RadialScale

写像の *S* で、定数の 1 です。設定はありません。

### DefaultLog2ViewScale

log2(*M*) の既定値で、0（*M* = 1）です。*M* = 1 でも写像は恒等になりません。

### DefaultFarClip

論理 Far の既定値です。`RadialCamera.DefaultFarClip` と同じ `Scalar` の上限（約 3.96×10²⁸ m）です。

### m_Log2ViewScale

保存する log2(*M*) です。有限でない値は、読むときに 0 として扱います。

### m_FarClip

保存する論理 Far（m）です。0 以下（未設定）は、読むときに `DefaultFarClip` として扱います。

### RadialSettings(float)

log2(*M*) を指定して作ります。論理 Far は `DefaultFarClip` です。`log2ViewScale` が有限でなければ 0 にします。

### RadialSettings(float, in Omni.Logical.Scalar)

log2(*M*) と論理 Far を指定して作ります。`log2ViewScale` が有限でなければ 0 に、`logicalFarClip` が正でなければ `DefaultFarClip` にします。

### Default

既定の設定です。log2(*M*) は 0、論理 Far は `DefaultFarClip` です。

### Log2ViewScale

log2(*M*) です。*M* そのものは `float` にしません。非常に大きい *M* や小さい *M* も表せます。

### FarClip

論理 Far（m）です。常に正の値を返します。

### TryMap

写像の中心からの Logical の相対位置を、描画空間の相対位置へ写します。向きは変えず、距離だけを写します。入力がゼロのベクトルなら、ゼロを返して成功します。

### TryUnmap

描画空間の相対位置を、写像の中心からの Logical の相対位置へ戻します。`TryMap` の逆です。入力が有限でないとき、または戻した距離が 0 になるときは失敗します。

### TryMapDistance

論理距離だけを写します。`TryMap` と同じ曲線です。距離が負のときは失敗します。距離が 0 なら 0 を返して成功します。

### TryUnmapDistance

描画距離を論理距離へ戻します。`TryMapDistance` の逆です。描画距離が負か有限でないときは失敗します。

### TryProject

相対位置を写し、同時にその位置での倍率（描画距離 ÷ 論理距離）の log2 を返します。`TryMap` と `TryGetLog2Scale` を別々に呼ぶより、写像の計算が 1 回で済みます。入力がゼロのベクトルなら、原点での倍率（*M* / ln 2）の log2 を返します。

### TryGetLog2Scale

その論理距離での倍率（描画距離 ÷ 論理距離）の log2 を返します。距離が 0 なら、原点での倍率（*M* / ln 2）の log2 を返します。距離が負のときは失敗します。

### ApplyTo

この設定を `RadialCamera` に書きます。論理 Near を「Camera の Near ÷ *M*」に、論理 Far を `FarClip` にします。Camera が無いときは Near を 0.3 として計算します。`driver` が `null` のとき、または論理 Near を正の値で求められないときは、何も変えずに `false` を返します。

### FromDriver

`RadialCamera` の今の log2(*M*)（`EffectiveLog2ViewScale`）と論理 Far（`EffectiveFarClip`）で設定を作ります。`driver` が `null` なら `Default` を返します。

### Equals(RadialSettings)

log2(*M*) と論理 Far が等しいかを返します。

### Equals(object)

`RadialSettings` で、log2(*M*) と論理 Far が等しいかを返します。

### GetHashCode

### ToString

`SmoothLog S=1 log2(M)=…` の形の文字列を返します。論理 Far は含みません。
