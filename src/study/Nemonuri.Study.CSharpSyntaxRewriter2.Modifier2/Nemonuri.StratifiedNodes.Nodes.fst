module Nemonuri.StratifiedNodes.Nodes

module L = FStar.List.Tot
module Math = FStar.Math.Lib
module I = Nemonuri.StratifiedNodes.Internals
module Base = Nemonuri.StratifiedNodes.Nodes.Base
include Nemonuri.StratifiedNodes.Nodes.Bijections

//--- (Base.node t) members ---
let get_children #t (node:Base.node t) : Tot (Base.node_list t) =
  to_node_list node.internal.children

let get_children_length #t (node:Base.node  t) 
  : Pure nat (requires True) (ensures fun r -> r = L.length (get_children node)) = 
  //assume (I.get_length node.internal.children = L.length (get_children node));
  I.get_length node.internal.children

let get_child_at #t (nd:Base.node  t) (index:nat)
  : Pure (Base.node  t) 
    (requires (index < (get_children_length nd)))
    (ensures fun r -> r = (L.index (get_children nd) index))
  =
  // TODO: Internals 단계에서 구현하기
  L.index (get_children nd) index

let is_leaf #t (node:Base.node  t) : Tot bool = I.SNil? node.internal.children

let is_branch #t (node:Base.node  t) : Tot bool = not (is_leaf node)
//---|

//--- propositions ---
let node_level_is_greater_than_levels_of_nodes_in_children (t:eqtype)
  : Tot prop =
  forall (node1:Base.node  t) (node2:Base.node  t). 
    (L.contains node2 (get_children node1)) ==> ((Base.get_level node1) > (Base.get_level node2))
//---|

//--- proofs ---
open FStar.Classical.Sugar

let lemma_to_node_is_bijection t
  : Lemma (ensures forall (lv:pos) (ni:I.node_internal t lv). (to_node_inverse (to_node ni)) = ni)
  =
  ()

private let rec lemma_to_node_list_is_bijection_aux #t #lv
  (nl:I.node_list_internal t lv) 
  : Lemma (ensures 
            (lv = (Base.get_list_level (to_node_list nl))) && 
            (to_node_list_inverse (to_node_list nl)) = nl)
          (decreases nl)
  =
  match nl with
  | I.SNil -> ()
  | I.SCons #_ #_ hd tl -> lemma_to_node_list_is_bijection_aux tl

let lemma_to_node_list_is_bijection t
  : Lemma (ensures forall (lv:pos) (nl:I.node_list_internal t lv). 
                     (lv = (Base.get_list_level (to_node_list nl))) && 
                     (to_node_list_inverse (to_node_list nl)) = nl)
  =
  introduce forall (lv2:pos) (nl2:I.node_list_internal t lv2).
    (lv2 = (Base.get_list_level (to_node_list nl2))) && (to_node_list_inverse (to_node_list nl2)) = nl2
    with lemma_to_node_list_is_bijection_aux nl2

let rec lemma_to_node_list_result_contains_to_node_result_entails_argument_node_list_internal_contains_argument_node_internal
  #t #node_level #node_list_level
  (node_internal:I.node_internal t node_level)
  (node_list_internal:I.node_list_internal t node_list_level)
  : Lemma (requires (L.contains (to_node node_internal) (to_node_list node_list_internal)))
          (ensures (I.contains node_internal node_list_internal))
          (decreases node_list_internal)
  = 
  let nd = to_node node_internal in
  let nl = to_node_list node_list_internal in
  //lemma_to_node_is_bijection t;
  //lemma_to_node_list_is_bijection t;
  if (((I.SCons?.hd_level node_list_internal) = node_level) && ((I.SCons?.hd node_list_internal) = node_internal)) then
    ()
  else
    lemma_to_node_list_result_contains_to_node_result_entails_argument_node_list_internal_contains_argument_node_internal node_internal (I.SCons?.tl node_list_internal)

let lemma_node1_children_contains_node2_entails_node1_internal_children_contains_node2_internal
  #t (node1:Base.node  t) (node2:Base.node  t)
  : Lemma (requires (L.contains node2 (get_children node1)))
          (ensures (I.contains node2.internal node1.internal.children))      
  = 
  let node1_internal_children = to_node_list_inverse (get_children node1) in
  let node2_internal = to_node_inverse node2 in
  lemma_to_node_is_bijection t;
  lemma_to_node_list_is_bijection t;
  lemma_to_node_list_result_contains_to_node_result_entails_argument_node_list_internal_contains_argument_node_internal node2_internal node1_internal_children //;
  //assert (node1_internal_children = node1.internal.children);
  //assert (node2_internal = node2.internal);
  //assert (I.contains node2_internal node1_internal_children)

let lemma_node_level_is_greater_than_level_of_node_in_children
  #t (node1:Base.node  t) (node2:Base.node  t)
  : Lemma (requires (L.contains node2 (get_children node1)))
          (ensures ((Base.get_level node1) > (Base.get_level node2)))
  =
  assert ((to_node node2.internal) = (node2));
  lemma_node1_children_contains_node2_entails_node1_internal_children_contains_node2_internal node1 node2;
  assert (I.contains node2.internal node1.internal.children);
  I.lemma_node_list_internal_level_is_greater_or_equal_than_element_level node2.internal node1.internal.children

let lemma_node_level_is_greater_than_levels_of_nodes_in_children (t:eqtype)
  : Lemma (ensures node_level_is_greater_than_levels_of_nodes_in_children t) =
  introduce 
    forall (node1:Base.node t) (node2:Base.node t{L.contains node2 (get_children node1)}).
      ((Base.get_level node1) > (Base.get_level node2))
    with (lemma_node_level_is_greater_than_level_of_node_in_children node1 node2)
//---|

include Nemonuri.StratifiedNodes.Nodes.Base