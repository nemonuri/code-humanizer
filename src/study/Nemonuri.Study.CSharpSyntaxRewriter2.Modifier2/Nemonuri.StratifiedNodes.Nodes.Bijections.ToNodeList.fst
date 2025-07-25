module Nemonuri.StratifiedNodes.Nodes.Bijections.ToNodeList

module I = Nemonuri.StratifiedNodes.Internals
module Base = Nemonuri.StratifiedNodes.Nodes.Base
module ToNode = Nemonuri.StratifiedNodes.Nodes.Bijections.ToNode
open Nemonuri.StratifiedNodes.Nodes.Bijections.ToNode
open Nemonuri.StratifiedNodes.Nodes.Base.Members


//--- private theory members ---
private let to_node_list_impl #t #max_level 
  (node_list_internal:I.node_list_internal t max_level) 
  : Tot (Base.node_list t) =
  I.select node_list_internal (fun ni -> to_node ni)  

private let rec to_node_list_inverse_impl
  #t (l:Base.node_list t)
  : Tot (I.node_list_internal t (get_list_level l))
        (decreases l)
  =
  match l with
  | [] -> I.SNil
  | hd::tl ->
      let hd2 = to_node_inverse hd in
      let tl2 = to_node_list_inverse_impl tl in
      I.SCons hd2 tl2
//---|


