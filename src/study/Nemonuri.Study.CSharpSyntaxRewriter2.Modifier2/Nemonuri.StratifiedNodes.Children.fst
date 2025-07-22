module Nemonuri.StratifiedNodes.Children

module L = FStar.List.Tot
module N = Nemonuri.StratifiedNodes.Nodes

//--- propositions ---

//---|

//--- theory members ---
let is_child #t (parent_node:N.node t) (node:N.node t) 
  : Pure bool (requires True) (ensures fun _ -> N.node_level_is_greater_than_levels_of_nodes_in_children t)
    //Tot (b:bool{N.node_level_is_greater_than_levels_of_nodes_in_children t}) 
    //if b then (N.get_level node) < (N.get_level parent_node) else true
  =
  N.lemma_node_level_is_greater_than_levels_of_nodes_in_children t;
  L.contains node (N.get_children parent_node)

(*
  let result = (L.contains node (N.get_children parent_node)) in
  if result then
    (
      N.lemma_node_level_is_greater_than_level_of_node_in_children parent_node node;
      true
    )
  else
    false
*)
//---|