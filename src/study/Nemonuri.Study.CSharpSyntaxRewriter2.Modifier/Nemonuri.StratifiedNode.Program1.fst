module Nemonuri.StratifiedNode.Program1

module L = FStar.List.Tot
open Nemonuri.StratifiedNode
open Nemonuri.StratifiedNode.NodeTheory
open Nemonuri.StratifiedNode.ListTheory
open Nemonuri.StratifiedNode.IndexTheory
module T = Nemonuri.StratifiedNode.TypeTheory1

(*
let rec run (#lv:pos) (sn:T.node lv{ T.has_kind_compilation_unit sn })
  : Tot (stratified_node_with_level T.data)
        (decreases %[T.get_count_of_argument_need_to_substitute_expression sn])
  = match (try_get_indexes_of_descendant_or_self_from_predicate sn T.may_have_kind_argument_need_to_substitute_expression) with
    | None -> (assert (T.get_count_of_argument_need_to_substitute_expression sn = 0); (get_stratified_node_with_level sn))
    | Some indexes1 -> (assert (T.get_count_of_argument_need_to_substitute_expression sn > 0); (get_stratified_node_with_level sn))
*)