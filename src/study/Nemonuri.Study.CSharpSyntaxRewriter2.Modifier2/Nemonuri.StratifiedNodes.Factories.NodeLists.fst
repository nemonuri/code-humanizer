module Nemonuri.StratifiedNodes.Factories.NodeLists

module L = FStar.List.Tot
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
    (ensures fun _ -> Common.filter_theorem exclusion_predicate node_list)
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
  : Tot (N.node_list t)
  =
  assume (Some? (N.try_get_first_index_of_predicate node_list (op_Equality node)));
  let Some index = N.try_get_first_index_of_predicate node_list (op_Equality node) in
  replace_range_at node_list index inserting_node_list


//---|