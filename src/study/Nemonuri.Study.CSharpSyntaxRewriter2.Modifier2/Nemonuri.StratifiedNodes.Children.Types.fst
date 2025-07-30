module Nemonuri.StratifiedNodes.Children.Types

module N = Nemonuri.StratifiedNodes.Nodes

//--- type defenitions ---
let continue_predicate (t:eqtype) (t2:Type)
  = N.node t -> t2 -> Tot bool
//---|
