---
title: Shader と Material
description: 同梱の標準 URP Shader、Material の適用と復元、Omni Manager のマテリアルとシェーダーのタブ。
sidebar:
  order: 2
---

Omni は、Material のアセットの Shader を Radial 版へ差し替えて対数描画にします。このページは、どの Shader が対象になり、Material がどう変わり、どう元に戻るかを説明します。切替の操作そのものは、[標準描画と対数描画を切り替える](../../getting-started/switch-rendering/)を見てください。

## 同梱の標準 URP Shader

標準の URP Shader の Radial 版は、製品に同梱しています。Unity `6000.6.0f1` と URP `17.6.0` で作ったもので、`Assets/Omni/Radial/Universal Render Pipeline` にあります。変更が必要だった URP の HLSL は `Assets/Omni/Radial/Hlsl` にあります。

利用者の環境では、これらを再生成しません。Import、Editor の起動、Play、Player のビルドでも作り直しません。

Material のカタログには 18 系統を登録しています。手書きの Shader の 17 系統と、URP 既定の Decal の Shader Graph です。

| 系統 | URP の元の Shader |
| --- | --- |
| メッシュ | Lit、ComplexLit、SimpleLit、BakedLit、Unlit |
| Particle | ParticlesLit、ParticlesSimpleLit、ParticlesUnlit |
| SpeedTree | SpeedTree7、SpeedTree7Billboard、SpeedTree8 |
| Terrain | TerrainLit、TerrainLitAdd、TerrainLitBase、TerrainLitBasemapGen |
| Spatial Mapping | SpatialMappingOcclusion、SpatialMappingWireframe |
| Decal | `Decal.shadergraph`（URP 既定の `Decal.mat` が使う Graph） |

- カタログは、元の Shader と Radial 版を、名前ではなく実際の参照で双方向に対応付けます。
- カタログに無い標準 URP Shader を、似た Shader へ推測で置き換えることはありません。対象の Material、Shader、Scene、オブジェクト、スロットと理由を復元の台帳に記録し、診断表示にします。
- URP に同梱の Shader Graph（AutodeskInteractive、SpeedTree8_PBRLit、SpeedTree9_URP など）は、カタログではなく、独自の Graph と同じ変換器で利用者の環境に生成します。詳しくは [Shader Graph と独自の Shader](../shader-graph/) を見てください。

### Decal

Radial 版の Decal の Graph は `Assets/Omni/Radial/Universal Render Pipeline/Shaders/Decal.shadergraph` です。Shader 名は `Omni/Radial/com.unity.render-pipelines.universal/Shaders/Decal` で、URP の元の `Shader Graphs/Decal` とは別の名前です。

| Pass | 扱い |
| --- | --- |
| Decal Projector の Pass | 頂点を写像しない。Scene の深度から復元した位置だけを、Radial で逆に変換する |
| メッシュに描く Pass（DBufferMesh など） | ほかの Graph と同じく、頂点を写像する |

## 変換しない Shader

次の Shader は変換しません。Omni Manager では「変換不要・対象外」と表示します。

| Shader | 理由 |
| --- | --- |
| URP 標準の 2D Shader（Sprite と Mesh2D の Lit／Unlit、Sprite Mask、VFX の Sprite 用の Graph） | 2D は対数描画の対象外。Radial 版を持たない |
| 手書きの Fullscreen／Blit の Shader、Skybox | 物体の頂点の前後関係を持たない |
| Shader 自身が対数描画を実装していて、標準版が無い Shader（`Omni/RadialGlobalLit` など） | 標準へ戻す先も、対数へ切り替える先も無い |

- 2D の Shader は、名前ではなく、パッケージの中の正確なパスで判定します。
- Fullscreen／Blit と Skybox は、全 Pass が Fullscreen に依存していることや Skybox のタグのように、用途を証明できる情報だけで判定します。
- Shader 名に `UI`、`2D`、`Sprite`、`Overlay`、`Screen` が含まれることや、透明の Queue であることだけでは除外しません。World Space の Canvas や TextMeshPro のように 3D 空間に置ける描画は、前後関係を保つ必要があるので、通常どおり変換します。
- Fullscreen の Shader Graph（霧など）は変換します。[Shader Graph と独自の Shader](../shader-graph/) を見てください。

### 2D の表示

2D の Material は、標準のまま残ります。

- RadialCamera が有効な間、`Omni.Logical.Transform` の下にある 2D の Renderer は、安全のため、そのカメラでだけ非表示になります。
- `Omni.Logical.Transform` の下に無い 2D の Renderer は、通常の Unity の Renderer として、実際の距離で描かれます。対数の写像は掛かりません。
- RadialCamera のカメラが 2D Renderer を使う Player のビルドは止まります。

:::tip[遠くに 2D を出す]
RadialCamera を付けない別のカメラで 2D を RenderTexture へ描き、メインのカメラで合成します。

- 2D 用のカメラの `cullingMask` を、2D 専用のレイヤーに絞ります。
- 2D の物体は `Omni.Logical.Transform` の下に置かず、2D 用のカメラの近くに置きます。
- 合成は、Fullscreen の Pass（Radial の変換の対象外）か、画面に固定した四角形で行います。

Camera Stacking は使えません。Base と Overlay の Renderer の種類が違うと、URP が描かないためです。3D との前後関係（深度）は付きません。
:::

## Material の適用

書き込める通常の Material は複製しません。同じ `.mat` と Renderer のスロットの参照を保ったまま、Shader だけを切り替えます。適用では次の値を保ちます。

- Texture と Scale／Offset、色、数値
- Surface、Alpha Clip、Blend、Cull、ZWrite
- Local Keyword、Pass、Instancing、GI
- Render Queue（既定と明示の両方）

書き換えられない Material には、管理用の写し（管理 Material）を作ります。

| Material | 扱い |
| --- | --- |
| Unity の組み込みの Material、Package の中の Material | `Assets/Radial` に写しを作り、Scene などの参照を写しへ差し替える |
| Material Variant（Shader を親から受け継ぐ） | 親を外した独立の写しを作り、参照を差し替える |

同じ適用をもう一度行っても、何も変わりません。変換できない同じ Material が複数のスロットで使われている場合は、1 つの診断用の Material を共有し、台帳に元の Material との対応を記録します。

### 対象範囲

Omni Manager の上部の「対象範囲:」で、範囲を切り替えます。

| 範囲 | 対象 |
| --- | --- |
| ロード済みScene | 読み込んだすべての Scene の Material。非アクティブのオブジェクトを含む。Preview Scene、Prefab Stage、Editor 専用の一時オブジェクトは含まない |
| プロジェクト全体（書換可能なAssets） | `Assets/` の下の、独立した書き換えられる `.mat` だけ |

- プロジェクト全体の範囲では、`Packages/` の Material、モデルに埋め込まれた Material、読み取り専用の Material を、理由付きの「読取専用・埋込みの対象外」として別に数えます。
- `Assets/Radial` は Omni の生成物なので、どちらの範囲でも、切替の対象にも件数にも入れません。
- プロジェクト全体の範囲で対数にすると、開いていなかった Scene も、開けば対数で表示されます。ロード済み Scene の範囲で切り替えた場合、ほかの Scene だけが使う Material は標準のままです。

## Material の復元

上部で「標準レンダリング」を選ぶと、台帳の記録から元に戻します。

- Omni が変えた値は、今の値が適用した直後と同じ場合だけ、元に戻します。
- 適用した後に利用者が変えた共通のプロパティは残します。元の Shader でも意味を保てる Keyword、Pass、Queue の設定も残します。
- 同梱の Radial Shader を使う Material は、カタログの逆の対応で、同じ Material を標準の Shader へ戻します。
- 診断用の Material に置き換えたスロットは、元の Material の参照へ戻します。
- プロジェクト全体の範囲の「標準レンダリング」は、すべての参照を元の Material へ戻し、写しと台帳の記録も消します。ロード済み Scene の範囲では、ほかの Scene が写しを参照している可能性があるので、写しを残します。

元の Shader が消えたなど、意味を保てない状態では、変更する前に止まります。対数描画に対応していても、標準の Shader との対応が決まらない独自の Shader は、詳細の欄に「標準へ復元不可」と理由が出ます。

適用から事後の確認までを、1 つの変更として扱います。予期しない例外が起きたときは、変更した Camera、Material、診断の参照、台帳を、同じ Undo のグループで元に戻します。変換できないだけの場合（診断表示）は、使用箇所を見せるために自動では戻しません。「標準レンダリング」で明示的に戻します。Domain Reload、Editor の再起動、Scene の開き直しの後も、台帳から診断の状態を見つけ直します。

### 復元の台帳と自動の整理

復元の台帳は `Assets/Radial/Mappings/MaterialShaderMappings.asset` です。

台帳は、Unity の外での変更（フォルダーの上書き、git checkout、`Assets/Radial` の削除）を知りません。そのため、標準と対数のどちらへ切り替えるときも、解析の前に台帳と今の Material を突き合わせて整理します。

| 見つかった状態 | 整理の内容 |
| --- | --- |
| Material が元の Shader に戻っている | 記録だけを消し、通常どおり変換し直す |
| 対数の Shader が消えた（欠落、またはエラーの Shader） | 元の Shader と状態へ戻して記録を消し、切替で作り直す |
| 別の対数の Shader へ替わっている | 元の情報を残したまま、記録の対数の Shader を付け替える |
| それ以外の Shader へ替わっている | 利用者の Shader は変えずに記録を消し、今の Shader を新しい元として変換し直す |
| Material が消えている | 記録を消す |
| 管理 Material が欠けている、または食い違う | 参照を元の Material へ戻し、管理 Material と記録を消す（次の適用で作り直す） |

- 整理した件数は Manager の上部に、内容は Console に出ます。
- 途中で終わった適用は、起動時の復旧に任せ、整理しません。
- マテリアルの一覧は台帳を書き換えません。食い違う記録は整理後と同じ分類で表示し、詳細の欄に「古い切替記録あり」と出します。

## 「マテリアル」タブ

診断の表示だけのタブです。生成、適用、復元のボタンはありません。これらは上部の切替にまとめています。

- Scene の開閉、Hierarchy の変更、Material と Shader の変更、Undo／Redo、Play の終了の後、一覧を自動で更新します。
- 一覧は、読み込んだすべての Scene（非アクティブのオブジェクトを含む）で使う Material の和です。Scene を切り替えている間は、直前の一覧と「Scene変更を反映中…」を出します。
- 1 つの Material の参照につき 1 行です。同じ名前でも、別の Material は別の行です。
- VFX Graph が持つ、Asset ではない実行時の Material は、一覧に入れません。VFX の準備状態で別に確かめます。

| 列 | 内容 |
| --- | --- |
| Material | Material |
| 使用中のShader | 今 Material に割り当てられている Shader |
| 現在のShader状態 | 「標準」「対数」「変換エラー・診断表示中」「変換不要・対象外」「参照切れ」など |
| 使用数 | 使われている数 |

行を選ぶと、詳細の欄に、アセットパス、元の Shader と対数の Shader、対数版の準備状態、生成元、復元できるか、使用している Scene・オブジェクト・スロットが出ます。

### 対数版の準備状態

| 分類 | 準備状態 |
| --- | --- |
| 独自・標準 | 「未生成」「生成済み・未使用」 |
| 独自・対数 | 「使用中」 |
| URP・標準 | 「同梱済み・未使用」 |
| URP・対数 | 「使用中（同梱）」 |
| BuiltinRP・標準 | 完全に一致する対数の Shader がある場合だけ切り替えられる |
| 診断 | 「変換エラー・診断表示中」 |
| 変換不要・対象外 | 物体の空間を持たないと確かめた Fullscreen／Blit、または Skybox |

ほかに「更新が必要」「使用中・更新が必要」「生成中…」「生成失敗」「変換非対応」「手編集あり・確認が必要」「参照切れ」も出ます。「使用中」は、Material に対数の Shader が割り当てられているという意味です。

「表示:」で、「すべて」「準備が必要」「問題あり」に絞り込めます。

### 上部の状態との関係

上部の「標準レンダリング／対数レンダリング」は、選んだ範囲の Material の Shader の構成だけを表します。Camera、VFX、Renderer の準備状態は、切替のときの検証と警告で扱います。

- 「対数」になるのは、標準と未解決が 0 件で、残りがすべて対数か診断表示のときです。
- 診断表示が 1 件以上あるときは「対数（警告 N件）」と表示します。
- 切替の後も標準が残る場合は、完了にしません。失敗した段階（事前の確認、準備、適用、事後の確認）、対象、理由を上部に出します。
- 診断用の Shader は、Radial の座標変換を実装していても、対応済みには数えません。

## 「シェーダー」タブ

ロード済みの Scene（「シーン使用中」）か `Assets` 全体（「Assets全体」）が使う Shader と、その対数版の状態を確かめるタブです。生成は上部の切替が行うので、生成のボタンはありません。

| 状態 | 意味 |
| --- | --- |
| 生成済 | 生成済み、または同梱のカタログにある |
| 対応済 | Shader 自身が Radial の変換を実装していて、生成が要らない（例: `StencilOnly.shader`）。判定は切替と同じく、`_RadialGlobal_UseRadialTransform` の宣言で行う |
| 未生成 | まだ生成していない |
| 要再生成 | 生成物が、今の生成器の出力と食い違う |
| 不正 | 同梱の対数 Shader と同じ名前で、別の GUID の Shader を参照している |

「生成が必要なものだけ表示」は、「未生成」と「要再生成」だけに絞ります。

### Camera MotionVectors と SSAO

折りたたみの欄「Camera MotionVectors / SSAO」は、URP の共有の resource（画面全体の Camera MotionVectors の Shader と、SSAO の Shader）が Omni 版かどうかを示します。

- Omni 版は、対数の深度を論理の位置へ戻してから計算します。Omni 版でないと、RadialCamera を含む Scene の Player のビルドは止まります。
- 対数への切替が完了すると、自動でつなぎます。Omni 版は RadialCamera の無い Camera では標準と同じ計算をするので、標準へ戻しても外しません。
- URP の標準以外の値が設定されている場合は、上書きしません。理由を Manager の上部に警告として出します。
- 欄の中の「Omni Shaderを導入」と「URP Shaderへ復元」で、手動でつなぐことも、URP の標準へ戻すこともできます。結果はタブの中のログに出ます。

## BuiltinRP の Material と自動対応の限界

`GUI/Text Shader` や `Legacy Shaders/Diffuse` は、独自の Shader ではなく BuiltinRP に分類します。BuiltinRP の分類は、対応済みという意味ではありません。完全に一致する対数の Shader が無ければ、変換できません。

- `Library/unity default resources` の Material は、独自の Shader の生成器へ渡しません。
- 実際の Scene の保存されるオブジェクトが使う組み込みの Material は、元の Shader から厳密な対数の Shader を決められる場合だけ、管理 Material にできます。
- 厳密な対応が無い場合は、Material 名、Shader、使用しているオブジェクト、スロット、理由を示し、使用箇所を `Omni/Radial/Diagnostics/UnsupportedConversion` へ置き換えます。
- Editor 専用の一時的な描画は対象外です。

診断用の Shader は、黄色と黒の斜線と赤い X を、両面、不透明、深度の書き込みありで描きます。Unity の Shader の欠落を示すマゼンタとは区別できます。

次は、通常の自動の切替の範囲外です。

- 読み取り専用の Package の Material、モデルに埋め込まれた Material（プロジェクト全体の範囲では、理由付きの対象外）
- 対応していない Renderer
- 変換できない Shader、厳密な対応が無い組み込みの Material
- 生成物の手編集との競合
- Shader と Material の参照切れ
- Play 中の切替

Omni はこれらを黙って除外したり、似た Shader へ置き換えたりして、成功とは扱いません。元に戻せる変換不能は診断表示に進み、元に戻せない項目は変更の前に止まります。ビルドの対象の Scene に診断表示のスロットか、元に戻していない診断の台帳が残る場合は、元の Shader 名と理由を示してビルドを止めます（[Fail Closed](../../concepts/fail-closed/)）。

:::tip[変換できない Shader の代わりを登録する]
固定機能や Built-in 専用の Shader は、代わりの URP Shader を登録すると変換できる場合があります。[Shader Graph と独自の Shader](../shader-graph/) の「代わりの Shader を登録する」を見てください。
:::

## ベイク最適化

対数版の URP Shader は同梱しているので、ベイクしなくても描画は成り立ちます。ベイクは、必要なキャッシュや生成物を前もって用意する、性能の最適化です。

- 対数への切替は、描画を確かめた後に、必要な Scene だけをベイクします。
- ベイクに失敗しても、描画の切替は失敗に戻しません。結果は、対数への切替は完了、最適化は未完了として報告します。
- 「ベイク最適化」タブの「現在のシーンをベイク」と「ロード済み全シーンをベイク」で、手動でもベイクできます。未ロードの Scene は対象外です。
- ベイクには、有効な RadialCamera が必要です。

Scene BRG の sidecar の準備状態は、「Scene BRG」タブで確かめます。[Scene BRG](../scene-brg/) を見てください。

## 確かめる

`Assets/Omni/Verification/Functions/Shaders` に、Shader を 1 つずつ確かめる Scene があります。一覧は `Functions/FunctionsVerification.unity` から開けます。

| Scene | 内容 |
| --- | --- |
| `Functions/Shaders/Modules` | 同梱のカタログの手書きの Shader 17 種を、1 Shader ずつ分けた Scene。左が URP の元の Shader、中央が Radial の恒等、右が同梱の Omni の Shader（Radial の対数）。変換できない Shader の負の例もある |
| `Functions/Decal.unity` | カタログの Decal の Graph。投影の模様と床の模様の位置が変わらないことを確かめる |
| `Functions/Shaders/Graphs` | URP に同梱の Shader Graph を Radial にしたもの。Radial 版は開いたときに利用者の環境で生成する |

Particle 系は、Play Mode で Particle System も確かめてください。Scene を保存した後に、Material の Shader の参照が変わっていないことも確かめます。

:::caution[大きな三角形]
1 つの三角形が、写像が大きく曲がる範囲（論理の距離で約 1/*M* より先）までまたがると、頂点ごとの写像では被覆と深度が崩れ、近くの物体を覆うことがあります。たとえば、Unity 標準の Plane を 50 倍した床（三角形 1 つが約 50 m）です。こうした面は分割して作ってください。*M* については [Radial の写像と View Scale](../../concepts/radial-mapping/) を見てください。
:::

## Omni の更新と削除

### 元を失った生成物の掃除

元の Shader や Material を削除・移動すると、`Assets/Radial` には対応する生成物だけが残ります。対数への切替が完了するたびに、Omni は次の生成物を OS のごみ箱へ移し、対応する記録を消します。確認のダイアログは出しません。

- 対象: 元が消えたと確認でき、どこからも参照されていない、生成した Shader、生成した Material、BRG の sidecar、管理 Material
- 残すもの: まだ Scene や Prefab から参照されているもの、元の有無を確定できないもの。件数と理由を Console と Manager の上部に出す
- 触れないもの: Bake のデータ、`Resources`、Material の対応表など

### 更新と削除の前に

- Omni を更新する前に、`Assets/Radial` と `ProjectSettings/Omni/Generated.manifest.json` を、バージョン管理かバックアップに入れてください。
- 製品の `Radial/Universal Render Pipeline` と `Radial/Hlsl` を、利用者の環境で作り直す必要はありません。
- Omni を削除する前に、Manager の上部で「標準レンダリング」へ戻し、対象の Material が標準の Shader へ戻ったことを確かめてください。
