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

let is_children_empty (#t:eqtype) (bn:bound_node #t) : bool =
  (get_children_length bn) = 0

type syntax_node_kind =
  | No_kind
  | Compilation_unit
  | Block
  | Statement
  | Argument
  | Expression

type syntax_node_subkind =
  | No_subkind
  | Statement_forwarded_decl

let is_valid_kind_and_subkind_pair (k:syntax_node_kind) (sk:syntax_node_subkind)
  : bool
  = match k with
  | Statement -> sk = No_subkind || sk = Statement_forwarded_decl
  | _ -> sk = No_subkind

type original_syntax_node_data_schema = { kind:syntax_node_kind; subkind:syntax_node_subkind }
let original_syntax_node_data = o:original_syntax_node_data_schema{ is_valid_kind_and_subkind_pair o.kind o.subkind }

let context = list original_syntax_node_data

type syntax_node_builder =
  | No_build
  | Temporary_variable_identifier : identifier_number:nat -> syntax_node_builder
  | Local_variable_declaration : identifier_number:nat -> expression_index:nat -> syntax_node_builder

let get_data_from_builder (builder:syntax_node_builder) 
  : original_syntax_node_data 
  = match builder with
    | No_build -> { kind=No_kind; subkind=No_subkind }
    | Temporary_variable_identifier _ -> { kind=Expression; subkind=No_subkind }
    | Local_variable_declaration _ _ -> { kind=Statement; subkind=Statement_forwarded_decl }

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

let get_data_from_syntax_node_info (#c:context) (s:syntax_node_info #c)
  : original_syntax_node_data
  = match s with
  | From_ref_index i -> L.index c i
  | From_builder b -> get_data_from_builder b

let get_data_from_syntax_node (#c:context) (s:syntax_node #c)
  : original_syntax_node_data
  = get_data_from_syntax_node_info s.value

let is_block_syntax_node (#c:context) (s:syntax_node #c) : bool = 
  (get_data_from_syntax_node s).kind = Block &&
  L.for_all (fun (s1:syntax_node #c) -> (get_data_from_syntax_node s1).kind = Statement) s.children

let is_expression_syntax_node (#c:context) (s:syntax_node #c) : bool = 
  (get_data_from_syntax_node s).kind = Expression &&
  L.for_all (fun (s1:syntax_node #c) -> is_block_syntax_node s1) s.children

let expression_syntax_node (#c:context) = s:(syntax_node #c){ is_expression_syntax_node s }

let is_argument_syntax_node (#c:context) (s:syntax_node #c) : bool =
  (get_data_from_syntax_node s).kind = Argument &&
  get_children_length s = 1 &&
  is_expression_syntax_node (L.index s.children 0)
let argument_syntax_node (#c:context) = s:(syntax_node #c){ is_argument_syntax_node s }