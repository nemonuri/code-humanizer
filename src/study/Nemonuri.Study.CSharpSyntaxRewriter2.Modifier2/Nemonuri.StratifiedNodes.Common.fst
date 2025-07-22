module Nemonuri.StratifiedNodes.Common

//--- type definitions ---
let aggregator (t:Type) = (aggregated:t) -> (aggregating:t) -> t
//---|