module Nemonuri.StratifiedNodes.Nodes.Base.Members

module L = FStar.List.Tot
module I = Nemonuri.StratifiedNodes.Internals
module T = Nemonuri.StratifiedNodes.Nodes.Base.Types
module Math = FStar.Math.Lib
module Common = Nemonuri.StratifiedNodes.Common
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

let has_index #t
  (#node_list:T.node_list t)
  (source:T.maybe_node_list_index node_list)
  : Tot bool =
  T.ISome? source

let to_int #t
  (#node_list:T.node_list t)
  (source:T.maybe_node_list_index node_list)
  : Pure int True
    (ensures fun r -> 
      match has_index source with
      | true -> (r >= 0) && (is_node_list_index r node_list)
      | false -> (r = -1)
    )
  =
  match has_index source with
  | true -> (T.ISome?.index source)
  | false -> -1

(*
let has_index #t 
  (#node_list:T.node_list t)
  (source:T.maybe_node_list_index node_list)
  : Tot bool =
  (source.value <> -1)
*)

(*
let get_default_maybe_node_list_index #t (node_list:T.node_list t)
  : Pure (T.maybe_node_list_index node_list) True
    (ensures fun r -> not (has_index r))
  =
  T.INone node_list
*)

let to_maybe_node_list_index #t (index:nat) (node_list:T.node_list t) 
  : Pure (T.maybe_node_list_index node_list) True
    (ensures fun r ->
      match (index >= (L.length node_list)) with
      | true -> (to_int r) = -1
      | false -> (to_int r) = index
    )
  =
  match is_node_list_index index node_list with
  | true -> T.ISome node_list index 
  | false -> T.INone node_list

private let is_node_list_index_rewrite #t (node_list:T.node_list t) (index:nat)
  : Tot bool =
  is_node_list_index index node_list

let is_node_list_index_list #t (l:list nat) (node_list:T.node_list t)
  : Pure bool True
    (ensures fun b ->
      match b with
      | true -> (forall i. L.contains i l ==> is_node_list_index i node_list)
      | false -> True
    )
  =
  L.for_all_mem (is_node_list_index_rewrite node_list) l;
  L.for_all (is_node_list_index_rewrite node_list) l 
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

private let rec get_index_list_from_predicate_core #t 
  (original_node_list: T.node_list t)
  (current_index: nat) 
  (predicate: (T.node t) -> Tot bool) 
  : Pure (list nat) 
    (requires (current_index = (L.length original_node_list)) ||
              (is_node_list_index current_index original_node_list)
    )
    (ensures fun r -> 
      (is_node_list_index_list r original_node_list) /\
      (forall i. (L.contains i r) ==> (predicate (L.index original_node_list i)))
    )
    (decreases (L.length original_node_list) - current_index)
  =
  let length = (L.length original_node_list) in
  match (length = current_index) with
  | true -> []
  | false -> 
  let current_item = (L.index original_node_list current_index) in
  let next_index = (current_index + 1) in
  let results_from_end = (get_index_list_from_predicate_core original_node_list next_index predicate) in
  match (predicate current_item) with
  | true -> current_index::results_from_end
  | false -> results_from_end

//---|

//--- (T.node_list t) propositions ---
private let is_node_list_index_and_is_node_list_index_rewrite_are_equal
  : prop =
  (forall (t:eqtype) (index:nat) (node_list:T.node_list t).
    (is_node_list_index index node_list) = (is_node_list_index_rewrite node_list index)
  )

let list_level_is_greater_or_equal_than_any_element_level 
  (t:eqtype) (l:T.node_list t) 
  : prop =
  (forall (node:T.node t). 
    (L.contains node l) ==> ((get_level node) <= get_list_level_impl l))

(*
let result_of_get_index_list_from_predicate_is_node_list_index_list #t
  (node_list: T.node_list t) (predicate: (T.node t) -> Tot bool) 
  : prop =
  let v0 = get_index_list_from_predicate_impl node_list predicate in
  is_node_list_index_list v0 node_list
*)

let result_of_get_index_list_from_predicate_is_shorter_or_equal_than_original_node_list #t
  (original_node_list: T.node_list t)
  (current_index: nat) 
  (predicate: (T.node t) -> Tot bool) 
  : prop =
  (L.length (get_index_list_from_predicate_core original_node_list 0 predicate)) <= (L.length original_node_list)

let element_of_result_of_get_index_list_from_predicate_satisfies_predicate #t
  (node_list: T.node_list t) (predicate: (T.node t) -> Tot bool) 
  (index: nat)
  : prop =
  let index_list = get_index_list_from_predicate_core node_list 0 predicate in
  ((L.contains index index_list) <==> (
    (is_node_list_index index node_list) /\
    (predicate (L.index node_list index))
  ))
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

(*
let lemma_result_of_get_index_list_from_predicate_is_shorter_or_equal_than_original_node_list #t
  (original_node_list: T.node_list t)
  (current_index: nat) 
  (predicate: (T.node t) -> Tot bool) 
  : Lemma 
    (ensures result_of_get_index_list_from_predicate_is_shorter_or_equal_than_original_node_list
      original_node_list current_index predicate
    )
  = 
  admit ()
*)

private let lemma_element_of_result_of_get_index_list_from_predicate_satisfies_predicate_aux1 #t
  (node_list: T.node_list t) (predicate: (T.node t) -> Tot bool) 
  (index: nat)
  : Lemma (requires L.contains index (get_index_list_from_predicate_core node_list 0 predicate))
    (ensures is_node_list_index index node_list)
  =
  ()
  //assert (is_node_list_index_and_is_node_list_index_rewrite_are_equal);
  //let v1 = (get_index_list_from_predicate_core node_list 0 predicate) in (
    //assert (Cons? node_list);
    //L.for_all_mem (is_node_list_index_rewrite node_list) v1
    //assert (is_node_list_index_list v1 node_list);
    //assert (forall i. (L.contains i v1) ==> (is_node_list_index i node_list))
  //)

private let lemma_element_of_result_of_get_index_list_from_predicate_satisfies_predicate_aux2 #t
  (node_list: T.node_list t) (predicate: (T.node t) -> Tot bool) 
  (index: nat)
  : Lemma (requires 
      (L.contains index (get_index_list_from_predicate_core node_list 0 predicate)) /\ 
      (is_node_list_index index node_list)
    )
    (ensures predicate (L.index node_list index))
  =
  ()
  //lemma_element_of_result_of_get_index_list_from_predicate_satisfies_predicate_aux1 node_list predicate index

(*
let lemma_element_of_result_of_get_index_list_from_predicate_satisfies_predicate #t
  (node_list: T.node_list t) (predicate: (T.node t) -> Tot bool) 
  (index: nat)
  : Lemma (ensures element_of_result_of_get_index_list_from_predicate_satisfies_predicate node_list predicate index)
  =
  if not (L.contains index (get_index_list_from_predicate_core node_list 0 predicate)) then ()
  else (
    lemma_element_of_result_of_get_index_list_from_predicate_satisfies_predicate_aux1 node_list predicate index;
    lemma_element_of_result_of_get_index_list_from_predicate_satisfies_predicate_aux2 node_list predicate index
  )
*)
  
(*
private let rec lemma_result_of_get_index_list_from_predicate_is_node_list_index_list_core #t
  (original_node_list: T.node_list t)
  (current_index: nat) 
  (predicate: (T.node t) -> Tot bool) 
  : Lemma 
    (requires current_index <= (L.length original_node_list) )
    (ensures result_of_get_index_list_from_predicate_is_node_list_index_list 
        original_node_list predicate)
    (decreases (L.length original_node_list) - current_index)
  =
  let length = (L.length original_node_list) in
  match (length = current_index) with
  | true -> ()
  | false -> 
  let current_item = (L.index original_node_list current_index) in
  let next_index = (current_index + 1) in
  let results_from_end = (lemma_result_of_get_index_list_from_predicate_is_node_list_index_list_core 
                          original_node_list next_index predicate) in
  match (predicate current_item) with
  | true -> current_index::results_from_end
  | false -> results_from_end

let lemma_result_of_get_index_list_from_predicate_is_node_list_index_list #t
  (node_list: T.node_list t) //(predicate: (T.node t) -> Tot bool)
  : Lemma 
    (ensures (forall predicate. result_of_get_index_list_from_predicate_is_node_list_index_list
      node_list predicate))
  =
  introduce forall (predicate: (T.node t) -> Tot bool).
  result_of_get_index_list_from_predicate_is_node_list_index_list node_list predicate with
  lemma_result_of_get_index_list_from_predicate_is_node_list_index_list_core
    node_list node_list predicate 0 []
*)


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
  match to_maybe_node_list_index index node_list with
  | T.ISome _ v1 -> Some (get_item node_list v1)
  | T.INone _ -> None

let get_index_list_from_predicate #t
  (node_list: T.node_list t) (predicate: (T.node t) -> Tot bool) 
  : Pure (list nat) True
    (ensures fun r -> //is_node_list_index_list r node_list
      ( forall (index:nat).
        element_of_result_of_get_index_list_from_predicate_satisfies_predicate 
          node_list predicate index
      )
    )
  =
  get_index_list_from_predicate_core node_list 0 predicate

let try_get_first_index_of_predicate #t 
  (node_list:T.node_list t) (predicate: (T.node t) -> Tot bool)
  : Pure (T.maybe_node_list_index node_list) True
        (ensures fun r ->
          match r with
          | T.INone _ -> true
          | T.ISome _ v1 -> 
            (element_of_result_of_get_index_list_from_predicate_satisfies_predicate node_list predicate v1)
        )
        (decreases node_list)
  = 
  match get_index_list_from_predicate node_list predicate with
  | [] -> T.INone node_list
  | hd::tl -> T.ISome node_list hd
//---|

//--- try_get_index ---
private let try_get_index_impl #t
  (node_list:T.node_list t) (node:T.node t)
  : Pure (T.maybe_node_list_index node_list) True
  
    (ensures fun r ->
      match (L.contains node node_list) with
      | true -> T.ISome? r
      | false -> T.INone? r
    )
  
  =
  try_get_first_index_of_predicate node_list (op_Equality node)

(*
let try_get_index #t
  (node_list:T.node_list t) (node:T.node t)
  : Pure (T.maybe_node_list_index node_list) True
    (ensures fun r ->
      match (L.contains node node_list) with
      | true -> T.ISome? r
      | false -> T.INone? r
    )
  =
  assert 
*)
//---