---
title: Fail Closed
description: 正しく描けると確かめられない経路で Omni が止まる場所と、そのときの対処。
sidebar:
  order: 4
---

Omni は、正しく描けると確かめられない経路では、もっともらしい絵を出さずに止まります。これを Fail Closed と呼びます。

見た目が正しそうでも、深度、照明、Culling、Picking のどれかが空間の約束を破っていることがあります。Omni は、そのような状態を成功として見せません。止まった理由を示し、元の状態を残します。

## 止まる場所

| 段階 | 止まり方 |
| --- | --- |
| Player のビルド | ビルドの開始時、または Scene の処理中に、理由を示してビルドを止める |
| 対数描画への切替 | 何も変更する前に止める。または、変換できない部分だけを診断表示にして、成功とは表示しない |
| 描画 | 正しく描けない対象を描かない |
| 入力と Picking | 確かめられない結果を、空のクリックとして扱わない |

## Player のビルドで止まるもの

ビルドのエラーには理由が出ます。多くの場合、対象のカメラと Scene、直し方も出ます。

### ビルド対象と Graphics API

RadialCamera を含む Scene のビルドで検査します。Radial の Shader は Shader Model 4.5（OpenGL ES 3.1 相当）を必要とします。対応表は [動作環境](../../getting-started/requirements/) にあります。

| 条件 | 対処 |
| --- | --- |
| Android、iOS、tvOS、visionOS | Windows、macOS、Linux、または WebGPU の Web でビルドする |
| Web で Auto Graphics API が有効、または WebGPU 以外の API がある | Player Settings > Web > Other Settings で Auto Graphics API を外し、WebGPU だけを残す |
| Graphics API の一覧に OpenGL ES 3.0 がある | 一覧から外す |
| macOS で OpenGL Core を使う | Metal を使う |

Player には Material を URP の元の Shader へ戻す仕組みがありません。実行時に Radial を止めても表示は直らないので、ビルドの時点で止めます。

### URP の Renderer

| 条件 | 対処 |
| --- | --- |
| ビルド対象の品質レベルが参照する URP 資産のどれかに、Deferred または Deferred+ の Renderer がある | その Renderer を Forward+（または Forward）にする |
| RadialCamera のカメラが 2D Renderer を使う | Forward か Forward+ の Universal Renderer を使う。2D は RadialCamera の無い別のカメラで描く |
| RadialCamera のカメラの Renderer が Universal Renderer でない、または解決できない | Forward か Forward+ の Universal Renderer を使う |
| Render Objects の Override の Material や Shader が、Scene BRG と両立しない | エラーの理由に従って Override を直す |

Deferred の検査は、カメラが使う Renderer だけでなく、URP 資産の全 Renderer に及びます。URP は Deferred の Renderer が 1 つでもあると、使わなくても全 Shader の GBuffer の Pass を Player に入れるためです。

### Omni の Shader の接続

| 条件 | 対処 |
| --- | --- |
| Camera MotionVectors の Shader が Omni 版につながっていない | Omni Manager の「シェーダー」タブの Camera MotionVectors の欄で「Omni Shader を導入」を押す |
| 有効な SSAO があり、SSAO の Shader が Omni 版につながっていない | 同じタブの SSAO の欄で「Omni Shader を導入」を押す |
| Radial の Scene が使う VFX の準備ができていない | エラーに出る状態と理由を確かめ、VFX の変換を直す（[VFX Graph](../../guides/vfx/)） |

Camera MotionVectors と SSAO は、対数描画への切替が自動でつなぎます。URP の標準以外の値が設定されていた場合は上書きしないので、手動でつなぎます。

### Scene の構成

| 条件 | 対処 |
| --- | --- |
| 有効な RadialCamera を含む Scene と、DOTS の SubScene を含む Scene を、同じ Player に入れる | 別の Player に分ける。Omni 0.1 は DOTS／Entities Graphics の描画を対数描画の対象にしない |
| `OmniLightProbeSpaceRequirement` を置いた表示の Root で、Light Probe の空間の約束が満たされない | 表示の Root の Omni Light Probe Space の Inspector の「検証」で確かめ直す（[照明の空間](../../guides/lighting-spaces/)） |
| `OmniAdaptiveProbeVolumeSpaceRequirement` を置いた表示の Root で、APV の空間の約束が満たされない | 表示の Root の `OmniAdaptiveProbeVolumeSpace` と `OmniAdaptiveProbeVolumeSpaceRequirement` を確かめる |

Light Probe の空間で止まるのは、Requirement を置いた Root だけです。推定による不一致は、警告に留めます。

### すべての Player のビルド

次の検査は、RadialCamera の有無によらず、Omni を入れたプロジェクトのすべての Player のビルドで行います。

| 条件 | 対処 |
| --- | --- |
| 現在の Render Pipeline が URP でない | Graphics の設定で URP の資産を使う |
| 変換エラーの診断表示が元に戻されていない | Omni Manager で「標準レンダリング」を選び、元に戻す |
| URP Global Settings の Strip Unused Variants が無効 | 有効にする |
| 1 つの Shader の Pass・stage の variant が 8,192 を超える、または合計が 100,000 を超える | 品質レベルの間で設定をそろえる（[動作環境](../../getting-started/requirements/) の「品質の設定」） |

## 対数描画への切替で止まるもの

切替の詳しい流れは [標準描画と対数描画を切り替える](../../getting-started/switch-rendering/) にあります。

| 種類 | 例 | 結果 |
| --- | --- | --- |
| 安全に元に戻せることを保証できない | Prefab Stage、復元の台帳の競合、診断用の Shader の欠落、Asset の書き込みの失敗 | 何も変更する前に止まる |
| DOTS／Entities Graphics | 読み込んだ Scene に SubScene、または Entities Graphics の描画 Entity がある | 何も変更する前に止まる |
| 変換できない Material | 対数の Shader が無い、生成やコンパイルに失敗した | そのスロットだけを診断用の Material に置き換え、結果は `CompletedWithConversionErrors` |

診断用の Material は、黄色と黒の斜めの縞に赤い × の模様です。元の Material の参照は残ります。Omni Manager の上部には「変換エラー・診断表示中」と件数が出ます。

変換器は、物理的な照明のデータを注入できない Lit の経路を変換しません。Shader の場合は変換の失敗として扱います。VFX の場合は、その VFX を対応外にして理由を出し、Player のビルドを止めます。

## 描画で描かないもの

| 対象 | 描かない条件 | 表示 |
| --- | --- | --- |
| Logical Transform の下の Renderer | RadialCamera が有効で、Material が Radial に対応しない | Console に警告。Omni Manager の「マテリアル」「シェーダー」で生成・適用する |
| 安全域の外にある VFX | Radial でないカメラ、影、Decal の出力 | 描かない |
| Camera MotionVectors | 最初のフレーム、または前のフレームの情報が無い | 速度をゼロにする |
| 階層が禁止の配置にある Logical Transform | Logical Transform → 通常の Transform → Logical Transform | Console にエラー。親との合成をやめる |

階層の規則は [Logical 座標と Proxy](../logical-coordinates/) にあります。

## Picking と入力

Scene View の Picking は、結果を `Hit`、`Miss`、`Unavailable` の 3 つに分けます。

| 結果 | 意味 |
| --- | --- |
| `Hit` | 対象が当たった |
| `Miss` | 確かめた上で、何も当たらなかった |
| `Unavailable` | 確かめられなかった |

- 選択の解除を求めるのは、修飾キーの無い実際の `Miss` だけです。
- `Unavailable`、UI の上の入力、修飾キー付きの `Miss` では、選択を解除しません。
- ホバーの表示を作れなかったときは `Unavailable` として、輪郭を出しません。
- 中ボタンのドラッグは、`Unavailable` では始めません。

詳しい操作は [Scene View の操作](../../guides/scene-view/) にあります。

## 逆に、すべて描く側へ倒すもの

Fail Closed は、正しさを確かめられない場合に使います。描画の量を減らすだけの判断では、逆に、見えるべき物を消さない側へ倒します。

- Radial が有効なカメラの描画、影、Picking、選択の輪郭では、Scene BRG は Culling の面で対象を外しません。写像の後の位置を面で判定できないためです。
- ハードウェアの Near は、論理 Near より奥を切らない側へ倒します（[Near と Far](../near-far/)）。

## 止まったときの対処

1. ビルドのエラー、Console、Omni Manager の上部に出た理由を読みます。カメラ、Scene、Renderer、Material の名前が出ます。
2. 表の対処に従って設定を直します。
3. もう一度ビルドするか、切替をやり直します。

直した後の表示が正しいかどうかは、[Verification Hub](../../getting-started/verification/) で確かめられます。
