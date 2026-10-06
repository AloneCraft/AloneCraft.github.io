---
title: Shader Graph と独自の Shader
description: 独自の ShaderLab／HLSL と Shader Graph を Radial へ変換する仕組み、Custom Function の入口、深度の約束、見本。
sidebar:
  order: 3
---

Omni は、プロジェクトの独自の Shader と Shader Graph を、利用者の環境で Radial（対数）用へ変換します。手動で前もって生成する必要はありません。対数レンダリングへの切替が、必要な生成と再検証を行います（[標準描画と対数描画を切り替える](../../getting-started/switch-rendering/)）。

:::tip[Shader Graph から始める]
変換は Graph の構造（どのノードがどの値を使うか）から作ります。そのため、手書きの `.shader` を Radial に合わせるより確実です。自分の効果を作るときは、Shader Graph から始めることを勧めます。
:::

## 生成物の置き場

| 生成物 | 置き場 |
| --- | --- |
| 変換した独自の Shader と Shader Graph | `Assets/Radial/User` |
| 変換が必要だった独自の include（HLSL） | `Assets/Radial/Hlsl`。元の論理パスを保つ |

- 変更していない Package の include は複製しません。元の `Packages/...` を参照します。
- 再生成しても、GUID、Scene と Prefab の参照、RenderQueue、Keyword、Pass、Properties を保ちます。保てない変換は、成功として扱いません。
- 生成物を手で編集すると、出力のハッシュが一致しなくなるので、Omni は上書きを止めます。マテリアルの一覧では「手編集あり・確認が必要」と出ます。

## ShaderLab／HLSL の変換

変換は次の順に進みます。

1. 元を ShaderLab か Shader Graph に分類する
2. 対象の SubShader、Pass、vertex と fragment のつながり、include、Property を、構文の境界で調べる
3. HLSL を書き換えて Radial の処理を入れ、必要な include の写しと記録を作る
4. 生成物が、必要な Pass、Property、タグ、深度と照明の約束を持つことを確かめる

- 相対パスの include は、元の Shader の論理的な位置を基準に解決します。
- `UsePass`、`Fallback`、`Dependency`、`CustomEditor` も解析します。
- Forward、Depth、DepthNormals、ShadowCaster、MotionVectors、Picking、Meta と独自の Pass を、Pass ごとに診断します。
- ShadowCaster と Meta には、描画するカメラの Radial の投影を入れません。物理の空間のままです。

## Shader Graph の変換

Shader Graph は、出どころ（プロジェクト、URP、ほかのパッケージ）によらず、同じ変換器で利用者の環境に生成します。

| Graph | 扱い |
| --- | --- |
| URP の Lit、Unlit | 変換する（頂点の Position に Radial を入れる） |
| URP の Fullscreen | 専用の経路で変換する（下を参照） |
| URP の Decal | 専用の経路で変換する |
| Canvas・UI、2D の Sprite、Six-way、Terrain Lit、URP 以外の Target だけの Graph | 変換しない。理由を示して止める |

- 頂点に Radial を入れるのは、物理の照明の約束を確かめた URP の Lit と Unlit の SubTarget だけです。
- Shader Graph 10 より前の形式（Master Node の形式）の Graph は、Shader Graph の取り込みと同じ方法で今の形式へ上げてから変換します。

### Fullscreen の Graph

霧のように、深度バッファから距離を求める効果のための経路です。Radial では、深度バッファに写像した後の深度が入っています。変換しないと、遠くの霧が標準より薄くなります。

- 頂点は変換しません。
- Scene Depth ノードと、そこから復元する world の位置は、写像した後の深度を物理の深度へ戻して返します。Custom Function の中で `SampleSceneDepth` を呼ぶ場合も同じです。
- Depth Write を有効にした Graph では、深度のブロック（Eye、Linear01、Raw）の手前で、Graph が書く物理の深度を、写像した後の深度へ直します。写像は視線の向きを変えないので、画面の位置はそのままです。
- 光線が当たらない画素の「遠方」の印は、そのまま書きます。Eye Depth なら 1e20 m 以上、Linear01 Depth なら 1 以上、Raw Depth なら far の値です。
- FullScreenPass が参照する、Graph に埋め込まれた Material は Shader を書き換えられないので、管理用の写しへ参照を差し替えます。

:::caution
Graph が深度を自前の Texture のプロパティ（RTHandle へコピーした深度など）から読む効果は、この置き換えの対象外です。標準の描画と一致しません。
:::

:::caution[Eye Depth の遠方]
Camera の far と同じ Eye Depth を「遠方」の印にしないでください。Radial では写像した後の far と区別できず、手前の深度へ直されます。
:::

## Custom Function の入口

`Assets/Omni/Runtime/Sharders/RadialWarpNode.hlsl` は、Shader Graph の Custom Function の入口です。変換器が Graph へ足すノードが、次の関数を使います。どの関数にも `_float` 版と `_half` 版があります。

| 関数 | 入力 → 出力 | 役割 |
| --- | --- | --- |
| `RadialKeepObjectPosition` | Object 空間の位置と Radial の Blackboard の値 → Object 空間の位置 | 頂点の Position 用。位置はそのまま返す。実際の Radial の投影は頂点のフックが行う。Radial の Blackboard のプロパティを Graph に保つためにつなぐ |
| `RadialGetLogicalViewDirection` | Object 空間の位置 → 論理の視線の方向 | fragment の照明に渡す、論理の視線の方向を返す |
| `RadialPreserveEmission` | 論理の視線の方向、元の Emission → 元の Emission | Emission をそのまま通す。PBR の初期化より前に、その fragment の論理の視線を保存する |
| `RadialPreserveDecalAlpha` | Alpha → 同じ Alpha | 値は変えない。Decal の Graph にこの include を保たせ、Projector の Pass で、深度から復元した位置の逆変換を有効にする |
| `RadialPreserveFullscreenColor` | 色 → 同じ色 | 値は変えない。Fullscreen の Graph にこの include を保たせ、Scene Depth の置き換えを有効にする |
| `RadialFullscreenDepth` | 画面の位置、深度、Mode → 深度 | Depth Write の Fullscreen の Graph で、物理の深度を写像した後の深度へ直す。Mode は 0 が Eye、1 が Linear01、2 が Raw |

- Shader Graph の include は全 Pass に展開されます。ShadowCaster と Meta では、頂点のフックが自動で無効になります。
- Decal Projector の Pass でも、頂点のフックは無効です。

:::note[頂点の段階の View Direction]
Shader Graph の頂点の段階では、URP が頂点側の処理（Custom Interpolator など）を、頂点の位置の計算より先に評価します。このときの View Direction ノードは、URP の標準と同じ物理の視線（カメラの位置 − 頂点の world の位置）です。Fog ノードは、URP の標準の計算が物理の world の位置と View 行列から距離を求めるので、この影響を受けません。
:::

## 手書きの Shader

通常の URP Shader として書けば、対数への切替が変換します。Radial に直接対応する Shader を書く場合は、`Assets/Omni/Runtime/Sharders` の公開の include を使います。

| include | 用途 |
| --- | --- |
| `RadialVertexInject.hlsl` | 手書きと生成の URP Shader の頂点の入口。`RadialInject.hlsl` と `RadialVertexHooks.hlsl` を読む。ShadowCaster と Meta では頂点のフックを無効にする |
| `RadialInject.hlsl` | Radial の packet、座標変換、深度の復元の共通の実装 |
| `RadialVertexHooks.hlsl` | `RadialInject.hlsl` の後で読み、URP の頂点の入口を、Object 空間からカメラ相対の写像した clip 空間へつなぐ。単独で include しない |
| `RadialObjectMotionVectors.hlsl` | 今と前のフレームの Object の位置から MotionVectors を作る専用の入口。通常の Forward の Pass には使わない |

`RadialVertexInject.hlsl` は、`Core.hlsl` と Material の入力（`LitInput.hlsl` など）の後、各 Pass の実装の前に include します。

```hlsl
#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
#define OMNI_RADIAL_EXPECT_ACTIVE 1
#include "Assets/Omni/Runtime/Sharders/RadialVertexInject.hlsl"
// この後に Pass の実装を include する
```

`OMNI_RADIAL_EXPECT_ACTIVE` を定義すると、`Core.hlsl` より前に include した場合にコンパイルエラーになります。同梱の Radial 版（例: `Assets/Omni/Radial/Universal Render Pipeline/Shaders/Lit.shader`）が、Properties の宣言と include の順の実例です。

Omni は、Shader が `_RadialGlobal_UseRadialTransform` を宣言しているかで、Radial に対応しているかを判定します。標準版を持たない Radial の Shader は、シェーダーのタブで「対応済」、マテリアルのタブで「変換不要・対象外」になり、標準へ戻す対象になりません。

## 深度の約束

Radial の Shader は、色だけでなく深度も、標準の描画と同じ空間の約束を守ります。

| 約束 | 内容 |
| --- | --- |
| 頂点の変換 | Object 空間から Radial の clip 空間へ。Forward、Depth、DepthNormals、Picking、MotionVectors で同じ空間の約束を使う |
| fragment の深度 | 物理の点を写像した深度（`SV_Depth`）。Forward、DepthOnly、DepthNormals、Picking で書く。深度を解くため、fragment でも同じ写像を画素ごとに評価する |
| Object の MotionVectors | 今と前のフレームの Radial の clip の位置から作る。履歴が無い場合は偽の速度を作らない |
| 照明のデータ | Lit の Forward と Forward+ は、Radial のラスターの位置と、物理の照明のデータを使う。物理の照明のデータを入れられない経路は描かない（[Fail Closed](../../concepts/fail-closed/)） |

深度を書くすべての Pass は、同じ位置に同じ深度を書かなければなりません。既定の Renderer（Forward+、Depth Priming が Forced）は、DepthOnly か DepthNormals の prepass で先に深度を書き、色の Pass を ZTest Equal で描きます。Pass ごとに深度が違うと、色が欠けます。Depth Priming を無効にしたときだけ正しく見えることは、対応の根拠になりません。

- 変換した Shader の深度の Pass と色の Pass は、Shader Model 4.5（OpenGL ES 3.1 相当）を必要とします。
- 写像、深度、照明のデータの約束は、品質レベルによって変わりません。

### Shader Graph の深度

URP の Lit と Unlit の Shader Graph には、画素ごとの深度（`SV_Depth`）を出すブロックがありません。Omni の変換器は、変換した Graph の深度を書く Pass（色、DepthOnly、DepthNormals、Scene の選択と Picking）に、Radial で写像した画素ごとの深度を足します。

- 手書きの Shader の変換と同じ約束なので、大きな面どうしの交差や Depth Priming でも、標準の描画に近い前後関係になります。
- Graph のノードで深度を指定することはできません。深度は、描いたメッシュの面から決まります。

## Shader Graph の見本

`Assets/Omni/Verification/Functions/Shaders/GraphSamples` に、CosmicScale のデモの手書きの Shader と同じ効果を、Shader Graph で作り直した見本があります。Graph の作り方と、Graph では表せない深度の制限を、並べて確かめられます。

| Scene | Graph | 内容 |
| --- | --- | --- |
| `BlackHole.unity` | `OmniGraphBlackHole`（Unlit、透明の Premultiply、裏面） | ブラックホールの重力レンズ。曲がった星空、光子リング、降着円盤とドップラーの色 |
| `Nebula.unity` | `OmniGraphNebula`（Unlit、透明の Premultiply、裏面） | 環状の惑星状星雲。殻の中を等間隔に歩いて放射を足す、手続き生成の体積 |
| `Star.unity` | `OmniGraphStar`（Unlit、不透明） | 恒星の表面。粒状斑、黒点と白斑、フレアのループ、周縁減光、差動回転 |
| `Planet.unity` | `OmniGraphPlanet`（Unlit、不透明） | 月。UV を求める Custom Function、Sample Texture 2D、照明の Custom Function |
| `DepthMarch.unity` | `OmniGraphDepthMarch`（Fullscreen、Depth Write） | 全画面の光線追跡で描いた球と輪が、床・柱・箱のメッシュと深度で交わる |

| 置き場 | 内容 |
| --- | --- |
| `Assets/Omni/Verification/Shaders/GraphSamples/` | Graph と Custom Function の HLSL |
| `Assets/Omni/Verification/Generated/Materials/GraphSamples/` | 見本の Material |
| `Assets/Omni/Demo/CosmicScale/Shaders/` | 手本の手書きの Shader |

### Scene の見方

物体の見本（BlackHole、Nebula、Star、Planet）は 4 列です。どの列も同じ球のメッシュで、列の見出しが物体の上に出ます。

| 列 | 中身 |
| --- | --- |
| Shader Graph（URP） | 変換する前の Graph。標準の URP の描画 |
| Radial 恒等 | 同じ Graph を Omni の変換器で Radial にし、写像を恒等にして描いたもの |
| Radial 対数 | 変換した Graph を、対数の写像で描いたもの |
| 手書き（CosmicScale） | 同じ効果の手書きの Shader（あるべき効果の手本） |

1. **Shader Graph と Radial 恒等**が同じに見えれば、変換は見た目を変えていません。
2. **Radial 恒等と Radial 対数**の違いは、対数の写像だけの影響です。形と位置は同じに見え、深度と照明の計算だけが写像の影響を受けます。
3. **Radial 対数と手書き**の違いは、Graph へ移したときの違いです。星雲の模様のノイズは、手書き版が焼いた 3 次元のノイズを使い、Graph 版は同じ種類のノイズを計算で求めるので、細部が一致しません。
4. 列は横に並んでいるので、カメラから見る角度が列ごとに少し違います。月の欠け方が列で違うのはこのためです。

- Scene は、変換する前の Material だけを保存しています。Radial の 2 列は、Editor で Scene を開いたときに、利用者の環境で生成した Radial 版（`Assets/Radial/User/` の下）へ差し替えます。保存の直前に元へ戻すので、Scene の変更にはなりません。
- 変換できなかったときは、その列を描かず、理由を Console に出します。
- 見本は CosmicScale と同じ露出とトーンマッピングで描きます。発光の強い効果は、トーンマッピングが無いと白く飛びます。
- 画面のパネルで、Radial と標準の切替、往復の動き、Motion Vectors の表示ができます。Edit Mode では、Scene View の「Omni Verification」Overlay と主カメラの Inspector に、同じ操作と「文書を開く」があります。

### 物体の Graph の深度の制限

光線追跡で中身を描く効果は、外側の殻のメッシュに描くので、面の深度では足りません。

| 見本 | 違い |
| --- | --- |
| ブラックホール | 手書き版は、画素の視線の上で中心と同じ距離にある点の深度を `SV_Depth` で書き、その深度で比べる。Graph 版は深度を書かない透明で、殻の裏面で比べる。殻の中で中心より奥にある物体は、本来は隠れるはずだが、Graph 版では隠れない |
| 星雲 | Graph 版は、光線が殻を通る全区間の光を足す。殻の中に物体があっても、その奥の光まで物体の上に重なる |
| 恒星、月 | 不透明の面なので、普通のメッシュと同じ深度で正しく前後する |

回避するには、効果の殻を、ほかの物体と重ならない大きさにします。どうしても重なる場合は、手書きの Shader か、全画面の Graph を使います。

### 全画面の Graph は深度を書ける

URP の Fullscreen の Shader Graph は、Graph の設定で **Depth Write** を有効にすると、出力に **Eye Depth**、**Linear01 Depth**、**Raw Depth** のどれか（Depth Write Mode で選ぶ）のブロックが現れ、画素ごとの深度を書けます。`DepthMarch.unity` がこの見本です。

- 光線が当たった画素に、カメラからの目の深度（カメラの前方向に沿った距離）を書きます。
- 当たらない画素はアルファを 0 にし、「遠方」の深度を書きます。深度の比較（Less Equal）で、既に描かれた物体を上書きしません。
- 「遠方」は、Eye Depth なら 1e30 のような非常に大きな値（見本と同じ）、Linear01 Depth なら 1、Raw Depth なら far の値（`UNITY_RAW_FAR_CLIP_VALUE`）で書きます。
- 全画面の Graph は、URP の Renderer Feature（Full Screen Pass）か、Scene の部品（見本では `GraphSampleFullscreenPass`。この Scene の Camera にだけ効く）で描きます。
- 光線追跡の中で床の深度を読むなら、Camera の深度テクスチャを有効にします。

### 自分の Graph を作るとき

1. 重い処理（ループを持つ光線追跡など）は、Custom Function ノード（Type = File）の HLSL に書きます。Graph では、入力のノード（Position、Camera、Screen Position、Scene Depth、Time、Normal Vector、View Direction）とプロパティをつなぎます。
2. Custom Function の関数は `関数名_float` を定義し、Graph の精度を Single にします。
3. 物体の空間での光線追跡は、Position（Object）と、Camera の Position を Transform（World → Object、Position）で物体の空間へ移した値から始めます。
4. 深度は Scene Depth ノードで読みます。Custom Function の HLSL の中で `SampleSceneDepth` を呼ぶ場合も置き換えが効きますが、Graph に Scene Depth ノードを置いて、深度テクスチャを要求させる必要があります。
5. 発光とアルファを別々に出す半透明の効果は、Blending Mode を Premultiply にします。
6. 手書きの Shader の global の値は Graph から使わず、Material のプロパティにします。

:::note[確かめていないこと]
安全域（10 km）より遠い物体で、Transform ノードの行列を使う Graph が正しく描かれるかは、この見本では確かめていません。
:::

## 代わりの Shader を登録する

固定機能の Shader や Built-in 専用の Shader など、Omni が変換できない Shader には、代わりの URP Shader を登録できます。

1. Project ウィンドウで **Create > Omni > Radial Shader Substitutes** を選び、登録表（`OmniRadialShaderSubstitutes`）を作ります。
2. 項目を足し、元の Shader（`source`）と、代わりの URP Shader（`substitute`）を設定します。

- Material を Radial へ切り替えるときだけ、代わりの Shader から Radial 版を生成して使います。元の Shader とそのファイルは変えません。標準へ戻すと、元の Shader へ戻ります。
- 代わりの Shader は、元の Shader の全 Property を、同じ名前と同じ型で宣言しなければなりません。Float と Range は同じ型とみなします。
- 代わりの Shader は、`Hidden/` 以外の名前で、URP の Shader として通常の検証を通らなければなりません。
- 登録表はプロジェクトの中に複数置けます。同じ元の Shader に対して表どうしの登録が食い違う場合、Property が足りない場合などは、理由を示して変換しません。

## 変換できないもの

| 対象 | 扱い |
| --- | --- |
| 固定機能、Built-in 専用の Shader | 変換しない。代わりの Shader を登録できる |
| Canvas・UI、2D の Sprite、Six-way、Terrain Lit、URP 以外の Target だけの Shader Graph | 理由を示して止める |
| 自前の Texture から深度を読む Fullscreen の効果 | 標準の描画と一致しない |
| Deferred、Deferred+ の GBuffer の Pass | 対応しない。RadialCamera を含む Player のビルドは止まる |
| XR の Camera MotionVectors | 対応の範囲に含まない |
| Terrain の木と細部 | Shader の変換だけでは対応しない（[Terrain](../terrain/)） |

変換できない Material は、似た Shader へ推測で置き換えません。元の Material の参照を残したまま診断表示にし、結果を成功とは表示しません（[Shader と Material](../shaders-and-materials/)）。
