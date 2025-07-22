module Nemonuri.StratifiedNodes.Internals

module L = FStar.List.Tot
module Math = FStar.Math.Lib

//--- type definitions ---
type node_internal (t:eqtype) : pos -> Type =
  | SNode : 
      #children_level:nat -> 
      children:(node_list_internal t children_level) -> 
      value:t -> 
      node_internal t (children_level + 1)
and node_list_internal (t:eqtype) : nat -> Type =
  | SNil : 
      node_list_internal t 0
  | SCons : 
      #hd_level:pos ->
      #tl_level:nat ->
      hd:(node_internal t hd_level) ->
      tl:(node_list_internal t tl_level) ->
      node_list_internal t (Math.max hd_level tl_level)
//---|

//--- theory members ---
let rec select 
  #t #t2 #max_level (nl:node_list_internal t max_level)
  (selector:((#lv:pos) -> node_internal t lv -> Tot t2))
  : Tot (list t2) (decreases nl) =
  match nl with
  | SNil -> []
  | SCons #_ #_ hd tl -> (selector hd)::(select tl selector)

let rec contains
  #t #node_level #max_level
  (nd:node_internal t node_level) (nl:node_list_internal t max_level)
  : Tot bool (decreases nl) =
  match nl with
  | SNil -> false
  | SCons #_ #_ hd tl ->
      if (((SCons?.hd_level nl) = node_level) && (hd = nd)) then true
      else (contains nd tl)

let rec get_length
  #t #max_level (nl:node_list_internal t max_level)
  : Tot nat (decreases nl) =
  match nl with
  | SNil -> 0
  | SCons #_ #_ _ tl -> 1 + get_length tl
//---|

//--- proofs ---
let rec lemma_select_result_contains_selector_result
  #t (#t2:eqtype) #node_level #max_level (nl:node_list_internal t max_level)
  (selector:((#lv:pos) -> node_internal t lv -> Tot t2))
  (nd:node_internal t node_level) (selector_result:t2)
  : Lemma (requires (contains nd nl) && ((selector nd) = selector_result))
          (ensures L.contains selector_result (select nl selector))
          (decreases nl)
  = 
  let select_result = select nl selector in
  if (((SCons?.hd_level nl) = node_level) && ((SCons?.hd nl) = nd)) then
    assert ((L.hd select_result) = selector_result)
  else
    lemma_select_result_contains_selector_result (SCons?.tl nl) selector nd selector_result

let rec lemma_node_list_internal_level_is_greater_or_equal_than_element_level
  #t #node_level #max_level
  (nd:node_internal t node_level) (nl:node_list_internal t max_level)
  : Lemma (requires contains nd nl)
          (ensures max_level >= node_level)
          (decreases nl)
  =
  if (((SCons?.hd_level nl) = node_level) && ((SCons?.hd nl) = nd)) then ()
  else lemma_node_list_internal_level_is_greater_or_equal_than_element_level nd (SCons?.tl nl)
//---|


