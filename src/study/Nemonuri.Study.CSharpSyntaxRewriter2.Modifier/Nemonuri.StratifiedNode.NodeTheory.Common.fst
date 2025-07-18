module Nemonuri.StratifiedNode.NodeTheory.Common

module L = FStar.List.Tot.Base
open Nemonuri.StratifiedNode
open Nemonuri.StratifiedNode.ListTheory


let get_level (#t:eqtype) (#lv:pos) (sn:stratified_node t lv) : Tot pos = lv

let is_equal_node 
  (#t:eqtype) (#lv1:pos) (sn1:stratified_node t lv1)
  (#lv2:pos) (sn2:stratified_node t lv2)
  : Tot bool
  = (lv1 = lv2) && (sn1 = sn2)

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

let is_child 
  (#t:eqtype) 
  (#parent_level:pos) (parent:stratified_node t parent_level)
  (#child_level:pos) (child:stratified_node t child_level) 
  : Tot bool
  = contains parent.children child

let stratified_node_level_is_children_level_plus_one
  (#t:eqtype) (#lv:pos) (sn:stratified_node t lv)
  : Lemma ((SNode?.children_level sn + 1) = (get_level sn))
  = ()

let lemma_child_node_level_is_lower_than_parent
  (#t:eqtype) 
  (#parent_level:pos) (parent:stratified_node t parent_level)
  (#child_level:pos) (child:stratified_node t child_level) 
  : Lemma (requires is_child parent child)
          (ensures (child_level < parent_level))
  = let children = parent.children in
    lemma_node_is_element_implies_node_level_is_less_or_equal_than_list_max_level children child

let is_shorter_or_equal_than_children 
  (#t:eqtype) (#lv:pos) (parent:stratified_node t lv)
  (#mlv:nat) (snl:stratified_node_list t mlv)
  : Tot bool
  = is_left_shorter_or_equal_than_right snl parent.children

let ends_with_children 
  (#t:eqtype) (#lv:pos) (parent:stratified_node t lv)
  (#end_mlv:nat) (end_snl:stratified_node_list t end_mlv{ is_shorter_or_equal_than_children parent end_snl })
  : Tot bool
  = ends_with parent.children end_snl

let is_subchildren 
  (#t:eqtype) (#lv:pos) (parent:stratified_node t lv)
  (#mlv:nat) (snl:stratified_node_list t mlv)
  : Tot bool
  = (is_shorter_or_equal_than_children parent snl) && (ends_with_children parent snl)

let lemma_subchildren_hd_is_child
  (#t:eqtype) (#lv:pos) (parent:stratified_node t lv) 
  (#children_mlv:nat)
  (subchildren:stratified_node_list t children_mlv{SCons? subchildren})
  : Lemma (requires is_subchildren parent subchildren)
          (ensures is_child parent (get_hd subchildren))
  = lemma_snl1_ends_with_snl2_means_snl1_contains_snl2_head parent.children subchildren

let lemma_subchildren_tl_is_subchildren
  (#t:eqtype) (#lv:pos) (parent:stratified_node t lv)
  (#children_mlv:nat) 
  (subchildren:stratified_node_list t children_mlv{SCons? subchildren})
  : Lemma (requires is_subchildren parent subchildren)
          (ensures is_subchildren parent (get_tl subchildren))
  = lemma_snl1_ends_with_snl2_means_snl1_ends_with_snl2_tl parent.children subchildren


let get_children_length
  (#t:eqtype) (#lv:pos) (sn:stratified_node t lv) 
  : Tot nat
  = get_length sn.children

let get_child_level_at
  (#t:eqtype) (#lv:pos) (sn:stratified_node t lv) 
  (index:nat{index < (get_children_length sn)})
  : Tot pos
  = get_node_level sn.children index

let get_child_at
  (#t:eqtype) (#lv:pos) (sn:stratified_node t lv) 
  (index:nat{index < (get_children_length sn)})
  : Tot (stratified_node t (get_child_level_at sn index))
  = get_node sn.children index