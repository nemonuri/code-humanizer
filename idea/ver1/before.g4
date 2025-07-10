grammar before;

compilation_unit
  : block*
  ;

block
  : statement*
  ;

statement
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
  ;

default_expression
  : default_expression_symbol
  ;

complex_expression
  : block*
  ;