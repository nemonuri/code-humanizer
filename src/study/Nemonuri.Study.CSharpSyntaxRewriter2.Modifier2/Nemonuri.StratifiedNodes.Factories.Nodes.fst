module Nemonuri.StratifiedNodes.Factories.Nodes

module L = FStar.List.Tot
module I = Nemonuri.StratifiedNodes.Internals
module N = Nemonuri.StratifiedNodes.Nodes
module C = Nemonuri.StratifiedNodes.Children
module Id = Nemonuri.StratifiedNodes.Indexes
module E = Nemonuri.StratifiedNodes.Nodes.Equivalence
module Common = Nemonuri.StratifiedNodes.Common

//--- theory members ---

let is_leaf_and_has_value #t (node:N.node t) (value:t)
  : Tot bool =
  (N.get_value node = value) && (N.is_leaf node)

let create_leaf_node (#t:eqtype) (value:t)
  : Pure (N.node t) 
         (requires True) 
         (ensures fun r -> is_leaf_and_has_value r value)
  =
  N.to_node (I.SNode (I.SNil) value)


let is_equal_to_result_of_create_leaf_node_if_node_is_leaf #t (node:N.node t) (value:t)
  : Tot bool =
  (N.get_value node = value) && (
    match N.is_leaf node with
    | true -> (create_leaf_node value) = node
    | false -> true
  )

let replace_value #t (node:N.node t) (value:t)
  : Pure (N.node t) 
         (requires True) 
         (ensures fun r -> is_equal_to_result_of_create_leaf_node_if_node_is_leaf r value)
  =
  let internal = node.internal in
  let new_internal = I.SNode internal.children value in
  N.to_node new_internal

let replace_children #t (node:N.node t) (children:N.node_list t)
  : Pure (N.node t) 
         (requires True)
         (ensures fun r -> E.are_equivalent_as_node_list children r.internal.children)
(*
         (ensures fun node -> 
           ((I.get_list_level node.internal.children) = (I.get_list_level (N.to_node_list_inverse children))) &&
           (node.internal.children = (N.to_node_list_inverse children)) &&
           ((N.get_list_level (N.get_children node)) = (N.get_list_level children)) &&
           ((N.get_children node) = children)
         )
*)
  =
  let internal = node.internal in
  let new_internal = I.SNode (N.to_node_list_inverse children) internal.value in
  N.to_node new_internal


let create_node (#t:eqtype) (value:t) (children:N.node_list t)
  : Tot (N.node t) =
  let node = create_leaf_node value in
  match children with
  | [] -> node
  | _ -> replace_children node children

//---|


//--- propositions ---

//---|

//#push-options "--query_stats"
//#pop-options

// normalized 된 결과를 볼 수 있나?
(*
let assert1 = assert ( forall (t:eqtype) (value:t) (children:N.node_list t). 
    let n = (create_node value children) in
    (n = (create_leaf_node value)) || (n = replace_children (create_leaf_node value) children) 
  ) by FStar.Tactics.V2.compute()
*)