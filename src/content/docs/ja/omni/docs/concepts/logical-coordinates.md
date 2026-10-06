---
title: Logical 座標と Proxy
description: 位置の正本を Logical 座標で持ち、Unity の Transform を有限な写しとして扱う仕組み。
sidebar:
  order: 1
---

## float では足りない理由

Unity の Transform は位置を 32 bit の `float` で持ちます。`float` の有効桁は 2 進で 24 桁です。そのため、表せる間隔は値の大きさに比例して広がります。

| 原点からの距離 | `float` で表せる間隔（およそ） |
| --- | --- |
| 1 m | 1.2×10⁻⁷ m（0.12 µm） |
| 10 km | 1 mm |
| 1,000 km | 6 cm |
| 1 天文単位（約 1.5×10¹¹ m） | 16 km |
| 1 光年（約 9.5×10¹⁵ m） | 1.1×10⁹ m |

天体規模の空間では、この間隔より細かく物を置けません。物体が震えたり、重なったり、置けなかったりします。

Omni は位置を固定小数点の **Logical 座標**で持ちます。固定小数点は、原点からの距離によらず分解能が同じです。

## 正本と Proxy

- `Omni.Logical.Transform`（Add Component の `Omni/Logical Transform`）が、位置・回転・Scale の正本です。
- 親子の合成、距離の判定は Logical の型のまま行います。
- 描画では、Logical の値のままカメラとの差を求め、カメラ基準にしてから有限な値へ変換します。巨大な絶対座標を先に `float` へ落としません。
- Unity の Transform は、有限な値へ変換した表示用の写し（Proxy）です。Logical Transform が Unity の Transform へ一方向に書きます（`SyncUnityTransform()`）。
- Logical Transform を付けた GameObject では、Unity の Transform の Inspector は読み取り専用になります。「この Transform は LogicalTransform によって管理されています。」と表示されます。

### 安全域と Proxy の値

Unity の Transform に入る値は、Logical の値の大きさで変わります。安全域の既定値は 10,000 m です。

| Logical の値 | Unity の Transform に入る値 |
| --- | --- |
| 安全域の中 | Logical の値を `float` にしたもの |
| 安全域の外で、`float` で表せる | 精度を落とした実際の値。正本としては扱わない |
| world の位置が 10¹⁸ m を超える、または `float` で表せない | 位置 0、Scale 1 のプレースホルダ |

安全域の外にあるとき、Logical Transform の Inspector に次のどちらかが出ます。

- 「Transform: 実座標圏（物体）外のため、精度を落とした実値が入っています。」
- 「Transform: 実座標圏（物体）外かつ表現不能のため、プレースホルダ（位置0・スケール1）です。」

RadialCamera を持つカメラが安全域の外にある場合、Unity のカメラの位置は原点に置き、回転だけを Logical の値に合わせます。描画は、Logical のカメラの位置からカメラ相対で組み立てます。

:::caution[Unity の Transform を正本として読まない]
安全域の外では、Unity の Transform の値から位置を判断しないでください。プレースホルダは位置 0 なので、値だけを見ると安全域の中に見えます。`IsPoseNeutralizedForFiniteProxy` と `IsUnityTransformPlaceholder` で状態を確かめられます。
:::

### Edit Mode で Transform を動かした場合

- 安全域の中では、Scene View で Unity の Transform を動かすと、その値を Logical の値へ取り込みます。取り込んだ値は `float` の精度です。
- 安全域の外では取り込みません。Logical の値で書き戻し、Console に警告を出します。位置・回転・Scale は Inspector の Logical Transform で編集してください。

## 数値の形式

| 値 | 型 | 形式 | 範囲と分解能 |
| --- | --- | --- | --- |
| 位置・距離・Scale | `Scalar` | Q95.96（192 bit の固定小数点） | ±2⁹⁵ m（約 ±3.96×10²⁸ m）、分解能 2⁻⁹⁶ m（約 1.26×10⁻²⁹ m） |
| 回転（Quaternion の各成分） | `RotationScalar` | Q1.126（128 bit の固定小数点） | 回転の丸めが位置に与える誤差は、距離のおよそ 2×10⁻³⁸ 倍 |

形式は固定です。実行時や設定で変わりません。Scale は符号付きの XYZ を `Scalar` で持ちます。

### 溢れる演算は Try の API

溢れる可能性のある演算は、すべて `Try` で始まる API です。結果が範囲を超えると `false` を返します。結果を黙って折り返したり、飽和させたりしません。

- 乗算、除算、文字列からの変換は、分解能 2⁻⁹⁶ に最近接偶数で丸めます。
- `float` や `double` との変換（`TryFromFloat`、`TryToFloat` など）は、非有限値と範囲外の値で失敗します。
- `Transform.SetWorld` は、親の Scale が 0 などで逆変換できないとき、何も変えずに `false` を返します。

## コードから使う

`Omni.Logical` の `Transform`、`Vector3`、`Quaternion` は、Unity の型と同じ名前です。別名を付けて使うと区別しやすくなります。

```csharp
using UnityEngine;
using L = Omni.Logical;

public sealed class PlaceAtOneAu : MonoBehaviour
{
    private void Start()
    {
        var logical = GetComponent<L.Transform>();
        if (logical == null ||
            !L.Scalar.TryParse("149597870700", out L.Scalar au))
        {
            return;
        }

        var position = new L.Position3(new L.Vector3(au, L.Scalar.Zero, L.Scalar.Zero));
        if (!logical.SetWorld(position, L.Quaternion.Identity, L.Vector3.One))
        {
            Debug.LogError("SetWorld failed.");
            return;
        }

        // Unity の Transform（Proxy）を今の値に合わせる
        logical.SyncUnityTransform();

        if (logical.TryGetWorld(out L.Position3 world, out L.Quaternion _, out L.Scalar _))
        {
            Debug.Log(world); // 10 進の全桁で出る
        }
    }
}
```

| API | 内容 |
| --- | --- |
| `LocalPosition`、`LocalRotation`、`LocalScale` | ローカルの値。固定小数点のまま読み書きする |
| `TryGetWorld`、`TryGetWorldTrs`、`TryGetWorldAffine` | 親を合成した world の値 |
| `SetWorld`、`TranslateWorld` | world の値で設定する。world の差分を加える |
| `SyncUnityTransform()` | Unity の Transform へ書く |
| `Position3.TryDifference`、`Vector3.TryLength` | 2 点の差と長さを Logical の精度で求める |
| `Scalar.TryParse`、`Scalar.ToString()` | 10 進の文字列との変換。`ToString()` は全桁を出す |

:::tip[ScalarField.FromString]
`ScalarField.FromString` は、変換できない文字列でゼロを返します。失敗を区別したいときは `Scalar.TryParse` を使います。
:::

## 階層の規則

Logical Transform に Logical Transform の祖先がある場合、それは**直接の親**でなければなりません。

| 階層 | 扱い |
| --- | --- |
| Logical Transform | 可 |
| Logical Transform → 通常の Transform | 可 |
| Logical Transform → 通常の Transform → 通常の Transform | 可 |
| Logical Transform → Logical Transform（直接の親子） | 可 |
| Logical Transform → 通常の Transform → Logical Transform | **禁止** |

禁止の配置では、間の通常の Transform のローカルの変換が Logical の合成から抜けます。一方、Unity の階層はそれを適用します。そのため、Logical の値と描画の位置がずれます。

禁止の配置を見つけると、Omni は次のようにします。

- Console にエラーを出します。Inspector には「階層が不正です（LogicalTransform → 通常Transform → LogicalTransformの再開）。」と出ます。
- その Logical Transform は親との合成をやめ、自分の値だけで位置を決めます。壊れた合成の結果は出しません。

直すには、その GameObject を Logical Transform の直接の子にするか、間の Transform にも Logical Transform を付けます。`IsInForbiddenHierarchyPlacement()` と `TransformHierarchy.IsValid` で判定できます。

:::note[子に追加するとき]
祖先に Logical Transform の無い GameObject へ Logical Transform を追加すると、「親オブジェクトにLogicalTransformがありません。ルートまでLogicalTransformをアタッチしますか？」と聞かれます。「OK（追加する）」はルートまでの祖先にも追加し、1 回の Undo にまとめます。「キャンセル」は追加した Logical Transform を取り除きます。
:::

## ほかの書き手と競合させない

1 つの Transform の書き手は Logical Transform だけです。同じ GameObject に次のものがあると、有効化のときに Console にエラーを出します。

- `CinemachineBrain`、`CinemachineCamera`
- `CharacterController`
- kinematic でない `Rigidbody`

Editor と Development Build では、Omni が書いた後に別の経路で Transform が書き換えられたことも検出します。検出すると Console にエラーを出し、Inspector に「Transform: 論理値と食い違っています（他の書き手を検出）。」と出します。

## Logical Transform を外す

| 方法 | 内容 |
| --- | --- |
| `TryDetach(out DetachResult)` | 今のローカルの値を Unity の Transform へ Bake してから、Logical Transform を削除する。直接の Logical の子は 1 段上へ移し、world の姿勢を保つ。Editor では Undo できる |
| `TryBakeToUnityTransformLossy(out DetachResult)` | 削除せず、今のローカルの値を Unity の Transform へ書く |
| Remove Component で直接外す | 最後のローカルの値を Unity の Transform へ書く。`float` に収まらない値は ±`float.MaxValue` に制限し、エラーを出す |

`TryDetach` は失敗すると何も変えず、理由を `DetachResult.Status` で返します。

| `DetachStatus` | 意味 |
| --- | --- |
| `Success` | Bake と削除が完了した |
| `AlreadyDetached` | 既に削除されている |
| `FloatRangeExceeded` | 位置または Scale が `float` の範囲を超えている |
| `InvalidHierarchy` | 禁止の配置にある |
| `SingularParent` | 親の Scale が 0 などで逆変換できない |
| `ConversionFailed` | Logical の値の変換に失敗した |

## Inspector で値を入れる

### Logical Transform

- 「位置」と Scale は、10 進の文字列で全桁を表示し、編集します。10 進の表記と指数の表記（例: `1.496e11`）を受け付けます。
- 変換できない入力は無視し、直前の値を残します。
- X、Y、Z のラベルをドラッグすると、標準の Transform と同じように値を変えられます。
- Scale の鍵のボタンが ON の間は、1 つの軸を変えると、ほかの軸にも同じ倍率をかけます。
- Rotation は `float` の Euler 角で編集します。

### ScalarField の欄（RadialCamera の Near、Far など）

`ScalarField` の欄は、数値のほかに式を受け付けます。保存するのは評価した結果の値だけで、式は保存しません。

| 種類 | 使えるもの |
| --- | --- |
| 演算子 | `+`、`-`、`*`、`/`、`^`（整数の指数。絶対値は 100 万以下）、括弧 |
| 定数 | `pi`、`tau`、`e`、`phi`、`sqrt2` |
| 関数 | `abs`、`sqrt`、`floor`、`ceil`、`round`、`min`、`max`、`clamp`、`lerp` |

- 計算は Logical の精度で行います。途中で `float` を使いません。
- 範囲を超える、0 で割る、知らない名前がある、などの式は適用しません。欄の下にエラーと位置を出し、直前の値を残します。
- 編集中に Esc を押すと、入力を取り消します。

## 保存したデータの互換

- 位置と Scale は、値ごとに 64 bit × 3（24 バイト）で保存します。回転は成分ごとに 64 bit × 2 です。
- 以前の形式（位置と Scale が 64 bit × 2、Q95.32）で保存された値も、同じ数値として読みます。
- それ以外の長さのデータはゼロとして読み、1 つの Logical Transform につき 1 回、Console に警告を出します。
