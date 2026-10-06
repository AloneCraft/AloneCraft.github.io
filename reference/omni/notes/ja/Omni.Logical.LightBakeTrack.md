---
description: "焼いた回転、光の強さ、色で Light を動かす Timeline の Track です。"
---
焼いた回転、光の強さ、色で `Light` を動かす Timeline の Track です。bind する型は `Light`、置ける Clip は [PoseBakeClip](../pose-bake-clip/) です。チャンネルは `LightRotation` の種類（[PoseBakeChannelKind](../pose-bake-channel-kind/)）を使います。

毎フレーム、重みが一番大きい Clip の [PoseBake](../pose-bake/) を確かめ、シーケンスの時刻で値を補間して、次を書きます。

- `Light` の Transform の回転（world の回転）。Logical の回転は、ここで初めて Unity の回転にします。
- 光の強さ。補間した値が 0 より大きいときだけ書きます。
- 色。

位置は書きません。Directional Light の陰影は向きだけで決まるためです。使えないときは何も書かず、理由を [BakePlaybackController](../bake-playback-controller/) に報告します。ミキサーを作る `CreateMixerPlayable` は protected の virtual で、派生のミキサーを使うときに override します。

再生の規則は [Sequencer と Pose Bake](../../../docs/guides/sequencer/) を見てください。

<!-- members -->

### m_ChannelTargetPath

この Track が動かすチャンネルの識別子です（`"KeyLight"` など）。Clip の Channel Target Path が空のときに使います。

### ChannelTargetPath

この Track が動かすチャンネルの識別子を取得・設定します。設定した値は、次にミキサーを作ったときから使います。

### CreateTrackMixer

Timeline がグラフを作るときに呼びます。各 Clip に、シーケンスの上の開始の時刻（`PoseBakeClip.ClipStart`）を書いてから、ミキサーを作ります。ミキサーには Track の識別子と、Director の GameObject の `BakePlaybackController`（あれば）を渡します。
