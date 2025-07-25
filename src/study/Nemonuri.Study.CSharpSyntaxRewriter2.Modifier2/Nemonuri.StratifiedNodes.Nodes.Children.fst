module Nemonuri.StratifiedNodes.Nodes.Children

module L = FStar.List.Tot
module Math = FStar.Math.Lib
module I = Nemonuri.StratifiedNodes.Internals
module T = Nemonuri.StratifiedNodes.Nodes.Base.Types
open Nemonuri.StratifiedNodes.Nodes.Base.Members
open Nemonuri.StratifiedNodes.Nodes.Bijections

//--- theory members ---
let get_children #t (node:T.node t) : Tot (T.node_list t) =
  to_node_list node.internal.children

let get_children_length #t (node:T.node  t) 
  : Pure nat (requires True) (ensures fun r -> r = L.length (get_children node)) = 
  I.get_length node.internal.children
//---|

//--- private theory members ---
let get_child_at #t (nd:T.node  t) (index:nat)
  : Pure (T.node  t) 
    (requires (index < (get_children_length nd)))
    (ensures fun r -> r = (L.index (get_children nd) index))
  =
  // TODO: Internals 단계에서 구현하기
  L.index (get_children nd) index

//---|