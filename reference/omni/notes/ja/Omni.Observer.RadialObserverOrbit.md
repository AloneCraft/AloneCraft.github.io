---
description: "観察の枠 1 つの Cinemachine の Virtual Camera を、距離、角度、クリップの範囲について毎フレーム動かします。"
---
観察の枠 1 つの Cinemachine の Virtual Camera（`CinemachineCamera` と `CinemachineOrbitalFollow`）を動かします。Cinemachine（`com.unity.cinemachine` 3.0.0 以上）と Input System（`com.unity.inputsystem` 1.0.0 以上）を使うので、どちらかが無い環境ではコンパイルの対象から外れます。

毎フレーム次を行います。

- 距離: [RadialObserverRig](../radial-observer-rig/) の枠取りの固定の距離へ、対数の空間で平滑化しながら近づけ、OrbitalFollow の `Radius` に書きます。
- レンズ: 画角に枠の `FieldOfView` を書きます。近いクリップと遠いクリップは、距離と枠取りの半径から決めます。
- 角度: 固定の枠では、枠の `Yaw` と `Pitch` を水平軸と垂直軸に書きます。周回の枠では、自動の周回とマウスの操作を軸に足します。

Rig が枠取りの結果を持たない間（ソースが無いときなど）は、何も書きません。このコンポーネントは `[ExecuteAlways]` を持たないので、Edit Mode では動きません。操作の説明は [Radial 観察カメラ](../../../docs/guides/observer-camera/) を見てください。

<!-- members -->

### m_Rig

枠取りの結果を読む [RadialObserverRig](../radial-observer-rig/) です。

### m_Slot

この Virtual Camera が属する [RadialObserverSlot](../radial-observer-slot/) です。種類、角度、画角と、マウスの位置を判定する Camera をここから読みます。

### m_VirtualCamera

レンズ（画角、近いクリップ、遠いクリップ）を書く `CinemachineCamera` です。未設定ならレンズは書きません。

### m_Orbital

距離と角度を書く `CinemachineOrbitalFollow` です。未設定なら何もしません。

### m_SpinDegreesPerSecond

周回の枠の自動の周回の速さ（度／秒）です。周回の枠だけで使います。時間は `Time.unscaledDeltaTime` で測るので、`Time.timeScale` の影響を受けません。

### m_ResumeDelaySeconds

周回の枠で、最後の操作から自動の周回に戻るまでの時間（秒）です。

### m_DragDegreesPerPixel

周回の枠で、右ボタンのドラッグ 1 ピクセルあたりの回転の角度（度）です。ドラッグは、マウスが枠の内側にあるときだけ効きます。

### m_ZoomPerScrollUnit

周回の枠で、ホイールのスクロール量 1 あたりの拡大・縮小の強さです。OrbitalFollow の動径の軸の値に `exp(−スクロール量 × この値)` を掛けます。

### m_DistanceSmoothSeconds

距離を目標へ近づける平滑化の時間（秒、0.01 以上）です。距離の対数に対して SmoothDamp を掛けるので、桁の違う距離の間も滑らかに動きます。

### Configure

Rig、枠、`CinemachineCamera`、`CinemachineOrbitalFollow` をコードから設定します。Prefab の生成とテスト用です。
