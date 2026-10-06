---
title: Omni リファレンス
description: Omni の公開 API、Editor の操作、Shader の契約、ファイルの一覧。
---

Omni の公開 API の一覧です。型とメンバーの一覧（シグネチャ、属性、既定値）は、Omni のソースから自動で生成しています。説明は手書きです。

使い方の流れは [ガイド](../docs/guides/scene-view/) を、仕組みは [概念](../docs/concepts/logical-coordinates/) を読んでください。

## 分類

| 分類 | 内容 |
| --- | --- |
| コンポーネント | `RadialCamera`、`RadialCameraToggle`、Logical Transform など、GameObject に付ける Component と、その関連の型 |
| 照明 | 焼いた空間と表示の空間を対応付ける Component（Light Probe、APV、Lightmap、Reflection Probe） |
| 観察カメラ | 変形後の世界を外から見る `RadialObserverRig` と、その部品 |
| Logical の型 | 192 bit の固定小数点の `Scalar`、位置 `Position3`、回転 `Quaternion` などの数値の型 |
| 単位の型 | 角度、時間、角速度、質量、エネルギーなど、単位を持つ固定小数点の型 |
| Sequencer | Logical の姿勢を焼いて Timeline で再生する `PoseBake` と、その再生の仕組み |
| 天体カタログ | 天体の大きさ・質量・軌道を持つカタログ |
| Editor | 描画方式の記録（`OmniRenderingModeSettings`）、Omni Manager、Overlay、ショートカット、メニュー |
| Shader | include ファイル、Shader Graph の Custom Function、グローバルのプロパティと define |
| ファイル | Omni が作るファイルと、プロジェクトの設定のファイル |

## 載せている範囲

- リファレンスに載せるのは、利用者が使うことを想定した型だけです。`public` でも、Omni の内部で使う型は載せません。
- メンバーは、`public` のものと、Inspector に出る保存フィールド（`[SerializeField]` を付けたもの）を載せます。
- Omni 0.1 の公開 API は、今後の版で変わることがあります。

## 名前の注意

`Omni.Logical` 名前空間には、Unity と同じ名前の型（`Transform`、`Vector3`、`Quaternion`）があります。両方を使うファイルでは、`using L = Omni.Logical;` のように別名を付けると区別できます。
