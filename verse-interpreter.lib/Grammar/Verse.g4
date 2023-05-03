grammar Verse;
options {tokenVocab=VerseLexer;}

verse_text: ( block | declaration | function_declaration ) * EOF;

declaration : ID ':' INTTYPE 
            | ID '=' (INT | expression)
            | ID ':=' INT 
            ;

function_declaration : (ID '(' ID ':' INTTYPE ')' ':' INTTYPE ':=' block)
                      | (ID ':=' '(' ID ':' INTTYPE '=>' block ')')
                      ;

block : expression ';' 
      | declaration ';' 
      | block block
      ;
    

// Math expression rules
expression
    : term
    | expression operator term 
    ;

term
    : factor
    | term operator factor
    | term operator factor
    | term operator factor
    ;

factor
    : primary
    | operator factor
    | operator factor
    ;

primary
    : ID
    | INT
    | '(' expression ')'
    ;
    
operator : ('*'|'-'|'+' | '/' | '>');


