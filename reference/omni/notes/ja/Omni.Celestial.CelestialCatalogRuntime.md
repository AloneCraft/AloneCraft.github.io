---
description: "Resources に置いた共有の CelestialCatalogAsset を、検証済みの Snapshot として渡します。"
---
`Resources` に置いた共有の [CelestialCatalogAsset](../celestial-catalog-asset/) を読み、検証済みで変わらない [CelestialCatalogSnapshot](../celestial-catalog-snapshot/) として渡す静的なクラスです。同梱のデモ（EarthToMoon、SolarSystem）は、天体の値をここから読みます。

```csharp
using Omni.Celestial;

if (CelestialCatalogRuntime.TryGetSnapshot(out CelestialCatalogSnapshot catalog, out string error) &&
    catalog.TryGet("Moon", out CelestialBodySnapshot moon))
{
    // moon.Physical.DiameterMeters などを使う
}
else
{
    Debug.LogError(error);
}
```

:::caution[結果の再利用]
同じアセットで `Revision` が同じ間は、前回の結果（成功の Snapshot、または失敗のエラー）をそのまま返します。Editor でアセットの値を変えたときは、`Revision` を増やすか、`InvalidateCacheForEditor` を呼んでください。
:::

<!-- members -->

### ResourcePath

Player で読む `Resources` からの相対パス（`"Omni/CelestialCatalog"`）です。

### AssetPath

共有のカタログのアセットのパス（`"Assets/Omni/Resources/Omni/CelestialCatalog.asset"`）です。

### TryGetSnapshot

`Resources.Load` で共有のカタログを読み、検証済みの Snapshot を返します。アセットが無いとき、または内容が不正なときは `false` を返し、`snapshot` は `null`、`error` に理由が入ります。成功したときの `error` は空の文字列です。`error` の前置きの文は日本語の固定の文です。同じアセットで改訂番号が同じなら、前回の結果を返します。

### GetSnapshotOrThrow

`TryGetSnapshot` と同じ方法で Snapshot を返します。失敗したときは、理由を含む `InvalidOperationException` を投げます。

### InvalidateCacheForEditor

覚えている結果を捨て、次の `TryGetSnapshot` でアセットを読み直させます。Editor でだけ使えます（`UNITY_EDITOR`）。改訂番号を変えずにアセットを更新した直後に呼びます。
