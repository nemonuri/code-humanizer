module Nemonuri.StratifiedNodes.Nodes.Base.Types

module L = FStar.List.Tot
module I = Nemonuri.StratifiedNodes.Internals

//--- type definitions ---
type node (t:eqtype) =
  | SNode : 
      level:pos ->
      internal:(I.node_internal t level) ->
      node t

let node_list (t:eqtype) = list (node t)

let node_list_index #t (nl:node_list t) = i:nat{ (Cons? nl) && (i < (L.length nl)) }

(*
type maybe_node_list_index #t: node_list t -> Type =
| MIndex: 
    (internal_node_list:node_list t) -> 
    //(has_index:bool) ->
    (value:int{ 
      ((0 <= value) && (value < (L.length internal_node_list))) ||
      (value = -1)
    }) ->
    maybe_node_list_index internal_node_list
*)

type maybe_node_list_index #t: node_list t -> Type =
| INone: (internal_node_list:node_list t) -> maybe_node_list_index internal_node_list
| ISome: 
  (internal_node_list:node_list t) ->
  (index:node_list_index internal_node_list) ->
  maybe_node_list_index internal_node_list
//---|