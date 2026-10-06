---
description: "固定の演出の姿勢を、等間隔の時刻のサンプル列として Logical のまま焼いたアセットです。"
---
固定の演出のカメラと光源の姿勢を、等間隔の時刻のサンプル列として Logical のまま焼いた ScriptableObject です。位置などの空間の値は、固定小数点の raw の値のまま保存し、`float` や `double` にしません。`double` を使うのは時刻（最初のサンプルの時刻と間隔）だけです。

- Omni 0.1 には、`PoseBake` を作る Editor の画面やメニューはありません。[PoseBakeChannel](../pose-bake-channel/) にサンプルを書き、`SetData` で設定して、スクリプトで Asset として保存します。
- 再生は Timeline の [PoseBakeTrack](../pose-bake-track/) と [LightBakeTrack](../light-bake-track/) が行います。再生の毎フレーム、`TryValidate` で、そのまま使えるかを確かめます。

作り方と再生の規則は [Sequencer と Pose Bake](../../../docs/guides/sequencer/) を見てください。

<!-- members -->

### CurrentFormatVersion

今の実装が読めるデータの形式の版です（現在 1）。`FormatVersion` がこの値と違うベイクは、`TryValidate` が `FormatVersionMismatch` で使えないと判定します。

### m_FormatVersion

データの形式の版です。`SetData` が `CurrentFormatVersion` を記録します。

### m_TotalBits

焼いたときの Logical の固定小数点の総ビット数です。`SetData` が記録します。

### m_FractionalBits

焼いたときの Logical の固定小数点の小数部のビット数です。`SetData` が記録します。

### m_RotationFractionalBits

焼いたときの回転の固定小数点の小数部のビット数です。`SetData` が記録します。

### m_SceneGuid

焼いた Scene の Asset の GUID です。

### m_SourceGuid

焼いたシーケンス（Timeline のアセット）の GUID です。

### m_SourceContentHash

焼いたときのシーケンスの内容と配置のハッシュです。焼いた後の変更を見つけるために使います。

### m_BakedUtc

焼いた時刻の文字列です（ISO 8601、UTC を想定）。診断の表示用で、照合には使いません。

### m_StartTimeSeconds

最初のサンプルの時刻（秒）です。シーケンスの時刻で表します。

### m_SampleIntervalSeconds

サンプルの間隔（秒）です。

### m_SampleCount

サンプルの数です。すべてのチャンネルが同じ数のサンプルを持ちます。

### m_TargetIds

注視対象の安定した ID の表です。チャンネルの注視対象の索引は、この表を指します。

### m_Channels

焼いたチャンネル（[PoseBakeChannel](../pose-bake-channel/)）の配列です。

### FormatVersion

データの形式の版を返します。

### SceneGuid

焼いた Scene の Asset の GUID を返します。

### SourceGuid

焼いたシーケンス（Timeline のアセット）の GUID を返します。

### SourceContentHash

焼いたときのシーケンスの内容と配置のハッシュを返します。

### BakedUtc

焼いた時刻の文字列を返します。

### StartTimeSeconds

最初のサンプルの時刻（秒）を返します。

### SampleIntervalSeconds

サンプルの間隔（秒）を返します。

### SampleCount

サンプルの数を返します。

### EndTimeSeconds

最後のサンプルの時刻（秒）を返します。`StartTimeSeconds + SampleIntervalSeconds × (SampleCount − 1)` です。サンプルが無いときは `StartTimeSeconds` を返します。

### TargetIds

注視対象の ID の表を返します。返すのは内部の配列そのものです。

### Channels

チャンネルの配列を返します。返すのは内部の配列そのものです。

### SetData

焼いた結果を設定します。焼く処理から呼びます。照合の情報、時刻、ID の表、チャンネルを設定し、同時に今の形式の版（`CurrentFormatVersion`）と固定小数点の構成（`Precision` の総ビット数、小数部のビット数、回転の小数部のビット数）を記録します。`targetIds` と `channels` が `null` なら空の配列にします。値の検証はしません。

### TryGetChannel

対象のパス（`PoseBakeChannel.TargetPath`）が一致するチャンネルを探します。比較は大文字小文字を区別します。見つからなければ `false` を返し、`channel` は `null` です。

### TryGetTargetId

索引から注視対象の ID を引きます。索引が範囲の外なら `false` を返し、`targetId` は `null` です。

### TryValidate

このベイクをそのまま使えるかを確かめます。使えないときは `false` を返し、最初に見つかった理由を `rejection` に入れます。確かめる順は、形式の版（`FormatVersionMismatch`）、固定小数点の構成（`PrecisionMismatch`）、Scene（`SceneMismatch`）、シーケンス（`SourceMismatch`）、内容のハッシュ（`ContentHashMismatch`）、サンプル（`EmptySamples`）です。期待する値が空の項目は照合しません。サンプルは、数と間隔が正であることと、すべてのチャンネルのサンプル数が一致し配列の長さが正しいことを確かめます。固定小数点の構成は実行時や設定では変わりませんが、構成の違う Omni の版で焼いたベイクは、焼き直すまで使えません。
