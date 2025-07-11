module Nemonuri.ListTheory

module L = FStar.List.Tot

let comparer (#t:Type) = l:t -> r:t -> int
let default_int_comparer: (comparer #int) = 
  fun (l:int) (r:int) ->
    if l > r then -1
    else if l < r then 1
    else 0

let rec max (#t:Type) (l:list t) (c:comparer #t) (candidate:t)
  : Tot t 
  = match l with
  | [] -> candidate
  | hd::tl ->
    let v1 = c candidate hd in
    if v1 > 0 then 
      max tl c hd 
    else 
      max tl c candidate

let has_type_unempty_list (#t:Type) (l:list t) : bool = not (L.isEmpty l)
let unempty_list (t:Type) = l:(list t){has_type_unempty_list l}
  
let max_for_unempty_list (#t:Type) (l:unempty_list t) (c:comparer #t) : Tot t =
  max #t (L.tl l) c (L.hd l)

let prop_equality_comparer (#t:Type) = l:t -> r:t -> prop
let rec prop_equals (#t:Type) (l1:list t) (l2:list t) (c:prop_equality_comparer #t)
  : prop
  = match (l1, l2) with
  | ([], []) -> True
  | (hd1::tl1, hd2::tl2) ->
      (c hd1 hd2) /\ (prop_equals tl1 tl2 c)
  | _ -> False
