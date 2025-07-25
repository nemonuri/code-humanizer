module Nemonuri.StratifiedNodes.Nodes.Equivalence

module L = FStar.List.Tot
module I = Nemonuri.StratifiedNodes.Internals
module N = Nemonuri.StratifiedNodes.Nodes.Base
open Nemonuri.StratifiedNodes.Nodes.Bijections

//--- private theory members ---

private let are_equivalent_as_node_impl #t #node_level 
  (node:N.node t) (node_internal:I.node_internal t node_level)
  : Tot bool =
  (to_node node_internal = node)

private let are_equivalent_as_node_list_impl #t #node_list_level
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
  ((are_equivalent_as_node_list_impl node_list node_list_internal) /\
  (index < (L.length node_list))) ==>
  (are_equivalent_as_node_impl 
    (L.index node_list index) (I.get_item node_list_internal index))

let equivalent_as_node_list_entails_any_item_pair_are_equivalent_as_node
  (t: eqtype)
  : prop =
  forall (node_list_level:nat) (node_list:N.node_list t)
  (node_list_internal:I.node_list_internal t node_list_level) (index:nat).
  (are_equivalent_as_node_list_entails_get_item_pair_are_equivalent_as_node
    node_list node_list_internal index)

let equivalent_as_node_list_entails_both_lengths_are_equal
  (t: eqtype)
  : prop =
  forall (node_list_level:nat) (node_list:N.node_list t)
  (node_list_internal:I.node_list_internal t node_list_level).
  (L.length node_list) = (I.get_length node_list_internal)

let equivalent_theorem (t:eqtype) : prop =
  equivalent_as_node_list_entails_any_item_pair_are_equivalent_as_node t

//---|

//--- lemma ---

let rec lemma_are_equivalent_as_node_list_entails_get_item_pair_are_equivalent_as_node
  #t #node_list_level
  (node_list:N.node_list t) 
  (node_list_internal:I.node_list_internal t node_list_level)
  (index:nat)
  : Lemma 
    (ensures are_equivalent_as_node_list_entails_get_item_pair_are_equivalent_as_node
      node_list node_list_internal index)
    (decreases node_list)
  =
  if not (are_equivalent_as_node_list_impl node_list node_list_internal) then ()
  else if not (index < (L.length node_list)) then ()
  else
    if index = 0 then
      assert (are_equivalent_as_node_impl (L.hd node_list) (I.SCons?.hd node_list_internal))
    else 
      lemma_are_equivalent_as_node_list_entails_get_item_pair_are_equivalent_as_node
        (L.tl node_list) (I.SCons?.tl node_list_internal) (index - 1)

let lemma_equivalent_as_node_list_entails_any_item_pair_are_equivalent_as_node (t: eqtype)
  : Lemma (ensures equivalent_as_node_list_entails_any_item_pair_are_equivalent_as_node t)
  =
  introduce forall (node_list_level:nat) (node_list:N.node_list t)
  (node_list_internal:I.node_list_internal t node_list_level) (index:nat).
  (are_equivalent_as_node_list_entails_get_item_pair_are_equivalent_as_node
    node_list node_list_internal index) with
  (lemma_are_equivalent_as_node_list_entails_get_item_pair_are_equivalent_as_node
    node_list node_list_internal index)

let lemma_equivalent_as_node_list_entails_both_lengths_are_equal (t: eqtype)
  : Lemma (ensures equivalent_as_node_list_entails_both_lengths_are_equal t)
  =
  ()
  //lemma_to_node_list_theorem t

let lemma_equivalent_theorem (t: eqtype)
  : Lemma (ensures equivalent_theorem t)
  =
  lemma_equivalent_as_node_list_entails_any_item_pair_are_equivalent_as_node t

//---|

//--- theory members ---

let are_equivalent_as_node #t #node_level 
  (node:N.node t) (node_internal:I.node_internal t node_level)
  : Pure bool True 
    (ensures fun _ -> equivalent_theorem t)
  =
  lemma_equivalent_theorem t;
  are_equivalent_as_node_impl node node_internal

let are_equivalent_as_node_list #t #node_list_level
  (node_list:N.node_list t) (node_list_internal:I.node_list_internal t node_list_level)
  : Pure bool True
    (ensures fun _ -> equivalent_theorem t)
  =
  lemma_equivalent_theorem t;
  are_equivalent_as_node_list_impl node_list node_list_internal

//---|