module Nemonuri.StratifiedNode.TypeTheory1

(*
compilation_unit = 
  | block*
  ;

block =
  | statement*
  ;

statement =
  | statement_normal: argument*
  | statement_substituted: substitution_index:nat expression{ this is (not expression_blockless_simple) and (not expression_blockless_substituted) }
  ;

argument =
  | expression
  ;

expression =
  | expression_block_containing: blocks:block*{ this.Count >= 1 }
  | expression_blockless_simple
  | expression_blockless_complex
  | expression_blockless_substituted: substitution_index:nat
  ;
*)

module L = FStar.List.Tot
open Nemonuri.StratifiedNode
open Nemonuri.StratifiedNode.NodeTheory
open Nemonuri.StratifiedNode.ListTheory

private let lemma_for_all
  (#t:eqtype) (#mlv:nat) (snl:stratified_node_list t mlv{SCons? snl})
  (#lv:pos) (sn:stratified_node t lv)
  (predicate:stratified_node_predicate t)
  : Lemma (requires (for_all snl predicate) && (contains snl sn))
          (ensures predicate sn)
          (decreases snl)
  = lemma_list_satisfies_for_all_predicate_means_element_satisfies_predicate snl sn predicate; ()

type statement_sum_kind =
  | Statement_normal
  | Statement_substituted: substitution_index:nat -> statement_sum_kind

type expression_sum_kind =
  | Expression_block_containing
  | Expression_blockless_simple
  | Expression_blockless_complex
  | Expression_blockless_substituted: substitution_index:nat -> expression_sum_kind

type kind =
  | Compilation_unit
  | Block
  | Statement: statement_sum_kind -> kind
  | Argument
  | Expression: expression_sum_kind -> kind

type data = { 
  kind: kind
}

let node (lv:pos) = (stratified_node data lv)
let node_list (mlv:nat) = (stratified_node_list data mlv)


let get_kind (#lv:pos) (sn:node lv)
  : Tot kind
  = sn.value.kind

let get_statement_sum_kind (#lv:pos) (sn:node lv{ Statement? (get_kind sn) })
  : Tot statement_sum_kind
  = let Statement v0 = (get_kind sn) in v0

let get_expression_sum_kind (#lv:pos) (sn:node lv{ Expression? (get_kind sn) })
  : Tot expression_sum_kind
  = let Expression v0 = (get_kind sn) in v0


let may_have_kind_argument (#lv:pos) (sn:node lv)
  : Tot bool
  = (Argument? (get_kind sn)) && 
    (get_children_length sn = 1) &&
    (Expression? (get_kind (get_child_at sn 0)))

let may_have_kind_argument_need_to_substitute_expression (#lv:pos) (sn:node lv)
  : Tot bool
  = (may_have_kind_argument sn) && (
      match (get_expression_sum_kind (get_child_at sn 0)) with
      | Expression_block_containing -> true
      | Expression_blockless_complex -> true
      | _ -> false
    )

let may_have_kind_argument_ready_to_substitute_expression (#lv:pos) (sn:node lv)
  : Tot bool
  = (may_have_kind_argument sn) && (
      let esn = (get_child_at sn 0) in
      match (get_expression_sum_kind esn) with
      | Expression_blockless_complex -> true
      | Expression_block_containing ->
          (not (exists_in_descendant_or_self esn may_have_kind_argument_need_to_substitute_expression))
      | _ -> false
    )


let rec has_kind_block (#lv:pos) (sn:node lv)
  : Tot bool (decreases lv)
  = Block? (get_kind sn) &&
    (for_all_children sn has_kind_statement)

and has_kind_statement (#lv:pos) (sn:node lv)
  : Tot bool (decreases lv)
  = Statement? (get_kind sn) && (
      match (get_statement_sum_kind sn) with
      | Statement_normal -> (for_all_children sn has_kind_argument)
      | Statement_substituted _ -> 
          (get_children_length sn = 1) && 
          (has_kind_expression (get_child_at sn 0)) && 
          (not (exists_in_descendant_or_self sn may_have_kind_argument_need_to_substitute_expression))
    )

and has_kind_argument (#lv:pos) (sn:node lv)
  : Tot bool (decreases lv)
  = (Argument? (get_kind sn)) && 
    (get_children_length sn = 1) &&
    (has_kind_expression (get_child_at sn 0))

and has_kind_expression (#lv:pos) (sn:node lv)
  : Tot bool (decreases lv)
  = (Expression? (get_kind sn)) && (
      match (get_expression_sum_kind sn) with
      | Expression_block_containing -> 
          (get_children_length sn >= 1) && 
          (for_all_children sn has_kind_block)
      | Expression_blockless_simple -> (is_leaf sn)
      | Expression_blockless_complex -> (is_leaf sn)
      | Expression_blockless_substituted _ -> (is_leaf sn)
    )

let has_kind_compilation_unit (#lv:pos) (sn:node lv)
  : Tot bool
  = (Compilation_unit? (get_kind sn)) &&
    (for_all_children sn has_kind_block)

