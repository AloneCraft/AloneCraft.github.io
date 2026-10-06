---
description: "焼いたサンプルを使えないと判断した理由です。"
---
焼いたサンプルを使えないと判断した理由です。`PoseBake.TryValidate`、[PoseBakeSampler](../pose-bake-sampler/)、ベイクの Track が返し、[BakePlaybackController](../bake-playback-controller/) に集まります。使えないことは正常な動作の一部です（焼いたデータが無くても、利用者の側の代わりの計算で絵を出せます）。そのため、`bool` 1 つではなく、どの前提が崩れたのかを値で返します。

`TargetMoved`、`ObserverEyeRequired`、`UserCameraOverride` は、Omni の Track は自分では返しません。利用者が `BakePlaybackController.ExternalOverride` に設定して、焼いた姿勢の適用を止めるために使います。詳しくは [Sequencer と Pose Bake](../../../docs/guides/sequencer/) を見てください。

<!-- members -->

### None

使えます。棄却していません。

### MissingAsset

Clip に `PoseBake` が設定されていません。サンプラーに `null` を渡したときも返します。

### FormatVersionMismatch

データの形式の版が、今の実装（`PoseBake.CurrentFormatVersion`）と違います。

### PrecisionMismatch

焼いたときと今で、固定小数点の構成（総ビット数、小数部のビット数、回転の小数部のビット数）が違います。値の意味が変わるので、詰め直さずに棄却します。

### SceneMismatch

焼いた Scene が、照合する Scene と違います。

### SourceMismatch

焼いたシーケンス（Timeline）が、照合するシーケンスと違います。

### ContentHashMismatch

内容のハッシュが違います。焼いた後に、シーケンスか配置が変わっています。

### EmptySamples

サンプルが無い、間隔が 0 以下、またはチャンネルの配列の長さが宣言と合いません。

### TimeOutOfRange

Track に、今の時刻で重みを持つ Clip がありません。焼いた範囲の外の時刻はこの理由にならず、端のサンプルで止まります。

### ChannelUnavailable

指定した識別子のチャンネルが `PoseBake` にありません。

### TargetUnresolved

Track に書き込み先（`Omni.Logical.Transform` または `Light`）が bind されていません。

### TargetMoved

焼いたときと今で、対象の位置が違います。`ExternalOverride` に設定して使います。

### ObserverEyeRequired

観察の視点の差が必要です。焼いたデータは絶対的な Logical の座標なので使えません。`ExternalOverride` に設定して使います。

### UserCameraOverride

利用者がカメラを操作したので、演出の姿勢で上書きできません。`ExternalOverride` に設定して使います。

### InterpolationFailed

Logical の値の復元、補間、または Unity の回転への変換に失敗しました。
