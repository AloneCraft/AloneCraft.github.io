---
description: "付けた MeshRenderer を Scene BRG の自動の収集から外し、通常の Radial の MeshRenderer の経路で描く印の Component です。"
---
`RadialSceneBatchRendererExclusion` は、同じ GameObject の MeshRenderer を [Scene BRG](../../../docs/guides/scene-brg/) に載せず、通常の Radial の MeshRenderer の経路に残す印です。設定の項目はありません。外れるのはその Renderer だけで、Scene 全体の Scene BRG は止まりません。

- Animator が `material.*` を書くなど、Omni 以外が MaterialPropertyBlock を設定する Renderer に付けます。MaterialPropertyBlock は保ちます。
- 印が無くても、Omni 以外の MaterialPropertyBlock を持つ Renderer、永続的な Asset ではない Material を使う Renderer、`RadialIdentityWarp` を付けた Renderer は、自動で外します。
- 外した Renderer は、Console に 1 度だけ `[Omni][Scene BRG]` で始まるメッセージを出します。
- 有効と無効を切り替えると、Scene BRG を作り直します。

<!-- members -->

### RegisterUnmappedSceneMaterial

永続的な Material へ戻せない Scene の Material を判定する関数を登録します。関数が `true` を返したスロットを持つ MeshRenderer は、Scene BRG から外します。Omni の Editor が起動時に登録します。登録できる関数は 1 つだけで、呼ぶと前の関数を置き換えます。利用者のコードから呼ぶと Omni の判定が無効になるので、通常は呼びません。

### RegisterExternalMaterialOwnership

Scene BRG に載せない Material を判定する関数を登録します。関数が `true` を返す Material を 1 つでも使う MeshRenderer は、Scene BRG から外します。登録できる関数は 1 つだけで、呼ぶと前の関数を置き換えます。`null` を渡すと登録を外します。
