---
description: "1 本のベイクのチャンネルが何を動かすかを表します。"
---
1 本のベイクのチャンネル（[PoseBakeChannel](../pose-bake-channel/)）が何を動かすかを表します。種類によって、`PoseBakeChannel.CreateForBake` が確保する値が変わります。

<!-- members -->

### CameraPose

カメラの姿勢です。位置、回転、カメラの距離、注視点のオフセット、注視対象の索引、画角、クリップの範囲を持ちます。[PoseBakeTrack](../pose-bake-track/) で再生します。

### LightRotation

主光源の姿勢です。回転、光の強さ、色を持ち、位置は持ちません（Directional Light の陰影は向きだけで決まるため）。[LightBakeTrack](../light-bake-track/) で再生します。
