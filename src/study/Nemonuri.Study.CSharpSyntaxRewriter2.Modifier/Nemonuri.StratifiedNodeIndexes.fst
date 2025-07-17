module Nemonuri.StratifiedNodeIndexes

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