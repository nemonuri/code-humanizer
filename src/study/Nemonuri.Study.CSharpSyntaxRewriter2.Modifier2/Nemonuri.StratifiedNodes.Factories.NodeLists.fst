module Nemonuri.StratifiedNodes.Factories.NodeLists

module L = FStar.List.Tot
module Lp = FStar.List.Pure
module I = Nemonuri.StratifiedNodes.Internals
module N = Nemonuri.StratifiedNodes.Nodes
module C = Nemonuri.StratifiedNodes.Children
module Id = Nemonuri.StratifiedNodes.Indexes
module E = Nemonuri.StratifiedNodes.Nodes.Equivalence
module Common = Nemonuri.StratifiedNodes.Common
module Fn = Nemonuri.StratifiedNodes.Factories.Nodes

//--- theory members ---

// https://learn.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxlist-1?view=roslyn-dotnet-4.12.0
// https://github.com/dotnet/roslyn/blob/main/src/Compilers/Core/Portable/Syntax/SyntaxList%601.cs

let rec create_leaf_node_list 
  (#t:eqtype) (values:list t)
  : Tot (N.node_list t)
    (decreases values)
  =
  match values with
  | [] -> []
  | hd::tl -> 
    (Fn.create_leaf_node hd)::
    (create_leaf_node_list tl)

let insert_range #t 
  (node_list:N.node_list t) 
  (index:nat{ index <= L.length node_list })
  (inserting_node_list:N.node_list t)
  : Tot (N.node_list t)
  =
  let (l1, l2) = L.splitAt index node_list in
  let l3 = L.append l1 inserting_node_list in
  L.append l3 l2

let insert #t
  (node_list:N.node_list t) 
  (index:nat{ index <= L.length node_list })
  (inserting_node:N.node t)
  : Tot (N.node_list t)
  =
  insert_range node_list index [inserting_node]


let add_range #t
  (node_list:N.node_list t) 
  (inserting_node_list:N.node_list t)
  : Tot (N.node_list t)
  =
  insert_range node_list (L.length node_list) inserting_node_list

let add #t
  (node_list:N.node_list t) 
  (inserting_node:N.node t)
  : Tot (N.node_list t)
  =
  insert node_list (L.length node_list) inserting_node


let remove_all #t 
  (node_list: N.node_list t)
  (exclusion_predicate: (N.node t) -> Tot bool) 
  : Pure (N.node_list t) True
    (ensures fun r -> (forall x. (L.contains x r) ==> (exclusion_predicate x)))
  =
  L.mem_filter_forall exclusion_predicate node_list;
  L.filter exclusion_predicate node_list

let rec remove_first #t 
  (node_list:N.node_list t) (exclusion_predicate: (N.node t) -> Tot bool)
  : Tot (N.node_list t) (decreases node_list)
  =
  match node_list with
  | [] -> []
  | hd::tl ->
  match (exclusion_predicate hd) with
  | false -> tl
  | true -> hd::(remove_first tl exclusion_predicate)

let remove #t (node_list:N.node_list t) (node:N.node t)
  : Tot (N.node_list t)
  =
  remove_first node_list (op_disEquality node)

let remove_at #t (node_list:N.node_list t) (index:nat{ index < L.length node_list })
  : Tot (N.node_list t)
  =
  remove node_list (L.index node_list index)


let replace_range_at #t 
  (node_list:N.node_list t) 
  (index:nat{ index < L.length node_list })
  (inserting_node_list:N.node_list t)
  : Tot (N.node_list t)
  =
  let v1 = remove_at node_list index in
  insert_range node_list index inserting_node_list

let replace_range #t
  (node_list:N.node_list t) 
  (node:N.node t) //{ L.contains node node_list }
  (inserting_node_list:N.node_list t)
  : Pure (N.node_list t)
    (requires L.contains node node_list)
    (ensures fun _ -> true)
  =
  //assume (N.ISome? (N.try_get_first_index_of_predicate node_list (op_Equality node)));
  let N.ISome _ index = N.try_get_index node_list node in
  replace_range_at node_list index inserting_node_list

let replace #t
  (node_list:N.node_list t) 
  (node:N.node t{ L.contains node node_list })
  (inserting_node:N.node t)
  : Tot (N.node_list t)
  =
  replace_range node_list node [inserting_node]

private let append #t (l1:list t) (l2:list t) 
  : Pure (list t) True
    (ensures fun r -> (L.length r) = (L.length l1 + L.length l2))
  =
  L.append_length l1 l2;
  L.append l1 l2

//#push-options "--split_queries always"
(*
let swap #t
  (node_list:N.node_list t)
  (index1:N.node_list_index node_list)
  (index2:N.node_list_index node_list)
  : Pure (N.node_list t) True
    (ensures fun r -> 
      ((L.length node_list) = (L.length r)) /\ (
      match (index1 = index2) with
      | true -> r = node_list
      | false -> true
    ))
  =
  match (index1 = index2) with
  | true -> node_list
  | false -> 
  let (index_1st, index_2nd) = (
    match (index1 < index2) with
    | true -> (index1, index2)
    | false -> (index2, index1)
  ) in (
    assert ((L.length node_list) >= 2);
    assert (index_1st < index_2nd);
    assert (index_1st < ((L.length node_list) - 1));

    let (v1, v2) = L.splitAt index_1st node_list in
    L.lemma_splitAt_snd_length index_1st node_list;
    Lp.splitAt_length index_1st node_list;
    assert ((L.length v1) = index_1st);
    assert (((L.length v1) + (L.length v2)) = L.length node_list);
    assert (Cons? v2);
    let v2_head::v2_tail = v2 in
    Lp.lemma_splitAt_index_hd index_1st node_list;
    assert ((L.index node_list index_1st) = v2_head);

    assert ((index_2nd - index_1st) >= 1);
    let next_index = (index_2nd - index_1st - 1) in
    let (v3, v4) = L.splitAt next_index v2_tail in
    L.lemma_splitAt_snd_length next_index v2_tail;
    Lp.splitAt_length next_index v2_tail;
    assert (Cons? v4);
    let v4_head::v4_tail = v4 in
    (*
    let q1_1 = (L.index v2_tail next_index) in
    assert (q1_1 = v4_head);
    let q1_2 = (L.index v2 (index_2nd - index_1st)) in
    assert (q1_2 = q1_1);
    Lp.lemma_splitAt node_list v1 v2 index_1st;
    assert ((L.append v1 v2) == node_list);
    L.index_extensionality (L.append v1 v2) node_list;
    Lp.lemma_splitAt_reindex_right index_1st node_list (index_2nd - index_1st);
    let q1_3 = (L.index node_list index_2nd) in
    assert (q1_3 = q1_2);
    *)
    //Lp.lemma_splitAt_index_hd next_index v2_tail;
    //Lp.lemma_splitAt_reindex_right index_1st node_list (index_2nd - index_1st);
    //...아니 왜 뻗어버리면서 증명이 안 되냐...? 너무 길어서...?
    assume ((L.index node_list index_2nd) = v4_head);

    let r1 = ([v2_head] `append` v4_tail) in
    assert (L.length r1 = L.length v4);
    let r2 = (v3 `append` r1) in
    assert (L.length r2 = L.length v2_tail);
    let r3 = ([v4_head] `append` r2) in
    assert (L.length r3 = L.length v2);
    let r4 = (v1 `append` r3) in
    assert (L.length r4 = (L.length v2 + L.length v1));
    assert ((L.length v2 + L.length v1) = L.length node_list);
    r4
  )
*)
//#pop-options 

// ...assert 는 나중에 필요한 것만 선택하기...
let swap #t
  (node_list:N.node_list t)
  (index1:N.node_list_index node_list)
  (index2:N.node_list_index node_list)
  : Tot (N.node_list t)
  =
  match (index1 = index2) with
  | true -> node_list
  | false -> 
  let (index_1st, index_2nd) = (
    match (index1 < index2) with
    | true -> (index1, index2)
    | false -> (index2, index1)
  ) in (
    let (v1, v2) = L.splitAt index_1st node_list in
    L.lemma_splitAt_snd_length index_1st node_list;
    assert (Cons? v2);
    let v2_head::v2_tail = v2 in

    let next_index = (index_2nd - index_1st - 1) in
    let (v3, v4) = L.splitAt next_index v2_tail in
    L.lemma_splitAt_snd_length next_index v2_tail;
    assert (Cons? v4);
    let v4_head::v4_tail = v4 in

    (v1 `append` [v4_head] `append` v3 `append` [v2_head] `append` v4_tail)
  )

//---|