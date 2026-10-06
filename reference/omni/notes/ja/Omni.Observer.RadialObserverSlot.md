---
description: "観察の枠 1 つの設定です。Camera と、レイアウトの上の枠の番号、種類、角度、画角を持ちます。"
---
観察の枠 1 つの設定です。枠の Camera と、レイアウトの上の枠の番号を持ちます。画面の上の配置と表示・非表示は [RadialObserverRig](../radial-observer-rig/) が決め、カメラの距離と角度は [RadialObserverOrbit](../radial-observer-orbit/) が Cinemachine で動かします。

同梱の Prefab の枠は次のとおりです。

| 番号 | 枠 | 種類 | Yaw | Pitch |
| --- | --- | --- | --- | --- |
| 0 | Orbit | `Orbit` | 0 | 25 |
| 1 | Front | `Fixed` | 0 | 0 |
| 2 | Right | `Fixed` | 90 | 0 |
| 3 | Top | `Fixed` | 0 | 89 |
| 4 | Iso | `Fixed` | 45 | 35.264 |

平行投影の Camera は使いません。Radial は平行投影では常に恒等の写像になり、変形後の世界を見られないためです。平行投影の Camera を設定すると、Inspector で値を変えたときに Console に警告を出します。詳しくは [Radial 観察カメラ](../../../docs/guides/observer-camera/) を見てください。

<!-- members -->

### m_Camera

この枠の Camera です。この Camera に `RadialCamera` を付けないでください。付いている Camera は、Rig が警告を出して登録しません。

### m_Index

レイアウトの上の枠の番号です（0〜4）。0 が周回の枠、1〜4 が固定の枠（Front、Right、Top、Iso）の位置です。

### m_Kind

枠の種類です。`Orbit` は自動で周回し、マウスで操作できます。`Fixed` は角度を固定します。

### m_Yaw

固定の枠の水平角（度、−180〜180）です。Cinemachine の OrbitalFollow の水平軸の値として使います。`Orbit` の枠では使いません。

### m_Pitch

固定の枠の仰角（度、−89〜89）です。Cinemachine の OrbitalFollow の垂直軸の値として使います。`Orbit` の枠では使いません。

### m_FieldOfView

この枠のカメラの垂直の画角（度、5〜120）です。

### Camera

この枠の Camera を返します。未設定なら、この GameObject とその子から（無効な子も含めて）最初の Camera を探して覚えます。

### Index

レイアウトの上の枠の番号を返します。

### Kind

枠の種類を返します。

### Yaw

固定の枠の水平角（度）を返します。

### Pitch

固定の枠の仰角（度）を返します。

### FieldOfView

画角（度）を返します。

### Configure

Camera、枠の番号、種類、水平角、仰角、画角をコードから設定します。Prefab の生成とテスト用です。枠の番号は 0〜4 に丸めます。ほかの値は丸めません。
