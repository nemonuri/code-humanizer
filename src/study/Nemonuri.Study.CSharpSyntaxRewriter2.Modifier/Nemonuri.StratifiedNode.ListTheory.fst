module Nemonuri.StratifiedNode.ListTheory

module L = FStar.List.Tot.Base
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
  : Tot nat (decreases snl)
  = if is_empty snl then
      0
    else
      1 + (get_length (get_tl snl))

// 오, 그러고보니 'L.length x = get_length snl' 이게 증명이 되네!
let rec select 
  (#t:eqtype) (#mlv:nat) (snl:stratified_node_list t mlv) 
  (#t2:Type) (selector:(#lv:pos -> stratified_node t lv -> Tot t2))
  : Tot (x:(list t2){ L.length x = get_length snl }) (decreases snl)
  = if is_empty snl then []
    else (selector (get_hd snl))::(select (get_tl snl) selector)

let rec get_node_level
  (#t:eqtype) (#mlv:nat) (snl:stratified_node_list t mlv)
  (index:nat{index < (get_length snl)})
  : Tot pos
  = if index = 0 then
      get_hd_level snl
    else
      get_node_level (get_tl snl) (index - 1)

let rec get_node
  (#t:eqtype) (#mlv:nat) (snl:stratified_node_list t mlv)
  (index:nat{index < (get_length snl)})
  : Tot (stratified_node t (get_node_level snl index))
  = if index = 0 then
      get_hd snl
    else
      get_node (get_tl snl) (index - 1)

let is_head 
  (#t:eqtype) (#mlv:pos) (snl:stratified_node_list t mlv)
  (#lv:pos) (sn:stratified_node t lv)
  : Tot bool
  = ((get_hd_level snl) = lv) && ((get_hd snl) = sn)

let rec contains
  (#t:eqtype) (#mlv:nat) (snl:stratified_node_list t mlv)
  (#lv:pos) (sn:stratified_node t lv)
  : Tot bool
  = if is_empty snl then
      false
    else
      if is_head snl sn then 
        true
      else
        contains (get_tl snl) sn

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
        //lemma_head_level_is_less_or_equal_then_list_max_level snl sn
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
