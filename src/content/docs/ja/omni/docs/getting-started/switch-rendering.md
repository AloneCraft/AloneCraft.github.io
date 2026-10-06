---
title: 標準描画と対数描画を切り替える
description: Omni Manager で Scene を対数描画へ切り替え、Play 中に見比べる。
sidebar:
  order: 4
---

## Omni Manager を開く

Omni Manager は、Scene View の Omni の Overlay にある「Omni Manager」のチェックボックスで開きます。Overlay のチェックボックスと Manager のウィンドウの表示は連動します。

## 対数描画へ切り替える

Omni Manager の上部の「標準レンダリング／対数レンダリング」が、通常の切替の入口です。対象は、上部の「対象範囲:」で選びます。

| 対象範囲 | 対象 |
| --- | --- |
| ロード済みScene | 読み込んだすべての Scene |
| プロジェクト全体（書換可能なAssets） | プロジェクトの中の、書き換えられる Asset |

どちらの範囲でも、`RadialCamera` を付けるのは、読み込んだ Scene の Camera です。

対数レンダリングを選ぶと、次の処理を一続きで行います。

1. Camera、Renderer、Terrain、Decal、VFX と Material を解析する
2. 必要な独自の Shader と VFX だけを生成する
3. 生成物のコンパイルと、対数描画の約束を確かめ直す
4. すべての対象の準備ができてから、Camera に `RadialCamera` を追加して有効にし、Material に対数の Shader を適用する
5. Camera、Material、VFX、元に戻すための台帳を確かめ、完了の状態を更新する
6. 必要な Scene だけを最適化のために Bake する

対数を選んだだけでは成功になりません。上部の「現在:」の欄には、処理の間は「処理中…」と出て、確認が終わってから実際の状態が出ます。

| 表示 | 意味 |
| --- | --- |
| 標準 | すべての対象が標準 |
| 対数 | すべての対象の Material が対数で、読み込んだ Scene の対象の Camera の `RadialCamera` がすべて有効 |
| 対数（警告 N件） | 対数。ただし N 個の Material が診断表示になっている（下を参照） |
| 混在／エラー | 標準と対数が混ざっている、または問題がある |

:::caution[Material は共有される]
切替は Material のアセットを書き換える方式です。同じ Material を使う、読み込んでいない Scene や Prefab にも、Shader の変更が及ぶことがあります。「マテリアル」タブの詳細の欄では、読み込んだ Scene の中で使っているオブジェクトとスロットを確かめられます。読み込んでいない Scene や Prefab の使用箇所は、この欄には出ません。
:::

### 止まる場合と、診断表示になる場合

- **適用の前に止まる**: Prefab Stage、復元の台帳の競合、診断用の Shader の欠落、Asset の書き込みの失敗など、安全に元に戻せることを保証できない問題。
- **診断表示で続ける**: 対数の Shader が無い、生成やコンパイルに失敗した、など、元の Material の参照を安全に残せる変換不能。正常な対象は対数にし、変換できないスロットだけを診断用の Material に置き換えます。結果は成功ではない `CompletedWithConversionErrors` になります。上部の「現在:」は「対数（警告 N件）」になり、「マテリアル」タブでは該当の Material に「変換エラー・診断表示中」と出ます。「標準レンダリング」を選ぶと元に戻せます。

### 切替の記録

切替が終わると、選んだ描画方式を `ProjectSettings/Omni/RenderingMode.json` に記録します（`Logarithmic` または `Standard`）。

Camera に付けた `RadialCamera` は Scene の変更なので、保存しないと失われます。そこで、対数が記録されている間は、Omni が次のときに対象の Camera へ `RadialCamera` を自動で付けて有効にします。

- Scene を開いたとき、Domain Reload の後、Play を終えて編集に戻ったとき
- Play を始めたとき、Player をビルドするとき

自動で付けた `RadialCamera` は Undo に記録せず、Scene を未保存の状態にもせず、Scene のファイルにも書き出しません。

`RadialObserverRig` の観察用の Camera と、保存されない隠しの Camera は対象外です。観察用の Camera に `RadialCamera` を付けると、その Camera 自身が写像の中心になり、観察にならないためです。

## Play 中に見比べる

対数描画に切り替えた Scene では、`RadialCamera` を持つ Camera に `Omni/Radial Camera Toggle`（`RadialCameraToggle`）を追加すると、Play 中や Player で、同じ視点のまま標準描画と対数描画を切り替えられます。

| 操作 | 内容 |
| --- | --- |
| 画面のボタン | 画面の左上に出る（Inspector で非表示にできる） |
| キー | 既定は F8（Input System がある場合。Inspector で変更・無効化できる） |
| API | `SetRadialEnabled(bool)`、`Toggle()`、イベント `RadialEnabledChanged` |

切り替えるのは `RadialCamera` の有効と無効だけです。無効のときは、Radial の Material が恒等の写像で描かれ、標準描画と同じ見た目になります。

:::note
`RadialCamera` が有効な間、Radial に対応しない Material を持つ、`Omni.Logical.Transform` の下の Renderer は、安全のために表示されません。標準の Material のままの Scene では、先に Omni Manager で対数描画に切り替えてください。
:::

## Scene View で切り替える

Scene View の Omni の Overlay では、View ごとに標準と Radial を切り替えられます。`RadialCamera` の欄で「標準カメラ」を選ぶとその View は通常の表示に、`RadialCamera` を選ぶとその設定を使った Radial の表示になります。複数の Scene View は、互いの選択を共有しません。
