module Nemonuri.StratifiedNodes.Nodes.Base.Members

module L = FStar.List.Tot
module I = Nemonuri.StratifiedNodes.Internals
module T = Nemonuri.StratifiedNodes.Nodes.Base.Types
module Math = FStar.Math.Lib
open FStar.Classical.Sugar

//--- (T.node_list_index t) or (T.option_node_list_index t) members ---

let is_node_list_index #t (index:nat) (node_list:T.node_list t) 
  : Tot bool =
  (Cons? node_list) && (index < (L.length node_list))

let is_not_node_list_index #t (index:nat) (node_list:T.node_list t) 
  : Tot bool =
  (Nil? node_list) || (index >= (L.length node_list))

private let _ = assert ( forall t index node_list .
  (is_node_list_index #t index node_list) \/
  (is_not_node_list_index #t index node_list)
 )

let try_convert_to_node_list_index #t (index:nat) (node_list:T.node_list t) 
  : Pure (T.option_node_list_index node_list) True
    (ensures fun r ->
      match r with
      | Some v1 -> (is_node_list_index index node_list) && (v1 = index)
      | None -> is_not_node_list_index index node_list
    )
  =
  match is_node_list_index index node_list with
  | true -> Some index
  | false -> None //<: (T.option_node_list_index node_list)

let is_node_list_index_list #t (l:list nat) (node_list:T.node_list t)
  : Tot bool =
  L.for_all (fun (index:nat) -> is_node_list_index index node_list) l 

//---|

//--- (T.node t) members ---
let get_level #t (node:T.node t) 
  : Pure pos True (ensures fun r -> (r = (I.get_level node.internal)))
  = 
  node.level

let get_value #t (node:T.node t) : Tot t = node.internal.value
//---|

//--- (T.node_list t) private members ---
private let rec get_list_level_impl #t (l:T.node_list t) : Tot nat =
  match l with
  | [] -> 0
  | hd::tl -> Math.max (get_level hd) (get_list_level_impl tl)

(*
private let rec get_index_list_from_predicate_core #t 
  (node_list:T.node_list t) (predicate: (T.node t) -> Tot bool)
  : Tot (nat & (list nat))
    (decreases node_list)
  =
  match node_list with
  | [] -> (0, [])
  | hd::tl ->
  let (reverse_index, satisfied_reverse_index_list) = get_index_list_from_predicate_core tl predicate in
  let next_reverse_index = reverse_index + 1 in
  let next_satisfied_reverse_index_list = (if (predicate hd) then (reverse_index::satisfied_reverse_index_list) else satisfied_reverse_index_list ) in
  ( reverse_index, next_satisfied_reverse_index_list )
*)

private let rec get_index_list_from_predicate_impl_core #t 
  (node_list: T.node_list t) (predicate: (T.node t) -> Tot bool) 
  (current_index: nat) (current_index_list: list nat)
  : Tot (list nat) 
    (decreases node_list)
  =
  match node_list with
  | [] -> current_index_list
  | hd::tl -> 
  let next_lndex_list = (
    match (predicate hd) with
    | true -> current_index::current_index_list
    | false -> current_index_list
  ) in
  get_index_list_from_predicate_impl_core 
    tl predicate (current_index + 1) next_lndex_list

private let get_index_list_from_predicate_impl #t
  (node_list: T.node_list t) (predicate: (T.node t) -> Tot bool) 
  : Tot (list nat) =
  get_index_list_from_predicate_impl_core 
    node_list predicate 0 []

//---|

//--- (T.node_list t) propositions ---
let list_level_is_greater_or_equal_than_any_element_level 
  (t:eqtype) (l:T.node_list t) 
  : prop =
  (forall (node:T.node t). 
    (L.contains node l) ==> ((get_level node) <= get_list_level_impl l))

let result_of_get_index_list_from_predicate_is_node_list_index_list #t
  (node_list: T.node_list t) (predicate: (T.node t) -> Tot bool) 
  : prop =
  let v0 = get_index_list_from_predicate_impl node_list predicate in
  is_node_list_index_list v0 node_list
//---|

//--- (T.node_list t) lemmas ---
private let rec lemma_list_level_is_greater_or_equal_than_element_level
  (t:eqtype) (n:T.node t) (l:T.node_list t)
  : Lemma 
    (requires L.contains n l)
    (ensures (
      ((get_level n) <= get_list_level_impl l)
    ))
    (decreases l)
  =
  let hd::tl = l in
  if (hd = n) then ()
  else 
    lemma_list_level_is_greater_or_equal_than_element_level t n tl

let lemma_list_level_is_greater_or_equal_than_any_element_level (t:eqtype) (l:T.node_list t)
  : Lemma
    (ensures (
      list_level_is_greater_or_equal_than_any_element_level t l
    ))
  = 
  introduce forall (it_node:T.node t{ L.contains it_node l }). 
    ((get_level it_node) <= get_list_level_impl l) with (
      lemma_list_level_is_greater_or_equal_than_element_level t it_node l
    )
//---|

//--- (T.node_list t) members ---
let get_list_level #t (l:T.node_list t) 
  : Pure nat True (ensures fun _ -> list_level_is_greater_or_equal_than_any_element_level t l) =
  lemma_list_level_is_greater_or_equal_than_any_element_level t l;
  get_list_level_impl l

let get_item #t (node_list:T.node_list t) (index:T.node_list_index node_list)
  : Tot (T.node t)
  =
  L.index node_list index

let try_get_item #t (node_list:T.node_list t) (index:nat)
  : Tot (option (T.node t))
  =
  match try_convert_to_node_list_index index node_list with
  | Some v1 -> Some (get_item node_list v1)
  | None -> None

(*
let rec get_index_list_from_predicate #t 
  (node_list:T.node_list t) (predicate: (T.node t) -> Tot bool)
  : Pure (list nat) True
    (ensures fun r ->
      forall (index_in_result:nat). (L.contains index_in_result node_list) ==>
      ((is_node_list_index index_in_result node_list) /\
       (predicate (L.index node_list index_in_result))
      )
    )
    (decreases node_list)
  =
  match node_list with
  | [] -> []
  | hd::tl ->
*)

let rec try_get_first_index_of_predicate #t 
  (node_list:T.node_list t) (predicate: (T.node t) -> Tot bool)
  : Pure (option nat) True
        (ensures fun r ->
          match r with
          | None -> true
          | Some v1 -> v1 < (L.length node_list)
        )
        (decreases node_list)
  = 
  match node_list with
  | [] -> None
  | hd::tl ->
  match (predicate hd) with
  | true -> Some 0
  | false ->
  match try_get_first_index_of_predicate tl predicate with
  | None -> None
  | Some v1 -> Some (1 + v1)

let get_index_list_from_predicate #t
  (node_list: T.node_list t) (predicate: (T.node t) -> Tot bool)
  : Pure (list nat) True
    (ensures fun r -> 
      result_of_get_index_list_from_predicate_is_node_list_index_list node_list predicate)
  =
  assume (result_of_get_index_list_from_predicate_is_node_list_index_list node_list predicate);
  get_index_list_from_predicate_impl node_list predicate

(*
private let _ = assert ( forall t node_list predicate.
  let r = get_index_list_from_predicate #t node_list predicate in
  ( forall index_in_result. 
      (L.contains index_in_result r) ==>
        ((is_node_list_index index_in_result node_list) /\
        (predicate (L.index node_list index_in_result)) )
  )
)
*)
//---|