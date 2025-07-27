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

let option_node_list_index #t (nl:node_list t) = 
  (*oi:*)(option (node_list_index nl))(*{
    match oi with
    | Some i -> ((Cons? nl) && (i < (L.length nl)))
    | None -> true
  }*)
//---|