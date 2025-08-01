module Nemonuri.StratifiedNodes.Children

module L = FStar.List.Tot
module N = Nemonuri.StratifiedNodes.Nodes
module T = Nemonuri.StratifiedNodes.Children.Types
module Common = Nemonuri.StratifiedNodes.Common
module O = FStar.Pervasives.Native

(*
note

모듈 이름을 Children 이 아니라, Parents 나 Ancestors 로 바꿔야 하지 않을까?
*)

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

//--- ancestor_list members ---

let get_one_if_ancestor_list_is_empty_or_zero #t
  (al:ancestor_list t)
  : Tot (Common.zero_or_one) 
  =
  match al with
  | [] -> 1
  | _ -> 0

let get_ancestor_list_head_level_or_zero #t
  (al:ancestor_list t)
  : Tot nat 
  =
  match al with
  | [] -> 0
  | hd::_ -> hd.level

private let get_ancestor_list_decreaser_tuple #t
  (al:ancestor_list t)
  : Tot (Common.zero_or_one & nat)
  =
  ((get_one_if_ancestor_list_is_empty_or_zero al), (get_ancestor_list_head_level_or_zero al))

//---|

//--- ancestor_list proofs ---

let lemma_ancestor_list_head_level_is_descending #t
  (al:ancestor_list t)
  : Lemma 
    (requires (Cons? al) /\ (Cons? (L.tl al)))
    (ensures 
      ((get_ancestor_list_head_level_or_zero (L.tl al)) > (get_ancestor_list_head_level_or_zero al))
    )
  =
  ()

let lemma_concatenate_as_ancestor_list_is_decreasing #t
  (al:ancestor_list t) (node:N.node t)
  : Lemma
    (requires (is_concatenatable_to_ancestor_list node al))
    (ensures 
      ( let al2 = concatenate_as_ancestor_list node al in
        let (al_1, al_2) = get_ancestor_list_decreaser_tuple al in
        let (al2_1, al2_2) = get_ancestor_list_decreaser_tuple al2 in
        ( (al2_1 << al_1) \/ ( (al2_1 = al_1) /\ (al2_2 << al_2) ) )
      ) 
    )
  =
  ()

//---|

//--- propositions ---

let node_list_is_subchildren_of_node #t
  (node_list:N.node_list t)
  (node:N.node t) 
  : prop =
  (forall (n:N.node t). (L.contains n node_list) ==> (is_parent node n))

//---|

//--- theory members 3 ---
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
let aggregate_or_get_fallback #t
  (aggregator:Common.aggregator t)
  (maybe_aggregated: option t) (aggregating: t) (fallback_factory: t -> t)
  : Tot t
  =
  match maybe_aggregated with
  | None -> fallback_factory aggregating
  | Some v -> (aggregator v aggregating)
*)

let aggregate_or_get_identity #t
  (aggregator:Common.aggregator t)
  (maybe_aggregated: option t) (aggregating: t)
  : Tot t
  =
  match maybe_aggregated with
  | None -> aggregating
  | Some v -> (aggregator v aggregating)
  //aggregate_or_get_fallback aggregator maybe_aggregated aggregating (fun v -> v)

(*
let aggregate_or_get_fallback_override #t
  (aggregator:Common.aggregator t)
  (maybe_aggregated: option t) (aggregating_or_fallback_aggregated: t)
  (fallback_aggregator:Common.aggregator t) (fallback_aggregating: t)
  : Tot t =
  let fallback_factory:(t -> t) = (fun v -> (fallback_aggregator v fallback_aggregating)) in
  aggregate_or_get_fallback aggregator maybe_aggregated aggregating_or_fallback_aggregated fallback_factory
*)

let aggregate_first_child_and_parent #t #t2
  (parent: N.node t)
  (parent_value: t2)
  (selector_for_child: ancestor_list_given_selector_for_child t t2 parent)
  (aggregator:Common.aggregator t2) 
  (first_child: N.node t)
  (ancestors: next_head_given_ancestor_list first_child)
  : Pure t2
    (requires 
      (N.is_branch parent) /\
      ((N.get_child_at parent 0) = first_child)
    )
    (ensures fun r -> true)
  =
  let first_child_value = selector_for_child first_child ancestors in
  aggregator first_child_value parent_value

let aggregate_left_to_right #t #t2
  (parent: N.node t)
  (prev_aggregated: t2)
  (selector_for_child: ancestor_list_given_selector_for_child t t2 parent)
  (aggregator:Common.aggregator t2) 
  (successor_child: N.node t)
  (ancestors: next_head_given_ancestor_list successor_child)
  : Pure t2
    (requires
      (N.is_branch parent) /\
      (is_parent parent successor_child) /\
      ( exists (i:pos). (i < (N.get_children_length parent)) && ((N.get_child_at parent i) = successor_child) )
    )
    (ensures fun r -> true)
  =
  let successor_child_value = selector_for_child successor_child ancestors in
  aggregator prev_aggregated successor_child_value

let rec aggregate_children #t #t2
  (parent:N.node t) 
  (parent_value:t2)
  (selector_for_child:ancestor_list_given_selector_for_child t t2 parent)
  (to_first_child_from_parent:Common.aggregator t2) 
  (from_left_to_right:Common.aggregator t2)
  (from_last_child_to_parent:Common.aggregator t2) 
  (continue_predicate:T.continue_predicate t t2)
  (current_ancestors:head_given_ancestor_list parent)
  (subchildren:N.node_list t)
  (maybe_aggregated: option t2)
  : Pure t2
    (requires 
      (node_list_is_subchildren_of_node subchildren parent) /\
      ( (subchildren = (N.get_children parent)) \/ (subchildren << (N.get_children parent)) ) /\
      (*( (Cons? subchildren) ==> (
        (((N.get_children_length parent) - (L.length subchildren)) >= 0) /\
        ( (N.get_child_at parent ((N.get_children_length parent) - (L.length subchildren))) = (L.hd subchildren) )) ) /\*)
      ((subchildren = (N.get_children parent)) <==>
       (None? maybe_aggregated)
      )
    )
    (ensures fun r -> true)
    (decreases subchildren)
  =
  let children = (N.get_children parent) in
  match subchildren with
  | [] -> aggregate_or_get_identity from_last_child_to_parent maybe_aggregated parent_value
  | hd::tl -> (
    let aggregated = (
      match maybe_aggregated with
      | None -> 
          assert (subchildren = (N.get_children parent));
          aggregate_first_child_and_parent 
            parent parent_value selector_for_child to_first_child_from_parent hd current_ancestors
      | Some prev_aggregated -> 
          assert (subchildren << (N.get_children parent));
          //assert (((N.get_children_length parent) - (L.length subchildren)) > 0 );
          //assert (hd <> (N.get_child_at parent 0));
          //aggregate_left_to_right
          //  parent prev_aggregated selector_for_child from_left_to_right hd current_ancestors                       
          from_left_to_right prev_aggregated (selector_for_child hd current_ancestors)
    ) in
    //let head_child_value = selector_for_child hd current_ancestors in
    //let fallback_factory:(t2 -> t2) = (fun v -> (to_first_child_from_parent v parent_value)) in
    //let aggregated = aggregate_or_get_fallback_override from_left_to_right maybe_aggregated head_child_value to_first_child_from_parent parent_value in
    //aggregate_or_get_fallback from_left_to_right maybe_aggregated head_child_value fallback_factory in 
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
    aggregate_children
      parent parent_value selector_for_child
      to_first_child_from_parent
      from_left_to_right 
      from_last_child_to_parent 
      continue_predicate
      current_ancestors next_subchildren (Some aggregated)
  )

let aggregate_children_overload #t #t2
  (parent:N.node t) 
  (selector:ancestor_list_given_selector t t2)
  (selector_for_child:ancestor_list_given_selector_for_child t t2 parent)
  (to_first_child_from_parent:Common.aggregator t2) 
  (from_left_to_right:Common.aggregator t2)
  (from_last_child_to_parent:Common.aggregator t2) 
  (continue_predicate:T.continue_predicate t t2)
  (ancestors:next_head_given_ancestor_list parent)
  : Pure t2 True
    (ensures fun r -> true)
  =
  let parent_value = (selector parent ancestors) in
  let current_ancestors = (concatenate_as_ancestor_list parent ancestors) in
  let children = (N.get_children parent) in
  aggregate_children
    parent parent_value selector_for_child
    to_first_child_from_parent
    from_left_to_right
    from_last_child_to_parent
    continue_predicate
    current_ancestors 
    children
    None


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

let lemma_maybe_aggregated_is_none_entails_result_of_aggregate_or_get_identity_is_identity #t
  (aggregator:Common.aggregator t)
  (maybe_aggregated: option t) (aggregating: t)
  : Lemma (requires (None? maybe_aggregated))
    (ensures 
      (aggregate_or_get_identity aggregator maybe_aggregated aggregating) ==
      (aggregating)
    )
  =
  ()

//---|

include Nemonuri.StratifiedNodes.Children.Types