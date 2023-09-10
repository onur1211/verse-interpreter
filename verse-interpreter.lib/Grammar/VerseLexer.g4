lexer grammar VerseLexer;

NOT : '!' ;
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
GTEQ : '<=';
LTEQ : '>=';
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
ARRAY: 'array';
// Operators
PLUS : '+' ;
MINUS : '-' ;
MUL : '*' ;
DIV : '/' ;
LAMBDA : '=>' ;
CHOICE : '|' ;
NOVALUE : 'false?';
QUESTIONMARK: '?';

// Keywords
IF: 'if';
FOR: 'for';
THEN: 'then';
ELSE: 'else';
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