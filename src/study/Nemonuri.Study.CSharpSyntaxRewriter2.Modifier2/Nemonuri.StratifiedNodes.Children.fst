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

let rec skip_while #t 
  (node_list:N.node_list t) 
  (predicate:N.node t -> Tot bool)
  : Tot (N.node_list t) (decreases node_list) =
  match node_list with
  | [] -> []
  | hd::tl ->
    match (predicate hd) with
    | true -> node_list
    | false -> skip_while tl predicate
//---|

//--- asserts ---
let test_is_child1 = assert (
  forall (t:eqtype) (parent_node:N.node t) (node:N.node t).
    (is_child parent_node node) ==> (parent_node.level > node.level))
//---|

//--- propositions ---
let node_list_is_ancestor_list #t (node_list:N.node_list t) : prop =
  forall (n:N.node t).
  let skipped = skip_while node_list (Prims.op_Equality n) in (
    (L.length skipped >= 2) ==> (
      let hd::tl = skipped in
      is_child hd (L.hd tl)
    )
  )
//---|

//--- type definitions ---
let ancestor_list (t:eqtype) = N.node_list t
  //al:(N.node_list t){ node_list_is_ancestor_list al }

let can_append_to_ancestor_list #t (ancestor_list1:ancestor_list t) (ancestor_list2:ancestor_list t) //(node:N.node t)
  : Tot bool =
  match (ancestor_list1, ancestor_list2) with
  | (hd1::tl1, hd2::tl2) -> is_child (L.last ancestor_list1) hd2
  | _ -> true

  //if (L.isEmpty al) then true
  //else is_child (L.last al) node

(*
let rec append_to_ancestor_list #t (ancestor_list1:ancestor_list t) (ancestor_list2:ancestor_list t{ can_append_to_ancestor_list ancestor_list1 ancestor_list2 })
  : Tot (ancestor_list t) (decreases ancestor_list1) =
  match ancestor_list1 with
  | [] -> ancestor_list2
  | hd::tl -> (
    assume ( forall (t:eqtype) (al:ancestor_list t). (not (L.isEmpty al)) ==> (node_list_is_ancestor_list (L.tl al)) );
    hd::(append_to_ancestor_list tl ancestor_list2)
  )
*)

let parent_child_selector (t:eqtype) (t2:Type) =
  (parent:N.node t) ->
  (child:N.node t{is_child parent child}) ->
  Tot t2

let ancestor_list_given_selector (t:eqtype) (t2:Type) =
  (ancestors:ancestor_list t) ->
  (node:N.node t) -> 
  Tot t2

let ancestor_list_given_selector_for_child t (t2:Type) (parent:N.node t) =
  (ancestors:ancestor_list t) ->
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
  (selector:ancestor_list_given_selector_for_child t t2 parent)
  (child_parent_aggregator:Common.aggregator t2) 
  (left_right_aggregator:Common.aggregator t2)
  (continue_predicate:N.node t -> t2 -> bool)
  (subchildren:N.node_list t{ forall (n:N.node t). (L.contains n subchildren) ==> (is_child parent n) })
  (ancestors:ancestor_list t)
  (seed:t2)
  : Tot t2 (decreases subchildren) =
  match subchildren with
  | [] -> seed
  | hd::tl ->
      let v1 = selector ancestors hd in
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
  (ancestors:ancestor_list t)
  (seed:t2)
  : Tot t2 =
  select_and_aggregate_from_children_core 
    parent selector 
    child_parent_aggregator left_right_aggregator 
    continue_predicate (N.get_children parent) ancestors seed
//---|