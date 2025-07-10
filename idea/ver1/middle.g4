grammar middle;

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
  : expression
  ;

expression
  : complex_expression
  | default_expression
  | forwarded_identifier_symbol
  ;
