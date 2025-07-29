module Nemonuri.StratifiedNodes.Nodes.Base.Members

module L = FStar.List.Tot
module I = Nemonuri.StratifiedNodes.Internals
module T = Nemonuri.StratifiedNodes.Nodes.Base.Types
module Math = FStar.Math.Lib
module Common = Nemonuri.StratifiedNodes.Common
open FStar.Classical.Sugar

//--- (T.node_list_index t) or (T.option_node_list_index t) members ---

let is_node_list_index #t (index:nat) (node_list:T.node_list t) 
  : Tot bool =
  (Cons? node_list) && (index < (L.length node_list))

let is_not_node_list_index #t (index:nat) (node_list:T.node_list t) 
  : Tot bool =
  (Nil? node_list) || (index >= (L.length node_list))

private let _ = assert ( forall t index node_list .
  (is_node_list_index #t index node_list) \/
  (is_not_node_list_index #t index node_list)
 )

let has_index #t
  (#node_list:T.node_list t)
  (source:T.maybe_node_list_index node_list)
  : Tot bool =
  T.ISome? source

let to_int #t
  (#node_list:T.node_list t)
  (source:T.maybe_node_list_index node_list)
  : Pure int True
    (ensures fun r -> 
      match has_index source with
      | true -> (r >= 0) && (is_node_list_index r node_list)
      | false -> (r = -1)
    )
  =
  match has_index source with
  | true -> (T.ISome?.index source)
  | false -> -1

(*
let has_index #t 
  (#node_list:T.node_list t)
  (source:T.maybe_node_list_index node_list)
  : Tot bool =
  (source.value <> -1)
*)

(*
let get_default_maybe_node_list_index #t (node_list:T.node_list t)
  : Pure (T.maybe_node_list_index node_list) True
    (ensures fun r -> not (has_index r))
  =
  T.INone node_list
*)

let to_maybe_node_list_index #t (index:nat) (node_list:T.node_list t) 
  : Pure (T.maybe_node_list_index node_list) True
    (ensures fun r ->
      match (index >= (L.length node_list)) with
      | true -> (to_int r) = -1
      | false -> (to_int r) = index
    )
  =
  match is_node_list_index index node_list with
  | true -> T.ISome node_list index 
  | false -> T.INone node_list

private let is_node_list_index_rewrite #t (node_list:T.node_list t) (index:nat)
  : Tot bool =
  is_node_list_index index node_list

let is_node_list_index_list #t (l:list nat) (node_list:T.node_list t)
  : Pure bool True
    (ensures fun b ->
      match b with
      | true -> (forall i. L.contains i l ==> is_node_list_index i node_list)
      | false -> True
    )
  =
  L.for_all_mem (is_node_list_index_rewrite node_list) l;
  L.for_all (is_node_list_index_rewrite node_list) l 
//---|

//--- (T.node t) members ---
let get_level #t (node:T.node t) 
  : Pure pos True (ensures fun r -> (r = (I.get_level node.internal)))
  = 
  node.level

let get_value #t (node:T.node t) : Tot t = node.internal.value
//---|

//--- (T.node_list t) private members ---
private let rec get_list_level_impl #t (l:T.node_list t) : Tot nat =
  match l with
  | [] -> 0
  | hd::tl -> Math.max (get_level hd) (get_list_level_impl tl)

private let rec get_index_list_from_predicate_core #t 
  (original_node_list: T.node_list t)
  (current_index: nat) 
  (predicate: (T.node t) -> Tot bool) 
  : Pure (list nat) 
    (requires (current_index = (L.length original_node_list)) ||
              (is_node_list_index current_index original_node_list)
    )
    (ensures fun r -> 
      (is_node_list_index_list r original_node_list) /\
      (forall i. (L.contains i r) ==> (predicate (L.index original_node_list i))) /\
      ((Cons? r) <==> (exists i2. (L.contains i2 r) /\ (predicate (L.index original_node_list i2))))
    )
    (decreases (L.length original_node_list) - current_index)
  =
  let length = (L.length original_node_list) in
  match (length = current_index) with
  | true -> []
  | false -> 
  let current_item = (L.index original_node_list current_index) in
  let next_index = (current_index + 1) in
  let results_from_end = (get_index_list_from_predicate_core original_node_list next_index predicate) in
  match (predicate current_item) with
  | true -> current_index::results_from_end
  | false -> results_from_end

//---|

//--- (T.node_list t) propositions ---
private let is_node_list_index_and_is_node_list_index_rewrite_are_equal
  : prop =
  (forall (t:eqtype) (index:nat) (node_list:T.node_list t).
    (is_node_list_index index node_list) = (is_node_list_index_rewrite node_list index)
  )

let list_level_is_greater_or_equal_than_any_element_level 
  (t:eqtype) (l:T.node_list t) 
  : prop =
  (forall (node:T.node t). 
    (L.contains node l) ==> ((get_level node) <= get_list_level_impl l))

(*
let result_of_get_index_list_from_predicate_is_node_list_index_list #t
  (node_list: T.node_list t) (predicate: (T.node t) -> Tot bool) 
  : prop =
  let v0 = get_index_list_from_predicate_impl node_list predicate in
  is_node_list_index_list v0 node_list
*)

let result_of_get_index_list_from_predicate_is_shorter_or_equal_than_original_node_list #t
  (original_node_list: T.node_list t)
  (current_index: nat) 
  (predicate: (T.node t) -> Tot bool) 
  : prop =
  (L.length (get_index_list_from_predicate_core original_node_list 0 predicate)) <= (L.length original_node_list)

let element_of_result_of_get_index_list_from_predicate_satisfies_predicate #t
  (node_list: T.node_list t) (predicate: (T.node t) -> Tot bool) 
  (index: nat)
  : prop =
  let index_list = get_index_list_from_predicate_core node_list 0 predicate in
  ((L.contains index index_list) <==> (
    (is_node_list_index index node_list) /\
    (predicate (L.index node_list index))
  ))
//---|

//--- (T.node_list t) lemmas ---
private let rec lemma_list_level_is_greater_or_equal_than_element_level
  (t:eqtype) (n:T.node t) (l:T.node_list t)
  : Lemma 
    (requires L.contains n l)
    (ensures (
      ((get_level n) <= get_list_level_impl l)
    ))
    (decreases l)
  =
  let hd::tl = l in
  if (hd = n) then ()
  else 
    lemma_list_level_is_greater_or_equal_than_element_level t n tl

let lemma_list_level_is_greater_or_equal_than_any_element_level (t:eqtype) (l:T.node_list t)
  : Lemma
    (ensures (
      list_level_is_greater_or_equal_than_any_element_level t l
    ))
  = 
  introduce forall (it_node:T.node t{ L.contains it_node l }). 
    ((get_level it_node) <= get_list_level_impl l) with (
      lemma_list_level_is_greater_or_equal_than_element_level t it_node l
    )

(*
let lemma_result_of_get_index_list_from_predicate_is_shorter_or_equal_than_original_node_list #t
  (original_node_list: T.node_list t)
  (current_index: nat) 
  (predicate: (T.node t) -> Tot bool) 
  : Lemma 
    (ensures result_of_get_index_list_from_predicate_is_shorter_or_equal_than_original_node_list
      original_node_list current_index predicate
    )
  = 
  admit ()
*)

private let lemma_element_of_result_of_get_index_list_from_predicate_satisfies_predicate_aux1 #t
  (node_list: T.node_list t) (predicate: (T.node t) -> Tot bool) 
  (index: nat)
  : Lemma (requires L.contains index (get_index_list_from_predicate_core node_list 0 predicate))
    (ensures is_node_list_index index node_list)
  =
  ()
  //assert (is_node_list_index_and_is_node_list_index_rewrite_are_equal);
  //let v1 = (get_index_list_from_predicate_core node_list 0 predicate) in (
    //assert (Cons? node_list);
    //L.for_all_mem (is_node_list_index_rewrite node_list) v1
    //assert (is_node_list_index_list v1 node_list);
    //assert (forall i. (L.contains i v1) ==> (is_node_list_index i node_list))
  //)

private let lemma_element_of_result_of_get_index_list_from_predicate_satisfies_predicate_aux2 #t
  (node_list: T.node_list t) (predicate: (T.node t) -> Tot bool) 
  (index: nat)
  : Lemma (requires 
      (L.contains index (get_index_list_from_predicate_core node_list 0 predicate)) /\ 
      (is_node_list_index index node_list)
    )
    (ensures predicate (L.index node_list index))
  =
  ()
  //lemma_element_of_result_of_get_index_list_from_predicate_satisfies_predicate_aux1 node_list predicate index

let lemma5 #t
  (node_list: T.node_list t) 
  (predicate: (T.node t) -> Tot bool) (index: nat)
  : Lemma 
    (requires 
      (index <= (L.length node_list)) /\ (
        let v1 = (get_index_list_from_predicate_core node_list index predicate) in
        (Cons? v1) && (L.hd v1 = index)
      )
    )
    (ensures
      (index >= 1) ==> (
        let v1 = (get_index_list_from_predicate_core node_list index predicate) in
        let l1 = L.length v1 in
        let v2 = (get_index_list_from_predicate_core node_list (index - 1) predicate) in
        let l2 = L.length v2 in (
          ((l1 = l2) ==> (L.hd v2 = index)) /\
          ((l1 + 1 = l2) ==> 
            ((L.hd v2 = index - 1) /\ (L.hd (L.tl v2) = index)))
        )
      )
    )
  =
  if not (index >= 1) then ()
  else (
    let v1 = (get_index_list_from_predicate_core node_list index predicate) in
    let l1 = L.length v1 in
    let v2 = (get_index_list_from_predicate_core node_list (index - 1) predicate) in
    let l2 = L.length v2 in
    (
      assert (Cons? v2);
      assert ((l1 = l2) || (l1 + 1 = l2)); // 하나라도 빼면 증명실패
      //assert (forall (n:nat). (n=0 \/ n=1) ==> (l1 + n = l2));
      // - 이건 증명 실패...뭔가 미묘한 차이가 있나
      assert (forall (n:nat). (l1 + n = l2) ==> (n=0 \/ n=1))
    )
  )

let lemma4 #t
  (node_list: T.node_list t) (predicate: (T.node t) -> Tot bool) (index: nat)
  : Lemma
    (requires
      (index <= (L.length node_list)) /\ 
      (L.contains index (get_index_list_from_predicate_core node_list index predicate))
    )
    (ensures
      ((index >= 1) ==> (L.contains index (get_index_list_from_predicate_core node_list (index-1) predicate))) /\
      ((index = 0) ==> (L.contains index (get_index_list_from_predicate_core node_list 0 predicate)) )
    )
  =
  lemma5 node_list predicate index

// 수학적 귀납법의, '기본'으로 돌아가서 'requires' 와 'ensures' 를 작성하자.
let rec lemma6 #t
  (node_list: T.node_list t) (predicate: (T.node t) -> Tot bool) (index: nat) 
  (index_decrease: nat)
  : Lemma
    (requires (
      let v1 = index - index_decrease in
      (0 <= v1) /\ (v1 <= index) /\ (index <= (L.length node_list)) /\ 
      (L.contains index (get_index_list_from_predicate_core node_list v1 predicate))
    ))
    (ensures (
      let v1 = index - index_decrease in
      ((v1 >= 1) ==> (L.contains index (get_index_list_from_predicate_core node_list (v1 - 1) predicate))) /\
      ((v1 = 0) ==> (L.contains index (get_index_list_from_predicate_core node_list 0 predicate)))
    ))
    (decreases (index - index_decrease))
  =
  if index_decrease = 0 then (
    lemma4 node_list predicate index
  ) else if (index = index_decrease) then (
    assert (index - index_decrease = 0)
  ) else (
    assert (index_decrease < index);
    lemma6 node_list predicate index (index_decrease + 1)
  )
  
let rec lemma7 #t
  (node_list: T.node_list t) (predicate: (T.node t) -> Tot bool) (index: nat) 
  (index_decrease: nat)
  : Lemma
    (requires (
      let v1 = index - index_decrease in
      (0 <= v1) /\ (v1 <= index) /\ (index <= (L.length node_list)) /\ 
      (L.contains index (get_index_list_from_predicate_core node_list v1 predicate))
    ))
    (ensures (L.contains index (get_index_list_from_predicate_core node_list 0 predicate)))
    (decreases (index - index_decrease))
  =
  lemma6 node_list predicate index index_decrease; //...이거 없어도 증명은 되네...??
  let v1 = index - index_decrease in
  if v1 = 0 then ()
  else
    lemma7 #t node_list predicate index (index_decrease + 1)
// 대체 얘가 재귀적 증명을 어떻게 하는건지...??
// - requires 를 계속 만족하다가, 딱 마지막에만 ensures 를 만족하면 되는거야...? 그렇게 해도 증명이 돼...? 어떻게지?
// - lemma n의 증명은, lemma n'{n' << n}이다, 뭐 이런거냐?
//   - 이 증명 방법이 타당한 이유는?


let lemma1 #t
  (node_list: T.node_list t) (predicate: (T.node t) -> Tot bool) (index: nat)
  : Lemma 
    (requires 
      (index <= (L.length node_list)) /\ 
      (let v1 = (get_index_list_from_predicate_core node_list index predicate) in
      (Cons? v1) && (index = L.hd v1))
    )
    (ensures (L.contains index (get_index_list_from_predicate_core node_list 0 predicate)))
  =
  lemma5 node_list predicate index;
  lemma4 node_list predicate index;
  lemma7 node_list predicate index 0
  (*
  introduce forall (index_decrease: nat{
    let v2 = index - index_decrease in
    (0 <= v2) /\ (v2 <= index) /\ (index <= (L.length node_list)) /\ 
    (L.contains index (get_index_list_from_predicate_core node_list v2 predicate))
  }). (L.contains index (get_index_list_from_predicate_core node_list 0 predicate)) with
  lemma7 node_list predicate index index_decrease
  *)


private let rec lemma2_core #t
  (original_node_list: T.node_list t) (predicate: (T.node t) -> Tot bool) (original_index: nat)
  (current_index: nat)
  : Lemma
    (requires 
      //(original_index <= (L.length original_node_list)) /\ 
      (is_node_list_index original_index original_node_list) /\ 
      (predicate (L.index original_node_list original_index)) /\
      (current_index <= original_index)
      // /\ (current_index < (L.length original_node_list))
    )
    (ensures 
      //(original_index = current_index) ==>
      (L.contains original_index (get_index_list_from_predicate_core original_node_list 0 predicate))
    )
    (decreases (L.length original_node_list) - current_index)
  =
  assert (current_index < (L.length original_node_list));
  lemma1 original_node_list predicate original_index;
  let length = (L.length original_node_list) in
  match (original_index = current_index) with
  | true -> assert (
      let v1 = get_index_list_from_predicate_core original_node_list original_index predicate in
      (Cons? v1) && (original_index = L.hd v1)
    )
  | false -> 
    lemma2_core
      original_node_list predicate original_index (current_index + 1)

let lemma2 #t
  (node_list: T.node_list t) (predicate: (T.node t) -> Tot bool) (index: nat)
  : Lemma 
    (requires       
      (is_node_list_index index node_list) /\ 
      (predicate (L.index node_list index))
    )
    (ensures 
      //(index <= (L.length node_list)) /\
      (L.contains index (get_index_list_from_predicate_core node_list 0 predicate))
    )
  =
  lemma2_core node_list predicate index 0


(*
let lemma_element_of_result_of_get_index_list_from_predicate_satisfies_predicate #t
  (node_list: T.node_list t) (predicate: (T.node t) -> Tot bool) 
  (index: nat)
  : Lemma (ensures element_of_result_of_get_index_list_from_predicate_satisfies_predicate node_list predicate index)
  =
  if not (L.contains index (get_index_list_from_predicate_core node_list 0 predicate)) then ()
  else (
    lemma_element_of_result_of_get_index_list_from_predicate_satisfies_predicate_aux1 node_list predicate index;
    lemma_element_of_result_of_get_index_list_from_predicate_satisfies_predicate_aux2 node_list predicate index
  )
*)
  
(*
private let rec lemma_result_of_get_index_list_from_predicate_is_node_list_index_list_core #t
  (original_node_list: T.node_list t)
  (current_index: nat) 
  (predicate: (T.node t) -> Tot bool) 
  : Lemma 
    (requires current_index <= (L.length original_node_list) )
    (ensures result_of_get_index_list_from_predicate_is_node_list_index_list 
        original_node_list predicate)
    (decreases (L.length original_node_list) - current_index)
  =
  let length = (L.length original_node_list) in
  match (length = current_index) with
  | true -> ()
  | false -> 
  let current_item = (L.index original_node_list current_index) in
  let next_index = (current_index + 1) in
  let results_from_end = (lemma_result_of_get_index_list_from_predicate_is_node_list_index_list_core 
                          original_node_list next_index predicate) in
  match (predicate current_item) with
  | true -> current_index::results_from_end
  | false -> results_from_end

let lemma_result_of_get_index_list_from_predicate_is_node_list_index_list #t
  (node_list: T.node_list t) //(predicate: (T.node t) -> Tot bool)
  : Lemma 
    (ensures (forall predicate. result_of_get_index_list_from_predicate_is_node_list_index_list
      node_list predicate))
  =
  introduce forall (predicate: (T.node t) -> Tot bool).
  result_of_get_index_list_from_predicate_is_node_list_index_list node_list predicate with
  lemma_result_of_get_index_list_from_predicate_is_node_list_index_list_core
    node_list node_list predicate 0 []
*)


//---|

//--- (T.node_list t) members ---
let get_list_level #t (l:T.node_list t) 
  : Pure nat True (ensures fun _ -> list_level_is_greater_or_equal_than_any_element_level t l) =
  lemma_list_level_is_greater_or_equal_than_any_element_level t l;
  get_list_level_impl l

let get_item #t (node_list:T.node_list t) (index:T.node_list_index node_list)
  : Tot (T.node t)
  =
  L.index node_list index

let try_get_item #t (node_list:T.node_list t) (index:nat)
  : Tot (option (T.node t))
  =
  match to_maybe_node_list_index index node_list with
  | T.ISome _ v1 -> Some (get_item node_list v1)
  | T.INone _ -> None

let get_index_list_from_predicate #t
  (node_list: T.node_list t) (predicate: (T.node t) -> Tot bool) 
  : Pure (list nat) True
    (ensures fun r -> //is_node_list_index_list r node_list
      ( forall (index:nat).
        element_of_result_of_get_index_list_from_predicate_satisfies_predicate 
          node_list predicate index
      )
    )
  =
  introduce forall (index:nat).
    ((is_node_list_index index node_list) /\ (predicate (L.index node_list index)) ==> 
    (L.contains index (get_index_list_from_predicate_core node_list 0 predicate))) with
  (
    if not (is_node_list_index index node_list) then ()
    else if not (predicate (L.index node_list index)) then ()
    else lemma2 node_list predicate index
  );
  get_index_list_from_predicate_core node_list 0 predicate

let try_get_first_index_of_predicate #t 
  (node_list:T.node_list t) (predicate: (T.node t) -> Tot bool)
  : Pure (T.maybe_node_list_index node_list) True
        (ensures fun r ->
          match r with
          | T.INone _ -> true
          | T.ISome _ v1 -> 
            (element_of_result_of_get_index_list_from_predicate_satisfies_predicate node_list predicate v1)
        )
        (decreases node_list)
  = 
  match get_index_list_from_predicate node_list predicate with
  | [] -> T.INone node_list
  | hd::tl -> T.ISome node_list hd
//---|

//--- try_get_index ---

let lemma8 #t
  (node_list:T.node_list t) (node:T.node t)
  : Lemma 
    (ensures 
      (Cons? (L.filter (op_Equality node) node_list)) <==> (L.contains node node_list)
    )
  =
  L.mem_filter_forall (op_Equality node) node_list

(*
private let rec prop1 
  (t:eqtype) (l:list t) (item:t) 
  (i:nat{i < (L.length l)})
  : Tot prop (decreases i) =
  match i with
  | 0 -> ((L.index l i) = item)
  | _ -> ((L.index l i) = item) \/ (prop1 t l item (i-1))
*)

let lemma11 (#t:eqtype) 
  (l:list t) (item:t)
  : Lemma 
    (requires (L.contains item l))
    (ensures (exists i. (L.index l i) = item))
  =
  assert ((L.index l (L.index_of l item)) = item)
  (*
  match (L.index l index) = item with
  | true -> ()
  | false -> (
      assert (index > 0);
      (lemma11 t l item (index-1))
    )
  *)
  
  (*
  let head_item = L.hd l in
  match (head_item = item) with
  | true -> (assert ((L.index l 0) = item))
  | false -> (lemma11 t (L.tl l) item)
  *)


let lemma10 #t
  (node_list:T.node_list t) (predicate: (T.node t) -> Tot bool) 
  : Lemma
    (ensures (exists n. (L.contains n node_list) /\ (predicate n)) <==> (Cons? (get_index_list_from_predicate node_list predicate)))
    //(decreases node_list)
  =
  //get_index_list_from_predicate
  let v1 = (get_index_list_from_predicate node_list predicate) in
  let v2 = (exists i2. (L.contains i2 v1) /\ (predicate (L.index node_list i2))) in
  assert (v2 <==> (Cons? v1));
  //(introduce exists (i3:nat{i3 < L.length node_list}). ((predicate (L.index node_list i3)) <==> (L.contains (L.index node_list i3) node_list)) with
  //  (L.lemma_index_memP node_list i3))  
  introduce forall (i3:T.node_list_index node_list). 
    (L.contains (L.index node_list i3) node_list) with (L.lemma_index_memP node_list i3);
  introduce forall (n:T.node t). (L.contains n node_list) ==> (exists i. (L.index node_list i) = n) with
    (
      if not (L.contains n node_list) then ()
      else lemma11 node_list n
    );
  assert (forall n. (L.contains n node_list) ==> (exists i. (L.index node_list i) = n));
  assert (v2 <==> (exists i. (L.contains (L.index node_list i) node_list) /\ (predicate (L.index node_list i))));
  assert ((exists i. (L.contains (L.index node_list i) node_list) /\ (predicate (L.index node_list i))) <==> (exists n. (L.contains n node_list) /\ (predicate n)))
  // assert (v2 <==> (exists n. (L.contains n node_list) /\ (predicate n)))
  //introduce forall (i4:nat{i4 < L.length node_list}). 
  //assert (forall (i4:nat). (predicate (L.index node_list i4)) <==> ((L.contains (L.index node_list i4) node_list) /\ (predicate (L.index node_list i4))))
  //assert ((exists i2. (predicate (L.index node_list i2))) <==> (exists n. (L.contains n node_list) /\ (predicate n)))

let lemma9 #t
  (node_list:T.node_list t) (predicate: (T.node t) -> Tot bool) 
  : Lemma 
    (ensures
      (Cons? (get_index_list_from_predicate node_list predicate)) <==>
      (Cons? (L.filter predicate node_list))
    )
  =
  L.mem_filter_forall predicate node_list;
  assert ((exists n. (L.contains n node_list) /\ (predicate n)) <==> (Cons? (L.filter predicate node_list)));
  lemma10 node_list predicate;
  assert ((exists n. (L.contains n node_list) /\ (predicate n)) <==> (Cons? (get_index_list_from_predicate node_list predicate)))

(*
    let v1 = get_index_list_from_predicate node_list predicate in 
    let v2 = L.filter predicate node_list in
    if predicate hd then (
      //assert (((Cons? v1) && (L.hd v1 = 0)) ==> )
      assert (Cons? v1);
      assert (Cons? v2)
    ) else (
      assert ((Cons? (get_index_list_from_predicate tl predicate)) <==> (Cons? (L.filter predicate tl)));
      lemma9 tl node predicate
    )
*)

let try_get_index #t
  (node_list:T.node_list t) (node:T.node t)
  : Pure (T.maybe_node_list_index node_list) True
  (*
    (ensures fun r ->
      match r with
      | T.ISome nl v1 -> L.contains node node_list
      | T.INone nl -> not (L.contains node node_list)
    )
  *)
  (*  
    (ensures fun r ->
      match (L.contains node node_list) with
      | true -> T.ISome? r
      | false -> T.INone? r
    )
  *)
    (ensures (fun r ->
      (T.ISome? r) <==> (L.contains node node_list)
    ))
  =
  lemma8 node_list node;
  lemma9 node_list (op_Equality node);
  assert ((Cons? (get_index_list_from_predicate node_list (op_Equality node))) <==>
          (Cons? (L.filter (op_Equality node) node_list)));
  try_get_first_index_of_predicate node_list (op_Equality node)

(*
let try_get_index #t
  (node_list:T.node_list t) (node:T.node t)
  : Pure (T.maybe_node_list_index node_list) True
    (ensures fun r ->
      match (L.contains node node_list) with
      | true -> T.ISome? r
      | false -> T.INone? r
    )
  =
  assert 
*)
//---