module Nemonuri.StratifiedNodes.Children

module L = FStar.List.Tot
module N = Nemonuri.StratifiedNodes.Nodes
module T = Nemonuri.StratifiedNodes.Children.Types
module Common = Nemonuri.StratifiedNodes.Common
module O = FStar.Pervasives.Native

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

//--- propositions ---

let node_list_is_subchildren_of_node #t
  (node_list:N.node_list t)
  (node:N.node t) 
  : prop =
  (forall (n:N.node t). (L.contains n node_list) ==> (is_parent node n))

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

let rec select_and_aggregate_from_children #t #t2
  (parent:N.node t) 
  (selector:ancestor_list_given_selector t t2)
  (selector_for_child:ancestor_list_given_selector_for_child t t2 parent)
  (to_first_child_from_parent:Common.aggregator t2) 
  (from_left_to_right:Common.aggregator t2)
  (from_last_child_to_parent:Common.aggregator t2) 
  (continue_predicate:T.continue_predicate t t2)
  (ancestors:next_head_given_ancestor_list parent)
  (subchildren:N.node_list t)
  (maybe_aggregated_by_depth_first: option t2)
  : Pure t2
    (requires 
      (node_list_is_subchildren_of_node subchildren parent) /\
      ((subchildren = (N.get_children parent)) \/
       (subchildren << (N.get_children parent))
      ) /\
      ((subchildren = (N.get_children parent)) <==>
       (None? maybe_aggregated_by_depth_first)
      )
    )
    (ensures fun r -> true)
    (decreases subchildren)
  =
  let children = (N.get_children parent) in
  let current_ancestors = concatenate_as_ancestor_list parent ancestors in
  let looped = (subchildren <> children) in
  match subchildren with
  | [] -> (
    let parent_value = (selector parent ancestors) in 
    match looped with
    | false -> parent_value
    | true -> (from_last_child_to_parent (O.Some?.v maybe_aggregated_by_depth_first) parent_value) 
  )
  | hd::tl -> (
    let head_child_value = selector_for_child hd current_ancestors in
    let aggregated = (
      match looped with
      | false -> to_first_child_from_parent head_child_value (selector parent ancestors)
      | true -> from_left_to_right (O.Some?.v maybe_aggregated_by_depth_first) head_child_value
    ) in 
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
      parent selector selector_for_child
      to_first_child_from_parent
      from_left_to_right 
      from_last_child_to_parent 
      continue_predicate
      ancestors next_subchildren (Some aggregated)
  )

//---|

//--- propositions 2 ---

let result_of_to_ancestor_list_given_selector_is_extensionality_equal_to_source
  (t:eqtype) (t2:Type)
  (node:N.node t)
  (source_selector:N.node t -> t2)
  : prop =
  forall (ancestors:next_head_given_ancestor_list node). (
    let selector2 = ( to_ancestor_list_given_selector t t2 source_selector ) in
    ( (source_selector node) == (selector2 node ancestors) )
  )

let result_of_to_ancestor_list_given_selector_for_child_is_extensionality_equal_to_source
  (t:eqtype) (t2:Type)
  (parent:N.node t)
  (source_selector:ancestor_list_given_selector t t2)
  : prop =
  forall 
  (child:N.node t)
  (ancestors_of_parent:next_head_given_ancestor_list parent).
  ( is_parent parent child ) ==> (
    let selector2 = ( to_ancestor_list_given_selector_for_child t t2 parent source_selector ) in
    let ancestors = ( concatenate_as_ancestor_list parent ancestors_of_parent ) in
    ( (source_selector child ancestors) == (selector2 child ancestors) )
  )

//---|

//--- proof 2 ---

let lemma_result_of_to_ancestor_list_given_selector_is_extensionality_equal_to_source #t
  (t2:Type)
  (node:N.node t) (source_selector:N.node t -> t2)
  : Lemma 
    (ensures result_of_to_ancestor_list_given_selector_is_extensionality_equal_to_source 
      t t2 node source_selector )
  =
  ()

let lemma_result_of_to_ancestor_list_given_selector_for_child_is_extensionality_equal_to_source #t
  (t2:Type)
  (parent:N.node t)
  (source_selector:ancestor_list_given_selector t t2)
  : Lemma
    (ensures result_of_to_ancestor_list_given_selector_for_child_is_extensionality_equal_to_source
      t t2 parent source_selector )
  =
  ()

private let lemma3 #t
  (t2:Type)
  (node:N.node t)
  (source_selector:N.node t -> t2)
  : Lemma
    (ensures result_of_to_ancestor_list_given_selector_for_child_is_extensionality_equal_to_source
      t t2 node (to_ancestor_list_given_selector t t2 source_selector) )
  =
  ()

//---|

include Nemonuri.StratifiedNodes.Children.Types