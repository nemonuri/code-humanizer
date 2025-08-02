module Nemonuri.StratifiedNodes.Indexes

module L = FStar.List.Tot
module N = Nemonuri.StratifiedNodes.Nodes
module C = Nemonuri.StratifiedNodes.Children
module Common = Nemonuri.StratifiedNodes.Common

//--- theory members ---
private let rec get_indexes_core #t (hd:N.node t) (tl:C.ancestor_list t) 
  : Pure (list nat) 
    (requires (C.is_concatenatable_to_ancestor_list hd tl))
    (ensures fun r -> 
      (L.length r) = (L.length tl)
    )
    (decreases tl)
  =
  match tl with
  | [] -> []
  | hd2::tl2 -> (
      assert (C.is_parent hd2 hd);
      (C.get_child_index hd2 hd)::
      (get_indexes_core hd2 tl2)
  )

let get_indexes #t (ancestors:C.ancestor_list t)
  : Pure (list nat) 
    (requires (Cons? ancestors)) 
    (ensures fun r -> 
      (L.length r) = ((L.length ancestors) - 1)
    )
  =
  let hd::tl = ancestors in
  get_indexes_core hd tl

private let rec can_get_descendant_or_self_from_indexes_core #t
  (hd:N.node t) (reversed_indexes:list nat)
  : Tot bool (decreases reversed_indexes) =
  if (not ((N.get_level hd) > (L.length reversed_indexes))) then
    false
  else if (L.isEmpty reversed_indexes) then
    true
  else
    let children_length = (N.get_children_length hd) in
    let child_index::next_reversed_indexes = reversed_indexes in
    if (child_index < children_length) then (
      let next_hd = (N.get_child_at hd child_index) in
      can_get_descendant_or_self_from_indexes_core next_hd next_reversed_indexes
    ) else false

let can_get_descendant_or_self_from_indexes #t 
  (hd:N.node t) (indexes:list nat)
  : Tot bool =
  can_get_descendant_or_self_from_indexes_core hd (L.rev indexes)

private let rec get_descendant_or_self_from_indexes_core #t
  (hd:N.node t) (reversed_indexes:list nat)
  : Pure (N.node t)
    (requires can_get_descendant_or_self_from_indexes_core hd reversed_indexes)
    (ensures fun _ -> true)
    (decreases reversed_indexes)
  =
  if (L.isEmpty reversed_indexes) then hd
  else
    let children_length = (N.get_children_length hd) in
    let child_index::next_reversed_indexes = reversed_indexes in
    let next_hd = (N.get_child_at hd child_index) in
    get_descendant_or_self_from_indexes_core next_hd next_reversed_indexes

let get_descendant_or_self_from_indexes #t
  (hd:N.node t) (indexes:list nat)
  : Pure (N.node t)
    (requires can_get_descendant_or_self_from_indexes hd indexes)
    (ensures fun _ -> true)
  =
  get_descendant_or_self_from_indexes_core hd (L.rev indexes)

(*
private let rec are_indexes_of_core #t
  (indexes:list nat)
  (ancestors_hd:N.node t) (ancestors_tl:C.next_head_given_ancestor_list ancestors_hd)
  : Pure bool 
    (requires (L.length indexes) = (L.length ancestors_tl))
    (ensures fun _ -> true)
    (decreases indexes)
  =
  match ancestors_tl with
  | [] -> true
  | a_hd2::a_tl2 -> (
      assert (C.is_parent a_hd2 ancestors_hd);
      let i_hd::i_tl = indexes in
      match (i_hd < (N.get_children_length ancestors_hd)) && ((N.get_child_at ancestors_hd i_hd) = a_hd2) with
      | false -> false
      | true -> (are_indexes_of_core i_tl a_hd2 a_tl2)
    )

let are_indexes_of2 #t 
  (indexes:list nat) (ancestors:C.ancestor_list t) 
  : Pure bool
    (requires (L.length indexes) = ((L.length ancestors) - 1))
    (ensures fun _ -> true)
    (decreases indexes)
  =
  let a_hd::a_tl = ancestors in
  are_indexes_of_core indexes a_hd a_tl
*)

let has_index #t
  (parent:N.node t) (child:N.node t) (index:nat)
  : Pure bool 
    (requires C.is_parent parent child)
    (ensures fun _ -> true)
  =
  if not (index < (N.get_children_length parent)) then false else
  ((N.get_child_at parent index) = child)


let rec have_indexes #t
  (ancestors:C.ancestor_list t) (indexes:list nat) 
  : Tot bool 
    (decreases indexes)
  =
  if not ((L.length indexes) = ((L.length ancestors) - 1)) then false else
  if Nil? indexes then true else
  let (i_hd::i_tl, a_hd::a_tl) = (indexes, ancestors) in
  let a_hd2::a_tl2 = a_tl in
  assert (C.is_parent a_hd2 a_hd);
  if not (has_index a_hd2 a_hd i_hd) then false else
  have_indexes a_tl i_tl

//---|