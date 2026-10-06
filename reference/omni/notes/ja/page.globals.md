---
description: "Radial の Shader が読むカメラの packet、Radial への対応を表すプロパティ、include の動きを変える define です。"
---
Radial の Shader が読むグローバルの値と define です。

- `_OmniRadialCamera_*` はカメラの packet です。Omni がカメラごとに、描画の前に書きます（SRP ではカメラのコマンドの列へ順に書きます）。Scene の BRG の描画では、同じ名前の値をカメラごとのインスタンスのデータから読むので、同じフレームに描く Game View と Scene View の値が混ざりません。宣言は `RadialInject.hlsl` にあります。
- 位置の値は、絶対の float の座標を持ちません。Logical の差を、仮数（xyz）と 2 の指数（w）に分けて持ちます。値は `xyz × 2^w` です。
- `Previous*` は、同じカメラの、前に書いた packet の値です。そのカメラで初めて書くときは今の値と同じなので、履歴が無いことによる大きな偽の速度は出ません。Object の MotionVectors が使います。
- Omni が描画の仕組みを解放するときは、`_OmniRadialCamera_State` と `_OmniRadialCamera_PreviousState` を 0 にします。

成分の意味は Omni の内部の約束です。独自の Shader では、`RadialInject.hlsl` の関数を通して読むことを勧めます。include の使い分けは [include ファイル](../includes/)、写像は [Radial の写像](../../../docs/concepts/radial-mapping/) を見てください。

<!-- members -->

### _OmniRadialCamera_State

カメラの状態です。x は、このカメラで Radial の写像が有効なら 1、無効なら 0 です（ワープが無効、写像しない、Radial の対象が無い、ワープの中心を決められない、のどれかなら 0）。y、z、w は Omni の内部の印（ラベルの表示、視点の扱い、カメラ相対のインスタンスのデータを使うか）です。

### _OmniRadialCamera_Curve

写像の曲線です。x = S（常に 1。Shader は読みません）、y = 写像するか（`OMNI_RADIAL_MODE_SMOOTHLOG` の 0 か、`OMNI_RADIAL_MODE_IDENTITY` の −1）、z = 予約（0。Shader は読みません）、w = log2(M)（View Scale の M の 2 を底とする対数）です。

### _OmniRadialCamera_EyeRelative

視点の、Logical のワープの中心からの差（視点 − 中心）です。xyz が仮数、w が 2 の指数です。カメラ自身がワープの中心のとき（`RadialCamera` を付けたカメラ）は 0 です。

### _OmniRadialCamera_OriginRelative

Logical の world の原点の、ワープの中心からの差（原点 − 中心）です。xyz が仮数、w が 2 の指数です。State の x が 0 のときは 0 です。

### _OmniRadialCamera_ViewRow0

world から view への回転の 1 行目です。平行移動を含みません。Unity の view と同じ Z の反転を含みます。

### _OmniRadialCamera_ViewRow1

world から view への回転の 2 行目です。

### _OmniRadialCamera_ViewRow2

world から view への回転の 3 行目です。カメラの前の向きの逆を表します。

### _OmniRadialCamera_PreviousState

前に書いた `_OmniRadialCamera_State` です。x が 0 の間（初めのフレーム、Radial でない状態からの切り替わり）は、Object の MotionVectors は速度を 0 にします。

### _OmniRadialCamera_PreviousCurve

前に書いた `_OmniRadialCamera_Curve` です。

### _OmniRadialCamera_PreviousEyeRelative

前に書いた `_OmniRadialCamera_EyeRelative` です。

### _OmniRadialCamera_PreviousOriginRelative

前に書いた `_OmniRadialCamera_OriginRelative` です。ワープの中心が動いても、今と前の原点の差から、前のフレームの位置を絶対の座標へ戻さずに求めます。

### _OmniRadialCamera_PreviousViewRow0

前に書いた `_OmniRadialCamera_ViewRow0` です。

### _OmniRadialCamera_PreviousViewRow1

前に書いた `_OmniRadialCamera_ViewRow1` です。

### _OmniRadialCamera_PreviousViewRow2

前に書いた `_OmniRadialCamera_ViewRow2` です。

### _RadialGlobal_UseRadialTransform

Radial への対応を表す、Shader のプロパティです。Omni は、Shader の Properties がこの名前を宣言しているかで、Radial に対応した Shader かを判定します（`RadialMaterialContract`）。Material に古い値が残っているだけでは対応とみなしません。値は Omni が Renderer ごとに設定します。1 は Logical の精密な変換、−1 は通常の Unity の Transform とカメラの packet を合わせる変換、0 は写像しない（Unity の標準の変換）です。Properties の既定値は 0 にします。

### OMNI_RADIAL_MODE_SMOOTHLOG

写像する描画のモードの値（0.0）です。`RadialWarp.hlsl` で定義します。C# の `RadialGlobalRenderSystem` の値と一致します。

### OMNI_RADIAL_MODE_IDENTITY

写像しない描画のモードの値（−1.0）です。ワープが無効なとき、`RadialCamera` が無いとき、平行投影のときに Omni が配ります。`RadialWarp.hlsl` で定義します。

### OMNI_RADIAL_HOOKS_DISABLED

定義すると、`RadialVertexHooks.hlsl` が URP の頂点の関数を置き換えず、その Pass は標準の URP の頂点の変換で描きます。`RadialVertexInject.hlsl` は ShadowCaster と Meta の Pass で、`RadialWarpNode.hlsl` は ShadowCaster、Meta、Decal Projector の Pass と Fullscreen の Graph で、自動で定義します。独自の Pass を物理の空間のまま描くときは、include の前に定義します。

### OMNI_RADIAL_EXPECT_ACTIVE

`RadialVertexInject.hlsl` の前に定義すると、URP の `Core.hlsl` より前に include したときにコンパイルエラーにします。`Core.hlsl` が無いと頂点のフックは黙って無効になるので、include の順の誤りを見つけるために定義します。同梱の Radial 版の Shader（例: `Lit.shader`）は、`RadialVertexInject.hlsl` の直前で `1` を定義しています。
