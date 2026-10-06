---
description: "天体の物理定数（直径、質量、自転軸の傾き、軌道要素）を持つ、唯一の元データのアセットです。"
---
天体の物理定数（直径、質量、自転軸の傾き、軌道要素）を持つ ScriptableObject です。天体の値の唯一の元データで、Prefab、Scene、描画のデータに半径、質量、軌道を複製しないための入れ物です。

- Assets > Create > Omni > Celestial > Catalog で作れます。
- 同梱のデモ（EarthToMoon、SolarSystem）は、`Assets/Omni/Resources/Omni/CelestialCatalog.asset` に置いた共有のカタログを、[CelestialCatalogRuntime](../celestial-catalog-runtime/) から読みます。
- 天体ごとの値は [CelestialBodyAuthoring](../celestial-body-authoring/) に、10 進の文字列で書きます。使うときは、必ず `TryCreateSnapshot` などで検証した [CelestialCatalogSnapshot](../celestial-catalog-snapshot/) にしてから読みます。

<!-- members -->

### m_Revision

内容を意図して変えたときに増やす改訂番号です。1 以上にします。[CelestialCatalogRuntime](../celestial-catalog-runtime/) は、同じアセットで改訂番号が変わらない間、前回の結果を使い続けます。

### m_Bodies

天体のレコード（[CelestialBodyAuthoring](../celestial-body-authoring/)）の配列です。1 件以上必要です。

### Revision

改訂番号を返します。

### TryCreateSnapshot

今の内容を検証し、すべて有効なときだけ、変わらない [CelestialCatalogSnapshot](../celestial-catalog-snapshot/) を作ります。失敗したときは `false` を返し、`snapshot` は `null` です。`validation` には、見つけたすべてのエラーが入ります。呼ぶたびに新しく検証して作ります。

### Validate

今の内容の検証だけを行い、結果（`CelestialCatalogValidationResult`）を返します。`IsValid` でエラーが無いかを、`Issues` でエラーの一覧を、`ToDisplayString()` で表示用の文字列を得られます。

### CreateSnapshotOrThrow

今の内容から [CelestialCatalogSnapshot](../celestial-catalog-snapshot/) を作ります。内容が不正なときは、すべてのエラーを含む `InvalidOperationException` を投げます。

### ConfigureForEditor

改訂番号と天体のレコードを、まとめて差し替えます。Editor でだけ使えます（`UNITY_EDITOR`）。渡した配列と各レコードは複製するので、呼んだ後に元の配列を変えてもアセットには影響しません。`bodies` が `null` なら空にします。アセットとして保存する処理（`EditorUtility.SetDirty` など）はしません。
