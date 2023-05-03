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

// Types
INTTYPE : 'int';

// Operators
PLUS : '+' ;
MINUS : '-' ;
MUL : '*' ;
DIV : '/' ;
LAMBDA : '=>' ;

// Keywords
IF: 'if';
THEN: 'then';
ELSE: 'else';


INT : [0-9]+ ;
ID: [a-zA-Z_][a-zA-Z_0-9]* ;
WS: [ \t\n\r\f]+ -> skip ;