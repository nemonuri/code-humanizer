module Nemonuri.StratifiedNodes.AncestorContexts.Types

module L = FStar.List.Tot
module N = Nemonuri.StratifiedNodes.Nodes
module C = Nemonuri.StratifiedNodes.Children
module Common = Nemonuri.StratifiedNodes.Common
module Id = Nemonuri.StratifiedNodes.Indexes

//--- type definition ---

type ancestor_context (t:eqtype) = 
| AContext :
  (ancestors: C.ancestor_list t) ->
  (indexes:list nat{Id.have_indexes ancestors indexes}) ->
  ancestor_context t

//---|