module Nemonuri.StratifiedNodes.Factories

module L = FStar.List.Tot
module I = Nemonuri.StratifiedNodes.Internals
module N = Nemonuri.StratifiedNodes.Nodes
module C = Nemonuri.StratifiedNodes.Children
module Common = Nemonuri.StratifiedNodes.Common

let create_leaf_node_from_value (t:eqtype) (value:t)
  : Pure (N.node t) 
         (requires True) 
         (ensures fun node -> (N.get_value node = value) && (N.is_leaf node))
  =
  N.to_node (I.SNode (I.SNil) value)

let with_value #t (node:N.node t) (value:t)
  : Pure (N.node t) 
         (requires True) 
         (ensures fun node -> (N.get_value node = value))
  =
  let internal = node.internal in
  let new_internal = I.SNode internal.children value in
  N.to_node new_internal

//#push-options "--query_stats"
let with_children #t (node:N.node t) (children:N.node_list t)
  : Pure (N.node t) 
         (requires True)
         (ensures fun node -> 
           ((I.get_max_level node.internal.children) = (I.get_max_level (N.to_node_list_inverse children))) &&
           (node.internal.children = (N.to_node_list_inverse children)) (*&&
           ((N.get_list_level (N.get_children node)) = (N.get_list_level children)) &&
           ((N.get_children node) = children)*)
         )
  =
  //N.lemma_to_node_list_is_bijection t;
  let internal = node.internal in
  let new_internal = I.SNode (N.to_node_list_inverse children) internal.value in
  N.to_node new_internal
//#pop-options