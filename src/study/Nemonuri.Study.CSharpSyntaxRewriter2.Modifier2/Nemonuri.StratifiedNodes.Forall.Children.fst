module Nemonuri.StratifiedNodes.Forall.Children

module L = FStar.List.Tot
module N = Nemonuri.StratifiedNodes.Nodes
module C = Nemonuri.StratifiedNodes.Children
module Common = Nemonuri.StratifiedNodes.Common
module D = Nemonuri.StratifiedNodes.Decreasers
module F = Nemonuri.StratifiedNodes.Factories

//--- theory members ---

let for_all_aggregator
  : (Common.aggregator bool) =
  fun v1 v2 -> (v1 && v2)

let for_all_continue_predicate (t:eqtype)
  : (C.continue_predicate t bool) 
  =
  fun n v -> v


let for_all_self_and_children #t
  (parent:N.node t) 
  (node_predicate:C.ancestor_list_given_selector t bool)
  (ancestors:C.next_head_given_ancestor_list parent)
  : Tot bool =
  let node_predicate_for_child : (C.ancestor_list_given_selector_for_child t bool parent) = (
    C.to_ancestor_list_given_selector_for_child t bool parent node_predicate
  ) in
  C.select_and_aggregate_from_children 
    parent node_predicate node_predicate_for_child
    for_all_aggregator
    for_all_aggregator
    (Common.aggregated_identity bool)
    (for_all_continue_predicate t)
    ancestors
    (N.get_children parent)
    None

//--- propositions ---

let node1_satisfies_for_all_self_and_children_entails_node2_satisfies_predicate #t
  (node1:N.node t) 
  (node_predicate:C.ancestor_list_given_selector t bool)
  (ancestors1:C.next_head_given_ancestor_list node1)
  (node2:N.node t)
  (ancestors2:C.next_head_given_ancestor_list node2)
  : prop =
  ((for_all_self_and_children node1 node_predicate ancestors1) ==> 
   (node_predicate node2 ancestors2))

let node_satisfies_for_all_self_and_children_entails_all_children_nodes_satisfy_predicate #t
  (node:N.node t)
  (node_predicate:C.ancestor_list_given_selector t bool)
  (ancestors:C.next_head_given_ancestor_list node)
  : prop =
  forall child_node. 
  (C.is_parent node child_node) ==>
  (node1_satisfies_for_all_self_and_children_entails_node2_satisfies_predicate 
    node node_predicate ancestors
    child_node (C.concatenate_as_ancestor_list node ancestors)
  )

//---|

//--- proof ---

let lemma1 #t
  (parent:N.node t) 
  (node_predicate:C.ancestor_list_given_selector t bool)
  (ancestors:C.next_head_given_ancestor_list parent)
  : Lemma
    (requires (N.is_leaf parent))
    (ensures 
      node_satisfies_for_all_self_and_children_entails_all_children_nodes_satisfy_predicate
        parent node_predicate ancestors
    )
  =
  ()



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