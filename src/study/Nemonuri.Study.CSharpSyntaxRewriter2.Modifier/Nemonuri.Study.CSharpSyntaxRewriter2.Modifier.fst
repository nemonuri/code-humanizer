// For more information see http://www.fstar-lang.org/tutorial/
module Nemonuri.Study.CSharpSyntaxRewriter2.Modifier

(*
type bound_tree (#t_label:eqtype) (#t_value:eqtype) =
  | Leaf : label:t_label -> value:t_value -> bound_tree #t_label #t_value
  | Node : label:t_label -> nodes:list (bound_tree #t_label #t_value) -> bound_tree #t_label #t_value
*)

type bound_node (#t:eqtype) =
  { value:t; children:list (bound_node #t) }

type syntax_node_kind =
  | No_kind
  | Compilation_unit
  | Block
  | Statement
  | Argument
  | Expression

type syntax_node_builder =
  | No_build
  | Temporary_variable_identifier : identifier_number:nat -> syntax_node_builder
  | Local_variable_declaration : identifier_number:nat -> expression_index:nat -> syntax_node_builder

let get_kind_from_builder (builder:syntax_node_builder) 
  : syntax_node_kind 
  = match builder with
    | No_build -> No_kind
    | Temporary_variable_identifier _ -> Expression
    | Local_variable_declaration _ _ -> Statement

type syntax_node_info_schema = { kind:syntax_node_kind; builder:syntax_node_builder }

let is_valid_syntax_node_info (s:syntax_node_info_schema)
  : bool
  = if No_build? s.builder then 
      true
    else
      (get_kind_from_builder s.builder) = s.kind

type syntax_node_info = x:syntax_node_info_schema{ is_valid_syntax_node_info x }

let syntax_node = bound_node #syntax_node_info

open FStar.List.Tot

let get_children_length (#t:eqtype) (bn:bound_node #t) : Tot int =
  length bn.children

