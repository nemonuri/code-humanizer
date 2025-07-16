module Nemonuri.StratifiedNode.ListTheory.Base

module L = FStar.List.Tot.Base
open Nemonuri.StratifiedNode

let get_max_level (#t:eqtype) (#mlv:nat) (snl:stratified_node_list t mlv) : Tot nat = mlv

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
  (#t2:Type) (selector:stratified_node_func t t2)
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

let is_left_shorter_or_equal_than_right
  (#t:eqtype) (#l_mlv:nat) (l_snl:stratified_node_list t l_mlv)
  (#r_mlv:nat) (r_snl:stratified_node_list t r_mlv)
  : Tot bool
  = (get_length l_snl) <= (get_length r_snl)

let is_left_length_is_right_length_minus_one
  (#t:eqtype) (#l_mlv:nat) (l_snl:stratified_node_list t l_mlv)
  (#r_mlv:nat) (r_snl:stratified_node_list t r_mlv)
  : Tot bool
  = (get_length l_snl) = ((get_length r_snl) - 1)

let is_tail
  (#t:eqtype) (#mlv:pos) (snl:stratified_node_list t mlv)
  (#tail_mlv:nat) (tail_snl:stratified_node_list t tail_mlv{ is_left_length_is_right_length_minus_one tail_snl snl })
  : Tot bool
  = ((get_tl_level snl) = tail_mlv) && ((get_tl snl) = tail_snl)

let is_equal
  (#t:eqtype) (#l_mlv:nat) (l_snl:stratified_node_list t l_mlv)
  (#r_mlv:nat) (r_snl:stratified_node_list t r_mlv)
  : Tot bool
  = (l_mlv = r_mlv) && (l_snl = r_snl)

let rec ends_with
  (#t:eqtype) (#mlv:nat) (snl:stratified_node_list t mlv)
  (#end_mlv:nat) (end_snl:stratified_node_list t end_mlv{ is_left_shorter_or_equal_than_right end_snl snl })
  : Tot bool (decreases snl)
  = if (is_empty end_snl) then true
    else if (is_equal snl end_snl) then true
    (* else
    if (is_left_length_is_right_length_minus_one end_snl snl) then 
      is_tail snl end_snl *)
    else
      let next_snl = get_tl snl in
      if is_left_shorter_or_equal_than_right end_snl next_snl then
        ends_with next_snl end_snl
      else
        false

let rec for_all
  (#t:eqtype) (#mlv:nat) (snl:stratified_node_list t mlv)
  (predicate:stratified_node_predicate t)
  : Tot bool (decreases snl)
  = if is_empty snl then 
      true
    else
      (predicate (get_hd snl)) && (for_all (get_tl snl) predicate)

