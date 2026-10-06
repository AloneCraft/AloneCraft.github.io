---
title: ショートカットとメニュー
description: Omni が加えるショートカット、GameObject のメニュー、Add Component のパス、Create のメニューの一覧。
sidebar:
  order: 4
---

Omni が Unity Editor に加えるショートカットとメニューの一覧です。メニューのパスとショートカットの ID は、Editor の言語にかかわらず英語のままです。

## ショートカット

Edit > Shortcuts（Shortcut Manager）に出る項目です。キーは Shortcut Manager で変えられます。

| ID | 既定のキー | 文脈 | 動き |
| --- | --- | --- | --- |
| `Omni/Frame Logical Selection` | F | Scene View | 選択した対象を Logical 座標でフレーミングする |
| `Omni/Editing/OrbitLeft` | なし | Scene View | 注視点と距離を保って、視点を左へ 15° 回す |
| `Omni/Editing/OrbitRight` | なし | Scene View | 同じく右へ 15° 回す |
| `Omni/Editing/OrbitUp` | なし | Scene View | 同じく上へ 15° 回す |
| `Omni/Editing/OrbitDown` | なし | Scene View | 同じく下へ 15° 回す |
| `Omni/Editing/Closer` | なし | Scene View | 注視点までの距離を 5/6 倍にする。平行投影では表示の範囲を 1/1.2 倍にする |
| `Omni/Editing/Further` | なし | Scene View | 注視点までの距離を 6/5 倍にする。平行投影では表示の範囲を 1.2 倍にする |
| `Omni/Editing/Restore` | なし | Scene View | F でフォーカスする前の視点へ 1 つ戻る |

`Omni/Editing/*` は、Overlay の「操作・表示設定」の「視点編集」のボタンと同じ操作です。詳しくは [Scene View の Overlay](../scene-view-overlay/#視点編集) を読んでください。

### 働く条件

どのショートカットも、次の間は働きません。

- Scene View にキーボードのフォーカスが無いとき
- 文字を入力している欄があるとき
- マウスのドラッグの途中のとき
- Unity の標準の飛行の操作の途中のとき

`Omni/Frame Logical Selection` は、標準の表示、客観、主観のどれでも働きます。Scene View の文脈のショートカットなので、同じ F を使う Unity の全体のショートカット（Frame Selected in Window under Cursor）より優先されます。Hierarchy での F、Edit > Frame Selected、Hierarchy のダブルクリックも、同じ Logical のフレーミングになります。

`Omni/Editing/*` には、さらに次の条件があります。

| ID | 条件 |
| --- | --- |
| すべて | 視点が「主観」の Radial の表示のとき |
| `OrbitLeft`、`OrbitRight`、`OrbitUp`、`OrbitDown`、`Restore` | 2D モードでも、回転のロック中でもないとき |
| `Restore` | フォーカスの履歴があるとき |

`Omni/Editing/*` に初期のキーを割り当てていないのは、Unity の既存のキーを優先するためです。割り当てるときは、既存のキーと重ならない組み合わせを選んでください。

## GameObject のメニュー

| パス | 動き |
| --- | --- |
| `GameObject/Omni/Radial Observer Camera` | 観察カメラの Rig の Prefab（`Assets/Omni/Runtime/Observer/Prefabs/RadialObserverRig.prefab`）を Scene に置く |

- Cinemachine が入っていないときは、Package Manager から `com.unity.cinemachine` を入れるかを確かめるダイアログが出ます。入れた後、Script の再コンパイルが終わってから、もう一度同じメニューを選びます。
- メニューを開いた対象の GameObject があるときは、その子に置きます。無いときは、アクティブな Scene に置きます。そのとき Scene に `SceneMetaData` という名前の Component があり、その `Root` が設定されていれば、`Root` の子に置きます。
- 置いた Rig は Undo で取り消せます。置いた後は Rig を選択します。

使い方は [Radial 観察カメラ](../../../docs/guides/observer-camera/) を読んでください。

## ウィンドウを開く

Omni は Window メニューに利用者向けの項目を加えません。Omni Manager のウィンドウは、Scene View の Overlay の「Omni Manager」のチェックで開きます（[Scene View の Overlay](../scene-view-overlay/#omni-manager)）。

Inspector の Component の右クリックメニュー（`CONTEXT/...`）に、Omni が加える項目はありません。

## Add Component のメニュー

Add Component の `Omni` の下に、次の Component が出ます。

| パス | Component | 必要なもの |
| --- | --- | --- |
| `Omni/Logical Transform` | [`Omni.Logical.Transform`](../../components/logical-transform/) | — |
| `Omni/Radial Camera Toggle` | [`RadialCameraToggle`](../../components/radial-camera-toggle/) | — |
| `Omni/Radial Observer Rig` | [`RadialObserverRig`](../../observer/radial-observer-rig/) | — |
| `Omni/Radial Observer Slot` | [`RadialObserverSlot`](../../observer/radial-observer-slot/) | — |
| `Omni/Radial Observer Orbit` | [`RadialObserverOrbit`](../../observer/radial-observer-orbit/) | Cinemachine 3.0.0 以上、Input System 1.0.0 以上 |
| `Omni/Radial Observer Channel Guard` | [`RadialObserverChannelGuard`](../../observer/radial-observer-channel-guard/) | Cinemachine 3.0.0 以上、Input System 1.0.0 以上 |

`RadialObserverOrbit` と `RadialObserverChannelGuard` は、Cinemachine と Input System の両方が入っているときだけコンパイルされます。どちらかが無いときは、メニューに出ません。

次の Component は `Omni` の下に出ません。Add Component の検索欄に型の名前を入れて追加します。

| Component | 分類 |
| --- | --- |
| [`RadialCamera`](../../components/radial-camera/) | コンポーネント |
| [`RadialIdentityWarp`](../../components/radial-identity-warp/) | コンポーネント |
| [`RadialRigidAnchor`](../../components/radial-rigid-anchor/) | コンポーネント |
| [`RadialSceneBatchRendererExclusion`](../../components/radial-scene-batch-renderer-exclusion/) | コンポーネント |
| [`OmniLightingSpace`](../../lighting/omni-lighting-space/) | 照明 |
| [`OmniLightProbeSpace`](../../lighting/omni-light-probe-space/) | 照明 |
| [`OmniLightProbeSpaceRequirement`](../../lighting/omni-light-probe-space-requirement/) | 照明 |
| [`OmniAdaptiveProbeVolumeSpace`](../../lighting/omni-adaptive-probe-volume-space/) | 照明 |
| [`OmniAdaptiveProbeVolumeSpaceRequirement`](../../lighting/omni-adaptive-probe-volume-space-requirement/) | 照明 |
| [`BakePlaybackController`](../../sequencer/bake-playback-controller/) | Sequencer |

## Create のメニュー

Project ウィンドウの Create（Assets > Create）の `Omni` の下に、次のアセットが出ます。

| パス | アセット | 既定のファイル名 |
| --- | --- | --- |
| `Omni/Celestial/Catalog` | [`CelestialCatalogAsset`](../../celestial/celestial-catalog-asset/) | `CelestialCatalog` |
| `Omni/Radial Shader Substitutes` | `OmniRadialShaderSubstitutes` | `OmniRadialShaderSubstitutes` |

`OmniRadialShaderSubstitutes` は、Omni が変換できない Shader の代わりに使う URP の Shader を登録する表です。使い方は [Shader Graph と独自の Shader](../../../docs/guides/shader-graph/) を読んでください。
