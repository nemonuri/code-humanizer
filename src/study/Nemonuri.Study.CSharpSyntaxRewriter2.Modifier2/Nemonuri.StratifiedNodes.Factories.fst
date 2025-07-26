module Nemonuri.StratifiedNodes.Factories

include Nemonuri.StratifiedNodes.Factories.Nodes
include Nemonuri.StratifiedNodes.Factories.NodeLists



(*
let with_node_at #t (node:N.node t) (indexes:list nat)
  : Pure (N.node t)
    (requires Id.can_get_descendant_or_self_from_indexes node indexes)
    (ensures fun _ -> true)
*)

//--- node list members ---





// 사담: 의미가 있나 이거? 결국 항등 함수일 뿐이잖아?
(*
let create_node_list (t:eqtype) (node_list:N.node_list t)
  : Pure (N.node_list t) 
         (requires True) 
         (ensures fun _ -> (N.to_node_list_theorem t) /\ (E.equivalent_theorem t))
  =
  N.lemma_to_node_list_theorem t;
  E.lemma_equivalent_theorem t;
  node_list
*)


//---|