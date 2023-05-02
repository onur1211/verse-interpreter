grammar Verse;
options {tokenVocab=VerseLexer;}

verse_text: ( block ) * EOF;

declaration : ID ':' INTTYPE 
            | ID '=' (INT | expression)
            | ID ':=' INT 
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


