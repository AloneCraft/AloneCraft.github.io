---
description: "焼いたデータを使えたかを、シーケンス全体で 1 か所に集める Component です。"
---
焼いたデータを使えたかを、シーケンス全体で 1 か所に集める Component です。`PlayableDirector` と同じ GameObject に付けます。

- [PoseBakeTrack](../pose-bake-track/) と [LightBakeTrack](../light-bake-track/) は、Director の GameObject からこの Component を見つけ、照合する GUID とハッシュ、`ExternalOverride` を受け取ります。毎フレーム、チャンネルごとに「焼いたデータを適用できたか」を報告します。
- 演出を動かす側は、この 1 か所だけを見て、焼いたデータを使うか、代わりの計算で動かすかを決めます。カメラと光源で判断を分けると、カメラだけ焼いたデータ、光源だけ別の計算、のような食い違った絵になるためです。`AllChannelsApplied` が `false` の間は、すべての対象を同じ方法で動かしてください。

この Component が無くても Track は動きます。その場合、Scene、シーケンス、内容のハッシュの照合はせず、報告もしません。詳しくは [Sequencer と Pose Bake](../../../docs/guides/sequencer/) を見てください。

<!-- members -->

### m_ExpectedSceneGuid

照合する Scene の GUID です。空なら照合しません。

### m_ExpectedSourceGuid

照合するシーケンス（Timeline のアセット）の GUID です。空なら照合しません。

### m_ExpectedContentHash

照合する内容のハッシュです。空なら照合しません。

### ExternalOverride

焼いた姿勢の適用を、外の都合で止めます。`PoseBakeRejection.None` 以外を設定すると、Track はどのチャンネルにも書かず、この値を理由として報告します。`None` で解除します。利用者がカメラを操作している間の `UserCameraOverride` などに使います。

### ExpectedSceneGuid

照合する Scene の GUID を返します。

### ExpectedSourceGuid

照合するシーケンスの GUID を返します。

### ExpectedContentHash

照合する内容のハッシュを返します。

### ConfigureExpectations

照合する Scene の GUID、シーケンスの GUID、内容のハッシュを設定します。空の値はその項目を照合しないことを表します。シリアライズするフィールドに書きますが、Editor で Asset の変更として保存するのは呼び出す側です。

### ReportChannel

チャンネル 1 つの今の状態を記録します。Track のミキサーが毎フレーム呼びます。同じ識別子（大文字小文字を区別）の報告があれば置き換え、無ければ追加します。

### AllChannelsApplied

報告したチャンネルが 1 つ以上あり、そのすべてに焼いたデータを適用できたときに `true` を返します。報告が 1 つも無いときは `false` です。

### FirstRejection

適用できなかったチャンネルのうち、最初に報告されたものの理由を返します。すべて適用できたとき、または報告が無いときは `PoseBakeRejection.None` です。

### ChannelStates

報告したチャンネルごとの直近の状態を返します。診断の表示用です。各要素（`ChannelState`）は、識別子 `TargetPath`、適用できたか `Applied`、理由 `Rejection` を持ちます。

### ResetReports

報告をすべて捨てます。演出の切替やシークの直前に呼び、前のシーケンスの状態を残さないようにします。報告は自動では消えないので、Track が動かなくなったチャンネルの状態も、呼ぶまで残ります。
