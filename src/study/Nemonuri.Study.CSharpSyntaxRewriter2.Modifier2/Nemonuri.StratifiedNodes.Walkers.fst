module Nemonuri.StratifiedNodes.Walkers

module L = FStar.List.Tot
module N = Nemonuri.StratifiedNodes.Nodes
module C = Nemonuri.StratifiedNodes.Children
module Option = FStar.Option
module Common = Nemonuri.StratifiedNodes.Common

//--- type definitions ---

//---|

//--- theory members ---
let rec walk #t #t2
  (node:N.node t)
  (selector:C.ancestor_list_given_selector t t2)
  (child_parent_aggregator:Common.aggregator t2) 
  (left_right_aggregator:Common.aggregator t2)
  (continue_predicate:N.node t -> t2 -> bool)
  (ancestors:C.ancestor_list t) //{ if L.isEmpty ancestor_list then true else C.is_child (L.last ancestor_list) node }
  : Tot t2 (decreases (node.level)) =
  let v1 = selector ancestors node in
  if (N.is_leaf node) || (not (continue_predicate node v1)) then
    v1
  else
    let next_ancestors = (node::ancestors) in
    C.select_and_aggregate_from_children
      node (fun ancestors child -> 
        (walk child selector child_parent_aggregator left_right_aggregator continue_predicate ancestors)
      ) child_parent_aggregator left_right_aggregator continue_predicate next_ancestors v1
//---|
