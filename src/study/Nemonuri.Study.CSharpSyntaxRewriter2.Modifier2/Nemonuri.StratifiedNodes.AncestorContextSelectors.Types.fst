module Nemonuri.StratifiedNodes.AncestorContextSelectors.Types

module L = FStar.List.Tot
module N = Nemonuri.StratifiedNodes.Nodes
module C = Nemonuri.StratifiedNodes.Children
module Common = Nemonuri.StratifiedNodes.Common
module Id = Nemonuri.StratifiedNodes.Indexes
module Ac = Nemonuri.StratifiedNodes.AncestorContexts

//--- types ---

type ancestor_context_given_selector_result_verifier
  (t: eqtype) (t2: Type) =
  (ancestor_context: Ac.ancestor_context t) ->
  (index: Common.nat_or_minus_one) ->
  (node: N.node t) -> 
  (result: t2) ->
  Pure bool 
  (requires Ac.is_prependable_to_ancestor_context node index ancestor_context ) 
  (ensures fun r -> true)

(*
type ancestor_context_given_aggregator (t: eqtype) (t2: Type) =
  (verifier: ancestor_context_given_selector_result_verifier t t2) ->
  (ancestor_context: Ac.ancestor_context t) ->
  (index: Common.nat_or_minus_one) ->
  (node: N.node t) ->  
  Pure t2 
  (requires Ac.is_prependable_to_ancestor_context node index ancestor_context ) 
  (ensures fun r -> verifier ancestor_context index node r )
*)

type ancestor_context_given_selector (t: eqtype) (t2: Type) =
  (verifier: ancestor_context_given_selector_result_verifier t t2) ->
  (ancestor_context: Ac.ancestor_context t) ->
  (index: Common.nat_or_minus_one) ->
  (node: N.node t) -> 
  Pure t2 
  (requires Ac.is_prependable_to_ancestor_context node index ancestor_context ) 
  (ensures fun r -> verifier ancestor_context index node r )

type ancestor_context_and_verifier_given_selector (t: eqtype) (t2: Type) 
  (verifier: ancestor_context_given_selector_result_verifier t t2) =
  (ancestor_context: Ac.ancestor_context t) ->
  (index: Common.nat_or_minus_one) ->
  (node: N.node t) -> 
  Pure t2 
  (requires Ac.is_prependable_to_ancestor_context node index ancestor_context ) 
  (ensures fun r -> verifier ancestor_context index node r )

type ancestor_context_given_subselector (t: eqtype) (t2: Type)
  (verifier: ancestor_context_given_selector_result_verifier t t2)
  (ancestor_context: Ac.ancestor_context t) =
  (index: Common.nat_or_minus_one) ->
  (node: N.node t) -> 
  Pure t2 
  (requires Ac.is_prependable_to_ancestor_context node index ancestor_context ) 
  (ensures fun r -> verifier ancestor_context index node r )

noeq
type ancestor_context_given_selector_info (t: eqtype) (t2: Type) =
| AInfo:
  (verifier: ancestor_context_given_selector_result_verifier t t2) ->
  (ancestor_context: Ac.ancestor_context t) ->
  (selector: ancestor_context_given_selector t t2) ->
  ancestor_context_given_selector_info t t2

//---|