// For more information see http://www.fstar-lang.org/tutorial/
module Nemonuri.Study.CSharpSyntaxRewriter2.Modifier

(*
type bound_tree (#t_label:eqtype) (#t_value:eqtype) =
  | Leaf : label:t_label -> value:t_value -> bound_tree #t_label #t_value
  | Node : label:t_label -> nodes:list (bound_tree #t_label #t_value) -> bound_tree #t_label #t_value
*)

module L = FStar.List.Tot

type bound_node (#t:eqtype) =
  { value:t; children:list (bound_node #t) }

let get_children_length (#t:eqtype) (bn:bound_node #t) : Tot int =
  L.length bn.children


type syntax_node_kind =
  | No_kind
  | Compilation_unit
  | Block
  | Statement
  | Argument
  | Expression

type original_syntax_node_data = { kind:syntax_node_kind }

let context = list original_syntax_node_data

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

type syntax_node_info_schema =
  | From_ref_index : ref_index:nat -> syntax_node_info_schema
  | From_builder : builder:syntax_node_builder -> syntax_node_info_schema

let is_ref_index_in_context (c:context) (s:syntax_node_info_schema) 
  : bool
  = match s with
  | From_ref_index i -> i < (L.length c)
  | From_builder _ -> true

let is_valid_syntax_node_info (c:context) (s:syntax_node_info_schema)
  : bool
  = (is_ref_index_in_context c s)

let syntax_node_info (#c:context) = x:syntax_node_info_schema{ is_valid_syntax_node_info c x }

let syntax_node (#c:context) = bound_node #(syntax_node_info #c)

let get_kind_from_syntax_node_info (#c:context) (s:syntax_node_info #c)
  : syntax_node_kind
  = match s with
  | From_ref_index i -> (L.index c i).kind
  | From_builder b -> get_kind_from_builder b

let get_kind_from_syntax_node (#c:context) (s:syntax_node #c)
  : syntax_node_kind
  = get_kind_from_syntax_node_info s.value

