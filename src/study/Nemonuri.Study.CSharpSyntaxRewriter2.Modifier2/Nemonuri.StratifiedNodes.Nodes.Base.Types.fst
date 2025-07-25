module Nemonuri.StratifiedNodes.Nodes.Base.Types

module L = FStar.List.Tot
module I = Nemonuri.StratifiedNodes.Internals

//--- type definitions ---
type node (t:eqtype) =
  | SNode : 
      level:pos ->
      internal:(I.node_internal t level) ->
      node t

let node_list (t:eqtype) = list (node t)
//---|