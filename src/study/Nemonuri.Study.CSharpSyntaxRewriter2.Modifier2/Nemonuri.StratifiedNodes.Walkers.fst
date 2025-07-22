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
  (selector:N.node t -> t2)
  (child_parent_aggregator:Common.aggregator t2) 
  (left_right_aggregator:Common.aggregator t2)
  (continue_predicate:N.node t -> t2 -> bool)
  : Tot t2 (decreases (node.level)) =
  let v1 = selector node in
  if (N.is_leaf node) || (not (continue_predicate node v1)) then
    v1
  else
    C.select_and_aggregate_from_children
      node (fun child -> 
        (walk child selector child_parent_aggregator left_right_aggregator continue_predicate)
      ) child_parent_aggregator left_right_aggregator continue_predicate v1
//---|
