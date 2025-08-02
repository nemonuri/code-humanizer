module Nemonuri.StratifiedNodes.Walkers.AncestorContexts

module L = FStar.List.Tot
module N = Nemonuri.StratifiedNodes.Nodes
module C = Nemonuri.StratifiedNodes.Children
module Common = Nemonuri.StratifiedNodes.Common
module Ac = Nemonuri.StratifiedNodes.AncestorContexts
module Acs = Nemonuri.StratifiedNodes.AncestorContextSelectors

//--- type definitions ---

noeq
type walk_premise (t: eqtype) (t2: Type) =
| WPremise:
  (verifier: Acs.ancestor_context_given_selector_result_verifier t t2) ->
  (selector: Acs.ancestor_context_given_selector t t2) ->
  (first_child_to_parent_aggregator:Common.aggregator t2) ->
  (left_to_right_aggregator:Common.aggregator t2) ->
  (walk_as_child_to_parent_aggregator:Common.aggregator t2) ->
  walk_premise t t2

//---|

//--- theory members ---

let get_subselector #t #t2
  (premise: walk_premise t t2)
  (ancestor_context: Ac.ancestor_context t)
  : Tot (Acs.ancestor_context_given_selector_info t t2)
  =
  Acs.AInfo premise.verifier ancestor_context premise.selector


let rec walk_as_node #t #t2
  (premise: walk_premise t t2)
  (ancestor_context: Ac.ancestor_context t)
  (index: N.node_list_index (Ac.get_head_ancestor_children ancestor_context))
  (node: N.node t)
  : Pure t2
    (requires 
      (Ac.is_prependable_to_ancestor_context node index ancestor_context) &&
      ((N.get_child_at (Ac.get_head_ancestor ancestor_context) index) = node)
    )
    (ensures fun r -> premise.verifier ancestor_context index node r )
    (decreases 
      %[Ac.get_first_decreaser_of_ancestor_context ancestor_context;
        Ac.get_second_decreaser_of_ancestor_context ancestor_context;
        0]
    )
  =
  (*
  let selector_info: (Acs.ancestor_context_given_selector_info t t2) = 
      get_subselector premise ancestor_context in
  let subselector: (Acs.ancestor_context_given_subselector t t2 
        selector_info.verifier selector_info.ancestor_context) =
      (Acs.get_subselector selector_info) in
  *)
  //let selector_value = subselector index node in
  let selector_value = premise.selector premise.verifier ancestor_context index node in
  match (N.is_leaf node) with
  | true -> selector_value
  | false ->
  let next_ancestor_context = Ac.prepend_to_ancestor_context node index ancestor_context in
  let walk_as_child_value = walk_as_child premise next_ancestor_context 0 selector_value (None #t2) in
  premise.walk_as_child_to_parent_aggregator walk_as_child_value selector_value
  
and walk_as_child #t #t2
  (premise: walk_premise t t2)
  (ancestor_context: Ac.ancestor_context t)
  (index: N.node_list_index (Ac.get_head_ancestor_children ancestor_context))
  (parent_selector_value: t2)
  (maybe_aggregated_value: option t2{ (None? maybe_aggregated_value) <==> (index = 0) })
  //(subselector: Acs.ancestor_context_given_subselector t t2 premise.verifier ancestor_context)
  : Tot t2
    (*(requires 
      (Ac.is_prependable_to_ancestor_context node index ancestor_context)
    )
    (ensures fun r -> premise.verifier ancestor_context index node r )*)
    (decreases 
      %[Ac.get_first_decreaser_of_ancestor_context ancestor_context;
        Ac.get_second_decreaser_of_ancestor_context ancestor_context;
        (N.get_children_length (Ac.get_head_ancestor ancestor_context)) - index]
    )
  =
  let head_ancestor = (Ac.get_head_ancestor ancestor_context) in
  let node = (N.get_child_at head_ancestor index) in
  assert (C.is_parent head_ancestor node);
  let walk_as_node_value = walk_as_node premise ancestor_context index node in
  let children_length = (N.get_children_length (Ac.get_head_ancestor ancestor_context)) in
  let next_aggregated_value = (
    if (index = 0) then
      (premise.first_child_to_parent_aggregator walk_as_node_value parent_selector_value)
    else (
      let Some aggregated_value = maybe_aggregated_value in
      premise.left_to_right_aggregator aggregated_value walk_as_node_value
    )
  ) in
  let next_index = index + 1 in
  if next_index = children_length then
    next_aggregated_value
  else
    walk_as_child premise ancestor_context next_index parent_selector_value (Some next_aggregated_value)




//---|