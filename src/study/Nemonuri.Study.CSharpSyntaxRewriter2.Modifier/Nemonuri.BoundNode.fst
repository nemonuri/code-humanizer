module Nemonuri.BoundNode

module L = FStar.List.Tot
module LT = Nemonuri.ListTheory

type bound_node (t:Type) =
  | Leaf : 
      bound_value:t -> 
      parent:(option (bound_node t)) -> depth:nat ->
      bound_node t
  | Branch : 
      bound_value:t -> 
      children:list (bound_node t) -> height:nat -> 
      parent:(option (bound_node t)) -> depth:nat ->
      bound_node t

let get_bound_value (#t:Type) (bn:bound_node t) : Tot t =
  if Leaf? bn then Leaf?.bound_value bn
  else Branch?.bound_value bn

let try_get_parent (#t:Type) (bn:bound_node t) : Tot (option (bound_node t)) =
  if Leaf? bn then Leaf?.parent bn
  else Branch?.parent bn

let get_depth (#t:Type) (bn:bound_node t) : Tot nat =
  if Leaf? bn then Leaf?.depth bn
  else Branch?.depth bn

let bound_node_has_valid_parent_and_depth_relation (#t:Type) (bn:bound_node t) 
  : Tot bool 
  = match (try_get_parent bn) with
  | None -> get_depth bn = 0
  | Some parent_bound_node -> 
      (get_depth bn > 0) &&
      ((get_depth bn - 1) = (get_depth parent_bound_node))
  

//let bound_node_is_nil_branch (#t:Type) (bn:bound_node t) : prop = 
    