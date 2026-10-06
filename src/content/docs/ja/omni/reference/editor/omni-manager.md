---
title: Omni Manager
description: 標準描画と対数描画の切替と、Material・Shader・ベイク・Scene BRG の状態を確かめる Editor のウィンドウ。
sidebar:
  order: 2
---

Omni Manager は、標準描画と対数描画を切り替え、その対象の状態を確かめる Editor のウィンドウです。切替の流れは [標準描画と対数描画を切り替える](../../../docs/getting-started/switch-rendering/) を、Material の変わり方と状態の意味は [Shader と Material](../../../docs/guides/shaders-and-materials/) を見てください。

## 開き方

Scene View の Omni の Overlay の先頭にある「Omni Manager」のチェックボックスで開きます。

- チェックボックスとウィンドウの表示は連動します。チェックを外すと、開いているすべての Omni Manager を閉じます。
- Unity のメニューとショートカットには、Omni Manager を開く項目はありません。
- Editor の起動の直後など、Omni Manager の準備ができていない間は、チェックボックスを操作できません。

Overlay については [Scene View の Overlay](../scene-view-overlay/) を見てください。

## ウィンドウの構成

上から順に、次の 3 つが並びます。

| 部分 | 内容 |
| --- | --- |
| 「マテリアル切替」の欄 | 描画方式の切替と、今の状態。どのタブを選んでいても表示する |
| タブの列 | 「マテリアル」「シェーダー」「ベイク最適化」「Scene BRG」 |
| タブの中身 | 選んだタブの表示と操作 |

タブの列の右端にある更新のボタン（ツールチップ「現在のタブを更新」）は、選んでいるタブの一覧を読み直します。Scene の開閉、Hierarchy と Project の変更、Undo／Redo、Play の終了の後は、Omni Manager が「マテリアル」「シェーダー」「ベイク最適化」のタブを自動で読み直します。

表示の言語は、Unity Editor の言語に合わせて日本語か英語になります。

## 「マテリアル切替」の欄

| 行 | 内容 |
| --- | --- |
| 対象範囲: | 「ロード済みScene」か「プロジェクト全体（書換可能なAssets）」を選ぶ。範囲の詳細は [Shader と Material](../../../docs/guides/shaders-and-materials/) の「対象範囲」を見る |
| 描画方式: | 「標準レンダリング」と「対数レンダリング」のラジオボタン。選ぶと切替を始める |
| 現在: | 実際の状態（下の表） |
| 状態: | 対象範囲の Material の数。「標準」「対数」「診断」「未解決」「対象外」の別 |
| 対象: | 「ロード済みScene」では、読み込んだ Scene の数と名前。「プロジェクト全体」では、`Assets` の下の書き換えられる Material の数と、対象外の数 |
| カメラ ・ マテリアル | 読み込んだ Scene の Camera と Material のスロットの状態（下を参照） |
| Scene BRG: | Scene BRG の準備状態。状態の意味は [Scene BRG](../../../docs/guides/scene-brg/) を見る |
| 元を失った生成物: | 直前の対数への切替で、元を失った生成物を掃除した結果。掃除が無ければ出ない |
| RadialCameraを自動で付ける（開いたScene・Play・Build） | 描画方式の記録のチェックボックス（下を参照） |
| 古い切替記録を N件整理しました | 直前の切替で、復元の台帳の古い記録を整理した件数。整理が無ければ出ない |

### 描画方式のラジオボタン

- クリックした後、次の Editor の更新で切替を実行します。
- 次の間は、ラジオボタンを操作できません。Play 中、コンパイル中、Asset の更新中、切替の処理中、対象範囲を変えた後の一覧の作り直しの間。
- 標準と対数が混ざっているときは、どちらのラジオボタンも選ばれていない表示になります。
- 「対数レンダリング」が選ばれた表示になるのは、対象の Material がすべて対数（診断表示を含む）で、かつ読み込んだ Scene の対象の Camera がすべて有効な `RadialCamera` を持つときです。
- 「プロジェクト全体」で「対数レンダリング」を選ぶと、確認のダイアログ「Project Material切替の確認」が出ます。変更対象、生成前の診断候補、読取専用・埋込みの対象外の件数を確かめ、「続行」を押すと始まります。
- 「プロジェクト全体」でも、Material の切替に続けて、読み込んだ Scene の Camera を切り替えます。対数では `RadialCamera` を追加して有効にし、標準では無効にします。

### 「現在:」の表示

| 表示 | 条件 |
| --- | --- |
| 処理中… | 切替を実行している |
| 走査中… | 対象範囲を変えた後、一覧を作り直している |
| 対数 | 対象の Material がすべて対数で、読み込んだ Scene の対象の Camera がすべて有効な `RadialCamera` を持つ |
| 対数（警告 N件） | 「対数」と同じ。ただし N 個の Material が診断表示 |
| 標準 | 対象の Material がすべて標準 |
| 混在／エラー | 上のどれでもない。対象の Material が 1 つも無いときと、Runtime の Material を検出して状態を確定できないときも、この表示になる |

### カメラとマテリアルの行

`カメラ a/b ・ マテリアル c/d ・ VFX …` の形で出ます。

| 部分 | 内容 |
| --- | --- |
| カメラ a/b | 有効な `RadialCamera` を持つ Camera の数／対象の Camera の数 |
| マテリアル c/d | 対数の Shader を実際に使っているスロットの数／対象のスロットの数 |
| (未ワープN件) | 生成物を指しているのに、写像しない Shader を参照しているスロットの数。1 件以上のときだけ出る |
| VFX | VFX の backend と、準備のできた数／必要な数 |
| （無効化カメラ N 台あり） | `RadialCamera` を持つが無効な Camera の数。1 台以上のときだけ出る |

### 警告とエラー

欄の下には、必要なときだけ次の表示が出ます。

| 表示 | 内容 |
| --- | --- |
| Play中は切替できません | Play 中。ツールチップに理由が出る |
| ⚠ 未解決 N 件 | 元の Material を解決できないスロットがある。ツールチップに、最大 10 件の理由が出る |
| ⚠ VFX …: ready a/b | VFX の backend の準備が終わっていない。ツールチップに理由が出る |
| 対数レンダリングへ切り替えました。変換できないMaterialは診断表示です: … | 切替は完了したが、変換できない Material を診断表示にした |
| 描画切替は…で完了しませんでした: … | 切替が完了しなかった。段階（「事前確認」「準備（解析とShader生成）」「適用」「事後確認」）、対象、理由が出る。理由が複数行のときは「詳細」で全文を開ける |
| Scene BRG の準備の失敗 | 段階、元の Material、理由が出る。「Scene BRGタブを開く」で「Scene BRG」タブへ移る |
| Camera MotionVectors と SSAO の警告 | 対数への切替で、URP の共有の resource を Omni 版へつなげなかった理由 |
| GUID manifestの保存は保留中です。… | `ProjectSettings/Omni/Generated.manifest.json` の保存が保留になっている。「manifest保存を再試行」で保存をやり直す |
| 元を失った生成物（警告） | 参照中で残したもの、または削除に失敗したものがある。パスと参照元は Console に出る |

切替の失敗と警告の表示は、Scene を開閉したとき、アクティブな Scene が変わったとき、対象範囲を変えたときに消えます。

### RadialCamera の自動付与

「RadialCameraを自動で付ける（開いたScene・Play・Build）」は、`ProjectSettings/Omni/RenderingMode.json` に記録した描画方式を表します。記録が対数のときにチェックが付きます。

- 切替が完了すると、Omni が自動で記録します。対数への切替でオン、標準への切替でオフになります。
- チェックを付けると、すぐに読み込んだ Scene の対象の Camera へ `RadialCamera` を付けて有効にします。
- チェックを外すと、Omni が自動で付けた `RadialCamera` を外し、自動で有効にした `RadialCamera` を無効に戻します。切替で追加した `RadialCamera`（Scene に保存されるもの）は残ります。それを無効にするのは「標準レンダリング」です。

自動で付ける時期と対象は [標準描画と対数描画を切り替える](../../../docs/getting-started/switch-rendering/) の「切替の記録」を、API は [OmniRenderingModeSettings](../omni-rendering-mode-settings/) を見てください。

## 「マテリアル」タブ

切替の対象の Material の状態を表示するタブです。生成、適用、復元のボタンはありません。これらは上部の切替が行います。

| 部分 | 内容 |
| --- | --- |
| 表示: | 「すべて」「準備が必要」「問題あり」で一覧を絞り込む |
| 検索の欄 | アセットパスか Shader 名に含まれる文字で絞り込む |
| 警告 | Runtime の Material を検出したとき、対数への切替を自動で完了できない Material があるとき、Scene の変更を反映している間（「Scene変更を反映中…」）に出る |
| 切替対象 | 範囲の対象（Scene の数と名前、または Material の数と対象外の数）と、読み込んだ Scene の「Renderer」「マテリアル」「カメラ」「Radial Camera」の数。すべての Camera が `RadialCamera` を持つと「Radial Camera」が緑になる |
| マテリアル一覧 | 列は「Material」「使用中のShader」「現在のShader状態」「使用数」 |
| 詳細 | 選んだ行の「アセットパス」「元シェーダー」「対数Shader」「対数版の準備状態」「生成元」、復元・対象外・問題の説明、読み込んだ Scene の中の使用箇所（「Scene／オブジェクト／スロット」） |

- Material の名前をクリックすると、Project ウィンドウでその Material を示します。
- 行をダブルクリックすると、元の Material と、対応する Omni 側の Material（管理 Material など）を並べて開きます。
- 一覧と詳細の間の境界をドラッグすると、詳細の欄の高さを変えられます。

「使用中のShader」は、分類と Shader 名の 2 行です。

| 分類 | 内容 |
| --- | --- |
| URP・標準、URP・対数 | URP の Shader と、その同梱の対数版 |
| 独自・標準、独自・対数 | 利用者の Shader と、その対数版 |
| BuiltinRP・標準 | BuiltinRP の Shader |
| 変換エラー・診断表示中 | 変換できず、診断用の Material に置き換えた |
| 変換不要・対象外 | 対数描画の対象にしない Shader |
| 参照切れ | Shader の参照が無い |

「現在のShader状態」と「対数版の準備状態」の値の意味は、[Shader と Material](../../../docs/guides/shaders-and-materials/) の「「マテリアル」タブ」を見てください。

## 「シェーダー」タブ

Shader と、その対数版の状態を確かめるタブです。生成のボタンはありません。生成は上部の切替が行います。

| 部分 | 内容 |
| --- | --- |
| 診断情報 | 「Omni Shader Diagnostic」のウィンドウを開く（下を参照） |
| シーン使用中／Assets全体 | 読み込んだ Scene が使う Shader か、`Assets` 全体の Shader かを選ぶ |
| 生成が必要なものだけ表示 | 「未生成」と「要再生成」だけに絞る |
| 検索の欄 | Shader のパスに含まれる文字で絞り込む |
| Camera MotionVectors / SSAO | 折りたたみの欄。URP の共有の resource が Omni 版かどうかを示す（下を参照） |
| ログ | 欄の中のボタンの結果。「クリア」で消す |
| シーン概要 | 「総数」「手書き」「ShaderGraph」と、状態ごとの数（「Generated」「未生成」「要再生成」「不正参照」「スキップ」） |
| シェーダー一覧 | 列は「状態」「元シェーダー」「相対パス」「使用数」 |

- 「状態」の値は「生成済」「対応済」「未生成」「要再生成」「不正」「スキップ」です。意味は [Shader と Material](../../../docs/guides/shaders-and-materials/) の「「シェーダー」タブ」を見てください。
- 「元シェーダー」の名前をクリックすると、Project ウィンドウでその Shader を示します。
- 「相対パス」の「元」と「先」のボタンは、Project ウィンドウで元の Shader と変換後の Shader を示します。
- 行をダブルクリックすると、元の Shader と変換後の Shader を並べて開きます。
- 「不正」の行を選ぶと、理由と「現在のShader」「期待するShader」が出ます。

### Camera MotionVectors / SSAO

見出しには、Camera MotionVectors と SSAO のそれぞれが「Omni版」か「未導入」かが出ます。対数への切替が完了すると、Omni が自動でつなぎます。

| ボタン | 内容 |
| --- | --- |
| Omni Shaderを導入 | URP の共有の resource を、Omni 版の Shader へつなぐ |
| URP Shaderへ復元 | URP の標準の Shader へ戻す |

Camera MotionVectors と SSAO の行に、それぞれこの 2 つのボタンがあります。

### Omni Shader Diagnostic

選んでいる Shader か、選んでいる Material の Shader を解析するウィンドウです。選択を変えると解析し直します。外部への送信はしません。このウィンドウの表示は、Editor の言語によらず同じです。

| ボタン | 内容 |
| --- | --- |
| Refresh | 選んでいる Shader を解析し直す |
| Open Offline Guide | 製品に同梱の `Assets/Omni/Documentation/ShaderAndMaterialWorkflow.md` を開く |
| Export Report | 結果を `Assets/Radial/Diagnostics/<Shader 名>.omni-shader-diagnostic.json` に書き出す |
| Copy Japanese AI Prompt | Shader を直すための日本語の指示文をクリップボードに写す |

## 「ベイク最適化」タブ

ベイクを実行し、アクティブな Scene のベイクの状態を表示するタブです。ベイクは任意の性能の最適化で、無くても標準と対数の描画は動きます。詳しくは [Shader と Material](../../../docs/guides/shaders-and-materials/) の「ベイク最適化」を見てください。

| 部分 | 内容 |
| --- | --- |
| 現在のシーンをベイク | アクティブな Scene をベイクする。有効な `RadialCamera` が無いと失敗する |
| ロード済み全シーンをベイク | 読み込んだすべての Scene をベイクする。未ロードの Scene は対象外 |
| 現在のシーン | アクティブな Scene のベイクの結果。対象の Renderer の数（NearField／MidField／FarField の内訳）、遠景のグループの数、反射プローブの誤差の最大の見積り、ベイクしたときのワープの設定、ベイクのデータのアセット |
| 選択 | Project ウィンドウでベイクのデータを示す |
| ログ | ベイクの結果。「クリア」で消す |

## 「Scene BRG」タブ

Scene BRG の sidecar の準備状態を表示し、再確認と再生成を頼むタブです。状態の意味と sidecar の仕組みは [Scene BRG](../../../docs/guides/scene-brg/) を見てください。

| 部分 | 内容 |
| --- | --- |
| Backend: | 今の backend。「BatchRendererGroup（製品）」か「Legacy（検証用）」 |
| 状態: | 「準備完了」「準備中…」「失敗（Scene BRGの更新を停止中）」「不要（Legacy backend）」「未確認」 |
| 操作 | 2 つのボタン（下の表）。Play 中とコンパイル中は押せない |
| 最終確認 | 直近の確認の番号、時刻、きっかけ、範囲、対象の Material の数、更新した sidecar の数、確認したときの backend、対象の Scene |
| 失敗内容 | 失敗したときだけ出る。「段階」「元Material」「元Shader」「BRG変換元Shader」「理由」「影響するRenderer」。「表示」で元の Material を Project ウィンドウで示す |

| ボタン | 内容 |
| --- | --- |
| ロード済みSceneのsidecarを再確認・再生成 | 読み込んだ Scene の sidecar を確かめ、足りないものと古いものを作り直し、すべての sidecar に元の Material の値を写し直す |
| Build SettingsのSceneを含めて全体生成… | 確認のダイアログの後、Build Settings の有効な Scene と読み込んだ Scene を対象に sidecar を作り、manifest をその集合で作り直す。Build の Scene を 1 つずつ Preview で開くので時間がかかる |

どちらのボタンも、すぐには実行しません。次の安全な Editor の更新で実行します。待っている間は「次の安全なEditor更新で実行します。」と出ます。

「最終確認」のきっかけは「Omni Manager」「Build Settings全体生成」「Play開始」「自動化コマンド」「自動」のどれかです。

## 関連

- [標準描画と対数描画を切り替える](../../../docs/getting-started/switch-rendering/)
- [Shader と Material](../../../docs/guides/shaders-and-materials/)
- [Scene BRG](../../../docs/guides/scene-brg/)
- [Omni が作るファイル](../../files/project-files/)
