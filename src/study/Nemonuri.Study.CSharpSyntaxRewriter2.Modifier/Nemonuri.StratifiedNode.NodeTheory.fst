module Nemonuri.StratifiedNode.NodeTheory

module L = FStar.List.Tot.Base
open Nemonuri.StratifiedNode
open Nemonuri.StratifiedNode.ListTheory

let get_level (#t:eqtype) (#lv:pos) (sn:stratified_node t lv) : Tot pos = lv

let is_child 
  (#t:eqtype) 
  (#child_level:pos) (child:stratified_node t child_level) 
  (#parent_level:pos) (parent:stratified_node t parent_level)
  : Tot bool
  = contains parent.children child

let stratified_node_level_is_children_level_plus_one
  (#t:eqtype) (#lv:pos) (sn:stratified_node t lv)
  : Lemma ((SNode?.children_level sn + 1) = (get_level sn))
  = ()

let child_node_level_is_lower_than_parent
  (#t:eqtype) 
  (#child_level:pos) (child:stratified_node t child_level) 
  (#parent_level:pos) (parent:stratified_node t parent_level)
  : Lemma (requires is_child child parent)
          (ensures (child_level < parent_level) \/ True)
  = let children = parent.children in
    assert (contains children child)
    

let is_leaf 
  (#t:eqtype) (#lv:pos) (sn:stratified_node t lv) 
  : Tot bool
  = is_empty (SNode?.children sn)

let stratified_node_is_leaf_is_equivalent_to_stratified_node_level_is_one
  (#t:eqtype) (#lv:pos) (sn:stratified_node t lv)
  : Lemma (
      (is_leaf sn) <==> ((get_level sn) = 1)
    )
  = ()

let is_branch
  (#t:eqtype) (#lv:pos) (sn:stratified_node t lv) 
  : Tot bool
  = not (is_leaf sn)

let stratified_node_is_branch_is_equivalent_to_stratified_node_level_is_greater_than_one
  (#t:eqtype) (#lv:pos) (sn:stratified_node t lv)
  : Lemma (
      (is_branch sn) <==> ((get_level sn) > 1)
    )
  = ()

(*
let rec get_count
  (#t:eqtype) (#lv:pos) (sn:stratified_node t lv) 
  : Tot pos (decreases (get_level sn))
  = if is_leaf sn then 
      1
    else 
      L.length (
        stratified_node_level_is_children_level_plus_one sn;
        select (SNode?.children sn) get_count
      )
*)