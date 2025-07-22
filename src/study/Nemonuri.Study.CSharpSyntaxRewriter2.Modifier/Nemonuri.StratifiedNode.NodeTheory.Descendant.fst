module Nemonuri.StratifiedNode.NodeTheory.Descendant

module L = FStar.List.Tot.Base
open Nemonuri.StratifiedNode
open Nemonuri.StratifiedNode.ListTheory
open Nemonuri.StratifiedNode.NodeTheory.Common
open Nemonuri.StratifiedNode.NodeTheory.Child


private let rec sum (l:list nat)
  : Tot nat
  = match l with
  | [] -> 0
  | hd::tl -> hd + (sum tl)

private let sum_for_get_count
  (#t:eqtype) (#lv:pos) (#sn:stratified_node t lv)
  (l:list nat{ L.length l = get_length sn.children })
  : Tot nat
  = sum l

let rec get_count
  (#t:eqtype) (#lv:pos) (sn:stratified_node t lv) 
  : Tot pos (decreases lv)
  = if is_leaf sn then 
      1
    else 
      sum_for_get_count #t #lv #sn (
        select_in_children sn (
          fun csn -> (
            //lemma_child_node_level_is_lower_than_parent psn csn;
            get_count csn <: nat
          )
        )
      )

let rec get_count_from_predicate
  (#t:eqtype) (#lv:pos) (sn:stratified_node t lv) 
  (predicate:stratified_node_predicate t)
  : Tot nat (decreases lv)
  = if is_leaf sn then 
      (if (predicate sn) then 1 else 0)
    else 
      sum_for_get_count #t #lv #sn (
        select_in_children sn (
          fun csn -> (
            get_count_from_predicate csn predicate
          )
        )
      )

let rec exists_in_descendant_or_self
  (#t:eqtype) (#lv:pos) (sn:stratified_node t lv) 
  (predicate:stratified_node_predicate t)
  : Tot bool (decreases lv)
  = if is_leaf sn then
      (predicate sn)
    else
      exists_in_children sn (
        fun csn -> (
          exists_in_descendant_or_self csn predicate
        )
      )

let is_descendant_or_self
  (#t:eqtype) (#lv:pos) (sn:stratified_node t lv)
  (#descendant_lv:pos) (descendant:stratified_node t descendant_lv)
  : Tot bool
  = exists_in_descendant_or_self sn (is_equal_node descendant)

let rec exists_in_descendant
  (#t:eqtype) (#lv:pos) (sn:stratified_node t lv) 
  (predicate:parent_and_child_node_predicate t)
  : Tot bool (decreases lv)
  = if (exists_in_children sn (predicate sn)) then true
    else
      exists_in_children sn (
        fun csn -> (
          exists_in_descendant csn predicate
        )
      )