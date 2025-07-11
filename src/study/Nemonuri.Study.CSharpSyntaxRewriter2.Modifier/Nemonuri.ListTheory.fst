module Nemonuri.ListTheory

module L = FStar.List.Tot

let comparer (#t:Type) = l:t -> r:t -> int

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
