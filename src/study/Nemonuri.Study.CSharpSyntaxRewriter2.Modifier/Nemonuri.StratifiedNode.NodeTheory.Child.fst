module Nemonuri.StratifiedNode.NodeTheory.Child

module L = FStar.List.Tot.Base
open Nemonuri.StratifiedNode
open Nemonuri.StratifiedNode.ListTheory
open Nemonuri.StratifiedNode.NodeTheory.Common

let child_level (parent_level:pos) = cl:pos{ cl < parent_level }

let child_node
  (#t:eqtype) (#parent_level:pos) 
  (parent:stratified_node t parent_level)
  (clv:(child_level parent_level)) =
    (cn:(stratified_node t clv){ is_child parent cn })

let child_node_func
  (#t:eqtype) (#parent_level:pos) (parent:stratified_node t parent_level) (t2:Type) =
    (#clv:(child_level parent_level)) ->
    (cn:(child_node parent clv)) ->
    Tot t2

let child_node_predicate
  (#t:eqtype) (#parent_level:pos) (parent:stratified_node t parent_level) =
  child_node_func parent bool

private let rec select_in_children_core
  (#t:eqtype) (#lv:pos) (parent:stratified_node t lv) 
  (#children_mlv:nat)
  (subchildren:stratified_node_list t children_mlv{ is_subchildren parent subchildren })
  (#t2:Type) (selector:(child_node_func parent t2))
  : Tot (x:(list t2){ L.length x = get_length subchildren }) 
        (decreases subchildren)
  = if is_empty subchildren then 
      []
    else
      (
        lemma_subchildren_hd_is_child parent subchildren;
        lemma_subchildren_tl_is_subchildren parent subchildren;
        let hd = get_hd subchildren in
        lemma_child_node_level_is_lower_than_parent parent hd;
        (selector #(get_level hd) hd)::
        (select_in_children_core parent (get_tl subchildren) selector)
      )

let select_in_children
  (#t:eqtype) (#lv:pos) (parent:stratified_node t lv) 
  (#t2:Type) (selector:(child_node_func parent t2))
  : Tot (x:(list t2){ L.length x = get_length parent.children })
  = select_in_children_core parent parent.children selector

private let rec exists_in_children_core
  (#t:eqtype) (#lv:pos) (parent:stratified_node t lv) 
  (#children_mlv:nat)
  (subchildren:stratified_node_list t children_mlv{ is_subchildren parent subchildren })
  (predicate:(child_node_predicate parent))
  : Tot bool (decreases subchildren)
  = if is_empty subchildren then 
      false
    else
      (
        lemma_subchildren_hd_is_child parent subchildren;
        lemma_subchildren_tl_is_subchildren parent subchildren;
        lemma_child_node_level_is_lower_than_parent parent (get_hd subchildren);
        if predicate (get_hd subchildren) then 
          true
        else 
          exists_in_children_core parent (get_tl subchildren) predicate
      )

let exists_in_children
  (#t:eqtype) (#lv:pos) (parent:stratified_node t lv)
  (predicate:(child_node_predicate parent))
  : Tot bool
  = exists_in_children_core parent parent.children predicate