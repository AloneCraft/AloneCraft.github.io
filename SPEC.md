# AloneCraft Webサイト 仕様

状態: 基本方針は確定（2026-10-06）。作る段階で決める事項は「12. 後で決めること」に置く。

## 1. 目的

- AloneCraft 全体のサイト（ブランド・製品一覧・サポート・法務）。
- 最初の製品として、Unity アセット **Omni**（0.1.0、販売予定）の技術紹介・導入ガイド・リファレンスを載せる。
- 将来の製品を同じ構造で追加できるようにする（`/<product>/...`）。

## 2. 情報源（正本）

サイトの内容は次から作る。開発用の内部資料（`Docs/CurrentWork.md`、`UserBugHunt.md`、`Assets/Development` など）は載せない。

| 内容 | 正本 |
| --- | --- |
| 製品説明・対応環境・デモ | `Assets/Omni/Documentation/README.md` |
| 導入確認 | `Assets/Omni/Verification/README.md` |
| Shader／Material運用 | `Assets/Omni/Documentation/ShaderAndMaterialWorkflow.md` |
| Shader API | `Assets/Omni/Documentation/ShaderApi.md`、`Runtime/Sharders/*.hlsl` |
| Shader Graph見本 | `Assets/Omni/Documentation/ShaderGraphSamples.md` |
| Scene View Overlay | `Assets/Omni/Documentation/SceneViewRadialOverlay.md` |
| 概念・制約 | `Docs/Architecture.md`、`Docs/OmniUniversalInvariants.md`、`Docs/LogicalNearClip仕様.md` |
| C# API | `Assets/Omni/Runtime`、`Editor` の公開型 |
| ライセンス | `Assets/Omni/Documentation/Third-Party Notices.md` |

## 3. Omni の要点（ソースから読み取った事実）

- Logical 座標: `Scalar` は Q95.96 の 192 bit 固定小数点。範囲 ±3.96×10²⁸ m、分解能 2⁻⁹⁶ m ≈ 1.26×10⁻²⁹ m。回転は Q1.126（128 bit）。溢れる演算はすべて `Try*` API で、黙って丸めたり飽和したりしない。
- `Omni.Logical.Transform` が位置・回転・Scale の正本で、Unity の Transform は有限な表示 Proxy。
- Radial（対数）描画: `F(d) = log2(1 + d·M)`。調整できるのは View Scale M（Camera の Near ÷ 論理 Near）だけ。1台のカメラで cm から 10²⁷ m までを途切れずに描く。
- Standard と Radial は位置・深度（`SV_Depth`）・影・Picking・Culling で同じ空間契約を満たす。物理 Lighting Data を注入できない経路は Fail Closed。
- 標準 URP Shader は事前変換済みで同梱。独自 Shader／Shader Graph／VFX はプロジェクト内で生成し `Assets/Radial` へ置く（GUID は `ProjectSettings/Omni/Generated.manifest.json`）。
- 対応: Unity 6000.6.0f1、URP 17.6.0（Forward／Forward+）、Input System 1.20.0。Windows D3D12 と Web（WebGPU のみ）が受入済み。モバイル、WebGL 2、Deferred、2D Renderer は Fail Closed。DOTS／Entities Graphics の描画は 0.1 の対象外（0.2 で対応予定）。
- デモ: EarthToMoon、SolarSystem、CosmicScale、CosmicHierarchy（WebGPU ビルドあり）。CosmicEvents、Asteria は制作中。

## 4. サイト構成（案）

```text
/                         AloneCraft トップ（ブランド、製品一覧、最新情報）
/about/                   AloneCraft について
/support/                 問い合わせ、FAQ
/legal/                   利用規約、プライバシー
/omni/                    製品紹介（何ができるか、デモ映像、対応環境、購入リンク）
/omni/demos/              デモ（WebGPU で遊べるもの＋動画）
/omni/docs/
  getting-started/        導入、URP 設定、Hub で確認、標準／対数の切替
  concepts/               Logical 座標と Proxy、Radial 写像と View Scale、Near/Far、Fail Closed
  guides/                 Shader と Material、Shader Graph、VFX、Terrain、照明空間
                          （Light Probe／APV／Lightmap）、Scene BRG、観察カメラ、Sequencer
  platforms/              ビルド対象、品質設定、Shader variant
  limitations/            0.1 の対象外と既知の制約
/omni/reference/
  components/             RadialCamera、RadialCameraToggle、Logical Transform、
                          RadialIdentityWarp、RadialRigidAnchor、照明空間、Observer、Sequencer
  logical/                Scalar、Position3、Vector3、Quaternion、Matrix3x3、Math、Precision
  units/                  Angle、Duration、AngularVelocity、Mass、Energy など
  celestial/              CelestialCatalog
  editor/                 Omni Manager（各タブ）、Scene View Overlay、ショートカット、メニュー
  shader/                 include、Custom Function、グローバル packet、define
  files/                  生成物と ProjectSettings のファイル
/omni/changelog/
/omni/third-party-notices/
```

## 5. 技術構成（確定）

- Astro + Starlight の静的サイト。本文は Markdown／MDX。
- AloneCraft のトップと製品紹介は Astro の独自ページ、ドキュメントとリファレンスは Starlight で作る。
- 公開先: 静的ホスティング（GitHub Pages または Cloudflare Pages。後で決める）。

## 6. 言語（確定）

- 日本語と英語。日本語を正本とし、英語はその翻訳として作る。
- Starlight の i18n を使う。日本語は `/ja/`、英語は `/en/` に置く（ルートの既定言語は後で決める）。
- 日本語のページを変えたら、同じ作業で英語のページも直す。翻訳が遅れている英語ページは、Starlight の「未翻訳」表示に任せて、古い内容のまま放置しない。

## 7. 製品名（確定）

- 製品名は **Omni**。URL は `/omni/`。描画方式の名前は、コードと同じく Radial（対数描画）とする。

## 8. リファレンスの作り方（確定）

- 型とメンバーの一覧（シグネチャ、属性、既定値、Add Component のメニュー名）は Omni のソースからスクリプトで抽出し、Markdown を生成する。概念・使い方・例は手書きにする。
- 抽出の対象は、サイト側に置いた**公開APIの許可リスト**に載せた型だけにする。
  - 理由（事実）: Runtime には public の型が 193 個あるが、`RadialGlobalRenderSystem` など、利用者が直接使わない内部向けの型も含まれる。
- ソースの XML コメント（`<summary>`、Runtime の 145 ファイル中 140 ファイルにある）は下書きとして使うが、そのまま載せない。
  - 理由（事実）: 「旧: …」のような実装の経緯が混じっている（例: `RadialCamera.cs`）。
  - 載せる説明は、型ごとにサイト側の手書きの文で上書きする。
- 生成したページには、元にした Omni のコミットと製品版数（`OmniShaderCatalog.ProductVersion`、今は 0.1.0）を書く。
- 再生成したとき、許可リストの型が消えていたり、名前が変わっていたりしたら、スクリプトを失敗させる。古いページを黙って残さない。

### 生成器の構成（2026-10-07 実装）

| 部品 | 役割 |
| --- | --- |
| `tools/ApiExtract`（.NET 10、Roslyn の構文解析） | Omni の `Assets/Omni/{Runtime,Editor,Converter}` の公開型・メンバー（シグネチャ、属性、既定値、XML の summary、`[SerializeField]` のフィールド）と、`Runtime/Sharders/*.hlsl` の関数・include・Shader の識別子を `src/data/api/omni.json` へ書き出す。コミットと製品版数も書く |
| `reference/omni/allowlist.yaml` | 載せる型（カテゴリごと）と Shader のページ（include、Custom Function、グローバルのプロパティと define） |
| `reference/omni/notes/<lang>/<型の完全名>.md` | 手書きの説明。frontmatter の `description`、冒頭の本文、`<!-- members -->` の後の `### メンバーID` ごとの説明 |
| `scripts/gen-reference.mjs` | 上を合わせて `src/content/docs/<lang>/omni/reference/<カテゴリ>/<slug>.md` を書く。1 型 1 ページ。Inspector の項目（保存フィールド）は箇条書き、ほかのメンバーは種類ごとの見出し |

- 失敗の条件: 許可リストの型・除外・HLSL の関数・include・識別子がソースに無い、説明文が存在しないメンバーを指す、許可リストに無い型の説明文が残っている。
- 説明の無いメンバーは警告。`--strict`（`npm run check:reference`）では失敗。`==`、`Equals` などの決まったメンバーには定型の説明を入れる。
- Editor（Omni Manager、Overlay、ショートカットとメニュー）とファイルのページは手書き。

## 9. デモの載せ方（確定）

- 各デモは動画で見せる。動画の置き場（サイトに直接置くか、YouTube などへ置くか）は、作るときに容量を見て決める。
- CosmicScale と CosmicHierarchy（WebGPU ビルド、それぞれ約 34MB と 32MB）は、ブラウザーで動かせるように埋め込む。
  - ページを開いただけでは読み込まない。利用者が「起動」を押してから読み込む。
  - WebGPU が使えないブラウザーでは起動ボタンを出さず、理由と動画を出す（Fail Closed。動かないものを動くように見せない）。
  - 日本語の表示は、デスクトップ版にしかない。Web 版は英語だけ、とページに書く（Omni の README のとおり）。
- EarthToMoon（WebGPU で 281MB）と SolarSystem は動画だけにする。
- 制作中のデモ（CosmicEvents、Asteria）は、完成するまで載せない。

## 10. デザイン（確定）

- 製品紹介（`/omni/`）: 暗い背景に、デモの映像（CosmicScale など）を大きく使う。
- ドキュメント・リファレンス: 読みやすさを優先し、ライトとダークを切り替えられるようにする（Starlight の標準機能）。
- AloneCraft のトップ: 特定の製品に寄せすぎない中立の色にし、製品が増えても使えるようにする。
- スマートフォンの幅でも読めるようにする（横スクロールを出さない）。

## 11. 実装の状況（2026-10-07）

作ったもの:

| URL | 中身 | ファイル |
| --- | --- | --- |
| `/` | ブラウザーの言語で `/ja/` か `/en/` へ振り分ける（既定言語が決まるまでの仮） | `src/pages/index.astro` |
| `/<lang>/` | AloneCraft のトップ（製品の一覧） | `src/pages/[lang]/index.astro` |
| `/<lang>/omni/` | Omni の製品紹介 | `src/pages/[lang]/omni/index.astro` |
| `/<lang>/omni/docs/` | 概要 | `src/content/docs/<lang>/omni/docs/index.md` |
| `/<lang>/omni/docs/getting-started/*` | 動作環境、導入、Verification Hub、標準／対数の切替 | `src/content/docs/<lang>/omni/docs/getting-started/` |
| `/<lang>/omni/docs/concepts/*` | Logical 座標と Proxy、Radial の写像と View Scale、Near と Far、Fail Closed | `src/content/docs/<lang>/omni/docs/concepts/` |
| `/<lang>/omni/docs/guides/*` | Scene View、Shader と Material、Shader Graph、VFX Graph、Terrain、照明の空間、Scene BRG、Radial 観察カメラ、Sequencer | `src/content/docs/<lang>/omni/docs/guides/` |
| `/<lang>/omni/reference/*` | リファレンス。型のページ 53 と Shader のページ 3 は生成（`npm run gen:reference`）。Editor の 3 ページとファイルの一覧は手書き | `src/content/docs/<lang>/omni/reference/`、`reference/omni/` |

- ブランドページの文言は `src/i18n/ui.ts` に日英で置く。
- 画像（`src/assets/omni/`）は CosmicHierarchy の録画（`records/CosmicHierarchy_1080p_vp9.webm`）から、ラベルの無い範囲を切り出した仮のもの。UI を消した状態（デモの H キー）で撮り直した画像に差し替える。
  - 文字を画像処理で消す方法（ffmpeg の delogo）は、跡が見えたので使わない。

未着手: ドキュメントの「プラットフォーム」「制約」「トラブルシューティング」、デモのページ、変更履歴、Third-Party Notices、AloneCraft の About／Support／Legal。

- 概念・ガイドのページは Omni の購入者向け文書とコードから書いた。文書とコードが食い違う所はコードに合わせた（例: 診断表示のときの Manager の上部は「対数（警告 N件）」、VFX は取り込み時のパッチ方式、`Radial/VFX` フォルダーは無い）。
- UI の名前は `Assets/Omni/Localization/ja.po` に合わせる（日本語は msgstr、英語は msgid）。

## 12. 後で決めること（作る段階で必要になったら）

- 公開先（GitHub Pages／Cloudflare Pages）と独自ドメイン。
- ルート（`/`）の既定言語。
- 動画の置き場。
- 購入リンク（Asset Store のページ URL）。
- AloneCraft の会社・個人情報、問い合わせ先、利用規約とプライバシーポリシーの本文。
- AloneCraft のトップの説明文（今は「AloneCraft の製品とドキュメント。」という仮の文）。
- GitHub の AloneCraft organization（`CSharpGameEngine` の sourcelink に `github.com/AloneCraft` がある）へのリンクを載せるか。
