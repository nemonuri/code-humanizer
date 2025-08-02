module Nemonuri.StratifiedNodes.AncestorContexts

module L = FStar.List.Tot
module N = Nemonuri.StratifiedNodes.Nodes
module C = Nemonuri.StratifiedNodes.Children
module Common = Nemonuri.StratifiedNodes.Common
module Id = Nemonuri.StratifiedNodes.Indexes
module T = Nemonuri.StratifiedNodes.AncestorContexts.Types


//--- members ---

let get_ancestors #t
  (ancestor_context:T.ancestor_context t)
  : Tot (C.ancestor_list t)
  =
  T.AContext?.ancestors ancestor_context

let get_indexes #t
  (ancestor_context:T.ancestor_context t)
  : Tot (list nat)
  =
  T.AContext?.indexes ancestor_context

private let has_head_ancestor #t
  (ancestor_context:T.ancestor_context t)
  : Tot bool =
  Cons? (get_ancestors ancestor_context)

private let _ = assert (
  forall t ancestor_context.
  has_head_ancestor #t ancestor_context
) //...의미없구나

let get_head_ancestor #t
  (ancestor_context:T.ancestor_context t)
  : Tot (N.node t)
  =
  L.hd (get_ancestors ancestor_context)

let get_head_ancestor_children #t
  (ancestor_context:T.ancestor_context t)
  : Tot (N.node_list t)
  =
  N.get_children (get_head_ancestor ancestor_context)

let is_prependable_to_ancestor_context #t
  (node:N.node t) (index:nat) (ancestor_context:T.ancestor_context t)
  : Tot bool
  =
  let ancestors = (get_ancestors ancestor_context) in
  (C.is_concatenatable_to_ancestor_list node ancestors) &&
  (Id.has_index (L.hd ancestors) node index)

let prepend_to_ancestor_context #t
  (node:N.node t) (index:nat) (ancestor_context:T.ancestor_context t)
  : Pure (T.ancestor_context t)
    (requires is_prependable_to_ancestor_context node index ancestor_context)
    (ensures fun r -> 
      ((L.tl (get_ancestors r)) = (get_ancestors ancestor_context)) &&
      ((L.tl (get_indexes r)) = (get_indexes ancestor_context))
    )
  = 
  let new_ancestors = C.concatenate_as_ancestor_list node (get_ancestors ancestor_context) in
  let new_indexes = index::(get_indexes ancestor_context) in
  T.AContext new_ancestors new_indexes

let next_heads_given_ancestor_context #t (node:N.node t) (index:nat) =
  ac:T.ancestor_context t{ is_prependable_to_ancestor_context node index ac }

let heads_given_ancestor_context #t (node:N.node t) (index:nat) =
  ac:T.ancestor_context t{
    match (get_ancestors ac, get_indexes ac) with
    | (_, []) -> false
    | (a_hd::a_tl, i_hd::i_tl) ->
      (a_hd = node) && (i_hd = index)
  }


let get_first_decreaser_of_ancestor_context #t
  (ancestor_context: T.ancestor_context t)
  : Tot (Common.zero_or_one) 
  =
  C.get_one_if_ancestor_list_is_empty_or_zero 
    (get_ancestors ancestor_context)

let get_second_decreaser_of_ancestor_context #t
  (ancestor_context: T.ancestor_context t)
  : Tot nat
  =
  C.get_ancestor_list_head_level_or_zero 
    (get_ancestors ancestor_context)

type ancestor_context_bound_selector (t: eqtype) (t2: Type)
  (bound_ancestors: T.ancestor_context t) =
  (node: N.node t) -> 
  (index: nat) ->
  (ancestors: 
    next_heads_given_ancestor_context node index{ancestors = bound_ancestors}
  ) ->
  Tot t2

(*
let to_next_ancestor_context_bound_selector (t: eqtype) (t2: Type)
  (node: N.node t) (index: nat) 
  (ancestors: 
    next_heads_given_ancestor_context node index
  )
  (selector: ancestor_context_bound_selector t t2 ancestors)
  : Tot 
    (ancestor_context_bound_selector t t2 (
      prepend_to_ancestor_context node index ancestors
    ))
  =
  fun (next_node: N.node t) (next_index: nat)
  (next_ancestors:
    next_heads_given_ancestor_context next_node next_index{
      next_ancestors =
      (prepend_to_ancestor_context node index ancestors)
  }) ->
  (selector next_node next_index (prepend_to_ancestor_context node index ancestors))
*)
//---|

//--- proposition ---
let ancestor_context2_is_decrease_of_ancestor_context1 #t
  (ancestor_context1: T.ancestor_context t) 
  (ancestor_context2: T.ancestor_context t) 
  : prop
  =
  ( let (ac_1, ac_2) = 
      ( get_first_decreaser_of_ancestor_context ancestor_context1,
        get_second_decreaser_of_ancestor_context ancestor_context1
      ) in
    let (ac2_1, ac2_2) = 
      ( 
        get_first_decreaser_of_ancestor_context ancestor_context2,
        get_second_decreaser_of_ancestor_context ancestor_context2
      ) in
    ( (ac2_1 << ac_1) \/ ( (ac2_1 = ac_1) /\ (ac2_2 << ac_2) ) )
  ) 
//---|

//--- proof ---

let lemma_prepend_to_ancestor_context_is_decreasing #t
  (ancestor_context:T.ancestor_context t) 
  (node:N.node t) (index:nat)
  : Lemma
    (requires (is_prependable_to_ancestor_context node index ancestor_context))
    (ensures 
      ancestor_context2_is_decrease_of_ancestor_context1
        ancestor_context
        (prepend_to_ancestor_context node index ancestor_context)
      //prepend_to_ancestor_context_is_decreasing ancestor_context node index
    )
  =
  ()
  //C.lemma_concatenate_as_ancestor_list_is_decreasing
  //  (get_ancestors ancestor_context) node

//---|

include Nemonuri.StratifiedNodes.AncestorContexts.Types