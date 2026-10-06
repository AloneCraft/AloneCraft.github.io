---
title: Scene BRG
description: 読み込んだ Scene の MeshRenderer を BatchRendererGroup でまとめて描く経路と、その準備状態。
sidebar:
  order: 7
---

Scene BRG は、読み込んだ Scene の MeshRenderer を、Unity の BatchRendererGroup（BRG）でまとめて GPU に渡す Omni の描画の経路です。標準描画でも対数描画でも働きます。

## 2 つの backend

Omni Manager の「Scene BRG」タブには、今の backend が出ます。

| 表示 | 内容 |
| --- | --- |
| BatchRendererGroup（製品） | Scene BRG で描く。既定 |
| Legacy（検証用） | 元の Renderer で描く。Scene BRG を使わない |

URP の Forward（Forward+ ではない）の Renderer で描く Game View または Scene View のカメラがある間は、Scene BRG を使わず、Legacy で描きます。Forward は追加のライトを物体ごとの一覧で渡しますが、BRG の物体はそれを読めないためです。そのカメラが無くなると、Scene BRG を作り直します。

:::caution[Forward と照明]
Legacy の Radial は余裕を持たせた Bounds を使います。そのため Forward では、物体ごとの Reflection Probe と Light Probe の選び方が標準の描画とずれます。Light Probe の空間の変換も行いません（[照明の空間](../lighting-spaces/)）。Radial で標準と同じ照明を求める Scene では、Forward+ の Renderer を使ってください。
:::

## 対象になる Renderer

Scene BRG は、読み込んだ通常の Scene の MeshRenderer を自動で集めます。非アクティブなオブジェクトも含みます。Material が 1 つも無い Renderer と、Mesh がまだ無い Renderer は集めません（Mesh ができた後の作り直しで対象になります）。

次の Renderer は Scene BRG に載せず、通常の Radial の MeshRenderer の経路に残します。外れるのはその Renderer だけで、Scene 全体の Scene BRG は止まりません。

| 条件 | 理由 |
| --- | --- |
| `RadialSceneBatchRendererExclusion` を付けた | 明示的に外す印 |
| Omni 以外が設定した MaterialPropertyBlock を持つ（Animator が `material.*` を書く Renderer など） | BRG で表せない。MaterialPropertyBlock は保つ |
| 永続的な Asset ではない Material を使う（スクリプトが Edit Mode で作った複製など） | BRG 用の Material を作れない |
| `RadialIdentityWarp` を付けた | Scene BRG には Instance ごとに恒等の写像へ切り替える仕組みが無い |
| 保存されない隠しの GameObject の下にある | 自動の収集の対象外 |

MaterialPropertyBlock の判定は、Scene BRG を作るときに行います。作った後に付いた MaterialPropertyBlock は、次の作り直しで判定します。

### 実行時に作る Renderer

Scene に保存した MeshRenderer は自動で登録されます。実行時にコードで作る MeshRenderer は、`RadialBatchRendererRegistry` で登録します。

| API | 内容 |
| --- | --- |
| `Register(renderer, provider)` | Renderer を登録し、Handle を返す。`provider` は省略できる |
| `Unregister(handle)` | 登録を外す。Renderer は通常の描画に戻り、もう一度登録するまで自動の収集からも外れる |
| `MarkDirty(handle, flags)` | 変わった入力だけを、次の安全な更新で反映する |

`MarkDirty` の `flags`（`RadialBatchDirtyFlags`）は次のとおりです。`Renderer.enabled` や GameObject の active を変えた後は、`Visibility` を通知します。

| フラグ | 変わったもの |
| --- | --- |
| `Transform` | Transform または Logical Transform |
| `GeometryOrMaterial` | Mesh、SubMesh、Material、Material の Property |
| `Filtering` | Layer、Shadow、Lightmap、Motion などの設定 |
| `Visibility` | `Renderer.enabled` または GameObject の active |
| `MeshVertexData` | 同じ Mesh の頂点と Bounds だけ |
| `MeshBinding` | 同じ Renderer の Mesh の参照だけ（Material などは同じ） |
| `All` | すべて |

Instance ごとの独自の値は、`IRadialBatchInstanceDataProvider` で渡します。渡せるのは `float` と `Vector4` だけです。Texture は Instance ごとに変えられません。

登録した Renderer に Play 中に割り当てた、永続的ではない Material は、検証済みの BRG 用の Shader の複製へ変換して描きます。複製には、毎フレーム元の Material の値を写します。Play 中に割り当てる Material の元を、Scene の MonoBehaviour に `IRadialBatchMaterialDependencyProvider` を実装して申告すると、その Material の BRG 用の Material も前もって作ります。

## BRG 用の Material（sidecar）

Scene BRG は、Renderer の元の Material ごとに作る BRG 用の Material（sidecar）で描きます。

- sidecar は、Scene ごとではなく、元の Material の GUID ごとに `Assets/Radial/BRG` に作ります。同じ Material を使うすべての Scene が共有します。
- Scene BRG で描く限り、標準描画でも対数描画でも sidecar が要ります。Legacy では要りません。
- Omni Manager を開いていなくても、Scene の開閉、Asset の変更、Undo、Play の開始、標準と対数の切替の後、次の Editor の更新で、読み込んだ Scene の sidecar を自動で準備します。
- Terrain 系の Shader（TerrainLit、TerrainLitAdd、TerrainLitBase、TerrainLitBasemapGen）を MeshRenderer に割り当てた場合も、入力を BRG 用に変換した sidecar で描きます。

## 準備状態

Omni Manager の上部の「Scene BRG」の行と、「Scene BRG」タブで状態を確かめます。

| 状態 | 意味 |
| --- | --- |
| 準備完了 | 読み込んだ Scene が必要とする sidecar と manifest が揃っている |
| 準備中… | 準備を予約した、または実行している |
| 失敗（Scene BRGの更新を停止中） | 準備に失敗した。段階、元の Material、Shader、理由、影響する Renderer を表示する |
| 不要（Legacy backend） | 描画に sidecar を使わない。直近の準備の結果は参考として表示する |
| 未確認 | この Editor の起動の間にまだ確かめていない |

準備が失敗している間は、成功するまで、Scene BRG は Renderer の引き継ぎと更新を行いません（[Fail Closed](../../concepts/fail-closed/)）。これは Material の切替の成否とは別の結果です。

:::note
「準備完了」は sidecar が揃ったことを示します。GPU がすべての描画を受け入れたことまでは示しません。Unity が「not SRP Batcher compatible」などの理由で BRG の描画を拒んだ場合は、Console にエラーが出ます。
:::

### 手動の操作

「Scene BRG」タブの「操作」には、次のボタンがあります。どちらも次の安全な Editor の更新で実行します。Play 中とコンパイル中は実行できません。

| ボタン | 内容 |
| --- | --- |
| ロード済みSceneのsidecarを再確認・再生成 | 読み込んだ Scene の sidecar を確かめ、足りないものと古いものを作り直し、すべての sidecar に元の Material の値を写し直す。保存する前にスクリプトで変えた元の Material の値を反映したいときにも使う |
| Build SettingsのSceneを含めて全体生成… | Build Settings の有効な Scene と読み込んだ Scene を対象に sidecar を作り、manifest をその集合で作り直す。Build の Scene を 1 つずつ Preview で開くので時間がかかる |

### Play の開始時

Play を始めるとき（Edit Mode を出るとき）、Play 中と同じ検証を編集中のうちに行います。通らなければ、保存と Refresh で状態を確定させて sidecar を作り直します（最大 2 回）。それでも通らなければ、Play の準備を理由付きで止めます。理由には、保存した Hash、今の Hash、sidecar の Shader、Instancing、DOTS の keyword が出ます。

## カリングと Layer

- Camera の Culling Mask は守ります。Unity は BRG の描画を Culling Mask で絞らないので、Scene BRG のカリングが Layer を照合して外します。
- Radial が無効なカメラの View だけが、Unity の平面でカリングします。
- Radial が有効なカメラの View と、カメラ以外の View（影、Picking、選択の輪郭）は、平面でのカリングをせず、すべての対象を出します。対数で圧縮した後の位置は、平面では判定できないためです。

## Reflection Probe と Light Probe

- 焼く Reflection Probe（Baked、または Dynamic Objects を含まない Custom）を描くときは、Reflection Probe Static の対象だけを描きます。Realtime の Probe、Dynamic Objects を含む Probe、Play 中は、すべての対象を描きます。
- Light Probe の補間には、表示の位置ではなく、Probe を焼いた空間の位置を使います。表示用に Root を動かした Scene の設定は [照明の空間](../lighting-spaces/) を参照してください。
