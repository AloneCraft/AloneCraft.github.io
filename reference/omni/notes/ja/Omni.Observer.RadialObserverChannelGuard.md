---
description: "観察の枠の Virtual Camera を、その枠の Brain だけが拾うように、Cinemachine の Channel を分けます。"
---
観察の枠の Virtual Camera を、その枠の `CinemachineBrain` だけが拾うようにします。`CinemachineBrain` の Channel Mask は既定ですべてのチャンネルを含むので、Scene の別の Brain（ゲームの主カメラなど）が観察の Virtual Camera を拾うと、主カメラが観察の位置へ動いてしまいます。これを防ぐためのコンポーネントです。

有効になったとき、次を行います。

1. 枠の番号に対応する専用のチャンネル（Channel01〜05）を、Virtual Camera の Output Channel に設定します。
2. 枠の Brain の Channel Mask を、そのチャンネルだけにします。
3. その時点で Scene にある他の Brain（無効な GameObject の Brain を除く）の Channel Mask から、そのチャンネルを外します。

無効になったとき、3 で外した Brain にチャンネルを戻します。枠の Brain と Virtual Camera の設定は戻しません。

:::caution
有効になった後に作られた Brain は対象外です。その Brain の Channel Mask から、観察のチャンネルを外してください。
:::

Cinemachine と Input System が無い環境では、コンパイルの対象から外れます。詳しくは [Radial 観察カメラ](../../../docs/guides/observer-camera/) を見てください。

<!-- members -->

### m_Slot

チャンネルを決める枠（[RadialObserverSlot](../radial-observer-slot/)）です。`m_Slot`、`m_Brain`、`m_VirtualCamera` のどれかが未設定なら、何もしません。

### m_Brain

この枠の Camera の `CinemachineBrain` です。Channel Mask を枠の専用のチャンネルだけにします。

### m_VirtualCamera

この枠の `CinemachineCamera` です。Output Channel を枠の専用のチャンネルにします。

### ChannelForSlot

枠の番号に対応する観察の専用のチャンネルを返します。枠 0 が Channel01、枠 4 が Channel05 です。番号は 0〜4 に丸めます。

### Configure

枠、Brain、Virtual Camera をコードから設定します。Prefab の生成とテスト用です。チャンネルの設定は、次に有効になったときに行います。
