---
description: "Assets/Omni/Runtime/Sharders の公開の include ファイルと、どれをどこで使うかです。"
---
`Assets/Omni/Runtime/Sharders` の公開の include ファイルです。通常の URP の Shader と Shader Graph は、対数描画への切替が変換するので、自分で include する必要はありません（[Shader Graph と独自の Shader](../../../docs/guides/shader-graph/)）。Radial に直接対応する Shader を手で書くときだけ、次のように使い分けます。

| 用途 | include |
| --- | --- |
| 手書きの URP の Shader の、MotionVectors 以外の Pass | `RadialVertexInject.hlsl` |
| 手書きの URP の Shader の、MotionVectors と XRMotionVectors の Pass | `RadialVertexInject.hlsl` の後に `RadialObjectMotionVectors.hlsl` |
| Shader Graph | 変換器が `RadialWarpNode.hlsl` の Custom Function を足す。手で include しない |

```hlsl
#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
#define OMNI_RADIAL_EXPECT_ACTIVE 1
#include "Assets/Omni/Runtime/Sharders/RadialVertexInject.hlsl"
// この後に Pass の実装を include する
```

Omni がその Shader を Radial に対応していると判定するには、Properties で `_RadialGlobal_UseRadialTransform` を宣言する必要があります（[グローバルのプロパティと define](../globals/)）。同梱の Radial 版の Shader（例: `Assets/Omni/Radial/Universal Render Pipeline/Shaders/Lit.shader`）が、Properties の宣言と include の順の実例です。

<!-- members -->

### RadialWarpNode.hlsl

Shader Graph の Custom Function の入口です。変換器が Graph に足す Custom Function ノードが、このファイルの関数を使います（[Custom Function](../custom-functions/)）。中で `RadialInject.hlsl` と `RadialVertexHooks.hlsl` を読み、Graph の頂点を Radial の投影につなぎます。Graph の include は全 Pass に展開されるので、ShadowCaster、Meta、Decal Projector、Fullscreen の Pass では、頂点のフックを自動で無効にします。深度を書く Pass（色、DepthOnly、DepthNormals）では画素ごとの深度を足し、深度テクスチャを使う Graph では Scene Depth と `SampleSceneDepth` を物理の深度を返すものに置き換えます。手書きの Shader からは include しません。

### RadialVertexInject.hlsl

手書きと生成した URP の Shader の、頂点の入口です。`RadialInject.hlsl` と `RadialVertexHooks.hlsl` をこの順に読みます。`Core.hlsl` と Material の入力（`LitInput.hlsl` など）の後、各 Pass の実装の前に include します。ShadowCaster と Meta の Pass では、`OMNI_RADIAL_HOOKS_DISABLED` を定義して頂点のフックを無効にします（物理の空間のまま描きます）。前に `OMNI_RADIAL_EXPECT_ACTIVE` を定義すると、`Core.hlsl` より前に include したときにコンパイルエラーにします。

### RadialInject.hlsl

Radial の共通の実装です。`RadialWarp.hlsl` を読み、カメラの packet（`_OmniRadialCamera_*`）と、Material ごとの `_RadialGlobal_*` の宣言、Object 空間からカメラ相対の写像した位置への変換、深度の復元の関数を持ちます。このファイルだけでは URP の関数を置き換えません。`_RadialGlobal_*` は `RadialGlobalPerMaterial` の CBUFFER に宣言します（Shader Graph、DOTS Instancing、Scene の BRG の経路ではそれぞれの宣言を使います）。

### RadialVertexHooks.hlsl

URP の頂点の入口を、Object 空間からカメラ相対の写像した clip 空間へつなぐフックです。`GetVertexPositionInputs`、`GetVertexNormalInputs`、`TransformObjectToHClip`、`TransformObjectToWorldNormal`、`TransformObjectToWorldDir`、`GetWorldSpaceNormalizeViewDir`、`GetWorldSpaceViewDir` をマクロで置き換えるので、それらを使う Pass の実装より前に読む必要があります。単独で include しないでください。`RadialInject.hlsl` が先に無いとコンパイルエラーになります。URP の `Core.hlsl` が無いとき、または `OMNI_RADIAL_HOOKS_DISABLED` が定義されているときは何も置き換えません。通常は `RadialVertexInject.hlsl` から読みます。

### RadialObjectMotionVectors.hlsl

今と前のフレームの Object の位置から MotionVectors を作る、MotionVectors の Pass の実装そのものです。`#pragma vertex vert` と `#pragma fragment frag` を含み、`vert` と `frag` を定義します。MotionVectors と XRMotionVectors の Pass で、`RadialVertexInject.hlsl` の後に `#include_with_pragmas` で読み、URP の `ObjectMotionVectors.hlsl` の代わりに使います。`_ALPHATEST_ON` では `_BaseMap`、`_BaseColor`、`_Cutoff` を使うので、Material の入力の宣言が要ります。初めのフレーム、Radial でない状態からの切り替わり、剛体のアンカーでは、偽の速度を作らず速度を 0 にします。通常の Forward の Pass には使いません。

### RadialWarp.hlsl

Radial の写像の数式だけを置くファイルです。写像は Smooth Log の 1 つだけで、`F(d) = S·log2(1 + d·M/S)`（S = 1）です。入力と出力は常に Logical のワープの中心からの相対のベクトルで、絶対の world 座標、カメラの位置、Unity の View 行列を扱いません。`OMNI_RADIAL_MODE_SMOOTHLOG` と `OMNI_RADIAL_MODE_IDENTITY` を定義します。`RadialInject.hlsl` が読むので、普通は直接 include しません。関数の引数の `S` と予約の引数は、呼び出す側の形を保つために残しているだけで、読みません。
