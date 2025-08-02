module Nemonuri.StratifiedNodes.AncestorContextSelectors

module L = FStar.List.Tot
module N = Nemonuri.StratifiedNodes.Nodes
module C = Nemonuri.StratifiedNodes.Children
module Common = Nemonuri.StratifiedNodes.Common
module Id = Nemonuri.StratifiedNodes.Indexes
module Ac = Nemonuri.StratifiedNodes.AncestorContexts
module T = Nemonuri.StratifiedNodes.AncestorContextSelectors.Types

//--- members ---

let get_subselector #t #t2
  (selector_info: T.ancestor_context_given_selector_info t t2)
  : T.ancestor_context_given_subselector t t2 selector_info.verifier selector_info.ancestor_context
  =
  selector_info.selector selector_info.verifier selector_info.ancestor_context

let to_selector_info_for_children #t #t2
  (selector_info: T.ancestor_context_given_selector_info t t2)
  (node: N.node t)
  (index: nat)
  : Pure (T.ancestor_context_given_selector_info t t2)
    (requires Ac.is_prependable_to_ancestor_context node index selector_info.ancestor_context)
    (ensures fun r ->
      Ac.ancestor_context2_is_decrease_of_ancestor_context1 selector_info.ancestor_context r.ancestor_context )
  =
  T.AInfo 
    selector_info.verifier
    (Ac.prepend_to_ancestor_context node index selector_info.ancestor_context)
    selector_info.selector

//---|

include Nemonuri.StratifiedNodes.AncestorContextSelectors.Types

