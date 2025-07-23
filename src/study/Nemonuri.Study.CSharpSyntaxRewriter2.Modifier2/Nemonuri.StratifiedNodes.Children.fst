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

private let rec is_ancestor_list_core #t (hd:N.node t) (tl:N.node_list t) 
  : Tot bool (decreases tl) =
  match tl with
  | [] -> true
  | hd2::tl2 -> 
      (is_child hd2 hd) &&
      (is_ancestor_list_core hd2 tl2)

let is_ancestor_list #t (node_list:N.node_list t)
  : Tot bool
  =
  match node_list with
  | [] -> true
  | hd::tl -> is_ancestor_list_core hd tl

let is_concatenatable_to_ancestor_list #t (node:N.node t) (ancestor_list:N.node_list t)
  : Pure bool 
    (requires is_ancestor_list ancestor_list) 
    (ensures fun b ->
      match b with
      | false -> true
      | true -> is_ancestor_list (node::ancestor_list)
    )
  =
  match ancestor_list with
  | [] -> true
  | hd::_ -> is_child hd node

let concatenate_as_ancestor_list #t
  (node:N.node t) (ancestor_list:N.node_list t)
  : Pure (N.node_list t)
    (requires (is_ancestor_list ancestor_list) && (is_concatenatable_to_ancestor_list node ancestor_list))
    (ensures fun r -> is_ancestor_list r)
  =
  node::ancestor_list
//---|

//--- asserts ---
let test_is_child1 = assert (
  forall (t:eqtype) (parent_node:N.node t) (node:N.node t).
    (is_child parent_node node) ==> (parent_node.level > node.level))
//---|

//--- propositions ---
(*
let node_list_is_ancestor_list #t (node_list:N.node_list t) : prop =
  forall (n:N.node t).
  let skipped = Common.skip_while node_list (Prims.op_Equality n) in (
    (L.length skipped >= 2) ==> (
      let hd::tl = skipped in
      is_child hd (L.hd tl)
    )
  )
*)
//---|

//--- type definitions ---
let ancestor_list (t:eqtype) = l:N.node_list t{ is_ancestor_list l }

let next_head_given_ancestor_list #t (next_head:N.node t) =
  l:ancestor_list t{ is_concatenatable_to_ancestor_list next_head l }

let head_given_ancestor_list #t (head:N.node t) =
  l:ancestor_list t{ 
    match l with
    | [] -> false
    | hd::tl -> (head = hd)
  }

let parent_child_selector (t:eqtype) (t2:Type) =
  (parent:N.node t) ->
  (child:N.node t{is_child parent child}) ->
  Tot t2

let ancestor_list_given_selector (t:eqtype) (t2:Type) =
  (node:N.node t) -> 
  (ancestors:next_head_given_ancestor_list node) ->
  Tot t2

let ancestor_list_given_selector_for_child t (t2:Type) (parent:N.node t) =
  (child:N.node t{is_child parent child}) ->
  (ancestors:next_head_given_ancestor_list child) ->
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
  (selector:ancestor_list_given_selector_for_child t t2 parent)
  (child_parent_aggregator:Common.aggregator t2) 
  (left_right_aggregator:Common.aggregator t2)
  (continue_predicate:N.node t -> t2 -> bool)
  (subchildren:N.node_list t{ forall (n:N.node t). (L.contains n subchildren) ==> (is_child parent n) })
  (ancestors:head_given_ancestor_list parent)
  (seed:t2)
  : Tot t2 (decreases subchildren) =
  match subchildren with
  | [] -> seed
  | hd::tl ->
      let v1 = selector hd ancestors in
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
          continue_predicate tl ancestors v2
      )

let select_and_aggregate_from_children #t #t2
  (parent:N.node t) 
  (selector:ancestor_list_given_selector_for_child t t2 parent)
  (child_parent_aggregator:Common.aggregator t2) 
  (left_right_aggregator:Common.aggregator t2)
  (continue_predicate:N.node t -> t2 -> bool)
  (ancestors:head_given_ancestor_list parent)
  (seed:t2)
  : Tot t2 =
  select_and_aggregate_from_children_core 
    parent selector 
    child_parent_aggregator left_right_aggregator 
    continue_predicate (N.get_children parent) ancestors seed
//---|