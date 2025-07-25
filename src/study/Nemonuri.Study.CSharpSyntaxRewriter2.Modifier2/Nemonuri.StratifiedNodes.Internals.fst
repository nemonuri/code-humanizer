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
let rec get_length
  #t #max_level (nl:node_list_internal t max_level)
  : Tot nat (decreases nl) =
  match nl with
  | SNil -> 0
  | SCons #_ #_ _ tl -> 1 + get_length tl

let rec select 
  #t #t2 #max_level (nl:node_list_internal t max_level)
  (selector:((#lv:pos) -> node_internal t lv -> Tot t2))
  : Pure (list t2) True
    (ensures fun r -> (L.length r) = (get_length nl))
    (decreases nl) 
  =
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

let get_level
  #t #node_level (nd:node_internal t node_level) : Tot pos =
  node_level

let get_list_level
  #t #max_level (nl:node_list_internal t max_level) : Tot nat =
  max_level

let can_get_item
  #t #list_level (nl:node_list_internal t list_level) (index:nat)
  : Tot bool =
  (index < (get_length nl))

let rec get_item_level
  #t #list_level (nl:node_list_internal t list_level) (index:nat)
  : Pure pos
    (requires can_get_item nl index)
    (ensures fun r -> r <= get_list_level nl)
    (decreases nl)
  =
  let SCons hd tl = nl in
  if index = 0 then 
    get_level hd
  else
    get_item_level tl (index - 1)
    
#push-options "--query_stats"
let rec get_item
  #t #list_level (nl:node_list_internal t list_level) (index:nat{can_get_item nl index})
  : Pure (node_internal t (get_item_level nl index))
    //(requires can_get_item nl index)
    (requires True)
    (ensures fun r -> contains r nl)
    (decreases nl)
  =
  let SCons hd tl = nl in
  if index = 0 then
    hd
  else
    get_item tl (index - 1)
#pop-options


//let get_element_at
//  #t #max_level 
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


