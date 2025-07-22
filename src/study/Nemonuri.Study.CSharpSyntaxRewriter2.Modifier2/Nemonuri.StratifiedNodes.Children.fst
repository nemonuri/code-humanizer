module Nemonuri.StratifiedNodes.Children

module L = FStar.List.Tot
module N = Nemonuri.StratifiedNodes.Nodes
module Common = Nemonuri.StratifiedNodes.Common

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

//--- type definitions ---
let parent_child_selector (t:eqtype) (t2:Type) =
  (parent:N.node t) ->
  (child:N.node t{is_child parent child}) ->
  Tot t2

let child_selector t (t2:Type) (parent:N.node t) =
  (child:N.node t{is_child parent child}) ->
  Tot t2
//---|

//--- theory members 2 ---
private let rec select_and_aggregate_from_children_core #t #t2
  (parent:N.node t) 
  (selector:child_selector t t2 parent)
  (aggregator:Common.aggregator t2) 
  (continue_predicate:t2 -> bool)
  (subchildren:N.node_list t{ forall (n:N.node t). (L.contains n subchildren) ==> (is_child parent n) }) //L.for_all (is_child parent) subchildren
  (seed:t2)
  : Tot t2 (decreases subchildren) =
  if not (continue_predicate seed) then
    seed
  else
    match subchildren with
    | [] -> seed
    | hd::tl ->
        let v1 = selector hd in
        let v2 = aggregator seed v1 in
        select_and_aggregate_from_children_core parent selector aggregator continue_predicate tl v2

let select_and_aggregate_from_children #t #t2
  (parent:N.node t) 
  (selector:parent_child_selector t t2)
  (aggregator:Common.aggregator t2) 
  (continue_predicate:t2 -> bool) 
  (seed:t2)
  : Tot t2 =
  //assume ( L.for_all (is_child parent) (N.get_children parent) ); // <- TODO
  select_and_aggregate_from_children_core parent (selector parent) aggregator continue_predicate (N.get_children parent) seed
//---|