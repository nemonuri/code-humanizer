module Nemonuri.StratifiedNode.ListTheory

include Nemonuri.StratifiedNode.ListTheory.Base
include Nemonuri.StratifiedNode.ListTheory.Properties



(*
let stratified_node_head_level_is_less_or_equal_than_list_max_level
  (#t:eqtype) (#mlv:nat) (snl:stratified_node_list t mlv{SCons? snl})
  : prop
  = (get_hd_level snl) <= (mlv)

let stratified_node_max_level_is_greater_or_equal_than_tail_max_level
  (#t:eqtype) (#mlv:nat) (snl:stratified_node_list t mlv{SCons? snl})
  : prop
  = (get_tl_level snl) <= (mlv)

let rec lemma_stratified_node_list_max_level_is_greater_or_equal_than_list_element_level
  (#t:eqtype) (#mlv:nat) (snl:stratified_node_list t mlv{SCons? snl})
  : Lemma (
      (stratified_node_head_level_is_less_or_equal_than_list_max_level snl) /\
      (stratified_node_max_level_is_greater_or_equal_than_tail_max_level snl) /\
      (contains snl (get_hd snl))
    )
  = let tl = (get_tl snl) in
    if is_empty tl then ()
    else (lemma_stratified_node_list_max_level_is_greater_or_equal_than_list_element_level tl)
*)


(*
let stratified_node_level_is_less_or_equal_than_list_max_level
  (#t:eqtype) (#mlv:pos) (snl:stratified_node_list t mlv)
  (#lv:pos) (sn:stratified_node t lv)
  : Lemma (requires (get_hd snl) = sn)
          (ensures lv <= mlv)
  = ()
*)
