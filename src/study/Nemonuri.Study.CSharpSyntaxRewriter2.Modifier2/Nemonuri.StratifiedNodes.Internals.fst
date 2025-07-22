module Nemonuri.StratifiedNodes.Internals

module L = FStar.List.Tot

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
      node_list_internal t (if hd_level >= tl_level then hd_level else tl_level)
//---|

//--- theory members ---
let rec select 
  #t #t2 #max_level (nl:node_list_internal t max_level)
  (selector:((#lv:pos) -> node_internal t lv -> Tot t2))
  : Tot (list t2) (decreases nl) =
  match nl with
  | SNil -> []
  | SCons #_ #_ hd tl -> (selector hd)::(select tl selector)
//---|


