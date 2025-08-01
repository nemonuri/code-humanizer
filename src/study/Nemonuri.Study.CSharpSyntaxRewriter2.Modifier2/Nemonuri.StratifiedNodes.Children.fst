module Nemonuri.StratifiedNodes.Children

module L = FStar.List.Tot
module N = Nemonuri.StratifiedNodes.Nodes
module T = Nemonuri.StratifiedNodes.Children.Types
module Common = Nemonuri.StratifiedNodes.Common

//--- theory members ---
let always_continue_predicate (t:eqtype) (t2:Type)
  : T.continue_predicate t t2 =
  fun n v -> true

let is_parent #t (parent_node:N.node t) (node:N.node t) 
  : Pure bool (requires True) (ensures fun _ -> N.node_level_is_greater_than_levels_of_nodes_in_children t)
  =
  N.lemma_node_level_is_greater_than_levels_of_nodes_in_children t;
  L.contains node (N.get_children parent_node)

private let rec is_ancestor_list_core #t (hd:N.node t) (tl:N.node_list t) 
  : Tot bool (decreases tl) =
  match tl with
  | [] -> true
  | hd2::tl2 -> 
      (is_parent hd2 hd) &&
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
  | hd::_ -> is_parent hd node

let concatenate_as_ancestor_list #t
  (node:N.node t) (ancestor_list:N.node_list t)
  : Pure (N.node_list t)
    (requires (is_ancestor_list ancestor_list) && (is_concatenatable_to_ancestor_list node ancestor_list))
    (ensures fun r -> is_ancestor_list r)
  =
  node::ancestor_list

#push-options "--query_stats"
let get_child_index #t (parent_node:N.node t) (node:N.node t)
  : Pure nat
    (requires (is_parent parent_node node))
    (ensures fun r ->
      let v1 = L.length (N.get_children parent_node) in
      (0 <= r) && (r < v1)
    )
  =
  assume ((is_parent parent_node node) ==> (Some? (L.find (op_Equality node) (N.get_children parent_node))));
  Common.find_index (N.get_children parent_node) (op_Equality node)
#pop-options

//---|

//--- asserts ---
let test_is_parent1 = assert (
  forall (t:eqtype) (parent_node:N.node t) (node:N.node t).
    (is_parent parent_node node) ==> (parent_node.level > node.level))
//---|

//--- propositions ---
(*
let node_list_is_ancestor_list #t (node_list:N.node_list t) : prop =
  forall (n:N.node t).
  let skipped = Common.skip_while node_list (Prims.op_Equality n) in (
    (L.length skipped >= 2) ==> (
      let hd::tl = skipped in
      is_parent hd (L.hd tl)
    )
  )
*)

let node_list_is_subchildren_of_node #t
  (node_list:N.node_list t)
  (node:N.node t) 
  : prop =
  (forall (n:N.node t). (L.contains n node_list) ==> (is_parent node n))

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
  (child:N.node t{is_parent parent child}) ->
  Tot t2

let ancestor_list_given_selector (t:eqtype) (t2:Type) =
  (node:N.node t) -> 
  (ancestors:next_head_given_ancestor_list node) ->
  Tot t2

let ancestor_list_given_selector_for_child t (t2:Type) (parent:N.node t) =
  (child:N.node t{is_parent parent child}) ->
  (ancestors:next_head_given_ancestor_list child) ->
  Tot t2
//---|

//--- theory members 2 ---
let to_parent_child_selector #t #t2 (selector:N.node t -> t2) 
  : Tot (parent_child_selector t t2) =
  fun 
    (parent:N.node t)
    (child:N.node t{is_parent parent child}) ->
    selector child

let to_ancestor_list_given_selector (t:eqtype) (t2:Type)
  (selector:N.node t -> t2)
  : Tot (ancestor_list_given_selector t t2) =
  fun node ancestors -> (selector node)

let to_ancestor_list_given_selector_for_child t (t2:Type) (parent:N.node t) 
  (selector:ancestor_list_given_selector t t2)
  : Tot (ancestor_list_given_selector_for_child t t2 parent)
  =
  fun child ancestors -> (selector child ancestors)


private let rec lemma_empty_list_is_decrease_of_list #t
  (l:list t)
  : Lemma 
    (ensures (Nil? l) \/ ((Nil #t) << l))
    (decreases l)
  =
  match l with
  | [] -> ()
  | _::tl -> lemma_empty_list_is_decrease_of_list tl

(*
let rec select_and_aggregate_from_children_internal #t #t2 (parent:N.node t) 
  (parent_value: t2)
  (selector:ancestor_list_given_selector_for_child t t2 parent)
  (from_left_to_right:Common.aggregator t2)
  (from_last_child_to_parent:Common.aggregator t2) 
  (continue_predicate:T.continue_predicate t t2)
  (subchildren:N.node_list t{ node_list_is_subchildren_of_node subchildren parent })
  (ancestors:head_given_ancestor_list parent)
  (seed:t2)
  : Tot t2 (decreases subchildren) =
  match subchildren with
  | [] -> (from_last_child_to_parent seed parent_value)
  | hd::tl ->
  let v1 = selector hd ancestors in
  let v2 = from_left_to_right seed v1 in
  let next_subchildren = (
    if (continue_predicate hd v2) then
      tl
    else (
      let empty_list = [] in
      lemma_empty_list_is_decrease_of_list subchildren;
      assert (empty_list << subchildren);
      empty_list
    )
  ) in (
    select_and_aggregate_from_children_internal parent
    parent_value selector 
    from_left_to_right
    from_last_child_to_parent
    continue_predicate next_subchildren ancestors v2
  )

let select_and_aggregate_from_children #t #t2
  (parent:N.node t) 
  (parent_value: t2)
  (selector:ancestor_list_given_selector_for_child t t2 parent)
  (to_first_child_from_parent:Common.aggregator t2) 
  (from_left_to_right:Common.aggregator t2)
  (from_last_child_to_parent:Common.aggregator t2) 
  (continue_predicate:T.continue_predicate t t2)
  (ancestors:head_given_ancestor_list parent)
  : Tot t2 =
  let children = (N.get_children parent) in
  match children with
  | [] -> parent_value
  | children_head::children_tail ->
  let head_child_value = selector children_head ancestors in
  let seed = to_first_child_from_parent head_child_value parent_value in
  select_and_aggregate_from_children_internal 
    parent parent_value selector 
    from_left_to_right
    from_last_child_to_parent
    continue_predicate children_tail ancestors
    seed
*)

let rec select_and_aggregate_from_children #t #t2
  (parent:N.node t) 
  //(parent_selector:ancestor_list_given_selector t t2)
  (selector:ancestor_list_given_selector t t2)
  (to_first_child_from_parent:Common.aggregator t2) 
  (from_left_to_right:Common.aggregator t2)
  (from_last_child_to_parent:Common.aggregator t2) 
  (continue_predicate:T.continue_predicate t t2)
  (ancestors:next_head_given_ancestor_list parent)
  (subchildren:N.node_list t)
  (aggregating_from_parent_or_aggregated_by_depth_first: option t2)
  : Pure t2
    (requires 
      (node_list_is_subchildren_of_node subchildren parent) /\
      ((subchildren = (N.get_children parent)) \/
       (subchildren << (N.get_children parent))
      ) /\
      ((subchildren = (N.get_children parent)) <==>
       (None? aggregating_from_parent_or_aggregated_by_depth_first)
      )
    )
    (ensures fun r -> true)
    (decreases subchildren)
  =
  let agg = aggregating_from_parent_or_aggregated_by_depth_first in
  let selector_for_child : (ancestor_list_given_selector_for_child t t2 parent) = (
    to_ancestor_list_given_selector_for_child t t2 parent selector
  ) in
  let children = (N.get_children parent) in
  match subchildren = children with
  | true -> (
    match subchildren with
    | [] -> (selector parent ancestors)
    | children_head::tl ->
    let current_ancestors = concatenate_as_ancestor_list parent ancestors in
    let head_child_value = selector_for_child children_head current_ancestors in
    let aggregated = to_first_child_from_parent head_child_value (selector parent ancestors) in
    let next_subchildren = (
      if (continue_predicate children_head aggregated) then
        tl
      else (
        let empty_list = [] in
        lemma_empty_list_is_decrease_of_list subchildren;
        assert (empty_list << subchildren);
        empty_list
      )
    ) in
    select_and_aggregate_from_children
      parent selector 
      to_first_child_from_parent
      from_left_to_right 
      from_last_child_to_parent 
      continue_predicate
      ancestors next_subchildren (Some aggregated)
  )
  | false -> (
    let Some agg_some = agg in
    match subchildren with
    | [] -> (from_last_child_to_parent agg_some (selector parent ancestors))
    | hd::tl ->
    let current_ancestors = concatenate_as_ancestor_list parent ancestors in
    let v1 = selector_for_child hd current_ancestors in
    let aggregated = from_left_to_right agg_some v1 in
    let next_subchildren = (
      if (continue_predicate hd aggregated) then
        tl
      else (
        let empty_list = [] in
        lemma_empty_list_is_decrease_of_list subchildren;
        assert (empty_list << subchildren);
        empty_list
      )
    ) in
    select_and_aggregate_from_children
      parent selector 
      to_first_child_from_parent
      from_left_to_right 
      from_last_child_to_parent 
      continue_predicate
      ancestors next_subchildren (Some aggregated)
  )

(*
  let children = (N.get_children parent) in
  match children with
  | [] -> parent_value
  | children_head::children_tail ->
  let head_child_value = selector children_head ancestors in
  let seed = to_first_child_from_parent head_child_value parent_value in
  select_and_aggregate_from_children_internal 
    parent parent_value selector 
    from_left_to_right
    from_last_child_to_parent
    continue_predicate children_tail ancestors
    seed
*)

//---|

include Nemonuri.StratifiedNodes.Children.Types