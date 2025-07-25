module Nemonuri.StratifiedNodes.Nodes.Bijections.ToNode

module I = Nemonuri.StratifiedNodes.Internals
module Base = Nemonuri.StratifiedNodes.Nodes.Base
open Nemonuri.StratifiedNodes.Nodes.Base.Members

//--- private theory members ---
private let to_node_impl #t #level (node_internal:I.node_internal t level) : Tot (Base.node t) =
  Base.SNode level node_internal

private let to_node_inverse_impl #t (nd:Base.node t) : Tot (I.node_internal t (get_level nd)) =
  nd.internal
//---|

//--- propositions ---
let applying_to_node_inverse_after_to_node_is_identity 
  #t #node_internal_level (node_internal:I.node_internal t node_internal_level) : prop =
  to_node_inverse_impl (to_node_impl node_internal) == node_internal

let applying_to_node_after_to_node_inverse_is_identity
  #t (node:Base.node t) : prop =
  to_node_impl (to_node_inverse_impl node) == node

let to_node_is_bijection (t:eqtype) : prop =
  (forall (node_internal_level:pos) (node_internal:I.node_internal t node_internal_level).
    applying_to_node_inverse_after_to_node_is_identity node_internal) /\
  (forall (node:Base.node t). applying_to_node_after_to_node_inverse_is_identity node)
//---|

//--- lemma ---
let lemma_to_node_is_bijection (t:eqtype)
  : Lemma (ensures to_node_is_bijection t) =
  ()
//---|

//--- theory members ---
let to_node #t #level (node_internal:I.node_internal t level)
  : Pure (Base.node t) True (ensures fun _ -> to_node_is_bijection t) =
  lemma_to_node_is_bijection t;
  to_node_impl node_internal

let to_node_inverse #t (node:Base.node t)
  : Pure (I.node_internal t (get_level node)) True (ensures fun _ -> to_node_is_bijection t) =
  lemma_to_node_is_bijection t;
  to_node_inverse_impl node
//---|