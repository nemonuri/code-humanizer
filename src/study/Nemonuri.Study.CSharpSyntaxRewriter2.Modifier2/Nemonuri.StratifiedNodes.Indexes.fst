module Nemonuri.StratifiedNodes.Indexes

module L = FStar.List.Tot
module N = Nemonuri.StratifiedNodes.Nodes
module C = Nemonuri.StratifiedNodes.Children
module Common = Nemonuri.StratifiedNodes.Common

//--- theory members ---
let to_indexes #t (ancestors:C.ancestor_list t)
  : Pure (list nat) (requires True) (ensures fun r -> true)
  =
  ()

//---|