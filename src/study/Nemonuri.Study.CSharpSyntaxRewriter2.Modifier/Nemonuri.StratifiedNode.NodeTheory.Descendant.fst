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
            lemma_child_node_level_is_lower_than_parent sn csn;
            get_count #t #(get_level csn) csn <: nat
          )
        )
      )
      
let rec exists_in_descendant_or_self
  (#t:eqtype) (#lv:pos) (sn:stratified_node t lv) 
  (predicate:stratified_node_predicate t)
  : Tot bool (decreases lv)
  = if is_leaf sn then
      false
    else
      exists_in_children sn (
        fun csn -> (
          lemma_child_node_level_is_lower_than_parent sn csn;
          exists_in_descendant_or_self #t #(get_level csn) csn predicate
        )
      )

let is_descendant_or_self
  (#t:eqtype) (#lv:pos) (sn:stratified_node t lv)
  (#descendant_lv:pos) (descendant:stratified_node t descendant_lv)
  : Tot bool
  = exists_in_descendant_or_self sn (fun (#clv:pos) (csn:stratified_node t clv) -> is_equal_node descendant csn)

