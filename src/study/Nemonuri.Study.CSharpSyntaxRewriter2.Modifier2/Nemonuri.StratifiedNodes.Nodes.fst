module Nemonuri.StratifiedNodes.Nodes

module L = FStar.List.Tot
module Math = FStar.Math.Lib
module I = Nemonuri.StratifiedNodes.Internals

//--- type definitions ---
type node (t:eqtype) =
  | SNode : 
      level:pos ->
      internal:(I.node_internal t level) ->
      node t

let node_list (t:eqtype) = list (node t)
//---|

//--- theory members ---
let to_node #t #level (node_internal:I.node_internal t level) : Tot (node t) =
  SNode level node_internal

let to_node_list #t #max_level 
  (node_list_internal:I.node_list_internal t max_level) 
  : Tot (node_list t) =
  I.select node_list_internal (fun ni -> to_node ni)  
//---|

//--- (node t) members ---
let get_level #t (node:node t) : Tot pos = node.level

let get_value #t (node:node t) : Tot t = node.internal.value

let get_children #t (node:node t) : Tot (node_list t) =
  to_node_list node.internal.children
//---|

//--- (node_list t) members ---
let rec get_list_level #t (l:node_list t) : Tot nat =
  match l with
  | [] -> 0
  | hd::tl -> Math.max (get_level hd) (get_list_level tl)
//---|

//--- propositions ---
//let node_internal_children_and_get_children_result_

let node_level_is_greater_than_levels_of_nodes_in_children 
  #t (node1:node t) : Tot prop =
  forall (node2:node t). 
    (L.contains node2 (get_children node1)) ==> ((get_level node1) > (get_level node2))
//---|

//--- theory members for proofs ---
private let to_node_inverse #t (nd:node t) : Tot (I.node_internal t (get_level nd)) =
  nd.internal

private let rec to_node_list_inverse
  #t (l:node_list t)
  : Tot (I.node_list_internal t (get_list_level l))
        (decreases l)
  =
  match l with
  | [] -> I.SNil
  | hd::tl ->
      let hd2 = to_node_inverse hd in
      let tl2 = to_node_list_inverse tl in
      I.SCons hd2 tl2


(*
  : Tot (I.node_list_internal t (
      match l with
      | [] -> node_list_internal_level
      | hd::_ -> Math.max (get_level hd) node_list_internal_level
    )) 
    (decreases l)
*)

(*
private let rec to_node_list_inverse #t (l:node_list t) 
  : Tot (I.node_list_internal t (get_max_level l)) 
        (decreases l)     
  =
  match l with
  | [] -> I.SNil
  | hd::tl -> 
      let hd2 = to_node_inverse hd in
      I.SCons hd2 (to_node_list_inverse tl)
  //I.SCons 
*)
//---|

//--- proofs ---
open FStar.Classical.Sugar

let lemma_to_node_is_bijection #t
  : Lemma (ensures forall (lv:pos) (ni:I.node_internal t lv). (to_node_inverse (to_node ni)) = ni)
  =
  ()

//let lemma_to_node_

(*
let lemma_node1_children_contains_node2_entails_node1_internal_children_contains_node2_internal
  #t (node1:node t) (node2:node t)
  : Lemma (requires (L.contains node2 (get_children node1)))
          (ensures (I.contains node2.internal node1.internal.children))
          
  =
  introduce forall (node1:node t) (node2:node t)
*)


let lemma_node_level_is_greater_than_level_of_node_in_children
  #t (node1:node t) (node2:node t)
  : Lemma (requires (L.contains node2 (get_children node1)))
          (ensures ((get_level node1) > (get_level node2)))
  =
  assert ((to_node node2.internal) = (node2));
  assert (I.contains node2.internal node1.internal.children);
  I.lemma_node_list_internal_level_is_greater_or_equal_than_element_level node2.internal node1.internal.children



(*
let lemma_node_level_is_greater_than_levels_of_nodes_in_children
  #t (node1:node t)
  : Lemma (ensures node_level_is_greater_than_levels_of_nodes_in_children node1) =
  introduce forall (node2:node t{L.contains node2 (get_children node1)}). 
    (I.contains node2.internal node1.internal.children) with ((to_node node2.internal) = (node2))
*)
//---|
  