module Nemonuri.StratifiedNodes.Forall.Children

module L = FStar.List.Tot
module N = Nemonuri.StratifiedNodes.Nodes
module C = Nemonuri.StratifiedNodes.Children
module Common = Nemonuri.StratifiedNodes.Common
module D = Nemonuri.StratifiedNodes.Decreasers
module F = Nemonuri.StratifiedNodes.Factories
open FStar.FunctionalExtensionality
open Nemonuri.StratifiedNodes.Forall.Aggregating

//--- theory members ---

let for_all_root_and_children #t
  (root:N.node t) 
  (node_predicate:N.node t -> bool)
  : Tot bool =
  let selector : (C.ancestor_list_given_selector t bool) = (
    C.to_ancestor_list_given_selector t bool node_predicate
  ) in
  let selector_for_child : (C.ancestor_list_given_selector_for_child t bool root) = (
    C.to_ancestor_list_given_selector_for_child t bool root selector
  ) in
  C.aggregate_children_overload 
    root selector selector_for_child
    for_all_aggregator
    for_all_aggregator
    (Common.aggregated_identity bool)
    (for_all_continue_predicate t)
    []

//---|

//--- propositions ---

let node1_satisfies_for_all_root_and_children_entails_node2_satisfies_predicate #t
  (node1:N.node t) 
  (node2:N.node t)
  (node_predicate:N.node t -> bool)
  : prop =
  ((for_all_root_and_children node1 node_predicate) ==> 
   (node_predicate node2))

let node_satisfies_for_all_root_and_children_entails_all_children_nodes_satisfy_predicate #t
  (node:N.node t)
  (node_predicate:N.node t -> bool)
  : prop =
  forall child_node. 
  (C.is_parent node child_node) ==>
  (node1_satisfies_for_all_root_and_children_entails_node2_satisfies_predicate 
    node child_node node_predicate
  )

//---|

//--- proof ---

let lemma1 #t
  (root:N.node t) 
  (node_predicate:N.node t -> bool)
  : Lemma
    (requires (N.is_leaf root) && (node_predicate root))
    (ensures 
      for_all_root_and_children root node_predicate
    )
  =
  ()

(*
#push-options "--query_stats"
let lemma2 #t
  (root:N.node t) 
  (child:N.node t) 
  (node_predicate:N.node t -> bool)
  : Lemma
    (requires 
      (N.is_leaf child) && (node_predicate child) &&
      (N.is_leaf root) &&
      ( let root2 = D.prepend_child child root in
        node_predicate root2 )
    )
    (ensures 
      for_all_root_and_children root node_predicate
    )
  =
  ()
#pop-options 
*)

(*
let lemma4 #t
  (parent:N.node t) 
  (node_predicate:C.ancestor_list_given_selector t bool)
  (prepending:N.node t)
  : Lemma
    (requires 
      (N.is_leaf parent) /\ 
      (for_all_self_and_children parent node_predicate []) /\ 
      (N.is_leaf prepending) /\
      ( let parent2 = D.prepend_child prepending parent in
        let prepended = N.get_child_at parent2 0 in
        node_predicate prepended [parent2] )
    )
    (ensures (
      let parent2 = D.prepend_child prepending parent in
      for_all_self_and_children parent2 node_predicate []
    ))
  =
  let parent2 = D.prepend_child prepending parent in
  let prepended = N.get_child_at parent2 0 in
  assert (N.get_children_length parent2 = 1);
  assert (N.get_level parent2 = 2)
*)

(*
let lemma3 #t
  (parent:N.node t) 
  (node_predicate:C.ancestor_list_given_selector t bool)
  (prepending:N.node t)
  : Lemma
    (requires 
      (for_all_self_and_children parent node_predicate []) /\ 
      ( let parent2 = D.prepend_child prepending parent in
        let prepended = N.get_child_at parent2 0 in
        node_predicate prepended [parent2] )
    )
    (ensures (
      let parent2 = D.prepend_child prepending parent in
      for_all_self_and_children parent2 node_predicate []
    ))
  =
  ()
*)

(*
let lemma3 #t
  (parent:N.node t) 
  (node_predicate:C.ancestor_list_given_selector t bool)
  //(ancestors:C.next_head_given_ancestor_list parent)
  (prepending:N.node t)
  : Lemma
    (requires for_all_self_and_children 
      (D.prepend_child prepending parent) node_predicate []
    )
    (ensures for_all_self_and_children 
      parent node_predicate []
    )
  =
  ()
*)

(*
let lemma2 #t
  (parent:N.node t) 
  (node_predicate:C.ancestor_list_given_selector t bool)
  : Lemma 
    (ensures 
      node_satisfies_for_all_self_and_children_entails_all_children_nodes_satisfy_predicate
        parent node_predicate []
    )
  =
  ()
*)

//---|