module Nemonuri.StratifiedNodes.Forall.Walkers

module L = FStar.List.Tot
module N = Nemonuri.StratifiedNodes.Nodes
module C = Nemonuri.StratifiedNodes.Children
module F = Nemonuri.StratifiedNodes.Factories
module Common = Nemonuri.StratifiedNodes.Common
module B = Nemonuri.StratifiedNodes.Walkers.Base
module D = Nemonuri.StratifiedNodes.Decreasers
module Fc = Nemonuri.StratifiedNodes.Forall.Children

//--- theory members ---
let for_all_children_values #t (t2:Type)
  (node:N.node t) 
  (selector:N.node t -> t2)
  (predicate:t2 -> Tot bool)
  : Tot bool =
  let children = (N.get_children node) in
  let children_values = L.map selector children in
  L.for_all predicate children_values

let for_all_children_and_self_values #t (t2:Type)
  (node:N.node t) 
  (selector:N.node t -> t2)
  (predicate:t2 -> Tot bool)
  : Tot bool =
  match predicate (selector node) with
  | false -> false
  | true -> (for_all_children_values t2 node selector predicate)


(*
let for_all_nodes #t
  (node:N.node t)
  (node_predicate:N.node t -> Tot bool)
  : Tot bool =
  let selector = (C.to_ancestor_list_given_selector t bool node_predicate) in
  B.walk node selector 
  Fc.for_all_aggregator
  Fc.for_all_aggregator
  (Common.aggregated_identity bool)
  (Fc.for_all_continue_predicate t)
  []
//---|

//--- propositions ---
let node1_satisfies_for_all_nodes_entails_node2_satisfies_predicate #t
  (node1:N.node t) (node2:N.node t)
  (node_predicate:N.node t -> Tot bool)
  : prop =
  ((for_all_nodes node1 node_predicate) ==> (node_predicate node2))

let node1_satisfies_for_all_nodes_is_equivalent_to_node2_satisfies_predicate #t
  (node1:N.node t) (node2:N.node t)
  (node_predicate:N.node t -> Tot bool)
  : prop =
  ((for_all_nodes node1 node_predicate) <==> (node_predicate node2))

let node_satisfies_for_all_nodes_entails_all_children_nodes_satisfy_predicate #t
  (node:N.node t)
  (node_predicate:N.node t -> Tot bool)
  : prop =
  forall child_node. 
  (C.is_parent node child_node) ==>
  (node1_satisfies_for_all_nodes_entails_node2_satisfies_predicate node child_node node_predicate)

let node_has_leaf_children #t
  (node:N.node t)
  : prop =
  (N.is_branch node) /\ 
  (N.get_level node = 2)

let node_has_single_leaf_child #t
  (node:N.node t)
  : prop =
  (node_has_leaf_children node) /\ 
  (N.get_children_length node = 1)

//---|

//--- proofs ---
let lemma3 #t
  (node:N.node t)
  (node_predicate:N.node t -> Tot bool)
  : Lemma
    (requires (N.is_leaf node) /\ (node_predicate node))
    (ensures (for_all_nodes node node_predicate))
  =
  ()

let lemma4 #t
  (node:N.node t)
  (node_predicate:N.node t -> Tot bool)
  : Lemma
    (requires (N.is_leaf node) /\ (~(node_predicate node)))
    (ensures ~(for_all_nodes node node_predicate))
  =
  ()

let lemma5 #t
  (node:N.node t)
  (node_predicate:N.node t -> Tot bool)
  : Lemma
    (ensures (N.is_leaf node) ==> 
     (node1_satisfies_for_all_nodes_is_equivalent_to_node2_satisfies_predicate node node node_predicate))
  =
  if not (N.is_leaf node) then ()
  else
  match (node_predicate node) with
  | true -> lemma3 node node_predicate
  | false -> lemma4 node node_predicate

let lemma6 #t
  (node:N.node t)
  (node_predicate:N.node t -> Tot bool)
  : Lemma
    (requires 
      (node_has_single_leaf_child node) /\
      (node_predicate node) /\ (node_predicate (N.get_child_at node 0))
    )
    (ensures for_all_nodes node node_predicate)
  =
  ()

let lemma7 #t
  (node:N.node t)
  (node_predicate:N.node t -> Tot bool)
  : Lemma
    (requires 
      (node_has_single_leaf_child node) /\
      (for_all_nodes node node_predicate)
    )
    (ensures (node_predicate node) /\ (node_predicate (N.get_child_at node 0)))
  =
  ()

let lemma8 #t
  (node:N.node t)
  (node_predicate:N.node t -> Tot bool)
  : Lemma
    (requires (node_has_single_leaf_child node))
    (ensures 
      (for_all_nodes node node_predicate) <==>
      ((node_predicate node) /\ (node_predicate (N.get_child_at node 0)))
    )
  =
  if ((node_predicate node) && (node_predicate (N.get_child_at node 0))) then
    lemma6 node node_predicate
  else if (for_all_nodes node node_predicate) then 
    lemma7 node node_predicate
  else
    ()

let lemma9 #t
  (node:N.node t)
  (node_predicate:N.node t -> Tot bool)
  : Lemma
    (requires (node_has_single_leaf_child node))
    (ensures node_satisfies_for_all_nodes_entails_all_children_nodes_satisfy_predicate node node_predicate)
  =
  lemma8 node node_predicate

let lemma10 #t
  (node:N.node t)
  (node_predicate:N.node t -> Tot bool)
  : Lemma
    (ensures node1_satisfies_for_all_nodes_entails_node2_satisfies_predicate node node node_predicate)
  = 
  ()
*)

(*
let rec lemma11 #t
  (node:N.node t)
  (node_predicate:N.node t -> Tot bool)
  : Lemma
    (requires 
      (node_has_leaf_children node) /\
      (for_all_nodes node node_predicate)
    )
    (ensures 
      (for_all_nodes (D.get_previous_node node) node_predicate)
    )
    (decreases (D.get_decreaser_from_children node))
  =
  if (N.is_branch node) && (N.get_level node = 2) && (N.get_children_length node = 1) then
    lemma9 node node_predicate
  else
    lemma11 (D.get_previous_node node) node_predicate
*)
  
    


(*
#push-options "--query_stats"
let lemma2 #t
  (node:N.node t)
  (node_predicate:N.node t -> Tot bool)
  (child_node:N.node t)
  : Lemma
    (requires 
      (for_all_nodes node node_predicate) /\ (C.is_parent node child_node)
    )
    (ensures (node_predicate child_node))
  =
  ()
#pop-options 

let lemma1 #t
  (node:N.node t)
  (node_predicate:N.node t -> Tot bool)
  : Lemma 
    (requires for_all_nodes node node_predicate)
    (ensures forall child_node. 
      (C.is_parent node child_node) ==>
      (node_predicate child_node)
    )
  =
  ()
*)
//---|
