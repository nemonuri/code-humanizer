module Nemonuri.StratifiedNodes.VerifiableAggregators

module L = FStar.List.Tot
module Common = Nemonuri.StratifiedNodes.Common

//--- types ---
type aggregator_verifier (t:Type) = 
  (aggregated:t) -> (aggregating:t) ->
  (next_aggregated:t) -> bool

type verifiable_aggregator (t:Type) =
  (verifier: (aggregator_verifier t)) ->
  (aggregated: t) -> 
  (aggregating: t) -> 
  Pure t 
  (requires True)
  (ensures fun r -> verifier aggregated aggregating r)

noeq
type verifiable_aggregator_context (t:Type) =
| VAggregator: 
  (verifier: (aggregator_verifier t)) ->
  (aggregator: (verifiable_aggregator t)) ->
  verifiable_aggregator_context t
//---|

//--- members ---

let to_aggregator #t (context: verifiable_aggregator_context t)
: Tot (Common.aggregator t)
=
context.aggregator context.verifier

//---|