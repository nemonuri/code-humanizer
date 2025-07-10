grammar after;

compilation_unit
  : block*
  ;

block
  : statement*
  ;

statement
  : default_statement
  | forwarded_local_declaration_statement
  ;

default_statement
  : argument_list*
  ;

argument_list
  : argument*
  ;

argument
  : argument_expression
  ;

expression
  : default_expression
  | forwarded_identifier_symbol
  ;

default_expression
  : default_expression_symbol
  ;

forwarded_local_declaration_statement
  : forwarded_variable_declarator
  ;

forwarded_variable_declarator
  : default_expression
  | forwarded_complex_expression
  ;

forwarded_complex_expression
  : forwarded_complex_expression_symbol
  ;