---
title: Omni が作るファイル
description: Omni が利用者のプロジェクトに作るファイル、読むファイル、書き換えるファイルと、バージョン管理での扱い。
sidebar:
  order: 1
---

Omni の本体は `Assets/Omni` にあります。このページは、Omni が本体の外に作るファイルと、書き換えるプロジェクトのファイルの一覧です。導入の手順は [導入](../../../docs/getting-started/installation/) を見てください。

## 一覧

| 場所 | 内容 | バージョン管理 |
| --- | --- | --- |
| `Assets/Radial/` | 変換した Shader、管理 Material、復元の台帳、ベイク、Scene BRG の sidecar などの生成物 | 入れる |
| `ProjectSettings/Omni/Generated.manifest.json` | 生成物の GUID と元の Asset の対応 | 入れる |
| `ProjectSettings/Omni/RenderingMode.json` | 選んだ描画方式（標準か対数か） | 入れる |
| `ProjectSettings/OmniEditorSettings.asset` | Omni の Scene View の設定 | 共有するなら入れる |
| `Logs/Omni…` | Verification の Scene の結果と撮影した画像 | 入れない |
| `Library/OmniRadialVfxPatch.hash` | VFX の確認済みの印 | 入れない |

`Assets/Radial` と `ProjectSettings/Omni` は、Omni の本体とは別の場所です。Omni を更新しても、上書きしません。Omni を更新する前に、この 2 つをバージョン管理かバックアップに入れてください。

## Assets/Radial

Omni が利用者のプロジェクトに作る生成物のルートです。`Assets/Radial` の下の Material は、標準と対数の切替の対象にも、Omni Manager の件数にも入りません。

| フォルダー・ファイル | 作るもの | 内容 |
| --- | --- | --- |
| `User/` | 対数への切替（Shader の生成） | 変換した独自の Shader と Shader Graph。元の `Assets/` か `Packages/` の下のパスを保つ |
| `Hlsl/` | 対数への切替（Shader の生成） | 変換が必要だった独自の include（HLSL）。元の論理パスを保つ |
| `Project/`、`Packages/` | Scene BRG の準備など | 元の Material から作った対数版の Material。元が `Assets/` なら `Project/`、`Packages/` なら `Packages/` の下に、元のパスを保って置く |
| `ManagedBuiltInMaterials/` | 切替（適用） | 管理 Material。書き換えられない Material（Unity の組み込み、Package の中、Material Variant）の写し。名前は `BuiltIn_<hash>.mat` |
| `DiagnosticMaterials/` | 切替（適用） | 変換できない Material の代わりに使う診断用の Material。名前は `Unsupported_<hash>.mat` |
| `Mappings/MaterialShaderMappings.asset` | 切替（適用と復元） | 復元の台帳。Material の元の Shader と値、置き換えた参照の記録 |
| `Bake/<Scene 名>.RadialBake.asset` | ベイク | Scene ごとのベイクのデータ |
| `Resources/RadialBakeRegistry.asset` | ベイク | Scene の GUID とベイクのデータの対応。実行時に `Resources` から読む |
| `BRG/Materials/`、`BRG/Shaders/` | Scene BRG の準備 | 元の Material の GUID ごとの sidecar の Material と Shader（`<GUID>_BRG.mat`、`<GUID>_BRG.shader`） |
| `BRG/Hlsl/` | Scene BRG の準備 | Terrain 系の Shader の入力を BRG 用に変換した HLSL |
| `BRG/Editor/RadialBrgMaterialManifest.asset` | Scene BRG の準備 | 元の Material と sidecar の対応 |
| `Diagnostics/` | 「Omni Shader Diagnostic」の「Export Report」 | Shader の診断の結果（`<Shader 名>.omni-shader-diagnostic.json`）。必要なときだけ作る |

- 対数への切替が完了するたびに、元が消えたと確かめられ、どこからも参照されていない生成物を、OS のごみ箱へ移します。バージョン管理では削除として見えます。対象と、残すものは [Shader と Material](../../../docs/guides/shaders-and-materials/) の「元を失った生成物の掃除」を見てください。
- 「標準レンダリング」へ戻すと、参照を元の Material へ戻し、診断用の Material を削除します。「プロジェクト全体」の範囲では、管理 Material も削除します。「ロード済みScene」の範囲では、ほかの Scene が参照している可能性があるので、管理 Material を残します。
- 古い版が使った `Assets/Radial/Generated.manifest.json` は、初めての移行のときだけ読みます。Omni はこのファイルを書き換えず、削除もしません。

### バージョン管理

`Assets/Radial` は、`.meta` を含めて丸ごとバージョン管理に入れてください。

- Scene や Prefab は管理 Material と診断用の Material を、Scene はベイクのデータを、GUID で参照します。これらが無いと参照が切れます。
- 復元の台帳（`Mappings/`）が無いと、「標準レンダリング」で元に戻せません。
- 変換した Shader と sidecar は作り直せますが、作り直したときの GUID は `Generated.manifest.json` に頼ります。台帳や Scene と同じ時点の生成物をそろえて持つほうが安全です。
- `Diagnostics/` の診断の結果は、入れなくてもかまいません。

## ProjectSettings/Omni/Generated.manifest.json

`Assets/Radial` の生成物のパスと GUID、元の Asset の対応を記録するファイルです。Unity の Asset ではなく、Unity の Import の対象になりません。

| 項目 | 内容 |
| --- | --- |
| 書くもの | 生成物を作る処理（Shader と Material の生成、ベイク、Scene BRG の準備など） |
| 中身 | JSON。`version` と、生成物ごとの `path`、`guid`、元の Asset の `sourceGuid`、`sourceLocalId`、`sourceName` |
| 使い方 | 生成物を作り直すとき、記録した GUID の `.meta` を先に書き、同じ GUID を復元する |
| バージョン管理 | 入れる。`Assets/Radial` を消しても、このファイルがあれば同じ GUID で作り直せる |

保存は、同じフォルダーの一時ファイルを書き終えてから置き換えます。保存に失敗したときは、Omni Manager の上部に保存の保留と、やり直しのボタンが出ます（[Omni Manager](../../editor/omni-manager/)）。新しい生成物の記録を保存できない場合、その変換は成功になりません。

## ProjectSettings/Omni/RenderingMode.json

標準と対数の切替で選んだ描画方式を記録するファイルです。

| 項目 | 内容 |
| --- | --- |
| 書くもの | 切替の完了。Omni Manager の「RadialCameraを自動で付ける（開いたScene・Play・Build）」のチェックボックス |
| 中身 | JSON。`{ "mode": "Logarithmic" }` か `{ "mode": "Standard" }` |
| 使い方 | `Logarithmic` の間、Scene を開いたとき、Play を始めたとき、Player をビルドするときに、対象の Camera へ `RadialCamera` を自動で付ける |
| ファイルが無いとき | 何もしない（未設定） |
| バージョン管理 | 入れる。無いと、ほかの環境では `RadialCamera` が自動で付かない |

自動で付ける時期と対象は [標準描画と対数描画を切り替える](../../../docs/getting-started/switch-rendering/) の「切替の記録」を、API は [OmniRenderingModeSettings](../../editor/omni-rendering-mode-settings/) を見てください。

## ProjectSettings/OmniEditorSettings.asset

プロジェクトで共通の、Omni の Scene View の設定です。設定を変えたときに Omni が保存します。

| 項目 | 内容 |
| --- | --- |
| 書くもの | Scene View の Omni の Overlay の「操作・表示設定」の「独自ナビゲーション」の欄と、「RMB回転基準」の欄 |
| 中身 | 飛行の速度、近づく操作、ホイールの感度、右ボタンの見回しの基準と反転などの設定と、Omni が使う Scene View の既定値 |
| バージョン管理 | チームで同じ設定を使うなら入れる。移動の速度など、個人の好みの値も入る |

Overlay の設定は [Scene View の Overlay](../../editor/scene-view-overlay/) を見てください。

## Logs

Verification の Scene の結果は、プロジェクトの `Logs` の下に保存します。`Logs` は Unity のログのフォルダーなので、バージョン管理には入れません。

| フォルダー | 作るもの |
| --- | --- |
| `Logs/OmniRenderingRegression/<Run ID>/` | Verification Hub の「Capture」。結果の JSON（`rendering-regression.json`）、JUnit 互換の XML（`rendering-regression.xml`）、画像 |
| `Logs/OmniVfxModules/<Run ID>/` | VFX の Module の Scene の撮影と採点 |
| `Logs/OmniShaderGraphModules/<Run ID>/`、`Logs/OmniShaderGraphSamples/<Run ID>/` | Shader Graph の検証の Scene の撮影と採点 |

Player で Verification Hub を動かした場合は、`Application.persistentDataPath/OmniRenderingRegression` に保存します。使い方は [Verification Hub で確かめる](../../../docs/getting-started/verification/) を見てください。

## Library

`Library/OmniRadialVfxPatch.hash` は、プロジェクトの VFX Graph の Asset に Omni の今の VFX のパッチが行き渡ったことを示す印です。この印が無いとき、または Omni の VFX のパッチが変わったときは、Editor が落ち着いた後に VFX Graph の Asset を確かめ、パッチの無いものを取り込み直します。`Library` は Unity のキャッシュなので、バージョン管理には入れません。

## Omni が書き換えるプロジェクトのファイル

Omni は、生成物のほかに、次のプロジェクトのファイルを書き換えます。どれも通常どおりバージョン管理に入れます。

| ファイル | 書き換える処理 | 内容 |
| --- | --- | --- |
| `Assets/` の Material（`.mat`） | 標準と対数の切替 | Material の Shader を、同じ `.mat` のまま対数版へ、または元へ替える |
| Scene と、Material を参照するほかの Asset（Prefab など） | 標準と対数の切替 | 書き換えられない Material の参照を管理 Material へ、変換できない Material の参照を診断用の Material へ差し替える。「ロード済みScene」では読み込んだ Scene の参照を、「プロジェクト全体」では Material を参照する Asset のファイルの参照を差し替える。「標準レンダリング」で元に戻す |
| Scene | 標準と対数の切替 | 対象の Camera へ `RadialCamera` を追加し、有効にする（標準では無効にする）。Undo に記録し、保存すると Scene に残る |
| Scene | ベイク | `RadialBakeBinding` の GameObject を作り、ベイクのデータを参照させる |
| URP の Global Settings のアセット | 対数への切替、「シェーダー」タブの「Omni Shaderを導入」「URP Shaderへ復元」 | Camera MotionVectors と SSAO の共有の resource の Shader の参照 |

`RenderingMode.json` が対数のときに Omni が自動で付ける `RadialCamera` は、Scene のファイルに書き出しません（[標準描画と対数描画を切り替える](../../../docs/getting-started/switch-rendering/)）。

## Omni を削除する前に

Omni Manager の上部で「標準レンダリング」へ戻し、対象の Material が標準の Shader へ戻ったことを確かめてください。詳しくは [Shader と Material](../../../docs/guides/shaders-and-materials/) の「Omni の更新と削除」を見てください。
