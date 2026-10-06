---
description: "RadialCamera が見ている変形後の世界を、別のカメラ（観察の枠）から外側へ引いて見るための Rig です。"
---
`RadialCamera` が見ている「変形後の世界」を、別のカメラから外側へ引いて見るための Rig です。観察の枠の Camera を、ソースの `RadialCamera` をワープの中心とする観察者として登録し、Scene View の比較表示と同じ約束で描かせます。対象だけを中心 C の周りへ圧縮し、観察の Camera の視点は物理の位置のままにします。

- 通常は GameObject > Omni > Radial Observer Camera で、Prefab ごと置きます。構成、操作、守ることは [Radial 観察カメラ](../../../docs/guides/observer-camera/) を見てください。
- Rig の原点が中心 C に当たる有限の描画の空間です。Rig 自体は動きません。
- カメラまでの距離は、境界球などから求めず、`DistanceInScale` × S の固定の距離です。S は Radial の尺度（`RadialSettings.RadialScale`）で、現在は定数 1 です。
- 既定では Play Mode でだけ動きます。Edit Mode で動かすには `RunInEditMode` をオンにします。

枠の配置は [ObserverLayoutPreset](../observer-layout-preset/) と [ObserverCorner](../observer-corner/)、枠ごとの設定は [RadialObserverSlot](../radial-observer-slot/) を見てください。

<!-- members -->

### m_Source

ワープの中心と曲線を借りる `RadialCamera` です。未設定のときは MainCamera の `RadialCamera`、無ければ Scene の中の `RadialCamera` を探します（自動で探すのは 0.5 秒に 1 回です）。見つからないときは Console に警告を出します。ソースが無効なときは、警告と画面の右下の表示で知らせます。どちらの場合も枠を消します。

### m_DistanceInScale

中心 C から観察のカメラまでの距離を、Radial の尺度 S の何倍にするかです（5〜200）。S は現在 1 なので、描画の空間での距離はこの値と同じになります。

### m_Layout

枠の並べ方です。値は [ObserverLayoutPreset](../observer-layout-preset/) を見てください。

### m_Corner

枠を寄せる画面の隅です。値は [ObserverCorner](../observer-corner/) を見てください。

### m_OrbitWipeArea

`OrbitWipe` の周回の枠の一辺です。画面の幅・高さに対する比で、0.05〜1 に丸めます。

### m_GridArea

`FixedGrid` と `Combined` が占める範囲の一辺です。画面の幅・高さに対する比で、0.05 以上に丸めます。上限は `FixedGrid` で 1、`Combined` で 0.8 です（`Combined` は横幅がこの値の 1.25 倍になるため）。

### m_BaseDepth

観察の枠の `Camera.depth` の基準です。各枠の depth は、この値に枠の番号を足した値になります。主カメラの depth より大きくします。

### m_IncludeUnwarpedCullingVolume

オンにすると、観察のカリングに通常の視錐台も含めます。Scene View の比較表示と同じ設定です。登録の時点で読むので、変更はソースの登録をやり直したときに効きます。

### m_RunInEditMode

オンにすると、Edit Mode（Play していないとき）でも枠を登録して表示します。オフの Rig は Edit Mode で何も変えません。

### m_Focus

すべての枠が共有する注視点の Transform です。Rig の直下の子にします。Rig は毎フレーム、この Transform の `localPosition` を、平滑化しながら中心 C（Rig の原点）へ動かします。未設定のまま動かすと Console にエラーを出します。

### m_Slots

観察の枠（[RadialObserverSlot](../radial-observer-slot/)）の配列です。Rig はこの配列の枠の Camera を登録し、配置し、有効・無効を切り替えます。

### Focus

注視点の Transform（`m_Focus`）を返します。

### ActiveSource

今観察しているソースの `RadialCamera` を返します。見つからない間と、ソースが無効な間は `null` です。

### DistanceInScale

中心からカメラまでの距離を、S の何倍にするかを取得・設定します。設定した値は、Inspector の範囲（5〜200）には丸めません。0 以下や有限でない値は、枠を作るときに 40 として扱います。

### RunInEditMode

Edit Mode でも動かすかを取得・設定します。Edit Mode でオフにすると、次の更新で登録を外して枠を消します。

### Layout

枠の並べ方を取得・設定します。次の更新から配置に反映します。

### Corner

枠を寄せる画面の隅を取得・設定します。次の更新から配置に反映します。

### TryGetFrame

直近の更新で求めた枠取りの結果（`ObserverFrame`）を返します。ソースが見つからない、またはソースの Logical の位置を解決できなかったときは `false` を返します。`ObserverFrame` は、注視点 `Focus`（常に原点）、カメラの距離 `FixedDistance`（`DistanceInScale` × S）、近い・遠いクリップを決めるための半径 `BoundingRadius`（距離の 0.4 倍）を持ちます。

### Configure

注視点、枠の配列、ソースの `RadialCamera` をコードから設定します。Prefab の生成とテスト用です。`slots` が `null` なら空の配列にします。`source` を省略すると、ソースを自動で探します。
