parser grammar Verse;
options {tokenVocab=VerseLexer;}

verse_text: ( block | declaration | function_declaration ) * EOF;

declaration : ID ':' INTTYPE 
            | ID '=' (INT | expression)
            | ID ':=' (INT | expression) 
            ;

function_call        : ID '(' expression ')'
                     | ID '(' ')'
                     ;

function_declaration :  (ID '(' param')' ':' INTTYPE ':=' block)
                      | (ID ':=' '(' ID ':' INTTYPE '=>' block ')')
                      | (ID '(' ')' ':' INTTYPE ':=' block)
                      ;
                    

block : (expression ';' block *)
      | (declaration ';' block *)
      | (function_call block *)
      | (function_declaration ';' block *)
      | (if_rules block *)
      ;
    
if_rules : IF '(' expression ')' THEN (ID | INT ) ELSE ( ID | INT | function_call )
         ;

param : declaration
      | declaration ',' param
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
    | function_call
    | '(' expression ')'
    ;
    
operator : ('*'|'-'|'+' | '/' | '>' | '=');


