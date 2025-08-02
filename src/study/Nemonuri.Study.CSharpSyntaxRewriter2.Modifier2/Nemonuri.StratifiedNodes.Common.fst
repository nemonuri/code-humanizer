module Nemonuri.StratifiedNodes.Common

module L = FStar.List.Tot

//--- type definitions ---
let aggregator (t:Type) = (aggregated:t) -> (aggregating:t) -> t

let aggregated_identity (t:Type) 
  : (aggregator t) =
  fun aggregated aggregating -> aggregated

let zero_or_one = n:nat{n <= 1}
//---|

//--- propositions ---
(*
let filter_theorem (#a: Type) (f: (a -> Tot bool)) (l: list a) : prop =
  forall x. L.memP x (L.filter f l) <==> L.memP x l /\ f x
*)

let list1_length_is_equal_to_list2_length_plus_list3_length 
  #t1 #t2 #t3
  (list1: list t1) (list2: list t2) (list3: list t3)
  : bool
  =
  (L.length list1) = (L.length list2) + (L.length list3)

let list1_length_is_less_or_equal_than_list2_length_plus_list3_length
  #t1 #t2 #t3
  (list1: list t1) (list2: list t2) (list3: list t3)
  : bool
  =
  (L.length list1) >= (L.length list2) + (L.length list3)
//---|

//--- proof ---
(*
let rec lemma_splitAt_fst_length_is_n #t (n:nat) (l:list t)
  : Lemma 
    (requires (n <= L.length l))  
    (ensures (L.length (fst (L.splitAt n l)) = n))
    (decreases n)
  =
  if n = 0 then (
    assert (Nil? (fst (L.splitAt n l)))
  ) else //if (L.length (fst (L.splitAt (n-1) l)) = (n-1)) then
  let l_fst1 = L.length (fst (L.splitAt (n-1) l)) in
  let l_fst2 = L.length (fst (L.splitAt n l)) in
  assert ((L.tl (fst (L.splitAt n l))) == (fst (L.splitAt (n-1) l)));
  assert (l_fst1 + 1 = l_fst2);
  if (l_fst1 = (n-1)) then (
    assert (l_fst2 = n)
  ) else
    lemma_splitAt_fst_length_is_n (n-1) l
*)
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


let rec splitAt (#a:Type) (n:nat) (l:list a) : Tot (list a & list a) =
  if n = 0 then [], l
  else
    match l with
    | [] -> [], l
    | x :: xs -> let l1, l2 = splitAt (n-1) xs in x :: l1, l2

let rec lemma_splitAt_fst_length (#a:Type) (n:nat) (l:list a) :
  Lemma
    (requires (n <= L.length l))
    (ensures (L.length (fst (splitAt n l)) = n)) =
  if n = 0 then ()
  else
    match l with
    | [] -> ()
    | x :: xs -> lemma_splitAt_fst_length (n - 1) xs

let rec lemma_splitAt_snd_length (#a:Type) (n:nat) (l:list a) :
  Lemma
    (requires (n <= L.length l))
    (ensures (L.length (snd (splitAt n l)) = L.length l - n)) =
  match n, l with
  | 0, _ -> ()
  | _, [] -> ()
  | _, _ :: l' -> lemma_splitAt_snd_length (n - 1) l'


let rec ends_with (#t:eqtype)
  (l:list t) (sub_l:list t)
  : Tot bool (decreases l) =
  let (len, sub_len) = (L.length l, L.length sub_l) in
  if len < sub_len then false
  else if len = sub_len then (l = sub_l)
  else (ends_with (L.tl l) sub_l)


//---|