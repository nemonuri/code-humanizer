module Nemonuri.StratifiedNodes.Forall.Aggregating

module L = FStar.List.Tot
module N = Nemonuri.StratifiedNodes.Nodes
module T = Nemonuri.StratifiedNodes.Children.Types
module C = Nemonuri.StratifiedNodes.Children
module Common = Nemonuri.StratifiedNodes.Common

//--- members ---

let for_all_aggregator
  : (Common.aggregator bool) =
  fun v1 v2 -> (v1 && v2)

let for_all_continue_predicate (t:eqtype)
  : (C.continue_predicate t bool) 
  =
  fun n v -> v

//---|

//--- propositions ---

//---|

//--- proof ---

//---|