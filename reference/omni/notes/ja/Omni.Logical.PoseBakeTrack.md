---
description: "焼いた Logical の姿勢で Omni.Logical.Transform を動かす Timeline の Track です。"
---
焼いた Logical の姿勢で [Omni.Logical.Transform](../../components/logical-transform/) を動かす Timeline の Track です。bind する型は `Omni.Logical.Transform`、置ける Clip は [PoseBakeClip](../pose-bake-clip/) です。

毎フレーム、重みが一番大きい Clip の [PoseBake](../pose-bake/) を確かめ、シーケンスの時刻で位置と回転を補間して、`LocalPosition` と `LocalRotation` に書き、Unity の Transform へ同期します。使えないときは何も書かず、理由を [BakePlaybackController](../bake-playback-controller/) に報告します。

- Unity の Transform を直接書く Timeline の Animation Track は使えません。Logical Transform が Transform の唯一の書き手であるという約束を壊し、大きな座標では姿勢が `float` に丸められて Logical の値と食い違うためです。
- 標準の `PoseBakeTrack` は、ミキサー（`PoseBakeMixerBehaviour`）の `Camera` を設定しません。そのため、焼いた画角とクリップの範囲はカメラに書かず、姿勢だけを書きます。
- ミキサーを作る `CreateMixerPlayable` は protected の virtual です。Profiler のマーカーを足すなど、派生のミキサーを使うときに override します。

再生の規則は [Sequencer と Pose Bake](../../../docs/guides/sequencer/) を見てください。

<!-- members -->

### m_ChannelTargetPath

この Track が動かすチャンネルの識別子です。Clip の Channel Target Path が空のときに使います。Clip にも Track にも識別子が無いと、空の識別子のチャンネルを探します。

### ChannelTargetPath

この Track が動かすチャンネルの識別子を取得・設定します。設定した値は、次にミキサーを作ったときから使います。

### CreateTrackMixer

Timeline がグラフを作るときに呼びます。各 Clip に、シーケンスの上の開始の時刻（`PoseBakeClip.ClipStart`）を書いてから、ミキサーを作ります。ミキサーには Track の識別子と、Director の GameObject の `BakePlaybackController`（あれば）を渡します。
