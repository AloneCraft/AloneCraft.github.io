---
description: "シーケンスの時刻の 1 区間を、どの PoseBake のどのチャンネルで動かすかを持つ Timeline の Clip です。"
---
シーケンスの時刻の 1 区間を、どの [PoseBake](../pose-bake/) のどのチャンネルで動かすかを持つ Timeline の Clip です。[PoseBakeTrack](../pose-bake-track/) と [LightBakeTrack](../light-bake-track/) に置きます。

- Clip 自体はサンプルを持ちません。シーケンス全体で 1 つの `PoseBake` を指し、焼いたデータはシーケンスの時刻（Clip の開始の時刻 + Clip の中の経過時刻）で引きます。ショットごとにサンプル列を分けると、境目でサンプルが二重に必要になり、Clip を動かすたびに焼き直すことになるためです。
- Clip はブレンドもループもしません（`clipCaps` が `None`）。ショットの境目では切り替わります。

再生の規則は [Sequencer と Pose Bake](../../../docs/guides/sequencer/) を見てください。

<!-- members -->

### m_Bake

この Clip が参照する `PoseBake` です。未設定なら、Track は書かずに `MissingAsset` を報告します。代わりの姿勢を決めるのは、演出を動かす利用者の側です。

### m_ChannelTargetPath

動かすチャンネルの識別子です（`"Camera"`、`"KeyLight"` など）。空なら Track の Channel Target Path を使います。

### Bake

参照する `PoseBake` を取得・設定します。

### ChannelTargetPath

動かすチャンネルの識別子を取得・設定します。

### ClipStart

この Clip がシーケンスの上で始まる時刻（秒）です。Track がミキサーを作るたびに書きます。シリアライズしないので、Clip をドラッグしても焼いたデータは無効になりません。

### clipCaps

Clip の機能です。常に `ClipCaps.None` を返し、ブレンドもループも許しません。2 つのショットのカメラの姿勢を混ぜた中間の姿勢には意味が無いためです。

### CreatePlayable

Timeline が Clip の Playable を作るときに呼びます。`Bake`、`ChannelTargetPath`、`ClipStart` を Playable に渡します。値を書くのは Track のミキサーです。
