---
title: Sequencer と Pose Bake
description: 固定の演出のカメラと光源の姿勢を、Logical のまま時刻のサンプル列に焼き、Timeline で再生する。
sidebar:
  order: 9
---

`PoseBake` は、固定の演出の姿勢を、等間隔の時刻のサンプル列として Logical のまま焼いたアセットです。演出が固定なら、対象を探す、親子の合成をやり直す、カメラの距離と注視点を組み直す、主光源の向きを求める、といった処理は時刻だけで決まります。焼いておけば、再生のときにこれらを毎フレーム計算しなくて済みます。

`PoseBake` は特定の Scene 専用ではありません。`Omni.Logical.Transform` と `Light` を動かす Scene なら、どれでも同じ形式を使えます。

## 必要なもの

この機能のコードは Unity の Timeline（`com.unity.timeline`）を使います。

:::note[作り方について]
Omni 0.1 には、`PoseBake` を作る Editor の画面やメニューはありません。`PoseBake` はスクリプトで作ります（[スクリプトで焼く](#スクリプトで焼く)）。
:::

## 部品

型はすべて `Omni.Logical` の名前空間にあります。

| 型 | 役割 |
| --- | --- |
| `PoseBake` | サンプル列と、使えるかを照合するための情報を持つ ScriptableObject |
| `PoseBakeChannel` | 1 つの対象（カメラ、主光源など）の時系列のサンプル |
| `PoseBakeTrack` | 焼いた姿勢で `Omni.Logical.Transform` を動かす Timeline の Track |
| `LightBakeTrack` | 焼いた回転・強さ・色で `Light` を動かす Timeline の Track |
| `PoseBakeClip` | シーケンスの時刻の 1 区間を、どの `PoseBake` のどのチャンネルで動かすかを持つ Clip |
| `BakePlaybackController` | 焼いたデータを使えたかを、シーケンス全体で 1 か所に集める Component |
| `PoseBakeSampler` | サンプル列から任意の時刻の値を求める関数 |
| `PoseBakeRejection` | 焼いたデータを使えなかった理由 |

## 何を保存するか

| 値 | 保存の形 |
| --- | --- |
| 位置、カメラの距離、注視点のオフセット | Logical の raw の値のまま。`float` や `double` にしない |
| 回転 | Logical の Quaternion |
| 画角、クリップの範囲、光の強さ、色 | `float`（空間の量ではないため） |
| 時刻 | 最初のサンプルの時刻と間隔（秒、`double`）、サンプル数 |
| 照合の情報 | 焼いた Scene の GUID、シーケンス（Timeline のアセット）の GUID、内容のハッシュ、形式の版、固定小数点の構成 |

チャンネルの種類（`PoseBakeChannelKind`）は 2 つです。

| 種類 | 持つ値 |
| --- | --- |
| `CameraPose` | 位置、回転、画角、クリップの範囲、注視対象の索引、カメラの距離、注視点のオフセット |
| `LightRotation` | 回転、強さ、色。位置は持たない（Directional Light の陰影は向きだけで決まるため） |

## スクリプトで焼く

1. `PoseBakeChannel.CreateForBake(targetPath, kind, sampleCount)` でチャンネルを作ります。`targetPath` は `"Camera"` や `"KeyLight"` のような、対象を指す安定した識別子です。
2. サンプルごとに値を書きます。`CameraPose` は `WritePosition`、`WriteRotation`、`WriteCameraSettings`、`WriteDistance`、`WriteLookOffset`、`LightRotation` は `WriteRotation` と `WriteLightShading` を使います。
3. `PoseBake` を作り、`SetData` で照合の情報、時刻、チャンネルを設定して、Asset として保存します。`SetData` は、今の形式の版と固定小数点の構成も記録します。

```csharp
using UnityEditor;
using UnityEngine;
using L = Omni.Logical;

static class MyShowBaker
{
    // EvaluateShow は、演出を時刻から評価する利用者の関数です。
    public static L.PoseBake Bake(
        double duration, string sceneGuid, string timelineGuid, string contentHash, string path)
    {
        const int rate = 120;
        double interval = 1.0 / rate;
        int count = (int)(duration * rate) + 1;

        var camera = L.PoseBakeChannel.CreateForBake(
            "Camera", L.PoseBakeChannelKind.CameraPose, count);
        for (int i = 0; i < count; i++)
        {
            EvaluateShow(i * interval, out L.Position3 position, out L.Quaternion rotation);
            camera.WritePosition(i, position);
            camera.WriteRotation(i, rotation);
            camera.WriteCameraSettings(i, 0, 55f, 0.03f, 1000f);
        }

        var bake = ScriptableObject.CreateInstance<L.PoseBake>();
        bake.SetData(
            sceneGuid, timelineGuid, contentHash, System.DateTime.UtcNow.ToString("o"),
            0.0, interval, count, new[] { "MyTarget" }, new[] { camera });
        AssetDatabase.CreateAsset(bake, path);
        return bake;
    }

    static void EvaluateShow(double time, out L.Position3 position, out L.Quaternion rotation)
    {
        // 利用者の演出の評価
        position = L.Position3.Zero;
        rotation = L.Quaternion.Identity;
    }
}
```

`WriteCameraSettings(index, targetIndex, fieldOfView, nearClip, farClip)` の `targetIndex` は、`SetData` に渡す注視対象の ID の表（`targetIds`）の索引です。

## Timeline で再生する

1. `PlayableDirector` の GameObject に `BakePlaybackController` を付けます。
2. Timeline に `PoseBakeTrack` を追加し、動かす `Omni.Logical.Transform` を bind します。光源には `LightBakeTrack` を追加し、`Light` を bind します。
3. Track に `PoseBakeClip` を置き、Bake に `PoseBake` を、Channel Target Path にチャンネルの識別子を設定します。Clip に識別子が無いときは、Track の Channel Target Path を使います。

再生の規則は次のとおりです。

- 焼いたデータは、シーケンスの時刻（Clip の開始の時刻 + Clip の中の経過時刻）で引きます。Clip は、シーケンス全体で 1 つの `PoseBake` を指します。Clip をドラッグしても、焼いたデータは無効になりません。
- Clip はブレンドもループもしません。ショットの境目では切り替わります。重みが一番大きい Clip を使います。
- 焼いた範囲の外の時刻は、端のサンプルで止まります。
- 位置は Logical のまま線形に補間し、回転は正規化した線形補間をします。クリップの範囲は対数で補間します。
- `PoseBakeTrack` は、`Omni.Logical.Transform` の `LocalPosition` と `LocalRotation` に書き、Unity の Transform へ同期します。Unity の Transform を直接書く Timeline の Animation Track は、Logical Transform の値と食い違うので使えません。
- `LightBakeTrack` は、`Light` の Transform の回転、強さ（0 より大きいときだけ）、色を書きます。

:::caution[画角とクリップの範囲]
`PoseBakeTrack` が作るミキサー（`PoseBakeMixerBehaviour`）は、`Camera` プロパティが設定されているときだけ、画角とクリップの範囲をカメラに書きます。標準の `PoseBakeTrack` はこのプロパティを設定しません。そのため、標準の Track では姿勢だけを書きます。
:::

## 使えないとき

Track は毎フレーム、焼いたデータを使えるかを確かめます。使えないときはその対象に書かず、理由を `BakePlaybackController` に報告します。データが無い場合でも絵を出せるように、代わりの姿勢を決めるのは、演出を動かす利用者の側です。

| `BakePlaybackController` の API | 内容 |
| --- | --- |
| `AllChannelsApplied` | 報告したチャンネルが 1 つ以上あり、すべてに焼いたデータを適用できたら `true` |
| `FirstRejection` | 適用できなかったチャンネルの最初の理由 |
| `ChannelStates` | チャンネルごとの直近の状態（診断用） |
| `ExternalOverride` | 焼いたデータの適用を、外の都合で止める。`PoseBakeRejection.None` で解除する |
| `ConfigureExpectations(sceneGuid, sourceGuid, contentHash)` | 照合する Scene、シーケンス、内容のハッシュを設定する。空の値は照合しない |
| `ResetReports()` | 報告を捨てる。演出の切替やシークの直前に呼ぶ |

カメラごと・光源ごとに判断を分けると、カメラだけ焼いたデータで光源だけ別の計算、のような食い違った絵になります。`AllChannelsApplied` が `false` の間は、すべての対象を同じ方法で動かしてください。EarthToMoon のデモは、`AllChannelsApplied` が `false` の間だけ、同じ演出の定義から姿勢を計算して書きます。利用者がカメラを操作している間は `ExternalOverride` を `UserCameraOverride` にします。

### 理由（`PoseBakeRejection`）

Track が自分で判定する理由です。

| 値 | 意味 |
| --- | --- |
| `MissingAsset` | Clip に `PoseBake` が設定されていない |
| `FormatVersionMismatch` | データの形式の版が、今の実装と違う |
| `PrecisionMismatch` | 焼いたときと今で、固定小数点の構成（総ビット数、小数部のビット数）が違う。値を詰め直さずに使わない |
| `SceneMismatch` | 焼いた Scene が今の Scene と違う |
| `SourceMismatch` | 焼いたシーケンス（Timeline）が違う |
| `ContentHashMismatch` | 焼いた後に、シーケンスまたは配置が変わった |
| `EmptySamples` | サンプルが無い、または配列の長さが宣言と合わない |
| `TimeOutOfRange` | 有効な Clip が無い |
| `ChannelUnavailable` | 指定したチャンネルが `PoseBake` に無い |
| `TargetUnresolved` | Track に書き込み先が bind されていない |
| `InterpolationFailed` | Logical の値の復元または補間に失敗した |

次の値は、`ExternalOverride` に設定して使う理由です。

| 値 | 意味 |
| --- | --- |
| `UserCameraOverride` | 利用者がカメラを操作したので、演出の姿勢で上書きできない |
| `ObserverEyeRequired` | 観察の視点の差が必要。焼いたデータは絶対的な Logical の座標なので使えない |
| `TargetMoved` | 焼いたときと今で、対象の位置が違う |
