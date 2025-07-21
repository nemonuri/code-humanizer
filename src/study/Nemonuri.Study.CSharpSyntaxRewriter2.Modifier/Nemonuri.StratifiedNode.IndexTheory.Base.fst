module Nemonuri.StratifiedNode.IndexTheory.Base


module L = FStar.List.Tot
open Nemonuri.StratifiedNode
open Nemonuri.StratifiedNode.ListTheory
open Nemonuri.StratifiedNode.NodeTheory

let stratified_node_indexes = list nat

let add_index (sni:stratified_node_indexes) (index:nat) = L.append sni [index]

let empty_stratified_node_indexes : stratified_node_indexes = []

let convert_level_to_indexes_length (level:pos)
  : Tot nat
  = level - 1

let convert_indexes_length_to_level (indexes_length:nat)
  : Tot pos
  = indexes_length + 1

let is_stratified_node_level_less_than_indexes_length 
  (#t:eqtype) (#lv:pos) (sn:stratified_node t lv) 
  (sni:stratified_node_indexes)
  : Tot bool
  = (convert_level_to_indexes_length lv) < (L.length sni)

let rec contains_indexes
  (#t:eqtype) (#lv:pos) (sn:stratified_node t lv)
  (sni:stratified_node_indexes)
  : Tot bool (decreases sni)
  = if (is_stratified_node_level_less_than_indexes_length sn sni) then 
      false
    else if (L.isEmpty sni) then 
      true
    else
      let child_index::next_sni = sni in
      if (child_index >= (get_children_length sn)) then false
      else 
        let next_sn = get_child_at sn child_index in
        contains_indexes next_sn next_sni
      
let rec get_node_level_from_indexes
  (#t:eqtype) (#lv:pos) (sn:stratified_node t lv)
  (sni:stratified_node_indexes{contains_indexes sn sni})
  : Tot pos (decreases sni)
  = if (L.isEmpty sni) then 
      lv
    else
      let child_index::next_sni = sni in
      let next_sn = get_child_at sn child_index in
      get_node_level_from_indexes next_sn next_sni

let rec get_node_from_indexes
  (#t:eqtype) (#lv:pos) (sn:stratified_node t lv)
  (sni:stratified_node_indexes{contains_indexes sn sni})
  : Tot (stratified_node t (get_node_level_from_indexes sn sni))
        (decreases sni)
  = if (L.isEmpty sni) then 
      sn
    else
      let child_index::next_sni = sni in
      let next_sn = get_child_at sn child_index in
      get_node_from_indexes next_sn next_sni

private let rec try_get_index_of_child_from_predicate_core
  (#t:eqtype) (#lv:pos) (parent:stratified_node t lv) 
  (#children_mlv:nat)
  (subchildren:stratified_node_list t children_mlv{ is_subchildren parent subchildren })
  (predicate:child_node_predicate t parent)
  : Tot (option (child_node_index parent)) 
        (decreases subchildren)
  = if is_empty subchildren then 
      None
    else
      (
        lemma_for_subchildren parent subchildren;
        if predicate (get_hd subchildren) then 
          Some ((get_children_length parent) - (get_length subchildren))
        else 
          try_get_index_of_child_from_predicate_core parent (get_tl subchildren) predicate
      )

let try_get_index_of_child_from_predicate
  (#t:eqtype) (#lv:pos) (parent:stratified_node t lv)
  (predicate:child_node_predicate t parent)
  : Tot (option (child_node_index parent)) 
  = try_get_index_of_child_from_predicate_core parent parent.children predicate

private let rec try_get_indexes_of_descendant_or_self_from_predicate_core
  (#t:eqtype) (#lv:pos) (parent:stratified_node t lv) (parent_indexes:stratified_node_indexes)
  (predicate_builder:child_node_predicate_builder t)
  : Tot (option (stratified_node_indexes)) 
        (decreases lv)
  = match (try_get_index_of_child_from_predicate #t #lv parent (predicate_builder parent)) with
    | Some v1 -> Some (add_index parent_indexes v1)
    | None -> 
    let v2 =
      select_in_children parent (
        fun csn -> (
          let cni = get_child_node_index csn in
          let new_parent_indexes = add_index parent_indexes cni in
          try_get_indexes_of_descendant_or_self_from_predicate_core csn new_parent_indexes predicate_builder
        )
      ) in
    let v3 = L.find (Some?) v2 in
    match v3 with
    | None -> None
    | Some v4 -> v4


let try_get_indexes_of_descendant_or_self_from_predicate
  (#t:eqtype) (#lv:pos) (root:stratified_node t lv)
  (predicate:stratified_node_predicate t)
  : Tot (option (stratified_node_indexes)) 
  = if (predicate root) then
      Some empty_stratified_node_indexes
    else
      try_get_indexes_of_descendant_or_self_from_predicate_core root empty_stratified_node_indexes
        (fun (#parent_level:pos) (parent:stratified_node t parent_level) ->
          (fun (#child_level:pos) (child:(child_node parent child_level)) -> predicate child)
        )

(*
  = match (if (predicate parent) then (Some empty_stratified_node_indexes) else None) with
  | None -> None
  | Some v1 ->
      let cnp = convert_stratified_node_predicate_to_child_node_predicate predicate parent in
      match (try_get_index_of_child_from_predicate parent cnp) with
      | None -> None
      | Some v2 -> 
*)    


(*
let rec get_indexes_from_node
  (#t:eqtype) (#root_lv:pos) (root:stratified_node t root_lv)
  (#lv:pos) (sn:stratified_node t lv{is_descendant_or_self root sn})
  : Tot (sni:stratified_node_indexes{ contains_indexes root sni })
  =
*)