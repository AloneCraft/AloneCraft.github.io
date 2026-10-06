---
description: "Play 中や Player で RadialCamera の有効と無効を切り替え、同じ視点で標準描画と対数描画を見比べる Component です。"
---
`RadialCameraToggle` は、`RadialCamera` を持つ Camera に付けて使います。画面のボタン、キー、API のどれからでも、[RadialCamera](../radial-camera/) の有効と無効を切り替えます。

- 切り替えるのは `RadialCamera.enabled` だけです。無効のときは、Radial の Material が恒等の写像で描かれ、標準描画と同じ見た目になります。
- 対数描画に切り替えた Scene で使います。`RadialCamera` が有効な間、Radial に対応しない Material を持つ、`Omni.Logical.Transform` の下の Renderer は表示されません。標準の Material のままの Scene では、先に Omni Manager で対数描画に切り替えてください。
- ボタンは Play 中だけ作ります。Edit Mode では作りません。

使い方は [標準描画と対数描画を切り替える](../../../docs/getting-started/switch-rendering/) を見てください。

```csharp
using Omni;
using UnityEngine;

var toggle = Camera.main.GetComponent<RadialCameraToggle>();
toggle.RadialEnabledChanged += enabled => Debug.Log($"Radial: {enabled}");
toggle.SetRadialEnabled(false); // 標準描画にする
```

<!-- members -->

### m_Target

切り替える `RadialCamera` です。空なら、同じ GameObject の `RadialCamera` を使います。

### m_ShowButton

Play 中に、画面の左上に切替のボタンを出すかどうかです。ボタンには「対数レンダリング: ON」または「対数レンダリング: OFF」と出ます。対象が無いときは「RadialCameraが見つかりません」と出て、押せません。

### m_Theme

ボタンの UI Toolkit のテーマ（`ThemeStyleSheet`）です。文字と背景の色は、テーマによらず読める色を指定しています。

### m_SortingOrder

ボタンのパネルの並び順です。値が大きいほど、ほかの UI Toolkit の画面より手前に出ます。

### m_ToggleKey

押すたびに切り替えるキーです。`None` ならキーを使いません。この項目は、プロジェクトに Input System（`com.unity.inputsystem`）があるときだけあります。

### RadialEnabledChanged

有効と無効が変わったときに、新しい状態（`true` が対数描画）で呼ばれます。状態が変わらない呼び出しでは呼ばれません。

### Target

切り替える `RadialCamera` です。`m_Target` が空なら、同じ GameObject の `RadialCamera` を探して覚えます。見つからなければ `null` です。

### RadialEnabled

対象の `RadialCamera` が有効（対数描画）なら `true` です。対象が無いときは `false` です。

### SetRadialEnabled

`true` で対数描画、`false` で標準描画にします。対象の `RadialCamera` があれば `true` を返し、無ければ何もせずに `false` を返します。

### Toggle

対数描画と標準描画を入れ替えます。対象の `RadialCamera` が無ければ `false` を返します。
