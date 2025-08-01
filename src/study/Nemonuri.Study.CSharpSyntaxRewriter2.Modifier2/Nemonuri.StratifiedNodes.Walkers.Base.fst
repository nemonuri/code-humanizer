module Nemonuri.StratifiedNodes.Walkers.Base

module L = FStar.List.Tot
module N = Nemonuri.StratifiedNodes.Nodes
module C = Nemonuri.StratifiedNodes.Children
module Common = Nemonuri.StratifiedNodes.Common

//--- type definitions ---

//---|

//--- theory members ---
let rec walk #t #t2
  (node:N.node t)
  (selector:C.ancestor_list_given_selector t t2)
  (to_first_child_from_parent:Common.aggregator t2) 
  (from_left_to_right:Common.aggregator t2)
  (from_last_child_to_parent:Common.aggregator t2) 
  (continue_predicate:C.continue_predicate t t2)
  (ancestors:C.next_head_given_ancestor_list node)
  : Tot t2 (decreases (node.level)) =
  let v1 = selector node ancestors in
  if (N.is_leaf node) || (not (continue_predicate node v1)) then
    v1
  else
    let child_selector : (C.ancestor_list_given_selector_for_child t t2 node) = (
      fun (child:N.node t{C.is_parent node child}) 
          (ancestors1:C.next_head_given_ancestor_list child) -> (
      walk 
        child selector 
        to_first_child_from_parent 
        from_left_to_right 
        from_last_child_to_parent
        continue_predicate ancestors1
    )) in
    C.select_and_aggregate_from_children
      node 
      selector
      child_selector
      to_first_child_from_parent 
      from_left_to_right 
      from_last_child_to_parent
      continue_predicate 
      ancestors
      (N.get_children node)
      (None #t2)
//---|
