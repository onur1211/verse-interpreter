lexer grammar VerseLexer;

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
RANGE: '..';

// Types
INTTYPE : 'int';
STRINGTYPE: 'string';
COLLECTIONTYPE: ID'[]';
DATA : 'data';
VOID : 'void';
// Operators
PLUS : '+' ;
MINUS : '-' ;
MUL : '*' ;
DIV : '/' ;
LAMBDA : '=>' ;
CHOICE : '|' ;
NOVALUE : 'false?';

// Keywords
IF: 'if';
THEN: 'then';
ELSE: 'else';
FST: 'fst';
ARRAY: 'array';
INSTANCE: 'instance';
RETURN : 'return';

INDENT: '    ';
INT : [0-9]+ ;
SEARCH_TYPE : '"' (~["\\] | '\\' .)* '"';
ID: [a-zA-Z_][a-zA-Z_0-9]* ;
COMMENT : '#' ~[\r\n]* -> skip;
MULTILINECOMMENT : '#*' .*? '*#' -> skip;
WHITESPACE: [ \t\f]+ -> skip ;
NEWLINE: '\r'? '\n';