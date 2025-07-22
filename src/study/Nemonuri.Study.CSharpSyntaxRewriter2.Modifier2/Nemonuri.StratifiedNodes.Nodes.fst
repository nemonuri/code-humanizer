module Nemonuri.StratifiedNodes.Nodes

module I = Nemonuri.StratifiedNodes.Internals

//--- type definitions ---
type node (t:eqtype) =
  | SNode : 
      level:pos ->
      internal:(I.node_internal t level) ->
      node t

let node_list (t:eqtype) = list (node t)
//---|

//--- theory members ---
let to_node #t #level (node_internal:I.node_internal t level) : Tot (node t) =
  SNode level node_internal

let to_node_list #t #max_level 
  (node_list_internal:I.node_list_internal t max_level) 
  : Tot (node_list t) =
  I.select node_list_internal (fun ni -> to_node ni)
//---|

//--- (node t) members ---
let get_level #t (node:node t) : Tot pos = node.level

let get_value #t (node:node t) : Tot t = node.internal.value

let get_children #t (node:node t) : Tot (node_list t) =
  to_node_list node.internal.children
//---|

  