module Nemonuri.StratifiedNodes.Indexes

module L = FStar.List.Tot
module N = Nemonuri.StratifiedNodes.Nodes
module C = Nemonuri.StratifiedNodes.Children
module Common = Nemonuri.StratifiedNodes.Common

//--- theory members ---
private let rec to_indexes_core #t (hd:N.node t) (tl:C.ancestor_list t) 
  : Pure (list nat) 
    (requires (C.is_concatenatable_to_ancestor_list hd tl))
    (ensures fun r -> true)
  =
  match tl with
  | [] -> []
  | hd2::tl2 -> 

let to_indexes #t (ancestors:C.ancestor_list t)
  : Pure (list nat) 
    (requires (Cons? ancestors)) 
    (ensures fun r -> true)
  =
  let hd::tl = ancestors in
  match tl with
  | [] -> []
  | 

//---|