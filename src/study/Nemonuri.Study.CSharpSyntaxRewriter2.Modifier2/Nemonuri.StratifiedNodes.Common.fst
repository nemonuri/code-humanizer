module Nemonuri.StratifiedNodes.Common

//--- type definitions ---
let aggregator (t:Type) = (prev:t) -> (current:t) -> t
//---|