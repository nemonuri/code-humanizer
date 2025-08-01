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

(*
let for_all_select_and_aggregate_from_children_internal #t (parent:N.node t) 
  (parent_value: t2)
  (node_predicate:N.node t -> Tot bool)
*)

let for_all_select_and_aggregate_from_children #t
  (parent:N.node t) 
  (node_predicate:N.node t -> Tot bool)
  (ancestors:C.head_given_ancestor_list parent)
  : Tot bool =
  let selector : (C.ancestor_list_given_selector_for_child t bool parent) = (
    C.to_ancestor_list_given_selector_for_child t bool
      parent node_predicate
  ) in
  let parent_value = node_predicate parent in
  C.select_and_aggregate_from_children 
    parent parent_value selector
    for_all_aggregator
    for_all_aggregator
    (Common.aggregated_identity bool)
    (for_all_continue_predicate t)
    ancestors

//---|

//--- propositions ---
//---|

//--- proof ---
//---|