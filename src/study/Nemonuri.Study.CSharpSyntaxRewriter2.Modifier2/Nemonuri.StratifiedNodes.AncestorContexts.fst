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
  match ancestor_context with
  | T.ANil -> []
  | T.ACons ancestors _ -> ancestors

let get_indexes #t
  (ancestor_context:T.ancestor_context t)
  : Tot (list nat)
  =
  match ancestor_context with
  | T.ANil -> []
  | T.ACons _ indexes -> indexes

let has_head_ancestor #t
  (ancestor_context:T.ancestor_context t)
  : Pure (bool) True 
    (ensures fun b -> 
      match b with
      | false -> true
      | true -> Cons? (get_ancestors ancestor_context)
    )
  =
  T.ACons? ancestor_context
  //Cons? (get_ancestors ancestor_context)

[@@expect_failure]
private let _ = assert (
  forall t ancestor_context.
  has_head_ancestor #t ancestor_context
)

let get_head_ancestor #t
  (ancestor_context:T.ancestor_context t{has_head_ancestor ancestor_context})
  : Tot (N.node t)
  =
  L.hd (get_ancestors ancestor_context)

let get_head_ancestor_children #t
  (ancestor_context:T.ancestor_context t{has_head_ancestor ancestor_context})
  : Tot (N.node_list t)
  =
  N.get_children (get_head_ancestor ancestor_context)

let get_head_ancestor_children_or_empty #t
  (ancestor_context:T.ancestor_context t)
  : Tot (N.node_list t)
  =
  match (has_head_ancestor ancestor_context) with
  | true -> get_head_ancestor_children ancestor_context
  | false -> []

let is_prependable_to_ancestor_context #t
  (node:N.node t) 
  (maybe_index:Common.nat_or_minus_one) 
  (ancestor_context:T.ancestor_context t)
  : Tot bool
  =
  match ancestor_context with
  | T.ANil -> (maybe_index = -1)
  | T.ACons ancestors indexes -> 
  (maybe_index >= 0) &&
  (C.is_concatenatable_to_ancestor_list node ancestors) &&
  (Id.has_index (L.hd ancestors) node maybe_index)

let prepend_to_ancestor_context #t
  (node:N.node t) (maybe_index:Common.nat_or_minus_one) 
  (ancestor_context:T.ancestor_context t)
  : Pure (T.ancestor_context t)
    (requires is_prependable_to_ancestor_context node maybe_index ancestor_context)
    (ensures fun r -> 
      ((T.ACons? r) && (T.ACons? ancestor_context)) ==>
      ((L.tl (get_ancestors r)) = (get_ancestors ancestor_context)) &&
      ((L.tl (get_indexes r)) = (get_indexes ancestor_context))
    )
  = 
  match ancestor_context with
  | T.ANil -> T.ACons [node] []
  | T.ACons ancestors indexes -> (
    let new_ancestors = C.concatenate_as_ancestor_list node ancestors in
    let new_indexes = maybe_index::indexes in
    T.ACons new_ancestors new_indexes
  )

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