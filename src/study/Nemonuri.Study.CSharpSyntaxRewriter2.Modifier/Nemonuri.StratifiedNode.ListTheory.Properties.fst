module Nemonuri.StratifiedNode.ListTheory.Properties

module L = FStar.List.Tot.Base
open Nemonuri.StratifiedNode.ListTheory.Base
open Nemonuri.StratifiedNode

let lemma_stratified_node_list_which_max_level_is_zero_has_no_elements
  (#t:eqtype) (#mlv:nat) (snl:stratified_node_list t mlv)
  : Lemma (requires get_max_level snl = 0)
          (ensures SNil? snl)
  = ()

let rec lemma_stratified_node_list_which_max_level_greater_than_zero_has_a_element_which_level_is_max_level
  (#t:eqtype) (#mlv:nat) (snl:stratified_node_list t mlv)
  : Lemma (requires get_max_level snl > 0)
          (ensures 
            (SCons? snl) /\
            (
              ((get_hd_level snl) = (get_max_level snl)) \/
              ((get_tl_level snl) = (get_max_level snl))
            )
          )
          (decreases snl)
  = if SNil? (get_tl snl) then 
      ()
    else
      lemma_stratified_node_list_which_max_level_greater_than_zero_has_a_element_which_level_is_max_level (get_tl snl)

let lemma_head_level_is_less_or_equal_then_list_max_level
  (#t:eqtype) (#mlv:pos) (snl:stratified_node_list t mlv)
  (#lv:pos) (sn:stratified_node t lv)
  : Lemma (requires is_head snl sn)
          (ensures lv <= mlv)
  = ()

let lemma_node_is_head_implies_node_is_element
  (#t:eqtype) (#mlv:pos) (snl:stratified_node_list t mlv)
  (#lv:pos) (sn:stratified_node t lv)
  : Lemma (ensures ((is_head snl sn) ==> (contains snl sn)))
  = ()

let rec lemma_node_is_element_implies_node_level_is_less_or_equal_than_list_max_level
  (#t:eqtype) (#mlv:pos) (snl:stratified_node_list t mlv)
  (#lv:pos) (sn:stratified_node t lv)
  : Lemma (ensures (contains snl sn) ==> (lv <= mlv)) (decreases snl)
  = if (is_head snl sn) then
      (
        (* 음...둘 중 하나만 있어도 되네? *)
        lemma_head_level_is_less_or_equal_then_list_max_level snl sn;
        lemma_node_is_head_implies_node_is_element snl sn
      )
    else
      let tl = get_tl snl in
      let tl_level = get_max_level tl in
      if tl_level = 0 then ()
      else lemma_node_is_element_implies_node_level_is_less_or_equal_than_list_max_level tl sn

let lemma_element_level_is_less_or_equal_than_list_max_level
  (#t:eqtype) (#mlv:pos) (snl:stratified_node_list t mlv)
  (#lv:pos) (sn:stratified_node t lv)
  : Lemma (requires contains snl sn)
          (ensures lv <= mlv)
  = lemma_node_is_element_implies_node_level_is_less_or_equal_than_list_max_level snl sn

let lemma_two_list_are_equal_is_equivalent_to_two_heads_are_equal
  (#t:eqtype) (#l_mlv:nat) (l_snl:stratified_node_list t l_mlv{SCons? l_snl})
  (#r_mlv:nat) (r_snl:stratified_node_list t r_mlv{SCons? r_snl})
  : Lemma (requires is_equal l_snl r_snl)
          (ensures (get_hd l_snl) = (get_hd r_snl))
  = ()

let rec lemma_ends_with_snl1_snl2_implies_snl1_contains_snl2_head
  (#t:eqtype) (#mlv1:nat) (snl1:stratified_node_list t mlv1{SCons? snl1})
  (#mlv2:nat) (snl2:stratified_node_list t mlv2{SCons? snl2})
  : Lemma (requires (is_left_shorter_or_equal_than_right snl2 snl1) &&
                    (ends_with snl1 snl2) )
          (ensures contains snl1 (get_hd snl2))
          (decreases snl1)
  = if (is_equal snl1 snl2) then
      (
        lemma_two_list_are_equal_is_equivalent_to_two_heads_are_equal snl1 snl2
      )
    else
      lemma_ends_with_snl1_snl2_implies_snl1_contains_snl2_head (get_tl snl1) snl2

(*
let rec lemma_ends_with_snl1_snl2_implies_snl1_contains_snl2_head
  (#t:eqtype) (#mlv1:nat) (snl1:stratified_node_list t mlv1)
  (#mlv2:nat) (snl2:stratified_node_list t mlv2)
  : Lemma (ensures 
            (
              (not (is_empty snl2)) &&
              (is_left_shorter_or_equal_than_right snl2 snl1) && 
              (ends_with snl1 snl2)
            ) ==>
            (contains snl1 (get_hd snl2))
          )
          (decreases snl2)
  = if (is_empty snl2) then ()
    else if not (
      (not (is_empty snl2)) &&
      (is_left_shorter_or_equal_than_right snl2 snl1) && 
      (ends_with snl1 snl2)
    ) 
    then (lemma_ends_with_snl1_snl2_implies_snl1_contains_snl2_head snl1 (get_tl snl2))
    else
*)
