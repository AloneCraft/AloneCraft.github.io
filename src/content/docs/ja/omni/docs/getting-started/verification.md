---
title: Verification Hub で確かめる
description: 導入したプロジェクトで、描画・深度・尺度・VFX・カメラを同じ操作で確かめる。
sidebar:
  order: 3
---

`Assets/Omni/Verification/Hub.unity` は、Omni を導入したプロジェクトで、描画、深度、尺度、VFX、ECS／BRG、カメラの状態を同じ操作で確かめる入口です。

## 必要なもの

- Hub は、Omni の専用の URP 資産（`Verification/Settings/OmniVerification_URP.asset`）で描きます。Play の間だけ有効な URP 資産をこれに切り替え、Play を終えると元に戻します。プロジェクトの Quality と Graphics の設定は書き換えません。
- ECS の経路には `com.unity.entities` が必要です。無い環境では、これらの経路は理由付きの `Unsupported` になります。
- VFX を描く経路には `com.unity.visualeffectgraph` が必要です。無い環境では、これらの経路は理由付きの `Unsupported` になります。

## 使い方

1. `Hub.unity` を開き、Play Mode に入ります。
2. **Case** で確かめる対象を選びます。系列は `DEP`、`SHD`、`DEC`、`BUF`、`MOT`、`ALP`、`LIT`、`SCL`、`VFX`、`ECS`、`CAM`、`ED` です。
3. **Preset** で尺度、距離、カメラの条件、固定の時間刻みを切り替えます。
4. **Execution Path** で GameObject、ECS／Omni BRG、VFX の経路を切り替えます。
5. **Renderer Request** で Forward と Forward+ を比べます。状態の表示には、要求した Renderer と、実際に選ばれた Renderer が別々に出ます。
6. **Play**、**Pause**、**Rewind**、**Step** で、決定的な時計を操作します。
7. **A** は必要な機能をすべて有効にし、**B** は選んだ診断の機能だけを無効にします。同じ時刻で比べられます。
8. **Capture** で、今の画像と結果の JSON を保存します。

結果の保存先は、Editor ではプロジェクトの `Logs/OmniRenderingRegression`、Player では `Application.persistentDataPath/OmniRenderingRegression` です。

## 結果の読み方

| 状態 | 意味 |
| --- | --- |
| `Passed` | その Case で決めた独立の測定が、許容差の中にある |
| `Unsupported` | 選んだ Renderer や Execution Path を、今の環境で確かめられない |
| `Failed` | fixture、描画のサンプル、Renderer の選択、測定値のどれかが条件を満たさない |

:::caution
画像だけで判断しないでください。`status`、`reason`、要求した Renderer、実際の Renderer、描画のサンプル数（`renderedSampleCount`）も確かめます。背景だけが一致している画像は、合格の根拠になりません。
:::

## Player で確かめる

Hub を Player に入れるときは、ビルドの前に次を行います。

1. Omni Manager で対数描画に切り替えます（[標準描画と対数描画を切り替える](../switch-rendering/)）。
2. Omni Manager の「シェーダー」タブの「Camera MotionVectors / SSAO」の欄で、Camera MotionVectors と SSAO の Shader が Omni 版につながっていることを確かめます。手順 1 の切替が自動でつなぎます。ただし、URP の標準以外の値が設定されていた場合は上書きしません。その場合は、Manager の上部の警告を確かめ、欄の中の「Omni Shaderを導入」を押します。つながっていないと、RadialCamera を含む Scene のビルドは理由を示して止まります。
3. Project Settings の Quality で品質レベルを 1 つ追加し、Render Pipeline Asset に `Verification/Settings/OmniVerification_URP.asset` を設定します。URP は、ビルド対象の品質レベルの URP 資産だけを見て Shader variant を間引きます。これが無いと、Hub が Play 中に切り替える専用の資産の機能（Decal など）の variant が間引かれ、Player で Decal がマゼンタになります。
4. Build Settings に `Hub.unity` を入れてビルドします。

Player では、画面の左上のパネルから操作します。

## 機能ごとの Scene

`Verification/Functions` には、Hub の結果を小さい構成へ切り分けるための Scene があります。一覧は `Functions/FunctionsVerification.unity` から開けます。各 Scene は単独で開き、Scene View と Play の両方で確かめられます。総合的な確認は Hub から始めてください。
