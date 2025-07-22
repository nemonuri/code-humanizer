module Nemonuri.StratifiedNodes.Children

module L = FStar.List.Tot
module N = Nemonuri.StratifiedNodes.Nodes

//--- theory members ---
let is_child #t (parent_node:N.node t) (node:N.node t) 
  : Pure bool (requires True) (ensures fun _ -> N.node_level_is_greater_than_levels_of_nodes_in_children t)
  =
  N.lemma_node_level_is_greater_than_levels_of_nodes_in_children t;
  L.contains node (N.get_children parent_node)
//---|

//--- asserts ---
let test_is_child1 = assert (
  forall (t:eqtype) (parent_node:N.node t) (node:N.node t).
    (is_child parent_node node) ==> (parent_node.level > node.level))
//---|

//--- propositions ---

//---|