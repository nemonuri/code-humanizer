// For more information see http://www.fstar-lang.org/tutorial/
module Adder

module L = FStar.List.Tot
module N = Nemonuri.StratifiedNodes.Nodes
module C = Nemonuri.StratifiedNodes.Children
module F = Nemonuri.StratifiedNodes.Factories
module Common = Nemonuri.StratifiedNodes.Common
module W = Nemonuri.StratifiedNodes.Walkers

open FStar.IO

let node_t = N.node nat

let is_adder_node (node:node_t)
  : Tot bool
  =
  match (N.is_leaf node) with
  | true -> true
  | false -> (N.get_value node) = 0

let selector : (C.ancestor_list_given_selector nat nat) =
  fun node ancestors -> (N.get_value node)

let aggregator : (Common.aggregator nat) =
  fun v1 v2 -> v1 + v2

let continue_predicate : (N.node nat -> nat -> bool) =
  fun n t -> true

let create_adder_node1 ()
  : Pure (node_t) True
    (fun r -> is_adder_node r)
  =
  let n0_c = F.create_leaf_node_list [1;2;3] in
  let n0 = F.create_node 0 n0_c in
  let n1_c = F.create_leaf_node_list [4;5] in
  let n1 = F.create_node 0 n1_c in
  let n = F.create_node 0 [n0; n1] in
  n

let create_adder_node2 ()
  : Pure (node_t) True
    (fun r -> is_adder_node r)
  =
  let v1 = create_adder_node1 () in
  let v2 = F.swap (N.get_children v1) 0 1 in
  let v3 = F.replace_children v1 v2 in
  v3

let walk_adder_node (node:node_t)
  : Pure nat (requires is_adder_node node)
    (ensures fun _ -> true)
  =
  W.walk node selector aggregator aggregator continue_predicate []

(*
let _ = assert ( 
  ( walk_adder_node (create_adder_node1 ()) ) =
  ( walk_adder_node (create_adder_node2 ()) )
)
*)

let main () = 
  let v1 = walk_adder_node (create_adder_node1 ()) in
  print_any v1;
  print_newline ();
  let v2 = walk_adder_node (create_adder_node2 ()) in
  print_any v2

let _ = main ()
  
