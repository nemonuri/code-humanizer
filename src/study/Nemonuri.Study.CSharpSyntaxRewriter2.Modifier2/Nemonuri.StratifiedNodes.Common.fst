module Nemonuri.StratifiedNodes.Common

//--- type definitions ---
let aggregator (t:Type) = (aggregated:t) -> (aggregating:t) -> t
//---|

//--- theory members ---
// https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.skipwhile?view=net-9.0
let rec skip_while #t
  (l:list t) 
  (predicate:t -> Tot bool)
  : Tot (list t) (decreases l) =
  match l with
  | [] -> []
  | hd::tl ->
    match (predicate hd) with
    | true -> l
    | false -> skip_while tl predicate

// https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.takewhile?view=net-9.0
(*
let rec take_while #t
  (l:list t) 
  (predicate:t -> Tot bool)
  : Tot (list t) (decreases l) =
*)
//---|