---
description: "CelestialCatalogAsset から作る、検証済みで読み取り専用の天体のカタログです。"
---
[CelestialCatalogAsset](../celestial-catalog-asset/) から作る、検証済みで読み取り専用の天体のカタログです。作った後は変わりません。Unity の Object を含みません。天体は、キーの順（序数の比較の昇順）に並べ直して持ちます。

同じ元の内容からは、必ず同じ `Fingerprint` を作ります。ほかのデータが、どのカタログの内容から作られたかを確かめるのに使えます。通常は [CelestialCatalogRuntime](../celestial-catalog-runtime/) か `CelestialCatalogAsset.TryCreateSnapshot` で得ます。

<!-- members -->

### FingerprintSchemaVersion

`Fingerprint` を求めるときの、内容の並べ方の版です（現在 2）。版が変わると、同じ内容でも `Fingerprint` が変わります。

### Revision

元のアセットの改訂番号です。

### Fingerprint

カタログの内容を表す SHA-256 の 16 進の文字列（小文字の 64 文字）です。並べ方の版、改訂番号、各天体のすべての値から求めます。

### Count

天体の数を返します。

### Bodies

天体（[CelestialBodySnapshot](../celestial-body-snapshot/)）の読み取り専用の一覧を返します。キーの昇順です。

### GetAt

キーの昇順の添字で天体を返します。添字が範囲の外なら `ArgumentOutOfRangeException` を投げます。

### TryGet

キーで天体を探します。比較は大文字小文字を区別します。見つからないとき、またはキーが `null` か空のときは `false` を返し、`body` は既定値です。

### TryCreate

改訂番号と天体のレコードを検証し、すべて有効なときだけ Snapshot を作ります。改訂番号は 1 以上、レコードは 1 件以上必要です。失敗したときは `false` を返し、`snapshot` は `null` です。`validation` には、見つけたすべてのエラーが入ります。検証の規則は [CelestialBodyAuthoring](../celestial-body-authoring/) を見てください。
