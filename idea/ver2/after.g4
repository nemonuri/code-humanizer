module after;

compilation_unit
  : block*
  ;

block
  : statement*

// statement
//   : normal_statement
//   | local_var_decl_statement
//   | modified_argument_statement
//   ;

statement
  : argument*
  ;

argument
  : block*
  ;

argument
  : expression
  ;