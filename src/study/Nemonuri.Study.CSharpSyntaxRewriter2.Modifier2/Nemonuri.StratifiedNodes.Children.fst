module Nemonuri.StratifiedNodes.Children

module L = FStar.List.Tot
module N = Nemonuri.StratifiedNodes.Nodes

//--- propositions ---

//---|

let is_child #t (parent_node:N.node t) (node:N.node t) 
  : Tot (b:bool{if b then (N.get_level node) < (N.get_level parent_node) else true})
  =
  L.contains node (N.get_children parent_node)

