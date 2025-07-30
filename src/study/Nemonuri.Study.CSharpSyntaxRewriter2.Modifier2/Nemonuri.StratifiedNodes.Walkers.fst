module Nemonuri.StratifiedNodes.Walkers

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
    let next_ancestors = C.concatenate_as_ancestor_list node ancestors in
    C.select_and_aggregate_from_children
      node v1
      (fun child ancestors -> (
        walk child selector 
        to_first_child_from_parent 
        from_left_to_right 
        from_last_child_to_parent
        continue_predicate ancestors)
      ) 
      to_first_child_from_parent 
      from_left_to_right 
      from_last_child_to_parent
      continue_predicate next_ancestors
//---|
