---
description: "Logical の位置・回転・符号付きの XYZ Scale を固定小数点で持つ、姿勢の正本の Component です。"
---
`Omni.Logical.Transform`（Add Component の `Omni/Logical Transform`）は、位置・回転・Scale の正本です。位置と Scale は `Scalar`（Q95.96、192 bit）、回転は Q1.126（128 bit）の `Quaternion` で持ちます。Unity の Transform は、有限な値へ変換した表示用の写し（Proxy）です。

- 親子の合成は、直接の親にある `Omni.Logical.Transform` とだけ行います。階層の規則は [TransformHierarchy](../transform-hierarchy/) を見てください。
- コードからプロパティを書き換えても、Unity の Transform はすぐには変わりません。合わせるときは `SyncUnityTransform()` を呼びます。Inspector での編集、Undo、有効化のときは Omni が合わせます。
- Edit Mode で、安全域の中にある物体の Unity の Transform を Scene View で動かすと、その値を `float` の精度で取り込みます。安全域の外では取り込まず、Logical の値で書き戻します。
- `Omni.Logical` の `Transform` は Unity の `Transform` と同じ名前です。`using L = Omni.Logical;` のように別名を付けると区別できます。

仕組みと例は [Logical 座標と Proxy](../../../docs/concepts/logical-coordinates/) を見てください。

```csharp
using UnityEngine;
using L = Omni.Logical;

var logical = GetComponent<L.Transform>();
if (L.Scalar.TryParse("149597870700", out L.Scalar au) &&
    logical.SetWorld(new L.Position3(new L.Vector3(au, L.Scalar.Zero, L.Scalar.Zero)),
                     L.Quaternion.Identity, L.Vector3.One))
{
    logical.SyncUnityTransform(); // Unity の Transform（Proxy）を今の値に合わせる
}
```

<!-- members -->

### m_PositionX

ローカルの位置の X を、固定小数点の生のデータ（64 bit × 3）で保存します。Inspector では、この配列ではなく「位置」の欄で編集します。

### m_PositionY

ローカルの位置の Y の保存データです。形式は `m_PositionX` と同じです。

### m_PositionZ

ローカルの位置の Z の保存データです。形式は `m_PositionX` と同じです。

### m_RotationX

ローカルの回転（Quaternion）の X を、固定小数点の生のデータ（64 bit × 2）で保存します。Inspector では「Rotation」の欄（Euler 角）で編集します。

### m_RotationY

ローカルの回転の Y の保存データです。形式は `m_RotationX` と同じです。

### m_RotationZ

ローカルの回転の Z の保存データです。形式は `m_RotationX` と同じです。

### m_RotationW

ローカルの回転の W の保存データです。形式は `m_RotationX` と同じです。

### m_ScaleX

ローカルの Scale の X を、固定小数点の生のデータ（64 bit × 3）で保存します。符号付きです。Inspector では「Scale」の欄で編集します。

### m_ScaleY

ローカルの Scale の Y の保存データです。形式は `m_ScaleX` と同じです。

### m_ScaleZ

ローカルの Scale の Z の保存データです。形式は `m_ScaleX` と同じです。

### HasDetectedConflictingWriter

Omni が書いた後に、ほかの経路でこの Unity の Transform が書き換えられたことを検出していれば `true` です。このプロパティは Editor と Development Build にだけあります。それ以外のビルドで使うコードは `#if UNITY_EDITOR || DEVELOPMENT_BUILD` で囲んでください。

### LocalPosition

ローカルの位置です。固定小数点のまま読み書きします。

### LocalRotation

ローカルの回転です。設定した値は正規化して持ちます。正規化できない値を設定すると、単位の回転（Identity）になります。

### LocalScale

ローカルの符号付きの XYZ Scale です。固定小数点のまま読み書きします。

### TryGetWorld

親を合成した world の位置と回転、および Scale を返します。`worldScale` は、world の Scale の XYZ の絶対値の最大です。軸ごとの値が要るときは `TryGetWorldTrs` を使います。計算できないときは `false` を返します。

### TryGetWorldTrs

親を合成した world の位置、回転、符号付きの XYZ Scale を返します。演算が範囲を超える、親の基底から Scale を分解できない、親の深さが 256 を超える、などのときは `false` を返します。

### TryGetWorldAffine

親を合成した world の位置と、回転・非均一の Scale・Shear を含む 3×3 の基底を返します。`TryGetWorldTrs` の Scale は基底を分解した値で、Shear を表せません。Shear を含む姿勢を正確に扱うときは、こちらを使います。

### SetWorld

world の位置・回転・Scale を指定し、親を考慮したローカルの値にして設定します。回転を正規化できない、親の Scale が 0 などで逆変換できない、演算が範囲を超える、のどれかのときは、何も変えずに `false` を返します。Unity の Transform は変えないので、必要なら `SyncUnityTransform()` を呼びます。

### TranslateWorld

world の差分 `worldDelta` を、親の基底でローカルの差分にしてから位置に足します。巨大な絶対位置を `float` にせずに動かせます。親の基底を逆変換できない、または結果が範囲を超えるときは、何も変えずに `false` を返します。

### CaptureFromTransform

`source` のローカルの位置・回転・Scale（`float`）を、この Logical Transform のローカルの値として取り込みます。取り込む値は `float` の精度です。値が有限でない、または回転を正規化できないときは、何も変えずに `false` を返します。`source` が `null` なら `ArgumentNullException` を投げます。

### SyncUnityTransform

ローカルの Logical の値を、Unity の Transform（Proxy）へ書きます。安全域の外でも、`float` で表せる間はその値を書きます（正本としては扱いません）。world の値が 10¹⁸ m を超えるなど `float` で表せないときは、位置 0・Scale 1 のプレースホルダを書きます。`RadialCamera` を持つ Camera が安全域の外にあるときは、位置を原点にし、回転だけを合わせます。値が同じなら書きません。

### IsPoseNeutralizedForFiniteProxy

直前の `SyncUnityTransform()` で、安全域の外にあったために、Unity の Transform が正本ではない値（精度を落とした値、Camera の原点の Proxy、またはプレースホルダ）になっていれば `true` です。安全域の外かどうかは、Unity の Transform の値ではなく、このプロパティで判断してください。

### IsUnityTransformPlaceholder

直前の `SyncUnityTransform()` で、Unity の Transform にプレースホルダ（位置 0・Scale 1）を書いていれば `true` です。`IsPoseNeutralizedForFiniteProxy` が `true` で、こちらが `false` なら、精度を落とした値が入っています。

### IsInForbiddenHierarchyPlacement

禁止の配置（Logical Transform → 通常の Transform → Logical Transform）にあれば `true` を返します。`TransformHierarchy.IsValid` と同じ判定です。

### TryBakeToUnityTransformLossy

今のローカルの値を、Logical Transform を残したまま Unity の Transform へ書きます。位置または Scale が `float` で表せないときは、何も変えずに `false` を返し、`result.Status` は `FloatRangeExceeded` です。

### TryDetach

今のローカルの値を Unity の Transform へ書いてから、この Logical Transform を削除します。直接の子の Logical Transform は 1 段上の親へ移し、world の姿勢を保ちます。Edit Mode では Undo できます。失敗したときは Scene を元のままにして `false` を返し、理由を `result.Status`（[DetachStatus](../detach-status/)）で返します。

### OnBeforeSerialize

Unity の保存の処理から呼ばれます。何もしません。直接呼ぶ必要はありません。

### OnAfterDeserialize

Unity の読み込みの処理から呼ばれます。読み込んだ個体であることを記録します。直接呼ぶ必要はありません。Add Component で新しく作った個体だけが、有効化のときに Unity の Transform の値を取り込みます。
