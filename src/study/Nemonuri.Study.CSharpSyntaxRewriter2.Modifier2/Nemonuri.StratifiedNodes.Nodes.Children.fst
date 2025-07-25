module Nemonuri.StratifiedNodes.Nodes.Children

module L = FStar.List.Tot
module Math = FStar.Math.Lib
module I = Nemonuri.StratifiedNodes.Internals
module T = Nemonuri.StratifiedNodes.Nodes.Base.Types
module E = Nemonuri.StratifiedNodes.Nodes.Equivalence
open Nemonuri.StratifiedNodes.Nodes.Base.Members
open Nemonuri.StratifiedNodes.Nodes.Bijections

//--- theory members ---
let get_children #t (node:T.node t) 
  : Pure (T.node_list t) True
    (ensures fun r -> E.are_equivalent_as_node_list r node.internal.children)
  =
  to_node_list node.internal.children

let get_children_length #t (node:T.node  t) 
  : Pure nat (requires True) (ensures fun r -> r = L.length (get_children node)) = 
  I.get_length node.internal.children
//---|

//--- private theory members ---

#push-options "--query_stats"
let get_child_at #t (node:T.node t) (index:nat)
  : Pure (T.node  t) 
    (requires (index < (get_children_length node)))
    (ensures fun r -> (
      let node2 = (L.index (get_children node) index) in
      r = node2)
    )
  =
  //assert (E.are_equivalent_as_node_list (get_children node) node.internal.children);
  let node_internal = I.get_item node.internal.children index in (
    //assume ((L.index (get_children node) index).level = (I.get_level node_internal));
    //assume ((L.index (get_children node) index).internal = node_internal);
    assume ( E.are_equivalent_as_node_list_entails_get_item_pair_are_equivalent_as_node
    (get_children node) (node.internal.children) index );
    //assert (E.are_equivalent_as_node (to_node node_internal) node_internal);
    to_node node_internal
  )
  // TODO: Internals 단계에서 구현하기
  //L.index (get_children nd) index
#pop-options 

//---|