# Category

- node
- node_internal

## Object

|| node | node_internal |
| --- | --- | --- |
| Alias | N | I |
| Node type constructor | node: *t* → Type | node_internal: *t* → (node_level: pos) → Type |
| Node list type constructor | node_list: *t* → Type | node_list_internal: *t* → (node_list_level: nat) → Type |
| Node level getter | get_level: *t* → (node t) → pos | get_level: *t* → (node_level: pos) → (node_internal t pos) → (r:pos{r = node_level}) |
| Empty node list constructor | *t* → (_: node_list t{Nil? _}) | *t* → (node_list_internal 0) |
| Leaf node constructor | | |

## Functor

|| node → node_internal | node_internal → node | note |
| --- | --- | --- | --- |
| Node mapping | to_node_inverse: *t* → (n: N.node t) → (I.node_internal t (N.get_level n) ) | to_node: *t* → (node_level: pos) -> (I.node_internal t node_level) -> (N.node t) | fully faithful functors pair |

## Axiom



## Reference link

- [Category (mathematics)](https://en.wikipedia.org/wiki/Category_(mathematics))
- [Functor](https://en.wikipedia.org/wiki/Functor)
- [Full and faithful functors](https://en.wikipedia.org/wiki/Full_and_faithful_functors)