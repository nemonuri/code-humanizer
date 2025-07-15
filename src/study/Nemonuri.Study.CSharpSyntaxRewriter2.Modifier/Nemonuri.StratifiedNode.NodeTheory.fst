module Nemonuri.StratifiedNode.NodeTheory

module L = FStar.List.Tot.Base
open Nemonuri.StratifiedNode
open Nemonuri.StratifiedNode.ListTheory

let get_level (#t:eqtype) (#lv:pos) (sn:stratified_node t lv) : Tot pos = lv

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

let rec sum (l:list nat)
  : Tot nat
  = match l with
  | [] -> 0
  | hd::tl -> hd + (sum tl)

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

let lemma_subchilren_hd_is_child
  (#t:eqtype) (#lv:pos) (parent:stratified_node t lv) 
  (#children_mlv:nat)
  (subchildren:stratified_node_list t children_mlv)
  : Lemma (requires (not (is_empty subchildren)) && (is_subchildren parent subchildren))
          (ensures is_child parent (get_hd subchildren))
  = ()

private let rec select_in_children_core
  (#t:eqtype) (#lv:pos) (parent:stratified_node t lv) 
  (#children_mlv:nat)
  (subchildren:stratified_node_list t children_mlv{ is_subchildren parent subchildren })
  (#t2:Type) (selector:(#clv:pos -> csn:(stratified_node t clv){ is_child parent csn } -> Tot t2))
  : Tot (x:(list t2){ L.length x = get_length subchildren }) 
        (decreases subchildren)
  = if is_empty subchildren then 
      []
    else
      (
        //assert ( is_child parent (get_hd subchildren) );
        let hd = get_hd subchildren in
        (selector #(get_level hd) hd)::
        (select_in_children_core parent (get_tl subchildren) selector)
      )


(*
let rec select_in_children
  (#t:eqtype) (#lv:pos) (sn:stratified_node t lv) 
  (#t2:Type) 
  (selector:(#clv:pos -> csn:(stratified_node t lv){ is_child csn sn } -> Tot t2))
  : Tot (x:(list t2){ L.length x = get_length sn.children })
  = let children = sn.children in
    if is_empty sn.children then []
    else
      (selector (get_hd children))::
      (select )
*)
  
//select sn.children selector


(*
let rec get_count
  (#t:eqtype) (#lv:pos) (sn:stratified_node t lv) 
  : Tot pos (decreases lv)
  = if is_leaf sn then 
      1
    else 
      sum (
        select sn.children (
          fun (#lv2:pos) (sn2:stratified_node t lv2) -> (
            assert (is_child sn2 sn);
            lemma_child_node_level_is_lower_than_parent sn2 sn;
            get_count #t #lv2 sn2
          )
        )
      )
*)