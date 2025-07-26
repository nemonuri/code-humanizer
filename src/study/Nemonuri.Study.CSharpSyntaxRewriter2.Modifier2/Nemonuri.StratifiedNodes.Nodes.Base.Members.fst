module Nemonuri.StratifiedNodes.Nodes.Base.Members

module L = FStar.List.Tot
module I = Nemonuri.StratifiedNodes.Internals
module T = Nemonuri.StratifiedNodes.Nodes.Base.Types
module Math = FStar.Math.Lib
open FStar.Classical.Sugar

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
//---|

//--- (T.node_list t) propositions ---
let list_level_is_greater_or_equal_than_any_element_level 
  (t:eqtype) (l:T.node_list t) 
  : prop =
  (forall (node:T.node t). 
    (L.contains node l) ==> ((get_level node) <= get_list_level_impl l))
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
//---|