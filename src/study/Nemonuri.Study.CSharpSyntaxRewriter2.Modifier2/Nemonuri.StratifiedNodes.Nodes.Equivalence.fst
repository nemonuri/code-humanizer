module Nemonuri.StratifiedNodes.Nodes.Equivalence

module L = FStar.List.Tot
module I = Nemonuri.StratifiedNodes.Internals
module N = Nemonuri.StratifiedNodes.Nodes.Base
open Nemonuri.StratifiedNodes.Nodes.Bijections

//--- theory members ---

let are_equivalent_as_node #t #node_level 
  (node:N.node t) (node_internal:I.node_internal t node_level)
  : Tot bool =
  (to_node node_internal = node)

let are_equivalent_as_node_list #t #node_list_level
  (node_list:N.node_list t) (node_list_internal:I.node_list_internal t node_list_level)
  : Pure bool True
    (ensures fun b -> 
      if b then 
        (L.length node_list) = (I.get_length node_list_internal)
      else true
    )
  =
  (to_node_list node_list_internal = node_list)

//---|

//--- propositions ---

let are_equivalent_as_node_list_entails_get_item_pair_are_equivalent_as_node
  #t #node_list_level
  (node_list:N.node_list t) 
  (node_list_internal:I.node_list_internal t node_list_level)
  (index:nat)
  : prop =
  ((are_equivalent_as_node_list node_list node_list_internal) /\
  (index < (L.length node_list))) ==>
  (are_equivalent_as_node 
    (L.index node_list index) (I.get_item node_list_internal index))

//---|

//--- lemma ---

(*
let lemma_are_equivalent_as_node_list_entails_get_item_pair_are_equivalent_as_node
  #t #node_list_level
  (node_list:N.node_list t) 
  (node_list_internal:I.node_list_internal t node_list_level)
  (index:nat)
  : Lemma 
    (requires (are_equivalent_as_node_list node_list node_list_internal) /\
      (index < (L.length node_list)))
    (ensures (are_equivalent_as_node 
      (L.index node_list index) (I.get_item node_list_internal index)))
  =
  ()
*)

//---|