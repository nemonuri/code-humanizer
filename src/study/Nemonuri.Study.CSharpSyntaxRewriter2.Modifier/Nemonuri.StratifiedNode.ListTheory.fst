module Nemonuri.StratifiedNode.ListTheory

open Nemonuri.StratifiedNode

let get_max_level (#t:eqtype) (#mlv:nat) (snl:stratified_node_list t mlv) : Tot nat = mlv

let stratified_node_list_which_max_level_is_zero_has_no_elements
  (#t:eqtype) (#mlv:nat) (snl:stratified_node_list t mlv)
  : Lemma (requires get_max_level snl = 0)
          (ensures SNil? snl)
  = ()

let get_hd_level (#t:eqtype) (#lv:pos) (snl:stratified_node_list t lv{SCons? snl})
  : Tot pos
  = SCons?.hd_level snl

let get_hd 
  (#t:eqtype) (#mlv:nat) (snl:stratified_node_list t mlv{SCons? snl}) 
  : Tot (stratified_node t (get_hd_level snl))
  = SCons?.hd snl

let get_tl_level (#t:eqtype) (#lv:pos) (snl:stratified_node_list t lv{SCons? snl})
  : Tot nat
  = SCons?.tl_level snl

let get_tl
  (#t:eqtype) (#lv:pos) (snl:stratified_node_list t lv{SCons? snl})
  : Tot (stratified_node_list t (get_tl_level snl))
  = SCons?.tl snl

let hd_tl_pair (#t:eqtype) (#lv:pos) (snl:stratified_node_list t lv{SCons? snl}) =
  stratified_node t (get_hd_level snl) & stratified_node_list t (get_tl_level snl)

let get_hd_tl_pair (#t:eqtype) (#lv:pos) (snl:stratified_node_list t lv{SCons? snl})
  : Tot (hd_tl_pair snl)
  = (get_hd snl, get_tl snl)

let try_get_hd_tl (#t:eqtype) (#lv:pos) (snl:stratified_node_list t lv)
  : Tot (option (hd_tl_pair snl))
  = if (SCons? snl) then
      Some (get_hd_tl_pair snl)
    else
      None


let rec stratified_node_list_which_max_level_greater_than_zero_has_a_element_which_level_is_max_level
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
      stratified_node_list_which_max_level_greater_than_zero_has_a_element_which_level_is_max_level (get_tl snl)
    

let is_empty (#t:eqtype) (#mlv:nat) (snl:stratified_node_list t mlv) 
  : Tot bool
  = SNil? snl

let rec get_length (#t:eqtype) (#mlv:nat) (snl:stratified_node_list t mlv) 
  : Tot nat
  = if is_empty snl then
      0
    else
      1 + (get_length (get_tl snl))
