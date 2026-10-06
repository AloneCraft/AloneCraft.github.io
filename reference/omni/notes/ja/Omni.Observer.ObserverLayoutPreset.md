---
description: "観察の枠の並べ方です。"
---
観察の枠の並べ方です。[RadialObserverRig](../radial-observer-rig/) の `Layout` で選びます。枠は画面の中に収まり、互いに重なりません。表示しない枠の Camera は無効になります。

<!-- members -->

### OrbitWipe

周回の枠（枠 0）1 つを、`Corner` の隅に小さく置きます。一辺は `OrbitWipeArea` です。時間で変わる異常を探す用途です。

### FixedGrid

固定の 4 枠（枠 1〜4: Front、Right、Top、Iso）を、`Corner` の隅に 2×2 で置きます。全体の一辺は `GridArea` です。角度ごとの静止した確認と、画像の比較の用途です。

### Combined

周回の枠を一辺 `GridArea` で置き、固定の 4 枠を一辺 `GridArea` の 1/4 で、その横（画面の内側）に縦に並べます。`GridArea` は 0.8 までに丸めます。
