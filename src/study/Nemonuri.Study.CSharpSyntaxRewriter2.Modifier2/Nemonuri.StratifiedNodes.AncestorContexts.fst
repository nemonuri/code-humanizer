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

let ancestor_context_given_selector (t:eqtype) (t2:Type) =
  (node:N.node t) -> 
  (index:nat) ->
  (ancestors:next_heads_given_ancestor_context node index) ->
  Tot t2

//---|

//--- proof ---



//---|

include Nemonuri.StratifiedNodes.AncestorContexts.Types