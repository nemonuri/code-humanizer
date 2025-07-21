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

let lemma_two_list_are_equal_means_two_heads_are_equal
  (#t:eqtype) (#l_mlv:nat) (l_snl:stratified_node_list t l_mlv{SCons? l_snl})
  (#r_mlv:nat) (r_snl:stratified_node_list t r_mlv{SCons? r_snl})
  : Lemma (requires is_equal_list l_snl r_snl)
          (ensures (get_hd l_snl) = (get_hd r_snl))
  = ()

let rec lemma_snl1_ends_with_snl2_means_snl1_contains_snl2_head
  (#t:eqtype) (#mlv1:nat) (snl1:stratified_node_list t mlv1{SCons? snl1})
  (#mlv2:nat) (snl2:stratified_node_list t mlv2{SCons? snl2})
  : Lemma (requires (is_left_shorter_or_equal_than_right snl2 snl1) &&
                    (ends_with snl1 snl2) )
          (ensures contains snl1 (get_hd snl2))
          (decreases snl1)
  = if (is_equal_list snl1 snl2) then
      (
        lemma_two_list_are_equal_means_two_heads_are_equal snl1 snl2
      )
    else
      lemma_snl1_ends_with_snl2_means_snl1_contains_snl2_head (get_tl snl1) snl2

let lemma_two_list_are_equal_means_two_tails_are_equal
  (#t:eqtype) (#l_mlv:nat) (l_snl:stratified_node_list t l_mlv{SCons? l_snl})
  (#r_mlv:nat) (r_snl:stratified_node_list t r_mlv{SCons? r_snl})
  : Lemma (requires is_equal_list l_snl r_snl)
          (ensures (get_tl l_snl) = (get_tl r_snl))
  = ()

let rec lemma_snl1_ends_with_snl2_means_snl1_ends_with_snl2_tl
  (#t:eqtype) (#mlv1:nat) (snl1:stratified_node_list t mlv1{SCons? snl1})
  (#mlv2:nat) (snl2:stratified_node_list t mlv2{SCons? snl2})
  : Lemma (requires (is_left_shorter_or_equal_than_right snl2 snl1) &&
                    (ends_with snl1 snl2) )
          (ensures ends_with snl1 (get_tl snl2))
          (decreases snl1)
  = if (is_equal_list snl1 snl2) then
      (
        lemma_two_list_are_equal_means_two_tails_are_equal snl1 snl2
      )
    else
      lemma_snl1_ends_with_snl2_means_snl1_ends_with_snl2_tl (get_tl snl1) snl2

let rec lemma_list_satisfies_for_all_predicate_means_head_and_tail_satisfy_predicate
  (#t:eqtype) (#mlv:nat) (snl:stratified_node_list t mlv{SCons? snl})
  (predicate:stratified_node_predicate t)
  : Lemma (requires for_all snl predicate)
          (ensures (predicate (get_hd snl)) && (for_all (get_tl snl) predicate))
          (decreases snl)
  = if (get_length snl = 1) then ()
    else
      lemma_list_satisfies_for_all_predicate_means_head_and_tail_satisfy_predicate (get_tl snl) predicate

let rec lemma_list_satisfies_for_all_predicate_means_element_satisfies_predicate
  (#t:eqtype) (#mlv:nat) (snl:stratified_node_list t mlv{SCons? snl})
  (#lv:pos) (sn:stratified_node t lv)
  (predicate:stratified_node_predicate t)
  : Lemma (requires (for_all snl predicate) && (contains snl sn))
          (ensures predicate sn)
          (decreases snl)
  = lemma_list_satisfies_for_all_predicate_means_head_and_tail_satisfy_predicate snl predicate;
    if (get_hd_level snl) = lv && (get_hd snl) = sn then ()
    else
      lemma_list_satisfies_for_all_predicate_means_element_satisfies_predicate (get_tl snl) sn predicate
      
  