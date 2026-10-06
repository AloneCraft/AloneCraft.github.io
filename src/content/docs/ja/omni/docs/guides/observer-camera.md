---
title: Radial 観察カメラ
description: RadialCamera が見ている変形後の世界を、別のカメラから外側へ引いて見る。
sidebar:
  order: 8
---

`RadialObserverRig` は、`RadialCamera` が見ている「変形後の世界」を、別のカメラから外側へ引いて見るための Rig です。Scene View の「客観」の比較表示（[Scene View の操作](../scene-view/)）と同じ考え方を、Game View と Player のカメラで使えます。

## 必要なもの

| パッケージ | 版 |
| --- | --- |
| `com.unity.cinemachine` | 3.0.0 以上 |
| `com.unity.inputsystem` | 1.0.0 以上 |

Cinemachine は Omni の必須のパッケージではありません。Cinemachine が無い環境では、観察カメラの Cinemachine を使う部分はコンパイルの対象から外れます。

## Scene に置く

1. GameObject > Omni > Radial Observer Camera を選びます。
2. Cinemachine が入っていない場合は、Package Manager から入れるかを確かめるダイアログが出ます。入れた後、Script の再コンパイルが終わってから、もう一度同じメニューを選びます。
3. Rig の Prefab（`Assets/Omni/Runtime/Observer/Prefabs/RadialObserverRig.prefab`）が Scene に置かれます。

Rig は、既定では Play Mode でだけ動きます。

## 構成

```text
RadialObserverRig       RadialObserverRig（登録、枠の配置）
 ├─ Focus               すべての枠が共有する注視点
 └─ Slot_*              Orbit、Front、Right、Top、Iso の 5 枠
     ├─ Camera          Camera、UniversalAdditionalCameraData、CinemachineBrain
     └─ VCam            CinemachineCamera、OrbitalFollow（Sphere）、RotationComposer、
                        RadialObserverOrbit、RadialObserverChannelGuard
```

Rig の原点は、ワープの中心（観察する `RadialCamera` の位置）に当たる有限な描画の空間です。Rig 自体は動かしません。中心への追従は Logical の側で行います。

## Rig の設定

`RadialObserverRig`（Add Component の `Omni/Radial Observer Rig`）の Inspector の項目です。

| 項目 | 内容 | 既定 |
| --- | --- | --- |
| Source | ワープの中心と曲線を借りる `RadialCamera`。未設定なら MainCamera のもの、無ければ Scene の中の有効な `RadialCamera` を探す | なし |
| Distance In Scale | 中心からカメラまでの距離を、Radial の尺度 S の何倍にするか（5〜200）。S は定数の 1（`RadialSettings.RadialScale`）なので、この値がそのまま Unity の距離（m）になる | 40 |
| Layout | 枠の並べ方 | `OrbitWipe` |
| Corner | 枠を寄せる画面の隅（`BottomRight`、`BottomLeft`、`TopRight`、`TopLeft`） | `BottomRight` |
| Orbit Wipe Area | `OrbitWipe` の周回の枠の一辺。画面の幅・高さに対する比 | 0.3 |
| Grid Area | `FixedGrid` と `Combined` が占める範囲の一辺。画面の幅・高さに対する比 | 0.6 |
| Base Depth | 観察の枠の Camera の depth の基準。主カメラより大きくする | 10 |
| Include Unwarped Culling Volume | 観察のカリングに、通常の視錐台も含める（Scene View の比較表示と同じ設定） | オン |
| Run In Edit Mode | Edit Mode でも観察の枠を登録して表示する | オフ |

距離を尺度 S に比例させるので、S の値によらず同じ見え方になります。

### レイアウト

| `Layout` | 内容 |
| --- | --- |
| `OrbitWipe` | 周回の枠 1 つを画面の隅に小さく置く。時間で変わる異常を探す用途 |
| `FixedGrid` | 固定の 4 枠（Front、Right、Top、Iso）を 2×2 で大きく置く。角度ごとの静止した確認と、画像の比較の用途 |
| `Combined` | 周回の枠を大きく、固定の 4 枠をその横に縦に並べる |

枠は画面の中に収まり、互いに重なりません。

### 枠ごとの設定

各枠の `RadialObserverSlot` で、種類（`Orbit` または `Fixed`）、固定の枠の水平角（Yaw）と仰角（Pitch）、画角（Field Of View、既定 50）を設定します。

## 操作

| 枠 | 動き |
| --- | --- |
| 周回の枠 | 自動で周回する（既定 12°／秒）。枠の内側の右ドラッグで手で回し、ホイールで拡大・縮小する。操作をやめて 3 秒たつと周回に戻る |
| 固定の枠 | 角度は固定 |

近い・遠いクリップの距離は、カメラの距離に合わせて自動で決めます。

## 守ること

- 観察の枠の Camera に `RadialCamera` を付けないでください。付けると、その Camera 自身がワープの中心になり、主カメラの変形後の姿を見られません。付いている Camera は、Console に警告を出して登録しません。Omni Manager の対数描画への切替も、観察の枠の Camera には `RadialCamera` を付けません。
- Rig の下に `Omni.Logical.Transform` を置かないでください。Cinemachine と同じ Transform を書き合うためです。
- 平行投影の Camera は使いません。Radial は平行投影では常に恒等の写像になり、変形後の世界を見られないためです。固定の角度の枠も透視投影で作ります。

### Cinemachine の Channel

観察の枠の Virtual Camera は、枠ごとに専用の Output Channel（Channel01〜05）に置き、枠の Brain をそのチャンネルだけにします。Rig を有効にした時点で Scene にある他の Brain からは、そのチャンネルを外します（無効にすると元に戻します）。これは、主カメラの Brain が観察の Virtual Camera を拾って、観察の位置へ動いてしまうのを防ぐためです。

:::caution
Rig を有効にした後に作られた Brain は対象外です。その場合は、その Brain の Channel Mask から観察のチャンネルを外してください。
:::

## 枠が表示されないとき

観察する `RadialCamera` が無効、破棄された、または見つからない間は、登録を外し、すべての枠の Camera を無効にします（枠が消えます）。

| 状況 | 表示 |
| --- | --- |
| `RadialCamera` が見つからない | Console に警告 |
| `RadialCamera` が無効 | Console に警告。画面の右下に「Radial is OFF - enable it (F8) to show the observer view.」 |

`RadialCameraToggle` で Radial を無効にしている間も、観察する変形後の世界が無いので、枠は消えます（[標準描画と対数描画を切り替える](../../getting-started/switch-rendering/)）。

## Edit Mode で動かす

`Run In Edit Mode` をオンにした Rig だけが、Edit Mode でも登録と枠の表示を行います。オフの Rig は Edit Mode では何も変えません。Edit Mode でオフに戻すと、登録を外して枠を消します。

Edit Mode の更新は、Editor の Player Loop が回るときだけです。毎フレーム動かしたい場合は、利用する側で `EditorApplication.QueuePlayerLoopUpdate` を呼んでください。

:::caution[0.1 での検証の範囲]
Omni 0.1 では、枠の配置、レイアウト、登録、Prefab の構成を検証しています。観察の Camera の GPU の描画の結果（Scene View の客観との一致など）は、まだ検証が済んでいません。
:::

## デモ

CosmicScale のデモでは、O キーで画面の下半分に俯瞰の画面を出します。`RadialObserverRig` と Cinemachine のカメラで、Radial で圧縮した世界を主カメラの上から見下ろします。
