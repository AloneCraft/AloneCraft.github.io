---
description: "Scalar を Unity のシリアライズで保存するためのラッパーです。"
---

[Scalar](../scalar/) を Unity のシリアライズで保存するためのラッパーです。`Scalar` は書き込めるフィールドを持たないので、Unity はそのままでは保存できません。`ScalarField` は値を 64 bit × 3 の配列にして保存し、読み込むときに `Scalar` へ戻します。`Scalar` とは暗黙に変換できるので、通常の `Scalar` と同じように読み書きできます。

[RadialCamera](../../components/radial-camera/) の `nearClip` と `farClip` もこの型です。

```csharp
using UnityEngine;
using L = Omni.Logical;

public sealed class OrbitRadius : MonoBehaviour
{
    [SerializeField]
    private L.ScalarField m_Radius = L.ScalarField.FromString("149597870700");

    public L.Scalar Radius => m_Radius; // Scalar へ暗黙に変換する
}
```

Inspector の欄は 10 進の全桁で表示します。数値のほかに式（`+`、`*`、`pi`、`sqrt` など）も受け付けます。保存するのは評価した値だけです。式を使えるのは Editor の Inspector だけで、スクリプトの `FromString` と `Scalar.TryParse` は数値だけを受け付けます。使える式は [Logical 座標と Proxy](../../../docs/concepts/logical-coordinates/) を見てください。

<!-- members -->

### m_Limbs

保存される配列です。値の生の 192 bit を、下位から 64 bit ずつ 3 つに分けて持ちます。直接は編集しません。

### implicit operator Omni.Logical.Scalar

`ScalarField` を `Scalar` へ変換します。

### implicit operator Omni.Logical.ScalarField

`Scalar` を `ScalarField` へ変換します。

### FromString

10 進の文字列（指数の表記も可）から作ります。変換は `Scalar.TryParse` と同じです。変換できない文字列では 0 を返します。失敗を区別したいときは `Scalar.TryParse` を使います。

### OnBeforeSerialize

Unity が保存する前に呼びます。今の値を 64 bit × 3 の配列へ書きます。

### OnAfterDeserialize

Unity が読み込んだ後に呼びます。64 bit × 3 の配列は今の形式として読みます。64 bit × 2 の配列は以前の形式（Q95.32）として、同じ数値に読み替えます。配列が無い、またはそれ以外の長さのときは 0 になります。`ScalarField` 自身は警告を出しません。
