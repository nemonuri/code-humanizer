module Nemonuri.StratifiedNodes.Walkers

module L = FStar.List.Tot
module N = Nemonuri.StratifiedNodes.Nodes
module NC = Nemonuri.StratifiedNodes.Children
module Option = FStar.Option

//--- type definitions ---

//---|

//--- theory members ---
let rec walk #t (#t_result:Type) //(#t_state:Type)
  (node:N.node t) //(state:t_state)
  (selector:N.node t -> t_result)
  (aggregator:list t_result -> t_result)
  //(folder:(from_parent:t_result) -> (from_current:t_result) -> t_result)
  (continue_predicate:t_result -> bool)
  (maybe_parent:option (N.node t){ match maybe_parent with | Some p -> NC.is_child p node | None -> true })
  //(parent_result: (option (t_result)){ (Option.isNone parent && Option.isNone parent_result) || (Option.isSome parent && Option.isSome parent_result) })
  : Tot t_result (decreases (node.level)) =
  if (N.is_leaf node) then
    selector node
  else
    let children = N.get_children node in
    let children_results = L.map (fun child_node ->
      walk child_node selector aggregator continue_predicate (Some node)
    ) children in
    let aggregated_result = aggregator children_results in
    aggregated_result
    
//---|