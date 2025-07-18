module Nemonuri.StratifiedNode.IndexTheory.Base


module L = FStar.List.Tot
open Nemonuri.StratifiedNode
open Nemonuri.StratifiedNode.ListTheory
open Nemonuri.StratifiedNode.NodeTheory

let stratified_node_indexes = list nat

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

private let rec try_get_index_in_children_core
  (#t:eqtype) (#lv:pos) (parent:stratified_node t lv) 
  (#children_mlv:nat)
  (subchildren:stratified_node_list t children_mlv{ is_subchildren parent subchildren })
  (predicate:(child_node_predicate parent))
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
          try_get_index_in_children_core parent (get_tl subchildren) predicate
      )

let try_get_index_in_children
  (#t:eqtype) (#lv:pos) (parent:stratified_node t lv)
  (predicate:(child_node_predicate parent))
  : Tot (option (child_node_index parent)) 
  = try_get_index_in_children_core parent parent.children predicate


(*
let rec try_get_indexes_from_predicate
  (#t:eqtype) (#lv:pos) (sn:stratified_node t lv)
  (predicate:stratified_node_predicate t)
  : Tot (option (sni:stratified_node_indexes{ contains_indexes sn sni }))
  = 
*)

(*
let rec get_indexes_from_node
  (#t:eqtype) (#root_lv:pos) (root:stratified_node t root_lv)
  (#lv:pos) (sn:stratified_node t lv{is_descendant_or_self root sn})
  : Tot (sni:stratified_node_indexes{ contains_indexes root sni })
  =
*)