module Nemonuri.StratifiedNodes.Nodes.Bijections.ToNode

module I = Nemonuri.StratifiedNodes.Internals
module B = Nemonuri.StratifiedNodes.Nodes.Base
//open Nemonuri.StratifiedNodes.Nodes.Base.Members

//--- private theory members ---
private let to_node_impl #t #level (node_internal:I.node_internal t level) : Tot (B.node t) =
  B.SNode level node_internal

private let to_node_inverse_impl #t (node:B.node t) : Tot (I.node_internal t (B.get_level node)) =
  node.internal
//---|

//--- propositions ---
let applying_to_node_inverse_after_to_node_is_identity 
  #t #node_internal_level (node_internal:I.node_internal t node_internal_level) : prop =
  let result = to_node_inverse_impl (to_node_impl node_internal) in
  (I.get_level node_internal = node_internal_level) &&
  (to_node_inverse_impl (to_node_impl node_internal) = node_internal)

let applying_to_node_after_to_node_inverse_is_identity
  #t (node:B.node t) : prop =
  to_node_impl (to_node_inverse_impl node) = node

let to_node_is_bijection (t:eqtype) : prop =
  (forall (node_internal_level:pos) (node_internal:I.node_internal t node_internal_level).
    applying_to_node_inverse_after_to_node_is_identity node_internal) /\
  (forall (node:B.node t). applying_to_node_after_to_node_inverse_is_identity node)

let to_node_and_to_node_inverse_are_bijection_pair (t:eqtype) : prop =
  (forall (node_internal_level:pos) (node_internal:I.node_internal t node_internal_level) (node:B.node t).
    ((to_node_impl node_internal) = node) <==> 
    ((B.get_level node = node_internal_level) && (to_node_inverse_impl node = node_internal))
  )

let to_node_theorem (t:eqtype) : prop =
  to_node_is_bijection t /\ to_node_and_to_node_inverse_are_bijection_pair t
//---|

//--- lemma ---
let lemma_to_node_is_bijection (t:eqtype)
  : Lemma (ensures to_node_is_bijection t) =
  ()

let lemma_to_node_and_to_node_inverse_are_bijection_pair (t:eqtype)
  : Lemma (ensures to_node_and_to_node_inverse_are_bijection_pair t) =
  ()

let lemma_to_node_theorem (t:eqtype)
  : Lemma (ensures to_node_theorem t) =
  lemma_to_node_is_bijection t;
  lemma_to_node_and_to_node_inverse_are_bijection_pair t
//---|

//--- theory members ---
let to_node #t #level (node_internal:I.node_internal t level)
  : Pure (B.node t) True (ensures fun _ -> to_node_theorem t) =
  lemma_to_node_theorem t;
  to_node_impl node_internal

let to_node_inverse #t (node:B.node t)
  : Pure (I.node_internal t (B.get_level node)) True 
    (ensures fun _ -> to_node_theorem t) =
  lemma_to_node_theorem t;
  to_node_inverse_impl node
//---|