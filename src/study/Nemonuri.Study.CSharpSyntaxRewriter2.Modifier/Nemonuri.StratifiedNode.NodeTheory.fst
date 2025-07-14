module Nemonuri.StratifiedNode.NodeTheory

open Nemonuri.StratifiedNode

let get_level (#t:eqtype) (#lv:pos) (sn:stratified_node t lv) : Tot pos = lv

let stratified_node_level_is_children_level_plus_one
  (#t:eqtype) (#lv:pos) (sn:stratified_node t lv)
  : Lemma ((SNode?.children_level sn + 1) = (get_level sn))
  = ()