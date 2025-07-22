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
let to_parent_child_selector #t #t2 (selector:N.node t -> t2) 
  : Tot (parent_child_selector t t2) =
  fun 
    (parent:N.node t)
    (child:N.node t{is_child parent child}) ->
    selector child

private let rec select_and_aggregate_from_children_core #t #t2
  (parent:N.node t) 
  (selector:child_selector t t2 parent)
  (child_parent_aggregator:Common.aggregator t2) 
  (left_right_aggregator:Common.aggregator t2)
  (continue_predicate:N.node t -> t2 -> bool)
  (subchildren:N.node_list t{ forall (n:N.node t). (L.contains n subchildren) ==> (is_child parent n) })
  (seed:t2)
  : Tot t2 (decreases subchildren) =
  match subchildren with
  | [] -> seed
  | hd::tl ->
      let v1 = selector hd in
      let v2 = (
        if (N.get_children_length parent = L.length subchildren) then (
          child_parent_aggregator v1 seed
        ) else (
          left_right_aggregator seed v1
        )
      ) in
      if not (continue_predicate hd v2) then
        v2
      else (
        select_and_aggregate_from_children_core 
          parent selector 
          child_parent_aggregator left_right_aggregator
          continue_predicate tl v2
      )

let select_and_aggregate_from_children #t #t2
  (parent:N.node t) 
  (selector:child_selector t t2 parent)
  (child_parent_aggregator:Common.aggregator t2) 
  (left_right_aggregator:Common.aggregator t2)
  (continue_predicate:N.node t -> t2 -> bool)
  (seed:t2)
  : Tot t2 =
  select_and_aggregate_from_children_core 
    parent selector 
    child_parent_aggregator left_right_aggregator 
    continue_predicate (N.get_children parent) seed
//---|