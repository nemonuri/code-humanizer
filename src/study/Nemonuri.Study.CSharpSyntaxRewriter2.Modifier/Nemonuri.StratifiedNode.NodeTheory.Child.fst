module Nemonuri.StratifiedNode.NodeTheory.Child

module L = FStar.List.Tot.Base
open Nemonuri.StratifiedNode
open Nemonuri.StratifiedNode.ListTheory
open Nemonuri.StratifiedNode.NodeTheory.Common

let is_child_level (parent_level:pos) (cl:pos) : Tot bool = cl < parent_level 

(*
let child_level (parent_level:pos) = cl:pos{ is_child_level parent_level cl}
*)

let child_node
  (#t:eqtype) 
  (#parent_level:pos) (parent:stratified_node t parent_level)
  (child_level:pos) =
  (cn:(stratified_node t child_level){ is_child parent cn })

let is_child_node_index 
  (#t:eqtype) (#parent_level:pos) (parent:stratified_node t parent_level)
  (cni:nat)
  : Tot bool
  = cni < (get_children_length parent)

let child_node_index 
  (#t:eqtype) (#parent_level:pos) (parent:stratified_node t parent_level) =
  nat1:nat{ is_child_node_index parent nat1 }

let get_child_node_index
  (#t:eqtype) (#parent_level:pos) 
  (#parent:stratified_node t parent_level)
  (#child_level:pos)
  (cn:child_node parent child_level)
  : Tot (child_node_index parent)
  = index_of parent.children cn

let child_node_func
  (t:eqtype) (t2:Type) =
    (#parent_level:pos) ->
    (parent:stratified_node t parent_level) ->
    (#child_level:pos) ->
    (cn:(child_node parent child_level)) ->
    Tot t2

(*
let parent_bound_child_node_func
  (t:eqtype) (t2:Type)
  (#parent_level:pos) (parent:stratified_node t parent_level)
  = (refined_stratified_node_func t t2 (is_child parent))

let bound_parent
  (#t:eqtype) (#t2:Type) (#parent_level:pos) (parent:stratified_node t parent_level)
  (cnf:child_node_func t t2)
  : Tot (parent_bound_child_node_func t t2 parent)
  = cnf parent

noeq
type parent_bound_child_node_func_info (t:eqtype) (t2:Type) (#parent_level:pos) (parent:stratified_node t parent_level) 
= {
  original_func: (child_node_func t t2);
  bound_func: (parent_bound_child_node_func t t2 parent)
}
*)

(*
let to_stratified_node_func
  (#t:eqtype) (#t2:Type) (cnf:child_node_func t t2)
  (#parent_level:pos) (parent:stratified_node t parent_level)
  : Tot (stratified_node_func t t2)
  = cnf #parent_level parent
*)

(*
let child_node_and_index_func
  (t:eqtype) (t2:Type) =
    (#parent_level:pos) ->
    (parent:stratified_node t parent_level) ->
    (#clv:(child_level parent_level)) ->
    (cn:(child_node parent clv)) ->
    (cni:(child_node_index parent){get_child_node_index cn = cni}) ->
    Tot t2
*)

let child_node_predicate (t:eqtype) =
  child_node_func t bool

let refined_child_node_func
  (t:eqtype) (t2:Type) (predicate:child_node_predicate t) =
  (#parent_level:pos) ->
  (parent:stratified_node t parent_level) ->
  (#child_level:pos) ->
  (child:(child_node parent child_level){ predicate parent child }) ->
  Tot t2

let refined_child_node_predicate (t:eqtype) (predicate:child_node_predicate t) =
  refined_child_node_func t bool predicate

(*
let convert_stratified_node_predicate_to_child_node_predicate
  (#t:eqtype) (snp:stratified_node_predicate t)
  : Tot (child_node_predicate t)
  = fun 
      (#parent_level:pos) (parent:stratified_node t parent_level)
      (#clv:(child_level parent_level)) (cn:(child_node parent clv)) ->
    (snp #clv cn)
*)

(*
type parent_bound_child_node_func (t:eqtype) (t2:Type) =
  | PBound : 
    (#parent_level:pos) ->
    (parent:stratified_node t parent_level) ->
    (original_func:child_node_func t t2) ->
    (bound_func:)
*)

private let rec select_in_children_core
  (#t:eqtype) (#lv:pos) (parent:stratified_node t lv) 
  (#children_mlv:nat)
  (subchildren:stratified_node_list t children_mlv{ is_subchildren parent subchildren })
  (#t2:Type) 
  (selector:refined_child_node_func t t2 (fun parent1 child1 -> (is_equal_node parent parent1) && (is_child parent1 child1)))
  : Tot (x:(list t2){ L.length x = get_length subchildren }) 
        (decreases subchildren)
  = if is_empty subchildren then 
      []
    else
      (
        lemma_for_subchildren parent subchildren;
        let hd = get_hd subchildren in
        (selector parent hd)::
        (select_in_children_core parent (get_tl subchildren) selector)
      )

let select_in_children
  (#t:eqtype) (#lv:pos) (parent:stratified_node t lv) (#t2:Type)
  (selector:refined_child_node_func t t2 (fun parent1 child1 -> (is_equal_node parent parent1) && (is_child parent1 child1)))
  : Tot (x:(list t2){ L.length x = get_length parent.children })
  = select_in_children_core parent parent.children selector

private let rec exists_in_children_core
  (#t:eqtype) (#lv:pos) (parent:stratified_node t lv) 
  (#children_mlv:nat)
  (subchildren:stratified_node_list t children_mlv{ is_subchildren parent subchildren })
  (predicate:refined_child_node_predicate t (fun parent1 child1 -> (is_equal_node parent parent1) && (is_child parent1 child1)))
  : Tot bool (decreases subchildren)
  = if is_empty subchildren then 
      false
    else
      (
        lemma_for_subchildren parent subchildren;
        if predicate parent (get_hd subchildren) then 
          true
        else 
          exists_in_children_core parent (get_tl subchildren) predicate
      )

let exists_in_children
  (#t:eqtype) (#lv:pos) (parent:stratified_node t lv)
  (predicate:refined_child_node_predicate t (fun parent1 child1 -> (is_equal_node parent parent1) && (is_child parent1 child1)))
  : Tot bool
  = exists_in_children_core parent parent.children predicate

(*
private let rec select_with_index_in_children_core
  (#t:eqtype) (#lv:pos) (parent:stratified_node t lv) 
  (#children_mlv:nat)
  (subchildren:stratified_node_list t children_mlv{ is_subchildren parent subchildren })
  (#t2:Type) (selector:(child_node_and_index_func t t2))
  : Tot (x:(list t2){ L.length x = get_length subchildren }) 
        (decreases subchildren)
  = if is_empty subchildren then 
      []
    else
      (
        lemma_for_subchildren parent subchildren;
        let hd = get_hd subchildren in
        (selector parent hd (get_child_node_index hd))::
        (select_with_index_in_children_core parent (get_tl subchildren) selector)
      )

let select_with_index_in_children
  (#t:eqtype) (#lv:pos) (parent:stratified_node t lv) 
  (#t2:Type) (selector:(child_node_and_index_func t t2))
  : Tot (x:(list t2){ L.length x = get_length parent.children })
  = select_with_index_in_children_core parent parent.children selector
*)