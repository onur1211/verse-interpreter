﻿lexer grammar VerseLexer;

AND : 'and' ;
OR : 'or' ;
NOT : 'not' ;
ASSN : ':=';
DEF : ':' ;
EQ : '=' ;
COMMA : ',' ;
SEMI : ';' ;
LPAREN : '(' ;
RPAREN : ')' ;
LCURLY : '{' ;
RCURLY : '}' ;
LBRACK : '[' ;
RBRACK : ']' ;
GT : '>' ;
LT : '<' ;
QUOTE : '"';
DOT: '.';

// Types
INTTYPE : 'int';
STRINGTYPE: 'string';
DATA : 'data';
// Operators
PLUS : '+' ;
MINUS : '-' ;
MUL : '*' ;
DIV : '/' ;
LAMBDA : '=>' ;
CHOICE : '|' ;

// Keywords
IF: 'if';
THEN: 'then';
ELSE: 'else';
FST: 'fst';
ARRAY: 'array';

INDENT: '    ';
INT : [0-9]+ ;
ID: [a-zA-Z_][a-zA-Z_0-9]* ;
WHITESPACE: [ \t\f]+ -> skip ;
NEWLINE: '\r'? '\n';