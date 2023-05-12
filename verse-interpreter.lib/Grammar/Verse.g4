parser grammar Verse;
options {tokenVocab=VerseLexer;}

verse_text: ( program ) * EOF;

declaration : ID ':' INTTYPE 
            | ID '=' (INT | expression)
            | ID ':=' (INT | expression) 
            ;

program : declaration program
        | function_definition program
        | function_call program
        | (NEWLINE | NEWLINE NEWLINE) program
        | program ';' program
        | declaration
        | function_call
        | function_definition
        ;

block : declaration
      | function_call
      | expression
      | declaration block
      | function_call block
      | expression block
      ;

// Functions

function_call : ID '(' param_call_item ')'
              | ID '(' ')'
              ;

function_definition : ID function_param ':' INTTYPE '=' function_body
                     ;

function_body : NEWLINE INDENT block function_body
              | block
              ;

function_param : '(' ')'
               | '(' param_def_item ')'
               ;
               
param_def_item     : declaration
                   | declaration ',' param_def_item
                   ;
                   
param_call_item : (INT | ID | function_call)
                | (INT | ID | function_call) ',' param_call_item
                ;

// Conditionals

if_block    : 'if' '(' comp_expression ')' 'then' 'else' ;

comp_expression
    : comp_term
    | comp_expression comparsion_op comp_term 
    ;

comp_term
    : comp_factor
    | comp_term comparsion_op factor
    ;

comp_factor
    : comp_primary
    | comparsion_op comp_factor
    ;

comp_primary
    : ID
    | INT
    | '(' comp_expression ')'
    ;


// Math expression rules
expression
    : term
    | expression operator term 
    ;

term
    : factor
    | term operator factor
    ;

factor
    : primary
    | operator factor
    ;

primary
    : ID
    | INT
    | '(' expression ')'
    ;

comparsion_op : ('>' | '<' | '|' )   ; 
operator : ('*'|'-'|'+' | '/' | '>' | '=' | '|');