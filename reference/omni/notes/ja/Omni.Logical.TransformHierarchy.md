---
description: "Logical Transform の階層の規則を判定する静的クラスです。"
---
`TransformHierarchy` は、[Logical Transform](../logical-transform/) の階層の規則を判定します。Omni の Runtime と Editor は、すべてこの判定を使います。

規則: Logical Transform に Logical Transform の祖先がある場合、それは**直接の親**でなければなりません。

| 階層 | 扱い |
| --- | --- |
| Logical Transform → 通常の Transform → 通常の Transform | 可 |
| Logical Transform → Logical Transform（直接の親子） | 可 |
| Logical Transform → 通常の Transform → Logical Transform | **禁止** |

禁止の配置では、間の通常の Transform のローカルの変換が Logical の合成から抜けます。一方、Unity の階層はそれを適用するので、Logical の値と描画の位置がずれます。禁止の配置にある Logical Transform は、親との合成をやめ、自分の値だけで位置を決めます。詳しくは [Logical 座標と Proxy](../../../docs/concepts/logical-coordinates/) の「階層の規則」を見てください。

<!-- members -->

### IsValid

`logicalTransform` の階層が規則を満たせば `true` を返します。満たさないときは `false` を返し、直接の親ではない位置にある祖先の Logical Transform を `offendingAncestor` に入れます。`logicalTransform` が `null` なら `true` です。

### IsRoot

`logicalTransform` が Logical の合成の起点なら `true` を返します。直接の親が無いか、直接の親に Logical Transform が無ければ起点です。禁止の配置でも `true` です。`logicalTransform` が `null` なら `false` です。

### DescribeViolation

違反の内容と直し方を、人が読む文章で返します。文章は Omni の表示言語に合わせます。`null` の引数は、名前を「(不明)」として扱います。
