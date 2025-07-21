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

let rec has_kind_block (#lv:pos) (sn:node lv)
  : Tot bool (decreases lv)
  = Block? (get_kind sn) &&
    (for_all_children sn has_kind_statement)

and has_kind_statement (#lv:pos) (sn:node lv)
  : Tot bool (decreases lv)
  = Statement? (get_kind sn) && (
      match (get_statement_sum_kind sn) with
      | Statement_normal -> (for_all_children sn has_kind_argument)
      | Statement_substituted _ -> (get_children_length sn = 1) && (for_all_children sn has_kind_expression)
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
      | Expression_block_containing -> (get_children_length sn >= 1) && (for_all_children sn has_kind_block)
      | Expression_blockless_simple -> (is_leaf sn)
      | Expression_blockless_complex -> (is_leaf sn)
      | Expression_blockless_substituted _ -> (is_leaf sn)
    )

let has_kind_compilation_unit (#lv:pos) (sn:node lv)
  : Tot bool (decreases lv)
  = (Compilation_unit? (get_kind sn)) &&
    (for_all_children sn has_kind_block)








let has_kind_statement_normal (#lv:pos) (sn:node lv)
  : Tot bool
  = (has_kind_statement sn) && (Statement_normal? (get_statement_sum_kind sn))

let has_kind_statement_substituted (#lv:pos) (sn:node lv)
  : Tot bool
  = (has_kind_statement sn) && (Statement_substituted? (get_statement_sum_kind sn))




let has_kind_expression_block_containing (#lv:pos) (sn:node lv)
  : Tot bool
  = (has_kind_expression sn) && (Expression_block_containing? (get_expression_sum_kind sn))

let has_kind_expression_blockless_simple (#lv:pos) (sn:node lv)
  : Tot bool
  = (has_kind_expression sn) && (Expression_blockless_simple? (get_expression_sum_kind sn))

let has_kind_expression_blockless_complex (#lv:pos) (sn:node lv)
  : Tot bool
  = (has_kind_expression sn) && (Expression_blockless_complex? (get_expression_sum_kind sn))

let has_kind_expression_blockless_substituted (#lv:pos) (sn:node lv)
  : Tot bool
  = (has_kind_expression sn) && (Expression_blockless_substituted? (get_expression_sum_kind sn))


let get_substitution_index (#lv:pos) (sn:node lv{ (has_kind_statement_substituted sn) || (has_kind_expression_blockless_substituted sn) })
  : Tot nat
  = if (has_kind_statement_substituted sn) then
      Statement_substituted?.substitution_index (get_statement_sum_kind sn) 
    else
      Expression_blockless_substituted?.substitution_index (get_expression_sum_kind sn) 


let is_valid_expression_block_containing (#lv:pos) (sn:node lv{ has_kind_expression_block_containing sn })
  : Tot bool
  = (get_children_length sn >= 1) &&
    (for_all_children sn has_kind_block)

let is_valid_expression_blockless_simple (#lv:pos) (sn:node lv{ has_kind_expression_blockless_simple sn })
  : Tot bool
  = (is_leaf sn)

let is_valid_expression_blockless_complex (#lv:pos) (sn:node lv{ has_kind_expression_blockless_complex sn })
  : Tot bool
  = (is_leaf sn)

let is_valid_expression (#lv:pos) (sn:node lv{ has_kind_expression sn })
  : Tot bool
  = if (has_kind_expression_block_containing sn) then (is_valid_expression_block_containing sn)
    else if (has_kind_expression_blockless_simple sn) then (is_valid_expression_blockless_simple sn)
    else if (has_kind_expression_blockless_complex sn) then (is_valid_expression_blockless_complex sn)
    else true

let has_kind_expression_and_valid (#lv:pos) (sn:node lv)
  : Tot bool
  = (has_kind_expression sn) && (is_valid_expression sn)

let is_valid_argument (#lv:pos) (sn:node lv{ has_kind_argument sn })
  : Tot bool
  = (get_children_length sn = 1) &&
    (has_kind_expression_and_valid (get_child_at sn 0))

let has_kind_argument_and_valid (#lv:pos) (sn:node lv)
  : Tot bool
  = (has_kind_argument sn) && (is_valid_argument sn)

let is_valid_statement_normal (#lv:pos) (sn:node lv{ has_kind_statement_normal sn })
  : Tot bool
  = (for_all_children sn has_kind_argument_and_valid)

(*
let rec is_valid_argument_for_statement_substituted (#lv:pos) (sn:node lv{ has_kind_argument_and_valid sn })
  : Tot bool
  = 
*)

(*
let rec is_valid_expression_for_statement_substituted (#lv:pos) (sn:node lv{ has_kind_expression_and_valid sn })
  : Tot bool (decreases lv)
  = if (has_kind_expression_blockless_simple sn) then false
    else if (has_kind_expression_blockless_substituted sn) then false
    else if (has_kind_expression_block_containing sn) then (for_all_children sn is_valid_block_for_statement_substituted)

and is_valid_block_for_statement_substituted (#lv:pos) (sn:node lv{ has_kind_block sn })
  : Tot bool (decreases lv)
  = (for_all_children sn has_kind_statement) &&
    (for_all_children sn is_valid_statement_for_statement_substituted)

and is_valid_statement_for_statement_substituted (#lv:pos) (sn:node lv{ has_kind_statement sn })
  : Tot bool (decreases lv)
  = if (has_kind_statement_substituted sn) then 
*)


(*
let is_valid_statement_substituted (#lv:pos) (sn:node lv{ has_kind_statement_substituted sn })
  : Tot bool
  = (get_children_length sn = 1) &&
*)

(*
let is_kind_blockless_expression_normal (#lv:pos) (sn:node lv)
  : Tot bool
  = (is_kind_blockless_expression sn) && (Blockless_expression_normal? (get_blockless_expression_sum_kind sn))

let is_kind_blockless_expression_complex (#lv:pos) (sn:node lv)
  : Tot bool
  = (is_kind_blockless_expression sn) && (Blockless_expression_complex? (get_blockless_expression_sum_kind sn))

let is_kind_blockless_expression_substituted (#lv:pos) (sn:node lv)
  : Tot bool
  = (is_kind_blockless_expression sn) && (Blockless_expression_substituted? (get_blockless_expression_sum_kind sn))

let get_var_index (#lv:pos) (sn:node lv{is_kind_blockless_expression_substituted sn})
  : Tot nat
  = (Blockless_expression_substituted?.var_index (get_blockless_expression_sum_kind sn))

let is_kind_argument (#lv:pos) (sn:node lv)
  : Tot bool
  = (is_branch sn) && (Argument? (get_kind sn))

let get_argument_sum_kind (#lv:pos) (sn:node lv{ is_kind_argument sn })
  : Tot argument_sum_kind
  = let Argument v0 = (get_kind sn) in v0

let is_kind_argument_s1 (#lv:pos) (sn:node lv)
  : Tot bool
  = (is_kind_argument sn) && 
    (Argument_s1? (get_argument_sum_kind sn)) &&
    (get_children_length sn = 1) &&
    (is_kind_blockless_expression (get_child_at sn 0))

let is_kind_statement (#lv:pos) (sn:node lv)
  : Tot bool
  = (Statement? (get_kind sn)) &&
    (for_all sn.children is_kind_argument)

let is_kind_block (#lv:pos) (sn:node lv)
  : Tot bool
  = (Block? (get_kind sn)) &&
    (for_all sn.children is_kind_statement)

let is_kind_compilation_unit (#lv:pos) (sn:node lv)
  : Tot bool
  = (Compilation_unit? (get_kind sn)) &&
    (for_all sn.children is_kind_block)

let is_kind_argument_s0 (#lv:pos) (sn:node lv)
  : Tot bool
  = (is_kind_argument sn) && 
    (Argument_s0? (get_argument_sum_kind sn)) &&
    (get_children_length sn >= 1) &&
    (for_all sn.children is_kind_block)

private let lemma_test
  (#lv:pos) (sn:node lv{ is_kind_compilation_unit sn && get_children_length sn >= 3 })
  : Lemma (is_kind_block (get_child_at sn 2))
  = ()
*)

//let is_argument_ready_to_be_substituted