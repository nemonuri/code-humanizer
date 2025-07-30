// For more information see http://www.fstar-lang.org/tutorial/
module Adder

module L = FStar.List.Tot
module N = Nemonuri.StratifiedNodes.Nodes
module C = Nemonuri.StratifiedNodes.Children
module F = Nemonuri.StratifiedNodes.Factories
module Common = Nemonuri.StratifiedNodes.Common
module W = Nemonuri.StratifiedNodes.Walkers
module Log = Nemonuri.FStar.Logging

open FStar.IO
open FStar.Printf

let node_t = N.node nat

let is_adder_node (node:node_t)
  : Tot bool
  =
  match (N.is_leaf node) with
  | true -> true
  | false -> (N.get_value node) = 0

let selector : (C.ancestor_list_given_selector nat nat) =
  fun node ancestors -> (
    //let msg = sprintf "selected %d \n" (N.get_value node) in
    //let _ = Log.debug_print_string msg in
    N.get_value node
    )

let aggregator_impl : (Common.aggregator nat) =
  fun v1 v2 -> (
    let msg = sprintf "%d + %d \n" v1 v2 in
    let _ = Log.debug_print_string msg in
    v1 + v2
  )

let to_first_child_from_parent : (Common.aggregator nat) =
  fun first_child_value parent_value -> (
    let _ = Log.debug_print_string "to_first_child_from_parent: " in
    aggregator_impl first_child_value parent_value
  )

let from_left_to_right : (Common.aggregator nat) =
  fun first_child_value parent_value -> (
    let _ = Log.debug_print_string "from_left_to_right: " in
    aggregator_impl first_child_value parent_value
  )

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
  W.walk node selector 
  to_first_child_from_parent 
  from_left_to_right 
  (Common.aggregated_identity nat) 
  (C.always_continue_predicate nat nat)
  []

(*
let _ = assert ( 
  ( walk_adder_node (create_adder_node1 ()) ) =
  ( walk_adder_node (create_adder_node2 ()) )
)
*)

let forall_aggregator
  : (Common.aggregator bool) =
  fun v1 v2 -> (v1 && v2)

let forall_continue_predicate (t:eqtype)
  : (C.continue_predicate t bool) 
  =
  fun n v -> v

let is_adder_node2 (node:node_t) (ancestors:C.next_head_given_ancestor_list node)
  : Tot bool =
  is_adder_node node

let is_adder_node_really (node:node_t)
  : Tot bool
  = 
  W.walk node is_adder_node2
  forall_aggregator
  forall_aggregator
 (Common.aggregated_identity bool)
 (forall_continue_predicate nat)
 []

(*
let _ = assert ( is_adder_node_really (create_adder_node1 ()) )
*)

let main () = 
  let v1 = walk_adder_node (create_adder_node1 ()) in
  print_any v1;
  print_newline ();
  let v2 = walk_adder_node (create_adder_node2 ()) in
  print_any v2; print_newline ();
  print_any (is_adder_node_really (create_adder_node1 ())); print_newline ();
  print_any (is_adder_node_really (create_adder_node2 ())); print_newline ()

let _ = main ()
  
(* 출력

to_first_child_from_parent: 1 + 0 
from_left_to_right: 1 + 2
from_left_to_right: 3 + 3
to_first_child_from_parent: 6 + 0
to_first_child_from_parent: 4 + 0
from_left_to_right: 4 + 5
from_left_to_right: 6 + 9
15
to_first_child_from_parent: 4 + 0
from_left_to_right: 4 + 5
to_first_child_from_parent: 9 + 0
to_first_child_from_parent: 1 + 0
from_left_to_right: 1 + 2
from_left_to_right: 3 + 3
from_left_to_right: 9 + 6
15
true
true

*)