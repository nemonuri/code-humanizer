module Nemonuri.StratifiedNodes.Nodes.Bijections.ToNodeList

module L = FStar.List.Tot
module I = Nemonuri.StratifiedNodes.Internals
module B = Nemonuri.StratifiedNodes.Nodes.Base
open Nemonuri.StratifiedNodes.Nodes.Bijections.ToNode

//--- private theory members ---
private let to_node_list_impl #t #max_level 
  (node_list_internal:I.node_list_internal t max_level) 
  : Tot (B.node_list t) =
  I.select node_list_internal (fun ni -> to_node ni)  

private let rec to_node_list_inverse_impl
  #t (l:B.node_list t)
  : Tot (I.node_list_internal t (B.get_list_level l))
        (decreases l)
  =
  match l with
  | [] -> I.SNil
  | hd::tl ->
      let hd2 = to_node_inverse hd in
      let tl2 = to_node_list_inverse_impl tl in
      I.SCons hd2 tl2
//---|

//--- propositions ---
let applying_to_node_list_inverse_after_to_node_list_is_identity 
  #t #node_list_internal_level
  (node_list_internal:I.node_list_internal t node_list_internal_level) : prop =
  let result = to_node_list_inverse_impl (to_node_list_impl node_list_internal) in
  (I.get_list_level result = node_list_internal_level) &&
  (result = node_list_internal)

let applying_to_node_list_after_to_node_list_inverse_is_identity 
  #t (node_list:B.node_list t) : prop =
  (to_node_list_impl (to_node_list_inverse_impl node_list)) = node_list

let to_node_list_is_bijection (t:eqtype) : prop =
  (forall (node_list_internal_level:nat) 
    (node_list_internal:I.node_list_internal t node_list_internal_level).
    applying_to_node_list_inverse_after_to_node_list_is_identity node_list_internal) /\
  (forall (node_list:B.node_list t). applying_to_node_list_after_to_node_list_inverse_is_identity node_list)

let to_node_list_and_to_node_list_inverse_are_bijection_pair (t:eqtype) : prop =
  (forall (node_list_internal_level:nat) 
    (node_list_internal:I.node_list_internal t node_list_internal_level) 
    (node_list:B.node_list t).
    ((to_node_list_impl node_list_internal) = node_list) <==> 
    ((B.get_list_level node_list = node_list_internal_level) && 
     (to_node_list_inverse_impl node_list = node_list_internal))
  )

let length_is_invariant_of_to_node_list 
  #t #node_list_internal_level
  (node_list_internal:I.node_list_internal t node_list_internal_level) : prop = 
  (I.get_length node_list_internal) = (L.length (to_node_list_impl node_list_internal))

let length_is_invariant_of_to_node_list_inverse
  #t (node_list:B.node_list t) : prop =
  (I.get_length (to_node_list_inverse_impl node_list)) = (L.length node_list)

let to_node_list_and_to_node_list_inverse_have_length_invariant (t:eqtype) : prop =
  (forall (node_list_internal_level:nat) 
    (node_list_internal:I.node_list_internal t node_list_internal_level) 
    (node_list:B.node_list t).
    (length_is_invariant_of_to_node_list node_list_internal) /\
    (length_is_invariant_of_to_node_list_inverse node_list)
  )

let to_node_list_theorem (t:eqtype) : prop =
  to_node_list_is_bijection t /\
  to_node_list_and_to_node_list_inverse_are_bijection_pair t /\
  to_node_list_and_to_node_list_inverse_have_length_invariant t
//---|

//--- lemma ---
let rec lemma_applying_to_node_list_inverse_after_to_node_list_is_identity
  #t #node_list_internal_level
  (node_list_internal:I.node_list_internal t node_list_internal_level)
  : Lemma (ensures applying_to_node_list_inverse_after_to_node_list_is_identity node_list_internal)
          (decreases node_list_internal)
  =
  match node_list_internal with
  | I.SNil -> ()
  | I.SCons hd tl -> lemma_applying_to_node_list_inverse_after_to_node_list_is_identity tl

let rec lemma_applying_to_node_list_after_to_node_list_inverse_is_identity
  #t (node_list:B.node_list t) 
  : Lemma (ensures applying_to_node_list_after_to_node_list_inverse_is_identity node_list)
          (decreases node_list)
  = 
  match node_list with
  | [] -> ()
  | hd::tl -> (
      assert ((to_node (to_node_inverse hd)) = hd);
      lemma_applying_to_node_list_after_to_node_list_inverse_is_identity tl
    )

let lemma_to_node_list_is_bijection (t:eqtype)
  : Lemma (ensures to_node_list_is_bijection t)
  =
  (introduce forall (node_list_internal_level:nat) 
    (node_list_internal:I.node_list_internal t node_list_internal_level).
    applying_to_node_list_inverse_after_to_node_list_is_identity node_list_internal with
    lemma_applying_to_node_list_inverse_after_to_node_list_is_identity node_list_internal);
  (introduce forall (node_list:B.node_list t).
    applying_to_node_list_after_to_node_list_inverse_is_identity node_list with
    lemma_applying_to_node_list_after_to_node_list_inverse_is_identity node_list)

let lemma_to_node_list_and_to_node_list_inverse_are_bijection_pair (t:eqtype)
  : Lemma (ensures to_node_list_and_to_node_list_inverse_are_bijection_pair t)
  =
  lemma_to_node_list_is_bijection t

let lemma_length_is_invariant_of_to_node_list
  #t #node_list_internal_level
  (node_list_internal:I.node_list_internal t node_list_internal_level)
  : Lemma (ensures length_is_invariant_of_to_node_list node_list_internal)
  =
  lemma_to_node_list_is_bijection t

let lemma_length_is_invariant_of_to_node_list_inverse
  #t (node_list:B.node_list t)
  : Lemma (ensures length_is_invariant_of_to_node_list_inverse node_list)
  =
  lemma_to_node_list_is_bijection t

let lemma_to_node_list_and_to_node_list_inverse_have_length_invariant (t:eqtype) 
  : Lemma (ensures to_node_list_and_to_node_list_inverse_have_length_invariant t)
  =
  (introduce forall (node_list_internal_level:nat) 
    (node_list_internal:I.node_list_internal t node_list_internal_level).
    length_is_invariant_of_to_node_list node_list_internal with
    lemma_length_is_invariant_of_to_node_list node_list_internal);
  (introduce forall (node_list:B.node_list t).
    length_is_invariant_of_to_node_list_inverse node_list with
    lemma_length_is_invariant_of_to_node_list_inverse node_list)

let lemma_to_node_list_theorem (t:eqtype) 
  : Lemma (ensures to_node_list_theorem t)
  =
  lemma_to_node_list_is_bijection t;
  lemma_to_node_list_and_to_node_list_inverse_are_bijection_pair t;
  lemma_to_node_list_and_to_node_list_inverse_have_length_invariant t
//---|

//--- theory members ---
let to_node_list #t #max_level 
  (node_list_internal:I.node_list_internal t max_level) 
  : Pure (B.node_list t) True 
    (ensures fun _ -> to_node_list_theorem t)
  =
  lemma_to_node_list_theorem t;
  to_node_list_impl node_list_internal

let to_node_list_inverse
  #t (node_list:B.node_list t)
  : Pure (I.node_list_internal t (B.get_list_level node_list)) True
    (ensures fun _ -> to_node_list_theorem t)
  =
  lemma_to_node_list_theorem t;
  to_node_list_inverse_impl node_list
//---|