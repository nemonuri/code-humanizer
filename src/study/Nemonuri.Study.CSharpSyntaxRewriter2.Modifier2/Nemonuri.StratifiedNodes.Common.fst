module Nemonuri.StratifiedNodes.Common

module L = FStar.List.Tot

//--- type definitions ---
let aggregator (t:Type) = (aggregated:t) -> (aggregating:t) -> t
//---|

//--- predicates ---
let filter_theorem (#a: Type) (f: (a -> Tot bool)) (l: list a) : prop =
  forall x. L.memP x (L.filter f l) <==> L.memP x l /\ f x
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

// https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1.findindex?view=net-9.0
let rec find_index #t 
  (l:list t) (predicate:t -> Tot bool)
  : Pure nat 
    (requires 
      Some? (L.find predicate l)
    )
    (ensures fun r -> 
      let v1 = L.length l in
      (0 <= r) && (r < v1)
    )
    (decreases l)
  =
  let hd::tl = l in
  match predicate hd with
  | true -> 0
  | false -> 1 + (find_index tl predicate)

// https://github.com/FStarLang/FStar/blob/master/ulib/FStar.List.Tot.Base.fst
(*
let filter (#a: Type) (f: (a -> Tot bool)) (l: list a)
  : Pure (list a) True 
         (ensures fun _ -> filter_theorem f l)
  =
  L.filter f l
*)


//---|